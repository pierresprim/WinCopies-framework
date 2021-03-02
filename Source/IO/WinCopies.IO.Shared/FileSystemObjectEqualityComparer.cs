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
using System.Globalization;

using WinCopies.Collections;
using WinCopies.IO.ObjectModel;
using WinCopies.Collections.Generic;
using WinCopies.IO.PropertySystem;

#if CS8
using System.Diagnostics.CodeAnalysis;
#endif

namespace WinCopies.IO
{
    public class FileSystemObjectEqualityComparer<T> : EqualityComparer<T>
#if !WinCopies4
, IEqualityComparer<T>
#endif
        where T : IBrowsableObjectInfoBase
    {
#if !WinCopies4
        public bool Equals(
#if CS8
            [AllowNull]
#endif
        T x,
#if CS8
            [AllowNull]
#endif
        object y) => base.Equals(x, y);
#endif

        public bool Validate(in T x, in T y)
        {
            bool leftIsNull = x == null, rightIsNull = y == null;

            return (leftIsNull && rightIsNull) || (leftIsNull == rightIsNull && ReferenceEquals(x, y));
        }

        public bool EqualityCompareLocalizedNames(in T x, in T y) => x.Path.ToLower(CultureInfo.CurrentCulture) == y.Path.ToLower(CultureInfo.CurrentCulture);

        protected override bool EqualsOverride(
#if CS8
            [AllowNull]
#endif
        T x,
#if CS8
            [AllowNull]
#endif
        T y) => Validate(x, y) && EqualityCompareLocalizedNames(x, y);

        public override int GetHashCode(
#if CS8
            [DisallowNull]
#endif
        T obj) => obj.Path.ToLower(CultureInfo.CurrentCulture).GetHashCode(
#if !NETFRAMEWORK
            StringComparison.CurrentCulture
#endif
            );
    }

    public class FileSystemObjectInfoEqualityComparer<T> : FileSystemObjectEqualityComparer<T> where T : IBrowsableObjectInfoBase
    {
        protected override bool EqualsOverride(
#if CS8
            [AllowNull]
#endif
        T x,
#if CS8
            [AllowNull]
#endif
        T y) => x is IBrowsableObjectInfo<IFileSystemObjectInfoProperties> _x && y is IBrowsableObjectInfo<IFileSystemObjectInfoProperties> _y && _x.ObjectProperties.FileType == _y.ObjectProperties.FileType && Validate(x, y) && EqualityCompareLocalizedNames(x, y);

        public override int GetHashCode(
#if CS8
            [DisallowNull]
#endif
        T obj) => obj is IBrowsableObjectInfo<IFileSystemObjectInfoProperties> _obj ? _obj.ObjectProperties.FileType.GetHashCode() ^ _obj.Path.ToLower(CultureInfo.CurrentCulture).GetHashCode(
#if !NETFRAMEWORK
        StringComparison.CurrentCulture
#endif
            ) : base.GetHashCode(obj);
    }

    public class RegistryItemInfoEqualityComparer<T> : FileSystemObjectEqualityComparer<T> where T : IBrowsableObjectInfoBase
    {
        protected override bool EqualsOverride(
#if CS8
            [AllowNull]
#endif
        T x,
#if CS8
            [AllowNull]
#endif
        T y) => !(x is IBrowsableObjectInfo<IRegistryItemInfoProperties> _x && y is IBrowsableObjectInfo<IRegistryItemInfoProperties> _y && _x.ObjectProperties.RegistryItemType == _y.ObjectProperties.RegistryItemType) && Validate(x, y) && EqualityCompareLocalizedNames(x, y);

        public override int GetHashCode(
#if CS8
            [DisallowNull]
#endif
        T obj) => obj is IBrowsableObjectInfo<IRegistryItemInfoProperties> _obj ? _obj.ObjectProperties.RegistryItemType.GetHashCode() ^ _obj.Path.ToLower(CultureInfo.CurrentCulture).GetHashCode(
#if !NETFRAMEWORK
        StringComparison.CurrentCulture
#endif
            ) : base.GetHashCode(obj);
    }

    public class WMIItemInfoEqualityComparer<T> : FileSystemObjectEqualityComparer<T> where T : IBrowsableObjectInfoBase
    {
        protected override bool EqualsOverride(
#if CS8
            [AllowNull]
#endif
        T x,
#if CS8
            [AllowNull]
#endif
        T y) => !(x is IBrowsableObjectInfo<IWMIItemInfoProperties> _x && y is IBrowsableObjectInfo<IWMIItemInfoProperties> _y && _x.ObjectProperties.ItemType == _y.ObjectProperties.ItemType) && Validate(x, y) && EqualityCompareLocalizedNames(x, y);

        public override int GetHashCode(
#if CS8
            [DisallowNull]
#endif
        T obj) => obj is IBrowsableObjectInfo<IWMIItemInfoProperties> _obj ? _obj.ObjectProperties.ItemType.GetHashCode() ^ _obj.Path.ToLower(CultureInfo.CurrentCulture).GetHashCode(
#if !NETFRAMEWORK
        StringComparison.CurrentCulture
#endif
            ) : base.GetHashCode(obj);
    }
}
