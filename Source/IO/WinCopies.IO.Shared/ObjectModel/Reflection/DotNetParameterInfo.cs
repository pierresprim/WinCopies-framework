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
using System.Linq;
using System.Reflection;
using System.Text;
using WinCopies.IO.Reflection;
using WinCopies.Linq;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public interface IDotNetParameterInfo : IDotNetItemInfo
    {
        ParameterInfo ParameterInfo { get; }

        IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<CustomAttributeData> func);
    }

    public sealed class DotNetParameterInfo : BrowsableDotNetItemInfo, IDotNetParameterInfo
    {
        /// <summary>
        /// Gets the inner <see cref="System.Reflection.ParameterInfo"/>.
        /// </summary>
        public ParameterInfo ParameterInfo { get; }

        public override string ItemTypeName => ".Net parameter";

        internal DotNetParameterInfo(in ParameterInfo parameterInfo, in DotNetItemType dotNetItemType, in IDotNetItemInfo parent) : base($"{parent.Path}{WinCopies.IO.Path.PathSeparator}{parameterInfo.Name}", parameterInfo.Name, dotNetItemType, parent) => ParameterInfo = parameterInfo;

        public override IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(null);

        public IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<CustomAttributeData> func) => (func == null ? ParameterInfo.GetCustomAttributesData() : ParameterInfo.GetCustomAttributesData().WherePredicate(func)).Select(a => new DotNetAttributeInfo(a, this));
    }
}
