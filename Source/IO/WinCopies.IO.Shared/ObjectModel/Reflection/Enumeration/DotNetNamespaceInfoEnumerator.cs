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

    public sealed class DotNetNamespaceInfoEnumerator : Enumerator<TypeInfo, IDotNetItemInfo>
    {
        private Queue<string> _queue = new Queue<string>();
        private Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>> _dic = new Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>>();
        private Predicate<DotNetNamespaceEnumeratorStruct> _func;
        private IDotNetItemInfo _parent;
        private IEnumerator<IEnumerator<IDotNetItemInfo>> _enumerator;
        private bool _isCompleted = false;

        private IEnumerator<DotNetNamespaceInfo> GetNamespaceEnumerator()
        {
            string _namespace = _parent is DotNetNamespaceInfo ? _parent.Path.Replace(PathSeparator, '.') : null;

            DotNetNamespaceInfo select(string ____namespace) => new DotNetNamespaceInfo(_parent is DotNetNamespaceInfo ? $"{_parent.Path}{PathSeparator}{____namespace}" : ____namespace, _parent.ParentDotNetAssemblyInfo, _parent, false);

            Func<string, DotNetNamespaceInfo> _select = ___namespace =>
            {
                _select = select;

                _queue.Clear();

                return select(___namespace);
            };

            return new Enumerable<TypeInfo>(() => InnerEnumerator).Select(t => t.Namespace).Where(__namespace =>
            {
                if ((_parent is DotNetNamespaceInfo && __namespace.StartsWith(_namespace + '.')) || _parent is DotNetAssemblyInfo)
                {
                    if (_parent is DotNetNamespaceInfo)

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

        internal DotNetNamespaceInfoEnumerator(in IDotNetItemInfo dotNetItemInfo, in IEnumerable<DotNetItemType> typesToEnumerate, in Predicate<DotNetNamespaceEnumeratorStruct> func) : base(dotNetItemInfo.ParentDotNetAssemblyInfo.Assembly.DefinedTypes)
        {
            Debug.Assert(dotNetItemInfo.Is(false, typeof(DotNetAssemblyInfo), typeof(DotNetTypeInfo)));

            _parent = dotNetItemInfo;

            _func = func;

            IEnumerable<TypeInfo> enumerable = _parent.ParentDotNetAssemblyInfo.Assembly.DefinedTypes;

            void addEnumerator(DotNetItemType dotNetItemType, Predicate<TypeInfo> func) => _dic.Add(dotNetItemType, enumerable.WherePredicate(t => func(t) && _func(new DotNetNamespaceEnumeratorStruct(t))).Select(t => new DotNetTypeInfo(t, dotNetItemType)).GetEnumerator());

            foreach (DotNetItemType typeToEnumerate in typesToEnumerate)
            {
                if (typeToEnumerate == DotNetItemType.Namespace)

                    _dic.Add(DotNetItemType.Namespace, GetNamespaceEnumerator());

                else

                    switch (typeToEnumerate)
                    {
                        case DotNetItemType.Struct:

                            addEnumerator(DotNetItemType.Struct, t => t.IsValueType && !t.IsEnum);

                            break;

                        case DotNetItemType.Enum:

                            addEnumerator(DotNetItemType.Enum, t => t.IsEnum);

                            break;

                        case DotNetItemType.Class:

                            addEnumerator(DotNetItemType.Class, t => t.IsClass);

                            break;

                        case DotNetItemType.Interface:

                            addEnumerator(DotNetItemType.Interface, t => t.IsInterface);

                            break;

                        case DotNetItemType.Delegate:

                            addEnumerator(DotNetItemType.Delegate, t => typeof(Delegate).IsAssignableFrom(t));

                            break;

                        default:

                            throw new InvalidEnumArgumentException(nameof(typesToEnumerate), typeToEnumerate);
                    }
            }

            _enumerator = _dic.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns a new instance of the <see cref="DotNetNamespaceInfoEnumerator"/> class.
        /// </summary>
        /// <param name="dotNetItemInfo">The <see cref="DotNetNamespaceInfo"/> to enumerate.</param>
        /// <param name="typesToEnumerate">An enumerable that enumerates through the <see cref="DotNetItemType"/>s to enumerate. If this parameter is <see langword="null"/>, it will be filled in with all the fields of the <see cref="DotNetItemType"/> enumeration.</param>
        /// <param name="func">A custom predicate. If this parameter is <see langword="null"/>, it will be filled in with <see cref="GetCommonPredicate{T}"/>.</param>
        /// <returns>A new instance of the <see cref="DotNetNamespaceInfoEnumerator"/> class.</returns>
        public static DotNetNamespaceInfoEnumerator From(in IDotNetItemInfo dotNetItemInfo, in IEnumerable<DotNetItemType> typesToEnumerate, in Predicate<DotNetNamespaceEnumeratorStruct> func) => new DotNetNamespaceInfoEnumerator((dotNetItemInfo ?? throw GetArgumentNullException(nameof(dotNetItemInfo))).Is(false, typeof(DotNetAssemblyInfo), typeof(DotNetTypeInfo)) ? dotNetItemInfo : throw new ArgumentException($"{nameof(dotNetItemInfo)} must be {nameof(DotNetAssemblyInfo)} or {nameof(DotNetNamespaceInfo)}."), typesToEnumerate ?? typeof(DotNetItemType).GetFields().Select(f => (DotNetItemType)f.GetValue(null)), func ?? GetCommonPredicate<DotNetNamespaceEnumeratorStruct>());

        private Func<bool> _moveNext;

        private void ResetMoveNext() => _moveNext = () =>
        {
            if (_enumerator.MoveNext())
            {
                bool _func() => _enumerator.Current.MoveNext();

                bool func()
                {
                    if (_func())
                    {
                        Current = _enumerator.Current.Current;

                        return true;
                    }

                    ResetMoveNext();

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

        protected override bool MoveNextOverride()
        {
            if (_isCompleted)

                return false;

            if (_moveNext())

                return true;

            _isCompleted = true;

            return false;
        }

        protected override void ResetOverride()
        {
            base.ResetOverride();

            _isCompleted = false;

            ResetMoveNext();

            _enumerator.Reset();

            bool replaceNamespaceEnumerator = false;

            foreach (KeyValuePair<DotNetItemType, IEnumerator<IDotNetItemInfo>> item in _dic)

                if (item.Key == DotNetItemType.Namespace)

                    replaceNamespaceEnumerator = true;

                else

                    item.Value.Reset();

            if (replaceNamespaceEnumerator)

                _dic[DotNetItemType.Namespace] = GetNamespaceEnumerator();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _queue.Clear();

            _queue = null;

            _parent = null;

            _func = null;

            _dic = null;

            _enumerator = null;
        }
    }
}
