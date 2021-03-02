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
using System.Collections.Generic;
using System.IO;

using WinCopies.IO;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.IO.Enumeration;

#if WinCopies3
using WinCopies;

using static WinCopies.ThrowHelper;
using static WinCopies.Collections.ThrowHelper;
#else
using WinCopies.Util;

using static WinCopies.Util.Util;
#endif

namespace WinCopies.GUI.IO.Process
{
    public abstract class PathCollection<T> : ICollection<T>, IList<T> where T : WinCopies.IO.IPathInfo
    {
        protected IList<T> InnerList { get; }

        public string Path { get; }

        public int Count => InnerList.Count;

        public bool IsReadOnly => false;

        protected virtual void SetItem(int index, T item)
        {
            ValidatePath(item);

            InnerList[index] = item;
        }

        public T this[int index] { get => InnerList[index]; set => SetItem(index, value); }

        protected abstract Func<T> GetNewEmptyEnumeratorPathInfoDelegate { get; }

        protected abstract Func<T, T> GetNewEnumeratorPathInfoDelegate { get; }

        protected abstract Func<WinCopies.IO.IPathInfo, T> GetNewPathInfoDelegate { get; }

        public abstract Func<T, Size?> GetPathSizeDelegate { get; }

        public string GetConcatenatedPath(WinCopies.IO.IPathInfo pathInfo) => pathInfo == null ? throw GetArgumentNullException(nameof(pathInfo)) : $"{Path}{WinCopies.IO.Path.PathSeparator}{pathInfo.Path}";

        protected PathCollection(string path) : this(path, new List<T>()) { }

        protected PathCollection(string path, IList<T> list)
        {
            Path = path == null || path.Length == 0 ? string.Empty : System.IO.Path.IsPathRooted(path) ? path : throw new ArgumentException($"{nameof(path)} must be null, empty or rooted.");

            ThrowIfNull(list, nameof(list));

            foreach (T _path in list)

                ValidatePath(_path);

            InnerList = list;
        }

        protected virtual void ValidatePath(T item)
        {
            if (item == null) throw GetArgumentNullException(nameof(item));

            if ((Path.Length > 1 && System.IO.Path.IsPathRooted(item.Path)) || (Path.Length == 0 && !System.IO.Path.IsPathRooted(item.Path)))

                throw new ArgumentException("The path to add must be relative.");
        }

        public void Add(T item) => InsertItem(Count, item);

        protected virtual void InsertItem(int index, T item)
        {
            ValidatePath(item);

            InnerList.Add(item);
        }

        public void Clear() => ClearItems();

        protected virtual void ClearItems() => InnerList.Clear();

        public bool Contains(T item) => InnerList.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => InnerList.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            int itemIndex = InnerList.IndexOf(item);

            if (itemIndex == -1)

                return false;

            RemoveItemAt(itemIndex);

            return true;
        }

        protected virtual void RemoveItemAt(int index) => InnerList.RemoveAt(index);

        public PathCollectionEnumerator GetTopLevelPathsEnumerator(in FileSystemEntryEnumerationOrder enumerationOrder) => new PathCollectionEnumerator(this, enumerationOrder);

        public IEnumerable<T> Enumerate(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
            , FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => WinCopies.IO.Directory.Enumerate(this, null, null
#if NETCORE
                , null
#endif
                , FileSystemEntryEnumerationOrder.FilesThenDirectories, GetNewPathInfoDelegate
#if DEBUG
                    , simulationParameters
#endif
                    );

        public System.Collections.Generic.IEnumerator<T> GetEnumerator(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
            , FileSystemEntryEnumerationOrder enumerationOrder
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => Enumerate(searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
                , enumerationOrder
#if DEBUG
                , simulationParameters
#endif
                ).GetEnumerator();

        System.Collections.Generic.IEnumerator<T> IEnumerable<T>.GetEnumerator() => WinCopies.IO.Directory.Enumerate(this, null, null
#if NETCORE
            , null
#endif
            , FileSystemEntryEnumerationOrder.None, GetNewPathInfoDelegate
#if DEBUG
            , null
#endif
            ).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => WinCopies.IO.Directory.Enumerate(this, null, null
#if NETCORE
            , null
#endif
            , FileSystemEntryEnumerationOrder.None, GetNewPathInfoDelegate
#if DEBUG
            , null
#endif
            ).GetEnumerator();

        public int IndexOf(T item) => InnerList.IndexOf(item);

        public void Insert(int index, T item) => InsertItem(index, item);

        public void RemoveAt(int index) => InnerList.RemoveAt(index);

        public class PathCollectionEnumerator : WinCopies.Collections.
#if WinCopies3
            Generic.
#endif
            Enumerator<T, T>
        {
            private PathCollection<T> _pathCollection;
            private
#if !WinCopies3
IQueue
#else
                IQueueBase
#endif
                <T> _queue;
            private readonly FileSystemEntryEnumerationOrder _enumerationOrder;
            private Func<bool> _moveNext;
#if !WinCopies3
            private bool _completed = false;
#else
            private T _current;

            protected override T CurrentOverride => _current;

            public override bool? IsResetSupported => null;
#endif

            internal PathCollectionEnumerator(in PathCollection<T> pathCollection, in FileSystemEntryEnumerationOrder enumerationOrder) : base(pathCollection.InnerList)
            {
                _pathCollection = pathCollection;

#if !WinCopies3
enumerationOrder.
#endif
                ThrowIfNotValidEnumValue(nameof(enumerationOrder)
                    #if WinCopies3
                    , enumerationOrder
                    #endif
                    );

                if ((_enumerationOrder = enumerationOrder) != FileSystemEntryEnumerationOrder.None)

                    _queue =
#if !WinCopies3
                        new WinCopies.Collections.Generic.Queue<T>
#else
                        WinCopies.Collections.Generic.EnumerableHelper<T>.GetQueue
#endif
                        ();
            }

            public static PathCollectionEnumerator From(in PathCollection<T> pathCollection, in FileSystemEntryEnumerationOrder enumerationOrder) => new PathCollectionEnumerator(pathCollection ?? throw GetArgumentNullException(nameof(pathCollection)), enumerationOrder);

#if !WinCopies3
            protected override void ResetOverride()
            {
                base.ResetOverride();

                _completed = false;
            }
#endif

            protected override void
#if !WinCopies3
                Dispose(bool disposing)
#else
                DisposeManaged()
#endif
            {
                _pathCollection = null;

                _queue = null;

                _moveNext = null;

                base.
#if !WinCopies3
                    Dispose(disposing
#else
                DisposeManaged(
#endif
                    );
            }

            protected override bool MoveNextOverride()
            {
#if !WinCopies3
                if (_completed) return false;
#endif

                if (_pathCollection.Count == 0)
                {

#if !WinCopies3
Current
#else
                    _current
#endif
                             = _pathCollection.GetNewEmptyEnumeratorPathInfoDelegate();

#if !WinCopies3
                    _completed = true;
#endif

                    return true;
                }

                void updateCurrentWithInnerEnumeratorValue() =>
#if !WinCopies3
Current
#else
                        _current
#endif
                             = _pathCollection.GetNewEnumeratorPathInfoDelegate(InnerEnumerator.Current);

                bool moveNextNone()
                {
                    if (InnerEnumerator.MoveNext())
                    {
                        updateCurrentWithInnerEnumeratorValue();

                        return true;
                    }

                    return false;
                }

                bool moveNext()
                {
                    while (InnerEnumerator.MoveNext())
                    {
                        if ((_enumerationOrder == FileSystemEntryEnumerationOrder.FilesThenDirectories) == InnerEnumerator.Current.IsDirectory)

                            _queue.Enqueue(InnerEnumerator.Current);

                        else
                        {
                            updateCurrentWithInnerEnumeratorValue();

                            return true;
                        }
                    }

                    if (_queue.
#if !WinCopies3
                        Count != 0
#else
                        HasItems
#endif
                        )
                    {
#if !WinCopies3
Current
#else
                        _current
#endif
                            = _pathCollection.GetNewEnumeratorPathInfoDelegate(_queue.Dequeue());

                        return true;
                    }

                    return false;
                }

                if (_moveNext == null)

                    switch (_enumerationOrder)
                    {
                        case FileSystemEntryEnumerationOrder.None:

                            _moveNext = moveNextNone;

                            break;

                        case FileSystemEntryEnumerationOrder.FilesThenDirectories:

                            _moveNext = moveNext;

                            break;
                    }

                if (_moveNext())

                    return true;

                _moveNext = null;

#if !WinCopies3
                _completed = true;
#endif

                return false;
            }
        }
    }
}
