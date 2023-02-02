using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;
using DocumentFormat.OpenXml.Spreadsheet;

namespace TableGenerate
{
    public class ExportToCPPHeader : ExportBase
    {
        private string CPPClassPredefine;
        public ExportToCPPHeader(string cppClassPredefine)
        {
            CPPClassPredefine = cppClassPredefine;
        }
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
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", "TableRow.h");

                using var stream = new MemoryStream();
                {
                    var writer = new IndentedTextWriter(new StreamWriter(stream, Encoding.UTF8), "  ");
                    {
                        string filename = System.IO.Path.GetFileName(createFileName);
                        string[] sheets = imp.GetSheetList();
                        filename = filename.Replace(".h", string.Empty);

                        Dictionary<string, List<Column>> sheetsColumns = new ();
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            sheetsColumns.Add(trimSheetName,columns);
                        }

                        var enums = sheetsColumns.Values.SelectMany(t => t).Where(t => t.IsEnumType()).Select(t => (t,t.TypeInfo)).Distinct();
                        
                        writer.WriteLineEx($"// generate {filename}");
                        
                        writer.WriteLineEx("// DO NOT TOUCH SOURCE....");
                        writer.WriteLineEx($"#pragma once");
                        writer.WriteLineEx($"#include \"TableRowExtension.h\"");
                        foreach (var (_,typeInfo) in enums)
                        {
                            writer.WriteLineEx($"#include \"{typeInfo.Name}.h\"");
                        }
                        writer.WriteLineEx($"#include \"{filename}.generated.h\"");

                        max = sheets.GetLength(0);
                        current = 0;

                        //writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}::{filename}");
                        //writer.WriteLineEx("{");
                        foreach (var sheet in sheetsColumns)
                        {
                            current++;
                            SheetProcess(writer, $"F{sheet.Key}", sheet.Value);
                        }
                        //writer.WriteLineEx("}");
                        writer.Flush();
                    }
                    ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
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
            writer.WriteLineEx($"USTRUCT(BlueprintType)");
            writer.WriteLineEx($"struct {CPPClassPredefine} {sheetName}TableRow : public FTableRowExtension");
            writer.WriteLineEx("{");
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key == true);
            string keyType = keyColumn.GenerateType(_gen_type);

            writer.WriteLineEx($"GENERATED_USTRUCT_BODY()");

            InnerSheetProcess(writer, sheetName, columns);
            writer.WriteLineEx("};");
        }

        private void InnerSheetProcess(IndentedTextWriter writer, string sheetName, List<Column> columns)
        {
            var sn = sheetName.Remove(0, 1);
            foreach (var column in columns)
            {
                string name = column.var_name;
                string type = column.GenerateType(_gen_type);
                if (column.is_generated == false)
                {
                    continue;
                }
                if (column.array_index > 0 || column.is_key)
                {
                    continue;
                }
                writer.WriteLineEx($"UPROPERTY( EditAnywhere{ (column.bit_flags? $", Meta = (BitMask, BitmaskEnum = \"{column.str_bit_flags}E{column.type_name}\" )": $", BlueprintReadWrite, Category = {sn}")} )");
                writer.WriteLineNoTabs($"{string.Empty.PadLeft(writer.Indent*2)}{type} {name};{(column.desc.Any()?$" /// {column.desc}":string.Empty)}");
            }
        }
    }
}
