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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.Util.Commands;
using WinCopies.Util.Data;

using static WinCopies.Util.Util;

namespace WinCopies.GUI.IO.ObjectModel
{
    public class ExplorerControlBrowsableObjectInfoViewModel : ViewModelBase, IExplorerControlBrowsableObjectInfoViewModel
    {
        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue) => OnPropertyChanged(new WinCopies.Util.Data.PropertyChangedEventArgs(propertyName, oldValue, newValue));

        private SelectionMode _selectionMode = SelectionMode.Extended;

        private bool _isCheckBoxVisible;

        public bool IsCheckBoxVisible { get => _isCheckBoxVisible; set { if (value && _selectionMode == SelectionMode.Single) throw new ArgumentException("Cannot apply the true value for the IsCheckBoxVisible when SelectionMode is set to Single.", nameof(value)); _isCheckBoxVisible = value; OnPropertyChanged(nameof(IsCheckBoxVisible)); } }

        public SelectionMode SelectionMode { get => _selectionMode; set { _selectionMode = value; OnPropertyChanged(nameof(SelectionMode)); } }

        private string _text;

        public string Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }

        private IEnumerable<IBrowsableObjectInfoViewModel> _treeViewItems;

        public IEnumerable<IBrowsableObjectInfoViewModel> TreeViewItems { get => _treeViewItems; set { _treeViewItems = value; OnPropertyChanged(nameof(TreeViewItems)); } }

        private IBrowsableObjectInfoViewModel _path;

        public IBrowsableObjectInfoViewModel Path { get => _path; set { _path = value; OnPropertyChanged(nameof(Path)); OnPathChanged(); } }

        private IBrowsableObjectInfoFactory _factory;

        public IBrowsableObjectInfoFactory Factory { get => _factory; set { _factory = value ?? throw GetArgumentNullException(nameof(value)); OnPropertyChanged(nameof(BrowsableObjectInfoFactory)); } }

        protected virtual void OnPathChanged() => Text = _path.Path;

        private bool _isSelected;

        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }

        public static IExplorerControlBrowsableObjectInfoViewModel From(IBrowsableObjectInfoViewModel path) => new ExplorerControlBrowsableObjectInfoViewModel(path ?? throw GetArgumentNullException(nameof(path)), new BrowsableObjectInfoFactory(path.InnerBrowsableObjectInfo.ClientVersion.Value));

        public static IExplorerControlBrowsableObjectInfoViewModel From(IBrowsableObjectInfoViewModel path, IBrowsableObjectInfoFactory factory) => new ExplorerControlBrowsableObjectInfoViewModel(path??throw GetArgumentNullException(nameof(path)), factory??throw GetArgumentNullException(nameof(factory)));

        private ExplorerControlBrowsableObjectInfoViewModel(IBrowsableObjectInfoViewModel path, IBrowsableObjectInfoFactory factory)
        {
            _path = path;

            ItemClickCommand = new DelegateCommand<IBrowsableObjectInfoViewModel>(browsableObjectInfo => true, browsableObjectInfo =>
            {
                if (browsableObjectInfo.InnerBrowsableObjectInfo is IShellObjectInfo shellObjectInfo && shellObjectInfo.FileType == FileType.File)

                    _ = System.Diagnostics.Process.Start(new ProcessStartInfo(browsableObjectInfo.Path) { UseShellExecute = true });

                else

                    Path = browsableObjectInfo;
            });

            _factory = factory;
        }

        public static DelegateCommand<ExplorerControlBrowsableObjectInfoViewModel> GoCommand { get; } = new DelegateCommand<ExplorerControlBrowsableObjectInfoViewModel>(browsableObjectInfo => browsableObjectInfo != null && browsableObjectInfo.OnGoCommandCanExecute(), browsableObjectInfo => browsableObjectInfo.OnGoCommandExecuted());

        protected virtual bool OnGoCommandCanExecute() => true;

        protected virtual void OnGoCommandExecuted() => Path = _factory.GetBrowsableObjectInfoViewModel(_factory.GetBrowsableObjectInfo(Text));

        public DelegateCommand<IBrowsableObjectInfoViewModel> ItemClickCommand { get; }

        //private ViewStyle _viewStyle = ViewStyle.SizeThree;

        //public ViewStyle ViewStyle { get => _viewStyle; set { _viewStyle = value; OnPropertyChanged(nameof(ViewStyle)); } }
    }
}
