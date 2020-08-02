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
using static WinCopies.Util.Util;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// The base class for all file system objects in the WinCopies framework. This class can represent virtual file system objects (for example registry items, WMI items, ...).
    /// </summary>
    public abstract class FileSystemObject : IFileSystemObject
    {
        /// <summary>
        /// Gets the path of this <see cref="FileSystemObject"/>.
        /// </summary>
        public virtual string Path { get; }

        /// <summary>
        /// When overridden in a derived class, gets the localized path of this <see cref="FileSystemObject"/>.
        /// </summary>
        public abstract string LocalizedName { get; }

        /// <summary>
        /// When overridden in a derived class, gets the name of this <see cref="FileSystemObject"/>.
        /// </summary>
        public abstract string Name { get; }

        public abstract FileSystemType ItemFileSystemType { get; }

        /// <summary>
        /// When called from a derived class, initializes a new instance of the <see cref="FileSystemObject"/> class.
        /// </summary>
        /// <param name="path">The path of this <see cref="FileSystemObject"/>.</param>
        protected FileSystemObject(string path) => Path = path;

        public virtual WinCopies.Collections.IEqualityComparer<IFileSystemObject> GetDefaultEqualityComparer() => new FileSystemObjectEqualityComparer<IFileSystemObject>();

        ///// <summary>
        ///// Determines whether the specified <see cref="IFileSystemObject"/> is equal to the current object by calling the <see cref="Equals(object)"/> method.
        ///// </summary>
        ///// <param name="fileSystemObject">The <see cref="IFileSystemObject"/> to compare with the current object.</param>
        ///// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public virtual bool Equals(IFileSystemObject fileSystemObject) => GetDefaultEqualityComparer().Equals(this, fileSystemObject);

        ///// <summary>
        ///// Determines whether the specified object is equal to the current object by testing the following things, in order: whether both objects's references are equal, <paramref name="obj"/> implements the <see cref="IFileSystemObject"/> interface and <see cref="Path"/> properties are equal.
        ///// </summary>
        ///// <param name="obj">The object to compare with the current object.</param>
        ///// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => GetDefaultEqualityComparer().Equals(this, obj);

        /// <summary>
        /// Gets an hash code for this <see cref="FileSystemObject"/>.
        /// </summary>
        /// <returns>The hash code of the <see cref="Path"/> property.</returns>
        public override int GetHashCode() => GetDefaultEqualityComparer().GetHashCode(this);

        /// <summary>
        /// Gets a string representation of this <see cref="FileSystemObject"/>.
        /// </summary>
        /// <returns>The <see cref="LocalizedName"/> of this <see cref="FileSystemObject"/>.</returns>
        public override string ToString() => IsNullEmptyOrWhiteSpace(LocalizedName) ? Path : LocalizedName;

        #region Operators
        /// <summary>
        /// Checks if two <see cref="FileSystemObject"/>s are equal.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the two <see cref="FileSystemObject"/>s are equal.</returns>
        public static bool operator ==(FileSystemObject left, FileSystemObject right) => left is null ? right is null : left.Equals(right);

        /// <summary>
        /// Checks if two <see cref="FileSystemObject"/>s are different.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the two <see cref="FileSystemObject"/>s are different.</returns>
        public static bool operator !=(FileSystemObject left, FileSystemObject right) => !(left == right);

        /// <summary>
        /// Checks if a given <see cref="FileSystemObject"/> is lesser than an other <see cref="FileSystemObject"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObject"/> is lesser than the <see cref="FileSystemObject"/> to compare with.</returns>
        public static bool operator <(FileSystemObject left, FileSystemObject right) => left is null ? right is object : left.CompareTo(right) < 0;

        /// <summary>
        /// Checks if a given <see cref="FileSystemObject"/> is lesser or equal to an other <see cref="FileSystemObject"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObject"/> is lesser or equal to the <see cref="FileSystemObject"/> to compare with.</returns>
        public static bool operator <=(FileSystemObject left, FileSystemObject right) => left is null || left.CompareTo(right) <= 0;

        /// <summary>
        /// Checks if a given <see cref="FileSystemObject"/> is greater than an other <see cref="FileSystemObject"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObject"/> is greater than the <see cref="FileSystemObject"/> to compare with.</returns>
        public static bool operator >(FileSystemObject left, FileSystemObject right) => left is object && left.CompareTo(right) > 0;

        /// <summary>
        /// Checks if a given <see cref="FileSystemObject"/> is greater or equal to an other <see cref="FileSystemObject"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObject"/> is greater or equal to the <see cref="FileSystemObject"/> to compare with.</returns>
        public static bool operator >=(FileSystemObject left, FileSystemObject right) => left is null ? right is null : left.CompareTo(right) >= 0;
        #endregion

        public virtual IComparer<IFileSystemObject> GetDefaultComparer() => new FileSystemObjectComparer<IFileSystemObject>();

        /// <summary>
        /// Compares the current object to a given <see cref="FileSystemObject"/>.
        /// </summary>
        /// <param name="fileSystemObject">The <see cref="FileSystemObject"/> to compare with.</param>
        /// <returns>The comparison result. See <see cref="IComparable{T}.CompareTo(T)"/> for more details.</returns>
        public virtual int CompareTo(IFileSystemObject fileSystemObject) => GetDefaultComparer().Compare(this, fileSystemObject);
    }
}
