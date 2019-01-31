using System;
using System.Collections.Generic;
using System.Text;
using CSScriptLibrary;
namespace TableGenerate
{
    public class ExportToTF : ExportBase
    {
        private string _csFileDir;
        private string _enumTypeDir;
        private string _dllOutputDir;

        public string DllOutputDir
        {
            set
            {
                //if(System.IO.Path.DirectorySeparatorChar == '/')
                //    _dllOutputDir = Regex.Replace(value, "[^A-Za-z0-9 _]", string.Empty);
                //else
                _dllOutputDir = value;
            }
        }

        public string CSFileDir
        {
            set
            {
                _csFileDir = value;
            }
        }

        public string EnumTypeDir
        {
            set
            {
                _enumTypeDir = value;
            }
        }

        public static string include_str = @"
namespace TBL 
{
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	public interface ILoader
	{
		void Init();
		void ReadStream(System.IO.Stream stream);
		void ExcelLoad(string path, string language);
		void CheckReplaceFile(string tempFileName, string fileName);
		string GetFileName();
		byte[] GetHash(System.IO.Stream stream);
		//void GetMapAndArray(System.Collections.Generic.Dictionary<string, object> container);
	}
	  
    public struct StringEqualityComparer : IEqualityComparer<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(string x, string y)
        {
            bool x_empty = string.IsNullOrEmpty(x);
            bool y_empty = string.IsNullOrEmpty(y);
            if (!x_empty || y_empty)
                return false;
            if (x_empty || !y_empty)
                return false;
            if (x_empty && y_empty)
                return true;
            return x.Equals(y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
    public struct LongEqualityComparer : IEqualityComparer<long>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(long x, long y)
        {
            return x == y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(long obj)
        {
            return obj.GetHashCode();
        }
    }
    public struct IntEqualityComparer : IEqualityComparer<int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
    }
    public struct UIntEqualityComparer : IEqualityComparer<uint>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(uint x, uint y)
        {
            return x == y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(uint obj)
        {
            return obj.GetHashCode();
        }
    }
    public struct ShortEqualityComparer : IEqualityComparer<short>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(short x, short y)
        {
            return x == y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(short obj)
        {
            return obj.GetHashCode();
        }
    }
    public struct UShortEqualityComparer : IEqualityComparer<ushort>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ushort x, ushort y)
        {
            return x == y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(ushort obj)
        {
            return obj.GetHashCode();
        }
    }
  
}
";

        public override bool Generate(System.Reflection.Assembly refAssem, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            System.Text.Encoding enc = new System.Text.ASCIIEncoding();
            string viewFilename = System.IO.Path.GetFileName(sFileName);

            var regex = new System.Text.RegularExpressions.Regex(@"\.[x][l][s]?\w");
            string[] enumTypeFileNames = System.IO.Directory.GetFiles(_enumTypeDir, "*.cs");
            string csFileName = $"{_csFileDir}{System.IO.Path.DirectorySeparatorChar}{regex.Replace(viewFilename, ".cs")}";
            string csMngFileName = $"{_csFileDir}{System.IO.Path.DirectorySeparatorChar}{regex.Replace(viewFilename, "Manager.cs")}";
            string shared_directory = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location);

            StringBuilder str = new StringBuilder();
            string folder = (System.IO.Path.DirectorySeparatorChar == '/' ? "/" : string.Empty) + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(TableGenerate.ExportBase)).CodeBase.Replace("file:///", String.Empty));
            //string folder = System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName).ToString();
            string[] refAssemblies = new string[] {
                $"{folder}{System.IO.Path.DirectorySeparatorChar}DocumentFormat.OpenXml.dll",
                //$"{folder}{System.IO.Path.DirectorySeparatorChar}LambdaSqlBuilder.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}ICSharpCode.SharpZipLib.dll",
                //$"{folder}{System.IO.Path.DirectorySeparatorChar}Mono.Security.dll",
                //$"{folder}{System.IO.Path.DirectorySeparatorChar}Mono.CSharp.dll",
                $"{shared_directory}{System.IO.Path.DirectorySeparatorChar}System.dll",
                $"{shared_directory}{System.IO.Path.DirectorySeparatorChar}System.Xml.dll",
                $"{shared_directory}{System.IO.Path.DirectorySeparatorChar}System.Data.dll"
            };
            foreach(string enumTypeFileName in enumTypeFileNames)
            {
                str.AppendLine(System.IO.File.ReadAllText(enumTypeFileName, enc));
            }
            str.AppendLine(System.IO.File.ReadAllText(csFileName, enc));
            str.AppendLine(System.IO.File.ReadAllText(csMngFileName, enc));

            string str_class = string.Empty;
            string str_loader_class = string.Empty;
            str_class = regex.Replace(viewFilename, string.Empty);

            if (ExportToCSMgr.NameSpace.Length > 0)
                str_loader_class = $"{ExportToCSMgr.NameSpace}.{str_class}.Loader";

            object[] objects = { imp.GetFileName(), outputPath };
            str.AppendLine(include_str);
            str.AppendLine("namespace ScriptLibrary ");
            str.AppendLine("{");
            str.AppendLine("public class Script ");
            str.AppendLine("{");
            str.AppendLine("    public static int ExtractFromExcel(string inputPath, string outputPath)");
            str.AppendLine("    {");
            str.AppendLine("      string excel_name = string.Empty;");
            str.AppendLine("      try");
            str.AppendLine("      {");
            str.AppendLine($"       {str_loader_class}.Instance = new {str_loader_class}();");
            str.AppendLine($"       string directory = System.IO.Directory.GetParent(inputPath).FullName+System.IO.Path.DirectorySeparatorChar+{str_loader_class}.Instance.GetFileName()+System.IO.Path.DirectorySeparatorChar;");
            str.AppendLine($"       string[] files = new string[0];");
            str.AppendLine($"       if(System.IO.Directory.Exists(directory))");
            str.AppendLine($"         files = System.IO.Directory.GetFiles(directory,\"*.xlsm\");");
            str.AppendLine($"       excel_name = System.IO.Path.GetFileName(inputPath);");
            str.AppendLine($"       {str_loader_class}.Instance.ExcelLoad(inputPath,\"{language}\");");
            str.AppendLine($"       foreach(var file in files)");
            str.AppendLine($"       {{");
            str.AppendLine($"         excel_name = System.IO.Path.GetFileName(file);");
            str.AppendLine($"         if( excel_name.Contains(\"~$\"))continue;");
            str.AppendLine($"         {str_loader_class}.Instance.ExcelLoad(file,\"{language}\");");
            str.AppendLine($"       }}");
            str.AppendLine($"       {str_loader_class}.Instance.WriteFile(outputPath);");
            str.AppendLine("      }");
            str.AppendLine("      catch(System.Exception ex)");
            str.AppendLine("      {");
            str.AppendLine("        System.Console.WriteLine(excel_name + \":\" + ex.Message);");
            str.AppendLine("        return 1; ");
            str.AppendLine("      }");
            str.AppendLine("      return 0;");
            str.AppendLine("    }");
            str.AppendLine("    public System.Data.DataSet ExtractFromBytes(string inputPath)");
            str.AppendLine("    {");
            str.AppendLine("    System.Data.DataSet dts = null;");
            str.AppendLine("      try");
            str.AppendLine("      {");
            str.AppendLine($"       {str_loader_class}.Instance = new {str_loader_class}();");
            str.AppendLine($"       using (var stream = new System.IO.FileStream(inputPath, System.IO.FileMode.Open))");
            str.AppendLine("       {");
            str.AppendLine($"         {str_loader_class}.Instance.ReadStream(stream);");
            str.AppendLine($"         dts = {str_loader_class}.Instance.DataSet;");
            str.AppendLine("       }");
            str.AppendLine("      }");
            str.AppendLine("      catch(System.Exception ex)");
            str.AppendLine("      {");
            str.AppendLine("        System.Console.WriteLine(ex.Message);");
            str.AppendLine("      }");
            str.AppendLine("      return dts;");
            str.AppendLine("    }");
            str.AppendLine("    public int SaveFromDataSet(System.Data.DataSet dts, string outputPath)");
            str.AppendLine("    {");
            str.AppendLine("      //try");
            str.AppendLine("      //{");
            str.AppendLine($"       {str_loader_class}.Instance = new {str_loader_class}();");
            str.AppendLine($"       {str_loader_class}.Instance.DataSet = dts;");
            str.AppendLine($"       {str_loader_class}.Instance.WriteFile(outputPath);");
            str.AppendLine("      //}");
            str.AppendLine("      //catch(System.Exception ex)");
            str.AppendLine("      //{");
            str.AppendLine("      //  System.Console.WriteLine(ex.Message);");
            str.AppendLine("      //}");
            str.AppendLine("      return 0;");
            str.AppendLine("    }");
            str.AppendLine("}");
            str.AppendLine("}");

            CSScript.CacheEnabled = false;

            if (System.IO.Directory.Exists(_dllOutputDir) == false)
            {
                System.IO.Directory.CreateDirectory(_dllOutputDir);
            }
            try
            {
                CSScript.CompileCode(str.ToString(), System.IO.Path.Combine(_dllOutputDir, $"{str_class}.dll"), false, refAssemblies);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText($"{viewFilename}err.cs", str.ToString());
                System.IO.File.WriteAllText($"{viewFilename}ex.cs", ex.ToString());
                throw;
            }
            //using (var fs = System.IO.File.OpenRead(outputPath + "/" + str_class + ".dll"))
            //{
            //    long uncompressedLen = fs.Length;
            //    byte[] uncompressed = new byte[uncompressedLen];
            //    fs.Read(uncompressed, 0, (int)uncompressedLen);
            //    int compressedLen = (int)(uncompressedLen * 1.01 + 600);
            //    byte[] compressed = null;//new byte[compressedLen];
            //
            //    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(uncompressed))
            //    {
            //        using (System.IO.MemoryStream zip = new System.IO.MemoryStream())
            //        {
            //            ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(ms, zip, false, 3);
            //            compressed = zip.ToArray();
            //            compressedLen = (int)zip.Length;
            //            System.IO.File.WriteAllBytes(outputPath + "/" + str_class + ".dll.bzip2", compressed);
            //        }
            //    }
            //}

            using (var helper = new AsmHelper(CSScript.LoadCode(str.ToString(), refAssemblies)))
            {
                var obj = helper.Invoke("ScriptLibrary.Script.ExtractFromExcel", objects);
                if (obj != null)
                {
                    int status = (int)obj;
                    if (status == 1)
                    {
                        System.IO.File.WriteAllText($"{viewFilename}err.cs", str.ToString());
                        throw new System.Exception(viewFilename.ToString());
                    }
                }
            }
            current++;

            return true;
        }

    }
}
