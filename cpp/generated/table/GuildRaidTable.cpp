// generate GuildRaidTable.cpp
// DO NOT TOUCH SOURCE....
#include "GuildRaidTable.h"
using namespace TBL::GuildRaidTable;
const RaidManagerTable::Array RaidManagerTable::array;
const RaidManagerTable::Map RaidManagerTable::map;
RaidManagerTable::RaidManagerTable (const int& RaidManager_ID__
,const short& RaidType__
,const short& RaidMember__
,const double& StartTime__
,const double& EndTime__
,const std::vector<bool>& ResetDay__
,const double& ResetTime__
,const time_t& Today__)
:RaidManager_ID(RaidManager_ID__)
,RaidType(RaidType__)
,RaidMember(RaidMember__)
,StartTime(StartTime__)
,EndTime(EndTime__)
,ResetDay(ResetDay__)
,ResetTime(ResetTime__)
,Today(Today__)
{
}
RaidManagerTablePtr RaidManagerTable::Find(const int& RaidManager_ID)
{
    auto it = RaidManagerTable::map.find(RaidManager_ID);
    if (it != RaidManagerTable::map.end())
    {
        return it->second;
    }
    return RaidManagerTablePtr(nullptr);
}
const GuildRaidTable::Array GuildRaidTable::array;
const GuildRaidTable::Map GuildRaidTable::map;
GuildRaidTable::GuildRaidTable (const short& GuildRaid_ID__
,const std::wstring& RaidName__
,const short& RaidType__
,const short& RaidGroup__
,const int& MonsterID__
,const short& Area_ID__
,const short& SpawnPointIndex__
,const short& EntryLimitation__
,const int& Default_goldreward__
,const int& Default_cashreward__
,const std::vector<int>& DefaultReward__
,const std::vector<int>& Reward_quantity__
,const int& ApplyDate__)
:GuildRaid_ID(GuildRaid_ID__)
,RaidName(RaidName__)
,RaidType(RaidType__)
,RaidGroup(RaidGroup__)
,MonsterID(MonsterID__)
,Area_ID(Area_ID__)
,SpawnPointIndex(SpawnPointIndex__)
,EntryLimitation(EntryLimitation__)
,Default_goldreward(Default_goldreward__)
,Default_cashreward(Default_cashreward__)
,DefaultReward(DefaultReward__)
,Reward_quantity(Reward_quantity__)
,ApplyDate(ApplyDate__)
{
}
GuildRaidTablePtr GuildRaidTable::Find(const short& GuildRaid_ID)
{
    auto it = GuildRaidTable::map.find(GuildRaid_ID);
    if (it != GuildRaidTable::map.end())
    {
        return it->second;
    }
    return GuildRaidTablePtr(nullptr);
}
const RaidMonsterAdventTable::Array RaidMonsterAdventTable::array;
const RaidMonsterAdventTable::Map RaidMonsterAdventTable::map;
RaidMonsterAdventTable::RaidMonsterAdventTable (const int& Advent_ID__
,const short& RaidGroup__
,const float& GroupRate__
,const std::vector<short>& GuildRaid_ID__
,const std::vector<short>& ExpectationValue__)
:Advent_ID(Advent_ID__)
,RaidGroup(RaidGroup__)
,GroupRate(GroupRate__)
,GuildRaid_ID(GuildRaid_ID__)
,ExpectationValue(ExpectationValue__)
{
}
RaidMonsterAdventTablePtr RaidMonsterAdventTable::Find(const int& Advent_ID)
{
    auto it = RaidMonsterAdventTable::map.find(Advent_ID);
    if (it != RaidMonsterAdventTable::map.end())
    {
        return it->second;
    }
    return RaidMonsterAdventTablePtr(nullptr);
}
