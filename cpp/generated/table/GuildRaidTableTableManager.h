// generate GuildRaidTableTableManager.h
// DO NOT TOUCH SOURCE....
#ifndef GUILDRAIDTABLETABLEMANAGER_H
#define GUILDRAIDTABLETABLEMANAGER_H

#include "GuildRaidTable.h"
#include <TableManager.h>
namespace TBL
{
  namespace GuildRaidTable
  {
    class TableManager
    {
      public:
      static bool LoadTable(std::ifstream& stream);
    };
  };
};
#endif //GUILDRAIDTABLETABLEMANAGER_H
