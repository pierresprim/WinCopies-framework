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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media.Imaging;

using WinCopies.IO.Reflection;
using WinCopies.IO.Selectors;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.PropertySystem;

#if DEBUG
using WinCopies.Diagnostics;

using static WinCopies.Diagnostics.IfHelpers;
#endif

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetAttributeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : DotNetItemInfo<TObjectProperties, CustomAttributeData, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetAttributeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Properties
        public sealed override CustomAttributeData InnerObjectGeneric { get; }

        public override bool IsBrowsable => false;

        public override bool IsBrowsableByDefault => false;

        public override string ItemTypeName => Properties.Resources.DotNetAttribute;
        #endregion

        protected DotNetAttributeInfo(in CustomAttributeData customAttributeData, in IDotNetItemInfo parent) : base($"{parent.Path}{IO.Path.PathSeparator}{customAttributeData.AttributeType.Name}", customAttributeData.AttributeType.Name, parent)
#if DEBUG
        {
            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical, Comparison.NotEqual, null, parent, parent.ParentDotNetAssemblyInfo, customAttributeData));
#else
=>
#endif

            InnerObjectGeneric = customAttributeData;
#if DEBUG
        }
#endif

        protected sealed override BitmapSource TryGetBitmapSource(in int size) => TryGetBitmapSource(FileIcon, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);
    }

    public class DotNetAttributeInfo : DotNetAttributeInfo<IDotNetItemInfoProperties, object, IBrowsableObjectInfoSelectorDictionary<object>, object>
    {
        public override IDotNetItemInfoProperties ObjectPropertiesGeneric { get; }

        public override IPropertySystemCollection ObjectPropertySystem => null;

        protected DotNetAttributeInfo(in CustomAttributeData customAttributeData, in IDotNetItemInfo parent) : base(customAttributeData, parent) => ObjectPropertiesGeneric = new DotNetItemInfoProperties<IDotNetItemInfo>(this, DotNetItemType.Attribute);

        protected override IEnumerable<object> GetItemProviders() => null;

        protected override IEnumerable<object> GetItemProviders(Predicate<object> predicate) => null;

        public override IBrowsableObjectInfoSelectorDictionary<object> GetSelectorDictionary() => null;
    }
}
