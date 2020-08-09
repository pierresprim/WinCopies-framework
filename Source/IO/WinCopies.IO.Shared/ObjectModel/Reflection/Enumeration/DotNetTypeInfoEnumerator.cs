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
using System.Linq;
using System.Reflection;

using WinCopies.Collections;
using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.Linq;

using static WinCopies.Util.Util;

namespace WinCopies.IO.Reflection
{
    public struct DotNetTypeEnumeratorStruct
    {
        public TypeInfo TypeInfo { get; }

        public MemberInfo MemberInfo { get; }

        public CustomAttributeData CustomAttributeData { get; }

        public DotNetTypeEnumeratorStruct(TypeInfo typeInfo)
        {
            TypeInfo = typeInfo;

            MemberInfo = null;

            CustomAttributeData = null;
        }

        public DotNetTypeEnumeratorStruct(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;

            TypeInfo = null;

            CustomAttributeData = null;
        }

        public DotNetTypeEnumeratorStruct(CustomAttributeData customAttributeData)
        {
            CustomAttributeData = customAttributeData;

            TypeInfo = null;

            MemberInfo = null;
        }
    }

    public sealed class DotNetTypeInfoEnumerator : IEnumerator<IDotNetItemInfo>
    {
        private Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>> _dic;
        private IDotNetItemInfo _current;
        private bool _isCompleted = false;

        public IDotNetItemInfo Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

        public bool IsDisposed { get; private set; }

        private DotNetTypeInfoEnumerator(Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>> dic)
        {
            _dic = dic;

            _moveNext = new DotNetEnumerationMoveNext<IDotNetItemInfo>(dic.Values.GetEnumerator());
        }

        public static DotNetTypeInfoEnumerator From(IDotNetTypeInfo dotNetTypeInfo, IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetTypeEnumeratorStruct> func)
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
                GetCommonPredicate<DotNetTypeEnumeratorStruct>();

            IEnumerable<TypeInfo> enumerable = dotNetTypeInfo.TypeInfo.DeclaredNestedTypes;

            var dic = new Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>>();

            void addTypeInfoEnumerator(DotNetItemType dotNetItemType, Predicate<TypeInfo> __func) => dic.Add(dotNetItemType, DotNetEnumeration.GetDotNetItemInfoEnumerator(enumerable, t => __func(t) && func(new DotNetTypeEnumeratorStruct(t))));

            void addMemberInfoEnumerator(DotNetItemType dotNetItemType, IEnumerable<MemberInfo> _enumerable) => dic.Add(dotNetItemType, _enumerable.WherePredicate(f => func(new DotNetTypeEnumeratorStruct(f))).Select(f => new DotNetMemberInfo($"{dotNetTypeInfo.Path}{Path.PathSeparator}{f.Name}", dotNetItemType, f, dotNetTypeInfo)).GetEnumerator());

            foreach (DotNetItemType typeToEnumerate in typesToEnumerate)
            {
                switch (typeToEnumerate)
                {
                    case DotNetItemType.Attribute:

                        dic.Add(DotNetItemType.Attribute, dotNetTypeInfo.TypeInfo.CustomAttributes.WherePredicate(a => func(new DotNetTypeEnumeratorStruct(a))).Select(a => new DotNetAttributeInfo($"{dotNetTypeInfo.Path}{Path.PathSeparator}{a.AttributeType.Name}", dotNetTypeInfo, a)).GetEnumerator());

                        break;

                    case DotNetItemType.Field:

                        addMemberInfoEnumerator(DotNetItemType.Field, dotNetTypeInfo.TypeInfo.DeclaredFields);

                        break;

                    case DotNetItemType.Property:

                        addMemberInfoEnumerator(DotNetItemType.Property, dotNetTypeInfo.TypeInfo.DeclaredProperties);

                        break;

                    case DotNetItemType.Constructor:

                        addMemberInfoEnumerator(DotNetItemType.Constructor, dotNetTypeInfo.TypeInfo.DeclaredConstructors);

                        break;

                    case DotNetItemType.Method:

                        addMemberInfoEnumerator(DotNetItemType.Method, dotNetTypeInfo.TypeInfo.DeclaredMethods);

                        break;

                    case DotNetItemType.ImplementedInterface:

                        dic.Add(DotNetItemType.ImplementedInterface, dotNetTypeInfo.TypeInfo.ImplementedInterfaces.WherePredicate(t => func(new DotNetTypeEnumeratorStruct(t))).Select(t => new DotNetTypeInfo($"{dotNetTypeInfo.Path}{Path.PathSeparator}{t.Name}", t.GetTypeInfo(), DotNetItemType.ImplementedInterface, dotNetTypeInfo)).GetEnumerator());

                        break;

                    default:

                        addTypeInfoEnumerator(typeToEnumerate, DotNetEnumeration.GetTypeInfoPredicate(typeToEnumerate, nameof(typesToEnumerate)));

                        break;
                }
            }

            return new DotNetTypeInfoEnumerator(dic);
        }

        private DotNetEnumerationMoveNext<IDotNetItemInfo> _moveNext;

        public bool MoveNext()
        {
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
    }
}
