public enum eDictionaryType
{
    Consume,
    Equipment
}


/*!
 * 신규 업적 타입
 */

public enum eAchievementCategory : byte
{
    General = 0,    // 일반 업적 카테고리
    Item = 1,       // 아이템 관련 업적 카테고리
    Dungeon = 2,    // 던전 및 레이드 관련 업적 카테고리
    Guild = 3,  // 길드 관련 업적 카테고리
    PvP = 4,		// PvP 관련 업적 카테고리
}

/*!
 * 업적정보 타입
 */
public enum eAchievementType : int
{
    NONE = 0,
    Login_Count = 10001,                  ///< 로그인 횟수
	Login_PlayingTime = 10002,            ///< 플레잉 타임(초)
	Login_LastLoginDate = 10003,          ///< 마지막 로그인 날짜
	Login_ContinuousAttendance = 10004,   ///< 최근 연속 출석 체크 횟수
	Login_GuildLastLoginDate = 10101,     ///< 마지막 길드 출석 로그인 날짜
	Login_GuildAttendance = 10102,		  ///< 길드 출석 횟수
    Monster_Total_Kill_Quantity = 20000,  // 몬스터 Total 사냥(kill)
    Monster_ID_Kill_Quantity = 20001,     // 몬스터 사냥(kill)
    Monster_ClassType_Kill_Quantity = 20002,   // 몬스터 Type 사냥(kill)

    PvP_Total_Kill_Quantity = 20010,      // PvP Total 사냥(kill) 
    PvP_GameMode_Kill_Quantity = 20011,   // PvP GameMode 사냥(kill) 
    PvP_ClassType_Kill_Quantity = 20012,  // PvP Class별 Kill 헤일론, 이스린 등

    Get_Cost_Quantity = 20100,            		// 획득 cost	20: gold
    Character_LevelUp = 20101,                  // 캐릭터 레벨 업

    Get_Item_ID_Quantity = 20110,         		// 획득 아이템 아이디 별
    Get_Item_UseType_Quantity = 20111,          // 획득 아이템 아이템 타입
    Get_Item_InventoryType_Quantity = 20112,    // 획득 아이템 인벤토리 타입
    Get_Item_EquipSlot_Quantity = 20113,        // 획득 아이템 장착 타입
    Get_Item_Equip_Grade_Quantity = 20114,      // 획득 아이템 Grade 타입
    Get_Item_GameMode_Quantity = 20115,         // 획득 아이템 게임 모드별 

    Use_Item_ID_Quantity = 20200,               // 아이템 사용 아이디 별
    Use_Item_UseType_Quantity = 20201,          // 아이템 사용 사용타입
    Use_Item_ItemLevelUp_Quantity = 20210,      // 아이템 사용 레벨업
    Use_Item_ItemEnchant_Quantity = 20211,      // 아이템 사용 강화
    Use_Item_MakeItem_Quantity = 20212,         // 아이템 사용 제작

    Use_Fury_Count = 20300,                     // 변신 횟수
    Revival_Help_Count = 20301,                 // 부활 도움 횟수
    Item_LevelUp_Count = 20302,                 // 아이템 레렙업 횟수
    Item_Enchant_Count = 20303,                 // 아이템 강화 횟수
    Item_Option_Change_Count = 20304,           // 옵션 변경 횟수
    Item_Equip_Gem_Count = 20305,               // 아이템 보석 장착 횟수
    Item_Make_Item_Count = 20306,               // 아이템 제작 횟수
    Item_Gem_Type_Change_Count = 20307,         // 아이템 보석 변경 횟수

    Quest_Group_Complete_Count = 20400,         // 퀘스트 완료	
    Entrance_GameMode_Count = 20410,            // 게임 모드 입장 횟수
    CompleteGame_GameMode_Count = 20411,        // 게임 완료 횟수
    WinGame_GameMode_Count = 20412,     // 게임 승리 횟수
    LoseGame_GameMode_Count = 20413,        // 게임 패배 횟수

    Guild_CreateOrJoin = 20500,     // 길드 창설 및 가입
    DeathPlayerCharacter = 20510,		// 플레이어 캐릭터 사망

    Dungeon_Clear = 20600,                      // 던전 클리어
    MAX
}

public struct ItemID : System.IComparable, System.IComparable<int>
{
    public readonly int Value;
    public ItemID(int value)
    {
        Value = value;
    }
    public override bool Equals(object other)
    {
        if (other.GetType() == typeof(ItemID))
        {
            var o = (ItemID)other;
            return o.Value == Value;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public int CompareTo(int other)
    {
        return Value.CompareTo(other);
    }
    public int CompareTo(object other)
    {
        return Value.CompareTo(other);
    }

    public static implicit operator ItemID(int value)
    {
        return new ItemID(value);
    }
    public static explicit operator int(ItemID value)
    {
        return value.Value;
    }
    public override string ToString()
    {
        return Value.ToString();
    }
    public static bool operator ==(ItemID d1, ItemID d2)
    {
        return d1.Value == d2.Value;
    }
    public static bool operator !=(ItemID d1, ItemID d2)
    {
        return d1.Value != d2.Value;
    }

    public static bool operator ==(ItemID d1, int d2)
    {
        return d1.Value == d2;
    }
    public static bool operator !=(ItemID d1, int d2)
    {
        return d1.Value != d2;
    }
}


public struct ItemType : System.IComparable, System.IComparable<int>
{
    public readonly int Value;
    public ItemType(int value)
    {
        Value = value;
    }
    public override bool Equals(object other)
    {
        if (other.GetType() == typeof(ItemType))
        {
            var o = (ItemType)other;
            return o.Value == Value;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    public int CompareTo(int other)
    {
        return Value.CompareTo(other);
    }
    public int CompareTo(object other)
    {
        return Value.CompareTo(other);
    }
    public static implicit operator ItemType(int value)
    {
        return new ItemType(value);
    }
    public static explicit operator int(ItemType value)
    {
        return value.Value;
    }
    public override string ToString()
    {
        return Value.ToString();
    }
    public static bool operator ==(ItemType d1, ItemType d2)
    {
        return d1.Value == d2.Value;
    }
    public static bool operator !=(ItemType d1, ItemType d2)
    {
        return d1.Value != d2.Value;
    }
}

