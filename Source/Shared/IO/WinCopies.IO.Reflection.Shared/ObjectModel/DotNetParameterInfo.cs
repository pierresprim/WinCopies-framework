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

#region Usings
using System;
using System.Linq;
using System.Reflection;

#region WinCopies
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.Linq;
using WinCopies.PropertySystem;
#endregion WinCopies

using static WinCopies.ThrowHelper;
#endregion Usings

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetParameterInfo<TObjectProperties, TSelectorDictionary> : BrowsableDotNetItemInfo<TObjectProperties, ParameterInfo, CustomAttributeData, TSelectorDictionary, DotNetParameterInfoItemProvider>, IDotNetParameterInfo<TObjectProperties, TSelectorDictionary> where TObjectProperties : IDotNetParameterInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<DotNetParameterInfoItemProvider, IBrowsableObjectInfo>
    {
        private ParameterInfo _parameterInfo;

        #region Properties
        protected override bool IsLocalRootOverride => false;

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
        public class ItemSource : ItemSourceBase4<IDotNetParameterInfo, CustomAttributeData, IEnumerableSelectorDictionary<DotNetParameterInfoItemProvider, IBrowsableObjectInfo>, DotNetParameterInfoItemProvider>
        {
            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => null;

            public override bool IsPaginationSupported => false;

            public ItemSource(in IDotNetParameterInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<DotNetParameterInfoItemProvider> GetItemProviders(Predicate<CustomAttributeData>
#if CS8
                ?
#endif
                func) => (func == null ? BrowsableObjectInfo.InnerObject.GetCustomAttributesData() : BrowsableObjectInfo.InnerObject.GetCustomAttributesData().WherePredicate(func)).Select(a => new DotNetParameterInfoItemProvider(a, BrowsableObjectInfo));

            protected override System.Collections.Generic.IEnumerable<DotNetParameterInfoItemProvider> GetItemProviders() => GetItemProviders(null);
        }

        private IDotNetParameterInfoProperties _properties;

        #region Properties
        protected override IItemSourcesProvider<CustomAttributeData> ItemSourcesGenericOverride { get; }

        protected sealed override IDotNetParameterInfoProperties ObjectPropertiesGenericOverride => _properties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystemOverride => null;
        #endregion Properties

        internal DotNetParameterInfo(in ParameterInfo parameterInfo, in bool isReturn, in IDotNetItemInfo parent) : base(parameterInfo, parent)
        {
            _properties = new DotNetParameterInfoProperties<IDotNetParameterInfo>(this, isReturn);

            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(new ItemSource(this));
        }

        #region Methods
        protected override IEnumerableSelectorDictionary<DotNetParameterInfoItemProvider, IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetSelectorDictionaryOverride() => null;

        protected override void DisposeUnmanaged()
        {
            _properties.Dispose();
            _properties = null;

            base.DisposeUnmanaged();
        }
        #endregion Methods
    }
}
