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

using Microsoft.WindowsAPICodePack.Shell;

using System.IO;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Enumeration;
using WinCopies.IO.PropertySystem;

namespace WinCopies.IO.ObjectModel
{
    public interface IShellObjectInfoBase : IArchiveItemInfoProvider
    {
        bool IsArchiveOpen { get; }

        void OpenArchive(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int? bufferSize, FileOptions fileOptions);

        StreamInfo
#if CS8
            ?
#endif
            GetArchiveFileStream();

        void CloseArchive();
    }

    public interface IShellObjectInfoBase2 : IShellObjectInfoBase, IEncapsulatorBrowsableObjectInfo<ShellObject>
    {
        // Left empty.
    }

    public interface IShellObjectInfo<out TObjectProperties> : IShellObjectInfoBase2, IBrowsableObjectInfo<TObjectProperties> where TObjectProperties : IFileSystemObjectInfoProperties
    {
        // Left empty.
    }

    public interface IShellObjectInfo<out TObjectProperties,  TPredicateTypeParameter, out TSelectorDictionary, out TDictionaryItems> : IShellObjectInfo<TObjectProperties>, IArchiveItemInfoProvider<TObjectProperties, ShellObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        /*/// <summary>
        /// Gets a <see cref="FileSystemInfo"/> object that provides info for the folders and files. This property returns <see langword="null"/> when this <see cref="IShellObjectInfo"/> is not a folder, drive or file. See the <see cref="IFileSystemObjectInfo.FileType"/> property for more details.
        /// </summary>
        FileSystemInfo FileSystemInfoProperties { get; }

        /// <summary>
        /// Gets a <see cref="DriveInfo"/> object that provides info for drives. This property returns <see langword="null"/> when this <see cref="IShellObjectInfo"/> is not a drive. See the <see cref="IFileSystemObjectInfo.FileType"/> property for more details.
        /// </summary>
        DriveInfo DriveInfoProperties { get; }

        /// <summary>
        /// Gets a <see cref="IKnownFolder"/> object that provides info for the system known folders. This property returns <see langword="null"/> when this <see cref="IShellObjectInfo"/> is not a system known folder. See the <see cref="IFileSystemObjectInfo.FileType"/> property for more details.
        /// </summary>
        IKnownFolder KnownFolderInfo { get; }

        private FileStream _archiveFileStream = null;

        /// <summary>
        /// The <see cref="FileStream"/> for this <see cref="IShellObjectInfo"/> when it represents an archive file system item. See the remarks section.
        /// </summary>
        /// <remarks>
        /// This field is only used by the <see cref="IShellObjectInfo"/>, <see cref="FolderLoader"/> and the <see cref="ArchiveLoader{TPath, TItems, TFactory}"/> classes in order to lock the file that the <see cref="IShellObjectInfo"/> represents when the items of the archive are loaded.
        /// </remarks>
        public FileStream ArchiveFileStream { get => _archiveFileStream; internal set => OnPropertyChanged(nameof(ArchiveFileStream), nameof(_archiveFileStream), value, typeof(IShellObjectInfo)); }

        /// <summary>
        /// Gets the special folder type of this <see cref="IShellObjectInfo"/>. <see cref="SpecialFolder.None"/> if this <see cref="IShellObjectInfo"/> is a casual file system item.
        /// </summary>
        SpecialFolder SpecialFolder { get; }*/
    }

    public interface IShellObjectInfo : IShellObjectInfo<IFileSystemObjectInfoProperties, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>
    {
        // Left empty.
    }
}
