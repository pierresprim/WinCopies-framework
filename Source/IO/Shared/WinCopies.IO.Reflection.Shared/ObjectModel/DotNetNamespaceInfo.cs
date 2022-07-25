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

#region Usings
using System;

#region WinCopies
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors.Reflection;
using WinCopies.PropertySystem;
#endregion WinCopies

using static WinCopies.ThrowHelper;
#endregion Usings

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetNamespaceInfo<TObjectProperties, TItemSourceParent, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableDotNetItemInfo<TObjectProperties, object, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetNamespaceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo> where TItemSourceParent : IDotNetNamespaceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>
    {
        #region Properties
        protected abstract Shell.ComponentSources.Item.ItemSource<TItemSourceParent, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> NamespaceItemSource { get; }

        protected override bool IsLocalRootOverride => false;

        protected override string ItemTypeNameOverride { get; } = Shell.Properties.Resources.DotNetNamespace;

        protected sealed override object
#if CS8
            ?
#endif
            InnerObjectGenericOverride => null;
        #endregion

        protected DotNetNamespaceInfo(in string name, in IBrowsableObjectInfo parent) : base(parent is IDotNetAssemblyInfo ? name : parent == null ? throw GetArgumentNullException(nameof(parent)) : $"{parent.Path}{System.IO.Path.DirectorySeparatorChar}{name}", name, parent) { /* Left empty. */ }

        protected abstract System.Collections.Generic.IEnumerable<TDictionaryItems> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<TDictionaryItems> func);

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetItems(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<TDictionaryItems> func) => NamespaceItemSource.GetItems(GetItemProviders(typesToEnumerate, func));
    }

    public class DotNetNamespaceInfo : DotNetNamespaceInfo<IDotNetItemInfoProperties, DotNetNamespaceInfo, DotNetNamespaceInfoItemProvider, IEnumerableSelectorDictionary<DotNetNamespaceInfoItemProvider, IBrowsableObjectInfo>, DotNetNamespaceInfoItemProvider>, IDotNetNamespaceInfo
    {
        public abstract class ItemSource<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : Shell.ComponentSources.Item.ItemSource<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            public override bool IsDisposed => false;

            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => null;

            protected ItemSource(in TBrowsableObjectInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            internal new System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                    ?
#endif
                GetItems(System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
                    ?
#endif
                items) => base.GetItems(items);

            protected override void DisposeManaged() { /* Left empty. */ }
        }

        public class ItemSource : ItemSource<DotNetNamespaceInfo, DotNetNamespaceInfoItemProvider, IEnumerableSelectorDictionary<DotNetNamespaceInfoItemProvider, IBrowsableObjectInfo>, DotNetNamespaceInfoItemProvider>
        {
            public override bool IsPaginationSupported => false;

            public ItemSource(in DotNetNamespaceInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders(Predicate<DotNetNamespaceInfoItemProvider> predicate) => BrowsableObjectInfo.GetItemProviders(GetDefaultItemTypes(), predicate);

            protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders() => BrowsableObjectInfo.GetItemProviders(GetDefaultItemTypes(), null);
        }

        private IDotNetItemInfoProperties _properties;

        #region Properties
        public static IEnumerableSelectorDictionary<DotNetNamespaceInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new DotNetNamespaceInfoSelectorDictionary();

        protected override Shell.ComponentSources.Item.ItemSource<DotNetNamespaceInfo, DotNetNamespaceInfoItemProvider, IEnumerableSelectorDictionary<DotNetNamespaceInfoItemProvider, IBrowsableObjectInfo>, DotNetNamespaceInfoItemProvider> NamespaceItemSource { get; }

        protected override IItemSourcesProvider<DotNetNamespaceInfoItemProvider> ItemSourcesGenericOverride { get; }

        protected override IDotNetItemInfoProperties ObjectPropertiesGenericOverride { get; }

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystemOverride => null;
        #endregion Properties

        protected internal DotNetNamespaceInfo(in string name, in IBrowsableObjectInfo parent) : base(name, parent)
        {
#if DEBUG
            Debug.Assert(Path.EndsWith(System.IO.Path.DirectorySeparator + name, StringComparison.CurrentCulture) || name == Path);
#endif

            _properties = new DotNetItemInfoProperties<IDotNetNamespaceInfo>(this, DotNetItemType.Namespace);
            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(NamespaceItemSource = new ItemSource(this));
        }

        #region Methods
        public static DotNetItemType[] GetDefaultItemTypes() => new DotNetItemType[] { DotNetItemType.Namespace, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate };

        protected override IEnumerableSelectorDictionary<DotNetNamespaceInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoItemProvider> func) => DotNetNamespaceInfoEnumeration.From(this, typesToEnumerate, func);

        protected override void DisposeUnmanaged()
        {
            _properties.Dispose();
            _properties = null;

            base.DisposeUnmanaged();
        }
        #endregion Methods
    }
}
