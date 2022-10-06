// generate ItemTable.cpp
// DO NOT TOUCH SOURCE....
#include "ItemTable.h"
using namespace TBL::ItemTable;
const FItem::Array FItem::array;
const FItem::Map FItem::map;
FItem::FItem(void)
{
}
FItem& FItem::operator=(const FItem& rhs)
{
    const_cast<int32&>(Item_ID)=rhs.Item_ID;
    const_cast<FString&>(Name)=rhs.Name;
    const_cast<int32&>(Item_grade)=rhs.Item_grade;
    const_cast<int32&>(Require_lv)=rhs.Require_lv;
    const_cast<int32&>(Enchant_lv)=rhs.Enchant_lv;
    const_cast<int32&>(PhysicalAttack)=rhs.PhysicalAttack;
    const_cast<int32&>(PhysicalDefense)=rhs.PhysicalDefense;
    const_cast<int32&>(MagicalAttack)=rhs.MagicalAttack;
    const_cast<int32&>(MagicalDefense)=rhs.MagicalDefense;
    const_cast<float&>(Critical)=rhs.Critical;
    const_cast<int32&>(HP)=rhs.HP;
    const_cast<int32&>(KnockBackResist)=rhs.KnockBackResist;
    const_cast<int32&>(DictionaryType)=rhs.DictionaryType;
    const_cast<int32&>(ItemType)=rhs.ItemType;
    const_cast<int16&>(Gear_Score)=rhs.Gear_Score;
    const_cast<int16&>(InventoryType)=rhs.InventoryType;
    const_cast<bool&>(UsageType)=rhs.UsageType;
    const_cast<int16&>(Socket_quantity)=rhs.Socket_quantity;
    const_cast<int32&>(Removal_cost)=rhs.Removal_cost;
    const_cast<int16&>(Belonging)=rhs.Belonging;
    const_cast<int16&>(Sub_stats_quantity)=rhs.Sub_stats_quantity;
    const_cast<int32&>(Stack)=rhs.Stack;
    const_cast<int32&>(DesignScroll_ID)=rhs.DesignScroll_ID;
    const_cast<int32&>(BindingSkill_ID)=rhs.BindingSkill_ID;
    const_cast<int32&>(BindingAttack_ID)=rhs.BindingAttack_ID;
    const_cast<int32&>(Manufacture_gold)=rhs.Manufacture_gold;
    const_cast<int32&>(Manufacture_cash)=rhs.Manufacture_cash;
    const_cast<int32&>(SummonCompanion_ID)=rhs.SummonCompanion_ID;
    const_cast<int32&>(Next_itemID)=rhs.Next_itemID;
    const_cast<int32&>(Next_item_price)=rhs.Next_item_price;
    const_cast<TArray<int32>&>(Next_Item_material)=rhs.Next_Item_material;
    const_cast<TArray<int32>&>(Next_Item_material_quantity)=rhs.Next_Item_material_quantity;
    const_cast<FString&>(Resource_Path)=rhs.Resource_Path;
    const_cast<FString&>(WeaponName)=rhs.WeaponName;
    const_cast<int16&>(WeaponIndex)=rhs.WeaponIndex;
    const_cast<TArray<FString>&>(PartName)=rhs.PartName;
    const_cast<TArray<int16>&>(PartIndex)=rhs.PartIndex;
    const_cast<FString&>(Icon_path)=rhs.Icon_path;
    const_cast<int32&>(EXP)=rhs.EXP;
    const_cast<int32&>(Buy_cost)=rhs.Buy_cost;
    const_cast<int32&>(Sell_reward)=rhs.Sell_reward;
    const_cast<int32&>(Consignment_maxprice)=rhs.Consignment_maxprice;
    const_cast<int32&>(QuestBringer)=rhs.QuestBringer;
    const_cast<int32&>(ItemEvent_ID)=rhs.ItemEvent_ID;
    const_cast<FString&>(Description)=rhs.Description;
    const_cast<int32&>(Sub_Item)=rhs.Sub_Item;
    const_cast<int32&>(WeaponType)=rhs.WeaponType;
    const_cast<TArray<int32>&>(RandomBoxGroup_NO)=rhs.RandomBoxGroup_NO;
    return *this;
}
FItem::FItem (const int32& Item_ID__
,const FString& Name__
,const int32& Item_grade__
,const int32& Require_lv__
,const int32& Enchant_lv__
,const int32& PhysicalAttack__
,const int32& PhysicalDefense__
,const int32& MagicalAttack__
,const int32& MagicalDefense__
,const float& Critical__
,const int32& HP__
,const int32& KnockBackResist__
,const int32& DictionaryType__
,const int32& ItemType__
,const int16& Gear_Score__
,const int16& InventoryType__
,const bool& UsageType__
,const int16& Socket_quantity__
,const int32& Removal_cost__
,const int16& Belonging__
,const int16& Sub_stats_quantity__
,const int32& Stack__
,const int32& DesignScroll_ID__
,const int32& BindingSkill_ID__
,const int32& BindingAttack_ID__
,const int32& Manufacture_gold__
,const int32& Manufacture_cash__
,const int32& SummonCompanion_ID__
,const int32& Next_itemID__
,const int32& Next_item_price__
,const TArray<int32>& Next_Item_material__
,const TArray<int32>& Next_Item_material_quantity__
,const FString& Resource_Path__
,const FString& WeaponName__
,const int16& WeaponIndex__
,const TArray<FString>& PartName__
,const TArray<int16>& PartIndex__
,const FString& Icon_path__
,const int32& EXP__
,const int32& Buy_cost__
,const int32& Sell_reward__
,const int32& Consignment_maxprice__
,const int32& QuestBringer__
,const int32& ItemEvent_ID__
,const FString& Description__
,const int32& Sub_Item__
,const int32& WeaponType__
,const TArray<int32>& RandomBoxGroup_NO__)
:Item_ID(Item_ID__)
,Name(Name__)
,Item_grade(Item_grade__)
,Require_lv(Require_lv__)
,Enchant_lv(Enchant_lv__)
,PhysicalAttack(PhysicalAttack__)
,PhysicalDefense(PhysicalDefense__)
,MagicalAttack(MagicalAttack__)
,MagicalDefense(MagicalDefense__)
,Critical(Critical__)
,HP(HP__)
,KnockBackResist(KnockBackResist__)
,DictionaryType(DictionaryType__)
,ItemType(ItemType__)
,Gear_Score(Gear_Score__)
,InventoryType(InventoryType__)
,UsageType(UsageType__)
,Socket_quantity(Socket_quantity__)
,Removal_cost(Removal_cost__)
,Belonging(Belonging__)
,Sub_stats_quantity(Sub_stats_quantity__)
,Stack(Stack__)
,DesignScroll_ID(DesignScroll_ID__)
,BindingSkill_ID(BindingSkill_ID__)
,BindingAttack_ID(BindingAttack_ID__)
,Manufacture_gold(Manufacture_gold__)
,Manufacture_cash(Manufacture_cash__)
,SummonCompanion_ID(SummonCompanion_ID__)
,Next_itemID(Next_itemID__)
,Next_item_price(Next_item_price__)
,Next_Item_material(Next_Item_material__)
,Next_Item_material_quantity(Next_Item_material_quantity__)
,Resource_Path(Resource_Path__)
,WeaponName(WeaponName__)
,WeaponIndex(WeaponIndex__)
,PartName(PartName__)
,PartIndex(PartIndex__)
,Icon_path(Icon_path__)
,EXP(EXP__)
,Buy_cost(Buy_cost__)
,Sell_reward(Sell_reward__)
,Consignment_maxprice(Consignment_maxprice__)
,QuestBringer(QuestBringer__)
,ItemEvent_ID(ItemEvent_ID__)
,Description(Description__)
,Sub_Item(Sub_Item__)
,WeaponType(WeaponType__)
,RandomBoxGroup_NO(RandomBoxGroup_NO__)
{
}
/*FItem FItem::Find(const int32& Item_ID)
{
    return FItemPtr(map.Find(Item_ID));
}*/
const FItemEffect::Array FItemEffect::array;
const FItemEffect::Map FItemEffect::map;
FItemEffect::FItemEffect(void)
{
}
FItemEffect& FItemEffect::operator=(const FItemEffect& rhs)
{
    const_cast<int32&>(Index)=rhs.Index;
    const_cast<int32&>(Item_ID)=rhs.Item_ID;
    const_cast<int32&>(Effect_type)=rhs.Effect_type;
    const_cast<float&>(Effect_min)=rhs.Effect_min;
    const_cast<float&>(Effect_max)=rhs.Effect_max;
    const_cast<int32&>(Time_type)=rhs.Time_type;
    const_cast<float&>(Time_rate)=rhs.Time_rate;
    const_cast<float&>(Time)=rhs.Time;
    const_cast<float&>(Duration)=rhs.Duration;
    const_cast<FString&>(Description)=rhs.Description;
    return *this;
}
FItemEffect::FItemEffect (const int32& Index__
,const int32& Item_ID__
,const int32& Effect_type__
,const float& Effect_min__
,const float& Effect_max__
,const int32& Time_type__
,const float& Time_rate__
,const float& Time__
,const float& Duration__
,const FString& Description__)
:Index(Index__)
,Item_ID(Item_ID__)
,Effect_type(Effect_type__)
,Effect_min(Effect_min__)
,Effect_max(Effect_max__)
,Time_type(Time_type__)
,Time_rate(Time_rate__)
,Time(Time__)
,Duration(Duration__)
,Description(Description__)
{
}
/*FItemEffect FItemEffect::Find(const int32& Index)
{
    return FItemEffectPtr(map.Find(Index));
}*/
const FItemEnchant::Array FItemEnchant::array;
const FItemEnchant::Map FItemEnchant::map;
FItemEnchant::FItemEnchant(void)
{
}
FItemEnchant& FItemEnchant::operator=(const FItemEnchant& rhs)
{
    const_cast<int32&>(Index)=rhs.Index;
    const_cast<int32&>(Item_ID)=rhs.Item_ID;
    const_cast<int32&>(Enchant_lv)=rhs.Enchant_lv;
    const_cast<int32&>(Physical_attack)=rhs.Physical_attack;
    const_cast<int32&>(Physical_defense)=rhs.Physical_defense;
    const_cast<int32&>(Magic_attack)=rhs.Magic_attack;
    const_cast<int32&>(Magic_defense)=rhs.Magic_defense;
    const_cast<float&>(Critical)=rhs.Critical;
    const_cast<int32&>(HP)=rhs.HP;
    const_cast<int32&>(KnockBack_resist)=rhs.KnockBack_resist;
    const_cast<TArray<int32>&>(Material_IDS)=rhs.Material_IDS;
    const_cast<TArray<int32>&>(Material_quantitys)=rhs.Material_quantitys;
    const_cast<int32&>(Require_gold)=rhs.Require_gold;
    const_cast<int32&>(Require_cash)=rhs.Require_cash;
    return *this;
}
FItemEnchant::FItemEnchant (const int32& Index__
,const int32& Item_ID__
,const int32& Enchant_lv__
,const int32& Physical_attack__
,const int32& Physical_defense__
,const int32& Magic_attack__
,const int32& Magic_defense__
,const float& Critical__
,const int32& HP__
,const int32& KnockBack_resist__
,const TArray<int32>& Material_IDS__
,const TArray<int32>& Material_quantitys__
,const int32& Require_gold__
,const int32& Require_cash__)
:Index(Index__)
,Item_ID(Item_ID__)
,Enchant_lv(Enchant_lv__)
,Physical_attack(Physical_attack__)
,Physical_defense(Physical_defense__)
,Magic_attack(Magic_attack__)
,Magic_defense(Magic_defense__)
,Critical(Critical__)
,HP(HP__)
,KnockBack_resist(KnockBack_resist__)
,Material_IDS(Material_IDS__)
,Material_quantitys(Material_quantitys__)
,Require_gold(Require_gold__)
,Require_cash(Require_cash__)
{
}
/*FItemEnchant FItemEnchant::Find(const int32& Index)
{
    return FItemEnchantPtr(map.Find(Index));
}*/
const FItemManufacture::Array FItemManufacture::array;
const FItemManufacture::Map FItemManufacture::map;
FItemManufacture::FItemManufacture(void)
{
}
FItemManufacture& FItemManufacture::operator=(const FItemManufacture& rhs)
{
    const_cast<int32&>(Index)=rhs.Index;
    const_cast<int32&>(Subject_item_ID)=rhs.Subject_item_ID;
    const_cast<int32&>(Material_item_ID)=rhs.Material_item_ID;
    const_cast<int32&>(Material_quantity)=rhs.Material_quantity;
    return *this;
}
FItemManufacture::FItemManufacture (const int32& Index__
,const int32& Subject_item_ID__
,const int32& Material_item_ID__
,const int32& Material_quantity__)
:Index(Index__)
,Subject_item_ID(Subject_item_ID__)
,Material_item_ID(Material_item_ID__)
,Material_quantity(Material_quantity__)
{
}
/*FItemManufacture FItemManufacture::Find(const int32& Index)
{
    return FItemManufacturePtr(map.Find(Index));
}*/
const FRandomBoxGroup::Array FRandomBoxGroup::array;
const FRandomBoxGroup::Map FRandomBoxGroup::map;
FRandomBoxGroup::FRandomBoxGroup(void)
{
}
FRandomBoxGroup& FRandomBoxGroup::operator=(const FRandomBoxGroup& rhs)
{
    const_cast<int32&>(ID)=rhs.ID;
    const_cast<int32&>(RandomItemGroup_NO)=rhs.RandomItemGroup_NO;
    const_cast<int32&>(ClassType)=rhs.ClassType;
    const_cast<int32&>(Item_ID)=rhs.Item_ID;
    const_cast<int32&>(RatioAmount)=rhs.RatioAmount;
    const_cast<int32&>(Item_Quantity)=rhs.Item_Quantity;
    return *this;
}
FRandomBoxGroup::FRandomBoxGroup (const int32& ID__
,const int32& RandomItemGroup_NO__
,const int32& ClassType__
,const int32& Item_ID__
,const int32& RatioAmount__
,const int32& Item_Quantity__)
:ID(ID__)
,RandomItemGroup_NO(RandomItemGroup_NO__)
,ClassType(ClassType__)
,Item_ID(Item_ID__)
,RatioAmount(RatioAmount__)
,Item_Quantity(Item_Quantity__)
{
}
/*FRandomBoxGroup FRandomBoxGroup::Find(const int32& ID)
{
    return FRandomBoxGroupPtr(map.Find(ID));
}*/
