using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;

namespace TableGenerate
{
    public class ExportToCS : ExportBase
    {
        private readonly string _unityDefine = "UNITY_2018_2_OR_NEWER";

        private string _async = string.Empty;
        public string SetAsync
        {
            //set { _async = "unity3d"; }
            set { _async = value; }
        }

        public static string NameSpace = string.Empty;
        public int m_current = 0;
        public eGenType _gen_type = eGenType.cs;
        private bool _useInterface;
        public ExportToCS(string unityDefine, bool useInterface)
        {
            _unityDefine = unityDefine;
            _useInterface = useInterface;
        }

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            try
            {
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".cs");

                using MemoryStream stream = new MemoryStream();
                {
                    using var writer = new IndentedTextWriter(new StreamWriter(stream,  System.Text.Encoding.UTF8), " ");
                    {
                        string filename = System.IO.Path.GetFileName(createFileName);

                        writer.WriteLineEx("#pragma warning disable IDE0007, IDE0011, IDE0025, IDE1006, IDE0018");

                        string[] sheets = imp.GetSheetList();

                        filename = filename.Replace(".cs", string.Empty);

                        max = sheets.GetLength(0);
                        current = 0;

                        writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}.{filename}");
                        writer.WriteLineEx("{");
                        foreach (string sheetName in sheets)
                        {
                            current++;
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            SheetProcess(writer, filename, trimSheetName, columns);
                        }
                        writer.WriteLineEx("}");
                        writer.Flush();
                    }
                    ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}", TableGenerateCmd.ProgramCmd.using_perforce);
                }
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
            return true;
        }
        private void SheetProcess(IndentedTextWriter writer, string filename, string sheetName, List<Column> columns)
        {
            if(_useInterface)
            {
                InterfacePropertySheetProcess(sheetName, writer, columns);
            }
            
            writer.WriteLineEx($"/// <summary>");
            writer.WriteLineEx($"/// {sheetName}"); 
            writer.WriteLineEx($"/// </summary>");
            InnerSheetDescProcess(writer,columns);
            writer.WriteLineEx($"[System.CodeDom.Compiler.GeneratedCode(\"TableGenerateCmd\",\"1.0.0\")]");
            writer.WriteLineEx($"public partial class {sheetName}");
            writer.WriteLineEx("{");
            InnerSheetProcess(writer, columns);
            //SheetConstructorProcess(writer, sheetName, columns);
            writer.WriteLineEx("}");
        }
        private void InterfacePropertySheetProcess(string sheetName, IndentedTextWriter _writer, List<Column> columns)
        {
            _writer.WriteLineEx($"#if !{_unityDefine}");
            _writer.WriteLineEx($"public interface I{sheetName}");
            _writer.WriteLineEx("{");
            foreach (var column in columns)
            {
                string name = column.var_name;
                string type = column.GenerateType(_gen_type);
                if (column.is_generated == false)
                {
                    continue;
                }
                if (column.array_index > 0)
                {
                    continue;
                }
                if (column.is_key)
                {
                    _writer.WriteLineEx($"/// <summary>");
                    _writer.WriteLineEx($"/// Key Column");
                    _writer.WriteLineEx($"/// </summary>");
                }
                //if (_async == "unity3d")
                //{
                //    _writer.WriteLineEx($"public abstract {type} {name} {{get;}}");
                //}
                //else
                {
                    _writer.WriteLineEx($"  {type} {name} {{get;}}");
                }
            }
            _writer.WriteLineEx("}");
            _writer.WriteLineEx("#endif");
        }
        private void InnerSheetDescProcess(IndentedTextWriter writer, List<Column> columns)
        {
            foreach (var column in columns)
            {
                string name = column.var_name;
                string type = column.GenerateType(_gen_type);
                if (column.is_generated == false)
                {
                    continue;
                }
                if (column.array_index > 0)
                {
                    continue;
                }
                if(column.is_key)
                {
                    // writer.WriteLineEx($"/// <summary>");
                    // writer.WriteLineEx($"/// Key Column");
                    // writer.WriteLineEx($"/// </summary>");
                }
            }
        }
        private void InnerSheetProcess(IndentedTextWriter writer, List<Column> columns)
        {
            foreach (var column in columns)
            {
                string name = column.var_name;
                string type = column.GenerateType(_gen_type);
                string defaultValue = column.GenerateDefaultValue(_gen_type);
                if (column.is_generated == false)
                {
                    continue;
                }
                if (column.array_index > 0)
                {
                    continue;
                }
                if(column.is_key)
                {
                    // writer.WriteLineEx($"/// <summary>");
                    // writer.WriteLineEx($"/// Key Column");
                    // writer.WriteLineEx($"/// </summary>");
                }
                //if (_async == "unity3d")
                //{
                //    _writer.WriteLineEx($"public abstract {type} {name} {{get;}}");
                //}
                if(column.desc.Any())
                {
                    writer.WriteLineEx($"/// <param name=\"{name}\">{column.desc}</param> ");
                }
                {
                    writer.WriteLineEx($"  public {type} {name} {{get; private set;}} = {defaultValue};");
                }
                //isFirst = false;
            }
        }
    }
}
