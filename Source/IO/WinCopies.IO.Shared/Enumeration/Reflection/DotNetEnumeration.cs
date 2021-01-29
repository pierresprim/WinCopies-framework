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
using System.Reflection;

using WinCopies.IO.Reflection;

namespace WinCopies.IO.Enumeration.Reflection
{
    public static class DotNetEnumeration
    {
        // internal static System.Collections.Generic.IEnumerable<T> GetDotNetItemInfoEnumerator<T>(in System.Collections.Generic.IEnumerable<TypeInfo> enumerable, in Predicate<TypeInfo> func, Converter<TypeInfo, T> selector) where T : ITypeInfoItemProvider => enumerable?.WherePredicate(func).SelectConverter(selector);

        public static bool TryGetTypeInfoPredicate(in DotNetItemType typeToEnumerate, out Predicate<TypeInfo> result)
        {
            switch (typeToEnumerate)
            {
                case DotNetItemType.Struct:

                    result = t => t.IsValueType && !t.IsEnum;

                    return true;

                case DotNetItemType.Enum:

                    result = t => t.IsEnum;

                    return true;

                case DotNetItemType.Class:

                    result = t => t.IsClass;

                    return true;

                case DotNetItemType.Interface:

                    result = t => t.IsInterface;

                    return true;

                case DotNetItemType.Delegate:

                    result = t => typeof(Delegate).IsAssignableFrom(t);

                    return true;
            }

            result = null;

            return false;
        }

        public static Predicate<TypeInfo> GetTypeInfoPredicate(in DotNetItemType typeToEnumerate, in string typeToEnumerateEnumerableName) => TryGetTypeInfoPredicate(typeToEnumerate, out Predicate<TypeInfo> result)
            ? result
            : throw GetInvalidEnumArgumentException(typeToEnumerateEnumerableName, typeToEnumerate);

        public static InvalidEnumArgumentException GetInvalidEnumArgumentException(in string typeToEnumerateEnumerableName, in DotNetItemType typeToEnumerate) => new InvalidEnumArgumentException(typeToEnumerateEnumerableName, typeToEnumerate);
    }
}
