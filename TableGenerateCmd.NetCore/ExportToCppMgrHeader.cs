using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using TableGenerateCmd;

namespace TableGenerate
{
    public class ExportToCppMgrHeader : ExportBase
    {
        private string CPPClassPredefine;
        public ExportToCppMgrHeader(string cppClassPredefine)
        {
            CPPClassPredefine = cppClassPredefine;
        }
        //public StreamWriter _writer = null;
        public eGenType _gen_type = eGenType.cpp;

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", "TableManager.h");

            using var stream = new MemoryStream();
            {
                using var writer = new IndentedTextWriter(new StreamWriter(stream,  Encoding.UTF8), " ");
                {
                    string filename = System.IO.Path.GetFileName(createFileName);
                    filename = filename.Replace("TableManager.h", string.Empty);

                    writer.WriteLineEx($"// generated {filename}");
                    writer.WriteLineEx("// DO NOT TOUCH SOURCE....");

                    writer.WriteLineEx($"#pragma once");
                    writer.WriteLineEx($"#ifdef WITH_EDITOR");
                    writer.WriteLineEx($"#include \"TableManager.h\"");
                    writer.WriteLineEx($"#include \"TableManager.h\"");
                    
                    writer.WriteLineEx(string.Empty);

                    string[] sheets = imp.GetSheetList();

                    max = sheets.GetLength(0);
                    current = 0;
                    
                    Dictionary<string, List<Column>> sheetsColumns = new ();
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        sheetsColumns.Add(trimSheetName,columns);
                    }
                    current++;

                    writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}::{filename.Replace(" ", "_").Replace("TableManager", string.Empty)}");
                    writer.WriteLineEx($"{{");

                    writer.WriteLineEx($"class FTableManager final : public ITableManager");
                    writer.WriteLineEx($"{{");
                    writer.WriteLineEx("public:");
                    writer.WriteLineEx("static FTableManager* Register;");
                    writer.WriteLineEx($"virtual bool ConvertToUAsset(FBufferReader& Reader, const FString& Language) override;");
                    writer.WriteLineEx($"virtual FString GetTableName() override");
                    writer.WriteLineEx( "{");
                    writer.WriteLineEx($"return TEXT(\"{filename}\");");
                    writer.WriteLineEx( "}");
                    writer.WriteLineEx($"}};");
                    writer.WriteLineEx($"}}");
                    writer.WriteLineEx($"#endif");
                    writer.Flush();
                }
                ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
            }
            return true;
        }
    }
}
