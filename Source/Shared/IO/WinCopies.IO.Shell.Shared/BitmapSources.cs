using Microsoft.WindowsAPICodePack.Shell;

using System.Windows.Media.Imaging;

using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;

using static WinCopies.IO.ObjectModel.BrowsableObjectInfo;

namespace WinCopies.IO
{
    namespace ComponentSources.Bitmap
    {
        public class ShellObjectBitmapSources : BitmapSources<ShellObject>
        {
            protected override BitmapSource SmallOverride => InnerObject.Thumbnail.SmallBitmapSource;

            protected override BitmapSource MediumOverride => InnerObject.Thumbnail.MediumBitmapSource;

            protected override BitmapSource LargeOverride => InnerObject.Thumbnail.LargeBitmapSource;

            protected override BitmapSource ExtraLargeOverride => InnerObject.Thumbnail.ExtraLargeBitmapSource;

            public ShellObjectBitmapSources(in ShellObject shellObject) : base(shellObject) { /* Left empty. */ }
        }

        public class FileSystemObjectInfoBitmapSources<T> : BitmapSources<IFileSystemObjectInfo<T>> where T : IFileSystemObjectInfoProperties
        {
            protected BitmapSource TryGetBitmapSource(in int size) => FileSystemObjectInfo.TryGetDefaultBitmapSource(InnerObject, size);

            protected override BitmapSource SmallOverride => TryGetBitmapSource(SmallIconSize);

            protected override BitmapSource MediumOverride => TryGetBitmapSource(ExtraLargeIconSize);

            protected override BitmapSource LargeOverride => TryGetBitmapSource(ExtraLargeIconSize);

            protected override BitmapSource ExtraLargeOverride => TryGetBitmapSource(ExtraLargeIconSize);

            public FileSystemObjectInfoBitmapSources(in IFileSystemObjectInfo<T> fileSystemObjectInfo) : base(fileSystemObjectInfo) { /* Left empty. */ }
        }
    }
}
