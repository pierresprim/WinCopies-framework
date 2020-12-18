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
using System.Linq;
using System.Reflection;

using WinCopies.IO.Reflection;
using WinCopies.Linq;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public sealed class DotNetParameterInfo : BrowsableDotNetItemInfo<IDotNetItemInfoProperties, ParameterInfo>, IDotNetParameterInfo<IDotNetItemInfoProperties>
    {
        #region Properties
        public override string ItemTypeName => ".Net parameter";

        public override DotNetItemType DotNetItemType { get; }

        /// <summary>
        /// Gets the inner <see cref="ParameterInfo"/>.
        /// </summary>
        public sealed override ParameterInfo EncapsulatedObjectGeneric { get; }

        public sealed override IDotNetItemInfoProperties ObjectPropertiesGeneric { get; }
        #endregion

        internal DotNetParameterInfo(in ParameterInfo parameterInfo, in DotNetItemType dotNetItemType, in IDotNetItemInfo parent) : base($"{parent.Path}{WinCopies.IO.Path.PathSeparator}{parameterInfo.Name}", parameterInfo.Name, parent)
        {
            EncapsulatedObjectGeneric = parameterInfo;

            DotNetItemType = dotNetItemType;

            ObjectPropertiesGeneric = new DotNetItemInfoProperties<IDotNetItemInfo>(this);
        }

        public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(null);

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<CustomAttributeData> func) => (func == null ? EncapsulatedObjectGeneric.GetCustomAttributesData() : EncapsulatedObjectGeneric.GetCustomAttributesData().WherePredicate(func)).Select(a => new DotNetAttributeInfo(a, this));
    }
}
