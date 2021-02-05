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
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Selectors.Reflection;

using static WinCopies.IO.Path;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetTypeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableDotNetItemInfo<TObjectProperties, TypeInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetTypeInfoBase where TObjectProperties : IDotNetTypeInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Properties
        public sealed override TypeInfo InnerObjectGeneric { get; }

        public override string ItemTypeName => Properties.Resources.DotNetType;
        #endregion

        protected DotNetTypeInfo(in TypeInfo type, in IBrowsableObjectInfo parent) : base(parent is IDotNetAssemblyInfo ? type.Name : $"{parent.Path}{PathSeparator}{type.Name}", type.Name, parent)
#if DEBUG
        {
            if (!(parent is IDotNetAssemblyInfo))

                Debug.Assert(parent is IDotNetNamespaceInfoBase dotNetNamespaceInfo && dotNetNamespaceInfo.ParentDotNetAssemblyInfo != null);
#else
        =>
#endif

            InnerObjectGeneric = type ?? throw GetArgumentNullException(nameof(type));
#if DEBUG
        }
#endif

        protected DotNetTypeInfo(in Type type, in IBrowsableObjectInfo parent) : this(type.GetTypeInfo(), parent)
        {
            // Left empty.
        }
    }

    public class DotNetTypeInfo : DotNetTypeInfo<IDotNetTypeInfoProperties, DotNetTypeInfoItemProvider, IBrowsableObjectInfoSelectorDictionary<DotNetTypeInfoItemProvider>, DotNetTypeInfoItemProvider>, IDotNetTypeInfo
    {
        private static DotNetItemType[] _defaultTypesToEnumerate;

        public static DotNetItemType[] DefaultTypesToEnumerate => _defaultTypesToEnumerate ??= new DotNetItemType[] { DotNetItemType.GenericParameter, DotNetItemType.GenericArgument, DotNetItemType.Field, DotNetItemType.Property, DotNetItemType.Event, DotNetItemType.Constructor, DotNetItemType.Method, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate, DotNetItemType.Attribute, DotNetItemType.BaseTypeOrInterface };

        public static IBrowsableObjectInfoSelectorDictionary<DotNetTypeInfoItemProvider> DefaultItemSelectorDictionary { get; } = new DotNetTypeInfoSelectorDictionary();

        public sealed override IDotNetTypeInfoProperties ObjectPropertiesGeneric { get; }

        public override IPropertySystemCollection ObjectPropertySystem => null;

        protected internal DotNetTypeInfo(in TypeInfo type, in DotNetItemType itemType, in bool isRootType, in IBrowsableObjectInfo parent) : base(type, parent)
#if DEBUG
        {
            switch (itemType)
            {
                case DotNetItemType.Struct:

                    Debug.Assert(type.IsValueType && !type.IsEnum);

                    break;

                case DotNetItemType.Enum:

                    Debug.Assert(type.IsEnum);

                    break;

                case DotNetItemType.Class:
                case DotNetItemType.Attribute:

                    Debug.Assert(type.IsClass);

                    break;

                case DotNetItemType.Interface:

                    Debug.Assert(type.IsInterface);

                    break;

                case DotNetItemType.Delegate:

                    Debug.Assert(typeof(Delegate).IsAssignableFrom(type));

                    break;
            }
#else
        =>
#endif

            ObjectPropertiesGeneric = new DotNetTypeInfoProperties<IDotNetTypeInfo>(this, itemType, isRootType);

#if DEBUG
        }
#endif

        public override IBrowsableObjectInfoSelectorDictionary<DotNetTypeInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

        protected virtual System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetTypeInfoItemProvider> func) => DotNetTypeInfoEnumeration.From(this, typesToEnumerate, func);

        protected override System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> GetItemProviders(Predicate<DotNetTypeInfoItemProvider> predicate) => GetItemProviders(DefaultTypesToEnumerate, predicate);

        protected override System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> GetItemProviders() => GetItemProviders(DefaultTypesToEnumerate, null);
    }
}
