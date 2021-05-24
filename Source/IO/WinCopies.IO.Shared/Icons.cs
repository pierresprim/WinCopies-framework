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

using System.Windows.Media.Imaging;

using static WinCopies.IO.ObjectModel.BrowsableObjectInfo;

using static Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames;

namespace WinCopies.IO
{
    public static class Icons
    {
        public static class Folder
        {
            private static BitmapSource _folderSmall;
            private static BitmapSource _folderMedium;
            private static BitmapSource _folderLarge;
            private static BitmapSource _folderExtraLarge;

            public static BitmapSource FolderSmall => _folderSmall
#if CS8
                ??=
                #else
                ?? (_folderSmall =
#endif
                _TryGetFolderBitmapSource(SmallIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource FolderMedium => _folderMedium
#if CS8
                ??=
#else
                ?? (_folderMedium =
#endif
                _TryGetFolderBitmapSource(MediumIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource FolderLarge => _folderLarge
#if CS8
                ??=
#else
                ?? (_folderLarge =
#endif
                _TryGetFolderBitmapSource(LargeIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource FolderExtraLarge => _folderExtraLarge
#if CS8
                ??=
#else
                ?? (_folderExtraLarge =
#endif
                _TryGetFolderBitmapSource(ExtraLargeIconSize)
#if !CS8
)
#endif
                ;

            private static BitmapSource _TryGetFolderBitmapSource(in int size) => TryGetBitmapSource(FolderIcon, Shell32, size);

            public static BitmapSource TryGetFolderBitmapSource(in int size)
            {
                switch (size)
                {
                    case SmallIconSize:

                        return FolderSmall;

                    case MediumIconSize:

                        return FolderMedium;

                    case LargeIconSize:

                        return FolderLarge;

                    case ExtraLargeIconSize:

                        return FolderExtraLarge;
                }

                return null;
            }
        }

        public static class File
        {
            private static BitmapSource _fileSmall;
            private static BitmapSource _fileMedium;
            private static BitmapSource _fileLarge;
            private static BitmapSource _fileExtraLarge;

            public static BitmapSource FileSmall => _fileSmall
#if CS8
                ??=
#else
                ?? (_fileSmall =
#endif
                _TryGetFileBitmapSource(SmallIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource FileMedium => _fileMedium
#if CS8
                ??=
#else
                ?? (_fileMedium =
#endif
                _TryGetFileBitmapSource(MediumIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource FileLarge => _fileLarge
#if CS8
                ??=
#else
                ?? (_fileLarge =
#endif
                _TryGetFileBitmapSource(LargeIconSize)
#if !CS8
)
#endif
                ;

            public static BitmapSource FileExtraLarge => _fileExtraLarge
#if CS8
                ??=
#else
                ?? (_fileExtraLarge =
#endif
                _TryGetFileBitmapSource(ExtraLargeIconSize)
#if !CS8
)
#endif
                ;

            private static BitmapSource _TryGetFileBitmapSource(in int size) => TryGetBitmapSource(FileIcon, Shell32, size);

            public static BitmapSource TryGetFileBitmapSource(in int size)
            {
                switch (size)
                {
                    case SmallIconSize:

                        return FileSmall;

                    case MediumIconSize:

                        return FileMedium;

                    case LargeIconSize:

                        return FileLarge;

                    case ExtraLargeIconSize:

                        return FileExtraLarge;
                }

                return null;
            }
        }

        public static class Computer
        {
            private static BitmapSource _computerSmall;
            private static BitmapSource _computerMedium;
            private static BitmapSource _computerLarge;
            private static BitmapSource _computerExtraLarge;

            public static BitmapSource ComputerSmall => _computerSmall
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

            public static BitmapSource ComputerMedium => _computerMedium
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

            public static BitmapSource ComputerLarge => _computerLarge
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

            public static BitmapSource ComputerExtraLarge => _computerExtraLarge
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

            public static BitmapSource TryGetComputerBitmapSource(in int size)
            {
                switch (size)
                {
                    case SmallIconSize:

                        return ComputerSmall;

                    case MediumIconSize:

                        return ComputerMedium;

                    case LargeIconSize:

                        return ComputerLarge;

                    case ExtraLargeIconSize:

                        return ComputerExtraLarge;
                }

                return null;
            }
        }
    }
}
