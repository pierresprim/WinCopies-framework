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
using System.Linq;

using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Enumeration.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Util;

using static WinCopies.Collections.ThrowHelper;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Process
{
    public interface IReadOnlyProcessLinkedList<TItems, TError, TErrorItems, TAction> : IReadOnlyLinkedList2<TErrorItems>, ProcessTypes<TErrorItems>.IProcessQueue where TItems : IPath where TErrorItems : IProcessErrorItem<TItems, TError, TAction>
    {
        // Left empty.
    }

    public interface IProcessLinkedList<TItems, TError, TErrorItems, TAction> : ILinkedList3<TErrorItems>, ProcessTypes<TErrorItems>.IProcessQueue where TItems : IPath where TErrorItems : IProcessErrorItem<TItems, TError, TAction>
    {
        new void Enqueue(TErrorItems item);

        new void Clear();

        new ProcessTypes<IProcessErrorItem<TItems, TError, TAction>>.IProcessQueue AsReadOnly();
    }

    public class ProcessLinkedCollection<TItems, TError, TErrorItems, TAction> : LinkedCollection<TErrorItems>, IProcessLinkedList<TItems, TError, TErrorItems, TAction> where TItems : IPath where TErrorItems : IProcessErrorItem<TItems, TError, TAction>
    {
        public Size TotalSize { get; private set; }

        public object SyncRoot => InnerList.SyncRoot;

        public bool IsSynchronized => InnerList.IsSynchronized;

        public bool HasItems => InnerList.Count != 0;

        private static InvalidOperationException GetInvalidOperationException() => new InvalidOperationException("This operation is not available in the current context.");

        protected override void OnNodeRemoved(ILinkedListNode<TErrorItems> node)
        {
            base.OnNodeRemoved(node);

            if (node.Value.Size.HasValue)

                TotalSize -= node.Value.Size.Value;
        }

        protected virtual bool OnTryDequeue(out TErrorItems
#if CS9
            ?
#endif
            result)
        {
            if (InnerList.IsReadOnly)
            {
                result = default;

                return false;
            }

            if (TryPeek(out result))
            {
                InnerList.RemoveFirst();

                if (result.Size.HasValue)

                    TotalSize -= result.Size.Value;

                return true;
            }

            return false;
        }

        public TErrorItems Dequeue() => TryDequeue(out TErrorItems result)
                ? result
                : throw GetInvalidOperationException();

        protected virtual void OnEnqueue(TErrorItems item)
        {
            if (item.Size.HasValue)

                TotalSize += item.Size.Value;

            _ = InnerList.AddLast(item);
        }

        public void Enqueue(TErrorItems item) => OnEnqueue(item);

        public TErrorItems Peek() => TryPeek(out TErrorItems result) ? result : throw GetInvalidOperationException();

        public bool TryDequeue(out TErrorItems result) => OnTryDequeue(out result);

        public bool TryPeek(out TErrorItems
#if CS9
            ?
#endif
            result)
        {
            if (InnerList.Count == 0)
            {
                result = default;

                return false;
            }

            result = InnerList.First.Value;

            return true;
        }

        ProcessTypes<TErrorItems>.IProcessQueue ProcessTypes<TErrorItems>.IProcessQueue.AsReadOnly() => new ReadOnlyProcessLinkedCollection<TItems, TError, TErrorItems, TAction>(this);

        ProcessTypes<IProcessErrorItem<TItems, TError, TAction>>.IProcessQueue IProcessLinkedList<TItems, TError, TErrorItems, TAction>.AsReadOnly() => new ReadOnlyProcessLinkedList<TItems, TError, TErrorItems, IProcessErrorItem<TItems, TError, TAction>, TAction>(this);

#if !CS8
        object ISimpleLinkedList.Peek() => ((ISimpleLinkedList)InnerList).Peek();

        bool ISimpleLinkedList.TryPeek(out object result) => ((ISimpleLinkedList)InnerList).TryPeek(out result);
#endif
    }

    public class ReadOnlyProcessLinkedCollection<TItems, TError, TErrorItems, TAction> : ReadOnlyLinkedCollection<TErrorItems>, IReadOnlyProcessLinkedList<TItems, TError, TErrorItems, TAction> where TItems : IPath where TErrorItems : IProcessErrorItem<TItems, TError, TAction>
    {
        protected new IProcessLinkedList<TItems, TError, TErrorItems, TAction> InnerList => (IProcessLinkedList<TItems, TError, TErrorItems, TAction>)base.InnerList;

        public Size TotalSize => InnerList.TotalSize;

        object ISimpleLinkedListBase2.SyncRoot => ((ISimpleLinkedListBase2)InnerList).SyncRoot;

        bool ISimpleLinkedListBase2.IsSynchronized => ((ISimpleLinkedListBase2)InnerList).IsSynchronized;

        public bool HasItems => InnerList.HasItems;

        public ReadOnlyProcessLinkedCollection(in IProcessLinkedList<TItems, TError, TErrorItems, TAction> list) : base(list)
        {
            // Left empty.
        }

        ProcessTypes<TErrorItems>.IProcessQueue ProcessTypes<TErrorItems>.IProcessQueue.AsReadOnly() => this;

        void ISimpleLinkedListBase.Clear() => throw GetReadOnlyListOrCollectionException();

        TErrorItems IQueueBase<TErrorItems>.Dequeue() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<TErrorItems>.Enqueue(TErrorItems item) => throw GetReadOnlyListOrCollectionException();

        TErrorItems IPeekable<TErrorItems>.Peek() => InnerList.Peek();

        bool IQueueBase<TErrorItems>.TryDequeue(out TErrorItems
#if CS9
            ?
#endif
            result)
        {
            result = default;

            return false;
        }

        bool IPeekable<TErrorItems>.TryPeek(out TErrorItems result) => InnerList.TryPeek(out result);

        bool ISimpleLinkedListBase<TErrorItems>.TryPeek(out TErrorItems result) => InnerList.TryPeek(out result);

#if !CS8
        bool ISimpleLinkedList.TryPeek(out object result) => InnerList.TryPeek(out result);

        object ISimpleLinkedList.Peek() => ((ISimpleLinkedList)InnerList).Peek();
#endif
    }

    public class ReadOnlyProcessLinkedList<TItems, TError, TItemsIn, TItemsOut, TAction> : IReadOnlyProcessLinkedList<TItems, TError, TItemsOut, TAction> where TItems : IPath where TItemsIn : IProcessErrorItem<TItems, TError, TAction>, TItemsOut where TItemsOut : IProcessErrorItem<TItems, TError, TAction>
    {
        protected IProcessLinkedList<TItems, TError, TItemsIn, TAction> InnerList { get; }

        public Size TotalSize => InnerList.TotalSize;

        object ISimpleLinkedListBase2.SyncRoot => InnerList.AsFromType<ISimpleLinkedListBase2>().SyncRoot;

        bool ISimpleLinkedListBase2.IsSynchronized => InnerList.AsFromType<ISimpleLinkedListBase2>().IsSynchronized;

        public bool HasItems => InnerList.HasItems;

        public TItemsOut FirstValue => InnerList.FirstValue;

        public TItemsOut LastValue => InnerList.LastValue;

        public IReadOnlyLinkedListNode<TItemsOut> First => throw GetReadOnlyListOrCollectionException();

        public IReadOnlyLinkedListNode<TItemsOut> Last => throw GetReadOnlyListOrCollectionException();

        public bool SupportsReversedEnumeration => InnerList.SupportsReversedEnumeration;

        public uint Count => InnerList.Count;

        int System.Collections.Generic.ICollection<TItemsOut>.Count => InnerList.AsFromType<System.Collections.Generic.ICollection<TItemsIn>>().Count;

        public bool IsReadOnly => true;

        int ICollection.Count => InnerList.AsFromType<ICollection>().Count;

        public bool IsSynchronized => InnerList.AsFromType<ICollection>().IsSynchronized;

        public object SyncRoot => InnerList.AsFromType<ICollection>().SyncRoot;

        int System.Collections.Generic.IReadOnlyCollection<TItemsOut>.Count => InnerList.AsFromType<System.Collections.Generic.IReadOnlyCollection<TItemsIn>>().Count;

        public ReadOnlyProcessLinkedList(in IProcessLinkedList<TItems, TError, TItemsIn, TAction> list) => InnerList = list ?? throw GetArgumentNullException(nameof(list));

        ProcessTypes<TItemsOut>.IProcessQueue ProcessTypes<TItemsOut>.IProcessQueue.AsReadOnly() => this;

        TItemsOut IQueueBase<TItemsOut>.Dequeue() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<TItemsOut>.Enqueue(TItemsOut item) => throw GetReadOnlyListOrCollectionException();

        TItemsOut IPeekable<TItemsOut>.Peek() => InnerList.Peek();

        bool IQueueBase<TItemsOut>.TryDequeue(out TItemsOut
#if CS9
            ?
#endif
            result)
        {
            result = default;

            return false;
        }

        public bool TryPeek(out TItemsOut
#if CS9
            ?
#endif
            result)
        {
            if (InnerList.TryPeek(out TItemsIn _result))
            {
                result = _result;

                return true;
            }

            result = default;

            return false;
        }

        TItemsOut ISimpleLinkedList<TItemsOut>.Peek() => InnerList.AsFromType<ISimpleLinkedList<TItemsIn>>().Peek();

        protected IUIntCountableEnumerator<TItemsOut> GetEnumerator(in System.Collections.Generic.IEnumerable<TItemsIn> enumerable) => new UIntCountableEnumerator<EnumeratorInfo<TItemsOut>, TItemsOut>(new EnumeratorInfo<TItemsOut>(enumerable.Select<TItemsIn, TItemsOut>(item => item)), () => InnerList.Count);

        public IUIntCountableEnumerator<TItemsOut> GetEnumerator() => GetEnumerator(InnerList);

        public IUIntCountableEnumerator<TItemsOut> GetReversedEnumerator() => GetEnumerator(new Enumerable<TItemsIn>(InnerList.GetReversedEnumerator));

        public IReadOnlyLinkedListNode<TItemsOut> Find(TItemsOut value) => throw GetReadOnlyListOrCollectionException();

        public IReadOnlyLinkedListNode<TItemsOut> FindLast(TItemsOut value) => throw GetReadOnlyListOrCollectionException();

        public void Add(TItemsOut item) => throw GetReadOnlyListOrCollectionException();

        public void Clear() => throw GetReadOnlyListOrCollectionException();

        public bool Contains(TItemsOut item) => item is TItemsIn _item && InnerList.Contains(_item);

        public void CopyTo(TItemsOut[] array, int arrayIndex)
        {
            int i = (array.Length - arrayIndex < InnerList.Count) ? throw new InvalidOperationException("The given array does not have enough space.") : (-1);

            foreach (TItemsIn item in InnerList)

                array[++i] = item;
        }

        public bool Remove(TItemsOut item) => throw GetReadOnlyListOrCollectionException();

        public void CopyTo(Array array, int index)
        {
            int i = (array.Length - index < InnerList.Count) ? throw new InvalidOperationException("The given array does not have enough space.") : (-1);

            foreach (TItemsIn item in InnerList)

                array.SetValue(item, ++i);
        }

#if !CS8
        bool ISimpleLinkedList.TryPeek(out object result) => InnerList.TryPeek(out result);

        object ISimpleLinkedList.Peek() => ((ISimpleLinkedList)InnerList).Peek();

        System.Collections.Generic.IEnumerator<TItemsOut> System.Collections.Generic.IEnumerable<TItemsOut>.GetEnumerator() => GetEnumerator();

        System.Collections.Generic.IEnumerator<TItemsOut> Collections.Extensions.Generic.IEnumerable<TItemsOut>.GetReversedEnumerator() => GetReversedEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        System.Collections.IEnumerator Collections.Enumeration.IEnumerable.GetReversedEnumerator() => GetReversedEnumerator();
#endif
    }

    namespace ObjectModel
    {
        public static partial class ProcessObjectModelTypes<TItemsIn, TItemsOut, TFactory, TError, TAction, TProcessDelegates, TProcessEventDelegates, TProcessDelegateParam>
        {
            public abstract partial class Process
            {
                private interface _IQueue<T> : System.IDisposable
                {
                    bool HasItems { get; }

                    T Peek();

                    T Dequeue();
                }

                private class Queue : _IQueue<TItemsOut>
                {
                    private IQueue<TItemsOut> _queue;

                    public Queue(in IQueue<TItemsOut> queue) => _queue = queue;

                    bool _IQueue<TItemsOut>.HasItems => _queue.HasItems;

                    TItemsOut _IQueue<TItemsOut>.Peek() => _queue.Peek();

                    TItemsOut _IQueue<TItemsOut>.Dequeue() => _queue.Dequeue();

                    void System.IDisposable.Dispose() => _queue = null;
                }

                private class LinkedList : _IQueue<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem>
                {
                    private ILinkedList<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem> _list;
                    private ILinkedListNode<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem>
#if CS8
                        ?
#endif
                        _currentNode;
                    private ILinkedListNode<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem> _nextNode;
                    // private IProcessError<TError, TAction> _processError;
                    private Predicate<string> _func;
                    private TError _error;

                    public LinkedList(in ILinkedList<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem> list, in bool firstOnly)
                    {
                        _list = list;

                        _currentNode = list.First;

                        // _processError = _currentNode.Value.Error;

                        if (firstOnly)
                        {
                            string currentNode = _currentNode.Value.Item.Path;

                            _func = path => path.StartsWith(currentNode);
                        }

                        _error = _currentNode.Value.Error.Error;
                    }

                    private ILinkedListNode<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem>
#if CS8
                        ?
#endif
                        Peek()
                    {
                        ILinkedListNode<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem> node = _nextNode;

                        bool check() => node.Value.Error == null || Equals(node.Value.Error.Error, _error);

                        Func<bool> func = _func == null ?
#if !CS9
                            (Func<bool>)
#endif
                            check : (() => check() && _func(node.Value.Item.Path));

                        while (!(node == null || func()))

                            node = node.Next;

                        if (node == null)
                        {
                            Dispose();

                            return null;
                        }

                        // node.Value.Error = _processError;

                        return node;
                    }

                    bool _IQueue<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem>.HasItems => (_currentNode
#if CS8
                    ??=
#else
                    ?? (_currentNode =
#endif
                    Peek()
#if !CS8
                    )
#endif
                    ) != null;

                    ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem _IQueue<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem>.Peek() => _currentNode.Value;

                    ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem _IQueue<ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem>.Dequeue()
                    {
                        ProcessTypes<TItemsOut, TError, TAction>.ProcessErrorItem result = _currentNode.Value;

                        _nextNode = _currentNode.Next;

                        if (_currentNode.Value.Error != null)

                            _currentNode.Value.Error.Action = default;

                        _list.Remove(_currentNode);

                        _currentNode = null;

                        return result;
                    }

                    public void Dispose()
                    {
                        _list = null;
                        _currentNode = null;
                        _nextNode = null;
                        // _processError = null;
                        _func = null;
                        _error = default;
                    }
                }
            }
        }
    }
}
