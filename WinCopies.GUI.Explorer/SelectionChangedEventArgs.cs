/* Copyright © Pierre Sprimont, 2019
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WinCopies.Collections;

namespace WinCopies.GUI.Explorer
{
    public class SelectionChangedEventArgs : RoutedEventArgs
    {
        public ActionsFromObjects SelectionSource { get; private set; }

        public IReadOnlyList AddedItems { get; private set; }

        public IReadOnlyList RemovedItems { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionChangedEventArgs"/> class. This constructor will always create a <see cref="ReadOnlyArrayList"/> wrapper. To use directly a <see cref="IReadOnlyList"/>, see the <see cref="SelectionChangedEventArgs(AccessibilitySwitches)"/> constructor.
        /// </summary>
        /// <param name="selectionSource">The source of the selection change</param>
        /// <param name="addedItems">The added items</param>
        /// <param name="removedItems">The removed items.</param>
        public SelectionChangedEventArgs(ActionsFromObjects selectionSource, IList addedItems, IList removedItems) : this(selectionSource, new ReadOnlyArrayList(addedItems), new ReadOnlyArrayList(removedItems)) { }

        public SelectionChangedEventArgs(RoutedEvent routedEvent, ActionsFromObjects selectionSource, IList addedItems, IList removedItems) : this(routedEvent, selectionSource, new ReadOnlyArrayList(addedItems), new ReadOnlyArrayList(removedItems)) { }

        public SelectionChangedEventArgs(RoutedEvent routedEvent, object source, ActionsFromObjects selectionSource, IList addedItems, IList removedItems) : this(routedEvent, source, selectionSource, new ReadOnlyArrayList(addedItems), new ReadOnlyArrayList(removedItems)) { }

        public SelectionChangedEventArgs(ActionsFromObjects selectionSource, IReadOnlyList addedItems, IReadOnlyList removedItems) => Init(selectionSource, addedItems, removedItems);

        public SelectionChangedEventArgs(RoutedEvent routedEvent, ActionsFromObjects selectionSource, IReadOnlyList addedItems, IReadOnlyList removedItems) : base(routedEvent) => Init(selectionSource, addedItems, removedItems);

        public SelectionChangedEventArgs(RoutedEvent routedEvent, object source, ActionsFromObjects selectionSource, IReadOnlyList addedItems, IReadOnlyList removedItems) : base(routedEvent, source) => Init(selectionSource, addedItems, removedItems);

        private void Init(ActionsFromObjects selectionSource, IReadOnlyList addedItems, IReadOnlyList removedItems)
        {

            SelectionSource = selectionSource;

            AddedItems = addedItems;

            RemovedItems = removedItems;

        }

    }

    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);
}
