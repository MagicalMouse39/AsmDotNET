using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AsmDotNET
{
    public class KeyWord
    {
        public static List<KeyWord> KeyWords = new List<KeyWord>();
        private string name;
        private Color color;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public Color Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        public KeyWord(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }

        public static void InitKeywords()
        {
            //Primitive types:
            KeyWords.Add(new KeyWord("int", Colors.Blue));
            KeyWords.Add(new KeyWord("float", Colors.Blue));
            KeyWords.Add(new KeyWord("double", Colors.Blue));
            KeyWords.Add(new KeyWord("char", Colors.Blue));
            KeyWords.Add(new KeyWord("string", Colors.Blue));
            KeyWords.Add(new KeyWord("byte", Colors.Blue));
            KeyWords.Add(new KeyWord("uint", Colors.Blue));
            KeyWords.Add(new KeyWord("long", Colors.Blue));
            KeyWords.Add(new KeyWord("ulong", Colors.Blue));
            KeyWords.Add(new KeyWord("short", Colors.Blue));
            KeyWords.Add(new KeyWord("ushort", Colors.Blue));
            KeyWords.Add(new KeyWord("void", Colors.Blue));
            KeyWords.Add(new KeyWord("bool", Colors.Blue));
            KeyWords.Add(new KeyWord("decimal", Colors.Blue));
            KeyWords.Add(new KeyWord("sbyte", Colors.Blue));
            KeyWords.Add(new KeyWord("object", Colors.Blue));
            KeyWords.Add(new KeyWord("var", Colors.Blue));
            KeyWords.Add(new KeyWord("dynamic", Colors.Blue));

            //Values
            KeyWords.Add(new KeyWord("false", Colors.Blue));
            KeyWords.Add(new KeyWord("true", Colors.Blue));
            KeyWords.Add(new KeyWord("default", Colors.Blue));
            KeyWords.Add(new KeyWord("null", Colors.Blue));
            KeyWords.Add(new KeyWord("9", Colors.Green));
            KeyWords.Add(new KeyWord("8", Colors.Green));
            KeyWords.Add(new KeyWord("7", Colors.Green));
            KeyWords.Add(new KeyWord("6", Colors.Green));
            KeyWords.Add(new KeyWord("5", Colors.Green));
            KeyWords.Add(new KeyWord("4", Colors.Green));
            KeyWords.Add(new KeyWord("3", Colors.Green));
            KeyWords.Add(new KeyWord("2", Colors.Green));
            KeyWords.Add(new KeyWord("1", Colors.Green));
            KeyWords.Add(new KeyWord("0", Colors.Green));

            //Modifiers
            KeyWords.Add(new KeyWord("private", Colors.Blue));
            KeyWords.Add(new KeyWord("protected", Colors.Blue));
            KeyWords.Add(new KeyWord("internal", Colors.Blue));
            KeyWords.Add(new KeyWord("public", Colors.Blue));
            KeyWords.Add(new KeyWord("abstract", Colors.Blue));
            KeyWords.Add(new KeyWord("async", Colors.Blue));
            KeyWords.Add(new KeyWord("const", Colors.Blue));
            KeyWords.Add(new KeyWord("event", Colors.Blue));
            KeyWords.Add(new KeyWord("extern", Colors.Blue));
            KeyWords.Add(new KeyWord("in", Colors.Blue));
            KeyWords.Add(new KeyWord("new", Colors.Blue));
            KeyWords.Add(new KeyWord("out", Colors.Blue));
            KeyWords.Add(new KeyWord("override", Colors.Blue));
            KeyWords.Add(new KeyWord("readonly", Colors.Blue));
            KeyWords.Add(new KeyWord("sealed", Colors.Blue));
            KeyWords.Add(new KeyWord("static", Colors.Blue));
            KeyWords.Add(new KeyWord("unsafe", Colors.Blue));
            KeyWords.Add(new KeyWord("virtual", Colors.Blue));
            KeyWords.Add(new KeyWord("volatile", Colors.Blue));

            //Instructions
            KeyWords.Add(new KeyWord("if", Colors.Blue));
            KeyWords.Add(new KeyWord("else", Colors.Blue));
            KeyWords.Add(new KeyWord("switch", Colors.Blue));
            KeyWords.Add(new KeyWord("case", Colors.Blue));
            KeyWords.Add(new KeyWord("while", Colors.Blue));
            KeyWords.Add(new KeyWord("foreach", Colors.Blue));
            KeyWords.Add(new KeyWord("do", Colors.Blue));
            KeyWords.Add(new KeyWord("for", Colors.Blue));
            KeyWords.Add(new KeyWord("break", Colors.Blue));
            KeyWords.Add(new KeyWord("goto", Colors.Blue));
            KeyWords.Add(new KeyWord("return", Colors.Blue));
            KeyWords.Add(new KeyWord("continue", Colors.Blue));
            KeyWords.Add(new KeyWord("throw", Colors.Blue));
            KeyWords.Add(new KeyWord("try", Colors.Blue));
            KeyWords.Add(new KeyWord("catch", Colors.Blue));
            KeyWords.Add(new KeyWord("finally", Colors.Blue));
            KeyWords.Add(new KeyWord("checked", Colors.Blue));
            KeyWords.Add(new KeyWord("unchecked", Colors.Blue));
            KeyWords.Add(new KeyWord("fixed", Colors.Blue));
            KeyWords.Add(new KeyWord("lock", Colors.Blue));

            //Params
            KeyWords.Add(new KeyWord("ref", Colors.Blue));
            
            //Namespace
            KeyWords.Add(new KeyWord("using", Colors.Blue));
            KeyWords.Add(new KeyWord("namespace", Colors.Blue));
            KeyWords.Add(new KeyWord("alias", Colors.Blue));

            //Type
            KeyWords.Add(new KeyWord("is", Colors.Blue));

            //Selection
            KeyWords.Add(new KeyWord("from", Colors.Blue));
            KeyWords.Add(new KeyWord("where", Colors.Blue));
            KeyWords.Add(new KeyWord("select", Colors.Blue));
            KeyWords.Add(new KeyWord("group", Colors.Blue));
            KeyWords.Add(new KeyWord("into", Colors.Blue));
            KeyWords.Add(new KeyWord("orderby", Colors.Blue));
            KeyWords.Add(new KeyWord("join", Colors.Blue));
            KeyWords.Add(new KeyWord("let", Colors.Blue));
            KeyWords.Add(new KeyWord("in", Colors.Blue));
            KeyWords.Add(new KeyWord("on", Colors.Blue));
            KeyWords.Add(new KeyWord("equals", Colors.Blue));
            KeyWords.Add(new KeyWord("by", Colors.Blue));
            KeyWords.Add(new KeyWord("ascending", Colors.Blue));
            KeyWords.Add(new KeyWord("discending", Colors.Blue));

            //Access
            KeyWords.Add(new KeyWord("this", Colors.Blue));
            KeyWords.Add(new KeyWord("base", Colors.Blue));

            //Context
            KeyWords.Add(new KeyWord("add", Colors.Blue));
            KeyWords.Add(new KeyWord("get", Colors.Blue));
            KeyWords.Add(new KeyWord("set", Colors.Blue));
            KeyWords.Add(new KeyWord("partial", Colors.Blue));
            KeyWords.Add(new KeyWord("remove", Colors.Blue));
            KeyWords.Add(new KeyWord("when", Colors.Blue));
            KeyWords.Add(new KeyWord("yield", Colors.Blue));

            //Operators
            KeyWords.Add(new KeyWord("typeof", Colors.Blue));
            KeyWords.Add(new KeyWord("nameof", Colors.Blue));
            KeyWords.Add(new KeyWord("delegate", Colors.Blue));
            KeyWords.Add(new KeyWord("sizeof", Colors.Blue));
            KeyWords.Add(new KeyWord("stackalloc", Colors.Blue));

            //Special chars
            KeyWords.Add(new KeyWord("$", Colors.Gold));
            KeyWords.Add(new KeyWord("@", Colors.Gold));
        }
    }
}
