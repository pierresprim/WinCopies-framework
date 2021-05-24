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

using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Controls.ViewModels;
using WinCopies.GUI.Windows;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.Util.Data;
using WinCopies.Linq;

#if !WinCopies3
using WinCopies.Util.Commands;

using static WinCopies.Util.Util;
#else
using WinCopies.Commands;

using static WinCopies.ThrowHelper;
#endif

namespace WinCopies.GUI.IO.ObjectModel
{
    public class ExplorerControlBrowsableObjectInfoViewModel : ViewModelBase, IExplorerControlBrowsableObjectInfoViewModel
    {
        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue) => OnPropertyChanged(new WinCopies.Util.Data.PropertyChangedEventArgs(propertyName, oldValue, newValue));

        [InterfaceDataTemplateSelector.Ignore]
        private class ButtonModel : ExtendedButtonViewModel<IExtendedButtonModel<string, object>, string, object>, WinCopies.DotNetFix.IDisposable
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

        private class _ObservableLinkedCollectionEnumerable : ILinkedListEnumerable<IBrowsableObjectInfo>, INotifyPropertyChanged, WinCopies.DotNetFix.IDisposable
        {
            public ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo> List { get; }

            public ObservableLinkedCollection<IBrowsableObjectInfo> History { get; }

            public bool NotifyOnPropertyChanged { get; set; }

            public ILinkedListNodeEnumerable<IBrowsableObjectInfo> First => List.First;

            public ILinkedListNodeEnumerable<IBrowsableObjectInfo> Current => List.Current;

            public ILinkedListNodeEnumerable<IBrowsableObjectInfo> Last => List.Last;

            public EnumerationDirection EnumerationDirection => List.EnumerationDirection;

            ILinkedListNodeEnumerable ILinkedListEnumerable.First => First;

            ILinkedListNodeEnumerable ILinkedListEnumerable.Current => Current;

            ILinkedListNodeEnumerable ILinkedListEnumerable.Last => Last;

            public bool IsDisposed { get; private set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public _ObservableLinkedCollectionEnumerable()
            {
                (History = new ObservableLinkedCollection<IBrowsableObjectInfo>()).CollectionChanged += History_CollectionChanged;

                (List = new ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo>(History, EnumerationDirection.LIFO)).PropertyChanged += _ObservableLinkedCollectionEnumerable_PropertyChanged;
            }

            private void History_CollectionChanged(object sender, LinkedCollectionChangedEventArgs<IBrowsableObjectInfo> e)
            {
                switch (e.Action)
                {
                    case LinkedCollectionChangedAction.AddFirst:

                        NotifyOnPropertyChanged = false;

                        List.UpdateCurrent(e.Node);

                        NotifyOnPropertyChanged = true;

                        break;
                }
            }

            private void _ObservableLinkedCollectionEnumerable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (NotifyOnPropertyChanged)

                    PropertyChanged?.Invoke(sender, e);
            }

            public ILinkedListNode<IBrowsableObjectInfo> Add(IBrowsableObjectInfo value) => List.Add(value);

            public void UpdateCurrent(ILinkedListNode<IBrowsableObjectInfo> node) => List.UpdateCurrent(node);

            public System.Collections.Generic.IEnumerator<ILinkedListNode<IBrowsableObjectInfo>> GetEnumeratorToCurrent(bool keepCurrent) => List.GetEnumeratorToCurrent(keepCurrent);

            public System.Collections.Generic.IEnumerator<ILinkedListNode<IBrowsableObjectInfo>> GetEnumeratorFromCurrent(bool keepCurrent) => List.GetEnumeratorFromCurrent(keepCurrent);

            public bool MovePrevious() => List.MovePrevious();

            public bool MoveNext() => List.MoveNext();

            public void UpdateCurrent(IReadOnlyLinkedListNode node) => ((ILinkedListEnumerable)List).UpdateCurrent(node);

            public System.Collections.Generic.IEnumerator<IBrowsableObjectInfo> GetEnumerator() => List.GetEnumerator();

            System.Collections.IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)List).GetEnumerator();

            public ILinkedListNodeEnumerable<IBrowsableObjectInfo> GetLinkedListNodeEnumerable(ILinkedListNode<IBrowsableObjectInfo> node) => List.GetLinkedListNodeEnumerable(node);

            public void Dispose()
            {
                if (IsDisposed)

                    return;

                History.CollectionChanged -= History_CollectionChanged;

                List.PropertyChanged -= _ObservableLinkedCollectionEnumerable_PropertyChanged;

                IsDisposed = true;
            }

            ~_ObservableLinkedCollectionEnumerable() => Dispose();
        }

        private const int _newItemCommandIndex = 0;
        private bool _isSelected;
        private bool _isCheckBoxVisible;
        private SelectionMode _selectionMode = SelectionMode.Extended;
        private string _text;
        private System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> _treeViewItems;
        private IBrowsableObjectInfoViewModel _path;
        private _ObservableLinkedCollectionEnumerable _historyObservable;
        private IBrowsableObjectInfoFactory _factory;
        private IList _selectedItems;
        private System.Collections.Generic.IReadOnlyList<ButtonModel> _commonCommands;

        public event System.EventHandler<CustomProcessParametersGeneratedEventArgs> CustomProcessParametersGeneratedEventHandler;

        protected void UpdateValueIfNotDisposed<T>(ref T value, in T newValue, in string propertyName)
        {
            if (IsDisposed)

                throw GetExceptionForDispose(false);

            base.UpdateValue(ref value, newValue, propertyName);
        }

        protected T GetValueIfNotDisposed<T>(in T value) => IsDisposed ? throw GetExceptionForDispose(false) : value;

        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }

        public bool IsCheckBoxVisible { get => _isCheckBoxVisible; set { if (value && _selectionMode == SelectionMode.Single) throw new ArgumentException("Cannot apply the true value for the IsCheckBoxVisible when SelectionMode is set to Single.", nameof(value)); _isCheckBoxVisible = value; OnPropertyChanged(nameof(IsCheckBoxVisible)); } }

        public SelectionMode SelectionMode { get => _selectionMode; set { _selectionMode = value; OnPropertyChanged(nameof(SelectionMode)); } }

        public string Text { get => GetValueIfNotDisposed(_text); set => UpdateValueIfNotDisposed(ref _text, value, nameof(Text)); }

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> TreeViewItems { get => GetValueIfNotDisposed(_treeViewItems); set => UpdateValueIfNotDisposed(ref _treeViewItems, value, nameof(TreeViewItems)); }

        public IBrowsableObjectInfoViewModel Path { get => GetValueIfNotDisposed(_path); set { UpdateValueIfNotDisposed(ref _path, value, nameof(Path)); OnPathChanged(); } }

        public ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo> History => GetValueIfNotDisposed(_historyObservable).List;

        public IBrowsableObjectInfoFactory Factory { get => GetValueIfNotDisposed(_factory); set => UpdateValueIfNotDisposed(ref _factory, value ?? throw GetArgumentNullException(nameof(value)), nameof(Factory)); }

        public IList SelectedItems { get => GetValueIfNotDisposed(_selectedItems); set => UpdateValueIfNotDisposed(ref _selectedItems, value, nameof(SelectedItems)); }

        public IButtonModel NewItemCommand => GetValueIfNotDisposed(_commonCommands)[_newItemCommandIndex];

        public System.Collections.Generic.IEnumerable<IButtonModel> CommonCommands => GetValueIfNotDisposed(_commonCommands);

        public DelegateCommand<IBrowsableObjectInfoViewModel> ItemClickCommand { get; }

        public System.Collections.Generic.IEnumerable<MenuItemModel<string, MenuItemModel<string>, object>> CustomProcesses => Path.ProcessFactory.CustomProcesses?.GroupBy(item => item.GroupName, (groupName, processes) => new MenuItemModel<string, MenuItemModel<string>, object>(null, null) { Header = groupName, Items = processes.Select(process => new MenuItemModel<string>(process.Name, new DelegateCommand(obj => process.CanRun(obj, Path.Model, SelectedItems?.Select(_obj => ((IBrowsableObjectInfoViewModel)_obj).Model)), _obj =>
        {
            WinCopies.IO.Process.IProcessParameters processParameters = SelectedItems  ==null||SelectedItems.Count==0? process.GetProcessParameters(_obj, Path.Model.Parent, Path.Model ):process.GetProcessParameters(_obj, Path.Model, SelectedItems.Select(path=>((IBrowsableObjectInfoViewModel)path).Model));

            if (processParameters != null)

                CustomProcessParametersGeneratedEventHandler?.Invoke(this, new CustomProcessParametersGeneratedEventArgs(processParameters));
        }))) });

        public static DelegateCommand<ExplorerControlBrowsableObjectInfoViewModel> GoCommand { get; } = new DelegateCommand<ExplorerControlBrowsableObjectInfoViewModel>(browsableObjectInfo => browsableObjectInfo != null && browsableObjectInfo.OnGoCommandCanExecute(), browsableObjectInfo => browsableObjectInfo.OnGoCommandExecuted());

        public bool IsDisposed { get; private set; }

        protected virtual void OnPathChanged()
        {
            Text = _path.Path;

            foreach (ButtonModel command in _commonCommands)

                command.Update();

            OnPropertyChanged(nameof(CustomProcesses));

            if (_path.Path == History.Current.Node.Value.Path)

                return;

            if (_historyObservable.History.Count == 1)
            {
                _ = _historyObservable.Add(_path.Model);

                return;
            }

            else if (_historyObservable.History.Count >= 2)
            {
                _historyObservable.NotifyOnPropertyChanged = false;

                if (_path.Path == History.Current.Node.Next?.Value.Path)

                    _ = History.MovePrevious();

                else if (_path.Path == History.Current.Node.Previous?.Value.Path)

                    _ = History.MoveNext();

                else
                {
                    if (History.Current.Node.Previous != null)
                    {
                        ILinkedListNode<IBrowsableObjectInfo> node = History.Current.Node.Previous;
                        ILinkedListNode<IBrowsableObjectInfo> previousNode;

                        do
                        {
                            previousNode = node.Previous;

                            _historyObservable.History.Remove(node);

                        } while ((node = previousNode) != null);
                    }

                    _ = _historyObservable.Add(_path.Model);
                }

                _historyObservable.NotifyOnPropertyChanged = true;
            }
        }

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IDisposableEnumerable<IBrowsableObjectInfoViewModel> treeViewItems) => new ExplorerControlBrowsableObjectInfoViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, new BrowsableObjectInfoFactory(path.ClientVersion));

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IDisposableEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, IBrowsableObjectInfoFactory factory) => new ExplorerControlBrowsableObjectInfoViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, factory ?? throw GetArgumentNullException(nameof(factory)));

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IBrowsableObjectInfoFactory factory) => new ExplorerControlBrowsableObjectInfoViewModel(path, path.RootItems.Select(factory.GetBrowsableObjectInfoViewModel), factory);

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path) => From(path, new BrowsableObjectInfoFactory());

        protected ExplorerControlBrowsableObjectInfoViewModel(in IBrowsableObjectInfoViewModel path, in System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, in IBrowsableObjectInfoFactory factory)
        {
            _path = path;

            _commonCommands = new ButtonModel[] { new ButtonModel(()=>Path.ProcessFactory.NewItemProcessCommands?.Name, Icons.Properties.Resources.folder_add)
            {
                ToolTip = "Opens a dialog for item creation.",

                Command = new  DelegateCommand<object>(obj => !IsDisposed&& Path.ProcessFactory.NewItemProcessCommands?.CanCreateNewItem()==true, obj =>
                {
                    if (InputBox.ShowDialog(Path.ProcessFactory.NewItemProcessCommands.Name, DialogButton.OKCancel, Path.ProcessFactory.NewItemProcessCommands.Caption, null, out string result) == true)

                        if (!Path.ProcessFactory.NewItemProcessCommands.TryCreateNewItem(result, out _))

                            _ = MessageBox.Show("The item could not be created.", "Item creation error", MessageBoxButton.OK, MessageBoxImage.Error);
                })
            }};

            (_historyObservable = new _ObservableLinkedCollectionEnumerable()).PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == nameof(ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo>.Current))

                        OnHistoryCurrentChanged(e);
                };

            _ = _historyObservable.Add(path.Model);

            _treeViewItems = treeViewItems;

            ItemClickCommand = new DelegateCommand<IBrowsableObjectInfoViewModel>(browsableObjectInfo => true, browsableObjectInfo =>
            {
                if (browsableObjectInfo.InnerObject is ShellObject && browsableObjectInfo.ObjectProperties is IFileSystemObjectInfoProperties properties && properties.FileType == FileType.File)

                    _ = System.Diagnostics.Process.Start(new ProcessStartInfo(browsableObjectInfo.Path) { UseShellExecute = true });

                else

                    Path = browsableObjectInfo.RootParentIsRootNode ? new BrowsableObjectInfoViewModel(browsableObjectInfo.Model) : browsableObjectInfo;
            });

            _factory = factory;
        }

        protected virtual void OnHistoryCurrentChanged(System.ComponentModel.PropertyChangedEventArgs e) => Path = new BrowsableObjectInfoViewModel(_historyObservable.Current.Node.Value);

        protected virtual bool OnGoCommandCanExecute() => true;

        protected virtual void OnGoCommandExecuted() => Path = _factory.GetBrowsableObjectInfoViewModel(Text);

        protected virtual void Dispose(bool disposing)
        {
            foreach (ButtonModel command in _commonCommands)

                command.Dispose();

            _commonCommands = null;

            _text = null;

            _treeViewItems = null;

            _path = null;

            _historyObservable.Dispose();

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

        ~ExplorerControlBrowsableObjectInfoViewModel() => Dispose(false);

        //private ViewStyle _viewStyle = ViewStyle.SizeThree;

        //public ViewStyle ViewStyle { get => _viewStyle; set { _viewStyle = value; OnPropertyChanged(nameof(ViewStyle)); } }
    }
}
