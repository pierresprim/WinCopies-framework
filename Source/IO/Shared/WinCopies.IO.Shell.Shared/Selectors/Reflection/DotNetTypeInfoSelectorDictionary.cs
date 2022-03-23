/* Copyright © Pierre Sprimont, 2021
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

using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.ObjectModel.Reflection;

namespace WinCopies.IO.Selectors.Reflection
{
    public class DotNetTypeInfoSelectorDictionary : EnumerableSelectorDictionary<DotNetTypeInfoItemProvider, IBrowsableObjectInfo>
    {
        public static IBrowsableObjectInfo Convert(DotNetTypeInfoItemProvider item) => item.MemberInfoItemProvider != null ? new DotNetMemberInfo(item.MemberInfoItemProvider.MemberInfo, item.Parent)
            : item.CustomAttributeData != null ? new DotNetAttributeInfo(item.CustomAttributeData, item.Parent)
            : item.GenericTypeInfo != null ? new DotNetTypeInfo(item.GenericTypeInfo.TypeInfo, item.GenericTypeInfo.GenericTypeStructValue == DotNetTypeInfoProviderGenericTypeStructValue.GenericTypeParameter ? IO.Reflection.DotNetItemType.GenericParameter : IO.Reflection.DotNetItemType.GenericArgument, false, item.Parent)
            : (IBrowsableObjectInfo)(item.TypeInfoItemProvider == null ? throw SelectorDictionary.GetInvalidItemException()
            : new DotNetTypeInfo(item.TypeInfoItemProvider.TypeInfo, item.TypeInfoItemProvider.ItemType, true, item.Parent));

        protected override Converter<DotNetTypeInfoItemProvider, IBrowsableObjectInfo> DefaultActionOverride => Convert;

        public DotNetTypeInfoSelectorDictionary() { /* Left empty. */ }
    }
}
