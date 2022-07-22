/* Copyright © Pierre Sprimont, 2021
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

using WinCopies.Commands;
using WinCopies.IO.ObjectModel;
using WinCopies.Util;
using WinCopies.Util.Data;

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.IO.ObjectModel
{
    public interface IBrowsableObjectInfoCollectionViewModel : DotNetFix.IDisposable
    {
        IList<IExplorerControlViewModel> Paths { get; }

        IExplorerControlViewModel SelectedItem { get; set; }

        IEnumerable TabCommands { get; }

        int SelectedIndex { get; set; }

        bool IsCheckBoxVisible { get; set; }

        IBrowsableObjectInfo GetDefaultBrowsableObjectInfo();
    }

    public abstract class BrowsableObjectInfoCollectionViewModel : ViewModelBase, IBrowsableObjectInfoCollectionViewModel
    {
        private IExplorerControlViewModel _selectedItem;
        private int _selectedIndex;
        private bool _checkBoxVisible;
        private ObservableCollection<IExplorerControlViewModel> _paths;

        public bool IsDisposed => _paths == null;

        public ObservableCollection<IExplorerControlViewModel> Paths => GetIfNotDisposed(_paths);

        public IExplorerControlViewModel SelectedItem { get => _selectedItem; set => UpdateValue(ref _selectedItem, value, nameof(SelectedItem)); }

        public IEnumerable TabCommands { get; }

        public int SelectedIndex { get => _selectedIndex; set => UpdateValue(ref _selectedIndex, value, nameof(SelectedIndex)); }

        public bool IsCheckBoxVisible
        {
            get => _checkBoxVisible; set
            {
                if (UpdateValue(ref _checkBoxVisible, value, nameof(IsCheckBoxVisible)))

                    OnIsCheckBoxVisibleChanged();
            }
        }

        IList<IExplorerControlViewModel> IBrowsableObjectInfoCollectionViewModel.Paths => Paths;

        private BrowsableObjectInfoCollectionViewModel(ObservableCollection<IExplorerControlViewModel> items)
        {
            (_paths = items).CollectionChanged += Paths_CollectionChanged;

            TabCommands = new object[] { new DelegateCommand(Bool.True, obj => items.Add(BrowsableObjectInfo.GetDefaultExplorerControlViewModel(GetDefaultBrowsableObjectInfo()))) };
        }

        protected BrowsableObjectInfoCollectionViewModel() : this(new ObservableCollection<IExplorerControlViewModel>()) { /* Left empty. */ }

        protected BrowsableObjectInfoCollectionViewModel(in IEnumerable<IExplorerControlViewModel> items) : this(new ObservableCollection<IExplorerControlViewModel>(items)) { /* Left empty. */ }

        protected BrowsableObjectInfoCollectionViewModel(params IExplorerControlViewModel[] items) : this(items.AsEnumerable()) { /* Left empty. */ }

        public abstract IBrowsableObjectInfo GetDefaultBrowsableObjectInfo();

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

        protected virtual void OnPathAdded(IExplorerControlViewModel path)
        {
            path.IsCheckBoxVisible = _checkBoxVisible;

            path.OpenInNewContextCommand = new DelegateCommand<IBrowsableObjectInfo>(Bool.True, obj =>
            {
                IExplorerControlViewModel _obj = SelectedItem.Factory.GetExplorerControlViewModel(SelectedItem.Factory.GetBrowsableObjectInfoViewModel(obj));

                _obj.IsSelected = true;

                Paths.Add(_obj);
            });
        }

        protected virtual void OnPathRemoved(IExplorerControlViewModel path)
        {
            path.OpenInNewContextCommand = null;

            path.Dispose();
        }

        protected virtual void OnPathCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)

                foreach (object item in e.NewItems)

                    OnPathAdded((IExplorerControlViewModel)item);

            if (e.OldItems != null)

                foreach (object item in e.OldItems)

                    OnPathRemoved((IExplorerControlViewModel)item);
        }

        private void Paths_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPathCollectionChanged(e);

        protected virtual void OnIsCheckBoxVisibleChanged() => Paths.ForEach((in IExplorerControlViewModel path) => path.IsCheckBoxVisible = _checkBoxVisible);

        protected T GetIfNotDisposed<T>(in T value) => GetOrThrowIfDisposed(this, value);

        protected virtual void Dispose(in bool disposing)
        {
            _paths.CollectionChanged -= Paths_CollectionChanged;

            Application.Current.Dispatcher.Invoke(_paths.Clear, System.Windows.Threading.DispatcherPriority.Normal);

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
