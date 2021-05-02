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

#if CS7 && WinCopies3

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using WinCopies.Collections.DotNetFix.Generic;

using static WinCopies.ThrowHelper;
using static WinCopies.UtilHelpers;

namespace WinCopies.Collections.Generic
{
    public interface ILinkedListNodeEnumerable
    {
        object Value { get; }

        ILinkedListEnumerable List { get; }

        bool CanMovePreviousFromCurrent { get; }

        bool CanMoveNextFromCurrent { get; }
    }

    public interface ILinkedListNodeEnumerable<T> : ILinkedListNodeEnumerable
    {
        ILinkedListNode<T> Node { get; }

        ILinkedListEnumerable<T> List { get; }

#if CS8
        object ILinkedListNodeEnumerable.Value => Node.Value;

        ILinkedListEnumerable ILinkedListNodeEnumerable.List => List;

        bool ILinkedListNodeEnumerable.CanMovePreviousFromCurrent => Node.Previous != null;

        bool ILinkedListNodeEnumerable.CanMoveNextFromCurrent => Node.Next != null;
#endif
    }

    public sealed class LinkedListNodeEnumerable<T> : ILinkedListNodeEnumerable<T>
    {
        public ILinkedListNode<T> Node { get; }

        public ILinkedListEnumerable<T> List { get; }

        public LinkedListNodeEnumerable(in ILinkedListNode<T> node, in ILinkedListEnumerable<T> list)
        {
            Node = node;

            List = list;
        }

#if !CS8
        object ILinkedListNodeEnumerable.Value => Node.Value;

        ILinkedListEnumerable ILinkedListNodeEnumerable.List => List;

        bool ILinkedListNodeEnumerable.CanMovePreviousFromCurrent => Node.Previous != null;

        bool ILinkedListNodeEnumerable.CanMoveNextFromCurrent => Node.Next != null;
#endif
    }

    public interface ILinkedListEnumerable : System.Collections.IEnumerable
    {
        ILinkedListNodeEnumerable First { get; }

        ILinkedListNodeEnumerable Current { get; }

        ILinkedListNodeEnumerable Last { get; }

        bool MovePrevious();

        bool MoveNext();

        void UpdateCurrent(DotNetFix.IReadOnlyLinkedListNode node);
    }

    public interface ILinkedListEnumerable<T> : ILinkedListEnumerable, System.Collections.Generic.IEnumerable<T>
    {
        ILinkedListNodeEnumerable<T> First { get; }

        ILinkedListNodeEnumerable<T> Current { get; }

        ILinkedListNodeEnumerable<T> Last { get; }

        void UpdateCurrent(ILinkedListNode<T> node);

        System.Collections.Generic.IEnumerator<ILinkedListNode<T>> GetEnumeratorToCurrent(bool keepCurrent);

        System.Collections.Generic.IEnumerator<ILinkedListNode<T>> GetEnumeratorFromCurrent(bool keepCurrent);

#if CS8
        ILinkedListNodeEnumerable ILinkedListEnumerable.First => First;

        ILinkedListNodeEnumerable ILinkedListEnumerable.Current => Current;

        ILinkedListNodeEnumerable ILinkedListEnumerable.Last => Last;
#endif
    }

    public class LinkedListEnumerable<T> : ILinkedListEnumerable<T>
    {
        private ILinkedListNodeEnumerable<T> _first;
        private ILinkedListNodeEnumerable<T> _current;
        private ILinkedListNodeEnumerable<T> _last;

        protected ILinkedList<T> InnerList { get; }

        public ILinkedListNodeEnumerable<T> First => _first.Node.List == InnerList ? _first : (_first = null);

        public ILinkedListNodeEnumerable<T> Current => _current
#if CS8
            ??=
#else
            ?? (_current =
#endif
            new LinkedListNodeEnumerable<T>(InnerList.First, this)
#if !CS8
            )
#endif
            ;

        public ILinkedListNodeEnumerable<T> Last => _last.Node.List == InnerList ? _last : (_last = null);

        public LinkedListEnumerable(in ILinkedList<T> list, in ILinkedListNode<T> first, in ILinkedListNode<T> current, in ILinkedListNode<T> last)
        {
            InnerList = list ?? throw GetArgumentNullException(nameof(list));

            _first = new LinkedListNodeEnumerable<T>(first, this);

            _last = new LinkedListNodeEnumerable<T>(last, this);

            _current = new LinkedListNodeEnumerable<T>(current ?? first ?? list.First, this);
        }

        public LinkedListEnumerable(in ILinkedList<T> list) : this(list, null, null, null) { /* Left empty. */ }

        public LinkedListEnumerable() : this(new DotNetFix.Generic.LinkedList<T>(), null, null, null) { /* Left empty. */ }

        public bool MovePrevious()
        {
            if (Current?.Node.Previous == null)

                return false;

            _current = new LinkedListNodeEnumerable<T>(Current.Node.Previous, this);

            return true;
        }

        public bool MoveNext()
        {
            if (Current?.Node.Next == null)

                return false;

            _current = new LinkedListNodeEnumerable<T>(Current.Node.Next, this);

            return true;
        }

        public void UpdateCurrent(ILinkedListNode<T> node) => _current = new LinkedListNodeEnumerable<T>(node ?? throw GetArgumentNullException(nameof(node)), this);

        public static ILinkedListNode<T> TryGetNode(in DotNetFix.IReadOnlyLinkedListNode node) => node as ILinkedListNode<T>;

        public static ArgumentException GetInvalidNodeException(in string argumentName) => throw new ArgumentException($"The given node does not implement the {nameof(ILinkedListNode<T>)} interface.", argumentName);

        public static ILinkedListNode<T> GetNode(in DotNetFix.IReadOnlyLinkedListNode node, in string argumentName) => TryGetNode(node) ?? throw GetInvalidNodeException(argumentName);

        void ILinkedListEnumerable.UpdateCurrent(DotNetFix.IReadOnlyLinkedListNode node) => UpdateCurrent(GetNode(node, nameof(node)));

        public System.Collections.Generic.IEnumerator<ILinkedListNode<T>> GetEnumeratorToCurrent( bool keepCurrent) => WinCopies.Temp.Temp.GetNodeEnumerator(InnerList, DotNetFix.EnumerationDirection.FIFO,  InnerList. First, keepCurrent ? (ILinkedListNode<T>) Current.Node : (ILinkedListNode<T>)Current.Node.Previous);

        public System.Collections.Generic.IEnumerator<ILinkedListNode<T>> GetEnumeratorFromCurrent( bool keepCurrent) => WinCopies.Temp.Temp.GetNodeEnumerator(InnerList, DotNetFix.EnumerationDirection.FIFO, keepCurrent ? (ILinkedListNode<T>)Current.Node : (ILinkedListNode<T>)Current.Node.Next, InnerList. Last);

        public System.Collections.Generic.IEnumerator<T> GetEnumerator() => new Enumerable<ILinkedListNode<T>>(()=> Temp.Temp.GetNodeEnumerator(InnerList, DotNetFix.EnumerationDirection.FIFO, InnerList.First, InnerList.Last)).Select(node=>node.Value).GetEnumerator();

        System.Collections.IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#if !CS8
        ILinkedListNodeEnumerable ILinkedListEnumerable.First => First;

        ILinkedListNodeEnumerable ILinkedListEnumerable.Current => Current;

        ILinkedListNodeEnumerable ILinkedListEnumerable.Last => Last;
#endif
    }

    public class LinkedCollectionEnumerable<T> : ILinkedListEnumerable<T>
    {
        protected ILinkedListEnumerable<T> InnerList { get; }

        public ILinkedListNodeEnumerable<T> First => InnerList.First;

        public ILinkedListNodeEnumerable<T> Current => InnerList.Current;

        public ILinkedListNodeEnumerable<T> Last => InnerList.Last;

        public LinkedCollectionEnumerable(in ILinkedListEnumerable<T> list) => InnerList = list;

        public LinkedCollectionEnumerable() : this(new LinkedListEnumerable<T>()) { /* Left empty. */ }

        protected virtual void OnUpdateCurrent(ILinkedListNode<T> node) => InnerList.UpdateCurrent(node);

        public void UpdateCurrent(ILinkedListNode<T> node) => OnUpdateCurrent(node);

        void ILinkedListEnumerable.UpdateCurrent(DotNetFix.IReadOnlyLinkedListNode node) => OnUpdateCurrent(LinkedListEnumerable<T>.GetNode(node, nameof(node)));

        public bool MovePrevious() => InnerList.MovePrevious();

        public bool MoveNext() => InnerList.MoveNext();

        public System.Collections.Generic.IEnumerator<ILinkedListNode<T>> GetEnumeratorToCurrent(bool keepCurrent) => InnerList.GetEnumeratorToCurrent(keepCurrent);

        public System.Collections.Generic.IEnumerator<ILinkedListNode<T>> GetEnumeratorFromCurrent(bool keepCurrent) => InnerList.GetEnumeratorFromCurrent(keepCurrent);

        public System.Collections.Generic.IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)InnerList).GetEnumerator();

#if !CS8
        ILinkedListNodeEnumerable ILinkedListEnumerable.First => First;

        ILinkedListNodeEnumerable ILinkedListEnumerable.Current => Current;

        ILinkedListNodeEnumerable ILinkedListEnumerable.Last => Last;
#endif
    }

    public class ObservableLinkedCollectionEnumerable<T> : LinkedCollectionEnumerable<T>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableLinkedCollectionEnumerable(in ILinkedListEnumerable<T> list) : base(list) { /* Left empty. */ }

        public ObservableLinkedCollectionEnumerable() : this(new LinkedListEnumerable<T>()) { /* Left empty. */ }

        protected void OnPropertyChanged(in string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected override void OnUpdateCurrent(ILinkedListNode<T> node)
        {
            base.OnUpdateCurrent(node);

            OnPropertyChanged(nameof(Current));
        }
    }
}

#endif
