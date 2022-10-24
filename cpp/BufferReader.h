#pragma once
#include "CoreMinimal.h"
#include <zlib.h>
namespace TBL
{
    class BufferReader 
    {
        protected:
        /** Pointer to the data this reader is attached to */
        const uint8* Data;
        /** The size of the data in bytes */
        const int32 NumBytes;
        /** The current location in the byte stream for reading */
        int32 CurrentOffset;
        /** Indicates whether reading from the buffer caused an overflow or not */
        bool bHasOverflowed;

        /** Hidden on purpose */
        BufferReader(void);
        public:
        /**
         * Initializes the buffer, size, and zeros the read offset
         *
         * @param InData the buffer to attach to
         * @param Length the size of the buffer we are attaching to
         */
        BufferReader(const uint8* InData,int32 Length) :
            Data(InData),
            NumBytes(Length),
            CurrentOffset(0),
            bHasOverflowed(false)
        {
        }

        static int32 Read7BitEncodedInt(BufferReader& stream)
        {
#ifdef ENCODED
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            int32 count = 0;
            int32 shift = 0;
            uint8 b;
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                //if (shift == 5 * 7) // 5 bytes max per Int32, shift += 7
                //    ensureMsg(false, "Format_Bad7BitInt32");

                // ReadByte handles end of stream cases for us.
                stream >> b;
                count |= (b & 0x7F) << shift;
                shift += 7;
            }
            while ((b & 0x80) != 0);
            return count;
#else
            int32 count = 0;
            stream >> count;
            return count;
#endif
        }

        /**
         * Reads a char from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,char& Ch)
        {
            if (!Ar.HasOverflow() && Ar.CurrentOffset + 1 <= Ar.NumBytes)
            {
                Ch = Ar.Data[Ar.CurrentOffset++];
            }
            else
            {
                Ar.bHasOverflowed = true;
            }
            return Ar;
        }
        /**
         * Reads a bool from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,bool& B)
        {
            if (!Ar.HasOverflow() && Ar.CurrentOffset + 1 <= Ar.NumBytes)
            {
                B = Ar.Data[Ar.CurrentOffset++] > 0;
            }
            else
            {
                Ar.bHasOverflowed = true;
            }
            return Ar;
        }
        /**
         * Reads a byte from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,uint8& B)
        {
            if (!Ar.HasOverflow() && Ar.CurrentOffset + 1 <= Ar.NumBytes)
            {
                B = Ar.Data[Ar.CurrentOffset++];
            }
            else
            {
                Ar.bHasOverflowed = true;
            }
            return Ar;
        }
        /**
         * Reads an int32 from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,int16& I)
        {
            return Ar >> *(uint16*)&I;
        }

        /**
         * Reads a uint32 from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,uint16& D)
        {
            if (!Ar.HasOverflow() && Ar.CurrentOffset + 4 <= Ar.NumBytes)
            {
                uint32 D1 = Ar.Data[Ar.CurrentOffset + 1];
                uint32 D2 = Ar.Data[Ar.CurrentOffset + 0];
                D = D1 << 8 | D2;
                Ar.CurrentOffset += 2;
            }
            else
            {
                Ar.bHasOverflowed = true;
            }
            return Ar;
        }
        /**
         * Reads an int32 from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,int32& I)
        {
            return Ar >> *(uint32*)&I;
        }

        /**
         * Reads a uint32 from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,uint32& D)
        {
            if (!Ar.HasOverflow() && Ar.CurrentOffset + 4 <= Ar.NumBytes)
            {
                uint32 D1 = Ar.Data[Ar.CurrentOffset + 3];
                uint32 D2 = Ar.Data[Ar.CurrentOffset + 2];
                uint32 D3 = Ar.Data[Ar.CurrentOffset + 1];
                uint32 D4 = Ar.Data[Ar.CurrentOffset + 0];
                D = D1 << 24 | D2 << 16 | D3 << 8 | D4;
                Ar.CurrentOffset += 4;
            }
            else
            {
                Ar.bHasOverflowed = true;
            }
            return Ar;
        }

        /**
         * Adds a uint64 to the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,uint64& Q)
        {
            if (!Ar.HasOverflow() && Ar.CurrentOffset + 8 <= Ar.NumBytes)
            {
                Q = ((uint64)Ar.Data[Ar.CurrentOffset + 7] << 56) |
                    ((uint64)Ar.Data[Ar.CurrentOffset + 6] << 48) |
                    ((uint64)Ar.Data[Ar.CurrentOffset + 5] << 40) |
                    ((uint64)Ar.Data[Ar.CurrentOffset + 4] << 32) |
                    ((uint64)Ar.Data[Ar.CurrentOffset + 3] << 24) |
                    ((uint64)Ar.Data[Ar.CurrentOffset + 2] << 16) |
                    ((uint64)Ar.Data[Ar.CurrentOffset + 1] << 8) |
                    (uint64)Ar.Data[Ar.CurrentOffset + 0];
                Ar.CurrentOffset += 8;
            }
            else
            {
                Ar.bHasOverflowed = true;
            }
            return Ar;
        }

        /**
         * Reads a float from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,float& F)
        {
            return Ar >> *(uint32*)&F;
        }

        /**
         * Reads a double from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,double& Dbl)
        {
            return Ar >> *(uint64*)&Dbl;
        }

        /**
         * Reads a FString from the buffer
         */
        friend inline BufferReader& operator>>(BufferReader& Ar,FString& String)
        {
            // We send strings length prefixed
            int32 Len = Read7BitEncodedInt(Ar);

            // Check this way to trust NumBytes and CurrentOffset to be more accurate than the packet Len value
            const bool bSizeOk = (Len >= 0) && (Len <= (Ar.NumBytes - Ar.CurrentOffset));
            if (!Ar.HasOverflow() && bSizeOk)
            {
                // Handle strings of zero length
                if (Len > 0)
                {
                    char* Temp = (char*)FMemory_Alloca(Len + 1);
                    // memcpy it in from the buffer
                    FMemory::Memcpy(Temp, &Ar.Data[Ar.CurrentOffset], Len);
                    Temp[Len] = '\0';

                    FUTF8ToTCHAR Converted(Temp);
                    TCHAR* Ptr = (TCHAR*)Converted.Get();
                    String = Ptr;
                    Ar.CurrentOffset += Len;
                }
                else
                {
                    String.Empty();
                }
            }
            else
            {
                Ar.bHasOverflowed = true;
            }

            return Ar;
        }


        /**
         * Reads a blob of data from the buffer into an array
         * Prevents the necessity for additional allocation.
         *
         * @param OutArray the destination array
         * @param NumToRead the size of the blob to read
         */
        void ReadBinaryArray(TArray<uint8>& OutArray, uint32 NumToRead)
        {
            OutArray.Reserve(NumToRead);
            if (!HasOverflow() && CurrentOffset + (int32)NumToRead <= NumBytes)
            {
                for (uint32 i = 0; i < NumToRead; ++i)
                {
                    OutArray.Add(Data[CurrentOffset + i]);
                }
                CurrentOffset += NumToRead;
            }
            else
            {
                bHasOverflowed = true;
            }
        }

        /**
         * Reads a blob of data from the buffer
         *
         * @param OutBuffer the destination buffer
         * @param NumToRead the size of the blob to read
         */
        void ReadBinary(uint8* OutBuffer,uint32 NumToRead)
        {
            if (!HasOverflow() && CurrentOffset + (int32)NumToRead <= NumBytes)
            {
                FMemory::Memcpy(OutBuffer,&Data[CurrentOffset],NumToRead);
                CurrentOffset += NumToRead;
            }
            else
            {
                bHasOverflowed = true;
            }
        }

        static bool Decompress(BufferReader& stream, TArray<uint8>& bytes)
        {
            bool rtn = true;

            uint8 hashSize;

            uint32 uncompressedSize = 0;
            uint32 out_uncompressedSize = 0;
            uint32 compressedSize = 0;
            TArray<uint8> hashValue;

            stream >> hashSize;

            hashValue.SetNum(hashSize,true);

            stream.ReadBinary( hashValue.GetData(), hashSize);
            stream >> uncompressedSize;
            stream >> compressedSize;

            TArray<uint8> compressed;

            {
                compressed.SetNum(compressedSize,true);
                stream.ReadBinary( compressed.GetData(), compressedSize);

                /// @TODO: hash compare
                bytes.SetNum(uncompressedSize,true);
                uLong uncompressedLength = uncompressedSize;
                int bzError = uncompress(bytes.GetData(), &uncompressedLength, compressed.GetData(), compressedSize);
                if (bzError != Z_OK)
                {
                    //getBZ2Error(bzError);
                    return false;
                }
            }
            return true;
        }


        /**
         * Seek to the desired position in the buffer
         *
         * @param Pos the offset from the start of the buffer
         */
        void Seek(int32 Pos)
        {
            checkSlow(Pos >= 0);

            if (!HasOverflow() && Pos < NumBytes)
            {
                CurrentOffset = Pos;
            }
            else
            {
                bHasOverflowed = true;
            }
        }

        /** @return Current position of the buffer being to be read */
        inline int32 Tell(void) const
        {
            return CurrentOffset;
        }

        /** Returns whether the buffer had an overflow when reading from it */
        inline bool HasOverflow(void) const
        {
            return bHasOverflowed;
        }

        /** @return Number of bytes remaining to read from the current offset to the end of the buffer */
        inline int32 AvailableToRead(void) const
        {
            return FMath::Max<int32>(0,NumBytes - CurrentOffset);
        }

        /**
         * Returns the number of total bytes in the buffer
         */
        inline int32 GetBufferSize(void) const
        {
            return NumBytes;
        }
    };
}
