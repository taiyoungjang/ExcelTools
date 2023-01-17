using System.Diagnostics;

namespace TBL
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.IO;
    using System.Linq;
    public static class Base_
    {
        public static string Path { get; set; }
        public static string Language { get; set; }
    }
    public interface ILoader
    {
        void ReadStream(System.IO.Stream stream);
#if !UNITY_5_3_OR_NEWER && !NO_EXCEL_LOADER
        void ExcelLoad(string path, string language, string dataStage);
        public static string ColumnIndexToColumnLetter(int colIndex)
        {
            int div = colIndex;
            string colLetter = string.Empty;
            int mod = 0;
 
            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }
#endif
        string GetFileName();
        byte[] GetHash(System.IO.Stream stream);
        //void GetMapAndArray(System.Collections.Generic.Dictionary<string, object> container);

// #if !UNITY_5_3_OR_NEWER
//         System.Data.DataSet DataSet { get; set; }
// #endif
    }
    public static class Encoder
    {
        public static void Write(this System.IO.BinaryWriter writer__, string value)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            Write7BitEncodedInt(writer__, bytes.Length);
            if (bytes.Length > 0)
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
        public static void Write7BitEncodedInt(this System.IO.BinaryWriter writer__, int value)
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
            if (!x_empty && !y_empty)
            {
                return x.Equals(y);
            }
            if (x_empty && y_empty)
            {
                return true;
            }
            return false;
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
    public static class FileExtensions
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
            catch (System.Exception)
            {
            }
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

        public static void CheckReplaceFile(string tempFileName, string fileName2, bool usingPerforce)
        {
            tempFileName = System.IO.Path.GetFullPath(tempFileName);
            fileName2 = System.IO.Path.GetFullPath(fileName2);
            if (FileEquals(tempFileName, fileName2) == false)
            {
                if (usingPerforce)
                {
                    string command = "add";
                    if (System.IO.File.Exists(fileName2))
                    {
                        var attributes = System.IO.File.GetAttributes(fileName2);
                        var isReadOnly = (attributes & FileAttributes.ReadOnly) != 0;
                        command = "edit";
                    }
                    System.Diagnostics.Process.Start("p4", $"{command} {fileName2}");
                }
                File.Copy(tempFileName, fileName2, true);
            }
        }
        public static void CheckReplaceFile( MemoryStream tempFile, string fileName2, bool usingPerforce)
        {
            var fileName1 = System.IO.Path.GetTempFileName();
            {
                using var file1 = new System.IO.FileStream(fileName1, FileMode.Create);
                var array = tempFile.ToArray();
                file1.Write(array, 0, array.Length);
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
    }
}
