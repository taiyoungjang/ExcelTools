using System;
using System.Collections.Generic;
using System.Text;

namespace Examples
{
    public static class TableReader
    {
        public static bool Read<T>(string location, string language) where T : global::TBL.ILoader, new()
        {
            T loader = new T();
            string path = System.IO.Path.GetFullPath($"{location}/{language}/{loader.GetFileName()}.bytes");
            using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                loader.ReadStream(stream);
            }
            return true;
        }
    }
}
