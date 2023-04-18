using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;
using System.Reflection;

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
                        Dictionary<string, string> descriptions = new();
                        bool bit_flags = false;
                        string str_blue_print_type = string.Empty;
                        string enum_size_type = "int32";
                        var reflection = refAssembly.GetTypes().FirstOrDefault(t => t.Name == $"{typeInfo.Name}Reflection");
                        if (reflection != null)
                        {
                            var o = reflection.GetProperty("Descriptor");
                            var r = o.GetValue(null) as Google.Protobuf.Reflection.FileDescriptor;
                            foreach (var d in r.Dependencies.Select(t=>t.ToProto()))
                            {
                                bit_flags = d.Extension.Any( t => t.Name.Equals("bit_flags"));
                                if (d.Extension.Any(t => t.Name.Equals("blue_print_type")))
                                {
                                    str_blue_print_type = ", BlueprintType";
                                    enum_size_type = "uint8";
                                }
                                break;
                            }
                        }
                            
                        using var stream = new MemoryStream();
                        using var writer = new IndentedTextWriter(new StreamWriter(stream,  Encoding.UTF8), " ");

                        try
                        {
                            descriptions = EnumDescriptorSchemaFilter.BuildDescription(typeInfo);
                        }
                        catch (Exception e)
                        {
                            writer.WriteLineEx($"{e.StackTrace}");
                        }

                        writer.WriteLineEx($"// generate E{typeInfo.Name}");
                
                        writer.WriteLineEx("// DO NOT TOUCH SOURCE....");
                        writer.WriteLineEx($"#pragma once");
                        writer.WriteLineEx($"#include \"CoreMinimal.h\"");
                        writer.WriteLineEx($"UENUM(Meta = ({(bit_flags?"Bitflags, UseEnumValuesAsMaskValuesInEditor = \"true\"":typeInfo.Name)}){str_blue_print_type})");
                        writer.WriteLineEx($"enum class E{typeInfo.Name} : {enum_size_type} {{");
                        {
                            var types = typeInfo.DeclaredFields.Where(t => t.IsStatic).ToArray();
                            Type enumUnderlyingType = Enum.GetUnderlyingType(typeInfo);
                            Array enumValues = System.Enum.GetValues(typeInfo);
                            int namePadding = 0;
                            int valuePadding = 0;
                            for (int i = 0; i < enumValues.Length; i++)
                            {
#nullable enable                                
                                // Retrieve the value of the ith enum item.
                                object? value = enumValues.GetValue(i);
                                // Convert the value to its underlying type (int, byte, long, ...)
                                object? underlyingValue = Convert.ChangeType(value, enumUnderlyingType);
#nullable disable
                                namePadding = Math.Max(types[i].Name.Length,namePadding);
                                valuePadding = Math.Max(underlyingValue.ToString().Length,valuePadding);
                            }
                            for (int i = 0; i < enumValues.Length; i++)
                            {
                                #nullable enable                                
                                // Retrieve the value of the ith enum item.
                                object? value = enumValues.GetValue(i);
                                // Convert the value to its underlying type (int, byte, long, ...)
                                object? underlyingValue = System.Convert.ChangeType(value, enumUnderlyingType);
                                #nullable disable
                                string desc = string.Empty;
                                if (descriptions.TryGetValue(types[i].Name, out desc) || descriptions.TryGetValue($"{typeInfo.Name}_{types[i].Name}", out desc))
                                {
                                }
                                writer.WriteLineEx(
                                    $"{types[i].Name.PadRight(namePadding)} = {underlyingValue.ToString().PadLeft(valuePadding)}{(i==0&&bit_flags?" UMETA(Hidden)":string.Empty)} {(string.IsNullOrEmpty(desc)?"":$"UMETA(Tooltip = \"{desc}\")")}{(i < enumValues.Length ? "," : string.Empty)}");
                            }
                            if( !bit_flags )
                            {
                                writer.WriteLineEx($"{("Max".PadRight(namePadding))} UMETA(Hidden)");
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

