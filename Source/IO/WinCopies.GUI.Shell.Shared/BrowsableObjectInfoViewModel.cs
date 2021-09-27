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

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO.Process;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Shell
{
    public class BrowsableObjectInfoWindowMenuViewModel : ViewModelBase
    {
        private BrowsableObjectInfoWindowMenuItemViewModel _selectedItem;

        public BrowsableObjectInfoWindowMenuItemViewModel SelectedItem { get => _selectedItem; internal set => UpdateValue(ref _selectedItem, value.IsSelected ? value : null, nameof(SelectedItem)); }
    }

    public class BrowsableObjectInfoWindowMenuItemViewModel : ViewModelBase
    {
        private readonly BrowsableObjectInfoWindowMenuViewModel _parentMenu;

        public string ResourceId { get; set; }

        public BrowsableObjectInfoWindowMenuItemViewModel ParentMenuItem { get; }

        private bool _isSelected;

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

        private string _statusBarLabel;

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

    public class BrowsableObjectInfoWindowViewModel : ViewModelBase
    {
        public static IProcessPathCollectionFactory DefaultProcessPathCollectionFactory { get; } = new ProcessPathCollectionFactory();

        public BrowsableObjectInfoCollectionViewModel Paths { get; }

        public BrowsableObjectInfoWindowMenuViewModel Menu { get; }

        public BrowsableObjectInfoWindowViewModel(in BrowsableObjectInfoCollectionViewModel paths)
        {
            Paths = paths;

            Menu = new BrowsableObjectInfoWindowMenuViewModel();

            // MainWindowModel.Init(Paths);
        }

        public BrowsableObjectInfoWindowViewModel() : this(new BrowsableObjectInfoCollectionViewModel()) { /* Left empty. */ }
    }
}
