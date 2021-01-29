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
using System.Reflection;

using WinCopies.IO.ObjectModel;
using WinCopies.IO.Reflection;

namespace WinCopies.IO.AbstractionInterop.Reflection
{
    public interface ITypeInfoItemProvider
    {
        TypeInfoItemProvider TypeInfoItemProvider { get; }
    }

    public class TypeInfoItemProvider
    {
        public TypeInfo TypeInfo { get; }

        public DotNetItemType ItemType { get; }

        public TypeInfoItemProvider(in TypeInfo typeInfo, in DotNetItemType itemType)
        {
            TypeInfo = typeInfo;

            ItemType = itemType;
        }

        public TypeInfoItemProvider(in Type type, in DotNetItemType itemType) : this(type.GetTypeInfo(), itemType)
        {
            // Left empty.
        }
    }

    public class MemberInfoItemProvider
    {
        public MemberInfo MemberInfo { get; }

        public DotNetItemType ItemType { get; }

        public MemberInfoItemProvider(in MemberInfo memberInfo, in DotNetItemType itemType)
        {
            MemberInfo = memberInfo;

            ItemType = itemType;
        }
    }

    public class ParameterInfoItemProvider
    {
        public ParameterInfo ParameterInfo { get; }

        public bool IsReturnParameter { get; }

        public ParameterInfoItemProvider(in ParameterInfo parameterInfo, in bool isReturnParameter)
        {
            ParameterInfo = parameterInfo;

            IsReturnParameter = isReturnParameter;
        }
    }

    public class DotNetNamespaceInfoItemProvider : ITypeInfoItemProvider
    {
        public string NamespaceName { get; }

        public TypeInfoItemProvider TypeInfoItemProvider { get; }

        public IBrowsableObjectInfo Parent { get; }

        private DotNetNamespaceInfoItemProvider(in IBrowsableObjectInfo parent) => Parent = parent;

        public DotNetNamespaceInfoItemProvider(in string _namespace, in IBrowsableObjectInfo parent) : this(parent) => NamespaceName = _namespace;

        public DotNetNamespaceInfoItemProvider(in TypeInfoItemProvider typeInfoItemProvider, in IBrowsableObjectInfo parent) : this(parent) => TypeInfoItemProvider = typeInfoItemProvider;
    }
}
