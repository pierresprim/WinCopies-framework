using System;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.Selectors
{
    public class MultiIconInfoSelectorDictionary : EnumerableSelectorDictionary<MultiIconInfoItemProvider, IBrowsableObjectInfo>
    {
        public static IBrowsableObjectInfo Convert(MultiIconInfoItemProvider multiIconInfoItemProvider) => new SingleIconInfo(multiIconInfoItemProvider.MultiIconInfo.Path, multiIconInfoItemProvider.MultiIconInfo.ClientVersion, multiIconInfoItemProvider.Icon);

        protected override Converter<MultiIconInfoItemProvider, IBrowsableObjectInfo> DefaultActionOverride => Convert;

        public MultiIconInfoSelectorDictionary() { /* Left empty. */ }
    }

    public class SingleIconInfoSelectorDictionary : EnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo>
    {
        public static IBrowsableObjectInfo Convert(SingleIconInfoItemProvider icon) => new IconImageInfo(icon.Icon, icon.Parent);

        protected override Converter<SingleIconInfoItemProvider, IBrowsableObjectInfo> DefaultActionOverride => Convert;

        public SingleIconInfoSelectorDictionary() { /* Left empty. */ }
    }

    public class IconImageInfoSelectorDictionary : EnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo>
    {
        public static IBrowsableObjectInfo Convert(IconImageInfoItemProvider icon) => icon.Bitmap == null ? new IconInfo(icon.Parent, icon.Name, icon.Icon) : (IBrowsableObjectInfo)new BitmapInfo(icon.Parent, icon.Name, icon.Bitmap);

        protected override Converter<IconImageInfoItemProvider, IBrowsableObjectInfo> DefaultActionOverride => Convert;

        public IconImageInfoSelectorDictionary() { /* Left empty. */ }
    }
}
