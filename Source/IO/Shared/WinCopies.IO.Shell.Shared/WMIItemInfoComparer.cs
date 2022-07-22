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

using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.Util;

namespace WinCopies.IO
{
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

    public class WMIItemInfoComparer<T> : FileSystemObjectComparer<T> where T : IBrowsableObjectInfoBase
    {
        protected override int CompareOverride(T x, T y)
        {
            int? result = Validate(x, y);

            if (result.HasValue)

                return result.Value;

            if (x is IWMIItemInfo _x && y is IWMIItemInfo _y)
            {
                if (_x.ObjectProperties.ItemType == _y.ObjectProperties.ItemType) return CompareLocalizedNames(x, y);

                if (_x.ObjectProperties.ItemType.IsValidEnumValue())
                {
                    if (_y.ObjectProperties.ItemType.IsValidEnumValue())

                        switch (_x.ObjectProperties.ItemType)
                        {
                            case WMIItemType.Class:

                                return _y.ObjectProperties.ItemType == WMIItemType.Instance ? -1 : 1;

                            case WMIItemType.Instance:

                                return _y.ObjectProperties.ItemType == WMIItemType.Class ? 1 : -1;

                            case WMIItemType.Namespace:

                                return -1;
                        }

                    return 1;
                }

                if (_y.ObjectProperties.ItemType.IsValidEnumValue()) return -1;
            }

            return CompareLocalizedNames(x, y);
        }
    }
}
