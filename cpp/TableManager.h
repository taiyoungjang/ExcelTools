#pragma once
#include "CoreMinimal.h"
#include "BufferReader.h"

namespace TBL
{
    class TableManager 
    {
        public:
        virtual bool LoadTable(BufferReader& stream) = 0;

        void GetTableName(FString& name)
        {
            name = TEXT("TableManager");
        }

    };
}
