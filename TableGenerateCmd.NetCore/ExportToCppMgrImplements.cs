﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StreamWrite.Extension;

namespace TableGenerate
{
    public class ExportToCppMgrImplements : ExportBase
    {
        //public StreamWriter _writer = null;
        public eGenType _gen_type = eGenType.cpp;

        public override bool Generate(System.Reflection.Assembly[] refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", "TableManager.cpp");

            using MemoryStream stream = new();
            {
                StreamWriter writer = new (stream, Encoding.UTF8);
                {

                    string filename = System.IO.Path.GetFileName(createFileName);

                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace(".cpp", string.Empty);

                    //            writer.WriteLine("#include <Base/properties.h>");
                    //            writer.WriteLine("#include <Base/service.h>");

                    writer.WriteLineEx($"#include \"{filename}.h\"");
                    string fn = filename.Replace("TableManager", string.Empty);
                    
                    writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}::{filename.Replace(" ", "_").Replace("TableManager", string.Empty)}");
                    writer.WriteLineEx($"{{");

                    max = sheets.GetLength(0);
                    current = 0;
                    foreach (string sheetName in sheets)
                    {
                        current++;
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        SheetProcess(filename, trimSheetName, columns,writer);
                        //GetItemProcess(filename, trimSheetName, columns);
                        //AddItemMapProcess(filename, trimSheetName, imp.GetSheetShortCut(sheetName));
                        //AddArrayToMapProcess(filename, trimSheetName, columns);
                    }

                    //ManagerProcess(filename, sheets);
                    writer.WriteLineEx($"bool FTableManager::LoadTable(FBufferReader& Reader_, TMap<FName,UDataTable*>& DataTableMap)");
                    writer.WriteLineEx($"{{");
                    writer.WriteLineEx($"auto bRtn = true;");
                    writer.WriteLineEx($"TArray<uint8> Bytes_;");
                    writer.WriteLineEx($"if({ExportToCSMgr.NameSpace}::FBufferReader::Decompress(Reader_,Bytes_)==false) return false;");
                    writer.WriteLineEx($"FBufferReader BufferReader((uint8*)Bytes_.GetData(),(int32)Bytes_.Num());");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        writer.WriteLineEx($"F{trimSheetName}TableManager {trimSheetName}TableManager;");
                    }
                    writer.WriteLineEx($"");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        writer.WriteLineEx($"bRtn &= {trimSheetName}TableManager.LoadTable(BufferReader, DataTableMap);");
                    }
                    writer.WriteLineEx($"return bRtn;");
                    writer.WriteLineEx($"}};");
                    writer.WriteLineEx($"}}");
                    writer.Flush();
                }
                ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
            }

            return true;
        }

        private void SheetProcess(string filename, string sheetName, List<Column> columns, StreamWriter writer)
        {
            writer.WriteLineEx($"class F{sheetName}TableManager final");
            writer.WriteLineEx("{");
            writer.WriteLineEx("public:");
            writer.WriteLineEx($"  F{sheetName}TableManager(void){{}}");
            writer.WriteLineEx($"virtual ~F{sheetName}TableManager(void) = default;");
            writer.WriteLineEx($"bool LoadTable(FBufferReader& Reader_, TMap<FName,UDataTable*>& DataTableMap)");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"UDataTable* DataTable = NewObject<U{filename.Replace("TableManager",string.Empty)}_{sheetName}DataTable>();");
            InnerSheetProcess(filename, sheetName, columns, writer);
            writer.WriteLineEx($"DataTableMap.Emplace(TEXT(\"{filename.Replace("TableManager",string.Empty)}_{sheetName}\"), DataTable);");
            writer.WriteLineEx("return true;");
            writer.WriteLineEx("}");
            writer.WriteLineEx("};");
        }

        private void GetItemProcess(string sheetName, List<Column> columns, StreamWriter writer)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key == true);
            string primaryType = keyColumn.GenerateType(_gen_type);

            writer.WriteLineEx($"const {sheetName}Ptr {sheetName}::GetItem( const {primaryType}& id )");
            writer.WriteLineEx($"{{");
            writer.WriteLineEx($"  Map::const_iterator it = m_Map.find( id );");
            writer.WriteLineEx($"  if( it != m_Map.end() )");
            writer.WriteLineEx($"    return it->second;");
            writer.WriteLineEx($"  return nullptr;");
            writer.WriteLineEx($"}}");
        }
        
        private void InnerSheetProcess(string fileName, string sheetName, List<Column> columns, StreamWriter writer)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key);
            string primaryName = "" + keyColumn.var_name;

            writer.WriteLineEx($"int32 Count = 0;");
            writer.WriteLineEx($"Reader_ >> Count;");

            writer.WriteLineEx($"if(Count == 0) return true;");
            
            writer.WriteLineEx($"F{fileName.Replace("TableManager",string.Empty)}_{sheetName} Item;");
            writer.WriteLineEx($"for(auto Idx=0;Idx<Count;++Idx)");
            writer.WriteLineEx($"{{");
            foreach (var column in columns)
            {
                if (column.is_generated == false)
                    continue;
                if (column.array_index > 0)
                    continue;
                if (column.array_index == 0)
                {
                    writer.WriteLineEx("{");
                    writer.WriteLineEx($"auto ArrayCount = FBufferReader::Read7BitEncodedInt(Reader_);");
                    writer.WriteLineEx($"Item.{column.var_name}.SetNum(ArrayCount,true);");
                    writer.WriteLineEx($"for(auto ArrayIndex_=0;ArrayIndex_<ArrayCount;++ArrayIndex_)");
                    writer.WriteLineEx("{");
                    switch (column.base_type)
                    {
                        case eBaseType.Boolean:
                            writer.WriteLineEx($"  {{ {column.base_type.GenerateBaseType(this._gen_type)} element__; Reader_ >> element__; Item.{column.var_name}[ArrayIndex_] = element__; }}");
                            break;
                        case eBaseType.TimeSpan:
                            writer.WriteLineEx($"  {{ int64 element__; Reader_ >> element__; Item.{column.var_name}[ArrayIndex_] = ({column.base_type.GenerateBaseType(this._gen_type)}) element__ / 10000000; }}");
                            break;
                        case eBaseType.DateTime:
                            writer.WriteLineEx($"  {{ int64 element__; Reader_ >> element__; Item.{column.var_name}[ArrayIndex_] = ({column.base_type.GenerateBaseType(this._gen_type)}) (element__ - 621355968000000000) / 10000000; }}");
                            break;
                        case eBaseType.Struct:
                        case eBaseType.Enum:
                            writer.WriteLineEx($"Reader_ >> ({column.primitive_type.GenerateBaseType(this._gen_type)}&) Item.{column.var_name}[ArrayIndex_];");
                            break;
                        default:
                            writer.WriteLineEx($"Reader_ >> Item.{column.var_name}[ArrayIndex_];");
                            break;
                    }
                    writer.WriteLineEx("}");
                    writer.WriteLineEx("}");
                }
                else
                {
                    switch (column.base_type)
                    {
                        case eBaseType.TimeSpan:
                            writer.WriteLineEx($"  {{int64 element__; Reader_ >> element__; Item.{column.var_name} = ({column.GenerateType(this._gen_type)}) element__ / 10000000; }}");
                            break;
                        case eBaseType.DateTime:
                            writer.WriteLineEx($"  {{int64 element__; Reader_ >> element__; Item.{column.var_name} = ({column.GenerateType(this._gen_type)}) (element__ - 621355968000000000) / 10000000; }}");
                            break;
                        // case eBaseType.Struct:
                        // case eBaseType.Enum:
                        //     writer.WriteLineEx($"Reader_ >> reinterpret_cast<{column.primitive_type.GenerateBaseType(this._gen_type)}&>(Item.{column.var_name});");
                        //     break;
                        default:
                            writer.WriteLineEx($"Reader_ >> Item.{column.var_name};");
                            break;
                    }
                }
            }

            switch (keyColumn.base_type)
            {
                case eBaseType.String:
                    writer.WriteLineEx($"FName Key = Item.{primaryName};");
                    break;
                default:
                    writer.WriteLineEx($"FName Key; Key.SetNumber(Item.{primaryName});");
                    break;
            }
            writer.WriteLineEx($"DataTable->AddRow(Key,Item);");
            writer.WriteLineEx($"}}");
        }

    }
}
