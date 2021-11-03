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
using System.IO;

using WinCopies.Collections.Generic;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Enumeration
{
    public interface IRecursivelyEnumerablePath<T> : IEnumerablePath<T>, IRecursiveEnumerable<T> where T : IPathInfo
    {
        // Left empty.
    }

    public class RecursivelyEnumerablePath<T> : EnumerablePath<T>, IRecursivelyEnumerablePath<T> where T : IPathInfo
    {
        private readonly string _searchPattern;
        private readonly SearchOption? _searchOption;
        private readonly bool _safeEnumeration;
        //#if DEBUG
        //        private readonly FileSystemEntryEnumeratorProcessSimulation _simulationParameters;
        //#endif
#if CS8
        private readonly EnumerationOptions _enumerationOptions;
#endif

        public T Value { get; }

        public RecursivelyEnumerablePath(in T path, string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder, in RecursiveEnumerationOrder recursiveEnumerationOrder, in Func<PathTypes<IPathInfo>.PathInfo, T> getNewPathInfoDelegate, bool safeEnumeration
           //#if DEBUG
           //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
           //#endif
           ) : base(path
#if CS8
            ??
#else
            == null ?
#endif
            throw GetArgumentNullException(nameof(path))
#if !CS8
            : path
#endif
            , enumerationOrder, recursiveEnumerationOrder, getNewPathInfoDelegate)
        {
            Value = path;
            _searchPattern = searchPattern;
            _searchOption = searchOption;
            _safeEnumeration = safeEnumeration;
        }

        public virtual System.Collections.Generic.IEnumerator<WinCopies.Collections.Generic.IRecursiveEnumerable<T>> GetRecursiveEnumerator() => new Enumerator(this, _searchPattern, _searchOption
#if CS8
                , _enumerationOptions
#endif
                , EnumerationOrder, _safeEnumeration
                //#if DEBUG
                //                , _simulationParameters
                //#endif
                );

        public virtual RecursiveEnumeratorAbstract<T> GetEnumerator() => new RecursiveEnumerator<T>(this, RecursiveEnumerationOrder);

        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public sealed class Enumerator : Enumerator<T, IRecursivelyEnumerablePath<T>>
        {
            private IRecursivelyEnumerablePath<T> _current;
            private Func<RecursivelyEnumerablePath<T>> _getNewRecursivelyEnumerablePathDelegate;
            private Func<bool> _moveNext;

            protected override IRecursivelyEnumerablePath<T> CurrentOverride => _current;

            public
#if WinCopies3
                override
#endif
                bool? IsResetSupported => EnumerablePath<T>.Enumerator._isResetSupported;

            internal Enumerator( RecursivelyEnumerablePath<T> enumerablePath, string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder, bool safeEnumeration
                //#if DEBUG
                //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
                //#endif
                )
#if WinCopies3
                : base(

                ((IRecursivelyEnumerablePath<T>)enumerablePath).GetEnumerator(searchPattern, searchOption
#if CS8
                    , enumerationOptions
#endif
                    , enumerationOrder, enumerablePath.RecursiveEnumerationOrder, safeEnumeration
                    //#if DEBUG
                    //                    , simulationParameters
                    //#endif
                    ))
#endif
            {
                _getNewRecursivelyEnumerablePathDelegate = () => new RecursivelyEnumerablePath<T>(InnerEnumerator.Current, searchPattern, searchOption
#if CS8
                    , enumerationOptions
#endif
                    , enumerationOrder, enumerablePath.RecursiveEnumerationOrder, enumerablePath.GetNewPathInfoDelegate, safeEnumeration
                    //#if DEBUG
                    //                    , simulationParameters
                    //#endif
                    );

                _moveNext = () => enumerablePath.Value.IsDirectory && (_moveNext = _MoveNext)();
            }

            public static Enumerator From(in RecursivelyEnumerablePath<T> enumerablePath, in string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder, in bool safeEnumeration
                //#if DEBUG
                //            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
                //#endif
                ) => new
#if !CS9
            Enumerator
#endif
            (enumerablePath ?? throw GetArgumentNullException(nameof(enumerablePath)), searchPattern, searchOption
#if CS8
                    , enumerationOptions
#endif
                    , enumerationOrder, safeEnumeration
                    //#if DEBUG
                    //                    , simulationParameters
                    //#endif
                    );

            private bool _MoveNext()
            {
                if (InnerEnumerator.MoveNext())
                {
                    _current = _getNewRecursivelyEnumerablePathDelegate();

                    return true;
                }

                _current = null;

                return false;
            }

            protected override bool MoveNextOverride() => _moveNext();

            protected override void DisposeManaged()
            {
                base.DisposeManaged();

                _getNewRecursivelyEnumerablePathDelegate = null;
                _moveNext = null;
                _current = null;
            }
        }
    }
}
