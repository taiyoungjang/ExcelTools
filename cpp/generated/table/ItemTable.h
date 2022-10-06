// generate ItemTable
// DO NOT TOUCH SOURCE....
#pragma once
#include "CoreMinimal.h"
//#include "ItemTable.generated.h"
namespace TBL::ItemTable
{
    //USTRUCT(BlueprintType)
    struct FItem
    {
        //static FItem Find( const int32& Item_ID);
        typedef TArray<FItem> Array;
        typedef TMap<int32,FItem> Map;
        static const Array array;
        static const Map map;
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
        const int32 DictionaryType {};
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
        const TArray<int32> Next_Item_material_quantity {};  ///< 젬의 경우 전체 필요한 수량을 여기에 적는다상위 젬을 만들기 위해 하위 젬이 4개 필요한 경우 4라고 기재
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
        const int32 Sub_Item {};  ///< NETEASE-SH:방패등 서브아이템 아이디
        const int32 WeaponType {};  ///< wlfh qkr:0: 맨손1: 왼손무기2: 오른손 무기
        const TArray<int32> RandomBoxGroup_NO {};
        FItem(void);
        FItem& operator=(const FItem& rhs);
        FItem (const int32& Item_ID__,const FString& Name__,const int32& Item_grade__,const int32& Require_lv__,const int32& Enchant_lv__,const int32& PhysicalAttack__,const int32& PhysicalDefense__,const int32& MagicalAttack__,const int32& MagicalDefense__,const float& Critical__,const int32& HP__,const int32& KnockBackResist__,const int32& DictionaryType__,const int32& ItemType__,const int16& Gear_Score__,const int16& InventoryType__,const bool& UsageType__,const int16& Socket_quantity__,const int32& Removal_cost__,const int16& Belonging__,const int16& Sub_stats_quantity__,const int32& Stack__,const int32& DesignScroll_ID__,const int32& BindingSkill_ID__,const int32& BindingAttack_ID__,const int32& Manufacture_gold__,const int32& Manufacture_cash__,const int32& SummonCompanion_ID__,const int32& Next_itemID__,const int32& Next_item_price__,const TArray<int32>& Next_Item_material__,const TArray<int32>& Next_Item_material_quantity__,const FString& Resource_Path__,const FString& WeaponName__,const int16& WeaponIndex__,const TArray<FString>& PartName__,const TArray<int16>& PartIndex__,const FString& Icon_path__,const int32& EXP__,const int32& Buy_cost__,const int32& Sell_reward__,const int32& Consignment_maxprice__,const int32& QuestBringer__,const int32& ItemEvent_ID__,const FString& Description__,const int32& Sub_Item__,const int32& WeaponType__,const TArray<int32>& RandomBoxGroup_NO__);
    };
    //USTRUCT(BlueprintType)
    struct FItemEffect
    {
        //static FItemEffect Find( const int32& Index);
        typedef TArray<FItemEffect> Array;
        typedef TMap<int32,FItemEffect> Map;
        static const Array array;
        static const Map map;
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
        FItemEffect& operator=(const FItemEffect& rhs);
        FItemEffect (const int32& Index__,const int32& Item_ID__,const int32& Effect_type__,const float& Effect_min__,const float& Effect_max__,const int32& Time_type__,const float& Time_rate__,const float& Time__,const float& Duration__,const FString& Description__);
    };
    //USTRUCT(BlueprintType)
    struct FItemEnchant
    {
        //static FItemEnchant Find( const int32& Index);
        typedef TArray<FItemEnchant> Array;
        typedef TMap<int32,FItemEnchant> Map;
        static const Array array;
        static const Map map;
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
        FItemEnchant& operator=(const FItemEnchant& rhs);
        FItemEnchant (const int32& Index__,const int32& Item_ID__,const int32& Enchant_lv__,const int32& Physical_attack__,const int32& Physical_defense__,const int32& Magic_attack__,const int32& Magic_defense__,const float& Critical__,const int32& HP__,const int32& KnockBack_resist__,const TArray<int32>& Material_IDS__,const TArray<int32>& Material_quantitys__,const int32& Require_gold__,const int32& Require_cash__);
    };
    //USTRUCT(BlueprintType)
    struct FItemManufacture
    {
        //static FItemManufacture Find( const int32& Index);
        typedef TArray<FItemManufacture> Array;
        typedef TMap<int32,FItemManufacture> Map;
        static const Array array;
        static const Map map;
        //GENERATED_BODY()
        
        const int32 Index {};
        const int32 Subject_item_ID {};
        const int32 Material_item_ID {};
        const int32 Material_quantity {};
        FItemManufacture(void);
        FItemManufacture& operator=(const FItemManufacture& rhs);
        FItemManufacture (const int32& Index__,const int32& Subject_item_ID__,const int32& Material_item_ID__,const int32& Material_quantity__);
    };
    //USTRUCT(BlueprintType)
    struct FRandomBoxGroup
    {
        //static FRandomBoxGroup Find( const int32& ID);
        typedef TArray<FRandomBoxGroup> Array;
        typedef TMap<int32,FRandomBoxGroup> Map;
        static const Array array;
        static const Map map;
        //GENERATED_BODY()
        
        const int32 ID {};
        const int32 RandomItemGroup_NO {};
        const int32 ClassType {};
        const int32 Item_ID {};
        const int32 RatioAmount {};
        const int32 Item_Quantity {};
        FRandomBoxGroup(void);
        FRandomBoxGroup& operator=(const FRandomBoxGroup& rhs);
        FRandomBoxGroup (const int32& ID__,const int32& RandomItemGroup_NO__,const int32& ClassType__,const int32& Item_ID__,const int32& RatioAmount__,const int32& Item_Quantity__);
    };
}
