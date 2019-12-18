
#include "stdafx.h"
#include <time.h>

#include "generated/table/ItemTable.h"
#include "generated/table/ItemTableTableManager.h"

#include "generated/table/GuildRaidTable.h"
#include "generated/table/GuildRaidTableTableManager.h"

int main()
{
	{
		std::string executePath = "D:\\PCPlayer\\Development\\Server\\data\\chn\\ItemTable.bytes";
		std::ifstream infile;
		infile.open(executePath.c_str(), std::ios_base::binary);
		if (!infile.eof())
		{
			TBL::ItemTable::TableManager::LoadTable(infile);
		}
		infile.close();
	}
	{
		std::string executePath = "D:\\PCPlayer\\Development\\Server\\data\\chn\\GuildRaidTable.bytes";
		std::ifstream infile;
		infile.open(executePath.c_str(), std::ios_base::binary);
		if (!infile.eof())
		{
			TBL::GuildRaidTable::TableManager::LoadTable(infile);
		}
		infile.close();
	}
	//time_t today = TBL::GuildRaidTable::RaidManagerTable::array[0]->Today;
	//
	//struct tm  *t = nullptr;
	//t = gmtime(&today);
	//
	//printf("%04d-%02d-%02d %02d:%02d:%02d",
	//	t->tm_year + 1900, t->tm_mon + 1, t->tm_mday,
	//	t->tm_hour, t->tm_min, t->tm_sec
	//);

	return 0;
}

