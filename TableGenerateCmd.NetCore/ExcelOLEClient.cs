using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using System.IO;
using DocumentFormat.OpenXml;
using TableGenerateCmd;

namespace ClassUtil
{
    public class ExcelSheetMgr
    {
        protected static List<ExcelSheet> m_SheetMgr = new List<ExcelSheet>();

        public static void AddExcelSheet(string fileName, ExcelOLEClient client)
        {
            m_SheetMgr.Add(new ExcelSheet(fileName, client.GetSheetListWithIndex()));
        }

        public static void Clear()
        {
            m_SheetMgr.Clear();
        }

        public static string[] GetExcelSheet(string fileName)
        {
            foreach (ExcelSheet item in m_SheetMgr)
            {
                if (item.ExcelFileName == fileName)
                {
                    string[] sheets = new string[item.SheetList.Count];
                    for (int i = 0; i < item.SheetList.Count; i++)
                        sheets[i] = item.SheetList[i].Name;
                    return sheets;
                }
            }
            return null;
        }

        public static ExcelSheet GetUserExcelSheet(string fileName)
        {
            foreach (ExcelSheet item in m_SheetMgr)
            {
                if (item.ExcelFileName == fileName)
                {
                    return item;
                }
            }
            return null;
        }

        public static bool Exists(string fileName)
        {
            foreach (ExcelSheet item in m_SheetMgr)
                if (item.ExcelFileName == fileName)
                    return true;

            return false;
        }

        public static string CheckTag(string tag)
        {
            foreach (ExcelSheet item in m_SheetMgr)
                if (item.Tag == tag)
                    return item.ExcelFileName;

            return String.Empty;
        }

        public class UserExcelSheet
        {
            public string Name;
            public string IndexKey;

            public UserExcelSheet(string name, string key)
            {
                this.Name = name;
                IndexKey = key;
            }

            public UserExcelSheet()
            {
                Name = String.Empty;
                IndexKey = String.Empty;
            }
        };

        public class ExcelSheet
        {
            public string ExcelFileName;
            public string Tag;
            public List<UserExcelSheet> SheetList = new List<UserExcelSheet>();

            public ExcelSheet(string fileName, UserExcelSheet[] userExcelSheet)
            {
                ExcelFileName = fileName;
                foreach (UserExcelSheet item in userExcelSheet)
                {
                    SheetList.Add(item);
                }
            }
        }
    }

    public class ExcelOLEClient : IDisposable
    {
        private DocumentFormat.OpenXml.Packaging.SpreadsheetDocument _excel_file;
        private WorkbookPart workbookPart;
        private IEnumerable<SharedStringItem> sharedStringItem;
        private MemoryStream _stream = null;
        private static readonly Comment s_emptyComment = new Comment(){CommentText = new CommentText(string.Empty)}; 

        private string _fileName;
        //private int maxRowReadCount = 3;

        public ExcelOLEClient()
        {
            try
            {
                _excel_file = null;
                workbookPart = null;
                sharedStringItem = null;
                //_excel_file.Visible = false;
                //_excel_file.DisplayAlerts = false; // 저장할 것인가 확인하지 않도록 설정
                //workbooks = _excel_file.Worksheets;
            }
            catch (Exception)
            {
                //Dispose();
            }
        }

        public bool Open(String excelFileName)
        {
            _fileName = excelFileName;
            byte[] bytes = System.IO.File.ReadAllBytes(excelFileName);
            _stream = new MemoryStream(bytes);
            _excel_file = SpreadsheetDocument.Open(_stream, false);
            workbookPart = _excel_file.WorkbookPart;
            sharedStringItem = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>();
            return true;
        }

        public bool OpenWithForm(String excelFileName)
        {
            if (!Open(excelFileName))
                return false;
            return true;
        }

        public bool SaveAsTextData() => true;

        public string[] GetSheetList()
        {
            string[] sheetList = workbookPart.Workbook.Descendants<Sheet>().Select(t => t.Name.ToString()).ToArray();
            sheetList = sheetList.Where(compare => compare.Contains('$') == false).ToArray();
            return sheetList;
        }

        private int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();

            char[] colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            var convertedValue = 0;
            for (int i = 0; i < colLetters.Length; i++)
            {
                char letter = colLetters[i];
                // ASCII 'A' = 65
                int current = i == 0 ? letter - 65 : letter - 64;
                convertedValue += current * (int)Math.Pow(26, i);
            }

            return convertedValue;
        }

        private string GetColumnName(string cellReference)
        {
            var regex = new Regex("[A-Za-z]+");
            var match = regex.Match(cellReference);

            return match.Value;
        }
        private IEnumerator<Cell> GetExcelCellEnumerator(Row row)
        {
            int currentCount = 0;
            foreach (Cell cell in row.Descendants<Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);

                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    var emptycell = new Cell()
                    {
                        DataType = null,
                        CellValue = new CellValue(string.Empty)
                    };
                    yield return emptycell;
                }

                yield return cell;
                currentCount++;
            }
        }
        private string ReadExcelCell(Cell cell)
        {
            var cellValue = cell.CellValue;
            var text = (cellValue == null) ? cell.InnerText : cellValue.Text;
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                text = sharedStringItem.ElementAt(Convert.ToInt32(cell.CellValue.Text)).InnerText;
            }
            else
            {
                text = cell.CellValue?.Text;
            }

            if (!string.IsNullOrEmpty(text) && Regex.Match(text, "^[0-9]*[.][0-9]*$").Success)
            {
                double out_double = 0;
                if (double.TryParse(text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out out_double))
                    text = out_double.ToString();
            }

            if (text == null)
            {
                text = string.Empty;
            }

            return (text ?? string.Empty).Trim();
        }
        private string ReadExcelCellFormula(Cell cell)
        {
            if (cell.CellFormula != null)
                return cell.CellFormula.Text;

            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                return sharedStringItem.ElementAt(Convert.ToInt32(cell.CellValue.Text)).InnerText;
            }
            return cell.CellValue?.Text;
        }

        public ExcelSheetMgr.UserExcelSheet[] GetSheetListWithIndex()
        {
            ExcelSheetMgr.UserExcelSheet[] sheetList = new ExcelSheetMgr.UserExcelSheet[workbookPart.WorksheetParts.Count()];
            var sheets = workbookPart.Workbook.Descendants<Sheet>().ToList();
            for (int i = 0; i < sheetList.Count(); i++)
            {
                try
                {
                    var sheet = sheets[i];
                    var workSheet = (workbookPart.GetPartById(sheet.Id) as WorksheetPart).Worksheet;
                    var user_sheet = new ExcelSheetMgr.UserExcelSheet();
                    user_sheet.Name = sheet.Name.ToString();
                    var sheetData = workSheet.Elements<SheetData>().First();
                    var rows = sheetData.Elements<Row>().ToList();
                    if (!rows.Any())
                        throw new Exception("no data");
                    var cellEnumerator = GetExcelCellEnumerator(rows[0]);
                    while (cellEnumerator.MoveNext())
                    {
                        var cell = cellEnumerator.Current;
                        var text = ReadExcelCell(cell).Trim();
                        user_sheet.IndexKey = text;
                        break;
                    }
                    sheetList[i] = user_sheet;
                }
                catch (Exception)
                {
                    return null;
                }
                //finally
                //{
                //    if (sheet != null)
                //        Marshal.ReleaseComObject(sheet);
                //}
            }
            return sheetList;
        }

        public int GetSheetRowCount(string sheetName)
        {
            try
            {
                var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(t => t.Name.ToString() == sheetName);
                var workSheet = ((WorksheetPart)workbookPart
                        .GetPartById(sheet.Id)).Worksheet;
                var sheetData = workSheet.Elements<SheetData>().First();
                var rows = sheetData.Elements<Row>().Count();
                return rows;
            }
            catch (System.Exception)
            {
                throw new System.Exception($"no exist sheet!! \"{sheetName}\"");
            }
        }

        public int GetSheetColumnCount(string sheetName)
        {
            var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(t => t.Name.ToString() == sheetName);
            var workSheet = ((WorksheetPart)workbookPart
                    .GetPartById(sheet.Id)).Worksheet;
            var sheetData = workSheet.Elements<SheetData>().First();
            var rows = sheetData.Elements<Row>().ToList();

            int count = 0;
            try
            {
                if (rows.Any())
                {
                    var row = rows.First();
                    var cellEnumerator = GetExcelCellEnumerator(row);
                    while (cellEnumerator.MoveNext())
                    {
                        var cell = cellEnumerator.Current;
                        var text = ReadExcelCell(cell).Trim();
                        if(string.IsNullOrEmpty(text))
                            continue;
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                return -1;
            }
            return count;
        }

        public StringWithDesc[,] GetSheet(string sheetName)
        {
            int rows = GetSheetRowCount(sheetName);
            return GetSheet(sheetName, rows);
        }

        public StringWithDesc[,] GetSheet(string sheetName, int rows)
        {
            int cols = GetSheetColumnCount(sheetName);
            return GetSheet(sheetName, rows, cols);
        }

        public StringWithDesc[,] GetSheet(string sheetName, int rowCount, int cols)
        {
            StringWithDesc[,] rowList = new StringWithDesc[rowCount, cols];

            var columnDescDict = GetColumnDescDict(sheetName, cols);

            var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(t => t.Name?.ToString() == sheetName);
            var workSheet = ((WorksheetPart)workbookPart
                    .GetPartById(sheet.Id)).Worksheet;

            var sheetData = workSheet.Elements<SheetData>().First();
            var rows = sheetData.Elements<Row>().ToList();

            var i = 0;
            var j = 0;
            try
            {
                for (i = 0; i < rowCount; i++)
                {
                    var row = rows[i];
                    var cellEnumerator = GetExcelCellEnumerator(row);
                    for (j = 0; j < cols; j++)
                    {
                        if (cellEnumerator.MoveNext() )
                        {
                            var cell = cellEnumerator.Current;
                            var text = ReadExcelCell(cell).Trim();
                            if (
                                cell != null && 
                                cell.CellReference != null &&
                                !string.IsNullOrEmpty(cell.CellReference.Value) &&
                                columnDescDict.TryGetValue(cell.CellReference.Value, out var columnDesc))
                            {
                                rowList[i, j] = new StringWithDesc(Text: text,Desc: columnDesc.Desc);
                            }
                            else
                            {
                                rowList[i, j] = new StringWithDesc(Text: text,Desc: string.Empty);
                            }
                            
                        }
                        else
                        {
                            rowList[i, j] = new StringWithDesc(Text: string.Empty,Desc: string.Empty);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"{sheetName} i:{i} j:{j}");
                return null;
            }
            return rowList;
        }
        private Dictionary<string,ColumnDesc> GetColumnDescDict(string sheetName, int cols)
        {
            var descList = new ColumnDesc[cols];

            var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(t => t.Name?.ToString() == sheetName);
            var workSheet = ((WorksheetPart)workbookPart
                    .GetPartById(sheet.Id)).Worksheet;

            var sheetData = workSheet.Elements<SheetData>().First();
            var rows = sheetData.Elements<Row>().ToList();

            var i = 0;
            var j = 0;
            try
            {
                for (i = 0; i < 5; i++)
                {
                    var row = rows[i];
                    var cellEnumerator = GetExcelCellEnumerator(row);
                    for (j = 0; j < cols; j++)
                    {
                        descList[j] ??= new ColumnDesc();
                        var columnDesc = descList[j];
                        if (cellEnumerator.MoveNext() )
                        {
                            var cell = cellEnumerator.Current;
                            var text = ReadExcelCell(cell).Trim();
                            if (
                                cell != null && 
                                cell.CellReference != null &&
                                !string.IsNullOrEmpty(cell.CellReference.Value) )
                            {
                                switch (i)
                                {
                                    case 0: columnDesc.Desc = text;
                                        break;
                                    case 1: columnDesc.Name = text;
                                        break;
                                    case 2: columnDesc.Json = text;
                                        break;
                                    case 3: columnDesc.IsGenerated = text;
                                        break;
                                    case 4: columnDesc.Type = text;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"{sheetName} i:{i} j:{j}");
                return null;
            }

            Dictionary<string, ColumnDesc> ret = new ();
            foreach (var columnDesc in descList)
            {
                if (!ret.ContainsKey(columnDesc.Name))
                {
                    ret.Add(columnDesc.Name, columnDesc);
                }
            }
            return ret;
        }
        public string[,] GetSheetFormula(string sheetName, int rowCount, int cols)
        {
            string[,] rowList = new string[rowCount, cols];

            var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(t => t.Name.ToString() == sheetName);
            var workSheet = ((WorksheetPart)workbookPart
                    .GetPartById(sheet.Id)).Worksheet;

            var sheetData = workSheet.Elements<SheetData>().First();
            var rows = sheetData.Elements<Row>().ToList();

            int i = 0;
            int j = 0;
            try
            {
                for (i = 0; i < rowCount; i++)
                {
                    var row = rows[i];
                    var cellEnumerator = GetExcelCellEnumerator(row);
                    for (j = 0; j < cols; j++)
                    {
                        if (cellEnumerator.MoveNext())
                        {
                            var cell = cellEnumerator.Current;
                            var text = ReadExcelCellFormula(cell).Trim();
                            rowList[i, j] = text;
                        }
                        else
                        {
                            rowList[i, j] = string.Empty;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"{sheetName} i:{i} j:{j}");
                return null;
            }
            return rowList;
        }
        public string[,] GetRawSheet(string sheetName)
        {
            int rows = GetSheetRowCount(sheetName);
            return GetRawSheet(sheetName, rows);
        }

        public string[,] GetRawSheet(string sheetName, int rows)
        {
            int cols = GetSheetColumnCount(sheetName);
            return GetRawSheet(sheetName, rows, cols);
        }

        public string[,] GetRawSheet(string sheetName, int rowCount, int cols)
        {
            string[,] rowList = new string[rowCount, cols];

            var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(t => t.Name.ToString() == sheetName);
            var workSheet = ((WorksheetPart)workbookPart
                    .GetPartById(sheet.Id)).Worksheet;

            var sheetData = workSheet.Elements<SheetData>().First();
            var rows = sheetData.Elements<Row>();

            int i = 0;
            int j = 0;
            try
            {
                foreach (var row in rows)
                {
                    var cellEnumerator = GetExcelCellEnumerator(row);
                    for (j = 0; j < cols; j++)
                    {
                        if (cellEnumerator.MoveNext())
                        {
                            var cell = cellEnumerator.Current;
                            var text = ReadExcelCell(cell).Trim();
                            rowList[i, j] = text;
                        }
                        else
                        {
                            rowList[i, j] = string.Empty;
                        }
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{sheetName} i:{i} j:{j} {e.Message}");
                return null;
            }
            return rowList;
        }
        #region IDisposable 멤버

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_excel_file != null)
                {
                    _excel_file.Close();
                    _excel_file.Dispose();
                    _excel_file = null;
                }
                _stream?.Dispose();
            }
        }


        #endregion

    }
}
