#pragma warning disable IDE0007, IDE0011, IDE0025, IDE1006, IDE0018
namespace TBL.ItemTable
{
  /// <summary>
  /// Item
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class Item
  {
    public int Item_ID {get; private set;}
    public string Name {get; private set;}
    public int Item_grade {get; private set;}
    public int Require_lv {get; private set;}
    public int Enchant_lv {get; private set;}
    public int PhysicalAttack {get; private set;}
    public int PhysicalDefense {get; private set;}
    public int MagicalAttack {get; private set;}
    public int MagicalDefense {get; private set;}
    public float Critical {get; private set;}
    public int HP {get; private set;}
    public int KnockBackResist {get; private set;}
    public eDictionaryType DictionaryType {get; private set;}
    public int ItemType {get; private set;}
    public short Gear_Score {get; private set;}
    public short InventoryType {get; private set;}
    public bool UsageType {get; private set;}
    public short Socket_quantity {get; private set;}
    public int Removal_cost {get; private set;}
    public short Belonging {get; private set;}
    public short Sub_stats_quantity {get; private set;}
    public int Stack {get; private set;}
    public int DesignScroll_ID {get; private set;}
    public int BindingSkill_ID {get; private set;}
    public int BindingAttack_ID {get; private set;}
    public int Manufacture_gold {get; private set;}
    public int Manufacture_cash {get; private set;}
    public int SummonCompanion_ID {get; private set;}
    public int Next_itemID {get; private set;}
    public int Next_item_price {get; private set;}
    public int[] Next_Item_material {get; private set;}
    /// <param name="Next_Item_material_quantity">젬의 경우 전체 필요한 수량을 여기에 적는다상위 젬을 만들기 위해 하위 젬이 4개 필요한 경우 4라고 기재</param> 
    public int[] Next_Item_material_quantity {get; private set;}
    public string Resource_Path {get; private set;}
    public string WeaponName {get; private set;}
    public short WeaponIndex {get; private set;}
    public string[] PartName {get; private set;}
    public short[] PartIndex {get; private set;}
    public string Icon_path {get; private set;}
    public int EXP {get; private set;}
    public int Buy_cost {get; private set;}
    public int Sell_reward {get; private set;}
    public int Consignment_maxprice {get; private set;}
    public int QuestBringer {get; private set;}
    public int ItemEvent_ID {get; private set;}
    public string Description {get; private set;}
    /// <param name="Sub_Item">NETEASE-SH:방패등 서브아이템 아이디</param> 
    public int Sub_Item {get; private set;}
    /// <param name="WeaponType">wlfh qkr:0: 맨손1: 왼손무기2: 오른손 무기</param> 
    public int WeaponType {get; private set;}
    public int[] RandomBoxGroup_NO {get; private set;}
  }
  /// <summary>
  /// ItemEffect
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class ItemEffect
  {
    public int Index {get; private set;}
    public int Item_ID {get; private set;}
    public int Effect_type {get; private set;}
    public float Effect_min {get; private set;}
    public float Effect_max {get; private set;}
    public int Time_type {get; private set;}
    public float Time_rate {get; private set;}
    public float Time {get; private set;}
    public float Duration {get; private set;}
    public string Description {get; private set;}
  }
  /// <summary>
  /// ItemEnchant
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class ItemEnchant
  {
    public int Index {get; private set;}
    public int Item_ID {get; private set;}
    public int Enchant_lv {get; private set;}
    public int Physical_attack {get; private set;}
    public int Physical_defense {get; private set;}
    public int Magic_attack {get; private set;}
    public int Magic_defense {get; private set;}
    public float Critical {get; private set;}
    public int HP {get; private set;}
    public int KnockBack_resist {get; private set;}
    public int[] Material_IDS {get; private set;}
    public int[] Material_quantitys {get; private set;}
    public int Require_gold {get; private set;}
    public int Require_cash {get; private set;}
  }
  /// <summary>
  /// ItemManufacture
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class ItemManufacture
  {
    public int Index {get; private set;}
    public int Subject_item_ID {get; private set;}
    public int Material_item_ID {get; private set;}
    public int Material_quantity {get; private set;}
  }
  /// <summary>
  /// RandomBoxGroup
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class RandomBoxGroup
  {
    public int ID {get; private set;}
    public int RandomItemGroup_NO {get; private set;}
    public int ClassType {get; private set;}
    public int Item_ID {get; private set;}
    public int RatioAmount {get; private set;}
    public int Item_Quantity {get; private set;}
  }
}
