#include "ItemTableTableManager.h"
namespace TBL::ItemTable
{
  class FItemTableManager final : public TBL::FTableManager
  {
    public:
    FItemTableManager(void){}
    virtual ~FItemTableManager(void) override {}
    virtual bool LoadTable(FBufferReader& Reader_) override
    {
      int32 Count_ = 0;
      Reader_ >> Count_;
      if(Count_ == 0) return true;
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
      FItem::FArray Array_; Array_.SetNum(Count_,true);
      FItem::FMap Map_;
      for(auto Idx_=0;Idx_<Count_;++Idx_)
      {
        Reader_ >> Item_ID;
        Reader_ >> Name;
        Reader_ >> Item_grade;
        Reader_ >> Require_lv;
        Reader_ >> Enchant_lv;
        Reader_ >> PhysicalAttack;
        Reader_ >> PhysicalDefense;
        Reader_ >> MagicalAttack;
        Reader_ >> MagicalDefense;
        Reader_ >> Critical;
        Reader_ >> HP;
        Reader_ >> KnockBackResist;
        Reader_ >> DictionaryType;
        Reader_ >> ItemType;
        Reader_ >> Gear_Score;
        Reader_ >> InventoryType;
        Reader_ >> UsageType;
        Reader_ >> Socket_quantity;
        Reader_ >> Removal_cost;
        Reader_ >> Belonging;
        Reader_ >> Sub_stats_quantity;
        Reader_ >> Stack;
        Reader_ >> DesignScroll_ID;
        Reader_ >> BindingSkill_ID;
        Reader_ >> BindingAttack_ID;
        Reader_ >> Manufacture_gold;
        Reader_ >> Manufacture_cash;
        Reader_ >> SummonCompanion_ID;
        Reader_ >> Next_itemID;
        Reader_ >> Next_item_price;
        {
          auto ArrayCount_ = FBufferReader::Read7BitEncodedInt(Reader_);
          Next_Item_material.SetNum(ArrayCount_,true);
          for(auto ArrayIndex_=0;ArrayIndex_<ArrayCount_;++ArrayIndex_)
          {
            Reader_ >> Next_Item_material[ArrayIndex_];
          }
        }
        {
          auto ArrayCount_ = FBufferReader::Read7BitEncodedInt(Reader_);
          Next_Item_material_quantity.SetNum(ArrayCount_,true);
          for(auto ArrayIndex_=0;ArrayIndex_<ArrayCount_;++ArrayIndex_)
          {
            Reader_ >> Next_Item_material_quantity[ArrayIndex_];
          }
        }
        Reader_ >> Resource_Path;
        Reader_ >> WeaponName;
        Reader_ >> WeaponIndex;
        {
          auto ArrayCount_ = FBufferReader::Read7BitEncodedInt(Reader_);
          PartName.SetNum(ArrayCount_,true);
          for(auto ArrayIndex_=0;ArrayIndex_<ArrayCount_;++ArrayIndex_)
          {
            Reader_ >> PartName[ArrayIndex_];
          }
        }
        {
          auto ArrayCount_ = FBufferReader::Read7BitEncodedInt(Reader_);
          PartIndex.SetNum(ArrayCount_,true);
          for(auto ArrayIndex_=0;ArrayIndex_<ArrayCount_;++ArrayIndex_)
          {
            Reader_ >> PartIndex[ArrayIndex_];
          }
        }
        Reader_ >> Icon_path;
        Reader_ >> EXP;
        Reader_ >> Buy_cost;
        Reader_ >> Sell_reward;
        Reader_ >> Consignment_maxprice;
        Reader_ >> QuestBringer;
        Reader_ >> ItemEvent_ID;
        Reader_ >> Description;
        Reader_ >> Sub_Item;
        Reader_ >> WeaponType;
        {
          auto ArrayCount_ = FBufferReader::Read7BitEncodedInt(Reader_);
          RandomBoxGroup_NO.SetNum(ArrayCount_,true);
          for(auto ArrayIndex_=0;ArrayIndex_<ArrayCount_;++ArrayIndex_)
          {
            Reader_ >> RandomBoxGroup_NO[ArrayIndex_];
          }
        }
        auto FItemVar = FItem(Item_ID,Name,Item_grade,Require_lv,Enchant_lv,PhysicalAttack,PhysicalDefense,MagicalAttack,MagicalDefense,Critical,HP,KnockBackResist,DictionaryType,ItemType,Gear_Score,InventoryType,UsageType,Socket_quantity,Removal_cost,Belonging,Sub_stats_quantity,Stack,DesignScroll_ID,BindingSkill_ID,BindingAttack_ID,Manufacture_gold,Manufacture_cash,SummonCompanion_ID,Next_itemID,Next_item_price,Next_Item_material,Next_Item_material_quantity,Resource_Path,WeaponName,WeaponIndex,PartName,PartIndex,Icon_path,EXP,Buy_cost,Sell_reward,Consignment_maxprice,QuestBringer,ItemEvent_ID,Description,Sub_Item,WeaponType,RandomBoxGroup_NO);
        Array_[Idx_] = FItemVar;
        Map_.Emplace(Item_ID,FItemVar);
      }
      {
        auto& TargetArray = const_cast<FItem::FArray&>(FItem::Array_);
        TargetArray.Reset();
        TargetArray.SetNum(Count_,true);
        TargetArray.Append(Array_);
      }
      {
        auto& TargetMap = const_cast<FItem::FMap&>(FItem::Map_);
        TargetMap.Reset();
        TargetMap.Append(Map_);
      }
      return true;
    }
  };
  class FItemEffectTableManager final : public TBL::FTableManager
  {
    public:
    FItemEffectTableManager(void){}
    virtual ~FItemEffectTableManager(void) override {}
    virtual bool LoadTable(FBufferReader& Reader_) override
    {
      int32 Count_ = 0;
      Reader_ >> Count_;
      if(Count_ == 0) return true;
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
      FItemEffect::FArray Array_; Array_.SetNum(Count_,true);
      FItemEffect::FMap Map_;
      for(auto Idx_=0;Idx_<Count_;++Idx_)
      {
        Reader_ >> Index;
        Reader_ >> Item_ID;
        Reader_ >> Effect_type;
        Reader_ >> Effect_min;
        Reader_ >> Effect_max;
        Reader_ >> Time_type;
        Reader_ >> Time_rate;
        Reader_ >> Time;
        Reader_ >> Duration;
        Reader_ >> Description;
        auto FItemEffectVar = FItemEffect(Index,Item_ID,Effect_type,Effect_min,Effect_max,Time_type,Time_rate,Time,Duration,Description);
        Array_[Idx_] = FItemEffectVar;
        Map_.Emplace(Index,FItemEffectVar);
      }
      {
        auto& TargetArray = const_cast<FItemEffect::FArray&>(FItemEffect::Array_);
        TargetArray.Reset();
        TargetArray.SetNum(Count_,true);
        TargetArray.Append(Array_);
      }
      {
        auto& TargetMap = const_cast<FItemEffect::FMap&>(FItemEffect::Map_);
        TargetMap.Reset();
        TargetMap.Append(Map_);
      }
      return true;
    }
  };
  class FItemEnchantTableManager final : public TBL::FTableManager
  {
    public:
    FItemEnchantTableManager(void){}
    virtual ~FItemEnchantTableManager(void) override {}
    virtual bool LoadTable(FBufferReader& Reader_) override
    {
      int32 Count_ = 0;
      Reader_ >> Count_;
      if(Count_ == 0) return true;
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
      FItemEnchant::FArray Array_; Array_.SetNum(Count_,true);
      FItemEnchant::FMap Map_;
      for(auto Idx_=0;Idx_<Count_;++Idx_)
      {
        Reader_ >> Index;
        Reader_ >> Item_ID;
        Reader_ >> Enchant_lv;
        Reader_ >> Physical_attack;
        Reader_ >> Physical_defense;
        Reader_ >> Magic_attack;
        Reader_ >> Magic_defense;
        Reader_ >> Critical;
        Reader_ >> HP;
        Reader_ >> KnockBack_resist;
        {
          auto ArrayCount_ = FBufferReader::Read7BitEncodedInt(Reader_);
          Material_IDS.SetNum(ArrayCount_,true);
          for(auto ArrayIndex_=0;ArrayIndex_<ArrayCount_;++ArrayIndex_)
          {
            Reader_ >> Material_IDS[ArrayIndex_];
          }
        }
        {
          auto ArrayCount_ = FBufferReader::Read7BitEncodedInt(Reader_);
          Material_quantitys.SetNum(ArrayCount_,true);
          for(auto ArrayIndex_=0;ArrayIndex_<ArrayCount_;++ArrayIndex_)
          {
            Reader_ >> Material_quantitys[ArrayIndex_];
          }
        }
        Reader_ >> Require_gold;
        Reader_ >> Require_cash;
        auto FItemEnchantVar = FItemEnchant(Index,Item_ID,Enchant_lv,Physical_attack,Physical_defense,Magic_attack,Magic_defense,Critical,HP,KnockBack_resist,Material_IDS,Material_quantitys,Require_gold,Require_cash);
        Array_[Idx_] = FItemEnchantVar;
        Map_.Emplace(Index,FItemEnchantVar);
      }
      {
        auto& TargetArray = const_cast<FItemEnchant::FArray&>(FItemEnchant::Array_);
        TargetArray.Reset();
        TargetArray.SetNum(Count_,true);
        TargetArray.Append(Array_);
      }
      {
        auto& TargetMap = const_cast<FItemEnchant::FMap&>(FItemEnchant::Map_);
        TargetMap.Reset();
        TargetMap.Append(Map_);
      }
      return true;
    }
  };
  class FItemManufactureTableManager final : public TBL::FTableManager
  {
    public:
    FItemManufactureTableManager(void){}
    virtual ~FItemManufactureTableManager(void) override {}
    virtual bool LoadTable(FBufferReader& Reader_) override
    {
      int32 Count_ = 0;
      Reader_ >> Count_;
      if(Count_ == 0) return true;
      int32 Index;
      int32 Subject_item_ID;
      int32 Material_item_ID;
      int32 Material_quantity;
      FItemManufacture::FArray Array_; Array_.SetNum(Count_,true);
      FItemManufacture::FMap Map_;
      for(auto Idx_=0;Idx_<Count_;++Idx_)
      {
        Reader_ >> Index;
        Reader_ >> Subject_item_ID;
        Reader_ >> Material_item_ID;
        Reader_ >> Material_quantity;
        auto FItemManufactureVar = FItemManufacture(Index,Subject_item_ID,Material_item_ID,Material_quantity);
        Array_[Idx_] = FItemManufactureVar;
        Map_.Emplace(Index,FItemManufactureVar);
      }
      {
        auto& TargetArray = const_cast<FItemManufacture::FArray&>(FItemManufacture::Array_);
        TargetArray.Reset();
        TargetArray.SetNum(Count_,true);
        TargetArray.Append(Array_);
      }
      {
        auto& TargetMap = const_cast<FItemManufacture::FMap&>(FItemManufacture::Map_);
        TargetMap.Reset();
        TargetMap.Append(Map_);
      }
      return true;
    }
  };
  class FRandomBoxGroupTableManager final : public TBL::FTableManager
  {
    public:
    FRandomBoxGroupTableManager(void){}
    virtual ~FRandomBoxGroupTableManager(void) override {}
    virtual bool LoadTable(FBufferReader& Reader_) override
    {
      int32 Count_ = 0;
      Reader_ >> Count_;
      if(Count_ == 0) return true;
      int32 ID;
      int32 RandomItemGroup_NO;
      int32 ClassType;
      int32 Item_ID;
      int32 RatioAmount;
      int32 Item_Quantity;
      FRandomBoxGroup::FArray Array_; Array_.SetNum(Count_,true);
      FRandomBoxGroup::FMap Map_;
      for(auto Idx_=0;Idx_<Count_;++Idx_)
      {
        Reader_ >> ID;
        Reader_ >> RandomItemGroup_NO;
        Reader_ >> ClassType;
        Reader_ >> Item_ID;
        Reader_ >> RatioAmount;
        Reader_ >> Item_Quantity;
        auto FRandomBoxGroupVar = FRandomBoxGroup(ID,RandomItemGroup_NO,ClassType,Item_ID,RatioAmount,Item_Quantity);
        Array_[Idx_] = FRandomBoxGroupVar;
        Map_.Emplace(ID,FRandomBoxGroupVar);
      }
      {
        auto& TargetArray = const_cast<FRandomBoxGroup::FArray&>(FRandomBoxGroup::Array_);
        TargetArray.Reset();
        TargetArray.SetNum(Count_,true);
        TargetArray.Append(Array_);
      }
      {
        auto& TargetMap = const_cast<FRandomBoxGroup::FMap&>(FRandomBoxGroup::Map_);
        TargetMap.Reset();
        TargetMap.Append(Map_);
      }
      return true;
    }
  };
  bool FTableManager::LoadTable(FBufferReader& Reader_)
  {
    auto bRtn = true;
    TArray<uint8> Bytes_;
    if(TBL::FBufferReader::Decompress(Reader_,Bytes_)==false) return false;
    FBufferReader BufferReader((uint8*)Bytes_.GetData(),(int32)Bytes_.Num());
    FItemTableManager ItemTableManager;
    FItemEffectTableManager ItemEffectTableManager;
    FItemEnchantTableManager ItemEnchantTableManager;
    FItemManufactureTableManager ItemManufactureTableManager;
    FRandomBoxGroupTableManager RandomBoxGroupTableManager;
    
    bRtn &= ItemTableManager.LoadTable(BufferReader);
    bRtn &= ItemEffectTableManager.LoadTable(BufferReader);
    bRtn &= ItemEnchantTableManager.LoadTable(BufferReader);
    bRtn &= ItemManufactureTableManager.LoadTable(BufferReader);
    bRtn &= RandomBoxGroupTableManager.LoadTable(BufferReader);
    return bRtn;
  };
}
