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

namespace WinCopies.IO.Reflection
{
    public struct DotNetMemberInfoEnumeratorStruct
    {
        public ParameterInfo ParameterInfo { get; }

        public Type ReturnType { get; }

        public CustomAttributeData CustomAttributeData { get; }

        public DotNetTypeEnumeratorGenericTypeStruct? GenericTypeInfo { get; }

        public DotNetMemberInfoEnumeratorStruct(in ParameterInfo parameterInfo)
        {
            ParameterInfo = parameterInfo;

            ReturnType = null;

            CustomAttributeData = null;

            GenericTypeInfo = null;
        }

        public DotNetMemberInfoEnumeratorStruct(in CustomAttributeData customAttributeData)
        {
            CustomAttributeData = customAttributeData;

            ParameterInfo = null;

            ReturnType = null;

            GenericTypeInfo = null;
        }

        public DotNetMemberInfoEnumeratorStruct(in Type returnType)
        {
            ReturnType = returnType;

            ParameterInfo = null;

            CustomAttributeData = null;

            GenericTypeInfo = null;
        }

        public DotNetMemberInfoEnumeratorStruct(in DotNetTypeEnumeratorGenericTypeStruct genericTypeInfo)
        {
            GenericTypeInfo = genericTypeInfo;

            ParameterInfo = null;

            ReturnType = null;

            CustomAttributeData = null;
        }
    }

    public class DotNetMemberInfoEnumerator : Enumerator<IEnumerator<IDotNetItemInfo>, IDotNetItemInfo>
    {
        public DotNetMemberInfoEnumerator(IDotNetMemberInfo dotNetMemberInfo, IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetMemberInfoEnumeratorStruct> func)
        {
            var dic = new Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>>();

                void add(in DotNetItemType itemType, in IEnumerable<IDotNetItemInfo> enumerable) => dic.Add(itemType, enumerable.GetEnumerator());

            foreach (DotNetItemType itemType in typesToEnumerate)

                switch (itemType)
                {
                    case DotNetItemType.Parameter:

                        switch (dotNetMemberInfo.MemberInfo)
                        {
                            case PropertyInfo propertyInfo:

                                add(DotNetItemType.Parameter, propertyInfo.GetIndexParameters().WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetParameterInfo(p)));

                                break;

                            case MethodInfo methodInfo:

                                add(DotNetItemType.Parameter, methodInfo.GetParameters().WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetParameterInfo(p)));

                                break;

                            case ConstructorInfo constructorInfo:

                                add(DotNetItemType.Parameter, constructorInfo.GetParameters().WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetParameterInfo(p)));

                                break;
                        }

                        break;

                    case DotNetItemType.ReturnParameter:

                        switch (dotNetMemberInfo.MemberInfo)
                        {
                            case PropertyInfo _propertyInfo:

                                add(DotNetItemType.ReturnParameter, new Type[] { _propertyInfo.PropertyType }.WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetParameterInfo(p)).GetEnumerator());

                                break;

                            case MethodInfo _methodInfo:

                                add(DotNetItemType.ReturnParameter, new Type[] { _methodInfo.ReturnType }.WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetParameterInfo(p)));

                                break;
                        }

                        break;

                    case DotNetItemType.Attribute:

                        add(DotNetItemType.Attribute, dotNetMemberInfo.MemberInfo.CustomAttributes.WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetAttributeInfo(p, dotNetMemberInfo)));

                        break;

                    case DotNetItemType.GenericParameter:
                    case DotNetItemType.GenericArgument:

                        if (dotNetMemberInfo.MemberInfo is MethodInfo __methodInfo)

                            add(itemType, __methodInfo.GetGenericArguments().WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(new DotNetTypeEnumeratorGenericTypeStruct(__methodInfo.IsGenericMethodDefinition ? DotNetTypeEnumeratorGenericTypeStructValue.GenericTypeParameter : DotNetTypeEnumeratorGenericTypeStructValue.GenericTypeArgument, p)))).Select(p => new DotNetTypeInfo(p.GetTypeInfo(), itemType, dotNetMemberInfo)));

                        break;

                    default:

                        throw DotNetEnumeration.GetInvalidEnumArgumentException(nameof(typesToEnumerate), itemType);
                }
        }

        protected override bool MoveNextOverride() => throw new NotImplementedException();
    }
}
