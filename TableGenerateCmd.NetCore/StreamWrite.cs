using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamWrite
{
    namespace Extension
    {
        public static class Util
        {
            private static int s_indent;
            private static int s_indentSize = 2;
            public static void SetIndentSize(int size)
            {
                s_indentSize = size;
            }

            private static int UnIndentCount(string str) => Math.Max(str.Count(t => t == '}') - str.Count(t => t == '{'), 0);
            private static int IndentCount(string str) => Math.Max(str.Count(t => t == '{') - str.Count(t => t == '}'), 0);
            public static void WriteEx(this StreamWriter self, string str)
            {
                s_indent -= UnIndentCount(str) * s_indentSize;
                self.Write($"{string.Empty.PadLeft(s_indent)}{str}");
                s_indent += IndentCount(str) * s_indentSize;
            }
            public static void WriteLineEx(this StreamWriter self, string str)
            {
                s_indent -= UnIndentCount(str) * s_indentSize;
                self.Write($"{string.Empty.PadLeft(s_indent)}{str}\r\n");
                s_indent += IndentCount(str) * s_indentSize;
            }
            public static void WriteLineEx(this StreamWriter self)
            {
                self.Write(Environment.NewLine);
            }
            public static void WriteEx(this IndentedTextWriter self, string str)
            {
                self.Indent -= UnIndentCount(str) * s_indentSize;
                self.WriteLineNoTabs($"{str}");
                self.Indent += IndentCount(str) * s_indentSize;
            }
            public static void WriteLineEx(this IndentedTextWriter self, string str)
            {
                self.Indent -= UnIndentCount(str) * s_indentSize;
                self.WriteLine($"{str}");
                self.Indent += IndentCount(str) * s_indentSize;
            }
            public static void WriteLineEx(this IndentedTextWriter self)
            {
                self.WriteLine(Environment.NewLine);
            }
        }
    }
}
