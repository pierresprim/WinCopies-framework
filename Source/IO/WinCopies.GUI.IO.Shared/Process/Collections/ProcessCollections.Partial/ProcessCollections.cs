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

using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.IO;
using WinCopies.IO.Process;
using static WinCopies.Collections.ThrowHelper;

namespace WinCopies.GUI.IO.Process
{
    public static class ProcessCollections<TItems> where TItems : IPath
    {
        public class ObservableProcessCollection : ObservableQueueCollection<WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue, TItems>, WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue
        {
            private readonly ProcessQueueSizeHelper _totalSize = new ProcessQueueSizeHelper();

            public Size TotalSize => _totalSize.TotalSize;

            public ObservableProcessCollection() : this(new WinCopies.IO.Process.ProcessTypes<TItems>.ProcessQueue()) { }

            public ObservableProcessCollection(in WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue queue) : base(queue.IsReadOnly ? throw new ArgumentException($"{nameof(queue)} must be non-read-only.", nameof(queue)) : queue)
            {
                // Left empty.
            }

            protected override void EnqueueItem(TItems item)
            {
                base.EnqueueItem(item);

                _totalSize.OnEnqueueItem(item);
            }

            protected override TItems DequeueItem()
            {
                TItems result = base.DequeueItem();

                _totalSize.OnDequeueItem(result);

                return result;
            }

            protected override void ClearItems()
            {
                base.ClearItems();

                _totalSize.OnClearItems();
            }

            public WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue AsReadOnly() => new ReadOnlyObservableProcessCollection(this);
        }

        public class ReadOnlyObservableProcessCollection : ReadOnlyObservableQueueCollection<WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue, TItems>, WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue
        {
            public Size TotalSize => InnerQueue.TotalSize;

            public bool IsReadOnly => true;

            public object SyncRoot => InnerQueue.SyncRoot;

            public bool IsSynchronized => InnerQueue.IsSynchronized;

            public uint Count => InnerQueue.Count;

            public bool HasItems => InnerQueue.HasItems;

            public ReadOnlyObservableProcessCollection(in ObservableQueueCollection<WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue, TItems> processCollection) : base(processCollection.IsReadOnly ? throw new ArgumentException($"{nameof(processCollection)} must be read-only.", nameof(processCollection)) : processCollection)
            {
                // Left empty.
            }

            public TItems Peek() => InnerQueue.Peek();

            public bool TryPeek(out TItems result) => InnerQueue.TryPeek(out result);

            void IQueue<TItems>.Clear() => throw GetReadOnlyListOrCollectionException();

            void ISimpleLinkedListBase2.Clear() => throw GetReadOnlyListOrCollectionException();

            void IQueueBase<TItems>.Enqueue(TItems item) => throw GetReadOnlyListOrCollectionException();

            TItems IQueueBase<TItems>.Dequeue() => throw GetReadOnlyListOrCollectionException();

            bool IQueueBase<TItems>.TryDequeue(out TItems result)
            {
                result = default;

                return false;
            }

            void IQueueBase<TItems>.Clear() => throw GetReadOnlyListOrCollectionException();

            WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue.AsReadOnly() => this;
        }
    }
}
