#include "TableManager.h"
#include <vector>
#include <iterator>
#include <bzlib.h>
#include <codecvt>
#include <locale>

TBL::TableManager::TableManager()
{
}


TBL::TableManager::~TableManager()
{
}

bool TBL::TableManager::LoadTable(std::istream& stream)
{
	return true;
}

std::istream& TBL::TableManager::Read(std::istream& stream, unsigned char& data)
{
	stream.read((char*)&data, sizeof(unsigned char));
	return stream;
}

std::istream& TBL::TableManager::Read(std::istream& stream, char* data, int length)
{
	stream.read(data, length);
	return stream;
}

std::istream& TBL::TableManager::Read(std::istream& stream, unsigned char* data, int length)
{
	stream.read((char*)data, length);
	return stream;
}

std::istream& TBL::TableManager::Read(std::istream& stream, unsigned int& v)
{
	unsigned char bytes[4] = { 0 };
	stream.read((char*)bytes, sizeof(int));
	const unsigned char* src = &(*bytes);
#ifdef BIG_ENDIAN
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v) + sizeof(unsigned int) - 1;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest = *src;
#else
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v);
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest = *src;
#endif
	return stream;
}

std::istream& TBL::TableManager::Read(std::istream& stream, bool& data)
{
	unsigned char bytes[1] = { 0 };
	stream.read((char*)bytes, sizeof(bool));
	data = bytes[0] ==1;
	return stream;
}

std::istream& TBL::TableManager::Read(std::istream& stream, short& v)
{
	unsigned char bytes[2] = { 0 };
	stream.read((char*)bytes, sizeof(short));
	const unsigned char* src = &(*bytes);
#ifdef BIG_ENDIAN
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v) + sizeof(short) - 1;
	*dest-- = *src++;
	*dest = *src;
#else
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v);
	*dest++ = *src++;
	*dest = *src;
#endif
	return stream;
}


std::istream& TBL::TableManager::Read(std::istream& stream, int& v)
{
	unsigned char bytes[4] = { 0 };
	stream.read((char*)bytes, sizeof(int));
	const unsigned char* src = &(*bytes);
#ifdef BIG_ENDIAN
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v) + sizeof(int) - 1;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest = *src;
#else
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v);
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest = *src;
#endif
	return stream;
}

std::istream& TBL::TableManager::Read(std::istream& stream, long long& v)
{
	unsigned char bytes[8] = { 0 };
	stream.read((char*)bytes, sizeof(long long));
	const unsigned char* src = &(*bytes);
#ifdef BIG_ENDIAN
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v) + sizeof(long long) - 1;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest = *src;
#else
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v);
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest = *src;
#endif
	return stream;
}

std::istream& TBL::TableManager::Read(std::istream& stream, float& v)
{
	unsigned int d = 0;
	unsigned char bytes[4] = { 0 };
	stream.read((char*)bytes, sizeof(float));
	const unsigned char* src = &(*bytes);
#ifdef BIG_ENDIAN
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v) + sizeof(float) - 1;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest = *src;
#else
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v);
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest = *src;
#endif
	return stream;
}

std::istream& TBL::TableManager::Read(std::istream& stream, double& v)
{
	unsigned char bytes[8] = { 0 };
	stream.read((char*)bytes, sizeof(double));
	const unsigned char* src = &(*bytes);
#ifdef BIG_ENDIAN
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v) + sizeof(double) - 1;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest-- = *src++;
	*dest = *src;
#else
	unsigned char* dest = reinterpret_cast<unsigned char*>(&v);
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest++ = *src++;
	*dest = *src;
#endif
	return stream;
}

int Read7BitEncodedInt(std::istream& stream) {
	// Read out an Int32 7 bits at a time.  The high bit
	// of the byte when on means to continue reading more bytes.
	int count = 0;
	int shift = 0;
	unsigned char b;
	do {
		// Check for a corrupted stream.  Read a max of 5 bytes.
		// In a future version, add a DataFormatException.
		if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
			throw new std::exception("Format_Bad7BitInt32");

		// ReadByte handles end of stream cases for us.
		TBL::TableManager::Read( stream, b);
		count |= (b & 0x7F) << shift;
		shift += 7;
	} while ((b & 0x80) != 0);
	return count;
}
std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;

std::istream& TBL::TableManager::Read(std::istream& stream, std::wstring& data)
{
	int len__ = Read7BitEncodedInt(stream);
	if (len__ < 0)
	{
		std::string err;
		GetTableName(err);
		err += " Table len < 0";
		throw new std::exception(err.c_str());
	}
	if (len__ == 0)
	{
		data = L"";
		return stream;
	}
	if (len__ >= 32767)
	{
		std::string err;
		GetTableName(err);
		err += " Table len >= 32767";
		throw new std::exception(err.c_str());
	}
	std::vector<char> buffer; buffer.resize(len__ + 1);
	stream.read(buffer.data(), len__);
	buffer[len__] = 0x00;

	data = converter.from_bytes(buffer.data());
	return stream;
}

const char* getBZ2Error(int bzError)
{
	if (bzError == BZ_RUN_OK)
	{
		return ": BZ_RUN_OK";
	}
	else if (bzError == BZ_FLUSH_OK)
	{
		return ": BZ_FLUSH_OK";
	}
	else if (bzError == BZ_FINISH_OK)
	{
		return ": BZ_FINISH_OK";
	}
	else if (bzError == BZ_STREAM_END)
	{
		return ": BZ_STREAM_END";
	}
	else if (bzError == BZ_CONFIG_ERROR)
	{
		return ": BZ_CONFIG_ERROR";
	}
	else if (bzError == BZ_SEQUENCE_ERROR)
	{
		return ": BZ_SEQUENCE_ERROR";
	}
	else if (bzError == BZ_PARAM_ERROR)
	{
		return ": BZ_PARAM_ERROR";
	}
	else if (bzError == BZ_MEM_ERROR)
	{
		return ": BZ_MEM_ERROR";
	}
	else if (bzError == BZ_DATA_ERROR)
	{
		return ": BZ_DATA_ERROR";
	}
	else if (bzError == BZ_DATA_ERROR_MAGIC)
	{
		return ": BZ_DATA_ERROR_MAGIC";
	}
	else if (bzError == BZ_IO_ERROR)
	{
		return ": BZ_IO_ERROR";
	}
	else if (bzError == BZ_UNEXPECTED_EOF)
	{
		return ": BZ_UNEXPECTED_EOF";
	}
	else if (bzError == BZ_OUTBUFF_FULL)
	{
		return ": BZ_OUTBUFF_FULL";
	}
	else
	{
		return "";
	}
}

bool TBL::TableManager::Decompress(std::istream& stream, std::vector<char>& bytes)
{
	bool rtn = true;

	unsigned char hashSize;
	
	unsigned int uncompressedSize = 0;
	unsigned int out_uncompressedSize = 0;
	unsigned int compressedSize = 0;
	std::vector<char> hashValue;

	Read(stream, hashSize);

	hashValue.resize(hashSize);

	Read(stream, hashValue.data(), hashSize);
	Read(stream, uncompressedSize);
	Read(stream, compressedSize);

	std::vector<char> compressed;

	{
		compressed.resize(compressedSize);
		Read(stream, compressed.data(), compressedSize);

		/// @TODO: hash compare
		bytes.resize(uncompressedSize);
		int bzError = BZ2_bzBuffToBuffDecompress(bytes.data(), &uncompressedSize, compressed.data(), compressedSize, 0, 0);
		if (bzError != BZ_OK)
		{
			getBZ2Error(bzError);
			return false;
		}
	}
	return true;
}

void TBL::TableManager::GetTableName(std::string& name)
{
	name = std::string("TableManager");
}
