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

using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Enumeration;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;

using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.IO.Path;
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

    namespace ObjectModel
    {
        /// <summary>
        /// Represents a file system item.
        /// </summary>
        public abstract class ShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ArchiveItemInfoProvider<TObjectProperties, ShellObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
        {
            private ShellObject _shellObject;
            private IBrowsableObjectInfo _parent;

            #region Properties
            /// <summary>
            /// Gets a <see cref="ShellObject"/> that represents this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public sealed override ShellObject InnerObjectGeneric => _shellObject;

            public Stream ArchiveFileStream { get; private set; }

            public bool IsArchiveOpen => ArchiveFileStream is object;

            #region Overrides
            // public override FileSystemType ItemFileSystemType => FileSystemType.CurrentDeviceFileSystem;

            public override IBrowsabilityOptions Browsability => _shellObject is System.Collections.Generic.IEnumerable<ShellObject> ? BrowsabilityOptions.BrowsableByDefault : BrowsabilityOptions.NotBrowsable;

#if NETFRAMEWORK
            public override IBrowsableObjectInfo Parent => _parent ?? (_parent = GetParent());
#else
            public override IBrowsableObjectInfo Parent => _parent ??= GetParent();
#endif

            /// <summary>
            /// Gets the localized name of this <see cref="ShellObjectInfo"/> depending the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
            /// </summary>
            public override string LocalizedName => _shellObject.GetDisplayName(DisplayNameType.Default);

            /// <summary>
            /// Gets the name of this <see cref="ShellObjectInfo"/> depending of the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
            /// </summary>
            public override string Name => _shellObject.Name;

            #region BitmapSources
            /// <summary>
            /// Gets the small <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public override BitmapSource SmallBitmapSource => _shellObject.Thumbnail.SmallBitmapSource;

            /// <summary>
            /// Gets the medium <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public override BitmapSource MediumBitmapSource => _shellObject.Thumbnail.MediumBitmapSource;

            /// <summary>
            /// Gets the large <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public override BitmapSource LargeBitmapSource => _shellObject.Thumbnail.LargeBitmapSource;

            /// <summary>
            /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public override BitmapSource ExtraLargeBitmapSource => _shellObject.Thumbnail.ExtraLargeBitmapSource;
            #endregion

            /// <summary>
            /// Gets the type name of the current <see cref="ShellObjectInfo"/>. This value corresponds to the description of the file's extension.
            /// </summary>
            public override string ItemTypeName => _shellObject.Properties.System.ItemTypeText.Value;

            public override string Description => _shellObject.Properties.System.FileDescription.Value;

            /// <summary>
            /// Gets a value that indicates whether the current item has particularities.
            /// </summary>
            public override bool IsSpecialItem
            {
                get
                {
                    uint? value = _shellObject.Properties.System.FileAttributes.Value;

                    if (value.HasValue)
                    {
                        var _value = (Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes)value.Value;

                        return _value.HasFlag(Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes.Hidden) || _value.HasFlag(Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes.System);
                    }

                    else

                        return false;
                }
            }

            /// <summary>
            /// The parent <see cref="IShellObjectInfoBase"/> of the current archive item. If the current <see cref="ShellObjectInfo"/> represents an archive file, this property returns the current <see cref="ShellObjectInfo"/>, or <see langword="null"/> otherwise.
            /// </summary>
            public override IShellObjectInfoBase ArchiveShellObject => ObjectPropertiesGeneric.FileType == FileType.Archive ? this : null;

#if WinCopies3
            public override IPropertySystemCollection ObjectPropertySystem => ShellObjectPropertySystemCollection._GetShellObjectPropertySystemCollection(this);
#endif
            #endregion
            #endregion

            ///// <summary>
            ///// Initializes a new instance of the <see cref="ShellObjectInfo"/> class with a given <see cref="FileType"/> and <see cref="SpecialFolder"/> using custom factories for <see cref="ShellObjectInfo"/>s and <see cref="ArchiveItemInfo"/>s.
            ///// </summary>
            ///// <param name="path">The path of this <see cref="ShellObjectInfo"/>.</param>
            ///// <param name="fileType">The file type of this <see cref="ShellObjectInfo"/>.</param>
            ///// <param name="specialFolder">The special folder type of this <see cref="ShellObjectInfo"/>. <see cref="WinCopies.IO.SpecialFolder.None"/> if this <see cref="ShellObjectInfo"/> is a casual file system item.</param>
            ///// <param name="shellObject">The <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/> that this <see cref="ShellObjectInfo"/> represents.</param>
            protected ShellObjectInfo(in string path, in ShellObject shellObject, in ClientVersion clientVersion) : base(path, clientVersion) => _shellObject = shellObject;

            #region Methods
            #region Archive
            public void OpenArchive(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int? bufferSize, FileOptions fileOptions)
            {
                if (ObjectPropertiesGeneric.FileType == FileType.Archive)

                    ArchiveFileStream = ArchiveFileStream == null
                        ? (Stream)new FileStream(Path, fileMode, fileAccess, fileShare, bufferSize.HasValue ? bufferSize.Value : 4096, fileOptions)
                        : throw new InvalidOperationException("The archive is not open.");

                else

                    throw new InvalidOperationException("The current item is not an archive.");
            }

            public void CloseArchive()
            {
                if (ObjectPropertiesGeneric.FileType == FileType.Archive)
                {
                    if (ArchiveFileStream == null)

                        return;

                    ArchiveFileStream.Dispose();

                    ArchiveFileStream = null;
                }

                else

                    throw new InvalidOperationException("The current item is not an archive.");
            }
            #endregion

            #region Overrides
            protected override void Dispose(in bool disposing)
            {
                base.Dispose(disposing);

                _shellObject.Dispose();

                if (IsArchiveOpen)

                    CloseArchive();

                if (disposing)

                    _shellObject = null;
            }

            /// <summary>
            /// Gets a string representation of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            /// <returns>The <see cref="LocalizedName"/> of this <see cref="ShellObjectInfo"/>.</returns>
            public override string ToString() => string.IsNullOrEmpty(Path) ? _shellObject.GetDisplayName(DisplayNameType.Default) : System.IO.Path.GetFileName(Path);
            #endregion

            /// <summary>
            /// Returns the parent of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            /// <returns>The parent of this <see cref="ShellObjectInfo"/>.</returns>
            private IBrowsableObjectInfo GetParent()
            {
                bool equalsFileType()
                {
                    switch (ObjectPropertiesGeneric.FileType)
                    {
                        case FileType.Folder:
                        case FileType.Archive:

                            return true;

                        case FileType.KnownFolder:

                            return _shellObject.IsFileSystemObject || ((IKnownFolder)_shellObject).FolderId.ToString() != Microsoft.WindowsAPICodePack.Shell.Guids.KnownFolders.Computer;
                    }

                    return false;
                }

                return equalsFileType()
                    ? ShellObjectInfo.From(_shellObject.Parent, ClientVersion)
                    : ObjectPropertiesGeneric.FileType == FileType.Drive
                        ? new ShellObjectInfo(Computer.Path, FileType.KnownFolder, ShellObject.FromParsingName(Computer.ParsingName), ClientVersion)
                        : null;
            }
            #endregion
        }

        public class ShellObjectInfo : ShellObjectInfo<IFileSystemObjectInfoProperties, ShellObjectInfoEnumeratorStruct, IBrowsableObjectInfoSelectorDictionary<ShellObjectInfoItemProvider>, ShellObjectInfoItemProvider>, IShellObjectInfo
        {
            #region Properties
            public static IBrowsableObjectInfoSelectorDictionary<ShellObjectInfoItemProvider> DefaultItemSelectorDictionary { get; } = new ShellObjectInfoSelectorDictionary();

            public override bool IsBrowsableByDefault => true;

            public sealed override IFileSystemObjectInfoProperties ObjectPropertiesGeneric { get; }
            #endregion // Properties

            #region Constructors
            public ShellObjectInfo(in string path, in FileType fileType, in ShellObject shellObject, in ClientVersion clientVersion) : base(path, shellObject, clientVersion)
            {
                switch (fileType)
                {
                    case FileType.Folder:
                    case FileType.KnownFolder:

                        ObjectPropertiesGeneric = new FolderShellObjectInfoProperties<IShellObjectInfoBase2>(this, fileType);

                        break;

                    case FileType.File:
                    case FileType.Archive:
                    case FileType.Library:
                    case FileType.Link:

                        ObjectPropertiesGeneric = new FileShellObjectInfoProperties<IShellObjectInfoBase2>(this, fileType);

                        break;

                    case FileType.Drive:

                        ObjectPropertiesGeneric = new DriveShellObjectInfoProperties<IShellObjectInfoBase2>(this, fileType);

                        break;

                    default:

                        ObjectPropertiesGeneric = new FileSystemObjectInfoProperties(this, fileType);

                        break;
                }
            }

            public ShellObjectInfo(in IKnownFolder knownFolder, in ClientVersion clientVersion) : this(knownFolder.Path, FileType.KnownFolder, ShellObject.FromParsingName(knownFolder.ParsingName), clientVersion)
            {
                // Left empty.
            }
            #endregion

            #region Methods
            public static ShellObjectInitInfo GetInitInfo(in ShellObject shellObject)
            {
                if ((shellObject ?? throw GetArgumentNullException(nameof(shellObject))) is ShellFolder shellFolder)
                {
                    if (shellObject is ShellFileSystemFolder shellFileSystemFolder)
                    {
                        (string path, FileType fileType) = shellFileSystemFolder is FileSystemKnownFolder ? (shellObject.ParsingName, FileType.KnownFolder) : (shellFileSystemFolder.Path, FileType.Folder);

                        return new ShellObjectInitInfo(path, System.IO.Directory.GetParent(path) is null ? FileType.Drive : fileType);
                    }

                    switch (shellObject)
                    {
                        case NonFileSystemKnownFolder nonFileSystemKnownFolder:

                            return new ShellObjectInitInfo(nonFileSystemKnownFolder.Path, FileType.KnownFolder);

                        case ShellNonFileSystemFolder _:

                            return new ShellObjectInitInfo(shellObject.ParsingName, FileType.Folder);
                    }
                }

                if (shellObject is ShellLink shellLink)

                    return new ShellObjectInitInfo(shellLink.Path, FileType.Link);

                if (shellObject is ShellFile shellFile)

                    return new ShellObjectInitInfo(shellFile.Path, IsSupportedArchiveFormat(System.IO.Path.GetExtension(shellFile.Path)) ? FileType.Archive : shellFile.IsLink ? FileType.Link : System.IO.Path.GetExtension(shellFile.Path) == ".library-ms" ? FileType.Library : FileType.File);

                throw new ArgumentException($"The given {nameof(ShellObject)} is not supported.");
            }

            public static ShellObjectInfo From(in ShellObject shellObject, in ClientVersion clientVersion)
            {
                ShellObjectInitInfo initInfo = GetInitInfo(shellObject);

                return new ShellObjectInfo(initInfo.Path, initInfo.FileType, shellObject, clientVersion);
            }

            public override IBrowsableObjectInfoSelectorDictionary<ShellObjectInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

            #region GetItems
            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ArchiveFileInfoEnumeratorStruct> func) => func == null ? GetItems(GetItemProviders(item => true)) : GetItems(GetItemProviders(func));

            protected override System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetItemProviders(Predicate<ShellObjectInfoEnumeratorStruct> predicate) => IsBrowsable
                ? ObjectPropertiesGeneric.FileType == FileType.Archive
                    ? GetItemProviders(item => predicate(new ShellObjectInfoEnumeratorStruct(item)))
                    : ShellObjectInfoEnumeration.From(this, ClientVersion, predicate)
                : GetEmptyEnumerable();

            protected virtual System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetItemProviders(Predicate<ArchiveFileInfoEnumeratorStruct> func)
#if NETFRAMEWORK
            {
                switch (ObjectPropertiesGeneric.FileType)
                {
                    case FileType.Archive:
                        return GetArchiveItemInfoItems(func);
                    default:
                        return null;
                }
            }
#else
             => ObjectPropertiesGeneric.FileType switch
             {
                 FileType.Archive => ArchiveItemInfo.GetArchiveItemInfoItems(this, func).Select(ShellObjectInfoItemProvider.ToShellObjectInfoItemProvider),
                 _ => GetEmptyEnumerable()
             };
#endif

            protected override System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetItemProviders() => GetItemProviders((Predicate<ShellObjectInfoEnumeratorStruct>)(obj => true));
            #endregion // GetItems
            #endregion // Methods
        }
    }
}
