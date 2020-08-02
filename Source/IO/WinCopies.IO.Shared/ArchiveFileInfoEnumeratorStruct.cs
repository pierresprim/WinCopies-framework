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

using SevenZip;

using static WinCopies.Util.Util;

namespace WinCopies.IO
{
    /// <summary>
    /// Represents an archive item. This struct is used in enumeration methods.
    /// </summary>
    public struct ArchiveFileInfoEnumeratorStruct
    {
        /// <summary>
        /// Gets the path of the current archive item. This property is set only when <see cref="ArchiveFileInfo"/> is <see langword="null"/>.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the <see cref="SevenZip.ArchiveFileInfo"/> that represents the cyrrent archive item. This property is set only when <see cref="Path"/> is <see langword="null"/>.
        /// </summary>
        public ArchiveFileInfo? ArchiveFileInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFileInfoEnumeratorStruct"/> struct with the given path.
        /// </summary>
        /// <param name="path">The path of the archive item.</param>
        public ArchiveFileInfoEnumeratorStruct(string path)
        {
            Path = path ?? throw GetArgumentNullException(nameof(path));

            ArchiveFileInfo = null;
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="ArchiveFileInfoEnumeratorStruct"/> struct with the given <see cref="SevenZip.ArchiveFileInfo"/>.
        ///// </summary>
        ///// <param name="path">The <see cref="SevenZip.ArchiveFileInfo"/> that represents the archive item.</param>
        public ArchiveFileInfoEnumeratorStruct(ArchiveFileInfo archiveFileInfo)
        {
            Path = null;

            ArchiveFileInfo = archiveFileInfo;
        }
    }
}
