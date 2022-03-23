//* Copyright © Pierre Sprimont, 2021
//*
//* This file is part of the WinCopies Framework.
//*
//* The WinCopies Framework is free software: you can redistribute it and/or modify
//* it under the terms of the GNU General Public License as published by
//* the Free Software Foundation, either version 3 of the License, or
//* (at your option) any later version.
//*
//* The WinCopies Framework is distributed in the hope that it will be useful,
//* but WITHOUT ANY WARRANTY; without even the implied warranty of
//* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//* GNU General Public License for more details.
//*
//* You should have received a copy of the GNU General Public License
//* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System.Linq;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Shell
{
    public interface IBrowsableObjectInfoWindowMenuItemViewModel
    {
        string ResourceId { get; set; }

        IBrowsableObjectInfoWindowMenuItemViewModel ParentMenuItem { get; }

        bool IsSelected { get; set; }

        string StatusBarLabel { get; }
    }

    public class BrowsableObjectInfoWindowMenuItemViewModel : ViewModelBase, IBrowsableObjectInfoWindowMenuItemViewModel
    {
        private bool _isSelected;
        private string _statusBarLabel;
        private readonly BrowsableObjectInfoWindowMenuViewModel _parentMenu;

        public string ResourceId { get; set; }

        public IBrowsableObjectInfoWindowMenuItemViewModel ParentMenuItem { get; }

        public bool IsSelected
        {
            get => _isSelected; set
            {
                _isSelected = value;

                _parentMenu.SelectedItem = this; OnPropertyChanged(nameof(IsSelected)
#if !WinCopies4
                    , null, IsSelected
#endif
                    );
            }
        }

        public string StatusBarLabel => _statusBarLabel
#if CS8
            ??=
#else
            ?? (_statusBarLabel =
#endif
            (string)typeof(Properties.Resources).GetProperties().FirstOrDefault(p => p.Name == $"{ResourceId}StatusBarLabel")?.GetValue(null)
#if !CS8
            )
#endif
            ;

        public BrowsableObjectInfoWindowMenuItemViewModel(in BrowsableObjectInfoWindowMenuViewModel parentMenu/*, in string header, in string resourceId, in RoutedCommand command, Func commandParameter, ImageSource iconImageSource*/) /*: this(parentMenu._Items, header, resourceId, command, commandParameter, iconImageSource)*/ => _parentMenu = parentMenu;

        public BrowsableObjectInfoWindowMenuItemViewModel(in BrowsableObjectInfoWindowMenuItemViewModel parentMenuItem/*, in string header, in string resourceId, in RoutedCommand command, Func commandParameter, ImageSource iconImageSource*/) //: this(parentMenuItem._Items, header, resourceId, command, commandParameter, iconImageSource)
        {
            _parentMenu = parentMenuItem._parentMenu;

            ParentMenuItem = parentMenuItem;
        }
    }
}
