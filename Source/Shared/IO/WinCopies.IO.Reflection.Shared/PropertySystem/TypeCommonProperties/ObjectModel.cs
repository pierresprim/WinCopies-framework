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

using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.IO.PropertySystem;

using static WinCopies.IO.IOHelper;
using static WinCopies.IO.Reflection.ReflectionHelper;

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

        public abstract AccessModifier AccessModifier { get; }

        protected DotNetTypeOrMemberInfoProperties(in T dotNetItemInfo, in DotNetItemType itemType) : base(dotNetItemInfo, itemType)
        {
            // Left empty.
        }
    }

    public class DotNetFieldItemInfoProperties<T> : DotNetTypeOrMemberInfoProperties<T>, IDotNetTypeOrMemberInfoProperties where T : IDotNetMemberInfoBase
    {
        private readonly FieldInfo _field;

        public override Type DeclaringType => _field.DeclaringType;

        public override AccessModifier AccessModifier => GetAccessModifier(_field);

        public DotNetFieldItemInfoProperties(in T dotNetItemInfo) : base(dotNetItemInfo, DotNetItemType.Field) => _field = dotNetItemInfo.InnerObject as FieldInfo ?? throw GetInvalidInnerObjectException(nameof(FieldInfo), nameof(dotNetItemInfo));
    }

    public abstract class DotNetTypeOrMethodItemInfoProperties<T> : DotNetTypeOrMemberInfoProperties<T>, IDotNetTypeOrMethodItemInfoProperties where T : IDotNetItemInfo
    {
        public abstract bool IsAbstract { get; }

        public abstract bool IsSealed { get; }

        protected DotNetTypeOrMethodItemInfoProperties(in T dotNetItemInfo, in DotNetItemType itemType) : base(dotNetItemInfo, itemType)
        {
            // Left empty.
        }
    }

    public class DotNetPropertyOrMethodItemInfoProperties<T> : DotNetTypeOrMethodItemInfoProperties<T>, IDotNetMethodItemInfoProperties where T : IDotNetMemberInfoBase
    {
        private readonly MethodBase _method;
        private AccessModifier? _accessModifier;

        public override Type DeclaringType => _method.DeclaringType;

        public override AccessModifier AccessModifier => _accessModifier
#if CS8
            ??=
#else
            ?? (_accessModifier =
#endif
            GetAccessModifier(_method)
#if !CS8
            ).Value
#endif
            ;

        public override bool IsAbstract => _method.IsAbstract;

        public override bool IsSealed => _method.IsFinal;

        public bool IsPropertyMethod { get; }

        private DotNetPropertyOrMethodItemInfoProperties(in T dotNetItemInfo, in DotNetItemType itemType, in bool isPropertyMethod) : base(dotNetItemInfo, itemType) => IsPropertyMethod = isPropertyMethod;

        public static DotNetPropertyOrMethodItemInfoProperties<T> From(in T dotNetItemInfo) => dotNetItemInfo.Parent?.InnerObject is PropertyInfo
            ? dotNetItemInfo.InnerObject is MethodInfo ? new DotNetPropertyOrMethodItemInfoProperties<T>(dotNetItemInfo, DotNetItemType.Method, true) : throw GetInvalidInnerObjectException(nameof(MethodInfo), nameof(dotNetItemInfo))
            : new DotNetPropertyOrMethodItemInfoProperties<T>(dotNetItemInfo,
                dotNetItemInfo.InnerObject is MethodInfo ? DotNetItemType.Method
                : dotNetItemInfo.InnerObject is PropertyInfo ? DotNetItemType.Property
                : throw new ArgumentException("Invalid inner object.", nameof(dotNetItemInfo)), false);
    }

    public class DotNetTypeInfoProperties<T> : DotNetTypeOrMethodItemInfoProperties<T>, IDotNetTypeInfoProperties where T : IDotNetTypeInfoBase
    {
        private AccessModifier? _accessModifier;

        public override AccessModifier AccessModifier => _accessModifier
#if CS8
            ??=
#else
            ?? (_accessModifier =
#endif
            GetAccessModifier(InnerObject.InnerObject)
#if !CS8
            ).Value
#endif
            ;

        public bool? IsRootType { get; }

        public override bool IsAbstract => InnerObject.InnerObject.IsAbstract;

        public override bool IsSealed => InnerObject.InnerObject.IsSealed;

        public override Type DeclaringType => InnerObject.InnerObject.DeclaringType;

        public DotNetTypeInfoProperties(in T dotNetTypeInfo, in DotNetItemType itemType, bool? isRootType) : base(dotNetTypeInfo, itemType) => IsRootType = isRootType;
    }

    public class DotNetParameterInfoProperties<T> : DotNetItemInfoProperties<T>, IDotNetParameterInfoProperties where T : IDotNetParameterInfoBase
    {
        public bool IsReturn { get; }

        public DotNetParameterInfoProperties(in T dotNetParameterInfo, in bool isReturn) : base(dotNetParameterInfo, DotNetItemType.Parameter) => IsReturn = isReturn;
    }
}
