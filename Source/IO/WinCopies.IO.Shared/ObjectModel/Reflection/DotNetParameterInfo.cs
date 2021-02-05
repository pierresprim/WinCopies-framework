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

using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetParameterInfo<TObjectProperties, TSelectorDictionary> : BrowsableDotNetItemInfo<TObjectProperties, ParameterInfo, CustomAttributeData, TSelectorDictionary, DotNetParameterInfoItemProvider>, IDotNetParameterInfo<TObjectProperties, TSelectorDictionary> where TObjectProperties : IDotNetParameterInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<DotNetParameterInfoItemProvider>
    {
        #region Properties
        public override string ItemTypeName => Properties.Resources.DotNetParameter;

        /// <summary>
        /// Gets the inner <see cref="ParameterInfo"/>.
        /// </summary>
        public sealed override ParameterInfo InnerObjectGeneric { get; }
        #endregion

        internal DotNetParameterInfo(in ParameterInfo parameterInfo, in IDotNetItemInfo parent) : base($"{(parent ?? throw GetArgumentNullException(nameof(parent))).Path}{WinCopies.IO.Path.PathSeparator}{(parameterInfo ?? throw GetArgumentNullException(nameof(parameterInfo))).Name}", parameterInfo.Name, parent) => InnerObjectGeneric = parameterInfo;
    }

    public class DotNetParameterInfo : DotNetParameterInfo<IDotNetParameterInfoProperties, IBrowsableObjectInfoSelectorDictionary<DotNetParameterInfoItemProvider>>, IDotNetParameterInfo
    {
        public sealed override IDotNetParameterInfoProperties ObjectPropertiesGeneric { get; }

        public override IPropertySystemCollection ObjectPropertySystem => null;

        internal DotNetParameterInfo(in ParameterInfo parameterInfo, in bool isReturn, in IDotNetItemInfo parent) : base(parameterInfo, parent) => ObjectPropertiesGeneric = new DotNetParameterInfoProperties<IDotNetParameterInfo>(this, isReturn);

        protected override System.Collections.Generic.IEnumerable<DotNetParameterInfoItemProvider> GetItemProviders(Predicate<CustomAttributeData> func) => (func == null ? InnerObjectGeneric.GetCustomAttributesData() : InnerObjectGeneric.GetCustomAttributesData().WherePredicate(func)).Select(a => new DotNetParameterInfoItemProvider(a, this));

        protected override System.Collections.Generic.IEnumerable<DotNetParameterInfoItemProvider> GetItemProviders() => GetItemProviders(null);

        public override IBrowsableObjectInfoSelectorDictionary<DotNetParameterInfoItemProvider> GetSelectorDictionary() => null;
    }
}
