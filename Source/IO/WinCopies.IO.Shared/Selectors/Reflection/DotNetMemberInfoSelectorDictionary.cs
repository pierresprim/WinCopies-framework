﻿/* Copyright © Pierre Sprimont, 2021
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
using System.Reflection;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.ObjectModel.Reflection;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Selectors.Reflection
{
    public class DotNetMemberInfoSelectorDictionary : BrowsableObjectInfoSelectorDictionary<DotNetMemberInfoItemProvider>
    {
        public static IBrowsableObjectInfo Convert(DotNetMemberInfoItemProvider item) => (item ?? throw GetArgumentNullException(nameof(item))).ParameterInfoItemProvider != null ? new DotNetParameterInfo(item.ParameterInfoItemProvider.ParameterInfo, IO.Reflection.DotNetItemType.Parameter, item.ParameterInfoItemProvider.IsReturnParameter, item.Parent)
            : item.ReturnType != null ? new DotNetTypeInfo(item.ReturnType.GetTypeInfo(), DotNetEnumeration.GetTypeItemType(item.ReturnType), false, item.Parent)
            : item.MethodInfo != null ? new DotNetMemberInfo(item.MethodInfo, IO.Reflection.DotNetItemType.Method, true, item.Parent)
            : item.GenericTypeInfo != null ? new DotNetGenericItemInfo(item.GenericTypeInfo.Type, item.GenericTypeInfo.GenericTypeStructValue, item.Parent)
            : item.CustomAttributeData == null ? throw BrowsableObjectInfoSelectorDictionary.GetInvalidItemProviderException()
            : (IBrowsableObjectInfo)new DotNetAttributeInfo(item.CustomAttributeData, item.Parent);

        protected override Converter<DotNetMemberInfoItemProvider, IBrowsableObjectInfo> DefaultSelectorOverride => Convert;

        public DotNetMemberInfoSelectorDictionary() { /* Left empty. */ }
    }
}
