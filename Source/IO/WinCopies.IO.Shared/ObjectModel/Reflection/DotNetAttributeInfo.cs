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
using System.Reflection;

using WinCopies.IO.Reflection;

using static WinCopies.Util.Util;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public sealed class DotNetAttributeInfo : BrowsableDotNetItemInfo, IDotNetAttributeInfo
    {
        public override IBrowsableObjectInfo Parent { get; }

        public CustomAttributeData CustomAttributeData { get; }

        public override DotNetItemType DotNetItemType { get; } = DotNetItemType.Attribute;

        public override IDotNetAssemblyInfo ParentDotNetAssemblyInfo { get; }

        internal DotNetAttributeInfo(in string path, in IDotNetItemInfo parent, in CustomAttributeData customAttributeData) : base(path, customAttributeData.AttributeType.Name)
        {
            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical, Comparison.NotEqual, null, parent, parent.ParentDotNetAssemblyInfo, customAttributeData));

            Parent = parent;

            CustomAttributeData = customAttributeData;

            ParentDotNetAssemblyInfo = parent.ParentDotNetAssemblyInfo;
        }
    }
}
