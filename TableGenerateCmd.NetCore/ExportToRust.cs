using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;

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

                        writer.WriteLineEx("use super::super::proto::*;");
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
                        writer.WriteLineEx($"let _stream_length = reader.length;");
                        writer.WriteLineEx($"let _hash_length = reader.read_i8().unwrap() as usize;");
                        writer.WriteLineEx($"let _ = reader.read(_hash_length);");
                        writer.WriteLineEx($"let _decompressed_size = reader.read_u32().unwrap() as usize;");
                        writer.WriteLineEx($"let mut _compressed_size = reader.read_u32().unwrap() as usize;");
                        writer.WriteLineEx($"let mut compressed = reader.read(_compressed_size).unwrap();");
                        writer.WriteLineEx($"let mut decoder = flate2::read::ZlibDecoder::new(&mut compressed);");
                        writer.WriteLineEx($"let mut decompressed = Vec::new();");
                        writer.WriteLineEx($"let _ = std::io::Read::read_to_end( &mut decoder, &mut decompressed).unwrap();");
                        writer.WriteLineEx($"let mut decompress_reader = binary_reader::BinaryReader::from_vec(&mut decompressed);");
                        writer.WriteLineEx($"decompress_reader.set_endian(binary_reader::Endian::Little);");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            var pascalSheetName = ExportBaseUtil.ToPascalCase(sheetName);
                            var snakeSheetName = ExportBaseUtil.ToSnakeCase(sheetName);
                            writer.WriteLineEx($"let ({snakeSheetName}_vec, {snakeSheetName}_map) = {pascalSheetName}::read_stream(&mut decompress_reader);");
                        }
                        writer.WriteLineEx( "let static_data = StaticData {");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            var pascalSheetName = ExportBaseUtil.ToPascalCase(sheetName);
                            var snakeSheetName = ExportBaseUtil.ToSnakeCase(sheetName);
                            var firstColumn = columns.FirstOrDefault(t => t.is_key);
                            var firstColumnType = firstColumn.GenerateType(_gen_type);
                            var firstColumnName = firstColumn.var_name;
                            writer.WriteLineEx($"{pascalSheetName}_vec: {snakeSheetName}_vec,");
                            writer.WriteLineEx($"{pascalSheetName}_map :{snakeSheetName}_map,");
                        }
                        writer.WriteLineEx( "};");
                        writer.WriteLineEx(" STATIC_DATA.write().unwrap().push(static_data);");
                        writer.WriteLineEx($"}}");

                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            string pascalSheetName = ExportBaseUtil.ToPascalCase(sheetName);
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            writer.WriteLineEx($"impl {pascalSheetName}");
                            writer.WriteLineEx("{");
                            InnerSheetReadStreamProcess(pascalSheetName, writer, columns);
                            EGuiGen(pascalSheetName, writer, columns);
                            var firstColumn = columns.FirstOrDefault(t => t.is_key);
                            var firstColumnType = firstColumn.GenerateType(_gen_type);
                            var firstColumnName = firstColumn.var_name;
                            writer.WriteLineEx( $"pub fn key_string(&self) -> String {{");
                            writer.WriteLineEx(GenToString(firstColumn));
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx( $"pub fn title() -> &'static str {{");
                            writer.WriteLineEx($"\"{pascalSheetName}\"");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx( $"pub fn key_name() -> &'static str {{");
                            writer.WriteLineEx($"\"{firstColumnName}\"");
                            writer.WriteLineEx( "}");
                            //SheetConstructorProcess(writer, sheetName, columns);
                            writer.WriteLineEx("}");
                        }
                        foreach (string sheet in sheets)
                        {
                            string trimSheetName = sheet.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheet, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            var sheetName = ExportBaseUtil.ToPascalCase(sheet);
                            var firstColumn = columns.FirstOrDefault(t => t.is_key);
                            var firstColumnType = firstColumn.GenerateType(_gen_type);
                            var firstColumnName = firstColumn.var_name;
                            writer.WriteLineEx($"/// get vec_clone {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx($"#[allow(non_snake_case)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}vec_clone() -> Option<Vec<{sheetName}>> {{");
                            writer.WriteLineEx($"Some(STATIC_DATA.read().unwrap().last().unwrap().{sheetName}_vec.clone())");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get vec {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx($"#[allow(non_snake_case)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}vec<F: Fn (&Vec<{sheetName}>) -> Option<Vec<{sheetName}>>>(pred: F) -> Option<Vec<{sheetName}>> {{");
                            writer.WriteLineEx($"pred(&STATIC_DATA.read().unwrap().last().unwrap().{sheetName}_vec)");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get vec_one {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx($"#[allow(non_snake_case)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}vec_one<F: Fn (&Vec<{sheetName}>) -> Option<{sheetName}>>(pred: F) -> Option<{sheetName}> {{");
                            writer.WriteLineEx($"pred(&STATIC_DATA.read().unwrap().last().unwrap().{sheetName}_vec)");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get map {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx($"#[allow(non_snake_case)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}map_clone() -> Option<std::collections::HashMap<{firstColumnType},{sheetName}>> {{");
                            writer.WriteLineEx( $"Some(STATIC_DATA.read().unwrap().last().unwrap().{sheetName}_map.clone())");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get map {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx($"#[allow(non_snake_case)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}map<F: Fn (&std::collections::HashMap<{firstColumnType},{sheetName}>) -> Option<std::collections::HashMap<{firstColumnType},{sheetName}>>>(pred: F) -> Option<std::collections::HashMap<{firstColumnType},{sheetName}>> {{");
                            writer.WriteLineEx( $"pred(&STATIC_DATA.read().unwrap().last().unwrap().{sheetName}_map)");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get map_one {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx($"#[allow(non_snake_case)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}map_one<F: Fn (&std::collections::HashMap<{firstColumnType},{sheetName}>) -> Option<{sheetName}>>(pred: F) -> Option<{sheetName}> {{");
                            writer.WriteLineEx( $"pred(&STATIC_DATA.read().unwrap().last().unwrap().{sheetName}_map)");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx($"#[allow(non_snake_case)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}get({firstColumnName}: &{firstColumnType}) -> Option<{sheetName}> {{");
                            writer.WriteLineEx( $"STATIC_DATA.read().unwrap().last().unwrap().{sheetName}_map.get(&{firstColumnName}).cloned()");
                            writer.WriteLineEx( "}");
                        }
                        writer.WriteLineEx($"lazy_static::lazy_static! {{");
                        writer.WriteLineEx($"/// STATIC_DATA");
                        writer.WriteLineEx($"static ref STATIC_DATA: std::sync::RwLock<Vec<StaticData>> = std::sync::RwLock::new(Vec::with_capacity(100));");
                        writer.WriteLineEx($"}}");
                        writer.WriteLineEx($"#[allow(dead_code)]");
                        writer.WriteLineEx($"#[allow(non_snake_case)]");
                        writer.WriteLineEx($"#[derive(bevy_ecs::prelude::Resource)]");
                        writer.WriteLineEx($"pub struct StaticData {{");
                        foreach (string sheet in sheets)
                        {
                            string trimSheetName = sheet.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheet, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            var firstColumn = columns.FirstOrDefault(t => t.is_key);
                            var firstColumnType = firstColumn.GenerateType(_gen_type);
                            var firstColumnName = firstColumn.var_name;
                            var sheetName = ExportBaseUtil.ToPascalCase(sheet);
                            writer.WriteLineEx($"pub {sheetName}_vec: Vec<{sheetName}>,");
                            writer.WriteLineEx($"pub {sheetName}_map: std::collections::HashMap<{firstColumnType},{sheetName}>,");
                        }
                        writer.WriteLineEx($"}}");
                        writer.WriteLineEx($"/// file_name");
                        writer.WriteLineEx( "pub fn file_name() -> &'static str {");
                        writer.WriteLineEx($"r\"{filename}.bytes\"");
                        writer.WriteLineEx($"}}");
                        writer.WriteLineEx($"#[allow(dead_code)]");
                        writer.WriteLineEx( "pub fn insert_resource_from_file(world: &mut bevy_ecs::prelude::World, output_path: &str, language: & str) -> Result<(),std::io::Error> {");
                        writer.WriteLineEx( "match read_from_file(output_path, language) {");
                        writer.WriteLineEx( "Ok(_) => {");
                        writer.WriteLineEx( "let static_data = StaticData {");
                        foreach (string sheetName in sheets)
                        {
                            var sheet = ExportBaseUtil.ToPascalCase(sheetName);
                            writer.WriteLineEx( $"{sheet}_vec: vec_clone().unwrap(),");
                            writer.WriteLineEx( $"{sheet}_map: map_clone().unwrap(),");
                        }
                        writer.WriteLineEx( "};");
                        writer.WriteLineEx( "world.insert_resource(static_data);");
                        writer.WriteLineEx( "Ok(())");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "Err(e) => {");
                        writer.WriteLineEx( "Err(e)");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx($"/// read_from_file()");
                        writer.WriteLineEx( "pub fn read_from_file(output_path: &str, language: & str) -> Result<(),std::io::Error> {");
                        writer.WriteLineEx($"let file_name = file_name();");
                        writer.WriteLineEx($"match std::fs::File::open( std::path::Path::new(output_path).join(language).join(file_name)) {{");
                        writer.WriteLineEx($"Ok(mut file) => {{");
                        writer.WriteLineEx($"let mut reader = binary_reader::BinaryReader::from_file(&mut file);");
                        writer.WriteLineEx($"read_stream(&mut reader);");
                        writer.WriteLineEx($"Ok(())");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "Err(e) => {");
                        writer.WriteLineEx( "Err(e)");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");

                        writer.WriteLineEx($"#[test]");
                        writer.WriteLineEx( "fn read_tests() {");
                        writer.WriteLineEx($"let file_name = file_name();");
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
                        writer.WriteLineEx($"if let Ok(()) = read_from_file(output_path, folder.file_name().unwrap().to_str().unwrap()){{");
                        foreach (string sheetName in sheets)
                        {
                            writer.WriteLineEx($"    println!(\"{{}} {ExportBaseUtil.ToSnakeCase(sheetName)}:{{}}\", folder.file_name().unwrap().to_str().unwrap(), {(sheets.Length>1?$"{sheetName}_":string.Empty)}vec_clone().unwrap().len());");
                        }
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        
                        writer.Flush();
                    }
                    ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{ExportBaseUtil.ToSnakeCase(createFileName.Replace(".rs",""))}.rs", TableGenerateCmd.ProgramCmd.using_perforce);
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
            writer.WriteLineEx($"#[allow(non_snake_case)]");
            writer.WriteLineEx($"#[allow(non_camel_case_types)]");
            writer.WriteLineEx($"pub struct {ExportBaseUtil.ToPascalCase(sheetName)}");
            writer.WriteLineEx("{");
            InnerSheetProcess(writer, columns);
            writer.WriteLineEx("}");
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
                        writer.WriteLineNoTabs($"{string.Empty.PadLeft(writer.Indent)}/// {column.desc}");
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
                    eBaseType.Enum => $"{column.type_name}::from_i32(reader.read_i32().unwrap()).unwrap()",
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
        private void EGuiGen(string sheetName, IndentedTextWriter writer, List<Column> columns)
        {
            var firstColumn = columns.FirstOrDefault(t => t.is_key);
            var firstColumnType = firstColumn.GenerateType(_gen_type);
            var firstColumnName = firstColumn.var_name;
            writer.WriteLineEx($"#[allow(dead_code)]");
            writer.WriteLineEx($"#[allow(non_snake_case)]");
            writer.WriteLineEx($"pub fn egui_header() -> fn(egui_extras::TableRow) {{");
            writer.WriteLineEx($"|mut header| {{");
            int arrayCount = 0;
            foreach (var column in columns)
            {
                string name = column.var_name;
                if (column.is_generated == false || column.array_index > 0)
                {
                    continue;
                }
                writer.WriteLineEx($"  let _ = header.col(|ui|{{let _ = ui.strong(\"{name}\");}});");
                arrayCount++;
            }
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"  pub fn column_count() -> usize {{ {arrayCount} }}");
            writer.WriteLineEx($"#[allow(dead_code)]");
            writer.WriteLineEx($"#[allow(non_snake_case)]");
            writer.WriteLineEx($"pub fn egui_body(body: egui_extras::TableBody, items: Vec<&{sheetName}>) {{");
            writer.WriteLineEx($"body.heterogeneous_rows((0..items.len()).into_iter().map(|_| 18f32), |row_index, mut row| {{");
            writer.WriteLineEx($"let item = items.get(row_index).unwrap();");
            foreach (var column in columns)
            {
                string name = column.var_name;
                if (column.is_generated == false || column.array_index > 0)
                {
                    continue;
                }
                if (column.is_array)
                {
                    if (column.array_index == 0)
                    {
                        writer.WriteLineEx(GenEGuiBodyArray(column));
                    }
                }
                else
                {
                    writer.WriteLineEx(GenEGuiBody(column));
                }
            }
            writer.WriteLineEx($"}});");
            writer.WriteLineEx($"}}");
        }
        private string GenEGuiBody(Column column)
        {
            string ret = "";
            string name = column.var_name;
            if (column.IsEnumType())
            {
                ret = $"    let _ = row.col(|ui|{{let _ = ui.label(format!(\"{{:?}}\",item.{name}));}});";
            }
            else if (column.IsNumberType() || column.base_type == eBaseType.Boolean)
            {
                ret = $"  let _ = row.col(|ui|{{let _ = ui.label(item.{name}.to_string());}});";
            }
            else if (column.base_type == eBaseType.String)
            {
                ret = $"  let _ = row.col(|ui|{{let _ = ui.label(&item.{name});}});";
            }
            else if (column.base_type == eBaseType.Vector3)
            {
                ret = $"  let _ = row.col(|ui|{{let _ = ui.label(item.{name});}});";
            }
            return ret;
        }
        private string GenEGuiBodyArray(Column column)
        {
            string name = column.var_name;
            string ret = ret = $"    let _ = row.col(|ui|{{let _ = ui.label(format!(\"{{:?}}\",item.{name}));}});";
            return ret;
        }
        private string GenToString(Column column)
        {
            string ret = "";
            string name = column.var_name;
            if (column.IsEnumType())
            {
                ret = $"format!(\"{{:?}}\",self.{name}))";
            }
            else if (column.IsNumberType() || column.base_type == eBaseType.Boolean)
            {
                ret = $"self.{name}.to_string()";
            }
            else if (column.base_type == eBaseType.String)
            {
                ret = $"self.{name}.clone()";
            }
            else if (column.base_type == eBaseType.Vector3)
            {
                ret = $"format!(\"{{:?}}\",self.{name}))";
            }
            return ret;
        }
    }
}
