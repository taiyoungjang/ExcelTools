using System;
using System.IO;
using NUnit.Framework;
using TBL;
using TBL.ItemTable;


namespace Examples
{
    static class Program
    {
        [TestCase("English")]
        public static void ExcelLoad(string language)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var files = System.IO.Directory.GetFiles($"{baseDirectory}../../../Excel", "*.xlsm", SearchOption.AllDirectories);
            var loader = new TBL.ItemTable.Loader();
            foreach (var path in files)
            {
                loader.ExcelLoad(path, language);
                Assert.True(TBL.ItemTable.Item.Array_.Length > 0);
            }
        }
        [TestCase("English")]
        public static void WriteFile(string language)
        {
            ExcelLoad(language);
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            {
                var loader = new TBL.ItemTable.Loader();
                //Tests.TestData();
                var path = string.Format($"{baseDirectory}../../../Bytes/{language}");
                loader.WriteFile(path, false);
            }
        }
        [TestCase("English")]
        public static void ReadFile(string language)
        {
            var loader = new TBL.ItemTable.Loader();
            var fileName = System.IO.Path.GetFullPath($"{AppDomain.CurrentDomain.BaseDirectory}../../../Bytes/{language}/{loader.GetFileName()}.bytes");
            {
                //Tests.TestData();
                var fs = new System.IO.FileStream(fileName, FileMode.Open);
                loader.ReadStream(fs);
            }
        }
        [STAThread]
        static void Test(string[] args)
        {
            string path = System.IO.Path.GetFullPath($"{System.Environment.CurrentDirectory}/../../../Bytes");
            TBL.Base_.Path = path;
            TBL.Base_.Language = "English";
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
