using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;

namespace TableGenerate
{
    public class ExportToCSMgr : ExportBase
    {
        public static String NameSpace = string.Empty;
        public int m_current = 0;
        public eGenType _gen_type = eGenType.cs;
        private string _async = string.Empty;
        public string SetAsync
        {
            //set { _async = "unity3d"; }
            set { _async = value; }
        }

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".cs");

            using(var stream = new MemoryStream())
            {
                var _writer = new IndentedTextWriter(new StreamWriter(stream, new System.Text.ASCIIEncoding()), " ");
                {
                    string filename = System.IO.Path.GetFileName(createFileName);
                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace(".cs", string.Empty);
                    max = sheets.GetLength(0);

                    _writer.WriteLineEx($"// generate {filename}");
                    _writer.WriteLineEx( "// DO NOT TOUCH SOURCE....");
                    _writer.WriteLineEx( "#pragma warning disable IDE0007, IDE0011, IDE0025, IDE1006, IDE0018");

                    if (_async == "unity3d")
                    {
                        _writer.WriteLineEx("#if UNITY_EDITOR");
                        _writer.WriteLineEx($"public class {filename}Manager : UnityEngine.MonoBehaviour");
                        _writer.WriteLineEx("{");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            _writer.WriteLineEx($"public {NameSpace}.{filename}.{trimSheetName}[] {trimSheetName} = null;");
                        }
                        _writer.WriteLineEx("}");

                        _writer.WriteLineEx($"[UnityEditor.CustomEditor(typeof({filename}Manager))]");
                        _writer.WriteLineEx($"public class {filename}Editor : UnityEditor.Editor");
                        _writer.WriteLineEx("{");
                        _writer.WriteLineEx($"public override void OnInspectorGUI()");
                        _writer.WriteLineEx("{");
                        _writer.WriteLineEx($"{filename}Manager myScript = ({filename}Manager) target;");
                        _writer.WriteLineEx("if(UnityEngine.GUILayout.Button(\"Load\"))");
                        _writer.WriteLineEx("{");
                        foreach (string sheetName in sheets)
                        {
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            _writer.WriteLineEx($"myScript.{trimSheetName} = {NameSpace}.{filename}.{trimSheetName}.Array_;");
                        }
                        _writer.WriteLineEx("}");
                        //_writer.WriteLineEx("if(UnityEngine.GUILayout.Button(\"Save\"))");
                        //_writer.WriteLineEx("{");
                        //_writer.WriteLineEx("}");
                        _writer.WriteLineEx("DrawDefaultInspector();");
                        _writer.WriteLineEx("}");
                        _writer.WriteLineEx("}");

                        _writer.WriteLineEx("#endif");
                        _writer.WriteLineEx();
                    }

                    // Init class
                    _writer.WriteLineEx($"namespace {NameSpace}.{filename}");
                    _writer.WriteLineEx( "{");
                    _writer.WriteLineEx( "public class Loader : global::TBL.ILoader");
                    _writer.WriteLineEx( "{");
                    _writer.WriteLineEx( "public static Loader Instance = new Loader();");

                    _writer.WriteLineEx("public void Init()");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("Instance = this;");
                    _writer.WriteLineEx("}");

                    _writer.WriteLineEx($"#if !{UNITY_DEFINE}");
                    _writer.WriteLineEx("public System.Data.DataSet DataSet");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("get");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx($"System.Data.DataSet dts = new System.Data.DataSet(\"{filename}\");");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        _writer.WriteLineEx($"{trimSheetName}.GetDataTable(dts);");
                    }
                    _writer.WriteLineEx("return dts;");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("set");
                    _writer.WriteLineEx("{");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        _writer.WriteLineEx($"{trimSheetName}.SetDataSet(value);");
                    }
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("}");
                    // ExcelLoad function
                    _writer.WriteLineEx("#if !NO_EXCEL_LOADER");
                    _writer.WriteLineEx("public void ExcelLoad(string path, string language)");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("language = language.Trim();");
                    _writer.WriteLineEx("string directoryName = System.IO.Path.GetDirectoryName(path);");
                    _writer.WriteLineEx("var imp = new ClassUtil.ExcelImporter();");
                    _writer.WriteLineEx("imp.Open(path);");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        _writer.WriteLineEx($"{trimSheetName}.ExcelLoad(imp,directoryName,language);");
                    }
                    _writer.WriteLineEx("imp.Dispose();");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("#endif");
                    _writer.WriteLineEx("#endif");

                    // GetStaticObject function
                    _writer.WriteLineEx("/*");
                    _writer.WriteLineEx("public void GetMapAndArray(System.Collections.Immutable.ImmutableDictionary<string,object> container)");
                    _writer.WriteLineEx("{");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var key_column = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except).FirstOrDefault( t => t.is_key);
                        var type = key_column.GenerateType(_gen_type);
                        var equality_type = key_column.GetEqualityTypeValue(_gen_type);
                        _writer.WriteLineEx($"container.Remove( \"{filename}.{trimSheetName}.Map_\");");
                        _writer.WriteLineEx($"container.Remove( \"{filename}.{trimSheetName}.Array_\");");
                        _writer.WriteLineEx($"container.Add( \"{filename}.{trimSheetName}.Map_\", new System.Collections.Immutable.ImmutableDictionary<{type},{trimSheetName}>({trimSheetName}.Map_{(string.IsNullOrEmpty(equality_type)?string.Empty:$",{equality_type}")}));");
                        _writer.WriteLineEx($"container.Add( \"{filename}.{trimSheetName}.Array_\",{trimSheetName}.Array_);");
                    }
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("*/");

                    _writer.WriteLineEx("public void CheckReplaceFile( string tempFileName, string fileName ) ");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("System.IO.File.Copy(tempFileName, fileName, true);");
                    _writer.WriteLineEx("}");

                    // WriteFile function
                    _writer.WriteLineEx("#if !NO_EXCEL_LOADER");
                    _writer.WriteLineEx("public void WriteFile(string path)");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("int uncompressedLength = 0;");
                    _writer.WriteLineEx("System.IO.MemoryStream uncompressedMemoryStream = null;");
                    _writer.WriteLineEx("uncompressedMemoryStream = new System.IO.MemoryStream(128 * 1024);");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("var uncompressedMemoryStreamWriter = new System.IO.BinaryWriter(uncompressedMemoryStream);");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        _writer.WriteLineEx($"{trimSheetName}.WriteStream(uncompressedMemoryStreamWriter);");
                    }
                    _writer.WriteLineEx("uncompressedLength = (int) uncompressedMemoryStream.Position;");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("System.IO.FileStream stream = null;");
                    _writer.WriteLineEx("try");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("string tempFileName = System.IO.Path.GetTempFile" +
                        "Name();");
                    _writer.WriteLineEx("uncompressedMemoryStream.Position=0;");
                    _writer.WriteLineEx("stream = new System.IO.FileStream(tempFileName, System.IO.FileMode.Create);");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("using (System.IO.MemoryStream __zip = new System.IO.MemoryStream())");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(uncompressedMemoryStream, __zip,false,1);");
                    _writer.WriteLineEx("using(var md5 = System.Security.Cryptography.MD5.Create())");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("var __compressed = __zip.ToArray();");
                    _writer.WriteLineEx("byte[] hashBytes = md5.ComputeHash(__compressed);");
                    _writer.WriteLineEx("stream.WriteByte((byte)hashBytes.Length);");
                    _writer.WriteLineEx("stream.Write(hashBytes, 0, hashBytes.Length);");
                    _writer.WriteLineEx("stream.Write( System.BitConverter.GetBytes(uncompressedLength), 0, 4 );");
                    _writer.WriteLineEx("stream.Write( System.BitConverter.GetBytes(__compressed.Length), 0, 4 );");
                    _writer.WriteLineEx("stream.Write(__compressed, 0, __compressed.Length);");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("}");

                    _writer.WriteLineEx("}");

                    _writer.WriteLineEx("stream.Flush();");
                    _writer.WriteLineEx("stream.Close();");
                    _writer.WriteLineEx("stream = null;");
                    _writer.WriteLineEx($"CheckReplaceFile(tempFileName, System.IO.Path.GetDirectoryName( path + \"/\") + \"/{filename}.bytes\");");
                    _writer.WriteLineEx("}catch(System.Exception e)");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("System.Console.WriteLine(e.ToString());");
                    _writer.WriteLineEx("throw;");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("finally");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("if(uncompressedMemoryStream != null) uncompressedMemoryStream.Dispose();");
                    _writer.WriteLineEx("}");

                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("#endif //NO_EXCEL_LOADER");


                    _writer.WriteLineEx($"    public string GetFileName() {{ return \"{filename}\"; }}");
                    // ReadStream function
                    _writer.WriteLineEx( "public void ReadStream(System.IO.Stream stream)");
                    _writer.WriteLineEx( "{");
                    _writer.WriteLineEx( "stream.Position = 0;");
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
                    _writer.WriteLineEx("int streamLength = (int)stream.Length;");
                    _writer.WriteLineEx("int hashLength = stream.ReadByte();");
                    _writer.WriteLineEx("byte[] uncompressedSize = new byte[4];");
                    _writer.WriteLineEx("byte[] compressedSize = new byte[4];");
                    _writer.WriteLineEx("byte[] hashBytes = new byte[hashLength];");
                    _writer.WriteLineEx("stream.Read( hashBytes, 0, hashLength);");
                    _writer.WriteLineEx("byte[] bytes = new byte[streamLength-hashLength-compressedSize.Length-uncompressedSize.Length-1];");
                    _writer.WriteLineEx("stream.Read( uncompressedSize, 0, uncompressedSize.Length);");
                    _writer.WriteLineEx("stream.Read( compressedSize, 0, compressedSize.Length);");
                    _writer.WriteLineEx("stream.Read( bytes, 0, streamLength-hashLength-compressedSize.Length-uncompressedSize.Length-1);");
                    _writer.WriteLineEx("using(var md5 = System.Security.Cryptography.MD5.Create())");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("byte[] dataBytes = md5.ComputeHash(bytes);");
                    _writer.WriteLineEx("if(!System.Linq.Enumerable.SequenceEqual(hashBytes, dataBytes))");
                    _writer.WriteLineEx($"throw new System.Exception(\"{filename} verify failure...\");");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("using (System.IO.MemoryStream __ms = new System.IO.MemoryStream(bytes))");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("using (var decompressStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(__ms))");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("int uncompressedSize__ = System.BitConverter.ToInt32(uncompressedSize,0);");
                    _writer.WriteLineEx("bytes = new byte[uncompressedSize__];");
                    _writer.WriteLineEx("decompressStream.Read(bytes, 0, uncompressedSize__);");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("}");

                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("System.IO.MemoryStream __ms = null;");
                    _writer.WriteLineEx("try");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("__ms = new System.IO.MemoryStream(bytes);");
                    _writer.WriteLineEx("using (System.IO.BinaryReader reader = new System.IO.BinaryReader(__ms))");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("__ms = null;");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        _writer.WriteLineEx($"{trimSheetName}.ReadStream(reader);");
                    }
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("finally");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("if(__ms != null) __ms.Dispose();");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("}");
                    // Get Hash FUnction
                    _writer.WriteLineEx("public byte[] GetHash(System.IO.Stream stream)");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("stream.Position = 0;");
                    _writer.WriteLineEx("int hashLength = stream.ReadByte();");
                    _writer.WriteLineEx("byte[] hashBytes = new byte[hashLength];");
                    _writer.WriteLineEx("stream.Read( hashBytes, 0, hashLength);");
                    _writer.WriteLineEx("return hashBytes;");
                    _writer.WriteLineEx("}");

                    _writer.WriteLineEx("}");

                    current = 0;
                    foreach (string sheetName in sheets)
                    {
                        current++;
                        m_current = current - 1; 
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        SheetProcess(_writer, filename, trimSheetName, columns);
                    }

                    _writer.WriteLineEx("}");
                    _writer.Flush();
                }
                string tempCreateFileName = createFileName.Replace(".cs", "Manager.cs");
                ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{tempCreateFileName}");
            }

            return true;
        }

        private void SheetProcess(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            var key_column = columns.FirstOrDefault(compare => compare.is_key);
            var type = key_column.GenerateType(_gen_type);
            var equality_type = key_column.GetEqualityTypeValue(_gen_type);
            _writer.WriteLineEx();
            if (_async == "unity3d")
            {
                _writer.WriteLineEx($"#if !ENCRYPT");
                _writer.WriteLineEx($"[System.Serializable]");
                _writer.WriteLineEx($"#endif");
            }
            string primitiveName = key_column.var_name;
            _writer.WriteLineEx($"public partial class {sheetName} : BaseClasses.{sheetName}");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx($"public static {sheetName}[] Array_ = null;");
            _writer.WriteLineEx($"public static System.Collections.Immutable.ImmutableDictionary<{type},{sheetName}> Map_ = null;");
            _writer.WriteLineEx();
            _writer.WriteLineEx($"public static void ArrayToMap({sheetName}[] array__)");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx($"var map_ = new System.Collections.Generic.Dictionary<{type},{sheetName}> (array__.Length);");
            //_writer.WriteLineEx($"array.Sort(delegate({sheetName} a,{sheetName} b)");
            //_writer.WriteLineEx( "{");
            //_writer.WriteLineEx($"return a.{primitiveName}.CompareTo(b.{primitiveName});");
            //_writer.WriteLineEx("});");
            _writer.WriteLineEx($"{sheetName} __table = null;");
            _writer.WriteLineEx("int __i=0;");
            _writer.WriteLineEx($"try{{");
            _writer.WriteLineEx("for(__i=0;__i<array__.Length;__i++)");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx($"__table = array__[__i];");
            _writer.WriteLineEx($"map_.Add(__table.{primitiveName}, __table);");
            _writer.WriteLineEx( "}");
            _writer.WriteLineEx($"}}catch(System.Exception e)");
            _writer.WriteLineEx($"{{");
            _writer.WriteLineEx($"      throw new System.Exception($\"{{__table.{primitiveName}}} row:{{__i}} {{e.Message}}\");");
            _writer.WriteLineEx($"}}");
            _writer.WriteLineEx($"Map_ = System.Collections.Immutable.ImmutableDictionary<{type},{sheetName}>.Empty.AddRange(map_);");
            if(!string.IsNullOrEmpty(equality_type))
            {
                _writer.WriteLineEx($"Map_ = Map_.WithComparers({equality_type});");
            }
            _writer.WriteLineEx( "}");
            _writer.WriteLineEx();
            WriteStreamFunction(_writer, filename, sheetName,columns);
            _writer.WriteLineEx("#if !UNITY_5_3_OR_NEWER");
            SetDataTableFunction(_writer, filename, sheetName, columns);
            _writer.WriteLineEx("#if !NO_EXCEL_LOADER");
            ExcelLoadFunction(_writer, filename, sheetName, columns);
            _writer.WriteLineEx("#endif");
            GetDataTableFunction(_writer, filename, sheetName, columns);
            _writer.WriteLineEx("#endif");
            ReadStreamFunction(_writer, filename, sheetName, columns);
            ArrayCountFunction(_writer, filename, sheetName, columns);
            SheetConstructorProcess(_writer, sheetName, columns);
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
            _writer.WriteLineEx("}");
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
        private void ExcelLoadFunction(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            var key_column = columns.FirstOrDefault(compare => compare.is_key == true);
            var nation_column = columns.FirstOrDefault(compare => compare.var_name.Trim().ToLower() == "nation");

            _writer.WriteLineEx( "public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx( "var i=0; var j=0;");
            _writer.WriteLineEx( "string[,] rows = null;");
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
                _writer.WriteLineEx($"{type} {name};");
            }
            _writer.WriteLineEx( "try");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx($"rows = imp.GetSheet(\"{sheetName}\", language);");
            if (nation_column != null) _writer.WriteLineEx($"      bool useNation = rows[0,{nation_column.data_column_index}].Trim().ToLower() == \"nation\";");
            _writer.WriteLineEx($"var list__ = new System.Collections.Generic.List<{sheetName}>(rows.GetLength(0) - 3);");
            _writer.WriteLineEx( "for (i = 3; i < rows.GetLength(0); i++)");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx($"j=0;");
            _writer.WriteLineEx("if( rows[i,0].Length == 0) break;");
            if (nation_column != null) _writer.WriteLineEx($"if( useNation == true && (rows[i,{nation_column.data_column_index}].Trim().Length == 0 || rows[i,{nation_column.data_column_index}].Trim().ToLower() == \"all\") ) {{}}");
            if (nation_column != null) _writer.WriteLineEx($"else if( useNation == true && rows[i,{nation_column.data_column_index}].Trim()/*.ToLower()*/ != language ) continue;");
            foreach (var column in columns)
            {
                string type = column.GenerateType(_gen_type);
                string arg = $"rows[i,{column.data_column_index}]";
                string convert_function = column.GetConvertFunction(arg,_gen_type);
                if (column.is_generated == false)
                {
                    continue;
                }
                if (column.array_index >= 0)
                {
                    if (TableGenerateCmd.ProgramCmd.not_array_length_full)
                    {
                        if (column.array_index == 0)
                        {
                            int array_count = columns.Where(compare => compare.var_name == column.var_name).Count();
                            _writer.WriteLineEx($"var {column.var_name}_list__ = new System.Collections.Generic.List<{column.GenerateBaseType(_gen_type)}>();");
                            _writer.WriteLineEx($"bool not_empty_{column.var_name}__ = false;");
                        }

                        _writer.WriteLineEx($"j = {column.data_column_index};");
                        _writer.WriteLineEx($"if(!string.IsNullOrEmpty({arg}))");
                        _writer.WriteLineEx($"{{");
                        if (column.array_index > 0)
                        {
                            _writer.WriteLineEx($"if(not_empty_{column.var_name}__)");
                            _writer.WriteLineEx($"{{");
                            _writer.WriteLineEx($"    throw new System.Exception(string.Format(\"i:{{0}} j:{{1}} before is empty text\",i,j));");
                            _writer.WriteLineEx($"}}");
                        }
                        _writer.WriteLineEx($"{column.var_name}_list__.Add({convert_function});");
                        _writer.WriteLineEx($"}}");
                        _writer.WriteLineEx($"else");
                        _writer.WriteLineEx($"{{");
                        _writer.WriteLineEx($"not_empty_{column.var_name}__ = true;");
                        _writer.WriteLineEx($"}}");
                        if (column.is_last_array)
                        {
                            _writer.WriteLineEx($"{column.var_name} = {column.var_name}_list__.ToArray();");
                        }
                    }
                    else
                    {
                        if (column.array_index == 0)
                        {
                            int array_count = columns.Where(compare => compare.var_name == column.var_name).Count();
                            _writer.WriteLineEx($"{column.var_name} = new {column.GenerateBaseType(_gen_type)}[{array_count}];");
                        }

                        if (column.IsDateTime() == true)
                        {
                            _writer.WriteLineEx($"j = {column.data_column_index};");
                            _writer.WriteLineEx($"j = {column.data_column_index}; if(string.IsNullOrEmpty({arg})){{{column.var_name}[{column.array_index}] = new System.DateTime(1970,1,1);}}else{{{column.var_name}[{column.array_index}] = {convert_function};}}");
                        }
                        else if (column.IsTimeSpan() == true)
                        {
                            _writer.WriteLineEx($"j = {column.data_column_index};");
                            _writer.WriteLineEx($"j = {column.data_column_index}; if(string.IsNullOrEmpty({arg})){{{column.var_name}[{column.array_index}] = System.TimeSpan.FromTicks(0);}}else{{{column.var_name}[{column.array_index}] = {convert_function};}}");
                        }
                        else if (column.IsNumberType() == true)
                        {
                            _writer.WriteLineEx($"j = {column.data_column_index};");
                            _writer.WriteLineEx("{");
                            _writer.WriteLineEx($"{column.GetPrimitiveType(_gen_type)} outvalue = {column.GetInitValue(_gen_type)}; if(!string.IsNullOrEmpty({arg})) ");
                            _writer.WriteLineEx($"outvalue = {convert_function}; {column.var_name}[{column.array_index}] = outvalue;");
                            _writer.WriteLineEx("}");
                        }
                        else if (column.IsNumberType() == false)
                        {
                            _writer.WriteLineEx($"j = {column.data_column_index};");
                            _writer.WriteLineEx($"{column.var_name}[{column.array_index}] = {convert_function};");
                        }
                    }
                    continue;
                }
                if (column.IsDateTime() == true)
                {
                    _writer.WriteLineEx($"j = {column.data_column_index};");
                    _writer.WriteLineEx($"if(string.IsNullOrEmpty({arg})){{{column.var_name} = new System.DateTime(1970,1,1);}} else{{{column.var_name} = {convert_function};}}");
                }
                else if (column.IsTimeSpan() == true)
                {
                    _writer.WriteLineEx($"j = {column.data_column_index};");
                    _writer.WriteLineEx($"if(string.IsNullOrEmpty({arg})){{{column.var_name} = System.TimeSpan.FromTicks(0);}} else{{{column.var_name} = {convert_function};}}");
                }
                else if (column.IsNumberType() == true)
                {
                    _writer.WriteLineEx($"j = {column.data_column_index};");
                    _writer.WriteLineEx($"if(string.IsNullOrEmpty({arg}))");
                    _writer.WriteLineEx( "{");
                    _writer.WriteLineEx($"{column.var_name} = {column.GetInitValue(_gen_type)};}}else {{{column.var_name} = {convert_function};");
                    _writer.WriteLineEx( "}");
                }
                else if (column.IsNumberType() == false)
                {
                    _writer.WriteLineEx($"j = {column.data_column_index};");
                    _writer.WriteLineEx($"{column.var_name} = {convert_function};");
                }
            }

            if(TableGenerateCmd.ProgramCmd.not_array_length_full)
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
                    _writer.WriteLineEx($"if({string.Join(" || ", arrs.Skip(1).Select(t => $" {arrs[0].var_name}.Length != {t.var_name}.Length"))})");
                    _writer.WriteLineEx($"{{");
                    _writer.WriteLineEx($"    throw new System.Exception(string.Format(\"mismatch group:{{0}}\",\"{item.groupName}\"));");
                    _writer.WriteLineEx($"}}");
                }
            }

            _writer.WriteLineEx(string.Format("{0} values = new {0}({1});", sheetName, string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.var_name}").ToArray())));
            _writer.WriteLineEx("foreach (var preValues in list__)");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx($"if (preValues.{key_column.var_name}.Equals({key_column.var_name}))");
            _writer.WriteLineEx("{");
            _writer.WriteLineEx($"throw new System.Exception(\"row:\" + i + \" {sheetName}.{key_column.var_name}:\" + preValues.{key_column.var_name}.ToString() + \") Duplicated!!\");");
            _writer.WriteLineEx("}");
            _writer.WriteLineEx( "}");

            _writer.WriteLineEx("list__.Add(values);");
            _writer.WriteLineEx( "}");
            _writer.WriteLineEx("Array_ = list__.ToArray();");
            _writer.WriteLineEx("ArrayToMap(Array_);");
            _writer.WriteLineEx( "}catch(System.Exception e)");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx("if( rows == null ) throw;");
            _writer.WriteLineEx("if (j >= rows.GetLength(1))");
            _writer.WriteLineEx($"  throw new System.Exception(\"sheet({sheetName}) invalid column count:\" + j);");
            _writer.WriteLineEx($"throw new System.Exception(\" convert failure : excel({filename}).sheet({sheetName}) key:\" + rows[i,0] + \" Name:\" + rows[0,j] + \" \" + rows[i,j] + \" \" + e.Message );");
            _writer.WriteLineEx( "}");
            _writer.WriteLineEx( "}");
        }
        private void GetDataTableFunction(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            var key_column = columns.FirstOrDefault(compare => compare.is_key == true);
            var nation_column = columns.FirstOrDefault(compare => compare.var_name.Trim().ToLower() == "nation");

            _writer.WriteLineEx("public static void GetDataTable(System.Data.DataSet dts)");
            _writer.WriteLineEx("{");
            _writer.WriteLineEx($"System.Data.DataTable table = dts.Tables.Add(\"{sheetName}\");");
            foreach (var column in columns.Where(compare => compare.array_index <= 0))
            {
                string type = column.GenerateBaseType(_gen_type);

                if (column.is_generated == false)
                {
                    continue;
                }

                if (column.array_index >= 0)
                {
                    int array_count = columns.Where(compare => compare.var_name == column.var_name).Max(compare => compare.array_index);
                    for (int i = 0; i <= array_count; i++)
                    {
                        _writer.WriteLineEx($"table.Columns.Add(\"{column.var_name}{i}\", typeof({type}));");
                    }
                }
                else
                {
                    _writer.WriteLineEx($"table.Columns.Add(\"{column.var_name}\", typeof({type}));");
                }
            }
            _writer.WriteLineEx("foreach(var item in Array_ )");
            _writer.WriteLineEx("{");
            _writer.WriteLineEx("table.Rows.Add(");
            bool bFirst = true;
            foreach (var column in columns.Where(compare => compare.array_index <= 0))
            {
                string type = column.GenerateType(_gen_type);

                if (column.is_generated == false)
                {
                    continue;
                }

                string delim = string.Empty;


                if (column.array_index >= 0)
                {
                    int array_count = columns.Where(compare => compare.var_name == column.var_name).Max(compare => compare.array_index);
                    for (int i = 0; i <= array_count; i++)
                    {
                        if( i > 0 || bFirst == false)
                        {
                            delim = ",";
                        }
                        else
                        {
                            delim = string.Empty;
                        }
                        _writer.WriteLineEx($"{delim}item.{column.var_name}[{i}]");
                        bFirst = false;
                    }
                }
                else
                {
                    if (bFirst == false)
                    {
                        delim = ",";
                    }
                    else
                    {
                        delim = string.Empty;
                    }
                    _writer.WriteLineEx($"{delim}item.{column.var_name}");
                    bFirst = false;
                }
            }
            _writer.WriteLineEx(");");
            _writer.WriteLineEx("}");
            _writer.WriteLineEx("}");
        }
        private void ArrayCountFunction(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            foreach (var column in columns.Where(compare => compare.array_index <= 0))
            {
                string type = column.base_type.GenerateBaseType(_gen_type);

                if (column.is_generated == false)
                {
                    continue;
                }

                if (column.array_index >= 0)
                {
                    int array_count = columns.Where(compare => compare.var_name == column.var_name).Count();
                    _writer.WriteLineEx($"     public static int {column.var_name}_Length {{ get {{ return {array_count}; }} }}");
                }
            }
        }
        private void SetDataTableFunction(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            var key_column = columns.FirstOrDefault(compare => compare.is_key == true);
            var key_type = key_column.GenerateType(_gen_type);
            var nation_column = columns.FirstOrDefault(compare => compare.var_name.Trim().ToLower() == "nation");

            _writer.WriteLineEx("public static bool SetDataSet(System.Data.DataSet dts)");
            _writer.WriteLineEx("{");
            _writer.WriteLineEx($"{sheetName}.Map_ = System.Collections.Immutable.ImmutableDictionary<{key_type},{sheetName}>.Empty;");
            _writer.WriteLineEx($"{sheetName}.Array_ = new {sheetName}[dts.Tables[\"{sheetName}\"].Rows.Count];");
            _writer.WriteLineEx($"int row__ = 0;");
            _writer.WriteLineEx($"foreach (System.Data.DataRow row in dts.Tables[\"{sheetName}\"].Rows)");
            _writer.WriteLineEx("{");
            _writer.WriteLineEx($"{sheetName} table = new {sheetName}");
            _writer.WriteLineEx("(");
            bool bFirst = true;
            foreach (var column in columns.Where(compare => compare.array_index <= 0))
            {
                string type = column.base_type.GenerateBaseType(_gen_type);
                string generate_type = column.GenerateType(_gen_type);
                string append = string.Empty;

                if (column.is_generated == false)
                {
                    continue;
                }
                if (bFirst == false)
                {
                    append = ",";
                }
                else
                {
                    append ="";
                }
                if (column.array_index >= 0)
                {
                    int array_count = columns.Where(compare => compare.var_name == column.var_name).Max(compare => compare.array_index);
                    for (int i = 0; i <= array_count; i++)
                    {
                        string arg = $"row[\"{column.var_name}{i}\"].ToString()";
                        string convert_function = column.GetConvertFunction(arg, _gen_type);
                        if (i == 0)
                        {
                            _writer.WriteLineEx($"{append}new {generate_type}");
                            append = ",";
                            _writer.WriteLineEx("{");
                            _writer.WriteLineEx($"{convert_function}");
                        }
                        else
                        {
                            _writer.WriteLineEx($",{convert_function}");
                        }
                    }
                    _writer.WriteLineEx("}");
                }
                else
                {
                    string arg = $"row[\"{column.var_name}\"].ToString()";
                    string convert_function = column.GetConvertFunction(arg, _gen_type);
                    _writer.WriteLineEx($"{append}{convert_function}");
                }
                bFirst = false;
            }
            _writer.WriteLineEx(");");
            _writer.WriteLineEx($"{sheetName}.Map_ = {sheetName}.Map_.Add(table.{key_column.var_name}, table);");
            _writer.WriteLineEx($"{sheetName}.Array_[row__++] = table;");
            _writer.WriteLineEx("}");
            _writer.WriteLineEx("return true;");
            _writer.WriteLineEx("}");
        }
        private void WriteStreamFunction(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            _writer.WriteLineEx("public static void WriteStream(System.IO.BinaryWriter __writer)");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx($"__writer.Write(Array_.Length);"); ;
            _writer.WriteLineEx($"for (var __i=0;__i<Array_.Length;__i++)");
            _writer.WriteLineEx( "{");
            _writer.WriteLineEx($"var __table = Array_[__i];");
            foreach (var column in columns.Where( t => t.array_index <= 0))
            {
                string type = column.GenerateType(_gen_type);

                if (column.is_generated == false)
                {
                    continue;
                }

                if (column.array_index >= 0)
                {
                    _writer.WriteLineEx($"TBL.Encoder.Write7BitEncodedInt(__writer,__table.{column.var_name}.Length);");
                    _writer.Write($"for(var j__=0;j__<__table.{column.var_name}.Length;++j__)");
                    _writer.Write("{");
                    if (column.IsDateTime())
                        _writer.Write($"__writer.Write(__table.{column.var_name}[j__].ToBinary());");
                    else if (column.IsTimeSpan())
                        _writer.Write($"__writer.Write(__table.{column.var_name}[j__].Ticks);");
                    else if (column.IsEnumType() || column.IsStructType())
                    {
                        string primitive_type = column.primitive_type.GenerateBaseType(_gen_type);
                        _writer.Write($"__writer.Write(({primitive_type})__table.{column.var_name}[j__]);");
                    }
                    else
                        _writer.Write($"__writer.Write(__table.{column.var_name}[j__]);");
                    _writer.Write("}");
                    _writer.WriteLineEx("");
                }
                else
                {
                    if (column.IsDateTime())
                        _writer.WriteLineEx($"__writer.Write(__table.{column.var_name}.ToBinary());");
                    else if (column.IsTimeSpan())
                        _writer.WriteLineEx($"__writer.Write(__table.{column.var_name}.Ticks);");
                    else if (column.IsEnumType() || column.IsStructType())
                    {
                        string primitive_type = column.primitive_type.GenerateBaseType(_gen_type);
                        _writer.WriteLineEx($"__writer.Write(({primitive_type})__table.{column.var_name});");
                    }
                    else
                        _writer.WriteLineEx($"__writer.Write(__table.{column.var_name});");
                }
            }
            _writer.WriteLineEx( "}");
            _writer.WriteLineEx( "}");
        }
        private void ReadStreamFunction(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            _writer.WriteLineEx("public static void ReadStream(System.IO.BinaryReader __reader)");
            _writer.WriteLineEx("{");
            _writer.WriteLineEx($"var count__ = __reader.ReadInt32();");
            _writer.WriteLineEx($"var array__ = new {sheetName}[count__];");
            _writer.WriteLineEx($"for (var __i=0;__i<array__.Length;__i++)");
            _writer.WriteLineEx("{");
            foreach (var column in columns.Where( t => t.array_index <= 0))
            {
                string type = column.GenerateType(_gen_type);
                string convert_funtion = column.GetReadStreamFunction(_gen_type);

                if (column.is_generated == false)
                {
                    continue;
                }

                if (column.array_index >= 0)
                {
                    _writer.WriteLineEx($"{type} {column.var_name} = null;");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);");
                    _writer.WriteLineEx($"{column.var_name} = new {column.GenerateBaseType(_gen_type)}[arrayCount__];");
                    _writer.WriteLineEx($"for(var __j=0;__j<arrayCount__;++__j){column.var_name}[__j] = {convert_funtion};");
                    _writer.WriteLineEx("}");
                }
                else
                {
                    _writer.WriteLineEx($"var {column.var_name} = {convert_funtion};");
                }
            }
            _writer.WriteLineEx(
                string.Format("{0} __table = new {0}({1});", 
                sheetName,
                string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => t.var_name).ToArray())
            ));
            _writer.WriteLineEx($"array__[__i] = __table;");
            _writer.WriteLineEx("}");
            _writer.WriteLineEx($"Array_ = array__;");
            _writer.WriteLineEx($"ArrayToMap(array__);");
            _writer.WriteLineEx("}");
        }
        private void PropertyFunction(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
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
                    _writer.WriteLineEx($"private {type} {column.var_name}__prop;");
                    _writer.WriteLineEx($"public override {type} {column.var_name}");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx("get");
                    _writer.WriteLineEx("{");
                    _writer.WriteLineEx($"return {column.var_name}__prop;");
                    _writer.WriteLineEx("}");
                    _writer.WriteLineEx("}");
                }
                else if(column.array_index == -1)
                {
                    _writer.WriteLineEx($"private {type} {column.var_name}__prop;");
                    if (
                        column.base_type != eBaseType.Int32 &&
                        column.base_type != eBaseType.Int16 &&
                        column.base_type != eBaseType.Int8 &&
                        column.base_type != eBaseType.Int64 &&
                        column.base_type != eBaseType.String)
                    {
                        _writer.WriteLineEx($"public override {type} {column.var_name}");
                        _writer.WriteLineEx("{");
                        _writer.WriteLineEx("get");
                        _writer.WriteLineEx("{");
                        _writer.WriteLineEx($"return {column.var_name}__prop;");
                        _writer.WriteLineEx("}");
                        _writer.WriteLineEx("}");
                    }
                    else
                    {
                        _writer.WriteLineEx($"public override {type} {column.var_name}");
                        _writer.WriteLineEx("{");
                        _writer.WriteLineEx("get");
                        _writer.WriteLineEx("{");
                        //_writer.WriteLineEx($"return Utility.Decrypt({column.var_name}__prop);");
                        _writer.WriteLineEx($"return {column.var_name}__prop;");
                        _writer.WriteLineEx("}");
                        _writer.WriteLineEx("}");
                    }
                }
            }
            //_writer.WriteLineEx("#endif");
        }
    }
}
