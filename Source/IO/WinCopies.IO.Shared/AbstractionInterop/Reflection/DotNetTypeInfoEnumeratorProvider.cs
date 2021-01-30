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
using System.Reflection;

using WinCopies.IO.ObjectModel.Reflection;

namespace WinCopies.IO.AbstractionInterop.Reflection
{
    public enum DotNetTypeInfoProviderGenericTypeStructValue : byte
    {
        GenericTypeParameter = 0,

        GenericTypeArgument = 1
    }

    public class DotNetTypeInfoProviderGenericTypeStruct
    {
        public DotNetTypeInfoProviderGenericTypeStructValue GenericTypeStructValue { get; }

        public TypeInfo TypeInfo { get; }

        public DotNetTypeInfoProviderGenericTypeStruct(in DotNetTypeInfoProviderGenericTypeStructValue genericTypeStructValue, in TypeInfo typeInfo)
        {
            GenericTypeStructValue = genericTypeStructValue;

            TypeInfo = typeInfo;
        }

        public DotNetTypeInfoProviderGenericTypeStruct(in DotNetTypeInfoProviderGenericTypeStructValue genericTypeStructValue, in Type type) : this(genericTypeStructValue, type.GetTypeInfo())
        {
            // Left empty.
        }
    }

    public class DotNetTypeInfoItemProvider : ITypeInfoItemProvider
    {
        #region Properties
        public TypeInfoItemProvider TypeInfoItemProvider { get; }

        public MemberInfoItemProvider MemberInfoItemProvider { get; }

        public CustomAttributeData CustomAttributeData { get; }

        public DotNetTypeInfoProviderGenericTypeStruct GenericTypeInfo { get; }

        public IDotNetItemInfo Parent { get; }
        #endregion

        #region Constructors
        private DotNetTypeInfoItemProvider(in IDotNetItemInfo parent) => Parent = parent;

        public DotNetTypeInfoItemProvider(in TypeInfoItemProvider typeInfoItemProvider, in IDotNetItemInfo parent) : this(parent) => TypeInfoItemProvider = typeInfoItemProvider;

        public DotNetTypeInfoItemProvider(in MemberInfoItemProvider memberInfoItemProvider, in IDotNetItemInfo parent) : this(parent) => MemberInfoItemProvider = memberInfoItemProvider;

        public DotNetTypeInfoItemProvider(in CustomAttributeData customAttributeData, in IDotNetItemInfo parent) : this(parent) => CustomAttributeData = customAttributeData;

        public DotNetTypeInfoItemProvider(in DotNetTypeInfoProviderGenericTypeStruct genericTypeInfo, in IDotNetItemInfo parent) : this(parent) => GenericTypeInfo = genericTypeInfo;
        #endregion
    }
}
