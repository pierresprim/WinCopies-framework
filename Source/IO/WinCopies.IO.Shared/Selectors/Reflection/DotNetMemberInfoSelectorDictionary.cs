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
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.ObjectModel.Reflection;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Selectors.Reflection
{
    public class DotNetMemberInfoSelectorDictionary : BrowsableObjectInfoSelectorDictionary<DotNetMemberInfoItemProvider>
    {
        public static IBrowsableObjectInfo Convert(DotNetMemberInfoItemProvider item)
        {
            if ((item ?? throw GetArgumentNullException(nameof(item))).ParameterInfoItemProvider != null)

                return new DotNetParameterInfo(item.ParameterInfoItemProvider.ParameterInfo, IO.Reflection.DotNetItemType.Parameter, item.ParameterInfoItemProvider.IsReturnParameter, item.Parent);

            if (item.ReturnType != null)

                return new DotNetTypeInfo(item.ReturnType, DotNetEnumeration.GetTypeItemType(item.ReturnType), false, item.Parent);

            if (item.MethodInfo != null)

                return new DotNetMemberInfo(item.MethodInfo, IO.Reflection.DotNetItemType.Method, true, item.Parent);

            if (item.GenericTypeInfo != null)

                return new DotNetGenericItemInfo(item.GenericTypeInfo.Type, item.GenericTypeInfo.GenericTypeStructValue, item.Parent);

            if (item.CustomAttributeData != null)

                return new DotNetAttributeInfo(item.CustomAttributeData, item.Parent);

            throw BrowsableObjectInfoSelectorDictionary.GetInvalidItemProviderException();
        }

        protected override Converter<DotNetMemberInfoItemProvider, IBrowsableObjectInfo> DefaultSelectorOverride => Convert;

        public DotNetMemberInfoSelectorDictionary() { /* Left empty. */ }
    }
}
