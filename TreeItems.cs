using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AsmDotNET
{
    public enum ItemType
    {
        CLASS, ASSEMBLY, FIELD, METHOD, PACKAGE
    }

    public abstract class MagicalTreeViewItem : TreeViewItem
    {
        private ItemType type;
        
        public MagicalTreeViewItem(ItemType type)
        {
            this.type = type;
            this.Foreground = Brushes.White;
        }
    }

    public class TreeItemClass : MagicalTreeViewItem
    {
        private Type _class;

        public Type Class
        {
            get
            {
                return this._class;
            }
            set
            {
                this._class = value;
            }
        }

        public TreeItemClass(Type Class) : base(ItemType.CLASS)
        {
            this.Class = Class;
            this.Header = Utils.CreateItem(this.Class.Name, "Images/ClassIcon.png");
        }
    }

    public class TreeItemField : MagicalTreeViewItem
    {
        private FieldInfo _field;

        public FieldInfo Field
        {
            get
            {
                return this._field;
            }
            set
            {
                this._field = value;
            }
        }

        public TreeItemField(FieldInfo Field) : base(ItemType.FIELD)
        {
            this.Field = Field;
            this.Header = Utils.CreateItem(this.Field.Name, "Images/FieldIcon.png");
        }
    }

    public class TreeItemMethod : MagicalTreeViewItem
    {
        private MethodInfo _method;

        public MethodInfo Method
        {
            get
            {
                return this._method;
            }
            set
            {
                this._method = value;
            }
        }

        public TreeItemMethod(MethodInfo Field) : base(ItemType.METHOD)
        {
            this.Method = Method;
            this.Header = Utils.CreateItem(this.Method.Name, "Images/MethodIcon.png");
        }
    }

    public class TreeItemAssembly : MagicalTreeViewItem
    {
        private Assembly _asm;

        public Assembly Assembly
        {
            get
            {
                return this._asm;
            }
            set
            {
                this._asm = value;
            }
        }

        public TreeItemAssembly(Assembly asm) : base(ItemType.ASSEMBLY)
        {
            this.Assembly = asm;
            this.Header = Utils.CreateItem(this.Assembly.FullName, "Images/AssemblyIcon.png");
        }
    }

    public class TreeItemPackage : MagicalTreeViewItem
    {
        private string _packageName;

        public string PackageName
        {
            get
            {
                return this._packageName;
            }
            set
            {
                this._packageName = value;
            }
        }

        public TreeItemPackage(string PackageName) : base(ItemType.PACKAGE)
        {
            this.Header = Utils.CreateItem(PackageName, "Images/PackageIcon.png");
        }
    }
}
