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

using System.Diagnostics;

using static WinCopies.Util.Util;

using IfCT = WinCopies.Util.Util.ComparisonType;
using IfCM = WinCopies.Util.Util.ComparisonMode;
using IfComp = WinCopies.Util.Util.Comparison;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public sealed class DotNetNamespaceInfo : BrowsableObjectInfo, IDotNetItemInfo
    {
        public DotNetAssemblyInfo ParentDotNetAssemblyInfo { get; }

        public override IBrowsableObjectInfo Parent { get; }

        public bool IsRootNamespace { get; }

        public DotNetItemType DotNetItemType { get; } = DotNetItemType.Namespace;

        internal DotNetNamespaceInfo(in string path, DotNetAssemblyInfo dotNetAssemblyInfo, IBrowsableObjectInfo parent, bool isRootNamespace) : base(path)
        {
#if DEBUG
            #region Null, empty and white space checks
            Debug.Assert(!IsNullEmptyOrWhiteSpace(path));

            Debug.Assert(If(IfCT.And, IfCM.Logical, IfComp.NotEqual, null, dotNetAssemblyInfo, parent));
            #endregion

            #region Value checks
            Debug.Assert(object.ReferenceEquals(parent, dotNetAssemblyInfo) == isRootNamespace);

            Debug.Assert(isRootNamespace == !path.Contains('.'));
            #endregion
#endif

            ParentDotNetAssemblyInfo = dotNetAssemblyInfo;

            IsRootNamespace = isRootNamespace;

            Parent = parent;
        }
    }
}
