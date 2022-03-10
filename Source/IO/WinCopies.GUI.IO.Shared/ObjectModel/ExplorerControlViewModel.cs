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

using Microsoft.WindowsAPICodePack.Shell;

#region System
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
#endregion

#region WinCopies
using WinCopies.Collections;
using WinCopies.Collections.Generic;
using WinCopies.Commands;
using WinCopies.GUI.Controls;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Controls.ViewModels;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.Windows;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.Temp;
using WinCopies.Util.Data;
#endregion

#region Static Usings
using static WinCopies.GUI.IO.Properties.Resources;
using static WinCopies.ThrowHelper;
using static WinCopies.GUI.Icons.Properties.Resources;
using static WinCopies.GUI.IO.ObjectModel.ExplorerControlViewModel.CommonCommandsUtilities;
#endregion

using static WinCopies.Temp.Temp;

namespace WinCopies.GUI.IO
{
    public struct ExplorerControlViewModelStruct
    {
        public IBrowsableObjectInfoViewModel Path;
        public DotNetFix.IDisposable Callback;

        public ExplorerControlViewModelStruct(in IBrowsableObjectInfoViewModel path)
        {
            Path = path;

            Callback = null;
        }
    }

    namespace ObjectModel
    {
        public abstract class ExplorerControlViewModel : ViewModelBase, IExplorerControlViewModel
        {
            //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue) => OnPropertyChanged(new WinCopies.Util.Data.PropertyChangedEventArgs(propertyName, oldValue, newValue));

            #region Types
            public static class CommonCommandsUtilities
            {
                public enum CommonCommandsIndex
                {
                    NewItem = 0,

                    Rename = 1
                }

                public static IProcessCommand GetNewItemCommand(IProcessFactory factory) => factory.NewItemProcessCommand;

                public static IProcessCommand GetRenameItemCommand(IProcessFactory factory) => factory.RenameItemProcessCommand;
            }

            [InterfaceDataTemplateSelector.Ignore]
            private class ButtonModel : ExtendedButtonViewModel<IExtendedButtonModel<string, object>, string, object>, DotNetFix.IDisposable
            {
                private IExplorerControlViewModel _explorerControlViewModel;
                private Func<IProcessFactory, IProcessCommand> _contentFunc;
                private string _defaultName;

                public bool IsDisposed => _contentFunc == null;

                public ButtonModel(in IExplorerControlViewModel explorerControlViewModel, in Func<IProcessFactory, IProcessCommand> contentFunc, in string defaultName, in Bitmap icon) : base(new ExtendedButtonModel<string, object>())
                {
                    _explorerControlViewModel = explorerControlViewModel;

                    _contentFunc = contentFunc;

                    _defaultName = defaultName;

                    ModelGeneric.Content = GetContent();

                    ModelGeneric.ContentDecoration = icon;
                }

                private string GetContent() => IsDisposed ? throw GetExceptionForDispose(false) : _contentFunc(_explorerControlViewModel.Path.ProcessFactory)?.Name ?? _defaultName;

                public void Update() => Content = GetContent();

                public void Dispose()
                {
                    if (IsDisposed)

                        return;

                    _explorerControlViewModel = null;
                    Command = null;
                    _contentFunc = null;
                    _defaultName = null;

                    GC.SuppressFinalize(this);
                }

                ~ButtonModel() => Dispose();
            }

            private class BackgroundWorker
            {
                private WinCopies.BackgroundWorker _backgroundWorker = new
#if !CS9
                WinCopies.BackgroundWorker
#endif
            ();
                private IBrowsableObjectInfo _updateWith;
                private IBrowsableObjectInfo _workOn;

                public BackgroundWorker()
                {
                    _backgroundWorker.WorkerSupportsCancellation = _backgroundWorker.WorkerReportsProgress = true;

                    _backgroundWorker.DoWork += (object sender, DoWorkEventArgs e) => OnDoWork();

                    _backgroundWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => OnProgressChanged((IBitmapSourcesLinker)e.UserState);
                }

                private void OnDoWork()
                {
                    System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> enumerable = _workOn.GetItems();

                    if (enumerable == null)

                        return;

                    ulong? count;
                    ulong i;

                    void initCount()
                    {
                        count = enumerable is ICollection collection ? (ulong
#if !CS9
                    ?
#endif
                    )collection.Count : enumerable is ICountable countable ? (ulong)countable.Count : enumerable is IUIntCountable uintCountable ? uintCountable.Count : enumerable is ILongCountable longCountable ? (ulong)longCountable.Count : enumerable is IULongCountable ulongCountable ? ulongCountable.Count :
#if !CS9
                    (ulong?)
#endif
                    null;

                        i = 0ul;
                    }

                    initCount();

                    bool doWork()
                    {
                        bool _doWork()
                        {
                            void load(in IBrowsableObjectInfo item)
                            {
                                if (item.BitmapSources is IBitmapSourcesLinker bitmapSourcesLinker)

                                    bitmapSourcesLinker.Load();
                            }

                            load(_workOn);

                            foreach (IBrowsableObjectInfo item in enumerable)

                                if (_updateWith == _workOn)
                                {
                                    try
                                    {
                                        load(item);
                                    }

                                    catch { /* Left empty. */ }

                                    _backgroundWorker.ReportProgress(count.HasValue ? (int)((++i) / count.Value * 100) : 0, item.BitmapSources);
                                }

                                else
                                {
                                    enumerable = (_workOn = _updateWith).GetItems();

                                    initCount();

                                    _backgroundWorker.ReportProgress(0);

                                    return true;
                                }

                            i = 0ul;

                            return false;
                        }

                        while (_doWork()) { /* Left empty. */ }

                        return _doWork();
                    }

                    while (doWork()) { /* Left empty. */ }
                }

                private static void OnProgressChanged(IBitmapSourcesLinker bitmapSourcesLinker)
                {
                    if (bitmapSourcesLinker == null)

                        return;

                    _ = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(bitmapSourcesLinker.OnBitmapSourcesLoaded));
                }

                public void UpdatePath(in IBrowsableObjectInfo browsableObjectInfo)
                {
                    _updateWith = browsableObjectInfo;

                    if (!_backgroundWorker.IsBusy)
                    {
                        _workOn = _updateWith;

                        _backgroundWorker.RunWorkerAsync();
                    }
                }
            }
            #endregion

            #region Constants
            public const string NewItem = "New item";
            public const string RenameItem = "Rename item";
            #endregion

            #region Fields
            private ExplorerControlViewModelStruct _path;
            private bool _autoStartMonitoring = true;
            private System.Collections.Generic.IReadOnlyList<ButtonModel> _commonCommands;
            private IBrowsableObjectInfoFactory _factory;
            private Predicate<IBrowsableObjectInfo> _oldPredicate;
            private Predicate<IBrowsableObjectInfo> _predicate;
            private HistoryObservableCollection<IBrowsableObjectInfo> _historyObservable;
            private bool _isCheckBoxVisible;
            private bool _isSelected;
            private SelectionMode _selectionMode = SelectionMode.Extended;
            private ObservableCollection<IBrowsableObjectInfoViewModel> _selectedItems = new ObservableCollection<IBrowsableObjectInfoViewModel>();
            private string _text;
            private System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> _treeViewItems;
            private readonly BackgroundWorker _backgroundWorker = new
#if !CS9
            BackgroundWorker
#endif
            ();
            #endregion

            #region Properties
            public bool AutoStartMonitoring { get => GetValueIfNotDisposed(_autoStartMonitoring); set => UpdateValueIfNotDisposed(ref _autoStartMonitoring, value, nameof(AutoStartMonitoring)); }

            protected Converter<string, IBrowsableObjectInfo> GetBrowsableObjectInfoViewModelConverter { get; }

            public ICommand BrowseToParent { get; }

            public System.Collections.Generic.IEnumerable<IMenuItemModel<string>> BrowsabilityPaths => Path.SelectedItem?.BrowsabilityPaths?.Select(path => new MenuItemModel<string>(path.Name, new DelegateCommand(obj => path.IsValid(), obj => Path = new BrowsableObjectInfoViewModel(path.GetPath()) { SortComparison = Path.SortComparison, Factory = Path.Factory })));

            public System.Collections.Generic.IEnumerable<IButtonModel> CommonCommands => GetValueIfNotDisposed(_commonCommands);

            public System.Collections.Generic.IEnumerable<IMenuItemModel<string, IMenuItemModel<string>, object>> CustomProcesses => Path.CustomProcesses?.GroupBy(item => item.GroupName, (groupName, processes) => new MenuItemModel<string, IMenuItemModel<string>, object>(null, null)
            {
                Header = groupName,

                Items = processes.Select(process => new MenuItemModel<string>(process.Name, new DelegateCommand(obj => !IsDisposed && process.CanRun(obj, Path.Model, SelectedItemsInnerObjects), _obj =>
               {
                   IProcessParameters processParameters;

                   void tryGetProcessParameters(in IBrowsableObjectInfo sourcePath, in System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> enumerable) => processParameters = process.TryGetProcessParameters(_obj, sourcePath, enumerable);

                   if (_selectedItems.Count == 0)

                       tryGetProcessParameters(Path.Model.Parent, Path.Model);

                   else

                       tryGetProcessParameters(Path.Model, SelectedItemsInnerObjects);

                   if (processParameters != null)

                       CustomProcessParametersGenerated?.Invoke(this, new CustomProcessParametersGeneratedEventArgs(process, processParameters));
               })))
            });

            public IBrowsableObjectInfoFactory Factory { get => GetValueIfNotDisposed(_factory); set => UpdateValueIfNotDisposed(ref _factory, value ?? throw GetArgumentNullException(nameof(value)), nameof(Factory)); }

            public Predicate<IBrowsableObjectInfo> Filter
            {
                get => GetValueIfNotDisposed(_predicate);

                set
                {
                    ThrowIfDisposed();

                    if (UtilHelpers.UpdateValue(ref _predicate, value))
                    {
                        _path.Path.Filter = value;

                        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(Filter)));
                    }
                }
            }

            public static DelegateCommand<ExplorerControlViewModel> GoCommand { get; } = new DelegateCommand<ExplorerControlViewModel>(browsableObjectInfo => browsableObjectInfo != null && browsableObjectInfo.OnGoCommandCanExecute(), browsableObjectInfo => browsableObjectInfo.OnGoCommandExecuted());

            public HistoryObservableCollection<IBrowsableObjectInfo> History => GetValueIfNotDisposed(_historyObservable);

            public bool IsCheckBoxVisible
            {
                get => GetValueIfNotDisposed(_isCheckBoxVisible);

                set
                {
                    ThrowIfDisposed();

                    if (value && _selectionMode == SelectionMode.Single)

                        throw new ArgumentException("Cannot set IsCheckBoxVisible to true when SelectionMode is set to Single.", nameof(value));

                    _ = UpdateValue(ref _isCheckBoxVisible, value, nameof(IsCheckBoxVisible));
                }
            }

            public bool IsDisposed { get; private set; }

            public bool IsSelected { get => GetValueIfNotDisposed(_isSelected); set => UpdateValueIfNotDisposed(ref _isSelected, value, nameof(IsSelected)); }

            public DelegateCommand<IBrowsableObjectInfoViewModel> ItemClickCommand { get; }

            public IButtonModel NewItemCommand => GetCommonCommand(CommonCommandsIndex.NewItem);

            public IButtonModel RenameItemCommand => GetCommonCommand(CommonCommandsIndex.Rename);

            public IBrowsableObjectInfoViewModel Path
            {
                get => GetValueIfNotDisposed(_path).Path;

                set
                {
                    ThrowIfDisposed();

                    ThrowIfNull(value, nameof(value));

                    IBrowsableObjectInfoViewModel oldPath = _path.Path;

                    UpdateValueIfNotDisposed(ref _path.Path, value, nameof(Path));

                    OnPathChanged(oldPath);
                }
            }

            public ReadOnlyObservableCollection<IBrowsableObjectInfoViewModel> SelectedItems { get; private set; }

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> SelectedItemsInnerObjects => SelectedItems?.Select(obj => obj.Model);

            public SelectionMode SelectionMode { get => GetValueIfNotDisposed(_selectionMode); set => UpdateValueIfNotDisposed(ref _selectionMode, value, nameof(SelectionMode)); }

            public string Text { get => GetValueIfNotDisposed(_text); set => UpdateValueIfNotDisposed(ref _text, value, nameof(Text)); }

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> TreeViewItems { get => GetValueIfNotDisposed(_treeViewItems); set => UpdateValueIfNotDisposed(ref _treeViewItems, value, nameof(TreeViewItems)); }
            #endregion

            public event System.EventHandler<CustomProcessParametersGeneratedEventArgs> CustomProcessParametersGenerated;

            protected ExplorerControlViewModel(in IBrowsableObjectInfoViewModel path, in System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, in IBrowsableObjectInfoFactory factory, in Converter<string, IBrowsableObjectInfo> converter)
            {
                ThrowIfNull(path, nameof(path));
                ThrowIfNull(converter, nameof(converter));

                _path = new ExplorerControlViewModelStruct(path);

                SelectedItems = new ReadOnlyObservableCollection<IBrowsableObjectInfoViewModel>(_selectedItems);

                GetBrowsableObjectInfoViewModelConverter = converter;

                BrowseToParent = new DelegateCommand(o => !IsDisposed && Path.Parent != null, o => Path = new BrowsableObjectInfoViewModel(Path.Parent));

                ButtonModel getNewButtonModel(in Func<IProcessFactory, IProcessCommand> func, in string defaultName, in Bitmap icon, in object toolTip, in ICommand<object> command) => new ButtonModel(this, func, defaultName, icon)
                {
                    ToolTip = toolTip,

                    Command = command
                };

                _commonCommands = new ButtonModel[] {
                    getNewButtonModel(processFactory => processFactory.NewItemProcessCommand, NewItem, folder_add, NewItemCommandToolTip, new DelegateCommand<object>(OnCanCreateNewItem, OnCreateNewItem)),
                    getNewButtonModel(processFactory => processFactory.RenameItemProcessCommand, RenameItem, Path?.IsLocalRoot == true ? drive_rename : textfield_rename, RenameItemCommandToolTip, new DelegateCommand<object>(OnCanRenameItem, OnRenameItem)) };

                ((INotifyPropertyChanged)(_historyObservable = new HistoryObservableCollection<IBrowsableObjectInfo>())).PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == nameof(ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo>.Current))

                        OnHistoryCurrentChanged(e);
                };

                _historyObservable.Add(path.Model);

                _treeViewItems = treeViewItems;

                ItemClickCommand = new DelegateCommand<IBrowsableObjectInfoViewModel>(browsableObjectInfo => true, OnItemClick);

                _factory = factory;

                OnPathAdded(path);
            }

            #region Methods
            #region Common commands
            public IButtonModel GetCommonCommand(CommonCommandsIndex index) => GetValueIfNotDisposed(_commonCommands)[(int)index];

            public IProcessFactory GetProcessFactory() => Path.ProcessFactory;

            protected virtual bool OnCanExecuteCommand(object obj, Converter<IProcessFactory, IProcessCommand> command) => command == null ? throw GetArgumentNullException(nameof(command)) : !IsDisposed && command(GetProcessFactory())?.CanExecute(SelectedItemsInnerObjects) == true;

            protected virtual bool OnCanCreateNewItem(object obj) => OnCanExecuteCommand(obj, GetNewItemCommand);

            protected virtual bool OnCanRenameItem(object obj) => OnCanExecuteCommand(obj, GetRenameItemCommand);

            protected virtual void OnTryExecuteCommand(object obj, Converter<IProcessFactory, IProcessCommand> command, string errorMessage, string errorCaption)
            {
                ThrowIfNull(command, nameof(command));

                IProcessCommand _command = command(GetProcessFactory());

                if (InputBox.ShowDialog(_command.Name, DialogButton.OKCancel, _command.Caption, null, out string result) == true)

                    if (!_command.TryExecute(result, SelectedItemsInnerObjects, out _))

                        _ = MessageBox.Show(errorMessage, errorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            protected virtual void OnCreateNewItem(object obj) => OnTryExecuteCommand(obj, GetNewItemCommand, ItemCouldNotBeCreated, ItemCreationError);

            protected virtual void OnRenameItem(object obj) => OnTryExecuteCommand(obj, GetRenameItemCommand, ItemCouldNotBeRenamed, ItemRenameError);
            #endregion

            public static IBrowsableObjectInfoViewModel GetBrowsableObjectInfoOrLaunchItem(IBrowsableObjectInfoViewModel browsableObjectInfo)
            {
                if (browsableObjectInfo.InnerObject is ShellObject && browsableObjectInfo.ObjectProperties is IFileSystemObjectInfoProperties properties && properties.FileType == FileType.File)
                {
                    _ = System.Diagnostics.Process.Start(new ProcessStartInfo(browsableObjectInfo.Path) { UseShellExecute = true });

                    return null;
                }

                else

                    return browsableObjectInfo.RootParentIsRootNode ? new BrowsableObjectInfoViewModel(browsableObjectInfo.Model) { SortComparison = browsableObjectInfo.SortComparison, Factory = browsableObjectInfo.Factory } : browsableObjectInfo;
            }

            public void OnItemClick(IBrowsableObjectInfoViewModel browsableObjectInfo)
            {
                IBrowsableObjectInfoViewModel item = GetBrowsableObjectInfoOrLaunchItem(browsableObjectInfo);

                if (item != null)

                    Path = item;
            }

            protected void ThrowIfDisposed() => ThrowHelper.ThrowIfDisposed(this);

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

            protected void UpdateValueIfNotDisposed<T>(ref T value, in T newValue, in string propertyName) => _ = IsDisposed ? throw GetExceptionForDispose(false) : base.UpdateValue(ref value, newValue, propertyName);

            protected T GetValueIfNotDisposed<T>(in T value) => IsDisposed ? throw GetExceptionForDispose(false) : value;

            private void LoadPath()
            {
                _path.Path.LoadItems();

                _backgroundWorker.UpdatePath(_path.Path);
            }

            protected void OnRegisterCallback(BrowsableObjectInfoCallbackArgs parameters)
            {
                IBrowsableObjectInfoViewModel path = _path.Path;

                System.Collections.ObjectModel.ObservableCollection<IBrowsableObjectInfoViewModel> items = path.Items;

                switch (parameters.CallbackReason)
                {
                    case BrowsableObjectInfoCallbackReason.Added:

                        items.Add(new BrowsableObjectInfoViewModel(parameters.BrowsableObjectInfo));

                        path.UpdateItems();

                        break;

                    case BrowsableObjectInfoCallbackReason.Updated:

                        for (int i = 0; i < items.Count; i++)

                            if (items[i].Path == parameters.Path)
                            {
                                items[i] = new BrowsableObjectInfoViewModel(parameters.BrowsableObjectInfo);

                                break;
                            }

                        path.UpdateItems();

                        break;

                    case BrowsableObjectInfoCallbackReason.Removed:

                        for (int i = 0; i < items.Count; i++)

                            if (items[i].Path == parameters.Path)
                            {
                                items.RemoveAt(i);

                                break;
                            }

                        break;
                }
            }

            public void StartMonitoring()
            {
                IBrowsableObjectInfoViewModel path = _path.Path;

                _path.Callback = path.RegisterCallback(OnRegisterCallback);

                path.StartMonitoring();
            }

            protected virtual void OnPathAdded(in IBrowsableObjectInfoViewModel path)
            {
                path.PropertyChanged += Path_PropertyChanged;
                path.SelectedItemsChanged += Path_SelectedItemsChanged;

                _oldPredicate = path.Filter;

                path.Filter = _predicate;

                if (_autoStartMonitoring)

                    StartMonitoring();

                LoadPath();
            }

            protected virtual void OnPathSelectedItemsChanged(ItemsChangedEventArgs<IBrowsableObjectInfoViewModel> e)
            {
                RunActionIfNotNull(e.OldItems, item => _selectedItems.Remove(item));

                RunActionIfNotNull(e.NewItems, item => _selectedItems.Add(item));
            }

            private void Path_SelectedItemsChanged(object sender, ItemsChangedEventArgs<IBrowsableObjectInfoViewModel> e) => OnPathSelectedItemsChanged(e);

            private void StopMonitoring(in IBrowsableObjectInfoViewModel path)
            {
                path.StopMonitoring();

                if (_path.Callback != null)
                {
                    _path.Callback.Dispose();
                    _path.Callback = null;
                }
            }

            public void StopMonitoring() => StopMonitoring(_path.Path);

            protected virtual void OnPathRemoved(IBrowsableObjectInfoViewModel path)
            {
                if (_autoStartMonitoring)

                    StopMonitoring(path);

                path.SelectedItemsChanged -= Path_SelectedItemsChanged;
                path.PropertyChanged -= Path_PropertyChanged;

                path.Filter = _oldPredicate;
            }

            protected virtual void OnUpdateText() => Text = _path.Path.Path;

            protected virtual void OnUpdateCommands()
            {
                foreach (ButtonModel command in _commonCommands)

                    command.Update();

                OnPropertyChanged(nameof(CustomProcesses)
#if !WinCopies4
                , null, CustomProcesses
#endif
                );
            }

            protected virtual void OnUpdateHistory()
            {
                if (_path.Path.Path == History.Current.Path)

                    return;

                if (_historyObservable.Count == 1)
                {
                    _historyObservable.Insert(0, _path.Path.Model);

                    return;
                }

                else if (_historyObservable.Count >= 2)
                {
                    _historyObservable.NotifyOnPropertyChanged = false;

                    if (_historyObservable.CanMovePreviousFromCurrent && _path.Path.Path == _historyObservable[_historyObservable.CurrentIndex + 1].Path)

                        _historyObservable.CurrentIndex++;

                    else if (_historyObservable.CanMoveNextFromCurrent && _path.Path.Path == _historyObservable[_historyObservable.CurrentIndex - 1].Path)

                        _historyObservable.CurrentIndex--;

                    else
                    {
                        if (_historyObservable.CurrentIndex > 0)

                            for (int i = 0; i < _historyObservable.CurrentIndex; i++)

                                _historyObservable.RemoveAt(0);

                        _historyObservable.Insert(0, _path.Path.Model);

                        _historyObservable.CurrentIndex = 0;
                    }

                    _historyObservable.NotifyOnPropertyChanged = true;
                }
            }

            protected virtual void OnPathChanged(IBrowsableObjectInfoViewModel oldPath)
            {
                OnPathRemoved(oldPath);

                OnPathAdded(_path.Path);

                OnUpdateText();

                OnUpdateCommands();

                OnUpdateHistory();
            }

            protected virtual void OnHistoryCurrentChanged(System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (Path.Path != _historyObservable.Current.Path)

                    Path = new BrowsableObjectInfoViewModel(_historyObservable.Current) { SortComparison = Path.SortComparison, Factory = Path.Factory };
            }

            protected virtual bool OnGoCommandCanExecute() => true;

            protected void OnGoCommandExecuted() => Path = _factory.GetBrowsableObjectInfoViewModel(GetBrowsableObjectInfoViewModelConverter(Text));

            protected virtual void Dispose(bool disposing)
            {
                foreach (ButtonModel command in _commonCommands)

                    command.Dispose();

                _commonCommands = null;

                _text = null;

                _treeViewItems = null;

                OnPathRemoved(_path.Path);

                _path.Path = null;

                _historyObservable.NotifyOnPropertyChanged = false;

                if (disposing)

                    _historyObservable.Clear();

                _historyObservable = null;

                _factory = null;

                if (_selectedItems != null)
                {
                    _selectedItems.Clear();

                    _selectedItems = null;
                    SelectedItems = null;
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

            ~ExplorerControlViewModel() => Dispose(false);

            //private ViewStyle _viewStyle = ViewStyle.SizeThree;

            //public ViewStyle ViewStyle { get => _viewStyle; set { _viewStyle = value; OnPropertyChanged(nameof(ViewStyle)); } }
        }
    }
}