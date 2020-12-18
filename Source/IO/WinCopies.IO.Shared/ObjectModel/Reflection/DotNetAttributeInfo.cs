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

#if DEBUG
#if WinCopies2
using static WinCopies.Util.Util;
#else
using WinCopies.Diagnostics;

using static WinCopies.Diagnostics.IfHelpers;
#endif
#endif

namespace WinCopies.IO.ObjectModel.Reflection
{
    public sealed class DotNetAttributeInfo : DotNetItemInfo<IDotNetItemInfoProperties, CustomAttributeData>, IDotNetAttributeInfo<IDotNetItemInfoProperties>
    {
        #region Properties
        public sealed override CustomAttributeData EncapsulatedObjectGeneric { get; }

        public override bool IsBrowsable => false;

        public override string ItemTypeName => ".Net attribute";

        public override DotNetItemType DotNetItemType => DotNetItemType.Attribute;

        public override IDotNetItemInfoProperties ObjectPropertiesGeneric { get; }
        #endregion

        internal DotNetAttributeInfo(in CustomAttributeData customAttributeData, in IDotNetItemInfo parent) : base($"{parent.Path}{IO.Path.PathSeparator}{customAttributeData.AttributeType.Name}", customAttributeData.AttributeType.Name, parent)
        {
#if DEBUG
            Debug.Assert(If(ComparisonType.And, ComparisonMode.Logical, Comparison.NotEqual, null, parent, parent.ParentDotNetAssemblyInfo, customAttributeData));
#endif

            EncapsulatedObjectGeneric = customAttributeData;

            ObjectPropertiesGeneric = new DotNetItemInfoProperties<IDotNetItemInfo>(this);
        }

        #region Methods
        protected sealed override BitmapSource TryGetBitmapSource(in int size) => TryGetBitmapSource(FileIcon, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);

        public override IEnumerable<IBrowsableObjectInfo> GetItems() => throw new NotSupportedException("This item does not support browsing.");
        #endregion
    }
}
