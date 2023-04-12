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
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Commands;
using WinCopies.GUI.Controls;
using WinCopies.GUI.Controls.Models;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;

using static WinCopies.GUI.IO.ObjectModel.ExplorerControlViewModel.CommonCommandsUtilities;

namespace WinCopies.GUI.IO
{
    public class CustomProcessParametersGeneratedEventArgs : EventArgs
    {
        public IProcessInfo Process { get; }

        public IProcessParameters ProcessParameters { get; }

        public CustomProcessParametersGeneratedEventArgs(in IProcessInfo process, in IProcessParameters processParameters)
        {
            Process = process;

            ProcessParameters = processParameters;
        }
    }

    namespace ObjectModel
    {
        public interface IBrowsableObjectInfoViewModelCommon : INotifyPropertyChanged
        {
            bool IsSelected { get; set; }
        }

        public interface IExplorerControlViewModel : IBrowsableObjectInfoViewModelCommon, DotNetFix.IDisposable
        {
            System.Collections.Generic.IEnumerable<IMenuItemModel<string>>
#if CS8
                ?
#endif
                BrowsabilityPaths
            { get; }

            ICommand BrowseToParent { get; }

            System.Collections.Generic.IEnumerable<IButtonModel> CommonCommands { get; }

            System.Collections.Generic.IEnumerable<IMenuItemModel<string, IMenuItemModel<string>, object>>
#if CS8
                ?
#endif
                CustomProcesses
            { get; }

            IBrowsableObjectInfoFactory Factory { get; set; }

            Predicate<IBrowsableObjectInfo> Filter { get; set; }

            ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo> History { get; }

            bool IsCheckBoxVisible { get; set; }

            DelegateCommand<IBrowsableObjectInfoViewModel> ItemClickCommand { get; }

            DelegateCommand<IBrowsableObjectInfo> OpenInNewContextCommand { get; set; }

            IButtonModel NewItemCommand { get; }

            IButtonModel RenameItemCommand { get; }

            IBrowsableObjectInfoViewModel Path { get; set; }

            ReadOnlyObservableCollection<IBrowsableObjectInfoViewModel> SelectedItems { get; }

            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                SelectedItemsInnerObjects { get; }

            SelectionMode SelectionMode { get; set; }

            string Text { get; set; }

            System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel>
#if CS8
                ?
#endif
                TreeViewItems
            { get; set; }

            event System.EventHandler<CustomProcessParametersGeneratedEventArgs> CustomProcessParametersGenerated;

            void OnItemClick(IBrowsableObjectInfoViewModel browsableObjectInfo);

            IButtonModel GetCommonCommand(CommonCommandsIndex index);

            IProcessFactory
#if CS8
                ?
#endif
                GetProcessFactory();

            void StartMonitoring();

            void StopMonitoring();
        }
    }
}
