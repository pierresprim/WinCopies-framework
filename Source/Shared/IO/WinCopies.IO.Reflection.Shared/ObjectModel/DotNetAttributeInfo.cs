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
#region System
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media.Imaging;
#endregion System

#region WinCopies
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.PropertySystem;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
#endregion WinCopies

#if DEBUG
using WinCopies.Diagnostics;

using static WinCopies.Diagnostics.IfHelpers;
#endif
#endregion Usings

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetAttributeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : DotNetItemInfo<TObjectProperties, CustomAttributeData, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetAttributeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        private CustomAttributeData _data;

        #region Properties
        protected override bool IsLocalRootOverride => false;

        protected sealed override CustomAttributeData InnerObjectGenericOverride => _data;

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.NotBrowsable;

        protected override string ItemTypeNameOverride => Shell.Properties.Resources.DotNetAttribute;
        #endregion

        protected DotNetAttributeInfo(in CustomAttributeData customAttributeData, in IDotNetItemInfo parent) : base($"{parent.Path}{IO.Path.PathSeparator}{customAttributeData.AttributeType.Name}", customAttributeData.AttributeType.Name, parent)
#if DEBUG
        {
            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical, Comparison.NotEqual, null, parent, parent.ParentDotNetAssemblyInfo, customAttributeData));
#else
=>
#endif

            _data = customAttributeData;
#if DEBUG
        }
#endif

        protected override void DisposeManaged()
        {
            _data = null;

            base.DisposeManaged();
        }
    }

    public class DotNetAttributeInfo : DotNetAttributeInfo<IDotNetItemInfoProperties, object, IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>, object>, IDotNetAttributeInfo
    {
        public class ItemSource : ItemSourceBase4<IDotNetAttributeInfo, object, IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>, object>
        {
            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => null;

            public override bool IsPaginationSupported => false;

            public ItemSource(in IDotNetAttributeInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            /// <summary>
            /// Returns <see langword="null"/> as this item does not contain any item.
            /// </summary>
            /// <returns>A <see langword="null"/> value.</returns>
            protected override IEnumerable<object>
#if CS8
                ?
#endif
                GetItemProviders() => null;

            /// <summary>
            /// Returns <see langword="null"/> as this item does not contain any item.
            /// </summary>
            /// <returns>A <see langword="null"/> value.</returns>
            protected override IEnumerable<object>
#if CS8
                ?
#endif
                GetItemProviders(Predicate<object> predicate) => null;
        }

        private IDotNetItemInfoProperties _properties;

        #region Properties
        protected override IItemSourcesProvider<object>
#if CS8
            ?
#endif
            ItemSourcesGenericOverride { get; }

        protected override IDotNetItemInfoProperties ObjectPropertiesGenericOverride => _properties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride => null;
        #endregion Properties

        protected internal DotNetAttributeInfo(in CustomAttributeData customAttributeData, in IDotNetItemInfo parent) : base(customAttributeData, parent)
        {
            _properties = new DotNetItemInfoProperties<IDotNetItemInfo>(this, DotNetItemType.Attribute);
            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(new ItemSource(this));
        }

        /// <summary>
        /// Returns <see langword="null"/> as this item does not contain any item.
        /// </summary>
        /// <returns>A <see langword="null"/> value.</returns>
        protected override IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>
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
    }
}
