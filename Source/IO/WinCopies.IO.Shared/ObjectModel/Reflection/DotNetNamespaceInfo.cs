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

using WinCopies.Collections;
using WinCopies.IO.Reflection;

using static WinCopies.Util.Util;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public interface IDotNetNamespaceInfo : IDotNetItemInfo
    {
        bool IsRootNamespace { get; }
    }

    public sealed class DotNetNamespaceInfo : BrowsableDotNetItemInfo, IDotNetNamespaceInfo
    {
        public bool IsRootNamespace { get; }

        public override string ItemTypeName { get; } = ".Net namespace";

        internal DotNetNamespaceInfo(in string name, bool isRootNamespace, IBrowsableObjectInfo parent) : base(isRootNamespace ? name : $"{parent.Path}{IO.Path.PathSeparator}{name}", name, DotNetItemType.Namespace, parent)
        {
#if DEBUG
            Debug.Assert((parent is IDotNetAssemblyInfo) == isRootNamespace);

            Debug.Assert(isRootNamespace == !Path.Contains(
#if NETFRAMEWORK
                "."
#else
                '.', StringComparison.CurrentCulture
#endif
                ));

            Debug.Assert(Path.EndsWith('.' + name, StringComparison.CurrentCulture) || name == Path);
#endif

            IsRootNamespace = isRootNamespace;
        }

        public override IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(new DotNetItemType[] { DotNetItemType.Namespace, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate }, GetCommonPredicate<DotNetNamespaceInfoEnumeratorStruct>());

        public IEnumerable<IBrowsableObjectInfo> GetItems(IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoEnumeratorStruct> func) => new Enumerable<IBrowsableObjectInfo>(() => new DotNetNamespaceInfoEnumerator(this, ParentDotNetAssemblyInfo.Assembly.DefinedTypes, typesToEnumerate, func));
    }
}
