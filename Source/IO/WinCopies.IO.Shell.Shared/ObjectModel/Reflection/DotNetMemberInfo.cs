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

using WinCopies.Diagnostics;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors.Reflection;
using WinCopies.PropertySystem;

using static WinCopies.Diagnostics.IfHelpers;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetMemberInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableDotNetItemInfo<TObjectProperties, MemberInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetMemberInfoBase where TObjectProperties : IDotNetTypeOrMemberInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        private MemberInfo _memberInfo;

        #region Properties
        protected sealed override MemberInfo InnerObjectGenericOverride => _memberInfo;

        protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;

        protected override string ItemTypeNameOverride => Shell.Properties.Resources.DotNetMember;
        #endregion

        protected DotNetMemberInfo(in MemberInfo memberInfo, in IDotNetItemInfo parent) : base($"{(parent ?? throw GetArgumentNullException(nameof(parent))).Path}{IO.Path.PathSeparator}{(memberInfo ?? throw GetArgumentNullException(nameof(memberInfo))).Name}", memberInfo.Name, parent)
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
        private IDotNetTypeOrMemberInfoProperties _properties;

        #region Properties
        public static IEnumerableSelectorDictionary<DotNetMemberInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new DotNetMemberInfoSelectorDictionary();

        protected sealed override IDotNetTypeOrMemberInfoProperties ObjectPropertiesGenericOverride => _properties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

        #endregion

        protected internal DotNetMemberInfo(in MemberInfo memberInfo, in IDotNetItemInfo parent) : base(memberInfo, parent)
#if CS9
        => _properties =
#else
        {
            if (
#endif

                memberInfo is MethodBase

#if CS9
                ?
#else
                )

                _properties =
#endif

DotNetPropertyOrMethodItemInfoProperties<IDotNetMemberInfo>.From(this)

#if CS9
                :
#else
                ;

            else

                _properties =
#endif

                new DotNetFieldItemInfoProperties<IDotNetMemberInfo>(this);
#if !CS9
        }
#endif

        #region Methods
        public static System.Collections.Generic.IEnumerable<DotNetItemType> GetDefaultItemTypes() => new DotNetItemType[] { DotNetItemType.Parameter, DotNetItemType.Attribute };

        protected override IEnumerableSelectorDictionary<DotNetMemberInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected virtual System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoItemProvider> func) => DotNetMemberInfoEnumeration.From(this, enumerable, func);

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(System.Collections.Generic.IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoItemProvider> func) => GetItems(GetItemProviders(enumerable, func));

        protected override IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders(Predicate<DotNetMemberInfoItemProvider> predicate) => GetItemProviders(GetDefaultItemTypes(), predicate);

        protected override System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders() => GetItemProviders(GetDefaultItemTypes(), null);

        protected override void DisposeUnmanaged()
        {
            _properties.Dispose();
            _properties = null;

            base.DisposeUnmanaged();
        }
        #endregion
    }
}
