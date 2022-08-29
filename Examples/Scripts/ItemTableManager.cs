#pragma warning disable IDE0007, IDE0011, IDE0025, IDE1006, IDE0018
#if UNITY_EDITOR
[System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
public class ItemTableManager : UnityEngine.MonoBehaviour
{
  public TBL.ItemTable.Item[] Item = null;
  public TBL.ItemTable.ItemEffect[] ItemEffect = null;
  public TBL.ItemTable.ItemEnchant[] ItemEnchant = null;
  public TBL.ItemTable.ItemManufacture[] ItemManufacture = null;
  public TBL.ItemTable.RandomBoxGroup[] RandomBoxGroup = null;
}
[UnityEditor.CustomEditor(typeof(ItemTableManager))]
public class ItemTableEditor : UnityEditor.Editor
{
  public override void OnInspectorGUI()
  {
    ItemTableManager myScript = (ItemTableManager) target;
    if(UnityEngine.GUILayout.Button("Load"))
    {
      myScript.Item = TBL.ItemTable.Item.Array_;
      myScript.ItemEffect = TBL.ItemTable.ItemEffect.Array_;
      myScript.ItemEnchant = TBL.ItemTable.ItemEnchant.Array_;
      myScript.ItemManufacture = TBL.ItemTable.ItemManufacture.Array_;
      myScript.RandomBoxGroup = TBL.ItemTable.RandomBoxGroup.Array_;
    }
    DrawDefaultInspector();
  }
}
#endif


namespace TBL.ItemTable;
[System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
public class Loader : ILoader
{
  #if !UNITY_2018_2_OR_NEWER
  public System.Data.DataSet DataSet
  {
    get
    {
      var dts = new System.Data.DataSet(nameof(ItemTable));
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
    System.Action<string> excelAction = excelName =>
    {
      language = language.Trim();
      string directoryName = System.IO.Path.GetDirectoryName(path);
      var imp = new ClassUtil.ExcelImporter();
      imp.Open(path);
      switch (excelName)
      {
        case "Item":Item.ExcelLoad(imp,directoryName,language);
        break;
        case "ItemEffect":ItemEffect.ExcelLoad(imp,directoryName,language);
        break;
        case "ItemEnchant":ItemEnchant.ExcelLoad(imp,directoryName,language);
        break;
        case "ItemManufacture":ItemManufacture.ExcelLoad(imp,directoryName,language);
        break;
        case "RandomBoxGroup":RandomBoxGroup.ExcelLoad(imp,directoryName,language);
        break;
      }
      imp.Dispose();
    };
  System.Threading.Tasks.Parallel.ForEach(new string[]{"Item","ItemEffect","ItemEnchant","ItemManufacture","RandomBoxGroup"},excelAction);
  }
  #endif
  #endif
  /*
  public void GetMapAndArray(System.Collections.Immutable.ImmutableDictionary<string,object> container)
  {
    container.Remove( "ItemTable.Item.map_");
    container.Remove( "ItemTable.Item.array_");
    container.Add( "ItemTable.Item.map_", new System.Collections.Immutable.ImmutableDictionary<int,Item>(Item.map_,default(IntEqualityComparer)));
    container.Add( "ItemTable.Item.array_",Item.array_);
    container.Remove( "ItemTable.ItemEffect.map_");
    container.Remove( "ItemTable.ItemEffect.array_");
    container.Add( "ItemTable.ItemEffect.map_", new System.Collections.Immutable.ImmutableDictionary<int,ItemEffect>(ItemEffect.map_,default(IntEqualityComparer)));
    container.Add( "ItemTable.ItemEffect.array_",ItemEffect.array_);
    container.Remove( "ItemTable.ItemEnchant.map_");
    container.Remove( "ItemTable.ItemEnchant.array_");
    container.Add( "ItemTable.ItemEnchant.map_", new System.Collections.Immutable.ImmutableDictionary<int,ItemEnchant>(ItemEnchant.map_,default(IntEqualityComparer)));
    container.Add( "ItemTable.ItemEnchant.array_",ItemEnchant.array_);
    container.Remove( "ItemTable.ItemManufacture.map_");
    container.Remove( "ItemTable.ItemManufacture.array_");
    container.Add( "ItemTable.ItemManufacture.map_", new System.Collections.Immutable.ImmutableDictionary<int,ItemManufacture>(ItemManufacture.map_,default(IntEqualityComparer)));
    container.Add( "ItemTable.ItemManufacture.array_",ItemManufacture.array_);
    container.Remove( "ItemTable.RandomBoxGroup.map_");
    container.Remove( "ItemTable.RandomBoxGroup.array_");
    container.Add( "ItemTable.RandomBoxGroup.map_", new System.Collections.Immutable.ImmutableDictionary<int,RandomBoxGroup>(RandomBoxGroup.map_,default(IntEqualityComparer)));
    container.Add( "ItemTable.RandomBoxGroup.array_",RandomBoxGroup.array_);
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
  public string GetFileName() => "ItemTable";
  public void ReadStream(System.IO.Stream stream)
  {
    stream.Position = 0;
    var streamLength = (int)stream.Length;
    var hashLength = stream.ReadByte();
    var uncompressedSize = new byte[4];
    var compressedSize = new byte[4];
    var hashBytes = new byte[hashLength];
    stream.Read( hashBytes, 0, hashLength);
    var bytes = new byte[streamLength-hashLength-compressedSize.Length-uncompressedSize.Length-1];
    stream.Read( uncompressedSize, 0, uncompressedSize.Length);
    stream.Read( compressedSize, 0, compressedSize.Length);
    stream.Read( bytes, 0, streamLength-hashLength-compressedSize.Length-uncompressedSize.Length-1);
    {
      using var md5 = System.Security.Cryptography.MD5.Create();
      var dataBytes = md5.ComputeHash(bytes);
      if(!System.Linq.Enumerable.SequenceEqual(hashBytes, dataBytes))
      {throw new System.Exception("ItemTable verify failure...");}
    }
    {
      using var __ms = new System.IO.MemoryStream(bytes);
      using var decompressStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(__ms);
      var uncompressedSize__ = System.BitConverter.ToInt32(uncompressedSize,0);
      bytes = new byte[uncompressedSize__];
      decompressStream.Read(bytes, 0, uncompressedSize__);
    }
    {
      System.IO.MemoryStream __ms = null;
      try
      {
        __ms = new System.IO.MemoryStream(bytes);
        {
          using var reader = new System.IO.BinaryReader(__ms);
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
        __ms?.Dispose();
      }
    }
  }
  public byte[] GetHash(System.IO.Stream stream)
  {
    stream.Position = 0;
    var hashLength = stream.ReadByte();
    var hashBytes = new byte[hashLength];
    stream.Read( hashBytes, 0, hashLength);
    return hashBytes;
  }
}


#if !ENCRYPT
[System.Serializable]
#endif
public partial record Item
{
  private static Item[] array_;
  public static Item[] Array_
  {
    get
    {
      if (array_ != null) return array_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return array_;
    }
  }
  private static System.Collections.Immutable.ImmutableDictionary<int,Item> map_;
  public static System.Collections.Immutable.ImmutableDictionary<int,Item> Map_
  {
    get
    {
      if (map_ != null) return map_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return map_;
    }
  }
  public object[] DataRow_ => new object[]{
    Item_ID
    ,Name
    ,Item_grade
    ,Require_lv
    ,Enchant_lv
    ,PhysicalAttack
    ,PhysicalDefense
    ,MagicalAttack
    ,MagicalDefense
    ,Critical
    ,HP
    ,KnockBackResist
    ,DictionaryType
    ,ItemType
    ,Gear_Score
    ,InventoryType
    ,UsageType
    ,Socket_quantity
    ,Removal_cost
    ,Belonging
    ,Sub_stats_quantity
    ,Stack
    ,DesignScroll_ID
    ,BindingSkill_ID
    ,BindingAttack_ID
    ,Manufacture_gold
    ,Manufacture_cash
    ,SummonCompanion_ID
    ,Next_itemID
    ,Next_item_price
    ,Next_Item_material[0]
    ,Next_Item_material[1]
    ,Next_Item_material[2]
    ,Next_Item_material_quantity[0]
    ,Next_Item_material_quantity[1]
    ,Next_Item_material_quantity[2]
    ,Resource_Path
    ,WeaponName
    ,WeaponIndex
    ,PartName[0]
    ,PartIndex[0]
    ,Icon_path
    ,EXP
    ,Buy_cost
    ,Sell_reward
    ,Consignment_maxprice
    ,QuestBringer
    ,ItemEvent_ID
    ,Description
    ,Sub_Item
    ,WeaponType
    ,RandomBoxGroup_NO[0]
    ,RandomBoxGroup_NO[1]
    ,RandomBoxGroup_NO[2]
    ,RandomBoxGroup_NO[3]
    ,RandomBoxGroup_NO[4]
  };
  public static void ArrayToMap(Item[] array__)
  {
    var map__ = new System.Collections.Generic.Dictionary<int,Item> (array__.Length);
    var __i=0;
    try{
      for(__i=0;__i<array__.Length;__i++)
      {
        var __table = array__[__i];
        map__.Add(__table.Item_ID, __table);
      }
    }catch(System.Exception e)
    {
        throw new System.Exception($"Row:{__i} {e.Message}");
    }
    map_ = System.Collections.Immutable.ImmutableDictionary<int,Item>.Empty.AddRange(map__);
    map_ = map_.WithComparers(default(IntEqualityComparer));
  }
  

  public static void WriteStream(System.IO.BinaryWriter __writer)
  {
    __writer.Write(array_.Length);
    foreach (var __table in array_)
    {
      __writer.Write(__table.Item_ID);
      Encoder.Write(__writer,__table.Name);__writer.Write(__table.Item_grade);
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
      Encoder.Write7BitEncodedInt(__writer,__table.Next_Item_material.Length);
      foreach(var t__ in __table.Next_Item_material){__writer.Write(t__);}
      Encoder.Write7BitEncodedInt(__writer,__table.Next_Item_material_quantity.Length);
      foreach(var t__ in __table.Next_Item_material_quantity){__writer.Write(t__);}
      Encoder.Write(__writer,__table.Resource_Path);Encoder.Write(__writer,__table.WeaponName);__writer.Write(__table.WeaponIndex);
      Encoder.Write7BitEncodedInt(__writer,__table.PartName.Length);
      foreach(var t__ in __table.PartName){Encoder.Write(__writer,t__);}
      Encoder.Write7BitEncodedInt(__writer,__table.PartIndex.Length);
      foreach(var t__ in __table.PartIndex){__writer.Write(t__);}
      Encoder.Write(__writer,__table.Icon_path);__writer.Write(__table.EXP);
      __writer.Write(__table.Buy_cost);
      __writer.Write(__table.Sell_reward);
      __writer.Write(__table.Consignment_maxprice);
      __writer.Write(__table.QuestBringer);
      __writer.Write(__table.ItemEvent_ID);
      Encoder.Write(__writer,__table.Description);__writer.Write(__table.Sub_Item);
      __writer.Write(__table.WeaponType);
      Encoder.Write7BitEncodedInt(__writer,__table.RandomBoxGroup_NO.Length);
      foreach(var t__ in __table.RandomBoxGroup_NO){__writer.Write(t__);}
    }
  }
  #if !UNITY_2018_2_OR_NEWER
  public static bool SetDataSet(System.Data.DataSet dts)
  {
    if(dts?.Tables[nameof(Item)] is null) throw new System.Exception("dts.Tables['Item'] is null");
    var tables__ = dts.Tables[nameof(Item)];
    if(tables__ is null) throw new System.Exception("dts.Tables['Item'] is null");
    map_ = System.Collections.Immutable.ImmutableDictionary<int,Item>.Empty;
    array_ = new Item[tables__.Rows.Count];
    var row__ = 0;
    foreach (System.Data.DataRow row in tables__.Rows)
    {
      var table__ = new Item
      (
      System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Item_ID)].ToString() ?? string.Empty))))
      ,(row[nameof(Name)].ToString() ?? string.Empty)
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Item_grade)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Require_lv)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Enchant_lv)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(PhysicalAttack)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(PhysicalDefense)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(MagicalAttack)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(MagicalDefense)].ToString() ?? string.Empty))))
      ,System.Convert.ToSingle((row[nameof(Critical)].ToString() ?? string.Empty))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(HP)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(KnockBackResist)].ToString() ?? string.Empty))))
      ,(eDictionaryType) System.Enum.Parse(typeof(eDictionaryType),(row[nameof(DictionaryType)].ToString() ?? string.Empty))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(ItemType)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt16(System.Math.Round(double.Parse((row[nameof(Gear_Score)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt16(System.Math.Round(double.Parse((row[nameof(InventoryType)].ToString() ?? string.Empty))))
      ,((row[nameof(UsageType)].ToString() ?? string.Empty).Trim()=="1"||(row[nameof(UsageType)].ToString() ?? string.Empty).Trim().ToUpper()=="TRUE")
      ,System.Convert.ToInt16(System.Math.Round(double.Parse((row[nameof(Socket_quantity)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Removal_cost)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt16(System.Math.Round(double.Parse((row[nameof(Belonging)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt16(System.Math.Round(double.Parse((row[nameof(Sub_stats_quantity)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Stack)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(DesignScroll_ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(BindingSkill_ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(BindingAttack_ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Manufacture_gold)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Manufacture_cash)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(SummonCompanion_ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Next_itemID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Next_item_price)].ToString() ?? string.Empty))))
      ,new int[]
      {
        System.Convert.ToInt32(System.Math.Round(double.Parse((row["Next_Item_material0"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Next_Item_material1"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Next_Item_material2"].ToString() ?? string.Empty))))
      }
      ,new int[]
      {
        System.Convert.ToInt32(System.Math.Round(double.Parse((row["Next_Item_material_quantity0"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Next_Item_material_quantity1"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Next_Item_material_quantity2"].ToString() ?? string.Empty))))
      }
      ,(row[nameof(Resource_Path)].ToString() ?? string.Empty)
      ,(row[nameof(WeaponName)].ToString() ?? string.Empty)
      ,System.Convert.ToInt16(System.Math.Round(double.Parse((row[nameof(WeaponIndex)].ToString() ?? string.Empty))))
      ,new string[]
      {
        (row["PartName0"].ToString() ?? string.Empty)
      }
      ,new short[]
      {
        System.Convert.ToInt16(System.Math.Round(double.Parse((row["PartIndex0"].ToString() ?? string.Empty))))
      }
      ,(row[nameof(Icon_path)].ToString() ?? string.Empty)
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(EXP)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Buy_cost)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Sell_reward)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Consignment_maxprice)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(QuestBringer)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(ItemEvent_ID)].ToString() ?? string.Empty))))
      ,(row[nameof(Description)].ToString() ?? string.Empty)
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Sub_Item)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(WeaponType)].ToString() ?? string.Empty))))
      ,new int[]
      {
        System.Convert.ToInt32(System.Math.Round(double.Parse((row["RandomBoxGroup_NO0"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["RandomBoxGroup_NO1"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["RandomBoxGroup_NO2"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["RandomBoxGroup_NO3"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["RandomBoxGroup_NO4"].ToString() ?? string.Empty))))
      }
      );
      map_ = map_.Add(table__.Item_ID, table__);
      array_[row__++] = table__;
    }
    return true;
  }
  #if !NO_EXCEL_LOADER
  public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
  {
    var i=0; var j=0;
    TableGenerateCmd.StringWithDesc[,] rows = null;
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
      var list__ = new System.Collections.Generic.List<Item>(rows.GetLength(0) - 3);
      for (i = 3; i < rows.GetLength(0); i++)
      {
        j=0;
        if(rows[i,0].Text.Length == 0) break;
        j = 0;
        if(string.IsNullOrEmpty(rows[i,0].Text))
        {
        Item_ID = 0;}else {Item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0].Text)));
        }
        j = 4;
        Name = rows[i,4].Text;
        j = 5;
        if(string.IsNullOrEmpty(rows[i,5].Text))
        {
        Item_grade = 0;}else {Item_grade = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,5].Text)));
        }
        j = 6;
        if(string.IsNullOrEmpty(rows[i,6].Text))
        {
        Require_lv = 0;}else {Require_lv = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,6].Text)));
        }
        j = 7;
        if(string.IsNullOrEmpty(rows[i,7].Text))
        {
        Enchant_lv = 0;}else {Enchant_lv = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,7].Text)));
        }
        j = 8;
        if(string.IsNullOrEmpty(rows[i,8].Text))
        {
        PhysicalAttack = 0;}else {PhysicalAttack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,8].Text)));
        }
        j = 9;
        if(string.IsNullOrEmpty(rows[i,9].Text))
        {
        PhysicalDefense = 0;}else {PhysicalDefense = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,9].Text)));
        }
        j = 10;
        if(string.IsNullOrEmpty(rows[i,10].Text))
        {
        MagicalAttack = 0;}else {MagicalAttack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,10].Text)));
        }
        j = 11;
        if(string.IsNullOrEmpty(rows[i,11].Text))
        {
        MagicalDefense = 0;}else {MagicalDefense = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,11].Text)));
        }
        j = 12;
        if(string.IsNullOrEmpty(rows[i,12].Text))
        {
        Critical = 0;}else {Critical = System.Convert.ToSingle(rows[i,12].Text);
        }
        j = 13;
        if(string.IsNullOrEmpty(rows[i,13].Text))
        {
        HP = 0;}else {HP = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,13].Text)));
        }
        j = 14;
        if(string.IsNullOrEmpty(rows[i,14].Text))
        {
        KnockBackResist = 0;}else {KnockBackResist = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,14].Text)));
        }
        j = 16;
        if(string.IsNullOrEmpty(rows[i,16].Text))
        {
        DictionaryType = eDictionaryType.Consume;}else {DictionaryType = (eDictionaryType) System.Enum.Parse(typeof(eDictionaryType),rows[i,16].Text);
        }
        j = 17;
        if(string.IsNullOrEmpty(rows[i,17].Text))
        {
        ItemType = 0;}else {ItemType = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,17].Text)));
        }
        j = 18;
        if(string.IsNullOrEmpty(rows[i,18].Text))
        {
        Gear_Score = 0;}else {Gear_Score = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,18].Text)));
        }
        j = 19;
        if(string.IsNullOrEmpty(rows[i,19].Text))
        {
        InventoryType = 0;}else {InventoryType = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,19].Text)));
        }
        j = 20;
        if(string.IsNullOrEmpty(rows[i,20].Text))
        {
        UsageType = false;}else {UsageType = (rows[i,20].Text.Trim()=="1"||rows[i,20].Text.Trim().ToUpper()=="TRUE");
        }
        j = 21;
        if(string.IsNullOrEmpty(rows[i,21].Text))
        {
        Socket_quantity = 0;}else {Socket_quantity = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,21].Text)));
        }
        j = 22;
        if(string.IsNullOrEmpty(rows[i,22].Text))
        {
        Removal_cost = 0;}else {Removal_cost = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,22].Text)));
        }
        j = 23;
        if(string.IsNullOrEmpty(rows[i,23].Text))
        {
        Belonging = 0;}else {Belonging = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,23].Text)));
        }
        j = 24;
        if(string.IsNullOrEmpty(rows[i,24].Text))
        {
        Sub_stats_quantity = 0;}else {Sub_stats_quantity = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,24].Text)));
        }
        j = 25;
        if(string.IsNullOrEmpty(rows[i,25].Text))
        {
        Stack = 0;}else {Stack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,25].Text)));
        }
        j = 26;
        if(string.IsNullOrEmpty(rows[i,26].Text))
        {
        DesignScroll_ID = 0;}else {DesignScroll_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,26].Text)));
        }
        j = 27;
        if(string.IsNullOrEmpty(rows[i,27].Text))
        {
        BindingSkill_ID = 0;}else {BindingSkill_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,27].Text)));
        }
        j = 28;
        if(string.IsNullOrEmpty(rows[i,28].Text))
        {
        BindingAttack_ID = 0;}else {BindingAttack_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,28].Text)));
        }
        j = 29;
        if(string.IsNullOrEmpty(rows[i,29].Text))
        {
        Manufacture_gold = 0;}else {Manufacture_gold = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,29].Text)));
        }
        j = 30;
        if(string.IsNullOrEmpty(rows[i,30].Text))
        {
        Manufacture_cash = 0;}else {Manufacture_cash = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,30].Text)));
        }
        j = 31;
        if(string.IsNullOrEmpty(rows[i,31].Text))
        {
        SummonCompanion_ID = 0;}else {SummonCompanion_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,31].Text)));
        }
        j = 32;
        if(string.IsNullOrEmpty(rows[i,32].Text))
        {
        Next_itemID = 0;}else {Next_itemID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,32].Text)));
        }
        j = 33;
        if(string.IsNullOrEmpty(rows[i,33].Text))
        {
        Next_item_price = 0;}else {Next_item_price = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,33].Text)));
        }
        var Next_Item_material_list__ = new System.Collections.Generic.List<int>();
        bool not_empty_Next_Item_material__ = false;
        j = 34;
        if(!string.IsNullOrEmpty(rows[i,34].Text))
        {
          Next_Item_material_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,34].Text))));
        }
        else
        {
          not_empty_Next_Item_material__ = true;
        }
        j = 35;
        if(!string.IsNullOrEmpty(rows[i,35].Text))
        {
          if(not_empty_Next_Item_material__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Next_Item_material_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,35].Text))));
        }
        else
        {
          not_empty_Next_Item_material__ = true;
        }
        j = 36;
        if(!string.IsNullOrEmpty(rows[i,36].Text))
        {
          if(not_empty_Next_Item_material__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Next_Item_material_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,36].Text))));
        }
        else
        {
          not_empty_Next_Item_material__ = true;
        }
        Next_Item_material = Next_Item_material_list__.ToArray();
        var Next_Item_material_quantity_list__ = new System.Collections.Generic.List<int>();
        bool not_empty_Next_Item_material_quantity__ = false;
        j = 37;
        if(!string.IsNullOrEmpty(rows[i,37].Text))
        {
          Next_Item_material_quantity_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,37].Text))));
        }
        else
        {
          not_empty_Next_Item_material_quantity__ = true;
        }
        j = 38;
        if(!string.IsNullOrEmpty(rows[i,38].Text))
        {
          if(not_empty_Next_Item_material_quantity__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Next_Item_material_quantity_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,38].Text))));
        }
        else
        {
          not_empty_Next_Item_material_quantity__ = true;
        }
        j = 39;
        if(!string.IsNullOrEmpty(rows[i,39].Text))
        {
          if(not_empty_Next_Item_material_quantity__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Next_Item_material_quantity_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,39].Text))));
        }
        else
        {
          not_empty_Next_Item_material_quantity__ = true;
        }
        Next_Item_material_quantity = Next_Item_material_quantity_list__.ToArray();
        j = 40;
        Resource_Path = rows[i,40].Text;
        j = 41;
        WeaponName = rows[i,41].Text;
        j = 42;
        if(string.IsNullOrEmpty(rows[i,42].Text))
        {
        WeaponIndex = 0;}else {WeaponIndex = System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,42].Text)));
        }
        var PartName_list__ = new System.Collections.Generic.List<string>();
        bool not_empty_PartName__ = false;
        j = 43;
        if(!string.IsNullOrEmpty(rows[i,43].Text))
        {
          PartName_list__.Add(rows[i,43].Text);
        }
        else
        {
          not_empty_PartName__ = true;
        }
        PartName = PartName_list__.ToArray();
        var PartIndex_list__ = new System.Collections.Generic.List<short>();
        bool not_empty_PartIndex__ = false;
        j = 44;
        if(!string.IsNullOrEmpty(rows[i,44].Text))
        {
          PartIndex_list__.Add(System.Convert.ToInt16(System.Math.Round(double.Parse(rows[i,44].Text))));
        }
        else
        {
          not_empty_PartIndex__ = true;
        }
        PartIndex = PartIndex_list__.ToArray();
        j = 45;
        Icon_path = rows[i,45].Text;
        j = 46;
        if(string.IsNullOrEmpty(rows[i,46].Text))
        {
        EXP = 0;}else {EXP = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,46].Text)));
        }
        j = 47;
        if(string.IsNullOrEmpty(rows[i,47].Text))
        {
        Buy_cost = 0;}else {Buy_cost = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,47].Text)));
        }
        j = 48;
        if(string.IsNullOrEmpty(rows[i,48].Text))
        {
        Sell_reward = 0;}else {Sell_reward = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,48].Text)));
        }
        j = 49;
        if(string.IsNullOrEmpty(rows[i,49].Text))
        {
        Consignment_maxprice = 0;}else {Consignment_maxprice = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,49].Text)));
        }
        j = 50;
        if(string.IsNullOrEmpty(rows[i,50].Text))
        {
        QuestBringer = 0;}else {QuestBringer = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,50].Text)));
        }
        j = 51;
        if(string.IsNullOrEmpty(rows[i,51].Text))
        {
        ItemEvent_ID = 0;}else {ItemEvent_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,51].Text)));
        }
        j = 52;
        Description = rows[i,52].Text;
        j = 53;
        if(string.IsNullOrEmpty(rows[i,53].Text))
        {
        Sub_Item = 0;}else {Sub_Item = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,53].Text)));
        }
        j = 54;
        if(string.IsNullOrEmpty(rows[i,54].Text))
        {
        WeaponType = 0;}else {WeaponType = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,54].Text)));
        }
        var RandomBoxGroup_NO_list__ = new System.Collections.Generic.List<int>();
        bool not_empty_RandomBoxGroup_NO__ = false;
        j = 55;
        if(!string.IsNullOrEmpty(rows[i,55].Text))
        {
          RandomBoxGroup_NO_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,55].Text))));
        }
        else
        {
          not_empty_RandomBoxGroup_NO__ = true;
        }
        j = 56;
        if(!string.IsNullOrEmpty(rows[i,56].Text))
        {
          if(not_empty_RandomBoxGroup_NO__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          RandomBoxGroup_NO_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,56].Text))));
        }
        else
        {
          not_empty_RandomBoxGroup_NO__ = true;
        }
        j = 57;
        if(!string.IsNullOrEmpty(rows[i,57].Text))
        {
          if(not_empty_RandomBoxGroup_NO__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          RandomBoxGroup_NO_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,57].Text))));
        }
        else
        {
          not_empty_RandomBoxGroup_NO__ = true;
        }
        j = 58;
        if(!string.IsNullOrEmpty(rows[i,58].Text))
        {
          if(not_empty_RandomBoxGroup_NO__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          RandomBoxGroup_NO_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,58].Text))));
        }
        else
        {
          not_empty_RandomBoxGroup_NO__ = true;
        }
        j = 59;
        if(!string.IsNullOrEmpty(rows[i,59].Text))
        {
          if(not_empty_RandomBoxGroup_NO__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          RandomBoxGroup_NO_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,59].Text))));
        }
        else
        {
          not_empty_RandomBoxGroup_NO__ = true;
        }
        RandomBoxGroup_NO = RandomBoxGroup_NO_list__.ToArray();
        if( Next_Item_material.Length != Next_Item_material_quantity.Length)
        {
            throw new System.Exception(string.Format("mismatch group:{0}","material"));
        }
        Item values = new Item(Item_ID,Name,Item_grade,Require_lv,Enchant_lv,PhysicalAttack,PhysicalDefense,MagicalAttack,MagicalDefense,Critical,HP,KnockBackResist,DictionaryType,ItemType,Gear_Score,InventoryType,UsageType,Socket_quantity,Removal_cost,Belonging,Sub_stats_quantity,Stack,DesignScroll_ID,BindingSkill_ID,BindingAttack_ID,Manufacture_gold,Manufacture_cash,SummonCompanion_ID,Next_itemID,Next_item_price,Next_Item_material,Next_Item_material_quantity,Resource_Path,WeaponName,WeaponIndex,PartName,PartIndex,Icon_path,EXP,Buy_cost,Sell_reward,Consignment_maxprice,QuestBringer,ItemEvent_ID,Description,Sub_Item,WeaponType,RandomBoxGroup_NO);
        foreach (var preValues in list__)
        {
          if (preValues.Item_ID.Equals(Item_ID))
          {
            throw new System.Exception("row:" + i + " Item.Item_ID:" + preValues.Item_ID.ToString() + ") Duplicated!!");
          }
        }
        list__.Add(values);
      }
      array_ = list__.ToArray();
      ArrayToMap(array_);
    }catch(System.Exception e)
    {
      if( rows == null ) throw;
      if (j >= rows.GetLength(1))
        throw new System.Exception("sheet(Item) invalid column count:" + j);
      throw new System.Exception(" convert failure : excel(ItemTable).sheet(Item) key:" + rows[i,0].Text + " Name:" + rows[0,j].Text + " " + rows[i,j].Text + " " + e.Message );
    }
  }
  #endif
  public static void GetDataTable(System.Data.DataSet dts)
  {
    var table = dts.Tables.Add(nameof(Item));
    table.Columns.Add(nameof(Item_ID), typeof(int));
    table.Columns.Add(nameof(Name), typeof(string));
    table.Columns.Add(nameof(Item_grade), typeof(int));
    table.Columns.Add(nameof(Require_lv), typeof(int));
    table.Columns.Add(nameof(Enchant_lv), typeof(int));
    table.Columns.Add(nameof(PhysicalAttack), typeof(int));
    table.Columns.Add(nameof(PhysicalDefense), typeof(int));
    table.Columns.Add(nameof(MagicalAttack), typeof(int));
    table.Columns.Add(nameof(MagicalDefense), typeof(int));
    table.Columns.Add(nameof(Critical), typeof(float));
    table.Columns.Add(nameof(HP), typeof(int));
    table.Columns.Add(nameof(KnockBackResist), typeof(int));
    table.Columns.Add(nameof(DictionaryType), typeof(eDictionaryType));
    table.Columns.Add(nameof(ItemType), typeof(int));
    table.Columns.Add(nameof(Gear_Score), typeof(short));
    table.Columns.Add(nameof(InventoryType), typeof(short));
    table.Columns.Add(nameof(UsageType), typeof(bool));
    table.Columns.Add(nameof(Socket_quantity), typeof(short));
    table.Columns.Add(nameof(Removal_cost), typeof(int));
    table.Columns.Add(nameof(Belonging), typeof(short));
    table.Columns.Add(nameof(Sub_stats_quantity), typeof(short));
    table.Columns.Add(nameof(Stack), typeof(int));
    table.Columns.Add(nameof(DesignScroll_ID), typeof(int));
    table.Columns.Add(nameof(BindingSkill_ID), typeof(int));
    table.Columns.Add(nameof(BindingAttack_ID), typeof(int));
    table.Columns.Add(nameof(Manufacture_gold), typeof(int));
    table.Columns.Add(nameof(Manufacture_cash), typeof(int));
    table.Columns.Add(nameof(SummonCompanion_ID), typeof(int));
    table.Columns.Add(nameof(Next_itemID), typeof(int));
    table.Columns.Add(nameof(Next_item_price), typeof(int));
    table.Columns.Add("Next_Item_material0", typeof(int));
    table.Columns.Add("Next_Item_material1", typeof(int));
    table.Columns.Add("Next_Item_material2", typeof(int));
    table.Columns.Add("Next_Item_material_quantity0", typeof(int));
    table.Columns.Add("Next_Item_material_quantity1", typeof(int));
    table.Columns.Add("Next_Item_material_quantity2", typeof(int));
    table.Columns.Add(nameof(Resource_Path), typeof(string));
    table.Columns.Add(nameof(WeaponName), typeof(string));
    table.Columns.Add(nameof(WeaponIndex), typeof(short));
    table.Columns.Add("PartName0", typeof(string));
    table.Columns.Add("PartIndex0", typeof(short));
    table.Columns.Add(nameof(Icon_path), typeof(string));
    table.Columns.Add(nameof(EXP), typeof(int));
    table.Columns.Add(nameof(Buy_cost), typeof(int));
    table.Columns.Add(nameof(Sell_reward), typeof(int));
    table.Columns.Add(nameof(Consignment_maxprice), typeof(int));
    table.Columns.Add(nameof(QuestBringer), typeof(int));
    table.Columns.Add(nameof(ItemEvent_ID), typeof(int));
    table.Columns.Add(nameof(Description), typeof(string));
    table.Columns.Add(nameof(Sub_Item), typeof(int));
    table.Columns.Add(nameof(WeaponType), typeof(int));
    table.Columns.Add("RandomBoxGroup_NO0", typeof(int));
    table.Columns.Add("RandomBoxGroup_NO1", typeof(int));
    table.Columns.Add("RandomBoxGroup_NO2", typeof(int));
    table.Columns.Add("RandomBoxGroup_NO3", typeof(int));
    table.Columns.Add("RandomBoxGroup_NO4", typeof(int));
    foreach(var item__ in array_ )
    {
      table.Rows.Add(item__.DataRow_);
    }
  }
  #endif
  public static void ReadStream(System.IO.BinaryReader __reader)
  {
    var count__ = __reader.ReadInt32();
    var array__ = new Item[count__];
    for (var __i=0;__i<array__.Length;__i++)
    {
      var Item_ID = __reader.ReadInt32();
      var Name = Encoder.ReadString(ref __reader);
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
      int[] Next_Item_material;
      {
        var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
        Next_Item_material = arrayCount__ > 0?new int[arrayCount__]:System.Array.Empty<int>();
        for(var __j=0;__j<arrayCount__;++__j)Next_Item_material[__j] = __reader.ReadInt32();
      }
      int[] Next_Item_material_quantity;
      {
        var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
        Next_Item_material_quantity = arrayCount__ > 0?new int[arrayCount__]:System.Array.Empty<int>();
        for(var __j=0;__j<arrayCount__;++__j)Next_Item_material_quantity[__j] = __reader.ReadInt32();
      }
      var Resource_Path = Encoder.ReadString(ref __reader);
      var WeaponName = Encoder.ReadString(ref __reader);
      var WeaponIndex = __reader.ReadInt16();
      string[] PartName;
      {
        var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
        PartName = arrayCount__ > 0?new string[arrayCount__]:System.Array.Empty<string>();
        for(var __j=0;__j<arrayCount__;++__j)PartName[__j] = Encoder.ReadString(ref __reader);
      }
      short[] PartIndex;
      {
        var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
        PartIndex = arrayCount__ > 0?new short[arrayCount__]:System.Array.Empty<short>();
        for(var __j=0;__j<arrayCount__;++__j)PartIndex[__j] = __reader.ReadInt16();
      }
      var Icon_path = Encoder.ReadString(ref __reader);
      var EXP = __reader.ReadInt32();
      var Buy_cost = __reader.ReadInt32();
      var Sell_reward = __reader.ReadInt32();
      var Consignment_maxprice = __reader.ReadInt32();
      var QuestBringer = __reader.ReadInt32();
      var ItemEvent_ID = __reader.ReadInt32();
      var Description = Encoder.ReadString(ref __reader);
      var Sub_Item = __reader.ReadInt32();
      var WeaponType = __reader.ReadInt32();
      int[] RandomBoxGroup_NO;
      {
        var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
        RandomBoxGroup_NO = arrayCount__ > 0?new int[arrayCount__]:System.Array.Empty<int>();
        for(var __j=0;__j<arrayCount__;++__j)RandomBoxGroup_NO[__j] = __reader.ReadInt32();
      }
      var __table = new Item(Item_ID,Name,Item_grade,Require_lv,Enchant_lv,PhysicalAttack,PhysicalDefense,MagicalAttack,MagicalDefense,Critical,HP,KnockBackResist,DictionaryType,ItemType,Gear_Score,InventoryType,UsageType,Socket_quantity,Removal_cost,Belonging,Sub_stats_quantity,Stack,DesignScroll_ID,BindingSkill_ID,BindingAttack_ID,Manufacture_gold,Manufacture_cash,SummonCompanion_ID,Next_itemID,Next_item_price,Next_Item_material,Next_Item_material_quantity,Resource_Path,WeaponName,WeaponIndex,PartName,PartIndex,Icon_path,EXP,Buy_cost,Sell_reward,Consignment_maxprice,QuestBringer,ItemEvent_ID,Description,Sub_Item,WeaponType,RandomBoxGroup_NO);
      array__[__i] = __table;
    }
    array_ = array__;
    ArrayToMap(array__);
  }
  public static int Next_Item_material_Length => 3;
  public static int Next_Item_material_quantity_Length => 3;
  public static int PartName_Length => 1;
  public static int PartIndex_Length => 1;
  public static int RandomBoxGroup_NO_Length => 5;
}


#if !ENCRYPT
[System.Serializable]
#endif
public partial record ItemEffect
{
  private static ItemEffect[] array_;
  public static ItemEffect[] Array_
  {
    get
    {
      if (array_ != null) return array_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return array_;
    }
  }
  private static System.Collections.Immutable.ImmutableDictionary<int,ItemEffect> map_;
  public static System.Collections.Immutable.ImmutableDictionary<int,ItemEffect> Map_
  {
    get
    {
      if (map_ != null) return map_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return map_;
    }
  }
  public object[] DataRow_ => new object[]{
    Index
    ,Item_ID
    ,Effect_type
    ,Effect_min
    ,Effect_max
    ,Time_type
    ,Time_rate
    ,Time
    ,Duration
    ,Description
  };
  public static void ArrayToMap(ItemEffect[] array__)
  {
    var map__ = new System.Collections.Generic.Dictionary<int,ItemEffect> (array__.Length);
    var __i=0;
    try{
      for(__i=0;__i<array__.Length;__i++)
      {
        var __table = array__[__i];
        map__.Add(__table.Index, __table);
      }
    }catch(System.Exception e)
    {
        throw new System.Exception($"Row:{__i} {e.Message}");
    }
    map_ = System.Collections.Immutable.ImmutableDictionary<int,ItemEffect>.Empty.AddRange(map__);
    map_ = map_.WithComparers(default(IntEqualityComparer));
  }
  

  public static void WriteStream(System.IO.BinaryWriter __writer)
  {
    __writer.Write(array_.Length);
    foreach (var __table in array_)
    {
      __writer.Write(__table.Index);
      __writer.Write(__table.Item_ID);
      __writer.Write(__table.Effect_type);
      __writer.Write(__table.Effect_min);
      __writer.Write(__table.Effect_max);
      __writer.Write(__table.Time_type);
      __writer.Write(__table.Time_rate);
      __writer.Write(__table.Time);
      __writer.Write(__table.Duration);
      Encoder.Write(__writer,__table.Description);}
  }
  #if !UNITY_2018_2_OR_NEWER
  public static bool SetDataSet(System.Data.DataSet dts)
  {
    if(dts?.Tables[nameof(ItemEffect)] is null) throw new System.Exception("dts.Tables['ItemEffect'] is null");
    var tables__ = dts.Tables[nameof(ItemEffect)];
    if(tables__ is null) throw new System.Exception("dts.Tables['ItemEffect'] is null");
    map_ = System.Collections.Immutable.ImmutableDictionary<int,ItemEffect>.Empty;
    array_ = new ItemEffect[tables__.Rows.Count];
    var row__ = 0;
    foreach (System.Data.DataRow row in tables__.Rows)
    {
      var table__ = new ItemEffect
      (
      System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Index)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Item_ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Effect_type)].ToString() ?? string.Empty))))
      ,System.Convert.ToSingle((row[nameof(Effect_min)].ToString() ?? string.Empty))
      ,System.Convert.ToSingle((row[nameof(Effect_max)].ToString() ?? string.Empty))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Time_type)].ToString() ?? string.Empty))))
      ,System.Convert.ToSingle((row[nameof(Time_rate)].ToString() ?? string.Empty))
      ,System.Convert.ToSingle((row[nameof(Time)].ToString() ?? string.Empty))
      ,System.Convert.ToSingle((row[nameof(Duration)].ToString() ?? string.Empty))
      ,(row[nameof(Description)].ToString() ?? string.Empty)
      );
      map_ = map_.Add(table__.Index, table__);
      array_[row__++] = table__;
    }
    return true;
  }
  #if !NO_EXCEL_LOADER
  public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
  {
    var i=0; var j=0;
    TableGenerateCmd.StringWithDesc[,] rows = null;
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
      var list__ = new System.Collections.Generic.List<ItemEffect>(rows.GetLength(0) - 3);
      for (i = 3; i < rows.GetLength(0); i++)
      {
        j=0;
        if(rows[i,0].Text.Length == 0) break;
        j = 0;
        if(string.IsNullOrEmpty(rows[i,0].Text))
        {
        Index = 0;}else {Index = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0].Text)));
        }
        j = 1;
        if(string.IsNullOrEmpty(rows[i,1].Text))
        {
        Item_ID = 0;}else {Item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,1].Text)));
        }
        j = 2;
        if(string.IsNullOrEmpty(rows[i,2].Text))
        {
        Effect_type = 0;}else {Effect_type = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,2].Text)));
        }
        j = 3;
        if(string.IsNullOrEmpty(rows[i,3].Text))
        {
        Effect_min = 0;}else {Effect_min = System.Convert.ToSingle(rows[i,3].Text);
        }
        j = 4;
        if(string.IsNullOrEmpty(rows[i,4].Text))
        {
        Effect_max = 0;}else {Effect_max = System.Convert.ToSingle(rows[i,4].Text);
        }
        j = 5;
        if(string.IsNullOrEmpty(rows[i,5].Text))
        {
        Time_type = 0;}else {Time_type = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,5].Text)));
        }
        j = 6;
        if(string.IsNullOrEmpty(rows[i,6].Text))
        {
        Time_rate = 0;}else {Time_rate = System.Convert.ToSingle(rows[i,6].Text);
        }
        j = 7;
        if(string.IsNullOrEmpty(rows[i,7].Text))
        {
        Time = 0;}else {Time = System.Convert.ToSingle(rows[i,7].Text);
        }
        j = 8;
        if(string.IsNullOrEmpty(rows[i,8].Text))
        {
        Duration = 0;}else {Duration = System.Convert.ToSingle(rows[i,8].Text);
        }
        j = 10;
        Description = rows[i,10].Text;
        ItemEffect values = new ItemEffect(Index,Item_ID,Effect_type,Effect_min,Effect_max,Time_type,Time_rate,Time,Duration,Description);
        foreach (var preValues in list__)
        {
          if (preValues.Index.Equals(Index))
          {
            throw new System.Exception("row:" + i + " ItemEffect.Index:" + preValues.Index.ToString() + ") Duplicated!!");
          }
        }
        list__.Add(values);
      }
      array_ = list__.ToArray();
      ArrayToMap(array_);
    }catch(System.Exception e)
    {
      if( rows == null ) throw;
      if (j >= rows.GetLength(1))
        throw new System.Exception("sheet(ItemEffect) invalid column count:" + j);
      throw new System.Exception(" convert failure : excel(ItemTable).sheet(ItemEffect) key:" + rows[i,0].Text + " Name:" + rows[0,j].Text + " " + rows[i,j].Text + " " + e.Message );
    }
  }
  #endif
  public static void GetDataTable(System.Data.DataSet dts)
  {
    var table = dts.Tables.Add(nameof(ItemEffect));
    table.Columns.Add(nameof(Index), typeof(int));
    table.Columns.Add(nameof(Item_ID), typeof(int));
    table.Columns.Add(nameof(Effect_type), typeof(int));
    table.Columns.Add(nameof(Effect_min), typeof(float));
    table.Columns.Add(nameof(Effect_max), typeof(float));
    table.Columns.Add(nameof(Time_type), typeof(int));
    table.Columns.Add(nameof(Time_rate), typeof(float));
    table.Columns.Add(nameof(Time), typeof(float));
    table.Columns.Add(nameof(Duration), typeof(float));
    table.Columns.Add(nameof(Description), typeof(string));
    foreach(var item__ in array_ )
    {
      table.Rows.Add(item__.DataRow_);
    }
  }
  #endif
  public static void ReadStream(System.IO.BinaryReader __reader)
  {
    var count__ = __reader.ReadInt32();
    var array__ = new ItemEffect[count__];
    for (var __i=0;__i<array__.Length;__i++)
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
      var Description = Encoder.ReadString(ref __reader);
      var __table = new ItemEffect(Index,Item_ID,Effect_type,Effect_min,Effect_max,Time_type,Time_rate,Time,Duration,Description);
      array__[__i] = __table;
    }
    array_ = array__;
    ArrayToMap(array__);
  }
}


#if !ENCRYPT
[System.Serializable]
#endif
public partial record ItemEnchant
{
  private static ItemEnchant[] array_;
  public static ItemEnchant[] Array_
  {
    get
    {
      if (array_ != null) return array_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return array_;
    }
  }
  private static System.Collections.Immutable.ImmutableDictionary<int,ItemEnchant> map_;
  public static System.Collections.Immutable.ImmutableDictionary<int,ItemEnchant> Map_
  {
    get
    {
      if (map_ != null) return map_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return map_;
    }
  }
  public object[] DataRow_ => new object[]{
    Index
    ,Item_ID
    ,Enchant_lv
    ,Physical_attack
    ,Physical_defense
    ,Magic_attack
    ,Magic_defense
    ,Critical
    ,HP
    ,KnockBack_resist
    ,Material_IDS[0]
    ,Material_IDS[1]
    ,Material_IDS[2]
    ,Material_IDS[3]
    ,Material_IDS[4]
    ,Material_quantitys[0]
    ,Material_quantitys[1]
    ,Material_quantitys[2]
    ,Material_quantitys[3]
    ,Material_quantitys[4]
    ,Require_gold
    ,Require_cash
  };
  public static void ArrayToMap(ItemEnchant[] array__)
  {
    var map__ = new System.Collections.Generic.Dictionary<int,ItemEnchant> (array__.Length);
    var __i=0;
    try{
      for(__i=0;__i<array__.Length;__i++)
      {
        var __table = array__[__i];
        map__.Add(__table.Index, __table);
      }
    }catch(System.Exception e)
    {
        throw new System.Exception($"Row:{__i} {e.Message}");
    }
    map_ = System.Collections.Immutable.ImmutableDictionary<int,ItemEnchant>.Empty.AddRange(map__);
    map_ = map_.WithComparers(default(IntEqualityComparer));
  }
  

  public static void WriteStream(System.IO.BinaryWriter __writer)
  {
    __writer.Write(array_.Length);
    foreach (var __table in array_)
    {
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
      Encoder.Write7BitEncodedInt(__writer,__table.Material_IDS.Length);
      foreach(var t__ in __table.Material_IDS){__writer.Write(t__);}
      Encoder.Write7BitEncodedInt(__writer,__table.Material_quantitys.Length);
      foreach(var t__ in __table.Material_quantitys){__writer.Write(t__);}
      __writer.Write(__table.Require_gold);
      __writer.Write(__table.Require_cash);
    }
  }
  #if !UNITY_2018_2_OR_NEWER
  public static bool SetDataSet(System.Data.DataSet dts)
  {
    if(dts?.Tables[nameof(ItemEnchant)] is null) throw new System.Exception("dts.Tables['ItemEnchant'] is null");
    var tables__ = dts.Tables[nameof(ItemEnchant)];
    if(tables__ is null) throw new System.Exception("dts.Tables['ItemEnchant'] is null");
    map_ = System.Collections.Immutable.ImmutableDictionary<int,ItemEnchant>.Empty;
    array_ = new ItemEnchant[tables__.Rows.Count];
    var row__ = 0;
    foreach (System.Data.DataRow row in tables__.Rows)
    {
      var table__ = new ItemEnchant
      (
      System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Index)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Item_ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Enchant_lv)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Physical_attack)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Physical_defense)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Magic_attack)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Magic_defense)].ToString() ?? string.Empty))))
      ,System.Convert.ToSingle((row[nameof(Critical)].ToString() ?? string.Empty))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(HP)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(KnockBack_resist)].ToString() ?? string.Empty))))
      ,new int[]
      {
        System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_IDS0"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_IDS1"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_IDS2"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_IDS3"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_IDS4"].ToString() ?? string.Empty))))
      }
      ,new int[]
      {
        System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_quantitys0"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_quantitys1"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_quantitys2"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_quantitys3"].ToString() ?? string.Empty))))
        ,System.Convert.ToInt32(System.Math.Round(double.Parse((row["Material_quantitys4"].ToString() ?? string.Empty))))
      }
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Require_gold)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Require_cash)].ToString() ?? string.Empty))))
      );
      map_ = map_.Add(table__.Index, table__);
      array_[row__++] = table__;
    }
    return true;
  }
  #if !NO_EXCEL_LOADER
  public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
  {
    var i=0; var j=0;
    TableGenerateCmd.StringWithDesc[,] rows = null;
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
      var list__ = new System.Collections.Generic.List<ItemEnchant>(rows.GetLength(0) - 3);
      for (i = 3; i < rows.GetLength(0); i++)
      {
        j=0;
        if(rows[i,0].Text.Length == 0) break;
        j = 0;
        if(string.IsNullOrEmpty(rows[i,0].Text))
        {
        Index = 0;}else {Index = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0].Text)));
        }
        j = 1;
        if(string.IsNullOrEmpty(rows[i,1].Text))
        {
        Item_ID = 0;}else {Item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,1].Text)));
        }
        j = 8;
        if(string.IsNullOrEmpty(rows[i,8].Text))
        {
        Enchant_lv = 0;}else {Enchant_lv = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,8].Text)));
        }
        j = 9;
        if(string.IsNullOrEmpty(rows[i,9].Text))
        {
        Physical_attack = 0;}else {Physical_attack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,9].Text)));
        }
        j = 10;
        if(string.IsNullOrEmpty(rows[i,10].Text))
        {
        Physical_defense = 0;}else {Physical_defense = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,10].Text)));
        }
        j = 11;
        if(string.IsNullOrEmpty(rows[i,11].Text))
        {
        Magic_attack = 0;}else {Magic_attack = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,11].Text)));
        }
        j = 12;
        if(string.IsNullOrEmpty(rows[i,12].Text))
        {
        Magic_defense = 0;}else {Magic_defense = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,12].Text)));
        }
        j = 13;
        if(string.IsNullOrEmpty(rows[i,13].Text))
        {
        Critical = 0;}else {Critical = System.Convert.ToSingle(rows[i,13].Text);
        }
        j = 14;
        if(string.IsNullOrEmpty(rows[i,14].Text))
        {
        HP = 0;}else {HP = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,14].Text)));
        }
        j = 15;
        if(string.IsNullOrEmpty(rows[i,15].Text))
        {
        KnockBack_resist = 0;}else {KnockBack_resist = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,15].Text)));
        }
        var Material_IDS_list__ = new System.Collections.Generic.List<int>();
        bool not_empty_Material_IDS__ = false;
        j = 16;
        if(!string.IsNullOrEmpty(rows[i,16].Text))
        {
          Material_IDS_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,16].Text))));
        }
        else
        {
          not_empty_Material_IDS__ = true;
        }
        var Material_quantitys_list__ = new System.Collections.Generic.List<int>();
        bool not_empty_Material_quantitys__ = false;
        j = 17;
        if(!string.IsNullOrEmpty(rows[i,17].Text))
        {
          Material_quantitys_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,17].Text))));
        }
        else
        {
          not_empty_Material_quantitys__ = true;
        }
        j = 18;
        if(!string.IsNullOrEmpty(rows[i,18].Text))
        {
          if(not_empty_Material_IDS__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Material_IDS_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,18].Text))));
        }
        else
        {
          not_empty_Material_IDS__ = true;
        }
        j = 19;
        if(!string.IsNullOrEmpty(rows[i,19].Text))
        {
          if(not_empty_Material_quantitys__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Material_quantitys_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,19].Text))));
        }
        else
        {
          not_empty_Material_quantitys__ = true;
        }
        j = 20;
        if(!string.IsNullOrEmpty(rows[i,20].Text))
        {
          if(not_empty_Material_IDS__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Material_IDS_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,20].Text))));
        }
        else
        {
          not_empty_Material_IDS__ = true;
        }
        j = 21;
        if(!string.IsNullOrEmpty(rows[i,21].Text))
        {
          if(not_empty_Material_quantitys__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Material_quantitys_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,21].Text))));
        }
        else
        {
          not_empty_Material_quantitys__ = true;
        }
        j = 22;
        if(!string.IsNullOrEmpty(rows[i,22].Text))
        {
          if(not_empty_Material_IDS__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Material_IDS_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,22].Text))));
        }
        else
        {
          not_empty_Material_IDS__ = true;
        }
        j = 23;
        if(!string.IsNullOrEmpty(rows[i,23].Text))
        {
          if(not_empty_Material_quantitys__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Material_quantitys_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,23].Text))));
        }
        else
        {
          not_empty_Material_quantitys__ = true;
        }
        j = 24;
        if(!string.IsNullOrEmpty(rows[i,24].Text))
        {
          if(not_empty_Material_IDS__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Material_IDS_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,24].Text))));
        }
        else
        {
          not_empty_Material_IDS__ = true;
        }
        Material_IDS = Material_IDS_list__.ToArray();
        j = 25;
        if(!string.IsNullOrEmpty(rows[i,25].Text))
        {
          if(not_empty_Material_quantitys__)
          {
            throw new System.Exception(string.Format("i:{0} j:{1} before is empty text",i,j));
          }
          Material_quantitys_list__.Add(System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,25].Text))));
        }
        else
        {
          not_empty_Material_quantitys__ = true;
        }
        Material_quantitys = Material_quantitys_list__.ToArray();
        j = 26;
        if(string.IsNullOrEmpty(rows[i,26].Text))
        {
        Require_gold = 0;}else {Require_gold = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,26].Text)));
        }
        j = 27;
        if(string.IsNullOrEmpty(rows[i,27].Text))
        {
        Require_cash = 0;}else {Require_cash = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,27].Text)));
        }
        ItemEnchant values = new ItemEnchant(Index,Item_ID,Enchant_lv,Physical_attack,Physical_defense,Magic_attack,Magic_defense,Critical,HP,KnockBack_resist,Material_IDS,Material_quantitys,Require_gold,Require_cash);
        foreach (var preValues in list__)
        {
          if (preValues.Index.Equals(Index))
          {
            throw new System.Exception("row:" + i + " ItemEnchant.Index:" + preValues.Index.ToString() + ") Duplicated!!");
          }
        }
        list__.Add(values);
      }
      array_ = list__.ToArray();
      ArrayToMap(array_);
    }catch(System.Exception e)
    {
      if( rows == null ) throw;
      if (j >= rows.GetLength(1))
        throw new System.Exception("sheet(ItemEnchant) invalid column count:" + j);
      throw new System.Exception(" convert failure : excel(ItemTable).sheet(ItemEnchant) key:" + rows[i,0].Text + " Name:" + rows[0,j].Text + " " + rows[i,j].Text + " " + e.Message );
    }
  }
  #endif
  public static void GetDataTable(System.Data.DataSet dts)
  {
    var table = dts.Tables.Add(nameof(ItemEnchant));
    table.Columns.Add(nameof(Index), typeof(int));
    table.Columns.Add(nameof(Item_ID), typeof(int));
    table.Columns.Add(nameof(Enchant_lv), typeof(int));
    table.Columns.Add(nameof(Physical_attack), typeof(int));
    table.Columns.Add(nameof(Physical_defense), typeof(int));
    table.Columns.Add(nameof(Magic_attack), typeof(int));
    table.Columns.Add(nameof(Magic_defense), typeof(int));
    table.Columns.Add(nameof(Critical), typeof(float));
    table.Columns.Add(nameof(HP), typeof(int));
    table.Columns.Add(nameof(KnockBack_resist), typeof(int));
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
    table.Columns.Add(nameof(Require_gold), typeof(int));
    table.Columns.Add(nameof(Require_cash), typeof(int));
    foreach(var item__ in array_ )
    {
      table.Rows.Add(item__.DataRow_);
    }
  }
  #endif
  public static void ReadStream(System.IO.BinaryReader __reader)
  {
    var count__ = __reader.ReadInt32();
    var array__ = new ItemEnchant[count__];
    for (var __i=0;__i<array__.Length;__i++)
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
      int[] Material_IDS;
      {
        var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
        Material_IDS = arrayCount__ > 0?new int[arrayCount__]:System.Array.Empty<int>();
        for(var __j=0;__j<arrayCount__;++__j)Material_IDS[__j] = __reader.ReadInt32();
      }
      int[] Material_quantitys;
      {
        var arrayCount__ = TBL.Encoder.Read7BitEncodedInt(ref __reader);
        Material_quantitys = arrayCount__ > 0?new int[arrayCount__]:System.Array.Empty<int>();
        for(var __j=0;__j<arrayCount__;++__j)Material_quantitys[__j] = __reader.ReadInt32();
      }
      var Require_gold = __reader.ReadInt32();
      var Require_cash = __reader.ReadInt32();
      var __table = new ItemEnchant(Index,Item_ID,Enchant_lv,Physical_attack,Physical_defense,Magic_attack,Magic_defense,Critical,HP,KnockBack_resist,Material_IDS,Material_quantitys,Require_gold,Require_cash);
      array__[__i] = __table;
    }
    array_ = array__;
    ArrayToMap(array__);
  }
  public static int Material_IDS_Length => 5;
  public static int Material_quantitys_Length => 5;
}


#if !ENCRYPT
[System.Serializable]
#endif
public partial record ItemManufacture
{
  private static ItemManufacture[] array_;
  public static ItemManufacture[] Array_
  {
    get
    {
      if (array_ != null) return array_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return array_;
    }
  }
  private static System.Collections.Immutable.ImmutableDictionary<int,ItemManufacture> map_;
  public static System.Collections.Immutable.ImmutableDictionary<int,ItemManufacture> Map_
  {
    get
    {
      if (map_ != null) return map_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return map_;
    }
  }
  public object[] DataRow_ => new object[]{
    Index
    ,Subject_item_ID
    ,Material_item_ID
    ,Material_quantity
  };
  public static void ArrayToMap(ItemManufacture[] array__)
  {
    var map__ = new System.Collections.Generic.Dictionary<int,ItemManufacture> (array__.Length);
    var __i=0;
    try{
      for(__i=0;__i<array__.Length;__i++)
      {
        var __table = array__[__i];
        map__.Add(__table.Index, __table);
      }
    }catch(System.Exception e)
    {
        throw new System.Exception($"Row:{__i} {e.Message}");
    }
    map_ = System.Collections.Immutable.ImmutableDictionary<int,ItemManufacture>.Empty.AddRange(map__);
    map_ = map_.WithComparers(default(IntEqualityComparer));
  }
  

  public static void WriteStream(System.IO.BinaryWriter __writer)
  {
    __writer.Write(array_.Length);
    foreach (var __table in array_)
    {
      __writer.Write(__table.Index);
      __writer.Write(__table.Subject_item_ID);
      __writer.Write(__table.Material_item_ID);
      __writer.Write(__table.Material_quantity);
    }
  }
  #if !UNITY_2018_2_OR_NEWER
  public static bool SetDataSet(System.Data.DataSet dts)
  {
    if(dts?.Tables[nameof(ItemManufacture)] is null) throw new System.Exception("dts.Tables['ItemManufacture'] is null");
    var tables__ = dts.Tables[nameof(ItemManufacture)];
    if(tables__ is null) throw new System.Exception("dts.Tables['ItemManufacture'] is null");
    map_ = System.Collections.Immutable.ImmutableDictionary<int,ItemManufacture>.Empty;
    array_ = new ItemManufacture[tables__.Rows.Count];
    var row__ = 0;
    foreach (System.Data.DataRow row in tables__.Rows)
    {
      var table__ = new ItemManufacture
      (
      System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Index)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Subject_item_ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Material_item_ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Material_quantity)].ToString() ?? string.Empty))))
      );
      map_ = map_.Add(table__.Index, table__);
      array_[row__++] = table__;
    }
    return true;
  }
  #if !NO_EXCEL_LOADER
  public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
  {
    var i=0; var j=0;
    TableGenerateCmd.StringWithDesc[,] rows = null;
    int Index;
    int Subject_item_ID;
    int Material_item_ID;
    int Material_quantity;
    try
    {
      rows = imp.GetSheet("ItemManufacture", language);
      var list__ = new System.Collections.Generic.List<ItemManufacture>(rows.GetLength(0) - 3);
      for (i = 3; i < rows.GetLength(0); i++)
      {
        j=0;
        if(rows[i,0].Text.Length == 0) break;
        j = 0;
        if(string.IsNullOrEmpty(rows[i,0].Text))
        {
        Index = 0;}else {Index = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0].Text)));
        }
        j = 1;
        if(string.IsNullOrEmpty(rows[i,1].Text))
        {
        Subject_item_ID = 0;}else {Subject_item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,1].Text)));
        }
        j = 2;
        if(string.IsNullOrEmpty(rows[i,2].Text))
        {
        Material_item_ID = 0;}else {Material_item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,2].Text)));
        }
        j = 4;
        if(string.IsNullOrEmpty(rows[i,4].Text))
        {
        Material_quantity = 0;}else {Material_quantity = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,4].Text)));
        }
        ItemManufacture values = new ItemManufacture(Index,Subject_item_ID,Material_item_ID,Material_quantity);
        foreach (var preValues in list__)
        {
          if (preValues.Index.Equals(Index))
          {
            throw new System.Exception("row:" + i + " ItemManufacture.Index:" + preValues.Index.ToString() + ") Duplicated!!");
          }
        }
        list__.Add(values);
      }
      array_ = list__.ToArray();
      ArrayToMap(array_);
    }catch(System.Exception e)
    {
      if( rows == null ) throw;
      if (j >= rows.GetLength(1))
        throw new System.Exception("sheet(ItemManufacture) invalid column count:" + j);
      throw new System.Exception(" convert failure : excel(ItemTable).sheet(ItemManufacture) key:" + rows[i,0].Text + " Name:" + rows[0,j].Text + " " + rows[i,j].Text + " " + e.Message );
    }
  }
  #endif
  public static void GetDataTable(System.Data.DataSet dts)
  {
    var table = dts.Tables.Add(nameof(ItemManufacture));
    table.Columns.Add(nameof(Index), typeof(int));
    table.Columns.Add(nameof(Subject_item_ID), typeof(int));
    table.Columns.Add(nameof(Material_item_ID), typeof(int));
    table.Columns.Add(nameof(Material_quantity), typeof(int));
    foreach(var item__ in array_ )
    {
      table.Rows.Add(item__.DataRow_);
    }
  }
  #endif
  public static void ReadStream(System.IO.BinaryReader __reader)
  {
    var count__ = __reader.ReadInt32();
    var array__ = new ItemManufacture[count__];
    for (var __i=0;__i<array__.Length;__i++)
    {
      var Index = __reader.ReadInt32();
      var Subject_item_ID = __reader.ReadInt32();
      var Material_item_ID = __reader.ReadInt32();
      var Material_quantity = __reader.ReadInt32();
      var __table = new ItemManufacture(Index,Subject_item_ID,Material_item_ID,Material_quantity);
      array__[__i] = __table;
    }
    array_ = array__;
    ArrayToMap(array__);
  }
}


#if !ENCRYPT
[System.Serializable]
#endif
public partial record RandomBoxGroup
{
  private static RandomBoxGroup[] array_;
  public static RandomBoxGroup[] Array_
  {
    get
    {
      if (array_ != null) return array_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return array_;
    }
  }
  private static System.Collections.Immutable.ImmutableDictionary<int,RandomBoxGroup> map_;
  public static System.Collections.Immutable.ImmutableDictionary<int,RandomBoxGroup> Map_
  {
    get
    {
      if (map_ != null) return map_;
      if (string.IsNullOrEmpty(Base_.Path) || string.IsNullOrEmpty(Base_.Language)) throw new System.Exception("TBL.Base_.Path or TBL.Base_.Language is empty");
      Loader loader = new();
      var path = $"{Base_.Path}/ScriptTable/{Base_.Language}/{loader.GetFileName()}.bytes";
      using var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      loader.ReadStream(stream);
      return map_;
    }
  }
  public object[] DataRow_ => new object[]{
    ID
    ,RandomItemGroup_NO
    ,ClassType
    ,Item_ID
    ,RatioAmount
    ,Item_Quantity
  };
  public static void ArrayToMap(RandomBoxGroup[] array__)
  {
    var map__ = new System.Collections.Generic.Dictionary<int,RandomBoxGroup> (array__.Length);
    var __i=0;
    try{
      for(__i=0;__i<array__.Length;__i++)
      {
        var __table = array__[__i];
        map__.Add(__table.ID, __table);
      }
    }catch(System.Exception e)
    {
        throw new System.Exception($"Row:{__i} {e.Message}");
    }
    map_ = System.Collections.Immutable.ImmutableDictionary<int,RandomBoxGroup>.Empty.AddRange(map__);
    map_ = map_.WithComparers(default(IntEqualityComparer));
  }
  

  public static void WriteStream(System.IO.BinaryWriter __writer)
  {
    __writer.Write(array_.Length);
    foreach (var __table in array_)
    {
      __writer.Write(__table.ID);
      __writer.Write(__table.RandomItemGroup_NO);
      __writer.Write(__table.ClassType);
      __writer.Write(__table.Item_ID);
      __writer.Write(__table.RatioAmount);
      __writer.Write(__table.Item_Quantity);
    }
  }
  #if !UNITY_2018_2_OR_NEWER
  public static bool SetDataSet(System.Data.DataSet dts)
  {
    if(dts?.Tables[nameof(RandomBoxGroup)] is null) throw new System.Exception("dts.Tables['RandomBoxGroup'] is null");
    var tables__ = dts.Tables[nameof(RandomBoxGroup)];
    if(tables__ is null) throw new System.Exception("dts.Tables['RandomBoxGroup'] is null");
    map_ = System.Collections.Immutable.ImmutableDictionary<int,RandomBoxGroup>.Empty;
    array_ = new RandomBoxGroup[tables__.Rows.Count];
    var row__ = 0;
    foreach (System.Data.DataRow row in tables__.Rows)
    {
      var table__ = new RandomBoxGroup
      (
      System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(RandomItemGroup_NO)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(ClassType)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Item_ID)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(RatioAmount)].ToString() ?? string.Empty))))
      ,System.Convert.ToInt32(System.Math.Round(double.Parse((row[nameof(Item_Quantity)].ToString() ?? string.Empty))))
      );
      map_ = map_.Add(table__.ID, table__);
      array_[row__++] = table__;
    }
    return true;
  }
  #if !NO_EXCEL_LOADER
  public static void ExcelLoad(ClassUtil.ExcelImporter imp,string path,string language)
  {
    var i=0; var j=0;
    TableGenerateCmd.StringWithDesc[,] rows = null;
    int ID;
    int RandomItemGroup_NO;
    int ClassType;
    int Item_ID;
    int RatioAmount;
    int Item_Quantity;
    try
    {
      rows = imp.GetSheet("RandomBoxGroup", language);
      var list__ = new System.Collections.Generic.List<RandomBoxGroup>(rows.GetLength(0) - 3);
      for (i = 3; i < rows.GetLength(0); i++)
      {
        j=0;
        if(rows[i,0].Text.Length == 0) break;
        j = 0;
        if(string.IsNullOrEmpty(rows[i,0].Text))
        {
        ID = 0;}else {ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,0].Text)));
        }
        j = 2;
        if(string.IsNullOrEmpty(rows[i,2].Text))
        {
        RandomItemGroup_NO = 0;}else {RandomItemGroup_NO = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,2].Text)));
        }
        j = 3;
        if(string.IsNullOrEmpty(rows[i,3].Text))
        {
        ClassType = 0;}else {ClassType = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,3].Text)));
        }
        j = 4;
        if(string.IsNullOrEmpty(rows[i,4].Text))
        {
        Item_ID = 0;}else {Item_ID = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,4].Text)));
        }
        j = 5;
        if(string.IsNullOrEmpty(rows[i,5].Text))
        {
        RatioAmount = 0;}else {RatioAmount = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,5].Text)));
        }
        j = 6;
        if(string.IsNullOrEmpty(rows[i,6].Text))
        {
        Item_Quantity = 0;}else {Item_Quantity = System.Convert.ToInt32(System.Math.Round(double.Parse(rows[i,6].Text)));
        }
        RandomBoxGroup values = new RandomBoxGroup(ID,RandomItemGroup_NO,ClassType,Item_ID,RatioAmount,Item_Quantity);
        foreach (var preValues in list__)
        {
          if (preValues.ID.Equals(ID))
          {
            throw new System.Exception("row:" + i + " RandomBoxGroup.ID:" + preValues.ID.ToString() + ") Duplicated!!");
          }
        }
        list__.Add(values);
      }
      array_ = list__.ToArray();
      ArrayToMap(array_);
    }catch(System.Exception e)
    {
      if( rows == null ) throw;
      if (j >= rows.GetLength(1))
        throw new System.Exception("sheet(RandomBoxGroup) invalid column count:" + j);
      throw new System.Exception(" convert failure : excel(ItemTable).sheet(RandomBoxGroup) key:" + rows[i,0].Text + " Name:" + rows[0,j].Text + " " + rows[i,j].Text + " " + e.Message );
    }
  }
  #endif
  public static void GetDataTable(System.Data.DataSet dts)
  {
    var table = dts.Tables.Add(nameof(RandomBoxGroup));
    table.Columns.Add(nameof(ID), typeof(int));
    table.Columns.Add(nameof(RandomItemGroup_NO), typeof(int));
    table.Columns.Add(nameof(ClassType), typeof(int));
    table.Columns.Add(nameof(Item_ID), typeof(int));
    table.Columns.Add(nameof(RatioAmount), typeof(int));
    table.Columns.Add(nameof(Item_Quantity), typeof(int));
    foreach(var item__ in array_ )
    {
      table.Rows.Add(item__.DataRow_);
    }
  }
  #endif
  public static void ReadStream(System.IO.BinaryReader __reader)
  {
    var count__ = __reader.ReadInt32();
    var array__ = new RandomBoxGroup[count__];
    for (var __i=0;__i<array__.Length;__i++)
    {
      var ID = __reader.ReadInt32();
      var RandomItemGroup_NO = __reader.ReadInt32();
      var ClassType = __reader.ReadInt32();
      var Item_ID = __reader.ReadInt32();
      var RatioAmount = __reader.ReadInt32();
      var Item_Quantity = __reader.ReadInt32();
      var __table = new RandomBoxGroup(ID,RandomItemGroup_NO,ClassType,Item_ID,RatioAmount,Item_Quantity);
      array__[__i] = __table;
    }
    array_ = array__;
    ArrayToMap(array__);
  }
}
