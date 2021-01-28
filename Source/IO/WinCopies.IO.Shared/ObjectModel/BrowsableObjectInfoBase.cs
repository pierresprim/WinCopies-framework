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

using static WinCopies.
    #if !WinCopies3
    Util.Util
#else
    UtilHelpers
    #endif
    ;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// The base class for all file system objects in the WinCopies framework. This class can represent virtual file system objects (for example registry items, WMI items, ...).
    /// </summary>
    public abstract class BrowsableObjectInfoBase : IBrowsableObjectInfoBase
    {
#region Properties
        /// <summary>
        /// Gets the path of this <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// When overridden in a derived class, gets the localized name of this <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        public abstract string LocalizedName { get; }

        /// <summary>
        /// When overridden in a derived class, gets the name of this <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        public abstract string Name { get; }
#endregion

        /// <summary>
        /// When called from a derived class, initializes a new instance of the <see cref="BrowsableObjectInfoBase"/> class.
        /// </summary>
        /// <param name="path">The path of this <see cref="BrowsableObjectInfoBase"/>.</param>
        protected BrowsableObjectInfoBase(string path) => Path = path;

#region Methods
        public virtual Collections.IEqualityComparer<IBrowsableObjectInfoBase> GetDefaultEqualityComparer() => new FileSystemObjectEqualityComparer<IBrowsableObjectInfoBase>();

        ///// <summary>
        ///// Determines whether the specified <see cref="IFileSystemObject"/> is equal to the current object by calling the <see cref="Equals(object)"/> method.
        ///// </summary>
        ///// <param name="fileSystemObject">The <see cref="IFileSystemObject"/> to compare with the current object.</param>
        ///// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public virtual bool Equals(IBrowsableObjectInfoBase fileSystemObject) => GetDefaultEqualityComparer().Equals(this, fileSystemObject);

        ///// <summary>
        ///// Determines whether the specified object is equal to the current object by testing the following things, in order: whether both objects's references are equal, <paramref name="obj"/> implements the <see cref="IFileSystemObject"/> interface and <see cref="Path"/> properties are equal.
        ///// </summary>
        ///// <param name="obj">The object to compare with the current object.</param>
        ///// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => GetDefaultEqualityComparer().Equals(this, obj);

        /// <summary>
        /// Gets an hash code for this <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        /// <returns>The hash code of the <see cref="Path"/> property.</returns>
        public override int GetHashCode() => GetDefaultEqualityComparer().GetHashCode(this);

        /// <summary>
        /// Gets a string representation of this <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        /// <returns>The <see cref="LocalizedName"/> of this <see cref="BrowsableObjectInfoBase"/>.</returns>
        public override string ToString() => IsNullEmptyOrWhiteSpace(LocalizedName) ? Path : LocalizedName;

        public virtual IComparer<IBrowsableObjectInfoBase> GetDefaultComparer() => new FileSystemObjectComparer<IBrowsableObjectInfoBase>();

        /// <summary>
        /// Compares the current object to a given <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        /// <param name="fileSystemObject">The <see cref="BrowsableObjectInfoBase"/> to compare with.</param>
        /// <returns>The comparison result. See <see cref="IComparable{T}.CompareTo(T)"/> for more details.</returns>
        public virtual int CompareTo(IBrowsableObjectInfoBase fileSystemObject) => GetDefaultComparer().Compare(this, fileSystemObject);
#endregion

#region Operators
        /// <summary>
        /// Checks if two <see cref="BrowsableObjectInfoBase"/>s are equal.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the two <see cref="BrowsableObjectInfoBase"/>s are equal.</returns>
        public static bool operator ==(BrowsableObjectInfoBase left, BrowsableObjectInfoBase right) => left is null ? right is null : left.Equals(right);

        /// <summary>
        /// Checks if two <see cref="BrowsableObjectInfoBase"/>s are different.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the two <see cref="BrowsableObjectInfoBase"/>s are different.</returns>
        public static bool operator !=(BrowsableObjectInfoBase left, BrowsableObjectInfoBase right) => !(left == right);

        /// <summary>
        /// Checks if a given <see cref="BrowsableObjectInfoBase"/> is lesser than an other <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="BrowsableObjectInfoBase"/> is lesser than the <see cref="BrowsableObjectInfoBase"/> to compare with.</returns>
        public static bool operator <(BrowsableObjectInfoBase left, BrowsableObjectInfoBase right) => left is null ? right is object : left.CompareTo(right) < 0;

        /// <summary>
        /// Checks if a given <see cref="BrowsableObjectInfoBase"/> is lesser or equal to an other <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="BrowsableObjectInfoBase"/> is lesser or equal to the <see cref="BrowsableObjectInfoBase"/> to compare with.</returns>
        public static bool operator <=(BrowsableObjectInfoBase left, BrowsableObjectInfoBase right) => left is null || left.CompareTo(right) <= 0;

        /// <summary>
        /// Checks if a given <see cref="BrowsableObjectInfoBase"/> is greater than an other <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="BrowsableObjectInfoBase"/> is greater than the <see cref="BrowsableObjectInfoBase"/> to compare with.</returns>
        public static bool operator >(BrowsableObjectInfoBase left, BrowsableObjectInfoBase right) => left is object && left.CompareTo(right) > 0;

        /// <summary>
        /// Checks if a given <see cref="BrowsableObjectInfoBase"/> is greater or equal to an other <see cref="BrowsableObjectInfoBase"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="BrowsableObjectInfoBase"/> is greater or equal to the <see cref="BrowsableObjectInfoBase"/> to compare with.</returns>
        public static bool operator >=(BrowsableObjectInfoBase left, BrowsableObjectInfoBase right) => left is null ? right is null : left.CompareTo(right) >= 0;
#endregion
    }
}
