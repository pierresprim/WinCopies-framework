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

using WinCopies.Collections
#if !WinCopies2
    .Generic
#endif
    ;
using WinCopies.IO.Reflection;

#if DEBUG
#if WinCopies2
using static WinCopies.Util.Util;
#else
using WinCopies.Diagnostics;

using static WinCopies.Diagnostics.IfHelpers;
#endif
#endif

namespace WinCopies.IO.ObjectModel.Reflection
{
    public sealed class DotNetMemberInfo : BrowsableDotNetItemInfo<IDotNetItemInfoProperties, MemberInfo>, IDotNetMemberInfo
    {
        #region Properties
        public sealed override MemberInfo EncapsulatedObjectGeneric { get; }

        public override DotNetItemType DotNetItemType { get; }

        public override string ItemTypeName => ".Net member";

        public sealed override IDotNetItemInfoProperties ObjectPropertiesGeneric { get; }
        #endregion

        internal DotNetMemberInfo(in MemberInfo memberInfo, in DotNetItemType dotNetItemType, in IDotNetTypeInfo dotNetTypeInfo) : base($"{dotNetTypeInfo.Path}{IO.Path.PathSeparator}{memberInfo.Name}", memberInfo.Name, dotNetTypeInfo)
        {
#if DEBUG
            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical,
#if WinCopies2
Util.Util.
#endif
                Comparison.NotEqual, null, dotNetTypeInfo, dotNetTypeInfo.ParentDotNetAssemblyInfo));
#endif

            EncapsulatedObjectGeneric = memberInfo;

            DotNetItemType = dotNetItemType;

            ObjectPropertiesGeneric = new DotNetItemInfoProperties<IDotNetItemInfo>(this);
        }

        public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(new DotNetItemType[] { DotNetItemType.Parameter, DotNetItemType.Attribute }, null);

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(System.Collections.Generic.IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoEnumeratorStruct> func) => new Enumerable<IBrowsableObjectInfo>(() => DotNetMemberInfoEnumerator.From(this, enumerable, func));
    }
}
