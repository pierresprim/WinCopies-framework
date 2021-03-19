/* Copyright © Pierre Sprimont, 2021
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

using WinCopies.Collections.AbstractionInterop.Generic;
using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;

using static WinCopies.Collections.ThrowHelper;

namespace WinCopies.IO.Process
{
    public static partial class ProcessTypes<T> where T : IPath
    {
        public interface IProcessCollection : IQueue<T>
        {
            Size TotalSize { get; }
        }

        public class ProcessCollection : IProcessCollection
        {
            protected IQueue<T> InnerQueue { get; }

            public Size TotalSize { get; private set; }

            public object SyncRoot => InnerQueue.SyncRoot;

            public bool IsSynchronized => InnerQueue.IsSynchronized;

            public uint Count => InnerQueue.Count;

            public bool IsReadOnly => false;

            public bool HasItems => InnerQueue.HasItems;

            public ProcessCollection(in IQueue<T> queue) => InnerQueue = queue.IsReadOnly ? throw new ArgumentException($"{nameof(queue)} must be non-read-only.", nameof(queue)) : queue;

            protected virtual void OnClear() => TotalSize = new Size();

            public void Clear()
            {
                InnerQueue.Clear();

                OnClear();
            }

            protected virtual void OnDequeue(in T item)
            {
                if (item.Size.HasValue)

                    TotalSize -= item.Size.Value;
            }

            public T Dequeue()
            {
                T result = InnerQueue.Dequeue();

                OnDequeue(result);

                return result;
            }

            protected virtual void OnEnqueue(T item)
            {
                if (item.Size.HasValue)

                    TotalSize += item.Size.Value;
            }

            public void Enqueue(T item)
            {
                InnerQueue.Enqueue(item);

                OnEnqueue(item);
            }

            public T Peek() => InnerQueue.Peek();

            public bool TryDequeue(out T result)
            {
                if (InnerQueue.TryDequeue(out result))
                {
                    OnDequeue(result);

                    return true;
                }

                return false;
            }

            public bool TryPeek(out T result) => InnerQueue.TryPeek(out result);
        }

        public class ReadOnlyProcessCollection : IProcessCollection
        {
            protected IProcessCollection ProcessCollection { get; }

            public Size TotalSize => ProcessCollection.TotalSize;

            public bool IsReadOnly => true;

            public object SyncRoot => ProcessCollection.SyncRoot;

            public bool IsSynchronized => ProcessCollection.IsSynchronized;

            public uint Count => ProcessCollection.Count;

            public bool HasItems => ProcessCollection.HasItems;

            public ReadOnlyProcessCollection(in IProcessCollection processCollection) => ProcessCollection = processCollection.IsReadOnly ? throw new ArgumentException($"{nameof(processCollection)} must be read-only.", nameof(processCollection)) : processCollection;

            public T Peek() => ProcessCollection.Peek();

            public bool TryPeek(out T result) => ProcessCollection.TryPeek(out result);

            void IQueue<T>.Clear() => throw GetReadOnlyListOrCollectionException();

            void ISimpleLinkedListBase2.Clear() => throw GetReadOnlyListOrCollectionException();

            void IQueueBase<T>.Enqueue(T item) => throw GetReadOnlyListOrCollectionException();

            T IQueueBase<T>.Dequeue() => throw GetReadOnlyListOrCollectionException();

            bool IQueueBase<T>.TryDequeue(out T result) => throw GetReadOnlyListOrCollectionException();

            void IQueueBase<T>.Clear() => throw GetReadOnlyListOrCollectionException();
        }
    }

    public class AbstractionProcessCollection<TSource, TDestination> : AbstractionTypes<TSource, TDestination>.Queue<ProcessTypes<TSource>.IProcessCollection>, ProcessTypes<TDestination>.IProcessCollection where TSource : IPathInfo, TDestination where TDestination : IPath
    {
        Size ProcessTypes<TDestination>.IProcessCollection.TotalSize => InnerQueue.TotalSize;

        public AbstractionProcessCollection(in ProcessTypes<TSource>.IProcessCollection queue) : base(queue)
        {
            // Left empty.
        }
    }
}
