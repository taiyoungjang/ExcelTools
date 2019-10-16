public enum eDictionaryType
{
    Consume,
    Equipment
}


/*!
 * �ű� ���� Ÿ��
 */

public enum eAchievementCategory : byte
{
    General = 0,    // �Ϲ� ���� ī�װ�
    Item = 1,       // ������ ���� ���� ī�װ�
    Dungeon = 2,    // ���� �� ���̵� ���� ���� ī�װ�
    Guild = 3,  // ��� ���� ���� ī�װ�
    PvP = 4,		// PvP ���� ���� ī�װ�
}

/*!
 * �������� Ÿ��
 */
public enum eAchievementType : int
{
    NONE = 0,
    Login_Count = 10001,                  ///< �α��� Ƚ��
	Login_PlayingTime = 10002,            ///< �÷��� Ÿ��(��)
	Login_LastLoginDate = 10003,          ///< ������ �α��� ��¥
	Login_ContinuousAttendance = 10004,   ///< �ֱ� ���� �⼮ üũ Ƚ��
	Login_GuildLastLoginDate = 10101,     ///< ������ ��� �⼮ �α��� ��¥
	Login_GuildAttendance = 10102,		  ///< ��� �⼮ Ƚ��
    Monster_Total_Kill_Quantity = 20000,  // ���� Total ���(kill)
    Monster_ID_Kill_Quantity = 20001,     // ���� ���(kill)
    Monster_ClassType_Kill_Quantity = 20002,   // ���� Type ���(kill)

    PvP_Total_Kill_Quantity = 20010,      // PvP Total ���(kill) 
    PvP_GameMode_Kill_Quantity = 20011,   // PvP GameMode ���(kill) 
    PvP_ClassType_Kill_Quantity = 20012,  // PvP Class�� Kill ���Ϸ�, �̽��� ��

    Get_Cost_Quantity = 20100,            		// ȹ�� cost	20: gold
    Character_LevelUp = 20101,                  // ĳ���� ���� ��

    Get_Item_ID_Quantity = 20110,         		// ȹ�� ������ ���̵� ��
    Get_Item_UseType_Quantity = 20111,          // ȹ�� ������ ������ Ÿ��
    Get_Item_InventoryType_Quantity = 20112,    // ȹ�� ������ �κ��丮 Ÿ��
    Get_Item_EquipSlot_Quantity = 20113,        // ȹ�� ������ ���� Ÿ��
    Get_Item_Equip_Grade_Quantity = 20114,      // ȹ�� ������ Grade Ÿ��
    Get_Item_GameMode_Quantity = 20115,         // ȹ�� ������ ���� ��庰 

    Use_Item_ID_Quantity = 20200,               // ������ ��� ���̵� ��
    Use_Item_UseType_Quantity = 20201,          // ������ ��� ���Ÿ��
    Use_Item_ItemLevelUp_Quantity = 20210,      // ������ ��� ������
    Use_Item_ItemEnchant_Quantity = 20211,      // ������ ��� ��ȭ
    Use_Item_MakeItem_Quantity = 20212,         // ������ ��� ����

    Use_Fury_Count = 20300,                     // ���� Ƚ��
    Revival_Help_Count = 20301,                 // ��Ȱ ���� Ƚ��
    Item_LevelUp_Count = 20302,                 // ������ ������ Ƚ��
    Item_Enchant_Count = 20303,                 // ������ ��ȭ Ƚ��
    Item_Option_Change_Count = 20304,           // �ɼ� ���� Ƚ��
    Item_Equip_Gem_Count = 20305,               // ������ ���� ���� Ƚ��
    Item_Make_Item_Count = 20306,               // ������ ���� Ƚ��
    Item_Gem_Type_Change_Count = 20307,         // ������ ���� ���� Ƚ��

    Quest_Group_Complete_Count = 20400,         // ����Ʈ �Ϸ�	
    Entrance_GameMode_Count = 20410,            // ���� ��� ���� Ƚ��
    CompleteGame_GameMode_Count = 20411,        // ���� �Ϸ� Ƚ��
    WinGame_GameMode_Count = 20412,     // ���� �¸� Ƚ��
    LoseGame_GameMode_Count = 20413,        // ���� �й� Ƚ��

    Guild_CreateOrJoin = 20500,     // ��� â�� �� ����
    DeathPlayerCharacter = 20510,		// �÷��̾� ĳ���� ���

    Dungeon_Clear = 20600,                      // ���� Ŭ����
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

