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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using WinCopies.Util;
using WinCopies.Util.Data;

namespace WinCopies.GUI.IO.ObjectModel
{
    public interface IBrowsableObjectInfoCollectionViewModel
    {
        ICollection<IExplorerControlBrowsableObjectInfoViewModel> Paths { get; }

        IExplorerControlBrowsableObjectInfoViewModel SelectedItem { get; set; }

        int SelectedIndex { get; set; }
    }

    public class BrowsableObjectInfoCollectionViewModel : ViewModel<Collection<IExplorerControlBrowsableObjectInfoViewModel>>, IBrowsableObjectInfoCollectionViewModel
    {
        private IExplorerControlBrowsableObjectInfoViewModel _selectedItem;
        private int _selectedIndex;
        private bool _checkBoxVisible;

        public ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> Paths { get; } = new ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>();

        public IExplorerControlBrowsableObjectInfoViewModel SelectedItem { get => _selectedItem; set => UpdateValue(ref _selectedItem, value, nameof(SelectedItem)); }

        public int SelectedIndex { get => _selectedIndex; set => UpdateValue(ref _selectedIndex, value, nameof(SelectedIndex)); }

        public bool IsCheckBoxVisible
        {
            get => _checkBoxVisible; set
            {
                if (UpdateValue(ref _checkBoxVisible, value, nameof(IsCheckBoxVisible)))

                    OnIsCheckBoxVisibleChanged();
            }
        }

        ICollection<IExplorerControlBrowsableObjectInfoViewModel> IBrowsableObjectInfoCollectionViewModel.Paths => Paths;

        public BrowsableObjectInfoCollectionViewModel(Collection<IExplorerControlBrowsableObjectInfoViewModel> collection) : base(collection)
        {
            if (collection is INotifyCollectionChanged _collection)

                _collection.CollectionChanged += Paths_CollectionChanged;
        }

        public BrowsableObjectInfoCollectionViewModel() : this(new ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>()) { /* Left empty. */ }

        private bool UpdateValue<T>(ref T value, in T newValue, in string propertyName)
        {
            T oldValue = value;

            if (UtilHelpers.UpdateValue(ref value, newValue))
            {
                OnPropertyChanged(propertyName, oldValue, newValue);

                return true;
            }

            return false;
        }

        protected virtual void OnPathAdded(IExplorerControlBrowsableObjectInfoViewModel path)
        {
            path.IsCheckBoxVisible = _checkBoxVisible;

            // path.CustomProcessParametersGeneratedEventHandler += Item_CustomProcessParametersGeneratedEventHandler;
        }

        protected virtual void OnPathCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)

                foreach (object item in e.NewItems)

                    OnPathAdded((IExplorerControlBrowsableObjectInfoViewModel)item);

            //if (e.OldItems != null)

            //    foreach (object _item in e.OldItems)

            //        ((IExplorerControlBrowsableObjectInfoViewModel)_item).CustomProcessParametersGeneratedEventHandler -= Item_CustomProcessParametersGeneratedEventHandler;
        }

        private void Paths_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPathCollectionChanged(e);

        protected virtual void OnIsCheckBoxVisibleChanged() => Paths.ForEach((in IExplorerControlBrowsableObjectInfoViewModel path) => path.IsCheckBoxVisible = _checkBoxVisible);
    }
}
