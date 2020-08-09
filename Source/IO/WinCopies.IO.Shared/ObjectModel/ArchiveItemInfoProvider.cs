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

using Microsoft.WindowsAPICodePack.PortableDevices;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// The base class for <see cref="ArchiveItemInfoProvider"/> objects.
    /// </summary>
    public abstract class ArchiveItemInfoProvider : FileSystemObjectInfo, IArchiveItemInfoProvider
    {
        /// <summary>
        /// The <see cref="IO.FileType"/> of this item.
        /// </summary>
        public override FileType FileType { get; }

        /// <summary>
        /// The parent <see cref="IShellObjectInfo"/> of this item. For <see cref="ShellObjectInfo"/> items, this property returns the current object.
        /// </summary>
        public abstract IShellObjectInfo ArchiveShellObject { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveItemInfoProvider"/> class.
        /// </summary>
        /// <param name="path">The path of the new item.</param>
        /// <param name="fileType">The <see cref="IO.FileType"/> of the new item.</param>
        protected ArchiveItemInfoProvider(in string path, in FileType fileType) : this(path, fileType, null)
        {
            // Left empty.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveItemInfoProvider"/> class.
        /// </summary>
        /// <param name="path">The path of this <see cref="ArchiveItemInfoProvider"/>.</param>
        /// <param name="fileType">The <see cref="IO.FileType"/> of this <see cref="ArchiveItemInfoProvider"/>.</param>
        /// <param name="clientVersion">The <see cref="ClientVersion"/> that will be used for <see cref="PortableDeviceInfo"/> and <see cref="PortableDeviceObjectInfo"/> initialization.</param>
        protected ArchiveItemInfoProvider(in string path, in FileType fileType, ClientVersion? clientVersion) : base(path, clientVersion) => FileType = fileType;
    }
}
