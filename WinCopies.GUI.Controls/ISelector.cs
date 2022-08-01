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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.GUI.Controls
{

    // todo: to add this interface to the other FrameworkElements to which they applies.

    /// <summary>
    /// Provides properties that apply to the UI controls that can select at least one child.
    /// </summary>
    public interface ISelector
    {

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        object SelectedItem { get; }

    }

    public interface ISelectionIndexableSelector : ISelector
    {

        /// <summary>
        /// Gets the selected index.
        /// </summary>
        int SelectedIndex { get; }

    }

    /// <summary>
    /// Provides properties that apply to the UI controls that can select at least one child by the item itself or by the parent selector.
    /// </summary>
    public interface ISettableSelector : ISelector
    {

        /// <summary>
        /// Sets the selected item.
        /// </summary>
        new object SelectedItem { get; set; }

    }

    public interface ISelectionIndexableSettableSelector : ISettableSelector
    {

        /// <summary>
        /// Sets the selected index.
        /// </summary>
        int SelectedIndex { get; set; }

    }

    // todo: to implement this interface in the other selectable controls 'listbox, listview, ...)

    /// <summary>
    /// Provides properties that apply to the UI controls that can be selected.
    /// </summary>
    public interface ISelectable

    {

        bool IsSelected { get; set; }

        /// <summary>
        /// Gets a value that indicates whether this item is selected by focus.
        /// </summary>
        bool IsFocusSelection { get; }

        /// <summary>
        /// Gets a value that indicates whether this item was selected before a new selection by focus.
        /// </summary>
        bool IsPreviouslySelectedItem { get; }

    }
}
