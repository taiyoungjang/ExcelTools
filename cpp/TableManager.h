#pragma once
#include <config.h>
#include <iostream>
#include <fstream>
#include <vector>

namespace TBL
{
	class TableManager
	{
	public:
		TableManager();
		~TableManager();

		static bool Decompress(std::istream& stream, std::vector<char>& bytes);

		virtual bool LoadTable(std::istream& stream);

		std::istream& Read(std::istream& stream, std::wstring& data);

		static std::istream& Read(std::istream& stream, unsigned char& data);
		static std::istream& Read(std::istream& stream, unsigned int& data);
		static std::istream& Read(std::istream& stream, bool& data);
		static std::istream& Read(std::istream& stream, short& data);
		static std::istream& Read(std::istream& stream, int& data);
		static std::istream& Read(std::istream& stream, long long& data);
		static std::istream& Read(std::istream& stream, float& data);
		static std::istream& Read(std::istream& stream, double& data);
		static std::istream& Read(std::istream& stream, char* data, int length);
		static std::istream& Read(std::istream& stream, unsigned char* data, int length);

		void GetTableName(std::string& name);
	};

	template<typename CharT, typename TraitsT = std::char_traits<CharT> >
	class vectorwrapbuf : public std::basic_streambuf<CharT, TraitsT> {
	public:
		vectorwrapbuf(std::vector<CharT> &vec) {
			setg(vec.data(), vec.data(), vec.data() + vec.size());
		}
	};
};

