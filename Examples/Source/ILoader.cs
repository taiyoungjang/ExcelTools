namespace TBL
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    public static class Base
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
        void CheckReplaceFile(string tempFileName, string fileName);
        string GetFileName();
        byte[] GetHash(System.IO.Stream stream);
        //void GetMapAndArray(System.Collections.Generic.Dictionary<string, object> container);

#if !UNITY_5_3_OR_NEWER
        System.Data.DataSet DataSet { get; set; }
#endif
    }
    public static class Encoder
    {
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
}
