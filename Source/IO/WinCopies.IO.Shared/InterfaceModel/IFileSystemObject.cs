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

using System;
using System.Collections.Generic;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// Provides data about file system items.
    /// </summary>
    public interface IFileSystemObject : IComparable<IFileSystemObject>, IEquatable<IFileSystemObject>
    {
        /// <summary>
        /// Gets the path of this <see cref="IFileSystemObject"/>.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the localized name of this <see cref="IFileSystemObject"/>.
        /// </summary>
        string LocalizedName { get; }

        /// <summary>
        /// Gets the name of this <see cref="IFileSystemObject"/>.
        /// </summary>
        string Name { get; }

        FileSystemType ItemFileSystemType { get; }

        WinCopies.Collections.IEqualityComparer<IFileSystemObject> GetDefaultEqualityComparer();

        IComparer<IFileSystemObject> GetDefaultComparer();
    }
}
