using System;
using System.Collections.Generic;
using System.Text;
using Ini;
using System.IO;
using System.Text.RegularExpressions;
using TableGenerate;
using System.Linq;
using System.Threading.Tasks;

namespace TableGenerateCmd
{
    class TableGenerateCmd
    {
        protected string _iniFile;
        protected IniFile _historyIni;
        protected bool _isWriteLog;
        protected int _cmd;
        protected string _async;
        List<XlsFileName> _xlsList;
        List<JobItem> _JobList;
        List<string> _impList;
        public string _enumFilePath;
        protected string _outputPath, _version, _lang, _dllOutputPath;
        protected string _extType, _ignoreCase;
        protected List<string> _except = new List<string>();

        public TableGenerateCmd(int cmd, string iniFile, string srcDir, string lang, string version, string async)
        {
            this._cmd = cmd;
            this._iniFile = iniFile;
            this._version = version;
            //this._srcDir = srcDir;
            this._lang = lang;
            this._async = async;
            _historyIni = null;
            _isWriteLog = false;
            _impList = new List<string>();

            _xlsList = new List<XlsFileName>();
            _JobList = new List<JobItem>();

            _JobList.Add(new JobImportTable(0, ProgramCmd.TABLE_DIR, "TableGenerate", "TableInput", srcDir));        // JobList의 Index 0은 항상 Import Table 정보를 저장함.
            if ((cmd & ProgramCmd.EXPORT_CS) > 0) _JobList.Add(new JobExportData(new ExportToCS(), ProgramCmd.EXPORT_CS, ProgramCmd.ICECS_DIR, "Directory", "CS", "C#_FILES"));
            if ((cmd & ProgramCmd.EXPORT_CSMGR) > 0) _JobList.Add(new JobExportData(new ExportToCSMgr(), ProgramCmd.EXPORT_CSMGR, ProgramCmd.ICECS_DIR, "Directory", "CSMGR", "C#_FILES"));
            if ((cmd & ProgramCmd.EXPORT_CPP) > 0) _JobList.Add(new JobExportData(new ExportToCPP(), ProgramCmd.EXPORT_CPP, ProgramCmd.ICECPP_DIR, "Directory", "CPP", "CPP_FILES"));
            if ((cmd & ProgramCmd.EXPORT_CPPHEADER) > 0) _JobList.Add(new JobExportData(new ExportToCPPHeader(), ProgramCmd.EXPORT_CPPHEADER, ProgramCmd.ICECPP_DIR, "Directory", "HPP", "CPP_FILES"));
            if ((cmd & ProgramCmd.EXPORT_HPPMGR) > 0) _JobList.Add(new JobExportData(new ExportToCppMgrHeader(), ProgramCmd.EXPORT_HPPMGR, ProgramCmd.ICECPP_DIR, "Directory", "HPPMGR", "CPP_FILES"));
            if ((cmd & ProgramCmd.EXPORT_CPPMGR) > 0) _JobList.Add(new JobExportData(new ExportToCppMgrImplements(), ProgramCmd.EXPORT_CPPMGR, ProgramCmd.ICECPP_DIR, "Directory", "CPPMGR", "CPP_FILES"));
            if ((cmd & ProgramCmd.EXPORT_TABLE) > 0) _JobList.Add(new JobExportData(new ExportToTF(), ProgramCmd.EXPORT_TABLE, ProgramCmd.TF_DIR, "Directory", "TableFile", "TF_FILES"));
            if ((cmd & ProgramCmd.EXPORT_SQLITE) > 0) _JobList.Add(new JobExportData(new ExportToSQLLite(), ProgramCmd.EXPORT_SQLITE, ProgramCmd.DB_DIR, "Directory", "SQLITE", "SQLITE_FILES"));
            if ((cmd & ProgramCmd.EXPORT_MSSQL) > 0) _JobList.Add(new JobExportData(new ExportToMSSQL(), ProgramCmd.EXPORT_MSSQL, ProgramCmd.DB_DIR, "Directory", "MSSQL", "MSSQL_FILES"));
            if ((cmd & ProgramCmd.EXPORT_MYSQL) > 0) _JobList.Add(new JobExportData(new ExportToMySQL(), ProgramCmd.EXPORT_MYSQL, ProgramCmd.DB_DIR, "Directory", "MYSQL", "MYSQL_FILES"));
        }

        public bool InitJob()
        {
            Ini.IniFile ini = new Ini.IniFile(_iniFile);
            //_inputPath = ini.IniReadValue("TableGenerate", "InputPath");
            _dllOutputPath = ini.IniReadValue("TableGenerate", "DllOutputPath").Replace("//", "/"); 
            _outputPath = ini.IniReadValue("TableGenerate", "OutputPath").Replace("//", "/");
            _extType = ini.IniReadValue("TableGenerate", "ExtType");
            _ignoreCase = ini.IniReadValue("TableGenerate", "IgnoreCase");
            _enumFilePath = ini.IniReadValue("Directory", "ENUMTYPES");
            System.Reflection.Assembly _Assembly = null;
            if (!string.IsNullOrEmpty(_enumFilePath))
            {
                StringBuilder sb = new StringBuilder();
                foreach(var text in System.IO.Directory.GetFiles(_enumFilePath, "*.cs"))
                {
                    sb.Append(System.IO.File.ReadAllText(text));
                }
                _Assembly = CSScriptLibrary.CSScript.LoadCode(sb.ToString());
            }

            _except = ini.IniReadValue("TableGenerate", "Except").Split(',').Select( item => item.Trim().ToLower()).ToList();
            if( _except.Count() > 0 )
                _except.RemoveAll(compare => compare == string.Empty);

            if (string.IsNullOrEmpty(_extType) == true)
                _extType = ProgramCmd.EXT_TYPE;
            if ( string.IsNullOrEmpty(_lang) == true)
                _lang = ini.IniReadValue("Default", "LANG");
            if (string.IsNullOrEmpty(_version) == true)
                _version = ini.IniReadValue("Default", "VERSION");

            string modified = ini.IniReadValue("TableGenerate", "OnlyModifiedFile");

            if (modified.Length > 0 && modified.ToLower().Substring(0, 1) == "y")
                _isWriteLog = true;

            string impFiles = ini.IniReadValue("TableGenerate", "FILES");
            if (impFiles.Length > 0)
            {
                string[] impFileList = impFiles.Split(',');
                foreach (string file in impFileList)
                    _impList.Add(file);
            }


            for (int i = 0; i < _JobList.Count; i++)
            {
                var job = _JobList[i];
                job.SetEtc(_version, _lang, ini.IniReadValue("LANG", _lang+"_TBL"), ini.IniReadValue("LANG", _lang+"_SRC"), ini.IniReadValue("LANG", _lang+"_EXT"));
                job.SetDefaultDir(ini.IniReadValue("Default", job.GetItem()));
                bool sub_folder = ProgramCmd.single_file.Contains(System.IO.Path.DirectorySeparatorChar);
                if(_Assembly != null)
                    job.SetEnumTypesAssembly(_Assembly);
                if (sub_folder && (job.GetExportBase() is ExportToCS || job.GetExportBase() is ExportToCSMgr ))
                {
                    job.SetDest(_outputPath, System.IO.Path.GetTempPath());
                }
                else if(sub_folder && job.GetExportBase() is ExportToTF)
                {
                    job.SetDest(_outputPath, ini.IniReadValue(job.GetSection(), job.GetItem()) + System.IO.Path.DirectorySeparatorChar + ProgramCmd.single_file.Substring(0,ProgramCmd.single_file.IndexOf(System.IO.Path.DirectorySeparatorChar)) );
                }
                else 
                {
                    job.SetDest(_outputPath, ini.IniReadValue(job.GetSection(), job.GetItem()));
                }
                if (job.GetFileItem() != null)
                    job.SetExportFileList(ini.IniReadValue(job.GetSection(), job.GetFileItem()));
                if ( !job.IsExistedDir()) 
                {
                    System.Console.WriteLine("[{0}] Directory is not existed.", job.GetDestDir());
                    return false;
                }
                
            }

            if (LoadXlsFileList() == 0)
            {
                System.Console.WriteLine("XLS File not found.");
                return false;
            }

            {
                JobExportData eCS = (JobExportData)GetJobItem(ProgramCmd.EXPORT_CS);
                JobExportData eCSMgr = (JobExportData)GetJobItem(ProgramCmd.EXPORT_CSMGR);
                JobExportData eTF = (JobExportData)GetJobItem(ProgramCmd.EXPORT_TABLE);

                if(eCS != null)
                {
                    ((ExportToCS)eCS.GetExportBase()).SetAsync = this._async;
                }
                if (eTF != null && eCS != null)
                {
                    ((ExportToTF)eTF.GetExportBase()).CSFileDir = eCS.GetDestDir();
                    ((ExportToTF)eTF.GetExportBase()).DllOutputDir = _dllOutputPath;
                    ((ExportToTF)eTF.GetExportBase()).EnumTypeDir = _enumFilePath;
                }
                if(eCSMgr != null)
                {
                    var eCSMgrBase = eCSMgr.GetExportBase() as ExportToCSMgr;
                    if(eCSMgrBase!=null)eCSMgrBase.SetAsync = this._async;
                }
            }
            return true;
        }

        protected JobItem GetJobItem(int JobType)
        {
            for (int i = 0; i < _JobList.Count; i++)
                if (_JobList[i].GetJobType() == JobType)
                    return (JobItem)_JobList[i];
            return null;
        }

        public void DoWork()
        {
            for(int x=0;x< _xlsList.Count;x++)
            {
                var xls = _xlsList[x];
                ClassUtil.ExcelImporter imp = null;
                if (imp == null)
                {
                    imp = new ClassUtil.ExcelImporter();
                    if (imp.Open(xls.FullPath) == false)
                    {
                        System.Console.WriteLine("[XLS[{0}] open failure...", xls.FullPath);
                        imp.Dispose();
                        imp = null;
                        return;
                    }
                }
                for (int i = 1; i < _JobList.Count; i++)     // index 0은 Import Table 디렉토리 정보이므로...
                {
                    var job = _JobList[i];
                    if ((_cmd & job.GetJobType()) == 0)
                        continue;
                    if (job.GetFileList().Count == 0 || (job.GetFileList().IndexOf(xls.FileName) >= 0))
                    {
                        System.Console.WriteLine("Generate {0} for {1} in [{2}]", xls.FileName, job.GetItem(), job.GetDestDir());
                        if (job.GetJobType() == ProgramCmd.EXPORT_TABLE)
                        {
                            job.DoExport(imp, xls.RealPath, _lang, _except);
                        }
                        else
                            job.DoExport(imp, xls.FileName, _lang, _except);
                    }
                }
                if (imp != null)
                {
                    imp.Dispose();
                    imp = null;
                }

                WriteLog(xls.FullPath, xls.LastWriteTime);
            }
        }

        //public void DoWork()
        //{
        //    for (int k = 0; k < _xlsList.Count; k++)
        //    {
        //        var xls = _xlsList[k];
        //        ClassUtil.ExcelImporter imp = null;
        //        if (imp == null)
        //        {
        //            imp = new ClassUtil.ExcelImporter();
        //            if (imp.Open(xls.RealPath) == false)
        //            {
        //                System.Console.WriteLine("[XLS[{0}] open failure...", xls.FullPath);
        //                imp.Dispose();
        //                imp = null;
        //                return;
        //            }
        //            imp.SetSheetList(ClassUtil.ExcelSheetMgr.GetExcelSheet(xls.RealPath), _checkDupList);
        //        }
        //        for (int i = 1; i < _JobList.Count; i++)     // index 0은 Import Table 디렉토리 정보이므로...
        //        {
        //            var job = _JobList[i];
        //            if ((_cmd & job.GetJobType()) == 0)
        //                continue;
        //            if (job.GetFileList().Count == 0 || (job.GetFileList().IndexOf(xls.FileName) >= 0))
        //            {
        //                System.Console.WriteLine("Generate {0} for {1} in [{2}]", xls.FileName, job.GetItem(), job.GetDestDir());
        //                if (job.GetJobType() == ProgramCmd.EXPORT_TABLE)
        //                {
        //                    job.DoExport(imp, xls.RealPath, _lang, _except);
        //                }
        //                else
        //                    job.DoExport(imp, xls.FileName, _lang, _except);
        //            }
        //        }
        //        if (imp != null)
        //        {
        //            imp.Dispose();
        //            imp = null;
        //        }
        //
        //        WriteLog(xls.FullPath, xls.LastWriteTime);
        //    };
        //}

        protected int LoadXlsFileList()
        {
            bool isExisted;
            _xlsList.Clear();
            //ClassUtil.ExcelOLEClient client = new ClassUtil.ExcelOLEClient();
            DirectoryInfo[] di = new DirectoryInfo[2];
            JobImportTable jobItem = ((JobImportTable)_JobList[0]);
            if (!jobItem.IsExistedDir())
            {
                System.Console.WriteLine("XLS Table Path is not existed.");
                return 0;
            }
            di[0] = new DirectoryInfo(jobItem.GetDestDir());
            if (string.IsNullOrEmpty(jobItem.GetLangDir()) == false)
                di[1] = new DirectoryInfo(jobItem.GetLangDir());

            Regex re = new Regex(_ignoreCase);
            for (int i = 0; i < 2; i++)
            {
                if (i == 1 && string.IsNullOrEmpty(jobItem.GetLangDir()) == true)
                    break;

                System.Console.WriteLine("Start to get XLS File list in[{0}].", i == 0 ? _JobList[0].GetDestDir() : jobItem.GetLangDir());

                //string []ptn = extType
                FileInfo[] rgFiles = null;
                if (ProgramCmd.single_file.Length > 0)
                    rgFiles = new FileInfo[] { new FileInfo(di[i].FullName + ProgramCmd.single_file) };
                else
                { // only single file
                    return 0;
                }

                foreach (FileInfo fi in rgFiles) // rgFiles has only one element
                {// to process
                    if (re.IsMatch(fi.Name))
                        continue;

                    if (_impList.Count > 0 && _impList.IndexOf(fi.Name) == -1)
                        continue;

                    isExisted = false;
                    for (int j = 0; j < _xlsList.Count; j++)
                    {
                        if (_xlsList[j].FileName == fi.Name)
                        {
                            _xlsList[j].FullPath = fi.FullName;
                            isExisted = true;
                            System.Console.WriteLine("Modified XLS File. [{0}].", fi.Name);
                            break;
                        }
                    }
                    if (isExisted)
                        continue;

                    if (IsModifiedFile(fi.FullName, fi.LastWriteTime) == false)     //INI 설정이 No이거나 실제로 변경되지 않았으면...
                        continue;

                    string realFile = fi.Name;
                    _xlsList.Add(new XlsFileName(realFile, fi.FullName, fi.Name, fi.LastWriteTime));
                    System.Console.WriteLine("Added XLS File. [{0}].", fi.Name);
                }
            }
            //client.Dispose();
            //client = null;

            return _xlsList.Count;
        }

        protected void WriteLog(string fileName, DateTime lastWrite)
        {
            if (!_isWriteLog)
                return;

            try
            {
            }
            catch (Exception)
            {
                
            }
        }

        protected bool IsModifiedFile(string fileName, DateTime lastWrite)
        {
            if (!_isWriteLog)
                return true;

            try 
            {
                if (_historyIni == null)
                {
                    string hisFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName) + System.IO.Path.DirectorySeparatorChar + "TableGenHistory.ini";
                    if (File.Exists(hisFile) == false)
                    {
                        FileStream fs = File.Open(hisFile, FileMode.Create);
                        fs.Close();
                    }

                    _historyIni = new IniFile(hisFile);
                }

                string dirName = Path.GetDirectoryName(fileName);
                string name = Path.GetFileName(fileName);
                string writeTime = _historyIni.IniReadValue(dirName, name);
                if (writeTime.Length == 0)
                    return true;
                string lastWriteStr = String.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                    lastWrite.Date.Year, lastWrite.Date.Month, lastWrite.Date.Day,
                    lastWrite.Hour, lastWrite.Minute, lastWrite.Second);
                if (lastWriteStr == writeTime)
                    return false;

                return true;
            }
            catch(Exception)
            {
                return true;
            }
        }

        public void Finialize()
        {
            try {
                ClassUtil.MicrosoftExcelClient.CheckLogMsg();
            }
            catch(Exception)
            {

            }
        }
    }

    public class XlsFileName
    {
        public string RealPath;
        public string FullPath;
        public string FileName;
        public DateTime LastWriteTime;

        public XlsFileName(string realpath, string full, string name, DateTime lastWriteTime)
        {
            RealPath = realpath;
            FullPath = full;
            FileName = name;
            LastWriteTime = lastWriteTime;
        }
    }

    public abstract class JobItem
    {
        protected int JobType;
        protected string DefaultDir, DestDir, version, lang, langTbl, langSrc, extName;
        protected string INISection, INIItem, FileItem;
        protected int current, max;
        protected ExportBase expBase;
        protected List<string> files;
        protected System.Reflection.Assembly _Assembly = null;
        protected System.Reflection.Assembly mscorlibAssembly = null;
        public JobItem(int JobType, string Default, string INISection, string INIItem)
        {
            this.JobType = JobType;
            this.DefaultDir = Default;
            this.INISection = INISection;
            this.INIItem = INIItem;
            files = new List<string>();
        }

        public JobItem(ExportBase expBase, int JobType, string Default, string INISection, string INIItem, string FileItem)
        {
            this.expBase = expBase;
            this.JobType = JobType;
            this.DefaultDir = Default.Replace("//","/");
            this.INISection = INISection;
            this.INIItem = INIItem;
            this.FileItem = FileItem.Replace("//", "/");
            files = new List<string>();
        }

        public void SetEtc(string version, string lang, string langTbl, string langSrc, string extName)
        {
            this.version = version;
            this.lang = lang;
            this.langTbl = langTbl;
            this.langSrc = langSrc;
            this.extName = extName;
        }

        public void SetDefaultDir(string dir)
        {
            if ( string.IsNullOrEmpty(dir) == false)
                this.DefaultDir = dir;
        }

        public void SetExportFileList(string files)
        {
            if (files.Length == 0)
                return;
            string[] fileList = files.Split(',');

            foreach (string file in fileList)
                this.files.Add(file);
        }

        virtual public void SetDest(string output, string dir)
        {
            string newName = string.Empty;
            if (string.IsNullOrEmpty(dir) == true)
                dir = DefaultDir;
            newName = version;
            if (version != "Develop" && string.IsNullOrEmpty(langSrc) == false)
                newName += langSrc;
            dir = dir.Replace("$Version", newName);

            if (string.IsNullOrEmpty(output) == true)
                DestDir = Path.GetDirectoryName(dir + System.IO.Path.DirectorySeparatorChar).Replace("//","/");
            else
                DestDir = Path.GetDirectoryName( output + System.IO.Path.DirectorySeparatorChar + dir + System.IO.Path.DirectorySeparatorChar ).Replace("//", "/");
        }

        virtual public void SetEnumTypesAssembly(System.Reflection.Assembly Assembly)
        {
            _Assembly = Assembly;
            string shared_directory = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location);
            mscorlibAssembly = System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(shared_directory, "mscorlib.dll"));
        }

        virtual public bool IsExistedDir()
        {
            if (!System.IO.Directory.Exists(DestDir))
                System.IO.Directory.CreateDirectory(DestDir);
            return System.IO.Directory.Exists(DestDir);
        }
        //public abstract void SetDest(string output, string dir);
        //public abstract bool IsExistedDir();

        public ExportBase GetExportBase() => expBase;
        public List<string> GetFileList() => files;
        public int GetJobType() => JobType;
        public string GetSection() => INISection;
        public string GetItem() => INIItem;
        public string GetFileItem() => FileItem;
        public string GetDestDir() => DestDir;
        public string GerVerion() => version;
        public abstract void DoExport(ClassUtil.ExcelImporter imp, string xlsFileName, string language, List<string> except);
    }

    public class JobImportTable : JobItem
    {
        protected string LangDir, source;

        public JobImportTable(int JobType, string Default, string INISection, string INIItem, string source)
            : base(JobType, Default, INISection, INIItem)
        {
            this.source = source;
            LangDir = string.Empty;
        }

        public string GetLangDir() => LangDir;

        public override void SetDest(string output, string dir)
        {
            string newName = string.Empty;
            if (string.IsNullOrEmpty(dir) == true)
                dir = DefaultDir;
            if (string.IsNullOrEmpty(source) == false)
                dir = source;

            if (version != "Develop")
                newName = "_" + version;

            DestDir = dir.Replace("$Version", newName) + ProgramCmd.ext_path + Path.DirectorySeparatorChar;

            if (string.IsNullOrEmpty(langTbl) == false)
                LangDir = DestDir + System.IO.Path.DirectorySeparatorChar + langTbl;
        }

        public override bool IsExistedDir()
        {
            if (!System.IO.Directory.Exists(DestDir))
                return false;
            if (string.IsNullOrEmpty(LangDir) == false && !System.IO.Directory.Exists(LangDir))
                return false;
            return true;
        }

        public override void DoExport(ClassUtil.ExcelImporter imp, string xlsFileName, string language, List<string> except)
        {
        }
    }

    public class JobExportData : JobItem
    {
        public JobExportData(ExportBase expBase, int JobType, string Default, string INISection, string INIItem, string FileItem)
            : base(expBase, JobType, Default, INISection, INIItem, FileItem)
        {
        }

        public override void DoExport(ClassUtil.ExcelImporter imp, string xlsFileName, string language, List<string> except)
        {
            try
            {
               if( expBase.Generate(base._Assembly, base.mscorlibAssembly, imp, DestDir, xlsFileName, ref current, ref max, language, except) == false)
               {
                   ProgramCmd.exit_code = 1;
               }
            }
            catch (Exception e)
            {
                ProgramCmd.exit_code = 1;
                System.Console.Write("Export Error: {0}", e.Message);
            }
        }
    }

}
