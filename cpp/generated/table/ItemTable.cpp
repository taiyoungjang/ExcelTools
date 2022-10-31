// generate ItemTable.cpp
// DO NOT TOUCH SOURCE....
#include "ItemTable.h"
using namespace TBL::ItemTable;
const FItem::FArray FItem::Array_;
const FItem::FMap FItem::Map_;
FItem::FItem(void)
{
}
FItem& FItem::operator=(const FItem& RHS)
{
    const_cast<int32&>(Item_ID)=RHS.Item_ID;
    const_cast<FString&>(Name)=RHS.Name;
    const_cast<int32&>(Item_grade)=RHS.Item_grade;
    const_cast<int32&>(Require_lv)=RHS.Require_lv;
    const_cast<int32&>(Enchant_lv)=RHS.Enchant_lv;
    const_cast<int32&>(PhysicalAttack)=RHS.PhysicalAttack;
    const_cast<int32&>(PhysicalDefense)=RHS.PhysicalDefense;
    const_cast<int32&>(MagicalAttack)=RHS.MagicalAttack;
    const_cast<int32&>(MagicalDefense)=RHS.MagicalDefense;
    const_cast<float&>(Critical)=RHS.Critical;
    const_cast<int32&>(HP)=RHS.HP;
    const_cast<int32&>(KnockBackResist)=RHS.KnockBackResist;
    const_cast<::eDictionaryType&>(DictionaryType)=RHS.DictionaryType;
    const_cast<int32&>(ItemType)=RHS.ItemType;
    const_cast<int16&>(Gear_Score)=RHS.Gear_Score;
    const_cast<int16&>(InventoryType)=RHS.InventoryType;
    const_cast<bool&>(UsageType)=RHS.UsageType;
    const_cast<int16&>(Socket_quantity)=RHS.Socket_quantity;
    const_cast<int32&>(Removal_cost)=RHS.Removal_cost;
    const_cast<int16&>(Belonging)=RHS.Belonging;
    const_cast<int16&>(Sub_stats_quantity)=RHS.Sub_stats_quantity;
    const_cast<int32&>(Stack)=RHS.Stack;
    const_cast<int32&>(DesignScroll_ID)=RHS.DesignScroll_ID;
    const_cast<int32&>(BindingSkill_ID)=RHS.BindingSkill_ID;
    const_cast<int32&>(BindingAttack_ID)=RHS.BindingAttack_ID;
    const_cast<int32&>(Manufacture_gold)=RHS.Manufacture_gold;
    const_cast<int32&>(Manufacture_cash)=RHS.Manufacture_cash;
    const_cast<int32&>(SummonCompanion_ID)=RHS.SummonCompanion_ID;
    const_cast<int32&>(Next_itemID)=RHS.Next_itemID;
    const_cast<int32&>(Next_item_price)=RHS.Next_item_price;
    const_cast<TArray<int32>&>(Next_Item_material)=RHS.Next_Item_material;
    const_cast<TArray<int32>&>(Next_Item_material_quantity)=RHS.Next_Item_material_quantity;
    const_cast<FString&>(Resource_Path)=RHS.Resource_Path;
    const_cast<FString&>(WeaponName)=RHS.WeaponName;
    const_cast<int16&>(WeaponIndex)=RHS.WeaponIndex;
    const_cast<TArray<FString>&>(PartName)=RHS.PartName;
    const_cast<TArray<int16>&>(PartIndex)=RHS.PartIndex;
    const_cast<FString&>(Icon_path)=RHS.Icon_path;
    const_cast<int32&>(EXP)=RHS.EXP;
    const_cast<int32&>(Buy_cost)=RHS.Buy_cost;
    const_cast<int32&>(Sell_reward)=RHS.Sell_reward;
    const_cast<int32&>(Consignment_maxprice)=RHS.Consignment_maxprice;
    const_cast<int32&>(QuestBringer)=RHS.QuestBringer;
    const_cast<int32&>(ItemEvent_ID)=RHS.ItemEvent_ID;
    const_cast<FString&>(Description)=RHS.Description;
    const_cast<int32&>(Sub_Item)=RHS.Sub_Item;
    const_cast<int32&>(WeaponType)=RHS.WeaponType;
    const_cast<TArray<int32>&>(RandomBoxGroup_NO)=RHS.RandomBoxGroup_NO;
    return *this;
}
FItem::FItem (const int32& Item_ID
,const FString& Name
,const int32& Item_grade
,const int32& Require_lv
,const int32& Enchant_lv
,const int32& PhysicalAttack
,const int32& PhysicalDefense
,const int32& MagicalAttack
,const int32& MagicalDefense
,const float& Critical
,const int32& HP
,const int32& KnockBackResist
,const ::eDictionaryType& DictionaryType
,const int32& ItemType
,const int16& Gear_Score
,const int16& InventoryType
,const bool& UsageType
,const int16& Socket_quantity
,const int32& Removal_cost
,const int16& Belonging
,const int16& Sub_stats_quantity
,const int32& Stack
,const int32& DesignScroll_ID
,const int32& BindingSkill_ID
,const int32& BindingAttack_ID
,const int32& Manufacture_gold
,const int32& Manufacture_cash
,const int32& SummonCompanion_ID
,const int32& Next_itemID
,const int32& Next_item_price
,const TArray<int32>& Next_Item_material
,const TArray<int32>& Next_Item_material_quantity
,const FString& Resource_Path
,const FString& WeaponName
,const int16& WeaponIndex
,const TArray<FString>& PartName
,const TArray<int16>& PartIndex
,const FString& Icon_path
,const int32& EXP
,const int32& Buy_cost
,const int32& Sell_reward
,const int32& Consignment_maxprice
,const int32& QuestBringer
,const int32& ItemEvent_ID
,const FString& Description
,const int32& Sub_Item
,const int32& WeaponType
,const TArray<int32>& RandomBoxGroup_NO)
:Item_ID(Item_ID)
,Name(Name)
,Item_grade(Item_grade)
,Require_lv(Require_lv)
,Enchant_lv(Enchant_lv)
,PhysicalAttack(PhysicalAttack)
,PhysicalDefense(PhysicalDefense)
,MagicalAttack(MagicalAttack)
,MagicalDefense(MagicalDefense)
,Critical(Critical)
,HP(HP)
,KnockBackResist(KnockBackResist)
,DictionaryType(DictionaryType)
,ItemType(ItemType)
,Gear_Score(Gear_Score)
,InventoryType(InventoryType)
,UsageType(UsageType)
,Socket_quantity(Socket_quantity)
,Removal_cost(Removal_cost)
,Belonging(Belonging)
,Sub_stats_quantity(Sub_stats_quantity)
,Stack(Stack)
,DesignScroll_ID(DesignScroll_ID)
,BindingSkill_ID(BindingSkill_ID)
,BindingAttack_ID(BindingAttack_ID)
,Manufacture_gold(Manufacture_gold)
,Manufacture_cash(Manufacture_cash)
,SummonCompanion_ID(SummonCompanion_ID)
,Next_itemID(Next_itemID)
,Next_item_price(Next_item_price)
,Next_Item_material(Next_Item_material)
,Next_Item_material_quantity(Next_Item_material_quantity)
,Resource_Path(Resource_Path)
,WeaponName(WeaponName)
,WeaponIndex(WeaponIndex)
,PartName(PartName)
,PartIndex(PartIndex)
,Icon_path(Icon_path)
,EXP(EXP)
,Buy_cost(Buy_cost)
,Sell_reward(Sell_reward)
,Consignment_maxprice(Consignment_maxprice)
,QuestBringer(QuestBringer)
,ItemEvent_ID(ItemEvent_ID)
,Description(Description)
,Sub_Item(Sub_Item)
,WeaponType(WeaponType)
,RandomBoxGroup_NO(RandomBoxGroup_NO)
{
}
/*FItem FItem::Find(const int32& Item_ID)
{
    return FItemPtr(map.Find(Item_ID));
}*/
const FItemEffect::FArray FItemEffect::Array_;
const FItemEffect::FMap FItemEffect::Map_;
FItemEffect::FItemEffect(void)
{
}
FItemEffect& FItemEffect::operator=(const FItemEffect& RHS)
{
    const_cast<int32&>(Index)=RHS.Index;
    const_cast<int32&>(Item_ID)=RHS.Item_ID;
    const_cast<int32&>(Effect_type)=RHS.Effect_type;
    const_cast<float&>(Effect_min)=RHS.Effect_min;
    const_cast<float&>(Effect_max)=RHS.Effect_max;
    const_cast<int32&>(Time_type)=RHS.Time_type;
    const_cast<float&>(Time_rate)=RHS.Time_rate;
    const_cast<float&>(Time)=RHS.Time;
    const_cast<float&>(Duration)=RHS.Duration;
    const_cast<FString&>(Description)=RHS.Description;
    return *this;
}
FItemEffect::FItemEffect (const int32& Index
,const int32& Item_ID
,const int32& Effect_type
,const float& Effect_min
,const float& Effect_max
,const int32& Time_type
,const float& Time_rate
,const float& Time
,const float& Duration
,const FString& Description)
:Index(Index)
,Item_ID(Item_ID)
,Effect_type(Effect_type)
,Effect_min(Effect_min)
,Effect_max(Effect_max)
,Time_type(Time_type)
,Time_rate(Time_rate)
,Time(Time)
,Duration(Duration)
,Description(Description)
{
}
/*FItemEffect FItemEffect::Find(const int32& Index)
{
    return FItemEffectPtr(map.Find(Index));
}*/
const FItemEnchant::FArray FItemEnchant::Array_;
const FItemEnchant::FMap FItemEnchant::Map_;
FItemEnchant::FItemEnchant(void)
{
}
FItemEnchant& FItemEnchant::operator=(const FItemEnchant& RHS)
{
    const_cast<int32&>(Index)=RHS.Index;
    const_cast<int32&>(Item_ID)=RHS.Item_ID;
    const_cast<int32&>(Enchant_lv)=RHS.Enchant_lv;
    const_cast<int32&>(Physical_attack)=RHS.Physical_attack;
    const_cast<int32&>(Physical_defense)=RHS.Physical_defense;
    const_cast<int32&>(Magic_attack)=RHS.Magic_attack;
    const_cast<int32&>(Magic_defense)=RHS.Magic_defense;
    const_cast<float&>(Critical)=RHS.Critical;
    const_cast<int32&>(HP)=RHS.HP;
    const_cast<int32&>(KnockBack_resist)=RHS.KnockBack_resist;
    const_cast<TArray<int32>&>(Material_IDS)=RHS.Material_IDS;
    const_cast<TArray<int32>&>(Material_quantitys)=RHS.Material_quantitys;
    const_cast<int32&>(Require_gold)=RHS.Require_gold;
    const_cast<int32&>(Require_cash)=RHS.Require_cash;
    return *this;
}
FItemEnchant::FItemEnchant (const int32& Index
,const int32& Item_ID
,const int32& Enchant_lv
,const int32& Physical_attack
,const int32& Physical_defense
,const int32& Magic_attack
,const int32& Magic_defense
,const float& Critical
,const int32& HP
,const int32& KnockBack_resist
,const TArray<int32>& Material_IDS
,const TArray<int32>& Material_quantitys
,const int32& Require_gold
,const int32& Require_cash)
:Index(Index)
,Item_ID(Item_ID)
,Enchant_lv(Enchant_lv)
,Physical_attack(Physical_attack)
,Physical_defense(Physical_defense)
,Magic_attack(Magic_attack)
,Magic_defense(Magic_defense)
,Critical(Critical)
,HP(HP)
,KnockBack_resist(KnockBack_resist)
,Material_IDS(Material_IDS)
,Material_quantitys(Material_quantitys)
,Require_gold(Require_gold)
,Require_cash(Require_cash)
{
}
/*FItemEnchant FItemEnchant::Find(const int32& Index)
{
    return FItemEnchantPtr(map.Find(Index));
}*/
const FItemManufacture::FArray FItemManufacture::Array_;
const FItemManufacture::FMap FItemManufacture::Map_;
FItemManufacture::FItemManufacture(void)
{
}
FItemManufacture& FItemManufacture::operator=(const FItemManufacture& RHS)
{
    const_cast<int32&>(Index)=RHS.Index;
    const_cast<int32&>(Subject_item_ID)=RHS.Subject_item_ID;
    const_cast<int32&>(Material_item_ID)=RHS.Material_item_ID;
    const_cast<int32&>(Material_quantity)=RHS.Material_quantity;
    return *this;
}
FItemManufacture::FItemManufacture (const int32& Index
,const int32& Subject_item_ID
,const int32& Material_item_ID
,const int32& Material_quantity)
:Index(Index)
,Subject_item_ID(Subject_item_ID)
,Material_item_ID(Material_item_ID)
,Material_quantity(Material_quantity)
{
}
/*FItemManufacture FItemManufacture::Find(const int32& Index)
{
    return FItemManufacturePtr(map.Find(Index));
}*/
const FRandomBoxGroup::FArray FRandomBoxGroup::Array_;
const FRandomBoxGroup::FMap FRandomBoxGroup::Map_;
FRandomBoxGroup::FRandomBoxGroup(void)
{
}
FRandomBoxGroup& FRandomBoxGroup::operator=(const FRandomBoxGroup& RHS)
{
    const_cast<int32&>(ID)=RHS.ID;
    const_cast<int32&>(RandomItemGroup_NO)=RHS.RandomItemGroup_NO;
    const_cast<int32&>(ClassType)=RHS.ClassType;
    const_cast<int32&>(Item_ID)=RHS.Item_ID;
    const_cast<int32&>(RatioAmount)=RHS.RatioAmount;
    const_cast<int32&>(Item_Quantity)=RHS.Item_Quantity;
    return *this;
}
FRandomBoxGroup::FRandomBoxGroup (const int32& ID
,const int32& RandomItemGroup_NO
,const int32& ClassType
,const int32& Item_ID
,const int32& RatioAmount
,const int32& Item_Quantity)
:ID(ID)
,RandomItemGroup_NO(RandomItemGroup_NO)
,ClassType(ClassType)
,Item_ID(Item_ID)
,RatioAmount(RatioAmount)
,Item_Quantity(Item_Quantity)
{
}
/*FRandomBoxGroup FRandomBoxGroup::Find(const int32& ID)
{
    return FRandomBoxGroupPtr(map.Find(ID));
}*/
