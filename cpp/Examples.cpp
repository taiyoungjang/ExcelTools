
#include "stdafx.h"
#include <time.h>
#include <filesystem>
#include <iostream>
#include <filesystem>
namespace fs = std::filesystem;

#include "generated/table/ItemTable.h"
#include "generated/table/ItemTableTableManager.h"

int main()
{
	{
        std::wstring executePath = fs::current_path().append("..\\Examples\\Bytes\\Korean\\ItemTable.bytes").c_str();
		std::ifstream infile;
		infile.open(executePath.c_str(), std::ios_base::binary);
		if (!infile.eof())
		{
			TBL::ItemTable::TableManager::LoadTable(infile);
            int size = TBL::ItemTable::Item::array.size();
            std::cout << size;
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

