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
using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.IO.PropertySystem;

namespace WinCopies.IO.Reflection.PropertySystem
{
    public class DotNetItemInfoProperties<T> : BrowsableObjectInfoProperties<T>, IDotNetItemInfoProperties where T : IDotNetItemInfo
    {
        public DotNetItemType ItemType { get; }

        public DotNetItemInfoProperties(in T dotNetItemInfo, in DotNetItemType itemType) : base(dotNetItemInfo) => ItemType = itemType;
    }

    public abstract class DotNetTypeOrMemberInfoProperties<T> : DotNetItemInfoProperties<T>, IDotNetTypeOrMemberInfoProperties where T : IDotNetItemInfo
    {
        public abstract Type DeclaringType { get; }

        protected DotNetTypeOrMemberInfoProperties(in T dotNetItemInfo, in DotNetItemType itemType) : base(dotNetItemInfo, itemType)
        {
            // Left empty.
        }
    }

    public abstract class DotNetTypeOrMethodItemInfoProperties<T> : DotNetTypeOrMemberInfoProperties<T>, IDotNetTypeOrMethodItemInfoProperties where T : IDotNetItemInfo
    {
        public abstract bool IsAbstract { get; }

        public abstract bool IsSealed { get; }

        public abstract IReadOnlyList<Type> GenericTypeParameters { get; }

        public abstract IReadOnlyList<Type> GenericTypeArguments { get; }

        protected DotNetTypeOrMethodItemInfoProperties(in T dotNetItemInfo, in DotNetItemType itemType) : base(dotNetItemInfo, itemType)
        {
            // Left empty.
        }
    }

    public class DotNetTypeInfoProperties<T> : DotNetTypeOrMethodItemInfoProperties<T>, IDotNetTypeInfoProperties where T : IDotNetTypeInfo
    {
        private AccessModifier? _accessModifier;

        public AccessModifier AccessModifier
        {
            get
            {
                if (!_accessModifier.HasValue)
                {
                    TypeInfo t = BrowsableObjectInfo.EncapsulatedObject;

                    if (t.IsPublic || t.IsNestedPublic)

                        _accessModifier = AccessModifier.Public;

                    else if (t.IsNestedFamily)

                        _accessModifier = AccessModifier.Protected;

                    else if (t.IsNestedFamORAssem)

                        _accessModifier = AccessModifier.ProtectedInternal;

                    else if (t.IsNestedAssembly)

                        _accessModifier = AccessModifier.Internal;

                    else if (t.IsNestedFamANDAssem)

                        _accessModifier = AccessModifier.PrivateProtected;

                    else if (t.IsNestedPrivate)

                        _accessModifier = AccessModifier.Private;

                    else

                        _accessModifier = (AccessModifier)0;
                }

                return _accessModifier.Value;
            }
        }

        public bool? IsRootType { get; } 

        public override bool IsAbstract => BrowsableObjectInfo.EncapsulatedObject.IsAbstract;

        public override bool IsSealed => BrowsableObjectInfo.EncapsulatedObject.IsSealed;

        public override IReadOnlyList<Type> GenericTypeParameters => new CountableEnumerableArray<Type>(BrowsableObjectInfo.EncapsulatedObject.GenericTypeParameters);

        public override IReadOnlyList<Type> GenericTypeArguments => new CountableEnumerableArray<Type>(BrowsableObjectInfo.EncapsulatedObject.GenericTypeArguments);

        public override Type DeclaringType => BrowsableObjectInfo.EncapsulatedObject.DeclaringType;

        public DotNetTypeInfoProperties(in T dotNetTypeInfo, in DotNetItemType itemType, bool? isRootType) : base(dotNetTypeInfo, itemType) => IsRootType = isRootType;
    }
}
