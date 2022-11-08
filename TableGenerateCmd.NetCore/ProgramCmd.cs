using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace TableGenerateCmd
{
    public static class ProgramCmd
    {
        public static long EXPORT_CS         = 1 << 1;     // 0000000001
        public static long EXPORT_CPPHEADER  = 1 << 2;     // 0000000010
        public static long EXPORT_CPP        = 1 << 3;     // 0000000010
        public static long EXPORT_CPPMGR     = 1 << 4;     // 0000000100
        public static long EXPORT_HPPMGR     = 1 << 5;    // 0000001000
        public static long EXPORT_CSMGR      = 1 << 6;    // 0000010000
        public static long EXPORT_TABLE      = 1 << 7;    // 0000100000
        public static long EXPORT_PROTO      = 1 << 8;   // 0001000000
        public static long EXPORT_SQLITE     = 1 << 9;   // 0010000000
        public static long EXPORT_MSSQL      = 1 << 10;   // 0100000000
        public static long EXPORT_MYSQL      = 1 << 11;  // 0111111111
        public static long EXPORT_RUST       = 1 << 12;  // 1111111111
        public static long EXPORT_ALL        = EXPORT_CS | EXPORT_CPPHEADER | EXPORT_CPP | EXPORT_CPPMGR | EXPORT_HPPMGR | EXPORT_CSMGR | EXPORT_TABLE | EXPORT_PROTO | EXPORT_RUST;  

        public static string EXT_TYPE = ".xls";
        public static string TABLE_DIR = $"Data{System.IO.Path.DirectorySeparatorChar}Table";
        public static string ICECPP_DIR = $"Source{System.IO.Path.DirectorySeparatorChar}cpp/generated{System.IO.Path.DirectorySeparatorChar}table";
        public static string CS_DIR = $"Server{System.IO.Path.DirectorySeparatorChar}WebTools{System.IO.Path.DirectorySeparatorChar}App_Code{System.IO.Path.DirectorySeparatorChar}table";
        public static string TF_DIR = $"engine{System.IO.Path.DirectorySeparatorChar}table";
        public static string DLL_DIR = $"engine{System.IO.Path.DirectorySeparatorChar}table";
        public static string DB_DIR = $"Server{System.IO.Path.DirectorySeparatorChar}engine{System.IO.Path.DirectorySeparatorChar}table{System.IO.Path.DirectorySeparatorChar}DB";

        public static long cmd = 0;
        public static int exit_code = 0;
        public static string inifilename = string.Empty, source = string.Empty, version = string.Empty, lang = string.Empty, single_file = string.Empty, ext_path = string.Empty, async = string.Empty;
        public static int parallel = 1;
        public static string argstr = string.Empty;
        public static bool not_array_length_full = false;
        public static bool using_perforce = false;
        public static string dataStage = String.Empty;

        static DateTime _begin_time;

        static Dictionary<string, ManualResetEvent> _mre_dic = new Dictionary<string,ManualResetEvent>();

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            _begin_time = DateTime.Now;
            if (args.Length < 2 || ParseArgs(args) == false)
            {
                System.Console.WriteLine("Usage: TableGenerateCmd -i 환경설정파일 -c 추출명령 -s 소스폴더 -l 언어[kor|chi] -v 마일스톤버젼 -a [unity3d|ctp] -p 병렬처리수 -f 파일네임 -k true array length not full");
                return 1;
            }

            TableGenerate.ExportToCSMgr.NameSpace = "TBL";

            //System.Console.WriteLine("Start Time : {0}", DateTime.Now.ToString());
            if (cmd == 0) cmd = EXPORT_ALL;
            if (string.IsNullOrEmpty(inifilename) == true) inifilename = $"{System.IO.Directory.GetCurrentDirectory()}/{ExtractAppName()}.ini";

            //if (string.IsNullOrEmpty(single_file) == true) // start multi-process
            //{
            //    {
            //        var process_array = Process.GetProcessesByName("TableGenerateCmd");
            //        if (process_array.Length > 1)
            //        {
            //            Console.ForegroundColor = ConsoleColor.Red;
            //            Console.WriteLine("Fail: TableGenerateCmd is already running.");
            //            Console.ResetColor();
            //            return 1;
            //        }
            //    }
            //
            //    List<string> filelist = new List<string>();
            //
            //    string ext_temp = "s" + new Random().Next();
            //
            //    //int path_length = 0;
            //    List<string> pathfilelist = new List<string>();
            //    {
            //        Ini.IniFile ini = new Ini.IniFile(inifilename);
            //        var path = ini.IniReadValue("TableGenerate", "TableInput");
            //        string extType = ini.IniReadValue("TableGenerate", "ExtType");
            //        if (string.IsNullOrEmpty(extType) == false)
            //        {
            //            EXT_TYPE = extType;
            //        }
            //        var di = new DirectoryInfo(path);
            //        var rgFiles = di.GetFiles(EXT_TYPE, SearchOption.TopDirectoryOnly).ToList();
            //        rgFiles.RemoveAll(t => t.Name.Contains("~$"));
            //        filelist = rgFiles.Select(t => t.Name).ToList();
            //    }
            //
            //    // generate after making value only file
            //    string this_program = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //
            //    ParallelProcess(filelist, this_program);
            //
            //    System.Console.WriteLine("Table Export Time(sec) : " + (DateTime.Now - _begin_time).TotalSeconds);
            //
            //    return exit_code;
            //}

            TableGenerateCmd job = null;
            try
            {
                job = new TableGenerateCmd(cmd, inifilename, source, lang, version, async);
                if (!job.InitJob())
                    return 1;

                job.DoWork();
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"InitJob " +e.ToString());
                exit_code = 1;
            }
            finally
            {
                if (job != null)
                    job.Finialize();
            }

            return exit_code;
        }

        private static void ParallelProcess(List<string> top_filelist, string this_program)
        {
            System.Threading.Tasks.Parallel.ForEach(top_filelist, new ParallelOptions { MaxDegreeOfParallelism = parallel },
            f =>
            {
                //_mre_dic[f].WaitOne();

                using (var process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    if (System.IO.Path.DirectorySeparatorChar == '\\')
                    {
                        process.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
                        process.StartInfo.FileName = this_program;
                        process.StartInfo.Arguments = $"{ProgramCmd.argstr}-f {f}";// +" -ep " + ext_temp;
                    }
                    else
                    {
                        process.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
                        process.StartInfo.FileName = "mono";
                        process.StartInfo.Arguments = $"{this_program} {ProgramCmd.argstr}-f {f}";// +" -ep " + ext_temp;
                    }
                    process.Start();

                    string standardOutput = string.Empty;
                    string standardError = string.Empty;

                    using (Task processWaiter = Task.Factory.StartNew(() => process.WaitForExit()))
                    using (Task<string> outputReader = Task.Factory.StartNew(() => process.StandardOutput.ReadToEnd()))
                    using (Task<string> errorReader = Task.Factory.StartNew(() => process.StandardError.ReadToEnd()))
                    {
                        Task.WaitAll(processWaiter, outputReader, errorReader);

                        standardOutput = outputReader.Result;
                        standardError = errorReader.Result;
                    }

                    if (process.ExitCode == 0)
                        Console.WriteLine("Success " + f, process.ExitCode);
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Fail, exit code {0} - " + f, process.ExitCode);
                        Console.WriteLine(standardOutput);
                        Console.WriteLine(standardError);
                        Console.ResetColor();
                        exit_code = process.ExitCode;
                    }
                }
            });
        }

        public static bool ParseArgs(string[] args)
        {
            if (args.Length < 2)
                return false;

            foreach (var a in args)
            {
                argstr += a + " ";
            }

            long tmp, i=0;

            try {               // out of index Exception을 잡기 위해서...
                while (i < args.Length)
                {
                    if (args[i] == "-i")
                    {
                        string val = args[++i];
                        inifilename = val;
                    }
                    else if( args[i] == "-k")
                    {
                        not_array_length_full = bool.Parse(args[++i]);
                    }
                    else if (args[i] == "-c")
                    {
                        while (i < args.Length - 1)
                        {
                            tmp = CmdToMask(args[++i]);
                            if (tmp == -1)
                            {
                                --i;    // args[i] 값이 NULL이거나 out of index 일 경우..
                                break;
                            }
                            cmd = cmd | tmp;
                        }
                    }
                    else if (args[i] == "-s")
                        source = args[++i];
                    else if (args[i] == "-l")
                        lang = args[++i];
                    else if (args[i] == "-v")
                        version = args[++i];
                    else if (args[i] == "-f")
                        single_file = args[++i];
                    else if (args[i] == "-ep")
                        ext_path = args[++i];
                    else if (args[i] == "-a")
                        async = args[++i];
                    else if (args[i] == "-p")
                        parallel = Int32.Parse(args[++i]);
                    else if (args[i] == "-p4")
                        using_perforce = true;
                    else
                    {
                        System.Console.WriteLine("unknown argument. [{0}]", args[i]);
                        return false;
                    }
                    i++;
                }
            } catch(Exception e)
            {
                System.Console.WriteLine("argument parsing error. [{0}]", e.ToString());
                return false;
            }

            return true;
        }

        public static String ExtractAppName() => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        public static long CmdToMask(string val)
            => val.ToLower() switch
            {
                "c++" or "cpp" => EXPORT_CPP | EXPORT_CPPHEADER | EXPORT_HPPMGR | EXPORT_CPPMGR,
                "c#" or "cs" => EXPORT_CS | EXPORT_CSMGR,
                "table" or "tbl" or "bytes" => EXPORT_TABLE,
                "proto" or "bytes" => EXPORT_PROTO,
                "rust" or "rs" => EXPORT_RUST,
                "sqlite" or "sqllite" => EXPORT_SQLITE,
                "mssql" => EXPORT_MSSQL,
                "mysql" => EXPORT_MYSQL,
                "all" => EXPORT_ALL,
                _ => -1
            };


        public static string save_as_value_powershell =
@"
$value_only_macro = '
Public Sub SaveAsValuesOnly()
    Application.DisplayAlerts = False

    Dim Wb As Workbook
    
    For Each Wb In Workbooks
        Wb.Activate
        
        Dim wSheet As Worksheet
        
        On Error Resume Next
        For Each wSheet In Worksheets
            wSheet.Activate
            wSheet.UsedRange.NumberFormat = ""@""
            wSheet.UsedRange.Value = wSheet.UsedRange.Value
            ''With wSheet.UsedRange
            ''.Copy
            ''.PasteSpecial xlPasteValues
            ''End With
        Next wSheet
        
        ''Application.CutCopyMode = False
        On Error GoTo 0
        
        Wb.Save
        
    Next Wb
    
    Application.Quit
End Sub
'
$ExcelPath = split-path -Path $MyInvocation.MyCommand.Definition
[System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Office.Interop.Excel') | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Vbe.Interop') | Out-Null
$excelApp = New-Object -ComObject Excel.Application
#New-ItemProperty -Path ""HKCU:\Software\Microsoft\Office\${excelApp.Version}\excel\Security"" -Name AccessVBOM -Value 1 -Force | Out-Null
#New-ItemProperty -Path ""HKCU:\Software\Microsoft\Office\${excelApp.Version}\excel\Security"" -Name VBAWarnings -Value 1 -Force | Out-Null
$excelApp.Visible = $False
$excelApp.DisplayAlerts = $False
$excelApp.MultiThreadedCalculation.Enabled = $True
$filelist = gci ""${ExcelPath}/*.xlsm""
$wb_macro = $Null
foreach( $file in $filelist)
{
    $FileName = $file.Name
    write-host ""Loading $FileName""
    if( $wb_macro -eq $Null)
    {
        $wb_macro = $excelApp.Workbooks.Open($file.FullName) 
    }
    else
    {
        $excelApp.Workbooks.Open($file.FullName) | Out-Null
    }

}
write-host ""CalculateFullRebuild...""
$excelApp.CalculateFullRebuild();
$oModule = $wb_macro.VBProject.VBComponents.Add(1)
$oModule.CodeModule.AddFromString($value_only_macro);
write-host ""Save as Values...""
$excelApp.Run($wb_macro.Name + '!' + $oModule.CodeModule.Name + '.SaveAsValuesOnly' );
$excelApp.Quit()
";

        public static void MakeValueOnly(List<string> filelist, int path_length)
        {
            try
            {
                string save_as_value_file_name = System.IO.Path.GetDirectoryName(filelist.FirstOrDefault()) + "/save_as_value.ps1";
                System.IO.File.WriteAllText(save_as_value_file_name, save_as_value_powershell);
                //Windows
                if( System.IO.Path.DirectorySeparatorChar == '\\' )
                {
                    using (System.Diagnostics.Process process = new Process())
                    {
                        string powershellPath = $"{System.Environment.GetEnvironmentVariable("SystemRoot")}{@"\system32\WindowsPowerShell\v1.0\powershell.exe"}";
                        process.StartInfo.FileName = powershellPath;
                        process.StartInfo.Arguments = $"-ExecutionPolicy RemoteSigned -File {save_as_value_file_name}";
                        process.StartInfo.CreateNoWindow = false;
                        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                        process.WaitForExit();
                        if (process.ExitCode != 0)
                        {
                            Console.WriteLine("MakeValueOnly Failure Exit Code" + process.ExitCode);
                            return;
                        }
                    }
                }
                else
                {
                    Console.Write("for iOS");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                
            }
            finally
            {
                
            }
            foreach( var a in _mre_dic )
            {
                a.Value.Set();
            }
        }
    }
}
