/* Copyright © Pierre Sprimont, 2019
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

using SevenZip;
using System;
using WinCopies.Util;

namespace WinCopies.IO
{

    /// <summary>
    /// A factory to create new <see cref="ArchiveItemInfo"/>s.
    /// </summary>
    public interface IArchiveItemInfoFactory : IBrowsableObjectInfoFactory
    {

        IBrowsableObjectInfo GetBrowsableObjectInfo(string path, FileType fileType, IShellObjectInfo archiveShellObject, ArchiveFileInfo? archiveFileInfo, DeepClone<ArchiveFileInfo?> archiveFileInfoDelegate);

    }

    /// <summary>
    /// A factory for creating new <see cref="ShellObjectInfo"/>s.
    /// </summary>
    public class ArchiveItemInfoFactory : BrowsableObjectInfoFactory, IArchiveItemInfoFactory
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveItemInfoFactory"/> class.
        /// </summary>
        public ArchiveItemInfoFactory() : base() { }

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(string path, FileType fileType, IShellObjectInfo archiveShellObject, ArchiveFileInfo? archiveFileInfo, DeepClone<ArchiveFileInfo?> archiveFileInfoDelegate) => new ArchiveItemInfo(path, fileType, archiveShellObject, archiveFileInfo, archiveFileInfoDelegate);

        protected override BrowsableObjectInfoFactory DeepCloneOverride() => new ArchiveItemInfoFactory();

    }

}
