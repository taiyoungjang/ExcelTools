// generate ItemTableTableManager.h
// DO NOT TOUCH SOURCE....
#ifndef ITEMTABLETABLEMANAGER_H
#define ITEMTABLETABLEMANAGER_H

#include "ItemTable.h"
#include <TableManager.h>
namespace TBL
{
  namespace ItemTable
  {
    class TableManager
    {
      public:
      static bool LoadTable(std::ifstream& stream);
    };
  };
};
#endif //ITEMTABLETABLEMANAGER_H
