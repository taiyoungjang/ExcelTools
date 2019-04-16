// generate ItemTable
// DO NOT TOUCH SOURCE....
#if UNITY_EDITOR
public class ItemTableManager : UnityEngine.MonoBehaviour
{
  public System.Collections.Generic.List<TBL.ItemTable.Item> Item = new System.Collections.Generic.List<TBL.ItemTable.Item>();
  public System.Collections.Generic.List<TBL.ItemTable.ItemEffect> ItemEffect = new System.Collections.Generic.List<TBL.ItemTable.ItemEffect>();
  public System.Collections.Generic.List<TBL.ItemTable.ItemEnchant> ItemEnchant = new System.Collections.Generic.List<TBL.ItemTable.ItemEnchant>();
  public System.Collections.Generic.List<TBL.ItemTable.ItemManufacture> ItemManufacture = new System.Collections.Generic.List<TBL.ItemTable.ItemManufacture>();
  public System.Collections.Generic.List<TBL.ItemTable.RandomBoxGroup> RandomBoxGroup = new System.Collections.Generic.List<TBL.ItemTable.RandomBoxGroup>();
}
[UnityEditor.CustomEditor(typeof(ItemTableManager))]
public class ItemTableEditor : UnityEditor.Editor
{
  public override void OnInspectorGUI()
  {
    ItemTableManager myScript = (ItemTableManager) target;
    if(UnityEngine.GUILayout.Button("Load"))
    {
      myScript.Item = TBL.ItemTable.Item._array;
      myScript.ItemEffect = TBL.ItemTable.ItemEffect._array;
      myScript.ItemEnchant = TBL.ItemTable.ItemEnchant._array;
      myScript.ItemManufacture = TBL.ItemTable.ItemManufacture._array;
      myScript.RandomBoxGroup = TBL.ItemTable.RandomBoxGroup._array;
    }
    DrawDefaultInspector();
  }
}
#endif


namespace TBL.ItemTable
{
  public class Loader : global::TBL.ILoader
  {
    public static Loader Instance = new Loader();
    public void Init()
    {
      Instance = this;
    }
    #if !UNITY_2018_2_OR_NEWER
    public System.Data.DataSet DataSet
    {
      get
      {
        System.Data.DataSet dts = new System.Data.DataSet("ItemTable");
        Item.GetDataTable(dts);
        ItemEffect.GetDataTable(dts);
        ItemEnchant.GetDataTable(dts);
        ItemManufacture.GetDataTable(dts);
        RandomBoxGroup.GetDataTable(dts);
        return dts;
      }
      set
      {
        Item.SetDataSet(value);
        ItemEffect.SetDataSet(value);
        ItemEnchant.SetDataSet(value);
        ItemManufacture.SetDataSet(value);
        RandomBoxGroup.SetDataSet(value);
      }
    }
    #if !NO_EXCEL_LOADER
    public void ExcelLoad(string path, string language)
    {
      language = language.Trim().ToLower();
      string directoryName = System.IO.Path.GetDirectoryName(path);
      var imp = new ClassUtil.ExcelImporter();
      imp.Open(path);
      Item.ExcelLoad(imp,directoryName,language);
      ItemEffect.ExcelLoad(imp,directoryName,language);
      ItemEnchant.ExcelLoad(imp,directoryName,language);
      ItemManufacture.ExcelLoad(imp,directoryName,language);
      RandomBoxGroup.ExcelLoad(imp,directoryName,language);
      imp.Dispose();
    }
    #endif
    #endif
    /*
    public void GetMapAndArray(System.Collections.Generic.Dictionary<string,object> container)
    {
      container.Remove( "ItemTable.Item._map");
      container.Remove( "ItemTable.Item._array");
      container.Add( "ItemTable.Item._map", new System.Collections.Generic.Dictionary<int,Item>(Item._map,default(global::TBL.IntEqualityComparer)));
      container.Add( "ItemTable.Item._array",new System.Collections.Generic.List<Item>(Item._array));
      container.Remove( "ItemTable.ItemEffect._map");
      container.Remove( "ItemTable.ItemEffect._array");
      container.Add( "ItemTable.ItemEffect._map", new System.Collections.Generic.Dictionary<int,ItemEffect>(ItemEffect._map,default(global::TBL.IntEqualityComparer)));
      container.Add( "ItemTable.ItemEffect._array",new System.Collections.Generic.List<ItemEffect>(ItemEffect._array));
      container.Remove( "ItemTable.ItemEnchant._map");
      container.Remove( "ItemTable.ItemEnchant._array");
      container.Add( "ItemTable.ItemEnchant._map", new System.Collections.Generic.Dictionary<int,ItemEnchant>(ItemEnchant._map,default(global::TBL.IntEqualityComparer)));
      container.Add( "ItemTable.ItemEnchant._array",new System.Collections.Generic.List<ItemEnchant>(ItemEnchant._array));
      container.Remove( "ItemTable.ItemManufacture._map");
      container.Remove( "ItemTable.ItemManufacture._array");
      container.Add( "ItemTable.ItemManufacture._map", new System.Collections.Generic.Dictionary<int,ItemManufacture>(ItemManufacture._map,default(global::TBL.IntEqualityComparer)));
      container.Add( "ItemTable.ItemManufacture._array",new System.Collections.Generic.List<ItemManufacture>(ItemManufacture._array));
      container.Remove( "ItemTable.RandomBoxGroup._map");
      container.Remove( "ItemTable.RandomBoxGroup._array");
      container.Add( "ItemTable.RandomBoxGroup._map", new System.Collections.Generic.Dictionary<int,RandomBoxGroup>(RandomBoxGroup._map,default(global::TBL.IntEqualityComparer)));
      container.Add( "ItemTable.RandomBoxGroup._array",new System.Collections.Generic.List<RandomBoxGroup>(RandomBoxGroup._array));
    }
    */
    public void CheckReplaceFile( string tempFileName, string fileName ) 
    {
      System.IO.File.Copy(tempFileName, fileName, true);
    }
    #if !NO_EXCEL_LOADER
    public void WriteFile(string path)
    {
      int uncompressedLength = 0;
      System.IO.MemoryStream uncompressedMemoryStream = null;
      uncompressedMemoryStream = new System.IO.MemoryStream(128 * 1024);
      {
        var uncompressedMemoryStreamWriter = new System.IO.BinaryWriter(uncompressedMemoryStream);
        Item.WriteStream(uncompressedMemoryStreamWriter);
        ItemEffect.WriteStream(uncompressedMemoryStreamWriter);
        ItemEnchant.WriteStream(uncompressedMemoryStreamWriter);
        ItemManufacture.WriteStream(uncompressedMemoryStreamWriter);
        RandomBoxGroup.WriteStream(uncompressedMemoryStreamWriter);
        uncompressedLength = (int) uncompressedMemoryStream.Position;
      }
      System.IO.FileStream stream = null;
      try
      {
        string tempFileName = System.IO.Path.GetTempFileName();
        uncompressedMemoryStream.Position=0;
        stream = new System.IO.FileStream(tempFileName, System.IO.FileMode.Create);
        {
          using (System.IO.MemoryStream __zip = new System.IO.MemoryStream())
          {
            ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(uncompressedMemoryStream, __zip,false,1);
            using(var md5 = System.Security.Cryptography.MD5.Create())
            {
              var __compressed = __zip.ToArray();
              byte[] hashBytes = md5.ComputeHash(__compressed);
              stream.WriteByte((byte)hashBytes.Length);
              stream.Write(hashBytes, 0, hashBytes.Length);
              stream.Write( System.BitConverter.GetBytes(uncompressedLength), 0, 4 );
              stream.Write( System.BitConverter.GetBytes(__compressed.Length), 0, 4 );
              stream.Write(__compressed, 0, __compressed.Length);
            }
          }
        }
        stream.Flush();
        stream.Close();
        stream = null;
        CheckReplaceFile(tempFileName, System.IO.Path.GetDirectoryName( path + "/") + "/ItemTable.bytes");
      }catch(System.Exception e)
      {
        System.Console.WriteLine(e.ToString());
        throw;
      }
      finally
      {
        if(uncompressedMemoryStream != null) uncompressedMemoryStream.Dispose();
      }
    }
    #endif //NO_EXCEL_LOADER
      public string GetFileName() { return "ItemTable"; }
    public void ReadStream(System.IO.Stream stream)
    {
      stream.Position = 0;
      int streamLength = (int)stream.Length;
      int hashLength = stream.ReadByte();
      byte[] uncompressedSize = new byte[4];
      byte[] compressedSize = new byte[4];
      byte[] hashBytes = new byte[hashLength];
      stream.Read( hashBytes, 0, hashLength);
      byte[] bytes = new byte[streamLength-hashLength-compressedSize.Length-uncompressedSize.Length-1];
      stream.Read( uncompressedSize, 0, uncompressedSize.Length);
      stream.Read( compressedSize, 0, compressedSize.Length);
      stream.Read( bytes, 0, streamLength-hashLength-compressedSize.Length-uncompressedSize.Length-1);
      using(var md5 = System.Security.Cryptography.MD5.Create())
      {
        byte[] dataBytes = md5.ComputeHash(bytes);
        if(!System.Linq.Enumerable.SequenceEqual(hashBytes, dataBytes))
        throw new System.Exception("ItemTable verify failure...");
      }
      using (System.IO.MemoryStream __ms = new System.IO.MemoryStream(bytes))
      {
        using (var decompressStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(__ms))
        {
          int uncompressedSize__ = System.BitConverter.ToInt32(uncompressedSize,0);
          bytes = new byte[uncompressedSize__];
          decompressStream.Read(bytes, 0, uncompressedSize__);
        }
      }
      {
        System.IO.MemoryStream __ms = null;
        try
        {
          __ms = new System.IO.MemoryStream(bytes);
          using (System.IO.BinaryReader reader = new System.IO.BinaryReader(__ms))
          {
            __ms = null;
            Item.ReadStream(reader);
            ItemEffect.ReadStream(reader);
            ItemEnchant.ReadStream(reader);
            ItemManufacture.ReadStream(reader);
            RandomBoxGroup.ReadStream(reader);
          }
        }
        finally
        {
          if(__ms != null) __ms.Dispose();
        }
      }
    }
    public byte[] GetHash(System.IO.Stream stream)
    {
      stream.Position = 0;
      int hashLength = stream.ReadByte();
      byte[] hashBytes = new byte[hashLength];
      stream.Read( hashBytes, 0, hashLength);
      return hashBytes;
    }
  }
  

  #if !ENCRYPT
  [System.Serializable]
  #endif
  public partial class Item : BaseClasses.Item
  {
    public static System.Collections.Generic.List<Item> _array = null;
    public static System.Collections.Generic.Dictionary<int,Item> _map = null;
    

    public static void ArrayToMap(System.Collections.Generic.List<Item> array__)
    {
      _map = new System.Collections.Generic.Dictionary<int,Item> (array__.Count,default(global::TBL.IntEqualityComparer));
      Item __table = null;
      for( int __i=0;__i<array__.Count;__i++)
      {
        __table = array__[__i];
        try{
          _map.Add(__table.Item_ID, __table);
        }catch(System.Exception e)
        {
          throw new System.Exception(__table.Item_ID.ToString() + " " + e.Message);
        }
      }
    }
    

    public static void WriteStream(System.IO.BinaryWriter __writer)
    {
      __writer.Write(_array.Count);
      for (var __i=0;__i<_array.Count;__i++)
      {
        var __table = _array[__i];
        __writer.Write(__table.Item_ID);
        __writer.Write(__table.Name);
        __writer.Write(__table.Item_grade);
        __writer.Write(__table.Require_lv);
        __writer.Write(__table.Enchant_lv);
        __writer.Write(__table.PhysicalAttack);
        __writer.Write(__table.PhysicalDefense);
        __writer.Write(__table.MagicalAttack);
        __writer.Write(__table.MagicalDefense);
        __writer.Write(__table.Critical);
        __writer.Write(__table.HP);
        __writer.Write(__table.KnockBackResist);
        __writer.Write((int)__table.DictionaryType);
        __writer.Write(__table.ItemType);
        __writer.Write(__table.Gear_Score);
        __writer.Write(__table.InventoryType);
        __writer.Write(__table.UsageType);
        __writer.Write(__table.Socket_quantity);
        __writer.Write(__table.Removal_cost);
        __writer.Write(__table.Belonging);
        __writer.Write(__table.Sub_stats_quantity);
        __writer.Write(__table.Stack);
        __writer.Write(__table.DesignScroll_ID);
        __writer.Write(__table.BindingSkill_ID);
        __writer.Write(__table.BindingAttack_ID);
        __writer.Write(__table.Manufacture_gold);
        __writer.Write(__table.Manufacture_cash);
        __writer.Write(__table.SummonCompanion_ID);
        __writer.Write(__table.Next_itemID);
        __writer.Write(__table.Next_item_price);
        TBL.Encoder.Write7BitEncodedInt(__writer,__table.Next_Item_material.Length);
        for(var j__=0;j__<__table.Next_Item_material.Length;++j__){__writer.Write(__table.Next_Item_material[j__]);}
        TBL.Encoder.Write7BitEncodedInt(__writer,__table.Next_Item_material_quantity.Length);
        for(var j__=0;j__<__table.Next_Item_material_quantity.Length;++j__){__writer.Write(__table.Next_Item_material_quantity[j__]);}
        __writer.Write(__table.Resource_Path);
        __writer.Write(__table.WeaponName);
        __writer.Write(__table.WeaponIndex);
        TBL.Encoder.Write7BitEncodedInt(__writer,__table.PartName.Length);
        for(var j__=0;j__<__table.PartName.Length;++j__){__writer.Write(__table.PartName[j__]);}
        TBL.Encoder.Write7BitEncodedInt(__writer,__table.PartIndex.Length);
        for(var j__=0;j__<__table.PartIndex.Length;++j__){__writer.Write(__table.PartIndex[j__]);}
        __writer.Write(__table.Icon_path);
        __writer.Write(__table.EXP);
        __writer.Write(__table.Buy_cost);
        __writer.Write(__table.Sell_reward);
        __writer.Write(__table.Consignment_maxprice);
        __writer.Write(__table.QuestBringer);
        __writer.Write(__table.ItemEvent_ID);
        __writer.Write(__table.Description);
        __writer.Write(__table.Sub_Item);
        __writer.Write(__table.WeaponType);
        TBL.Encoder.Write7BitEncodedInt(__writer,__table.RandomBoxGroup_NO.Length);
        for(var j__=0;j__<__table.RandomBoxGroup_NO.Length;++j__){__writer.Write(__table.RandomBoxGroup_NO[j__]);}
      }
    }
    #if !UNITY_5_3_OR_NEWER
    public static bool SetDataSet(System.Data.DataSet dts)
    {
      Item._map = new System.Collections.Generic.Dictionary<int,Item>();
      Item._array = new System.Collections.Generic.List<Item>();
      foreach (System.Data.DataRow row in dts.Tables["Item"].Rows)
      {
        Item table = new Item
        (
        System.Convert.ToInt32(System.Math.Round(double.Parse(row["Item_ID"].ToString())))
        ,row["Name"].ToString()
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Item_grade"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Require_lv"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Enchant_lv"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["PhysicalAttack"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["PhysicalDefense"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["MagicalAttack"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["MagicalDefense"].ToString())))
        ,System.Convert.ToSingle(row["Critical"].ToString())
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["HP"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["KnockBackResist"].ToString())))
        ,(eDictionaryType) System.Enum.Parse(typeof(eDictionaryType),row["DictionaryType"].ToString())
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["ItemType"].ToString())))
        ,System.Convert.ToInt16(System.Math.Round(double.Parse(row["Gear_Score"].ToString())))
        ,System.Convert.ToInt16(System.Math.Round(double.Parse(row["InventoryType"].ToString())))
        ,(row["UsageType"].ToString().Trim()=="1"||row["UsageType"].ToString().Trim().ToUpper()=="TRUE")?true:false
        ,System.Convert.ToInt16(System.Math.Round(double.Parse(row["Socket_quantity"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Removal_cost"].ToString())))
        ,System.Convert.ToInt16(System.Math.Round(double.Parse(row["Belonging"].ToString())))
        ,System.Convert.ToInt16(System.Math.Round(double.Parse(row["Sub_stats_quantity"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Stack"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["DesignScroll_ID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["BindingSkill_ID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["BindingAttack_ID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Manufacture_gold"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Manufacture_cash"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["SummonCompanion_ID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Next_itemID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Next_item_price"].ToString())))
        ,new int[]
        {
          System.Convert.ToInt32(System.Math.Round(double.Parse(row["Next_Item_material0"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Next_Item_material1"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Next_Item_material2"].ToString())))
        }
        ,new int[]
        {
          System.Convert.ToInt32(System.Math.Round(double.Parse(row["Next_Item_material_quantity0"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Next_Item_material_quantity1"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Next_Item_material_quantity2"].ToString())))
        }
        ,row["Resource_Path"].ToString()
        ,row["WeaponName"].ToString()
        ,System.Convert.ToInt16(System.Math.Round(double.Parse(row["WeaponIndex"].ToString())))
        ,new string[]
        {
          row["PartName0"].ToString()
        }
        ,new short[]
        {
          System.Convert.ToInt16(System.Math.Round(double.Parse(row["PartIndex0"].ToString())))
        }
        ,row["Icon_path"].ToString()
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["EXP"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Buy_cost"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Sell_reward"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Consignment_maxprice"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["QuestBringer"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["ItemEvent_ID"].ToString())))
        ,row["Description"].ToString()
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Sub_Item"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["WeaponType"].ToString())))
        ,new int[]
        {
          System.Convert.ToInt32(System.Math.Round(double.Parse(row["RandomBoxGroup_NO0"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["RandomBoxGroup_NO1"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["RandomBoxGroup_NO2"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["RandomBoxGroup_NO3"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["RandomBoxGroup_NO4"].ToString())))
        }
        );
        Item._map.Add(table.Item_ID, table);
        Item._array.Add(table);
      }
      return true;
    }
    #if !NO_EXCEL_LOADER
    public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
    {
      var i=0; var j=0;
      string[,] rows = null;
      int Item_ID;
      string Name;
      int Item_grade;
      int Require_lv;
      int Enchant_lv;
      int PhysicalAttack;
      int PhysicalDefense;
      int MagicalAttack;
      int MagicalDefense;
      float Critical;
      int HP;
      int KnockBackResist;
      eDictionaryType DictionaryType;
      int ItemType;
      short Gear_Score;
      short InventoryType;
      bool UsageType;
      short Socket_quantity;
      int Removal_cost;
      short Belonging;
      short Sub_stats_quantity;
      int Stack;
      int DesignScroll_ID;
      int BindingSkill_ID;
      int BindingAttack_ID;
      int Manufacture_gold;
      int Manufacture_cash;
      int SummonCompanion_ID;
      int Next_itemID;
      int Next_item_price;
      int[] Next_Item_material;
      int[] Next_Item_material_quantity;
      string Resource_Path;
      string WeaponName;
      short WeaponIndex;
      string[] PartName;
      short[] PartIndex;
      string Icon_path;
      int EXP;
      int Buy_cost;
      int Sell_reward;
      int Consignment_maxprice;
      int QuestBringer;
      int ItemEvent_ID;
      string Description;
      int Sub_Item;
      int WeaponType;
      int[] RandomBoxGroup_NO;
      try
      {
        rows = imp.GetSheet("Item", language);
        var array__ = new System.Collections.Generic.List<Item>(rows.GetLength(0) - 3);
        for (i = 3; i < rows.GetLength(0); i++)
        {
          j=0;
          if( rows[i,0].Length == 0) break;
          j = 0;
          if(string.IsNullOrEmpty(rows[i,0]))
          {
          Item_ID = 0;}else {Item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0])));
          }
          j = 4;
          Name = rows[i,4];
          j = 5;
          if(string.IsNullOrEmpty(rows[i,5]))
          {
          Item_grade = 0;}else {Item_grade = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,5])));
          }
          j = 6;
          if(string.IsNullOrEmpty(rows[i,6]))
          {
          Require_lv = 0;}else {Require_lv = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,6])));
          }
          j = 7;
          if(string.IsNullOrEmpty(rows[i,7]))
          {
          Enchant_lv = 0;}else {Enchant_lv = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,7])));
          }
          j = 8;
          if(string.IsNullOrEmpty(rows[i,8]))
          {
          PhysicalAttack = 0;}else {PhysicalAttack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,8])));
          }
          j = 9;
          if(string.IsNullOrEmpty(rows[i,9]))
          {
          PhysicalDefense = 0;}else {PhysicalDefense = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,9])));
          }
          j = 10;
          if(string.IsNullOrEmpty(rows[i,10]))
          {
          MagicalAttack = 0;}else {MagicalAttack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,10])));
          }
          j = 11;
          if(string.IsNullOrEmpty(rows[i,11]))
          {
          MagicalDefense = 0;}else {MagicalDefense = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,11])));
          }
          j = 12;
          if(string.IsNullOrEmpty(rows[i,12]))
          {
          Critical = 0;}else {Critical = System.Convert.ToSingle(rows[i,12]);
          }
          j = 13;
          if(string.IsNullOrEmpty(rows[i,13]))
          {
          HP = 0;}else {HP = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,13])));
          }
          j = 14;
          if(string.IsNullOrEmpty(rows[i,14]))
          {
          KnockBackResist = 0;}else {KnockBackResist = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,14])));
          }
          j = 16;
          if(string.IsNullOrEmpty(rows[i,16]))
          {
          DictionaryType = eDictionaryType.Consume;}else {DictionaryType = (eDictionaryType) System.Enum.Parse(typeof(eDictionaryType),rows[i,16]);
          }
          j = 17;
          if(string.IsNullOrEmpty(rows[i,17]))
          {
          ItemType = 0;}else {ItemType = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,17])));
          }
          j = 18;
          if(string.IsNullOrEmpty(rows[i,18]))
          {
          Gear_Score = 0;}else {Gear_Score = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,18])));
          }
          j = 19;
          if(string.IsNullOrEmpty(rows[i,19]))
          {
          InventoryType = 0;}else {InventoryType = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,19])));
          }
          j = 20;
          if(string.IsNullOrEmpty(rows[i,20]))
          {
          UsageType = false;}else {UsageType = (rows[i,20].Trim()=="1"||rows[i,20].Trim().ToUpper()=="TRUE")?true:false;
          }
          j = 21;
          if(string.IsNullOrEmpty(rows[i,21]))
          {
          Socket_quantity = 0;}else {Socket_quantity = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,21])));
          }
          j = 22;
          if(string.IsNullOrEmpty(rows[i,22]))
          {
          Removal_cost = 0;}else {Removal_cost = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,22])));
          }
          j = 23;
          if(string.IsNullOrEmpty(rows[i,23]))
          {
          Belonging = 0;}else {Belonging = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,23])));
          }
          j = 24;
          if(string.IsNullOrEmpty(rows[i,24]))
          {
          Sub_stats_quantity = 0;}else {Sub_stats_quantity = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,24])));
          }
          j = 25;
          if(string.IsNullOrEmpty(rows[i,25]))
          {
          Stack = 0;}else {Stack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,25])));
          }
          j = 26;
          if(string.IsNullOrEmpty(rows[i,26]))
          {
          DesignScroll_ID = 0;}else {DesignScroll_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,26])));
          }
          j = 27;
          if(string.IsNullOrEmpty(rows[i,27]))
          {
          BindingSkill_ID = 0;}else {BindingSkill_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,27])));
          }
          j = 28;
          if(string.IsNullOrEmpty(rows[i,28]))
          {
          BindingAttack_ID = 0;}else {BindingAttack_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,28])));
          }
          j = 29;
          if(string.IsNullOrEmpty(rows[i,29]))
          {
          Manufacture_gold = 0;}else {Manufacture_gold = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,29])));
          }
          j = 30;
          if(string.IsNullOrEmpty(rows[i,30]))
          {
          Manufacture_cash = 0;}else {Manufacture_cash = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,30])));
          }
          j = 31;
          if(string.IsNullOrEmpty(rows[i,31]))
          {
          SummonCompanion_ID = 0;}else {SummonCompanion_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,31])));
          }
          j = 32;
          if(string.IsNullOrEmpty(rows[i,32]))
          {
          Next_itemID = 0;}else {Next_itemID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,32])));
          }
          j = 33;
          if(string.IsNullOrEmpty(rows[i,33]))
          {
          Next_item_price = 0;}else {Next_item_price = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,33])));
          }
          Next_Item_material = new int[3];
          j = 34;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,34])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,34]))); Next_Item_material[0] = outvalue;
          }
          j = 35;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,35])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,35]))); Next_Item_material[1] = outvalue;
          }
          j = 36;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,36])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,36]))); Next_Item_material[2] = outvalue;
          }
          Next_Item_material_quantity = new int[3];
          j = 37;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,37])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,37]))); Next_Item_material_quantity[0] = outvalue;
          }
          j = 38;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,38])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,38]))); Next_Item_material_quantity[1] = outvalue;
          }
          j = 39;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,39])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,39]))); Next_Item_material_quantity[2] = outvalue;
          }
          j = 40;
          Resource_Path = rows[i,40];
          j = 41;
          WeaponName = rows[i,41];
          j = 42;
          if(string.IsNullOrEmpty(rows[i,42]))
          {
          WeaponIndex = 0;}else {WeaponIndex = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,42])));
          }
          PartName = new string[1];
          j = 43;
          PartName[0] = rows[i,43];
          PartIndex = new short[1];
          j = 44;
          {
            short outvalue = 0; if(!string.IsNullOrEmpty(rows[i,44])) 
            outvalue = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,44]))); PartIndex[0] = outvalue;
          }
          j = 45;
          Icon_path = rows[i,45];
          j = 46;
          if(string.IsNullOrEmpty(rows[i,46]))
          {
          EXP = 0;}else {EXP = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,46])));
          }
          j = 47;
          if(string.IsNullOrEmpty(rows[i,47]))
          {
          Buy_cost = 0;}else {Buy_cost = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,47])));
          }
          j = 48;
          if(string.IsNullOrEmpty(rows[i,48]))
          {
          Sell_reward = 0;}else {Sell_reward = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,48])));
          }
          j = 49;
          if(string.IsNullOrEmpty(rows[i,49]))
          {
          Consignment_maxprice = 0;}else {Consignment_maxprice = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,49])));
          }
          j = 50;
          if(string.IsNullOrEmpty(rows[i,50]))
          {
          QuestBringer = 0;}else {QuestBringer = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,50])));
          }
          j = 51;
          if(string.IsNullOrEmpty(rows[i,51]))
          {
          ItemEvent_ID = 0;}else {ItemEvent_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,51])));
          }
          j = 52;
          Description = rows[i,52];
          j = 53;
          if(string.IsNullOrEmpty(rows[i,53]))
          {
          Sub_Item = 0;}else {Sub_Item = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,53])));
          }
          j = 54;
          if(string.IsNullOrEmpty(rows[i,54]))
          {
          WeaponType = 0;}else {WeaponType = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,54])));
          }
          RandomBoxGroup_NO = new int[5];
          j = 55;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,55])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,55]))); RandomBoxGroup_NO[0] = outvalue;
          }
          j = 56;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,56])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,56]))); RandomBoxGroup_NO[1] = outvalue;
          }
          j = 57;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,57])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,57]))); RandomBoxGroup_NO[2] = outvalue;
          }
          j = 58;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,58])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,58]))); RandomBoxGroup_NO[3] = outvalue;
          }
          j = 59;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,59])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,59]))); RandomBoxGroup_NO[4] = outvalue;
          }
          Item values = new Item(Item_ID,Name,Item_grade,Require_lv,Enchant_lv,PhysicalAttack,PhysicalDefense,MagicalAttack,MagicalDefense,Critical,HP,KnockBackResist,DictionaryType,ItemType,Gear_Score,InventoryType,UsageType,Socket_quantity,Removal_cost,Belonging,Sub_stats_quantity,Stack,DesignScroll_ID,BindingSkill_ID,BindingAttack_ID,Manufacture_gold,Manufacture_cash,SummonCompanion_ID,Next_itemID,Next_item_price,Next_Item_material,Next_Item_material_quantity,Resource_Path,WeaponName,WeaponIndex,PartName,PartIndex,Icon_path,EXP,Buy_cost,Sell_reward,Consignment_maxprice,QuestBringer,ItemEvent_ID,Description,Sub_Item,WeaponType,RandomBoxGroup_NO);
          foreach (var preValues in array__)
          {
            if (preValues.Item_ID == Item_ID)
            {
              i=0;j=0;
              throw new System.Exception("Item.Item_ID:" + preValues.Item_ID.ToString() + ") Duplicated!!");
            }
          }
          array__.Add(values);
        }
        ArrayToMap(array__);
        _array = array__;
      }catch(System.Exception e)
      {
        if( rows == null ) throw;
        if (j >= rows.GetLength(1))
          throw new System.Exception("sheet(Item) invalid column count:" + j);
        throw new System.Exception(" convert failure : excel(ItemTable).sheet(Item) key:" + rows[i,0] + " Name:" + rows[0,j] + " " + rows[i,j] + " " + e.Message );
      }
    }
    #endif
    public static void GetDataTable(System.Data.DataSet dts)
    {
      System.Data.DataTable table = dts.Tables.Add("Item");
      table.Columns.Add("Item_ID", typeof(int));
      table.Columns.Add("Name", typeof(string));
      table.Columns.Add("Item_grade", typeof(int));
      table.Columns.Add("Require_lv", typeof(int));
      table.Columns.Add("Enchant_lv", typeof(int));
      table.Columns.Add("PhysicalAttack", typeof(int));
      table.Columns.Add("PhysicalDefense", typeof(int));
      table.Columns.Add("MagicalAttack", typeof(int));
      table.Columns.Add("MagicalDefense", typeof(int));
      table.Columns.Add("Critical", typeof(float));
      table.Columns.Add("HP", typeof(int));
      table.Columns.Add("KnockBackResist", typeof(int));
      table.Columns.Add("DictionaryType", typeof(eDictionaryType));
      table.Columns.Add("ItemType", typeof(int));
      table.Columns.Add("Gear_Score", typeof(short));
      table.Columns.Add("InventoryType", typeof(short));
      table.Columns.Add("UsageType", typeof(bool));
      table.Columns.Add("Socket_quantity", typeof(short));
      table.Columns.Add("Removal_cost", typeof(int));
      table.Columns.Add("Belonging", typeof(short));
      table.Columns.Add("Sub_stats_quantity", typeof(short));
      table.Columns.Add("Stack", typeof(int));
      table.Columns.Add("DesignScroll_ID", typeof(int));
      table.Columns.Add("BindingSkill_ID", typeof(int));
      table.Columns.Add("BindingAttack_ID", typeof(int));
      table.Columns.Add("Manufacture_gold", typeof(int));
      table.Columns.Add("Manufacture_cash", typeof(int));
      table.Columns.Add("SummonCompanion_ID", typeof(int));
      table.Columns.Add("Next_itemID", typeof(int));
      table.Columns.Add("Next_item_price", typeof(int));
      table.Columns.Add("Next_Item_material0", typeof(int));
      table.Columns.Add("Next_Item_material1", typeof(int));
      table.Columns.Add("Next_Item_material2", typeof(int));
      table.Columns.Add("Next_Item_material_quantity0", typeof(int));
      table.Columns.Add("Next_Item_material_quantity1", typeof(int));
      table.Columns.Add("Next_Item_material_quantity2", typeof(int));
      table.Columns.Add("Resource_Path", typeof(string));
      table.Columns.Add("WeaponName", typeof(string));
      table.Columns.Add("WeaponIndex", typeof(short));
      table.Columns.Add("PartName0", typeof(string));
      table.Columns.Add("PartIndex0", typeof(short));
      table.Columns.Add("Icon_path", typeof(string));
      table.Columns.Add("EXP", typeof(int));
      table.Columns.Add("Buy_cost", typeof(int));
      table.Columns.Add("Sell_reward", typeof(int));
      table.Columns.Add("Consignment_maxprice", typeof(int));
      table.Columns.Add("QuestBringer", typeof(int));
      table.Columns.Add("ItemEvent_ID", typeof(int));
      table.Columns.Add("Description", typeof(string));
      table.Columns.Add("Sub_Item", typeof(int));
      table.Columns.Add("WeaponType", typeof(int));
      table.Columns.Add("RandomBoxGroup_NO0", typeof(int));
      table.Columns.Add("RandomBoxGroup_NO1", typeof(int));
      table.Columns.Add("RandomBoxGroup_NO2", typeof(int));
      table.Columns.Add("RandomBoxGroup_NO3", typeof(int));
      table.Columns.Add("RandomBoxGroup_NO4", typeof(int));
      foreach(var item in _array )
      {
        table.Rows.Add(
        item.Item_ID
        ,item.Name
        ,item.Item_grade
        ,item.Require_lv
        ,item.Enchant_lv
        ,item.PhysicalAttack
        ,item.PhysicalDefense
        ,item.MagicalAttack
        ,item.MagicalDefense
        ,item.Critical
        ,item.HP
        ,item.KnockBackResist
        ,item.DictionaryType
        ,item.ItemType
        ,item.Gear_Score
        ,item.InventoryType
        ,item.UsageType
        ,item.Socket_quantity
        ,item.Removal_cost
        ,item.Belonging
        ,item.Sub_stats_quantity
        ,item.Stack
        ,item.DesignScroll_ID
        ,item.BindingSkill_ID
        ,item.BindingAttack_ID
        ,item.Manufacture_gold
        ,item.Manufacture_cash
        ,item.SummonCompanion_ID
        ,item.Next_itemID
        ,item.Next_item_price
        ,item.Next_Item_material[0]
        ,item.Next_Item_material[1]
        ,item.Next_Item_material[2]
        ,item.Next_Item_material_quantity[0]
        ,item.Next_Item_material_quantity[1]
        ,item.Next_Item_material_quantity[2]
        ,item.Resource_Path
        ,item.WeaponName
        ,item.WeaponIndex
        ,item.PartName[0]
        ,item.PartIndex[0]
        ,item.Icon_path
        ,item.EXP
        ,item.Buy_cost
        ,item.Sell_reward
        ,item.Consignment_maxprice
        ,item.QuestBringer
        ,item.ItemEvent_ID
        ,item.Description
        ,item.Sub_Item
        ,item.WeaponType
        ,item.RandomBoxGroup_NO[0]
        ,item.RandomBoxGroup_NO[1]
        ,item.RandomBoxGroup_NO[2]
        ,item.RandomBoxGroup_NO[3]
        ,item.RandomBoxGroup_NO[4]
        );
      }
    }
    #endif
    public static void ReadStream(System.IO.BinaryReader __reader)
    {
      var array__ = new System.Collections.Generic.List<Item>();
      int __count = __reader.ReadInt32();
      for (int __i=0;__i<__count;__i++)
      {
        var Item_ID = __reader.ReadInt32();
        var Name = __reader.ReadString();
        var Item_grade = __reader.ReadInt32();
        var Require_lv = __reader.ReadInt32();
        var Enchant_lv = __reader.ReadInt32();
        var PhysicalAttack = __reader.ReadInt32();
        var PhysicalDefense = __reader.ReadInt32();
        var MagicalAttack = __reader.ReadInt32();
        var MagicalDefense = __reader.ReadInt32();
        var Critical = __reader.ReadSingle();
        var HP = __reader.ReadInt32();
        var KnockBackResist = __reader.ReadInt32();
        var DictionaryType = (eDictionaryType)__reader.ReadInt32();
        var ItemType = __reader.ReadInt32();
        var Gear_Score = __reader.ReadInt16();
        var InventoryType = __reader.ReadInt16();
        var UsageType = __reader.ReadBoolean();
        var Socket_quantity = __reader.ReadInt16();
        var Removal_cost = __reader.ReadInt32();
        var Belonging = __reader.ReadInt16();
        var Sub_stats_quantity = __reader.ReadInt16();
        var Stack = __reader.ReadInt32();
        var DesignScroll_ID = __reader.ReadInt32();
        var BindingSkill_ID = __reader.ReadInt32();
        var BindingAttack_ID = __reader.ReadInt32();
        var Manufacture_gold = __reader.ReadInt32();
        var Manufacture_cash = __reader.ReadInt32();
        var SummonCompanion_ID = __reader.ReadInt32();
        var Next_itemID = __reader.ReadInt32();
        var Next_item_price = __reader.ReadInt32();
        int[] Next_Item_material = null;
        {
          var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
          Next_Item_material = new int[arrayCount__];
          for(var __j=0;__j<arrayCount__;++__j)Next_Item_material[__j] = __reader.ReadInt32();
        }
        int[] Next_Item_material_quantity = null;
        {
          var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
          Next_Item_material_quantity = new int[arrayCount__];
          for(var __j=0;__j<arrayCount__;++__j)Next_Item_material_quantity[__j] = __reader.ReadInt32();
        }
        var Resource_Path = __reader.ReadString();
        var WeaponName = __reader.ReadString();
        var WeaponIndex = __reader.ReadInt16();
        string[] PartName = null;
        {
          var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
          PartName = new string[arrayCount__];
          for(var __j=0;__j<arrayCount__;++__j)PartName[__j] = __reader.ReadString();
        }
        short[] PartIndex = null;
        {
          var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
          PartIndex = new short[arrayCount__];
          for(var __j=0;__j<arrayCount__;++__j)PartIndex[__j] = __reader.ReadInt16();
        }
        var Icon_path = __reader.ReadString();
        var EXP = __reader.ReadInt32();
        var Buy_cost = __reader.ReadInt32();
        var Sell_reward = __reader.ReadInt32();
        var Consignment_maxprice = __reader.ReadInt32();
        var QuestBringer = __reader.ReadInt32();
        var ItemEvent_ID = __reader.ReadInt32();
        var Description = __reader.ReadString();
        var Sub_Item = __reader.ReadInt32();
        var WeaponType = __reader.ReadInt32();
        int[] RandomBoxGroup_NO = null;
        {
          var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
          RandomBoxGroup_NO = new int[arrayCount__];
          for(var __j=0;__j<arrayCount__;++__j)RandomBoxGroup_NO[__j] = __reader.ReadInt32();
        }
        Item __table = new Item(Item_ID,Name,Item_grade,Require_lv,Enchant_lv,PhysicalAttack,PhysicalDefense,MagicalAttack,MagicalDefense,Critical,HP,KnockBackResist,DictionaryType,ItemType,Gear_Score,InventoryType,UsageType,Socket_quantity,Removal_cost,Belonging,Sub_stats_quantity,Stack,DesignScroll_ID,BindingSkill_ID,BindingAttack_ID,Manufacture_gold,Manufacture_cash,SummonCompanion_ID,Next_itemID,Next_item_price,Next_Item_material,Next_Item_material_quantity,Resource_Path,WeaponName,WeaponIndex,PartName,PartIndex,Icon_path,EXP,Buy_cost,Sell_reward,Consignment_maxprice,QuestBringer,ItemEvent_ID,Description,Sub_Item,WeaponType,RandomBoxGroup_NO);
        array__.Add(__table);
      }
      ArrayToMap(array__);
      _array = array__;
    }
     public static int Next_Item_material_Length { get { return 3; } }
     public static int Next_Item_material_quantity_Length { get { return 3; } }
     public static int PartName_Length { get { return 1; } }
     public static int PartIndex_Length { get { return 1; } }
     public static int RandomBoxGroup_NO_Length { get { return 5; } }
  public Item (int Item_ID,string Name,int Item_grade,int Require_lv,int Enchant_lv,int PhysicalAttack,int PhysicalDefense,int MagicalAttack,int MagicalDefense,float Critical,int HP,int KnockBackResist,eDictionaryType DictionaryType,int ItemType,short Gear_Score,short InventoryType,bool UsageType,short Socket_quantity,int Removal_cost,short Belonging,short Sub_stats_quantity,int Stack,int DesignScroll_ID,int BindingSkill_ID,int BindingAttack_ID,int Manufacture_gold,int Manufacture_cash,int SummonCompanion_ID,int Next_itemID,int Next_item_price,int[] Next_Item_material,int[] Next_Item_material_quantity,string Resource_Path,string WeaponName,short WeaponIndex,string[] PartName,short[] PartIndex,string Icon_path,int EXP,int Buy_cost,int Sell_reward,int Consignment_maxprice,int QuestBringer,int ItemEvent_ID,string Description,int Sub_Item,int WeaponType,int[] RandomBoxGroup_NO) : base(Item_ID,Name,Item_grade,Require_lv,Enchant_lv,PhysicalAttack,PhysicalDefense,MagicalAttack,MagicalDefense,Critical,HP,KnockBackResist,DictionaryType,ItemType,Gear_Score,InventoryType,UsageType,Socket_quantity,Removal_cost,Belonging,Sub_stats_quantity,Stack,DesignScroll_ID,BindingSkill_ID,BindingAttack_ID,Manufacture_gold,Manufacture_cash,SummonCompanion_ID,Next_itemID,Next_item_price,Next_Item_material,Next_Item_material_quantity,Resource_Path,WeaponName,WeaponIndex,PartName,PartIndex,Icon_path,EXP,Buy_cost,Sell_reward,Consignment_maxprice,QuestBringer,ItemEvent_ID,Description,Sub_Item,WeaponType,RandomBoxGroup_NO){}
  }
  

  #if !ENCRYPT
  [System.Serializable]
  #endif
  public partial class ItemEffect : BaseClasses.ItemEffect
  {
    public static System.Collections.Generic.List<ItemEffect> _array = null;
    public static System.Collections.Generic.Dictionary<int,ItemEffect> _map = null;
    

    public static void ArrayToMap(System.Collections.Generic.List<ItemEffect> array__)
    {
      _map = new System.Collections.Generic.Dictionary<int,ItemEffect> (array__.Count,default(global::TBL.IntEqualityComparer));
      ItemEffect __table = null;
      for( int __i=0;__i<array__.Count;__i++)
      {
        __table = array__[__i];
        try{
          _map.Add(__table.Index, __table);
        }catch(System.Exception e)
        {
          throw new System.Exception(__table.Index.ToString() + " " + e.Message);
        }
      }
    }
    

    public static void WriteStream(System.IO.BinaryWriter __writer)
    {
      __writer.Write(_array.Count);
      for (var __i=0;__i<_array.Count;__i++)
      {
        var __table = _array[__i];
        __writer.Write(__table.Index);
        __writer.Write(__table.Item_ID);
        __writer.Write(__table.Effect_type);
        __writer.Write(__table.Effect_min);
        __writer.Write(__table.Effect_max);
        __writer.Write(__table.Time_type);
        __writer.Write(__table.Time_rate);
        __writer.Write(__table.Time);
        __writer.Write(__table.Duration);
        __writer.Write(__table.Description);
      }
    }
    #if !UNITY_5_3_OR_NEWER
    public static bool SetDataSet(System.Data.DataSet dts)
    {
      ItemEffect._map = new System.Collections.Generic.Dictionary<int,ItemEffect>();
      ItemEffect._array = new System.Collections.Generic.List<ItemEffect>();
      foreach (System.Data.DataRow row in dts.Tables["ItemEffect"].Rows)
      {
        ItemEffect table = new ItemEffect
        (
        System.Convert.ToInt32(System.Math.Round(double.Parse(row["Index"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Item_ID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Effect_type"].ToString())))
        ,System.Convert.ToSingle(row["Effect_min"].ToString())
        ,System.Convert.ToSingle(row["Effect_max"].ToString())
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Time_type"].ToString())))
        ,System.Convert.ToSingle(row["Time_rate"].ToString())
        ,System.Convert.ToSingle(row["Time"].ToString())
        ,System.Convert.ToSingle(row["Duration"].ToString())
        ,row["Description"].ToString()
        );
        ItemEffect._map.Add(table.Index, table);
        ItemEffect._array.Add(table);
      }
      return true;
    }
    #if !NO_EXCEL_LOADER
    public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
    {
      var i=0; var j=0;
      string[,] rows = null;
      int Index;
      int Item_ID;
      int Effect_type;
      float Effect_min;
      float Effect_max;
      int Time_type;
      float Time_rate;
      float Time;
      float Duration;
      string Description;
      try
      {
        rows = imp.GetSheet("ItemEffect", language);
        var array__ = new System.Collections.Generic.List<ItemEffect>(rows.GetLength(0) - 3);
        for (i = 3; i < rows.GetLength(0); i++)
        {
          j=0;
          if( rows[i,0].Length == 0) break;
          j = 0;
          if(string.IsNullOrEmpty(rows[i,0]))
          {
          Index = 0;}else {Index = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0])));
          }
          j = 1;
          if(string.IsNullOrEmpty(rows[i,1]))
          {
          Item_ID = 0;}else {Item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,1])));
          }
          j = 2;
          if(string.IsNullOrEmpty(rows[i,2]))
          {
          Effect_type = 0;}else {Effect_type = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,2])));
          }
          j = 3;
          if(string.IsNullOrEmpty(rows[i,3]))
          {
          Effect_min = 0;}else {Effect_min = System.Convert.ToSingle(rows[i,3]);
          }
          j = 4;
          if(string.IsNullOrEmpty(rows[i,4]))
          {
          Effect_max = 0;}else {Effect_max = System.Convert.ToSingle(rows[i,4]);
          }
          j = 5;
          if(string.IsNullOrEmpty(rows[i,5]))
          {
          Time_type = 0;}else {Time_type = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,5])));
          }
          j = 6;
          if(string.IsNullOrEmpty(rows[i,6]))
          {
          Time_rate = 0;}else {Time_rate = System.Convert.ToSingle(rows[i,6]);
          }
          j = 7;
          if(string.IsNullOrEmpty(rows[i,7]))
          {
          Time = 0;}else {Time = System.Convert.ToSingle(rows[i,7]);
          }
          j = 8;
          if(string.IsNullOrEmpty(rows[i,8]))
          {
          Duration = 0;}else {Duration = System.Convert.ToSingle(rows[i,8]);
          }
          j = 10;
          Description = rows[i,10];
          ItemEffect values = new ItemEffect(Index,Item_ID,Effect_type,Effect_min,Effect_max,Time_type,Time_rate,Time,Duration,Description);
          foreach (var preValues in array__)
          {
            if (preValues.Index == Index)
            {
              i=0;j=0;
              throw new System.Exception("ItemEffect.Index:" + preValues.Index.ToString() + ") Duplicated!!");
            }
          }
          array__.Add(values);
        }
        ArrayToMap(array__);
        _array = array__;
      }catch(System.Exception e)
      {
        if( rows == null ) throw;
        if (j >= rows.GetLength(1))
          throw new System.Exception("sheet(ItemEffect) invalid column count:" + j);
        throw new System.Exception(" convert failure : excel(ItemTable).sheet(ItemEffect) key:" + rows[i,0] + " Name:" + rows[0,j] + " " + rows[i,j] + " " + e.Message );
      }
    }
    #endif
    public static void GetDataTable(System.Data.DataSet dts)
    {
      System.Data.DataTable table = dts.Tables.Add("ItemEffect");
      table.Columns.Add("Index", typeof(int));
      table.Columns.Add("Item_ID", typeof(int));
      table.Columns.Add("Effect_type", typeof(int));
      table.Columns.Add("Effect_min", typeof(float));
      table.Columns.Add("Effect_max", typeof(float));
      table.Columns.Add("Time_type", typeof(int));
      table.Columns.Add("Time_rate", typeof(float));
      table.Columns.Add("Time", typeof(float));
      table.Columns.Add("Duration", typeof(float));
      table.Columns.Add("Description", typeof(string));
      foreach(var item in _array )
      {
        table.Rows.Add(
        item.Index
        ,item.Item_ID
        ,item.Effect_type
        ,item.Effect_min
        ,item.Effect_max
        ,item.Time_type
        ,item.Time_rate
        ,item.Time
        ,item.Duration
        ,item.Description
        );
      }
    }
    #endif
    public static void ReadStream(System.IO.BinaryReader __reader)
    {
      var array__ = new System.Collections.Generic.List<ItemEffect>();
      int __count = __reader.ReadInt32();
      for (int __i=0;__i<__count;__i++)
      {
        var Index = __reader.ReadInt32();
        var Item_ID = __reader.ReadInt32();
        var Effect_type = __reader.ReadInt32();
        var Effect_min = __reader.ReadSingle();
        var Effect_max = __reader.ReadSingle();
        var Time_type = __reader.ReadInt32();
        var Time_rate = __reader.ReadSingle();
        var Time = __reader.ReadSingle();
        var Duration = __reader.ReadSingle();
        var Description = __reader.ReadString();
        ItemEffect __table = new ItemEffect(Index,Item_ID,Effect_type,Effect_min,Effect_max,Time_type,Time_rate,Time,Duration,Description);
        array__.Add(__table);
      }
      ArrayToMap(array__);
      _array = array__;
    }
  public ItemEffect (int Index,int Item_ID,int Effect_type,float Effect_min,float Effect_max,int Time_type,float Time_rate,float Time,float Duration,string Description) : base(Index,Item_ID,Effect_type,Effect_min,Effect_max,Time_type,Time_rate,Time,Duration,Description){}
  }
  

  #if !ENCRYPT
  [System.Serializable]
  #endif
  public partial class ItemEnchant : BaseClasses.ItemEnchant
  {
    public static System.Collections.Generic.List<ItemEnchant> _array = null;
    public static System.Collections.Generic.Dictionary<int,ItemEnchant> _map = null;
    

    public static void ArrayToMap(System.Collections.Generic.List<ItemEnchant> array__)
    {
      _map = new System.Collections.Generic.Dictionary<int,ItemEnchant> (array__.Count,default(global::TBL.IntEqualityComparer));
      ItemEnchant __table = null;
      for( int __i=0;__i<array__.Count;__i++)
      {
        __table = array__[__i];
        try{
          _map.Add(__table.Index, __table);
        }catch(System.Exception e)
        {
          throw new System.Exception(__table.Index.ToString() + " " + e.Message);
        }
      }
    }
    

    public static void WriteStream(System.IO.BinaryWriter __writer)
    {
      __writer.Write(_array.Count);
      for (var __i=0;__i<_array.Count;__i++)
      {
        var __table = _array[__i];
        __writer.Write(__table.Index);
        __writer.Write(__table.Item_ID);
        __writer.Write(__table.Enchant_lv);
        __writer.Write(__table.Physical_attack);
        __writer.Write(__table.Physical_defense);
        __writer.Write(__table.Magic_attack);
        __writer.Write(__table.Magic_defense);
        __writer.Write(__table.Critical);
        __writer.Write(__table.HP);
        __writer.Write(__table.KnockBack_resist);
        TBL.Encoder.Write7BitEncodedInt(__writer,__table.Material_IDS.Length);
        for(var j__=0;j__<__table.Material_IDS.Length;++j__){__writer.Write(__table.Material_IDS[j__]);}
        TBL.Encoder.Write7BitEncodedInt(__writer,__table.Material_quantitys.Length);
        for(var j__=0;j__<__table.Material_quantitys.Length;++j__){__writer.Write(__table.Material_quantitys[j__]);}
        __writer.Write(__table.Require_gold);
        __writer.Write(__table.Require_cash);
      }
    }
    #if !UNITY_5_3_OR_NEWER
    public static bool SetDataSet(System.Data.DataSet dts)
    {
      ItemEnchant._map = new System.Collections.Generic.Dictionary<int,ItemEnchant>();
      ItemEnchant._array = new System.Collections.Generic.List<ItemEnchant>();
      foreach (System.Data.DataRow row in dts.Tables["ItemEnchant"].Rows)
      {
        ItemEnchant table = new ItemEnchant
        (
        System.Convert.ToInt32(System.Math.Round(double.Parse(row["Index"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Item_ID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Enchant_lv"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Physical_attack"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Physical_defense"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Magic_attack"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Magic_defense"].ToString())))
        ,System.Convert.ToSingle(row["Critical"].ToString())
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["HP"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["KnockBack_resist"].ToString())))
        ,new int[]
        {
          System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_IDS0"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_IDS1"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_IDS2"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_IDS3"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_IDS4"].ToString())))
        }
        ,new int[]
        {
          System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_quantitys0"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_quantitys1"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_quantitys2"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_quantitys3"].ToString())))
          ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_quantitys4"].ToString())))
        }
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Require_gold"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Require_cash"].ToString())))
        );
        ItemEnchant._map.Add(table.Index, table);
        ItemEnchant._array.Add(table);
      }
      return true;
    }
    #if !NO_EXCEL_LOADER
    public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
    {
      var i=0; var j=0;
      string[,] rows = null;
      int Index;
      int Item_ID;
      int Enchant_lv;
      int Physical_attack;
      int Physical_defense;
      int Magic_attack;
      int Magic_defense;
      float Critical;
      int HP;
      int KnockBack_resist;
      int[] Material_IDS;
      int[] Material_quantitys;
      int Require_gold;
      int Require_cash;
      try
      {
        rows = imp.GetSheet("ItemEnchant", language);
        var array__ = new System.Collections.Generic.List<ItemEnchant>(rows.GetLength(0) - 3);
        for (i = 3; i < rows.GetLength(0); i++)
        {
          j=0;
          if( rows[i,0].Length == 0) break;
          j = 0;
          if(string.IsNullOrEmpty(rows[i,0]))
          {
          Index = 0;}else {Index = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0])));
          }
          j = 1;
          if(string.IsNullOrEmpty(rows[i,1]))
          {
          Item_ID = 0;}else {Item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,1])));
          }
          j = 9;
          if(string.IsNullOrEmpty(rows[i,9]))
          {
          Enchant_lv = 0;}else {Enchant_lv = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,9])));
          }
          j = 10;
          if(string.IsNullOrEmpty(rows[i,10]))
          {
          Physical_attack = 0;}else {Physical_attack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,10])));
          }
          j = 11;
          if(string.IsNullOrEmpty(rows[i,11]))
          {
          Physical_defense = 0;}else {Physical_defense = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,11])));
          }
          j = 12;
          if(string.IsNullOrEmpty(rows[i,12]))
          {
          Magic_attack = 0;}else {Magic_attack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,12])));
          }
          j = 13;
          if(string.IsNullOrEmpty(rows[i,13]))
          {
          Magic_defense = 0;}else {Magic_defense = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,13])));
          }
          j = 14;
          if(string.IsNullOrEmpty(rows[i,14]))
          {
          Critical = 0;}else {Critical = System.Convert.ToSingle(rows[i,14]);
          }
          j = 15;
          if(string.IsNullOrEmpty(rows[i,15]))
          {
          HP = 0;}else {HP = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,15])));
          }
          j = 16;
          if(string.IsNullOrEmpty(rows[i,16]))
          {
          KnockBack_resist = 0;}else {KnockBack_resist = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,16])));
          }
          Material_IDS = new int[5];
          j = 17;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,17])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,17]))); Material_IDS[0] = outvalue;
          }
          Material_quantitys = new int[5];
          j = 18;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,18])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,18]))); Material_quantitys[0] = outvalue;
          }
          j = 19;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,19])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,19]))); Material_IDS[1] = outvalue;
          }
          j = 20;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,20])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,20]))); Material_quantitys[1] = outvalue;
          }
          j = 21;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,21])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,21]))); Material_IDS[2] = outvalue;
          }
          j = 22;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,22])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,22]))); Material_quantitys[2] = outvalue;
          }
          j = 23;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,23])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,23]))); Material_IDS[3] = outvalue;
          }
          j = 24;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,24])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,24]))); Material_quantitys[3] = outvalue;
          }
          j = 25;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,25])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,25]))); Material_IDS[4] = outvalue;
          }
          j = 26;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,26])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,26]))); Material_quantitys[4] = outvalue;
          }
          j = 27;
          if(string.IsNullOrEmpty(rows[i,27]))
          {
          Require_gold = 0;}else {Require_gold = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,27])));
          }
          j = 28;
          if(string.IsNullOrEmpty(rows[i,28]))
          {
          Require_cash = 0;}else {Require_cash = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,28])));
          }
          ItemEnchant values = new ItemEnchant(Index,Item_ID,Enchant_lv,Physical_attack,Physical_defense,Magic_attack,Magic_defense,Critical,HP,KnockBack_resist,Material_IDS,Material_quantitys,Require_gold,Require_cash);
          foreach (var preValues in array__)
          {
            if (preValues.Index == Index)
            {
              i=0;j=0;
              throw new System.Exception("ItemEnchant.Index:" + preValues.Index.ToString() + ") Duplicated!!");
            }
          }
          array__.Add(values);
        }
        ArrayToMap(array__);
        _array = array__;
      }catch(System.Exception e)
      {
        if( rows == null ) throw;
        if (j >= rows.GetLength(1))
          throw new System.Exception("sheet(ItemEnchant) invalid column count:" + j);
        throw new System.Exception(" convert failure : excel(ItemTable).sheet(ItemEnchant) key:" + rows[i,0] + " Name:" + rows[0,j] + " " + rows[i,j] + " " + e.Message );
      }
    }
    #endif
    public static void GetDataTable(System.Data.DataSet dts)
    {
      System.Data.DataTable table = dts.Tables.Add("ItemEnchant");
      table.Columns.Add("Index", typeof(int));
      table.Columns.Add("Item_ID", typeof(int));
      table.Columns.Add("Enchant_lv", typeof(int));
      table.Columns.Add("Physical_attack", typeof(int));
      table.Columns.Add("Physical_defense", typeof(int));
      table.Columns.Add("Magic_attack", typeof(int));
      table.Columns.Add("Magic_defense", typeof(int));
      table.Columns.Add("Critical", typeof(float));
      table.Columns.Add("HP", typeof(int));
      table.Columns.Add("KnockBack_resist", typeof(int));
      table.Columns.Add("Material_IDS0", typeof(int));
      table.Columns.Add("Material_IDS1", typeof(int));
      table.Columns.Add("Material_IDS2", typeof(int));
      table.Columns.Add("Material_IDS3", typeof(int));
      table.Columns.Add("Material_IDS4", typeof(int));
      table.Columns.Add("Material_quantitys0", typeof(int));
      table.Columns.Add("Material_quantitys1", typeof(int));
      table.Columns.Add("Material_quantitys2", typeof(int));
      table.Columns.Add("Material_quantitys3", typeof(int));
      table.Columns.Add("Material_quantitys4", typeof(int));
      table.Columns.Add("Require_gold", typeof(int));
      table.Columns.Add("Require_cash", typeof(int));
      foreach(var item in _array )
      {
        table.Rows.Add(
        item.Index
        ,item.Item_ID
        ,item.Enchant_lv
        ,item.Physical_attack
        ,item.Physical_defense
        ,item.Magic_attack
        ,item.Magic_defense
        ,item.Critical
        ,item.HP
        ,item.KnockBack_resist
        ,item.Material_IDS[0]
        ,item.Material_IDS[1]
        ,item.Material_IDS[2]
        ,item.Material_IDS[3]
        ,item.Material_IDS[4]
        ,item.Material_quantitys[0]
        ,item.Material_quantitys[1]
        ,item.Material_quantitys[2]
        ,item.Material_quantitys[3]
        ,item.Material_quantitys[4]
        ,item.Require_gold
        ,item.Require_cash
        );
      }
    }
    #endif
    public static void ReadStream(System.IO.BinaryReader __reader)
    {
      var array__ = new System.Collections.Generic.List<ItemEnchant>();
      int __count = __reader.ReadInt32();
      for (int __i=0;__i<__count;__i++)
      {
        var Index = __reader.ReadInt32();
        var Item_ID = __reader.ReadInt32();
        var Enchant_lv = __reader.ReadInt32();
        var Physical_attack = __reader.ReadInt32();
        var Physical_defense = __reader.ReadInt32();
        var Magic_attack = __reader.ReadInt32();
        var Magic_defense = __reader.ReadInt32();
        var Critical = __reader.ReadSingle();
        var HP = __reader.ReadInt32();
        var KnockBack_resist = __reader.ReadInt32();
        int[] Material_IDS = null;
        {
          var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
          Material_IDS = new int[arrayCount__];
          for(var __j=0;__j<arrayCount__;++__j)Material_IDS[__j] = __reader.ReadInt32();
        }
        int[] Material_quantitys = null;
        {
          var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
          Material_quantitys = new int[arrayCount__];
          for(var __j=0;__j<arrayCount__;++__j)Material_quantitys[__j] = __reader.ReadInt32();
        }
        var Require_gold = __reader.ReadInt32();
        var Require_cash = __reader.ReadInt32();
        ItemEnchant __table = new ItemEnchant(Index,Item_ID,Enchant_lv,Physical_attack,Physical_defense,Magic_attack,Magic_defense,Critical,HP,KnockBack_resist,Material_IDS,Material_quantitys,Require_gold,Require_cash);
        array__.Add(__table);
      }
      ArrayToMap(array__);
      _array = array__;
    }
     public static int Material_IDS_Length { get { return 5; } }
     public static int Material_quantitys_Length { get { return 5; } }
  public ItemEnchant (int Index,int Item_ID,int Enchant_lv,int Physical_attack,int Physical_defense,int Magic_attack,int Magic_defense,float Critical,int HP,int KnockBack_resist,int[] Material_IDS,int[] Material_quantitys,int Require_gold,int Require_cash) : base(Index,Item_ID,Enchant_lv,Physical_attack,Physical_defense,Magic_attack,Magic_defense,Critical,HP,KnockBack_resist,Material_IDS,Material_quantitys,Require_gold,Require_cash){}
  }
  

  #if !ENCRYPT
  [System.Serializable]
  #endif
  public partial class ItemManufacture : BaseClasses.ItemManufacture
  {
    public static System.Collections.Generic.List<ItemManufacture> _array = null;
    public static System.Collections.Generic.Dictionary<int,ItemManufacture> _map = null;
    

    public static void ArrayToMap(System.Collections.Generic.List<ItemManufacture> array__)
    {
      _map = new System.Collections.Generic.Dictionary<int,ItemManufacture> (array__.Count,default(global::TBL.IntEqualityComparer));
      ItemManufacture __table = null;
      for( int __i=0;__i<array__.Count;__i++)
      {
        __table = array__[__i];
        try{
          _map.Add(__table.Index, __table);
        }catch(System.Exception e)
        {
          throw new System.Exception(__table.Index.ToString() + " " + e.Message);
        }
      }
    }
    

    public static void WriteStream(System.IO.BinaryWriter __writer)
    {
      __writer.Write(_array.Count);
      for (var __i=0;__i<_array.Count;__i++)
      {
        var __table = _array[__i];
        __writer.Write(__table.Index);
        __writer.Write(__table.Subject_item_ID);
        __writer.Write(__table.Material_item_ID);
        __writer.Write(__table.Material_quantity);
      }
    }
    #if !UNITY_5_3_OR_NEWER
    public static bool SetDataSet(System.Data.DataSet dts)
    {
      ItemManufacture._map = new System.Collections.Generic.Dictionary<int,ItemManufacture>();
      ItemManufacture._array = new System.Collections.Generic.List<ItemManufacture>();
      foreach (System.Data.DataRow row in dts.Tables["ItemManufacture"].Rows)
      {
        ItemManufacture table = new ItemManufacture
        (
        System.Convert.ToInt32(System.Math.Round(double.Parse(row["Index"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Subject_item_ID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_item_ID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Material_quantity"].ToString())))
        );
        ItemManufacture._map.Add(table.Index, table);
        ItemManufacture._array.Add(table);
      }
      return true;
    }
    #if !NO_EXCEL_LOADER
    public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
    {
      var i=0; var j=0;
      string[,] rows = null;
      int Index;
      int Subject_item_ID;
      int Material_item_ID;
      int Material_quantity;
      try
      {
        rows = imp.GetSheet("ItemManufacture", language);
        var array__ = new System.Collections.Generic.List<ItemManufacture>(rows.GetLength(0) - 3);
        for (i = 3; i < rows.GetLength(0); i++)
        {
          j=0;
          if( rows[i,0].Length == 0) break;
          j = 0;
          if(string.IsNullOrEmpty(rows[i,0]))
          {
          Index = 0;}else {Index = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0])));
          }
          j = 1;
          if(string.IsNullOrEmpty(rows[i,1]))
          {
          Subject_item_ID = 0;}else {Subject_item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,1])));
          }
          j = 2;
          if(string.IsNullOrEmpty(rows[i,2]))
          {
          Material_item_ID = 0;}else {Material_item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,2])));
          }
          j = 4;
          if(string.IsNullOrEmpty(rows[i,4]))
          {
          Material_quantity = 0;}else {Material_quantity = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,4])));
          }
          ItemManufacture values = new ItemManufacture(Index,Subject_item_ID,Material_item_ID,Material_quantity);
          foreach (var preValues in array__)
          {
            if (preValues.Index == Index)
            {
              i=0;j=0;
              throw new System.Exception("ItemManufacture.Index:" + preValues.Index.ToString() + ") Duplicated!!");
            }
          }
          array__.Add(values);
        }
        ArrayToMap(array__);
        _array = array__;
      }catch(System.Exception e)
      {
        if( rows == null ) throw;
        if (j >= rows.GetLength(1))
          throw new System.Exception("sheet(ItemManufacture) invalid column count:" + j);
        throw new System.Exception(" convert failure : excel(ItemTable).sheet(ItemManufacture) key:" + rows[i,0] + " Name:" + rows[0,j] + " " + rows[i,j] + " " + e.Message );
      }
    }
    #endif
    public static void GetDataTable(System.Data.DataSet dts)
    {
      System.Data.DataTable table = dts.Tables.Add("ItemManufacture");
      table.Columns.Add("Index", typeof(int));
      table.Columns.Add("Subject_item_ID", typeof(int));
      table.Columns.Add("Material_item_ID", typeof(int));
      table.Columns.Add("Material_quantity", typeof(int));
      foreach(var item in _array )
      {
        table.Rows.Add(
        item.Index
        ,item.Subject_item_ID
        ,item.Material_item_ID
        ,item.Material_quantity
        );
      }
    }
    #endif
    public static void ReadStream(System.IO.BinaryReader __reader)
    {
      var array__ = new System.Collections.Generic.List<ItemManufacture>();
      int __count = __reader.ReadInt32();
      for (int __i=0;__i<__count;__i++)
      {
        var Index = __reader.ReadInt32();
        var Subject_item_ID = __reader.ReadInt32();
        var Material_item_ID = __reader.ReadInt32();
        var Material_quantity = __reader.ReadInt32();
        ItemManufacture __table = new ItemManufacture(Index,Subject_item_ID,Material_item_ID,Material_quantity);
        array__.Add(__table);
      }
      ArrayToMap(array__);
      _array = array__;
    }
  public ItemManufacture (int Index,int Subject_item_ID,int Material_item_ID,int Material_quantity) : base(Index,Subject_item_ID,Material_item_ID,Material_quantity){}
  }
  

  #if !ENCRYPT
  [System.Serializable]
  #endif
  public partial class RandomBoxGroup : BaseClasses.RandomBoxGroup
  {
    public static System.Collections.Generic.List<RandomBoxGroup> _array = null;
    public static System.Collections.Generic.Dictionary<int,RandomBoxGroup> _map = null;
    

    public static void ArrayToMap(System.Collections.Generic.List<RandomBoxGroup> array__)
    {
      _map = new System.Collections.Generic.Dictionary<int,RandomBoxGroup> (array__.Count,default(global::TBL.IntEqualityComparer));
      RandomBoxGroup __table = null;
      for( int __i=0;__i<array__.Count;__i++)
      {
        __table = array__[__i];
        try{
          _map.Add(__table.ID, __table);
        }catch(System.Exception e)
        {
          throw new System.Exception(__table.ID.ToString() + " " + e.Message);
        }
      }
    }
    

    public static void WriteStream(System.IO.BinaryWriter __writer)
    {
      __writer.Write(_array.Count);
      for (var __i=0;__i<_array.Count;__i++)
      {
        var __table = _array[__i];
        __writer.Write(__table.ID);
        __writer.Write(__table.RandomItemGroup_NO);
        __writer.Write(__table.ClassType);
        __writer.Write(__table.Item_ID);
        TBL.Encoder.Write7BitEncodedInt(__writer,__table.RatioAmount.Length);
        for(var j__=0;j__<__table.RatioAmount.Length;++j__){__writer.Write(__table.RatioAmount[j__]);}
        __writer.Write(__table.Item_Quantity);
      }
    }
    #if !UNITY_5_3_OR_NEWER
    public static bool SetDataSet(System.Data.DataSet dts)
    {
      RandomBoxGroup._map = new System.Collections.Generic.Dictionary<int,RandomBoxGroup>();
      RandomBoxGroup._array = new System.Collections.Generic.List<RandomBoxGroup>();
      foreach (System.Data.DataRow row in dts.Tables["RandomBoxGroup"].Rows)
      {
        RandomBoxGroup table = new RandomBoxGroup
        (
        System.Convert.ToInt32(System.Math.Round(double.Parse(row["ID"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["RandomItemGroup_NO"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["ClassType"].ToString())))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Item_ID"].ToString())))
        ,new int[]
        {
          System.Convert.ToInt32(System.Math.Round(double.Parse(row["RatioAmount0"].ToString())))
        }
        ,System.Convert.ToInt32(System.Math.Round(double.Parse(row["Item_Quantity"].ToString())))
        );
        RandomBoxGroup._map.Add(table.ID, table);
        RandomBoxGroup._array.Add(table);
      }
      return true;
    }
    #if !NO_EXCEL_LOADER
    public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
    {
      var i=0; var j=0;
      string[,] rows = null;
      int ID;
      int RandomItemGroup_NO;
      int ClassType;
      int Item_ID;
      int[] RatioAmount;
      int Item_Quantity;
      try
      {
        rows = imp.GetSheet("RandomBoxGroup", language);
        var array__ = new System.Collections.Generic.List<RandomBoxGroup>(rows.GetLength(0) - 3);
        for (i = 3; i < rows.GetLength(0); i++)
        {
          j=0;
          if( rows[i,0].Length == 0) break;
          j = 0;
          if(string.IsNullOrEmpty(rows[i,0]))
          {
          ID = 0;}else {ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0])));
          }
          j = 2;
          if(string.IsNullOrEmpty(rows[i,2]))
          {
          RandomItemGroup_NO = 0;}else {RandomItemGroup_NO = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,2])));
          }
          j = 3;
          if(string.IsNullOrEmpty(rows[i,3]))
          {
          ClassType = 0;}else {ClassType = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,3])));
          }
          j = 4;
          if(string.IsNullOrEmpty(rows[i,4]))
          {
          Item_ID = 0;}else {Item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,4])));
          }
          RatioAmount = new int[1];
          j = 5;
          {
            int outvalue = 0; if(!string.IsNullOrEmpty(rows[i,5])) 
            outvalue = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,5]))); RatioAmount[0] = outvalue;
          }
          j = 6;
          if(string.IsNullOrEmpty(rows[i,6]))
          {
          Item_Quantity = 0;}else {Item_Quantity = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,6])));
          }
          RandomBoxGroup values = new RandomBoxGroup(ID,RandomItemGroup_NO,ClassType,Item_ID,RatioAmount,Item_Quantity);
          foreach (var preValues in array__)
          {
            if (preValues.ID == ID)
            {
              i=0;j=0;
              throw new System.Exception("RandomBoxGroup.ID:" + preValues.ID.ToString() + ") Duplicated!!");
            }
          }
          array__.Add(values);
        }
        ArrayToMap(array__);
        _array = array__;
      }catch(System.Exception e)
      {
        if( rows == null ) throw;
        if (j >= rows.GetLength(1))
          throw new System.Exception("sheet(RandomBoxGroup) invalid column count:" + j);
        throw new System.Exception(" convert failure : excel(ItemTable).sheet(RandomBoxGroup) key:" + rows[i,0] + " Name:" + rows[0,j] + " " + rows[i,j] + " " + e.Message );
      }
    }
    #endif
    public static void GetDataTable(System.Data.DataSet dts)
    {
      System.Data.DataTable table = dts.Tables.Add("RandomBoxGroup");
      table.Columns.Add("ID", typeof(int));
      table.Columns.Add("RandomItemGroup_NO", typeof(int));
      table.Columns.Add("ClassType", typeof(int));
      table.Columns.Add("Item_ID", typeof(int));
      table.Columns.Add("RatioAmount0", typeof(int));
      table.Columns.Add("Item_Quantity", typeof(int));
      foreach(var item in _array )
      {
        table.Rows.Add(
        item.ID
        ,item.RandomItemGroup_NO
        ,item.ClassType
        ,item.Item_ID
        ,item.RatioAmount[0]
        ,item.Item_Quantity
        );
      }
    }
    #endif
    public static void ReadStream(System.IO.BinaryReader __reader)
    {
      var array__ = new System.Collections.Generic.List<RandomBoxGroup>();
      int __count = __reader.ReadInt32();
      for (int __i=0;__i<__count;__i++)
      {
        var ID = __reader.ReadInt32();
        var RandomItemGroup_NO = __reader.ReadInt32();
        var ClassType = __reader.ReadInt32();
        var Item_ID = __reader.ReadInt32();
        int[] RatioAmount = null;
        {
          var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
          RatioAmount = new int[arrayCount__];
          for(var __j=0;__j<arrayCount__;++__j)RatioAmount[__j] = __reader.ReadInt32();
        }
        var Item_Quantity = __reader.ReadInt32();
        RandomBoxGroup __table = new RandomBoxGroup(ID,RandomItemGroup_NO,ClassType,Item_ID,RatioAmount,Item_Quantity);
        array__.Add(__table);
      }
      ArrayToMap(array__);
      _array = array__;
    }
     public static int RatioAmount_Length { get { return 1; } }
  public RandomBoxGroup (int ID,int RandomItemGroup_NO,int ClassType,int Item_ID,int[] RatioAmount,int Item_Quantity) : base(ID,RandomItemGroup_NO,ClassType,Item_ID,RatioAmount,Item_Quantity){}
  }
}
