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

using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.IO.Reflection;
using WinCopies.Linq;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Enumeration.Reflection
{
    //    public class DotNetEnumerator<T> : Enumerator<IEnumerator<T>, T> where T : IDotNetItemInfo
    //    {
    //        private IEnumerator<T> _currentEnumerator;

    //#if WinCopies3
    //        private T _current;

    //        protected override T CurrentOverride => _current;

    //        public override bool? IsResetSupported => null;
    //#endif

    //        public DotNetEnumerator(System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<T>> enumerable) : this(enumerable.Select(_enumerable => _enumerable.GetEnumerator())) { }

    //        public DotNetEnumerator(System.Collections.Generic.IEnumerable<IEnumerator<T>> enumerable) : base(enumerable) { }

    //        protected override void ResetOverride()
    //        {
    //            base.ResetOverride();

    //            _currentEnumerator = null;
    //        }

    //        protected override bool MoveNextOverride()
    //        {
    //            bool _moveNext()
    //            {
    //                while (InnerEnumerator.MoveNext())
    //                {
    //                    _currentEnumerator = InnerEnumerator.Current;

    //                    if (moveNext())
    //                    {
    //#if !WinCopies3
    //Current
    //#else
    //                        _current
    //#endif
    //                            = _currentEnumerator.Current;

    //                        return true;
    //                    }
    //                }

    //                return false;
    //            }

    //            bool moveNext() => _currentEnumerator.MoveNext();

    //            return _currentEnumerator == null ? _moveNext() : moveNext() || _moveNext();
    //        }

    //        protected override void
    //#if !WinCopies3
    //            Dispose(bool disposing)
    //        {
    //            base.Dispose(disposing);

    //            if (disposing)
    //#else
    //            DisposeManaged()
    //        {
    //            base.DisposeManaged();
    //#endif
    //            _currentEnumerator = null;
    //        }
    //    }

    public static class DotNetMemberInfoEnumeration
    {
        public static System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider> From(IDotNetMemberInfoBase dotNetMemberInfo, in System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, in Predicate<DotNetMemberInfoItemProvider> func)
        {
            MemberInfo memberInfo = (dotNetMemberInfo ?? throw GetArgumentNullException(nameof(dotNetMemberInfo))).InnerObject;

            System.Collections.Generic.IEnumerator<DotNetItemType> typesToEnumerateEnumerator = (typesToEnumerate ?? DotNetMemberInfo.GetDefaultItemTypes()).GetEnumerator();

            EnumerableHelper<System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider>>.IEnumerableQueue queue = EnumerableHelper<System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider>>.GetEnumerableQueue();

            void add<T>(in System.Collections.Generic.IEnumerable<T> _enumerable, in Converter<T, DotNetMemberInfoItemProvider> selector)
            {
                if (_enumerable != null)

                    queue.Enqueue(_enumerable.SelectConverter(selector));
            }

            void addGenericItems(DotNetTypeInfoProviderGenericTypeStructValue itemType, in System.Collections.Generic.IEnumerable<Type> types) => add(types, item => new DotNetMemberInfoItemProvider(new DotNetTypeInfoProviderGenericTypeStruct(itemType, item), dotNetMemberInfo));

            DotNetMemberInfoItemProvider select(in ParameterInfo p, in bool isReturnParameter) => new DotNetMemberInfoItemProvider(new ParameterInfoItemProvider(p, isReturnParameter), dotNetMemberInfo);

            void addParameters(in MethodBase method)
            {
                add(method.GetParameters(), p => select(p, false));

                if (method is MethodInfo methodInfo && methodInfo.ReturnParameter != null)

                    queue.Enqueue(new DotNetMemberInfoItemProvider[] { select(methodInfo.ReturnParameter, true) });
            }

            void addPropertyMethod(in EnumerableHelper<MethodInfo>.IEnumerableQueue _queue, in MethodInfo methodInfo)
            {
                if (methodInfo != null)

                    _queue.Enqueue(methodInfo);
            }

            void addGenericParameters()
            {
                if (memberInfo is MethodInfo methodInfo)
                {
                    MethodInfo _methodInfo = methodInfo.GetGenericMethodDefinition();

                    if (_methodInfo.IsGenericMethodDefinition && _methodInfo.ContainsGenericParameters)

                        addGenericItems(DotNetTypeInfoProviderGenericTypeStructValue.GenericTypeParameter, _methodInfo.GetGenericArguments());
                }
            }

            void addGenericArguments()
            {
                if (memberInfo is MethodInfo methodInfo)
                {
                    if (methodInfo.IsGenericMethod && methodInfo.ContainsGenericParameters)

                        addGenericItems(DotNetTypeInfoProviderGenericTypeStructValue.GenericTypeParameter, methodInfo.GetGenericArguments());
                }
            }

            while (typesToEnumerateEnumerator.MoveNext())

                switch (typesToEnumerateEnumerator.Current)
                {
                    case DotNetItemType.Parameter:

                        switch (memberInfo)
                        {
                            case PropertyInfo propertyInfo:

                                add(propertyInfo.GetIndexParameters(), p => select(p, false));

                                if (propertyInfo.PropertyType != null)

                                    queue.Enqueue(new DotNetMemberInfoItemProvider[] { new DotNetMemberInfoItemProvider(propertyInfo.PropertyType, dotNetMemberInfo) });

                                break;

                            case MethodBase method:

                                addParameters(method);

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

                        add(memberInfo.CustomAttributes, a => new DotNetMemberInfoItemProvider(a, dotNetMemberInfo));

                        break;

                    case DotNetItemType.GenericParameter:

                        addGenericParameters();

                        break;

                    case DotNetItemType.GenericArgument:

                        addGenericArguments();

                        break;

                    case DotNetItemType.Method:

                        if (memberInfo is PropertyInfo __propertyInfo)
                        {
                            EnumerableHelper<MethodInfo>.IEnumerableQueue _queue = EnumerableHelper<MethodInfo>.GetEnumerableQueue();

                            addPropertyMethod(_queue, __propertyInfo.GetMethod);
                            addPropertyMethod(_queue, __propertyInfo.SetMethod);

                            add(_queue, m => new DotNetMemberInfoItemProvider(m, dotNetMemberInfo));
                        }

                        break;
                }

            typesToEnumerateEnumerator.Dispose();

            System.Collections.Generic.IEnumerable<DotNetMemberInfoItemProvider> enumerable = queue.Merge<DotNetMemberInfoItemProvider>();

            return func == null ? enumerable : enumerable.WherePredicate(func);
        }
    }
}
