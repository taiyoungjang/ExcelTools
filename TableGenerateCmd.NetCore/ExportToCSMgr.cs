using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;
using TableGenerateCmd;

namespace TableGenerate
{
    public class ExportToCSMgr : ExportBase
    {
        private readonly string _unityDefine = "UNITY_2018_2_OR_NEWER";

        public static string NameSpace = string.Empty;
        public int m_current = 0;
        public eGenType _gen_type = eGenType.cs;
        private string _async = string.Empty;
        public string SetAsync
        {
            //set { _async = "unity3d"; }
            set { _async = value; }
        }

        public ExportToCSMgr(string unityDefine)
        {
            _unityDefine = unityDefine;
        }

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".cs");

            using(var stream = new MemoryStream())
            {
                var writer = new IndentedTextWriter(new StreamWriter(stream, Encoding.UTF8), " ");
                {
                    string filename = System.IO.Path.GetFileName(createFileName);
                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace(".cs", string.Empty);
                    max = sheets.GetLength(0);

                    writer.WriteLineEx( "#pragma warning disable IDE0007, IDE0011, IDE0025, IDE1006, IDE0018");

                    if (_async == "unity3d")
                    {
                        writer.WriteLineEx("#if UNITY_EDITOR");
                        writer.WriteLineEx($"[System.CodeDom.Compiler.GeneratedCode(\"TableGenerateCmd\",\"1.0.0\")]");
                        writer.WriteLineEx($"public class {filename}Manager : UnityEngine.MonoBehaviour");
                        writer.WriteLineEx("{");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            writer.WriteLineEx($"public {NameSpace}.{filename}.{trimSheetName}[] {trimSheetName} = null;");
                        }
                        writer.WriteLineEx("}");

                        writer.WriteLineEx($"[UnityEditor.CustomEditor(typeof({filename}Manager))]");
                        writer.WriteLineEx($"public class {filename}Editor : UnityEditor.Editor");
                        writer.WriteLineEx("{");
                        writer.WriteLineEx($"public override void OnInspectorGUI()");
                        writer.WriteLineEx("{");
                        writer.WriteLineEx($"{filename}Manager myScript = ({filename}Manager) target;");
                        writer.WriteLineEx("if(UnityEngine.GUILayout.Button(\"Load\"))");
                        writer.WriteLineEx("{");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            writer.WriteLineEx($"myScript.{trimSheetName} = {NameSpace}.{filename}.{trimSheetName}.Array_;");
                        }
                        writer.WriteLineEx("}");
                        //_writer.WriteLineEx("if(UnityEngine.GUILayout.Button(\"Save\"))");
                        //_writer.WriteLineEx("{");
                        //_writer.WriteLineEx("}");
                        writer.WriteLineEx("DrawDefaultInspector();");
                        writer.WriteLineEx("}");
                        writer.WriteLineEx("}");

                        writer.WriteLineEx("#endif");
                        writer.WriteLineEx();
                    }

                    // Init class
                    writer.WriteLineEx($"namespace {NameSpace}.{filename}");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("using System.Linq;");
                    writer.WriteLineEx($"[System.CodeDom.Compiler.GeneratedCode(\"TableGenerateCmd\",\"1.0.0\")]");
                    writer.WriteLineEx( "public class Loader : ILoader");
                    writer.WriteLineEx( "{");
                    
                    writer.WriteLineEx($"#if !{_unityDefine}");
                    /*
                    writer.WriteLineEx("public System.Data.DataSet DataSet");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("get");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx($"var dts = new System.Data.DataSet(nameof({filename}));");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        writer.WriteLineEx($"{trimSheetName}.GetDataTable(dts);");
                    }
                    writer.WriteLineEx("return dts;");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("set");
                    writer.WriteLineEx("{");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        writer.WriteLineEx($"{trimSheetName}.SetDataSet(value);");
                    }
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("}");
                    */
                    // ExcelLoad function
                    writer.WriteLineEx("#if !NO_EXCEL_LOADER");
                    writer.WriteLineEx("public void ExcelLoad(string path, string language, string dataStage)");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("System.Action<string> excelAction = excelName =>");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("language = language.Trim();");
                    writer.WriteLineEx("string directoryName = System.IO.Path.GetDirectoryName(path);");
                    writer.WriteLineEx("var imp = new ClassUtil.ExcelImporter();");
                    writer.WriteLineEx("imp.Open(path);");
                    writer.WriteLineEx("switch (excelName)");
                    writer.WriteLineEx("{");
                    var sheetNames = sheets.Select(sheetName => sheetName.Trim().Replace(" ", "_"));
                    foreach (string sheetName in sheetNames)
                    {
                        writer.WriteLineEx($@"case ""{sheetName}"":{sheetName}.ExcelLoad( imp, directoryName, language, dataStage);");
                        writer.WriteLineEx("break;");
                    }
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("imp.Dispose();");
                    writer.WriteLineEx("};");
                    writer.WriteLineEx($"System.Threading.Tasks.Parallel.ForEach(new string[]{{{(string.Join(',', sheetNames.Select(t => $@"""{t}""")))}}},excelAction);");
                    writer.WriteLineEx("}");

                    writer.WriteLineEx("#endif");
                    writer.WriteLineEx("#endif");

                    // GetStaticObject function
                    writer.WriteLineEx("/*");
                    writer.WriteLineEx("public void GetMapAndArray(System.Collections.Immutable.ImmutableDictionary<string,object> container)");
                    writer.WriteLineEx("{");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var key_column = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except).FirstOrDefault( t => t.is_key);
                        var type = key_column.GenerateType(_gen_type);
                        var equality_type = key_column.GetEqualityTypeValue(_gen_type);
                        writer.WriteLineEx($"container.Remove( \"{filename}.{trimSheetName}.map_\");");
                        writer.WriteLineEx($"container.Remove( \"{filename}.{trimSheetName}.array_\");");
                        writer.WriteLineEx($"container.Add( \"{filename}.{trimSheetName}.map_\", new System.Collections.Immutable.ImmutableDictionary<{type},{trimSheetName}>({trimSheetName}.map_{(string.IsNullOrEmpty(equality_type)?string.Empty:$",{equality_type}")}));");
                        writer.WriteLineEx($"container.Add( \"{filename}.{trimSheetName}.array_\",{trimSheetName}.array_);");
                    }
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("*/");

                    // WriteFile function
                    writer.WriteLineEx("#if !NO_EXCEL_LOADER");
                    writer.WriteLineEx("public void WriteFile(string path, bool usingPerforce)");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("int uncompressedLength = 0;");
                    writer.WriteLineEx("int compressedLength = 0;");
                    writer.WriteLineEx("System.IO.MemoryStream ms = null;");
                    writer.WriteLineEx("ms = new System.IO.MemoryStream(128 * 1024);");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("var uncompressedMemoryStreamWriter = new System.IO.BinaryWriter(ms);");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        writer.WriteLineEx($"{trimSheetName}.WriteStream(uncompressedMemoryStreamWriter);");
                    }
                    writer.WriteLineEx("uncompressedLength = (int) ms.Position;");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("System.IO.FileStream stream = null;");
                    writer.WriteLineEx("try");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("string tempFileName = System.IO.Path.GetTempFile" +
                        "Name();");
                    writer.WriteLineEx("ms.Position=0;");
                    writer.WriteLineEx("stream = new System.IO.FileStream(tempFileName, System.IO.FileMode.Create);");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("using System.IO.MemoryStream __zipMs = new System.IO.MemoryStream();");
                    writer.WriteLineEx("using( Ionic.Zlib.ZlibStream zip = new Ionic.Zlib.ZlibStream(__zipMs, Ionic.Zlib.CompressionMode.Compress, true))");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("zip.Write(ms.ToArray(),0,uncompressedLength);");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("using var md5 = System.Security.Cryptography.MD5.Create();");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("var __compressed = __zipMs.ToArray();");
                    writer.WriteLineEx("compressedLength = __compressed.Length;");
                    writer.WriteLineEx("byte[] hashBytes = md5.ComputeHash(__compressed);");
                    writer.WriteLineEx("stream.WriteByte((byte)hashBytes.Length);");
                    writer.WriteLineEx("stream.Write(hashBytes, 0, hashBytes.Length);");
                    writer.WriteLineEx("stream.Write( System.BitConverter.GetBytes(uncompressedLength), 0, 4 );");
                    writer.WriteLineEx("stream.Write( System.BitConverter.GetBytes(compressedLength), 0, 4 );");
                    writer.WriteLineEx("stream.Write(__compressed, 0, __compressed.Length);");
                    writer.WriteLineEx("}");

                    writer.WriteLineEx("}");

                    writer.WriteLineEx("stream.Flush();");
                    writer.WriteLineEx("stream.Close();");
                    writer.WriteLineEx("stream = null;");
                    writer.WriteLineEx("using var file  = new System.IO.FileStream(tempFileName, System.IO.FileMode.Open);");
                    writer.WriteLineEx("ms.Position=0;");
                    writer.WriteLineEx("ms.SetLength(file.Length);");
                    writer.WriteLineEx("file.CopyTo(ms);");
                    writer.WriteLineEx($"TBL.FileExtensions.CheckReplaceFile(ms, System.IO.Path.GetDirectoryName( path + \"/\") + \"/{filename}.bytes\", usingPerforce);");
                    writer.WriteLineEx("}catch(System.Exception e)");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("System.Console.WriteLine(e.ToString());");
                    writer.WriteLineEx("throw;");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("finally");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("ms?.Dispose();");
                    writer.WriteLineEx("}");

                    writer.WriteLineEx("}");
                    writer.WriteLineEx("#endif //NO_EXCEL_LOADER");


                    writer.WriteLineEx($"public string GetFileName() => \"{filename}\";");
                    // ReadStream function
                    writer.WriteLineEx( "public void ReadStream(System.IO.Stream stream)");
                    writer.WriteLineEx( "{");
                    writer.WriteLineEx( "stream.Position = 0;");
                    //if(sheets.Any())
                    //{
                    //    _writer.WriteLineEx("if(" + string.Join( "\r\n      || ", sheets.Select(sheetName => $"communicator.findObjectFactory({ExportToIceCSMgr.NameSpace}.{filename}.{sheetName.Trim().Replace(" ", "_")}.ice_staticId()) == null")) + ")");
                    //    _writer.WriteLineEx("{");
                    //    foreach (string sheetName in sheets)
                    //    {
                    //        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                    //        string classNameType = $"::{ExportToIceCSMgr.NameSpace}::{filename}::{trimSheetName}";
                    //        string classNameValue = $"{ExportToIceCSMgr.NameSpace}.{filename}.{trimSheetName}";
                    //        _writer.WriteLineEx($"  communicator.addObjectFactory(ObjectFactoryI.Instance,{classNameValue}.ice_staticId());");
                    //    }
                    //    _writer.WriteLineEx("}");
                    //}
                    writer.WriteLineEx("var streamLength = (int)stream.Length;");
                    writer.WriteLineEx("var hashLength = stream.ReadByte();");
                    writer.WriteLineEx("var uncompressedSize = new byte[4];");
                    writer.WriteLineEx("var compressedSize = new byte[4];");
                    writer.WriteLineEx("var hashBytes = new byte[hashLength];");
                    writer.WriteLineEx("stream.Read( hashBytes, 0, hashLength);");
                    writer.WriteLineEx("var bytes = new byte[streamLength-hashLength-compressedSize.Length-uncompressedSize.Length-1];");
                    writer.WriteLineEx("stream.Read( uncompressedSize, 0, uncompressedSize.Length);");
                    writer.WriteLineEx("stream.Read( compressedSize, 0, compressedSize.Length);");
                    writer.WriteLineEx("stream.Read( bytes, 0, streamLength-hashLength-compressedSize.Length-uncompressedSize.Length-1);");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("using var md5 = System.Security.Cryptography.MD5.Create();");
                    writer.WriteLineEx("var dataBytes = md5.ComputeHash(bytes);");
                    writer.WriteLineEx("if(!System.Linq.Enumerable.SequenceEqual(hashBytes, dataBytes))");
                    writer.WriteLineEx($"  {{throw new System.Exception(\"{filename} verify failure...\");}}");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("using var __ms = new System.IO.MemoryStream(bytes);");
                    writer.WriteLineEx("using var decompressStream = new Ionic.Zlib.ZlibStream(__ms, Ionic.Zlib.CompressionMode.Decompress, Ionic.Zlib.CompressionLevel.Default, true);");
                    writer.WriteLineEx("var uncompressedSize__ = System.BitConverter.ToInt32(uncompressedSize,0);");
                    writer.WriteLineEx("bytes = new byte[uncompressedSize__];");
                    writer.WriteLineEx("decompressStream.Read(bytes, 0, uncompressedSize__);");
                    writer.WriteLineEx("}");

                    writer.WriteLineEx("{");
                    writer.WriteLineEx("System.IO.MemoryStream __ms = null;");
                    writer.WriteLineEx("try");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("__ms = new System.IO.MemoryStream(bytes);");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("using var reader = new System.IO.BinaryReader(__ms);");
                    writer.WriteLineEx("__ms = null;");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        writer.WriteLineEx($"{trimSheetName}.ReadStream(reader);");
                    }
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("finally");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("__ms?.Dispose();");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("}");
                    // Get Hash FUnction
                    writer.WriteLineEx("public byte[] GetHash(System.IO.Stream stream)");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("stream.Position = 0;");
                    writer.WriteLineEx("var hashLength = stream.ReadByte();");
                    writer.WriteLineEx("var hashBytes = new byte[hashLength];");
                    writer.WriteLineEx("stream.Read( hashBytes, 0, hashLength);");
                    writer.WriteLineEx("return hashBytes;");
                    writer.WriteLineEx("}");

                    writer.WriteLineEx("}");

                    current = 0;
                    foreach (string sheetName in sheets)
                    {
                        current++;
                        m_current = current - 1; 
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        SheetProcess(writer, filename, trimSheetName, columns);
                    }

                    writer.WriteLineEx("}");
                    writer.Flush();
                }
                string tempCreateFileName = createFileName.Replace(".cs", "Manager.cs");
                ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{tempCreateFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
            }

            return true;
        }

        private void SheetProcess(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key);
            var type = keyColumn.GenerateType(_gen_type);
            var equalityType = keyColumn.GetEqualityTypeValue(_gen_type);
            writer.WriteLineEx();
            if (_async == "unity3d")
            {
                writer.WriteLineEx($"#if !ENCRYPT");
                writer.WriteLineEx($"[System.Serializable]");
                writer.WriteLineEx($"#endif");
            }
            string primitiveName = keyColumn.var_name;
            writer.WriteLineEx($"public partial class {sheetName}");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"private static {sheetName}[] array_;");
            writer.WriteLineEx($"public static {sheetName}[] Array_");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"get");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"if (array_ != null) return array_;");
            writer.WriteLineEx($"if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception(\"TBL.Base_.Path or TBL.Base_.Language is empty\");");
            writer.WriteLineEx($"Loader loader = new();");
            writer.WriteLineEx($"      var path = $\"{{Base_.Path}}/ScriptTable/{{Base_.Language}}/{{loader.GetFileName()}}.bytes\";");
            writer.WriteLineEx($"using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);");
            writer.WriteLineEx($"loader.ReadStream(stream);");
            writer.WriteLineEx($"return array_;");
            writer.WriteLineEx("}");
            writer.WriteLineEx("}");
            writer.WriteLineEx($"private static System.Collections.Immutable.ImmutableDictionary<{type},{sheetName}> map_;");
            writer.WriteLineEx($"public static System.Collections.Immutable.ImmutableDictionary<{type},{sheetName}> Map_");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"get");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"if (map_ != null) return map_;");
            writer.WriteLineEx($"if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception(\"TBL.Base_.Path or TBL.Base_.Language is empty\");");
            writer.WriteLineEx($"Loader loader = new();");
            writer.WriteLineEx($"      var path = $\"{{Base_.Path}}/ScriptTable/{{Base_.Language}}/{{loader.GetFileName()}}.bytes\";");
            writer.WriteLineEx($"using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);");
            writer.WriteLineEx($"loader.ReadStream(stream);");
            writer.WriteLineEx($"return map_;");
            writer.WriteLineEx("}");
            writer.WriteLineEx("}");
            writer.WriteLineEx("public object[] DataRow_ => new object[]{");
            {
                bool bFirst = true;
                foreach (var column in columns.Where(compare => compare.array_index <= 0))
                {
                    if (column.is_generated == false)
                    {
                        continue;
                    }

                    string delim = string.Empty;


                    if (column.array_index >= 0)
                    {
                        int arrayCount = columns.Where(compare => compare.var_name == column.var_name).Max(compare => compare.array_index);
                        for (int i = 0; i <= arrayCount; i++)
                        {
                            if( i > 0 || bFirst == false)
                            {
                                delim = ",";
                            }
                            else
                            {
                                delim = string.Empty;
                            }
                            writer.WriteLineEx($"{delim}{column.var_name}[{i}]");
                            bFirst = false;
                        }
                    }
                    else
                    {
                        delim = bFirst == false ? "," : string.Empty;
                        writer.WriteLineEx($"{delim}{column.var_name}");
                        bFirst = false;
                    }
                }
            }
            writer.WriteLineEx("};");
            writer.WriteLineEx($"public static void ArrayToMap({sheetName}[] array__)");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"var map__ = new System.Collections.Generic.Dictionary<{type},{sheetName}> (array__.Length);");
            //_writer.WriteLineEx($"array.Sort(delegate({sheetName} a,{sheetName} b)");
            //_writer.WriteLineEx( "{");
            //_writer.WriteLineEx($"return a.{primitiveName}.CompareTo(b.{primitiveName});");
            //_writer.WriteLineEx("});");
            writer.WriteLineEx("var __i=0;");
            writer.WriteLineEx($"try{{");
            writer.WriteLineEx("for(__i=0;__i<array__.Length;__i++)");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"var __table = array__[__i];");
            writer.WriteLineEx($"map__.Add(__table.{primitiveName}, __table);");
            writer.WriteLineEx( "}");
            writer.WriteLineEx($"}}catch(System.Exception e)");
            writer.WriteLineEx($"{{");
            writer.WriteLineEx($"      throw new System.Exception($\"Row:{{__i}} {{e.Message}}\");");
            writer.WriteLineEx($"}}");
            writer.WriteLineEx($"map_ = System.Collections.Immutable.ImmutableDictionary<{type},{sheetName}>.Empty.AddRange(map__);");
            if(!string.IsNullOrEmpty(equalityType))
            {
                writer.WriteLineEx($"map_ = map_.WithComparers({equalityType});");
            }
            writer.WriteLineEx( "}");
            writer.WriteLineEx();
            WriteStreamFunction(writer, filename, sheetName,columns);
            writer.WriteLineEx($"#if !{_unityDefine}");
            //SetDataTableFunction(writer, filename, sheetName, columns);
            writer.WriteLineEx("#if !NO_EXCEL_LOADER");
            ExcelLoadFunction(writer, filename, sheetName, columns);
            writer.WriteLineEx("#endif");
            //GetDataTableFunction(writer, filename, sheetName, columns);
            writer.WriteLineEx("#endif");
            ReadStreamFunction(writer, filename, sheetName, columns);
            ArrayCountFunction(writer, filename, sheetName, columns);
            //SheetConstructorProcess(_writer, sheetName, columns);
            if (_async == "unity3d")
            {
                //PropertyFunction(_writer, filename, sheetName, columns);
                //_writer.WriteLineEx($"#if {UNITY_DEFINE}");
                //WhereFunctionCoroutine(filename, sheetName, columns);
                //WhereStringFunctionCoroutine(filename, sheetName, columns);
                //_writer.WriteLineEx("#endif");
            }
            else 
            {
                //WhereFunction(filename, sheetName, columns);
                //WhereStringFunction(filename, sheetName, columns);
                //WhereIsInStringFunction(filename, sheetName, columns);
                //WhereNotInStringFunction(filename, sheetName, columns);
            }
            writer.WriteLineEx("}");
        }
        private void SheetConstructorProcess(IndentedTextWriter _writer, string sheetName, List<Column> columns)
        {
            //if (_async == "unity3d")
            //{
            //    string arr = string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.GenerateType(_gen_type)} {t.var_name}").ToArray());
            //    _writer.WriteLineEx($"public {sheetName} ({arr})");
            //    _writer.WriteLineEx("{");
            //    foreach (var column in columns)
            //    {
            //        string type = column.GenerateType(_gen_type);
            //        if (column.is_generated == false)
            //        {
            //            continue;
            //        }
            //        if (column.array_index == 0)
            //        {
            //            _writer.WriteLineEx($"{column.var_name}__prop = {column.var_name};");
            //        }
            //        else if(column.array_index == -1)
            //        {
            //            if (
            //                column.base_type != eBaseType.Int32 &&
            //                column.base_type != eBaseType.Int16 &&
            //                column.base_type != eBaseType.Int8 &&
            //                column.base_type != eBaseType.Int64 &&
            //                column.base_type != eBaseType.String)
            //            {
            //                _writer.WriteLineEx($"{column.var_name}__prop = {column.var_name};");
            //            }
            //            else
            //            {
            //                //_writer.WriteLineEx($"{column.var_name}__prop = Utility.Encrypt({column.var_name});");
            //                _writer.WriteLineEx($"{column.var_name}__prop = {column.var_name};");
            //            }
            //        }
            //    }
            //    _writer.WriteLineEx("}");
            //}
            //else
            {
                _writer.WriteLineEx(string.Format("public {0} ({1}) : base({2}){{}}",
                    sheetName,
                    string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.GenerateType(_gen_type)} {t.var_name}").ToArray()),
                    string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.var_name}").ToArray())
                    ));
            }
        }

        private void ExcelLoadFunction(IndentedTextWriter writer, string filename, string sheetName,
            List<Column> columns)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key == true);
            var dataStageColumn = columns.FirstOrDefault(compare => compare.var_name.Trim().ToLower() == "datastage");

            writer.WriteLineEx(
                "public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language, string dataStage)");
            writer.WriteLineEx("{");
            if (dataStageColumn != null)
            {
                writer.WriteLineEx($"dataStage = dataStage.Length == 0 ? ({dataStageColumn.max_value}).ToString() : dataStage;");
                writer.WriteLineEx($"var dataStageEnum__ = System.Enum.Parse<{dataStageColumn.type_name}>( dataStage, ignoreCase: true );");
            }
            writer.WriteLineEx("var i=0; var j=0;");
            writer.WriteLineEx("TableGenerateCmd.StringWithDesc[,] rows = null;");
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

                writer.WriteLineEx($"{type} {name};");
            }

            writer.WriteLineEx("try");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"rows = imp.GetSheet(\"{sheetName}\", language);");
            writer.WriteLineEx($"var list__ = new System.Collections.Generic.List<{sheetName}>(rows.GetLength(0) - 3);");
            writer.WriteLineEx("for (i = 3; i < rows.GetLength(0); i++)");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"j=0;");
            writer.WriteLineEx("if(rows[i,0].Text.Length == 0) break;");
            if (dataStageColumn != null)
            {
                writer.WriteLineEx($"var dataStageText = rows[i,{dataStageColumn.data_column_index}].Text.Trim();");
                writer.WriteLineEx($"  if( !string.IsNullOrEmpty(dataStageText) && (dataStageText.Split('|').Select(x__=>(int)System.Enum.Parse<{dataStageColumn.type_name}>(x__, ignoreCase: true)).Sum() & (int)dataStageEnum__) != (int) dataStageEnum__ ) {{ continue;}}");
            }
            foreach (var column in columns)
            {
                string type = column.GenerateType(_gen_type);
                string arg = $"rows[i,{column.data_column_index}].Text";
                string convert_function = column.GetConvertFunction(arg,_gen_type);
                if (column.is_generated == false)
                {
                    continue;
                }
                if (column.array_index >= 0)
                {
                    if (column.array_index == 0)
                    {
                        int array_count = columns.Where(compare => compare.var_name == column.var_name).Count();
                        writer.WriteLineEx($"var {column.var_name}_list__ = new System.Collections.Generic.List<{column.GenerateBaseType(_gen_type)}>();");
                        writer.WriteLineEx($"bool not_empty_{column.var_name}__ = false;");
                    }

                    writer.WriteLineEx($"j = {column.data_column_index};");
                    writer.WriteLineEx($"if(!string.IsNullOrEmpty({arg}))");
                    writer.WriteLineEx($"{{");
                    if (column.array_index > 0)
                    {
                        writer.WriteLineEx($"if(not_empty_{column.var_name}__)");
                        writer.WriteLineEx($"{{");
                        writer.WriteLineEx($"    throw new System.Exception(string.Format(\"{{0}}{{1}} before is empty text\",ILoader.ColumnIndexToColumnLetter(j+1),i+1));");
                        writer.WriteLineEx($"}}");
                    }
                    writer.WriteLineEx($"var v_ = {convert_function};");
                    if (column.IsNumberType() && 
                        !string.IsNullOrEmpty(column.min_value) &&
                        !string.IsNullOrEmpty(column.max_value))
                    {
                        writer.WriteLineEx($"if( v_ < {column.min_value})");
                        writer.WriteLineEx($"{{");
                        writer.WriteLineEx($"    throw new System.Exception(string.Format(\"{{0}}{{1}} value:{{2}} < min:{column.min_value} \",ILoader.ColumnIndexToColumnLetter(j+1),i+1,v_));");
                        writer.WriteLineEx($"}}");
                        writer.WriteLineEx($"if( v_ > {column.max_value})");
                        writer.WriteLineEx($"{{");
                        writer.WriteLineEx($"    throw new System.Exception(string.Format(\"{{0}}{{1}} value:{{2}} > max:{column.max_value} \",ILoader.ColumnIndexToColumnLetter(j+1),i+1,v_));");
                        writer.WriteLineEx($"}}");
                    }
                    writer.WriteLineEx($"{column.var_name}_list__.Add(v_);");
                    writer.WriteLineEx($"}}");
                    writer.WriteLineEx($"else");
                    writer.WriteLineEx($"{{");
                    writer.WriteLineEx($"not_empty_{column.var_name}__ = true;");
                    writer.WriteLineEx($"}}");
                    if (column.is_last_array)
                    {
                        writer.WriteLineEx($"{column.var_name} = {column.var_name}_list__.ToArray();");
                    }
                    continue;
                }
                if (column.IsEnumType() && column.bit_flags)
                {
                    writer.WriteLineEx($"j = {column.data_column_index};");
                    writer.WriteLineEx($"    {{{column.var_name} = {arg}.Length == 0 ? {column.max_value} : ({column.type_name}) {arg}.Split('|').Select(x__ => (int) System.Enum.Parse<{column.GetPrimitiveType(_gen_type)}>(x__, ignoreCase: true)).Sum(); }}");
                }
                else if (column.IsDateTime() == true)
                {
                    writer.WriteLineEx($"j = {column.data_column_index};");
                    writer.WriteLineEx($"if(string.IsNullOrEmpty({arg})){{{column.var_name} = new System.DateTime(1970,1,1);}} else{{{column.var_name} = {convert_function};}}");
                }
                else if (column.IsTimeSpan() == true)
                {
                    writer.WriteLineEx($"j = {column.data_column_index};");
                    writer.WriteLineEx($"if(string.IsNullOrEmpty({arg})){{{column.var_name} = System.TimeSpan.FromTicks(0);}} else{{{column.var_name} = {convert_function};}}");
                }
                else if (column.IsNumberType() == true)
                {
                    writer.WriteLineEx($"j = {column.data_column_index};");
                    writer.WriteLineEx($"if(string.IsNullOrEmpty({arg}))");
                    writer.WriteLineEx( "{");
                    writer.WriteLineEx($"{column.var_name} = {column.GetInitValue(_gen_type)};}}else {{{column.var_name} = {convert_function};");
                    if (!string.IsNullOrEmpty(column.min_value) &&
                        !string.IsNullOrEmpty(column.max_value))
                    {
                        writer.WriteLineEx($"if( {column.var_name} < {column.min_value})");
                        writer.WriteLineEx($"{{");
                        writer.WriteLineEx($"    throw new System.Exception(string.Format(\"{{0}}{{1}} {{2}} < min:{column.min_value} \",ILoader.ColumnIndexToColumnLetter(j+1),i+1,{column.var_name}));");
                        writer.WriteLineEx($"}}");
                        writer.WriteLineEx($"if({column.var_name} > {column.max_value})");
                        writer.WriteLineEx($"{{");
                        writer.WriteLineEx($"    throw new System.Exception(string.Format(\"{{0}}{{1}} {{2}} > max:{column.max_value} \",ILoader.ColumnIndexToColumnLetter(j+1),i+1,{column.var_name}));");
                        writer.WriteLineEx($"}}");
                    }
                    
                    writer.WriteLineEx( "}");
                }
                else if (column.IsNumberType() == false)
                {
                    writer.WriteLineEx($"j = {column.data_column_index};");
                    writer.WriteLineEx($"{column.var_name} = {convert_function};");
                }
            }

            {
                var query = columns
                    .Where(t => !string.IsNullOrEmpty(t.array_group_name) && t.array_index == 0)
                    .GroupBy(t => t.array_group_name, t => t.array_group_name, (groupName, gns) =>
                   new
                   {
                       groupName,
                       Count = gns.Count()
                   });
                foreach (var item in query.Where(t => t.Count > 1))
                {
                    var arrs = columns.Where(t => t.array_group_name == item.groupName && t.array_index == 0).ToArray();
                    writer.WriteLineEx($"if({string.Join(" || ", arrs.Skip(1).Select(t => $" {arrs[0].var_name}.Length != {t.var_name}.Length"))})");
                    writer.WriteLineEx($"{{");
                    writer.WriteLineEx($"    throw new System.Exception(string.Format(\"mismatch group:{{0}}\",\"{item.groupName}\"));");
                    writer.WriteLineEx($"}}");
                }
            }

            writer.WriteLineEx(string.Format("  {0} values = new {0}{{ {1} }};", sheetName, string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.var_name}={t.var_name}").ToArray())));
            if (dataStageColumn != null)writer.WriteLineEx("bool needAdd = true;");
            writer.WriteLineEx("for (var idx_ = list__.Count-1; idx_ >= 0; --idx_)");
            writer.WriteLineEx( "{");
            writer.WriteLineEx( "var preValues = list__[idx_];");
            writer.WriteLineEx($"if (preValues.{keyColumn.var_name}.Equals({keyColumn.var_name}))");
            writer.WriteLineEx("{");
            if (dataStageColumn != null)
            {
                writer.WriteLineEx($"if ( (preValues.{dataStageColumn.var_name} & dataStageEnum__) == dataStageEnum__ )");
                writer.WriteLineEx("{");
                writer.WriteLineEx("needAdd = false;");
                writer.WriteLineEx("break;");
                writer.WriteLineEx("}");
                writer.WriteLineEx($"if ( (values.{dataStageColumn.var_name} & dataStageEnum__) == dataStageEnum__ )");
                writer.WriteLineEx("{");
                writer.WriteLineEx("list__.RemoveAt(idx_);");
                writer.WriteLineEx("idx_++;");
                writer.WriteLineEx("break;");
                writer.WriteLineEx("}");
            }
            writer.WriteLineEx($"throw new System.Exception(\"row:\" + i + \" {sheetName}.{keyColumn.var_name}:\" + preValues.{keyColumn.var_name}.ToString() + \") Duplicated!!\");");
            writer.WriteLineEx("}");
            writer.WriteLineEx( "}");

            if (dataStageColumn != null)writer.WriteLineEx("if(needAdd)");
            writer.WriteLineEx("list__.Add(values);");
            writer.WriteLineEx( "}");
            writer.WriteLineEx("array_ = list__.ToArray();");
            writer.WriteLineEx("ArrayToMap(array_);");
            writer.WriteLineEx( "}catch(System.Exception e)");
            writer.WriteLineEx( "{");
            writer.WriteLineEx("if( rows == null ) throw;");
            writer.WriteLineEx("if (j >= rows.GetLength(1))");
            writer.WriteLineEx($"  throw new System.Exception(\"sheet({sheetName}) invalid column count:\" + j);");
            writer.WriteLineEx($"throw new System.Exception(\" convert failure : excel({filename}).sheet({sheetName}) key:\" + rows[i,0].Text + \" Name:\" + rows[0,j].Text + \" \" + rows[i,j].Text + \" \" + e.Message );");
            writer.WriteLineEx( "}");
            writer.WriteLineEx( "}");
        }
        private void GetDataTableFunction(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key == true);
            var nationColumn = columns.FirstOrDefault(compare => compare.var_name.Trim().ToLower() == "nation");

            writer.WriteLineEx("public static void GetDataTable(System.Data.DataSet dts)");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"var table = dts.Tables.Add(nameof({sheetName}));");
            foreach (var column in columns.Where(compare => compare.array_index <= 0))
            {
                string type = column.GenerateBaseType(_gen_type);

                if (column.is_generated == false)
                {
                    continue;
                }

                if (column.array_index >= 0)
                {
                    int arrayCount = columns.Where(compare => compare.var_name == column.var_name).Max(compare => compare.array_index);
                    for (int i = 0; i <= arrayCount; i++)
                    {
                        writer.WriteLineEx($"table.Columns.Add(\"{column.var_name}{i}\", typeof({type}));");
                    }
                }
                else
                {
                    writer.WriteLineEx($"table.Columns.Add(nameof({column.var_name}), typeof({type}));");
                }
            }
            writer.WriteLineEx("foreach(var item__ in array_ )");
            writer.WriteLineEx("{");
            writer.WriteLineEx("table.Rows.Add(item__.DataRow_);");
            writer.WriteLineEx("}");
            writer.WriteLineEx("}");
        }
        private void ArrayCountFunction(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            foreach (var column in columns.Where(compare => compare.array_index <= 0))
            {
                string type = column.base_type.GenerateBaseType(_gen_type);

                if (column.is_generated == false)
                {
                    continue;
                }

                if (column.array_index < 0) continue;
                int arrayCount = columns.Count(compare => compare.var_name == column.var_name);
                writer.WriteLineEx($"public static int {column.var_name}_Length => {arrayCount};");
            }
        }
        private void SetDataTableFunction(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key == true);
            var keyType = keyColumn.GenerateType(_gen_type);
            var nationColumn = columns.FirstOrDefault(compare => compare.var_name.Trim().ToLower() == "nation");

            writer.WriteLineEx("public static bool SetDataSet(System.Data.DataSet dts)");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"if(dts?.Tables[nameof({sheetName})] is null) throw new System.Exception(\"dts.Tables['{sheetName}'] is null\");");
            writer.WriteLineEx($"var tables__ = dts.Tables[nameof({sheetName})];");
            writer.WriteLineEx($"if(tables__ is null) throw new System.Exception(\"dts.Tables['{sheetName}'] is null\");");
            writer.WriteLineEx($"map_ = System.Collections.Immutable.ImmutableDictionary<{keyType},{sheetName}>.Empty;");
            writer.WriteLineEx($"array_ = new {sheetName}[tables__.Rows.Count];");
            writer.WriteLineEx($"var row__ = 0;");
            writer.WriteLineEx($"foreach (System.Data.DataRow row in tables__.Rows)");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"var table__ = new {sheetName}");
            writer.WriteLineEx("(");
            bool bFirst = true;
            foreach (var column in columns.Where(compare => compare.array_index <= 0))
            {
                string type = column.base_type.GenerateBaseType(_gen_type);
                string generateType = column.GenerateType(_gen_type);
                string append = string.Empty;

                if (column.is_generated == false)
                {
                    continue;
                }
                append = bFirst == false ? "," : "";
                if (column.array_index >= 0)
                {
                    int arrayCount = columns.Where(compare => compare.var_name == column.var_name).Max(compare => compare.array_index);
                    for (int i = 0; i <= arrayCount; i++)
                    {
                        string arg = $"(row[\"{column.var_name}{i}\"].ToString() ?? string.Empty)";
                        string convertFunction = column.GetConvertFunction(arg, _gen_type);
                        if (i == 0)
                        {
                            writer.WriteLineEx($"{append}new {generateType}");
                            append = ",";
                            writer.WriteLineEx("{");
                            writer.WriteLineEx($"{convertFunction}");
                        }
                        else
                        {
                            writer.WriteLineEx($",{convertFunction}");
                        }
                    }
                    writer.WriteLineEx("}");
                }
                else
                {
                    string arg = $"(row[nameof({column.var_name})].ToString() ?? string.Empty)";
                    string convertFunction = column.GetConvertFunction(arg, _gen_type);
                    writer.WriteLineEx($"{append}{convertFunction}");
                }
                bFirst = false;
            }
            writer.WriteLineEx(");");
            writer.WriteLineEx($"map_ = map_.Add(table__.{keyColumn.var_name}, table__);");
            writer.WriteLineEx($"array_[row__++] = table__;");
            writer.WriteLineEx("}");
            writer.WriteLineEx("return true;");
            writer.WriteLineEx("}");
        }
        private void WriteStreamFunction(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            writer.WriteLineEx("public static void WriteStream(System.IO.BinaryWriter __writer)");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"__writer.Write(array_.Length);");
            writer.WriteLineEx($"foreach (var __table in array_)");
            writer.WriteLineEx( "{");
            foreach (var column in columns.Where( t => t.array_index <= 0))
            {
                string type = column.GenerateType(_gen_type);

                if (column.is_generated == false)
                {
                    continue;
                }

                if (column.array_index >= 0)
                {
                    writer.WriteLineEx($"Encoder.Write7BitEncodedInt(__writer,__table.{column.var_name}.Length);");
                    writer.Write($"foreach(var t__ in __table.{column.var_name})");
                    writer.Write("{");
                    if (column.IsDateTime())
                        writer.Write($"__writer.Write(t__.ToBinary());");
                    else if (column.IsTimeSpan())
                        writer.Write($"__writer.Write(t__.Ticks);");
                    else if (column.IsVector3())
                    {
                        writer.Write($"__writer.Write(t__.X);");
                        writer.Write($"__writer.Write(t__.Y);");
                        writer.Write($"__writer.Write(t__.Z);");
                    }   
                    else if (column.IsEnumType() || column.IsStructType())
                    {
                        string primitiveType = column.primitive_type.GenerateBaseType(_gen_type);
                        writer.Write($"__writer.Write(({primitiveType})t__);");
                    }
                    else if(column.IsString())
                    {
                        writer.Write($"Encoder.Write(__writer,t__);");
                    }
                    else
                    {
                        writer.Write($"__writer.Write(t__);");
                    }
                    writer.Write("}");
                    writer.WriteLineEx("");
                }
                else
                {
                    if (column.IsDateTime())
                        writer.WriteLineEx($"__writer.Write(__table.{column.var_name}.ToBinary());");
                    else if (column.IsVector3())
                        writer.WriteLineEx($"__writer.Write(__table.{column.var_name}.X);__writer.Write(__table.{column.var_name}.Y);__writer.Write(__table.{column.var_name}.Z);");
                    else if (column.IsTimeSpan())
                        writer.WriteLineEx($"__writer.Write(__table.{column.var_name}.Ticks);");
                    else if (column.IsEnumType() || column.IsStructType())
                    {
                        string primitiveType = column.primitive_type.GenerateBaseType(_gen_type);
                        writer.WriteLineEx($"__writer.Write(({primitiveType})__table.{column.var_name});");
                    }
                    else if (column.IsString())
                    {
                        writer.Write($"Encoder.Write(__writer,__table.{column.var_name});");
                    }
                    else
                    {
                        writer.WriteLineEx($"__writer.Write(__table.{column.var_name});");
                    }
                }
            }
            writer.WriteLineEx( "}");
            writer.WriteLineEx( "}");
        }
        private void ReadStreamFunction(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            writer.WriteLineEx("public static void ReadStream(System.IO.BinaryReader __reader)");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"var count__ = __reader.ReadInt32();");
            writer.WriteLineEx($"var array__ = new {sheetName}[count__];");
            writer.WriteLineEx($"for (var __i=0;__i<array__.Length;__i++)");
            writer.WriteLineEx("{");
            foreach (var column in columns.Where( t => t.array_index <= 0))
            {
                string type = column.GenerateType(_gen_type);
                string readStreamFunction = column.GetReadStreamFunction(_gen_type);

                if (column.is_generated == false)
                {
                    continue;
                }

                if (column.array_index >= 0)
                {
                    writer.WriteLineEx($"{type} {column.var_name};");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);");
                    writer.WriteLineEx($"{column.var_name} = arrayCount__ > 0?new {column.GenerateBaseType(_gen_type)}[arrayCount__]:System.Array.Empty<{column.GenerateBaseType(_gen_type)}>();");
                    writer.WriteLineEx($"for(var __j=0;__j<arrayCount__;++__j){column.var_name}[__j] = {readStreamFunction};");
                    writer.WriteLineEx("}");
                }
                else
                {
                    writer.WriteLineEx($"var {column.var_name} = {readStreamFunction};");
                }
            }
            writer.WriteLineEx(
                string.Format("var __table = new {0}(){{ {1} }};", 
                sheetName,
                string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.var_name}={t.var_name}").ToArray())
            ));
            writer.WriteLineEx($"array__[__i] = __table;");
            writer.WriteLineEx("}");
            writer.WriteLineEx($"array_ = array__;");
            writer.WriteLineEx($"ArrayToMap(array__);");
            writer.WriteLineEx("}");
        }
        private void PropertyFunction(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            //_writer.WriteLineEx($"#if {UNITY_DEFINE} && !DEBUG");
            foreach (var column in columns)
            {
                string type = column.GenerateType(_gen_type);
                if (column.is_generated == false)
                {
                    continue;
                }
                if (column.array_index == 0)
                {
                    writer.WriteLineEx($"private {type} {column.var_name}__prop;");
                    writer.WriteLineEx($"public override {type} {column.var_name}");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("get");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx($"return {column.var_name}__prop;");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("}");
                }
                else if(column.array_index == -1)
                {
                    writer.WriteLineEx($"private {type} {column.var_name}__prop;");
                    if (
                        column.base_type != eBaseType.Int32 &&
                        column.base_type != eBaseType.Int16 &&
                        column.base_type != eBaseType.Int8 &&
                        column.base_type != eBaseType.Int64 &&
                        column.base_type != eBaseType.String)
                    {
                        writer.WriteLineEx($"public override {type} {column.var_name}");
                        writer.WriteLineEx("{");
                        writer.WriteLineEx("get");
                        writer.WriteLineEx("{");
                        writer.WriteLineEx($"return {column.var_name}__prop;");
                        writer.WriteLineEx("}");
                        writer.WriteLineEx("}");
                    }
                    else
                    {
                        writer.WriteLineEx($"public override {type} {column.var_name}");
                        writer.WriteLineEx("{");
                        writer.WriteLineEx("get");
                        writer.WriteLineEx("{");
                        //_writer.WriteLineEx($"return Utility.Decrypt({column.var_name}__prop);");
                        writer.WriteLineEx($"return {column.var_name}__prop;");
                        writer.WriteLineEx("}");
                        writer.WriteLineEx("}");
                    }
                }
            }
            //_writer.WriteLineEx("#endif");
        }
    }
}
