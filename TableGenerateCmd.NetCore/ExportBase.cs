using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TableGenerateCmd;
using File = System.IO.File;
using Stream = System.IO.Stream;

namespace TableGenerate
{
    public enum eBaseType
    {
        Null,
        String,
        Boolean,
        Int8,
        Int16,
        Int32,
        Int64,
        Float,
        Double,
        DateTime,
        TimeSpan,
        Enum,
        Struct
    };

    public enum eGenType
    {
        cs,
        cpp,
        mssql,
        mysql,
        sqllite,
        tf,
        proto,
        rust,
    };

    public class Column
    {
        public bool is_key;
        public int data_column_index;
        public string var_name;
        public eBaseType base_type;
        public int array_index;
        public bool is_array;
        public bool is_last_array;
        public string array_group_name;
        public bool is_generated;
        public bool is_out_string;
        public string type_name;
        public eBaseType primitive_type;
        public string min_value;
        public string max_value;
        public string desc;
        public System.Reflection.TypeInfo TypeInfo;
        public bool bit_flags;
    };

    public abstract class ExportBase
    {
        public abstract bool Generate(System.Reflection.Assembly[] refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except);
    }

    public static class ExportBaseUtil
    {
        static bool FileEquals(string fileName1, string fileName2)
        {
            // Check the file size and CRC equality here.. if they are equal...    
            try
            {
                using FileStream file1 = new FileStream(fileName1, FileMode.Open), file2 = new FileStream(fileName2, FileMode.Open);
                    return StreamsContentsAreEqual(file1, file2);
            }
            catch (System.Exception)
            {
            }
            return false;
        }

        static bool FileEquals(Stream file1, string fileName2)
        {
            // Check the file size and CRC equality here.. if they are equal...    
            try
            {
                using var file2 = new FileStream(fileName2, FileMode.Open);
                return StreamsContentsAreEqual(file1, file2);
            }
            catch (System.Exception)
            {
            }
            return false;
        }
        public static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            {
                var stream1_md5 = md5.ComputeHash(stream1);
                var stream2_md5 = md5.ComputeHash(stream2);
                return Enumerable.SequenceEqual(stream1_md5, stream2_md5);
            }
            /*
            const int bufferSize = 2048 * 2;

            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                {
                    return false;
                }

                if (count1 == 0)
                {
                    return true;
                }

                int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
                for (int i = 0; i < iterations; i++)
                {
                    if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                    {
                        return false;
                    }
                }
            }
            */
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

        public static List<Column> GetColumnInfo(System.Reflection.Assembly[] refAssem, System.Reflection.Assembly mscorlibAssembly, string sheetName, StringWithDesc[,] rows, List<string> except)
        {
            var columns = new List<Column>();
            for (int i = 0; i < rows.GetLength(1); i++)
            {
                string name = rows[0,i].Text.Trim().Replace(' ', '_');
                string desc = rows[0, i].Desc;
                string generate = rows[1,i].Text.Trim().Replace(' ', '_').ToLower();
                string type = rows[2,i].Text.Trim().Replace(' ', '_');
                if (name.Length == 0)
                    continue;


                Column column = new Column
                {
                    is_key = i == 0,
                    data_column_index = i,
                    var_name = name,
                    is_generated = except.Count() == 0 || except.Exists(compare => generate.Trim().ToLower().Contains(compare) ) == false,
                    is_out_string = type.IndexOf("out_string") >= 0,
                    array_group_name = null,
                    array_index = -1,
                    is_array = false, 
                    desc = desc,
                    bit_flags = generate.ToLower() == "bitflags"
                };

                GetBaseType(ref column, refAssem, mscorlibAssembly, type);

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
                returnTypeName = $"({column.GetPrimitiveType(genType)}) System.Enum.Parse(typeof({column.GetPrimitiveType(genType)}),{arg})";
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

        private static string GetConvertFunction(this eBaseType baseType, string arg, eGenType genType)
        {
            string returnTypeName = baseType switch
            {
                eBaseType.Int64 => $"System.Convert.ToInt64(System.Math.Round(double.Parse({arg})))",
                eBaseType.Int32 => $"System.Convert.ToInt32(System.Math.Round(double.Parse({arg})))",
                eBaseType.Int16 => $"System.Convert.ToInt16(System.Math.Round(double.Parse({arg})))",
                eBaseType.String => $"{arg}",
                eBaseType.Float => $"System.Convert.ToSingle({arg})",
                eBaseType.Double => $"System.Convert.ToDouble({arg})",
                eBaseType.Int8 => $"System.Convert.ToByte({arg})",
                eBaseType.Boolean => $"({arg}.Trim()==\"1\"||{arg}.Trim().ToUpper()==\"TRUE\")",
                eBaseType.DateTime => $"System.DateTime.Parse({arg})",
                eBaseType.TimeSpan => $"System.TimeSpan.Parse({arg})",
                _ => string.Empty
            };
            return returnTypeName;
        }

        public static string GetParseString(this Column column, string data)
        {
            string returnTypeName = string.Empty;
            switch (column.base_type)
            {
                case eBaseType.Int64: returnTypeName    = long.Parse(data).ToString(); break;
                case eBaseType.Int32: returnTypeName    = int.Parse(data).ToString(); break;
                case eBaseType.Int16: returnTypeName    = short.Parse(data).ToString(); break;
                case eBaseType.String: returnTypeName   = data; break;
                case eBaseType.Float: returnTypeName    = float.Parse(data).ToString(); break;
                case eBaseType.Double: returnTypeName   = double.Parse(data).ToString(); break;
                case eBaseType.Int8: returnTypeName     = byte.Parse(data).ToString(); break;
                case eBaseType.Boolean: returnTypeName  = (data.Trim()=="1"||data.Trim().ToUpper()=="TRUE")?"true":"false"; break;
                case eBaseType.DateTime: returnTypeName = "/"; break;
                case eBaseType.TimeSpan: returnTypeName = "/"; break;
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
                case eBaseType.String: return false;
                case eBaseType.DateTime: return false;
                case eBaseType.TimeSpan: return false;
            }
            return true;
        }
        public static bool IsDateTime(this Column column) => column.base_type == eBaseType.DateTime;
        public static bool IsTimeSpan(this Column column) => column.base_type == eBaseType.TimeSpan;
        public static bool IsEnumType(this Column column) => column.base_type == eBaseType.Enum;
        public static bool IsStructType(this Column column) => column.base_type == eBaseType.Struct;
        public static bool IsString(this Column column) => column.base_type == eBaseType.String;

        public static string GetSqlitekitFunction(this Column column)
        {
            string returnTypeName = string.Empty;
            if (column.array_index >= 0)
            {
                string var_name = column.var_name + column.array_index;
                switch (column.base_type)
                {
                    case eBaseType.Int64: returnTypeName    = $"qr__.GetLong(\"{var_name}\")"; break;
                    case eBaseType.Int32: returnTypeName    = $"qr__.GetInteger(\"{var_name}\")"; break;
                    case eBaseType.Int16: returnTypeName    = $"(short) qr__.GetInteger(\"{var_name}\")"; break;
                    case eBaseType.String: returnTypeName   = $"qr__.GetString(\"{var_name}\")"; break;
                    case eBaseType.Float: returnTypeName    = $"(float) qr__.GetDouble(\"{var_name}\")"; break;
                    case eBaseType.Double: returnTypeName   = $"qr__.GetDouble(\"{var_name}\")"; break;
                    case eBaseType.Int8: returnTypeName     = $"(byte) qr__.GetInteger(\"{var_name}\")"; break;
                    case eBaseType.Boolean: returnTypeName  = $"qr__.GetInteger(\"{var_name}\")>0?true:false"; break;
                    case eBaseType.DateTime: returnTypeName = $"System.DateTime.Parse(qr__.GetString(\"{var_name}\"))"; break;
                    case eBaseType.TimeSpan: returnTypeName = $"System.TimeSpan.Parse(qr__.GetString(\"{var_name}\"))"; break;
                }
            }
            else
            {
                string var_name = column.var_name;
                switch (column.base_type)
                {
                    case eBaseType.Int64: returnTypeName    = $"qr__.GetLong(\"{var_name}\")"; break;
                    case eBaseType.Int32: returnTypeName    = $"qr__.GetInteger(\"{var_name}\")"; break;
                    case eBaseType.Int16: returnTypeName    = $"(short) qr__.GetInteger(\"{var_name}\")"; break;
                    case eBaseType.String: returnTypeName   = $"qr__.GetString(\"{var_name}\")"; break;
                    case eBaseType.Float: returnTypeName    = $"(float) qr__.GetDouble(\"{var_name}\")"; break;
                    case eBaseType.Double: returnTypeName   = $"qr__.GetDouble(\"{var_name}\")"; break;
                    case eBaseType.Int8: returnTypeName     = $"(byte) qr__.GetInteger(\"{var_name}\")"; break;
                    case eBaseType.Boolean: returnTypeName  = $"qr__.GetInteger(\"{var_name}\")>0?true:false"; break;
                    case eBaseType.DateTime: returnTypeName = $"System.DateTime.Parse(qr__.GetString(\"{var_name}\"))"; break;
                    case eBaseType.TimeSpan: returnTypeName = $"System.TimeSpan.Parse(qr__.GetString(\"{var_name}\"))"; break;
                }
            }
            return returnTypeName;
        }
        public static string GetReadStreamFunction(this Column column, eGenType gen_type)
        {
            string returnTypeName = string.Empty;
            if (column.array_index >= 0)
            {
                if(!column.IsEnumType() && !column.IsStructType() )
                    returnTypeName = GetReadStreamFunction(column.base_type);
                else
                    returnTypeName = $"({column.GetPrimitiveType(gen_type)}){GetReadStreamFunction(column.primitive_type)}";
            }
            else
            {
                if (!column.IsEnumType() && !column.IsStructType())
                    returnTypeName = GetReadStreamFunction(column.base_type);
                else
                    returnTypeName = $"({column.GetPrimitiveType(gen_type)}){GetReadStreamFunction(column.primitive_type)}";
            }
            return returnTypeName;
        }

        private static string GetReadStreamFunction(this eBaseType baseType)
        {
            string returnTypeName = baseType switch
            {
                eBaseType.Int64 => $"__reader.ReadInt64()",
                eBaseType.Int32 => $"__reader.ReadInt32()",
                eBaseType.Int16 => $"__reader.ReadInt16()",
                eBaseType.String => $"Encoder.ReadString(ref __reader)",
                eBaseType.Float => $"__reader.ReadSingle()",
                eBaseType.Double => $"__reader.ReadDouble()",
                eBaseType.Int8 => $"__reader.ReadByte()",
                eBaseType.Boolean => $"__reader.ReadBoolean()",
                eBaseType.DateTime => $"System.DateTime.FromBinary(__reader.ReadInt64())",
                eBaseType.TimeSpan => $"System.TimeSpan.FromTicks(__reader.ReadInt64())",
                _ => string.Empty
            };
            return returnTypeName;
        }

        private static void GetBaseType(ref Column column, System.Reflection.Assembly[] refAssembly, System.Reflection.Assembly mscorlibAssembly, string typename)
        {
            column.base_type = eBaseType.Null;
            string type_name = typename;
            string subtypename = type_name;
            string sizeChecker = typename;
            int startIndex = sizeChecker.IndexOf('<');
            if(startIndex >= 0)
            {
                int lastIndex = sizeChecker.LastIndexOf('>');
                if (type_name.StartsWith("array"))
                {
                    type_name = "array";
                    if (startIndex >= 0)
                    {
                        sizeChecker = sizeChecker.Substring(startIndex + 1, sizeChecker.Length - startIndex - 2);
                        sizeChecker = sizeChecker.Replace("out_", string.Empty);
                        sizeChecker = sizeChecker.Replace("array", string.Empty);
                        subtypename = sizeChecker;
                    }
                    column.is_array = true;
                }
                if (subtypename.IndexOf("enum") >= 0)
                {
                    startIndex = subtypename.IndexOf('<');
                    lastIndex = subtypename.LastIndexOf('>');
                    if (startIndex >= 0)
                    {
                        sizeChecker = subtypename;
                        sizeChecker = sizeChecker.Substring(startIndex + 1, sizeChecker.Length - startIndex - 2);
                        column.type_name = sizeChecker;
                        column.base_type = eBaseType.Enum;
                        if (refAssembly != null)
                        {
                            System.Reflection.TypeInfo type = refAssembly.SelectMany(t=>t.DefinedTypes).FirstOrDefault(t => t.FullName.Equals(sizeChecker));
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
                            }
                            if (type != null)
                            {
                                column.TypeInfo = type;
                                {
                                    var types = type.DeclaredFields.Where(t => t.IsStatic).ToArray();
                                    column.min_value = $"global::{type.FullName}.{types.First().Name}";
                                    column.max_value = $"global::{type.FullName}.{types.Last().Name}";
                                }
                                var DeclaredField = type.DeclaredFields.First();
                                switch ("")
                                {
                                    case string anyName when DeclaredField.FieldType == typeof(System.SByte):
                                        column.primitive_type = eBaseType.Int8;
                                        break;
                                    case string anyName when DeclaredField.FieldType == typeof(System.Byte):
                                        column.primitive_type = eBaseType.Int8;
                                        break;
                                    case string anyName when DeclaredField.FieldType == typeof(System.Int16):
                                        column.primitive_type = eBaseType.Int16;
                                        break;
                                    case string anyName when DeclaredField.FieldType == typeof(System.Int32):
                                        column.primitive_type = eBaseType.Int32;
                                        break;
                                    case string anyName when DeclaredField.FieldType == typeof(System.Int64):
                                        column.primitive_type = eBaseType.Int64;
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
                        column.base_type = eBaseType.Struct;
                        if (refAssembly != null)
                        {
                            System.Reflection.TypeInfo type = refAssembly.SelectMany(t=>t.DefinedTypes).FirstOrDefault(t => t.FullName.Equals(sizeChecker));
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
                                        column.primitive_type = eBaseType.Int8;
                                        break;
                                    case "SByte":
                                        column.primitive_type = eBaseType.Int8;
                                        break;
                                    case "Int16":
                                        column.primitive_type = eBaseType.Int16;
                                        break;
                                    case "Int32":
                                        column.primitive_type = eBaseType.Int32;
                                        break;
                                    case "Int64":
                                        column.primitive_type = eBaseType.Int64;
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
                if (subtypename.IndexOf("string") >= 0)
                {
                    type_name = "string";
                    startIndex = sizeChecker.IndexOf('<');
                    lastIndex = sizeChecker.LastIndexOf('>');
                    if (startIndex >= 0)
                    {
                        sizeChecker = sizeChecker.Substring(startIndex + 1, sizeChecker.Length - startIndex - 2);
                        sizeChecker = sizeChecker.Replace("out_", string.Empty);
                        sizeChecker = sizeChecker.Replace("string", string.Empty);
                        //size = Int32.Parse(sizeChecker);
                    }
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
            switch (type_name)
            {
                case "long": column.base_type = eBaseType.Int64; break;
                case "int64": column.base_type = eBaseType.Int64; break;
                case "int": column.base_type = eBaseType.Int32; break;
                case "int32": column.base_type = eBaseType.Int32; break;
                case "int16": column.base_type = eBaseType.Int16; break;
                case "short": column.base_type = eBaseType.Int16; break;
                case "string": column.base_type = eBaseType.String; break;
                case "float": column.base_type = eBaseType.Float; break;
                case "double": column.base_type = eBaseType.Double; break;
                case "int8": column.base_type = eBaseType.Int8; break;
                case "byte": column.base_type = eBaseType.Int8; break;
                case "bool": column.base_type = eBaseType.Boolean; break;
                case "datetime": column.base_type = eBaseType.DateTime; break;
                case "timespan": column.base_type = eBaseType.TimeSpan; break;
                case "array":
                    {
                        switch (subtypename)
                        {
                            case "long": column.base_type = eBaseType.Int64; break;
                            case "int64": column.base_type = eBaseType.Int64; break;
                            case "int": column.base_type = eBaseType.Int32; break;
                            case "int32": column.base_type = eBaseType.Int32; break;
                            case "short": column.base_type = eBaseType.Int16; break;
                            case "int16": column.base_type = eBaseType.Int16; break;
                            case "string": column.base_type = eBaseType.String; break;
                            case "float": column.base_type = eBaseType.Float; break;
                            case "double": column.base_type = eBaseType.Double; break;
                            case "int8": column.base_type = eBaseType.Int8; break;
                            case "byte": column.base_type = eBaseType.Int8; break;
                            case "bool": column.base_type = eBaseType.Boolean; break;
                            case "datetime": column.base_type = eBaseType.DateTime; break;
                            case "timespan": column.base_type = eBaseType.TimeSpan; break;
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
               case eBaseType.Int64:    returnTypeName = "long"; break;
               case eBaseType.Int32:    returnTypeName = "int"; break;
               case eBaseType.Int16:    returnTypeName = "short"; break;
               case eBaseType.String:   returnTypeName = "string"; break;
               case eBaseType.Float:    returnTypeName = "float"; break;
               case eBaseType.Double:   returnTypeName = "double"; break;
               case eBaseType.Int8:     returnTypeName = "byte"; break;
               case eBaseType.Boolean:  returnTypeName = "bool"; break;
               case eBaseType.DateTime: returnTypeName = "datetime"; break;
               case eBaseType.TimeSpan: returnTypeName = "timespan"; break;
               case eBaseType.Enum: returnTypeName = column.type_name; break;
               case eBaseType.Struct: returnTypeName = column.type_name; break;
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
                    case eBaseType.Int64: returnTypeName = "0"; break;
                    case eBaseType.Int32: returnTypeName = "0"; break;
                    case eBaseType.Int16: returnTypeName = "0"; break;
                    case eBaseType.String: returnTypeName = "string.Empty"; break;
                    case eBaseType.Float: returnTypeName = "0"; break;
                    case eBaseType.Double: returnTypeName = "0"; break;
                    case eBaseType.Int8: returnTypeName = "0"; break;
                    case eBaseType.Boolean: returnTypeName = "false"; break;
                    case eBaseType.DateTime: returnTypeName = "datetime"; break;
                    case eBaseType.TimeSpan: returnTypeName = "TimeSpan"; break;
                    case eBaseType.Enum: returnTypeName = column.min_value; break;
                    case eBaseType.Struct: returnTypeName = column.min_value; break;
                }
            }
            else if (gen_type == eGenType.sqllite || gen_type == eGenType.mssql || gen_type == eGenType.mysql) 
            {
                switch (column.base_type)
                {
                    case eBaseType.Int64:       returnTypeName = "0"; break;
                    case eBaseType.Int32:       returnTypeName = "0"; break;
                    case eBaseType.Int16:       returnTypeName = "0"; break;
                    case eBaseType.String:      returnTypeName = "''"; break;
                    case eBaseType.Float:       returnTypeName = "0"; break;
                    case eBaseType.Double:      returnTypeName = "0"; break;
                    case eBaseType.Int8:        returnTypeName = "0"; break;
                    case eBaseType.Boolean:     returnTypeName = "0"; break;
                    case eBaseType.DateTime:    returnTypeName = "''"; break;
                    case eBaseType.TimeSpan:    returnTypeName = "''"; break;
                    case eBaseType.Enum:    returnTypeName = "0"; break;
                    case eBaseType.Struct: returnTypeName = "0"; break;
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
                    eBaseType.Int64 => "default(LongEqualityComparer)",
                    eBaseType.Int32 => "default(IntEqualityComparer)",
                    eBaseType.Int16 => "default(ShortEqualityComparer)",
                    eBaseType.String => "default(StringEqualityComparer)",
                    eBaseType.Float => "",
                    eBaseType.Double => "",
                    eBaseType.Int8 => "",
                    eBaseType.Boolean => "",
                    eBaseType.DateTime => "",
                    eBaseType.TimeSpan => "",
                    eBaseType.Enum => $"",
                    eBaseType.Struct => $"",
                    _ => returnTypeName
                };
            }
            return returnTypeName;
        }
        public static string GenerateType(this Column column, eGenType gen_type)
        {
            int array_count = column.array_index;
            string returnTypeName = string.Empty;
            if (array_count != -1)
            {
                switch (column.base_type)
                {
                    case eBaseType.Int64:
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
                    case eBaseType.Int32:
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
                    case eBaseType.Int16:
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
                    case eBaseType.String:
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
                    case eBaseType.Float:
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
                    case eBaseType.Double:
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
                    case eBaseType.Int8:
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
                    case eBaseType.Boolean:
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
                    case eBaseType.DateTime:
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
                    case eBaseType.TimeSpan:
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
                    case eBaseType.Enum:
                    case eBaseType.Struct:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp when !column.bit_flags: returnTypeName = $"TArray<int32>"; break;
                                case eGenType.cpp when column.bit_flags: returnTypeName = $"TArray<{(column.IsEnumType()?"E":"F")}{column.type_name}>"; break;
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
                case eBaseType.Int64:
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
                case eBaseType.Int32: 
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
                case eBaseType.Int16:
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
                case eBaseType.String:
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
                case eBaseType.Float:
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
                case eBaseType.Double:
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
                case eBaseType.Int8:
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
                case eBaseType.Boolean:
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
                case eBaseType.DateTime:
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
                case eBaseType.TimeSpan:
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
                case eBaseType.Enum:
                case eBaseType.Struct:
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
            if (array_count != -1)
            {
                switch (column.base_type)
                {
                    case eBaseType.Int64:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int64>::Empty()"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<long>()"; break;
                                case eGenType.proto: returnTypeName = "-"; break;
                                case eGenType.mssql: returnTypeName = "-"; break;
                                case eGenType.mysql: returnTypeName = "-"; break;
                                case eGenType.sqllite: returnTypeName = "-"; break;
                                case eGenType.rust: returnTypeName = "-"; break;
                            }
                        }
                        break;
                    case eBaseType.Int32:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int32>"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<int>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int32"; break;
                                case eGenType.mssql: returnTypeName = "int"; break;
                                case eGenType.mysql: returnTypeName = "int"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i32>"; break;
                            }
                        }
                        break;
                    case eBaseType.Int16:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int16>"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<short>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int32"; break;
                                case eGenType.mssql: returnTypeName = "smallint"; break;
                                case eGenType.mysql: returnTypeName = "smallint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i16>"; break;
                            }
                        }
                        break;
                    case eBaseType.String:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<FString>"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<string>()"; break;
                                case eGenType.proto: returnTypeName = "repeated string"; break;
                                case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                                case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                                case eGenType.rust: returnTypeName = "Vec<String>"; break;
                            }
                        }
                        break;
                    case eBaseType.Float:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<float>"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<float>()"; break;
                                case eGenType.proto: returnTypeName = "repeated float"; break;
                                case eGenType.mssql: returnTypeName = "float"; break;
                                case eGenType.mysql: returnTypeName = "float"; break;
                                case eGenType.sqllite: returnTypeName = "real"; break;
                                case eGenType.rust: returnTypeName = "Vec<f32>"; break;
                            }
                        }
                        break;
                    case eBaseType.Double:
                        {

                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<double>"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<double>()"; break;
                                case eGenType.proto: returnTypeName = "repeated double"; break;
                                case eGenType.mssql: returnTypeName = "double"; break;
                                case eGenType.mysql: returnTypeName = "double"; break;
                                case eGenType.sqllite: returnTypeName = "real"; break;
                                case eGenType.rust: returnTypeName = "Vec<f64>"; break;
                            }
                        }
                        break;
                    case eBaseType.Int8:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int8>"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<byte>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int8"; break;
                                case eGenType.mssql: returnTypeName = "tinyint"; break;
                                case eGenType.mysql: returnTypeName = "tinyint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<i8>"; break;
                            }
                        }
                        break;
                    case eBaseType.Boolean:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<bool>"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<bool>()"; break;
                                case eGenType.proto: returnTypeName = "repeated bool"; break;
                                case eGenType.mssql: returnTypeName = "bit"; break;
                                case eGenType.mysql: returnTypeName = "bool"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                                case eGenType.rust: returnTypeName = "Vec<bool>"; break;
                            }
                        }
                        break;
                    case eBaseType.DateTime:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<FDateTime>"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<System.DateTime>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int64"; break;
                                case eGenType.mssql: returnTypeName = "datetime"; break;
                                case eGenType.mysql: returnTypeName = "datetime"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                                case eGenType.rust: returnTypeName = "Vec<chrono::DateTime>"; break;
                            }
                        }
                        break;
                    case eBaseType.TimeSpan:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<Timespan>"; break;
                                case eGenType.cs: returnTypeName = "System.Array.Empty<System.TimeSpan>()"; break;
                                case eGenType.proto: returnTypeName = "repeated int64"; break;
                                case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                                case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                                case eGenType.rust: returnTypeName = "Vec<timespan::DateTimeSpan>"; break;
                            }
                        }
                        break;
                    case eBaseType.Enum:
                    case eBaseType.Struct:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "TArray<int32>"; break;
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
                case eBaseType.Int64:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int64"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "int64"; break;
                            case eGenType.mssql: returnTypeName = "bigint"; break;
                            case eGenType.mysql: returnTypeName = "bigint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i64"; break;
                        }
                    }
                    break;
                case eBaseType.Int32: 
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int32"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "int32"; break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i32"; break;
                        }
                    }
                    break;
                case eBaseType.Int16:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int16"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "int32"; break;
                            case eGenType.mssql: returnTypeName = "smallint"; break;
                            case eGenType.mysql: returnTypeName = "smallint"; break;
                            case eGenType.sqllite: returnTypeName = "smallint"; break;
                            case eGenType.rust: returnTypeName = "i16"; break;
                        }
                    }
                    break;
                case eBaseType.String:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FString"; break;
                            case eGenType.cs: returnTypeName = "string.Empty"; break;
                            case eGenType.proto: returnTypeName = "string"; break;
                            case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "String"; break;
                        }
                    }
                    break;
                case eBaseType.Float:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "float"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "float"; break;
                            case eGenType.mssql: returnTypeName = "float"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f32"; break;
                        }
                    }
                    break;
                case eBaseType.Double:
                    {

                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "double"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "double"; break;
                            case eGenType.mssql: returnTypeName = "double"; break;
                            case eGenType.mysql: returnTypeName = "double"; break;
                            case eGenType.sqllite: returnTypeName = "real"; break;
                            case eGenType.rust: returnTypeName = "f64"; break;
                        }
                    }
                    break;
                case eBaseType.Int8:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "uint8"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "byte"; break;
                            case eGenType.mssql: returnTypeName = "tinyint"; break;
                            case eGenType.mysql: returnTypeName = "tinyint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "i8"; break;
                        }
                    }
                    break;
                case eBaseType.Boolean:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "bool"; break;
                            case eGenType.cs: returnTypeName = "default"; break;
                            case eGenType.proto: returnTypeName = "bool"; break;
                            case eGenType.mssql: returnTypeName = "bit"; break;
                            case eGenType.mysql: returnTypeName = "bool"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                            case eGenType.rust: returnTypeName = "bool"; break;
                        }
                    }
                    break;
                case eBaseType.DateTime:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "FDateTime"; break;
                            case eGenType.cs: returnTypeName = "System.DateTime.MinValue"; break;
                            case eGenType.proto: returnTypeName = "int64"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "chrono::DateTime"; break;
                        }
                    }
                    break;
                case eBaseType.TimeSpan:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "Timespan"; break;
                            case eGenType.cs: returnTypeName = "System.TimeSpan.MinValue"; break;
                            case eGenType.proto: returnTypeName = "int64"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                            case eGenType.rust: returnTypeName = "timespan::DateTimeSpan"; break;
                        }
                    }
                    break;
                case eBaseType.Enum:
                case eBaseType.Struct:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int32"; break;
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
                case eBaseType.Int64:
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
                case eBaseType.Int32:
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
                case eBaseType.Int16:
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
                case eBaseType.String:
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
                case eBaseType.Float:
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
                case eBaseType.Double:
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
                case eBaseType.Int8:
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
                case eBaseType.Boolean:
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
                case eBaseType.DateTime:
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
                case eBaseType.TimeSpan:
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
                case eBaseType.Enum:
                case eBaseType.Struct:
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

        public static string GenerateBaseType(this eBaseType base_type, eGenType gen_type)
        {
            string returnTypeName = string.Empty;
            switch (base_type)
            {
                case eBaseType.Int64:
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
                case eBaseType.Int32:
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
                case eBaseType.Int16:
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
                case eBaseType.String:
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
                case eBaseType.Float:
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
                case eBaseType.Double:
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
                case eBaseType.Int8:
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
                case eBaseType.Boolean:
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
                case eBaseType.DateTime:
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
                case eBaseType.TimeSpan:
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
    }
}
