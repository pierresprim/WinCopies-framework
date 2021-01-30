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

using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors.Reflection;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetNamespaceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableDotNetItemInfo<TObjectProperties, object, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetNamespaceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Properties
        public override string ItemTypeName { get; } = Properties.Resources.DotNetNamespace;

        public sealed override object InnerObjectGeneric => null;
        #endregion

        protected DotNetNamespaceInfo(in string name, in IBrowsableObjectInfo parent) : base(parent is IDotNetAssemblyInfo ? name : parent == null ? throw GetArgumentNullException(nameof(parent)) : $"{parent.Path}{IO.Path.PathSeparator}{name}", name, parent)
        {
            // Left empty.
        }

        protected abstract System.Collections.Generic.IEnumerable<TDictionaryItems> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<TDictionaryItems> func);

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<TDictionaryItems> func) => GetItems(GetItemProviders(typesToEnumerate, func));
    }

    public class DotNetNamespaceInfo : DotNetNamespaceInfo<IDotNetItemInfoProperties, DotNetNamespaceInfoItemProvider, IBrowsableObjectInfoSelectorDictionary<DotNetNamespaceInfoItemProvider>, DotNetNamespaceInfoItemProvider>, IDotNetNamespaceInfo
    {
        #region Properties
        public static IBrowsableObjectInfoSelectorDictionary<DotNetNamespaceInfoItemProvider> DefaultItemSelectorDictionary { get; } = new DotNetNamespaceInfoSelectorDictionary();

        public override IDotNetItemInfoProperties ObjectPropertiesGeneric { get; }

        public override IPropertySystemCollection ObjectPropertySystem => null;
        #endregion

        protected internal DotNetNamespaceInfo(in string name, in IBrowsableObjectInfo parent) : base(name, parent)
        {
#if DEBUG
            Debug.Assert(Path.EndsWith(WinCopies.IO.Path.PathSeparator + name, StringComparison.CurrentCulture) || name == Path);
#endif

            ObjectPropertiesGeneric = new DotNetItemInfoProperties<IDotNetNamespaceInfo>(this, DotNetItemType.Namespace);
        }

        #region Methods
        public static DotNetItemType[] GetDefaultItemTypes() => new DotNetItemType[] { DotNetItemType.Namespace, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate };

        public override IBrowsableObjectInfoSelectorDictionary<DotNetNamespaceInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

        protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoItemProvider> func) => DotNetNamespaceInfoEnumeration.From(this, typesToEnumerate, func);

        protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders(Predicate<DotNetNamespaceInfoItemProvider> predicate) => GetItemProviders(GetDefaultItemTypes(), predicate);

        protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders() => GetItemProviders(GetDefaultItemTypes(), null);
        #endregion
    }
}
