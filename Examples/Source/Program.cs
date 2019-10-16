using System;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = System.IO.Path.GetFullPath($"{System.Environment.CurrentDirectory}/../../../Bytes");
            bool ret = TableReader.Read<TBL.ItemTable.Loader>(path, "Korean");
            if(ret)
            {
                foreach(var tbl_item in TBL.ItemTable.Item.Array_)
                {
                    ItemID b = 100;
                    var compare = tbl_item.Item_ID == 100;
                    var eq = tbl_item.Item_ID.Equals(100);
                    var eq2 = tbl_item.Item_ID == b;
                    System.Console.WriteLine($"Item_ID:{tbl_item.Item_ID} {tbl_item.Name} {tbl_item.DictionaryType}");
                }
            }
        }
    }
}
