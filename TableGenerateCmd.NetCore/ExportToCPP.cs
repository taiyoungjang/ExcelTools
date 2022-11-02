using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;
using System.Text;

namespace TableGenerate
{
    public class ExportToCPP : ExportBase
    {
        protected string iceFileDir;

        private string _async = string.Empty;
        public string SetAsync
        {
            //set { _async = "unity3d"; }
            set { _async = value; }
        }

        public static string s_nameSpace = string.Empty;
        public int m_current = 0;
        public eGenType _gen_type = eGenType.cpp;

        public override bool Generate(System.Reflection.Assembly[] refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            try
            {
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".cpp");
                string filename = System.IO.Path.GetFileName(createFileName);
                string @namespace = $"{ ExportToCSMgr.NameSpace }::{filename.Replace(".cpp", string.Empty)}";
                using MemoryStream stream = new ();
                var writer = new IndentedTextWriter(new StreamWriter(stream, Encoding.UTF8), "  ");
                {

                    writer.WriteLineEx($"// generate {filename}");
                    writer.WriteLineEx("// DO NOT TOUCH SOURCE....");
                    writer.WriteLineEx($"#include \"{filename.Replace(".cpp", ".h")}\"");
                    //writer.WriteLineEx($"using namespace {@namespace};");

                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace(".cs", string.Empty);

                    max = sheets.GetLength(0);
                    current = 0;

                    foreach (string sheetName in sheets)
                    {
                        current++;
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        SheetProcess(writer, $"F{filename.Replace(".cpp","")}_{trimSheetName}", columns);
                    }
                    writer.Flush();
                }
                ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
            return true;
        }

        private void SheetProcess(IndentedTextWriter writer, string sheetName, List<Column> columns)
        {
            SheetConstructorProcess(writer, sheetName, columns);
            SheetFindFunction(writer, sheetName, columns);
        }

        private void SheetConstructorProcess(IndentedTextWriter writer, string sheetName, IReadOnlyCollection<Column> columns)
        {
            writer.WriteLineEx($"const {sheetName}::FArray {sheetName}::Array_;");
            writer.WriteLineEx($"const {sheetName}::FMap {sheetName}::Map_;");
            writer.WriteLineEx($"{sheetName}::{sheetName}(void)");
            /*
            bool isFirst =true;
            foreach (var column in columns.Where(t => t.is_generated == true && t.array_index <= 0))
            {
                if (!isFirst)
                {
                    writer.Write(',');
                }
                string genDefaultValue = column.GenerateDefaultValue(_gen_type);
                writer.WriteLineEx($"{column.var_name}({genDefaultValue})");
                isFirst = true;
            }
            */
            writer.WriteLineEx("{");
            writer.WriteLineEx("}");
            writer.WriteLineEx($"{sheetName}& {sheetName}::operator=(const {sheetName}& RHS)");
            writer.WriteLineEx("{");
            foreach(var column in columns.Where(t => t.is_generated == true && t.array_index <= 0))
            {
                string genType = column.GenerateType(_gen_type);
                writer.WriteLineEx($"const_cast<{genType}&>({column.var_name})=RHS.{column.var_name};");
            }
            writer.WriteLineEx("return *this;");
            writer.WriteLineEx("}");
            writer.WriteLineEx(string.Format("{0} ({1})\n:{2}",
                $"{sheetName}::{sheetName}",
                string.Join("\n,", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"const {t.GenerateType(_gen_type)}& {t.var_name}").ToArray()),
                string.Join("\n,", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.var_name}({t.var_name})").ToArray()))
            );
            writer.WriteLineEx($"{{");
            writer.WriteLineEx($"}}");
        }
        private void SheetFindFunction(IndentedTextWriter writer, string sheetName, IEnumerable<Column> columns)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key);
            string keyType = keyColumn.GenerateType(_gen_type);

            writer.WriteLineEx($"/*{sheetName} {sheetName}::Find(const {keyType}& {keyColumn.var_name})");
            writer.WriteLineEx($"{{");
            writer.WriteLineEx($"return {sheetName}Ptr(map.Find({keyColumn.var_name}));");
            writer.WriteLineEx($"}}*/");
        }
    }
}
