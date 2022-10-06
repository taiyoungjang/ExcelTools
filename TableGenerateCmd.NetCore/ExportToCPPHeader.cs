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

                using var stream = new MemoryStream();
                {
                    var writer = new IndentedTextWriter(new StreamWriter(stream, Encoding.UTF8), "  ");
                    {
                        string filename = System.IO.Path.GetFileName(createFileName);
                        string[] sheets = imp.GetSheetList();
                        filename = filename.Replace(".h", string.Empty);

                        writer.WriteLineEx($"// generate {filename}");
                        
                        writer.WriteLineEx("// DO NOT TOUCH SOURCE....");
                        writer.WriteLineEx($"#pragma once");
                        writer.WriteLineEx($"#include \"CoreMinimal.h\"");
                        writer.WriteLineEx($"//#include \"{filename}.generated.h\"");

                        max = sheets.GetLength(0);
                        current = 0;

                        writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}::{filename}");
                        writer.WriteLineEx("{");
                        foreach (string sheetName in sheets)
                        {
                            current++;
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            SheetProcess(writer, trimSheetName, columns);
                        }
                        writer.WriteLineEx("}");
                        writer.Flush();
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

        private void SheetProcess(IndentedTextWriter writer, string sheetName, List<Column> columns)
        {
            sheetName = 'F' + sheetName;
            writer.WriteLineEx($"//USTRUCT(BlueprintType)");
            writer.WriteLineEx($"struct {sheetName}");
            writer.WriteLineEx("{");
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key == true);
            string keyType = keyColumn.GenerateType(_gen_type);

            writer.WriteLine($"//static {sheetName} Find( const {keyType}& {keyColumn.var_name});");
            writer.WriteLine($"typedef TArray<{sheetName}> Array;");
            writer.WriteLine($"typedef TMap<{keyType},{sheetName}> Map;");
            writer.WriteLine("static const Array array;");
            writer.WriteLine("static const Map map;");
            writer.WriteLineEx($"//GENERATED_BODY()");
            writer.WriteLine("");

            InnerSheetProcess(writer, columns);
            SheetConstructorProcess(writer, sheetName, columns);
            writer.WriteLineEx("};");
        }

        private void InnerSheetProcess(IndentedTextWriter writer, List<Column> columns)
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
                writer.WriteLineEx($"    const {type} {name} {{}};{(column.desc.Any()?$"  ///< {column.desc}":string.Empty)}");
            }
        }
        private void SheetConstructorProcess(IndentedTextWriter writer, string sheetName, List<Column> columns)
        {
            writer.WriteLineEx($"{sheetName}(void);");
            writer.WriteLineEx($"{sheetName}& operator=(const {sheetName}& rhs);");
            writer.WriteLineEx(string.Format("{0} ({1});",
                sheetName,
                string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"const {t.GenerateType(_gen_type)}& {t.var_name}__").ToArray()))
            );
        }
    }
}
