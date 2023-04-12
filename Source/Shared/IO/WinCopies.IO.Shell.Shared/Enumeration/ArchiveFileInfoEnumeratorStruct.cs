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

using static WinCopies.
#if !WinCopies3
    Util.Util
#else
    ThrowHelper
#endif
    ;

namespace WinCopies.IO.Enumeration
{
    public struct ArchiveFileInfoEnumeratorStruct3
    {
        /// <summary>
        /// Gets the <see cref="SevenZip.ArchiveFileInfo"/> that represents the cyrrent archive item. This property is set only when <see cref="Path"/> is <see langword="null"/>.
        /// </summary>
        public ArchiveFileInfo ArchiveFileInfo { get; }

        public string ArchiveFileName { get; }

        public ArchiveFileInfoEnumeratorStruct3(in ArchiveFileInfo archiveFileInfo, in string archiveFileName)
        {
            ArchiveFileInfo = archiveFileInfo;

            ArchiveFileName = archiveFileName;
        }
    }

    public struct ArchiveFileInfoEnumeratorStruct2
    {
        ///// <summary>
        ///// Gets the relative path of the current archive item. This property is set only when <see cref="ArchiveFileInfo"/> is <see langword="null"/>. This property can be <see langword="null"/> if the relative path of the current item is the archive root path.
        ///// </summary>
        public string RelativePath { get; }

        public string Name { get; }

        public string Path => Name == null ? null : RelativePath == null ? Name : $"{RelativePath}{System.IO.Path.DirectorySeparatorChar}{Name}";

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFileInfoEnumeratorStruct"/> struct with a given path.
        /// </summary>
        /// <param name="relativePath">The relative path of the archive item.</param>
        public ArchiveFileInfoEnumeratorStruct2(in string relativePath, in string name)
        {
            RelativePath = relativePath.Length > 0 ? relativePath : null;

            Name = name ?? throw GetArgumentNullException(nameof(relativePath));
        }
    }

    /// <summary>
    /// Represents an archive item. This struct is used in enumeration methods.
    /// </summary>
    public class ArchiveFileInfoEnumeratorStruct
    {
        public ArchiveFileInfoEnumeratorStruct3? ArchiveFileInfo { get; }

        public ArchiveFileInfoEnumeratorStruct2? RelativePath { get; }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="ArchiveFileInfoEnumeratorStruct"/> struct with the given <see cref="SevenZip.ArchiveFileInfo"/>.
        ///// </summary>
        ///// <param name="path">The <see cref="SevenZip.ArchiveFileInfo"/> that represents the archive item.</param>
        public ArchiveFileInfoEnumeratorStruct(in ArchiveFileInfoEnumeratorStruct3 archiveFileInfo) => ArchiveFileInfo = archiveFileInfo;

        public ArchiveFileInfoEnumeratorStruct(in ArchiveFileInfoEnumeratorStruct2 relativePath) => RelativePath = relativePath;

        public string GetFileName() => RelativePath.HasValue ? RelativePath.Value.Name : ArchiveFileInfo.Value.ArchiveFileName;
    }
}
