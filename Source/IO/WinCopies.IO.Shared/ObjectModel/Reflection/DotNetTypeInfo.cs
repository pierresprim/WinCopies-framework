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
using System.Diagnostics;
using System.Reflection;

using WinCopies.Collections
#if WinCopies3
    .Generic
#endif
    ;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;

using static WinCopies.IO.Path;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public sealed class DotNetTypeInfo : BrowsableDotNetItemInfo<IDotNetTypeInfoProperties, TypeInfo>, IDotNetTypeInfo
    {
        #region Properties
        public sealed override TypeInfo EncapsulatedObjectGeneric { get; }

        public override string ItemTypeName => ".Net type";

        public sealed override IDotNetTypeInfoProperties ObjectPropertiesGeneric { get; }
        #endregion

        internal DotNetTypeInfo(TypeInfo typeInfo, in DotNetItemType itemType, in bool? isRootType, in IBrowsableObjectInfo parent) : base(isRootType.HasValue && isRootType.Value ? typeInfo.Name : $"{parent.Path}{PathSeparator}{typeInfo.Name}", typeInfo.Name, parent)
        {
#if DEBUG
            if (isRootType.HasValue && isRootType.Value)

                Debug.Assert(parent is IDotNetAssemblyInfo);

            else

                Debug.Assert(parent is IDotNetNamespaceInfo dotNetNamespaceInfo && dotNetNamespaceInfo.ParentDotNetAssemblyInfo != null);

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

            EncapsulatedObjectGeneric = typeInfo;

            ObjectPropertiesGeneric = new DotNetTypeInfoProperties<IDotNetTypeInfo>(this, itemType, isRootType);
        }

        public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(new DotNetItemType[] { DotNetItemType.GenericParameter, DotNetItemType.GenericArgument, DotNetItemType.Field, DotNetItemType.Property, DotNetItemType.Event, DotNetItemType.Constructor, DotNetItemType.Method, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate, DotNetItemType.Attribute, DotNetItemType.ImplementedInterface }, null);

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetTypeInfoEnumeratorStruct> func) => new Enumerable<IBrowsableObjectInfo>(() => DotNetTypeInfoEnumerator.From(this, typesToEnumerate, func));
    }
}
