﻿/* Copyright © Pierre Sprimont, 2021
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

using static WinCopies.Collections.ThrowHelper;

namespace WinCopies.GUI.IO.Process
{
    public static class ProcessCollections<TItems> where TItems : IPath
    {
        public class ObservableProcessCollection : ObservableQueueCollection<WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue, TItems>, WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue
        {
            public Size TotalSize => InnerQueue.TotalSize;

            public ObservableProcessCollection() : this(new WinCopies.IO.Process.ProcessTypes<TItems>.ProcessQueue()) { /* Left empty. */ }

            public ObservableProcessCollection(in WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue queue) : base(queue.IsReadOnly ? throw new ArgumentException($"{nameof(queue)} must be non-read-only.", nameof(queue)) : queue)
            {
                // Left empty.
            }

            public WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue AsReadOnly() => new ReadOnlyObservableProcessCollection(this);
        }

        public class ReadOnlyObservableProcessCollection : ReadOnlyObservableQueueCollection<WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue, TItems>, WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue
        {
            public Size TotalSize => InnerQueue.TotalSize;

            public object SyncRoot => InnerQueue.SyncRoot;

            public bool IsSynchronized => InnerQueue.IsSynchronized;

            public bool HasItems => InnerQueue.HasItems;

            public ReadOnlyObservableProcessCollection(in ObservableQueueCollection<WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue, TItems> processCollection) : base(processCollection.IsReadOnly ? throw new ArgumentException($"{nameof(processCollection)} must be read-only.", nameof(processCollection)) : processCollection)
            {
                // Left empty.
            }

            void ISimpleLinkedListBase.Clear() => throw GetReadOnlyListOrCollectionException();

            void IQueueBase<TItems>.Enqueue(TItems item) => throw GetReadOnlyListOrCollectionException();

            TItems IQueueBase<TItems>.Dequeue() => throw GetReadOnlyListOrCollectionException();

            bool IQueueBase<TItems>.TryDequeue(out TItems
#if CS9
                ?
#endif
                result)
            {
                result = default;

                return false;
            }

            WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue WinCopies.IO.Process.ProcessTypes<TItems>.IProcessQueue.AsReadOnly() => this;
        }
    }
}
