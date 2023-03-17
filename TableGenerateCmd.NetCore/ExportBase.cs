using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Google.Protobuf.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TableGenerateCmd;
using File = System.IO.File;

namespace TableGenerate
{
    public abstract class ExportBase
    {
        public abstract bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except);
    }

    public static class ExportBaseUtil
    {
        static bool FileEquals(string fileName1, string fileName2)
        {
            // Check the file size and CRC equality here.. if they are equal...    
            try
            {
                byte[] file1 = System.IO.File.ReadAllBytes(fileName1);
                byte[] file2 = System.IO.File.ReadAllBytes(fileName2);
                return StreamsContentsAreEqual(file1, file2);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            System.Console.WriteLine("return false");
            return false;
        }

        public static bool StreamsContentsAreEqual(byte[] bytes1, byte[] bytes2)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            {
                var md51 = md5.ComputeHash(bytes1, 0, bytes1.Length);
                var md52 = md5.ComputeHash(bytes2, 0, bytes2.Length);
                return Enumerable.SequenceEqual(md51, md52);
            }
        }

        public static void CheckReplaceFile( MemoryStream tempFile, string fileName2, bool usingPerforce)
        {
            var fileName1 = System.IO.Path.GetTempFileName();
            {
                using var file1 = new System.IO.FileStream(fileName1, FileMode.Create);
                tempFile.TryGetBuffer(out var buffer);
                file1.Write(buffer);
            }   
            fileName2 = System.IO.Path.GetFullPath(fileName2);
            if (FileEquals(fileName1, fileName2) == false)
            {
                if (usingPerforce && !fileName2.Contains(System.IO.Path.GetTempPath()) )
                {
                    string command = "add";
                    if (System.IO.File.Exists(fileName2))
                    {
                        var attributes = System.IO.File.GetAttributes(fileName2);
                        var isReadOnly = (attributes & FileAttributes.ReadOnly) != 0;
                        command = "edit";
                    }
                    Process.Start("p4", $"{command} {fileName2}").WaitForExit();
                }

                File.Copy(fileName1, fileName2, overwrite: true);
            }
            File.Delete(fileName1);
        }

        public static List<Column> GetColumnInfo(System.Reflection.Assembly refAssem, System.Reflection.Assembly mscorlibAssembly, string sheetName, StringWithDesc[,] rows, List<string> except)
        {
            var columns = new List<Column>();
            JsonSerializerSettings jsonSettings = new ()
            {
                Error =     (_, args) =>
                {
                    //Console.WriteLine($"Error:{args.ErrorContext.Error.Message}");
                    args.ErrorContext.Handled = true;
                }
            };
            for (int i = 0; i < rows.GetLength(1); i++)
            {
                string desc = rows[0,i] != null ? rows[0,i].Text : string.Empty;
                string name = rows[1,i].Text.Trim().Replace(' ', '_');
                string json = rows[2,i].Text;
                string generate = rows[3,i].Text.Trim().Replace(' ', '_').ToLower();
                string type = rows[4,i].Text.Trim().Replace(' ', '_');
                if (name.Length == 0)
                    continue;
                RangeValue range = null;
                if (!string.IsNullOrEmpty(json) && 
                    json.StartsWith('{') && json.EndsWith('}'))
                {
                    range = JsonConvert.DeserializeObject<RangeValue>(json, jsonSettings);
                }

                var column = new Column
                {
                    json = json,
                    array_one_cell = false,
                    is_key = i == 0,
                    data_column_index = i,
                    var_name = name,
                    is_generated = except.Count() == 0 || except.Exists(compare => generate.Trim().ToLower().Contains(compare) ) == false,
                    is_out_string = type.IndexOf("out_string") >= 0,
                    array_group_name = null,
                    array_index = -1,
                    is_array = false, 
                    desc = desc,
                    str_bit_flags = string.Empty,
                };

                GetBaseType(ref column, range, refAssem, mscorlibAssembly, type);

                if(column.is_array)
                {
                    if(generate.Contains(','))
                    {
                        var commaSplit = generate.Split(',');
                        foreach(var com in commaSplit)
                        {
                            var equalSplit = com.Split('=');
                            if(equalSplit.Length == 2)
                            {
                                switch(equalSplit[0].ToLower())
                                {
                                    case "index":
                                        column.array_index = int.Parse(equalSplit[1]);
                                        break;
                                    case "group":
                                        column.array_group_name = equalSplit[1].Trim();
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!int.TryParse(generate, out column.array_index))
                        {
                            column.array_index = -1;
                            column.is_last_array = false;
                        }
                        column.array_group_name = null;
                    }
                }
                else
                {
                    column.array_index = -1;
                    column.is_last_array = false;
                }

                if ( column.array_index >= 0)
                {
                    var duplicate_column_count = columns.Where(compare => compare.var_name == column.var_name && compare.array_index == column.array_index ).Count();
                    var name_column_count = columns.Where(compare => compare.var_name == column.var_name).Count();
                    if (duplicate_column_count > 0)
                    {
                        throw new System.Exception($"duplicate table:{sheetName} column name:{column.var_name} array_index:{column.array_index}");
                    }
                    if (name_column_count != column.array_index)
                    {
                        throw new System.Exception($"invalid array_index sheet:{sheetName} column name:{column.var_name} array_index:{column.array_index}");
                    }
                }

                columns.Add(column);
            }

            int key_index = columns.FindIndex(t => t.is_generated == true);
            for( int i=0; i < columns.Count();i++)
            {
                var column = columns[i];
                if (key_index == i)
                    column.is_key = true;
                else
                    column.is_key = false;

                if( column.array_index >=0)
                {
                    var max = columns.Where(t => t.var_name == column.var_name).Select(t => t.array_index).Max();
                    column.is_last_array = column.array_index == max;
                }
            }
            for (int i = 0; i < columns.Count(); i++)
            {
                var column = columns[i];
                if (column.array_index == 0)
                {
                    foreach(var col in columns.Where(t => t.var_name == column.var_name).OrderBy( t => t.array_index))
                    {
                        col.array_group_name = column.array_group_name;
                    }
                }
            }

            return columns;
        }

        public static string GetConvertFunction(this Column column, string arg, eGenType genType)
        {
            string returnTypeName = string.Empty;
            if(column.IsEnumType())
            {
                
                returnTypeName = $" System.Enum.Parse<{column.GetPrimitiveType(genType)}>( ({arg}.StartsWith(\"{column.type_name}_\")? {arg}.Substring(\"{column.type_name}_\".Length):{arg}).Replace(\"_\",\"\"), ignoreCase: true)";
            }
            else if(column.IsStructType())
            {
                returnTypeName = $"{column.primitive_type.GetConvertFunction(arg, genType)}";
            }
            else
            {
                returnTypeName = GetConvertFunction(column.base_type, arg, genType);
            }
            return returnTypeName;
        }

        private static string GetConvertFunction(this BaseType baseType, string arg, eGenType genType)
        {
            string returnTypeName = baseType switch
            {
                BaseType.Vector3 => $"Vector3.Parse({arg})",
                BaseType.Vector2 => $"Vector2.Parse({arg})",
                BaseType.Int64 => $"System.Convert.ToInt64(System.Math.Round(double.Parse({arg})))",
                BaseType.Int32 => $"System.Convert.ToInt32(System.Math.Round(double.Parse({arg})))",
                BaseType.Int16 => $"System.Convert.ToInt16(System.Math.Round(double.Parse({arg})))",
                BaseType.String => $"{arg}",
                BaseType.Float => $"System.Convert.ToSingle({arg})",
                BaseType.Double => $"System.Convert.ToDouble({arg})",
                BaseType.Int8 => $"System.Convert.ToByte({arg})",
                BaseType.Boolean => $"({arg}.Trim()==\"1\"||{arg}.Trim().ToUpper()==\"TRUE\")",
                BaseType.DateTime => $"System.DateTime.Parse({arg})",
                BaseType.TimeSpan => $"System.TimeSpan.Parse({arg})",
                _ => string.Empty
            };
            return returnTypeName;
        }

        public static string GetParseString(this Column column, string data)
        {
            string returnTypeName = string.Empty;
            switch (column.base_type)
            {
                case BaseType.Int64: returnTypeName    = long.Parse(data).ToString(); break;
                case BaseType.Int32: returnTypeName    = int.Parse(data).ToString(); break;
                case BaseType.Int16: returnTypeName    = short.Parse(data).ToString(); break;
                case BaseType.String: returnTypeName   = data; break;
                case BaseType.Float: returnTypeName    = float.Parse(data).ToString(); break;
                case BaseType.Double: returnTypeName   = double.Parse(data).ToString(); break;
                case BaseType.Int8: returnTypeName     = byte.Parse(data).ToString(); break;
                case BaseType.Boolean: returnTypeName  = (data.Trim()=="1"||data.Trim().ToUpper()=="TRUE")?"true":"false"; break;
                case BaseType.DateTime: returnTypeName = "/"; break;
                case BaseType.TimeSpan: returnTypeName = "/"; break;
            }
            return returnTypeName;
        }
        public static string ToParse(this Column column, string row)
        {
            string ret = string.Empty;
            return ret;
        }
        public static bool IsNumberType(this Column column)
        {
            switch (column.base_type)
            {
                case BaseType.String: return false;
                case BaseType.DateTime: return false;
                case BaseType.TimeSpan: return false;
            }
            return true;
        }
        public static bool IsDateTime(this Column column) => column.base_type == BaseType.DateTime;
        public static bool IsTimeSpan(this Column column) => column.base_type == BaseType.TimeSpan;
        public static bool IsEnumType(this Column column) => column.base_type == BaseType.Enum;
        public static bool IsStructType(this Column column) => column.base_type == BaseType.Struct;
        public static bool IsString(this Column column) => column.base_type == BaseType.String;
        public static bool IsVector3(this Column column) => column.base_type == BaseType.Vector3;
        public static bool IsVector2(this Column column) => column.base_type == BaseType.Vector2;

        public static string GetSqlitekitFunction(this Column column)
        {
            string returnTypeName = string.Empty;
            if (column.array_index >= 0)
            {
                string var_name = column.var_name + column.array_index;
                switch (column.base_type)
                {
                    case BaseType.Int64: returnTypeName    = $"qr__.GetLong(\"{var_name}\")"; break;
                    case BaseType.Int32: returnTypeName    = $"qr__.GetInteger(\"{var_name}\")"; break;
                    case BaseType.Int16: returnTypeName    = $"(short) qr__.GetInteger(\"{var_name}\")"; break;
                    case BaseType.String: returnTypeName   = $"qr__.GetString(\"{var_name}\")"; break;
                    case BaseType.Float: returnTypeName    = $"(float) qr__.GetDouble(\"{var_name}\")"; break;
                    case BaseType.Double: returnTypeName   = $"qr__.GetDouble(\"{var_name}\")"; break;
                    case BaseType.Int8: returnTypeName     = $"(byte) qr__.GetInteger(\"{var_name}\")"; break;
                    case BaseType.Boolean: returnTypeName  = $"qr__.GetInteger(\"{var_name}\")>0?true:false"; break;
                    case BaseType.DateTime: returnTypeName = $"System.DateTime.Parse(qr__.GetString(\"{var_name}\"))"; break;
                    case BaseType.TimeSpan: returnTypeName = $"System.TimeSpan.Parse(qr__.GetString(\"{var_name}\"))"; break;
                }
            }
            else
            {
                string var_name = column.var_name;
                switch (column.base_type)
                {
                    case BaseType.Int64: returnTypeName    = $"qr__.GetLong(\"{var_name}\")"; break;
                    case BaseType.Int32: returnTypeName    = $"qr__.GetInteger(\"{var_name}\")"; break;
                    case BaseType.Int16: returnTypeName    = $"(short) qr__.GetInteger(\"{var_name}\")"; break;
                    case BaseType.String: returnTypeName   = $"qr__.GetString(\"{var_name}\")"; break;
                    case BaseType.Float: returnTypeName    = $"(float) qr__.GetDouble(\"{var_name}\")"; break;
                    case BaseType.Double: returnTypeName   = $"qr__.GetDouble(\"{var_name}\")"; break;
                    case BaseType.Int8: returnTypeName     = $"(byte) qr__.GetInteger(\"{var_name}\")"; break;
                    case BaseType.Boolean: returnTypeName  = $"qr__.GetInteger(\"{var_name}\")>0?true:false"; break;
                    case BaseType.DateTime: returnTypeName = $"System.DateTime.Parse(qr__.GetString(\"{var_name}\"))"; break;
                    case BaseType.TimeSpan: returnTypeName = $"System.TimeSpan.Parse(qr__.GetString(\"{var_name}\"))"; break;
                }
            }
            return returnTypeName;
        }
        public static string GetReadStreamFunction(this Column column, eGenType genType)
        {
            string returnTypeName = string.Empty;
            if (column.array_index >= 0)
            {
                if(!column.IsEnumType() && !column.IsStructType() )
                    returnTypeName = GetReadStreamFunction(column.base_type);
                else
                    returnTypeName = $"({column.GetPrimitiveType(genType)}){GetReadStreamFunction(column.primitive_type)}";
            }
            else
            {
                if (!column.IsEnumType() && !column.IsStructType())
                    returnTypeName = GetReadStreamFunction(column.base_type);
                else
                    returnTypeName = $"({column.GetPrimitiveType(genType)}){GetReadStreamFunction(column.primitive_type)}";
            }
            return returnTypeName;
        }

        private static string GetReadStreamFunction(this BaseType baseType)
        {
            string returnTypeName = baseType switch
            {
                BaseType.Vector3 => $"  new Vector3(){{ X = __reader.ReadDouble(), Y = __reader.ReadDouble(), Z = __reader.ReadDouble() }}",
                BaseType.Vector2 => $"  new Vector2(){{ X = __reader.ReadDouble(), Y = __reader.ReadDouble() }}",
                BaseType.Int64 => $"__reader.ReadInt64()",
                BaseType.Int32 => $"__reader.ReadInt32()",
                BaseType.Int16 => $"__reader.ReadInt16()",
                BaseType.String => $"Encoder.ReadString(ref __reader)",
                BaseType.Float => $"__reader.ReadSingle()",
                BaseType.Double => $"__reader.ReadDouble()",
                BaseType.Int8 => $"__reader.ReadByte()",
                BaseType.Boolean => $"__reader.ReadBoolean()",
                BaseType.DateTime => $"System.DateTime.FromBinary(__reader.ReadInt64())",
                BaseType.TimeSpan => $"System.TimeSpan.FromTicks(__reader.ReadInt64())",
                _ => string.Empty
            };
            return returnTypeName;
        }

        private static void GetBaseType(ref Column column, RangeValue rangeValue, System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, string typename)
        {
            column.base_type = BaseType.Null;
            string type_name = typename;
            string subtypename = type_name;
            string sizeChecker = typename;
            int startIndex = sizeChecker.IndexOf('<');
            if (type_name.EndsWith("[]"))
            {
                column.is_array = true;
                column.array_one_cell = true;
                column.array_index = 0;
            }
            if(startIndex >= 0 || column.is_array)
            {
                int lastIndex = sizeChecker.LastIndexOf('>');
                if (type_name.StartsWith("array"))
                {
                    type_name = "array";
                    if (startIndex >= 0)
                    {
                        sizeChecker = sizeChecker.Substring(startIndex + 1, sizeChecker.Length - startIndex - 2);
                        sizeChecker = sizeChecker.Replace("array", string.Empty);
                        subtypename = sizeChecker;
                    }
                    column.is_array = true;
                }
                else if (column.is_array)
                {
                    sizeChecker = typename.Replace("[]", string.Empty);
                    subtypename = sizeChecker;
                    type_name = "array";
                    column.is_array = true;
                }
                if (subtypename.IndexOf("enum") >= 0)
                {
                    startIndex = subtypename.IndexOf('<');
                    if (startIndex >= 0)
                    {
                        sizeChecker = subtypename;
                        sizeChecker = sizeChecker.Substring(startIndex + 1, sizeChecker.Length - startIndex - 2);
                        column.type_name = sizeChecker;
                        column.base_type = BaseType.Enum;
                        if (refAssembly != null)
                        {
                            System.Reflection.TypeInfo type = refAssembly.DefinedTypes.FirstOrDefault(t => t.FullName.Equals(sizeChecker));
                            if( type == null)
                            {
                                type = mscorlibAssembly.DefinedTypes.FirstOrDefault(t => t.FullName.Equals(sizeChecker));

                                if(type == null && sizeChecker.Equals("System.DayOfWeek"))
                                {
                                    type = typeof(System.DayOfWeek).GetTypeInfo();
                                }
                                if (type == null && sizeChecker.Equals("System.DateTime"))
                                {
                                    type = typeof(System.DateTime).GetTypeInfo();
                                }
                                if (type == null && sizeChecker.Equals("System.TimeSpan"))
                                {
                                    type = typeof(System.TimeSpan).GetTypeInfo();
                                }

                                if (type is null)
                                {
                                    throw new Exception($"no exist enum {column.type_name}{Environment.NewLine}");
                                }
                            }
                            if (type != null)
                            {
                                column.TypeInfo = type;
                                {
                                    var types = type.DeclaredFields.Where(t => t.IsStatic).ToArray();
                                    column.min_value = $"global::{type.FullName}.{types.First().Name}";
                                    column.max_value = $"global::{type.FullName}.{types.Last().Name}";
                                }
                                var reflection = refAssembly.GetTypes().FirstOrDefault(t => t.Name == $"{type.Name}Reflection");
                                if (reflection != null)
                                {
                                    var propertyInfo = reflection.GetProperty("Descriptor");
                                    var fileDescriptor = propertyInfo.GetValue(null) as FileDescriptor;
                                    foreach (var fileDescriptorProto in fileDescriptor.Dependencies.Select(t=>t.ToProto()))
                                    {
                                        var flag = fileDescriptorProto.Extension.FirstOrDefault(t => t.Name.Equals("bit_flags"));
                                        if (flag != null)
                                        {
                                            var enumDescriptor = fileDescriptor.FindTypeByName<EnumDescriptor>(type.Name);
                                            var o = enumDescriptor.GetOptions();
                                            var value = o.GetExtension(BitFlagsExtensions.BitFlags);
                                            column.str_bit_flags = value;
                                        }
                                        break;
                                    }
                                }
                                var DeclaredField = type.DeclaredFields.First();
                                switch ("")
                                {
                                    case string anyName when DeclaredField.FieldType == typeof(System.SByte):
                                        column.primitive_type = BaseType.Int8;
                                        break;
                                    case string anyName when DeclaredField.FieldType == typeof(System.Byte):
                                        column.primitive_type = BaseType.Int8;
                                        break;
                                    case string anyName when DeclaredField.FieldType == typeof(System.Int16):
                                        column.primitive_type = BaseType.Int16;
                                        break;
                                    case string anyName when DeclaredField.FieldType == typeof(System.Int32):
                                        column.primitive_type = BaseType.Int32;
                                        break;
                                    case string anyName when DeclaredField.FieldType == typeof(System.Int64):
                                        column.primitive_type = BaseType.Int64;
                                        break;
                                }
                            }
                        }
                    }
                }
                else if (subtypename.IndexOf("struct") >= 0)
                {
                    startIndex = subtypename.IndexOf('<');
                    lastIndex = subtypename.LastIndexOf('>');
                    if (startIndex >= 0)
                    {
                        sizeChecker = subtypename;
                        sizeChecker = sizeChecker.Substring(startIndex + 1, sizeChecker.Length - startIndex - 2);
                        column.type_name = sizeChecker;
                        column.base_type = BaseType.Struct;
                        if (refAssembly != null)
                        {
                            System.Reflection.TypeInfo type = refAssembly.DefinedTypes.FirstOrDefault(t => t.FullName.Equals(sizeChecker));
                            if (type == null)
                            {
                                type = mscorlibAssembly.DefinedTypes.FirstOrDefault(t => t.FullName.Equals(sizeChecker));
                            }
                            if (type != null)
                            {
                                var Value = type.DeclaredFields.FirstOrDefault(t => t.Name == "Value");
                                column.min_value = $"0";
                                switch (Value.FieldType.Name)
                                {
                                    case "Byte":
                                        column.primitive_type = BaseType.Int8;
                                        break;
                                    case "SByte":
                                        column.primitive_type = BaseType.Int8;
                                        break;
                                    case "Int16":
                                        column.primitive_type = BaseType.Int16;
                                        break;
                                    case "Int32":
                                        column.primitive_type = BaseType.Int32;
                                        break;
                                    case "Int64":
                                        column.primitive_type = BaseType.Int64;
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    subtypename = subtypename.ToLower();
                }
            }
            else
            {
                type_name = type_name.ToLower();
            }
            if (string.IsNullOrEmpty(column.type_name) )
            {
                column.type_name = type_name;
            }

            void SetMinMax(Column column, RangeValue rangeValue)
            {
                if (rangeValue == null) return;
                if(rangeValue.Min.HasValue) column.min_value = rangeValue.Min.Value.ToString();
                if(rangeValue.Max.HasValue) column.max_value = rangeValue.Max.Value.ToString();
            }

            switch (type_name)
            {
                case "vector3": column.base_type = BaseType.Vector3; break;
                case "vector2": column.base_type = BaseType.Vector2; break;
                case "long": column.base_type = BaseType.Int64; SetMinMax(column,rangeValue);  break;
                case "int64": column.base_type = BaseType.Int64; SetMinMax(column,rangeValue); break;
                case "int": column.base_type = BaseType.Int32; SetMinMax(column,rangeValue); break;
                case "int32": column.base_type = BaseType.Int32; SetMinMax(column,rangeValue); break;
                case "int16": column.base_type = BaseType.Int16; SetMinMax(column,rangeValue); break;
                case "short": column.base_type = BaseType.Int16; SetMinMax(column,rangeValue); break;
                case "string": column.base_type = BaseType.String; break;
                case "float": column.base_type = BaseType.Float; SetMinMax(column,rangeValue); break;
                case "double": column.base_type = BaseType.Double; SetMinMax(column,rangeValue); break;
                case "int8": column.base_type = BaseType.Int8; SetMinMax(column,rangeValue); break;
                case "byte": column.base_type = BaseType.Int8; SetMinMax(column,rangeValue); break;
                case "bool": column.base_type = BaseType.Boolean; break;
                case "datetime": column.base_type = BaseType.DateTime; break;
                case "timespan": column.base_type = BaseType.TimeSpan; break;
                case "array":
                    {
                        switch (subtypename)
                        {
                            case "vector3": column.base_type = BaseType.Vector3; break;
                            case "vector2": column.base_type = BaseType.Vector2; break;
                            case "long": column.base_type = BaseType.Int64; SetMinMax(column,rangeValue); break;
                            case "int64": column.base_type = BaseType.Int64; SetMinMax(column,rangeValue); break;
                            case "int": column.base_type = BaseType.Int32; SetMinMax(column,rangeValue); break;
                            case "int32": column.base_type = BaseType.Int32; SetMinMax(column,rangeValue); break;
                            case "short": column.base_type = BaseType.Int16; SetMinMax(column,rangeValue); break;
                            case "int16": column.base_type = BaseType.Int16; SetMinMax(column,rangeValue); break;
                            case "string": column.base_type = BaseType.String;SetMinMax(column,rangeValue); break;
                            case "float": column.base_type = BaseType.Float; SetMinMax(column,rangeValue); break;
                            case "double": column.base_type = BaseType.Double; SetMinMax(column,rangeValue); break;
                            case "int8": column.base_type = BaseType.Int8; SetMinMax(column,rangeValue); break;
                            case "byte": column.base_type = BaseType.Int8; SetMinMax(column,rangeValue); break;
                            case "bool": column.base_type = BaseType.Boolean; break;
                            case "datetime": column.base_type = BaseType.DateTime; break;
                            case "timespan": column.base_type = BaseType.TimeSpan; break;
                        }
                    }
                    break;
            }
        }

        public static string GetPrimitiveType(this Column column, eGenType gen_type)
        {
            string returnTypeName = string.Empty;
            switch (column.base_type)
            {
               case BaseType.Vector3:    returnTypeName = "Vector3"; break;
               case BaseType.Vector2:    returnTypeName = "Vector2"; break;
               case BaseType.Int64:    returnTypeName = "long"; break;
               case BaseType.Int32:    returnTypeName = "int"; break;
               case BaseType.Int16:    returnTypeName = "short"; break;
               case BaseType.String:   returnTypeName = "string"; break;
               case BaseType.Float:    returnTypeName = "float"; break;
               case BaseType.Double:   returnTypeName = "double"; break;
               case BaseType.Int8:     returnTypeName = "byte"; break;
               case BaseType.Boolean:  returnTypeName = "bool"; break;
               case BaseType.DateTime: returnTypeName = "datetime"; break;
               case BaseType.TimeSpan: returnTypeName = "timespan"; break;
               case BaseType.Enum: returnTypeName = column.type_name; break;
               case BaseType.Struct: returnTypeName = column.type_name; break;
            }
            return returnTypeName;
        }

        public static string GetInitValue(this Column column, eGenType gen_type)
        {
            string returnTypeName = string.Empty;
            if(gen_type == eGenType.cs)
            {
                switch (column.base_type)
                {
                    case BaseType.Vector3: returnTypeName = "default"; break;
                    case BaseType.Vector2: returnTypeName = "default"; break;
                    case BaseType.Int64: returnTypeName = "0"; break;
                    case BaseType.Int32: returnTypeName = "0"; break;
                    case BaseType.Int16: returnTypeName = "0"; break;
                    case BaseType.String: returnTypeName = "string.Empty"; break;
                    case BaseType.Float: returnTypeName = "0"; break;
                    case BaseType.Double: returnTypeName = "0"; break;
                    case BaseType.Int8: returnTypeName = "0"; break;
                    case BaseType.Boolean: returnTypeName = "false"; break;
                    case BaseType.DateTime: returnTypeName = "datetime"; break;
                    case BaseType.TimeSpan: returnTypeName = "TimeSpan"; break;
                    case BaseType.Enum: returnTypeName = column.min_value; break;
                    case BaseType.Struct: returnTypeName = column.min_value; break;
                }
            }
            else if (gen_type is eGenType.sqllite or eGenType.mssql or eGenType.mysql) 
            {
                switch (column.base_type)
                {
                    case BaseType.Vector3:       returnTypeName = "[0,0,0]"; break;
                    case BaseType.Vector2:       returnTypeName = "[0,0]"; break;
                    case BaseType.Int64:       returnTypeName = "0"; break;
                    case BaseType.Int32:       returnTypeName = "0"; break;
                    case BaseType.Int16:       returnTypeName = "0"; break;
                    case BaseType.String:      returnTypeName = "''"; break;
                    case BaseType.Float:       returnTypeName = "0"; break;
                    case BaseType.Double:      returnTypeName = "0"; break;
                    case BaseType.Int8:        returnTypeName = "0"; break;
                    case BaseType.Boolean:     returnTypeName = "0"; break;
                    case BaseType.DateTime:    returnTypeName = "''"; break;
                    case BaseType.TimeSpan:    returnTypeName = "''"; break;
                    case BaseType.Enum:    returnTypeName = "0"; break;
                    case BaseType.Struct: returnTypeName = "0"; break;
                }
            }
            return returnTypeName;
        }
        public static string GetEqualityTypeValue(this Column column, eGenType genType)
        {
            string returnTypeName = string.Empty;
            if (genType == eGenType.cs)
            {
                returnTypeName = column.base_type switch
                {
                    BaseType.Int64 => "default(LongEqualityComparer)",
                    BaseType.Int32 => "default(IntEqualityComparer)",
                    BaseType.Int16 => "default(ShortEqualityComparer)",
                    BaseType.String => "default(StringEqualityComparer)",
                    BaseType.Float => "",
                    BaseType.Double => "",
                    BaseType.Int8 => "",
                    BaseType.Boolean => "",
                    BaseType.DateTime => "",
                    BaseType.TimeSpan => "",
                    BaseType.Enum => $"",
                    BaseType.Struct => $"",
                    _ => returnTypeName
                };
            }
            return returnTypeName;
        }
        public static string GenerateType(this Column column, eGenType gen_type)
        {
            int array_count = column.array_index;
            string returnTypeName = string.Empty;
            if (array_count != -1 || column.array_one_cell)
            {
                switch (column.base_type)
                {
                    case BaseType.Vector3:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "TArray<FVector>"; break;
                            case eGenType.cs: returnTypeName = "Vector3[]"; break;
                            case eGenType.proto: returnTypeName = "repeated Vector3"; break;
                            case eGenType.mssql: returnTypeName = "Vector3"; break;
                            case eGenType.mysql: returnTypeName = "Vector3"; break;
                            case eGenType.sqllite: returnTypeName = "Vector3"; break;
                            case eGenType.rust: returnTypeName = "Vec<glam::f64::DVec3>"; break;
                        }
                    }
                    break;
                    case BaseType.Vector2:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "TArray<FVector2D>"; break;
                            case eGenType.cs: returnTypeName = "Vector2[]"; break;
                            case eGenType.proto: returnTypeName = "repeated Vector2"; break;
                            case eGenType.mssql: returnTypeName = "Vector2"; break;
                            case eGenType.mysql: returnTypeName = "Vector2"; break;
                            case eGenType.sqllite: returnTypeName = "Vector2"; break;
                            case eGenType.rust: returnTypeName = "Vec<glam::f64::DVec2>"; break;
                        }
                    }
                    break;  
                    case BaseType.Int64:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int64>"; break;
                                case eGenType.cs: returnTypeName = "long[]"; break;
                                case eGenType.proto: returnTypeName = "repeated int64"; break;
                                case eGenType.mssql: returnTypeName = "bigint"; break;
                                case eGenType.mysql: returnTypeName = "bigint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i64>"; break;
                            }
                        }
                        break;
                    case BaseType.Int32:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int32>"; break;
                                case eGenType.cs: returnTypeName = "int[]"; break;
                                case eGenType.proto: returnTypeName = "repeated int32"; break;
                                case eGenType.mssql: returnTypeName = "int"; break;
                                case eGenType.mysql: returnTypeName = "int"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i32>"; break;
                            }
                        }
                        break;
                    case BaseType.Int16:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int16>"; break;
                                case eGenType.cs: returnTypeName = "short[]"; break;
                                case eGenType.proto: returnTypeName = "repeated int32"; break;
                                case eGenType.mssql: returnTypeName = "smallint"; break;
                                case eGenType.mysql: returnTypeName = "smallint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i16>"; break;
                            }
                        }
                        break;
                    case BaseType.String:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<FString>"; break;
                                case eGenType.cs: returnTypeName = "string[]"; break;
                                case eGenType.proto: returnTypeName = "repeated string"; break;
                                case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                                case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                                case eGenType.rust: returnTypeName = "Vec<String>"; break;
                            }
                        }
                        break;
                    case BaseType.Float:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<float>"; break;
                                case eGenType.cs: returnTypeName = "float[]"; break;
                                case eGenType.proto: returnTypeName = "repeated float"; break;
                                case eGenType.mssql: returnTypeName = "float"; break;
                                case eGenType.mysql: returnTypeName = "float"; break;
                                case eGenType.sqllite: returnTypeName = "real"; break;
                                case eGenType.rust: returnTypeName = "Vec<f32>"; break;
                            }
                        }
                        break;
                    case BaseType.Double:
                        {

                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<double>"; break;
                                case eGenType.cs: returnTypeName = "double[]"; break;
                                case eGenType.proto: returnTypeName = "repeated double"; break;
                                case eGenType.mssql: returnTypeName = "double"; break;
                                case eGenType.mysql: returnTypeName = "double"; break;
                                case eGenType.sqllite: returnTypeName = "real"; break;
                                case eGenType.rust: returnTypeName = "Vec<f64>"; break;
                            }
                        }
                        break;
                    case BaseType.Int8:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int8>"; break;
                                case eGenType.cs: returnTypeName = "byte[]"; break;
                                case eGenType.proto: returnTypeName = "repeated int8"; break;
                                case eGenType.mssql: returnTypeName = "tinyint"; break;
                                case eGenType.mysql: returnTypeName = "tinyint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i8>"; break;
                            }
                        }
                        break;
                    case BaseType.Boolean:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<bool>"; break;
                                case eGenType.cs: returnTypeName = "bool[]"; break;
                                case eGenType.proto: returnTypeName = "repeated bool"; break;
                                case eGenType.mssql: returnTypeName = "bit"; break;
                                case eGenType.mysql: returnTypeName = "bool"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<bool>"; break;
                            }
                        }
                        break;
                    case BaseType.DateTime:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<FDateTime>"; break;
                                case eGenType.cs: returnTypeName = "System.DateTime[]"; break;
                                case eGenType.proto: returnTypeName = "repeated int64"; break;
                                case eGenType.mssql: returnTypeName = "datetime"; break;
                                case eGenType.mysql: returnTypeName = "datetime"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                                case eGenType.rust: returnTypeName = "Vec<chrono::DateTime>"; break;
                            }
                        }
                        break;
                    case BaseType.TimeSpan:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<Timespan>"; break;
                                case eGenType.cs: returnTypeName = "System.TimeSpan[]"; break;
                                case eGenType.proto: returnTypeName = "repeated int64"; break;
                                case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                                case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                                case eGenType.rust: returnTypeName = "Vec<timespan::DateTimeSpan>"; break;
                            }
                        }
                        break;
                    case BaseType.Enum:
                    case BaseType.Struct:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = $"TArray<{(column.IsEnumType()?"E":"F")}{column.type_name}>"; break;
                                case eGenType.cs: returnTypeName = $"{column.type_name}[]"; break;
                                case eGenType.proto: returnTypeName = $"repeated {column.type_name.Split('.').Last()}"; break;
                                case eGenType.mssql: returnTypeName = "int"; break;
                                case eGenType.mysql: returnTypeName = "int"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = $"Vec<{column.type_name}>"; break;
                            }
                        }
                        break;
                }
                return returnTypeName;
            }

            switch (column.base_type)
            {
                case BaseType.Vector3:
                {
                    switch (gen_type)
                    {
                        case eGenType.cpp: returnTypeName = "FVector"; break;
                        case eGenType.cs: returnTypeName = "Vector3"; break;
                        case eGenType.proto: returnTypeName = "Vector3"; break;
                        case eGenType.mssql: returnTypeName = "Vector3"; break;
                        case eGenType.mysql: returnTypeName = "Vector3"; break;
                        case eGenType.sqllite: returnTypeName = "Vector3"; break;
                        case eGenType.rust: returnTypeName = "glam::f64::DVec3"; break;
                    }
                }
                break;
                case BaseType.Vector2:
                {
                    switch (gen_type)
                    {
                        case eGenType.cpp: returnTypeName = "FVector2D"; break;
                        case eGenType.cs: returnTypeName = "Vector2"; break;
                        case eGenType.proto: returnTypeName = "Vector2"; break;
                        case eGenType.mssql: returnTypeName = "Vector2"; break;
                        case eGenType.mysql: returnTypeName = "Vector2"; break;
                        case eGenType.sqllite: returnTypeName = "Vector2"; break;
                        case eGenType.rust: returnTypeName = "glam::f64::DVec2"; break;
                    }
                }
                break;                
                case BaseType.Int64:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int64"; break;
                            case eGenType.cs: returnTypeName = "long"; break;
                            case eGenType.proto: returnTypeName = "int64"; break;
                            case eGenType.mssql: returnTypeName = "bigint"; break;
                            case eGenType.mysql: returnTypeName = "bigint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i64"; break;
                        }
                    }
                    break;
                case BaseType.Int32: 
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int32"; break;
                            case eGenType.cs: returnTypeName = "int"; break;
                            case eGenType.proto: returnTypeName = "int32"; break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i32"; break;
                        }
                    }
                    break;
                case BaseType.Int16:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int16"; break;
                            case eGenType.cs: returnTypeName = "short"; break;
                            case eGenType.proto: returnTypeName = "int32"; break;
                            case eGenType.mssql: returnTypeName = "smallint"; break;
                            case eGenType.mysql: returnTypeName = "smallint"; break;
                            case eGenType.sqllite: returnTypeName = "smallint"; break;
                            case eGenType.rust: returnTypeName = "i16"; break;
                        }
                    }
                    break;
                case BaseType.String:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FString"; break;
                            case eGenType.cs: returnTypeName = "string"; break;
                            case eGenType.proto: returnTypeName = "string"; break;
                            case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "String"; break;
                        }
                    }
                    break;
                case BaseType.Float:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "float"; break;
                            case eGenType.cs: returnTypeName = "float"; break;
                            case eGenType.proto: returnTypeName = "float"; break;
                            case eGenType.mssql: returnTypeName = "float"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f32"; break;
                        }
                    }
                    break;
                case BaseType.Double:
                    {

                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "double"; break;
                            case eGenType.cs: returnTypeName = "double"; break;
                            case eGenType.proto: returnTypeName = "double"; break;
                            case eGenType.mssql: returnTypeName = "double"; break;
                            case eGenType.mysql: returnTypeName = "double"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f64"; break;
                        }
                    }
                    break;
                case BaseType.Int8:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "uint8"; break;
                            case eGenType.cs: returnTypeName = "byte"; break;
                            case eGenType.proto: returnTypeName = "byte"; break;
                            case eGenType.mssql: returnTypeName = "tinyint"; break;
                            case eGenType.mysql: returnTypeName = "tinyint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i8"; break;
                        }
                    }
                    break;
                case BaseType.Boolean:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "bool"; break;
                            case eGenType.cs: returnTypeName = "bool"; break;
                            case eGenType.proto: returnTypeName = "bool"; break;
                            case eGenType.mssql: returnTypeName = "bit"; break;
                            case eGenType.mysql: returnTypeName = "bool"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "bool"; break;
                        }
                    }
                    break;
                case BaseType.DateTime:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FDateTime"; break;
                            case eGenType.cs: returnTypeName = "System.DateTime"; break;
                            case eGenType.proto: returnTypeName = "int64"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "chrono::DateTime"; break;
                        }
                    }
                    break;
                case BaseType.TimeSpan:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "Timespan"; break;
                            case eGenType.cs: returnTypeName = "System.TimeSpan"; break;
                            case eGenType.proto: returnTypeName = "int64"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "timespan::DateTimeSpan"; break;
                        }
                    }
                    break;
                case BaseType.Enum:
                case BaseType.Struct:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp when !column.bit_flags: returnTypeName = $"{(column.IsEnumType()?"E":"F")}{column.type_name}"; break;
                            case eGenType.cpp when column.bit_flags: returnTypeName = $"int32"; break;
                            case eGenType.cs: returnTypeName = column.type_name; break;
                            case eGenType.proto: returnTypeName = column.type_name.Split('.').Last(); break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = column.type_name; break;
                        }
                    }
                    break;

            }
            return returnTypeName;
        }
        public static string GenerateDefaultValue(this Column column, eGenType gen_type)
        {
            int array_count = column.array_index;
            string returnTypeName = string.Empty;
            if (array_count != -1 || column.array_one_cell)
            {
                switch (column.base_type)
                {
                    case BaseType.Vector3:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "TArray<FVector>::TArray()"; break;
                            case eGenType.cs: returnTypeName = "System.Array.Empty<Vector3>()"; break;
                            case eGenType.proto: returnTypeName = "-"; break;
                            case eGenType.mssql: returnTypeName = "-"; break;
                            case eGenType.mysql: returnTypeName = "-"; break;
                            case eGenType.sqllite: returnTypeName = "-"; break;
                            case eGenType.rust: returnTypeName = "Vec::Empty<glam::f64::DVec3>()"; break;
                        }
                    }
                    break;
                    case BaseType.Vector2:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "TArray<FVector2D>::TArray()"; break;
                            case eGenType.cs: returnTypeName = "System.Array.Empty<Vector2D>()"; break;
                            case eGenType.proto: returnTypeName = "-"; break;
                            case eGenType.mssql: returnTypeName = "-"; break;
                            case eGenType.mysql: returnTypeName = "-"; break;
                            case eGenType.sqllite: returnTypeName = "-"; break;
                            case eGenType.rust: returnTypeName = "Vec::Empty<glam::f64::DVec2>()"; break;
                        }
                    }
                        break;
                    case BaseType.Int64:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int64>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<long>()"; break;
                                case eGenType.proto: returnTypeName = "-"; break;
                                case eGenType.mssql: returnTypeName = "-"; break;
                                case eGenType.mysql: returnTypeName = "-"; break;
                                case eGenType.sqllite: returnTypeName = "-"; break;
                                case eGenType.rust: returnTypeName = "-"; break;
                            }
                        }
                        break;
                    case BaseType.Int32:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int32>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<int>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int32"; break;
                                case eGenType.mssql: returnTypeName = "int"; break;
                                case eGenType.mysql: returnTypeName = "int"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i32>"; break;
                            }
                        }
                        break;
                    case BaseType.Int16:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int16>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<short>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int32"; break;
                                case eGenType.mssql: returnTypeName = "smallint"; break;
                                case eGenType.mysql: returnTypeName = "smallint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i16>"; break;
                            }
                        }
                        break;
                    case BaseType.String:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<FString>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<string>()"; break;
                                case eGenType.proto: returnTypeName = "repeated string"; break;
                                case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                                case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                                case eGenType.rust: returnTypeName = "Vec<String>"; break;
                            }
                        }
                        break;
                    case BaseType.Float:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<float>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<float>()"; break;
                                case eGenType.proto: returnTypeName = "repeated float"; break;
                                case eGenType.mssql: returnTypeName = "float"; break;
                                case eGenType.mysql: returnTypeName = "float"; break;
                                case eGenType.sqllite: returnTypeName = "real"; break;
                                case eGenType.rust: returnTypeName = "Vec<f32>"; break;
                            }
                        }
                        break;
                    case BaseType.Double:
                        {

                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<double>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<double>()"; break;
                                case eGenType.proto: returnTypeName = "repeated double"; break;
                                case eGenType.mssql: returnTypeName = "double"; break;
                                case eGenType.mysql: returnTypeName = "double"; break;
                                case eGenType.sqllite: returnTypeName = "real"; break;
                                case eGenType.rust: returnTypeName = "Vec<f64>"; break;
                            }
                        }
                        break;
                    case BaseType.Int8:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int8>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<byte>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int8"; break;
                                case eGenType.mssql: returnTypeName = "tinyint"; break;
                                case eGenType.mysql: returnTypeName = "tinyint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i8>"; break;
                            }
                        }
                        break;
                    case BaseType.Boolean:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<bool>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<bool>()"; break;
                                case eGenType.proto: returnTypeName = "repeated bool"; break;
                                case eGenType.mssql: returnTypeName = "bit"; break;
                                case eGenType.mysql: returnTypeName = "bool"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<bool>"; break;
                            }
                        }
                        break;
                    case BaseType.DateTime:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<FDateTime>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<System.DateTime>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int64"; break;
                                case eGenType.mssql: returnTypeName = "datetime"; break;
                                case eGenType.mysql: returnTypeName = "datetime"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                                case eGenType.rust: returnTypeName = "Vec<chrono::DateTime>"; break;
                            }
                        }
                        break;
                    case BaseType.TimeSpan:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<Timespan>::TArray()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<System.TimeSpan>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int64"; break;
                                case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                                case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                                case eGenType.rust: returnTypeName = "Vec<timespan::DateTimeSpan>"; break;
                            }
                        }
                        break;
                    case BaseType.Enum:
                    case BaseType.Struct:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp when !column.bit_flags: returnTypeName = $"TArray<{(column.IsEnumType()?"E":"F")}{column.type_name}>::TArray()"; break;
                                case eGenType.cpp when column.bit_flags: returnTypeName = "TArray<int32>::TArray()"; break;
                                case eGenType.cs: returnTypeName = $"System.Array.Empty<{column.type_name}>()"; break;
                                case eGenType.proto: returnTypeName = $"repeated {column.type_name.Split('.').Last()}"; break;
                                case eGenType.mssql: returnTypeName = "int"; break;
                                case eGenType.mysql: returnTypeName = "int"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i32>"; break;
                            }
                        }
                        break;
                }
                return returnTypeName;
            }

            switch (column.base_type)
            {
                case BaseType.Vector3:
                {
                    switch (gen_type)
                    {
                        case eGenType.cpp: returnTypeName = "FVector::ZeroVector"; break;
                        case eGenType.cs: returnTypeName = "default"; break;
                        case eGenType.proto: returnTypeName = "Vector3"; break;
                        case eGenType.mssql: returnTypeName = "Vector3"; break;
                        case eGenType.mysql: returnTypeName = "Vector3"; break;
                        case eGenType.sqllite: returnTypeName = "Vector3"; break;
                        case eGenType.rust: returnTypeName = "glam::f64::DVec3"; break;
                    }
                }
                break;           
                case BaseType.Vector2:
                {
                    switch (gen_type)
                    {
                        case eGenType.cpp: returnTypeName = "FVector2D::ZeroVector"; break;
                        case eGenType.cs: returnTypeName = "default"; break;
                        case eGenType.proto: returnTypeName = "Vector3"; break;
                        case eGenType.mssql: returnTypeName = "Vector3"; break;
                        case eGenType.mysql: returnTypeName = "Vector3"; break;
                        case eGenType.sqllite: returnTypeName = "Vector3"; break;
                        case eGenType.rust: returnTypeName = "glam::f64::DVec3"; break;
                    }
                }
                break;   
                case BaseType.Int64:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "0"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "int64"; break;
                            case eGenType.mssql: returnTypeName = "bigint"; break;
                            case eGenType.mysql: returnTypeName = "bigint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i64"; break;
                        }
                    }
                    break;
                case BaseType.Int32: 
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "0"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "int32"; break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i32"; break;
                        }
                    }
                    break;
                case BaseType.Int16:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "0"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "int32"; break;
                            case eGenType.mssql: returnTypeName = "smallint"; break;
                            case eGenType.mysql: returnTypeName = "smallint"; break;
                            case eGenType.sqllite: returnTypeName = "smallint"; break;
                            case eGenType.rust: returnTypeName = "i16"; break;
                        }
                    }
                    break;
                case BaseType.String:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FString(TEXT(\"\"))"; break;
                            case eGenType.cs: returnTypeName = "string.Empty"; break;
                            case eGenType.proto: returnTypeName = "string"; break;
                            case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "String"; break;
                        }
                    }
                    break;
                case BaseType.Float:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "0"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "float"; break;
                            case eGenType.mssql: returnTypeName = "float"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f32"; break;
                        }
                    }
                    break;
                case BaseType.Double:
                    {

                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "0"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "double"; break;
                            case eGenType.mssql: returnTypeName = "double"; break;
                            case eGenType.mysql: returnTypeName = "double"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f64"; break;
                        }
                    }
                    break;
                case BaseType.Int8:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "0"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "byte"; break;
                            case eGenType.mssql: returnTypeName = "tinyint"; break;
                            case eGenType.mysql: returnTypeName = "tinyint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i8"; break;
                        }
                    }
                    break;
                case BaseType.Boolean:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "false"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "bool"; break;
                            case eGenType.mssql: returnTypeName = "bit"; break;
                            case eGenType.mysql: returnTypeName = "bool"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "bool"; break;
                        }
                    }
                    break;
                case BaseType.DateTime:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FDateTime::FDateTime()"; break;
                            case eGenType.cs: returnTypeName = "System.DateTime.MinValue"; break;
                            case eGenType.proto: returnTypeName = "int64"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "chrono::DateTime"; break;
                        }
                    }
                    break;
                case BaseType.TimeSpan:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "Timespan::Timespan()"; break;
                            case eGenType.cs: returnTypeName = "System.TimeSpan.MinValue"; break;
                            case eGenType.proto: returnTypeName = "int64"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "timespan::DateTimeSpan"; break;
                        }
                    }
                    break;
                case BaseType.Enum:
                case BaseType.Struct:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp when !column.bit_flags: returnTypeName = $"E{column.type_name}::{column.min_value.Split(".").Last()}"; break;
                            case eGenType.cpp when column.bit_flags: returnTypeName = $"0"; break;
                            case eGenType.cpp: returnTypeName = $"E{column.type_name}::{column.min_value.Split(".").Last()}"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = column.type_name.Split('.').Last(); break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i32"; break;
                        }
                    }
                    break;

            }
            return returnTypeName;
        }
        public static string GenerateBaseType(this Column column, eGenType gen_type)
        {
            string returnTypeName = string.Empty;
            switch (column.base_type)
            {
                case BaseType.Vector3:
                {
                    switch (gen_type)
                    {
                        case eGenType.cpp: returnTypeName = "FVector"; break;
                        case eGenType.cs: returnTypeName = "Vector3"; break;
                        case eGenType.mssql: returnTypeName = "Vector3"; break;
                        case eGenType.mysql: returnTypeName = "Vector3"; break;
                        case eGenType.sqllite: returnTypeName = "Vector3"; break;
                        case eGenType.rust: returnTypeName = "glam::f64::DVec3"; break;
                    }
                }
                break;       
                case BaseType.Vector2:
                {
                    switch (gen_type)
                    {
                        case eGenType.cpp: returnTypeName = "FVector2D"; break;
                        case eGenType.cs: returnTypeName = "Vector2"; break;
                        case eGenType.mssql: returnTypeName = "Vector2"; break;
                        case eGenType.mysql: returnTypeName = "Vector2"; break;
                        case eGenType.sqllite: returnTypeName = "Vector2"; break;
                        case eGenType.rust: returnTypeName = "glam::f64::DVec2"; break;
                    }
                }
                break;   
                case BaseType.Int64:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int64"; break;
                            case eGenType.cs: returnTypeName = "long"; break;
                            case eGenType.mssql: returnTypeName = "bigint"; break;
                            case eGenType.mysql: returnTypeName = "bigint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i64"; break;
                        }
                    }
                    break;
                case BaseType.Int32:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int32"; break;
                            case eGenType.cs: returnTypeName = "int"; break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i32"; break;
                        }
                    }
                    break;
                case BaseType.Int16:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int16"; break;
                            case eGenType.cs: returnTypeName = "short"; break;
                            case eGenType.mssql: returnTypeName = "smallint"; break;
                            case eGenType.mysql: returnTypeName = "smallint"; break;
                            case eGenType.sqllite: returnTypeName = "smallint"; break;
                            case eGenType.rust: returnTypeName = "i16"; break;
                        }
                    }
                    break;
                case BaseType.String:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FString"; break;
                            case eGenType.cs: returnTypeName = "string"; break;
                            case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "String"; break;
                        }
                    }
                    break;
                case BaseType.Float:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "float"; break;
                            case eGenType.cs: returnTypeName = "float"; break;
                            case eGenType.mssql: returnTypeName = "float"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f32"; break;
                        }
                    }
                    break;
                case BaseType.Double:
                    {

                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "double"; break;
                            case eGenType.cs: returnTypeName = "double"; break;
                            case eGenType.mssql: returnTypeName = "double"; break;
                            case eGenType.mysql: returnTypeName = "double"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f64"; break;
                        }
                    }
                    break;
                case BaseType.Int8:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "uint8"; break;
                            case eGenType.cs: returnTypeName = "byte"; break;
                            case eGenType.mssql: returnTypeName = "tinyint"; break;
                            case eGenType.mysql: returnTypeName = "tinyint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i8"; break;
                        }
                    }
                    break;
                case BaseType.Boolean:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "bool"; break;
                            case eGenType.cs: returnTypeName = "bool"; break;
                            case eGenType.proto: returnTypeName = "bool"; break;
                            case eGenType.mssql: returnTypeName = "bit"; break;
                            case eGenType.mysql: returnTypeName = "bool"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "bool"; break;
                        }
                    }
                    break;
                case BaseType.DateTime:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FDateTime"; break;
                            case eGenType.cs: returnTypeName = "System.DateTime"; break;
                            case eGenType.mssql: returnTypeName = "datetime"; break;
                            case eGenType.mysql: returnTypeName = "datetime"; break;
                            case eGenType.sqllite: returnTypeName = "date"; break;
                            case eGenType.rust: returnTypeName = "chrono::DateTime"; break;
                        }
                    }
                    break;
                case BaseType.TimeSpan:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "Timespan"; break;
                            case eGenType.cs: returnTypeName = "System.TimeSpan"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "timespan::DateTimeSpan"; break;
                        }
                    }
                    break;
                case BaseType.Enum:
                case BaseType.Struct:
                    {
                        //returnTypeName = column.primitive_type.GenerateBaseType(gen_type);
                        
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = column.type_name.Replace(".","::"); break;
                            case eGenType.cs: returnTypeName = column.type_name; break;
                            case eGenType.mssql: returnTypeName = "???"; break;
                            case eGenType.mysql: returnTypeName = "???"; break;
                            case eGenType.sqllite: returnTypeName = "???"; break;
                            case eGenType.rust: returnTypeName = "i32"; break;
                        }
                    }
                    break;
            }
            return returnTypeName;
        }

        public static string GenerateBaseType(this BaseType base_type, eGenType gen_type)
        {
            string returnTypeName = string.Empty;
            switch (base_type)
            {
                case BaseType.Vector3:
                {
                    switch (gen_type)
                    {
                        case eGenType.cpp: returnTypeName = "FVector"; break;
                        case eGenType.cs: returnTypeName = "Vector3"; break;
                        case eGenType.mssql: returnTypeName = "Vector3"; break;
                        case eGenType.mysql: returnTypeName = "Vector3"; break;
                        case eGenType.sqllite: returnTypeName = "Vector3"; break;
                        case eGenType.rust: returnTypeName = "glam::f64::DVec3"; break;
                    }
                }
                break;   
                case BaseType.Vector2:
                {
                    switch (gen_type)
                    {
                        case eGenType.cpp: returnTypeName = "FVector2D"; break;
                        case eGenType.cs: returnTypeName = "Vector2"; break;
                        case eGenType.mssql: returnTypeName = "Vector2"; break;
                        case eGenType.mysql: returnTypeName = "Vector2"; break;
                        case eGenType.sqllite: returnTypeName = "Vector2"; break;
                        case eGenType.rust: returnTypeName = "glam::f64::DVec2"; break;
                    }
                }
                break;        
                case BaseType.Int64:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int64"; break;
                            case eGenType.cs: returnTypeName = "long"; break;
                            case eGenType.mssql: returnTypeName = "bigint"; break;
                            case eGenType.mysql: returnTypeName = "bigint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i64"; break;
                        }
                    }
                    break;
                case BaseType.Enum:
                {
                    switch (gen_type)
                    {
                        case eGenType.cpp: returnTypeName = "int32"; break;
                        case eGenType.cs: returnTypeName = "int"; break;
                        case eGenType.mssql: returnTypeName = "int"; break;
                        case eGenType.mysql: returnTypeName = "int"; break;
                        case eGenType.sqllite: returnTypeName = "integer"; break;
                        case eGenType.rust: returnTypeName = "i32"; break;
                    }
                }
                break;                
                case BaseType.Int32:
                    {
                        
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int32"; break;
                            case eGenType.cs: returnTypeName = "int"; break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i32"; break;
                        }
                    }
                    break;
                case BaseType.Int16:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int16"; break;
                            case eGenType.cs: returnTypeName = "short"; break;
                            case eGenType.mssql: returnTypeName = "smallint"; break;
                            case eGenType.mysql: returnTypeName = "smallint"; break;
                            case eGenType.sqllite: returnTypeName = "smallint"; break;
                            case eGenType.rust: returnTypeName = "i16"; break;
                        }
                    }
                    break;
                case BaseType.String:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FString"; break;
                            case eGenType.cs: returnTypeName = "string"; break;
                            case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "String"; break;
                        }
                    }
                    break;
                case BaseType.Float:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "float"; break;
                            case eGenType.cs: returnTypeName = "float"; break;
                            case eGenType.mssql: returnTypeName = "float"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f32"; break;
                        }
                    }
                    break;
                case BaseType.Double:
                    {

                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "double"; break;
                            case eGenType.cs: returnTypeName = "double"; break;
                            case eGenType.mssql: returnTypeName = "double"; break;
                            case eGenType.mysql: returnTypeName = "double"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f64"; break;
                        }
                    }
                    break;
                case BaseType.Int8:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int8"; break;
                            case eGenType.cs: returnTypeName = "byte"; break;
                            case eGenType.mssql: returnTypeName = "tinyint"; break;
                            case eGenType.mysql: returnTypeName = "tinyint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i8"; break;
                        }
                    }
                    break;
                case BaseType.Boolean:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "bool"; break;
                            case eGenType.cs: returnTypeName = "bool"; break;
                            case eGenType.proto: returnTypeName = "bool"; break;
                            case eGenType.mssql: returnTypeName = "bit"; break;
                            case eGenType.mysql: returnTypeName = "bool"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "bool"; break;
                        }
                    }
                    break;
                case BaseType.DateTime:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FDateTime"; break;
                            case eGenType.cs: returnTypeName = "System.DateTime"; break;
                            case eGenType.mssql: returnTypeName = "datetime"; break;
                            case eGenType.mysql: returnTypeName = "datetime"; break;
                            case eGenType.sqllite: returnTypeName = "date"; break;
                            case eGenType.rust: returnTypeName = "chrono::DateTime"; break;
                        }
                    }
                    break;
                case BaseType.TimeSpan:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "Timespan"; break;
                            case eGenType.cs: returnTypeName = "System.TimeSpan"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "timespan::DateTimeSpan"; break;
                        }
                    }
                    break;

            }
            return returnTypeName;
        }
        public static string ToSnakeCase( string str) =>
            new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() }.GetResolvedPropertyName(str);
        public static string ToPascalCase(string original)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }        
    }
    public static partial class BitFlagsExtensions {
        public static readonly global::Google.Protobuf.Extension<global::Google.Protobuf.Reflection.EnumOptions, string> BitFlags =
            new global::Google.Protobuf.Extension<global::Google.Protobuf.Reflection.EnumOptions, string>(50000, Google.Protobuf.FieldCodec.ForString(400002, ""));
    }
}
