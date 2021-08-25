﻿/* Copyright © Pierre Sprimont, 2020
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

#if CS7 && WinCopies3
using System;
using System.Collections;
using System.Collections.Generic;

namespace WinCopies.Collections.DotNetFix.Generic.TEMP
{
    public class EnumerableQueueCollection<TQueue, TItems> : QueueCollection<TQueue, TItems>, IEnumerableQueue<TItems>, IReadOnlyCollection<TItems>, ICollection where TQueue : IEnumerableQueue<TItems>
    {
        bool ICollection.IsSynchronized => ((ICollection)InnerQueue).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)InnerQueue).SyncRoot;

        int ICollection.Count => ((ICollection)InnerQueue).Count;

        int IReadOnlyCollection<TItems>.Count => ((IReadOnlyCollection<TItems>)InnerQueue).Count;

        public EnumerableQueueCollection(in TQueue queue) : base(queue) { /* Left empty. */ }

        public void CopyTo(TItems[] array, int index) => InnerQueue.CopyTo(array, index);

        public void CopyTo(Array array, int index) => InnerQueue.CopyTo(array, index);

        System.Collections.Generic.IEnumerator<TItems> System.Collections.Generic.IEnumerable<TItems>.GetEnumerator() => InnerQueue.GetEnumerator();

        public System.Collections.IEnumerator GetEnumerator() => ((IEnumerable)InnerQueue).GetEnumerator();

        public TItems[] ToArray() => InnerQueue.ToArray();
    }

    public class EnumerableQueueCollection<T> : EnumerableQueueCollection<IEnumerableQueue<T>, T>, IEnumerableQueue<T>, IReadOnlyCollection<T>, ICollection
    {
        public EnumerableQueueCollection(in IEnumerableQueue<T> queue) : base(queue) { /* Left empty. */ }

        public EnumerableQueueCollection() : this(new EnumerableQueue<T>()) { /* Left empty. */ }
    }
}
#endif
