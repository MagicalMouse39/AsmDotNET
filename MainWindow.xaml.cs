using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.TypeSystem;
using Mono.Cecil;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace AsmDotNET
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static class CursorPosition
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct PointInter
            {
                public int X;
                public int Y;
                public static explicit operator System.Windows.Point(PointInter point) => new System.Windows.Point(point.X, point.Y);
            }

            [DllImport("user32.dll")]
            public static extern bool GetCursorPos(out PointInter lpPoint);

            // For your convenience
            public static System.Windows.Point GetCursorPosition()
            {
                PointInter lpPoint;
                GetCursorPos(out lpPoint);
                return (System.Windows.Point)lpPoint;
            }
        }

        private Rectangle currentScreen;
        private List<Assembly> assemblies;
        private SearchForm searchForm;
        private Assembly selectedAssembly;

        private void RecursiveAddResource(Type t, MagicalTreeViewItem item)
        {
            foreach (string pkg in t.Namespace.Split('.'))
            {
                if (item.Items.GetItemByName(pkg) == null)
                {
                    MagicalTreeViewItem newItem = new TreeItemPackage(pkg);
                    item.Items.Add(newItem);
                    item = newItem;
                }
                foreach (Type type in t.Assembly.GetTypes())
                    if (item.Header.GetText() == type.Namespace.Split('.').Last())
                        if (item.Items.GetItemByName(type.Name) == null)
                        {
                            MagicalTreeViewItem itemClass = new TreeItemClass(type);
                            item.Items.Add(itemClass);
                            /*foreach (FieldInfo fi in type.GetFields())
                                itemClass.Items.Add(new TreeItemField(fi));
                            foreach (MethodInfo mi in type.GetMethods())
                                try
                                {
                                    itemClass.Items.Add(new TreeItemMethod(mi));
                                }
                                catch { }*/
                        }
            }
        }

        private void AddByNamespace(Assembly asm, MagicalTreeViewItem item)
        {
            /*if (ns.Split('.').Length == 1)
                item.Items.Add(new TreeViewItem() { Header = ns, Foreground = System.Windows.Media.Brushes.White });
            else
                foreach (string f in ns.Split('.'))
                {
                    if (item.Items.GetItemByName(f) == null)
                    {
                        foreach (Type t in asm.GetTypes())
                        {
                            MagicalTreeViewItem i = null;
                            if (t.Name == f)
                                if (t.IsClass || t.IsInterface || t.IsEnum || t.IsAbstract)
                                {
                                    i = new TreeItemClass(t) { Header = f, Foreground = System.Windows.Media.Brushes.White };
                                    //this.AddFieldsToClass(i);
                                }
                            this.AddByNamespace(asm, i, ns.Substring(ns.Split('.')[0].Length + 1));
                            item.Items.Add(i);
                        }
                    }
                    else
                        this.AddByNamespace(asm, item.Items.GetItemByName(f), ns.Substring(ns.Split('.')[0].Length + 1));
                    break;
                }*/
            //Namespaces
            foreach (Type type in asm.GetTypes())
            {
                this.RecursiveAddResource(type, item);
            }
        }

        private void AddAssembly(Assembly asm)
        {
            this.assemblies.Add(asm);

            TreeItemAssembly assemblyItem = new TreeItemAssembly(asm);

            try
            {
                this.AddByNamespace(asm, assemblyItem);
            }
            catch { }


            this.AssemblyTree.Items.Add(assemblyItem);
            this.selectedAssembly = asm;
        }

        private string CodeBoxGetText()
        {
            var dispatcher = this.CodeBox.Dispatcher;
            string test = "";
            dispatcher.BeginInvoke(new Action(() => test = this.CodeBox.GetText()));
            return test;
        }

        private void CodeBoxAppendText(string text)
        {
            var dispatcher = this.CodeBox.Dispatcher;
            dispatcher.BeginInvoke(new Action(() => this.CodeBox.AppendText(text)));
        }

        private void CodeBoxSetText(string text)
        {
            var dispatcher = this.CodeBox.Dispatcher;
            dispatcher.BeginInvoke(new Action(() => this.CodeBox.SetText(text)));
        }

        private void CodeBoxClear()
        {
            this.CodeBox.Dispatcher.BeginInvoke(new Action(() => this.CodeBox.Document.Blocks.Clear()));
        }

        private void Reflect()
        {
            if ((MagicalTreeViewItem)AssemblyTree.SelectedItem == null || AssemblyTree.SelectedItem.GetType() != typeof(TreeItemClass))
                return;
            Type t = ((TreeItemClass)AssemblyTree.SelectedItem).Class;
            string selName = ((TreeViewItem)AssemblyTree.SelectedItem).Header.GetText();
            this.CodeBoxClear();
            this.selectedAssembly = t.Assembly;
            if ((bool)MSILCB.IsChecked)
            {
                if (t.IsClass)
                {
                    this.CodeBoxAppendText((t.IsPublic ? "public " : t.IsNestedAssembly ? "internal " : t.IsNestedFamily ? "protected " : "") + (t.IsSealed ? "static " : "") + " class " + t.Name + "\n{");

                    this.CodeBoxAppendText("\n");

                    foreach (FieldInfo fi in t.GetFields())
                    {
                        string val = "";
                        try
                        {
                            val = fi.GetRawConstantValue() != null ? " = " + fi.GetRawConstantValue().ToString() : "";
                        }
                        catch { }
                        this.CodeBoxAppendText("\t" + (fi.IsPublic ? "public " : fi.IsPrivate ? "private " : fi.IsFamily ? "protected " : fi.IsAssembly ? "internal " : "") + (fi.IsStatic ? "static " : "") + fi.FieldType.Name + " " + fi.Name + val + ";\n");
                    }

                    this.CodeBoxAppendText("\n");

                    foreach (MethodInfo mi in t.GetMethods())
                    {
                        string parameters = "";
                        List<Type> ptypes = new List<Type>();
                        foreach (var p in mi.GetParameters())
                        {
                            parameters += $"{p.ParameterType.Name} {p.Name}, ";
                            ptypes.Add(p.ParameterType);
                        }

                        if (parameters.Length > 0)
                            parameters = parameters.Substring(0, parameters.Length - 2);

                        this.CodeBoxAppendText("\t" + (mi.IsPublic ? "public" : mi.IsFamily ? "protected" : mi.IsAssembly ? "internal" : mi.IsPrivate ? "private" : "???") + " " + (mi.IsStatic ? "static " : "") + (mi.IsAbstract ? "abstract " : "") + (mi.IsVirtual ? "virtual " : "") + (mi.IsFinal ? "readonly " : "") + mi.ReturnType.Name + " " + mi.Name + "(" + parameters + ")\n" + "\t{\n");

                        if (mi.GetMethodBody() != null)
                        {
                            foreach (var ins in Disassembler.GetInstructions(mi))
                                this.CodeBoxAppendText($"\t\t{ins.OpCode} {ins.Operand}\n");
                        }
                        this.CodeBoxAppendText("\t}\n\n");
                    }
                    try
                    {
                        this.CodeBoxSetText(this.CodeBoxGetText().Substring(0, this.CodeBoxGetText().Length - 3));
                    }
                    catch { }
                    this.CodeBoxAppendText("}");
                }
            }
            else
            {
                DataTable table = new DataTable();

                DataColumn methodNameTable = new DataColumn("Method Name", typeof(string));
                DataColumn opCodeTable = new DataColumn("OpCode", typeof(string));
                DataColumn operandTable = new DataColumn("Operand", typeof(object));

                table.Columns.AddRange(new DataColumn[] { methodNameTable, opCodeTable, operandTable });

                foreach (MethodInfo mi in t.GetMethods())
                    if (mi.GetMethodBody() != null)
                        foreach (var ins in Disassembler.GetInstructions(mi))
                        {
                            DataRow row = table.NewRow();
                            row[0] = mi.Name;
                            row[1] = ins.OpCode;
                            row[2] = ins.Operand;
                            table.Rows.Add(row);
                        }

                this.MSILGrid.ItemsSource = table.DefaultView;

                this.CodeBoxSetText(new CSharpDecompiler(this.selectedAssembly.Location, new DecompilerSettings()).DecompileTypeAsString(new FullTypeName(t.FullName)));
            }
        }

        private void Resize()
        {
            var interopHelper = new WindowInteropHelper(Application.Current.MainWindow);
            var activeScreen = System.Windows.Forms.Screen.FromHandle(interopHelper.Handle);
            this.currentScreen = activeScreen.Bounds;
            this.TitleBar.Width = this.Width;
            this.TitleBar.Margin = new Thickness(-(this.Width / 10 * 2), 0, 0, 0);
            this.SidePanel.Width = this.Width / 10 * 2;
            this.ButtonsPanel.Width = this.Width / 10 * 2;
            this.CodeBox.Width = (bool)this.MSILCB.IsChecked ? this.MainPanel.Width : this.MainPanel.Width / 10 * 7;
            this.MSILGrid.Width = (bool)this.MSILCB.IsChecked ? 0 : this.MainPanel.Width / 10 * 3;
            try
            {
                this.MainPanel.Width = this.Width - this.SidePanel.Width;
                this.MainPanel.Height = this.Height - this.TitleBar.Height;
            }
            catch { }
        }

        public MainWindow()
        {
            InitializeComponent();

            KeyWord.InitKeywords();

            this.assemblies = new List<Assembly>();
            
            this.CloseBtn.Click += (s, e) => this.Close();
            this.MinimizeBtn.Click += (s, e) => this.WindowState = WindowState.Minimized;
            this.MaximizeBtn.Click += (s, e) => this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;

            this.CodeBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    if (this.searchForm == null || this.searchForm.IsDisposed)
                    {
                        this.searchForm = new SearchForm(this);
                        this.searchForm.Show();
                    }
                    else
                        this.searchForm.BringToFront();
                }
            };

            this.AssemblyTree.SelectedItemChanged += (s, e) => Reflect();
            #region TEST
            /*if (selName == t.Name)
            {
                this.selectedAssembly = a;
                if (t.IsClass)
                {
                    this.CodeBoxAppendText((t.IsPublic ? "public " : t.IsNestedAssembly ? "internal " : t.IsNestedFamily ? "protected " : "") + (t.IsSealed ? "static " : "") + " class " + t.Name + "\n{");

                    this.CodeBoxAppendText("\n");

                    foreach (FieldInfo fi in t.GetFields())
                    {
                        string val = "";
                        try
                        {
                            val = fi.GetRawConstantValue() != null ? " = " + fi.GetRawConstantValue().ToString() : "";
                        }
                        catch { }
                        this.CodeBoxAppendText("\t" + (fi.IsPublic ? "public " : fi.IsPrivate ? "private " : fi.IsFamily ? "protected " : fi.IsAssembly ? "internal " : "") + (fi.IsStatic ? "static " : "") + fi.FieldType.Name + " " + fi.Name + val + ";\n");
                    }

                    this.CodeBoxAppendText("\n");

                    foreach (MethodInfo mi in t.GetMethods())
                    {
                        string parameters = "";
                        List<Type> ptypes = new List<Type>();
                        foreach (var p in mi.GetParameters())
                        {
                            parameters += $"{p.ParameterType.Name} {p.Name}, ";
                            ptypes.Add(p.ParameterType);
                        }

                        if (parameters.Length > 0)
                            parameters = parameters.Substring(0, parameters.Length - 2);

                        this.CodeBoxAppendText("\t" + (mi.IsPublic ? "public" : mi.IsFamily ? "protected" : mi.IsAssembly ? "internal" : mi.IsPrivate ? "private" : "???") + " " + (mi.IsStatic ? "static " : "") + (mi.IsAbstract ? "abstract " : "") + (mi.IsVirtual ? "virtual " : "") + (mi.IsFinal ? "readonly " : "") + mi.ReturnType.Name + " " + mi.Name + "(" + parameters + ")\n" + "\t{\n");

                        if (mi.GetMethodBody() != null)
                        {
                            List<Instruction> actionList = new List<Instruction>();
                            byte[] IL = mi.GetMethodBody().GetILAsByteArray();
                            //foreach (var ins in Disassembler.GetInstructions(mi))
                            /*if (ins.OpCode.Name == "ret")
                                this.CodeBoxAppendText($"\t\treturn;\n");
                            else if (ins.OpCode.Name == "nop")
                            {
                                if (actionList.Count == 0)
                                    continue;
                                List<string> p = new List<string>();
                                string csharped = "";
                                foreach (Instruction instruction in actionList)
                                    if (instruction.OpCode.Name == "ldsfld")
                                    {
                                        string op = instruction.Operand.ToString();
                                        try
                                        {
                                            csharped = op.Substring(op.IndexOf("[") + 1, op.IndexOf("]") - op.IndexOf("[") - 1) + "." + op.Split(' ')[op.Split(' ').Length - 1];
                                        }
                                        catch { }
                                    }
                                    else if (instruction.OpCode.Name == "ldstr")
                                        p.Add("\"" + instruction.Operand.ToString() + "\"");
                                    else if (instruction.OpCode.Name == "call")
                                        p.Add(instruction.Operand.ToString().Split(' ')[0] + "." + instruction.Operand.ToString().Split(' ')[1]);
                                    else if (instruction.OpCode.Name == "newobj")
                                        continue;
                                    else if (instruction.OpCode.Name == "callvirt")
                                    {
                                        string pp = "";
                                        string op = instruction.Operand.ToString();
                                        foreach (string p_ in p)
                                            pp += $"{p_}, ";
                                        if (pp.Length != 0)
                                            pp = pp.Substring(0, pp.Length - 2);
                                        string tt = op.Substring(op.IndexOf("(") + 1, op.IndexOf(")") - op.IndexOf("("));
                                        csharped += "." + op.Split(' ')[1].Replace(tt, $"new {tt.Substring(0, tt.Length - 1)}({pp}))");
                                    }

                                this.CodeBoxAppendText($"\t\t{csharped};\n");
                                actionList.Clear();
                            }
                            else if (ins.OpCode.Name == "ldsfld" || ins.OpCode.Name == "ldstr" || ins.OpCode.Name == "call" || ins.OpCode.Name == "newobj" || ins.OpCode.Name == "callvirt")
                                actionList.Add(ins);
                        }
                        this.CodeBoxAppendText("\t}\n\n");
                    }
                    try
                    {
                        this.CodeBoxSetText(this.CodeBoxGetText().Substring(0, this.CodeBoxGetText().Length - 3));
                    }
                    catch { }
                    this.CodeBoxAppendText("}");
                }
            }
    /*TextRange range = new TextRange(this.CodeBox.Document.ContentStart, this.CodeBox.Document.ContentEnd);
    foreach (KeyWord k in KeyWord.KeyWords)
    {
        Regex rx = new Regex(k.Name);
        foreach (Match index in rx.Matches(this.CodeBox.GetText()))
        {
            int x = index.Index;
            try
            {
                TextRange innerRange = new TextRange(range.Start.GetPositionAtOffset(x), range.Start.GetPositionAtOffset(x + k.Name.Length));
                innerRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(k.Color));
            }
            catch (Exception ex) { }
        }
    }*/
            #endregion TEST
            this.MSILCB.Click += (s, e) =>
            {
                if (!(bool)this.MSILCB.IsChecked)
                {
                    this.CodeBox.Width = this.MainPanel.Width / 10 * 7;
                    this.MSILGrid.Width = this.MainPanel.Width / 10 * 3;
                }
                else
                {
                    this.CodeBox.Width = this.MainPanel.Width;
                    this.MSILGrid.Width = 0;
                }
                Reflect();
            };

            this.OpenAssemblyButton.Background = new ImageBrush() { ImageSource = BitmapFrame.Create(Application.GetResourceStream(new Uri("Images/OpenAssembly.png", UriKind.Relative)).Stream) };
            this.SaveAssemblyButton.Background = new ImageBrush() { ImageSource = BitmapFrame.Create(Application.GetResourceStream(new Uri("Images/SaveIcon.png", UriKind.Relative)).Stream) };

            this.OpenAssemblyButton.Click += (s, e) =>
            {
                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog() { Filter = "Assembly Files|*.exe;*.dll|EXE Files|*.exe|DLL Files|*.dll" };
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                this.AddAssembly(Assembly.LoadFile(ofd.FileName));
            };

            this.SaveAssemblyButton.Click += (s, e) =>
            {

            };
            
            var interopHelper = new WindowInteropHelper(Application.Current.MainWindow);
            var activeScreen = System.Windows.Forms.Screen.FromHandle(interopHelper.Handle);
            this.currentScreen = activeScreen.Bounds;

            this.TitleBar.MouseDown += (s, e) =>
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                    return;

                if (this.Width == this.currentScreen.Width && this.Height == this.currentScreen.Height && !this.MaximizeBtn.IsPressed)
                {
                    this.Width = 800;
                    this.Height = 450;
                    this.Left = CursorPosition.GetCursorPosition().X - 10;
                    this.Top = 0;
                }

                DragMove();

                if (e.ClickCount >= 2)
                    this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            };

            this.LocationChanged += (s, e) =>
            {
                if (this.Top == 0)
                    this.WindowState = WindowState.Normal;
                this.Resize();
            };

            this.SizeChanged += (s, e) =>
            {
                this.Resize();
            };

            this.StateChanged += (s, e) =>
            {
                this.Resize();
            };
        }
    }

    internal static class Utils
    {
        public static string prova = "test";

        public static MagicalTreeViewItem GetItemByName(this ItemCollection list, string name)
        {
            foreach (MagicalTreeViewItem i in list)
            {
                if (i.Header.GetText() == name)
                    return i;
            }
            return null;
        }

        public static string GetText(this object Header)
        {
            StackPanel pan = (StackPanel)Header;
            TextBlock tb = null;
            if (pan.Children[0].GetType() == typeof(TextBlock))
                tb = (TextBlock)pan.Children[0];
            else if (pan.Children[1].GetType() == typeof(TextBlock))
                tb = (TextBlock)pan.Children[1];
            Console.WriteLine(tb.Text.Trim());
            return tb.Text.Trim();
        }

        public static string GetText(this RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd).Text;
        }

        public static void SetText(this RichTextBox richTextBox, string text)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        public static object CreateItem(string header, string image)
        {
            TreeViewItem child = new TreeViewItem();
            StackPanel pan = new StackPanel();
            if (!string.IsNullOrEmpty(image))
            {
                pan.Orientation = Orientation.Horizontal;
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Height = 16;
                img.Source = BitmapFrame.Create(Application.GetResourceStream(new Uri(image, UriKind.Relative)).Stream);
                pan.Children.Add(img);
            }
            pan.Children.Add(new TextBlock() { Text = "  " + header });
            return pan;
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}
