using System.Drawing;
using WinCopies.GUI.Drawing;
using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.AbstractionInterop
{
    public class MultiIconInfoItemProvider
    {
        public SingleIcon Icon { get; }

        public IMultiIconInfo MultiIconInfo { get; }

        public MultiIconInfoItemProvider(in SingleIcon icon, in IMultiIconInfo multiIconInfo)
        {
            Icon = icon;
            MultiIconInfo = multiIconInfo;
        }
    }

    public class SingleIconInfoItemProvider
    {
        public IconImage Icon { get; }

        public ISingleIconInfo Parent { get; }

        public SingleIconInfoItemProvider(in IconImage icon, in ISingleIconInfo parent)
        {
            Icon = icon;
            Parent = parent;
        }
    }

    public class IconImageInfoItemProvider : Util.Data.NamedObject
    {
        public Bitmap Bitmap { get; }

        public Icon Icon { get; }

        public string Name { get; }

        public IIconImageInfo Parent { get; }

        private IconImageInfoItemProvider(in string name, in IIconImageInfo parent)
        {
            Name = name;
            Parent = parent;
        }

        public IconImageInfoItemProvider(in string name, in Bitmap bitmap, in IIconImageInfo iconImageInfo) : this(name, iconImageInfo) => Bitmap = bitmap;

        public IconImageInfoItemProvider(in string name, in Icon icon, in IIconImageInfo iconImageInfo) : this(name, iconImageInfo) => Icon = icon;
    }
}
