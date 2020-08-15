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
using WinCopies.Collections;
using WinCopies.IO.Reflection;

using static WinCopies.Util.Util;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public interface IDotNetMemberInfo : IDotNetItemInfo
    {
        MemberInfo MemberInfo { get; }

        IEnumerable<IBrowsableObjectInfo> GetItems(IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoEnumeratorStruct> func);
    }

    public sealed class DotNetMemberInfo : BrowsableDotNetItemInfo, IDotNetMemberInfo
    {
        public MemberInfo MemberInfo { get; }

        public override string ItemTypeName => ".Net member";

        internal DotNetMemberInfo(in MemberInfo memberInfo, in DotNetItemType dotNetItemType, in IDotNetTypeInfo dotNetTypeInfo) : base($"{dotNetTypeInfo.Path}{IO.Path.PathSeparator}{memberInfo.Name}", memberInfo.Name, dotNetItemType, dotNetTypeInfo)
        {
            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical, Util.Util.Comparison.NotEqual, null, dotNetTypeInfo, dotNetTypeInfo.ParentDotNetAssemblyInfo));

            MemberInfo = memberInfo;
        }

        public override IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(new DotNetItemType[] { DotNetItemType.Parameter, DotNetItemType.Attribute }, null);

        public IEnumerable<IBrowsableObjectInfo> GetItems(IEnumerable<DotNetItemType> enumerable, Predicate<DotNetMemberInfoEnumeratorStruct> func) => new Enumerable<IBrowsableObjectInfo>(() => DotNetMemberInfoEnumerator.From(this, enumerable, func));
    }
}
