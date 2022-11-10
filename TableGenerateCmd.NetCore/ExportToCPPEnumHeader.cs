using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;
using System.Reflection;
using Google.Protobuf.Reflection;

namespace TableGenerate
{
    public class ExportToCPPEnumHeader
    {
        private string CPPClassPredefine;
        public ExportToCPPEnumHeader(string cppClassPredefine)
        {
            CPPClassPredefine = cppClassPredefine;
        }
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

        public bool Generate(System.Reflection.Assembly refAssembly, string outputPath)
        {
            try
            {
                var enums = refAssembly.GetTypes().Where(t => t.IsEnum).Select(t => t.GetTypeInfo());
                {
                    foreach (var typeInfo in enums)
                    {
                        bool bit_flags = false;
                        var reflection = refAssembly.GetTypes().FirstOrDefault(t => t.Name == $"{typeInfo.Name}Reflection");
                        if (reflection != null)
                        {
                            var o = reflection.GetProperty("Descriptor");
                            var r = o.GetValue(null) as Google.Protobuf.Reflection.FileDescriptor;
                            foreach (var d in r.Dependencies.Select(t=>t.ToProto()))
                            {
                                bit_flags = d.Extension.Any( t => t.Name.Equals("bit_flags"));
                                break;
                            }
                        }
                            
                        using var stream = new MemoryStream();
                        var writer = new IndentedTextWriter(new StreamWriter(stream, Encoding.UTF8), "  ");

                        writer.WriteLineEx($"// generate E{typeInfo.Name}");
                
                        writer.WriteLineEx("// DO NOT TOUCH SOURCE....");
                        writer.WriteLineEx($"#pragma once");
                        writer.WriteLineEx($"#include \"CoreMinimal.h\"");
                        writer.WriteLineEx($"UENUM(Meta = ({(bit_flags?"Bitflags, UseEnumValuesAsMaskValuesInEditor = \"true\"":typeInfo.Name)}))");
                        writer.WriteLineEx($"enum class E{typeInfo.Name} : int32 {{");
                        {
                            var types = typeInfo.DeclaredFields.Where(t => t.IsStatic).ToArray();
                            System.Type enumUnderlyingType = System.Enum.GetUnderlyingType(typeInfo);
                            System.Array enumValues = System.Enum.GetValues(typeInfo);
                            for (int i = 0; i < enumValues.Length; i++)
                            {
                                #nullable enable                                
                                // Retrieve the value of the ith enum item.
                                object? value = enumValues.GetValue(i);
                                // Convert the value to its underlying type (int, byte, long, ...)
                                object? underlyingValue = System.Convert.ChangeType(value, enumUnderlyingType);
                                #nullable disable                                
                                writer.WriteLineEx(
                                    $"{types[i].Name}={underlyingValue}{(i==0&&bit_flags?" UMETA(Hidden)":string.Empty)}{(i < enumValues.Length ? "," : string.Empty)}");
                            }
                        }
                        writer.WriteLineEx($"}};");
                        writer.WriteLineEx($"ENUM_CLASS_FLAGS(E{typeInfo.Name});");
                        //writer.WriteLineEx($"#endif");
                        writer.Flush();
                        ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{typeInfo.Name}.h", TableGenerateCmd.ProgramCmd.using_perforce);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
            return true;
        }
    }
}

