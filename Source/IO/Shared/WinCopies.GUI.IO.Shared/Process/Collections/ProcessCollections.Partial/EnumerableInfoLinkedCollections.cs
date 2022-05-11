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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using WinCopies.Collections;
using WinCopies.Collections.AbstractionInterop.Generic;
using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Enumeration.Generic;
using WinCopies.Collections.Generic;
using WinCopies.IO;
using WinCopies.IO.Process;

using static WinCopies.Collections.ThrowHelper;
using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.IO.Process
{
    public interface IProcessLinkedList<TItems, TError, TErrorItems, TAction> : ICollection<TErrorItems>, System.Collections.Generic.IEnumerable<TErrorItems>, IEnumerable, System.Collections.Generic.IReadOnlyCollection<TErrorItems>, ICollection, ILinkedList3<TErrorItems>, IReadOnlyLinkedList2<TErrorItems>, IReadOnlyLinkedList<TErrorItems>, IUIntCountable, Collections.Generic.IEnumerable<TErrorItems>, Collections.Generic.IEnumerable<ILinkedListNode<TErrorItems>>, System.Collections.Generic.IEnumerable<ILinkedListNode<TErrorItems>>, INotifyPropertyChanged, INotifyLinkedCollectionChanged<TErrorItems>, WinCopies.IO.Process.IProcessLinkedList<TItems, TError, TErrorItems, TAction> where TItems : IPath where TErrorItems : IProcessErrorItem<TItems, TError, TAction>
    {
        new object SyncRoot { get; }

        new bool IsSynchronized { get; }
    }

    public class ObservableProcessLinkedCollection<TItems, TError, TErrorItems, TAction> : ObservableLinkedCollection<TErrorItems>, IProcessLinkedList<TItems, TError, TErrorItems, TAction> where TItems : IPath where TErrorItems : IProcessErrorItem<TItems, TError, TAction>
    {
        public Size TotalSize { get; private set; } = new Size(0);

        public object SyncRoot => null;

        public bool IsSynchronized => false;

        public bool HasItems => Count != 0;

        public ObservableProcessLinkedCollection() : base()
        {
            // Left empty.
        }

        public ObservableProcessLinkedCollection(in Collections.DotNetFix.Generic.LinkedList<TErrorItems> list) : base(list)
        {
            // Left empty.
        }

        private static InvalidOperationException GetInvalidOperationException() => new InvalidOperationException("This operation is invalid in the current context.");

        public TErrorItems Dequeue() => TryDequeue(out TErrorItems result) ? result : throw GetInvalidOperationException();

        protected virtual void OnPropertyChanged(in string propertyName) => OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));

        protected virtual void OnEnqueue(TErrorItems item)
        {
            if (item.Size.HasValue)

                TotalSize += item.Size.Value;

            _ = AddLast(item);

            OnPropertyChanged(nameof(TotalSize));
        }

        public void Enqueue(TErrorItems item) => OnEnqueue(item);

        protected override void OnNodeRemoved(ILinkedListNode<TErrorItems> node)
        {
            base.OnNodeRemoved(node);

            if (node.Value.Size.HasValue)

                TotalSize -= node.Value.Size.Value;
        }

        protected TErrorItems OnDequeue()
        {
            TErrorItems result = First.Value;

            if (result.Size.HasValue)

                TotalSize -= result.Size.Value;

            RemoveFirst();

            OnPropertyChanged(nameof(TotalSize));

            return result;
        }

        public TErrorItems Peek() => TryPeek(out TErrorItems result) ? result : throw GetInvalidOperationException();

        public bool TryDequeue(out TErrorItems result)
        {
            if (Count != 0)
            {
                result = OnDequeue();

                return true;
            }

            result = default;

            return false;
        }

        public bool TryPeek(out TErrorItems result)
        {
            if (Count != 0)
            {
                result = First.Value;

                return true;
            }

            result = default;

            return false;
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            TotalSize = new Size(0);

            OnPropertyChanged(nameof(TotalSize));
        }

        public ProcessTypes<TErrorItems>.IProcessQueue AsReadOnly() => new ReadOnlyObservableProcessLinkedCollection<TItems, TError, TErrorItems, TAction>(this);

        ProcessTypes<IProcessErrorItem<TItems, TError, TAction>>.IProcessQueue WinCopies.IO.Process.IProcessLinkedList<TItems, TError, TErrorItems, TAction>.AsReadOnly() => new ReadOnlyObservableProcessLinkedCollection<TItems, TError, TErrorItems, IProcessErrorItem<TItems, TError, TAction>, TAction>(this);

#if !CS8
        object ISimpleLinkedList.Peek() => ((ISimpleLinkedList)InnerList).Peek();

        bool ISimpleLinkedList.TryPeek(out object result) => ((ISimpleLinkedList)InnerList).TryPeek(out result);
#endif
    }

    public class ReadOnlyObservableProcessLinkedCollection<TItems, TError, TErrorItems, TAction> : ReadOnlyObservableLinkedCollection<TErrorItems>, ProcessTypes<TErrorItems>.IProcessQueue, IReadOnlyProcessLinkedList<TItems, TError, TErrorItems, TAction> where TItems : IPath where TErrorItems : IProcessErrorItem<TItems, TError, TAction>
    {
        public Size TotalSize => ((IProcessQueue)InnerLinkedCollection).TotalSize;

        object ISimpleLinkedListBase2.SyncRoot => ((ISimpleLinkedListBase2)InnerLinkedCollection).SyncRoot;

        bool ISimpleLinkedListBase2.IsSynchronized => ((ISimpleLinkedListBase2)InnerLinkedCollection).IsSynchronized;

        bool ISimpleLinkedListBase.HasItems => ((ISimpleLinkedListBase)InnerLinkedCollection).HasItems;

        public ReadOnlyObservableProcessLinkedCollection(in ObservableProcessLinkedCollection<TItems, TError, TErrorItems, TAction> collection) : base(collection)
        {
            // Left empty.
        }

        void IQueue<TErrorItems>.Clear() => throw GetReadOnlyListOrCollectionException();

        void ISimpleLinkedListBase2.Clear() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<TErrorItems>.Clear() => throw GetReadOnlyListOrCollectionException();

        TErrorItems IQueueBase<TErrorItems>.Dequeue() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<TErrorItems>.Enqueue(TErrorItems item) => throw GetReadOnlyListOrCollectionException();

        public TErrorItems Peek() => ((IQueue<TErrorItems>)InnerLinkedCollection).Peek();

        bool IQueueBase<TErrorItems>.TryDequeue(out TErrorItems result)
        {
            result = default;

            return false;
        }

        public bool TryPeek(out TErrorItems result) => ((IQueue<TErrorItems>)InnerLinkedCollection).TryPeek(out result);

        ProcessTypes<TErrorItems>.IProcessQueue ProcessTypes<TErrorItems>.IProcessQueue.AsReadOnly() => this;

#if !CS8
        object ISimpleLinkedList.Peek() => ((ISimpleLinkedList)InnerLinkedCollection).Peek();

        bool ISimpleLinkedList.TryPeek(out object result) => ((ISimpleLinkedList)InnerLinkedCollection).TryPeek(out result);
#endif
    }

    public class ReadOnlyObservableProcessLinkedCollection<TItems, TError, TItemsIn, TItemsOut, TAction> : ICollection<TItemsOut>, System.Collections.Generic.IEnumerable<TItemsOut>, IReadOnlyCollection<TItemsOut>, ICollection, INotifyPropertyChanged, INotifyLinkedCollectionChanged<TItemsOut>, IReadOnlyLinkedList2<TItemsOut>, IReadOnlyLinkedList<TItemsOut>, IUIntCountable, Collections.Generic.IEnumerable<TItemsOut>,
#if CS8
        Collections.DotNetFix.Generic.IEnumerable<TItemsOut>, 
#endif
        Collections.Enumeration.IEnumerable, ProcessTypes<TItemsOut>.IProcessQueue, IReadOnlyProcessLinkedList<TItems, TError, TItemsOut, TAction> where TItems : IPath where TItemsIn : TItemsOut where TItemsOut : IProcessErrorItem<TItems, TError, TAction>
    {
        protected ObservableProcessLinkedCollection<TItems, TError, TItemsIn, TAction> InnerLinkedCollection { get; }

        public Size TotalSize => InnerLinkedCollection.AsFromType<IProcessQueue>().TotalSize;

        object ISimpleLinkedListBase2.SyncRoot => InnerLinkedCollection.AsFromType<ISimpleLinkedListBase2>().SyncRoot;

        bool ISimpleLinkedListBase2.IsSynchronized => InnerLinkedCollection.AsFromType<ISimpleLinkedListBase2>().IsSynchronized;

        public bool HasItems => InnerLinkedCollection.AsFromType<ISimpleLinkedListBase>().HasItems;

        public uint Count => InnerLinkedCollection.Count;

        public bool IsReadOnly => true;

        int ICollection<TItemsOut>.Count => InnerLinkedCollection.AsFromType<ICollection<TItemsIn>>().Count;

        public bool SupportsReversedEnumeration => InnerLinkedCollection.SupportsReversedEnumeration;

        IReadOnlyLinkedListNode<TItemsOut> IReadOnlyLinkedList<TItemsOut>.First => throw GetReadOnlyListOrCollectionException();

        IReadOnlyLinkedListNode<TItemsOut> IReadOnlyLinkedList<TItemsOut>.Last => throw GetReadOnlyListOrCollectionException();

        int ICollection.Count => InnerLinkedCollection.AsFromType<ICollection>().Count;

        bool ICollection.IsSynchronized => InnerLinkedCollection.AsFromType<ICollection>().IsSynchronized;

        object ICollection.SyncRoot => InnerLinkedCollection.AsFromType<ICollection>().SyncRoot;

        int IReadOnlyCollection<TItemsOut>.Count => InnerLinkedCollection.AsFromType<IReadOnlyCollection<TItemsIn>>().Count;

        TItemsOut IReadOnlyLinkedList2<TItemsOut>.FirstValue => InnerLinkedCollection.FirstValue;

        TItemsOut IReadOnlyLinkedList2<TItemsOut>.LastValue => InnerLinkedCollection.LastValue;

        private Dictionary<LinkedCollectionChangedEventHandler<TItemsOut>, LinkedCollectionChangedEventHandler<TItemsIn>> _events = new Dictionary<LinkedCollectionChangedEventHandler<TItemsOut>, LinkedCollectionChangedEventHandler<TItemsIn>>();

        public event LinkedCollectionChangedEventHandler<TItemsOut> CollectionChanged
        {
            add
            {
#if CS8
                static
#endif
                AbstractionTypes<TItemsIn, TItemsOut>.LinkedListTypes<ILinkedList3<TItemsIn>, ILinkedListNode<TItemsIn>>.LinkedListNode getNode(in ILinkedListNode<TItemsIn> node) => new
#if !CS9
                AbstractionTypes<TItemsIn, TItemsOut>.LinkedListTypes<ILinkedList3<TItemsIn>, ILinkedListNode<TItemsIn>>.LinkedListNode
#endif
                    (node);

                void d(object sender, LinkedCollectionChangedEventArgs<TItemsIn> e) => value(sender, new LinkedCollectionChangedEventArgs<TItemsOut>(e.Action, getNode(e.AddedBefore), getNode(e.AddedAfter), getNode(e.Node)));

                _events.Add(value, d);

                InnerLinkedCollection.CollectionChanged += d;
            }

            remove
            {
                InnerLinkedCollection.CollectionChanged -= _events[value];

                _events.Remove(value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => ((INotifyPropertyChanged)InnerLinkedCollection).PropertyChanged += value;

            remove => ((INotifyPropertyChanged)InnerLinkedCollection).PropertyChanged -= value;
        }

        public ReadOnlyObservableProcessLinkedCollection(in ObservableProcessLinkedCollection<TItems, TError, TItemsIn, TAction> collection) => InnerLinkedCollection = collection ?? throw GetArgumentNullException(nameof(collection));

        void IQueue<TItemsOut>.Clear() => throw GetReadOnlyListOrCollectionException();

        void ISimpleLinkedListBase2.Clear() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<TItemsOut>.Clear() => throw GetReadOnlyListOrCollectionException();

        TItemsOut IQueueBase<TItemsOut>.Dequeue() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<TItemsOut>.Enqueue(TItemsOut item) => throw GetReadOnlyListOrCollectionException();

        public TItemsOut Peek() => ((IQueue<TItemsIn>)InnerLinkedCollection).Peek();

        bool IQueueBase<TItemsOut>.TryDequeue(out TItemsOut result)
        {
            result = default;

            return false;
        }

        public bool TryPeek(out TItemsOut result)
        {
            if (((IQueue<TItemsIn>)InnerLinkedCollection).TryPeek(out TItemsIn _result))
            {
                result = _result;

                return true;
            }

            result = default;

            return false;
        }

        ProcessTypes<TItemsOut>.IProcessQueue ProcessTypes<TItemsOut>.IProcessQueue.AsReadOnly() => this;

        void ICollection<TItemsOut>.Add(TItemsOut item) => throw GetReadOnlyListOrCollectionException();

        void ICollection<TItemsOut>.Clear() => throw GetReadOnlyListOrCollectionException();

        public bool Contains(TItemsOut item) => item is TItemsIn _item && InnerLinkedCollection.Contains(_item);

        public void CopyTo(TItemsOut[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < InnerLinkedCollection.Count)

                throw new InvalidOperationException("The given array does not have enough space.");

            int i = -1;

            foreach (TItemsIn item in InnerLinkedCollection)

                array[++i] = item;
        }

        bool ICollection<TItemsOut>.Remove(TItemsOut item) => throw GetReadOnlyListOrCollectionException();

        IReadOnlyLinkedListNode<TItemsOut> IReadOnlyLinkedList<TItemsOut>.Find(TItemsOut value) => throw GetReadOnlyListOrCollectionException();

        IReadOnlyLinkedListNode<TItemsOut> IReadOnlyLinkedList<TItemsOut>.FindLast(TItemsOut value) => throw GetReadOnlyListOrCollectionException();

        public void CopyTo(Array array, int index)
        {
            if (array.Length - index < InnerLinkedCollection.Count)

                throw new InvalidOperationException("The given array does not have enough space.");

            int i = -1;

            foreach (TItemsIn item in InnerLinkedCollection)

                array.SetValue(item, ++i);
        }

        protected IUIntCountableEnumerator<TItemsOut> GetEnumerator(in System.Collections.Generic.IEnumerable<TItemsIn> enumerable) => new UIntCountableEnumerator<EnumeratorInfo<TItemsOut>, TItemsOut>(new EnumeratorInfo<TItemsOut>(enumerable.Select<TItemsIn, TItemsOut>(item => item)), () => InnerLinkedCollection.Count);

        public IUIntCountableEnumerator<TItemsOut> GetEnumerator() => GetEnumerator(InnerLinkedCollection);

        public IUIntCountableEnumerator<TItemsOut> GetReversedEnumerator() => GetEnumerator(new Enumerable<TItemsIn>(InnerLinkedCollection.GetReversedEnumerator));

#if !CS8
        object ISimpleLinkedList.Peek() => ((ISimpleLinkedList)InnerLinkedCollection).Peek();

        bool ISimpleLinkedList.TryPeek(out object result) => ((ISimpleLinkedList)InnerLinkedCollection).TryPeek(out result);

        System.Collections.Generic.IEnumerator<TItemsOut> System.Collections.Generic.IEnumerable<TItemsOut>.GetEnumerator() => GetEnumerator();

        System.Collections.Generic.IEnumerator<TItemsOut> Collections.Generic.IEnumerable<TItemsOut>.GetReversedEnumerator() => GetReversedEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        System.Collections.IEnumerator Collections.Enumeration.IEnumerable.GetReversedEnumerator() => GetReversedEnumerator();
#endif
    }
}
