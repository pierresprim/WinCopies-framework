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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using WinCopies.Diagnostics;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors;

using static WinCopies.Diagnostics.IfHelpers;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetMemberInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableDotNetItemInfo<TObjectProperties, MemberInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetMemberInfoBase where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Properties
        public sealed override MemberInfo InnerObjectGeneric { get; }

        public override string ItemTypeName => Properties.Resources.DotNetMember;
        #endregion

        protected DotNetMemberInfo(in MemberInfo memberInfo, in DotNetItemType dotNetItemType, in IDotNetTypeInfo dotNetTypeInfo) : base($"{dotNetTypeInfo.Path}{IO.Path.PathSeparator}{memberInfo.Name}", memberInfo.Name, dotNetTypeInfo)
#if DEBUG
        {
            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical, Comparison.NotEqual, null, dotNetTypeInfo, dotNetTypeInfo.ParentDotNetAssemblyInfo));
#else
=>
#endif

            InnerObjectGeneric = memberInfo;
#if DEBUG
        }
#endif
    }

    public class DotNetMemberInfo : DotNetMemberInfo<IDotNetItemInfoProperties, DotNetMemberInfoItemProvider, IBrowsableObjectInfoSelectorDictionary<DotNetMemberInfoItemProvider>, DotNetMemberInfoItemProvider>, IDotNetMemberInfo
    {
        #region Properties
        public static IBrowsableObjectInfoSelectorDictionary<ShellObjectInfoItemProvider> DefaultItemSelectorDictionary { get; } = new ShellObjectInfoSelectorDictionary();

        public sealed override IDotNetItemInfoProperties ObjectPropertiesGeneric { get; }

        public override bool IsBrowsableByDefault => true;

        public override IPropertySystemCollection ObjectPropertySystem => null;
        #endregion

        protected DotNetMemberInfo(in MemberInfo memberInfo, in DotNetItemType dotNetItemType, in IDotNetTypeInfo dotNetTypeInfo) : base(memberInfo, dotNetItemType, dotNetTypeInfo) => ObjectPropertiesGeneric = new DotNetItemInfoProperties<IDotNetItemInfo>(this, dotNetItemType);

        #region Methods
        public static System.Collections.Generic.IEnumerable<DotNetItemType> GetDefaultItemTypes() => new DotNetItemType[] { DotNetItemType.Parameter, DotNetItemType.Attribute };

        protected virtual System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoItemProvider> func) => DotNetMemberInfoEnumeration.From(this, enumerable, func);

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(System.Collections.Generic.IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoItemProvider> func) => GetItems(GetItemProviders(enumerable, func));

        protected override IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders(Predicate<DotNetMemberInfoItemProvider> predicate) => GetItemProviders(GetDefaultItemTypes(), predicate);

        protected override System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders() => GetItemProviders(GetDefaultItemTypes(), null);
        #endregion
    }
}
