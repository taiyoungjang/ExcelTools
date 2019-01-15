using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

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
        Enum
    };

    public enum eGenType
    {
        cs,
        cpp,
        mssql,
        mysql,
        sqllite,
        tf
    };

    public class Column
    {
        public bool is_key;
        public int data_column_index;
        public string var_name;
        public eBaseType base_type;
        public int array_index;
        public bool is_generated;
        public bool is_out_string;
        public string type_name;
        public eBaseType primitive_type;
        public string min_value;
    };

    public abstract class ExportBase
    {
        public const string UNITY_DEFINE = "UNITY_2018_2_OR_NEWER";
        abstract public bool Generate(System.Reflection.Assembly refAssem, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except);
    }

    public static class ExportBaseUtil
    {
        static bool FileEquals(string fileName1, string fileName2)
        {
            // Check the file size and CRC equality here.. if they are equal...    
            try
            {
                using (FileStream file1 = new FileStream(fileName1, FileMode.Open), file2 = new FileStream(fileName2, FileMode.Open))
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
                using (var file2 = new FileStream(fileName2, FileMode.Open))
                    return StreamsContentsAreEqual(file1, file2);
            }
            catch (System.Exception)
            {
            }
            return false;
        }
        public static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
        {
            using( var md5 = System.Security.Cryptography.MD5.Create() )
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

        public static void CheckReplaceFile(MemoryStream tempFile, string fileName)
        {
            fileName = System.IO.Path.GetFullPath(fileName);
            if (FileEquals(tempFile, fileName) == false)
            {
                File.WriteAllBytes(fileName, tempFile.ToArray());
            }
        }

        public static void CheckReplaceFile(string tempFileName, string fileName)
        {
            tempFileName = System.IO.Path.GetFullPath(tempFileName);
            fileName = System.IO.Path.GetFullPath(fileName);
            if (FileEquals(tempFileName, fileName) == false)
                File.Copy(tempFileName, fileName, true);
        }


        public static List<Column> GetColumnInfo(System.Reflection.Assembly refAssem, System.Reflection.Assembly mscorlibAssembly, string sheetName, string[,] rows, List<string> except)
        {
            var columns = new List<Column>();
            for (int i = 0; i < rows.GetLength(1); i++)
            {
                string name = rows[0,i].Trim().Replace(' ', '_');
                string generate = rows[1,i].Trim().Replace(' ', '_').ToLower();
                string type = rows[2,i].Trim().Replace(' ', '_');
                if (name.Length == 0)
                    continue;


                Column column = new Column
                {
                    is_key = i == 0,
                    data_column_index = i,
                    var_name = name,
                    is_generated = except.Count() == 0 || except.Exists(compare => generate.Trim().ToLower().Contains(compare) ) == false,
                    is_out_string = type.IndexOf("out_string") >= 0
                };
                if(!int.TryParse(generate, out column.array_index))
                {
                    column.array_index = -1;
                }

                GetBaseType(ref column, refAssem, mscorlibAssembly, type);

                if( column.array_index >= 0)
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
            }

            return columns;
        }

        public static string GetConvertFunction(this Column column, string arg, eGenType gen_type)
        {
            string returnTypeName = string.Empty;
            if (!column.IsEnumType())
                returnTypeName = GetConvertFunction(column.base_type, arg,gen_type);
            else
                returnTypeName = $"({column.GetPrimitiveType(gen_type)}) System.Enum.Parse(typeof({column.GetPrimitiveType(gen_type)}),{arg})";
            return returnTypeName;
        }
        public static string GetConvertFunction(this eBaseType base_type, string arg, eGenType gen_type)
        {
            string returnTypeName = string.Empty;
            switch (base_type)
            {
                case eBaseType.Int64: returnTypeName = $"System.Convert.ToInt64(System.Math.Round(double.Parse({arg})))"; break;
                case eBaseType.Int32: returnTypeName = $"System.Convert.ToInt32(System.Math.Round(double.Parse({arg})))"; break;
                case eBaseType.Int16: returnTypeName = $"System.Convert.ToInt16(System.Math.Round(double.Parse({arg})))"; break;
                case eBaseType.String: returnTypeName = $"{arg}"; break;
                case eBaseType.Float: returnTypeName = $"System.Convert.ToSingle({arg})"; break;
                case eBaseType.Double: returnTypeName = $"System.Convert.ToDouble({arg})"; break;
                case eBaseType.Int8: returnTypeName = $"System.Convert.ToByte({arg})"; break;
                case eBaseType.Boolean: returnTypeName = $"({arg}.Trim()==\"1\"||{arg}.Trim().ToUpper()==\"TRUE\")?true:false"; break;
                case eBaseType.DateTime: returnTypeName = $"System.DateTime.Parse({arg})"; break;
                case eBaseType.TimeSpan: returnTypeName = $"System.TimeSpan.Parse({arg})"; break;
            }
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
                if(!column.IsEnumType())
                    returnTypeName = GetReadStreamFunction(column.base_type);
                else
                    returnTypeName = $"({column.GetPrimitiveType(gen_type)}){GetReadStreamFunction(column.primitive_type)}";
            }
            else
            {
                if (!column.IsEnumType())
                    returnTypeName = GetReadStreamFunction(column.base_type);
                else
                    returnTypeName = $"({column.GetPrimitiveType(gen_type)}){GetReadStreamFunction(column.primitive_type)}";
            }
            return returnTypeName;
        }
        public static string GetReadStreamFunction(this eBaseType base_type)
        {
            string returnTypeName = string.Empty;
            switch (base_type)
            {
                case eBaseType.Int64: returnTypeName = $"__reader.ReadInt64()"; break;
                case eBaseType.Int32: returnTypeName = $"__reader.ReadInt32()"; break;
                case eBaseType.Int16: returnTypeName = $"__reader.ReadInt16()"; break;
                case eBaseType.String: returnTypeName = $"__reader.ReadString()"; break;
                case eBaseType.Float: returnTypeName = $"__reader.ReadSingle()"; break;
                case eBaseType.Double: returnTypeName = $"__reader.ReadDouble()"; break;
                case eBaseType.Int8: returnTypeName = $"__reader.ReadByte()"; break;
                case eBaseType.Boolean: returnTypeName = $"__reader.ReadBoolean()"; break;
                case eBaseType.DateTime: returnTypeName = $"System.DateTime.FromBinary(__reader.ReadInt64())"; break;
                case eBaseType.TimeSpan: returnTypeName = $"System.TimeSpan.FromTicks(__reader.ReadInt64())"; break;
            }
            return returnTypeName;
        }

        private static void GetBaseType(ref Column column, System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, string typename)
        {
            column.base_type = eBaseType.Null;
            string type_name = typename;
            string subtypename = type_name;
            string sizeChecker = typename;
            int startIndex = sizeChecker.IndexOf('<');
            if(startIndex >= 0)
            {
                int lastIndex = sizeChecker.LastIndexOf('>');
                if (type_name.IndexOf("array") >= 0)
                {
                    type_name = "array";
                    if (startIndex >= 0)
                    {
                        sizeChecker = sizeChecker.Substring(startIndex + 1, sizeChecker.Length - startIndex - 2);
                        sizeChecker = sizeChecker.Replace("out_", string.Empty);
                        sizeChecker = sizeChecker.Replace("array", string.Empty);
                        subtypename = sizeChecker;
                    }
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
                            System.Reflection.TypeInfo type = refAssembly.DefinedTypes.FirstOrDefault(t => t.FullName.Equals(sizeChecker));
                            if( type == null)
                            {
                                type = mscorlibAssembly.DefinedTypes.FirstOrDefault(t => t.FullName.Equals(sizeChecker));
                            }
                            if (type != null)
                            {
                                var Values = type.DeclaredFields.Where(t => t.FieldType.BaseType == typeof(System.Enum)).Select(t => t.Name.ToString()).ToArray();
                                if(Values.Any())column.min_value = $"{type.FullName}.{Values[0]}";
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
                }
            }
            return returnTypeName;
        }
        public static string GetEqualityTypeValue(this Column column, eGenType gen_type)
        {
            string returnTypeName = string.Empty;
            if (gen_type == eGenType.cs)
            {
                switch (column.base_type)
                {
                    case eBaseType.Int64: returnTypeName = "default(global::TBL.LongEqualityComparer)"; break;
                    case eBaseType.Int32: returnTypeName = "default(global::TBL.IntEqualityComparer)"; break;
                    case eBaseType.Int16: returnTypeName = "default(global::TBL.ShortEqualityComparer)"; break;
                    case eBaseType.String: returnTypeName = "default(global::TBL.StringEqualityComparer)"; break;
                    case eBaseType.Float: returnTypeName = ""; break;
                    case eBaseType.Double: returnTypeName = ""; break;
                    case eBaseType.Int8: returnTypeName = ""; break;
                    case eBaseType.Boolean: returnTypeName = ""; break;
                    case eBaseType.DateTime: returnTypeName = ""; break;
                    case eBaseType.TimeSpan: returnTypeName = ""; break;
                    case eBaseType.Enum: returnTypeName = $""; break;
                }
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
                                case eGenType.cpp: returnTypeName = "std::vector<long long>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<long>"; break;
                                case eGenType.mssql: returnTypeName = "bigint"; break;
                                case eGenType.mysql: returnTypeName = "bigint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                            }
                        }
                        break;
                    case eBaseType.Int32:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<int>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<int>"; break;
                                case eGenType.mssql: returnTypeName = "int"; break;
                                case eGenType.mysql: returnTypeName = "int"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                            }
                        }
                        break;
                    case eBaseType.Int16:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<short>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<short>"; break;
                                case eGenType.mssql: returnTypeName = "smallint"; break;
                                case eGenType.mysql: returnTypeName = "smallint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                            }
                        }
                        break;
                    case eBaseType.String:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<std::wstring>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<string>"; break;
                                case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                                case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                            }
                        }
                        break;
                    case eBaseType.Float:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<float>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<float>"; break;
                                case eGenType.mssql: returnTypeName = "float"; break;
                                case eGenType.mysql: returnTypeName = "float"; break;
                                case eGenType.sqllite: returnTypeName = "real"; break;
                            }
                        }
                        break;
                    case eBaseType.Double:
                        {

                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<double>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<double>"; break;
                                case eGenType.mssql: returnTypeName = "double"; break;
                                case eGenType.mysql: returnTypeName = "double"; break;
                                case eGenType.sqllite: returnTypeName = "real"; break;
                            }
                        }
                        break;
                    case eBaseType.Int8:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<BYTE>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<byte>"; break;
                                case eGenType.mssql: returnTypeName = "tinyint"; break;
                                case eGenType.mysql: returnTypeName = "tinyint"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                            }
                        }
                        break;
                    case eBaseType.Boolean:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<bool>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<bool>"; break;
                                case eGenType.mssql: returnTypeName = "bit"; break;
                                case eGenType.mysql: returnTypeName = "bool"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
                            }
                        }
                        break;
                    case eBaseType.DateTime:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<time_t>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<System.DateTime>"; break;
                                case eGenType.mssql: returnTypeName = "datetime"; break;
                                case eGenType.mysql: returnTypeName = "datetime"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                            }
                        }
                        break;
                    case eBaseType.TimeSpan:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<double>"; break;
                                case eGenType.cs: returnTypeName = "System.Collections.Generic.List<System.TimeSpan>"; break;
                                case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                                case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                                case eGenType.sqllite: returnTypeName = "text"; break;
                            }
                        }
                        break;
                    case eBaseType.Enum:
                        {
                            switch (gen_type)
                            {
                                case eGenType.cpp: returnTypeName = "std::vector<int>"; break;
                                case eGenType.cs: returnTypeName = $"System.Collections.Generic.List<{column.type_name}>"; break;
                                case eGenType.mssql: returnTypeName = "int"; break;
                                case eGenType.mysql: returnTypeName = "int"; break;
                                case eGenType.sqllite: returnTypeName = "integer"; break;
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
                            case eGenType.cpp: returnTypeName = "long long"; break;
                            case eGenType.cs: returnTypeName = "long"; break;
                            case eGenType.mssql: returnTypeName = "bigint"; break;
                            case eGenType.mysql: returnTypeName = "bigint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.Int32: 
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int"; break;
                            case eGenType.cs: returnTypeName = "int"; break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.Int16:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "short"; break;
                            case eGenType.cs: returnTypeName = "short"; break;
                            case eGenType.mssql: returnTypeName = "smallint"; break;
                            case eGenType.mysql: returnTypeName = "smallint"; break;
                            case eGenType.sqllite: returnTypeName = "smallint"; break;
                        }
                    }
                    break;
                case eBaseType.String:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "std::wstring"; break;
                            case eGenType.cs: returnTypeName = "string"; break;
                            case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
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
                        }
                    }
                    break;
                case eBaseType.Int8:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "BYTE"; break;
                            case eGenType.cs: returnTypeName = "byte"; break;
                            case eGenType.mssql: returnTypeName = "tinyint"; break;
                            case eGenType.mysql: returnTypeName = "tinyint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.Boolean:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "bool"; break;
                            case eGenType.cs: returnTypeName = "bool"; break;
                            case eGenType.mssql: returnTypeName = "bit"; break;
                            case eGenType.mysql: returnTypeName = "bool"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.DateTime:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "time_t"; break;
                            case eGenType.cs: returnTypeName = "System.DateTime"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                        }
                    }
                    break;
                case eBaseType.TimeSpan:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "double"; break;
                            case eGenType.cs: returnTypeName = "System.TimeSpan"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                        }
                    }
                    break;
                case eBaseType.Enum:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int"; break;
                            case eGenType.cs: returnTypeName = column.type_name; break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
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
                            case eGenType.cpp: returnTypeName = "long long"; break;
                            case eGenType.cs: returnTypeName = "long"; break;
                            case eGenType.mssql: returnTypeName = "bigint"; break;
                            case eGenType.mysql: returnTypeName = "bigint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.Int32:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int"; break;
                            case eGenType.cs: returnTypeName = "int"; break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.Int16:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "short"; break;
                            case eGenType.cs: returnTypeName = "short"; break;
                            case eGenType.mssql: returnTypeName = "smallint"; break;
                            case eGenType.mysql: returnTypeName = "smallint"; break;
                            case eGenType.sqllite: returnTypeName = "smallint"; break;
                        }
                    }
                    break;
                case eBaseType.String:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "std::wstring"; break;
                            case eGenType.cs: returnTypeName = "string"; break;
                            case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
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
                        }
                    }
                    break;
                case eBaseType.Int8:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "BYTE"; break;
                            case eGenType.cs: returnTypeName = "byte"; break;
                            case eGenType.mssql: returnTypeName = "tinyint"; break;
                            case eGenType.mysql: returnTypeName = "tinyint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.Boolean:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "bool"; break;
                            case eGenType.cs: returnTypeName = "bool"; break;
                            case eGenType.mssql: returnTypeName = "bit"; break;
                            case eGenType.mysql: returnTypeName = "bool"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.DateTime:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "time_t"; break;
                            case eGenType.cs: returnTypeName = "System.DateTime"; break;
                            case eGenType.mssql: returnTypeName = "datetime"; break;
                            case eGenType.mysql: returnTypeName = "datetime"; break;
                            case eGenType.sqllite: returnTypeName = "date"; break;
                        }
                    }
                    break;
                case eBaseType.TimeSpan:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "double"; break;
                            case eGenType.cs: returnTypeName = "System.TimeSpan"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                        }
                    }
                    break;
                case eBaseType.Enum:
                    {
                        //returnTypeName = column.primitive_type.GenerateBaseType(gen_type);
                        returnTypeName = "string";
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
                            case eGenType.cpp: returnTypeName = "long long"; break;
                            case eGenType.cs: returnTypeName = "long"; break;
                            case eGenType.mssql: returnTypeName = "bigint"; break;
                            case eGenType.mysql: returnTypeName = "bigint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.Int32:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "int"; break;
                            case eGenType.cs: returnTypeName = "int"; break;
                            case eGenType.mssql: returnTypeName = "int"; break;
                            case eGenType.mysql: returnTypeName = "int"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.Int16:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "short"; break;
                            case eGenType.cs: returnTypeName = "short"; break;
                            case eGenType.mssql: returnTypeName = "smallint"; break;
                            case eGenType.mysql: returnTypeName = "smallint"; break;
                            case eGenType.sqllite: returnTypeName = "smallint"; break;
                        }
                    }
                    break;
                case eBaseType.String:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "std::wstring"; break;
                            case eGenType.cs: returnTypeName = "string"; break;
                            case eGenType.mssql: returnTypeName = "nvarchar(max)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
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
                        }
                    }
                    break;
                case eBaseType.Int8:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "BYTE"; break;
                            case eGenType.cs: returnTypeName = "byte"; break;
                            case eGenType.mssql: returnTypeName = "tinyint"; break;
                            case eGenType.mysql: returnTypeName = "tinyint"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.Boolean:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "bool"; break;
                            case eGenType.cs: returnTypeName = "bool"; break;
                            case eGenType.mssql: returnTypeName = "bit"; break;
                            case eGenType.mysql: returnTypeName = "bool"; break;
                            case eGenType.sqllite: returnTypeName = "integer"; break;
                        }
                    }
                    break;
                case eBaseType.DateTime:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "time_t"; break;
                            case eGenType.cs: returnTypeName = "System.DateTime"; break;
                            case eGenType.mssql: returnTypeName = "datetime"; break;
                            case eGenType.mysql: returnTypeName = "datetime"; break;
                            case eGenType.sqllite: returnTypeName = "date"; break;
                        }
                    }
                    break;
                case eBaseType.TimeSpan:
                    {
                        switch (gen_type)
                        {
                            case eGenType.cpp: returnTypeName = "double"; break;
                            case eGenType.cs: returnTypeName = "System.TimeSpan"; break;
                            case eGenType.mssql: returnTypeName = "varchar(300)"; break;
                            case eGenType.mysql: returnTypeName = "varchar(300)"; break;
                            case eGenType.sqllite: returnTypeName = "text"; break;
                        }
                    }
                    break;

            }
            return returnTypeName;
        }
    }
}
