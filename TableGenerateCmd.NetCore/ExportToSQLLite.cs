﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using TableGenerateCmd;

namespace TableGenerate
{
    public class ExportToSQLLite : ExportBase
    {
        //public StreamWriter _writer = null;
        public eGenType _gen_type = eGenType.sqllite;

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            string exec_path = System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName).ToString();

            string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".sql");

            using(var _stream = new MemoryStream())
            {
                var _writer = new StreamWriter(_stream, new System.Text.UTF8Encoding());
                {
                    string filename = System.IO.Path.GetFileName(createFileName);

                    string[] sheets = imp.GetSheetList();

                    filename = filename.Replace(".sql", string.Empty);

                    max = sheets.GetLength(0);
                    current = 0;
                    //_writer.WriteLine("PRAGMA encoding = \"UTF-8\"");
                    _writer.WriteLine("/** this document for SQLLite  ");
                    _writer.WriteLine($"  * generated by {sFileName}*/");
                    //_writer.WriteLine("PRAGMA hexkey=\"0x0102030405060708090a0b0c0d0e0f10\";");
                    _writer.WriteLine(string.Empty);
                    _writer.WriteLine(string.Empty);

                    foreach (string sheetName in sheets)
                    {
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheetShortCut(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        CreateTableProcess(filename, trimSheetName, columns,_writer);
                    }

                    foreach (string sheetName in sheets)
                    {
                        current++;
                        string trimSheetName = sheetName.Trim().Replace(" ", "_");
                        var rows = imp.GetSheet(sheetName, language);
                        var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                        ExportDataProcess(filename, trimSheetName, columns, rows,_writer);
                    }

                    _writer.WriteLine(".quit");
                    _writer.Flush();
                }
                ExportBaseUtil.CheckReplaceFile(_stream, $"{outputPath}/{createFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
            }

            string batch_file = $"{System.IO.Directory.GetCurrentDirectory()}/compile_{createFileName}.bat";

            {
                string db_file = $"{outputPath}/{createFileName.Replace(".sql", string.Empty)}.db";
                if (File.Exists(db_file) == true)
                    File.Delete(db_file);
            }

            //string batch_content = exec_path + "/" + "C#-SQLite3.exe " + createFileName.Replace(".sql", string.Empty) + ".db .read " + createFileName;
            string batch_content = $"{exec_path}/sqlite3.exe {createFileName.Replace(".sql", string.Empty)}.db < {createFileName}";
            using (var batch_stream = System.IO.File.CreateText(batch_file))
            {
                batch_stream.Write(batch_content);
            }

            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo(batch_file);
            startInfo.WorkingDirectory = outputPath;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            StreamReader readerError = process.StandardError;
            process.WaitForExit();
            System.Console.WriteLine(readerError.ReadToEnd());

            //File.Delete(batch_file);
            //File.Delete($"{outputPath}/{createFileName}");

            return true;
        }

        private void CreateTableProcess(string filename, string sheetName, List<Column> columns, StreamWriter _writer)
        {
            _writer.WriteLine($" DROP TABLE IF EXISTS {filename}_{sheetName}; ");
            _writer.WriteLine(string.Empty);
            //_writer.WriteLine("CREATE TABLE " + filename + "_" + sheetName + "(");
            _writer.WriteLine($"CREATE TABLE {sheetName}(");
            InnerCreateTableProcess(columns,_writer);
            _writer.WriteLine(");");
        }

        private void ExportDataProcess(string filename, string sheetName, List<Column> columns, StringWithDesc[,] rows, StreamWriter _writer)
        {
            _writer.WriteLine($"/** start insert {sheetName} */");
            InnerExportDataProcess(columns, rows, filename, sheetName, _writer);
            _writer.WriteLine($"/** end   insert {sheetName} */");
        }

        private void InnerCreateTableProcess( List<Column> columns, StreamWriter _writer)
        {
            foreach (var column in columns.Where( compare => compare.array_index <= 0 ))
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
                        _writer.Write($"  '{column.var_name}{i}' {type} ");
                    }
                }
                else
                {
                    _writer.Write($"  '{column.var_name}' {type} ");
                }
                
                if (column.is_key == true)
                {
                    _writer.Write(" not null primary key asc");
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

                _writer.Write($"  INSERT INTO {sheetName}");

                _writer.Write(" ( ");

                foreach( var column in columns)
                {
                    if (column == null)
                        continue;

                    if (column.is_generated == false)
                        continue;

                    if (column.is_key == false)
                    {
                        _writer.Write(",");
                    }
                    if( column.array_index >= 0 )
                        _writer.Write($"'{column.var_name}{column.array_index}'");
                    else
                        _writer.Write($"'{column.var_name}'");
                }
                _writer.WriteLine(")");

                _writer.Write($" VALUES(" );

                for (int j = 0; j < rows.GetLength(1); j++)
                {
                    var column = columns.FirstOrDefault(compare => compare.data_column_index == j);
                    if( column == null )
                        continue;

                    string data = rows[i, j].Text.Replace("\'", "\'\'");

                    if (column.is_generated == false)
                        continue;

                    if (column.is_key == false)
                    {
                        _writer.Write(",");
                    }

                    if(column.is_array && string.IsNullOrEmpty(data.Trim()) )
                    {
                        _writer.Write("null");
                        continue;
                    }

                    if (column.base_type == eBaseType.String || column.IsDateTime() || column.IsTimeSpan())
                    {
                        _writer.Write("'");
                    }
                    if(column.IsNumberType() == true )
                    {
                        if (column.base_type == eBaseType.Boolean)
                        {
                            if( data.Trim() == string.Empty || data.Trim() == "FALSE" )
                                _writer.Write(0);
                            else 
                                _writer.Write(1);
                        }
                        else if( data == string.Empty)
                        {
                            var default_value = column.GetInitValue(_gen_type);
                            _writer.Write(default_value);
                        }
                        else
                        {
                            _writer.Write(data);
                        }
                    }
                    else
                        _writer.Write(data);

                    if (column.base_type == eBaseType.String || column.IsDateTime() || column.IsTimeSpan() )
                    {
                        _writer.Write("'");
                    }
                }
                _writer.WriteLine(");");
            }
        }
    }
}
