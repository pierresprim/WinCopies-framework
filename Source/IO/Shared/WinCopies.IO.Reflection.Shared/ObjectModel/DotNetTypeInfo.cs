﻿/* Copyright © Pierre Sprimont, 2020
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

#region Usings
using System;
using System.Diagnostics;
using System.Reflection;

#region WinCopies
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors.Reflection;
using WinCopies.PropertySystem;
#endregion WinCopies

using static WinCopies.IO.Path;
using static WinCopies.ThrowHelper;
#endregion Usings

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetTypeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableDotNetItemInfo<TObjectProperties, TypeInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetTypeInfoBase where TObjectProperties : IDotNetTypeInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        private TypeInfo _typeInfo;

        #region Properties
        protected override bool IsLocalRootOverride => false;

        protected sealed override TypeInfo InnerObjectGenericOverride => _typeInfo;

        protected override string ItemTypeNameOverride => Shell.Properties.Resources.DotNetType;
        #endregion

        protected DotNetTypeInfo(in TypeInfo type, in IBrowsableObjectInfo parent) : base(parent is IDotNetAssemblyInfo ? type.Name : $"{parent.Path}{PathSeparator}{type.Name}", type.Name, parent)
#if DEBUG
        {
            if (
#if !CS9
                !(
#endif
                parent is
#if CS9
                not
#endif
                IDotNetAssemblyInfo)
#if !CS9
                )
#endif

                Debug.Assert(parent is IDotNetNamespaceInfoBase dotNetNamespaceInfo && dotNetNamespaceInfo.ParentDotNetAssemblyInfo != null);
#else
        =>
#endif

            _typeInfo = type ?? throw GetArgumentNullException(nameof(type));
#if DEBUG
        }
#endif

        protected DotNetTypeInfo(in Type type, in IBrowsableObjectInfo parent) : this(type.GetTypeInfo(), parent)
        {
            // Left empty.
        }

        protected override void DisposeManaged()
        {
            _typeInfo = null;

            base.DisposeManaged();
        }
    }

    public class DotNetTypeInfo : DotNetTypeInfo<IDotNetTypeInfoProperties, DotNetTypeInfoItemProvider, IEnumerableSelectorDictionary<DotNetTypeInfoItemProvider, IBrowsableObjectInfo>, DotNetTypeInfoItemProvider>, IDotNetTypeInfo
    {
        public class ItemSource : ItemSourceBase4<DotNetTypeInfo, DotNetTypeInfoItemProvider, IEnumerableSelectorDictionary<DotNetTypeInfoItemProvider, IBrowsableObjectInfo>, DotNetTypeInfoItemProvider>
        {
            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => null;

            public override bool IsPaginationSupported => false;

            public ItemSource(in DotNetTypeInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> GetItemProviders(Predicate<DotNetTypeInfoItemProvider> predicate) => BrowsableObjectInfo.GetItemProviders(DefaultTypesToEnumerate, predicate);

            protected override System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> GetItemProviders() => BrowsableObjectInfo.GetItemProviders(DefaultTypesToEnumerate, null);
        }

        private static DotNetItemType[]
#if CS8
            ?
#endif
            _defaultTypesToEnumerate;

        private IDotNetTypeInfoProperties _properties;

        #region Properties
        public static DotNetItemType[] DefaultTypesToEnumerate => _defaultTypesToEnumerate
#if CS8
            ??=
#else
            ?? (_defaultTypesToEnumerate =
#endif
            new DotNetItemType[] { DotNetItemType.GenericParameter, DotNetItemType.GenericArgument, DotNetItemType.Field, DotNetItemType.Property, DotNetItemType.Event, DotNetItemType.Constructor, DotNetItemType.Method, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate, DotNetItemType.Attribute, DotNetItemType.BaseTypeOrInterface }
#if !CS8
            )
#endif
            ;

        public static IEnumerableSelectorDictionary<DotNetTypeInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new DotNetTypeInfoSelectorDictionary();

        protected sealed override IDotNetTypeInfoProperties ObjectPropertiesGenericOverride => _properties;

        protected override IItemSourcesProvider<DotNetTypeInfoItemProvider> ItemSourcesGenericOverride { get; }

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystemOverride => null;
        #endregion Properties

        protected internal DotNetTypeInfo(in TypeInfo type, in DotNetItemType itemType, in bool isRootType, in IBrowsableObjectInfo parent) : base(type, parent)
        {
#if DEBUG
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
#endif

            _properties = new DotNetTypeInfoProperties<IDotNetTypeInfo>(this, itemType, isRootType);

            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(new ItemSource(this));
        }

        protected override IEnumerableSelectorDictionary<DotNetTypeInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected virtual System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetTypeInfoItemProvider>
#if CS8
            ?
#endif
            func) => DotNetTypeInfoEnumeration.From(this, typesToEnumerate, func);

        protected override void DisposeUnmanaged()
        {
            _properties.Dispose();
            _properties = null;

            base.DisposeUnmanaged();
        }
    }
}
