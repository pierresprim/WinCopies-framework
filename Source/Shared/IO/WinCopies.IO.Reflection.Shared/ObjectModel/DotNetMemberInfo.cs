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
#region System
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
#endregion System

#region WinCopies
using WinCopies.Diagnostics;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors.Reflection;
using WinCopies.PropertySystem;
using WinCopies.Util;
#endregion WinCopies

#region Static Usings
using static WinCopies.Diagnostics.IfHelpers;
using static WinCopies.ThrowHelper;
#endregion Static Usings
#endregion Usings

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetMemberInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableDotNetItemInfo<TObjectProperties, MemberInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetMemberInfoBase where TObjectProperties : IDotNetTypeOrMemberInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        private MemberInfo _memberInfo;

        #region Properties
        protected override bool IsLocalRootOverride => false;

        protected sealed override MemberInfo InnerObjectGenericOverride => _memberInfo;

        protected override string ItemTypeNameOverride => Shell.Properties.Resources.DotNetMember;
        #endregion

        protected DotNetMemberInfo(in MemberInfo memberInfo, in IDotNetItemInfo parent) : base($"{(parent ?? throw GetArgumentNullException(nameof(parent))).Path}{System.IO.Path.DirectorySeparatorChar}{(memberInfo ?? throw GetArgumentNullException(nameof(memberInfo))).Name}", memberInfo.Name, parent)
#if DEBUG
        {
            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical, WinCopies.Diagnostics.Comparison.NotEqual, null, parent, parent.ParentDotNetAssemblyInfo));
#else
=>
#endif

            _memberInfo = memberInfo;
#if DEBUG
        }
#endif

        protected override void DisposeManaged()
        {
            _memberInfo = null;

            base.DisposeManaged();
        }
    }

    public class DotNetMemberInfo : DotNetMemberInfo<IDotNetTypeOrMemberInfoProperties, DotNetMemberInfoItemProvider, IEnumerableSelectorDictionary<DotNetMemberInfoItemProvider, IBrowsableObjectInfo>, DotNetMemberInfoItemProvider>, IDotNetMemberInfo
    {
        public class ItemSource : ItemSourceBase4<IDotNetMemberInfo, DotNetMemberInfoItemProvider, IEnumerableSelectorDictionary<DotNetMemberInfoItemProvider, IBrowsableObjectInfo>, DotNetMemberInfoItemProvider>
        {
            public override bool IsPaginationSupported => false;

            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => null;

            public ItemSource(in IDotNetMemberInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected internal virtual IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders(IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoItemProvider>
#if CS8
                ?
#endif
                func) => DotNetMemberInfoEnumeration.From(BrowsableObjectInfo, enumerable, func);

            protected override IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders(Predicate<DotNetMemberInfoItemProvider> predicate) => GetItemProviders(GetDefaultItemTypes(), predicate);

            protected override IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders() => GetItemProviders(GetDefaultItemTypes(), null);

            internal IEnumerable<IBrowsableObjectInfo>
#if CS8
                    ?
#endif
                GetItems(IEnumerable<DotNetMemberInfoItemProvider>
#if CS8
                ?
#endif
                items) => base.GetItems(items);
        }

        private IDotNetTypeOrMemberInfoProperties _properties;
        private readonly ItemSource _itemSource;

        #region Properties
        protected override IItemSourcesProvider<DotNetMemberInfoItemProvider>
#if CS8
            ?
#endif
            ItemSourcesGenericOverride
        { get; }

        public static IEnumerableSelectorDictionary<DotNetMemberInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new DotNetMemberInfoSelectorDictionary();

        protected sealed override IDotNetTypeOrMemberInfoProperties ObjectPropertiesGenericOverride => _properties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride => null;

        #endregion

        protected internal DotNetMemberInfo(in MemberInfo memberInfo, in IDotNetItemInfo parent) : base(memberInfo, parent)
        {
            _properties = memberInfo is MethodBase ? DotNetPropertyOrMethodItemInfoProperties<IDotNetMemberInfo>.From(this) : new DotNetFieldItemInfoProperties<IDotNetMemberInfo>(this).AsFromType<IDotNetTypeOrMemberInfoProperties>();

            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(_itemSource = new ItemSource(this));
        }

        #region Methods
        public static IEnumerable<DotNetItemType> GetDefaultItemTypes() => new DotNetItemType[] { DotNetItemType.Parameter, DotNetItemType.Attribute };

        protected override IEnumerableSelectorDictionary<DotNetMemberInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        public IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetItems(IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoItemProvider> func) => _itemSource.GetItems(_itemSource.GetItemProviders(enumerable, func));

        protected override void DisposeUnmanaged()
        {
            _properties.Dispose();
            _properties = null;

            base.DisposeUnmanaged();
        }
        #endregion
    }
}
