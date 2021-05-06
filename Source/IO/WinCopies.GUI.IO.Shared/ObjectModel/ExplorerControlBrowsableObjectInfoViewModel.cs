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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

using WinCopies.IO;
using WinCopies.Util.Data;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Process;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.GUI.IO.Process;
using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using System.ComponentModel;
using WinCopies.Collections.DotNetFix;

#if !WinCopies3
using WinCopies.Util.Commands;

using static WinCopies.Util.Util;
#else
using WinCopies.Commands;

using static WinCopies.ThrowHelper;
#endif

namespace WinCopies.GUI.IO
{
    namespace ObjectModel
    {
        public class ExplorerControlBrowsableObjectInfoViewModel : ViewModelBase, IExplorerControlBrowsableObjectInfoViewModel
        {
            //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue) => OnPropertyChanged(new WinCopies.Util.Data.PropertyChangedEventArgs(propertyName, oldValue, newValue));

            private class _ObservableLinkedCollectionEnumerable : ILinkedListEnumerable<IBrowsableObjectInfo>, INotifyPropertyChanged
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
            }

            private bool _isSelected;
            private bool _isCheckBoxVisible;
            private SelectionMode _selectionMode = SelectionMode.Extended;
            private string _text;
            private System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> _treeViewItems;
            private IBrowsableObjectInfoViewModel _path;
            private _ObservableLinkedCollectionEnumerable _historyObservable;
            private IBrowsableObjectInfoFactory _factory;
            private IList _selectedItems;

            public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }

            public bool IsCheckBoxVisible { get => _isCheckBoxVisible; set { if (value && _selectionMode == SelectionMode.Single) throw new ArgumentException("Cannot apply the true value for the IsCheckBoxVisible when SelectionMode is set to Single.", nameof(value)); _isCheckBoxVisible = value; OnPropertyChanged(nameof(IsCheckBoxVisible)); } }

            public SelectionMode SelectionMode { get => _selectionMode; set { _selectionMode = value; OnPropertyChanged(nameof(SelectionMode)); } }

            public string Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> TreeViewItems { get => _treeViewItems; set { _treeViewItems = value; OnPropertyChanged(nameof(TreeViewItems)); } }

            public IBrowsableObjectInfoViewModel Path { get => _path; set { _path = value; OnPropertyChanged(nameof(Path)); OnPathChanged(); } }

            public ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo> History => _historyObservable.List;

            public IBrowsableObjectInfoFactory Factory { get => _factory; set { _factory = value ?? throw GetArgumentNullException(nameof(value)); OnPropertyChanged(nameof(Factory)); } }

            public IList SelectedItems { get => _selectedItems; set { _selectedItems = value; OnPropertyChanged(nameof(SelectedItems)); } }

            protected virtual void OnPathChanged()
            {
                Text = _path.Path;

                if (_path.Path == History.Current.Node.Value.Path)

                    return;

                if (_historyObservable.History.Count == 1)
                {
                    _ = _historyObservable.Add(_path.Model);

                    return;
                }

                if (_historyObservable.History.Count >= 2)
                {
                    _historyObservable.NotifyOnPropertyChanged = false;

                    if (_path.Path == History.Current.Node.Previous?.Value.Path)

                        _ = History.MovePrevious();

                    else if (_path.Path == History.Current.Node.Next?.Value.Path)

                        _ = History.MoveNext();

                    else
                    {
                        if (History.Current.Node.Next != null)
                        {
                            ILinkedListNode<IBrowsableObjectInfo> node = History.Current.Node;
                            ILinkedListNode<IBrowsableObjectInfo> nextNode;

                            do
                            {
                                nextNode = node.Next;

                                _historyObservable.History.Remove(node);

                            } while ((node = nextNode.Next) != null);
                        }

                        _ = _historyObservable.Add(_path.Model);
                    }

                    _historyObservable.NotifyOnPropertyChanged = true;
                }
            }

            public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> treeViewItems) => new ExplorerControlBrowsableObjectInfoViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, new BrowsableObjectInfoFactory(path.ClientVersion));

            public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, IBrowsableObjectInfoFactory factory) => new ExplorerControlBrowsableObjectInfoViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, factory ?? throw GetArgumentNullException(nameof(factory)));

            public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IBrowsableObjectInfoFactory factory) => new ExplorerControlBrowsableObjectInfoViewModel(path, path.RootItems.Select(factory.GetBrowsableObjectInfoViewModel), factory);

            public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path) => From(path, new BrowsableObjectInfoFactory());

            protected ExplorerControlBrowsableObjectInfoViewModel(in IBrowsableObjectInfoViewModel path, in System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, in IBrowsableObjectInfoFactory factory)
            {
                _path = path;

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

            public static DelegateCommand<ExplorerControlBrowsableObjectInfoViewModel> GoCommand { get; } = new DelegateCommand<ExplorerControlBrowsableObjectInfoViewModel>(browsableObjectInfo => browsableObjectInfo != null && browsableObjectInfo.OnGoCommandCanExecute(), browsableObjectInfo => browsableObjectInfo.OnGoCommandExecuted());

            protected virtual bool OnGoCommandCanExecute() => true;

            protected virtual void OnGoCommandExecuted() => Path = _factory.GetBrowsableObjectInfoViewModel(Text);

            public DelegateCommand<IBrowsableObjectInfoViewModel> ItemClickCommand { get; }

            //private ViewStyle _viewStyle = ViewStyle.SizeThree;

            //public ViewStyle ViewStyle { get => _viewStyle; set { _viewStyle = value; OnPropertyChanged(nameof(ViewStyle)); } }
        }
    }
}
