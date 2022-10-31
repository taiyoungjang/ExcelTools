#pragma warning disable IDE0007, IDE0011, IDE0025, IDE1006, IDE0018
namespace TBL.ItemTable
{
  /// <summary>
  /// Item
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class Item
  {
    public int Item_ID {get; private set;} = default;
    public string Name {get; private set;} = string.Empty;
    public int Item_grade {get; private set;} = default;
    public int Require_lv {get; private set;} = default;
    public int Enchant_lv {get; private set;} = default;
    public int PhysicalAttack {get; private set;} = default;
    public int PhysicalDefense {get; private set;} = default;
    public int MagicalAttack {get; private set;} = default;
    public int MagicalDefense {get; private set;} = default;
    public float Critical {get; private set;} = default;
    public int HP {get; private set;} = default;
    public int KnockBackResist {get; private set;} = default;
    public eDictionaryType DictionaryType {get; private set;} = default;
    public int ItemType {get; private set;} = default;
    public short Gear_Score {get; private set;} = default;
    public short InventoryType {get; private set;} = default;
    public bool UsageType {get; private set;} = default;
    public short Socket_quantity {get; private set;} = default;
    public int Removal_cost {get; private set;} = default;
    public short Belonging {get; private set;} = default;
    public short Sub_stats_quantity {get; private set;} = default;
    public int Stack {get; private set;} = default;
    public int DesignScroll_ID {get; private set;} = default;
    public int BindingSkill_ID {get; private set;} = default;
    public int BindingAttack_ID {get; private set;} = default;
    public int Manufacture_gold {get; private set;} = default;
    public int Manufacture_cash {get; private set;} = default;
    public int SummonCompanion_ID {get; private set;} = default;
    public int Next_itemID {get; private set;} = default;
    public int Next_item_price {get; private set;} = default;
    public int[] Next_Item_material {get; private set;} = System.Array.Empty<int>();
    /// 젬의 경우 전체 필요한 수량을 여기에 적는다상위 젬을 만들기 위해 하위 젬이 4개 필요한 경우 4라고 기재
    public int[] Next_Item_material_quantity {get; private set;} = System.Array.Empty<int>();
    public string Resource_Path {get; private set;} = string.Empty;
    public string WeaponName {get; private set;} = string.Empty;
    public short WeaponIndex {get; private set;} = default;
    public string[] PartName {get; private set;} = System.Array.Empty<string>();
    public short[] PartIndex {get; private set;} = System.Array.Empty<short>();
    public string Icon_path {get; private set;} = string.Empty;
    public int EXP {get; private set;} = default;
    public int Buy_cost {get; private set;} = default;
    public int Sell_reward {get; private set;} = default;
    public int Consignment_maxprice {get; private set;} = default;
    public int QuestBringer {get; private set;} = default;
    public int ItemEvent_ID {get; private set;} = default;
    public string Description {get; private set;} = string.Empty;
    /// NETEASE-SH:방패등 서브아이템 아이디
    public int Sub_Item {get; private set;} = default;
    /// wlfh qkr:0: 맨손1: 왼손무기2: 오른손 무기
    public int WeaponType {get; private set;} = default;
    public int[] RandomBoxGroup_NO {get; private set;} = System.Array.Empty<int>();
  }
  /// <summary>
  /// ItemEffect
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class ItemEffect
  {
    public int Index {get; private set;} = default;
    public int Item_ID {get; private set;} = default;
    public int Effect_type {get; private set;} = default;
    public float Effect_min {get; private set;} = default;
    public float Effect_max {get; private set;} = default;
    public int Time_type {get; private set;} = default;
    public float Time_rate {get; private set;} = default;
    public float Time {get; private set;} = default;
    public float Duration {get; private set;} = default;
    public string Description {get; private set;} = string.Empty;
  }
  /// <summary>
  /// ItemEnchant
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class ItemEnchant
  {
    public int Index {get; private set;} = default;
    public int Item_ID {get; private set;} = default;
    public int Enchant_lv {get; private set;} = default;
    public int Physical_attack {get; private set;} = default;
    public int Physical_defense {get; private set;} = default;
    public int Magic_attack {get; private set;} = default;
    public int Magic_defense {get; private set;} = default;
    public float Critical {get; private set;} = default;
    public int HP {get; private set;} = default;
    public int KnockBack_resist {get; private set;} = default;
    public int[] Material_IDS {get; private set;} = System.Array.Empty<int>();
    public int[] Material_quantitys {get; private set;} = System.Array.Empty<int>();
    public int Require_gold {get; private set;} = default;
    public int Require_cash {get; private set;} = default;
  }
  /// <summary>
  /// ItemManufacture
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class ItemManufacture
  {
    public int Index {get; private set;} = default;
    public int Subject_item_ID {get; private set;} = default;
    public int Material_item_ID {get; private set;} = default;
    public int Material_quantity {get; private set;} = default;
  }
  /// <summary>
  /// RandomBoxGroup
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
  public partial class RandomBoxGroup
  {
    public int ID {get; private set;} = default;
    public int RandomItemGroup_NO {get; private set;} = default;
    public int ClassType {get; private set;} = default;
    public int Item_ID {get; private set;} = default;
    public int RatioAmount {get; private set;} = default;
    public int Item_Quantity {get; private set;} = default;
  }
}
