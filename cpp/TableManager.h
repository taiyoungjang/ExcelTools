#pragma once
#include "CoreMinimal.h"
#include "BufferReader.h"

namespace TBL
{
    class FTableManager
    {
        public:
        virtual ~FTableManager() = default;
        virtual bool LoadTable(FBufferReader& Reader) = 0;

        virtual void GetTableName(FString& Name)
        {
            Name = TEXT("TableManager");
        }

    };
}
