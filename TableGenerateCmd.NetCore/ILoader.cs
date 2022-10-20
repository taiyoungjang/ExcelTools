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
        void ExcelLoad(string path, string language);
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
            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            uint v = (uint)value;   // support negative numbers
            while (v >= 0x80)
            {
                writer__.Write((byte)(v | 0x80));
                v >>= 7;
            }
            writer__.Write((byte)v);
        }
        public static int Read7BitEncodedInt(ref System.IO.BinaryReader reader__)
        {
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
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
                if (file1.Length != file2.Length)
                {
                    return false;
                }
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

        public static void CheckReplaceFile( MemoryStream tempFile, string fileName, bool usingPerforce)
        {
            fileName = System.IO.Path.GetFullPath(fileName);
            if (FileEquals(tempFile, fileName) == false)
            {
                if (usingPerforce)
                {
                    string command = "add";
                    if (System.IO.File.Exists(fileName))
                    {
                        var attributes = System.IO.File.GetAttributes(fileName);
                        var isReadOnly = (attributes & FileAttributes.ReadOnly) != 0;
                        command = "edit";
                    }
                    System.Diagnostics.Process.Start("p4", $"{command} {fileName}");
                }

                File.WriteAllBytes(fileName, tempFile.ToArray());
            }
        }
    }
}
