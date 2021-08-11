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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.Util;

namespace WinCopies.IO
{
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

    public class RegistryItemInfoComparer<T> : FileSystemObjectComparer<T> where T : IBrowsableObjectInfoBase
    {
        protected override int CompareOverride(T x, T y)
        {
            int? result = Validate(x, y);

            if (result.HasValue)

                return result.Value;

            if (x is IRegistryItemInfo _x && y is IRegistryItemInfo _y)
            {
                if (_x.ObjectProperties.RegistryItemType == _y.ObjectProperties.RegistryItemType) return CompareLocalizedNames(x, y);

                if (_x.ObjectProperties.RegistryItemType.IsValidEnumValue())
                {
                    if (_y.ObjectProperties.RegistryItemType.IsValidEnumValue())

                        switch (_x.ObjectProperties.RegistryItemType)
                        {
                            case RegistryItemType.Key:

                                return _y.ObjectProperties.RegistryItemType == RegistryItemType.Value ? -1 : 1;

                            case RegistryItemType.Value:

                                return _y.ObjectProperties.RegistryItemType == RegistryItemType.Key ? 1 : -1;

                            case RegistryItemType.Root:

                                return -1;
                        }

                    return 1;
                }

                if (_y.ObjectProperties.RegistryItemType.IsValidEnumValue()) return -1;
            }

            return CompareLocalizedNames(x, y);
        }
    }
}
