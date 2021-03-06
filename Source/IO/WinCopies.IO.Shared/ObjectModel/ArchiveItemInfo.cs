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

using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;

using SevenZip;

using System;
using System.Linq;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.Extensions;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Enumeration;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.PropertySystem;

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
    public abstract class ArchiveItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ArchiveItemInfoProvider<TObjectProperties, ArchiveFileInfo?, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IArchiveItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        protected class BrowsableObjectInfoBitmapSources : BrowsableObjectInfoBitmapSources<IArchiveItemInfo>
        {
            /// <summary>
            /// Gets the small <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            protected override BitmapSource SmallBitmapSourceOverride => InnerObject.TryGetBitmapSource(SmallIconSize);

            /// <summary>
            /// Gets the medium <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            protected override BitmapSource MediumBitmapSourceOverride => InnerObject.TryGetBitmapSource(MediumIconSize);

            /// <summary>
            /// Gets the large <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            protected override BitmapSource LargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(LargeIconSize);

            /// <summary>
            /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
            /// </summary>
            protected override BitmapSource ExtraLargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(ExtraLargeIconSize);

            public BrowsableObjectInfoBitmapSources(in IArchiveItemInfo archiveItemInfo) : base(archiveItemInfo) { /* Left empty. */ }
        }

        #region Fields
        private ArchiveFileInfo? _innerObject;
        private IBrowsableObjectInfo _parent;
        private IBrowsabilityOptions _browsability;
        private IBrowsableObjectInfoBitmapSources _bitmapSources;
        #endregion

        #region Properties
        #region Overrides
        /// <summary>
        /// The parent <see cref="IShellObjectInfoBase"/> of the current archive item.
        /// </summary>
        public override IShellObjectInfoBase ArchiveShellObject { get; }

        protected override IBrowsableObjectInfoBitmapSources BitmapSourcesOverride => _bitmapSources;

        protected override IBrowsabilityOptions BrowsabilityOverride
#if CS8
                => _browsability ??= ObjectPropertiesGeneric.FileType switch
                {
#if CS9
                    FileType.Folder or FileType.Drive => BrowsabilityOptions.BrowsableByDefault,// case FileType.Archive:
#else
                    FileType.Folder => BrowsabilityOptions.BrowsableByDefault,
                    FileType.Drive => BrowsabilityOptions.BrowsableByDefault,
#endif
                    _ => BrowsabilityOptions.NotBrowsable
                };
#else
        {
            get
            {
                if (_browsability != null)

                    return _browsability;

                switch (ObjectPropertiesGeneric.FileType)
                {
                    case FileType.Folder:
                    case FileType.Drive:
                        // case FileType.Archive:

                        _browsability = BrowsabilityOptions.BrowsableByDefault;

                        break;

                    default:

                        _browsability = BrowsabilityOptions.NotBrowsable;

                        break;
                }

                return _browsability;
            }
        }
#endif

        /// <summary>
        /// Not applicable for this item type.
        /// </summary>
        protected override string DescriptionOverride => UtilHelpers.NotApplicable;

        /// <summary>
        /// The <see cref="ArchiveFileInfo"/> that this <see cref="ArchiveItemInfo"/> represents.
        /// </summary>
        protected sealed override ArchiveFileInfo? InnerObjectGenericOverride => _innerObject;

        /// <summary>
        /// Gets a value that indicates whether this item is a hidden or system item.
        /// </summary>
        protected override bool IsSpecialItemOverride
        {
            get
            {
                if (_innerObject.HasValue)
                {
                    var value = (FileAttributes)_innerObject.Value.Attributes;

                    return value.HasFlag(FileAttributes.Hidden, FileAttributes.System);
                }

                else

                    return false;
            }
        }

        protected override string ItemTypeNameOverride => FileSystemObjectInfo.GetItemTypeName(System.IO.Path.GetExtension(Path), ObjectPropertiesGeneric.FileType);

        /// <summary>
        /// Returns the same value as <see cref="Name"/>.
        /// </summary>
        public override string LocalizedName => Name;

        /// <summary>
        /// Gets the name of this <see cref="ArchiveItemInfo"/>.
        /// </summary>
        public override string Name => System.IO.Path.GetFileName(Path);

        protected override IBrowsableObjectInfo ParentOverride => _parent
#if CS8
            ??= GetParent();
#else
            ?? (_parent = GetParent());
#endif

        protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;
        #endregion Overrides
        #endregion Properties

        protected ArchiveItemInfo(in string path, in IShellObjectInfoBase archiveShellObject, in ArchiveFileInfo? archiveFileInfo, in ClientVersion clientVersion/*, DeepClone<ArchiveFileInfo?> archiveFileInfoDelegate*/) : base(path, clientVersion)
        {
            ArchiveShellObject = archiveShellObject;

            _innerObject = archiveFileInfo;
        }

        #region Methods
        protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => ArchiveItemInfo.DefaultCustomProcessesSelectorDictionary.Select(this);

        private IBrowsableObjectInfo GetParent()
        {
            if (Path.Length > ArchiveShellObject.Path.Length)
            {
                string path = Path.Substring(0, Path.LastIndexOf(WinCopies.IO.Path.PathSeparator));

                ArchiveFileInfo? archiveFileInfo = null;

                using (var extractor = new SevenZipExtractor(ArchiveShellObject.ArchiveFileStream))

                    archiveFileInfo = extractor.ArchiveFileData.FirstOrDefault(item => string.Compare(item.FileName, path, StringComparison.OrdinalIgnoreCase) == 0);

                return new ArchiveItemInfo(path, FileType.Folder, ArchiveShellObject, archiveFileInfo, ClientVersion);
            }

            return ArchiveShellObject;
        }

        protected override void DisposeManaged()
        {
            _innerObject = null;

            base.DisposeManaged();
        }

        protected override void DisposeUnmanaged()
        {
            _parent = null;
            _browsability = null;
            _bitmapSources = null;

            base.DisposeUnmanaged();
        }
        #endregion Methods
    }

    public class ArchiveItemInfo : ArchiveItemInfo<IFileSystemObjectInfoProperties, ArchiveFileInfoEnumeratorStruct, IEnumerableSelectorDictionary<ArchiveItemInfoItemProvider, IBrowsableObjectInfo>, ArchiveItemInfoItemProvider>, IArchiveItemInfo
    {
        private static readonly BrowsabilityPathStack<IArchiveItemInfo> __browsabilityPathStack = new BrowsabilityPathStack<IArchiveItemInfo>();

        private IFileSystemObjectInfoProperties _objectProperties;

        #region Properties
        public static IBrowsabilityPathStack<IArchiveItemInfo> BrowsabilityPathStack { get; } = __browsabilityPathStack.AsWriteOnly();

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

        public static ISelectorDictionary<IArchiveItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IArchiveItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

        public static IEnumerableSelectorDictionary<ArchiveItemInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new ArchiveItemInfoSelectorDictionary();

        protected sealed override IFileSystemObjectInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;
        #endregion Properties

        protected internal ArchiveItemInfo(in string path, in FileType fileType, in IShellObjectInfoBase archiveShellObject, in ArchiveFileInfo? archiveFileInfo, ClientVersion clientVersion/*, DeepClone<ArchiveFileInfo?> archiveFileInfoDelegate*/) : base(path, archiveShellObject, archiveFileInfo, clientVersion)
#if CS9
                => _objectProperties = archiveFileInfo.HasValue ? new ArchiveItemInfoProperties<IArchiveItemInfoBase>(this, fileType) : new FileSystemObjectInfoProperties(this, fileType);
#else
        {
            if (archiveFileInfo.HasValue)

                _objectProperties = new ArchiveItemInfoProperties<IArchiveItemInfoBase>(this, fileType);

            else

                _objectProperties = new FileSystemObjectInfoProperties(this, fileType);
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

            return new ArchiveItemInfo(System.IO.Path.Combine(archiveShellObjectInfo.Path, archiveFileInfo.FileName), archiveFileInfo.IsDirectory ? FileType.Folder : extension == ".lnk" ? FileType.Link : extension == ".library.ms" ? FileType.Library : FileType.File, archiveShellObjectInfo, archiveFileInfo, archiveShellObjectInfo.ClientVersion);
        }

        public static ArchiveItemInfo From(in IShellObjectInfoBase archiveShellObjectInfo, in string archiveFilePath)
        {
            ThrowIfNull(archiveShellObjectInfo, nameof(archiveShellObjectInfo));
            ThrowIfNullEmptyOrWhiteSpace(archiveFilePath);

            return new ArchiveItemInfo(System.IO.Path.Combine(archiveShellObjectInfo.Path, archiveFilePath), FileType.Folder, archiveShellObjectInfo, null, archiveShellObjectInfo.ClientVersion);
        }
        #endregion

        protected override IEnumerableSelectorDictionary<ArchiveItemInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _objectProperties = null;
        }

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
