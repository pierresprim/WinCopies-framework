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
using WinCopies.IO.Shell;
#endregion WinCopies

#if DEBUG
using WinCopies.Diagnostics;

using static WinCopies.Diagnostics.IfHelpers;
#endif

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetAttributeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : DotNetItemInfo<TObjectProperties, CustomAttributeData, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetAttributeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        private CustomAttributeData _data;

        #region Properties
        protected override bool IsLocalRootOverride => false;

        protected sealed override CustomAttributeData InnerObjectGenericOverride => _data;

        protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;

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

        protected sealed override BitmapSource TryGetBitmapSource(in int size) => Icons.File.Instance.TryGetBitmapSource(size);

        protected override void DisposeManaged()
        {
            _data = null;

            base.DisposeManaged();
        }
    }

    public class DotNetAttributeInfo : DotNetAttributeInfo<IDotNetItemInfoProperties, object, IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>, object>
    {
        private IDotNetItemInfoProperties _properties;

        #region Properties
        protected override IDotNetItemInfoProperties ObjectPropertiesGenericOverride => _properties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;
        #endregion Properties

        protected internal DotNetAttributeInfo(in CustomAttributeData customAttributeData, in IDotNetItemInfo parent) : base(customAttributeData, parent) => _properties = new DotNetItemInfoProperties<IDotNetItemInfo>(this, DotNetItemType.Attribute);

        /// <summary>
        /// Returns <see langword="null"/> as this item does not contain any item.
        /// </summary>
        /// <returns>A <see langword="null"/> value.</returns>
        protected override IEnumerable<object> GetItemProviders() => null;

        /// <summary>
        /// Returns <see langword="null"/> as this item does not contain any item.
        /// </summary>
        /// <returns>A <see langword="null"/> value.</returns>
        protected override IEnumerable<object> GetItemProviders(Predicate<object> predicate) => null;

        /// <summary>
        /// Returns <see langword="null"/> as this item does not contain any item.
        /// </summary>
        /// <returns>A <see langword="null"/> value.</returns>
        protected override IEnumerableSelectorDictionary<object, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => null;

        protected override void DisposeUnmanaged()
        {
            _properties.Dispose();
            _properties = null;

            base.DisposeUnmanaged();
        }
    }
}
