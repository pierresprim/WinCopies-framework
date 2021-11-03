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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;

using WinCopies.Collections.Generic;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Enumeration
{
#if DEBUG
    public
#else
        internal 
#endif
        enum PathType
    {
        Directories = 1,

        Files = 2,

        All = Directories | Files
    }

    public class EnumerablePath<T> : IEnumerablePath<T> where T : IPathInfo
    {
        public IPathInfo Path { get; }

        public FileSystemEntryEnumerationOrder EnumerationOrder { get; }

        public RecursiveEnumerationOrder RecursiveEnumerationOrder { get; }

        protected Func<PathTypes<IPathInfo>.PathInfo, T> GetNewPathInfoDelegate { get; }

        public EnumerablePath(in IPathInfo path, in FileSystemEntryEnumerationOrder enumerationOrder, in RecursiveEnumerationOrder recursiveEnumerationOrder, in Func<PathTypes<IPathInfo>.PathInfo, T> getNewPathInfoDelegate)
        {
            Path = path;

            EnumerationOrder = enumerationOrder;

            RecursiveEnumerationOrder = recursiveEnumerationOrder;

            GetNewPathInfoDelegate = getNewPathInfoDelegate;
        }

        public System.Collections.Generic.IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
            , bool safeEnumeration
            //#if DEBUG
            //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            ) => EnumerablePath.GetFileSystemEntryEnumerable(Path.Path, searchPattern, searchOption
#if CS8
               , enumerationOptions
#endif
                , safeEnumeration
                //#if DEBUG
                //                , simulationParameters
                //#endif
                );

        public System.Collections.Generic.IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , bool safeEnumeration
            //#if DEBUG
            //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            ) => EnumerablePath.GetDirectoryEnumerable(Path.Path, searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , safeEnumeration
                //#if DEBUG
                //                , simulationParameters
                //#endif
                );

        public System.Collections.Generic.IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , bool safeEnumeration
            //#if DEBUG
            //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            ) => EnumerablePath.GetFileEnumerable(Path.Path, searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , safeEnumeration
                //#if DEBUG
                //                , simulationParameters
                //#endif
                );

        public Enumerator GetEnumerator(in string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder, in RecursiveEnumerationOrder recursiveEnumerationOrder, in bool safeEnumeration
            //#if DEBUG
            //            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            ) => new
#if !CS9
            Enumerator
#endif
            (this, searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , enumerationOrder, safeEnumeration
//#if DEBUG
//                , simulationParameters
//#endif
);

#if !WinCopies3
        IDisposableEnumeratorInfo
#else
        Collections.Generic.IEnumeratorInfo2
#endif
            <T> IEnumerablePath<T>.GetEnumerator(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder, RecursiveEnumerationOrder recursiveEnumerationOrder, bool safeEnumeration
            //#if DEBUG
            //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            ) => GetEnumerator(searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , enumerationOrder, recursiveEnumerationOrder, safeEnumeration
                //#if DEBUG
                //                , simulationParameters
                //#endif
                );

        protected virtual System.Collections.Generic.IEnumerator<T> GetEnumerator() => GetEnumerator(null, null
#if CS8
            , null
#endif
            , EnumerationOrder, RecursiveEnumerationOrder, true
            //#if DEBUG
            //            , null
            //#endif
            );

        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(null, null
#if CS8
            , null
#endif
            , EnumerationOrder, RecursiveEnumerationOrder, true
            //#if DEBUG
            //            , null
            //#endif
            );

        public sealed class Enumerator : WinCopies.Collections.Generic.Enumerator<T>
        {
            private SubEnumerator[] _enumerators;
            private SubEnumerator _currentEnumerator;
            private Func<bool> _moveNext;
            private T _current;
            internal const bool _isResetSupported = false;
            private Func<PathTypes<IPathInfo>.PathInfo, T> _func;

            protected override T CurrentOverride => _current;

            public override bool? IsResetSupported => _isResetSupported;

            // public int Count => IsDisposed ? throw GetExceptionForDispose(false) : _directoryEnumerator.Count + _filesEnumerator.Count;

            //#if DEBUG
            //            public FileSystemEntryEnumeratorProcessSimulation SimulationParameters { get; }
            //#endif

            private class SubEnumerator
            {
                public System.Collections.Generic.IEnumerator<PathTypes<IPathInfo>.IPathCommon> Enumerator { get; }

                public PathType PathType { get; }

                public SubEnumerator(System.Collections.Generic.IEnumerator<PathTypes<IPathInfo>.IPathCommon> enumerator, PathType pathType)
                {
                    Enumerator = enumerator;

                    PathType = pathType;
                }
            }

            internal Enumerator(EnumerablePath<T> enumerablePath, string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder, bool safeEnumeration
                //#if DEBUG
                //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
                //#endif
                )
            {
                PathTypes<IPathInfo>.IPathCommon getPathData(string path) => new PathTypes<IPathInfo>.PathInfoCommon(System.IO.Path.GetFileName(path), enumerablePath.Path);

                Debug.Assert(enumerablePath != null);

                ThrowIfNotValidEnumValue(nameof(enumerationOrder), enumerationOrder);

                _func = enumerablePath.GetNewPathInfoDelegate;

                SubEnumerator getDirectoryEnumerator() => new
#if !CS9
                    SubEnumerator
#endif
                    (enumerablePath.GetDirectoryEnumerable(searchPattern, searchOption
#if CS8
                    , enumerationOptions
#endif
                    , safeEnumeration
                    //#if DEBUG
                    //                    , simulationParameters
                    //#endif
                    ).Select(getPathData).GetEnumerator(), PathType.Directories);

                SubEnumerator getFileEnumerator() => new
#if !CS9
                    SubEnumerator
#endif
                    (enumerablePath.GetFileEnumerable(searchPattern, searchOption
#if CS8
                    , enumerationOptions
#endif
                    , safeEnumeration
                    //#if DEBUG
                    //                    , simulationParameters
                    //#endif
                    ).Select(getPathData).GetEnumerator(), PathType.Files);

                void initEnumeratorArray(in int length) => _enumerators = new SubEnumerator[length];

                void updateCurrent() => _current = _func(new PathTypes<IPathInfo>.PathInfo(_currentEnumerator.Enumerator.Current, _currentEnumerator.PathType == PathType.Directories || (_currentEnumerator.PathType == PathType.Files ? false : System.IO.Directory.Exists(_currentEnumerator.Enumerator.Current.Path))));

                void setMoveNext() => _moveNext = () =>
                {
                    bool __moveNext()
                    {
                        if (_currentEnumerator.Enumerator.MoveNext())
                        {
                            updateCurrent();

                            return true;
                        }

                        return false;
                    }

                    if (__moveNext())

                        return true;

                    if (_enumerators.Length == 1)

                        return false;

                    _currentEnumerator = _enumerators[1];

                    if (__moveNext())
                    {
                        _moveNext = __moveNext;

                        return true;
                    }

                    return false;
                };

                switch (enumerationOrder)
                {
                    case FileSystemEntryEnumerationOrder.FilesThenDirectories:

                        initEnumeratorArray(2);

                        _enumerators[0] = getFileEnumerator();

                        _enumerators[1] = getDirectoryEnumerator();

                        setMoveNext();

                        break;

                    case FileSystemEntryEnumerationOrder.DirectoriesThenFiles:

                        initEnumeratorArray(2);

                        _enumerators[0] = getDirectoryEnumerator();

                        _enumerators[1] = getFileEnumerator();

                        setMoveNext();

                        break;

                    case FileSystemEntryEnumerationOrder.None:

                        initEnumeratorArray(1);

                        _enumerators[0] = new SubEnumerator(enumerablePath.GetFileSystemEntryEnumerable(searchPattern, searchOption
#if CS8
                            , enumerationOptions
#endif
                            , safeEnumeration
                            //#if DEBUG
                            //                            , simulationParameters
                            //#endif
                            ).Select(getPathData).GetEnumerator(), PathType.All);

                        setMoveNext();

                        break;
                }

                _currentEnumerator = _enumerators[0];

                //#if DEBUG
                //                SimulationParameters = simulationParameters;
                //#endif
            }

            public static Enumerator From(in EnumerablePath<T> enumerablePath, in string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder, in Func<PathTypes<IPathInfo>.IPathInfo, T> getNewPathInfoDelegate, in bool safeEnumeration
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

            protected override bool MoveNextOverride() => _moveNext();

            protected override void ResetOverride2()
            {
                _enumerators = null;
                _func = null;
                _moveNext = null;
                _current = default;
            }

            protected override void DisposeManaged()
            {
                base.DisposeManaged();

                _currentEnumerator = null;
            }
        }
    }
}
