using System;
using System.CodeDom.Compiler;
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

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
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
                    writer.WriteLineEx($"#ifdef WITH_EDITOR");
                    writer.WriteLineEx($"#include \"{filename}.h\"");
                    writer.WriteLineEx($"#include \"CoreMinimal.h\"");
                    writer.WriteLineEx($"#include \"UObject/Package.h\"");
                    writer.WriteLineEx($"#include \"UObject/SavePackage.h\"");
                    writer.WriteLineEx($"#include \"EditorAssetLibrary.h\"");
                    writer.WriteLineEx($"#include \"ISourceControlProvider.h\"");
                    writer.WriteLineEx($"#include \"ISourceControlModule.h\"");
                    writer.WriteLineEx($"#include \"SourceControlHelpers.h\"");
                    writer.WriteLineEx($"#include \"SourceControlOperations.h\"");

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
                    writer.WriteLineEx("UTableManager* StaticRegister()");
                    writer.WriteLineEx("{");
                    writer.WriteLineEx("UTableManager* TableManager = new UTableManager;");
                    writer.WriteLineEx("ITableManager::Map.Emplace(TableManager->GetTableName(),TableManager);");
                    writer.WriteLineEx("return TableManager;");
                    writer.WriteLineEx("}");
                    writer.WriteLineEx($"UTableManager* UTableManager::Register = StaticRegister();");
                    writer.WriteLineEx($"bool UTableManager::ConvertToUAsset(FBufferReader& Reader, const FString& Language)");
                    writer.WriteLineEx($"{{");
                    writer.WriteLineEx($"auto bRtn = true;");
                    writer.WriteLineEx($"TArray<uint8> Bytes;");
                    writer.WriteLineEx($"if(!FBufferReader::Decompress( Reader, Bytes )) return false;");
                    writer.WriteLineEx($"FBufferReader BufferReader( Bytes.GetData(), Bytes.Num() );");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        writer.WriteLineEx($"const U{trimSheetName}TableManager {trimSheetName}TableManager;");
                    }
                    writer.WriteLineEx($"");
                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        writer.WriteLineEx($"bRtn &= {trimSheetName}TableManager.ConvertToUAsset( BufferReader, Language);");
                    }
                    writer.WriteLineEx($"return bRtn;");
                    writer.WriteLineEx($"}};");
                    writer.WriteLineEx($"}}");
                    writer.WriteLineEx($"#endif");
                    writer.Flush();
                }
                ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
            }

            return true;
        }

        private void SheetProcess(string filename, string sheetName, List<Column> columns, StreamWriter writer)
        {
            writer.WriteLineEx($"class U{sheetName}TableManager final");
            writer.WriteLineEx("{");
            writer.WriteLineEx("public:");
            writer.WriteLineEx($"U{sheetName}TableManager(void) = default;");
            writer.WriteLineEx($"virtual ~U{sheetName}TableManager(void) = default;");
            writer.WriteLineEx($"static bool Equals(const F{sheetName}TableRow& Left, const F{sheetName}TableRow& Right)");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"return ");
            writer.Write(string.Join($" &&{Environment.NewLine}", columns.Where(t => !t.is_key && t.is_generated && t.array_index <= 0).Select(t => $"      Left.{t.var_name} == Right.{t.var_name}")));
            writer.WriteLineEx($"");
            writer.WriteLineEx(";}");
            writer.WriteLineEx($"bool ConvertToUAsset(FBufferReader& Reader, const FString& Language) const");
            writer.WriteLineEx("{");
            string packageName = $"TEXT(\"{sheetName}\")";
            writer.WriteLineEx($"const FString PackageName = *FPaths::Combine(TEXT(\"/Game/Data\"), Language, {packageName});");
            writer.WriteLineEx($"const FString FileName = *FPackageName::LongPackageNameToFilename(PackageName, FPackageName::GetAssetPackageExtension());");
            writer.WriteLineEx($"UPackage* Package = nullptr;");
            writer.WriteLineEx($"UDataTable* DataTable = nullptr;");
            writer.WriteLineEx($"bool bNeedSave = false;");
            writer.WriteLineEx($"if( FPaths::FileExists(*FileName) )");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"DataTable = Cast<UDataTable>( UEditorAssetLibrary::LoadAsset(*PackageName) );");
            writer.WriteLineEx($"Package = DataTable->GetPackage();");
            writer.WriteLineEx($"UE_LOG(LogLevel, Log, TEXT(\"{sheetName}.uasset LoadAsset\"));");
            writer.WriteLineEx( "}");
            writer.WriteLineEx($"else");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"bNeedSave |= true;");
            writer.WriteLineEx($"Package = CreatePackage( *PackageName );");
            writer.WriteLineEx($"DataTable = NewObject<UDataTable>(Package, UDataTable::StaticClass(), {packageName}, RF_Public | RF_Standalone );");
            writer.WriteLineEx($"DataTable->RowStruct = F{sheetName}TableRow::StaticStruct();");
            writer.WriteLineEx($"UE_LOG(LogLevel, Log, TEXT(\"{sheetName}.uasset CreatePackage\"));");
            writer.WriteLineEx( "}");
            
            InnerSheetProcess(filename, sheetName, columns, writer);
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"auto DataTableNames = DataTable->GetRowNames();");
            writer.WriteLineEx($"bNeedSave |= ItemMap.Num() != DataTableNames.Num();");
            writer.WriteLineEx($"if(!bNeedSave)");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"for(auto DataTableName: DataTableNames)");
            writer.WriteLineEx( "{");
            writer.WriteLineEx( $"DataTable->ForeachRow<F{sheetName}TableRow>( DataTableName.ToString(), [ &bNeedSave, &ItemMap ](const FName& Key, const F{sheetName}TableRow& Left)");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"const auto Right = ItemMap.Find(Key);");
            writer.WriteLineEx($"if( Right == nullptr )");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"bNeedSave |= true;");
            writer.WriteLineEx($"return;");
            writer.WriteLineEx( "}");
            writer.WriteLineEx( "bNeedSave |= !Equals(Left,*Right);");            
            writer.WriteLineEx( "}");
            writer.WriteLineEx( ");");
            writer.WriteLineEx( "if(bNeedSave) break;");
            writer.WriteLineEx( "}");
            writer.WriteLineEx( "};");
            writer.WriteLineEx( "}");
            writer.WriteLineEx($"if(!bNeedSave) return true;");
            writer.WriteLineEx($"if( FPaths::FileExists(*FileName) )");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"if( IFileManager::Get().IsReadOnly(*FileName) )");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"if(!USourceControlHelpers::CheckOutFile(FileName))");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"UE_LOG(LogLevel, Error, TEXT(\"{sheetName}.uasset CheckOut Fail\"));");
            writer.WriteLineEx($"return false;");
            writer.WriteLineEx("}");            
            writer.WriteLineEx("}");
            writer.WriteLineEx("}");
            writer.WriteLineEx($"DataTable->EmptyTable();");
            writer.WriteLineEx($"DataTable->RowStruct = F{sheetName}TableRow::StaticStruct();");
            writer.WriteLineEx("for(auto Pair : ItemMap)");
            writer.WriteLineEx("{");
            writer.WriteLineEx($"DataTable->AddRow(Pair.Key,Pair.Value);");
            writer.WriteLineEx("}");
            writer.WriteLineEx($"FSavePackageArgs SavePackageArgs;");
            writer.WriteLineEx($"SavePackageArgs.TopLevelFlags = RF_Standalone;");
            writer.WriteLineEx($"const bool bRtn = UPackage::SavePackage(Package, nullptr, *FileName, SavePackageArgs);");
            writer.WriteLineEx($"if( bRtn )");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"UE_LOG(LogLevel, Log, TEXT(\"{sheetName}.uasset SavePackage Success\"));");
            writer.WriteLineEx( "}");
            writer.WriteLineEx($"else");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"UE_LOG(LogLevel, Error, TEXT(\"{sheetName}.uasset SavePackage Failure\"));");
            writer.WriteLineEx( "}");
            writer.WriteLineEx($"return bRtn;");
            writer.WriteLineEx("}");
            writer.WriteLineEx("};");
        }

        private void InnerSheetProcess(string fileName, string sheetName, List<Column> columns, StreamWriter writer)
        {
            var keyColumn = columns.FirstOrDefault(compare => compare.is_key);
            string primaryName = keyColumn.var_name;

            writer.WriteLineEx($"int32 Count = 0;");
            writer.WriteLineEx($"Reader >> Count;");

            writer.WriteLineEx($"if(Count == 0) return true;");
            
            string structName =$"F{sheetName}TableRow"; 
            writer.WriteLineEx($"TMap<FName,{structName}> ItemMap;");
            writer.WriteLineEx($"for(auto Idx=0;Idx<Count;++Idx)");
            writer.WriteLineEx( "{");
            writer.WriteLineEx($"{structName} Item;");
            writer.WriteLineEx( $"{keyColumn.base_type.GenerateBaseType(this._gen_type)} {keyColumn.var_name};");
            foreach (var column in columns)
            {
                if (column.is_generated == false)
                    continue;
                if (column.array_index > 0)
                    continue;
                if (column.is_key)
                {
                    writer.WriteLineEx($"Reader >> {column.var_name};");
                    continue;
                }
                if (column.array_index == 0)
                {
                    writer.WriteLineEx("{");
                    writer.WriteLineEx($"const int32 ArrayCount = FBufferReader::Read7BitEncodedInt(Reader);");
                    writer.WriteLineEx($"Item.{column.var_name}.SetNum(ArrayCount,true);");
                    writer.WriteLineEx($"for(int32 ArrayIndex=0;ArrayIndex<ArrayCount;++ArrayIndex)");
                    writer.WriteLineEx("{");
                    switch (column.base_type)
                    {
                        case eBaseType.Boolean:
                            writer.WriteLineEx($"  {{ {column.base_type.GenerateBaseType(this._gen_type)} element__; Reader >> element__; Item.{column.var_name}[ArrayIndex] = element__; }}");
                            break;
                        case eBaseType.TimeSpan:
                            writer.WriteLineEx($"  {{ int64 element__; Reader >> element__; Item.{column.var_name}[ArrayIndex] = ({column.base_type.GenerateBaseType(this._gen_type)}) element__ / 10000000; }}");
                            break;
                        case eBaseType.DateTime:
                            writer.WriteLineEx($"  {{ int64 element__; Reader >> element__; Item.{column.var_name}[ArrayIndex] = ({column.base_type.GenerateBaseType(this._gen_type)}) (element__ - 621355968000000000) / 10000000; }}");
                            break;
                        case eBaseType.Struct:
                        case eBaseType.Enum:
                            writer.WriteLineEx($"Reader >> ({column.primitive_type.GenerateBaseType(this._gen_type)}&) Item.{column.var_name}[ArrayIndex];");
                            break;
                        default:
                            writer.WriteLineEx($"Reader >> Item.{column.var_name}[ArrayIndex];");
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
                            writer.WriteLineEx($"  {{int64 element__; Reader >> element__; Item.{column.var_name} = ({column.GenerateType(this._gen_type)}) element__ / 10000000; }}");
                            break;
                        case eBaseType.DateTime:
                            writer.WriteLineEx($"  {{int64 element__; Reader >> element__; Item.{column.var_name} = ({column.GenerateType(this._gen_type)}) (element__ - 621355968000000000) / 10000000; }}");
                            break;
                        case eBaseType.Struct:
                        case eBaseType.Enum when !column.bit_flags:
                            writer.WriteLineEx($"Reader >> reinterpret_cast<{column.primitive_type.GenerateBaseType(this._gen_type)}&>(Item.{column.var_name});");
                            break;
                        default:
                            writer.WriteLineEx($"Reader >> Item.{column.var_name};");
                            break;
                    }
                }
            }

            switch (keyColumn.base_type)
            {
                case eBaseType.String:
                    writer.WriteLineEx($"const FName Key = {primaryName};");
                    break;
                case eBaseType.Int32:
                default:
                    writer.WriteLineEx($"const FName Key = *FString::FromInt({primaryName});");
                    break;
            }
            writer.WriteLineEx($"ItemMap.Add(Key, Item);");
            writer.WriteLineEx($"}}");
        }

    }
}
