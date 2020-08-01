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
    public interface IEnumerablePath : IEnumerable<IPathInfo>
    {
        string Path { get; }

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
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );
    }

    public class EnumerablePath : IEnumerablePath
    {
        public string Path { get; }

        public EnumerablePath(string path) => Path = path;

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
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => new Enumerator(this, searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
#if DEBUG
                , simulationParameters
#endif
);

        WinCopies.Collections.Generic.IDisposableEnumeratorInfo<IPathInfo> IEnumerablePath.GetEnumerator(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => GetEnumerator(searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
#if DEBUG
                , simulationParameters
#endif
                );

        System.Collections.Generic.IEnumerator<IPathInfo> IEnumerable<IPathInfo>.GetEnumerator() => GetEnumerator(null, null
#if NETCORE
            , null
#endif
#if DEBUG
            , null
#endif
            );

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(null, null
#if NETCORE
            , null
#endif
#if DEBUG
            , null
#endif
            );

        public sealed class Enumerator : WinCopies.Collections.Generic.IDisposableEnumeratorInfo<IPathInfo>
        {
            private System.Collections.Generic.IEnumerator<string> _directoryEnumerator;
            private System.Collections.Generic.IEnumerator<string> _filesEnumerator;
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

            internal Enumerator(in IEnumerablePath enumerablePath, in string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                )
            {
                Debug.Assert(enumerablePath != null);

                ResetMoveNextDelegate();

                _directoryEnumerator = enumerablePath.GetDirectoryEnumerable(searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
#if DEBUG
                    , simulationParameters
#endif
                    ).GetEnumerator();

                _filesEnumerator = enumerablePath.GetFileEnumerable(searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
#if DEBUG
                    , simulationParameters
#endif
                    ).GetEnumerator();

#if DEBUG
                SimulationParameters = simulationParameters;
#endif
            }

            public static Enumerator From(in IEnumerablePath enumerablePath, in string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                ) => new Enumerator(enumerablePath ?? throw GetArgumentNullException(nameof(enumerablePath)), searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
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

            private void ResetMoveNextDelegate()
            {
                void updateCurrent(bool isDirectory) => _current = new PathInfo(_directoryEnumerator.Current, isDirectory);

                _moveNext = () =>
                {
                    bool __moveNext()
                    {
                        if (_directoryEnumerator.MoveNext())
                        {
                            updateCurrent(false);

                            return true;
                        }

                        return false;
                    }

                    if (_filesEnumerator.MoveNext())
                    {
                        updateCurrent(true);

                        return true;
                    }

                    if (__moveNext())
                    {
                        _moveNext = __moveNext;

                        return true;
                    }

                    return false;
                };
            }

            public void Reset() => throw new NotSupportedException();

            private void _Reset()
            {
                _directoryEnumerator = null;
                _filesEnumerator = null;
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
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => new Enumerator(this, searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
#if DEBUG
                , simulationParameters
#endif
                );

        IEnumerator<ITreeEnumerable<IPathInfo>> IEnumerable<ITreeEnumerable<IPathInfo>>.GetEnumerator() => GetEnumerator(null, null
#if NETCORE
            , null
#endif
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
#if DEBUG
                    , simulationParameters
#endif
                    );
            }

            public static Enumerator From(in IRecursivelyEnumerablePath enumerablePath, in string searchPattern, in SearchOption? searchOption
#if NETCORE
            , in EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                ) => new Enumerator(enumerablePath ?? throw GetArgumentNullException(nameof(enumerablePath)), searchPattern, searchOption
#if NETCORE
                    , enumerationOptions
#endif
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
