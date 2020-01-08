// generate ItemTable.cpp
// DO NOT TOUCH SOURCE....
#include "ItemTable.h"
using namespace TBL::ItemTable;
const Item::Array Item::array;
const Item::Map Item::map;
Item::Item (const int& Item_ID__
,const int& Item_grade__
,const int& Require_lv__
,const int& Enchant_lv__
,const int& PhysicalAttack__
,const int& PhysicalDefense__
,const int& MagicalAttack__
,const int& MagicalDefense__
,const float& Critical__
,const int& HP__
,const int& KnockBackResist__
,const int& DictionaryType__
,const int& ItemType__
,const short& Gear_Score__
,const short& InventoryType__
,const bool& UsageType__
,const short& Socket_quantity__
,const int& Removal_cost__
,const short& Belonging__
,const short& Sub_stats_quantity__
,const int& Stack__
,const int& DesignScroll_ID__
,const int& BindingSkill_ID__
,const int& BindingAttack_ID__
,const int& Manufacture_gold__
,const int& Manufacture_cash__
,const int& SummonCompanion_ID__
,const int& Next_itemID__
,const int& Next_item_price__
,const std::vector<int>& Next_Item_material__
,const std::vector<int>& Next_Item_material_quantity__
,const std::wstring& Resource_Path__
,const std::wstring& WeaponName__
,const short& WeaponIndex__
,const std::vector<std::wstring>& PartName__
,const std::vector<short>& PartIndex__
,const std::wstring& Icon_path__
,const int& EXP__
,const int& Buy_cost__
,const int& Sell_reward__
,const int& Consignment_maxprice__
,const int& QuestBringer__
,const int& ItemEvent_ID__
,const int& Sub_Item__
,const int& WeaponType__
,const std::vector<int>& RandomBoxGroup_NO__)
:Item_ID(Item_ID__)
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
,Sub_Item(Sub_Item__)
,WeaponType(WeaponType__)
,RandomBoxGroup_NO(RandomBoxGroup_NO__)
{
}
ItemPtr Item::Find(const int& Item_ID)
{
    auto it = Item::map.find(Item_ID);
    if (it != Item::map.end())
    {
        return it->second;
    }
    return ItemPtr(nullptr);
}
const ItemEffect::Array ItemEffect::array;
const ItemEffect::Map ItemEffect::map;
ItemEffect::ItemEffect (const int& Index__
,const int& Item_ID__
,const int& Effect_type__
,const float& Effect_min__
,const float& Effect_max__
,const int& Time_type__
,const float& Time_rate__
,const float& Time__
,const float& Duration__)
:Index(Index__)
,Item_ID(Item_ID__)
,Effect_type(Effect_type__)
,Effect_min(Effect_min__)
,Effect_max(Effect_max__)
,Time_type(Time_type__)
,Time_rate(Time_rate__)
,Time(Time__)
,Duration(Duration__)
{
}
ItemEffectPtr ItemEffect::Find(const int& Index)
{
    auto it = ItemEffect::map.find(Index);
    if (it != ItemEffect::map.end())
    {
        return it->second;
    }
    return ItemEffectPtr(nullptr);
}
const ItemEnchant::Array ItemEnchant::array;
const ItemEnchant::Map ItemEnchant::map;
ItemEnchant::ItemEnchant (const int& Index__
,const int& Item_ID__
,const int& Enchant_lv__
,const int& Physical_attack__
,const int& Physical_defense__
,const int& Magic_attack__
,const int& Magic_defense__
,const float& Critical__
,const int& HP__
,const int& KnockBack_resist__
,const std::vector<int>& Material_IDS__
,const std::vector<int>& Material_quantitys__
,const int& Require_gold__
,const int& Require_cash__)
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
ItemEnchantPtr ItemEnchant::Find(const int& Index)
{
    auto it = ItemEnchant::map.find(Index);
    if (it != ItemEnchant::map.end())
    {
        return it->second;
    }
    return ItemEnchantPtr(nullptr);
}
const ItemManufacture::Array ItemManufacture::array;
const ItemManufacture::Map ItemManufacture::map;
ItemManufacture::ItemManufacture (const int& Index__
,const int& Subject_item_ID__
,const int& Material_item_ID__
,const int& Material_quantity__)
:Index(Index__)
,Subject_item_ID(Subject_item_ID__)
,Material_item_ID(Material_item_ID__)
,Material_quantity(Material_quantity__)
{
}
ItemManufacturePtr ItemManufacture::Find(const int& Index)
{
    auto it = ItemManufacture::map.find(Index);
    if (it != ItemManufacture::map.end())
    {
        return it->second;
    }
    return ItemManufacturePtr(nullptr);
}
const RandomBoxGroup::Array RandomBoxGroup::array;
const RandomBoxGroup::Map RandomBoxGroup::map;
RandomBoxGroup::RandomBoxGroup (const int& ID__
,const int& RandomItemGroup_NO__
,const int& ClassType__
,const int& Item_ID__
,const int& RatioAmount__
,const int& Item_Quantity__)
:ID(ID__)
,RandomItemGroup_NO(RandomItemGroup_NO__)
,ClassType(ClassType__)
,Item_ID(Item_ID__)
,RatioAmount(RatioAmount__)
,Item_Quantity(Item_Quantity__)
{
}
RandomBoxGroupPtr RandomBoxGroup::Find(const int& ID)
{
    auto it = RandomBoxGroup::map.find(ID);
    if (it != RandomBoxGroup::map.end())
    {
        return it->second;
    }
    return RandomBoxGroupPtr(nullptr);
}
