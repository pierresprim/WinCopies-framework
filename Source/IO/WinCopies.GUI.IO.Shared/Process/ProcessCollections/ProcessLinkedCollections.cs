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
        private Size _size = new Size(0ul);

        public ProcessLinkedCollectionAddMode AddMode { get; }

        public Size Size { get => _size; private set { _size = value; RaisePropertyChangedEvent(nameof(Size)); } }

        public ProcessLinkedCollection(ProcessLinkedCollectionAddMode addMode) => AddMode = addMode;

        public new event SimpleLinkedCollectionChangedEventHandler<T> CollectionChanged;

        internal static NotifyCollectionChangedAction LinkedCollectionChangedActionToNotifyCollectionChangedAction(LinkedCollectionChangedAction action)
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

            NotifyCollectionChangedAction action = LinkedCollectionChangedActionToNotifyCollectionChangedAction(e.Action);

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:

                    if (!e.Node.Value.IsDirectory)

                        Size += e.Node.Value.Size.Value;

                    break;

                case NotifyCollectionChangedAction.Reset:

                    Size = new Size(0ul);

                    break;
            }

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

        public T Remove()
        {
            T result = default;

            switch (AddMode)
            {
                case ProcessLinkedCollectionAddMode.Ascending:

                    result = First.Value;

                    RemoveFirst();

                    break;

                case ProcessLinkedCollectionAddMode.Descending:

                    result = Last.Value;

                    RemoveLast();

                    break;
            }

            return result;
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

        public void DecrementSize(ulong sizeInBytes) => Size -= sizeInBytes;
    }

    public sealed class ProcessLinkedCollection : ProcessLinkedCollection<IPathInfo>, IProcessCollection
    {
        public ProcessLinkedCollection(ProcessLinkedCollectionAddMode addMode) : base(addMode)
        {
            // Left empty.
        }
    }

    public class ReadOnlyProcessLinkedCollection<T> : ReadOnlyObservableLinkedCollection<T> where T : IPathInfo
    {
        public new event SimpleLinkedCollectionChangedEventHandler<T> CollectionChanged;

        public ReadOnlyProcessLinkedCollection(ProcessLinkedCollection<T> linkedCollection) : base(linkedCollection) { }

        protected override void OnCollectionChanged(LinkedCollectionChangedEventArgs<T> e)
        {
            base.OnCollectionChanged(e);

            CollectionChanged?.Invoke(this, new SimpleLinkedCollectionChangedEventArgs<T>(ProcessLinkedCollection.LinkedCollectionChangedActionToNotifyCollectionChangedAction(e.Action), e.Node == null ? default : e.Node.Value));
        }
    }

    public sealed class ReadOnlyProcessLinkedCollection : ReadOnlyProcessLinkedCollection<IPathInfo>, IReadOnlyProcessCollection
    {
        public Size Size => ((ProcessLinkedCollection)InnerLinkedCollection).Size;

        public ReadOnlyProcessLinkedCollection(ProcessLinkedCollection linkedCollection) : base(linkedCollection) { }
    }

    public sealed class ProcessErrorPathLinkedCollection : ProcessLinkedCollection<IErrorPathInfo>, IProcessErrorPathCollection
    {
        public ProcessErrorPathLinkedCollection(ProcessLinkedCollectionAddMode addMode) : base(addMode) { }
    }

    public sealed class ReadOnlyProcessErrorPathLinkedCollection : ReadOnlyProcessLinkedCollection<IErrorPathInfo>, IReadOnlyProcessErrorPathCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyProcessErrorPathLinkedCollection"/> class.
        /// </summary>
        /// <param name="linkedCollection">The inner <see cref="ObservableLinkedCollection{IErrorPathInfo}"/> of the new collection.</param>
        public ReadOnlyProcessErrorPathLinkedCollection(ProcessErrorPathLinkedCollection linkedCollection) : base(linkedCollection) { }
    }
}
