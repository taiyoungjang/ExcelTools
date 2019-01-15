#region Imports

using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;

#endregion

/// <summary>
/// A client which interfaces to excel work books
/// </summary>
/// 
namespace ClassUtil
{

    public class MicrosoftExcelClient
    {

        public List<String> m_SheetList = new List<string>();
        public static List<String> m_Logs = new List<string>();

        #region Variable Declarations

        /// <summary>
        /// Current message from the client
        /// </summary>
        string m_CurrentMessage = String.Empty;

        /// <summary>
        /// The file path for the source excel book
        /// </summary>
        string m_SourceFileName;

        /// <summary>
        /// Connects to the source excel workbook
        /// </summary>
        OleDbConnection m_ConnectionToExcelBook;

        /// <summary>
        /// Reads the data from the document to a System.Data object
        /// </summary>
        OleDbDataAdapter m_AdapterForExcelBook;

        #endregion

        #region Constructor Logic

        /// <summary>
        /// Parameterized constructor .. specifies path
        /// </summary>
        /// <param name="iSourceFileName">The source filename</param>
        public MicrosoftExcelClient(string iSourceFileName)
        {

            this.m_SourceFileName = iSourceFileName;

        }


        #endregion

        #region Properties

        /// <summary>
        /// Gets he current messages from the client
        /// </summary>
        public string CurrentMessage => this.m_CurrentMessage;

        /// <summary>
        /// Property that gets / sets the current source excel book
        /// </summary>
        public string FileName
        {

            get
            {
                return this.m_SourceFileName;
            }
            set
            {
                this.m_SourceFileName = value;
            }

        }

        #endregion

        #region Methods

        /// <summary>
        /// Runs a non query database command such as UPDATE , INSERT or DELETE
        /// </summary>
        /// <param name="iQuery">The required query</param>
        /// <returns></returns>
        public bool runNonQuery(string iQuery)
        {
            try
            {
                OleDbCommand nonQueryCommand = new OleDbCommand(iQuery);

                nonQueryCommand.Connection = this.m_ConnectionToExcelBook;
                nonQueryCommand.CommandText = iQuery;

                int rowsAffected = nonQueryCommand.ExecuteNonQuery();

                this.m_CurrentMessage = "SUCCESS - " + rowsAffected.ToString() + " Rows affected ";

                return true;
            }
            catch (Exception ex)
            {
                this.m_CurrentMessage = "ERROR " + ex.Message;
                Console.WriteLine("Error Editing Source :" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Reads data as per the user query
        /// </summary>
        /// <param name="iQuery">The speicfic Query</param>
        /// <returns></returns>
        public DataTable readForSpecificQuery(string iQuery)
        {
            try
            {
                DataTable returnDataObject = new DataTable();

                OleDbCommand selectCommand = new OleDbCommand(iQuery);
                selectCommand.Connection = this.m_ConnectionToExcelBook;

                this.m_AdapterForExcelBook = new OleDbDataAdapter();

                this.m_AdapterForExcelBook.SelectCommand = selectCommand;
                this.m_AdapterForExcelBook.Fill(returnDataObject);

                this.m_CurrentMessage = "SUCCESS - " + returnDataObject.Rows.Count + " Records Loaded ";

                return returnDataObject;
            }
            catch (Exception ex)
            {
                this.m_CurrentMessage = "ERROR " + ex.Message;
                //MessageBox.Show(ex.Message, "Error Reading Source", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Reads an entire excel sheet from an opened excel workbook
        /// </summary>
        /// <param name="iSheetName"></param>
        /// <returns></returns>
        public DataTable readEntireSheet(string iSheetName)
        {
            try
            {
                DataTable returnDataObject = new DataTable();

                OleDbCommand selectCommand = new OleDbCommand("select * from [" + iSheetName + "$]");
                selectCommand.Connection = this.m_ConnectionToExcelBook;

                this.m_AdapterForExcelBook = new OleDbDataAdapter();

                this.m_AdapterForExcelBook.SelectCommand = selectCommand;
                this.m_AdapterForExcelBook.Fill(returnDataObject);

                this.m_CurrentMessage = "SUCCESS - " + returnDataObject.Rows.Count + " Records Loaded ";

                return returnDataObject;
            }
            catch (Exception ex)
            {
                this.m_CurrentMessage = "ERROR " + ex.Message;
                //MessageBox.Show(ex.Message, "Error Reading Source", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_Logs.Add(ex.ToString() + System.Environment.NewLine);
                return null;
            }
        }

        public void checkDupliationData(ExcelSheetMgr.ExcelSheet sheets)
        {
            try
            {
                string tblSqlA = String.Format("select '{0}' as sheet, {1} as idx, count(1) as cnt from [{0}$] group by {1}", sheets.SheetList[0].Name, sheets.SheetList[0].IndexKey);
                string tblSqlB = String.Format("select '{0}' as sheet, {1} as idx from [{0}$]", sheets.SheetList[0].Name, sheets.SheetList[0].IndexKey);
                for (int i = 1; i < sheets.SheetList.Count; i++)
                {
                    tblSqlA += String.Format(" union select '{0}' as sheet, {1} as idx, count(1) as cnt from [{0}$] group by {1}", sheets.SheetList[i].Name, sheets.SheetList[i].IndexKey);
                    tblSqlB += String.Format(" union select '{0}' as sheet, {1} as idx from [{0}$]", sheets.SheetList[i].Name, sheets.SheetList[i].IndexKey);
                }
                string tblA = String.Format("select idx, sum(cnt) as cnt from ({0}) group by idx having sum(cnt) > 1", tblSqlA);
                string sql = String.Format("select b.sheet, a.idx, a.cnt from ({0}) A, ({1}) B where a.idx=b.idx order by b.idx", tblA, tblSqlB);

                DataTable returnDataObject = readForSpecificQuery(sql);
                if (returnDataObject == null)
                    return;

                List<string> dupLog = new List<string>();
                for (int i = 0; i < returnDataObject.Rows.Count; i++)
                {
                    if (!isNumeric(returnDataObject.Rows[i].ItemArray[1].ToString(), System.Globalization.NumberStyles.Integer))
                        continue;

                    String dupMsg = String.Format("Sheet[{0}] : [{1}], Count[{2}]",
                        returnDataObject.Rows[i].ItemArray[0].ToString(),
                        returnDataObject.Rows[i].ItemArray[1].ToString(),
                        returnDataObject.Rows[i].ItemArray[2].ToString());
                    dupLog.Add(dupMsg);
                }
                if (dupLog.Count > 0)
                {
                    m_Logs.Add(String.Format("Excel File Name : {0}", System.IO.Path.GetFileName(this.m_SourceFileName)));
                    foreach (string item in dupLog)
                        m_Logs.Add(item);
                    m_Logs.Add(System.Environment.NewLine);
                }
                returnDataObject.Dispose();
                returnDataObject = null;
            }
            catch (Exception ex)
            {
                this.m_CurrentMessage = "ERROR " + ex.Message;
                //MessageBox.Show(ex.Message, "Error Reading Source", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_Logs.Add(ex.ToString() + System.Environment.NewLine);
            }
        }

        public void checkDupliationData(string sheetName, string indexKey)
        {
            try
            {
                indexKey = "[" + indexKey + "]";
                String sql = String.Format("select {0}, count(1) from [{1}$] where {0} <> '' group by {0} having count(1) > 1", indexKey, sheetName);
                DataTable dt = readForSpecificQuery(sql);

                if (dt == null)
                    return;

                if (dt.Rows.Count > 0)
                {
                    m_Logs.Add(String.Format("Excel[{0}], Sheet[{1}]", System.IO.Path.GetFileName(this.m_SourceFileName), sheetName));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        String dupMsg = String.Format("Duplication Key {0} : [{1}], Count[{2}]",
                            indexKey,
                            dt.Rows[i].ItemArray[0].ToString(),
                            dt.Rows[i].ItemArray[1].ToString());
                        m_Logs.Add(dupMsg);
                    }
                    m_Logs.Add(System.Environment.NewLine);
                }
                dt.Dispose();
                dt = null;
            }
            catch (Exception ex)
            {
                this.m_CurrentMessage = "ERROR " + ex.Message;
                //MessageBox.Show(ex.Message, "Error Reading Source", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_Logs.Add(ex.ToString() + System.Environment.NewLine);
            }
        }

        public bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            Double result;
            return Double.TryParse(val, NumberStyle,
                System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        /// Opens the connection to the source excel document
        /// </summary>
        /// <returns></returns>
        public bool openConnection()
        {

            try
            {
                //this.m_ConnectionToExcelBook = new OleDbConnection(sConnectionString);
                this.m_ConnectionToExcelBook = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.m_SourceFileName + ";Extended Properties=Excel 12.0;");
                //this.m_ConnectionToExcelBook = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.m_SourceFileName + ";Extended Properties=Excel 8.0;HDR=YES");

                this.m_ConnectionToExcelBook.Open();

                this.m_CurrentMessage = "SUCCESS - Connection to Source Established" + Environment.NewLine;

                DataTable dt = this.m_ConnectionToExcelBook.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["TABLE_NAME"].ToString().IndexOf("FilterDatabase") >= 0 ||
                            row["TABLE_NAME"].ToString().IndexOf("$") == -1)
                            continue;
                        m_SheetList.Add(row["TABLE_NAME"].ToString());
                        this.m_CurrentMessage += row["TABLE_NAME"].ToString();
                        this.m_CurrentMessage += Environment.NewLine;
                    }
                    dt.Dispose();
                }
            }
            catch (Exception ex)
            {
                this.m_CurrentMessage = "ERROR " + ex.Message;
                Console.WriteLine("Error Opening Source" + ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes the connection to the source excel document
        /// </summary>
        /// <returns></returns>
        public bool closeConnection()
        {
            try
            {
                this.m_ConnectionToExcelBook.Close();

                this.m_CurrentMessage = "SUCCESS - Connection to Source Closed";
            }
            catch (Exception ex)
            {
                this.m_CurrentMessage = "ERROR " + ex.Message;
                Console.WriteLine("Error Closing Source:" + ex.Message);
                return false;
            }

            return true;

        }

        public static void CheckLogMsg()
        {
            try
            {
                if (MicrosoftExcelClient.m_Logs.Count > 0)
                {
                    String logFile = String.Format("{0}TableGen_{1}{2:00}{3:00}{4:00}{5:00}{6:00}.log", System.IO.Path.GetTempPath(),
                            DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                            DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    System.IO.TextWriter tw = new System.IO.StreamWriter(logFile);
                    foreach (string msg in MicrosoftExcelClient.m_Logs)
                    {
                        tw.WriteLine(msg);
                    }
                    tw.Close();
                    System.Diagnostics.Process.Start("notepad.exe", logFile);
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

    }
}