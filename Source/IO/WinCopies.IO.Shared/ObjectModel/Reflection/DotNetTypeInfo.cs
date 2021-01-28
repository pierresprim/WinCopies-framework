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

using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors;

using static WinCopies.IO.Path;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetTypeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableDotNetItemInfo<TObjectProperties, TypeInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetTypeInfoBase where TObjectProperties : IDotNetTypeInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Properties
        public sealed override TypeInfo InnerObjectGeneric { get; }

        public override string ItemTypeName => Properties.Resources.DotNetType;

        public sealed override IDotNetTypeInfoProperties ObjectPropertiesGeneric { get; }
        #endregion

        protected DotNetTypeInfo(in TypeInfo typeInfo,  in IBrowsableObjectInfo parent) : base(isRootType.HasValue && isRootType.Value ? typeInfo.Name : $"{parent.Path}{PathSeparator}{typeInfo.Name}", typeInfo.Name, parent)
        {
#if DEBUG
            if (isRootType.HasValue && isRootType.Value)

                Debug.Assert(parent is IDotNetAssemblyInfo);

            else

                Debug.Assert(parent is IDotNetNamespaceInfoBase dotNetNamespaceInfo && dotNetNamespaceInfo.ParentDotNetAssemblyInfo != null);

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

            InnerObjectGeneric = typeInfo;

            ObjectPropertiesGeneric = new DotNetTypeInfoProperties<IDotNetTypeInfo>(this, itemType, isRootType);
        }

        protected DotNetTypeInfo(in Type type, in IBrowsableObjectInfo parent):this(type.GetTypeInfo(), parent)
        {
            // Left empty.
        }
    }

    public sealed class DotNetTypeInfo : DotNetTypeInfo<IDotNetTypeInfoProperties, DotNetTypeInfoItemProvider, IBrowsableObjectInfoSelectorDictionary<DotNetTypeInfoItemProvider>, DotNetTypeInfoItemProvider>, IDotNetTypeInfo
    {
        private static DotNetItemType[] _defaultTypesToEnumerate;

        public static DotNetItemType[] DefaultTypesToEnumerate => _defaultTypesToEnumerate ??= new DotNetItemType[] { DotNetItemType.GenericParameter, DotNetItemType.GenericArgument, DotNetItemType.Field, DotNetItemType.Property, DotNetItemType.Event, DotNetItemType.Constructor, DotNetItemType.Method, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate, DotNetItemType.Attribute, DotNetItemType.ImplementedInterface };

        protected internal DotNetTypeInfo(in TypeInfo type, in DotNetItemType itemType, in bool isRootType, in IBrowsableObjectInfo parent):base(type,parent)
        {

        }

        protected virtual    System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetTypeInfoItemProvider> func) => new Enumerable<DotNetTypeInfoItemProvider>(() => DotNetTypeInfoEnumerator.From(this, typesToEnumerate, func));

        protected override System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> GetItemProviders(Predicate<DotNetTypeInfoItemProvider> predicate) => GetItemProviders(DefaultTypesToEnumerate, predicate);

        protected override System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> GetItemProviders() => GetItemProviders(DefaultTypesToEnumerate, null);
    }
}
