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
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

using WinCopies.Collections;
using WinCopies.Util;

using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.IO.Path;
using static WinCopies.Util.Util;

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
        public class ShellObjectInfo : ArchiveItemInfoProvider<IFileSystemObjectInfoProperties, ShellObject>, IShellObjectInfo<IFileSystemObjectInfoProperties>
        {
            private ShellObject _shellObject;
            private IBrowsableObjectInfo _parent;

            #region Properties
            public override FileType FileType { get; }

            /// <summary>
            /// Gets a <see cref="ShellObject"/> that represents this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public sealed override ShellObject EncapsulatedObject => _shellObject;

            public sealed override IFileSystemObjectInfoProperties ObjectPropertiesGeneric { get; }

            public Stream ArchiveFileStream { get; private set; }

            public bool IsArchiveOpen => ArchiveFileStream is object;

            #region Overrides
            public override FileSystemType ItemFileSystemType => FileSystemType.CurrentDeviceFileSystem;

            /// <summary>
            /// Gets a value that indicates whether this <see cref="ShellObjectInfo"/> is browsable.
            /// </summary>
            public override bool IsBrowsable => _shellObject is IEnumerable<ShellObject>;

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

            public override Size? Size
            {
                get
                {
                    ulong? value = _shellObject.Properties.System.Size.Value;

                    if (value.HasValue)

                        return new Size(value.Value);

                    else

                        return null;
                }
            }

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
            /// The parent <see cref="IShellObjectInfo"/> of the current archive item. If the current <see cref="ShellObjectInfo"/> represents an archive file, this property returns the current <see cref="ShellObjectInfo"/>, or <see langword="null"/> otherwise.
            /// </summary>
            public override IShellObjectInfo ArchiveShellObject => ObjectPropertiesGeneric.FileType == FileType.Archive ? this : null;
            #endregion
            #endregion

            ///// <summary>
            ///// Initializes a new instance of the <see cref="ShellObjectInfo"/> class with a given <see cref="FileType"/> and <see cref="SpecialFolder"/> using custom factories for <see cref="ShellObjectInfo"/>s and <see cref="ArchiveItemInfo"/>s.
            ///// </summary>
            ///// <param name="path">The path of this <see cref="ShellObjectInfo"/>.</param>
            ///// <param name="fileType">The file type of this <see cref="ShellObjectInfo"/>.</param>
            ///// <param name="specialFolder">The special folder type of this <see cref="ShellObjectInfo"/>. <see cref="WinCopies.IO.SpecialFolder.None"/> if this <see cref="ShellObjectInfo"/> is a casual file system item.</param>
            ///// <param name="shellObject">The <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/> that this <see cref="ShellObjectInfo"/> represents.</param>
            protected ShellObjectInfo(in string path, in FileType fileType, in ShellObject shellObject, in ClientVersion? clientVersion) : base(path, clientVersion)
            {
                _shellObject = shellObject;

                FileType = fileType;

                ObjectPropertiesGeneric = new FileSystemObjectInfoProperties<IShellObjectInfo>(this);
            }

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

            #region Archive
            public void OpenArchive(Stream stream)
            {
                CloseArchive();

                ObjectPropertiesGeneric.FileType.ThrowIfInvalidEnumValue(true, FileType.Archive);

                ArchiveFileStream = stream;
            }

            public void CloseArchive()
            {
                ArchiveFileStream.Flush();

                ArchiveFileStream.Dispose();

                ArchiveFileStream = null;
            }
            #endregion

            #region GetItems
            public virtual IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ShellObjectInfoEnumeratorStruct> func)
            {
                ThrowIfNull(func, nameof(func));

                if (IsBrowsable)

                    if (ObjectPropertiesGeneric.FileType == FileType.Archive)

                        return GetItems(item => func(new ShellObjectInfoEnumeratorStruct(item)));

                    else

                        return new Enumerable<IBrowsableObjectInfo>(() => ShellObjectInfoEnumerator.From(this, func, ClientVersion.Value));

                else return null;
            }

            public virtual IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ArchiveFileInfoEnumeratorStruct> func)
            {
                ThrowIfNull(func, nameof(func));

#if NETFRAMEWORK
            switch (ObjectPropertiesGeneric.FileType)
            {
                case FileType.Archive:
                    return GetArchiveItemInfoItems(func);
                default:
                    return null;
            }
#else
                return ObjectPropertiesGeneric.FileType switch
                {
                    FileType.Archive => GetArchiveItemInfoItems(func),
                    _ => null,
                };
#endif
            }

            public override IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems((Predicate<ShellObjectInfoEnumeratorStruct>)(obj => true));

            private IEnumerable<IBrowsableObjectInfo> GetArchiveItemInfoItems(Predicate<ArchiveFileInfoEnumeratorStruct> func) => new Enumerable<IBrowsableObjectInfo>(() => new ArchiveItemInfoEnumerator(this, func));
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

                if (equalsFileType())

                    return From(_shellObject.Parent, ClientVersion.Value);

                else if (ObjectPropertiesGeneric.FileType == FileType.Drive)

                    return new ShellObjectInfo(Computer.Path, FileType.KnownFolder, ShellObject.FromParsingName(Computer.ParsingName), ClientVersion.Value);

                else return null;
            }
            #endregion
        }
    }
}
