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

using System.Collections.ObjectModel;
using System.ComponentModel;
using WinCopies.IO;

namespace WinCopies.GUI.Explorer
{
    /// <summary>
    /// Provides properties to interact with graphical <see cref="WinCopies.IO.Explorer.IBrowsableObjectInfo"/> objects.
    /// </summary>
    public interface IBrowsableObjectInfo : IO.IBrowsableObjectInfo
    {

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="Explorer.IBrowsableObjectInfo"/> is selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the selected item of this <see cref="Explorer.IBrowsableObjectInfo"/>.
        /// </summary>
        Explorer.IBrowsableObjectInfo SelectedItem { get; set; }

        /// <summary>
        /// Gets a collection that represents the selected items of this <see cref="Explorer.IBrowsableObjectInfo"/>.
        /// </summary>
        ReadOnlyObservableCollection<Explorer.IBrowsableObjectInfo> SelectedItems { get; }

        bool IsCheckBoxEnabled { get; set; }

    }

    public interface IBrowsableObjectInfoViewModel : Explorer.IBrowsableObjectInfo, INotifyPropertyChanged
    {

        IBrowsableObjectInfo Model { get; }

    }

    internal interface IBrowsableObjectInfoInternal
    {

        ObservableCollection<Explorer.IBrowsableObjectInfo> SelectedItems { get; set; }

    }

    internal interface IBrowsableObjectInfoHelper
    {

        ReadOnlyObservableCollection<Explorer.IBrowsableObjectInfo> SelectedItems { set; }

    }
}
