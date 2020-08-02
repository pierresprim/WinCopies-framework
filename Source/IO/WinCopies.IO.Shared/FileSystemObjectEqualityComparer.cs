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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using WinCopies.Collections;
using WinCopies.IO.ObjectModel;

namespace WinCopies.IO
{
    public class FileSystemObjectEqualityComparer<T> : EqualityComparer<T>
#if WinCopies2
, IEqualityComparer<T>
#endif
        where T : IFileSystemObject
    {
#if WinCopies2
        public bool Equals(
#if !CS7
            [AllowNull]
#endif
        T x,
#if !CS7
            [AllowNull]
#endif
        object y) => base.Equals(x, y);
#endif

        public bool Validate(in T x, in T y)
        {
            bool leftIsNull = x == null, rightIsNull = y == null;

            if (leftIsNull && rightIsNull)

                return true;

            if (leftIsNull != rightIsNull)

                return false;

            return ReferenceEquals(x, y) || x.ItemFileSystemType == y.ItemFileSystemType;
        }

        public bool EqualityCompareLocalizedNames(in T x, in T y) => x.Path.ToLower(CultureInfo.CurrentCulture) == y.Path.ToLower(CultureInfo.CurrentCulture);

        protected override bool EqualsOverride(
#if !CS7
            [AllowNull]
#endif
        T x,
#if !CS7
            [AllowNull]
#endif
        T y) => Validate(x, y) && EqualityCompareLocalizedNames(x, y);

        public override int GetHashCode(
#if !CS7
            [DisallowNull]
#endif
        T obj) => obj.Path.ToLower(CultureInfo.CurrentCulture).GetHashCode(
#if !NETFRAMEWORK
            StringComparison.CurrentCulture
#endif
            );
    }

    public class FileSystemObjectInfoEqualityComparer<T> : FileSystemObjectEqualityComparer<T> where T : IFileSystemObject
    {
        protected override bool EqualsOverride(
#if !CS7
            [AllowNull]
#endif
        T x,
#if !CS7
            [AllowNull]
#endif
        T y) => !(x is IFileSystemObjectInfo _x && y is IFileSystemObjectInfo _y && _x.FileType == _y.FileType) && Validate(x, y) && EqualityCompareLocalizedNames(x, y);

        public override int GetHashCode(
#if !CS7
            [DisallowNull]
#endif
        T obj) => obj is IFileSystemObjectInfo _obj ? _obj.FileType.GetHashCode() ^ _obj.Path.ToLower(CultureInfo.CurrentCulture).GetHashCode(
#if !NETFRAMEWORK
        StringComparison.CurrentCulture
#endif
            ) : base.GetHashCode(obj);
    }

    public class RegistryItemInfoEqualityComparer<T> : FileSystemObjectEqualityComparer<T> where T : IFileSystemObject
    {
        protected override bool EqualsOverride(
#if !CS7
            [AllowNull]
#endif
        T x,
#if !CS7
            [AllowNull]
#endif
        T y) => !(x is IRegistryItemInfo _x && y is IRegistryItemInfo _y && _x.RegistryItemType == _y.RegistryItemType) && Validate(x, y) && EqualityCompareLocalizedNames(x, y);

        public override int GetHashCode(
#if !CS7
            [DisallowNull]
#endif
        T obj) => obj is IFileSystemObjectInfo _obj ? _obj.FileType.GetHashCode() ^ _obj.Path.ToLower(CultureInfo.CurrentCulture).GetHashCode(
#if !NETFRAMEWORK
        StringComparison.CurrentCulture
#endif
            ) : base.GetHashCode(obj);
    }

    public class WMIItemInfoEqualityComparer<T> : FileSystemObjectEqualityComparer<T> where T : IFileSystemObject
    {
        protected override bool EqualsOverride(
#if !CS7
            [AllowNull]
#endif
        T x,
#if !CS7
            [AllowNull]
#endif
        T y) => !(x is IWMIItemInfo _x && y is IWMIItemInfo _y && _x.WMIItemType == _y.WMIItemType) && Validate(x, y) && EqualityCompareLocalizedNames(x, y);

        public override int GetHashCode(
#if !CS7
            [DisallowNull]
#endif
        T obj) => obj is IFileSystemObjectInfo _obj ? _obj.FileType.GetHashCode() ^ _obj.Path.ToLower(CultureInfo.CurrentCulture).GetHashCode(
#if !NETFRAMEWORK
        StringComparison.CurrentCulture
#endif
            ) : base.GetHashCode(obj);
    }
}
