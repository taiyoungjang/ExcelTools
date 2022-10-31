using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StreamWrite.Extension;
using System.CodeDom.Compiler;

namespace TableGenerate
{
    public class ExportToProto : ExportBase
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
        public eGenType _gen_type = eGenType.proto;
        private bool _useInterface;
        public ExportToProto(string unityDefine, bool useInterface)
        {
            _unityDefine = unityDefine;
            _useInterface = useInterface;
        }

        public override bool Generate(System.Reflection.Assembly[] refAssembly, System.Reflection.Assembly mscorlibAssembly, ClassUtil.ExcelImporter imp, string outputPath, string sFileName, ref int current, ref int max, string language, List<string> except)
        {
            try
            {
                string createFileName = System.Text.RegularExpressions.Regex.Replace(sFileName, @"\.[x][l][s]?\w", ".proto");

                using (MemoryStream stream = new MemoryStream())
                {
                    var _writer = new IndentedTextWriter(new StreamWriter(stream, Encoding.UTF8), " ");
                    {
                        string filename = System.IO.Path.GetFileName(createFileName);

                        _writer.WriteLineEx($"syntax = \"proto3\";");
                        _writer.WriteLineEx($"import \"enum.proto\";");
                        _writer.WriteLineEx($"option csharp_namespace = \"Excel.{createFileName.Split('.')[0]}\";");

                        string[] sheets = imp.GetSheetList();

                        filename = filename.Replace(".cs", string.Empty);

                        max = sheets.GetLength(0);
                        current = 0;

                        foreach (string sheetName in sheets)
                        {
                            current++;
                            string trimSheetName = sheetName.Trim().Replace(" ", "_");
                            var rows = imp.GetSheetShortCut(sheetName, language);
                            var columns = ExportBaseUtil.GetColumnInfo(refAssembly, mscorlibAssembly, trimSheetName, rows, except);
                            SheetProcess(_writer, filename, trimSheetName, columns);
                        }
                        _writer.Flush();
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

        private void SheetProcess(IndentedTextWriter _writer, string filename, string sheetName, List<Column> columns)
        {
            if(_useInterface)
            {
                InterfacePropertySheetProcess(sheetName, _writer, columns);
            }

            _writer.WriteLineEx($"message {sheetName}");
            _writer.WriteLineEx("{");
            InnerSheetProcess(_writer, columns);
            //SheetConstructorProcess(_writer, sheetName, columns);
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
            int i = 0;
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
                //if(column.is_key)
                //{
                //    _writer.WriteLineEx($"/// <summary>");
                //    _writer.WriteLineEx($"/// Key Column");
                //    _writer.WriteLineEx($"/// </summary>");
                //}
                //if (_async == "unity3d")
                //{
                //    _writer.WriteLineEx($"public abstract {type} {name} {{get;}}");
                //}
                //else
                {
                    _writer.WriteLineEx($"  {type} {name} = {++i};");
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
