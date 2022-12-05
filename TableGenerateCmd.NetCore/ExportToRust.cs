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
        public ExportToRust(bool useInterface)
        {
            _useInterface = useInterface;
        }

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            try
            {
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".rs");

                using var stream = new MemoryStream();
                {
                    using var writer = new IndentedTextWriter(new StreamWriter(stream,  Encoding.UTF8), " ");
                    {
                        string filename = System.IO.Path.GetFileName(createFileName);

                        writer.WriteLineEx("include!(\"_.rs\");");
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
                        writer.WriteLineEx("#[allow(dead_code)]");
                        writer.WriteLineEx("fn read_string(reader: &mut binary_reader::BinaryReader) -> String {");
                        writer.WriteLineEx("let len = reader.read_u32().unwrap() as usize;");
                        writer.WriteLineEx("let vec = reader.read_bytes(len).unwrap();");
                        writer.WriteLineEx("let ret = String::from_utf8(Vec::from(vec)).map_err(|err| {");
                        writer.WriteLineEx("    std::io::Error::new(");
                        writer.WriteLineEx("        std::io::ErrorKind::InvalidData,");
                        writer.WriteLineEx("        format!(\"failed to convert to string: {:?}\", err),");
                        writer.WriteLineEx("    )");
                        writer.WriteLineEx("}).unwrap();");
                        writer.WriteLineEx("ret");
                        writer.WriteLineEx("}");
                        
                        writer.WriteLineEx($"#[allow(dead_code)]");
                        writer.WriteLineEx($"#[allow(non_snake_case)]");
                        writer.WriteEx($"pub fn read_stream(reader: &mut binary_reader::BinaryReader) {{");
                        writer.WriteLineEx($"reader.set_endian(binary_reader::Endian::Little);");
                        writer.WriteLineEx($"let _streamLength = reader.length;");
                        writer.WriteLineEx($"let _hashLength = reader.read_i8().unwrap() as usize;");
                        writer.WriteLineEx($"let _ = reader.read(_hashLength);");
                        writer.WriteLineEx($"let _decompressedSize = reader.read_u32().unwrap() as usize;");
                        writer.WriteLineEx($"let mut _compressedSize = reader.read_u32().unwrap() as usize;");
                        writer.WriteLineEx($"let mut compressed = reader.read(_compressedSize).unwrap();");
                        writer.WriteLineEx($"let mut decoder = flate2::read::ZlibDecoder::new(&mut compressed);");
                        writer.WriteLineEx($"let mut decompressed = Vec::new();");
                        writer.WriteLineEx($"let _ = std::io::Read::read_to_end( &mut decoder, &mut decompressed).unwrap();");
                        writer.WriteLineEx($"let mut decompressReader = binary_reader::BinaryReader::from_vec(&mut decompressed);");
                        writer.WriteLineEx($"decompressReader.set_endian(binary_reader::Endian::Little);");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            writer.WriteLineEx($"let ({sheetName}_vec, {sheetName}_map) = {sheetName}::read_stream(&mut decompressReader);");
                        }
                        writer.WriteLineEx( "let static_data = StaticData {");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            var firstColumn = columns.FirstOrDefault(t => t.is_key);
                            var firstColumnType = firstColumn.GenerateType(_gen_type);
                            var firstColumnName = firstColumn.var_name;
                            writer.WriteLineEx($"{sheetName}_vec,");
                            writer.WriteLineEx($"{sheetName}_map,");
                        }
                        writer.WriteLineEx( "};");
                        writer.WriteLineEx("let lock = RWLOCK.write().unwrap();");
                        writer.WriteLineEx("  unsafe { STATIC_DATA.push(static_data);}");
                        writer.WriteLineEx("drop(lock);");
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
                        writer.WriteLineEx($"/// STATIC_DATA RWLOCK");
                        writer.WriteLineEx($"static RWLOCK: std::sync::RwLock<()> = std::sync::RwLock::new(());");
                        writer.WriteLineEx($"/// STATIC_DATA");
                        writer.WriteLineEx($"static mut STATIC_DATA: once_cell::sync::Lazy<Vec<StaticData>> = once_cell::sync::Lazy::new(||Vec::with_capacity(100));");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            var firstColumn = columns.FirstOrDefault(t => t.is_key);
                            var firstColumnType = firstColumn.GenerateType(_gen_type);
                            var firstColumnName = firstColumn.var_name;
                            writer.WriteLineEx($"/// get vec {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}vec() -> &'static Vec<{sheetName}> {{");
                            writer.WriteLineEx( "unsafe {");
                            writer.WriteLineEx($"let lock = RWLOCK.read().unwrap();");
                            writer.WriteLineEx($"let ret = &STATIC_DATA.last().unwrap().{sheetName}_vec;");
                            writer.WriteLineEx($"drop(lock);");
                            writer.WriteLineEx($"ret");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get map {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}map() -> &'static std::collections::HashMap<{firstColumnType},{sheetName}> {{");
                            writer.WriteLineEx( "unsafe {");
                            writer.WriteLineEx($"let lock = RWLOCK.read().unwrap();");
                            writer.WriteLineEx( $"let ret = &STATIC_DATA.last().unwrap().{sheetName}_map;");
                            writer.WriteLineEx($"drop(lock);");
                            writer.WriteLineEx($"ret");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx( "}");
                        }
                        writer.WriteLineEx($"#[allow(dead_code)]");
                        writer.WriteLineEx($"#[allow(non_snake_case)]");
                        writer.WriteLineEx($"pub struct StaticData {{");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            var firstColumn = columns.FirstOrDefault(t => t.is_key);
                            var firstColumnType = firstColumn.GenerateType(_gen_type);
                            var firstColumnName = firstColumn.var_name;
                            writer.WriteLineEx($"{sheetName}_vec: Vec<{sheetName}>,");
                            writer.WriteLineEx($"{sheetName}_map: std::collections::HashMap<{firstColumnType},{sheetName}>,");
                        }
                        writer.WriteLineEx($"}}");
                        writer.WriteLineEx($"impl StaticData {{");
                        
                        writer.WriteLineEx($"#[allow(non_snake_case)]");
                        writer.WriteLineEx( "pub fn read_from_file(output_path: &str, language: & str) {");
                        writer.WriteLineEx($"let file_name = r\"{filename}.bytes\";");
                        writer.WriteLineEx($"let mut file = std::fs::File::open( std::path::Path::new(output_path).join(language).join(file_name) ).unwrap();");
                        writer.WriteLineEx($"let mut reader = binary_reader::BinaryReader::from_file(&mut file);");
                        writer.WriteLineEx($"read_stream(&mut reader);");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");

                        writer.WriteLineEx($"#[test]");
                        writer.WriteLineEx( "fn read_tests() {");
                        writer.WriteLineEx($"let file_name = r\"{filename}.bytes\";");
                        writer.WriteLineEx($"let output_path = r\"../../GameDesign/Output\";");
                        writer.WriteLineEx($"let folders = std::fs::read_dir(output_path)");
                        writer.WriteLineEx($"    .unwrap()");
                        writer.WriteLineEx($"    .map(|r| r.map(|e| e.path()))");
                        writer.WriteLineEx($"    .collect::<Result<Vec<std::path::PathBuf>, _>>()");
                        writer.WriteLineEx($"    .unwrap();");
                        writer.WriteLineEx( "for folder in folders.iter().filter(|f|f.is_dir()) {");
                        writer.WriteLineEx($"let files = std::fs::read_dir(folder)");
                        writer.WriteLineEx($"    .unwrap()");
                        writer.WriteLineEx($"    .map(|r| r.map(|e| e.path()))");
                        writer.WriteLineEx($"    .collect::<Result<Vec<std::path::PathBuf>, _>>()");
                        writer.WriteLineEx($"    .unwrap();");
                        writer.WriteLineEx( "for file  in files.iter().filter(|f|f.is_file()) {");
                        writer.WriteLineEx( "if file.file_name().unwrap().eq(file_name) {");
                        writer.WriteLineEx($"StaticData::read_from_file(output_path, folder.file_name().unwrap().to_str().unwrap());");
                        foreach (string sheetName in sheets)
                        {
                            writer.WriteLineEx($"    println!(\"{{}} {sheetName}:{{}}\", folder.file_name().unwrap().to_str().unwrap(), {(sheets.Length>1?$"{sheetName}_":string.Empty)}vec().len());");
                        }
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        
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
        private void SheetProcess(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            //writer.WriteLineEx($"/// <summary>");
            //writer.WriteLineEx($"/// {sheetName}"); 
            //writer.WriteLineEx($"/// </summary>");
            //InnerSheetDescProcess(writer,columns);
            writer.WriteLineEx($"#[derive(Clone)]");
            writer.WriteLineEx($"#[derive(Debug)]");
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
            writer.WriteLineEx($"#[allow(non_snake_case)]");
            writer.WriteLineEx($"pub fn read_stream(reader: &mut binary_reader::BinaryReader) -> (Vec<{sheetName}>,std::collections::HashMap<{firstColumnType},{sheetName}>,) {{");
            writer.WriteLineEx($"let size = reader.read_u32().unwrap() as usize;");
            writer.WriteLineEx($"let mut vec: Vec<{sheetName}> = Vec::with_capacity(size);");
            writer.WriteLineEx($"let mut map: std::collections::HashMap<{firstColumnType},{sheetName}> = std::collections::HashMap::with_capacity(size);");
            writer.WriteLineEx($"for _ in 0..size {{");
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
                    eBaseType.Enum => "unsafe { std::mem::transmute(reader.read_i32().unwrap()) }",
                    eBaseType.Int32 => "reader.read_i32().unwrap()",
                    eBaseType.Int64 => "reader.read_i64().unwrap()",
                    eBaseType.Float => "reader.read_f32().unwrap()",
                    eBaseType.Double => "reader.read_f64().unwrap()",
                    eBaseType.String => "read_string(reader)",
                    eBaseType.Vector3 => "glam::f64::DVec3::new(reader.read_f64().unwrap(),reader.read_f64().unwrap(),reader.read_f64().unwrap())",
                    _ => "util::reader.read_i32().unwrap()"
                };
                writer.WriteLineEx(column.is_array
                    ? $"  {name}: {{let size = reader.read_i32().unwrap() as usize; std::iter::repeat_with(||{readStream}).take(size).collect()}},"
                    : $"{(readStream.Contains('{')? $"  {name}":name)}: {readStream},");
            }
            writer.WriteLineEx($"}};");
            writer.WriteLineEx($"map.insert(v.{firstColumnName},v.clone());");
            writer.WriteLineEx($"vec.push(v);");
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"(vec,map)");
            writer.WriteLineEx($"}}");
        }
    }
}
