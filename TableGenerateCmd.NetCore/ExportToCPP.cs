﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;

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

        public static String NameSpace = string.Empty;
        public int m_current = 0;
        public eGenType _gen_type = eGenType.cpp;

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            try
            {
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".cpp");
                string filename = System.IO.Path.GetFileName(createFileName);
                string @namespace = $"{ ExportToCSMgr.NameSpace }::{filename.Replace(".cpp", string.Empty)}";
                using (MemoryStream stream = new MemoryStream())
                {
                    var _writer = new IndentedTextWriter(new StreamWriter(stream, new System.Text.ASCIIEncoding()), "  ");
                    {

                        _writer.WriteLineEx($"// generate {filename}");
                        _writer.WriteLineEx("// DO NOT TOUCH SOURCE....");
                        _writer.WriteLineEx($"#include \"{filename.Replace(".cpp",".h")}\"");
                        _writer.WriteLineEx($"using namespace {@namespace};");

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
                            SheetProcess(_writer, filename, trimSheetName, columns);
                        }
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
            SheetConstructorProcess(_writer, sheetName, columns);
            SheetFindFunction(_writer, sheetName, columns);
        }

        private void SheetConstructorProcess(IndentedTextWriter _writer, string sheetName, List<Column> columns)
        {
            _writer.WriteLineEx($"const {sheetName}::Array {sheetName}::array;");
            _writer.WriteLineEx($"const {sheetName}::Map {sheetName}::map;");
            _writer.WriteLineEx(string.Format("{0} ({1})\n:{2}",
                $"{sheetName}::{sheetName}",
                string.Join("\n,", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"const {t.GenerateType(_gen_type)}& {t.var_name}__").ToArray()),
                string.Join("\n,", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.var_name}({t.var_name}__)").ToArray()))
            );
            _writer.WriteLineEx($"{{");
            _writer.WriteLineEx($"}}");
        }
        private void SheetFindFunction(IndentedTextWriter _writer, string sheetName, List<Column> columns)
        {
            var key_column = columns.FirstOrDefault(compare => compare.is_key == true);
            string keyType = key_column.GenerateType(_gen_type);

            _writer.WriteLineEx($"{sheetName}Ptr {sheetName}::Find(const {keyType}& {key_column.var_name})");
            _writer.WriteLineEx($"{{");
            _writer.WriteLineEx($"auto it = {sheetName}::map.find({key_column.var_name});");
            _writer.WriteLineEx($"if (it != {sheetName}::map.end())");
            _writer.WriteLineEx($"{{");
            _writer.WriteLineEx($"return it->second;");
            _writer.WriteLineEx($"}}");
            _writer.WriteLineEx($"return {sheetName}Ptr(nullptr);");
            _writer.WriteLineEx($"}}");
        }
    }
}
