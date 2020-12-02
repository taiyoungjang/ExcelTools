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

        public static String NameSpace = string.Empty;
        public int m_current = 0;
        public eGenType _gen_type = eGenType.cs;
        public ExportToCS(string unityDefine)
        {
            _unityDefine = unityDefine;
        }

        public override bool Generate(System.Reflection.Assembly refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            try
            {
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".cs");

                using (MemoryStream stream = new MemoryStream())
                {
                    var _writer = new IndentedTextWriter(new StreamWriter(stream, new System.Text.ASCIIEncoding()), " ");
                    {
                        string filename = System.IO.Path.GetFileName(createFileName);

                        _writer.WriteLineEx($"// generate {filename}");
                        _writer.WriteLineEx("// DO NOT TOUCH SOURCE....");
                        _writer.WriteLineEx("#pragma warning disable IDE0007, IDE0011, IDE0025, IDE1006, IDE0018");

                        string[] sheets = imp.GetSheetList();

                        filename = filename.Replace(".cs", string.Empty);

                        max = sheets.GetLength(0);
                        current = 0;

                        _writer.WriteLineEx($"namespace {ExportToCSMgr.NameSpace}.{filename}");
                        _writer.WriteLineEx("{");

                        foreach (string sheetName in sheets)
                        {
                            current++;
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            SheetProcess(_writer, filename, trimSheetName, columns);
                        }
                        _writer.WriteLineEx("};");
                        _writer.Flush();
                    }
                    ExportBaseUtil.CheckReplaceFile(stream, $"{outputPath}/{createFileName}");
                }
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
            return true;
        }

        private void SheetProcess(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            InterfacePropertySheetProcess(sheetName, _writer, columns);

            _writer.WriteLineEx($"public partial class {sheetName}");
            _writer.WriteLineEx("{");
            InnerSheetProcess(_writer, columns);
            SheetConstructorProcess(_writer, sheetName, columns);
            _writer.WriteLineEx("}");
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

        private void InnerSheetProcess(IndentedTextWriter _writer, List<Column> columns)
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
                    _writer.WriteLineEx($"  public readonly {type} {name};");
                }
            }
        }
        private void SheetConstructorProcess(IndentedTextWriter _writer, string sheetName, List<Column> columns)
        {
            //if (_async != "unity3d")
            {
                _writer.WriteLineEx(string.Format("public {0} ({1})",
                    sheetName,
                    string.Join(",", columns.Where(t => t.is_generated == true && t.array_index <= 0).Select(t => $"{t.GenerateType(_gen_type)} {t.var_name}__").ToArray()))
                );
                _writer.WriteLineEx($"{{");
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
                    _writer.WriteLineEx($"this.{name} = {name}__;");
                }
                _writer.WriteLineEx($"}}");
            }
        }
    }
}
