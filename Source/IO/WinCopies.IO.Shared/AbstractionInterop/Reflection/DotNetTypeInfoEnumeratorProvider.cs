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

using WinCopies.IO.ObjectModel;

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

        public Type Type { get; }

        public DotNetTypeInfoProviderGenericTypeStruct(in DotNetTypeInfoProviderGenericTypeStructValue genericTypeStructValue, in Type type)
        {
            GenericTypeStructValue = genericTypeStructValue;

            Type = type;
        }
    }

    public class DotNetTypeInfoItemProvider : ITypeInfoItemProvider
    {
        #region Properties
        public TypeInfoItemProvider TypeInfoItemProvider { get; }

        public MemberInfoItemProvider MemberInfoItemProvider { get; }

        public CustomAttributeData CustomAttributeData { get; }

        public DotNetTypeInfoProviderGenericTypeStruct GenericTypeInfo { get; }

        public IBrowsableObjectInfo Parent { get; }
        #endregion

        #region Constructors
        private DotNetTypeInfoItemProvider(in IBrowsableObjectInfo parent) => Parent = parent;

        public DotNetTypeInfoItemProvider(in TypeInfoItemProvider typeInfoItemProvider, in IBrowsableObjectInfo parent) : this(parent) => TypeInfoItemProvider = typeInfoItemProvider;

        public DotNetTypeInfoItemProvider(in MemberInfoItemProvider memberInfoItemProvider, in IBrowsableObjectInfo parent) : this(parent) => MemberInfoItemProvider = memberInfoItemProvider;

        public DotNetTypeInfoItemProvider(in CustomAttributeData customAttributeData, in IBrowsableObjectInfo parent) : this(parent) => CustomAttributeData = customAttributeData;

        public DotNetTypeInfoItemProvider(in DotNetTypeInfoProviderGenericTypeStruct genericTypeInfo, in IBrowsableObjectInfo parent) : this(parent) => GenericTypeInfo = genericTypeInfo;
        #endregion
    }
}
