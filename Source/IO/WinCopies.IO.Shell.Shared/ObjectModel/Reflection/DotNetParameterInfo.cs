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
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.Linq;
using WinCopies.PropertySystem;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetParameterInfo<TObjectProperties, TSelectorDictionary> : BrowsableDotNetItemInfo<TObjectProperties, ParameterInfo, CustomAttributeData, TSelectorDictionary, DotNetParameterInfoItemProvider>, IDotNetParameterInfo<TObjectProperties, TSelectorDictionary> where TObjectProperties : IDotNetParameterInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<DotNetParameterInfoItemProvider, IBrowsableObjectInfo>
    {
        private ParameterInfo _parameterInfo;

        #region Properties
        protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;

        protected override string ItemTypeNameOverride => Shell.Properties.Resources.DotNetParameter;

        /// <summary>
        /// Gets the inner <see cref="ParameterInfo"/>.
        /// </summary>
        protected sealed override ParameterInfo InnerObjectGenericOverride => _parameterInfo;
        #endregion

        internal DotNetParameterInfo(in ParameterInfo parameterInfo, in IDotNetItemInfo parent) : base($"{(parent ?? throw GetArgumentNullException(nameof(parent))).Path}{WinCopies.IO.Path.PathSeparator}{(parameterInfo ?? throw GetArgumentNullException(nameof(parameterInfo))).Name}", parameterInfo.Name, parent) => _parameterInfo = parameterInfo;

        protected override void DisposeManaged()
        {
            _parameterInfo = null;

            base.DisposeManaged();
        }
    }

    public class DotNetParameterInfo : DotNetParameterInfo<IDotNetParameterInfoProperties, IEnumerableSelectorDictionary<DotNetParameterInfoItemProvider, IBrowsableObjectInfo>>, IDotNetParameterInfo
    {
        private IDotNetParameterInfoProperties _properties;

        #region Properties
        protected sealed override IDotNetParameterInfoProperties ObjectPropertiesGenericOverride => _properties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;
        #endregion Properties

        internal DotNetParameterInfo(in ParameterInfo parameterInfo, in bool isReturn, in IDotNetItemInfo parent) : base(parameterInfo, parent) => _properties = new DotNetParameterInfoProperties<IDotNetParameterInfo>(this, isReturn);

        #region Methods
        protected override System.Collections.Generic.IEnumerable<DotNetParameterInfoItemProvider> GetItemProviders(Predicate<CustomAttributeData> func) => (func == null ? InnerObjectGeneric.GetCustomAttributesData() : InnerObjectGeneric.GetCustomAttributesData().WherePredicate(func)).Select(a => new DotNetParameterInfoItemProvider(a, this));

        protected override System.Collections.Generic.IEnumerable<DotNetParameterInfoItemProvider> GetItemProviders() => GetItemProviders(null);

        protected override IEnumerableSelectorDictionary<DotNetParameterInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => null;

        protected override void DisposeUnmanaged()
        {
            _properties.Dispose();
            _properties = null;

            base.DisposeUnmanaged();
        }
        #endregion Methods
    }
}
