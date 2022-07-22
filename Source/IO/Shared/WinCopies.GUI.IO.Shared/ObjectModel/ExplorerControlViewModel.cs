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

#region Usings
using Microsoft.WindowsAPICodePack.Shell;

#region System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
#endregion

#region WinCopies
using WinCopies.Collections;
using WinCopies.Collections.Generic;
using WinCopies.Commands;
using WinCopies.Desktop;
using WinCopies.GUI.Controls;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Controls.ViewModels;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.Windows;
using WinCopies.IO;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.Util;
using WinCopies.Util.Data;
#endregion

#region Static Usings
using static WinCopies.Extensions.UtilHelpers;
using static WinCopies.GUI.Icons.Properties.Resources;
using static WinCopies.GUI.IO.ObjectModel.ExplorerControlViewModel.CommonCommandsUtilities;
using static WinCopies.GUI.IO.Properties.Resources;
using static WinCopies.ThrowHelper;
#endregion

using Application = System.Windows.Application;
#endregion Usings

namespace WinCopies.GUI.IO
{
    public struct ExplorerControlViewModelStruct
    {
        public IBrowsableObjectInfoViewModel Path;
        public DotNetFix.IDisposable
#if CS8
            ?
#endif
            Callback;

        public ExplorerControlViewModelStruct(in IBrowsableObjectInfoViewModel path)
        {
            Path = path;

            Callback = null;
        }
    }

    namespace ObjectModel
    {
        public class ExplorerControlViewModel : ViewModelBase, IExplorerControlViewModel
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
                private Func<IProcessFactory
#if CS8
                    ?
#endif
                    , IProcessCommand> _contentFunc;
                private string _defaultName;

                public bool IsDisposed => _contentFunc == null;

                public ButtonModel(in IExplorerControlViewModel explorerControlViewModel, in Func<IProcessFactory
#if CS8
                    ?
#endif
                    , IProcessCommand> contentFunc, in string defaultName, in Bitmap icon) : base(new ExtendedButtonModel<string, object>())
                {
                    _explorerControlViewModel = explorerControlViewModel;
                    _contentFunc = contentFunc;
                    _defaultName = defaultName;
                    ModelGeneric.Content = GetContent();
                    ModelGeneric.ContentDecoration = icon;
                }

                private string GetContent() => IsDisposed ? throw GetExceptionForDispose(false) : _contentFunc(_explorerControlViewModel.Path.ItemSources?.SelectedItem?.ProcessSettings?.ProcessFactory)?.Name ?? _defaultName;

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
                private readonly WinCopies.BackgroundWorker _backgroundWorker = new
#if !CS9
                WinCopies.BackgroundWorker
#endif
                ();
                private IBrowsableObjectInfoViewModel _updateWith;
                private IBrowsableObjectInfoViewModel _workOn;

                public BackgroundWorker()
                {
                    _backgroundWorker.WorkerSupportsCancellation = _backgroundWorker.WorkerReportsProgress = true;
                    _backgroundWorker.DoWork += (object sender, DoWorkEventArgs e) => OnDoWork();
                    _backgroundWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => OnProgressChanged((IBitmapSourcesLinker)e.UserState);
                }

                private void OnDoWork()
                {
                    System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                        ?
#endif
                        enumerable = _workOn.ItemSources?.SelectedItem?.Items;

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
                                    enumerable = (_workOn = _updateWith).ItemSources?.SelectedItem?.Items;

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

                public void UpdatePath(in IBrowsableObjectInfoViewModel browsableObjectInfo)
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
            private IBrowsableObjectInfoFactory _factory;
            private Predicate<IBrowsableObjectInfo> _oldPredicate;
            private Predicate<IBrowsableObjectInfo> _predicate;
            private HistoryObservableCollection<IBrowsableObjectInfo> _historyObservable;
            private ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo> _history;
            private bool _isCheckBoxVisible;
            private bool _isSelected;
            private SelectionMode _selectionMode = SelectionMode.Extended;
            private string _text;
            private System.Collections.Generic.IReadOnlyList<ButtonModel>
#if CS8
                ?
#endif
                _commonCommands;
            private System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel>
#if CS8
                ?
#endif
                _treeViewItems;
            private ObservableCollection<IBrowsableObjectInfoViewModel> _selectedItems = new
#if !CS9
                ObservableCollection<IBrowsableObjectInfoViewModel>
#endif
                ();
            private readonly BackgroundWorker _backgroundWorker = new
#if !CS9
                BackgroundWorker
#endif
                ();
            #endregion

            #region Properties
            public bool AutoStartMonitoring { get => GetValueIfNotDisposed(_autoStartMonitoring); set => UpdateValueIfNotDisposed(ref _autoStartMonitoring, value, nameof(AutoStartMonitoring)); }

            public ICommand BrowseToParent { get; }

            public System.Collections.Generic.IEnumerable<IMenuItemModel<string>>
#if CS8
                ?
#endif
                BrowsabilityPaths => Path.ItemSources.SelectedItem.SelectedItem?.BrowsabilityPaths?.Select(path => new MenuItemModel<string>(path.Name, new DelegateCommand(obj => path.IsValid(), obj => Path = new BrowsableObjectInfoViewModel(path.GetPath()) { SortComparison = Path.SortComparison, Factory = Path.Factory })));

            public System.Collections.Generic.IEnumerable<IButtonModel> CommonCommands => GetValueIfNotDisposed(_commonCommands);

            private System.Collections.Generic.IReadOnlyList<ButtonModel>
#if CS8
                ?
#endif
                _CommonCommands
            { set => UpdateValueIfNotDisposed(ref _commonCommands, value, nameof(CommonCommands)); }

            public System.Collections.Generic.IEnumerable<IMenuItemModel<string, IMenuItemModel<string>, object>>
#if CS8
                ?
#endif
                CustomProcesses => Path.ItemSources.SelectedItem.ProcessSettings?.CustomProcesses?.GroupBy(item => item.GroupName, (groupName, processes) => new MenuItemModel<string, IMenuItemModel<string>, object>(null, null)
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

            public ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo> History => GetValueIfNotDisposed(_history);

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

            public IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                SelectedItemsInnerObjects => SelectedItems?.Select(obj => obj.Model);

            public SelectionMode SelectionMode { get => GetValueIfNotDisposed(_selectionMode); set => UpdateValueIfNotDisposed(ref _selectionMode, value, nameof(SelectionMode)); }

            public string Text { get => GetValueIfNotDisposed(_text); set => UpdateValueIfNotDisposed(ref _text, value, nameof(Text)); }

            public IEnumerable<IBrowsableObjectInfoViewModel>
#if CS8
                ?
#endif
                TreeViewItems
            { get => GetValueIfNotDisposed(_treeViewItems); set => UpdateValueIfNotDisposed(ref _treeViewItems, value, nameof(TreeViewItems)); }

            public DelegateCommand<IBrowsableObjectInfo> OpenInNewContextCommand { get; set; }
            #endregion

            public event System.EventHandler<CustomProcessParametersGeneratedEventArgs> CustomProcessParametersGenerated;

            public ExplorerControlViewModel(in IBrowsableObjectInfoViewModel path, in IBrowsableObjectInfoFactory factory)
            {
                ThrowIfNull(path, nameof(path));
                ThrowIfNull(factory, nameof(factory));

                _path = new ExplorerControlViewModelStruct(path);

                SelectedItems = new ReadOnlyObservableCollection<IBrowsableObjectInfoViewModel>(_selectedItems);

                BrowseToParent = new DelegateCommand(o => !IsDisposed && Path.Parent != null, o =>
                {
                    IBrowsableObjectInfo
#if CS8
                    ?
#endif
                    checkHistory(in FuncOut<IBrowsableObjectInfo, bool> func)
                    {
                        IBrowsableObjectInfo _path = Path.Parent;

                        return func(out IBrowsableObjectInfo result) && _path.Path == result.Path && _path.Protocol == result.Protocol ? result : null;
                    }

                    Path = new BrowsableObjectInfoViewModel(checkHistory(_historyObservable.TryGetPrevious) ?? checkHistory(_historyObservable.TryGetNext) ?? Path.Parent);
                });

                (_historyObservable = new HistoryObservableCollection<IBrowsableObjectInfo>()).AsFromType<INotifyPropertyChanged>().PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == nameof(ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo>.Current))

                        OnHistoryCurrentChanged();
                };

                _historyObservable.Add(path.Model);
                _history = new ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo>(_historyObservable);

                ItemClickCommand = new DelegateCommand<IBrowsableObjectInfoViewModel>(browsableObjectInfo => true, OnItemClick);

                _factory = factory;

                OnPathAdded(path);
            }

            #region Methods
            #region Common Commands
            public IButtonModel GetCommonCommand(CommonCommandsIndex index) => GetValueIfNotDisposed(_commonCommands)[(int)index];

            public IProcessFactory
#if CS8
                ?
#endif
                GetProcessFactory() => Path.ItemSources?.SelectedItem?.ProcessSettings?.ProcessFactory;

            protected virtual bool OnCanExecuteCommand(object obj, Converter<IProcessFactory
#if CS8
                ?
#endif
                , IProcessCommand> command) => command == null ? throw GetArgumentNullException(nameof(command)) : !(IsDisposed || Path.ItemSources?.SelectedItem?.ProcessSettings?.ProcessFactory == null) && command(GetProcessFactory())?.CanExecute(SelectedItemsInnerObjects) == true;

            protected virtual bool OnCanCreateNewItem(object obj) => OnCanExecuteCommand(obj, GetNewItemCommand);

            protected virtual bool OnCanRenameItem(object obj) => OnCanExecuteCommand(obj, GetRenameItemCommand);

            protected virtual void OnTryExecuteCommand(Converter<IProcessFactory, IProcessCommand> command, string errorMessage, string errorCaption, in BitmapSource icon)
            {
                IProcessCommand _command = (command ?? throw GetArgumentNullException(nameof(command)))(GetProcessFactory());

                if (InputBox.ShowDialog(_command.Name, DialogButton.OKCancel, _command.Caption, SelectedItemsInnerObjects.FirstOrDefault()?.Name, null, out string
#if CS8
                    ?
#endif
                    result, icon) == true && !_command.TryExecute(result, SelectedItemsInnerObjects, out _))

                    _ = MessageBox.Show(errorMessage, errorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            protected virtual void OnCreateNewItem() => OnTryExecuteCommand(GetNewItemCommand, ItemCouldNotBeCreated, ItemCreationError, folder_add.ToImageSource());

            protected virtual void OnRenameItem(BitmapSource icon) => OnTryExecuteCommand(GetRenameItemCommand, ItemCouldNotBeRenamed, ItemRenameError, icon);
            #endregion

            public static IExplorerControlViewModel From(in IBrowsableObjectInfoViewModel path, in IBrowsableObjectInfoFactory factory) => new ExplorerControlViewModel(path, factory);

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
                if (e?.PropertyName == nameof(IBrowsableObjectInfoViewModel.ItemSources.SelectedItem))

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
                IBrowsableObjectInfoViewModel path = _path.Path;

                path.ItemSources?.SelectedItem?.LoadItems();

                _backgroundWorker.UpdatePath(path);
            }

            protected void OnRegisterCallback(BrowsableObjectInfoCallbackArgs parameters)
            {
                IBrowsableObjectInfoViewModel path = _path.Path;

                IItemSourceViewModel
#if CS8
                    ?
#endif
                    itemSource = path.ItemSources?.SelectedItem;

                ObservableCollection<IBrowsableObjectInfoViewModel>
#if CS8
                    ?
#endif
                    items = itemSource?.Items;

                if (items == null)

                    return;

                void updateItems() => itemSource.UpdateItems();

                switch (parameters.CallbackReason)
                {
                    case BrowsableObjectInfoCallbackReason.Added:

                        items.Add(new BrowsableObjectInfoViewModel(parameters.BrowsableObjectInfo));

                        updateItems();

                        break;

                    case BrowsableObjectInfoCallbackReason.Updated:

                        for (int i = 0; i < items.Count; i++)

                            if (items[i].Path == parameters.Path)
                            {
                                items[i] = new BrowsableObjectInfoViewModel(parameters.BrowsableObjectInfo);

                                break;
                            }

                        updateItems();

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
                (path ?? throw GetArgumentNullException(nameof(path))).PropertyChanged += Path_PropertyChanged;
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

                var items = path.ItemSources?.ItemSources;

                if (items != null)

                    foreach (var item in items)

                        item.ClearItems();

                path.SelectedItemsChanged -= Path_SelectedItemsChanged;
                path.PropertyChanged -= Path_PropertyChanged;

                path.Filter = _oldPredicate;
            }

            protected virtual void OnUpdateText() => Text = _path.Path.Path;

            protected virtual void OnUpdateCommands()
            {
                if (_commonCommands != null)

                    foreach (ButtonModel command in _commonCommands)

                        command.Update();
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

                    bool checkPaths(in sbyte decrement) => _path.Path.Path == _historyObservable[_historyObservable.CurrentIndex - decrement].Path;

                    if (_historyObservable.CanMoveBack && checkPaths(-1))

                        _historyObservable.CurrentIndex++;

                    else if (_historyObservable.CanMoveForward && checkPaths(1))

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

                IBrowsableObjectInfoViewModel path = _path.Path;

                OnPathAdded(path);

                OnUpdateText();

                OnUpdateHistory();

                OnPropertyChanged(nameof(CustomProcesses)
#if !WinCopies4
                , null, CustomProcesses
#endif
                );

                if (oldPath.Protocol == path.Protocol)

                    OnUpdateCommands();

                else
                {
                    TreeViewItems = path.GetRootItems()?.Select<IBrowsableObjectInfo, IBrowsableObjectInfoViewModel>(Factory.GetBrowsableObjectInfoViewModel);

                    if (Path.ItemSources?.SelectedItem?.ProcessSettings?.ProcessFactory == null)

                        _CommonCommands = null;

                    else
                    {
                        ButtonModel getNewButtonModel(in Func<IProcessFactory, IProcessCommand> func, in string defaultName, in Bitmap icon, in object toolTip, in ICommand<object> command) => new ButtonModel(this, func, defaultName, icon)
                        {
                            ToolTip = toolTip,

                            Command = command
                        };

                        ButtonModel getRenameButtonModel()
                        {
                            // todo: not updated when browsing

                            Bitmap icon = Path?.IsLocalRoot == true ? drive_rename : textfield_rename;

                            return getNewButtonModel(processFactory => processFactory?.RenameItemProcessCommand, RenameItem, icon, RenameItemCommandToolTip, new DelegateCommand<object>(OnCanRenameItem, obj => OnRenameItem(icon.ToImageSource())));
                        }

                        _CommonCommands = new ButtonModel[] {
                    getNewButtonModel(processFactory => processFactory?.NewItemProcessCommand, NewItem, folder_add, NewItemCommandToolTip, new DelegateCommand<object>(OnCanCreateNewItem, obj => OnCreateNewItem())),
                    getRenameButtonModel() };
                    }
                }
            }

            protected virtual void OnHistoryCurrentChanged()
            {
                if (Path.Path != _historyObservable.Current.Path)

                    Path = new BrowsableObjectInfoViewModel(_historyObservable.Current) { SortComparison = Path.SortComparison, Factory = Path.Factory };
            }

            protected virtual bool OnGoCommandCanExecute() => true;

            protected virtual void OnGoCommandExecuted() => Path = _factory.GetBrowsableObjectInfoViewModel(WinCopies.IO.ObjectModel.BrowsableObjectInfo.DefaultBrowsableObjectInfoSelectorDictionary.Select(new BrowsableObjectInfoURL3(new BrowsableObjectInfoURL2(Text), WinCopies.IO.ObjectModel.BrowsableObjectInfo.GetDefaultClientVersion())));

            protected virtual void Dispose(bool disposing)
            {
                if (_commonCommands != null)
                {
                    foreach (ButtonModel command in _commonCommands)

                        command.Dispose();

                    _commonCommands = null;
                }

                _text = null;
                _treeViewItems = null;

                OnPathRemoved(_path.Path);

                _path.Path = null;

                _historyObservable.NotifyOnPropertyChanged = false;

                if (disposing)

                    _historyObservable.Clear();

                _historyObservable = null;
                _history = null;

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
        }
    }
}
