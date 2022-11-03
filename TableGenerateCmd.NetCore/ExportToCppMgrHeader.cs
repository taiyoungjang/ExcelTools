﻿using System;
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

        public override bool Generate(System.Reflection.Assembly[] refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", "TableManager.h");

            using MemoryStream stream = new(32767);
            {
                StreamWriter writer = new (stream, Encoding.UTF8);
                {
                    string filename = System.IO.Path.GetFileName(createFileName);
                    filename = filename.Replace("TableManager.h", string.Empty);

                    writer.WriteLineEx($"// generated {filename}");
                    writer.WriteLineEx("// DO NOT TOUCH SOURCE....");

                    writer.WriteLineEx($"#pragma once");

                    writer.WriteLineEx($"#include \"Engine/DataTable.h\"");
                    writer.WriteLineEx($"#include \"{filename}.h\"");
                    writer.WriteLineEx($"#include \"TableManager.h\"");
                    writer.WriteLineEx($"#include \"{filename}TableManager.generated.h\"");
                    
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
                        writer.WriteLineEx($"UCLASS(BlueprintType)");
                        writer.WriteLineEx($"class {CPPClassPredefine} U{filename}DataTable : public UDataTable");
                        writer.WriteLineEx("{");

                        writer.WriteLineEx($"GENERATED_BODY()");
                        writer.WriteLineEx($"public:");
                        writer.WriteLineEx($"U{filename}DataTable();");
                        writer.WriteLineEx($"virtual ~U{filename}DataTable() = default;");
                        writer.WriteLineEx($"//~ Begin UObject Interface.");
                        writer.WriteLineEx($"virtual void PostLoad() override;");
                        writer.WriteLineEx($"//~ End UObject Interface");
                        foreach (var sheet in sheetsColumns)
                        {
                            var keyColumn = sheet.Value.FirstOrDefault(compare => compare.is_key == true);
                            string keyType = keyColumn.GenerateType(_gen_type);
                            string sheetName = $"{filename}_{sheet.Key}";
                            writer.WriteLineEx($"UPROPERTY(EditAnywhere)");
                            writer.WriteLineEx($"TArray<F{sheetName}> {sheet.Key}Array;");
                            writer.WriteLineEx($"UPROPERTY(EditAnywhere)");
                            writer.WriteLineEx($"TMap<{keyType},F{sheetName}> {sheet.Key}Map;");
                            writer.WriteLineEx("");
                        }
                        writer.WriteLineEx("};");
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
                    writer.WriteLineEx($"class FTableManager");
                    writer.WriteLineEx($"{{");
                    writer.WriteLineEx("public:");
                    writer.WriteLineEx($"static bool LoadTable(FBufferReader& Reader, U{filename}DataTable& DataTable);");
                    writer.WriteLineEx($"}};");
                    writer.WriteLineEx($"}}");
                    writer.Flush();
                }
                ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
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
