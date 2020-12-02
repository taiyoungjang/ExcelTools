using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;

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
		void ReadStream(System.IO.Stream stream);
		void ExcelLoad(string path, string language);
		void CheckReplaceFile(string tempFileName, string fileName);
		string GetFileName();
		byte[] GetHash(System.IO.Stream stream);
		//void GetMapAndArray(System.Collections.Generic.Dictionary<string, object> container);
	}
    
public static class Encoder
    {
        public static void Write(this System.IO.BinaryWriter writer__, string value)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            Write7BitEncodedInt(writer__, bytes.Length);
            if(bytes.Length>0)
            {
                writer__.Write(bytes, 0, bytes.Length);
            }
        }
        public static string ReadString(ref System.IO.BinaryReader reader__)
        {
            int count = Read7BitEncodedInt(ref reader__);
            byte[] bytes = reader__.ReadBytes(count);
            if (count > 0)
            {
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            return string.Empty;
        }
        public static void Write7BitEncodedInt(System.IO.BinaryWriter writer__, int value)
        {
#if ENCODED
            uint v = (uint) value;   // support negative numbers
            while (v >= 0x80)
            {
                writer__.Write((byte) (v | 0x80));
                v >>= 7;
            }
            writer__.Write((byte) v);
#else
            writer__.Write(value);
#endif
        }
        public static int Read7BitEncodedInt(ref System.IO.BinaryReader reader__)
        {
#if ENCODED
            var count = 0;
            var shift = 0;
            byte b;
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                {
                    //throw new FormatException();
                }

                // ReadByte handles end of stream cases for us.
                b = reader__.ReadByte();
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
#else
            return reader__.ReadInt32();
#endif
		}
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
            string folder = (System.IO.Path.DirectorySeparatorChar == '/' ? "/" : string.Empty) + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(TableGenerate.ExportBase)).Location.Replace("file:///", String.Empty));
            //string folder = System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName).ToString();
            string[] refAssemblies = new string[] {
                $"{folder}{System.IO.Path.DirectorySeparatorChar}DocumentFormat.OpenXml.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}ICSharpCode.SharpZipLib.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}System.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}System.Collections.Immutable.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}System.Private.CoreLib.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}System.Runtime.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}System.Xml.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}System.Data.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}System.Data.Common.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}System.Threading.Tasks.Parallel.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}System.Collections.Concurrent.dll",
                $"{folder}{System.IO.Path.DirectorySeparatorChar}TableGenerateCmd.dll"
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
            str.AppendLine($"    public bool IsGenArrayFull={TableGenerateCmd.ProgramCmd.not_array_length_full.ToString().ToLower()};");
            str.AppendLine("    public int ExtractFromExcel(string inputPath, string outputPath)");
            str.AppendLine("    {");
            str.AppendLine("      string excel_name = string.Empty;");
            str.AppendLine($"     var loader__ = new {str_loader_class}();");
            str.AppendLine("      try");
            str.AppendLine("      {");
            str.AppendLine($"       string directory = System.IO.Directory.GetParent(inputPath).FullName+System.IO.Path.DirectorySeparatorChar+loader__.GetFileName()+System.IO.Path.DirectorySeparatorChar;");
            str.AppendLine($"       string[] files = new string[0];");
            str.AppendLine($"       if(System.IO.Directory.Exists(directory))");
            str.AppendLine($"         files = System.IO.Directory.GetFiles(directory,\"*.xlsm\");");
            str.AppendLine($"       excel_name = System.IO.Path.GetFileName(inputPath);");
            str.AppendLine($"       loader__.ExcelLoad(inputPath,\"{language}\");");
            str.AppendLine($"       foreach(var file in files)");
            str.AppendLine($"       {{");
            str.AppendLine($"         excel_name = System.IO.Path.GetFileName(file);");
            str.AppendLine($"         if( excel_name.Contains(\"~$\"))continue;");
            str.AppendLine($"         loader__.ExcelLoad(file,\"{language}\");");
            str.AppendLine($"       }}");
            str.AppendLine($"       loader__.WriteFile(outputPath);");
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
            str.AppendLine("      System.Data.DataSet dts = null;");
            str.AppendLine($"      var loader__ = new {str_loader_class}();");
            str.AppendLine("      try");
            str.AppendLine("      {");
            str.AppendLine($"       loader__ = new {str_loader_class}();");
            str.AppendLine($"       using (var stream = new System.IO.FileStream(inputPath, System.IO.FileMode.Open))");
            str.AppendLine("       {");
            str.AppendLine($"         loader__.ReadStream(stream);");
            str.AppendLine($"         dts = loader__.DataSet;");
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
            str.AppendLine($"      var loader__ = new {str_loader_class}();");
            str.AppendLine("      //try");
            str.AppendLine("      //{");
            str.AppendLine($"       loader__ = new {str_loader_class}();");
            str.AppendLine($"       loader__.DataSet = dts;");
            str.AppendLine($"       loader__.WriteFile(outputPath);");
            str.AppendLine("      //}");
            str.AppendLine("      //catch(System.Exception ex)");
            str.AppendLine("      //{");
            str.AppendLine("      //  System.Console.WriteLine(ex.Message);");
            str.AppendLine("      //}");
            str.AppendLine("      return 0;");
            str.AppendLine("    }");
            str.AppendLine("}");
            str.AppendLine("}");

            System.Reflection.Assembly assembly = null;
            if (System.IO.Directory.Exists(_dllOutputDir) == false)
            {
                System.IO.Directory.CreateDirectory(_dllOutputDir);
            }
            try
            {
                assembly = CompileFiles(str.ToString(), refAssemblies);
            }
            catch (Exception)
            {
                //System.IO.File.WriteAllText($"{viewFilename}err.cs", str.ToString());
                //System.IO.File.WriteAllText($"{viewFilename}ex.cs", ex.ToString());
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
            
            //using (var helper = new AsmHelper(assembly))
            {
                var type = assembly.GetType("ScriptLibrary.Script");
                dynamic script = Activator.CreateInstance(type);
                int status = script.ExtractFromExcel(imp.GetFileName(), outputPath);
                if (status == 1)
                {
                    //System.IO.File.WriteAllText($"{viewFilename}err.cs", str.ToString());
                    throw new System.Exception(viewFilename.ToString());
                }
            }
            current++;

            return true;
        }
        public static System.Reflection.Assembly CompileFiles(string text, string[] refAssemblies)
        {
            string output_path = System.IO.Path.GetTempFileName() + ".dll";
            SyntaxTree[] syntaxTree = new SyntaxTree[] { CSharpSyntaxTree.ParseText(text) };
            string assemblyName = Path.GetRandomFileName();
            var references = refAssemblies.Where( t => System.IO.File.Exists(t) ).Select(t => MetadataReference.CreateFromFile(t)).ToList();
            string shared_directory = System.IO.Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            string[] references_dir = refAssemblies.Select(t => System.IO.Path.GetDirectoryName(t)).Distinct().ToArray();
            references.Add(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(System.Numerics.Vector3).GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "mscorlib.dll")));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "System.Runtime.dll")));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "System.IO.dll")));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "System.Console.dll")));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "System.IO.FileSystem.dll")));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "System.Security.Cryptography.Algorithms.dll")));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "System.Security.Cryptography.Primitives.dll")));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "netstandard.dll")));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "System.Net.Primitives.dll")));
            references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(shared_directory, "System.Runtime.Serialization.Xml.dll")));
            //references.Add(MetadataReference.CreateFromFile(typeof(Dapper.Contrib.Extensions.ComputedAttribute).GetTypeInfo().Assembly.Location));

            CSharpCompilation compilation =
                CSharpCompilation.Create(assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(syntaxTree)
                .AddReferences(references);
            //using (var symbols = new MemoryStream())
            {
                using var ms = new MemoryStream();
                {
                    EmitResult result = compilation.Emit(ms);

                    if (false == result.Success)
                    {
                        System.Collections.Generic.IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);

                        foreach (Diagnostic diagnostic in failures)
                        {
                            Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        }
                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        //symbols.Seek(0, SeekOrigin.Begin);
                        System.IO.File.WriteAllBytes(output_path, ms.ToArray());
                        //Assembly assembly = Assembly.Load( new AssemblyName(assemblyName));
                    }
                }
            }
            AssemblyLoadContext.Default.Resolving += (context, name) =>
            {
                // avoid loading *.resources dlls, because of: https://github.com/dotnet/coreclr/issues/8416
                if (name.Name.EndsWith("resources"))
                {
                    return null;
                }

                var dependencies = DependencyContext.Default.RuntimeLibraries;
                foreach (var library in dependencies)
                {
                    if (IsCandidateLibrary(library, name))
                    {
                        return context.LoadFromAssemblyName(new AssemblyName(library.Name));
                    }
                }

                {
                    var foundDlls = Directory.GetFileSystemEntries(new FileInfo(shared_directory).FullName, name.Name + ".dll", SearchOption.AllDirectories);
                    if (foundDlls.Any())
                    {
                        return context.LoadFromAssemblyPath(foundDlls[0]);
                    }
                }
                foreach (var dir in references_dir)
                {
                    var foundDlls = Directory.GetFileSystemEntries(new FileInfo(dir).FullName, name.Name + ".dll", SearchOption.AllDirectories);
                    if (foundDlls.Any())
                    {
                        return context.LoadFromAssemblyPath(foundDlls[0]);
                    }
                }


                return context.LoadFromAssemblyName(name);
            };
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(output_path);
        }
        private static bool IsCandidateLibrary(RuntimeLibrary library, AssemblyName assemblyName)
        {
            return library.Name == (assemblyName.Name)
                || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName.Name));
        }
    }
}
