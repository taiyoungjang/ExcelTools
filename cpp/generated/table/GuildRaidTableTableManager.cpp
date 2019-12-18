#include "GuildRaidTableTableManager.h"
namespace TBL
{
  namespace GuildRaidTable
  {
    class RaidManagerTableTableManager : public TBL::TableManager
    {
      public:
      bool RaidManagerTableTableManager::LoadTable(std::istream& stream__) override
      {
        int count__ = 0;
        Read(stream__,count__);
        if(count__ == 0) return true;
        int RaidManager_ID;
        short RaidType;
        short RaidMember;
        double StartTime;
        double EndTime;
        std::vector<bool> ResetDay; ResetDay.resize(7);
        double ResetTime;
        time_t Today;
        RaidManagerTable::Array array; array.resize(count__);
        RaidManagerTable::Map map;
        for(int i__=0;i__<count__;++i__)
        {
          Read(stream__, RaidManager_ID);
          Read(stream__, RaidType);
          Read(stream__, RaidMember);
          {long long element__; Read(stream__, element__); StartTime = (double) element__ / 10000000; }
          {long long element__; Read(stream__, element__); EndTime = (double) element__ / 10000000; }
          { bool element__; Read(stream__, element__); ResetDay[0] = element__; }
          { bool element__; Read(stream__, element__); ResetDay[1] = element__; }
          { bool element__; Read(stream__, element__); ResetDay[2] = element__; }
          { bool element__; Read(stream__, element__); ResetDay[3] = element__; }
          { bool element__; Read(stream__, element__); ResetDay[4] = element__; }
          { bool element__; Read(stream__, element__); ResetDay[5] = element__; }
          { bool element__; Read(stream__, element__); ResetDay[6] = element__; }
          {long long element__; Read(stream__, element__); ResetTime = (double) element__ / 10000000; }
          {long long element__; Read(stream__, element__); Today = (time_t) (element__ - 621355968000000000) / 10000000; }
          RaidManagerTablePtr item__ = RaidManagerTablePtr(new RaidManagerTable(RaidManager_ID,RaidType,RaidMember,StartTime,EndTime,ResetDay,ResetTime,Today));
          array[i__] = item__;
          map.insert( std::pair<int,RaidManagerTablePtr>(RaidManager_ID,item__));
        }
        {
          RaidManagerTable::Array& target = const_cast<RaidManagerTable::Array&>(RaidManagerTable::array);
          target.clear();
          target.resize(count__);
          std::copy(array.begin(),array.end(),target.begin());
        }
        {
          RaidManagerTable::Map& target = const_cast<RaidManagerTable::Map&>(RaidManagerTable::map);
          target.clear();
          target.insert(map.begin(),map.end());
        }
        return true;
      }
    };
    class GuildRaidTableTableManager : public TBL::TableManager
    {
      public:
      bool GuildRaidTableTableManager::LoadTable(std::istream& stream__) override
      {
        int count__ = 0;
        Read(stream__,count__);
        if(count__ == 0) return true;
        short GuildRaid_ID;
        std::wstring RaidName;
        short RaidType;
        short RaidGroup;
        int MonsterID;
        short Area_ID;
        short SpawnPointIndex;
        short EntryLimitation;
        int Default_goldreward;
        int Default_cashreward;
        std::vector<int> DefaultReward; DefaultReward.resize(2);
        std::vector<int> Reward_quantity; Reward_quantity.resize(2);
        int ApplyDate;
        GuildRaidTable::Array array; array.resize(count__);
        GuildRaidTable::Map map;
        for(int i__=0;i__<count__;++i__)
        {
          Read(stream__, GuildRaid_ID);
          Read(stream__, RaidName);
          Read(stream__, RaidType);
          Read(stream__, RaidGroup);
          Read(stream__, MonsterID);
          Read(stream__, Area_ID);
          Read(stream__, SpawnPointIndex);
          Read(stream__, EntryLimitation);
          Read(stream__, Default_goldreward);
          Read(stream__, Default_cashreward);
          Read(stream__, DefaultReward[0]);
          Read(stream__, DefaultReward[1]);
          Read(stream__, Reward_quantity[0]);
          Read(stream__, Reward_quantity[1]);
          Read(stream__, ApplyDate);
          GuildRaidTablePtr item__ = GuildRaidTablePtr(new GuildRaidTable(GuildRaid_ID,RaidName,RaidType,RaidGroup,MonsterID,Area_ID,SpawnPointIndex,EntryLimitation,Default_goldreward,Default_cashreward,DefaultReward,Reward_quantity,ApplyDate));
          array[i__] = item__;
          map.insert( std::pair<short,GuildRaidTablePtr>(GuildRaid_ID,item__));
        }
        {
          GuildRaidTable::Array& target = const_cast<GuildRaidTable::Array&>(GuildRaidTable::array);
          target.clear();
          target.resize(count__);
          std::copy(array.begin(),array.end(),target.begin());
        }
        {
          GuildRaidTable::Map& target = const_cast<GuildRaidTable::Map&>(GuildRaidTable::map);
          target.clear();
          target.insert(map.begin(),map.end());
        }
        return true;
      }
    };
    class RaidMonsterAdventTableTableManager : public TBL::TableManager
    {
      public:
      bool RaidMonsterAdventTableTableManager::LoadTable(std::istream& stream__) override
      {
        int count__ = 0;
        Read(stream__,count__);
        if(count__ == 0) return true;
        int Advent_ID;
        short RaidGroup;
        float GroupRate;
        std::vector<short> GuildRaid_ID; GuildRaid_ID.resize(4);
        std::vector<short> ExpectationValue; ExpectationValue.resize(4);
        RaidMonsterAdventTable::Array array; array.resize(count__);
        RaidMonsterAdventTable::Map map;
        for(int i__=0;i__<count__;++i__)
        {
          Read(stream__, Advent_ID);
          Read(stream__, RaidGroup);
          Read(stream__, GroupRate);
          Read(stream__, GuildRaid_ID[0]);
          Read(stream__, GuildRaid_ID[1]);
          Read(stream__, GuildRaid_ID[2]);
          Read(stream__, GuildRaid_ID[3]);
          Read(stream__, ExpectationValue[0]);
          Read(stream__, ExpectationValue[1]);
          Read(stream__, ExpectationValue[2]);
          Read(stream__, ExpectationValue[3]);
          RaidMonsterAdventTablePtr item__ = RaidMonsterAdventTablePtr(new RaidMonsterAdventTable(Advent_ID,RaidGroup,GroupRate,GuildRaid_ID,ExpectationValue));
          array[i__] = item__;
          map.insert( std::pair<int,RaidMonsterAdventTablePtr>(Advent_ID,item__));
        }
        {
          RaidMonsterAdventTable::Array& target = const_cast<RaidMonsterAdventTable::Array&>(RaidMonsterAdventTable::array);
          target.clear();
          target.resize(count__);
          std::copy(array.begin(),array.end(),target.begin());
        }
        {
          RaidMonsterAdventTable::Map& target = const_cast<RaidMonsterAdventTable::Map&>(RaidMonsterAdventTable::map);
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
      RaidManagerTableTableManager RaidManagerTableTableManager;
      GuildRaidTableTableManager GuildRaidTableTableManager;
      RaidMonsterAdventTableTableManager RaidMonsterAdventTableTableManager;
      
      rtn &= RaidManagerTableTableManager.LoadTable(is);
      rtn &= GuildRaidTableTableManager.LoadTable(is);
      rtn &= RaidMonsterAdventTableTableManager.LoadTable(is);
      return rtn;
    };
  };
};
