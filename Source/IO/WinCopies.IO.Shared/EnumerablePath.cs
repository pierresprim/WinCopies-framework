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
using System.Linq;

using WinCopies.Collections.Generic;
using WinCopies.IO.Enumeration;

using static WinCopies.ThrowHelper;

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
        IPathInfo Path { get; }

        System.Collections.Generic.IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
            , bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );

        System.Collections.Generic.IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
            , bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );

        System.Collections.Generic.IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
            , bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );

#if !WinCopies3
        WinCopies.Collections.Generic.IDisposableEnumeratorInfo
#else
        Collections.Generic.IEnumeratorInfo2
#endif
            <T> GetEnumerator(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder, bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            );
    }

    public static class EnumerablePath
    {
        public static System.Collections.Generic.IEnumerable<string> GetFileSystemEntryEnumerable(in string path, string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
            , in bool safeEnumeration
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            )
        {
            if (string.IsNullOrEmpty(searchPattern) && searchOption == null
#if CS8
                     && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return safeEnumeration ? Directory.EnumerateFileSystemEntriesIOSafe(path) : System.IO.Directory.EnumerateFileSystemEntries(path);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.All);
#endif
            }

            else if (searchPattern != null && searchOption == null
#if CS8
                    && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return safeEnumeration ? Directory.EnumerateFileSystemEntriesIOSafe(path, searchPattern) : System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.All);
#endif
            }

#if CS8
            else if (searchOption == null)
            {
                if (searchPattern == null)

                    searchPattern = "";
#if DEBUG
                if (simulationParameters == null)
#endif
                    return safeEnumeration ? Directory.EnumerateFileSystemEntriesIOSafe(path, searchPattern, enumerationOptions) : System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);
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
                    return safeEnumeration ? Directory.EnumerateFileSystemEntriesIOSafe(path, searchPattern, searchOption.Value) : System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption.Value);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.All);
#endif
            }
        }

        public static System.Collections.Generic.IEnumerable<string> GetDirectoryEnumerable(in string path, string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
            , in bool safeEnumeration
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            )
        {
            if (string.IsNullOrEmpty(searchPattern) && searchOption == null
#if CS8
                     && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return safeEnumeration ? Directory.EnumerateDirectoriesIOSafe(path) : System.IO.Directory.EnumerateDirectories(path);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
#endif
            }

            else if (searchPattern != null && searchOption == null
#if CS8
                    && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return safeEnumeration ? Directory.EnumerateDirectoriesIOSafe(path, searchPattern) : System.IO.Directory.EnumerateDirectories(path, searchPattern);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
#endif
            }

#if CS8
            else if (searchOption == null)
            {
                if (searchPattern == null)

                    searchPattern = "";
#if DEBUG
                if (simulationParameters == null)
#endif
                    return safeEnumeration ? Directory.EnumerateDirectoriesIOSafe(path, searchPattern, enumerationOptions) : System.IO.Directory.EnumerateDirectories(path, searchPattern, enumerationOptions);
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
                    return safeEnumeration ? Directory.EnumerateDirectoriesIOSafe(path, searchPattern, searchOption.Value) : System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOption.Value);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
#endif
            }
        }

        public static System.Collections.Generic.IEnumerable<string> GetFileEnumerable(in string path, string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
            , in bool safeEnumeration
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            )
        {
            if (string.IsNullOrEmpty(searchPattern) && searchOption == null
#if CS8
                     && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return safeEnumeration ? Directory.EnumerateFilesIOSafe(path) : System.IO.Directory.EnumerateFiles(path);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Files);
#endif
            }

            else if (searchPattern != null && searchOption == null
#if CS8
                    && enumerationOptions == null
#endif
                    )
            {
#if DEBUG
                if (simulationParameters == null)
#endif
                    return safeEnumeration ? Directory.EnumerateFilesIOSafe(path, searchPattern) : System.IO.Directory.EnumerateFiles(path, searchPattern);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Files);
#endif
            }

#if CS8
            else if (searchOption == null)
            {
                if (searchPattern == null)

                    searchPattern = "";
#if DEBUG
                if (simulationParameters == null)
#endif
                    return safeEnumeration ? Directory.EnumerateFilesIOSafe(path, searchPattern, enumerationOptions) : System.IO.Directory.EnumerateFiles(path, searchPattern, enumerationOptions);
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
                    return safeEnumeration ? Directory.EnumerateFilesIOSafe(path, searchPattern, searchOption.Value) : System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption.Value);
#if DEBUG

                else

                    return simulationParameters.EnumerateFunc(path, PathType.Files);
#endif
            }
        }
    }

    public class EnumerablePath<T> : IEnumerablePath<T> where T : IPathInfo
    {
        public IPathInfo Path { get; }

        protected Func<IPathInfo, T> GetNewPathInfoDelegate { get; }

        public EnumerablePath(in IPathInfo path, in Func<IPathInfo, T> getNewPathInfoDelegate)
        {
            Path = path;

            GetNewPathInfoDelegate = getNewPathInfoDelegate;
        }

        public System.Collections.Generic.IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
            , bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => EnumerablePath.GetFileSystemEntryEnumerable(Path.Path, searchPattern, searchOption
#if CS8
               , enumerationOptions
#endif
                , safeEnumeration
#if DEBUG
                , simulationParameters
#endif
                );

        public System.Collections.Generic.IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => EnumerablePath.GetDirectoryEnumerable(Path.Path, searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , safeEnumeration
#if DEBUG
                , simulationParameters
#endif
                );

        public System.Collections.Generic.IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => EnumerablePath.GetFileEnumerable(Path.Path, searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , safeEnumeration
#if DEBUG
                , simulationParameters
#endif
                );

        public Enumerator GetEnumerator(in string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder, in bool safeEnumeration
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => new
#if !CS9
            Enumerator
#endif
            (this, searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , enumerationOrder, safeEnumeration
#if DEBUG
                , simulationParameters
#endif
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
                , FileSystemEntryEnumerationOrder enumerationOrder, bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => GetEnumerator(searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , enumerationOrder, safeEnumeration
#if DEBUG
                , simulationParameters
#endif
                );

        protected virtual System.Collections.Generic.IEnumerator<T> GetEnumerator() => GetEnumerator(null, null
#if CS8
            , null
#endif
            , FileSystemEntryEnumerationOrder.None, true
#if DEBUG
            , null
#endif
            );

        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(null, null
#if CS8
            , null
#endif
            , FileSystemEntryEnumerationOrder.None, true
#if DEBUG
            , null
#endif
            );

        public sealed class Enumerator : WinCopies.Collections.Generic.Enumerator<T>
        {
            private SubEnumerator[] _enumerators;
            private SubEnumerator _currentEnumerator;
            private Func<bool> _moveNext;
            private T _current;
            internal const bool _isResetSupported = false;
            private Func<IPathInfo, T> _func;

            protected override T CurrentOverride => _current;

            public override bool? IsResetSupported => _isResetSupported;

            // public int Count => IsDisposed ? throw GetExceptionForDispose(false) : _directoryEnumerator.Count + _filesEnumerator.Count;

#if DEBUG

            public FileSystemEntryEnumeratorProcessSimulation SimulationParameters { get; }

#endif

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
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                )
            {
                PathTypes<IPathInfo>.IPathCommon getPathData(string path) => new PathTypes<IPathInfo>.PathInfoCommon(System.IO.Path.GetFileName(path), new NullableGeneric<IPathInfo>(enumerablePath.Path));

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
#if DEBUG
                    , simulationParameters
#endif
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
#if DEBUG
                    , simulationParameters
#endif
                    ).Select(getPathData).GetEnumerator(), PathType.Files);

                void initEnumeratorArray(in int length) => _enumerators = new SubEnumerator[length];

                void updateCurrent() => _current = _func(new PathTypes<IPathInfo>.PathInfo(_currentEnumerator.Enumerator.Current, _currentEnumerator.PathType == PathType.Directories || (_currentEnumerator.PathType == PathType.Files ? false : IO.Path.Exists(_currentEnumerator.Enumerator.Current.Path))));

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
#if CS8
                            , enumerationOptions
#endif
                            , safeEnumeration
#if DEBUG
                            , simulationParameters
#endif
                            ).Select(getPathData).GetEnumerator(), PathType.All);

                        setMoveNext();

                        break;
                }

#if DEBUG
                SimulationParameters = simulationParameters;
#endif
            }

            public static Enumerator From(in EnumerablePath<T> enumerablePath, in string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder, in Func<PathTypes<IPathInfo>.IPathInfo, T> getNewPathInfoDelegate, in bool safeEnumeration
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                ) => new
#if !CS9
            Enumerator
#endif
            (enumerablePath ?? throw GetArgumentNullException(nameof(enumerablePath)), searchPattern, searchOption
#if CS8
                    , enumerationOptions
#endif
                    , enumerationOrder, safeEnumeration
#if DEBUG
                    , simulationParameters
#endif
                    );

            protected override bool MoveNextOverride()
            {
                if (IsDisposed)

                    throw GetExceptionForDispose(false);

                if (IsCompleted)

                    return false;

                if (_moveNext())

                    return true;

                _Reset();

                return false;
            }

            private void _Reset()
            {
                _enumerators = null;
                _moveNext = null;
                _current = default;
                _func = null;
            }

            protected override void DisposeManaged()
            {
                base.DisposeManaged();

                _Reset();
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
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder, in Func<IPathInfo, T> getNewPathInfoDelegate, bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
           ) : base((path
#if CS8
            ??
#else
            == null ? 
#endif
            throw GetArgumentNullException(nameof(path))
#if !CS8
            : path
#endif
            ), getNewPathInfoDelegate)
        {
            Value = path;

            _getRecursiveEnumeratorDelegate = () => new Enumerator(this, searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , enumerationOrder, safeEnumeration
#if DEBUG
                , simulationParameters
#endif
                );
        }

        IEnumerator<WinCopies.Collections.Generic.IRecursiveEnumerable<T>> IRecursiveEnumerableProviderEnumerable<T>.GetRecursiveEnumerator() => _getRecursiveEnumeratorDelegate();

        RecursiveEnumerator<T> IRecursiveEnumerable<T>.GetEnumerator() => new
#if !CS9
            RecursiveEnumerator<T>
#endif
            (this);

        IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public sealed class Enumerator : Enumerator<T, IRecursivelyEnumerablePath<T>>
        {
            private RecursivelyEnumerablePath<T> _path;
            private IRecursivelyEnumerablePath<T> _current;
            private readonly Func<RecursivelyEnumerablePath<T>> _getNewRecursivelyEnumerablePathDelegate;

            protected override IRecursivelyEnumerablePath<T> CurrentOverride => _current;

            public
#if WinCopies3
                override
#endif
                bool? IsResetSupported => EnumerablePath<T>.Enumerator._isResetSupported;

            internal Enumerator(in RecursivelyEnumerablePath<T> enumerablePath, string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder, bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                )
#if WinCopies3
                : base(

                ((IRecursivelyEnumerablePath<T>)enumerablePath).GetEnumerator(searchPattern, searchOption
#if CS8
                    , enumerationOptions
#endif
                    , enumerationOrder, safeEnumeration
#if DEBUG
                    , simulationParameters
#endif
                    ))
#endif
            {
                _path = enumerablePath;

                _getNewRecursivelyEnumerablePathDelegate = () => new RecursivelyEnumerablePath<T>(InnerEnumerator.Current, searchPattern, searchOption
#if CS8
                    , enumerationOptions
#endif
                    , enumerationOrder, _path.GetNewPathInfoDelegate, safeEnumeration
#if DEBUG
                    , simulationParameters
#endif
                    );
            }

            public static Enumerator From(in RecursivelyEnumerablePath<T> enumerablePath, in string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
                , in FileSystemEntryEnumerationOrder enumerationOrder, in bool safeEnumeration
#if DEBUG
            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
                ) => new
#if !CS9
            Enumerator
#endif
            (enumerablePath ?? throw GetArgumentNullException(nameof(enumerablePath)), searchPattern, searchOption
#if CS8
                    , enumerationOptions
#endif
                    , enumerationOrder, safeEnumeration
#if DEBUG
                    , simulationParameters
#endif
                    );

            protected override bool MoveNextOverride()
            {
                if (_path.Value.IsDirectory && InnerEnumerator.MoveNext())
                {
                    _current = _getNewRecursivelyEnumerablePathDelegate();

                    return true;
                }

                _current = null;

                return false;
            }

            protected override void DisposeManaged()
            {
                base.DisposeManaged();

                _current = null;

                _path = null;
            }
        }
    }
}
