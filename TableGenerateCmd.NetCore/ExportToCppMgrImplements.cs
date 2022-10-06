using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StreamWrite.Extension;

namespace TableGenerate
{
    public class ExportToCppMgrImplements : ExportBase
    {
        //public StreamWriter _writer = null;
        public eGenType _gen_type = eGenType.cpp;

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", "TableManager.cpp");

            using MemoryStream stream = new();
            {
                StreamWriter writer = new (stream, Encoding.UTF8);
                {

                    string filename = System.IO.Path.GetFileName(createFileName);

                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace(".cpp", string.Empty);

                    //            writer.WriteLine("#include <Base/properties.h>");
                    //            writer.WriteLine("#include <Base/service.h>");

                    writer.WriteLineEx($"#include \"{filename}.h\"");

                    writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}::{filename.Replace(" ", "_").Replace("TableManager", string.Empty)}");
                    writer.WriteLineEx($"{{");

                    max = sheets.GetLength(0);
                    current = 0;
                    foreach (string sheetName in sheets)
                    {
                        current++;
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        SheetProcess(filename, trimSheetName, columns,writer);
                        //GetItemProcess(filename, trimSheetName, columns);
                        //AddItemMapProcess(filename, trimSheetName, imp.GetSheetShortCut(sheetName));
                        //AddArrayToMapProcess(filename, trimSheetName, columns);
                    }

                    //ManagerProcess(filename, sheets);
                    writer.WriteLineEx($"bool TableManager::LoadTable(BufferReader& stream)");
                    writer.WriteLineEx($"{{");
                    writer.WriteLineEx($"bool rtn = true;");
                    writer.WriteLineEx($"TArray<uint8> bytes;");
                    writer.WriteLineEx($"if({ExportToCSMgr.NameSpace}::BufferReader::Decompress(stream,bytes)==false) return false;");
                    writer.WriteLineEx($"BufferReader bufferReader((uint8*)bytes.GetData(),(int32)bytes.Num());");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        writer.WriteLineEx($"{trimSheetName}TableManager {trimSheetName}TableManager;");
                    }
                    writer.WriteLineEx($"");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        writer.WriteLineEx($"rtn &= {trimSheetName}TableManager.LoadTable(bufferReader);");
                    }
                    writer.WriteLineEx($"return rtn;");
                    writer.WriteLineEx($"}};");
                    writer.WriteLineEx($"}}");
                    writer.Flush();
                }
                ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}");
            }

            return true;
        }

        private void SheetProcess(string filename, string sheetName, List<Column> columns, StreamWriter _writer)
        {
            _writer.WriteLineEx($"class {sheetName}TableManager : public {ExportToCSMgr.NameSpace}::TableManager");
            _writer.WriteLineEx("{");
            _writer.WriteLineEx("public:");
            _writer.WriteLineEx($"  {sheetName}TableManager(void){{}}");
            _writer.WriteLineEx($"bool LoadTable(BufferReader& stream__) override");
            _writer.WriteLineEx("{");
            InnerSheetProcess(sheetName, columns,_writer);
            _writer.WriteLineEx("return true;");
            _writer.WriteLineEx("}");
            _writer.WriteLineEx("};");
        }

        private void GetItemProcess(string sheetName, List<Column> columns, StreamWriter writer)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key == true);
            string primaryType = keyColumn.GenerateType(_gen_type);

            writer.WriteLineEx($"const {sheetName}Ptr {sheetName}::GetItem( const {primaryType}& id )");
            writer.WriteLineEx($"{{");
            writer.WriteLineEx($"  Map::const_iterator it = m_Map.find( id );");
            writer.WriteLineEx($"  if( it != m_Map.end() )");
            writer.WriteLineEx($"    return it->second;");
            writer.WriteLineEx($"  return nullptr;");
            writer.WriteLineEx($"}}");
        }
        
        private void InnerSheetProcess(string sheetName, List<Column> columns, StreamWriter writer)
        {
            sheetName = 'F' + sheetName;
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key);
            string primaryName = "" + keyColumn.var_name;

            writer.WriteLineEx($"int32 count__ = 0;");
            writer.WriteLineEx($"stream__ >> count__;");

            writer.WriteLineEx($"if(count__ == 0) return true;");


            foreach (var column in columns.Where(column => column.is_generated != false).Where(column => column.array_index <= 0))
            {
                if(column.array_index == -1)
                    writer.WriteLineEx($"{column.GenerateType(this._gen_type)} {column.var_name};");
                if (column.array_index == 0)
                {
                    writer.WriteLineEx($"{column.GenerateType(this._gen_type)} {column.var_name};");
                }
            }
            writer.WriteLineEx($"{sheetName}::Array array; array.SetNum(count__,true);");
            writer.WriteLineEx($"{sheetName}::Map map;");
            writer.WriteLineEx($"for(int i__=0;i__<count__;++i__)");
            writer.WriteLineEx($"{{");
            foreach (var column in columns)
            {
                if (column.is_generated == false)
                    continue;
                if (column.array_index > 0)
                    continue;
                if (column.array_index == 0)
                {
                    writer.WriteLineEx("{");
                    writer.WriteLineEx($"int arrayCount__ = BufferReader::Read7BitEncodedInt(stream__);");
                    writer.WriteLineEx($"{column.var_name}.SetNum(arrayCount__,true);");
                    writer.WriteLineEx($"for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)");
                    writer.WriteLineEx("{");
                    switch (column.base_type)
                    {
                        case eBaseType.Boolean:
                            writer.WriteLineEx($"  {{ {column.base_type.GenerateBaseType(this._gen_type)} element__; stream__ >> element__; {column.var_name}[arrayIndex__] = element__; }}");
                            break;
                        case eBaseType.TimeSpan:
                            writer.WriteLineEx($"  {{ int64 element__; stream__ >> element__; {column.var_name}[arrayIndex__] = ({column.base_type.GenerateBaseType(this._gen_type)}) element__ / 10000000; }}");
                            break;
                        case eBaseType.DateTime:
                            writer.WriteLineEx($"  {{ int64 element__; stream__ >> element__; {column.var_name}[arrayIndex__] = ({column.base_type.GenerateBaseType(this._gen_type)}) (element__ - 621355968000000000) / 10000000; }}");
                            break;
                        default:
                            writer.WriteLineEx($"stream__ >> {column.var_name}[arrayIndex__];");
                            break;
                    }
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("}");
                }
                else
                {
                    switch (column.base_type)
                    {
                        case eBaseType.TimeSpan:
                            writer.WriteLineEx($"  {{int64 element__; stream__ >> element__; {column.var_name} = ({column.GenerateType(this._gen_type)}) element__ / 10000000; }}");
                            break;
                        case eBaseType.DateTime:
                            writer.WriteLineEx($"  {{int64 element__; stream__ >> element__; {column.var_name} = ({column.GenerateType(this._gen_type)}) (element__ - 621355968000000000) / 10000000; }}");
                            break;
                        default:
                            writer.WriteLineEx($"stream__ >> {column.var_name};");
                            break;
                    }
                }
            }
            writer.WriteLineEx(string.Format($"auto item__ = {sheetName}({{0}});",
                string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.var_name}").ToArray()))
            );
            writer.WriteLineEx("array[i__] = item__;");
            writer.WriteLineEx($"map.Emplace({primaryName},item__);");
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"{{");
            writer.WriteLineEx($"{sheetName}::Array& target = const_cast<{sheetName}::Array&>({sheetName}::array);");
            writer.WriteLineEx($"target.Reset();");
            writer.WriteLineEx($"target.SetNum(count__,true);");
            writer.WriteLineEx($"target.Append(array);");
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"{{");
            writer.WriteLineEx($"{sheetName}::Map& target = const_cast<{sheetName}::Map&>({sheetName}::map);");
            writer.WriteLineEx($"target.Reset();");
            writer.WriteLineEx($"target.Append(map);");
            writer.WriteLineEx($"}}");
        }

    }
}
