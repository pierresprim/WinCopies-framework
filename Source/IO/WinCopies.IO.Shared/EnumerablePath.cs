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
 * along with the WinCopies Framework.If not, see<https://www.gnu.org/licenses/>. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using WinCopies.Collections.Generic;
using WinCopies.Util;

using static WinCopies.Util.Util;

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

    public interface IEnumerablePath<T> : IEnumerable<T> where T : IPathInfo
    {
        string Path { get; }

        IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );

        IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );

        IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );

        WinCopies.Collections.Generic.IDisposableEnumeratorInfo<T> GetEnumerator(string searchPattern, SearchOption? searchOption
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
        public static IEnumerable<string> GetFileSystemEntryEnumerable(in string path, string searchPattern, in SearchOption? searchOption
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

        public static IEnumerable<string> GetDirectoryEnumerable(in string path, string searchPattern, in SearchOption? searchOption
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

        public static IEnumerable<string> GetFileEnumerable(in string path, string searchPattern, in SearchOption? searchOption
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

        public IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
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

        public IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
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

        public IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption
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

        IDisposableEnumeratorInfo<T> IEnumerablePath<T>.GetEnumerator(string searchPattern, SearchOption? searchOption
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

        System.Collections.Generic.IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(null, null
#if NETCORE
            , null
#endif
            , FileSystemEntryEnumerationOrder.None
#if DEBUG
            , null
#endif
            );

        public sealed class Enumerator : IDisposableEnumeratorInfo<T>
        {
            private SubEnumerator[] _enumerators;
            private SubEnumerator _currentEnumerator;
            private Func<bool> _moveNext;
            private T _current;
            internal const bool _isResetSupported = false;
            private Func<IPathInfo, T> _func;

            public bool IsStarted { get; private set; }

            public bool IsCompleted { get; private set; }

            public T Current => this.IsEnumeratorNotStartedOrDisposed() ? throw ThrowHelper.GetEnumeratorNotStartedOrDisposedException() : _current;

            object IEnumerator.Current => Current;

            public bool? IsResetSupported => _isResetSupported;

            // public int Count => IsDisposed ? throw GetExceptionForDispose(false) : _directoryEnumerator.Count + _filesEnumerator.Count;

#if DEBUG

            public FileSystemEntryEnumeratorProcessSimulation SimulationParameters { get; }

#endif

            public bool IsDisposed { get; private set; }

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

                enumerationOrder.ThrowIfNotValidEnumValue();

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

            public bool MoveNext()
            {
                if (IsDisposed)

                    throw GetExceptionForDispose(false);

                if (IsCompleted)

                    return false;

                IsStarted = true;

                if (_moveNext())

                    return true;

                _Reset();

                IsCompleted = true;

                return false;
            }

            public void Reset() => throw new NotSupportedException();

            private void _Reset()
            {
                _enumerators = null;
                _moveNext = null;
                _current = default;
                _func = null;
                IsStarted = false;
            }

            public void Dispose()
            {
                if (IsDisposed)

                    return;

                _Reset();

                IsCompleted = false;

                IsDisposed = true;
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
#if !CS7
            ??
#else
            == null ? 
#endif
            throw GetArgumentNullException(nameof(path))
#if CS7
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

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public sealed class Enumerator : IDisposableEnumeratorInfo<IRecursivelyEnumerablePath<T>>
        {
            private IEnumeratorInfo<T> _enumerator;
            private RecursivelyEnumerablePath<T> _path;
            private IRecursivelyEnumerablePath<T> _current;
            private readonly Func<RecursivelyEnumerablePath<T>> _getNewRecursivelyEnumerablePathDelegate;

            public bool IsDisposed { get; private set; }

            public IRecursivelyEnumerablePath<T> Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

            object IEnumerator.Current => Current;

            public bool? IsResetSupported => EnumerablePath<T>.Enumerator._isResetSupported;

            public bool IsStarted => _enumerator.IsStarted;

            public bool IsCompleted { get; private set; }

            internal Enumerator(in RecursivelyEnumerablePath<T> enumerablePath, string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                )
            {
                _path = enumerablePath;

                _enumerator = ((IRecursivelyEnumerablePath<T>)enumerablePath).GetEnumerator(searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
                    , enumerationOrder
#if DEBUG
                    , simulationParameters
#endif
                    );

                _getNewRecursivelyEnumerablePathDelegate = () => new RecursivelyEnumerablePath<T>(_enumerator.Current, searchPattern, searchOption
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

            public bool MoveNext()
            {
                if (IsDisposed)

                    throw GetExceptionForDispose(false);

                if (IsCompleted)

                    return false;

                if (_path.Value.IsDirectory && _enumerator.MoveNext())
                {
                    _current = _getNewRecursivelyEnumerablePathDelegate();

                    return true;
                }

                _current = null;

                IsCompleted = true;

                return false;
            }

            public void Reset() => throw new InvalidOperationException("Reset is not supported.");

            public void Dispose()
            {
                if (IsDisposed)

                    return;

                _current = null;

                _enumerator = null;

                _path = null;

                IsCompleted = false;

                IsDisposed = true;
            }
        }
    }
}
