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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using WinCopies.Collections.DotNetFix;
using WinCopies.IO;

namespace WinCopies.GUI.IO.Process
{
    public class ProcessQueueCollection<T> : ObservableQueueCollection<T>, IProcessCollection<T> where T : IPathInfo
    {
        private Size _size = new Size(0ul);

        public Size Size { get => _size; private set { _size = value; RaisePropertyChangedEvent(nameof(Size)); } }

        public void Add(T item) => Enqueue(item);

        public T Remove() => Dequeue();

        public void DecrementSize(ulong sizeInBytes) => Size -= sizeInBytes;

        protected override void OnCollectionChanged(SimpleLinkedCollectionChangedEventArgs<T> e) 
        {
            base.OnCollectionChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    if (!e.Item.IsDirectory)

                        Size += e.Item.Size.Value;

                    break;

                case NotifyCollectionChangedAction.Reset:

                    Size = new Size(0ul);

                    break;
            }
        }
    }

    public sealed class ProcessQueueCollection : ProcessQueueCollection<IPathInfo>, IProcessCollection { }

    public sealed class ReadOnlyProcessQueueCollection : ReadOnlyObservableQueueCollection<IPathInfo>, IReadOnlyProcessCollection
    {
        public Size Size => ((ProcessQueueCollection)InnerQueueCollection).Size;

        public ReadOnlyProcessQueueCollection(ProcessQueueCollection queueCollection) : base(queueCollection) { }
    }

    //public sealed class ReadOnlyProcessQueueCollection : IReadOnlyProcessCollection
    //{
    //    private readonly _ReadOnlyProcessQueueCollection _innerCollection;

    //    public Size Size => _innerCollection.Size;

    //    public int Count => _innerCollection.Count;

    //    public event PropertyChangedEventHandler PropertyChanged
    //    {
    //        add => _innerCollection.PropertyChanged += value;

    //        remove => _innerCollection.PropertyChanged -= value;
    //    }

    //    public event SimpleLinkedCollectionChangedEventHandler<IPathInfo> CollectionChanged
    //    {
    //        add => _innerCollection.CollectionChanged += value;

    //        remove => _innerCollection.CollectionChanged -= value;
    //    }

    //    public ReadOnlyProcessQueueCollection(_ReadOnlyProcessQueueCollection innerCollection) => _innerCollection = innerCollection;

    //    public IEnumerator<IPathInfo> GetEnumerator() => _innerCollection.GetEnumerator();

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //}

    public sealed class ProcessErrorPathQueueCollection : ProcessQueueCollection<IErrorPathInfo>, IProcessErrorPathCollection { }

    public sealed class ReadOnlyProcessErrorPathQueueCollection : ReadOnlyObservableQueueCollection<IErrorPathInfo>, IReadOnlyProcessErrorPathCollection
    {
        public ReadOnlyProcessErrorPathQueueCollection(ProcessErrorPathQueueCollection queue) : base(queue) { }
    }
}
