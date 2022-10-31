﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using TableGenerateCmd;

namespace TableGenerate
{
    public class ExportToMSSQL : ExportBase
    {
        //public StreamWriter _writer = null;
        public eGenType _gen_type = eGenType.mssql;

        public override bool Generate(System.Reflection.Assembly[] refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".sql");

            using( var _stream = new MemoryStream(32767))
            {
                var _writer = new StreamWriter(_stream, new System.Text.UTF8Encoding());
                {
                    string filename = System.IO.Path.GetFileName(createFileName);

                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace(".sql", string.Empty);

                    max = sheets.GetLength(0);
                    current = 0;

                    _writer.WriteLine("/** this document for MSSQL  ");
                    _writer.WriteLine($"  * generated by {sFileName}*/");
                    _writer.WriteLine(string.Empty);
                    _writer.WriteLine(string.Empty);

                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        string trimFileName = filename.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        CreateTableProcess(filename, trimSheetName, columns, _writer);
                    }

                    foreach (string sheetName in sheets)
                    {
                        current++;
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        string trimFileName = filename.Trim().Replace(" ", "_");
                        var rows = imp.GetSheet(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        ExportDataProcess(filename, trimSheetName, columns, rows,_writer);
                    }
                }
                ExportBaseUtil.CheckReplaceFile(_stream, $"{outputPath}/{createFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
            }

            return true;
        }

        private void CreateTableProcess(string filename, string sheetName, List<Column> columns, StreamWriter _writer)
        {
            _writer.WriteLine("IF OBJECT_ID(\'[dbo].[" + filename + "_" + sheetName + "]\') IS NOT NULL");
            _writer.WriteLine("  DROP TABLE [dbo].[" + filename + "_" + sheetName + "]; ");
            _writer.WriteLine(string.Empty);
            _writer.WriteLine("CREATE TABLE [dbo].[" + filename + "_" + sheetName + "](");
            InnerCreateTableProcess(columns,_writer);
            _writer.WriteLine(");");
        }

        private void ExportDataProcess(string filename, string sheetName, List<Column> columns, StringWithDesc[,] rows, StreamWriter _writer)
        {
            _writer.WriteLine($"/** start insert [dbo].[{sheetName}] */");
            InnerExportDataProcess(columns, rows, filename, sheetName,_writer);
            _writer.WriteLine($"/** end   insert [dbo].[{sheetName}] */");
            _writer.WriteLine(";");
        }

        private void InnerCreateTableProcess(List<Column> columns, StreamWriter _writer)
        {
            foreach (var column in columns.Where(compare => compare.array_index <= 0))
            {
                string type = column.GenerateType(_gen_type);
                if (column.is_generated == false)
                {
                    continue;
                }

                if (column.is_key == false)
                {
                    _writer.WriteLine(",");
                }

                if (column.array_index >= 0)
                {
                    int array_count = columns.Where(compare => compare.var_name == column.var_name).Max(compare => compare.array_index);
                    for (int i = 0; i <= array_count; i++)
                    {
                        if (i != 0)
                            _writer.Write(",");
                        _writer.Write($"  [{column.var_name}{i}] {type} NOT NULL");
                    }
                }
                else
                {
                    _writer.Write($"  [{column.var_name}] {type} NOT NULL");
                }
            }
            _writer.WriteLine(string.Empty);
        }

        private void InnerExportDataProcess(List<Column> columns, StringWithDesc[,] rows, string filename, string sheetName, StreamWriter _writer)
        {
            for (int i = 3; i < rows.GetLength(0); i++)
            {
                if (rows[i, 0].Text.ToUpper() == "EOF" || rows[i, 0].Text.Length == 0)
                    break;

                _writer.Write($"  INSERT INTO [dbo].[{filename}_{sheetName}]");

                _writer.Write(" ( ");

                foreach (var column in columns)
                {
                    if (column == null)
                        continue;

                    if (column.is_generated == false)
                        continue;

                    if (column.is_key == false)
                    {
                        _writer.Write(",");
                    }
                    if (column.array_index >= 0)
                        _writer.Write($"[{column.var_name}{column.array_index}]");
                    else
                        _writer.Write($"[{column.var_name}]");
                }
                _writer.WriteLine(")");

                _writer.Write(" VALUES" + "(");

                for (int j = 0; j < rows.GetLength(1); j++)
                {
                    var column = columns.FirstOrDefault(compare => compare.data_column_index == j);
                    if (column == null)
                        continue;

                    string data = rows[i, j].Text.Replace("\'", "\'\'");

                    if (column == null || column.is_generated == false)
                        continue;

                    if (j != 0)
                    {
                        _writer.Write(",");
                    }
                    if (column.base_type == eBaseType.String || column.IsDateTime())
                    {
                        _writer.Write("N'");
                    }
                    if(column.IsNumberType() == false )
                        _writer.Write(data);
                    else if (column.IsNumberType() == true)
                    {
                        if( string.IsNullOrEmpty(data) == true )
                        {
                            _writer.Write(column.GetInitValue(_gen_type));
                        }
                        else
                        {
                            _writer.Write(column.GetParseString(data)); 
                        }
                    }
                    if (column.base_type == eBaseType.String || column.IsDateTime())
                    {
                        _writer.Write("'");
                    }
                }
                _writer.WriteLine(");");
            }
        }

    }
}
