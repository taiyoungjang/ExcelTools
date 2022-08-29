using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace TableGenerate
{
    public class ExportToRust : ExportBase
    {
        private readonly string _unityDefine = "UNITY_2018_2_OR_NEWER";

        private string _async = string.Empty;
        public string SetAsync
        {
            //set { _async = "unity3d"; }
            set { _async = value; }
        }

        public static string NameSpace = string.Empty;
        public int m_current = 0;
        public eGenType _gen_type = eGenType.rust;
        private bool _useInterface;
        public ExportToRust(string unityDefine, bool useInterface)
        {
            _unityDefine = unityDefine;
            _useInterface = useInterface;
        }

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            try
            {
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".rs");

                using var stream = new MemoryStream();
                {
                    using var writer = new IndentedTextWriter(new StreamWriter(stream,  System.Text.Encoding.UTF8), " ");
                    {
                        string filename = System.IO.Path.GetFileName(createFileName);

                        writer.WriteLineEx("use std::collections::HashMap;");

                        string[] sheets = imp.GetSheetList();

                        filename = filename.Replace(".rs", string.Empty);

                        max = sheets.GetLength(0);
                        current = 0;

                        foreach (string sheetName in sheets)
                        {
                            current++;
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            SheetProcess(writer, filename, trimSheetName, columns);
                        }
                        writer.WriteLineEx($"#[allow(dead_code)]");
                        writer.WriteLineEx($"#[allow(non_snake_case)]");
                        writer.WriteLineEx($"pub fn readStream(reader: &mut binary_reader::BinaryReader) {{");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            writer.WriteLineEx($"let (_{sheetName}_map, _{sheetName}_vec) = {sheetName}::readStream(reader);");
                        }
                        writer.WriteLineEx($"}}");

                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            writer.WriteLineEx($"impl {sheetName}");
                            writer.WriteLineEx("{");
                            InnerSheetReadStreamProcess(sheetName, writer, columns);
                            //SheetConstructorProcess(writer, sheetName, columns);
                            writer.WriteLineEx("}");
                        }
                        //writer.WriteLineEx("}");
                        //writer.WriteLineEx("}");
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
        private void SheetProcess(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            //writer.WriteLineEx($"/// <summary>");
            //writer.WriteLineEx($"/// {sheetName}"); 
            //writer.WriteLineEx($"/// </summary>");
            //InnerSheetDescProcess(writer,columns);
            writer.WriteLineEx($"#[derive(Clone)]");
            writer.WriteLineEx($"#[allow(dead_code)]");
            writer.WriteLineEx($"#[allow(non_snake_case)]");
            writer.WriteLineEx($"pub struct {sheetName}");
            writer.WriteLineEx("{");
            InnerSheetProcess(writer, columns);
            //SheetConstructorProcess(writer, sheetName, columns);
            writer.WriteLineEx("}");
        }
        private void InnerSheetDescProcess(IndentedTextWriter writer, List<Column> columns)
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
                if(column.is_key)
                {
                    // writer.WriteLineEx($"/// <summary>");
                    // writer.WriteLineEx($"/// Key Column");
                    // writer.WriteLineEx($"/// </summary>");
                }
                
                //if (_async == "unity3d")
                //{
                //    _writer.WriteLineEx($"public abstract {type} {name} {{get;}}");
                //}
                //else
                {
                    writer.WriteLineNoTabs($"/// <param name=\"{name}\">{column.desc}</param> ");
                }
            }
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
                if(column.is_key)
                {
                    // writer.WriteLineEx($"/// <summary>");
                    // writer.WriteLineEx($"/// Key Column");
                    // writer.WriteLineEx($"/// </summary>");
                }
                //if (_async == "unity3d")
                //{
                //    _writer.WriteLineEx($"public abstract {type} {name} {{get;}}");
                //}
                //else
                {
                    if (!string.IsNullOrEmpty(column.desc))
                    {
                        writer.WriteLineEx($"/// {column.desc}");
                    }
                    writer.WriteLineEx($"pub {name}: {type},");
                }
            }
        }
        
        private void InnerSheetReadStreamProcess(string sheetName, IndentedTextWriter writer, List<Column> columns)
        {
            var firstColumn = columns.FirstOrDefault(t => t.is_key);
            var firstColumnType = firstColumn.GenerateType(_gen_type);
            var firstColumnName = firstColumn.var_name;
            writer.WriteLineEx($"#[allow(dead_code)]");
            writer.WriteLineEx($"pub fn readStream(reader: &mut binary_reader::BinaryReader) -> (Vec<{sheetName}>,HashMap<{firstColumnType},{sheetName}>) {{");
            writer.WriteLineEx($"let map:HashMap<{firstColumnType},{sheetName}> = std::iter::repeat(reader.read_i32().unwrap()).map(|_| {{");
            writer.WriteLineEx($"let v = {sheetName} {{");
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
                if(column.is_key)
                {
                    // writer.WriteLineEx($"/// <summary>");
                    // writer.WriteLineEx($"/// Key Column");
                    // writer.WriteLineEx($"/// </summary>");
                }
                //if (_async == "unity3d")
                //{
                //    _writer.WriteLineEx($"public abstract {type} {name} {{get;}}");
                //}
                //else
                string readStream = column.base_type switch 
                {
                    eBaseType.Boolean => "reader.read_bool().unwrap()",
                    eBaseType.Int8 => "reader.read_i8().unwrap()",
                    eBaseType.Int16 => "reader.read_i16().unwrap()",
                    eBaseType.Enum => "reader.read_i32().unwrap()",
                    eBaseType.Int32 => "reader.read_i32().unwrap()",
                    eBaseType.Int64 => "reader.read_i64().unwrap()",
                    eBaseType.Float => "reader.read_f32().unwrap()",
                    eBaseType.Double => "reader.read_f64().unwrap()",
                    eBaseType.String => "reader.read_cstr().unwrap()",
                    _ => "reader.read_i32().unwrap()"
                };
                if (column.is_array)
                {
                    readStream = $"std::iter::repeat(reader.read_i32().unwrap()).map(|_|{readStream}).collect()";
                }
                {
                    
                    writer.WriteLineEx($"{name}: {readStream},");
                }
            }
            writer.WriteLineEx($"}}; (v.{firstColumnName},v) }}).collect();");
            writer.WriteLineEx($"(map.values().cloned().collect(),map)");
            writer.WriteLineEx($"}}");
        }
        private void SheetConstructorProcess(IndentedTextWriter _writer, string sheetName, List<Column> columns)
        {
            //if (_async != "unity3d")
            {
                _writer.WriteLineEx(string.Format("public {0} ({1})",
                    sheetName,
                    string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.GenerateType(_gen_type)} {t.var_name}__").ToArray()))
                );
                _writer.WriteLineEx($"{{");
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
                    _writer.WriteLineEx($"this.{name} = {name}__;");
                }
                _writer.WriteLineEx($"}}");
            }
        }
    }
}
