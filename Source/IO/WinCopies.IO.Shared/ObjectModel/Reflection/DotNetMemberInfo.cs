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
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Selectors.Reflection;
using WinCopies.PropertySystem;
using static WinCopies.Diagnostics.IfHelpers;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetMemberInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableDotNetItemInfo<TObjectProperties, MemberInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetMemberInfoBase where TObjectProperties : IDotNetTypeOrMemberInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Properties
        public sealed override MemberInfo InnerObjectGeneric { get; }

        public override IProcessFactory ProcessFactory => IO.ProcessFactory.DefaultProcessFactory;

        public override string ItemTypeName => Properties.Resources.DotNetMember;
        #endregion

        protected DotNetMemberInfo(in MemberInfo memberInfo, in IDotNetItemInfo parent) : base($"{(parent ?? throw GetArgumentNullException(nameof(parent))).Path}{IO.Path.PathSeparator}{(memberInfo ?? throw GetArgumentNullException(nameof(memberInfo))).Name}", memberInfo.Name, parent)
#if DEBUG
        {
            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical, WinCopies.Diagnostics.Comparison.NotEqual, null, parent, parent.ParentDotNetAssemblyInfo));
#else
=>
#endif

            InnerObjectGeneric = memberInfo;
#if DEBUG
        }
#endif
    }

    public class DotNetMemberInfo : DotNetMemberInfo<IDotNetTypeOrMemberInfoProperties, DotNetMemberInfoItemProvider, IBrowsableObjectInfoSelectorDictionary<DotNetMemberInfoItemProvider>, DotNetMemberInfoItemProvider>, IDotNetMemberInfo
    {
        #region Properties
        public static IBrowsableObjectInfoSelectorDictionary<DotNetMemberInfoItemProvider> DefaultItemSelectorDictionary { get; } = new DotNetMemberInfoSelectorDictionary();

        public sealed override IDotNetTypeOrMemberInfoProperties ObjectPropertiesGeneric { get; }

        public override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystem => null;

        #endregion

        protected internal DotNetMemberInfo(in MemberInfo memberInfo, in IDotNetItemInfo parent) : base(memberInfo, parent)
#if CS9
        => ObjectPropertiesGeneric =
#else
        {
            if (
#endif

                memberInfo is MethodBase

#if CS9
                ?
#else
                )

                ObjectPropertiesGeneric =
#endif

DotNetPropertyOrMethodItemInfoProperties<IDotNetMemberInfo>.From(this)

#if CS9
                :
#else
                ;

            else

                ObjectPropertiesGeneric =
#endif

                new DotNetFieldItemInfoProperties<IDotNetMemberInfo>(this);
#if !CS9
    }
#endif

        #region Methods
        public static System.Collections.Generic.IEnumerable<DotNetItemType> GetDefaultItemTypes() => new DotNetItemType[] { DotNetItemType.Parameter, DotNetItemType.Attribute };

        public override IBrowsableObjectInfoSelectorDictionary<DotNetMemberInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

        protected virtual System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoItemProvider> func) => DotNetMemberInfoEnumeration.From(this, enumerable, func);

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(System.Collections.Generic.IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoItemProvider> func) => GetItems(GetItemProviders(enumerable, func));

        protected override IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders(Predicate<DotNetMemberInfoItemProvider> predicate) => GetItemProviders(GetDefaultItemTypes(), predicate);

        protected override System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider> GetItemProviders() => GetItemProviders(GetDefaultItemTypes(), null);
        #endregion
    }
}
