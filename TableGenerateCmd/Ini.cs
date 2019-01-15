using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Ini
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class IniFile
    {
        public string path;

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        public IniFile()
        {
            string exedir = System.IO.Path.GetDirectoryName(System.Environment.GetCommandLineArgs()[0]);
            string inifilename = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".ini";
            path = exedir + "/" + inifilename;
        }
        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            String[] lines = System.IO.File.ReadAllLines(path,System.Text.Encoding.ASCII);
            Section = Section.ToLower();
            Key = Key.ToLower();

            var it = lines.GetEnumerator();
            bool bSection = false;
            while(it.MoveNext() ==true )
            {
                string line = it.Current as string;
                if (line != null && line.Contains("[") == true && line.Contains("]") == true && line.Contains("=") == false)
                {
                    if (line.ToLower().Contains("[" + Section + "]") == true)
                        bSection = true;
                    else
                        bSection = false;
                    continue;
                }

                if (bSection == true && line.Contains("=") == true )
                {
                    int split_index = line.IndexOf('=');
                    string key = line.Substring(0,split_index);
                    string value = line.Substring(split_index+1, line.Length - (split_index+1) );
                    if ( key.ToLower() == Key)
                    {
                        return value;
                    }
                }
            }
            return string.Empty;
        }
    }
}

