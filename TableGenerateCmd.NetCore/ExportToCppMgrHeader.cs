using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;

namespace TableGenerate
{
    public class ExportToCppMgrHeader : ExportBase
    {
        //public StreamWriter _writer = null;
        public eGenType _gen_type = eGenType.cpp;

        public override bool Generate(System.Reflection.Assembly refAssem, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", "TableManager.h");

            using( var _stream = new MemoryStream(32767))
            {
                var _writer = new StreamWriter(_stream, new System.Text.ASCIIEncoding());
                {
                    string filename = System.IO.Path.GetFileName(createFileName);

                    string defineName = filename.Replace(".h", "_H");
                    defineName = defineName.ToUpper();
                    _writer.WriteLineEx($"// generate {filename}");
                    _writer.WriteLineEx("// DO NOT TOUCH SOURCE....");

                    _writer.WriteLineEx($"#ifndef {defineName}");
                    _writer.WriteLineEx($"#define {defineName}");

                    _writer.WriteLineEx(string.Empty);

                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace("TableManager.h", string.Empty);

                    _writer.WriteLineEx($"#include \"{filename}.h\"");
                    _writer.WriteLineEx($"#include <TableManager.h>");
                    max = sheets.GetLength(0);
                    current = 0;

                    _writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}");
                    _writer.WriteLineEx($"{{");
                    _writer.WriteLineEx($"namespace {filename.Replace(" ", "_").Replace("TableManager", string.Empty)}");
                    _writer.WriteLineEx($"{{");

                    //foreach (string sheetName in sheets)
                    //{
                    //    current++;
                    //    string trimSheetName = sheetName.Trim().Replace(" ", "_");
                    //    var rows = imp.GetSheetShortCut(sheetName, language);
                    //    var columns = ExportBaseUtil.GetColumnInfo(trimSheetName, rows, except);
                    //    SheetProcess(filename, trimSheetName, columns);
                    //}
                    _writer.WriteLineEx($"class TableManager");
                    _writer.WriteLineEx($"{{");
                    _writer.WriteLineEx("public:");
                    _writer.WriteLineEx($"static bool LoadTable(std::ifstream& stream);");
                    _writer.WriteLineEx($"}};");
                    _writer.WriteLineEx($"}};");
                    _writer.WriteLineEx($"}};");
                    _writer.WriteLineEx($"#endif //{defineName}");
                    _writer.Flush();
                }
                ExportBaseUtil.CheckReplaceFile(_stream, $"{outputPath}/{createFileName}");
            }
            return true;
        }

        private void SheetProcess(string filename, string sheetName, List<Column> columns, StreamWriter _writer)
        {
            _writer.WriteLineEx(string.Empty);
            _writer.WriteLineEx($"class {sheetName}TableManager : public {ExportToCSMgr.NameSpace}::TableManager");
            _writer.WriteLineEx("{");
            InnerSheetProcess(sheetName, columns, _writer);
            _writer.WriteLineEx("};");
        }

        private void InnerSheetProcess(string sheetName, List<Column> columns, StreamWriter _writer)
        {
            var key_column = columns.FirstOrDefault(compare => compare.is_key == true);
            string keyType = key_column.GenerateType(_gen_type); 
            _writer.WriteLineEx("public:");
            _writer.WriteLineEx("bool LoadTable(std::istream& stream) override;");
        }
    }
}
