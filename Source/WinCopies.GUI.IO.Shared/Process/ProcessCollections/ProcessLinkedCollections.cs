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

using System;
using System.Collections.Specialized;

using WinCopies.Collections.DotNetFix;
using WinCopies.IO;

namespace WinCopies.GUI.IO.Process
{
    public enum ProcessLinkedCollectionAddMode : byte
    {
        Ascending = 1,

        Descending = 2
    }

    public class ProcessLinkedCollection<T> : ObservableLinkedCollection<T>, IProcessCollection<T> where T : IPathInfo
    {
        public ProcessLinkedCollectionAddMode AddMode { get; }

        public ProcessLinkedCollection(ProcessLinkedCollectionAddMode addMode) => AddMode = addMode;

        public new event SimpleLinkedCollectionChangedEventHandler<T> CollectionChanged;

        private static NotifyCollectionChangedAction LinkedCollectionChangedActionToNotifyCollectionChangedAction(LinkedCollectionChangedAction action)
        {
            switch (action)
            {
                case LinkedCollectionChangedAction.AddFirst:
                case LinkedCollectionChangedAction.AddLast:
                case LinkedCollectionChangedAction.AddBefore:
                case LinkedCollectionChangedAction.AddAfter:

                    return NotifyCollectionChangedAction.Add;

                case LinkedCollectionChangedAction.Move:

                    return NotifyCollectionChangedAction.Move;

                case LinkedCollectionChangedAction.Remove:

                    return NotifyCollectionChangedAction.Remove;

                case LinkedCollectionChangedAction.Reset:

                    return NotifyCollectionChangedAction.Reset;

                default:

                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        }

        protected override void OnCollectionChanged(LinkedCollectionChangedEventArgs<T> e)
        {
            base.OnCollectionChanged(e);

            CollectionChanged?.Invoke(this, new SimpleLinkedCollectionChangedEventArgs<T>(LinkedCollectionChangedActionToNotifyCollectionChangedAction(e.Action), e.Node == null ? default : e.Node.Value));
        }

        public void Add(T item)
        {
            switch (AddMode)
            {
                case ProcessLinkedCollectionAddMode.Ascending:

                    _ = AddFirst(item);

                    break;

                case ProcessLinkedCollectionAddMode.Descending:

                    _ = AddLast(item);

                    break;
            }
        }

        public void Remove()
        {
            switch (AddMode)
            {
                case ProcessLinkedCollectionAddMode.Ascending:

                    RemoveFirst();

                    break;

                case ProcessLinkedCollectionAddMode.Descending:

                    RemoveLast();

                    break;
            }
        }

        public T Peek()
#if !CS7
            => AddMode switch
            {
                ProcessLinkedCollectionAddMode.Ascending => First.Value,
                ProcessLinkedCollectionAddMode.Descending => Last.Value,
                _ => throw new InvalidOperationException($"{nameof(AddMode)} is out of range."),
            };
#else
        {
            switch (AddMode)
            {
                case ProcessLinkedCollectionAddMode.Ascending:

                    return First.Value;

                case ProcessLinkedCollectionAddMode.Descending:

                    return Last.Value;

                default:

                    throw new InvalidOperationException($"{nameof(AddMode)} is out of range.");
            }
        }
#endif
    }

    public sealed class ProcessLinkedCollection : ProcessQueueCollection<IPathInfo>, IProcessCollection { }

    public sealed class ReadOnlyProcessLinkedCollection : ReadOnlyObservableQueueCollection<IPathInfo>, IReadOnlyProcessCollection
    {
        private Size _size = new Size(0ul);

        public Size Size { get => _size; private set { _size = value; RaisePropertyChangedEvent(nameof(Size)); } }

        public ReadOnlyProcessLinkedCollection(ObservableQueueCollection<IPathInfo> queueCollection) : base(queueCollection) { }

        protected override void OnCollectionChanged(SimpleLinkedCollectionChangedEventArgs<IPathInfo> e)
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

        public void DecrementSize(ulong sizeInBytes) => Size -= sizeInBytes;
    }

    public sealed class ProcessErrorPathLinkedCollection : ProcessLinkedCollection<IErrorPathInfo>, IProcessErrorPathCollection
    {
        public ProcessErrorPathLinkedCollection(ProcessLinkedCollectionAddMode addMode) : base(addMode) { }
    }

    public sealed class ReadOnlyProcessErrorPathLinkedCollection : ReadOnlyObservableQueueCollection<IErrorPathInfo>, IReadOnlyProcessErrorPathCollection
    {
        public ReadOnlyProcessErrorPathLinkedCollection(ObservableQueueCollection<IErrorPathInfo> queue) : base(queue) { }
    }
}
