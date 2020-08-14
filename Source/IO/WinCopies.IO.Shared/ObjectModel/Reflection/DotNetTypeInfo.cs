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
using System.Diagnostics;
using System.Reflection;

using WinCopies.IO.Reflection;

using static WinCopies.IO.Path;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public sealed class DotNetTypeInfo : BrowsableDotNetItemInfo, IDotNetTypeInfo
    {
        public TypeInfo TypeInfo { get; }

        internal DotNetTypeInfo(TypeInfo typeInfo, in DotNetItemType itemType, in IDotNetItemInfo parent) : base($"{parent.Path}{PathSeparator}{typeInfo.Name}", typeInfo.Name, itemType, parent)
        {
#if DEBUG
            Debug.Assert(parent.ParentDotNetAssemblyInfo != null);

            switch (itemType)
            {
                case DotNetItemType.Struct:

                    Debug.Assert(typeInfo.IsValueType && !typeInfo.IsEnum);

                    break;

                case DotNetItemType.Enum:

                    Debug.Assert(typeInfo.IsEnum);

                    break;

                case DotNetItemType.Class:
                case DotNetItemType.Attribute:

                    Debug.Assert(typeInfo.IsClass);

                    break;

                case DotNetItemType.Interface:

                    Debug.Assert(typeInfo.IsInterface);

                    break;

                case DotNetItemType.Delegate:

                    Debug.Assert(typeof(Delegate).IsAssignableFrom(typeInfo));

                    break;
            }
#endif

            TypeInfo = typeInfo;
        }
    }
}
