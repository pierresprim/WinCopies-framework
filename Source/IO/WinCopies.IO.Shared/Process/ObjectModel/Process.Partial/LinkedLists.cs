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

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;

using static WinCopies.Collections.ThrowHelper;

namespace WinCopies.IO.Process
{
    public interface IReadOnlyProcessLinkedList<TItems, TError> : IReadOnlyLinkedList2<IProcessErrorItem<TItems, TError>>, ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue where TItems : IPath
    {
        // Left empty.
    }

    public interface IProcessLinkedList<TItems, TError> : ILinkedList3<IProcessErrorItem<TItems, TError>>, ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue where TItems : IPath
    {
        void Enqueue(IProcessErrorItem<TItems, TError> item);

        new void Clear();
    }

    public class ProcessLinkedCollection<TItems, TError> : LinkedCollection<IProcessErrorItem<TItems, TError>>, IProcessLinkedList<TItems, TError> where TItems : IPath
    {
        public Size TotalSize => ((IProcessQueue)InnerList).TotalSize;

        public object SyncRoot => InnerList.SyncRoot;

        public bool IsSynchronized => InnerList.IsSynchronized;

        public bool HasItems => InnerList.Count != 0;

        private static InvalidOperationException GetInvalidOperationException() => new InvalidOperationException("This operation is not available in the current context.");

        public IProcessErrorItem<TItems, TError> Dequeue() => TryDequeue(out IProcessErrorItem<TItems, TError> result)
                ? result
                : throw GetInvalidOperationException();

        public void Enqueue(IProcessErrorItem<TItems, TError> item) => InnerList.AddLast(item);

        public IProcessErrorItem<TItems, TError> Peek() => TryPeek(out IProcessErrorItem<TItems, TError> result) ? result : throw GetInvalidOperationException();

        public bool TryDequeue(out IProcessErrorItem<TItems, TError> result)
        {
            if (InnerList.IsReadOnly)
            {
                result = null;

                return false;
            }

            return TryPeek(out result);
        }

        public bool TryPeek(out IProcessErrorItem<TItems, TError> result)
        {
            if (InnerList.Count == 0)
            {
                result = null;

                return false;
            }

            result = InnerList.First.Value;

            return true;
        }

        ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue.AsReadOnly() => new ReadOnlyProcessLinkedCollection<TItems, TError>(this);
    }

    public class ReadOnlyProcessLinkedCollection<TItems, TError> : ReadOnlyLinkedCollection<IProcessErrorItem<TItems, TError>>, IReadOnlyProcessLinkedList<TItems, TError> where TItems : IPath
    {
        protected new IProcessLinkedList<TItems, TError> InnerList => (IProcessLinkedList<TItems, TError>)base.InnerList;

        public Size TotalSize => InnerList.TotalSize;

        object ISimpleLinkedListBase2.SyncRoot => ((ISimpleLinkedListBase2)InnerList).SyncRoot;

        bool ISimpleLinkedListBase2.IsSynchronized => ((ISimpleLinkedListBase2)InnerList).IsSynchronized;

        public bool HasItems => InnerList.HasItems;

        public ReadOnlyProcessLinkedCollection(in IProcessLinkedList<TItems, TError> list) : base(list)
        {
            // Left empty.
        }

        ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue.AsReadOnly() => this;

        void IQueue<IProcessErrorItem<TItems, TError>>.Clear() => throw GetReadOnlyListOrCollectionException();

        void ISimpleLinkedListBase2.Clear() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<IProcessErrorItem<TItems, TError>>.Clear() => throw GetReadOnlyListOrCollectionException();

        IProcessErrorItem<TItems, TError> IQueueBase<IProcessErrorItem<TItems, TError>>.Dequeue() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<IProcessErrorItem<TItems, TError>>.Enqueue(IProcessErrorItem<TItems, TError> item) => throw GetReadOnlyListOrCollectionException();

        IProcessErrorItem<TItems, TError> IQueue<IProcessErrorItem<TItems, TError>>.Peek() => InnerList.Peek();

        IProcessErrorItem<TItems, TError> ISimpleLinkedListBase<IProcessErrorItem<TItems, TError>>.Peek() => InnerList.Peek();

        IProcessErrorItem<TItems, TError> IQueueBase<IProcessErrorItem<TItems, TError>>.Peek() => InnerList.Peek();

        bool IQueueBase<IProcessErrorItem<TItems, TError>>.TryDequeue(out IProcessErrorItem<TItems, TError> result)
        {
            result = null;

            return false;
        }

        bool IQueue<IProcessErrorItem<TItems, TError>>.TryPeek(out IProcessErrorItem<TItems, TError> result) => InnerList.TryPeek(out result);

        bool ISimpleLinkedListBase<IProcessErrorItem<TItems, TError>>.TryPeek(out IProcessErrorItem<TItems, TError> result) => InnerList.TryPeek(out result);

        bool IQueueBase<IProcessErrorItem<TItems, TError>>.TryPeek(out IProcessErrorItem<TItems, TError> result) => InnerList.TryPeek(out result);
    }

    namespace ObjectModel
    {
        public static partial class ProcessObjectModelTypes<TItems, TFactory, TError, TProcessDelegates, TProcessEventDelegates, TProcessDelegateParam>
        {
            public abstract partial class Process
            {
                private interface _IQueue<T> : System.IDisposable
                {
                    bool HasItems { get; }

                    T Peek();

                    T Dequeue();
                }

                private class Queue : _IQueue<TItems>
                {
                    private IQueue<TItems> _queue;

                    public Queue(in IQueue<TItems> queue) => _queue = queue;

                    bool _IQueue<TItems>.HasItems => _queue.HasItems;

                    TItems _IQueue<TItems>.Peek() => _queue.Peek();

                    TItems _IQueue<TItems>.Dequeue() => _queue.Dequeue();

                    void System.IDisposable.Dispose() => _queue = null;
                }

                private class LinkedList : _IQueue<IProcessErrorItem<TItems, TError>>
                {
                    private ILinkedList<IProcessErrorItem<TItems, TError>> _list;
                    private ILinkedListNode<IProcessErrorItem<TItems, TError>> _currentNode;
                    private ILinkedListNode<IProcessErrorItem<TItems, TError>> _previousNode;
                    private readonly TError _error;

                    public LinkedList(in ILinkedList<IProcessErrorItem<TItems, TError>> list)
                    {
                        _list = list;

                        _currentNode = list.First;

                        _error = _currentNode.Value.Error.Error;
                    }

                    private ILinkedListNode<IProcessErrorItem<TItems, TError>> Peek()
                    {
                        ILinkedListNode<IProcessErrorItem<TItems, TError>> node = _previousNode;

                        while ((node = node.Next) != null && !Equals(node.Value.Error.Error, _error))
                        {
                            // Left empty.
                        }

                        return node;
                    }

                    bool _IQueue<IProcessErrorItem<TItems, TError>>.HasItems => (_currentNode
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

                    IProcessErrorItem<TItems, TError> _IQueue<IProcessErrorItem<TItems, TError>>.Peek() => _currentNode.Value;

                    IProcessErrorItem<TItems, TError> _IQueue<IProcessErrorItem<TItems, TError>>.Dequeue()
                    {
                        IProcessErrorItem<TItems, TError> result = _currentNode.Value;

                        _list.Remove(_currentNode);

                        _previousNode = _currentNode;

                        _currentNode = null;

                        return result;
                    }

                    void System.IDisposable.Dispose() => _list = null;
                }
            }
        }
    }
}
