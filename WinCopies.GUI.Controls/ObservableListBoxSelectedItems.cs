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

using System.Collections.Specialized;
using System.Windows.Controls;

namespace WinCopies.GUI.Controls
{

    // todo: use PushBinding

    /// <summary>
    /// Allows the user to be notified when the selection of a <see cref="System.Windows.Controls.ListBox"/> has changed.
    /// </summary>
    public class ObservableListBoxSelectedItems : INotifyCollectionChanged
    {

        public ListBox ListBox { get; } = null;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableListBoxSelectedItems(ListBox listBox)

        {

            this.ListBox = listBox;

            listBox.SelectionChanged += ListBox_SelectionChanged;

        }

        private protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)

        {

            if (e.AddedItems.Count > 0)    

                CollectionChanged?.Invoke(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.AddedItems));    

            if (e.RemovedItems.Count > 0)    

                CollectionChanged?.Invoke(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.RemovedItems));    

        }

        //private protected void RaiseCollectionChangedEvent(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => OnSelectionChanged(e);

        //public IEnumerator GetEnumerator() => ListBox.SelectedItems.GetEnumerator();

        //public void CopyTo(Array array, int index) => ListBox.SelectedItems.CopyTo(array, index);

    }
}
