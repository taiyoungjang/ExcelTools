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
            static int indent = 0;
            public static void WriteEx(this StreamWriter self, string str)
            {
                indent -= str.Count(t => t == '}') * 2;
                self.Write($"{string.Empty.PadLeft(indent)}{str}");
                indent += str.Count(t => t == '{') * 2;
            }
            public static void WriteLineEx(this StreamWriter self, string str)
            {
                indent -= str.Count(t => t == '}') * 2;
                self.Write($"{string.Empty.PadLeft(indent)}{str}\r\n");
                indent += str.Count(t => t == '{') * 2;
            }
            public static void WriteLineEx(this StreamWriter self)
            {
                self.Write("\r\n");
            }
            public static void WriteEx(this IndentedTextWriter self, string str)
            {
                self.Indent -= str.Count(t => t == '}') * 2;
                self.WriteLineNoTabs($"{str}");
                self.Indent += str.Count(t => t == '{') * 2;
            }
            public static void WriteLineEx(this IndentedTextWriter self, string str)
            {
                self.Indent -= str.Count(t => t == '}') * 2;
                self.WriteLine($"{str}");
                self.Indent += str.Count(t => t == '{') * 2;
            }
            public static void WriteLineEx(this IndentedTextWriter self)
            {
                self.WriteLine(Environment.NewLine);
            }
        }
    }
}