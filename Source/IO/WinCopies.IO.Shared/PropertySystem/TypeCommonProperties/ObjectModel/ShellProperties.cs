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
using System.IO;

using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.PropertySystem
{
    public abstract class FileSystemObjectInfoProperties<T> : BrowsableObjectInfoProperties<T>, IFileSystemObjectInfoProperties where T : IFileSystemObjectInfo
    {
        ///// <summary>
        ///// The file type of this <see cref="FileSystemObject"/>.
        ///// </summary>
        public FileType FileType { get; }

        public abstract Size? Size { get; }

        protected FileSystemObjectInfoProperties(in T fileSystemObjectInfo, in FileType fileType) : base(fileSystemObjectInfo) => FileType = fileType;
    }

    public class FileSystemObjectInfoProperties : FileSystemObjectInfoProperties<IFileSystemObjectInfo>
    {
        public override Size? Size => null;

        public FileSystemObjectInfoProperties(in IFileSystemObjectInfo fileSystemObjectInfo, in FileType fileType) : base(fileSystemObjectInfo, fileType)
        {
            // Left empty.
        }
    }

    public class FileSystemEntryInfoProperties : FileSystemObjectInfoProperties<IFileSystemObjectInfo>
    {
        public override Size? Size { get; }

        public FileSystemEntryInfoProperties(in IFileSystemObjectInfo fileSystemObjectInfo, in FileType fileType, in Size size) : base(fileSystemObjectInfo, fileType) => Size = size;
    }

    public abstract class FileSystemObjectInfoProperties<TBrowsableObjectInfo, TInnerProperties> : FileSystemObjectInfoProperties<TBrowsableObjectInfo> where TBrowsableObjectInfo : IFileSystemObjectInfo
    {
        protected TInnerProperties InnerProperties { get; }

        protected FileSystemObjectInfoProperties(in TBrowsableObjectInfo fileSystemObjectInfo, in FileType fileType, in TInnerProperties innerProperties) : base(fileSystemObjectInfo, fileType) => InnerProperties = innerProperties;
    }

    public abstract class FileOrFolderShellObjectInfoProperties<TBrowsableObjectInfo, TInnerProperties> : FileSystemObjectInfoProperties<TBrowsableObjectInfo, TInnerProperties>, IFileSystemObjectInfoProperties2 where TBrowsableObjectInfo : IShellObjectInfoBase2 where TInnerProperties : FileSystemInfo
    {
        public DateTime LastWriteTime => InnerProperties.LastWriteTime;

        public DateTime LastAccessTimeUtc => InnerProperties.LastAccessTimeUtc;

        public DateTime LastAccessTime => InnerProperties.LastAccessTime;

        public string FullName => InnerProperties.FullName;

        public string Extension => InnerProperties.Extension;

        public bool Exists => InnerProperties.Exists;

        public DateTime CreationTime => InnerProperties.CreationTime;

        public DateTime LastWriteTimeUtc => InnerProperties.LastWriteTimeUtc;

        public System.IO.FileAttributes Attributes => InnerProperties.Attributes;

        public DateTime CreationTimeUtc => InnerProperties.CreationTimeUtc;

        protected FileOrFolderShellObjectInfoProperties(in TBrowsableObjectInfo shellObjectInfo, in FileType fileType, in TInnerProperties innerProperties) : base(shellObjectInfo, fileType, innerProperties)
        {
            // Left empty.
        }
    }

    public class FolderShellObjectInfoProperties<T> : FileOrFolderShellObjectInfoProperties<T, DirectoryInfo> where T : IShellObjectInfoBase2
    {
        public string ParentName => InnerProperties.Parent.FullName;

        public string RootName => InnerProperties.Root.FullName;

        public sealed override Size? Size => null;

        public FolderShellObjectInfoProperties(in T shellObjectInfo, in FileType fileType) : base(shellObjectInfo, fileType, new DirectoryInfo(shellObjectInfo.Path))
        {
            // Left empty.
        }
    }

    public class FileShellObjectInfoProperties<T> : FileOrFolderShellObjectInfoProperties<T, System.IO.FileInfo> where T : IShellObjectInfoBase2
    {
        public bool IsReadOnly => InnerProperties.IsReadOnly;

        public string? DirectoryName => InnerProperties.DirectoryName;

        public sealed override Size? Size => new Size(BrowsableObjectInfo.InnerObject.Properties.System.Size.Value.Value);

        public FileShellObjectInfoProperties(in T shellObjectInfo, in FileType fileType) : base(shellObjectInfo, fileType, new FileInfo(shellObjectInfo.Path))
        {
            // Left empty.
        }
    }

    public class DriveShellObjectInfoProperties<T> : FileSystemObjectInfoProperties<T, DriveInfo> where T : IShellObjectInfoBase2
    {
        public Size AvailableFreeSpace => new IO.Size((ulong)InnerProperties.AvailableFreeSpace);

        public string DriveFormat => InnerProperties.DriveFormat;

        public DriveType DriveType => InnerProperties.DriveType;

        public bool IsReady => InnerProperties.IsReady;

        public string RootDirectoryName => InnerProperties.RootDirectory.FullName;

        public Size TotalFreeSpace => new IO.Size((ulong)InnerProperties.TotalFreeSpace);

        public Size TotalSize => new Size((ulong)InnerProperties.TotalSize);

        public string VolumeLabel => InnerProperties.VolumeLabel;

        public sealed override Size? Size => new IO.Size((ulong)(InnerProperties.TotalSize - InnerProperties.TotalFreeSpace));

        public DriveShellObjectInfoProperties(in T shellObjectInfo, in FileType fileType) : base(shellObjectInfo, fileType, new DriveInfo(shellObjectInfo.Path))
        {
            // Left empty.
        }
    }
}
