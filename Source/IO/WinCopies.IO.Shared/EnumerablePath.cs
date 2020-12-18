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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework. If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using WinCopies.Collections.Generic;

#if WinCopies2
using WinCopies.Util;

using static WinCopies.Util.Util;
#else
using static WinCopies.ThrowHelper;
using static WinCopies.Collections.ThrowHelper;
#endif

namespace WinCopies.IO
{
    public enum FileSystemEntryEnumerationOrder : byte
    {
        /// <summary>
        /// Do not enumerates any item.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enumerates files then directories.
        /// </summary>
        FilesThenDirectories = 1,

        /// <summary>
        /// Enumerates directories then files.
        /// </summary>
        DirectoriesThenFiles = 2
    }

    public interface IEnumerablePath<T> : System.Collections.Generic.IEnumerable<T> where T : IPathInfo
    {
        string Path { get; }

        System.Collections.Generic.IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );

        System.Collections.Generic.IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );

        System.Collections.Generic.IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );

#if WinCopies2
        WinCopies.Collections.Generic.IDisposableEnumeratorInfo
#else
        Collections.Generic.IEnumeratorInfo2
#endif
            <T> GetEnumerator(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );
    }

    public static class EnumerablePath
    {
        public static System.Collections.Generic.IEnumerable<string> GetFileSystemEntryEnumerable(in string path, string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            )
        {
            if (string.IsNullOrEmpty(searchPattern) && searchOption == null
#if NETCORE
                     && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return System.IO.Directory.EnumerateFileSystemEntries(path);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.All);
#endif
            }

            else if (searchPattern != null && searchOption == null
#if NETCORE
                    && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.All);
#endif
            }

#if NETCORE
            else if (searchOption == null)
            {
                if (searchPattern == null)

                    searchPattern = "";
#if DEBUG
                if (simulationParameters == null)
#endif
                    return System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.All);
#endif
            }
#endif

            else
            {
                if (searchPattern == null)

                    searchPattern = "";
#if DEBUG
                if (simulationParameters == null)

#endif
                    return System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption.Value);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.All);
#endif
            }
        }

        public static System.Collections.Generic.IEnumerable<string> GetDirectoryEnumerable(in string path, string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            )
        {
            if (string.IsNullOrEmpty(searchPattern) && searchOption == null
#if NETCORE
                     && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return System.IO.Directory.EnumerateDirectories(path);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
#endif
            }

            else if (searchPattern != null && searchOption == null
#if NETCORE
                    && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return System.IO.Directory.EnumerateDirectories(path, searchPattern);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
#endif
            }

#if NETCORE
            else if (searchOption == null)
            {
                if (searchPattern == null)

                    searchPattern = "";
#if DEBUG
                if (simulationParameters == null)
#endif
                    return System.IO.Directory.EnumerateDirectories(path, searchPattern, enumerationOptions);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
#endif
            }
#endif

            else
            {
                if (searchPattern == null)

                    searchPattern = "";
#if DEBUG
                if (simulationParameters == null)

#endif
                    return System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOption.Value);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
#endif
            }
        }

        public static System.Collections.Generic.IEnumerable<string> GetFileEnumerable(in string path, string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            )
        {
            if (string.IsNullOrEmpty(searchPattern) && searchOption == null
#if NETCORE
                     && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return System.IO.Directory.EnumerateFiles(path);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Files);
#endif
            }

            else if (searchPattern != null && searchOption == null
#if NETCORE
                    && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return System.IO.Directory.EnumerateFiles(path, searchPattern);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Files);
#endif
            }

#if NETCORE
            else if (searchOption == null)
            {
                if (searchPattern == null)

                    searchPattern = "";
#if DEBUG
                if (simulationParameters == null)
#endif
                    return System.IO.Directory.EnumerateFiles(path, searchPattern, enumerationOptions);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Files);
#endif
            }
#endif

            else
            {
                if (searchPattern == null)

                    searchPattern = "";
#if DEBUG
                if (simulationParameters == null)

#endif
                    return System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption.Value);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Files);
#endif
            }
        }
    }

    public class EnumerablePath<T> : IEnumerablePath<T> where T : IPathInfo
    {
        public string Path { get; }

        protected Func<IPathInfo, T> GetNewPathInfoDelegate { get; }

        public EnumerablePath(in string path, in Func<IPathInfo, T> getNewPathInfoDelegate)
        {
            Path = path;

            GetNewPathInfoDelegate = getNewPathInfoDelegate;
        }

        public System.Collections.Generic.IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => EnumerablePath.GetFileSystemEntryEnumerable(Path, searchPattern, searchOption
#if NETCORE
               , enumerationOptions
#endif
#if DEBUG
                , simulationParameters
#endif
                );

        public System.Collections.Generic.IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => EnumerablePath.GetDirectoryEnumerable(Path, searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
#if DEBUG
                , simulationParameters
#endif
                );

        public System.Collections.Generic.IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => EnumerablePath.GetFileEnumerable(Path, searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
#if DEBUG
                , simulationParameters
#endif
                );

        public Enumerator GetEnumerator(in string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => new Enumerator(this, searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
                , enumerationOrder
#if DEBUG
                , simulationParameters
#endif
);

#if WinCopies2
        IDisposableEnumeratorInfo
#else
        Collections.Generic.IEnumeratorInfo2
#endif
            <T> IEnumerablePath<T>.GetEnumerator(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => GetEnumerator(searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
                , enumerationOrder
#if DEBUG
                , simulationParameters
#endif
                );

        protected virtual System.Collections.Generic.IEnumerator<T> GetEnumerator() => GetEnumerator(null, null
#if NETCORE
            , null
#endif
            , FileSystemEntryEnumerationOrder.None
#if DEBUG
            , null
#endif
            );

        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(null, null
#if NETCORE
            , null
#endif
            , FileSystemEntryEnumerationOrder.None
#if DEBUG
            , null
#endif
            );

        public sealed class Enumerator :
#if WinCopies2
            IDisposableEnumeratorInfo
#else
            WinCopies.Collections.Generic.Enumerator
#endif
            <T>
        {
            private SubEnumerator[] _enumerators;
            private SubEnumerator _currentEnumerator;
            private Func<bool> _moveNext;
            private T _current;
            internal const bool _isResetSupported = false;
            private Func<IPathInfo, T> _func;

#if WinCopies2
            public T Current => this.IsEnumeratorNotStartedOrDisposed() ? throw ThrowHelper.GetEnumeratorNotStartedOrDisposedException() : _current;

            public bool IsStarted { get; private set; }

            public bool IsCompleted { get; private set; }

            object IEnumerator.Current => Current;

            public bool IsDisposed { get; private set; }
#else
            protected override T CurrentOverride => _current;
#endif

            public
#if !WinCopies2
                override
#endif
                bool? IsResetSupported => _isResetSupported;

            // public int Count => IsDisposed ? throw GetExceptionForDispose(false) : _directoryEnumerator.Count + _filesEnumerator.Count;

#if DEBUG

            public FileSystemEntryEnumeratorProcessSimulation SimulationParameters { get; }

#endif

            private class SubEnumerator
            {
                public System.Collections.Generic.IEnumerator<string> Enumerator { get; }

                public PathType PathType { get; }

                public SubEnumerator(System.Collections.Generic.IEnumerator<string> enumerator, PathType pathType)
                {
                    Enumerator = enumerator;

                    PathType = pathType;
                }
            }

            internal Enumerator(EnumerablePath<T> enumerablePath, string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                )
            {
                Debug.Assert(enumerablePath != null);

#if WinCopies2
enumerationOrder.
#endif
                ThrowIfNotValidEnumValue(
#if !WinCopies2
                    nameof(enumerationOrder), enumerationOrder
#endif
                    );

                _func = enumerablePath.GetNewPathInfoDelegate;

                SubEnumerator getDirectoryEnumerator() => new SubEnumerator(enumerablePath.GetDirectoryEnumerable(searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
#if DEBUG
                    , simulationParameters
#endif
                    ).GetEnumerator(), PathType.Directories);

                SubEnumerator getFileEnumerator() => new SubEnumerator(enumerablePath.GetFileEnumerable(searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
#if DEBUG
                    , simulationParameters
#endif
                    ).GetEnumerator(), PathType.Files);

                void initEnumeratorArray(in int length) => _enumerators = new SubEnumerator[length];

                void updateCurrent() => _current = _func(new PathInfo(_currentEnumerator.Enumerator.Current, _currentEnumerator.PathType == PathType.Directories ? true : _currentEnumerator.PathType == PathType.Files ? false : WinCopies.IO.Path.Exists(_currentEnumerator.Enumerator.Current)));

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

                    if (_currentEnumerator.Enumerator.MoveNext())
                    {
                        updateCurrent();

                        return true;
                    }

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
#if NETCORE
                            , enumerationOptions
#endif
#if DEBUG
                            , simulationParameters
#endif
                            ).GetEnumerator(), PathType.All);

                        setMoveNext();

                        break;
                }

#if DEBUG
                SimulationParameters = simulationParameters;
#endif
            }

            public static Enumerator From(in EnumerablePath<T> enumerablePath, in string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder, Func<IPathInfo, T> getNewPathInfoDelegate
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                ) => new Enumerator(enumerablePath ?? throw GetArgumentNullException(nameof(enumerablePath)), searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
                    , enumerationOrder
#if DEBUG
                    , simulationParameters
#endif
                    );

#if WinCopies2
            public bool MoveNext
#else
            protected override bool MoveNextOverride
#endif
                ()
            {
                if (IsDisposed)

                    throw GetExceptionForDispose(false);

                if (IsCompleted)

                    return false;

#if WinCopies2
                IsStarted = true;
#endif

                if (_moveNext())

                    return true;

                _Reset();

#if WinCopies2
                IsCompleted = true;
#endif

                return false;
            }

#if WinCopies2
            public void Reset() => throw new NotSupportedException();
#endif

            private void _Reset()
            {
                _enumerators = null;
                _moveNext = null;
                _current = default;
                _func = null;
#if WinCopies2
                IsStarted = false;
#endif
            }

#if WinCopies2
            public void Dispose()
            {
#else
            protected override void DisposeManaged()
            {
                base.DisposeManaged();
#endif
#if WinCopies2
                if (IsDisposed)

                    return;
#endif

                _Reset();

#if WinCopies2
                IsCompleted = false;

                IsDisposed = true;
#endif
            }
        }
    }

    public interface IRecursivelyEnumerablePath<T> : IEnumerablePath<T>, IRecursiveEnumerable<T> where T : IPathInfo
    {
        // Left empty.
    }

    public class RecursivelyEnumerablePath<T> : EnumerablePath<T>, IRecursivelyEnumerablePath<T> where T : IPathInfo
    {
        private readonly Func<Enumerator> _getRecursiveEnumeratorDelegate;

        public T Value { get; }

        public RecursivelyEnumerablePath(in T path, string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            , in Func<IPathInfo, T> getNewPathInfoDelegate) : base((path
#if CS8
            ??
#else
            == null ? 
#endif
            throw GetArgumentNullException(nameof(path))
#if !CS8
            : path
#endif
            ).Path, getNewPathInfoDelegate)
        {
            Value = path;

            _getRecursiveEnumeratorDelegate = () => new Enumerator(this, searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
                , enumerationOrder
#if DEBUG
                , simulationParameters
#endif
                );
        }

        IEnumerator<WinCopies.Collections.Generic.IRecursiveEnumerable<T>> IRecursiveEnumerableProviderEnumerable<T>.GetRecursiveEnumerator() => _getRecursiveEnumeratorDelegate();

        RecursiveEnumerator<T> IRecursiveEnumerable<T>.GetEnumerator() => new RecursiveEnumerator<T>(this);

        IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public sealed class Enumerator :
#if WinCopies2
            IDisposableEnumeratorInfo<IRecursivelyEnumerablePath<T>>
#else
            Enumerator<T, IRecursivelyEnumerablePath<T>>
#endif
        {
            private RecursivelyEnumerablePath<T> _path;
            private IRecursivelyEnumerablePath<T> _current;
            private readonly Func<RecursivelyEnumerablePath<T>> _getNewRecursivelyEnumerablePathDelegate;

#if WinCopies2
            private IEnumeratorInfo<T> _enumerator;

            public bool IsDisposed { get; private set; }

            public IRecursivelyEnumerablePath<T> Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

            object IEnumerator.Current => Current;

            public bool IsStarted => _enumerator.IsStarted;

            public bool IsCompleted { get; private set; }
#else
            protected override IRecursivelyEnumerablePath<T> CurrentOverride => _current;
#endif

            public
#if !WinCopies2
                override
#endif
                bool? IsResetSupported => EnumerablePath<T>.Enumerator._isResetSupported;

            internal Enumerator(in RecursivelyEnumerablePath<T> enumerablePath, string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                )
#if !WinCopies2
                : base(

                ((IRecursivelyEnumerablePath<T>)enumerablePath).GetEnumerator(searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
                    , enumerationOrder
#if DEBUG
                    , simulationParameters
#endif
                    ))
#endif
            {
                _path = enumerablePath;

#if WinCopies2
                _enumerator = ((IRecursivelyEnumerablePath<T>)enumerablePath).GetEnumerator(searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
                    , enumerationOrder
#if DEBUG
                    , simulationParameters
#endif
                    );
#endif

                _getNewRecursivelyEnumerablePathDelegate = () => new RecursivelyEnumerablePath<T>(
#if WinCopies2
                    _enumerator
#else
                    InnerEnumerator
#endif
                    .Current, searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
                    , enumerationOrder
#if DEBUG
                    , simulationParameters
#endif
                    , _path.GetNewPathInfoDelegate);
            }

            public static Enumerator From(in RecursivelyEnumerablePath<T> enumerablePath, in string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                ) => new Enumerator(enumerablePath ?? throw GetArgumentNullException(nameof(enumerablePath)), searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
                    , enumerationOrder
#if DEBUG
                    , simulationParameters
#endif
                    );

#if WinCopies2
            public bool MoveNext()
            {
                if (IsDisposed)

                    throw GetExceptionForDispose(false);

                if (IsCompleted)

                    return false;
#else
            protected override bool MoveNextOverride()
            {
#endif
                if (_path.Value.IsDirectory &&
#if WinCopies2
                    _enumerator
#else
                    InnerEnumerator
#endif
                    .MoveNext())
                {
                    _current = _getNewRecursivelyEnumerablePathDelegate();

                    return true;
                }

                _current = null;

#if WinCopies2
                IsCompleted = true;
#endif

                return false;
            }

#if WinCopies2
            public void Reset() => throw new InvalidOperationException("Reset is not supported.");

            public void Dispose()
            {
                if (IsDisposed)

                    return;
#else
            protected override void DisposeManaged()
            {
                base.DisposeManaged();
#endif
                _current = null;

#if WinCopies2
                _enumerator = null;
#endif

                _path = null;

#if WinCopies2
                IsCompleted = false;

                IsDisposed = true;
#endif
            }
        }
    }
}
