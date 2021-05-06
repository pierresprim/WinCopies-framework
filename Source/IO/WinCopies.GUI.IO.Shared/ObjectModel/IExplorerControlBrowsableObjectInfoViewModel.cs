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

using System.Collections;
using System.ComponentModel;
using System.Windows.Controls;

using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public interface IBrowsableObjectInfoViewModelCommon : INotifyPropertyChanged
    {
        bool IsSelected { get; set; }
    }

    public interface IExplorerControlBrowsableObjectInfoViewModel : IBrowsableObjectInfoViewModelCommon
    {
        System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> TreeViewItems { get; set; }

        string Text { get; set; }

        IBrowsableObjectInfoViewModel Path { get; set; }

        ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo> History { get; }

        IBrowsableObjectInfoFactory Factory { get; set; }

        SelectionMode SelectionMode { get; set; }

        IList SelectedItems { get; set; }

        bool IsCheckBoxVisible { get; set; }
    }
}
