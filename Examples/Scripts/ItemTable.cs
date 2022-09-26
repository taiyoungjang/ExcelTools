#pragma warning disable IDE0007, IDE0011, IDE0025, IDE1006, IDE0018
namespace TBL.ItemTable;
/// <summary>
/// Item
/// </summary>
/// <param name="Item_ID"></param> 
/// <param name="Name"></param> 
/// <param name="Item_grade"></param> 
/// <param name="Require_lv"></param> 
/// <param name="Enchant_lv"></param> 
/// <param name="PhysicalAttack"></param> 
/// <param name="PhysicalDefense"></param> 
/// <param name="MagicalAttack"></param> 
/// <param name="MagicalDefense"></param> 
/// <param name="Critical"></param> 
/// <param name="HP"></param> 
/// <param name="KnockBackResist"></param> 
/// <param name="DictionaryType"></param> 
/// <param name="ItemType"></param> 
/// <param name="Gear_Score"></param> 
/// <param name="InventoryType"></param> 
/// <param name="UsageType"></param> 
/// <param name="Socket_quantity"></param> 
/// <param name="Removal_cost"></param> 
/// <param name="Belonging"></param> 
/// <param name="Sub_stats_quantity"></param> 
/// <param name="Stack"></param> 
/// <param name="DesignScroll_ID"></param> 
/// <param name="BindingSkill_ID"></param> 
/// <param name="BindingAttack_ID"></param> 
/// <param name="Manufacture_gold"></param> 
/// <param name="Manufacture_cash"></param> 
/// <param name="SummonCompanion_ID"></param> 
/// <param name="Next_itemID"></param> 
/// <param name="Next_item_price"></param> 
/// <param name="Next_Item_material"></param> 
/// <param name="Next_Item_material_quantity">젬의 경우 전체 필요한 수량을 여기에 적는다상위 젬을 만들기 위해 하위 젬이 4개 필요한 경우 4라고 기재</param> 
/// <param name="Resource_Path"></param> 
/// <param name="WeaponName"></param> 
/// <param name="WeaponIndex"></param> 
/// <param name="PartName"></param> 
/// <param name="PartIndex"></param> 
/// <param name="Icon_path"></param> 
/// <param name="EXP"></param> 
/// <param name="Buy_cost"></param> 
/// <param name="Sell_reward"></param> 
/// <param name="Consignment_maxprice"></param> 
/// <param name="QuestBringer"></param> 
/// <param name="ItemEvent_ID"></param> 
/// <param name="Description"></param> 
/// <param name="Sub_Item">NETEASE-SH:방패등 서브아이템 아이디</param> 
/// <param name="WeaponType">wlfh qkr:0: 맨손1: 왼손무기2: 오른손 무기</param> 
/// <param name="RandomBoxGroup_NO"></param> 
[System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
public partial record Item
(
  int Item_ID
, string Name
, int Item_grade
, int Require_lv
, int Enchant_lv
, int PhysicalAttack
, int PhysicalDefense
, int MagicalAttack
, int MagicalDefense
, float Critical
, int HP
, int KnockBackResist
, eDictionaryType DictionaryType
, int ItemType
, short Gear_Score
, short InventoryType
, bool UsageType
, short Socket_quantity
, int Removal_cost
, short Belonging
, short Sub_stats_quantity
, int Stack
, int DesignScroll_ID
, int BindingSkill_ID
, int BindingAttack_ID
, int Manufacture_gold
, int Manufacture_cash
, int SummonCompanion_ID
, int Next_itemID
, int Next_item_price
, int[] Next_Item_material
, int[] Next_Item_material_quantity
, string Resource_Path
, string WeaponName
, short WeaponIndex
, string[] PartName
, short[] PartIndex
, string Icon_path
, int EXP
, int Buy_cost
, int Sell_reward
, int Consignment_maxprice
, int QuestBringer
, int ItemEvent_ID
, string Description
, int Sub_Item
, int WeaponType
, int[] RandomBoxGroup_NO
);
/// <summary>
/// ItemEffect
/// </summary>
/// <param name="Index"></param> 
/// <param name="Item_ID"></param> 
/// <param name="Effect_type"></param> 
/// <param name="Effect_min"></param> 
/// <param name="Effect_max"></param> 
/// <param name="Time_type"></param> 
/// <param name="Time_rate"></param> 
/// <param name="Time"></param> 
/// <param name="Duration"></param> 
/// <param name="Description"></param> 
[System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
public partial record ItemEffect
(
  int Index
, int Item_ID
, int Effect_type
, float Effect_min
, float Effect_max
, int Time_type
, float Time_rate
, float Time
, float Duration
, string Description
);
/// <summary>
/// ItemEnchant
/// </summary>
/// <param name="Index"></param> 
/// <param name="Item_ID"></param> 
/// <param name="Enchant_lv"></param> 
/// <param name="Physical_attack"></param> 
/// <param name="Physical_defense"></param> 
/// <param name="Magic_attack"></param> 
/// <param name="Magic_defense"></param> 
/// <param name="Critical"></param> 
/// <param name="HP"></param> 
/// <param name="KnockBack_resist"></param> 
/// <param name="Material_IDS"></param> 
/// <param name="Material_quantitys"></param> 
/// <param name="Require_gold"></param> 
/// <param name="Require_cash"></param> 
[System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
public partial record ItemEnchant
(
  int Index
, int Item_ID
, int Enchant_lv
, int Physical_attack
, int Physical_defense
, int Magic_attack
, int Magic_defense
, float Critical
, int HP
, int KnockBack_resist
, int[] Material_IDS
, int[] Material_quantitys
, int Require_gold
, int Require_cash
);
/// <summary>
/// ItemManufacture
/// </summary>
/// <param name="Index"></param> 
/// <param name="Subject_item_ID"></param> 
/// <param name="Material_item_ID"></param> 
/// <param name="Material_quantity"></param> 
[System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
public partial record ItemManufacture
(
  int Index
, int Subject_item_ID
, int Material_item_ID
, int Material_quantity
);
/// <summary>
/// RandomBoxGroup
/// </summary>
/// <param name="ID"></param> 
/// <param name="RandomItemGroup_NO"></param> 
/// <param name="ClassType"></param> 
/// <param name="Item_ID"></param> 
/// <param name="RatioAmount"></param> 
/// <param name="Item_Quantity"></param> 
[System.CodeDom.Compiler.GeneratedCode("TableGenerateCmd","1.0.0")]
public partial record RandomBoxGroup
(
  int ID
, int RandomItemGroup_NO
, int ClassType
, int Item_ID
, int RatioAmount
, int Item_Quantity
);
