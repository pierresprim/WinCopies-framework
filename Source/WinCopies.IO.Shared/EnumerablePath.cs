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
        None = 0,

        FilesThenDirectories = 1,

        DirectoriesThenFiles = 2
    }

    public interface IEnumerablePath : IEnumerable<IPathInfo>
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

        WinCopies.Collections.Generic.IDisposableEnumeratorInfo<IPathInfo> GetEnumerator(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );
    }

    public class EnumerablePath : IEnumerablePath
    {
        public string Path { get; }

        public EnumerablePath(string path) => Path = path;

        public IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
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
                    return System.IO.Directory.EnumerateFileSystemEntries(Path);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.All);
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
                    return System.IO.Directory.EnumerateFileSystemEntries(Path, searchPattern);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.All);
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
                    return System.IO.Directory.EnumerateFileSystemEntries(Path, searchPattern, enumerationOptions);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.All);
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
                    return System.IO.Directory.EnumerateFileSystemEntries(Path, searchPattern, searchOption.Value);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.All);
#endif
            }
        }

        public IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
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
                    return System.IO.Directory.EnumerateDirectories(Path);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.Directories);
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
                    return System.IO.Directory.EnumerateDirectories(Path, searchPattern);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.Directories);
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
                    return System.IO.Directory.EnumerateDirectories(Path, searchPattern, enumerationOptions);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.Directories);
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
                    return System.IO.Directory.EnumerateDirectories(Path, searchPattern, searchOption.Value);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.Directories);
#endif
            }
        }

        public IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
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
                    return System.IO.Directory.EnumerateFiles(Path);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.Files);
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
                    return System.IO.Directory.EnumerateFiles(Path, searchPattern);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.Files);
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
                    return System.IO.Directory.EnumerateFiles(Path, searchPattern, enumerationOptions);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.Files);
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
                    return System.IO.Directory.EnumerateFiles(Path, searchPattern, searchOption.Value);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(Path, PathType.Files);
#endif
            }
        }

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

        WinCopies.Collections.Generic.IDisposableEnumeratorInfo<IPathInfo> IEnumerablePath.GetEnumerator(string searchPattern, SearchOption? searchOption
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

        System.Collections.Generic.IEnumerator<IPathInfo> IEnumerable<IPathInfo>.GetEnumerator() => GetEnumerator(null, null
#if NETCORE
            , null
#endif
            , FileSystemEntryEnumerationOrder.None
#if DEBUG
            , null
#endif
            );

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(null, null
#if NETCORE
            , null
#endif
            , FileSystemEntryEnumerationOrder.None
#if DEBUG
            , null
#endif
            );

        public sealed class Enumerator : WinCopies.Collections.Generic.IDisposableEnumeratorInfo<IPathInfo>
        {
            private SubEnumerator[] _enumerators;
            private SubEnumerator _currentEnumerator;
            private Func<bool> _moveNext;
            private IPathInfo _current;
            internal const bool _isResetSupported = false;

            public bool IsStarted { get; private set; }

            public bool IsCompleted { get; private set; }

            public IPathInfo Current => WinCopies.Util._Util.IsEnumeratorNotStartedOrDisposed(this) ? throw _ThrowHelper.GetEnumeratorNotStartedOrDisposedException() : _current;

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

            internal Enumerator(IEnumerablePath enumerablePath, string searchPattern, SearchOption? searchOption
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

                void updateCurrent() => _current = new PathInfo(_currentEnumerator.Enumerator.Current, _currentEnumerator.PathType == PathType.Directories ? true : _currentEnumerator.PathType == PathType.Files ? false : WinCopies.IO.Path.Exists(_currentEnumerator.Enumerator.Current));

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

            public static Enumerator From(in IEnumerablePath enumerablePath, in string searchPattern, in SearchOption? searchOption
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
                _current = null;
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

    public interface IRecursivelyEnumerablePath : IEnumerablePath, WinCopies.Collections.Generic.ITreeEnumerable<IPathInfo>
    {
        IPathInfo Value { get; }
    }

    public class RecursivelyEnumerablePath : EnumerablePath, IRecursivelyEnumerablePath
    {
        public IPathInfo Value { get; }

        public RecursivelyEnumerablePath(in IPathInfo path) : base((path ?? throw GetArgumentNullException(nameof(path))).Path) => Value = path;

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

        IEnumerator<ITreeEnumerable<IPathInfo>> IEnumerable<ITreeEnumerable<IPathInfo>>.GetEnumerator() => GetEnumerator(null, null
#if NETCORE
            , null
#endif
            , FileSystemEntryEnumerationOrder.None
#if DEBUG
            , null
#endif
            );

        public sealed class Enumerator : WinCopies.Collections.Generic.IDisposableEnumeratorInfo<ITreeEnumerable<IPathInfo>>
        {
            private WinCopies.Collections.Generic.IEnumeratorInfo<IPathInfo> _enumerator;
            private IRecursivelyEnumerablePath _path;
            private ITreeEnumerable<IPathInfo> _current;

            public bool IsDisposed { get; private set; }

            public ITreeEnumerable<IPathInfo> Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

            object IEnumerator.Current => Current;

            public bool? IsResetSupported => EnumerablePath.Enumerator._isResetSupported;

            public bool IsStarted => _enumerator.IsStarted;

            public bool IsCompleted { get; private set; }

            internal Enumerator(in IRecursivelyEnumerablePath enumerablePath, in string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                )
            {
                _path = enumerablePath;

                _enumerator = enumerablePath.GetEnumerator(searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
                    , enumerationOrder
#if DEBUG
                    , simulationParameters
#endif
                    );
            }

            public static Enumerator From(in IRecursivelyEnumerablePath enumerablePath, in string searchPattern, in SearchOption? searchOption
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
                    _current = new RecursivelyEnumerablePath(_enumerator.Current);

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

                IsCompleted = false;

                IsDisposed = true;
            }
        }
    }
}
