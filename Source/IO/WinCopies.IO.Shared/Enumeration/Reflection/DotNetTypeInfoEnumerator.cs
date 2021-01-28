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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.IO.Reflection;
using WinCopies.Linq;

using static WinCopies.
#if !WinCopies3
    Util.Util
#else
    ThrowHelper
#endif
    ;
using static WinCopies.UtilHelpers;

namespace WinCopies.IO.Enumeration.Reflection
{
    public enum DotNetTypeInfoEnumeratorGenericTypeStructValue : byte
    {
        GenericTypeParameter = 0,

        GenericTypeArgument = 1
    }

    public struct DotNetTypeInfoEnumeratorGenericTypeStruct
    {
        public DotNetTypeInfoEnumeratorGenericTypeStructValue GenericTypeStructValue { get; }

        public Type Type { get; }

        public DotNetTypeInfoEnumeratorGenericTypeStruct(in DotNetTypeInfoEnumeratorGenericTypeStructValue genericTypeStructValue, in Type type)
        {
            GenericTypeStructValue = genericTypeStructValue;

            Type = type;
        }
    }

    public struct DotNetTypeInfoEnumeratorStruct
    {
        #region Properties
        public TypeInfo TypeInfo { get; }

        public MemberInfo MemberInfo { get; }

        public CustomAttributeData CustomAttributeData { get; }

        public DotNetTypeInfoEnumeratorGenericTypeStruct? GenericTypeInfo { get; }
        #endregion

        #region Constructors
        public DotNetTypeInfoEnumeratorStruct(TypeInfo typeInfo)
        {
            TypeInfo = typeInfo;

            MemberInfo = null;

            CustomAttributeData = null;

            GenericTypeInfo = null;
        }

        public DotNetTypeInfoEnumeratorStruct(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;

            TypeInfo = null;

            CustomAttributeData = null;

            GenericTypeInfo = null;
        }

        public DotNetTypeInfoEnumeratorStruct(CustomAttributeData customAttributeData)
        {
            CustomAttributeData = customAttributeData;

            TypeInfo = null;

            MemberInfo = null;

            GenericTypeInfo = null;
        }

        public DotNetTypeInfoEnumeratorStruct(DotNetTypeInfoEnumeratorGenericTypeStruct genericTypeInfo)
        {
            GenericTypeInfo = genericTypeInfo;

            TypeInfo = null;

            MemberInfo = null;

            CustomAttributeData = null;
        }
        #endregion
    }

    public sealed class DotNetTypeInfoEnumerator : IEnumerator<IDotNetItemInfo>
    {
        private IDotNetItemInfo _current;
        private DotNetEnumerationMoveNext<IDotNetItemInfo> _moveNext;
        private bool _isCompleted = false;

        public IDotNetItemInfo Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

        object IEnumerator.Current => Current;

        public bool IsDisposed { get; private set; }

        private DotNetTypeInfoEnumerator(Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>> dic) => _moveNext = new DotNetEnumerationMoveNext<IDotNetItemInfo>(dic.Values.GetEnumerator());

        public static DotNetTypeInfoEnumerator From(IDotNetTypeInfo dotNetTypeInfo, IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetTypeInfoEnumeratorStruct> func)
        {
            ThrowIfNull(dotNetTypeInfo, nameof(dotNetTypeInfo));
            ThrowIfNull(typesToEnumerate, nameof(typesToEnumerate));

#if CS7
            if (func == null)
#endif

            func
#if CS7
                =
#else
                ??=
#endif
                GetCommonPredicate<DotNetTypeInfoEnumeratorStruct>();

            IEnumerable<TypeInfo> enumerable = dotNetTypeInfo.EncapsulatedObject.DeclaredNestedTypes;

            var dic = new Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>>();

            void addTypeInfoEnumerator(DotNetItemType dotNetItemType, Predicate<TypeInfo> __func) => dic.Add(dotNetItemType, DotNetEnumeration.GetDotNetItemInfoEnumerator(enumerable, dotNetItemType, false, dotNetTypeInfo, t => __func(t) && func(new DotNetTypeInfoEnumeratorStruct(t))));

            void addMemberInfoEnumerator(DotNetItemType dotNetItemType, IEnumerable<MemberInfo> _enumerable) => dic.Add(dotNetItemType, _enumerable.WherePredicate(f => func(new DotNetTypeInfoEnumeratorStruct(f))).Select(f => new DotNetMemberInfo(f, dotNetItemType, dotNetTypeInfo)).GetEnumerator());

            foreach (DotNetItemType typeToEnumerate in typesToEnumerate)
            {
                switch (typeToEnumerate)
                {
                    case DotNetItemType.Attribute:

                        dic.Add(DotNetItemType.Attribute, dotNetTypeInfo.EncapsulatedObject.CustomAttributes.WherePredicate(a => func(new DotNetTypeInfoEnumeratorStruct(a))).Select(a => new DotNetAttributeInfo(a, dotNetTypeInfo)).GetEnumerator());

                        break;

                    case DotNetItemType.Field:

                        addMemberInfoEnumerator(DotNetItemType.Field, dotNetTypeInfo.EncapsulatedObject.DeclaredFields);

                        break;

                    case DotNetItemType.Property:

                        addMemberInfoEnumerator(DotNetItemType.Property, dotNetTypeInfo.EncapsulatedObject.DeclaredProperties);

                        break;

                    case DotNetItemType.Constructor:

                        addMemberInfoEnumerator(DotNetItemType.Constructor, dotNetTypeInfo.EncapsulatedObject.DeclaredConstructors);

                        break;

                    case DotNetItemType.Method:

                        addMemberInfoEnumerator(DotNetItemType.Method, dotNetTypeInfo.EncapsulatedObject.DeclaredMethods);

                        break;

                    case DotNetItemType.ImplementedInterface:

                        dic.Add(DotNetItemType.ImplementedInterface, dotNetTypeInfo.EncapsulatedObject.ImplementedInterfaces.WherePredicate(t => func(new DotNetTypeInfoEnumeratorStruct(t))).Select(t => new DotNetTypeInfo(t.GetTypeInfo(), DotNetItemType.ImplementedInterface, null, dotNetTypeInfo)).GetEnumerator());

                        break;

                    case DotNetItemType.GenericParameter:
                    case DotNetItemType.GenericArgument:

                        dic.Add(typeToEnumerate, dotNetTypeInfo.EncapsulatedObject.GenericTypeParameters.WherePredicate(p => func(new DotNetTypeInfoEnumeratorStruct(new DotNetTypeInfoEnumeratorGenericTypeStruct(typeToEnumerate == DotNetItemType.GenericParameter ? DotNetTypeInfoEnumeratorGenericTypeStructValue.GenericTypeParameter : DotNetTypeInfoEnumeratorGenericTypeStructValue.GenericTypeArgument, p)))).Select(p => new DotNetTypeInfo(p.GetTypeInfo(), typeToEnumerate, null, dotNetTypeInfo)).GetEnumerator());

                        break;

                    default: // The invalid enum arguments are checked in the DotNetEnumeration.GetTypeInfoPredicate method.

                        addTypeInfoEnumerator(typeToEnumerate, DotNetEnumeration.GetTypeInfoPredicate(typeToEnumerate, nameof(typesToEnumerate)));

                        break;
                }
            }

            return new DotNetTypeInfoEnumerator(dic);
        }

        public bool MoveNext()
        {
            if (IsDisposed)

                throw GetExceptionForDispose(false);

            if (_isCompleted)

                return false;

            if (_moveNext.MoveNext())
            {
                _current = _moveNext.Current;

                return true;
            }

            _isCompleted = true;

            return false;
        }

        public void Reset() => throw new NotSupportedException("THis enumerator does not support reset.");

        public void Dispose()
        {
            _current = null;

            _moveNext = null;

            IsDisposed = true;
        }
    }
}
