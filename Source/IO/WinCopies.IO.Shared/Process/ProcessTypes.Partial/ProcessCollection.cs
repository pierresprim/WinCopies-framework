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
    public sealed class DefaultProcessPathCollectionFactory : IProcessPathCollectionFactory
    {
        ProcessTypes<T>.IProcessQueue IProcessPathCollectionFactory.GetProcessCollection<T>() => new ProcessTypes<T>.ProcessCollection();

        IProcessLinkedList<TItems, TError, TErrorItems, TAction> IProcessPathCollectionFactory.GetProcessLinkedList<TItems, TError, TErrorItems, TAction>() => new ProcessLinkedCollection<TItems, TError, TErrorItems, TAction>();
    }

    public interface IProcessQueue
    {
        Size TotalSize { get; }
    }

    public class ProcessQueueSizeHelper
    {
        public Size TotalSize { get; private set; } = GetNewSize();

        private static Size GetNewSize() => new Size(0ul);

        public void OnClearItems() => TotalSize = GetNewSize();

        public void OnEnqueueItem(in IPath item)
        {
            if (item.Size.HasValue)

                TotalSize += item.Size.Value;
        }

        public void OnDequeueItem(in IPath item)
        {
            if (item.Size.HasValue)

                TotalSize -= item.Size.Value;
        }
    }

    public static partial class ProcessTypes<T> where T : IPath
    {
        public interface IProcessQueue : Process.IProcessQueue, IQueue<T>
        {
            IProcessQueue AsReadOnly();
        }

        public class ProcessQueue : QueueCollection<T>, IProcessQueue
        {
            private readonly ProcessQueueSizeHelper _totalSize = new ProcessQueueSizeHelper();

            public Size TotalSize => _totalSize.TotalSize;

            protected override void EnqueueItem(T item)
            {
                base.EnqueueItem(item);

                _totalSize.OnEnqueueItem(item);
            }

            protected override T DequeueItem()
            {
                T result = base.DequeueItem();

                _totalSize.OnDequeueItem(result);

                return result;
            }

            protected override void ClearItems()
            {
                base.ClearItems();

                _totalSize.OnClearItems();
            }

            public IProcessQueue AsReadOnly() => new ReadOnlyProcessQueue(this);
        }

        public class ReadOnlyProcessQueue : ReadOnlyQueue
#if WinCopies4
<IProcessQueue, T>
#else
            <T>
#endif
            , IProcessQueue
        {
            public Size TotalSize { get; }

            public ReadOnlyProcessQueue(IProcessQueue processQueue) : base(processQueue)
            {
                // Left empty.
            }

            IProcessQueue IProcessQueue.AsReadOnly() => this;
        }

        public class ProcessCollection : QueueCollection<IProcessQueue, T>, IProcessQueue
        {
            public Size TotalSize { get; private set; }

            public ProcessCollection() : this(new ProcessQueue()) { }

            public ProcessCollection(in IProcessQueue queue) : base(queue.IsReadOnly ? throw new ArgumentException($"{nameof(queue)} must be non-read-only.", nameof(queue)) : queue)
            {
                // Left empty.
            }

            protected override void ClearItems()
            {
                base.ClearItems();

                TotalSize = new Size();
            }

            protected override T DequeueItem()
            {
                T result = base.DequeueItem();

                if (result.Size.HasValue)

                    TotalSize -= result.Size.Value;

                return result;
            }

            protected override void EnqueueItem(T item)
            {
                base.EnqueueItem(item);

                if (item.Size.HasValue)

                    TotalSize += item.Size.Value;
            }

            public IProcessQueue AsReadOnly() => new ReadOnlyProcessCollection(this);
        }

        public class ReadOnlyProcessCollection : ReadOnlyQueueCollection<IProcessQueue, T>, IProcessQueue
        {
            public Size TotalSize => InnerQueue.TotalSize;

            public bool IsReadOnly => true;

            public object SyncRoot => InnerQueue.SyncRoot;

            public bool IsSynchronized => InnerQueue.IsSynchronized;

            public uint Count => InnerQueue.Count;

            public bool HasItems => InnerQueue.HasItems;

            public ReadOnlyProcessCollection(in IProcessQueue processCollection) : base(processCollection.IsReadOnly ? throw new ArgumentException($"{nameof(processCollection)} must be read-only.", nameof(processCollection)) : processCollection)
            {
                // Left empty.
            }

            public T Peek() => InnerQueue.Peek();

            public bool TryPeek(out T result) => InnerQueue.TryPeek(out result);

            void IQueue<T>.Clear() => throw GetReadOnlyListOrCollectionException();

            void ISimpleLinkedListBase2.Clear() => throw GetReadOnlyListOrCollectionException();

            void IQueueBase<T>.Enqueue(T item) => throw GetReadOnlyListOrCollectionException();

            T IQueueBase<T>.Dequeue() => throw GetReadOnlyListOrCollectionException();

            bool IQueueBase<T>.TryDequeue(out T result) => throw GetReadOnlyListOrCollectionException();

            void IQueueBase<T>.Clear() => throw GetReadOnlyListOrCollectionException();

            IProcessQueue IProcessQueue.AsReadOnly() => this;
        }
    }

    public class AbstractionProcessCollection<TSource, TDestination> : AbstractionTypes<TSource, TDestination>.ReadOnlyQueue<ProcessTypes<TSource>.IProcessQueue>, ProcessTypes<TDestination>.IProcessQueue where TSource : IPathInfo, TDestination where TDestination : IPath
    {
        Size IProcessQueue.TotalSize => InnerQueue.TotalSize;

        public AbstractionProcessCollection(in ProcessTypes<TSource>.IProcessQueue queue) : base(queue)
        {
            // Left empty.
        }

        public ProcessTypes<TDestination>.IProcessQueue AsReadOnly() => InnerQueue.IsReadOnly ? this : new AbstractionProcessCollection<TSource, TDestination>(new ProcessTypes<TSource>.ReadOnlyProcessQueue(InnerQueue));
    }
}
