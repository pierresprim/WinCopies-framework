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
    public static class DotNetTypeInfoEnumeration
    {
        public static System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> From(IDotNetTypeInfo dotNetTypeInfo, in System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, in Predicate<DotNetTypeInfoItemProvider> func)
        {
            ThrowIfNull(dotNetTypeInfo, nameof(dotNetTypeInfo));
            ThrowIfNull(typesToEnumerate, nameof(typesToEnumerate));

            EnumerableHelper<System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider>>.IEnumerableQueue queue = EnumerableHelper<System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider>>.GetEnumerableQueue();

            void add<T>(in System.Collections.Generic.IEnumerable<T> _enumerable, in Converter<T, DotNetTypeInfoItemProvider> selector)
            {
                if (_enumerable != null)

                    queue.Enqueue(_enumerable.SelectConverter(selector));
            }

            void addMemberInfoEnumerator(DotNetItemType itemType, in System.Collections.Generic.IEnumerable<MemberInfo> _enumerable) => add(_enumerable, item => new DotNetTypeInfoItemProvider(new MemberInfoItemProvider(item, itemType), dotNetTypeInfo));

            void addGenericItems(DotNetTypeInfoProviderGenericTypeStructValue itemType, in System.Collections.Generic.IEnumerable<Type> types) => add(types, item => new DotNetTypeInfoItemProvider(new DotNetTypeInfoProviderGenericTypeStruct(itemType, item), dotNetTypeInfo));

            DotNetTypeInfoItemProvider select(Type t) => new DotNetTypeInfoItemProvider(new TypeInfoItemProvider(t.GetTypeInfo(), DotNetItemType.BaseTypeOrInterface), dotNetTypeInfo);

            TypeInfo typeInfo = dotNetTypeInfo.InnerObject;

            foreach (DotNetItemType typeToEnumerate in typesToEnumerate)
            {
                switch (typeToEnumerate)
                {
                    case DotNetItemType.Attribute:

                        add(typeInfo.CustomAttributes, a => new DotNetTypeInfoItemProvider(a, dotNetTypeInfo));

                        break;

                    case DotNetItemType.Field:

                        addMemberInfoEnumerator(DotNetItemType.Field, typeInfo.DeclaredFields);

                        break;

                    case DotNetItemType.Property:

                        addMemberInfoEnumerator(DotNetItemType.Property, typeInfo.DeclaredProperties);

                        break;

                    case DotNetItemType.Constructor:

                        addMemberInfoEnumerator(DotNetItemType.Constructor, typeInfo.DeclaredConstructors);

                        break;

                    case DotNetItemType.Method:

                        addMemberInfoEnumerator(DotNetItemType.Method, typeInfo.DeclaredMethods);

                        break;

                    case DotNetItemType.BaseTypeOrInterface:

                        add(new Type[] { typeInfo.BaseType }, select);

                        add(typeInfo.ImplementedInterfaces, select);

                        break;

                    case DotNetItemType.GenericParameter:

                        addGenericItems(DotNetTypeInfoProviderGenericTypeStructValue.GenericTypeParameter, typeInfo.GenericTypeParameters);

                        break;

                    case DotNetItemType.GenericArgument:

                        addGenericItems(DotNetTypeInfoProviderGenericTypeStructValue.GenericTypeArgument, typeInfo.GenericTypeArguments);

                        break;

                    default:

                        if (DotNetEnumeration.TryGetTypeInfoPredicate(typeToEnumerate, out Predicate<TypeInfo> predicate))

                            add(typeInfo.DeclaredNestedTypes.WherePredicate(predicate), item => new DotNetTypeInfoItemProvider(new TypeInfoItemProvider(item, typeToEnumerate), dotNetTypeInfo));

                        break;
                }
            }

            System.Collections.Generic.IEnumerable<DotNetTypeInfoItemProvider> enumerable = queue.Merge();

            return func == null ? enumerable : enumerable.WherePredicate(func);
        }
    }
}
