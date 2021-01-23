﻿/* Copyright © Pierre Sprimont, 2020
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
using System.Linq;
using System.Windows.Media.Imaging;

using WinCopies.Collections;
using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Enumeration;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;

using static WinCopies.
#if !WinCopies3
    Util.Util
#else
    ThrowHelper
#endif
    ;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// Represents an archive item.
    /// </summary>
    public abstract class ArchiveItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ArchiveItemInfoProvider<TObjectProperties, ArchiveFileInfo?, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IArchiveItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Private fields
        private ArchiveFileInfo? _encapsulatedObject;
        private IBrowsableObjectInfo _parent;
        private bool? _isBrowsable;
        #endregion

        #region Properties
        //IShellObjectInfo IArchiveItemInfoProvider.ArchiveShellObject => ArchiveShellObjectOverride;

        #region Overrides
        /// <summary>
        /// The <see cref="ArchiveFileInfo"/> that this <see cref="ArchiveItemInfo"/> represents.
        /// </summary>
        public sealed override ArchiveFileInfo? EncapsulatedObjectGeneric => _encapsulatedObject;

        // public override FileSystemType ItemFileSystemType => FileSystemType.Archive;

        /// <summary>
        /// Gets a value that indicates whether this <see cref="ArchiveItemInfo"/> is browsable.
        /// </summary>
        public override bool IsBrowsable
#if CS8
                => _isBrowsable ?? (_isBrowsable = ObjectPropertiesGeneric.FileType switch
                {
#if CS9
                    FileType.Folder or FileType.Drive => true,// case FileType.Archive:
#else
                    FileType.Folder => true,
                    FileType.Drive => true,
#endif
                    _ => false
                }).Value;
#else
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
#endif

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
        #endregion // BitmapSources

        public override string ItemTypeName => FileSystemObjectInfo. GetItemTypeName(System.IO.Path.GetExtension(Path), ObjectPropertiesGeneric.FileType);

        /// <summary>
        /// Not applicable for this item kind.
        /// </summary>
        public override string Description => "N/A";

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

                    return value.HasFlag(FileAttributes.Hidden, FileAttributes.System);
                }

                else

                    return false;
            }
        }

        /// <summary>
        /// The parent <see cref="IShellObjectInfoBase"/> of the current archive item.
        /// </summary>
        public override IShellObjectInfoBase ArchiveShellObject { get; }
        #endregion // Overrides
        #endregion // Properties

        protected ArchiveItemInfo(in string path, in IShellObjectInfoBase archiveShellObject, in ArchiveFileInfo? archiveFileInfo/*, DeepClone<ArchiveFileInfo?> archiveFileInfoDelegate*/) : base(path)
        {
            ArchiveShellObject = archiveShellObject;

            _encapsulatedObject = archiveFileInfo;
        }

        #region Methods
        private IBrowsableObjectInfo GetParent()
        {
            if (Path.Length > ArchiveShellObject.Path.Length)
            {
                string path = Path.Substring(0, Path.LastIndexOf(WinCopies.IO.Path.PathSeparator));

                ArchiveFileInfo? archiveFileInfo = null;

                using (var extractor = new SevenZipExtractor(ArchiveShellObject.ArchiveFileStream))

                    archiveFileInfo = extractor.ArchiveFileData.FirstOrDefault(item => string.Compare(item.FileName, path, StringComparison.OrdinalIgnoreCase) == 0);

                return new ArchiveItemInfo(path, FileType.Folder, ArchiveShellObject, archiveFileInfo);
            }

            return ArchiveShellObject;
        }

        protected override void Dispose(in bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)

                _encapsulatedObject = null;
        }
        #endregion // Methods
    }

    public class ArchiveItemInfo : ArchiveItemInfo<IFileSystemObjectInfoProperties, ArchiveFileInfoEnumeratorStruct, IBrowsableObjectInfoSelectorDictionary<ArchiveItemInfoItemProvider>, ArchiveItemInfoItemProvider>, IArchiveItemInfo
    {
        #region Properties
        public static IBrowsableObjectInfoSelectorDictionary<ArchiveItemInfoItemProvider> DefaultItemSelectorDictionary { get; } = new ArchiveItemInfoSelectorDictionary();

        public override bool IsBrowsableByDefault => true;

        public sealed override IFileSystemObjectInfoProperties ObjectPropertiesGeneric { get; }

        public override IPropertySystemCollection ObjectPropertySystem => null;
        #endregion // Properties

        protected internal ArchiveItemInfo(in string path, in FileType fileType, in IShellObjectInfoBase archiveShellObject, in ArchiveFileInfo? archiveFileInfo/*, DeepClone<ArchiveFileInfo?> archiveFileInfoDelegate*/) : base(path, archiveShellObject, archiveFileInfo)
#if CS9
                => ObjectPropertiesGeneric = archiveFileInfo.HasValue ? new ArchiveItemInfoProperties<IArchiveItemInfoBase>(this, fileType) : new FileSystemObjectInfoProperties(this, fileType);
#else
        {
            if (archiveFileInfo.HasValue)

                ObjectPropertiesGeneric = new ArchiveItemInfoProperties<IArchiveItemInfoBase>(this, fileType);

            else

                ObjectPropertiesGeneric = new FileSystemObjectInfoProperties(this, fileType);
        }
#endif

        #region Methods
        #region Construction helpers
        ///// <summary>
        ///// Initializes a new instance of the <see cref="ArchiveItemInfo"/> class using a custom factory for <see cref="ArchiveItemInfo"/>s.
        ///// </summary>
        ///// <param name="archiveShellObject">The <see cref="IShellObjectInfo"/> that correspond to the root path of the archive</param>
        ///// <param name="path">The full path to this archive item</param>
        ///// <param name="fileType">The file type of this archive item</param>
        public static ArchiveItemInfo From(in IShellObjectInfoBase archiveShellObjectInfo, in ArchiveFileInfo archiveFileInfo)
        {
            ThrowIfNull(archiveShellObjectInfo, nameof(archiveShellObjectInfo));

            string extension = System.IO.Path.GetExtension(archiveFileInfo.FileName);

            return new ArchiveItemInfo(System.IO.Path.Combine(archiveShellObjectInfo.Path, archiveFileInfo.FileName), archiveFileInfo.IsDirectory ? FileType.Folder : extension == ".lnk" ? FileType.Link : extension == ".library.ms" ? FileType.Library : FileType.File, archiveShellObjectInfo, archiveFileInfo);
        }

        public static ArchiveItemInfo From(in IShellObjectInfoBase archiveShellObjectInfo, in string archiveFilePath)
        {
            ThrowIfNull(archiveShellObjectInfo, nameof(archiveShellObjectInfo));
            ThrowIfNullEmptyOrWhiteSpace(archiveFilePath);

            return new ArchiveItemInfo(System.IO.Path.Combine(archiveShellObjectInfo.Path, archiveFilePath), FileType.Folder, archiveShellObjectInfo, null);
        }
        #endregion

        public override IBrowsableObjectInfoSelectorDictionary<ArchiveItemInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

        #region GetItems
        protected override System.Collections.Generic.IEnumerable<ArchiveItemInfoItemProvider> GetItemProviders(Predicate<ArchiveFileInfoEnumeratorStruct> predicate) => GetArchiveItemInfoItems(this, predicate);

        /// <summary>
        /// Returns the items of this <see cref="ArchiveItemInfo"/>.
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{IBrowsableObjectInfo}"/> that enumerates through the items of this <see cref="ArchiveItemInfo"/>.</returns>
        protected override System.Collections.Generic.IEnumerable<ArchiveItemInfoItemProvider> GetItemProviders() => GetArchiveItemInfoItems(this, item => true);

        internal static System.Collections.Generic.IEnumerable<ArchiveItemInfoItemProvider> GetArchiveItemInfoItems(IArchiveItemInfoProvider item, Predicate<ArchiveFileInfoEnumeratorStruct> func) => new Enumerable<ArchiveItemInfoItemProvider>(() => new ArchiveItemInfoEnumerator(item, func));
        #endregion // GetItems
        #endregion // Methods
    }
}
