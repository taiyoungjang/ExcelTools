using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;

namespace TableGenerate
{
    public class ExportToCPPHeader : ExportBase
    {
        protected string iceFileDir;

        private string _async = string.Empty;
        public string SetAsync
        {
            //set { _async = "unity3d"; }
            set { _async = value; }
        }

        public static String NameSpace = string.Empty;
        public int m_current = 0;
        public eGenType _gen_type = eGenType.cpp;

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            try
            {
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".h");

                using (MemoryStream stream = new MemoryStream())
                {
                    var _writer = new IndentedTextWriter(new StreamWriter(stream, new System.Text.ASCIIEncoding()), "  ");
                    {
                        string filename = System.IO.Path.GetFileName(createFileName);

                        _writer.WriteLineEx($"// generate {filename}");
                        _writer.WriteLineEx("// DO NOT TOUCH SOURCE....");
                        _writer.WriteLineEx($"#ifndef {filename.Replace(".","_").ToUpper()}");
                        _writer.WriteLineEx($"#define {filename.Replace(".", "_").ToUpper()}");
                        _writer.WriteLineEx($"#include <memory>");
                        _writer.WriteLineEx($"#include <string>");
                        _writer.WriteLineEx($"#include <vector>");
                        _writer.WriteLineEx($"#include <map>");

                        string[] sheets = imp.GetSheetList();

                        filename = filename.Replace(".h", string.Empty);

                        max = sheets.GetLength(0);
                        current = 0;

                        _writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}");
                        _writer.WriteLineEx("{");
                        _writer.WriteLineEx($"namespace {filename}");
                        _writer.WriteLineEx("{");
                        foreach (string sheetName in sheets)
                        {
                            current++;
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            SheetProcess(_writer, filename, trimSheetName, columns);
                        }
                        _writer.WriteLineEx("};");
                        _writer.WriteLineEx("};");
                        _writer.WriteLineEx($"#endif //{filename.Replace(".", "_").ToUpper()}");
                        _writer.Flush();
                    }
                    ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}");
                }
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
            return true;
        }

        private void SheetProcess(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            _writer.WriteLineEx($"class {sheetName};");
            _writer.WriteLineEx($"typedef std::shared_ptr<{sheetName}> {sheetName}Ptr;");
            _writer.WriteLineEx($"class {sheetName}");
            _writer.WriteLineEx("{");
            var key_column = columns.FirstOrDefault(compare => compare.is_key == true);
            string keyType = key_column.GenerateType(_gen_type);

            _writer.WriteLineEx("public:");
            _writer.WriteLine($"static {sheetName}Ptr Find( const {keyType}& {key_column.var_name});");
            _writer.WriteLine($"typedef std::vector<{sheetName}Ptr> Array;");
            _writer.WriteLine($"typedef std::map<{keyType},{sheetName}Ptr> Map;");
            _writer.WriteLine("static const Array array;");
            _writer.WriteLine("static const Map map;");
            _writer.WriteLine("");

            InnerSheetProcess(_writer, columns);
            SheetConstructorProcess(_writer, sheetName, columns);
            _writer.WriteLineEx("};");
        }

        private void InnerSheetProcess(IndentedTextWriter _writer, List<Column> columns)
        {
            foreach (var column in columns)
            {
                string name = column.var_name;
                string type = column.GenerateType(_gen_type);
                if (column.is_generated == false)
                {
                    continue;
                }
                if (column.array_index > 0)
                {
                    continue;
                }
                _writer.WriteLineEx($"const {type} {name};");
            }
        }
        private void SheetConstructorProcess(IndentedTextWriter _writer, string sheetName, List<Column> columns)
        {
            _writer.WriteLineEx(string.Format("{0} ({1});",
                sheetName,
                string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"const {t.GenerateType(_gen_type)}& {t.var_name}__").ToArray()))
            );
        }
    }
}
