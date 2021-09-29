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

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.IO.ObjectModel
{
    public interface IBrowsableObjectInfoCollectionViewModel : DotNetFix.IDisposable
    {
        ICollection<IExplorerControlBrowsableObjectInfoViewModel> Paths { get; }

        IExplorerControlBrowsableObjectInfoViewModel SelectedItem { get; set; }

        int SelectedIndex { get; set; }
    }

    public class BrowsableObjectInfoCollectionViewModel : ViewModelBase, IBrowsableObjectInfoCollectionViewModel
    {
        private IExplorerControlBrowsableObjectInfoViewModel _selectedItem;
        private int _selectedIndex;
        private bool _checkBoxVisible;
        private ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> _paths = new ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>();

        public bool IsDisposed => _paths == null;

        public ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> Paths => GetIfNotDisposed(_paths);

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

        public BrowsableObjectInfoCollectionViewModel() => Paths.CollectionChanged += Paths_CollectionChanged;

        private bool UpdateValue<T>(ref T value, in T newValue, in string propertyName)
        {
            if (IsDisposed)

                throw GetExceptionForDispose(false);

            T oldValue = value;

            if (UtilHelpers.UpdateValue(ref value, newValue))
            {
                OnPropertyChanged(propertyName, oldValue, newValue);

                return true;
            }

            return false;
        }

        protected virtual void OnPathAdded(IExplorerControlBrowsableObjectInfoViewModel path) => path.IsCheckBoxVisible = _checkBoxVisible;

        protected virtual void OnPathRemoved(IExplorerControlBrowsableObjectInfoViewModel path) => path.Dispose();

        protected virtual void OnPathCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)

                foreach (object item in e.NewItems)

                    OnPathAdded((IExplorerControlBrowsableObjectInfoViewModel)item);

            if (e.OldItems != null)

                foreach (object item in e.OldItems)

                    OnPathRemoved((IExplorerControlBrowsableObjectInfoViewModel)item);
        }

        private void Paths_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPathCollectionChanged(e);

        protected virtual void OnIsCheckBoxVisibleChanged() => Paths.ForEach((in IExplorerControlBrowsableObjectInfoViewModel path) => path.IsCheckBoxVisible = _checkBoxVisible);

        protected T GetIfNotDisposed<T>(in T value) => GetOrThrowIfDisposed(this, value);

        protected virtual void Dispose(in bool disposing)
        {
            _paths.CollectionChanged -= Paths_CollectionChanged;
            _paths.Clear();
            _paths = null;

            if (disposing)
            {
                _selectedItem = null;

                _selectedIndex = -1;

                _checkBoxVisible = false;
            }
        }

        public void Dispose() => Dispose(true);

        ~BrowsableObjectInfoCollectionViewModel() => Dispose(false);
    }
}
