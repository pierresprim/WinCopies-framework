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

    public class BrowsableObjectInfoBitmapSources : BrowsableObjectInfoBitmapSources<ShellObject>
    {
        protected override BitmapSource SmallBitmapSourceOverride => InnerObject.Thumbnail.SmallBitmapSource;

        protected override BitmapSource MediumBitmapSourceOverride => InnerObject.Thumbnail.MediumBitmapSource;

        protected override BitmapSource LargeBitmapSourceOverride => InnerObject.Thumbnail.LargeBitmapSource;

        protected override BitmapSource ExtraLargeBitmapSourceOverride => InnerObject.Thumbnail.ExtraLargeBitmapSource;

        public BrowsableObjectInfoBitmapSources(in ShellObject shellObject) : base(shellObject) { /* Left empty. */ }
    }
}
