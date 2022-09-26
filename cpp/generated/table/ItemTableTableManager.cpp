#include "ItemTableTableManager.h"
namespace TBL::ItemTable
{
  class ItemTableManager : public TBL::TableManager
  {
    public:
    ItemTableManager(void){}
    bool LoadTable(BufferReader& stream__) override
    {
      int32 count__ = 0;
      stream__ >> count__;
      if(count__ == 0) return true;
      int32 Item_ID;
      FString Name;
      int32 Item_grade;
      int32 Require_lv;
      int32 Enchant_lv;
      int32 PhysicalAttack;
      int32 PhysicalDefense;
      int32 MagicalAttack;
      int32 MagicalDefense;
      float Critical;
      int32 HP;
      int32 KnockBackResist;
      int32 DictionaryType;
      int32 ItemType;
      int16 Gear_Score;
      int16 InventoryType;
      bool UsageType;
      int16 Socket_quantity;
      int32 Removal_cost;
      int16 Belonging;
      int16 Sub_stats_quantity;
      int32 Stack;
      int32 DesignScroll_ID;
      int32 BindingSkill_ID;
      int32 BindingAttack_ID;
      int32 Manufacture_gold;
      int32 Manufacture_cash;
      int32 SummonCompanion_ID;
      int32 Next_itemID;
      int32 Next_item_price;
      TArray<int32> Next_Item_material;
      TArray<int32> Next_Item_material_quantity;
      FString Resource_Path;
      FString WeaponName;
      int16 WeaponIndex;
      TArray<FString> PartName;
      TArray<int16> PartIndex;
      FString Icon_path;
      int32 EXP;
      int32 Buy_cost;
      int32 Sell_reward;
      int32 Consignment_maxprice;
      int32 QuestBringer;
      int32 ItemEvent_ID;
      FString Description;
      int32 Sub_Item;
      int32 WeaponType;
      TArray<int32> RandomBoxGroup_NO;
      FItem::Array array; array.SetNum(count__,true);
      FItem::Map map;
      for(int i__=0;i__<count__;++i__)
      {
        stream__ >> Item_ID;
        stream__ >> Name;
        stream__ >> Item_grade;
        stream__ >> Require_lv;
        stream__ >> Enchant_lv;
        stream__ >> PhysicalAttack;
        stream__ >> PhysicalDefense;
        stream__ >> MagicalAttack;
        stream__ >> MagicalDefense;
        stream__ >> Critical;
        stream__ >> HP;
        stream__ >> KnockBackResist;
        stream__ >> DictionaryType;
        stream__ >> ItemType;
        stream__ >> Gear_Score;
        stream__ >> InventoryType;
        stream__ >> UsageType;
        stream__ >> Socket_quantity;
        stream__ >> Removal_cost;
        stream__ >> Belonging;
        stream__ >> Sub_stats_quantity;
        stream__ >> Stack;
        stream__ >> DesignScroll_ID;
        stream__ >> BindingSkill_ID;
        stream__ >> BindingAttack_ID;
        stream__ >> Manufacture_gold;
        stream__ >> Manufacture_cash;
        stream__ >> SummonCompanion_ID;
        stream__ >> Next_itemID;
        stream__ >> Next_item_price;
        {
          int arrayCount__ = BufferReader::Read7BitEncodedInt(stream__);
          Next_Item_material.SetNum(arrayCount__,true);
          for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
          {
            stream__ >> Next_Item_material[arrayIndex__];
          }
        }
        {
          int arrayCount__ = BufferReader::Read7BitEncodedInt(stream__);
          Next_Item_material_quantity.SetNum(arrayCount__,true);
          for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
          {
            stream__ >> Next_Item_material_quantity[arrayIndex__];
          }
        }
        stream__ >> Resource_Path;
        stream__ >> WeaponName;
        stream__ >> WeaponIndex;
        {
          int arrayCount__ = BufferReader::Read7BitEncodedInt(stream__);
          PartName.SetNum(arrayCount__,true);
          for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
          {
            stream__ >> PartName[arrayIndex__];
          }
        }
        {
          int arrayCount__ = BufferReader::Read7BitEncodedInt(stream__);
          PartIndex.SetNum(arrayCount__,true);
          for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
          {
            stream__ >> PartIndex[arrayIndex__];
          }
        }
        stream__ >> Icon_path;
        stream__ >> EXP;
        stream__ >> Buy_cost;
        stream__ >> Sell_reward;
        stream__ >> Consignment_maxprice;
        stream__ >> QuestBringer;
        stream__ >> ItemEvent_ID;
        stream__ >> Description;
        stream__ >> Sub_Item;
        stream__ >> WeaponType;
        {
          int arrayCount__ = BufferReader::Read7BitEncodedInt(stream__);
          RandomBoxGroup_NO.SetNum(arrayCount__,true);
          for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
          {
            stream__ >> RandomBoxGroup_NO[arrayIndex__];
          }
        }
        auto item__ = FItem(Item_ID,Name,Item_grade,Require_lv,Enchant_lv,PhysicalAttack,PhysicalDefense,MagicalAttack,MagicalDefense,Critical,HP,KnockBackResist,DictionaryType,ItemType,Gear_Score,InventoryType,UsageType,Socket_quantity,Removal_cost,Belonging,Sub_stats_quantity,Stack,DesignScroll_ID,BindingSkill_ID,BindingAttack_ID,Manufacture_gold,Manufacture_cash,SummonCompanion_ID,Next_itemID,Next_item_price,Next_Item_material,Next_Item_material_quantity,Resource_Path,WeaponName,WeaponIndex,PartName,PartIndex,Icon_path,EXP,Buy_cost,Sell_reward,Consignment_maxprice,QuestBringer,ItemEvent_ID,Description,Sub_Item,WeaponType,RandomBoxGroup_NO);
        array[i__] = item__;
        map.Emplace(Item_ID,item__);
      }
      {
        FItem::Array& target = const_cast<FItem::Array&>(FItem::array);
        target.Reset();
        target.SetNum(count__,true);
        target.Append(array);
      }
      {
        FItem::Map& target = const_cast<FItem::Map&>(FItem::map);
        target.Reset();
        target.Append(map);
      }
      return true;
    }
  };
  class ItemEffectTableManager : public TBL::TableManager
  {
    public:
    ItemEffectTableManager(void){}
    bool LoadTable(BufferReader& stream__) override
    {
      int32 count__ = 0;
      stream__ >> count__;
      if(count__ == 0) return true;
      int32 Index;
      int32 Item_ID;
      int32 Effect_type;
      float Effect_min;
      float Effect_max;
      int32 Time_type;
      float Time_rate;
      float Time;
      float Duration;
      FString Description;
      FItemEffect::Array array; array.SetNum(count__,true);
      FItemEffect::Map map;
      for(int i__=0;i__<count__;++i__)
      {
        stream__ >> Index;
        stream__ >> Item_ID;
        stream__ >> Effect_type;
        stream__ >> Effect_min;
        stream__ >> Effect_max;
        stream__ >> Time_type;
        stream__ >> Time_rate;
        stream__ >> Time;
        stream__ >> Duration;
        stream__ >> Description;
        auto item__ = FItemEffect(Index,Item_ID,Effect_type,Effect_min,Effect_max,Time_type,Time_rate,Time,Duration,Description);
        array[i__] = item__;
        map.Emplace(Index,item__);
      }
      {
        FItemEffect::Array& target = const_cast<FItemEffect::Array&>(FItemEffect::array);
        target.Reset();
        target.SetNum(count__,true);
        target.Append(array);
      }
      {
        FItemEffect::Map& target = const_cast<FItemEffect::Map&>(FItemEffect::map);
        target.Reset();
        target.Append(map);
      }
      return true;
    }
  };
  class ItemEnchantTableManager : public TBL::TableManager
  {
    public:
    ItemEnchantTableManager(void){}
    bool LoadTable(BufferReader& stream__) override
    {
      int32 count__ = 0;
      stream__ >> count__;
      if(count__ == 0) return true;
      int32 Index;
      int32 Item_ID;
      int32 Enchant_lv;
      int32 Physical_attack;
      int32 Physical_defense;
      int32 Magic_attack;
      int32 Magic_defense;
      float Critical;
      int32 HP;
      int32 KnockBack_resist;
      TArray<int32> Material_IDS;
      TArray<int32> Material_quantitys;
      int32 Require_gold;
      int32 Require_cash;
      FItemEnchant::Array array; array.SetNum(count__,true);
      FItemEnchant::Map map;
      for(int i__=0;i__<count__;++i__)
      {
        stream__ >> Index;
        stream__ >> Item_ID;
        stream__ >> Enchant_lv;
        stream__ >> Physical_attack;
        stream__ >> Physical_defense;
        stream__ >> Magic_attack;
        stream__ >> Magic_defense;
        stream__ >> Critical;
        stream__ >> HP;
        stream__ >> KnockBack_resist;
        {
          int arrayCount__ = BufferReader::Read7BitEncodedInt(stream__);
          Material_IDS.SetNum(arrayCount__,true);
          for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
          {
            stream__ >> Material_IDS[arrayIndex__];
          }
        }
        {
          int arrayCount__ = BufferReader::Read7BitEncodedInt(stream__);
          Material_quantitys.SetNum(arrayCount__,true);
          for(int arrayIndex__=0;arrayIndex__<arrayCount__;++arrayIndex__)
          {
            stream__ >> Material_quantitys[arrayIndex__];
          }
        }
        stream__ >> Require_gold;
        stream__ >> Require_cash;
        auto item__ = FItemEnchant(Index,Item_ID,Enchant_lv,Physical_attack,Physical_defense,Magic_attack,Magic_defense,Critical,HP,KnockBack_resist,Material_IDS,Material_quantitys,Require_gold,Require_cash);
        array[i__] = item__;
        map.Emplace(Index,item__);
      }
      {
        FItemEnchant::Array& target = const_cast<FItemEnchant::Array&>(FItemEnchant::array);
        target.Reset();
        target.SetNum(count__,true);
        target.Append(array);
      }
      {
        FItemEnchant::Map& target = const_cast<FItemEnchant::Map&>(FItemEnchant::map);
        target.Reset();
        target.Append(map);
      }
      return true;
    }
  };
  class ItemManufactureTableManager : public TBL::TableManager
  {
    public:
    ItemManufactureTableManager(void){}
    bool LoadTable(BufferReader& stream__) override
    {
      int32 count__ = 0;
      stream__ >> count__;
      if(count__ == 0) return true;
      int32 Index;
      int32 Subject_item_ID;
      int32 Material_item_ID;
      int32 Material_quantity;
      FItemManufacture::Array array; array.SetNum(count__,true);
      FItemManufacture::Map map;
      for(int i__=0;i__<count__;++i__)
      {
        stream__ >> Index;
        stream__ >> Subject_item_ID;
        stream__ >> Material_item_ID;
        stream__ >> Material_quantity;
        auto item__ = FItemManufacture(Index,Subject_item_ID,Material_item_ID,Material_quantity);
        array[i__] = item__;
        map.Emplace(Index,item__);
      }
      {
        FItemManufacture::Array& target = const_cast<FItemManufacture::Array&>(FItemManufacture::array);
        target.Reset();
        target.SetNum(count__,true);
        target.Append(array);
      }
      {
        FItemManufacture::Map& target = const_cast<FItemManufacture::Map&>(FItemManufacture::map);
        target.Reset();
        target.Append(map);
      }
      return true;
    }
  };
  class RandomBoxGroupTableManager : public TBL::TableManager
  {
    public:
    RandomBoxGroupTableManager(void){}
    bool LoadTable(BufferReader& stream__) override
    {
      int32 count__ = 0;
      stream__ >> count__;
      if(count__ == 0) return true;
      int32 ID;
      int32 RandomItemGroup_NO;
      int32 ClassType;
      int32 Item_ID;
      int32 RatioAmount;
      int32 Item_Quantity;
      FRandomBoxGroup::Array array; array.SetNum(count__,true);
      FRandomBoxGroup::Map map;
      for(int i__=0;i__<count__;++i__)
      {
        stream__ >> ID;
        stream__ >> RandomItemGroup_NO;
        stream__ >> ClassType;
        stream__ >> Item_ID;
        stream__ >> RatioAmount;
        stream__ >> Item_Quantity;
        auto item__ = FRandomBoxGroup(ID,RandomItemGroup_NO,ClassType,Item_ID,RatioAmount,Item_Quantity);
        array[i__] = item__;
        map.Emplace(ID,item__);
      }
      {
        FRandomBoxGroup::Array& target = const_cast<FRandomBoxGroup::Array&>(FRandomBoxGroup::array);
        target.Reset();
        target.SetNum(count__,true);
        target.Append(array);
      }
      {
        FRandomBoxGroup::Map& target = const_cast<FRandomBoxGroup::Map&>(FRandomBoxGroup::map);
        target.Reset();
        target.Append(map);
      }
      return true;
    }
  };
  bool TableManager::LoadTable(BufferReader& stream)
  {
    bool rtn = true;
    TArray<uint8> bytes;
    if(TBL::BufferReader::Decompress(stream,bytes)==false) return false;
    BufferReader bufferReader((uint8*)bytes.GetData(),(int32)bytes.Num());
    ItemTableManager ItemTableManager;
    ItemEffectTableManager ItemEffectTableManager;
    ItemEnchantTableManager ItemEnchantTableManager;
    ItemManufactureTableManager ItemManufactureTableManager;
    RandomBoxGroupTableManager RandomBoxGroupTableManager;
    
    rtn &= ItemTableManager.LoadTable(bufferReader);
    rtn &= ItemEffectTableManager.LoadTable(bufferReader);
    rtn &= ItemEnchantTableManager.LoadTable(bufferReader);
    rtn &= ItemManufactureTableManager.LoadTable(bufferReader);
    rtn &= RandomBoxGroupTableManager.LoadTable(bufferReader);
    return rtn;
  };
}
