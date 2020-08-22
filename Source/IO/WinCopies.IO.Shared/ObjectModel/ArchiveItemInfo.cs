/* Copyright © Pierre Sprimont, 2020
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

using Microsoft.WindowsAPICodePack.Win32Native.Shell;

using SevenZip;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

using WinCopies.Collections;
using WinCopies.Util;

using static WinCopies.Util.Util;

namespace WinCopies.IO
{
    namespace ObjectModel
    {
        /// <summary>
        /// Represents an archive item.
        /// </summary>
        public class ArchiveItemInfo : ArchiveItemInfoProvider<IFileSystemObjectInfoProperties, ArchiveFileInfo?>, IArchiveItemInfo<IFileSystemObjectInfoProperties>
        {
            #region Private fields
            private ArchiveFileInfo? _encapsulatedObject;
            private IBrowsableObjectInfo _parent;
            private bool? _isBrowsable;
            #endregion

            #region Properties
            //IShellObjectInfo IArchiveItemInfoProvider.ArchiveShellObject => ArchiveShellObjectOverride;

            #region Overrides
            public override FileType FileType { get; }

            /// <summary>
            /// The <see cref="ArchiveFileInfo"/> that this <see cref="ArchiveItemInfo"/> represents.
            /// </summary>
            public sealed override ArchiveFileInfo? EncapsulatedObjectGeneric => _encapsulatedObject;

            public sealed override IFileSystemObjectInfoProperties ObjectPropertiesGeneric { get; }

            public override FileSystemType ItemFileSystemType => FileSystemType.Archive;

            /// <summary>
            /// Gets a value that indicates whether this <see cref="ArchiveItemInfo"/> is browsable.
            /// </summary>
            public override bool IsBrowsable
            {
                get
                {
                    if (_isBrowsable.HasValue)

                        return _isBrowsable.Value;

                    switch (ObjectPropertiesGeneric.FileType)
                    {
                        case FileType.Folder:
                        case FileType.Drive:
                            // case FileType.Archive:

                            _isBrowsable = true;

                            break;

                        default:

                            _isBrowsable = false;

                            break;
                    }

                    return _isBrowsable.Value;
                }
            }

#if NETFRAMEWORK
        public override IBrowsableObjectInfo Parent => _parent ?? (_parent=GetParent());
#else
            public override IBrowsableObjectInfo Parent => _parent ??= GetParent();
#endif

            /// <summary>
            /// Returns the same value as <see cref="Name"/>.
            /// </summary>
            public override string LocalizedName => Name;

            /// <summary>
            /// Gets the name of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            public override string Name => System.IO.Path.GetFileName(Path);

            #region BitmapSources
            /// <summary>
            /// Gets the small <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            public override BitmapSource SmallBitmapSource => TryGetBitmapSource(SmallIconSize);

            /// <summary>
            /// Gets the medium <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            public override BitmapSource MediumBitmapSource => TryGetBitmapSource(MediumIconSize);

            /// <summary>
            /// Gets the large <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            public override BitmapSource LargeBitmapSource => TryGetBitmapSource(LargeIconSize);

            /// <summary>
            /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(ExtraLargeIconSize);
            #endregion

            public override string ItemTypeName => GetItemTypeName(System.IO.Path.GetExtension(Path), ObjectPropertiesGeneric.FileType);

            /// <summary>
            /// Not applicable for this item kind.
            /// </summary>
            public override string Description => "N/A";

            /// <summary>
            /// If <see cref="EncapsulatedObject"/> has value, gets the size of the inner <see cref="SevenZip.ArchiveFileInfo"/>; otherwise, returns <see langword="null"/>.
            /// </summary>
            public override Size? Size
            {
                get
                {
                    if (_encapsulatedObject.HasValue)

                        return new Size(_encapsulatedObject.Value.Size);

                    else

                        return null;
                }
            }

            /// <summary>
            /// Gets a value that indicates whether this item is a hidden or system item.
            /// </summary>
            public override bool IsSpecialItem
            {
                get
                {
                    if (_encapsulatedObject.HasValue)
                    {
                        var value = (FileAttributes)_encapsulatedObject.Value.Attributes;

                        return value.HasFlag(FileAttributes.Hidden) || value.HasFlag(FileAttributes.System);
                    }

                    else

                        return false;
                }
            }

            /// <summary>
            /// The parent <see cref="IShellObjectInfo"/> of the current archive item.
            /// </summary>
            public override IShellObjectInfo ArchiveShellObject { get; }
            #endregion
            #endregion

            private ArchiveItemInfo(in string path, in FileType fileType, in IShellObjectInfo archiveShellObject, in ArchiveFileInfo? archiveFileInfo/*, DeepClone<ArchiveFileInfo?> archiveFileInfoDelegate*/) : base(path)
            {
                FileType = fileType;

                ObjectPropertiesGeneric = new FileSystemObjectInfoProperties<IFileSystemObjectInfo>(this);

                ArchiveShellObject = archiveShellObject;

                _encapsulatedObject = archiveFileInfo;
            }

            #region Methods
            #region Construction helpers
            ///// <summary>
            ///// Initializes a new instance of the <see cref="ArchiveItemInfo"/> class using a custom factory for <see cref="ArchiveItemInfo"/>s.
            ///// </summary>
            ///// <param name="archiveShellObject">The <see cref="IShellObjectInfo"/> that correspond to the root path of the archive</param>
            ///// <param name="path">The full path to this archive item</param>
            ///// <param name="fileType">The file type of this archive item</param>
            public static ArchiveItemInfo From(in IShellObjectInfo archiveShellObjectInfo, in ArchiveFileInfo archiveFileInfo)
            {
                ThrowIfNull(archiveShellObjectInfo, nameof(archiveShellObjectInfo));

                string extension = System.IO.Path.GetExtension(archiveFileInfo.FileName);

                return new ArchiveItemInfo(System.IO.Path.Combine(archiveShellObjectInfo.Path, archiveFileInfo.FileName), archiveFileInfo.IsDirectory ? FileType.Folder : extension == ".lnk" ? FileType.Link : extension == ".library.ms" ? FileType.Library : FileType.File, archiveShellObjectInfo, archiveFileInfo);
            }

            public static ArchiveItemInfo From(in IShellObjectInfo archiveShellObjectInfo, in string archiveFilePath)
            {
                ThrowIfNull(archiveShellObjectInfo, nameof(archiveShellObjectInfo));

                return new ArchiveItemInfo(System.IO.Path.Combine(archiveShellObjectInfo.Path, archiveFilePath), FileType.Folder, archiveShellObjectInfo, null);
            }
            #endregion

            private IBrowsableObjectInfo GetParent()
            {
                IBrowsableObjectInfo result;

                if (Path.Length > ArchiveShellObject.Path.Length)
                {
                    string path = Path.Substring(0, Path.LastIndexOf(WinCopies.IO.Path.PathSeparator));

                    ArchiveFileInfo? archiveFileInfo = null;

                    using (var extractor = new SevenZipExtractor(ArchiveShellObject.ArchiveFileStream))

                        archiveFileInfo = extractor.ArchiveFileData.FirstOrDefault(item => string.Compare(item.FileName, path, StringComparison.OrdinalIgnoreCase) == 0);

                    result = new ArchiveItemInfo(path, FileType.Folder, ArchiveShellObject, archiveFileInfo);
                }

                else

                    result = ArchiveShellObject;

                return result;
            }

            #region GetItems
            public IEnumerable<IBrowsableObjectInfo> GetItems(in Predicate<ArchiveFileInfoEnumeratorStruct> func) => func is null ? throw GetArgumentNullException(nameof(func)) : GetArchiveItemInfoItems(func);

            /// <summary>
            /// Returns the items of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            /// <returns>An <see cref="IEnumerable{IBrowsableObjectInfo}"/> that enumerates through the items of this <see cref="ArchiveItemInfo"/>.</returns>
            public override IEnumerable<IBrowsableObjectInfo> GetItems() => GetArchiveItemInfoItems(null);

            private IEnumerable<IBrowsableObjectInfo> GetArchiveItemInfoItems(Predicate<ArchiveFileInfoEnumeratorStruct> func) => new Enumerable<IBrowsableObjectInfo>(() => new ArchiveItemInfoEnumerator(this, func));
            #endregion

            protected override void Dispose(in bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)

                    _encapsulatedObject = null;
            }
            #endregion
        }
    }
}
