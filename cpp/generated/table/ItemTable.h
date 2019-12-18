// generate ItemTable.h
// DO NOT TOUCH SOURCE....
#ifndef ITEMTABLE_H
#define ITEMTABLE_H
#include <memory>
#include <string>
#include <vector>
#include <map>
namespace TBL
{
    namespace ItemTable
    {
        class Item;
        typedef std::shared_ptr<Item> ItemPtr;
        class Item
        {
            public:
            static ItemPtr Find( const int& Item_ID);
            typedef std::vector<ItemPtr> Array;
            typedef std::map<int,ItemPtr> Map;
            static const Array array;
            static const Map map;
            
            const int Item_ID;
            const std::wstring Name;
            const int Item_grade;
            const int Require_lv;
            const int Enchant_lv;
            const int PhysicalAttack;
            const int PhysicalDefense;
            const int MagicalAttack;
            const int MagicalDefense;
            const float Critical;
            const int HP;
            const int KnockBackResist;
            const short DictionaryType;
            const int ItemType;
            const short Gear_Score;
            const short InventoryType;
            const bool UsageType;
            const short Socket_quantity;
            const int Removal_cost;
            const short Belonging;
            const short Sub_stats_quantity;
            const int Stack;
            const int DesignScroll_ID;
            const int BindingSkill_ID;
            const int BindingAttack_ID;
            const int Manufacture_gold;
            const int Manufacture_cash;
            const int SummonCompanion_ID;
            const int Next_itemID;
            const int Next_item_price;
            const std::vector<int> Next_Item_material;
            const std::vector<int> Next_Item_material_quantity;
            const std::wstring Resource_Path;
            const std::wstring WeaponName;
            const short WeaponIndex;
            const std::vector<std::wstring> PartName;
            const std::vector<short> PartIndex;
            const std::wstring Icon_path;
            const int EXP;
            const int Buy_cost;
            const int Sell_reward;
            const int Consignment_maxprice;
            const int QuestBringer;
            const int ItemEvent_ID;
            const std::wstring Description;
            const int Sub_Item;
            const int WeaponType;
            const std::vector<int> RandomBoxGroup_NO;
            Item (const int& Item_ID__,const std::wstring& Name__,const int& Item_grade__,const int& Require_lv__,const int& Enchant_lv__,const int& PhysicalAttack__,const int& PhysicalDefense__,const int& MagicalAttack__,const int& MagicalDefense__,const float& Critical__,const int& HP__,const int& KnockBackResist__,const short& DictionaryType__,const int& ItemType__,const short& Gear_Score__,const short& InventoryType__,const bool& UsageType__,const short& Socket_quantity__,const int& Removal_cost__,const short& Belonging__,const short& Sub_stats_quantity__,const int& Stack__,const int& DesignScroll_ID__,const int& BindingSkill_ID__,const int& BindingAttack_ID__,const int& Manufacture_gold__,const int& Manufacture_cash__,const int& SummonCompanion_ID__,const int& Next_itemID__,const int& Next_item_price__,const std::vector<int>& Next_Item_material__,const std::vector<int>& Next_Item_material_quantity__,const std::wstring& Resource_Path__,const std::wstring& WeaponName__,const short& WeaponIndex__,const std::vector<std::wstring>& PartName__,const std::vector<short>& PartIndex__,const std::wstring& Icon_path__,const int& EXP__,const int& Buy_cost__,const int& Sell_reward__,const int& Consignment_maxprice__,const int& QuestBringer__,const int& ItemEvent_ID__,const std::wstring& Description__,const int& Sub_Item__,const int& WeaponType__,const std::vector<int>& RandomBoxGroup_NO__);
        };
        class ItemEffect;
        typedef std::shared_ptr<ItemEffect> ItemEffectPtr;
        class ItemEffect
        {
            public:
            static ItemEffectPtr Find( const int& Index);
            typedef std::vector<ItemEffectPtr> Array;
            typedef std::map<int,ItemEffectPtr> Map;
            static const Array array;
            static const Map map;
            
            const int Index;
            const int Item_ID;
            const int Effect_type;
            const float Effect_min;
            const float Effect_max;
            const int Time_type;
            const float Time_rate;
            const float Time;
            const float Duration;
            const std::wstring Description;
            ItemEffect (const int& Index__,const int& Item_ID__,const int& Effect_type__,const float& Effect_min__,const float& Effect_max__,const int& Time_type__,const float& Time_rate__,const float& Time__,const float& Duration__,const std::wstring& Description__);
        };
        class ItemEnchant;
        typedef std::shared_ptr<ItemEnchant> ItemEnchantPtr;
        class ItemEnchant
        {
            public:
            static ItemEnchantPtr Find( const int& Index);
            typedef std::vector<ItemEnchantPtr> Array;
            typedef std::map<int,ItemEnchantPtr> Map;
            static const Array array;
            static const Map map;
            
            const int Index;
            const int Item_ID;
            const int Enchant_lv;
            const int Physical_attack;
            const int Physical_defense;
            const int Magic_attack;
            const int Magic_defense;
            const float Critical;
            const int HP;
            const int KnockBack_resist;
            const std::vector<int> Material_IDS;
            const std::vector<int> Material_quantitys;
            const int Require_gold;
            const int Require_cash;
            ItemEnchant (const int& Index__,const int& Item_ID__,const int& Enchant_lv__,const int& Physical_attack__,const int& Physical_defense__,const int& Magic_attack__,const int& Magic_defense__,const float& Critical__,const int& HP__,const int& KnockBack_resist__,const std::vector<int>& Material_IDS__,const std::vector<int>& Material_quantitys__,const int& Require_gold__,const int& Require_cash__);
        };
        class ItemManufacture;
        typedef std::shared_ptr<ItemManufacture> ItemManufacturePtr;
        class ItemManufacture
        {
            public:
            static ItemManufacturePtr Find( const int& Index);
            typedef std::vector<ItemManufacturePtr> Array;
            typedef std::map<int,ItemManufacturePtr> Map;
            static const Array array;
            static const Map map;
            
            const int Index;
            const int Subject_item_ID;
            const int Material_item_ID;
            const int Material_quantity;
            ItemManufacture (const int& Index__,const int& Subject_item_ID__,const int& Material_item_ID__,const int& Material_quantity__);
        };
        class RandomBoxGroup;
        typedef std::shared_ptr<RandomBoxGroup> RandomBoxGroupPtr;
        class RandomBoxGroup
        {
            public:
            static RandomBoxGroupPtr Find( const int& ID);
            typedef std::vector<RandomBoxGroupPtr> Array;
            typedef std::map<int,RandomBoxGroupPtr> Map;
            static const Array array;
            static const Map map;
            
            const int ID;
            const int RandomItemGroup_NO;
            const int ClassType;
            const int Item_ID;
            const int RatioAmount;
            const int Item_Quantity;
            RandomBoxGroup (const int& ID__,const int& RandomItemGroup_NO__,const int& ClassType__,const int& Item_ID__,const int& RatioAmount__,const int& Item_Quantity__);
        };
    };
};
#endif //ITEMTABLE
