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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using WinCopies.Collections;
using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.IO.Reflection;
using WinCopies.Linq;

using static WinCopies.IO.Path;
using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Enumeration.Reflection
{
    //    public sealed class DotNetEnumerationMoveNext<T> : Enumerator<IEnumerator<T>, T>
    //    {
    //        private Func<bool> _moveNext;
    //        private T _current;

    //#if !WinCopies3
    //        private IEnumerator<IEnumerator<T>> _enumerator;

    //        public T Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

    //        public bool IsDisposed { get; private set; }
    //#else
    //        protected override T CurrentOverride => _current;

    //        public override bool? IsResetSupported => null;
    //#endif

    //        public DotNetEnumerationMoveNext(in IEnumerator<IEnumerator<T>> enumerator)
    //#if !WinCopies3
    //        {
    //#else
    //            : base(enumerator) =>
    //#endif
    //#if !WinCopies3
    //            _enumerator = enumerator;
    //#endif

    //        Reset();
    //#if !WinCopies3
    //        }
    //#endif

    //        public DotNetEnumerationMoveNext(in System.Collections.Generic.IEnumerable<IEnumerator<T>> enumerable) : this(enumerable.GetEnumerator())
    //        {
    //            // Left empty.
    //        }

    //#if !WinCopies3
    //public bool MoveNext
    //#else
    //        protected override bool MoveNextOverride
    //#endif
    //            () => _moveNext();

    //        private void _Reset() =>

    //            _moveNext = () =>
    //            {
    //                if (
    //#if !WinCopies3
    //                _enumerator
    //#else
    //                InnerEnumerator
    //#endif
    //                .MoveNext())
    //                {
    //                    bool _func() =>
    //#if !WinCopies3
    //                _enumerator
    //#else
    //                InnerEnumerator
    //#endif
    //                .Current.MoveNext();

    //                    bool func()
    //                    {
    //                        if (_func())
    //                        {
    //                            _current =
    //#if !WinCopies3
    //                _enumerator
    //#else
    //                InnerEnumerator
    //#endif
    //                .Current.Current;

    //                            return true;
    //                        }

    //                        Reset();

    //                        return _moveNext();
    //                    }

    //                    if (func())
    //                    {
    //                        _moveNext = func;

    //                        return true;
    //                    }
    //                }

    //                return false;
    //            };

    //#if !WinCopies3
    //        public void Reset()
    //        {
    //            if (IsDisposed)

    //                throw GetExceptionForDispose(false);

    //                _enumerator
    //#else
    //        protected override void ResetOverride()
    //        {
    //            base.ResetOverride();

    //            InnerEnumerator
    //#endif
    //                .Reset();

    //            _Reset();
    //        }

    //#if !WinCopies3
    //        public void Dispose()
    //        {
    //            if (IsDisposed)

    //                return;
    //#else
    //        protected override void DisposeManaged()
    //        {
    //            base.DisposeManaged();
    //#endif

    //            _moveNext = null;

    //#if !WinCopies3
    //            _enumerator = null;
    //#endif

    //            _current = default;
    //        }
    //    }

    public static class DotNetNamespaceInfoEnumeration
    {
        public static System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> From(IBrowsableObjectInfo dotNetItemInfo, System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, in Predicate<DotNetNamespaceInfoItemProvider> func)
        {
            Debug.Assert(dotNetItemInfo.Is(false, typeof(IDotNetAssemblyInfo), typeof(IDotNetNamespaceInfoBase)));

            ThrowIfNull(dotNetItemInfo, nameof(dotNetItemInfo));

            if (typesToEnumerate == null)

                typesToEnumerate = typeof(DotNetItemType).GetFields().Select(f => (DotNetItemType)f.GetValue(null));

            System.Collections.Generic.IEnumerable<TypeInfo> enumerable;
            Predicate<Type> typePredicate;

            if (dotNetItemInfo is IDotNetAssemblyInfo dotNetAssemblyInfo)
            {
                enumerable = dotNetAssemblyInfo.Assembly.DefinedTypes;

                if (enumerable == null)

                    return null;

                typePredicate = t => IsNullEmptyOrWhiteSpace(t.Namespace);
            }

            else if (dotNetItemInfo is IDotNetNamespaceInfoBase dotNetNamespaceInfo)
            {
                enumerable = dotNetNamespaceInfo.ParentDotNetAssemblyInfo.Assembly.DefinedTypes;

                if (enumerable == null)

                    return null;

                typePredicate = t => t.Namespace == dotNetNamespaceInfo.Path.Replace(WinCopies.IO.Path.PathSeparator, '.');
            }

            else

                throw new ArgumentException($"{nameof(dotNetItemInfo)} must be an {nameof(IDotNetAssemblyInfo)} or an {nameof(IDotNetNamespaceInfoBase)}", nameof(dotNetItemInfo));

            EnumerableHelper<System.Collections.Generic.IEnumerable<string>>.IEnumerableQueue namespaces = EnumerableHelper<System.Collections.Generic.IEnumerable<string>>.GetEnumerableQueue();

            EnumerableHelper<System.Collections.Generic.IEnumerable<TypeInfoItemProvider>>.IEnumerableQueue types = EnumerableHelper<System.Collections.Generic.IEnumerable<TypeInfoItemProvider>>.GetEnumerableQueue();

            void addEnumerable(DotNetItemType itemType)
            {
                if (DotNetEnumeration.TryGetTypeInfoPredicate(itemType, out Predicate<TypeInfo> result))

                    types.Enqueue(enumerable.Where(t => result(t) && typePredicate(t)).Select(t => new TypeInfoItemProvider(t, itemType)));
            }

            void addNamespaceEnumerable(in System.Collections.Generic.IEnumerable<Type> types, IBrowsableObjectInfo parent)
            {
                EnumerableHelper<string>.IEnumerableQueue _queue = EnumerableHelper<string>.GetEnumerableQueue();
                string _namespace = parent is IDotNetNamespaceInfoBase ? parent.Path.Replace(PathSeparator, '.') : null;

                // DotNetNamespaceInfoItemProvider select(string ____namespace) => new DotNetNamespaceInfoItemProvider(____namespace, false, parent);

                //Func<string, DotNetNamespaceInfoItemProvider> _select = ___namespace =>
                //{
                //    _select = select;

                //    _queue.Clear();

                //    return select(___namespace);
                //};

                namespaces.Enqueue(types.Select(t => t.Namespace).Where(__namespace =>
               {
                   if ((parent is IDotNetNamespaceInfoBase && __namespace.StartsWith(_namespace + '.')) || parent is IDotNetAssemblyInfo)
                   {
                       if (parent is IDotNetNamespaceInfoBase)

                           __namespace = __namespace
#if CS8
                        [
#else
                        .Substring
#endif
                        (_namespace.Length + 1)
#if CS8
                        ..]
#endif
                        ;

                       if (__namespace.Contains('.'))

                           __namespace = __namespace.Substring(0, __namespace.IndexOf('.'));

                       if (_queue.Contains(__namespace))

                           return false;

                       _queue.Enqueue(__namespace);

                       return true;
                   }

                   return false;

               }));
            }

            foreach (DotNetItemType typeToEnumerate in typesToEnumerate)
            {
                if (typeToEnumerate == DotNetItemType.Namespace)

                    addNamespaceEnumerable(enumerable, dotNetItemInfo);

                else

                    addEnumerable(typeToEnumerate);
            }

            return namespaces.Merge().Select(_namespace => new DotNetNamespaceInfoItemProvider(_namespace, dotNetItemInfo)).AppendValues(types.Merge().Select(type => new DotNetNamespaceInfoItemProvider(type, dotNetItemInfo)));
        }
    }
}
