using Microsoft.WindowsAPICodePack.Shell;

using System.Windows.Media.Imaging;

using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    public struct ShellObjectInitInfo
    {
        public string Path { get; }

        public FileType FileType { get; }

        public ShellObjectInitInfo(string path, FileType fileType)
        {
            Path = path;

            FileType = fileType;
        }
    }

    public sealed class ShellLinkBrowsabilityOptions : IBrowsabilityOptions, DotNetFix.IDisposable
    {
        private IShellObjectInfoBase _shellObjectInfo;

        public bool IsDisposed => _shellObjectInfo == null;

        public Browsability Browsability => Browsability.RedirectsToBrowsableItem;

        public ShellLinkBrowsabilityOptions(in IShellObjectInfoBase shellObjectInfo) => _shellObjectInfo = shellObjectInfo ?? throw GetArgumentNullException(nameof(shellObjectInfo));

        public IBrowsableObjectInfo RedirectToBrowsableItem() => IsDisposed ? throw GetExceptionForDispose(false) : _shellObjectInfo;

        public void Dispose() => _shellObjectInfo = null;
    }

    namespace Shell
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
}