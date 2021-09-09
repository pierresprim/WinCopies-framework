using Microsoft.WindowsAPICodePack.Shell;

using System.Windows.Media.Imaging;

using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.Shell
{
    public class BitmapSources : BitmapSources<ShellObject>
    {
        protected override BitmapSource SmallOverride => InnerObject.Thumbnail.SmallBitmapSource;

        protected override BitmapSource MediumOverride => InnerObject.Thumbnail.MediumBitmapSource;

        protected override BitmapSource LargeOverride => InnerObject.Thumbnail.LargeBitmapSource;

        protected override BitmapSource ExtraLargeOverride => InnerObject.Thumbnail.ExtraLargeBitmapSource;

        public BitmapSources(in ShellObject shellObject) : base(shellObject) { /* Left empty. */ }
    }

    public class BitmapSourceProviderCommon : IO.BitmapSourceProvider
    {
        public BitmapSourceProviderCommon(in bool folder, in IBitmapSources bitmapSources, in bool disposeBitmapSources) : base(
            folder ?
#if !CS9
                (Icons.BitmapSources)
#endif
                Icons.Folder.Instance
            :
#if !CS9
                (Icons.BitmapSources)
#endif
                Icons.File.Instance, bitmapSources, disposeBitmapSources)
        { /* Left empty. */ }
    }

    public class BitmapSourceProvider : BitmapSourceProviderCommon
    {
        public BitmapSourceProvider(in ShellObject shellObject, in bool disposeBitmapSources) : base(shellObject is ShellFolder, new BitmapSources(shellObject), disposeBitmapSources)
        {
            // Left empty.
        }
    }

    public class BitmapSourceProviderCommon2 : BitmapSourceProviderCommon
    {
        public BitmapSourceProviderCommon2(in IBrowsableObjectInfo browsableObjectInfo, in IBitmapSources bitmapSources, in bool disposeBitmapSources) : base(browsableObjectInfo.IsBrowsable(), bitmapSources, disposeBitmapSources)
        {
            // Left empty.
        }
    }
}
