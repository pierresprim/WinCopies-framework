﻿/* Copyright © Pierre Sprimont, 2020
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
using System.Collections.ObjectModel;

using WinCopies.GUI.Controls.Models;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public interface IBrowsableObjectInfoViewModel : IBrowsableObjectInfo, IBrowsableObjectInfoViewModelCommon, IEquatable<IBrowsableObjectInfoViewModel>, IComparable<IBrowsableObjectInfoViewModel>
    {
        bool RootParentIsRootNode { get; }

        IBrowsableObjectInfo Model { get; }

        bool IsSelected { get; set; }

        int SelectedIndex { get; set; }

        IBrowsableObjectInfo SelectedItem { get; set; }

        ObservableCollection<IBrowsableObjectInfoViewModel> Items { get; }

        Comparison<IBrowsableObjectInfo> SortComparison { get; set; }

        IBrowsableObjectInfoFactory Factory { get; set; }
    }
}
