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

using WinCopies.Collections
#if WinCopies3
    .Generic
#endif
    ;
using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.Linq;

namespace WinCopies.IO.Reflection
{
    public struct DotNetMemberInfoEnumeratorStruct
    {
        #region Properties
        public ParameterInfo ParameterInfo { get; }

        public Type ReturnType { get; }

        public CustomAttributeData CustomAttributeData { get; }

        public DotNetTypeInfoEnumeratorGenericTypeStruct? GenericTypeInfo { get; }
        #endregion

        #region Constructors
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

        public DotNetMemberInfoEnumeratorStruct(in DotNetTypeInfoEnumeratorGenericTypeStruct genericTypeInfo)
        {
            GenericTypeInfo = genericTypeInfo;

            ParameterInfo = null;

            ReturnType = null;

            CustomAttributeData = null;
        }
        #endregion
    }

    public class DotNetEnumerator<T> : Enumerator<IEnumerator<T>, T> where T : IDotNetItemInfo
    {
        private IEnumerator<T> _currentEnumerator;

#if WinCopies3
        private T _current;

        protected override T CurrentOverride => _current;

        public override bool? IsResetSupported => null;
#endif

        public DotNetEnumerator(System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<T>> enumerable) : this(enumerable.Select(_enumerable => _enumerable.GetEnumerator())) { }

        public DotNetEnumerator(System.Collections.Generic.IEnumerable<IEnumerator<T>> enumerable) : base(enumerable) { }

        protected override void ResetOverride()
        {
            base.ResetOverride();

            _currentEnumerator = null;
        }

        protected override bool MoveNextOverride()
        {
            bool _moveNext()
            {
                while (InnerEnumerator.MoveNext())
                {
                    _currentEnumerator = InnerEnumerator.Current;

                    if (moveNext())
                    {
#if !WinCopies3
Current
#else
                        _current
#endif
                            = _currentEnumerator.Current;

                        return true;
                    }
                }

                return false;
            }

            bool moveNext() => _currentEnumerator.MoveNext();

            return _currentEnumerator == null ? _moveNext() : moveNext() || _moveNext();
        }

        protected override void
#if !WinCopies3
            Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
#else
            DisposeManaged()
        {
            base.DisposeManaged();
#endif
            _currentEnumerator = null;
        }
    }

    public static class DotNetMemberInfoEnumerator
    {
        public static DotNetEnumerator<IDotNetItemInfo> From(IDotNetMemberInfo dotNetMemberInfo, System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetMemberInfoEnumeratorStruct> func)
        {
            var dic = new Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>>();

            void add(in DotNetItemType itemType, in System.Collections.Generic.IEnumerable<IDotNetItemInfo> enumerable) => dic.Add(itemType, enumerable.GetEnumerator());

            foreach (DotNetItemType itemType in typesToEnumerate)

                switch (itemType)
                {
                    case DotNetItemType.Parameter:

                        switch (dotNetMemberInfo.EncapsulatedObject)
                        {
                            case PropertyInfo propertyInfo:

                                add(DotNetItemType.Parameter, propertyInfo.GetIndexParameters().WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetParameterInfo(p, DotNetItemType.Parameter, dotNetMemberInfo)));

                                break;

                            case MethodInfo methodInfo:

                                add(DotNetItemType.Parameter, methodInfo.GetParameters().WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetParameterInfo(p, DotNetItemType.Parameter, dotNetMemberInfo)));

                                break;

                            case ConstructorInfo constructorInfo:

                                add(DotNetItemType.Parameter, constructorInfo.GetParameters().WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetParameterInfo(p, DotNetItemType.Parameter, dotNetMemberInfo)));

                                break;
                        }

                        break;

                    //case DotNetItemType.ReturnParameter:

                    //    switch (dotNetMemberInfo.MemberInfo)
                    //    {
                    //        case PropertyInfo _propertyInfo:

                    //            add(DotNetItemType.ReturnParameter, new Type[] { _propertyInfo.PropertyType }.WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetTypeInfo(p.GetTypeInfo(), DotNetItemType.ReturnParameter,  dotNetMemberInfo)));

                    //            break;

                    //        case MethodInfo _methodInfo:

                    //            add(DotNetItemType.ReturnParameter, new Type[] { _methodInfo.ReturnType }.WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetTypeInfo(p.GetTypeInfo(), DotNetItemType.ReturnParameter, dotNetMemberInfo)));

                    //            break;
                    //    }

                    //    break;

                    case DotNetItemType.Attribute:

                        add(DotNetItemType.Attribute, dotNetMemberInfo.EncapsulatedObject.CustomAttributes.WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(p))).Select(p => new DotNetAttributeInfo(p, dotNetMemberInfo)));

                        break;

                    case DotNetItemType.GenericParameter:
                    case DotNetItemType.GenericArgument:

                        if (dotNetMemberInfo.EncapsulatedObject is MethodInfo __methodInfo)

                            add(itemType, __methodInfo.GetGenericArguments().WherePredicate(p => func(new DotNetMemberInfoEnumeratorStruct(new DotNetTypeInfoEnumeratorGenericTypeStruct(__methodInfo.IsGenericMethodDefinition ? DotNetTypeInfoEnumeratorGenericTypeStructValue.GenericTypeParameter : DotNetTypeInfoEnumeratorGenericTypeStructValue.GenericTypeArgument, p)))).Select(p => new DotNetTypeInfo(p.GetTypeInfo(), itemType, null, dotNetMemberInfo)));

                        break;

                    default:

                        throw DotNetEnumeration.GetInvalidEnumArgumentException(nameof(typesToEnumerate), itemType);
                }

            return new DotNetEnumerator<IDotNetItemInfo>(new Enumerable<IEnumerator<IDotNetItemInfo>>(() => dic.Values.GetEnumerator()));
        }
    }
}
