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
using WinCopies.IO.ObjectModel;
using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.Linq;

using static WinCopies.IO.Path;

#if !WinCopies3
using WinCopies.Util;

using static WinCopies.Util.Util;
#else
using WinCopies.Collections.Generic;

using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;
#endif

namespace WinCopies.IO.Reflection
{
    public struct DotNetNamespaceInfoEnumeratorStruct
    {
        public string Namespace { get; }

        public TypeInfo TypeInfo { get; }

        public DotNetNamespaceInfoEnumeratorStruct(in string _namespace)
        {
            Namespace = _namespace;

            TypeInfo = null;
        }

        public DotNetNamespaceInfoEnumeratorStruct(in TypeInfo typeInfo)
        {
            TypeInfo = typeInfo;

            Namespace = null;
        }
    }

    public static class DotNetEnumeration
    {
        internal static IEnumerator<IDotNetItemInfo> GetDotNetItemInfoEnumerator(in System.Collections.Generic.IEnumerable<TypeInfo> enumerable, DotNetItemType itemType, bool isRootType, IBrowsableObjectInfo parent, in Predicate<TypeInfo> func) => enumerable.WherePredicate(func).Select(t => new DotNetTypeInfo(t, itemType, isRootType, parent)).GetEnumerator();

        public static Predicate<TypeInfo> GetTypeInfoPredicate(in DotNetItemType typeToEnumerate, in string typeToEnumerateEnumerableName)
#if CS7
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
                
                    throw GetInvalidEnumArgumentException(typeToEnumerateEnumerableName, typeToEnumerate);
            }
        }
#else
            => typeToEnumerate switch
            {
                DotNetItemType.Struct => t => t.IsValueType && !t.IsEnum,
                DotNetItemType.Enum => t => t.IsEnum,
                DotNetItemType.Class => t => t.IsClass,
                DotNetItemType.Interface => t => t.IsInterface,
                DotNetItemType.Delegate => t => typeof(Delegate).IsAssignableFrom(t),
                _ => throw GetInvalidEnumArgumentException(typeToEnumerateEnumerableName, typeToEnumerate),
            };
#endif

        public static InvalidEnumArgumentException GetInvalidEnumArgumentException(in string typeToEnumerateEnumerableName, in DotNetItemType typeToEnumerate) => new InvalidEnumArgumentException(typeToEnumerateEnumerableName, typeToEnumerate);
    }

    public sealed class DotNetEnumerationMoveNext<T> :
#if !WinCopies3
        Util.DotNetFix.IDisposable
#else
        Enumerator<IEnumerator<T>, T>
#endif
    {
        private Func<bool> _moveNext;
        private T _current;

#if !WinCopies3
        private IEnumerator<IEnumerator<T>> _enumerator;

        public T Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

        public bool IsDisposed { get; private set; }
#else
        protected override T CurrentOverride => _current;

        public override bool? IsResetSupported => null;
#endif

        public DotNetEnumerationMoveNext(in IEnumerator<IEnumerator<T>> enumerator)
#if !WinCopies3
        {
#else
            : base(enumerator) =>
#endif
#if !WinCopies3
            _enumerator = enumerator;
#endif

        Reset();
#if !WinCopies3
        }
#endif

#if !WinCopies3
public bool MoveNext
#else
        protected override bool MoveNextOverride
#endif
            () => _moveNext();

        private void _Reset() =>

            _moveNext = () =>
            {
                if (
#if !WinCopies3
                _enumerator
#else
                InnerEnumerator
#endif
                .MoveNext())
                {
                    bool _func() =>
#if !WinCopies3
                _enumerator
#else
                InnerEnumerator
#endif
                .Current.MoveNext();

                    bool func()
                    {
                        if (_func())
                        {
                            _current =
#if !WinCopies3
                _enumerator
#else
                InnerEnumerator
#endif
                .Current.Current;

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

#if !WinCopies3
        public void Reset()
        {
            if (IsDisposed)

                throw GetExceptionForDispose(false);

                _enumerator
#else
        protected override void ResetOverride()
        {
            base.ResetOverride();

            InnerEnumerator
#endif
                .Reset();

            _Reset();
        }

#if !WinCopies3
        public void Dispose()
        {
            if (IsDisposed)

                return;
#else
        protected override void DisposeManaged()
        {
            base.DisposeManaged();
#endif

            _moveNext = null;

#if !WinCopies3
            _enumerator = null;
#endif

            _current = default;
        }
    }

    public sealed class DotNetNamespaceInfoEnumerator : Enumerator<TypeInfo, IDotNetItemInfo>
    {
        #region Private fields
        private
#if !WinCopies3
WinCopies.Collections.Generic.Queue
#else
            EnumerableHelper
#endif
            <string>
#if WinCopies3
            .IEnumerableQueue
#endif
            _queue =
#if !WinCopies3
            new WinCopies.Collections.Generic.Queue
#else
           EnumerableHelper
#endif
            <string>
#if WinCopies3
            .GetEnumerableQueue
#endif
            ();
        private DotNetEnumerationMoveNext<IDotNetItemInfo> _moveNext;
        private Predicate<DotNetNamespaceInfoEnumeratorStruct> _func;
        private IBrowsableObjectInfo _parent;
#if !WinCopies3
        private bool _isCompleted = false;
#else
        private IDotNetItemInfo _current;
#endif
        #endregion

#if WinCopies3
        protected override IDotNetItemInfo CurrentOverride => _current;

        public override bool? IsResetSupported => false;

        protected override void ResetCurrent() => _current = null;
#endif

        internal DotNetNamespaceInfoEnumerator(IBrowsableObjectInfo dotNetItemInfo, System.Collections.Generic.IEnumerable<TypeInfo> typeInfos, in System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoEnumeratorStruct> func) : base(typeInfos)
        {
            Debug.Assert(dotNetItemInfo.Is(false, typeof(IDotNetAssemblyInfo), typeof(IDotNetNamespaceInfo)));

            _parent = dotNetItemInfo;

            _func = func;

            System.Collections.Generic.IEnumerable<TypeInfo> enumerable = typeInfos;

            bool isRootType = dotNetItemInfo is IDotNetAssemblyInfo;

            var _dic = new Dictionary<DotNetItemType, IEnumerator<IDotNetItemInfo>>();

            void addEnumerator(DotNetItemType dotNetItemType, Predicate<TypeInfo> __func) => _dic.Add(dotNetItemType, DotNetEnumeration.GetDotNetItemInfoEnumerator(enumerable, dotNetItemType, isRootType, dotNetItemInfo, t => __func(t) && func(new DotNetNamespaceInfoEnumeratorStruct(t))));

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
        /// <param name="dotNetItemInfo">The <see cref="IBrowsableObjectInfo"/> to enumerate. This must be an <see cref="IDotNetAssemblyInfo"/> or an <see cref="IDotNetNamespaceInfo"/>.</param>
        /// <param name="typesToEnumerate">An enumerable that enumerates through the <see cref="DotNetItemType"/>s to enumerate. If this parameter is <see langword="null"/>, it will be filled in with all the fields of the <see cref="DotNetItemType"/> enumeration.</param>
        /// <param name="func">A custom predicate. If this parameter is <see langword="null"/>, it will be filled in with <see cref="GetCommonPredicate{T}"/>.</param>
        /// <returns>A new instance of the <see cref="DotNetNamespaceInfoEnumerator"/> class.</returns>
        public static DotNetNamespaceInfoEnumerator From(in IBrowsableObjectInfo dotNetItemInfo, in System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, in Predicate<DotNetNamespaceInfoEnumeratorStruct> func) => new DotNetNamespaceInfoEnumerator((dotNetItemInfo ?? throw GetArgumentNullException(nameof(dotNetItemInfo))).Is(false, typeof(IDotNetAssemblyInfo), typeof(IDotNetTypeInfo)) ? dotNetItemInfo : throw new ArgumentException($"{nameof(dotNetItemInfo)} must be {nameof(DotNetAssemblyInfo)} or {nameof(DotNetNamespaceInfo)}."), (dotNetItemInfo is IDotNetNamespaceInfo dotNetNamespaceInfo ? dotNetNamespaceInfo.ParentDotNetAssemblyInfo : dotNetItemInfo is IDotNetAssemblyInfo dotNetAssemblyInfo ? dotNetAssemblyInfo : throw new ArgumentException($"{nameof(dotNetItemInfo)} must be an {nameof(IDotNetAssemblyInfo)} or an {nameof(IDotNetNamespaceInfo)}", nameof(dotNetItemInfo))).EncapsulatedObject.DefinedTypes, typesToEnumerate ?? typeof(DotNetItemType).GetFields().Select(f => (DotNetItemType)f.GetValue(null)), func ?? GetCommonPredicate<DotNetNamespaceInfoEnumeratorStruct>());

        private IEnumerator<DotNetNamespaceInfo> GetNamespaceEnumerator()
        {
            string _namespace = _parent is IDotNetNamespaceInfo ? _parent.Path.Replace(PathSeparator, '.') : null;

            DotNetNamespaceInfo select(string ____namespace) => new DotNetNamespaceInfo(____namespace, false, _parent);

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

                    return _func(new DotNetNamespaceInfoEnumeratorStruct(__namespace));
                }

                return false;

            }).Select(_select).GetEnumerator();
        }

        protected override bool MoveNextOverride()
        {
#if !WinCopies3
            if (_isCompleted)

                return false;
#endif

            if (_moveNext.MoveNext())
            {
#if !WinCopies3
Current
#else
                _current
#endif
                    = _moveNext.Current;

                return true;
            }

#if !WinCopies3
            _isCompleted = true;
#endif

            return false;
        }

#if !WinCopies3
        protected override void ResetOverride() => throw new NotSupportedException("THis enumerator does not support reset.");
#endif

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

        protected override void
#if !WinCopies3
            Dispose(bool disposing)
        {
            base.Dispose(disposing);
#else
            DisposeManaged()
        {
            base.DisposeManaged();
#endif
            _queue.Clear();

            _queue = null;

            _parent = null;

            _func = null;

            _moveNext = null;
        }
    }
}
