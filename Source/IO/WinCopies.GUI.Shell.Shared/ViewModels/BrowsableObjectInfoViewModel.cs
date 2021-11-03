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

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO.Process;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Shell
{
    public interface IBrowsableObjectInfoWindowMenuViewModel
    {
        IBrowsableObjectInfoWindowMenuItemViewModel SelectedItem { get; }
    }

    public class BrowsableObjectInfoWindowMenuViewModel : ViewModelBase, IBrowsableObjectInfoWindowMenuViewModel
    {
        private IBrowsableObjectInfoWindowMenuItemViewModel _selectedItem;

        public IBrowsableObjectInfoWindowMenuItemViewModel SelectedItem { get => _selectedItem; internal set => UpdateValue(ref _selectedItem, value.IsSelected ? value : null, nameof(SelectedItem)); }
    }

    public interface IBrowsableObjectInfoWindowViewModel
    {
        IBrowsableObjectInfoCollectionViewModel Paths { get; }

        IBrowsableObjectInfoWindowMenuViewModel Menu { get; }
    }

    public class BrowsableObjectInfoWindowViewModel : ViewModelBase, IBrowsableObjectInfoWindowViewModel
    {
        public static IProcessPathCollectionFactory DefaultProcessPathCollectionFactory { get; } = new ProcessPathCollectionFactory();

        public IBrowsableObjectInfoCollectionViewModel Paths { get; }

        public IBrowsableObjectInfoWindowMenuViewModel Menu { get; }

        public BrowsableObjectInfoWindowViewModel(in IBrowsableObjectInfoCollectionViewModel paths, in IBrowsableObjectInfoWindowMenuViewModel menu)
        {
            Paths = paths;

            Menu = menu;

            // MainWindowModel.Init(Paths);
        }

        public BrowsableObjectInfoWindowViewModel() : this(new BrowsableObjectInfoCollectionViewModel(), new BrowsableObjectInfoWindowMenuViewModel()) { /* Left empty. */ }

        public BrowsableObjectInfoWindowViewModel(in IBrowsableObjectInfoCollectionViewModel paths) : this(paths, new BrowsableObjectInfoWindowMenuViewModel()) { /* Left empty. */ }

        public BrowsableObjectInfoWindowViewModel(in IBrowsableObjectInfoWindowMenuViewModel menu) : this(new BrowsableObjectInfoCollectionViewModel(), menu) { /* Left empty. */ }
    }
}
