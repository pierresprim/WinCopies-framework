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

using System;

namespace WinCopies.IO
{
    /// <summary>
    /// The file type of an I/O object.
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// Undefined file type.
        /// </summary>
        Other,

        /// <summary>
        /// The item is a folder.
        /// </summary>
        Folder,

        /// <summary>
        /// The item is a file.
        /// </summary>
        File,

        /// <summary>
        /// The item is a drive.
        /// </summary>
        Drive,

        /// <summary>
        /// The item is a link.
        /// </summary>
        Link,

        /// <summary>
        /// The item is an archive.
        /// </summary>
        Archive,

        /// <summary>
        /// The item is a special folder (system folder or virtual folder).
        /// </summary>
        SpecialFolder
    }

    /// <summary>
    /// File types to load in the <see cref="FolderLoader"/> and <see cref="ArchiveLoader{TPath, TItems, TFactory}"/> classes.
    /// </summary>
    [Flags]
    public enum FileTypes
    {
        /// <summary>
        /// Do not load any item type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Load folders.
        /// </summary>
        Folder = 1,

        /// <summary>
        /// Load files.
        /// </summary>
        File = 2,

        /// <summary>
        /// Load drives.
        /// </summary>
        Drive = 4,

        /// <summary>
        /// Load links.
        /// </summary>
        Link = 8,

        /// <summary>
        /// Load archives.
        /// </summary>
        Archive = 16
    }
}
