#include "ItemTableTableManager.h"
namespace TBL
{
  namespace ItemTable
  {
    class ItemTableManager : public TBL::TableManager
    {
      public:
      bool ItemTableManager::LoadTable(std::istream& stream__) override
      {
        int count__ = 0;
        Read(stream__,count__);
        if(count__ == 0) return true;
        int Item_ID;
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
        int DictionaryType;
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
        std::vector<int> Next_Item_material; Next_Item_material.resize(3);
        std::vector<int> Next_Item_material_quantity; Next_Item_material_quantity.resize(3);
        std::wstring Resource_Path;
        std::wstring WeaponName;
        short WeaponIndex;
        std::vector<std::wstring> PartName; PartName.resize(1);
        std::vector<short> PartIndex; PartIndex.resize(1);
        std::wstring Icon_path;
        int EXP;
        int Buy_cost;
        int Sell_reward;
        int Consignment_maxprice;
        int QuestBringer;
        int ItemEvent_ID;
        int Sub_Item;
        int WeaponType;
        std::vector<int> RandomBoxGroup_NO; RandomBoxGroup_NO.resize(5);
        Item::Array array; array.resize(count__);
        Item::Map map;
        for(int i__=0;i__<count__;++i__)
        {
          Read(stream__, Item_ID);
          Read(stream__, Item_grade);
          Read(stream__, Require_lv);
          Read(stream__, Enchant_lv);
          Read(stream__, PhysicalAttack);
          Read(stream__, PhysicalDefense);
          Read(stream__, MagicalAttack);
          Read(stream__, MagicalDefense);
          Read(stream__, Critical);
          Read(stream__, HP);
          Read(stream__, KnockBackResist);
          Read(stream__, DictionaryType);
          Read(stream__, ItemType);
          Read(stream__, Gear_Score);
          Read(stream__, InventoryType);
          Read(stream__, UsageType);
          Read(stream__, Socket_quantity);
          Read(stream__, Removal_cost);
          Read(stream__, Belonging);
          Read(stream__, Sub_stats_quantity);
          Read(stream__, Stack);
          Read(stream__, DesignScroll_ID);
          Read(stream__, BindingSkill_ID);
          Read(stream__, BindingAttack_ID);
          Read(stream__, Manufacture_gold);
          Read(stream__, Manufacture_cash);
          Read(stream__, SummonCompanion_ID);
          Read(stream__, Next_itemID);
          Read(stream__, Next_item_price);
          {
            int arrayCount__ = Read7BitEncodedInt(stream__);
            for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
            {
              Read(stream__, Next_Item_material[arrayIndex__]);
            }
          }
          {
            int arrayCount__ = Read7BitEncodedInt(stream__);
            for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
            {
              Read(stream__, Next_Item_material_quantity[arrayIndex__]);
            }
          }
          Read(stream__, Resource_Path);
          Read(stream__, WeaponName);
          Read(stream__, WeaponIndex);
          {
            int arrayCount__ = Read7BitEncodedInt(stream__);
            for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
            {
              Read(stream__, PartName[arrayIndex__]);
            }
          }
          {
            int arrayCount__ = Read7BitEncodedInt(stream__);
            for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
            {
              Read(stream__, PartIndex[arrayIndex__]);
            }
          }
          Read(stream__, Icon_path);
          Read(stream__, EXP);
          Read(stream__, Buy_cost);
          Read(stream__, Sell_reward);
          Read(stream__, Consignment_maxprice);
          Read(stream__, QuestBringer);
          Read(stream__, ItemEvent_ID);
          Read(stream__, Sub_Item);
          Read(stream__, WeaponType);
          {
            int arrayCount__ = Read7BitEncodedInt(stream__);
            for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
            {
              Read(stream__, RandomBoxGroup_NO[arrayIndex__]);
            }
          }
          ItemPtr item__ = ItemPtr(new Item(Item_ID,Item_grade,Require_lv,Enchant_lv,PhysicalAttack,PhysicalDefense,MagicalAttack,MagicalDefense,Critical,HP,KnockBackResist,DictionaryType,ItemType,Gear_Score,InventoryType,UsageType,Socket_quantity,Removal_cost,Belonging,Sub_stats_quantity,Stack,DesignScroll_ID,BindingSkill_ID,BindingAttack_ID,Manufacture_gold,Manufacture_cash,SummonCompanion_ID,Next_itemID,Next_item_price,Next_Item_material,Next_Item_material_quantity,Resource_Path,WeaponName,WeaponIndex,PartName,PartIndex,Icon_path,EXP,Buy_cost,Sell_reward,Consignment_maxprice,QuestBringer,ItemEvent_ID,Sub_Item,WeaponType,RandomBoxGroup_NO));
          array[i__] = item__;
          map.insert( std::pair<int,ItemPtr>(Item_ID,item__));
        }
        {
          Item::Array& target = const_cast<Item::Array&>(Item::array);
          target.clear();
          target.resize(count__);
          std::copy(array.begin(),array.end(),target.begin());
        }
        {
          Item::Map& target = const_cast<Item::Map&>(Item::map);
          target.clear();
          target.insert(map.begin(),map.end());
        }
        return true;
      }
    };
    class ItemEffectTableManager : public TBL::TableManager
    {
      public:
      bool ItemEffectTableManager::LoadTable(std::istream& stream__) override
      {
        int count__ = 0;
        Read(stream__,count__);
        if(count__ == 0) return true;
        int Index;
        int Item_ID;
        int Effect_type;
        float Effect_min;
        float Effect_max;
        int Time_type;
        float Time_rate;
        float Time;
        float Duration;
        ItemEffect::Array array; array.resize(count__);
        ItemEffect::Map map;
        for(int i__=0;i__<count__;++i__)
        {
          Read(stream__, Index);
          Read(stream__, Item_ID);
          Read(stream__, Effect_type);
          Read(stream__, Effect_min);
          Read(stream__, Effect_max);
          Read(stream__, Time_type);
          Read(stream__, Time_rate);
          Read(stream__, Time);
          Read(stream__, Duration);
          ItemEffectPtr item__ = ItemEffectPtr(new ItemEffect(Index,Item_ID,Effect_type,Effect_min,Effect_max,Time_type,Time_rate,Time,Duration));
          array[i__] = item__;
          map.insert( std::pair<int,ItemEffectPtr>(Index,item__));
        }
        {
          ItemEffect::Array& target = const_cast<ItemEffect::Array&>(ItemEffect::array);
          target.clear();
          target.resize(count__);
          std::copy(array.begin(),array.end(),target.begin());
        }
        {
          ItemEffect::Map& target = const_cast<ItemEffect::Map&>(ItemEffect::map);
          target.clear();
          target.insert(map.begin(),map.end());
        }
        return true;
      }
    };
    class ItemEnchantTableManager : public TBL::TableManager
    {
      public:
      bool ItemEnchantTableManager::LoadTable(std::istream& stream__) override
      {
        int count__ = 0;
        Read(stream__,count__);
        if(count__ == 0) return true;
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
        std::vector<int> Material_IDS; Material_IDS.resize(5);
        std::vector<int> Material_quantitys; Material_quantitys.resize(5);
        int Require_gold;
        int Require_cash;
        ItemEnchant::Array array; array.resize(count__);
        ItemEnchant::Map map;
        for(int i__=0;i__<count__;++i__)
        {
          Read(stream__, Index);
          Read(stream__, Item_ID);
          Read(stream__, Enchant_lv);
          Read(stream__, Physical_attack);
          Read(stream__, Physical_defense);
          Read(stream__, Magic_attack);
          Read(stream__, Magic_defense);
          Read(stream__, Critical);
          Read(stream__, HP);
          Read(stream__, KnockBack_resist);
          {
            int arrayCount__ = Read7BitEncodedInt(stream__);
            for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
            {
              Read(stream__, Material_IDS[arrayIndex__]);
            }
          }
          {
            int arrayCount__ = Read7BitEncodedInt(stream__);
            for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
            {
              Read(stream__, Material_quantitys[arrayIndex__]);
            }
          }
          Read(stream__, Require_gold);
          Read(stream__, Require_cash);
          ItemEnchantPtr item__ = ItemEnchantPtr(new ItemEnchant(Index,Item_ID,Enchant_lv,Physical_attack,Physical_defense,Magic_attack,Magic_defense,Critical,HP,KnockBack_resist,Material_IDS,Material_quantitys,Require_gold,Require_cash));
          array[i__] = item__;
          map.insert( std::pair<int,ItemEnchantPtr>(Index,item__));
        }
        {
          ItemEnchant::Array& target = const_cast<ItemEnchant::Array&>(ItemEnchant::array);
          target.clear();
          target.resize(count__);
          std::copy(array.begin(),array.end(),target.begin());
        }
        {
          ItemEnchant::Map& target = const_cast<ItemEnchant::Map&>(ItemEnchant::map);
          target.clear();
          target.insert(map.begin(),map.end());
        }
        return true;
      }
    };
    class ItemManufactureTableManager : public TBL::TableManager
    {
      public:
      bool ItemManufactureTableManager::LoadTable(std::istream& stream__) override
      {
        int count__ = 0;
        Read(stream__,count__);
        if(count__ == 0) return true;
        int Index;
        int Subject_item_ID;
        int Material_item_ID;
        int Material_quantity;
        ItemManufacture::Array array; array.resize(count__);
        ItemManufacture::Map map;
        for(int i__=0;i__<count__;++i__)
        {
          Read(stream__, Index);
          Read(stream__, Subject_item_ID);
          Read(stream__, Material_item_ID);
          Read(stream__, Material_quantity);
          ItemManufacturePtr item__ = ItemManufacturePtr(new ItemManufacture(Index,Subject_item_ID,Material_item_ID,Material_quantity));
          array[i__] = item__;
          map.insert( std::pair<int,ItemManufacturePtr>(Index,item__));
        }
        {
          ItemManufacture::Array& target = const_cast<ItemManufacture::Array&>(ItemManufacture::array);
          target.clear();
          target.resize(count__);
          std::copy(array.begin(),array.end(),target.begin());
        }
        {
          ItemManufacture::Map& target = const_cast<ItemManufacture::Map&>(ItemManufacture::map);
          target.clear();
          target.insert(map.begin(),map.end());
        }
        return true;
      }
    };
    class RandomBoxGroupTableManager : public TBL::TableManager
    {
      public:
      bool RandomBoxGroupTableManager::LoadTable(std::istream& stream__) override
      {
        int count__ = 0;
        Read(stream__,count__);
        if(count__ == 0) return true;
        int ID;
        int RandomItemGroup_NO;
        int ClassType;
        int Item_ID;
        int RatioAmount;
        int Item_Quantity;
        RandomBoxGroup::Array array; array.resize(count__);
        RandomBoxGroup::Map map;
        for(int i__=0;i__<count__;++i__)
        {
          Read(stream__, ID);
          Read(stream__, RandomItemGroup_NO);
          Read(stream__, ClassType);
          Read(stream__, Item_ID);
          Read(stream__, RatioAmount);
          Read(stream__, Item_Quantity);
          RandomBoxGroupPtr item__ = RandomBoxGroupPtr(new RandomBoxGroup(ID,RandomItemGroup_NO,ClassType,Item_ID,RatioAmount,Item_Quantity));
          array[i__] = item__;
          map.insert( std::pair<int,RandomBoxGroupPtr>(ID,item__));
        }
        {
          RandomBoxGroup::Array& target = const_cast<RandomBoxGroup::Array&>(RandomBoxGroup::array);
          target.clear();
          target.resize(count__);
          std::copy(array.begin(),array.end(),target.begin());
        }
        {
          RandomBoxGroup::Map& target = const_cast<RandomBoxGroup::Map&>(RandomBoxGroup::map);
          target.clear();
          target.insert(map.begin(),map.end());
        }
        return true;
      }
    };
    bool TableManager::LoadTable(std::ifstream& stream)
    {
      bool rtn = true;
      std::vector<char> bytes;
      if(TBL::TableManager::Decompress(stream,bytes)==false) return false;
      vectorwrapbuf<char> databuf(bytes);
      std::istream is(&databuf);
      ItemTableManager ItemTableManager;
      ItemEffectTableManager ItemEffectTableManager;
      ItemEnchantTableManager ItemEnchantTableManager;
      ItemManufactureTableManager ItemManufactureTableManager;
      RandomBoxGroupTableManager RandomBoxGroupTableManager;
      
      rtn &= ItemTableManager.LoadTable(is);
      rtn &= ItemEffectTableManager.LoadTable(is);
      rtn &= ItemEnchantTableManager.LoadTable(is);
      rtn &= ItemManufactureTableManager.LoadTable(is);
      rtn &= RandomBoxGroupTableManager.LoadTable(is);
      return rtn;
    };
  };
};
