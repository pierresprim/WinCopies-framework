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

using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using WinCopies.Collections.Generic;
using WinCopies.Commands;
using WinCopies.GUI.Controls;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Controls.ViewModels;
using WinCopies.GUI.Windows;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.Util.Data;

using static WinCopies.GUI.IO.Properties.Resources;
using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.IO.ObjectModel
{
    public class ExplorerControlBrowsableObjectInfoViewModel : ViewModelBase, IExplorerControlBrowsableObjectInfoViewModel
    {
        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue) => OnPropertyChanged(new WinCopies.Util.Data.PropertyChangedEventArgs(propertyName, oldValue, newValue));

        #region Classes
        [InterfaceDataTemplateSelector.Ignore]
        private class ButtonModel : ExtendedButtonViewModel<IExtendedButtonModel<string, object>, string, object>, DotNetFix.IDisposable
        {
            private Func<string> _contentFunc;

            public bool IsDisposed => _contentFunc == null;

            public ButtonModel(in Func<string> contentFunc, in Bitmap icon) : base(new ExtendedButtonModel<string, object>())
            {
                _contentFunc = contentFunc;

                ModelGeneric.Content = GetContent();

                ModelGeneric.ContentDecoration = icon;
            }

            private string GetContent() => _contentFunc() ?? "New item";

            public void Update() => Content = GetContent();

            public void Dispose()
            {
                if (IsDisposed)

                    return;

                Command = null;

                _contentFunc = null;
            }

            ~ButtonModel() => Dispose();
        }
        #endregion

        #region Fields
        private System.Collections.Generic.IReadOnlyList<ButtonModel> _commonCommands;
        private IBrowsableObjectInfoFactory _factory;
        private HistoryObservableCollection<IBrowsableObjectInfo> _historyObservable;
        private bool _isCheckBoxVisible;
        private bool _isSelected;
        private const int _newItemCommandIndex = 0;
        private IBrowsableObjectInfoViewModel _path;
        private SelectionMode _selectionMode = SelectionMode.Extended;
        private IList _selectedItems;
        private string _text;
        private System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> _treeViewItems;
        #endregion

        #region Properties
        public System.Collections.Generic.IEnumerable<IMenuItemModel<string>> BrowsabilityPaths => Path.SelectedItem?.BrowsabilityPaths?.Select(path => new MenuItemModel<string>(path.Name, new DelegateCommand(obj => path.IsValid(), obj => Path = new BrowsableObjectInfoViewModel(path.GetPath()) { SortComparison = Path.SortComparison, Factory = Path.Factory })));

        public System.Collections.Generic.IEnumerable<IButtonModel> CommonCommands => GetValueIfNotDisposed(_commonCommands);

        public System.Collections.Generic.IEnumerable<IMenuItemModel<string, IMenuItemModel<string>, object>> CustomProcesses => Path.CustomProcesses?.GroupBy(item => item.GroupName, (groupName, processes) => new MenuItemModel<string, IMenuItemModel<string>, object>(null, null)
        {
            Header = groupName,

            Items = processes.Select(process => new MenuItemModel<string>(process.Name, new DelegateCommand(obj => process.CanRun(obj, Path.Model, SelectedItems?.Select(_obj => ((IBrowsableObjectInfoViewModel)_obj).Model)), _obj =>
            {
                WinCopies.IO.Process.IProcessParameters processParameters = SelectedItems == null || SelectedItems.Count == 0 ? process.GetProcessParameters(_obj, Path.Model.Parent, Path.Model) : process.GetProcessParameters(_obj, Path.Model, SelectedItems.Select(path => ((IBrowsableObjectInfoViewModel)path).Model));

                if (processParameters != null)

                    CustomProcessParametersGeneratedEventHandler?.Invoke(this, new CustomProcessParametersGeneratedEventArgs(processParameters));
            })))
        });

        public IBrowsableObjectInfoFactory Factory { get => GetValueIfNotDisposed(_factory); set => UpdateValueIfNotDisposed(ref _factory, value ?? throw GetArgumentNullException(nameof(value)), nameof(Factory)); }

        public static DelegateCommand<ExplorerControlBrowsableObjectInfoViewModel> GoCommand { get; } = new DelegateCommand<ExplorerControlBrowsableObjectInfoViewModel>(browsableObjectInfo => browsableObjectInfo != null && browsableObjectInfo.OnGoCommandCanExecute(), browsableObjectInfo => browsableObjectInfo.OnGoCommandExecuted());

        public HistoryObservableCollection<IBrowsableObjectInfo> History => GetValueIfNotDisposed(_historyObservable);

        public bool IsCheckBoxVisible { get => _isCheckBoxVisible; set { if (value && _selectionMode == SelectionMode.Single) throw new ArgumentException("Cannot apply the true value for the IsCheckBoxVisible when SelectionMode is set to Single.", nameof(value)); UpdateValue(ref _isCheckBoxVisible, value, nameof(IsCheckBoxVisible)); } }

        public bool IsDisposed { get; private set; }

        public bool IsSelected { get => _isSelected; set { UpdateValue(ref _isSelected, value, nameof(IsSelected)); } }

        public DelegateCommand<IBrowsableObjectInfoViewModel> ItemClickCommand { get; }

        public IButtonModel NewItemCommand => GetValueIfNotDisposed(_commonCommands)[_newItemCommandIndex];

        public IBrowsableObjectInfoViewModel Path { get => GetValueIfNotDisposed(_path); set { ThrowIfNull(value, nameof(value)); IBrowsableObjectInfoViewModel oldPath = _path; UpdateValueIfNotDisposed(ref _path, value, nameof(Path)); OnPathChanged(oldPath); } }

        public IList SelectedItems { get => GetValueIfNotDisposed(_selectedItems); set => UpdateValueIfNotDisposed(ref _selectedItems, value, nameof(SelectedItems)); }

        public SelectionMode SelectionMode { get => _selectionMode; set { UpdateValue(ref _selectionMode, value, nameof(SelectionMode)); } }

        public string Text { get => GetValueIfNotDisposed(_text); set => UpdateValueIfNotDisposed(ref _text, value, nameof(Text)); }

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> TreeViewItems { get => GetValueIfNotDisposed(_treeViewItems); set => UpdateValueIfNotDisposed(ref _treeViewItems, value, nameof(TreeViewItems)); }
        #endregion

        public event System.EventHandler<CustomProcessParametersGeneratedEventArgs> CustomProcessParametersGeneratedEventHandler;

        protected ExplorerControlBrowsableObjectInfoViewModel(in IBrowsableObjectInfoViewModel path, in System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, in IBrowsableObjectInfoFactory factory)
        {
            _path = path;

            _commonCommands = new ButtonModel[] { new ButtonModel(() => Path.ProcessFactory.NewItemProcessCommands?.Name, Icons.Properties.Resources.folder_add)
            {
                ToolTip = NewItemCommandToolTip,

                Command = new  DelegateCommand<object>(obj => !IsDisposed && Path.ProcessFactory.NewItemProcessCommands?.CanCreateNewItem() == true, obj =>
                {
                    if (InputBox.ShowDialog(Path.ProcessFactory.NewItemProcessCommands.Name, DialogButton.OKCancel, Path.ProcessFactory.NewItemProcessCommands.Caption, null, out string result) == true)

                        if (!Path.ProcessFactory.NewItemProcessCommands.TryCreateNewItem(result, out _))

                            _ = MessageBox.Show(ItemCouldNotBeCreated, ItemCreationError, MessageBoxButton.OK, MessageBoxImage.Error);
                })
            }};

            ((INotifyPropertyChanged)(_historyObservable = new HistoryObservableCollection<IBrowsableObjectInfo>())).PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == nameof(ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo>.Current))

                    OnHistoryCurrentChanged(e);
            };

            path.PropertyChanged += Path_PropertyChanged;

            _historyObservable.Add(path.Model);

            _treeViewItems = treeViewItems;

            ItemClickCommand = new DelegateCommand<IBrowsableObjectInfoViewModel>(browsableObjectInfo => true, browsableObjectInfo =>
            {
                if (browsableObjectInfo.InnerObject is ShellObject && browsableObjectInfo.ObjectProperties is IFileSystemObjectInfoProperties properties && properties.FileType == FileType.File)

                    _ = System.Diagnostics.Process.Start(new ProcessStartInfo(browsableObjectInfo.Path) { UseShellExecute = true });

                else

                    Path = browsableObjectInfo.RootParentIsRootNode ? new BrowsableObjectInfoViewModel(browsableObjectInfo.Model) { SortComparison = browsableObjectInfo.SortComparison, Factory = browsableObjectInfo.Factory } : browsableObjectInfo;
            });

            _factory = factory;
        }

        #region Methods
        protected virtual void OnPathPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IBrowsableObjectInfoViewModel.SelectedItem))

                OnPropertyChanged(nameof(BrowsabilityPaths)
#if !WinCopies4
                    , null, BrowsabilityPaths
#endif
                    );
        }

        private void Path_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => OnPathPropertyChanged(e);

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IDisposableEnumerable<IBrowsableObjectInfoViewModel> treeViewItems) => new ExplorerControlBrowsableObjectInfoViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, new BrowsableObjectInfoFactory(path.ClientVersion) { SortComparison = path.SortComparison });

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IDisposableEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, IBrowsableObjectInfoFactory factory) => new ExplorerControlBrowsableObjectInfoViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, factory ?? throw GetArgumentNullException(nameof(factory)));

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IBrowsableObjectInfoFactory factory) => new ExplorerControlBrowsableObjectInfoViewModel(path, path.GetRootItems().Select<IBrowsableObjectInfo, IBrowsableObjectInfoViewModel>(factory.GetBrowsableObjectInfoViewModel), factory);

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path) => From(path, new BrowsableObjectInfoFactory(path.ClientVersion) { SortComparison = path.SortComparison });

        protected void UpdateValueIfNotDisposed<T>(ref T value, in T newValue, in string propertyName)
        {
            if (IsDisposed)

                throw GetExceptionForDispose(false);

            base.UpdateValue(ref value, newValue, propertyName);
        }

        protected T GetValueIfNotDisposed<T>(in T value) => IsDisposed ? throw GetExceptionForDispose(false) : value;

        protected virtual void OnPathRemoved(IBrowsableObjectInfoViewModel path) => path.PropertyChanged -= Path_PropertyChanged;

        protected virtual void OnPathChanged(IBrowsableObjectInfoViewModel oldPath)
        {
            OnPathRemoved(oldPath);

            Path.PropertyChanged += Path_PropertyChanged;

            Text = _path.Path;

            foreach (ButtonModel command in _commonCommands)

                command.Update();

            OnPropertyChanged(nameof(CustomProcesses)
#if !WinCopies4
                , null, CustomProcesses
#endif
                );

            if (_path.Path == History.Current.Path)

                return;

            if (_historyObservable.Count == 1)
            {
                _historyObservable.Insert(0, _path.Model);

                return;
            }

            else if (_historyObservable.Count >= 2)
            {
                _historyObservable.NotifyOnPropertyChanged = false;

                if (_historyObservable.CanMovePreviousFromCurrent && _path.Path == _historyObservable[_historyObservable.CurrentIndex + 1].Path)

                    _historyObservable.CurrentIndex++;

                else if (_historyObservable.CanMoveNextFromCurrent && _path.Path == _historyObservable[_historyObservable.CurrentIndex - 1].Path)

                    _historyObservable.CurrentIndex--;

                else
                {
                    if (_historyObservable.CurrentIndex > 0)

                        for (int i = 0; i < _historyObservable.CurrentIndex; i++)

                            _historyObservable.RemoveAt(0);

                    _historyObservable.Insert(0, _path.Model);

                    _historyObservable.CurrentIndex = 0;
                }

                _historyObservable.NotifyOnPropertyChanged = true;
            }
        }

        protected virtual void OnHistoryCurrentChanged(System.ComponentModel.PropertyChangedEventArgs e) => Path = new BrowsableObjectInfoViewModel(_historyObservable.Current) { SortComparison = Path.SortComparison, Factory = Path.Factory };

        protected virtual bool OnGoCommandCanExecute() => true;

        protected virtual void OnGoCommandExecuted() => Path = _factory.GetBrowsableObjectInfoViewModel(Text);

        protected virtual void Dispose(bool disposing)
        {
            foreach (ButtonModel command in _commonCommands)

                command.Dispose();

            _commonCommands = null;

            _text = null;

            _treeViewItems = null;

            OnPathRemoved(_path);

            _path = null;

            _historyObservable.NotifyOnPropertyChanged = false;
            _historyObservable.Clear();
            _historyObservable = null;

            _factory = null;

            if (_selectedItems != null)
            {
                _selectedItems.Clear();
                _selectedItems = null;
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            if (IsDisposed)

                return;

            Dispose(true);

            GC.SuppressFinalize(this);
        }
        #endregion

        ~ExplorerControlBrowsableObjectInfoViewModel() => Dispose(false);

        //private ViewStyle _viewStyle = ViewStyle.SizeThree;

        //public ViewStyle ViewStyle { get => _viewStyle; set { _viewStyle = value; OnPropertyChanged(nameof(ViewStyle)); } }
    }
}
