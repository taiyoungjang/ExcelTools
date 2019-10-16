using System;
using System.Collections.Generic;

namespace ClassUtil
{
    public class ExcelImporter : IDisposable
    {
        private string _fileName = String.Empty;
        private string _selectedSheet = String.Empty;
        private string[,] _dt = null;
        private string[] _sheetList = null;
        private ExcelOLEClient _client = null;

        public ExcelImporter()
        {
        }

        public bool Open(String excelFileName)
        {
            try
            {
                _fileName = excelFileName;
                _client = new ExcelOLEClient();
                _client.Open(excelFileName);
                _sheetList = _client.GetSheetList();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _client = null;
                return false;
            }
        }

        public bool ExecSQL(string sql) => false; //_client.runNonQuery(sql);

        public string GetFileName() => _fileName;

        public string[] GetSheetList()
        {
            if (_client == null)
                return null;
            return _sheetList;
        }

        public void SetSheetList(string[] sheets, List<String> checkDupList)
        {
            if (_client == null)
                return;
            //_sheetList = sheets;

            //ExcelSheetMgr.ExcelSheet execlSheet = ExcelSheetMgr.GetUserExcelSheet(_fileName);
            //foreach (string item in checkDupList)
            //{
            //    if (System.IO.Path.GetFileName(_fileName) == item)
            //    {
            //        _client.checkDupliationData(execlSheet);
            //        return;
            //    }
            //}

            //foreach(ExcelSheetMgr.UserExcelSheet sheet in execlSheet.SheetList)
            //{
            //    _client.checkDupliationData(sheet.Name, sheet.IndexKey);
            //}
        }

        protected string[,] GetDataTable(string sheetName, int rowCount, int colCount)
        {
            try
            {
                _dt = _client.GetSheet(sheetName, rowCount, colCount);
                _selectedSheet = sheetName;
            }
            catch (Exception)
            {
                _selectedSheet = String.Empty;
                _dt = null;
                return null;
            }

            return _dt;
        }

        public int GetSheetRowCount(string sheetName) => _client.GetSheetRowCount(sheetName);

        public int GetSheetColumnCount(string sheetName) => _client.GetSheetColumnCount(sheetName);

        public string[] GetSheetOneRow(string sheetName, int oneBaseRowIndex)
        {
            var sheet = _client.GetSheet(sheetName);
            if (sheet == null)
                return null;

            try
            {
                string[] row = new string[sheet.GetLength(1)];
                for (int i = 0; i < sheet.GetLength(1); i++)
                    row[i] = sheet[oneBaseRowIndex, i];
                return row;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string[,] GetSheet(string sheetName, string language)
        {
            int rows = GetSheetRowCount(sheetName);
            return GetSheet(sheetName, rows, language);
        }

        public string[,] GetSheetShortCut(string sheetName, string language)
        {
            return GetSheet(sheetName, 3, language);
        }
        public string[,] GetSheetFormula(string sheetName, int rows, int cols)
        {
            return _client.GetSheetFormula(sheetName, rows, cols);
        }

        public string[,] GetSheet(string sheetName, int rows, string language)
        {
            int cols = GetSheetColumnCount(sheetName);
            return GetSheet(sheetName, rows, cols, language);
        }

        protected string[,] GetSheet(string sheetName, int rowCount, int colCount, string language)
        {
            //language.ToLower();
            var sheet = GetDataTable(sheetName, rowCount, colCount);
            if (sheet == null)
                return null;

            int erase_col_count = 0;
            {
                for (int j = 0; j < sheet.GetLength(1); j++)
                {
                    string col_name = sheet[0, j].Trim();
                    if (col_name.IndexOf("<") >= 0 && col_name.IndexOf(">") >= 0)
                    {
                        if (col_name.IndexOf($"<{language}>") < 0)
                        {
                            erase_col_count++;
                        }
                    }
                }
            }

            string[,] rows = new string[rowCount, colCount - erase_col_count];
            try
            {
                int rows_col_index = 0;
                for (int c = 0; c < colCount; c++)
                {
                    if (sheet[0, c] == null)
                        throw new System.Exception($"sheet[0,{c}]");
                    string col_name = sheet[0, c].Trim();
                    if (col_name.IndexOf("<") >= 0 && col_name.IndexOf(">") >= 0)
                    {
                        if (col_name.IndexOf($"<{language}>") < 0)
                        {
                        }
                        else
                        {
                            rows[0, rows_col_index] = col_name.Substring(0, col_name.IndexOf("<"));
                            rows_col_index++;
                        }
                    }
                    else
                    {
                        rows[0, rows_col_index] = col_name;
                        rows_col_index++;
                    }
                }
                rows_col_index = 0;
                for (int c = 0; c < colCount; c++)
                {
                    if (sheet[0, c] == null)
                        throw new System.Exception($"sheet[0,{c}]");
                    string col_name = sheet[0, c].Trim();
                    if (col_name.IndexOf("<") >= 0 && col_name.IndexOf(">") >= 0)
                    {
                        if (col_name.IndexOf($"<{language}>") < 0)
                        {
                        }
                        else
                        {
                            for (int r = 1; r < rowCount; r++)
                            {
                                if (sheet[r, c] == null)
                                    throw new System.Exception($"sheet[r:{r},{c}]");

                                string data = sheet[r, c].Trim().ToString();
                                rows[r, rows_col_index] = data;
                            }
                            rows_col_index++;
                        }
                    }
                    else
                    {
                        for (int r = 1; r < rowCount; r++)
                        {
                            if (sheet[r, c] == null)
                                continue;
                            string data = sheet[r, c].Trim().ToString();
                            rows[r, rows_col_index] = data;
                        }
                        rows_col_index++;
                    }
                }
                return rows;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("GetSheet: {0}", e.ToString());
                return null;
            }
        }

        public string[,] GetRawSheet(string sheetName, string language)
        {
            int rows = GetSheetRowCount(sheetName);
            return GetRawSheet(sheetName, rows, language);
        }

        public string[,] GetRawSheet(string sheetName, int rows, string language)
        {
            int cols = GetSheetColumnCount(sheetName);
            return GetRawSheet(sheetName, rows, cols, language);
        }

        public string[,] GetRawSheet(string sheetName, int rows, int cols, string language) => GetSheet(sheetName, rows, cols, language);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                    _client.Dispose();
            }
        }
    }
}