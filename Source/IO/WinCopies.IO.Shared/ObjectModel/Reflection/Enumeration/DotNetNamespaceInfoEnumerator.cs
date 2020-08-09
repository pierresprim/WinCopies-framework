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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using WinCopies.Collections;
using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.Linq;
using WinCopies.Util;
using WinCopies.Util.DotNetFix;

using static WinCopies.IO.Path;
using static WinCopies.Util.Util;

namespace WinCopies.IO.Reflection
{
    public struct DotNetNamespaceEnumeratorStruct
    {
        public string Namespace { get; }

        public TypeInfo TypeInfo { get; }

        public DotNetNamespaceEnumeratorStruct(in string _namespace)
        {
            Namespace = _namespace;

            TypeInfo = null;
        }

        public DotNetNamespaceEnumeratorStruct(in TypeInfo typeInfo)
        {
            TypeInfo = typeInfo;

            Namespace = null;
        }
    }

    public static class DotNetEnumeration
    {
        public static IEnumerator<IDotNetItemInfo> GetDotNetItemInfoEnumerator(in IEnumerable<TypeInfo> enumerable, in Predicate<TypeInfo> func) => enumerable.WherePredicate(func).Select(t => new DotNetTypeInfo(t, dotNetItemType)).GetEnumerator();

        public static Predicate<TypeInfo> GetTypeInfoPredicate(in DotNetItemType typeToEnumerate, in string typeToEnumerateEnumerableName)
        {
            switch (typeToEnumerate)
            {
                case DotNetItemType.Struct:

                    return t => t.IsValueType && !t.IsEnum;

                case DotNetItemType.Enum:

                    return t => t.IsEnum;

                case DotNetItemType.Class:

                    return t => t.IsClass;

                case DotNetItemType.Interface:

                    return t => t.IsInterface;

                case DotNetItemType.Delegate:

                    return t => typeof(Delegate).IsAssignableFrom(t);

                default:

                    throw new InvalidEnumArgumentException(typeToEnumerateEnumerableName, typeToEnumerate);
            }
        }
    }

    public sealed class DotNetEnumerationMoveNext<T> : Util.DotNetFix.IDisposable
    {
        private Func<bool> _moveNext;
        private IEnumerator<IEnumerator<T>> _enumerator;
        private T _current;

        public T Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

        public bool IsDisposed { get; private set; }

        public DotNetEnumerationMoveNext(in IEnumerator<IEnumerator<T>> enumerator)
        {
            _enumerator = enumerator;

            Reset();
        }

        public bool MoveNext() => _moveNext();

        private void _reset() =>

            _moveNext = () =>
            {
                if (_enumerator.MoveNext())
                {
                    bool _func() => _enumerator.Current.MoveNext();

                    bool func()
                    {
                        if (_func())
                        {
                            _current = _enumerator.Current.Current;

                            return true;
                        }

                        Reset();

                        return _moveNext();
                    }

                    if (func())
                    {
                        _moveNext = func;

                        return true;
                    }
                }

                return false;
            };

        public void Reset()
        {
            if (IsDisposed)

                throw GetExceptionForDispose(false);

            _enumerator.Reset();

            _reset();
        }

        public void Dispose()
        {
            if (IsDisposed)

                return;

            _moveNext = null;

            _enumerator = null;

            _current = default;
        }
    }

    public sealed class DotNetNamespaceInfoEnumerator : Enumerator<TypeInfo, IDotNetItemInfo>
    {
        private Queue<string> _queue = new Queue<string>();
        private Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>> _dic = new Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>>();
        private Predicate<DotNetNamespaceEnumeratorStruct> _func;
        private IDotNetItemInfo _parent;
        private bool _isCompleted = false;

        private IEnumerator<DotNetNamespaceInfo> GetNamespaceEnumerator()
        {
            string _namespace = _parent is IDotNetNamespaceInfo ? _parent.Path.Replace(PathSeparator, '.') : null;

            DotNetNamespaceInfo select(string ____namespace) => new DotNetNamespaceInfo(_parent is IDotNetNamespaceInfo ? $"{_parent.Path}{PathSeparator}{____namespace}" : ____namespace, ____namespace, _parent, false);

            Func<string, DotNetNamespaceInfo> _select = ___namespace =>
            {
                _select = select;

                _queue.Clear();

                return select(___namespace);
            };

            return new Enumerable<TypeInfo>(() => InnerEnumerator).Select(t => t.Namespace).Where(__namespace =>
            {
                if ((_parent is IDotNetNamespaceInfo && __namespace.StartsWith(_namespace + '.')) || _parent is IDotNetAssemblyInfo)
                {
                    if (_parent is IDotNetNamespaceInfo)

                        __namespace = __namespace.Substring(_namespace.Length + 1);

                    if (__namespace.Contains('.'))

                        __namespace = __namespace.Substring(0, __namespace.IndexOf('.'));

                    if (_queue.Contains(__namespace))

                        return false;

                    _queue.Enqueue(__namespace);

                    return _func(new DotNetNamespaceEnumeratorStruct(__namespace));
                }

                return false;

            }).Select(_select).GetEnumerator();
        }

        internal DotNetNamespaceInfoEnumerator(in IDotNetItemInfo dotNetItemInfo, in IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceEnumeratorStruct> func) : base(dotNetItemInfo.ParentDotNetAssemblyInfo.Assembly.DefinedTypes)
        {
            Debug.Assert(dotNetItemInfo.Is(false, typeof(IDotNetAssemblyInfo), typeof(IDotNetTypeInfo)));

            _parent = dotNetItemInfo;

            _func = func;

            IEnumerable<TypeInfo> enumerable = _parent.ParentDotNetAssemblyInfo.Assembly.DefinedTypes;

            void addEnumerator(DotNetItemType dotNetItemType, Predicate<TypeInfo> __func) => _dic.Add(dotNetItemType, DotNetEnumeration.GetDotNetItemInfoEnumerator(enumerable, t => __func(t) && func(new DotNetNamespaceEnumeratorStruct(t))));

            foreach (DotNetItemType typeToEnumerate in typesToEnumerate)
            {
                if (typeToEnumerate == DotNetItemType.Namespace)

                    _dic.Add(DotNetItemType.Namespace, GetNamespaceEnumerator());

                else

                    addEnumerator(typeToEnumerate, DotNetEnumeration.GetTypeInfoPredicate(typeToEnumerate, nameof(typesToEnumerate)));
            }

            _moveNext = new DotNetEnumerationMoveNext<IDotNetItemInfo>(_dic.Values.GetEnumerator());
        }

        /// <summary>
        /// Returns a new instance of the <see cref="DotNetNamespaceInfoEnumerator"/> class.
        /// </summary>
        /// <param name="dotNetItemInfo">The <see cref="DotNetNamespaceInfo"/> to enumerate.</param>
        /// <param name="typesToEnumerate">An enumerable that enumerates through the <see cref="DotNetItemType"/>s to enumerate. If this parameter is <see langword="null"/>, it will be filled in with all the fields of the <see cref="DotNetItemType"/> enumeration.</param>
        /// <param name="func">A custom predicate. If this parameter is <see langword="null"/>, it will be filled in with <see cref="GetCommonPredicate{T}"/>.</param>
        /// <returns>A new instance of the <see cref="DotNetNamespaceInfoEnumerator"/> class.</returns>
        public static DotNetNamespaceInfoEnumerator From(in IDotNetItemInfo dotNetItemInfo, in IEnumerable<DotNetItemType> typesToEnumerate, in Predicate<DotNetNamespaceEnumeratorStruct> func) => new DotNetNamespaceInfoEnumerator((dotNetItemInfo ?? throw GetArgumentNullException(nameof(dotNetItemInfo))).Is(false, typeof(IDotNetAssemblyInfo), typeof(IDotNetTypeInfo)) ? dotNetItemInfo : throw new ArgumentException($"{nameof(dotNetItemInfo)} must be {nameof(DotNetAssemblyInfo)} or {nameof(DotNetNamespaceInfo)}."), typesToEnumerate ?? typeof(DotNetItemType).GetFields().Select(f => (DotNetItemType)f.GetValue(null)), func ?? GetCommonPredicate<DotNetNamespaceEnumeratorStruct>());

        private DotNetEnumerationMoveNext<IDotNetItemInfo> _moveNext;

        protected override bool MoveNextOverride()
        {
            if (_isCompleted)

                return false;

            if (_moveNext.MoveNext())
            {
                Current = _moveNext.Current;

                return true;
            }

            _isCompleted = true;

            return false;
        }

        protected override void ResetOverride()
        {
            throw new NotSupportedException("THis enumerator does not support reset.");

            //base.ResetOverride();

            //_isCompleted = false;

            //_moveNext.Reset();

            //bool replaceNamespaceEnumerator = false;

            //foreach (KeyValuePair<DotNetItemType, IEnumerator<IDotNetItemInfo>> item in _dic)

            //    if (item.Key == DotNetItemType.Namespace)

            //        replaceNamespaceEnumerator = true;

            //    else

            //        item.Value.Reset();

            //if (replaceNamespaceEnumerator)

            //    _dic[DotNetItemType.Namespace] = GetNamespaceEnumerator();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _queue.Clear();

            _queue = null;

            _parent = null;

            _func = null;

            _dic = null;
        }
    }
}
