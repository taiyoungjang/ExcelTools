namespace TBL
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    public interface ILoader
    {
        void Init();
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