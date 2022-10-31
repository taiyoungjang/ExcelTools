// generate ItemTable
// DO NOT TOUCH SOURCE....
#pragma once
#include "CoreMinimal.h"
#include "ItemType.pb.h"
//#include "ItemTable.generated.h"
namespace TBL::ItemTable
{
    //USTRUCT(BlueprintType)
    struct FItem
    {
        //static FItem Find( const int32& Item_ID);
        typedef TArray<FItem> FArray;
        typedef TMap<int32,FItem> FMap;
        static const FArray Array_;
        static const FMap Map_;
        //GENERATED_BODY()
        
        const int32 Item_ID {};
        const FString Name {};
        const int32 Item_grade {};
        const int32 Require_lv {};
        const int32 Enchant_lv {};
        const int32 PhysicalAttack {};
        const int32 PhysicalDefense {};
        const int32 MagicalAttack {};
        const int32 MagicalDefense {};
        const float Critical {};
        const int32 HP {};
        const int32 KnockBackResist {};
        const ::eDictionaryType DictionaryType {};
        const int32 ItemType {};
        const int16 Gear_Score {};
        const int16 InventoryType {};
        const bool UsageType {};
        const int16 Socket_quantity {};
        const int32 Removal_cost {};
        const int16 Belonging {};
        const int16 Sub_stats_quantity {};
        const int32 Stack {};
        const int32 DesignScroll_ID {};
        const int32 BindingSkill_ID {};
        const int32 BindingAttack_ID {};
        const int32 Manufacture_gold {};
        const int32 Manufacture_cash {};
        const int32 SummonCompanion_ID {};
        const int32 Next_itemID {};
        const int32 Next_item_price {};
        const TArray<int32> Next_Item_material {};
        const TArray<int32> Next_Item_material_quantity {}; /// 젬의 경우 전체 필요한 수량을 여기에 적는다상위 젬을 만들기 위해 하위 젬이 4개 필요한 경우 4라고 기재
        const FString Resource_Path {};
        const FString WeaponName {};
        const int16 WeaponIndex {};
        const TArray<FString> PartName {};
        const TArray<int16> PartIndex {};
        const FString Icon_path {};
        const int32 EXP {};
        const int32 Buy_cost {};
        const int32 Sell_reward {};
        const int32 Consignment_maxprice {};
        const int32 QuestBringer {};
        const int32 ItemEvent_ID {};
        const FString Description {};
        const int32 Sub_Item {}; /// NETEASE-SH:방패등 서브아이템 아이디
        const int32 WeaponType {}; /// wlfh qkr:0: 맨손1: 왼손무기2: 오른손 무기
        const TArray<int32> RandomBoxGroup_NO {};
        FItem(void);
        FItem& operator=(const FItem& RHS);
        FItem (const int32& Item_ID,const FString& Name,const int32& Item_grade,const int32& Require_lv,const int32& Enchant_lv,const int32& PhysicalAttack,const int32& PhysicalDefense,const int32& MagicalAttack,const int32& MagicalDefense,const float& Critical,const int32& HP,const int32& KnockBackResist,const ::eDictionaryType& DictionaryType,const int32& ItemType,const int16& Gear_Score,const int16& InventoryType,const bool& UsageType,const int16& Socket_quantity,const int32& Removal_cost,const int16& Belonging,const int16& Sub_stats_quantity,const int32& Stack,const int32& DesignScroll_ID,const int32& BindingSkill_ID,const int32& BindingAttack_ID,const int32& Manufacture_gold,const int32& Manufacture_cash,const int32& SummonCompanion_ID,const int32& Next_itemID,const int32& Next_item_price,const TArray<int32>& Next_Item_material,const TArray<int32>& Next_Item_material_quantity,const FString& Resource_Path,const FString& WeaponName,const int16& WeaponIndex,const TArray<FString>& PartName,const TArray<int16>& PartIndex,const FString& Icon_path,const int32& EXP,const int32& Buy_cost,const int32& Sell_reward,const int32& Consignment_maxprice,const int32& QuestBringer,const int32& ItemEvent_ID,const FString& Description,const int32& Sub_Item,const int32& WeaponType,const TArray<int32>& RandomBoxGroup_NO);
    };
    //USTRUCT(BlueprintType)
    struct FItemEffect
    {
        //static FItemEffect Find( const int32& Index);
        typedef TArray<FItemEffect> FArray;
        typedef TMap<int32,FItemEffect> FMap;
        static const FArray Array_;
        static const FMap Map_;
        //GENERATED_BODY()
        
        const int32 Index {};
        const int32 Item_ID {};
        const int32 Effect_type {};
        const float Effect_min {};
        const float Effect_max {};
        const int32 Time_type {};
        const float Time_rate {};
        const float Time {};
        const float Duration {};
        const FString Description {};
        FItemEffect(void);
        FItemEffect& operator=(const FItemEffect& RHS);
        FItemEffect (const int32& Index,const int32& Item_ID,const int32& Effect_type,const float& Effect_min,const float& Effect_max,const int32& Time_type,const float& Time_rate,const float& Time,const float& Duration,const FString& Description);
    };
    //USTRUCT(BlueprintType)
    struct FItemEnchant
    {
        //static FItemEnchant Find( const int32& Index);
        typedef TArray<FItemEnchant> FArray;
        typedef TMap<int32,FItemEnchant> FMap;
        static const FArray Array_;
        static const FMap Map_;
        //GENERATED_BODY()
        
        const int32 Index {};
        const int32 Item_ID {};
        const int32 Enchant_lv {};
        const int32 Physical_attack {};
        const int32 Physical_defense {};
        const int32 Magic_attack {};
        const int32 Magic_defense {};
        const float Critical {};
        const int32 HP {};
        const int32 KnockBack_resist {};
        const TArray<int32> Material_IDS {};
        const TArray<int32> Material_quantitys {};
        const int32 Require_gold {};
        const int32 Require_cash {};
        FItemEnchant(void);
        FItemEnchant& operator=(const FItemEnchant& RHS);
        FItemEnchant (const int32& Index,const int32& Item_ID,const int32& Enchant_lv,const int32& Physical_attack,const int32& Physical_defense,const int32& Magic_attack,const int32& Magic_defense,const float& Critical,const int32& HP,const int32& KnockBack_resist,const TArray<int32>& Material_IDS,const TArray<int32>& Material_quantitys,const int32& Require_gold,const int32& Require_cash);
    };
    //USTRUCT(BlueprintType)
    struct FItemManufacture
    {
        //static FItemManufacture Find( const int32& Index);
        typedef TArray<FItemManufacture> FArray;
        typedef TMap<int32,FItemManufacture> FMap;
        static const FArray Array_;
        static const FMap Map_;
        //GENERATED_BODY()
        
        const int32 Index {};
        const int32 Subject_item_ID {};
        const int32 Material_item_ID {};
        const int32 Material_quantity {};
        FItemManufacture(void);
        FItemManufacture& operator=(const FItemManufacture& RHS);
        FItemManufacture (const int32& Index,const int32& Subject_item_ID,const int32& Material_item_ID,const int32& Material_quantity);
    };
    //USTRUCT(BlueprintType)
    struct FRandomBoxGroup
    {
        //static FRandomBoxGroup Find( const int32& ID);
        typedef TArray<FRandomBoxGroup> FArray;
        typedef TMap<int32,FRandomBoxGroup> FMap;
        static const FArray Array_;
        static const FMap Map_;
        //GENERATED_BODY()
        
        const int32 ID {};
        const int32 RandomItemGroup_NO {};
        const int32 ClassType {};
        const int32 Item_ID {};
        const int32 RatioAmount {};
        const int32 Item_Quantity {};
        FRandomBoxGroup(void);
        FRandomBoxGroup& operator=(const FRandomBoxGroup& RHS);
        FRandomBoxGroup (const int32& ID,const int32& RandomItemGroup_NO,const int32& ClassType,const int32& Item_ID,const int32& RatioAmount,const int32& Item_Quantity);
    };
}
