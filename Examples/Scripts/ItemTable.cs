// generate ItemTable.cs
// DO NOT TOUCH SOURCE....
namespace TBL.ItemTable.BaseClasses
{
  #if !UNITY_2018_2_OR_NEWER
  public interface IItem
  {
    /// <summary>
    /// Key Column
    /// </summary>
    int Item_ID {get;}
    string Name {get;}
    int Item_grade {get;}
    int Require_lv {get;}
    int Enchant_lv {get;}
    int PhysicalAttack {get;}
    int PhysicalDefense {get;}
    int MagicalAttack {get;}
    int MagicalDefense {get;}
    float Critical {get;}
    int HP {get;}
    int KnockBackResist {get;}
    eDictionaryType DictionaryType {get;}
    int ItemType {get;}
    short Gear_Score {get;}
    short InventoryType {get;}
    bool UsageType {get;}
    short Socket_quantity {get;}
    int Removal_cost {get;}
    short Belonging {get;}
    short Sub_stats_quantity {get;}
    int Stack {get;}
    int DesignScroll_ID {get;}
    int BindingSkill_ID {get;}
    int BindingAttack_ID {get;}
    int Manufacture_gold {get;}
    int Manufacture_cash {get;}
    int SummonCompanion_ID {get;}
    int Next_itemID {get;}
    int Next_item_price {get;}
    int[] Next_Item_material {get;}
    int[] Next_Item_material_quantity {get;}
    string Resource_Path {get;}
    string WeaponName {get;}
    short WeaponIndex {get;}
    string[] PartName {get;}
    short[] PartIndex {get;}
    string Icon_path {get;}
    int EXP {get;}
    int Buy_cost {get;}
    int Sell_reward {get;}
    int Consignment_maxprice {get;}
    int QuestBringer {get;}
    int ItemEvent_ID {get;}
    string Description {get;}
    int Sub_Item {get;}
    int WeaponType {get;}
    int[] RandomBoxGroup_NO {get;}
  }
  #endif
  public class Item
  {
    /// <summary>
    /// Key Column
    /// </summary>
      public readonly int Item_ID;
      public readonly string Name;
      public readonly int Item_grade;
      public readonly int Require_lv;
      public readonly int Enchant_lv;
      public readonly int PhysicalAttack;
      public readonly int PhysicalDefense;
      public readonly int MagicalAttack;
      public readonly int MagicalDefense;
      public readonly float Critical;
      public readonly int HP;
      public readonly int KnockBackResist;
      public readonly eDictionaryType DictionaryType;
      public readonly int ItemType;
      public readonly short Gear_Score;
      public readonly short InventoryType;
      public readonly bool UsageType;
      public readonly short Socket_quantity;
      public readonly int Removal_cost;
      public readonly short Belonging;
      public readonly short Sub_stats_quantity;
      public readonly int Stack;
      public readonly int DesignScroll_ID;
      public readonly int BindingSkill_ID;
      public readonly int BindingAttack_ID;
      public readonly int Manufacture_gold;
      public readonly int Manufacture_cash;
      public readonly int SummonCompanion_ID;
      public readonly int Next_itemID;
      public readonly int Next_item_price;
      public readonly int[] Next_Item_material;
      public readonly int[] Next_Item_material_quantity;
      public readonly string Resource_Path;
      public readonly string WeaponName;
      public readonly short WeaponIndex;
      public readonly string[] PartName;
      public readonly short[] PartIndex;
      public readonly string Icon_path;
      public readonly int EXP;
      public readonly int Buy_cost;
      public readonly int Sell_reward;
      public readonly int Consignment_maxprice;
      public readonly int QuestBringer;
      public readonly int ItemEvent_ID;
      public readonly string Description;
      public readonly int Sub_Item;
      public readonly int WeaponType;
      public readonly int[] RandomBoxGroup_NO;
    public Item (int Item_ID__,string Name__,int Item_grade__,int Require_lv__,int Enchant_lv__,int PhysicalAttack__,int PhysicalDefense__,int MagicalAttack__,int MagicalDefense__,float Critical__,int HP__,int KnockBackResist__,eDictionaryType DictionaryType__,int ItemType__,short Gear_Score__,short InventoryType__,bool UsageType__,short Socket_quantity__,int Removal_cost__,short Belonging__,short Sub_stats_quantity__,int Stack__,int DesignScroll_ID__,int BindingSkill_ID__,int BindingAttack_ID__,int Manufacture_gold__,int Manufacture_cash__,int SummonCompanion_ID__,int Next_itemID__,int Next_item_price__,int[] Next_Item_material__,int[] Next_Item_material_quantity__,string Resource_Path__,string WeaponName__,short WeaponIndex__,string[] PartName__,short[] PartIndex__,string Icon_path__,int EXP__,int Buy_cost__,int Sell_reward__,int Consignment_maxprice__,int QuestBringer__,int ItemEvent_ID__,string Description__,int Sub_Item__,int WeaponType__,int[] RandomBoxGroup_NO__)
    {
      this.Item_ID = Item_ID__;
      this.Name = Name__;
      this.Item_grade = Item_grade__;
      this.Require_lv = Require_lv__;
      this.Enchant_lv = Enchant_lv__;
      this.PhysicalAttack = PhysicalAttack__;
      this.PhysicalDefense = PhysicalDefense__;
      this.MagicalAttack = MagicalAttack__;
      this.MagicalDefense = MagicalDefense__;
      this.Critical = Critical__;
      this.HP = HP__;
      this.KnockBackResist = KnockBackResist__;
      this.DictionaryType = DictionaryType__;
      this.ItemType = ItemType__;
      this.Gear_Score = Gear_Score__;
      this.InventoryType = InventoryType__;
      this.UsageType = UsageType__;
      this.Socket_quantity = Socket_quantity__;
      this.Removal_cost = Removal_cost__;
      this.Belonging = Belonging__;
      this.Sub_stats_quantity = Sub_stats_quantity__;
      this.Stack = Stack__;
      this.DesignScroll_ID = DesignScroll_ID__;
      this.BindingSkill_ID = BindingSkill_ID__;
      this.BindingAttack_ID = BindingAttack_ID__;
      this.Manufacture_gold = Manufacture_gold__;
      this.Manufacture_cash = Manufacture_cash__;
      this.SummonCompanion_ID = SummonCompanion_ID__;
      this.Next_itemID = Next_itemID__;
      this.Next_item_price = Next_item_price__;
      this.Next_Item_material = Next_Item_material__;
      this.Next_Item_material_quantity = Next_Item_material_quantity__;
      this.Resource_Path = Resource_Path__;
      this.WeaponName = WeaponName__;
      this.WeaponIndex = WeaponIndex__;
      this.PartName = PartName__;
      this.PartIndex = PartIndex__;
      this.Icon_path = Icon_path__;
      this.EXP = EXP__;
      this.Buy_cost = Buy_cost__;
      this.Sell_reward = Sell_reward__;
      this.Consignment_maxprice = Consignment_maxprice__;
      this.QuestBringer = QuestBringer__;
      this.ItemEvent_ID = ItemEvent_ID__;
      this.Description = Description__;
      this.Sub_Item = Sub_Item__;
      this.WeaponType = WeaponType__;
      this.RandomBoxGroup_NO = RandomBoxGroup_NO__;
    }
  }
  #if !UNITY_2018_2_OR_NEWER
  public interface IItemEffect
  {
    /// <summary>
    /// Key Column
    /// </summary>
    int Index {get;}
    int Item_ID {get;}
    int Effect_type {get;}
    float Effect_min {get;}
    float Effect_max {get;}
    int Time_type {get;}
    float Time_rate {get;}
    float Time {get;}
    float Duration {get;}
    string Description {get;}
  }
  #endif
  public class ItemEffect
  {
    /// <summary>
    /// Key Column
    /// </summary>
      public readonly int Index;
      public readonly int Item_ID;
      public readonly int Effect_type;
      public readonly float Effect_min;
      public readonly float Effect_max;
      public readonly int Time_type;
      public readonly float Time_rate;
      public readonly float Time;
      public readonly float Duration;
      public readonly string Description;
    public ItemEffect (int Index__,int Item_ID__,int Effect_type__,float Effect_min__,float Effect_max__,int Time_type__,float Time_rate__,float Time__,float Duration__,string Description__)
    {
      this.Index = Index__;
      this.Item_ID = Item_ID__;
      this.Effect_type = Effect_type__;
      this.Effect_min = Effect_min__;
      this.Effect_max = Effect_max__;
      this.Time_type = Time_type__;
      this.Time_rate = Time_rate__;
      this.Time = Time__;
      this.Duration = Duration__;
      this.Description = Description__;
    }
  }
  #if !UNITY_2018_2_OR_NEWER
  public interface IItemEnchant
  {
    /// <summary>
    /// Key Column
    /// </summary>
    int Index {get;}
    int Item_ID {get;}
    int Enchant_lv {get;}
    int Physical_attack {get;}
    int Physical_defense {get;}
    int Magic_attack {get;}
    int Magic_defense {get;}
    float Critical {get;}
    int HP {get;}
    int KnockBack_resist {get;}
    int[] Material_IDS {get;}
    int[] Material_quantitys {get;}
    int Require_gold {get;}
    int Require_cash {get;}
  }
  #endif
  public class ItemEnchant
  {
    /// <summary>
    /// Key Column
    /// </summary>
      public readonly int Index;
      public readonly int Item_ID;
      public readonly int Enchant_lv;
      public readonly int Physical_attack;
      public readonly int Physical_defense;
      public readonly int Magic_attack;
      public readonly int Magic_defense;
      public readonly float Critical;
      public readonly int HP;
      public readonly int KnockBack_resist;
      public readonly int[] Material_IDS;
      public readonly int[] Material_quantitys;
      public readonly int Require_gold;
      public readonly int Require_cash;
    public ItemEnchant (int Index__,int Item_ID__,int Enchant_lv__,int Physical_attack__,int Physical_defense__,int Magic_attack__,int Magic_defense__,float Critical__,int HP__,int KnockBack_resist__,int[] Material_IDS__,int[] Material_quantitys__,int Require_gold__,int Require_cash__)
    {
      this.Index = Index__;
      this.Item_ID = Item_ID__;
      this.Enchant_lv = Enchant_lv__;
      this.Physical_attack = Physical_attack__;
      this.Physical_defense = Physical_defense__;
      this.Magic_attack = Magic_attack__;
      this.Magic_defense = Magic_defense__;
      this.Critical = Critical__;
      this.HP = HP__;
      this.KnockBack_resist = KnockBack_resist__;
      this.Material_IDS = Material_IDS__;
      this.Material_quantitys = Material_quantitys__;
      this.Require_gold = Require_gold__;
      this.Require_cash = Require_cash__;
    }
  }
  #if !UNITY_2018_2_OR_NEWER
  public interface IItemManufacture
  {
    /// <summary>
    /// Key Column
    /// </summary>
    int Index {get;}
    int Subject_item_ID {get;}
    int Material_item_ID {get;}
    int Material_quantity {get;}
  }
  #endif
  public class ItemManufacture
  {
    /// <summary>
    /// Key Column
    /// </summary>
      public readonly int Index;
      public readonly int Subject_item_ID;
      public readonly int Material_item_ID;
      public readonly int Material_quantity;
    public ItemManufacture (int Index__,int Subject_item_ID__,int Material_item_ID__,int Material_quantity__)
    {
      this.Index = Index__;
      this.Subject_item_ID = Subject_item_ID__;
      this.Material_item_ID = Material_item_ID__;
      this.Material_quantity = Material_quantity__;
    }
  }
  #if !UNITY_2018_2_OR_NEWER
  public interface IRandomBoxGroup
  {
    /// <summary>
    /// Key Column
    /// </summary>
    int ID {get;}
    int RandomItemGroup_NO {get;}
    int ClassType {get;}
    int Item_ID {get;}
    int[] RatioAmount {get;}
    int Item_Quantity {get;}
  }
  #endif
  public class RandomBoxGroup
  {
    /// <summary>
    /// Key Column
    /// </summary>
      public readonly int ID;
      public readonly int RandomItemGroup_NO;
      public readonly int ClassType;
      public readonly int Item_ID;
      public readonly int[] RatioAmount;
      public readonly int Item_Quantity;
    public RandomBoxGroup (int ID__,int RandomItemGroup_NO__,int ClassType__,int Item_ID__,int[] RatioAmount__,int Item_Quantity__)
    {
      this.ID = ID__;
      this.RandomItemGroup_NO = RandomItemGroup_NO__;
      this.ClassType = ClassType__;
      this.Item_ID = Item_ID__;
      this.RatioAmount = RatioAmount__;
      this.Item_Quantity = Item_Quantity__;
    }
  }
};
