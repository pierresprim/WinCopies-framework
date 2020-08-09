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
using static WinCopies.Util.Util;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public interface IDotNetNamespaceInfo : IDotNetItemInfo
    {
        bool IsRootNamespace { get; }
    }

    public sealed class DotNetNamespaceInfo : BrowsableDotNetItemInfo, IDotNetNamespaceInfo
    {
        public IDotNetAssemblyInfo ParentDotNetAssemblyInfo { get; }

        public override IBrowsableObjectInfo Parent { get; }

        public bool IsRootNamespace { get; }

        public override DotNetItemType DotNetItemType { get; } = DotNetItemType.Namespace;

        internal DotNetNamespaceInfo(in string path, in string name, IDotNetItemInfo parent, bool isRootNamespace) : base(path,name)
        {
#if DEBUG
            #region Null, empty and white space checks
            Debug.Assert(!IsNullEmptyOrWhiteSpace(path));

            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical, Comparison.NotEqual, null, parent, parent.ParentDotNetAssemblyInfo));
            #endregion

            #region Value checks
            Debug.Assert(object.ReferenceEquals(parent, parent.ParentDotNetAssemblyInfo) == isRootNamespace);

            Debug.Assert(isRootNamespace == !path.Contains('.', StringComparison.CurrentCulture));

            Debug.Assert(path.EndsWith('.' + name, StringComparison.CurrentCulture) || name == path);
            #endregion
#endif

            ParentDotNetAssemblyInfo = parent.ParentDotNetAssemblyInfo;

            IsRootNamespace = isRootNamespace;

            Parent = parent;
        }
    }
}
