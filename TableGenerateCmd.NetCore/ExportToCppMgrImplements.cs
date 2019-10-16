using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
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

            using( var _stream = new MemoryStream(32767))
            {

                var _writer = new StreamWriter(_stream, new System.Text.ASCIIEncoding());
                {

                    string filename = System.IO.Path.GetFileName(createFileName);

                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace(".cpp", String.Empty);

                    //            writer.WriteLine("#include <Base/properties.h>");
                    //            writer.WriteLine("#include <Base/service.h>");

                    _writer.WriteLineEx($"#include \"{filename}.h\"");

                    _writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}");
                    _writer.WriteLineEx($"{{");
                    _writer.WriteLineEx($"namespace {filename.Replace(" ", "_").Replace("TableManager", String.Empty)}");
                    _writer.WriteLineEx($"{{");

                    max = sheets.GetLength(0);
                    current = 0;
                    foreach (string sheetName in sheets)
                    {
                        current++;
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        SheetProcess(filename, trimSheetName, columns,_writer);
                        //GetItemProcess(filename, trimSheetName, columns);
                        //AddItemMapProcess(filename, trimSheetName, imp.GetSheetShortCut(sheetName));
                        //AddArrayToMapProcess(filename, trimSheetName, columns);
                    }

                    //ManagerProcess(filename, sheets);
                    _writer.WriteLineEx($"bool TableManager::LoadTable(std::ifstream& stream)");
                    _writer.WriteLineEx($"{{");
                    _writer.WriteLineEx($"bool rtn = true;");
                    _writer.WriteLineEx($"std::vector<char> bytes;");
                    _writer.WriteLineEx($"if({ExportToCSMgr.NameSpace}::TableManager::Decompress(stream,bytes)==false) return false;");
                    _writer.WriteLineEx($"vectorwrapbuf<char> databuf(bytes);");
                    _writer.WriteLineEx($"std::istream is(&databuf);");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        _writer.WriteLineEx($"{trimSheetName}TableManager {trimSheetName}TableManager;");
                    }
                    _writer.WriteLineEx($"");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        _writer.WriteLineEx($"rtn &= {trimSheetName}TableManager.LoadTable(is);");
                    }
                    _writer.WriteLineEx($"return rtn;");
                    _writer.WriteLineEx($"}};");
                    _writer.WriteLineEx($"}};");
                    _writer.WriteLineEx($"}};");

                }
                ExportBaseUtil.CheckReplaceFile(_stream, $"{outputPath}/{createFileName}");
            }

            return true;
        }

        private void SheetProcess(string filename, string sheetName, List<Column> columns, StreamWriter _writer)
        {
            _writer.WriteLineEx($"class {sheetName}TableManager : public {ExportToCSMgr.NameSpace}::TableManager");
            _writer.WriteLineEx("{");
            _writer.WriteLineEx("public:");
            _writer.WriteLineEx($"bool {sheetName}TableManager::LoadTable(std::istream& stream__) override");
            _writer.WriteLineEx("{");
            InnerSheetProcess(filename, sheetName, columns,_writer);
            _writer.WriteLineEx("return true;");
            _writer.WriteLineEx("}");
            _writer.WriteLineEx("};");
        }

        private void GetItemProcess(string filename, string sheetName, List<Column> columns, StreamWriter _writer)
        {
            var key_column = columns.FirstOrDefault(compare => compare.is_key == true);
            string primaryType = key_column.GenerateType(_gen_type);

            _writer.WriteLineEx($"const {sheetName}Ptr {sheetName}::GetItem( const {primaryType}& id )");
            _writer.WriteLineEx($"{{");
            _writer.WriteLineEx($"  Map::const_iterator it = m_Map.find( id );");
            _writer.WriteLineEx($"  if( it != m_Map.end() )");
            _writer.WriteLineEx($"    return it->second;");
            _writer.WriteLineEx($"  return nullptr;");
            _writer.WriteLineEx($"}}");
        }

        private void AddArrayToMapProcess(string filename, string sheetName, List<Column> columns, StreamWriter _writer)
        {
            _writer.WriteLineEx("int " + sheetName + "::MapToArray()");
            _writer.WriteLineEx("{");
            _writer.WriteLineEx("  int rtn = TRUE;");
            _writer.WriteLineEx("  ");
            _writer.WriteLineEx("  Array& array = const_cast<Array&>(m_Array);");
            _writer.WriteLineEx("  std::for_each( m_Map.begin(), m_Map.end(), [&](const Map::value_type& itemtable)");
            _writer.WriteLineEx("  {");
            _writer.WriteLineEx("      array.push_back(itemtable.second);");
            _writer.WriteLineEx("  } );");
            _writer.WriteLineEx("  return rtn;");
            _writer.WriteLineEx("}");
        }

        private void InnerSheetProcess(string filename, string sheetName, List<Column> columns, StreamWriter _writer)
        {
            var key_column = columns.FirstOrDefault(compare => compare.is_key == true);
            string primaryName = "" + key_column.var_name;

            _writer.WriteLineEx($"int count__ = 0;");
            _writer.WriteLineEx($"Read(stream__,count__);");

            _writer.WriteLineEx($"if(count__ == 0) return true;");


            foreach (var column in columns)
            {
                if (column.is_generated == false)
                    continue;
                if (column.array_index > 0)
                    continue;
                if(column.array_index == -1)
                    _writer.WriteLineEx($"{column.GenerateType(this._gen_type)} {column.var_name};");
                if (column.array_index == 0)
                {
                    int array_count = columns.Where(compare => compare.var_name == column.var_name).Count();
                    _writer.WriteLineEx($"{column.GenerateType(this._gen_type)} {column.var_name}; {column.var_name}.resize({array_count});");
                }
            }
            _writer.WriteLineEx($"{sheetName}::Array array; array.resize(count__);");
            _writer.WriteLineEx($"{sheetName}::Map map;");
            _writer.WriteLineEx($"for(int i__=0;i__<count__;++i__)");
            _writer.WriteLineEx($"{{");
            foreach (var column in columns)
            {
                if (column.is_generated == false)
                    continue;
                if (column.array_index > 0)
                    continue;
                if (column.array_index == 0)
                {
                    int array_count = columns.Where(compare => compare.var_name == column.var_name).Count();
                    for(int i=0;i<array_count;i++)
                    {
                        if(column.base_type == eBaseType.Boolean)
                            _writer.WriteLineEx($"  {{ {column.base_type.GenerateBaseType(this._gen_type)} element__; Read(stream__, element__); {column.var_name}[{i}] = element__; }}");
                        else if (column.base_type == eBaseType.TimeSpan)
                            _writer.WriteLineEx($"  {{ long long element__; Read(stream__, element__); {column.var_name}[{i}] = ({column.base_type.GenerateBaseType(this._gen_type)}) element__ / 10000000; }}");
                        else if (column.base_type == eBaseType.DateTime)
                            _writer.WriteLineEx($"  {{ long long element__; Read(stream__, element__); {column.var_name}[{i}] = ({column.base_type.GenerateBaseType(this._gen_type)}) (element__ - 621355968000000000) / 10000000; }}");
                        else
                            _writer.WriteLineEx($"Read(stream__, {column.var_name}[{i}]);");
                    }
                }
                else
                {
                    if(column.base_type == eBaseType.TimeSpan)
                        _writer.WriteLineEx($"  {{long long element__; Read(stream__, element__); {column.var_name} = ({column.GenerateType(this._gen_type)}) element__ / 10000000; }}");
                    else if (column.base_type == eBaseType.DateTime)
                        _writer.WriteLineEx($"  {{long long element__; Read(stream__, element__); {column.var_name} = ({column.GenerateType(this._gen_type)}) (element__ - 621355968000000000) / 10000000; }}");
                    else
                        _writer.WriteLineEx($"Read(stream__, {column.var_name});");
                }
            }
            _writer.WriteLineEx(string.Format($"{sheetName}Ptr item__ = {sheetName}Ptr(new {sheetName}({{0}}));",
                string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.var_name}").ToArray()))
            );
            _writer.WriteLineEx("array[i__] = item__;");
            _writer.WriteLineEx($"map.insert( std::pair<{key_column.GenerateType(this._gen_type)},{sheetName}Ptr>({primaryName},item__));");
            _writer.WriteLineEx($"}}");
            _writer.WriteLineEx($"{{");
            _writer.WriteLineEx($"{sheetName}::Array& target = const_cast<{sheetName}::Array&>({sheetName}::array);");
            _writer.WriteLineEx($"target.clear();");
            _writer.WriteLineEx($"target.resize(count__);");
            _writer.WriteLineEx($"std::copy(array.begin(),array.end(),target.begin());");
            _writer.WriteLineEx($"}}");
            _writer.WriteLineEx($"{{");
            _writer.WriteLineEx($"{sheetName}::Map& target = const_cast<{sheetName}::Map&>({sheetName}::map);");
            _writer.WriteLineEx($"target.clear();");
            _writer.WriteLineEx($"target.insert(map.begin(),map.end());");
            _writer.WriteLineEx($"}}");
        }

    }
}
