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

            using MemoryStream stream = new(32767);
            {
                StreamWriter writer = new (stream, new System.Text.ASCIIEncoding());
                {
                    string filename = System.IO.Path.GetFileName(createFileName);

                    writer.WriteLineEx($"// generated {filename}");
                    writer.WriteLineEx("// DO NOT TOUCH SOURCE....");

                    writer.WriteLineEx($"#pragma once");

                    writer.WriteLineEx(string.Empty);

                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace("TableManager.h", string.Empty);

                    writer.WriteLineEx($"#include \"{filename}.h\"");
                    writer.WriteLineEx($"#include \"TableManager.h\"");
                    max = sheets.GetLength(0);
                    current = 0;

                    writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}::{filename.Replace(" ", "_").Replace("TableManager", string.Empty)}");
                    writer.WriteLineEx($"{{");

                    //foreach (string sheetName in sheets)
                    //{
                    //    current++;
                    //    string trimSheetName = sheetName.Trim().Replace(" ", "_");
                    //    var rows = imp.GetSheetShortCut(sheetName, language);
                    //    var columns = ExportBaseUtil.GetColumnInfo(trimSheetName, rows, except);
                    //    SheetProcess(filename, trimSheetName, columns);
                    //}
                    writer.WriteLineEx($"class TableManager");
                    writer.WriteLineEx($"{{");
                    writer.WriteLineEx("public:");
                    writer.WriteLineEx($"bool LoadTable(BufferReader& stream);");
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
            _writer.WriteLineEx(string.Empty);
            _writer.WriteLineEx($"class {sheetName}TableManager : public {ExportToCSMgr.NameSpace}::TableManager");
            _writer.WriteLineEx("{");
            InnerSheetProcess(sheetName, columns, _writer);
            _writer.WriteLineEx("};");
        }

        private void InnerSheetProcess(string sheetName, List<Column> columns, StreamWriter _writer)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key == true);
            string keyType = keyColumn.GenerateType(_gen_type); 
            _writer.WriteLineEx("public:");
            _writer.WriteLineEx("bool LoadTable(BufferReader& stream) override;");
        }
    }
}
