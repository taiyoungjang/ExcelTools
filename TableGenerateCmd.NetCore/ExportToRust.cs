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
        private bool _multi_sheet;
        public ExportToRust(bool useInterface)
        {
            _useInterface = useInterface;
        }
        private string GetVecName(string sheetName) => _multi_sheet? $"{sheetName}_vec" : "vec";
        private string GetMapName(string sheetName) => _multi_sheet? $"{sheetName}_map" : "map";
        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            try
            {
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".rs");

                using var stream = new MemoryStream();
                {
                    using var writer = new IndentedTextWriter(new StreamWriter(stream,  Encoding.UTF8), " ");
                    {
                        Util.SetIndentSize(2);
                        string filename = System.IO.Path.GetFileName(createFileName);

                        writer.WriteLineEx("use super::super::proto::*;");
                        writer.WriteLineEx("use anyhow::{anyhow, Result};");
                        writer.WriteLineEx("use std::{collections::HashMap, sync::RwLock};");
                        string[] sheets = imp.GetSheetList();
                        _multi_sheet = sheets.Length > 1;

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

                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            string pascalSheetName = ExportBaseUtil.ToPascalCase(sheetName);
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            writer.WriteLineEx($"impl {pascalSheetName} {{");
                            InnerSheetReadStreamProcess(pascalSheetName, writer, columns);
                            writer.WriteLineEx("}");
                            writer.WriteLineEx($"impl super::EguiTable for {pascalSheetName} {{");
                            EGuiGen(pascalSheetName, writer, columns);
                            var firstColumn = columns.FirstOrDefault(t => t.is_key);
                            var firstColumnType = firstColumn.GenerateType(_gen_type);
                            var firstColumnName = firstColumn.var_name;
                            writer.WriteLineEx( $"fn file_name() -> &'static str {{");
                            writer.WriteLineEx("file_name()");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"fn read_from_file(output_path: &str, language: &str)");
                            writer.WriteLineEx($"  -> Result<(), anyhow::Error> {{");
                            writer.WriteLineEx( "read_from_file(output_path, language)");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"fn read_stream(reader: &mut binary_reader::BinaryReader)");
                            writer.WriteLineEx($"  -> Result<(), anyhow::Error> {{");
                            writer.WriteLineEx( "read_stream(reader)");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx( $"fn key_string(&self) -> String {{");
                            writer.WriteLineEx(GenToString(firstColumn));
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx( $"fn title() -> &'static str {{");
                            writer.WriteLineEx($"\"{pascalSheetName}\"");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx( $"fn key_name() -> &'static str {{");
                            writer.WriteLineEx($"\"{firstColumnName}\"");
                            writer.WriteLineEx( "}");
                            //SheetConstructorProcess(writer, sheetName, columns);
                            writer.WriteLineEx($"/// get vec_clone {sheetName}");
                            writer.WriteLineEx( $"fn {GetVecName(sheetName)}_clone() -> Option<Vec<Self>> {{");
                            writer.WriteLineEx($"{GetVecName(sheetName)}_clone()");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get vec {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx( $"fn {GetVecName(sheetName)}<F: Fn(&Vec<Self>) -> Option<Vec<Self>>>(");
                            writer.WriteLineEx( $"  pred: F");
                            writer.WriteLineEx( $") -> Option<Vec<Self>> {{");
                            
                            writer.WriteLineEx($"vec(pred)");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get vec_one {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx( $"fn {GetVecName(sheetName)}_one<F: Fn(&Vec<Self>) -> Option<Self>>(");
                            writer.WriteLineEx( $"  pred: F");
                            writer.WriteLineEx( $") -> Option<Self> {{");
                            writer.WriteLineEx($"vec_one(pred)");
                            writer.WriteLineEx( "}");
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
                            writer.WriteLineEx( $"pub fn {GetVecName(sheetName)}_clone() -> Option<Vec<{sheetName}>> {{");
                            writer.WriteLineEx($"Some(STATIC_DATA.read().unwrap().last()?.{GetVecName(sheetName)}.clone())");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get vec {sheetName}");
                            writer.WriteLineEx( $"pub fn {GetVecName(sheetName)}<F: Fn(&Vec<{sheetName}>) -> Option<Vec<{sheetName}>>>(");
                            writer.WriteLineEx( $"  pred: F");
                            writer.WriteLineEx( $") -> Option<Vec<{sheetName}>> {{");
                            writer.WriteLineEx($"pred(&STATIC_DATA.read().unwrap().last()?.{GetVecName(sheetName)})");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get vec_one {sheetName}");
                            writer.WriteLineEx( $"pub fn {GetVecName(sheetName)}_one<F: Fn(&Vec<{sheetName}>) -> Option<{sheetName}>>(");
                            writer.WriteLineEx( $"  pred: F");
                            writer.WriteLineEx( $") -> Option<{sheetName}> {{");
                            writer.WriteLineEx($"pred(&STATIC_DATA.read().unwrap().last()?.{GetVecName(sheetName)})");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get map {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx( $"pub fn {GetMapName(sheetName)}_clone() -> Option<HashMap<{firstColumnType}, {sheetName}>> {{");
                            writer.WriteLineEx( $"Some(STATIC_DATA.read().unwrap().last()?.{GetMapName(sheetName)}.clone())");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get map {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx( $"pub fn {GetMapName(sheetName)}<F: Fn(&HashMap<{firstColumnType}, {sheetName}>) -> Option<HashMap<{firstColumnType}, {sheetName}>>>(");
                            writer.WriteLineEx( $"  pred: F");
                            writer.WriteLineEx( $") -> Option<HashMap<{firstColumnType}, {sheetName}>> {{");
                            writer.WriteLineEx( $"pred(&STATIC_DATA.read().unwrap().last()?.{GetMapName(sheetName)})");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get map_one {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx( $"pub fn {GetMapName(sheetName)}_one<F: Fn(&HashMap<{firstColumnType}, {sheetName}>) -> Option<{sheetName}>>(");
                            writer.WriteLineEx( $"  pred: F");
                            writer.WriteLineEx( $") -> Option<{sheetName}> {{");
                            writer.WriteLineEx( $"pred(&STATIC_DATA.read().unwrap().last()?.{GetMapName(sheetName)})");
                            writer.WriteLineEx( "}");
                            writer.WriteLineEx($"/// get {sheetName}");
                            writer.WriteLineEx($"#[allow(dead_code)]");
                            writer.WriteLineEx( $"pub fn {(sheets.Length>1?$"{sheetName}_":string.Empty)}get({firstColumnName}: &{firstColumnType}) -> Option<{sheetName}> {{");
                            writer.WriteLineEx( $"STATIC_DATA.read().unwrap().last()?.{GetMapName(sheetName)}.get(&{firstColumnName}).cloned()");
                            writer.WriteLineEx( "}");
                        }
                        writer.WriteLineEx($"lazy_static::lazy_static! {{");
                        writer.WriteLineEx($"/// STATIC_DATA");
                        writer.WriteLineEx($"static ref STATIC_DATA: RwLock<Vec<StaticData>> = RwLock::new(Vec::with_capacity(100));");
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
                            writer.WriteLineEx($"pub {GetVecName(sheetName)}: Vec<{sheetName}>,");
                            writer.WriteLineEx($"pub {GetMapName(sheetName)}: HashMap<{firstColumnType}, {sheetName}>,");
                        }
                        writer.WriteLineEx($"}}");
                        writer.WriteLineEx( "pub fn file_name() -> &'static str {");
                        writer.WriteLineEx($"r\"{filename}.bytes\"");
                        writer.WriteLineEx("}");
                        writer.WriteLineEx($"/// file_name");
                        writer.WriteLineEx( "impl super::StaticData for StaticData {");
                        writer.WriteLineEx( "fn len() -> usize {");
                        writer.WriteLineEx( "let mut len = 0usize;");
                        foreach (string sheetName in sheets)
                        {
                            var sheet = ExportBaseUtil.ToPascalCase(sheetName);
                            writer.WriteLineEx( $"len += STATIC_DATA.read().unwrap().last().unwrap().{GetVecName(sheetName)}.len();");
                        }
                        writer.WriteLineEx("len");
                        writer.WriteLineEx("}");
                        writer.WriteLineEx( "fn file_name() -> &'static str {");
                        writer.WriteLineEx($"file_name()");
                        writer.WriteLineEx("}");
                        writer.WriteLineEx($"#[allow(dead_code)]");
                        writer.WriteLineEx( "fn insert_resource_from_stream(");
                        writer.WriteLineEx( "  world: &mut bevy_ecs::prelude::World,");
                        writer.WriteLineEx( "  reader: &mut binary_reader::BinaryReader,");
                        writer.WriteLineEx( ") -> Result<(), anyhow::Error> {");
                        writer.WriteLineEx( "read_stream(reader)?;");
                        writer.WriteLineEx( "let static_data = Self {");
                        foreach (string sheetName in sheets)
                        {
                            var sheet = ExportBaseUtil.ToPascalCase(sheetName);
                            writer.WriteLineEx( $"{GetVecName(sheetName)}: vec_clone().unwrap(),");
                            writer.WriteLineEx( $"{GetMapName(sheetName)}: map_clone().unwrap(),");
                        }
                        writer.WriteLineEx( "};");
                        writer.WriteLineEx( "world.insert_resource(static_data);");
                        writer.WriteLineEx( "Ok(())");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx($"#[allow(dead_code)]");
                        writer.WriteLineEx( "fn insert_resource_from_file(");
                        writer.WriteLineEx( "world: &mut bevy_ecs::prelude::World,");
                        writer.WriteLineEx( "output_path: &str,");
                        writer.WriteLineEx( "language: &str,");
                        writer.WriteLineEx( ") -> Result<(), anyhow::Error> {");
                        writer.WriteLineEx( "read_from_file(output_path, language)?;");
                        writer.WriteLineEx( "let static_data = Self {");
                        foreach (string sheetName in sheets)
                        {
                            var sheet = ExportBaseUtil.ToPascalCase(sheetName);
                            writer.WriteLineEx( $"{GetVecName(sheetName)}: vec_clone().unwrap(),");
                            writer.WriteLineEx( $"{GetMapName(sheetName)}: map_clone().unwrap(),");
                        }
                        writer.WriteLineEx( "};");
                        writer.WriteLineEx( "world.insert_resource(static_data);");
                        writer.WriteLineEx( "Ok(())");
                        writer.WriteLineEx( "}");

                        writer.WriteLineEx($"fn read_from_file(output_path: &str, language: &str)");
                        writer.WriteLineEx($"  -> Result<(), anyhow::Error> {{");
                        writer.WriteLineEx( "read_from_file(output_path, language)");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx($"fn read_stream(reader: &mut binary_reader::BinaryReader)");
                        writer.WriteLineEx($"  -> Result<(), anyhow::Error> {{");
                        writer.WriteLineEx( "read_stream(reader)");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx($"/// read_from_file()");
                        writer.WriteLineEx( "pub fn read_from_file(output_path: &str, language: &str)");
                        writer.WriteLineEx( "  -> Result<(), anyhow::Error> {");
                        writer.WriteLineEx($"let file_name = file_name();");
                        writer.WriteLineEx($"match std::fs::File::open(");
                        writer.WriteLineEx($"  std::path::Path::new(output_path)");
                        writer.WriteLineEx($"    .join(language)");
                        writer.WriteLineEx($"    .join(file_name),");
                        writer.WriteLineEx($") {{");
                        writer.WriteLineEx($"Ok(mut file) => {{");
                        writer.WriteLineEx($"let mut reader = binary_reader::BinaryReader::from_file(&mut file);");
                        writer.WriteLineEx($"read_stream(&mut reader)");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "Err(e) => Err(anyhow!(\"read_from_file {}\", e)),");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx( "}");
                        writer.WriteLineEx($"#[allow(dead_code)]");
                        writer.WriteLineEx($"pub fn read_stream(reader: &mut binary_reader::BinaryReader)");
                        writer.WriteLineEx($"  -> Result<(), anyhow::Error> {{");
                        writer.WriteLineEx($"reader.set_endian(binary_reader::Endian::Little);");
                        writer.WriteLineEx($"let _stream_length = reader.length;");
                        writer.WriteLineEx($"let _hash_length = reader.read_i8()? as usize;");
                        writer.WriteLineEx($"let _ = reader.read(_hash_length);");
                        writer.WriteLineEx($"let _decompressed_size = reader.read_u32()? as usize;");
                        writer.WriteLineEx($"let mut _compressed_size = reader.read_u32()? as usize;");
                        writer.WriteLineEx($"let mut compressed = reader.read(_compressed_size).unwrap();");
                        writer.WriteLineEx($"let mut decoder = flate2::read::ZlibDecoder::new(&mut compressed);");
                        writer.WriteLineEx($"let mut decompressed = Vec::new();");
                        writer.WriteLineEx($"let _ = std::io::Read::read_to_end(&mut decoder, &mut decompressed)?;");
                        writer.WriteLineEx($"let mut decompress_reader = binary_reader::BinaryReader::from_vec(&mut decompressed);");
                        writer.WriteLineEx($"decompress_reader.set_endian(binary_reader::Endian::Little);");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            var pascalSheetName = ExportBaseUtil.ToPascalCase(sheetName);
                            var snakeSheetName = ExportBaseUtil.ToSnakeCase(sheetName);
                            writer.WriteLineEx($"let ({GetVecName(snakeSheetName)}, {GetMapName(snakeSheetName)}) = {pascalSheetName}::read_stream(&mut decompress_reader)?;");
                        }
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
                            if (!_multi_sheet)
                            {
                                writer.WriteLineEx( "let static_data = StaticData { vec, map };");
                            }
                            else
                            {
                                writer.WriteLineEx( "let static_data = StaticData {");
                                writer.WriteLineEx($"{GetVecName(pascalSheetName)}: {GetVecName(snakeSheetName)}, {GetMapName(pascalSheetName)} :{GetMapName(snakeSheetName)}");
                                writer.WriteLineEx( "};");
                            }
                        }
                        writer.WriteLineEx(" STATIC_DATA.write().unwrap().push(static_data);");
                        writer.WriteLineEx("Ok(())");
                        writer.WriteLineEx("}");

                        writer.WriteLineEx($"#[test]");
                        writer.WriteLineEx( "fn read_tests() {");
                        writer.WriteLineEx($"let file_name = file_name();");
                        writer.WriteLineEx($"let output_path = r\"../../GameDesign/Output\";");
                        writer.WriteLineEx($"let folders = std::fs::read_dir(output_path)");
                        writer.WriteLineEx($"  .unwrap()");
                        writer.WriteLineEx($"  .map(|r| r.map(|e| e.path()))");
                        writer.WriteLineEx($"  .collect::<Result<Vec<std::path::PathBuf>, _>>()");
                        writer.WriteLineEx($"  .unwrap();");
                        writer.WriteLineEx( "for folder in folders.iter().filter(|f| f.is_dir()) {");
                        writer.WriteLineEx($"let files = std::fs::read_dir(folder)");
                        writer.WriteLineEx($"  .unwrap()");
                        writer.WriteLineEx($"  .map(|r| r.map(|e| e.path()))");
                        writer.WriteLineEx($"  .collect::<Result<Vec<std::path::PathBuf>, _>>()");
                        writer.WriteLineEx($"  .unwrap();");
                        writer.WriteLineEx( "for file in files.iter().filter(|f| f.is_file()) {");
                        writer.WriteLineEx( "if file.file_name().unwrap().eq(file_name) {");
                        writer.WriteLineEx($"if let Ok(()) = ");
                        writer.WriteLineEx($"read_from_file(output_path, folder.file_name().unwrap().to_str().unwrap())");
                        writer.WriteLineEx($"{{");
                        foreach (string sheetName in sheets)
                        {
                            writer.WriteLineEx($"println!(");
                            writer.WriteLineEx($"  \"{{}} {ExportBaseUtil.ToSnakeCase(sheetName)}:{{}}\",");
                            writer.WriteLineEx($"  folder.file_name().unwrap().to_str().unwrap(),");
                            writer.WriteLineEx($"  {(sheets.Length>1?$"{sheetName}_":string.Empty)}vec_clone().unwrap().len()");
                            writer.WriteLineEx($");");
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
            writer.WriteLineEx($"#[derive(Clone, Debug)]");
            writer.WriteLineEx($"#[allow(non_snake_case)]");
            writer.WriteLineEx($"#[allow(non_camel_case_types)]");
            writer.WriteLineEx($"pub struct {ExportBaseUtil.ToPascalCase(sheetName)} {{");
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
            writer.WriteLineEx($"pub fn read_stream(");
            writer.WriteLineEx($"  reader: &mut binary_reader::BinaryReader,");
            writer.WriteLineEx($") -> Result<(Vec<Self>, HashMap<{firstColumnType}, Self>), anyhow::Error> {{");
            writer.WriteLineEx($"let size = reader.read_u32()? as usize;");
            writer.WriteLineEx($"let mut vec: Vec<{sheetName}> = Vec::with_capacity(size);");
            writer.WriteLineEx($"let mut map: HashMap<{firstColumnType}, Self> = HashMap::with_capacity(size);");
            writer.WriteLineEx($"for _ in 0..size {{");
            writer.WriteLineEx($"let v = Self {{");
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
                if (column.is_array)
                {
                    var common_func = (string str) =>
                    {
                        writer.WriteLineEx($"{name}: {{");
                        writer.WriteLineEx($"let size = reader.read_i32()? as usize;");
                        writer.WriteLineEx($"std::iter::repeat_with(|| {str})");
                        writer.WriteLineEx($".take(size)");
                        writer.WriteLineEx($".collect()");
                        writer.WriteLineEx($"}},");
                    };
                    switch(column.base_type) 
                    {
                        case BaseType.Boolean: common_func("reader.read_bool().unwrap()"); break;
                        case BaseType.Int8 : common_func("reader.read_i8().unwrap()"); break;
                        case BaseType.Int16 : common_func("reader.read_i16().unwrap()"); break;
                        case BaseType.Int32 : common_func("reader.read_i32().unwrap()"); break;
                        case BaseType.Int64 : common_func("reader.read_i64().unwrap()"); break;
                        case BaseType.Float : common_func("reader.read_f32().unwrap()"); break;
                        case BaseType.Double : common_func("reader.read_f64().unwrap()"); break;
                        case BaseType.String : common_func("super::read_string(reader).unwrap()"); break;
                        case BaseType.Vector3 : common_func("glam::f64::DVec3::new(reader.read_f64()?,reader.read_f64()?,reader.read_f64()?)"); break;
                        case BaseType.Vector2 : common_func("glam::f64::DVec2::new(reader.read_f64()?,reader.read_f64()?)"); break;
                        case BaseType.Enum : 
                            writer.WriteLineEx($"{name}: {{");
                            writer.WriteLineEx($"let size = reader.read_i32()? as usize;");
                            writer.WriteLineEx($"std::iter::repeat_with(|| {{");
                            writer.WriteLineEx($"{column.type_name}::from_i32(reader.read_i32().unwrap_or_default()).unwrap()");
                            writer.WriteLineEx($"}})");
                            writer.WriteLineEx($".take(size)");
                            writer.WriteLineEx($".collect()");
                            writer.WriteLineEx($"}},");
                            break;
                        case BaseType.DateTime:
                        case BaseType.TimeSpan:
                        case BaseType.Struct:
                            writer.WriteLineEx($"unimplemented!($\"{column.base_type} array\"),");  
                            break;
                    }
                }
                else
                {
                    switch(column.base_type)  
                    {
                       case BaseType.Boolean: writer.WriteLineEx($"{name}: reader.read_bool()?,"); break;
                       case BaseType.Int8: writer.WriteLineEx($"{name}: reader.read_i8()?,"); break;
                       case BaseType.Int16 : writer.WriteLineEx($"{name}: reader.read_i16()?,"); break;
                       case BaseType.Int32 : writer.WriteLineEx($"{name}: reader.read_i32()?,"); break;
                       case BaseType.Int64 : writer.WriteLineEx($"{name}: reader.read_i64()?,"); break;
                       case BaseType.Float : writer.WriteLineEx($"{name}: reader.read_f32()?,"); break;
                       case BaseType.Double : writer.WriteLineEx($"{name}: reader.read_f64()?,"); break;
                       case BaseType.String : writer.WriteLineEx($"{name}: super::read_string(reader)?,"); break;
                       case BaseType.Vector3 :
                           writer.WriteLineEx($"{name}: glam::f64::DVec3::new(");
                           writer.WriteLineEx($"reader.read_f64()?,");
                           writer.WriteLineEx($"reader.read_f64()?,");
                           writer.WriteLineEx($"reader.read_f64()?,");
                           writer.WriteLineEx($"),");
                           break;
                       case BaseType.Vector2 : 
                           writer.WriteLineEx($"{name}: glam::f64::DVec2::new(");
                           writer.WriteLineEx($"reader.read_f64()?,");
                           writer.WriteLineEx($"reader.read_f64()?,");
                           writer.WriteLineEx($"),");
                           break;
                       case BaseType.Enum:
                           writer.WriteLineEx($"{name}: {{");
                           writer.WriteLineEx($"let v = {column.type_name}::from_i32(reader.read_i32().unwrap_or_default());");
                           writer.WriteLineEx($"if v.is_none() {{");
                           writer.WriteLineEx($"return Err(anyhow::anyhow!(\"{column.type_name} is none\"));");
                           writer.WriteLineEx($"}}");
                           writer.WriteLineEx($"v.unwrap()");
                           writer.WriteLineEx($"}},");
                           break;
                       case BaseType.DateTime:
                       case BaseType.TimeSpan:
                       case BaseType.Struct:
                           writer.WriteLineEx($"unimplemented!($\"{column.base_type}\"),");  
                           break;
                    }
                }
            }
            writer.WriteLineEx($"}};");
            writer.WriteLineEx($"map.insert(v.{firstColumnName}, v.clone());");
            writer.WriteLineEx($"vec.push(v);");
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"Ok((vec, map))");
            writer.WriteLineEx($"}}");
        }
        private void EGuiGen(string sheetName, IndentedTextWriter writer, List<Column> columns)
        {
            var firstColumn = columns.FirstOrDefault(t => t.is_key);
            var firstColumnType = firstColumn.GenerateType(_gen_type);
            var firstColumnName = firstColumn.var_name;
            writer.WriteLineEx($"#[cfg(feature = \"egui\")]");
            writer.WriteLineEx($"fn egui_header() -> fn(egui_extras::TableRow) {{");
            writer.WriteLineEx($"|mut header| {{");
            int arrayCount = 0;
            foreach (var column in columns)
            {
                string name = column.var_name;
                if (column.is_generated == false || column.array_index > 0)
                {
                    continue;
                }

                writer.WriteLineEx($"let _ = header.col(|ui| {{");
                writer.WriteLineEx($"let _ = ui");
                writer.WriteLineEx($"  .strong(\"{name}\")");
                writer.WriteLineEx($"  .on_hover_text(\"{column.desc}\");");
                writer.WriteLineEx($"}});");
                arrayCount++;
            }
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"fn column_count() -> usize {{");
            writer.WriteLineEx($"{arrayCount}");
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"#[cfg(feature = \"egui\")]");
            writer.WriteLineEx($"fn egui_body(body: egui_extras::TableBody, items: Vec<&Self>) {{");
            writer.WriteLineEx($"body.heterogeneous_rows(");
            writer.WriteLineEx($"(0..items.len()).into_iter().map(|_| 18f32),");
            writer.WriteLineEx($"|row_index, mut row| {{");
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
                        GenEGuiBodyArray(writer, column);
                    }
                }
                else
                {
                    GenEGuiBody(writer, column);
                }
            }
            writer.WriteLineEx($"}},");
            writer.WriteLineEx($");");
            writer.WriteLineEx($"}}");
        }
        private void GenEGuiBody(IndentedTextWriter writer, Column column)
        {
            string name = column.var_name;
            writer.WriteLineEx($"let _ = row.col(|ui| {{");
            
            if (column.IsEnumType())
            {
                writer.WriteLineEx($"let _ = ui.label(format!(\"{{:?}}\", item.{name}));");
            }
            else if (column.IsNumberType() || column.base_type == BaseType.Boolean)
            {
                writer.WriteLineEx($"let _ = ui.label(item.{name}.to_string());");
            }
            else if (column.base_type == BaseType.String)
            {
                writer.WriteLineEx($"let _ = ui.label(&item.{name});");
            }
            else if (column.base_type is BaseType.Vector3 or BaseType.Vector2)
            {
                writer.WriteLineEx($"let _ = ui.label(item.{name});");
            }
            writer.WriteLineEx($"}});");
        }
        private void GenEGuiBodyArray(IndentedTextWriter writer, Column column)
        {
            string name = column.var_name;
            writer.WriteLineEx($"let _ = row.col(|ui| {{");
            writer.WriteLineEx($"let _ = ui.label(format!(\"{{:?}}\", item.{name}));");
            writer.WriteLineEx($"}});");
        }
        private string GenToString(Column column)
        {
            string ret = "";
            string name = column.var_name;
            if (column.IsEnumType())
            {
                ret = $"format!(\"{{:?}}\",self.{name}))";
            }
            else if (column.IsNumberType() || column.base_type == BaseType.Boolean)
            {
                ret = $"self.{name}.to_string()";
            }
            else if (column.base_type == BaseType.String)
            {
                ret = $"self.{name}.clone()";
            }
            else if (column.base_type is BaseType.Vector3 or BaseType.Vector2)
            {
                ret = $"format!(\"{{:?}}\",self.{name}))";
            }
            return ret;
        }
    }
}
