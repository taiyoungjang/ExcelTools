// generate GuildRaidTable.h
// DO NOT TOUCH SOURCE....
#ifndef GUILDRAIDTABLE_H
#define GUILDRAIDTABLE_H
#include <memory>
#include <string>
#include <vector>
#include <map>
namespace TBL
{
    namespace GuildRaidTable
    {
        class RaidManagerTable;
        typedef std::shared_ptr<RaidManagerTable> RaidManagerTablePtr;
        class RaidManagerTable
        {
            public:
            static RaidManagerTablePtr Find( const int& RaidManager_ID);
            typedef std::vector<RaidManagerTablePtr> Array;
            typedef std::map<int,RaidManagerTablePtr> Map;
            static const Array array;
            static const Map map;
            
            const int RaidManager_ID;
            const short RaidType;
            const short RaidMember;
            const double StartTime;
            const double EndTime;
            const std::vector<bool> ResetDay;
            const double ResetTime;
            const time_t Today;
            RaidManagerTable (const int& RaidManager_ID__,const short& RaidType__,const short& RaidMember__,const double& StartTime__,const double& EndTime__,const std::vector<bool>& ResetDay__,const double& ResetTime__,const time_t& Today__);
        };
        class GuildRaidTable;
        typedef std::shared_ptr<GuildRaidTable> GuildRaidTablePtr;
        class GuildRaidTable
        {
            public:
            static GuildRaidTablePtr Find( const short& GuildRaid_ID);
            typedef std::vector<GuildRaidTablePtr> Array;
            typedef std::map<short,GuildRaidTablePtr> Map;
            static const Array array;
            static const Map map;
            
            const short GuildRaid_ID;
            const std::wstring RaidName;
            const short RaidType;
            const short RaidGroup;
            const int MonsterID;
            const short Area_ID;
            const short SpawnPointIndex;
            const short EntryLimitation;
            const int Default_goldreward;
            const int Default_cashreward;
            const std::vector<int> DefaultReward;
            const std::vector<int> Reward_quantity;
            const int ApplyDate;
            GuildRaidTable (const short& GuildRaid_ID__,const std::wstring& RaidName__,const short& RaidType__,const short& RaidGroup__,const int& MonsterID__,const short& Area_ID__,const short& SpawnPointIndex__,const short& EntryLimitation__,const int& Default_goldreward__,const int& Default_cashreward__,const std::vector<int>& DefaultReward__,const std::vector<int>& Reward_quantity__,const int& ApplyDate__);
        };
        class RaidMonsterAdventTable;
        typedef std::shared_ptr<RaidMonsterAdventTable> RaidMonsterAdventTablePtr;
        class RaidMonsterAdventTable
        {
            public:
            static RaidMonsterAdventTablePtr Find( const int& Advent_ID);
            typedef std::vector<RaidMonsterAdventTablePtr> Array;
            typedef std::map<int,RaidMonsterAdventTablePtr> Map;
            static const Array array;
            static const Map map;
            
            const int Advent_ID;
            const short RaidGroup;
            const float GroupRate;
            const std::vector<short> GuildRaid_ID;
            const std::vector<short> ExpectationValue;
            RaidMonsterAdventTable (const int& Advent_ID__,const short& RaidGroup__,const float& GroupRate__,const std::vector<short>& GuildRaid_ID__,const std::vector<short>& ExpectationValue__);
        };
    };
};
#endif //GUILDRAIDTABLE
