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

using WinCopies.Collections.DotNetFix.Generic;

namespace WinCopies.IO.Process
{
    public static partial class ProcessTypes<T> where T : IPath
    {
        public class ProcessCollection : IQueue<T>
        {
            protected IQueue<T> InnerQueue { get; }

            public Size TotalSize { get; private set; }

            public object SyncRoot => InnerQueue.SyncRoot;

            public bool IsSynchronized => InnerQueue.IsSynchronized;

            public uint Count => InnerQueue.Count;

            public bool IsReadOnly => InnerQueue.IsReadOnly;

            public bool HasItems => InnerQueue.HasItems;

            public ProcessCollection(in IQueue<T> queue) => InnerQueue = queue;

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
    }
}
