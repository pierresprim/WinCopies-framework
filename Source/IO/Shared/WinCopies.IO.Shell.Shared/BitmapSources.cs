using Microsoft.WindowsAPICodePack.Shell;

using System.Windows.Media.Imaging;

using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;

using static WinCopies.IO.ObjectModel.BrowsableObjectInfo;

namespace WinCopies.IO
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

    namespace Shell
    {
        public struct BitmapSourcesStruct
        {
            public int IconIndex { get; }

            public string DllName { get; }

            public BitmapSourcesStruct(in int iconIndex, in string dllName)
            {
                IconIndex = iconIndex;

                DllName = dllName;
            }
        }

        public class BitmapSources : BitmapSources2<BitmapSourcesStruct>
        {
            public BitmapSources(in BitmapSourcesStruct parameters) : base(parameters) { /* Left empty. */ }

            protected BitmapSource TryGetBitmapSource(in int size) => ObjectModel.BrowsableObjectInfo.TryGetBitmapSource(InnerObject.IconIndex, InnerObject.DllName, size);

            protected override BitmapSource GetSmall() => TryGetBitmapSource(SmallIconSize);

            protected override BitmapSource GetMedium() => TryGetBitmapSource(MediumIconSize);

            protected override BitmapSource GetLarge() => TryGetBitmapSource(LargeIconSize);

            protected override BitmapSource GetExtraLarge() => TryGetBitmapSource(ExtraLargeIconSize);
        }

        public class BitmapSourceProvider : IO.BitmapSourceProvider
        {
            private static IBitmapSources GetDefaultBitmapSources(in BrowsableAs browsableAs)
            {
#if CS8
                return
#else
                switch (
#endif
                    browsableAs
#if CS8
                    switch
#else
                    )
#endif
                    {
#if !CS8
                    case
#endif
                        BrowsableAs.File
#if CS8
                        =>
#else
                        : return
#endif
                        Icons.File.Instance
#if CS8
                        ,
#else
                        ; case
#endif
                        BrowsableAs.Folder
#if CS8
                        =>
#else
                        : return
#endif
                        Icons.Folder.Instance
#if CS8
                        ,
#else
                        ; case
#endif
                        BrowsableAs.LocalRoot
#if CS8
                        =>
#else
                        : return
#endif
                        Icons.Computer.Instance
#if CS8
                        ,
                        _ =>
#else
                        ; default:
#endif
                        throw new InvalidEnumArgumentException(nameof(BrowsableAs), browsableAs)
#if CS8
                    };
#else
                    ;
                }
#endif
            }

            public BitmapSourceProvider(in BrowsableAs browsableAs, in IBitmapSources intermediate, in IBitmapSources sources, in bool disposeBitmapSources) : base(GetDefaultBitmapSources(browsableAs), intermediate, sources, disposeBitmapSources) { /* Left empty. */ }

            public BitmapSourceProvider(in IBrowsableObjectInfo browsableObjectInfo, in IBitmapSources intermediate, in IBitmapSources bitmapSources, in bool disposeBitmapSources) : this(browsableObjectInfo.GetBrowsableAsValue(), intermediate, bitmapSources, disposeBitmapSources) { /* Left empty. */ }

            public BitmapSourceProvider(in IBrowsableObjectInfo browsableObjectInfo, in IBitmapSources bitmapSources, in bool disposeBitmapSources) : this(browsableObjectInfo.GetBrowsableAsValue(), null, bitmapSources, disposeBitmapSources) { /* Left empty. */ }

            public BitmapSourceProvider(in IBrowsableObjectInfo browsableObjectInfo) : this(browsableObjectInfo, null, false) { /* Left empty. */ }
        }
    }
}
