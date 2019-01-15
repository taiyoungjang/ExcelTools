using System;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = System.IO.Path.GetFullPath($"{System.Environment.CurrentDirectory}/../../../Bytes");
            bool ret = TableReader.Read<TBL.ItemTable.Loader>(path, "kor");
            if(ret)
            {
                foreach(var tbl_item in TBL.ItemTable.Item._array)
                {
                    System.Console.WriteLine($"Item_ID:{tbl_item.Item_ID} {tbl_item.Name} {tbl_item.DictionaryType}");
                }
            }
        }
    }
}
