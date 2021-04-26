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
using System.Collections.Generic;
using System.ComponentModel;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.IO;
using WinCopies.IO.Process;

using static WinCopies.Collections.ThrowHelper;

namespace WinCopies.GUI.IO.Process
{
    public interface IProcessLinkedList<TItems, TError> : ICollection<IProcessErrorItem<TItems, TError>>, System.Collections.Generic.IEnumerable<IProcessErrorItem<TItems, TError>>, System.Collections.IEnumerable, IReadOnlyCollection<IProcessErrorItem<TItems, TError>>, System.Collections.ICollection, Collections.DotNetFix.Generic.ILinkedList3<IProcessErrorItem<TItems, TError>>, IReadOnlyLinkedList2<IProcessErrorItem<TItems, TError>>, IReadOnlyLinkedList<IProcessErrorItem<TItems, TError>>,  IUIntCountable, Collections.Generic.IEnumerable<IProcessErrorItem<TItems, TError>>, Collections.Generic.IEnumerable<ILinkedListNode<IProcessErrorItem<TItems, TError>>>, System.Collections.Generic.IEnumerable<ILinkedListNode<IProcessErrorItem<TItems, TError>>>,    INotifyPropertyChanged, INotifyLinkedCollectionChanged<IProcessErrorItem<TItems, TError>>, WinCopies.IO.Process.IProcessLinkedList<TItems, TError> where TItems : IPath
    {
        new object SyncRoot { get; }

        new bool IsSynchronized { get; }
    }

    public class ObservableProcessLinkedCollection<TItems, TError> : ObservableLinkedCollection<IProcessErrorItem<TItems, TError>>, IProcessLinkedList<TItems, TError> where TItems : IPath
    {
        public Size TotalSize { get; private set; }

        public object SyncRoot => null;

        public bool IsSynchronized => false;

        public bool HasItems => Count != 0;

        public ObservableProcessLinkedCollection() : base()
        {
            // Left empty.
        }

        public ObservableProcessLinkedCollection(in Collections.DotNetFix.Generic.LinkedList<IProcessErrorItem<TItems, TError>> list) : base(list)
        {
            // Left empty.
        }

        private static InvalidOperationException GetInvalidOperationException() => new InvalidOperationException("This operation is invalid in the current context.");

        public IProcessErrorItem<TItems, TError> Dequeue() => TryDequeue(out IProcessErrorItem<TItems, TError> result) ? result : throw GetInvalidOperationException();

        protected virtual void OnPropertyChanged(in string propertyName) => OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));

        protected virtual void OnEnqueue(IProcessErrorItem<TItems, TError> item)
        {
            if (item.Size.HasValue)

                TotalSize += item.Size.Value;

            _ = AddLast(item);

            OnPropertyChanged(nameof(TotalSize));
        }

        public void Enqueue(IProcessErrorItem<TItems, TError> item) => OnEnqueue(item);

        protected IProcessErrorItem<TItems, TError> OnDequeue()
        {
            IProcessErrorItem<TItems, TError> result = First.Value;

            if (result.Size.HasValue)

                TotalSize -= result.Size.Value;

            RemoveFirst();

            OnPropertyChanged(nameof(TotalSize));

            return result;
        }

        public IProcessErrorItem<TItems, TError> Peek() => TryDequeue(out IProcessErrorItem<TItems, TError> result) ? result : throw GetInvalidOperationException();

        public bool TryDequeue(out IProcessErrorItem<TItems, TError> result)
        {
            if (Count != 0)
            {
                result = OnDequeue();

                return true;
            }

            result = default;

            return false;
        }

        public bool TryPeek(out IProcessErrorItem<TItems, TError> result)
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

            TotalSize = new Size();

            OnPropertyChanged(nameof(TotalSize));
        }

        public ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue AsReadOnly() => new ReadOnlyObservableProcessLinkedCollection<TItems, TError>(this);
    }

    public class ReadOnlyObservableProcessLinkedCollection<TItems, TError> : Collections.DotNetFix.Generic.ReadOnlyObservableLinkedCollection<IProcessErrorItem<TItems, TError>>, ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue, IReadOnlyProcessLinkedList<TItems, TError> where TItems : IPath
    {
        public Size TotalSize => ((IProcessQueue)InnerLinkedCollection).TotalSize;

        object ISimpleLinkedListBase2.SyncRoot => ((ISimpleLinkedListBase2)InnerLinkedCollection).SyncRoot;

        bool ISimpleLinkedListBase2.IsSynchronized => ((ISimpleLinkedListBase2)InnerLinkedCollection).IsSynchronized;

        bool ISimpleLinkedListBase.HasItems => ((ISimpleLinkedListBase)InnerLinkedCollection).HasItems;

        public ReadOnlyObservableProcessLinkedCollection(in ObservableProcessLinkedCollection<TItems, TError> collection) : base(collection)
        {
            // Left empty.
        }

        void IQueue<IProcessErrorItem<TItems, TError>>.Clear() => throw GetReadOnlyListOrCollectionException();

        void ISimpleLinkedListBase2.Clear() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<IProcessErrorItem<TItems, TError>>.Clear() => throw GetReadOnlyListOrCollectionException();

        IProcessErrorItem<TItems, TError> IQueueBase<IProcessErrorItem<TItems, TError>>.Dequeue() => throw GetReadOnlyListOrCollectionException();

        void IQueueBase<IProcessErrorItem<TItems, TError>>.Enqueue(IProcessErrorItem<TItems, TError> item) => throw GetReadOnlyListOrCollectionException();

        public IProcessErrorItem<TItems, TError> Peek() => ((IQueue<IProcessErrorItem<TItems, TError>>)InnerLinkedCollection).Peek();

        bool IQueueBase<IProcessErrorItem<TItems, TError>>.TryDequeue(out IProcessErrorItem<TItems, TError> result)
        {
            result = null;

            return false;
        }

        public bool TryPeek(out IProcessErrorItem<TItems, TError> result) => ((IQueue<IProcessErrorItem<TItems, TError>>)InnerLinkedCollection).TryPeek(out result);

        ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue.AsReadOnly() => this;
    }
}
