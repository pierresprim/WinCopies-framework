/* Copyright © Pierre Sprimont, 2021
*
* This file is part of the WinCopies Framework.
*
* The WinCopies Framework is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* The WinCopies Framework is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Windows.Media.Imaging;

using static Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames;

using static WinCopies.IO.ObjectModel.BrowsableObjectInfo;

namespace WinCopies.IO.ComponentSources.Bitmap
{
    public static class Icons
    {
        public abstract class BitmapSources : IBitmapSources
        {
            private readonly Type _type;

            public BitmapSource Small => GetBitmapSource(nameof(Small));

            public BitmapSource Medium => GetBitmapSource(nameof(Medium));

            public BitmapSource Large => GetBitmapSource(nameof(Large));

            public BitmapSource ExtraLarge => GetBitmapSource(nameof(ExtraLarge));

            bool DotNetFix.IDisposable.IsDisposed => false;

            private protected BitmapSources() => _type = typeof(Icons).GetNestedType(GetType().Name + "Static");

            private BitmapSource GetBitmapSource(in string name) => (BitmapSource)_type.GetProperty(name).GetValue(null);

            protected virtual void Dispose(in bool disposing) { /* Left empty. */ }

            void System.IDisposable.Dispose() => Dispose(true);
        }

        public static class FolderStatic
        {
            private static BitmapSource _small;
            private static BitmapSource _medium;
            private static BitmapSource _large;
            private static BitmapSource _extraLarge;

            public static BitmapSource Small => _small
#if CS8
                ??=
#else
                ?? (_small =
#endif
                _TryGetFolderBitmapSource(SmallIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource Medium => _medium
#if CS8
                ??=
#else
                ?? (_medium =
#endif
                _TryGetFolderBitmapSource(MediumIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource Large => _large
#if CS8
                ??=
#else
                ?? (_large =
#endif
                _TryGetFolderBitmapSource(LargeIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource ExtraLarge => _extraLarge
#if CS8
                ??=
#else
                ?? (_extraLarge =
#endif
                _TryGetFolderBitmapSource(ExtraLargeIconSize)
#if !CS8
)
#endif
                ;

            private static BitmapSource _TryGetFolderBitmapSource(in int size) => TryGetBitmapSource(FolderIcon, Shell32, size);
        }

        public class Folder : BitmapSources
        {
            private static Folder _folder;

            public static Folder Instance => _folder
#if CS8
                ??=
#else
                ?? (_folder =
#endif
                new Folder()
#if !CS8
                )
#endif
                ;

            private Folder() { /* Left empty. */ }
        }

        public static class FileStatic
        {
            private static BitmapSource _small;
            private static BitmapSource _medium;
            private static BitmapSource _large;
            private static BitmapSource _extraLarge;

            public static BitmapSource Small => _small
#if CS8
                ??=
#else
                ?? (_small =
#endif
                _TryGetFileBitmapSource(SmallIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource Medium => _medium
#if CS8
                ??=
#else
                ?? (_medium =
#endif
                _TryGetFileBitmapSource(MediumIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource Large => _large
#if CS8
                ??=
#else
                ?? (_large =
#endif
                _TryGetFileBitmapSource(LargeIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource ExtraLarge => _extraLarge
#if CS8
                ??=
#else
                ?? (_extraLarge =
#endif
                _TryGetFileBitmapSource(ExtraLargeIconSize)
#if !CS8
)
#endif
                ;

            private static BitmapSource _TryGetFileBitmapSource(in int size) => TryGetBitmapSource(FileIcon, Shell32, size);
        }

        public class File : BitmapSources
        {
            private static File _file;

            public static File Instance => _file
#if CS8
                ??=
#else
                ?? (_file =
#endif
                new File()
#if !CS8
                )
#endif
                ;

            private File() { /* Left empty. */ }
        }

        public static class ComputerStatic
        {
            private static BitmapSource _computerSmall;
            private static BitmapSource _computerMedium;
            private static BitmapSource _computerLarge;
            private static BitmapSource _computerExtraLarge;

            public static BitmapSource Small => _computerSmall
#if CS8
                ??=
#else
                ?? (_computerSmall =
#endif
                _TryGetComputerBitmapSource(SmallIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource Medium => _computerMedium
#if CS8
                ??=
#else
                ?? (_computerMedium =
#endif
                _TryGetComputerBitmapSource(MediumIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource Large => _computerLarge
#if CS8
                ??=
#else
                ?? (_computerLarge =
#endif
                _TryGetComputerBitmapSource(LargeIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource ExtraLarge => _computerExtraLarge
#if CS8
                ??=
#else
                ?? (_computerExtraLarge =
#endif
                _TryGetComputerBitmapSource(ExtraLargeIconSize)
#if !CS8
)
#endif
                ;

            private static BitmapSource _TryGetComputerBitmapSource(in int size) => TryGetBitmapSource(ComputerIcon, Shell32, size);
        }

        public class Computer : BitmapSources
        {
            private static Computer _computer;

            public static Computer Instance => _computer
#if CS8
                ??=
#else
                ?? (_computer =
#endif
                new Computer()
#if !CS8
                )
#endif
                ;

            private Computer() { /* Left empty. */ }
        }
    }
}
