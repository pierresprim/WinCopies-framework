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

using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.PropertySystem
{
    public class ArchiveItemInfoProperties<T> : FileSystemObjectInfoPropertiesAbstract<T>, IFileSystemObjectInfoProperties2 where T : IArchiveItemInfo
    {
        public DateTime CreationTime => BrowsableObjectInfo.EncapsulatedObject.Value.CreationTime;

        public DateTime LastAccessTime => BrowsableObjectInfo.EncapsulatedObject.Value.LastAccessTime;

        public DateTime LastWriteTime => BrowsableObjectInfo.EncapsulatedObject.Value.LastWriteTime;

        public System.IO.FileAttributes Attributes => (System.IO.FileAttributes)BrowsableObjectInfo.EncapsulatedObject.Value.Attributes;

        public sealed override Size? Size => new IO.Size(BrowsableObjectInfo.EncapsulatedObject.Value.Size);

        public ArchiveItemInfoProperties(in T fileSystemObjectInfo, in FileType fileType) : base(fileSystemObjectInfo, fileType)
        {
            // Left empty.
        }
    }
}
