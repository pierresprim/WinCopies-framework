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

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;

using static WinCopies.Collections.ThrowHelper;

namespace WinCopies.IO.Process.ObjectModel
{
    public static partial class ProcessObjectModelTypes<TItems, TFactory, TError, TProcessDelegates, TProcessDelegateParam>
    {
        public abstract partial class Process : IProcess<TItems, TError>, DotNetFix.IDisposable
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

                bool _IQueue<IProcessErrorItem<TItems, TError>>.HasItems => (_currentNode ??= Peek()) != null;

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

            public class ReadOnlyQueue : IQueue<IProcessErrorItem<TItems, TError>>
            {
                private readonly ILinkedList<IProcessErrorItem<TItems, TError>> _list;

                object ISimpleLinkedListBase2.SyncRoot => _list.SyncRoot;

                bool ISimpleLinkedListBase2.IsSynchronized => _list.IsSynchronized;

                uint IUIntCountable.Count => _list.Count;

                bool ISimpleLinkedListBase.IsReadOnly => true;

                bool ISimpleLinkedListBase.HasItems => _list.Count != 0;

                public ReadOnlyQueue(in ILinkedList<IProcessErrorItem<TItems, TError>> list) => _list = list;

                public void Clear() => throw GetReadOnlyListOrCollectionException();

                public IProcessErrorItem<TItems, TError> Peek() => _list.First.Value;

                public bool TryPeek(out IProcessErrorItem<TItems, TError> result)
                {
                    if (_list.Count == 0)
                    {
                        result = null;

                        return false;
                    }

                    result = _list.First.Value;

                    return true;
                }

                void IQueueBase<IProcessErrorItem<TItems, TError>>.Enqueue(IProcessErrorItem<TItems, TError> item) => throw GetReadOnlyListOrCollectionException();

                IProcessErrorItem<TItems, TError> IQueueBase<IProcessErrorItem<TItems, TError>>.Dequeue() => throw GetReadOnlyListOrCollectionException();

                bool IQueueBase<IProcessErrorItem<TItems, TError>>.TryDequeue(out IProcessErrorItem<TItems, TError> result)
                {
                    result = null;

                    return false;
                }
            }
        }
    }
}
