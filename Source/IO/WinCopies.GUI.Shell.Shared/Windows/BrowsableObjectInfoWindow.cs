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

using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using WinCopies.Desktop;
using WinCopies.GUI.IO.Controls;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.Windows;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.Linq;

using static System.Windows.Input.ApplicationCommands;

using static System.Windows.MessageBoxButton;
using static System.Windows.MessageBoxImage;
using static System.Windows.MessageBoxResult;

using static WinCopies.Commands.ApplicationCommands;
using static WinCopies.Commands.Commands;
using static WinCopies.GUI.Shell.ObjectModel.BrowsableObjectInfo;

namespace WinCopies.GUI.Shell
{
    public abstract class BrowsableObjectInfoWindow : Windows.Window
    {
        private System.Windows.Interop.HwndSourceHook _hook;

        private static RoutedUICommand GetRoutedCommand(in string text, in string name) => new
#if !CS9
            RoutedUICommand
#endif
            (text, name, typeof(BrowsableObjectInfoWindow));

        public static RoutedCommand NewRegistryTab { get; } = GetRoutedCommand(Properties.Resources.NewRegistryTab, nameof(NewRegistryTab));

        public static RoutedCommand NewWMITab { get; } = GetRoutedCommand(Properties.Resources.NewWMITab, nameof(NewWMITab));

        public static RoutedCommand Quit { get; } = GetRoutedCommand(Properties.Resources.Quit, nameof(Quit));

        public static RoutedCommand SubmitABug { get; } = GetRoutedCommand(Properties.Resources.SubmitABug, nameof(SubmitABug));

        static BrowsableObjectInfoWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BrowsableObjectInfoWindow), new FrameworkPropertyMetadata(typeof(BrowsableObjectInfoWindow)));

            EventManager.RegisterClassHandler(typeof(BrowsableObjectInfoWindow), ExplorerControlListView.ContextMenuRequestedEvent, new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                var listView = (ExplorerControlListView)e.OriginalSource;

                BrowsableObjectInfoWindow window = listView.GetParent<BrowsableObjectInfoWindow>(false);

                IExplorerControlViewModel selectedItem = ((BrowsableObjectInfoWindowViewModel)window.DataContext).Paths.SelectedItem;

                if (selectedItem.SelectedItems == null || selectedItem.SelectedItems.Count != 1)

                    return;

                if ((selectedItem.SelectedItems[0]).InnerObject is ShellObject shellObject)
                {
                    var folder = (ShellContainer)selectedItem.Path.InnerObject;

                    ShellContextMenu contextMenu;

                    contextMenu = new ShellContextMenu((ShellContainer)ShellObjectFactory.Create(folder.ParsingName), new HookRegistration(hook =>
                    {
                        window._hook = (IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) => hook((WindowMessage)msg, wParam, lParam, ref handled);

                        HwndSource.FromHwnd(new WindowInteropHelper(window).Handle).AddHook(window._hook);
                    }, hook => HwndSource.FromHwnd(new WindowInteropHelper(window).Handle).RemoveHook(window._hook)), ShellObjectFactory.Create(shellObject.ParsingName));

                    _ = contextMenu.Query(1u, uint.MaxValue, ContextMenuFlags.Explore | ContextMenuFlags.CanRename);

                    Point point = ((ExplorerControlListViewContextMenuRequestedEventArgs)e).MouseButtonEventArgs.GetPosition(null);
                    var _point = new System.Drawing.Point((int)point.X, (int)point.Y);

                    contextMenu.Show(new WindowInteropHelper(window).Handle, _point);
                }
            }));
        }

        public BrowsableObjectInfoWindow(in IBrowsableObjectInfoWindowViewModel dataContext)
        {
            ThrowHelper.ThrowIfNull(dataContext, nameof(dataContext));

            SetResourceReference(StyleProperty, typeof(BrowsableObjectInfoWindow));

            DataContext = dataContext;

            if (dataContext.Paths.Paths.Count == 0)

                dataContext.Paths.Paths.Add(GetDefaultExplorerControlViewModel());

            AddDefaultCommandBindings();
        }

        protected BrowsableObjectInfoWindow() : this(GetDefaultDataContext()) { /* Left empty. */ }

        public static IBrowsableObjectInfoWindowViewModel GetDefaultDataContext()
        {
            var dataContext = new BrowsableObjectInfoWindowViewModel();

            dataContext.Paths.IsCheckBoxVisible = true;

            return dataContext;
        }

        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void CloseAllTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IList<IExplorerControlViewModel> paths = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths;

            paths.Clear();

            IExplorerControlViewModel explorerControlBrowsableObjectInfo = GetDefaultExplorerControlViewModel();

            explorerControlBrowsableObjectInfo.IsSelected = true;

            paths.Add(explorerControlBrowsableObjectInfo);

            e.Handled = true;
        }

        protected virtual bool OnCanCancelClose(IList<IExplorerControlViewModel> paths) => /* !Current.IsClosing && */ paths.Count > 1 && MessageBox.Show(this, Properties.Resources.WindowClosingMessage, "WinCopies", YesNo, Question, No) != Yes;

        protected override void OnClosing(CancelEventArgs e)
        {
            IList<IExplorerControlViewModel> paths = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths;

            if (OnCanCancelClose(paths))

                e.Cancel = true;

            else
            {
                foreach (IExplorerControlViewModel path in paths)

                    path.Dispose();

                paths.Clear();
            }

            base.OnClosing(e);
        }

        private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();

            e.Handled = true;
        }

        protected abstract void OnQuit(ExecutedRoutedEventArgs e);

        private void Quit_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnQuit, e);

        protected abstract void OnAboutWindowRequested(ExecutedRoutedEventArgs e); // => _ = new About().ShowDialog();

        private void About_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnAboutWindowRequested, e);

        protected abstract void OnSubmitABug(ExecutedRoutedEventArgs e);

        private void SubmitABug_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnSubmitABug, e);

        protected System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetEnumerable() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.Path.Items.WhereSelect(item => item.IsSelected, item => item.Model);

        private void CanRunCommand(in IProcessFactoryProcessInfo processFactory, in CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = processFactory.CanRun(GetEnumerable());

            e.Handled = true;
        }

        private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>

            // todo:
            //e.CanExecute = new EmptyCheckEnumerator<IBrowsableObjectInfoViewModel>(((MainWindowViewModel)DataContext).SelectedItem.Path.Items.Where(item => item.IsSelected).GetEnumerator()).HasItems;

            CanRunCommand(GetProcessFactory().Copy, e);

        protected IProcessFactory GetProcessFactory() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.Path.ProcessFactory;

        private void RunProcess(in ExecutedRoutedEventArgs e, in FuncIn<IProcessFactory, IRunnableProcessInfo> func)
        {
            IRunnableProcessInfo processFactory = func(GetProcessFactory());

            RunCommand(_e =>
            {
                if (processFactory.UserConfirmationRequired && MessageBox.Show(processFactory.GetUserConfirmationText(), Assembly.GetExecutingAssembly().GetName().Name, YesNo, Question, No) == No)

                    return;

                processFactory.Run(GetEnumerable(), 10u);
            }, e);
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e) => RunProcess(e, (in IProcessFactory processFactory) => processFactory.Copy);

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e) => RunProcess(e, (in IProcessFactory processFactory) => processFactory.Cut);

        private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GetProcessFactory().CanPaste(10u);

            e.Handled = true;
        }

        protected abstract void OnPaste(ExecutedRoutedEventArgs e); // => StartInstance(GetProcessFactory().Copy.TryGetProcessParameters(10u));

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnPaste, e);

        private void Recycle_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory().Recycling, e);

        protected abstract void OnRecycle(ExecutedRoutedEventArgs e); // => StartInstance(GetProcessFactory().Recycling.TryGetProcessParameters(GetEnumerable()));

        private void Recycle_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnRecycle, e);

        private void Empty_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory().Clearing, e);

        protected abstract void OnEmpty(ExecutedRoutedEventArgs e); // => StartInstance(GetProcessFactory().Clearing.TryGetProcessParameters(GetEnumerable()));

        private void Empty_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnEmpty, e);

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory().Deletion, e);

        protected abstract void OnDelete(ExecutedRoutedEventArgs e); // => StartInstance(GetProcessFactory().Deletion.TryGetProcessParameters(GetEnumerable()));

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnDelete, e);

        private static InvalidOperationException GetInvalidParameterException() => new InvalidOperationException("The given parameter is not valid.");

        private void CloseTabsTo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var parameter = (CloseTabsTo)e.Parameter;
            var dataContext = (BrowsableObjectInfoWindowViewModel)DataContext;

#if CS8
            e.CanExecute =
#else
            switch (
#endif
            parameter
#if CS8
            switch
#else
            )
#endif
            {
#if !CS8
                case
#endif
                CloseTabsTo.Left
#if CS8
                =>
#else
                :
                    e.CanExecute =
#endif
                dataContext.Paths.SelectedIndex > 0
#if CS8
                ,
#else
                ;

                    break;

                case
#endif
                CloseTabsTo.Right
#if CS8
                =>
#else
                :
                    e.CanExecute =
#endif
                dataContext.Paths.SelectedIndex < dataContext.Paths.Paths.Count - 1
#if CS8
                ,

                _ =>
#else
                ;

                    break;

                default:
#endif
                    throw GetInvalidParameterException()
#if CS8
            };
#else
                ;
            }
#endif

            e.Handled = true;
        }

        private void CloseTabsTo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var parameter = (CloseTabsTo)e.Parameter;
            var dataContext = (BrowsableObjectInfoWindowViewModel)DataContext;

            switch (parameter)
            {
                case CloseTabsTo.Left:

                    for (int i = 0; i < dataContext.Paths.SelectedIndex; i++)

                        dataContext.Paths.Paths.RemoveAt(0);

                    break;

                case CloseTabsTo.Right:

                    for (int i = dataContext.Paths.Paths.Count - 1; i > dataContext.Paths.SelectedIndex; i--)

                        dataContext.Paths.Paths.RemoveAt(0);

                    break;

                default:

                    throw GetInvalidParameterException();
            }

            e.Handled = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var menuItems = new TitleBarMenuItemQueue();

            menuItems.Enqueue(new TitleBarMenuItem() { Command = Commands.ApplicationCommands.NewTab });
            menuItems.Enqueue(new TitleBarMenuItem() { Command = Commands.ApplicationCommands.CloseTab, CommandParameter = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem });

            TitleBarMenuItems = menuItems;
        }

        private void Window_PreviousCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.History.CanMovePreviousFromCurrent;

            e.Handled = true;
        }

        private void Window_NextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.History.CanMoveNextFromCurrent;

            e.Handled = true;
        }

        private void Window_PreviousExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.History.CurrentIndex++;

            CommandManager.InvalidateRequerySuggested();

            e.Handled = true;
        }

        private void Window_NextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.History.CurrentIndex--;

            CommandManager.InvalidateRequerySuggested();

            e.Handled = true;
        }

        protected virtual void AddDefaultCommandBindings()
        {
            void add(in ICommand command, in ExecutedRoutedEventHandler executedRoutedEventHandler, in CanExecuteRoutedEventHandler canExecuteRoutedEventHandler) => CommandBindings.Add(new CommandBinding(command, executedRoutedEventHandler, canExecuteRoutedEventHandler));

            add(NavigationCommands.BrowseBack, Window_PreviousExecuted, Window_PreviousCanExecute);
            add(NavigationCommands.BrowseForward, Window_NextExecuted, Window_NextCanExecute);

            #region File
            #region New
            add(NewTab, NewTab_Executed, Command_CanExecute);
            add(NewRegistryTab, NewRegistryTab_Executed, Command_CanExecute);
            add(NewWMITab, NewWMITab_Executed, Command_CanExecute);

            add(NewWindow, NewWindow_Executed, Command_CanExecute);
            #endregion New

            #region Open
            add(OpenOrLaunch, Open_Executed, Open_CanExecute);
            add(OpenInNewTab, OpenInNewTab_Executed, OpenInNewTabOrWindow_CanExecute);
            add(OpenInNewWindow, OpenInNewWindow_Executed, OpenInNewTabOrWindow_CanExecute);
            #endregion Open

            #region Close
            add(CloseTab, CloseTab_Executed, CloseTab_CanExecute);
            add(CloseOtherTabs, CloseOtherTabs_Executed, CloseTab_CanExecute);
            add(CloseAllTabs, CloseAllTabs_Executed, CloseTab_CanExecute);
            add(CloseTabsToTheLeftOrRight, CloseTabsTo_Executed, CloseTabsTo_CanExecute);

            add(ApplicationCommands.Close, CloseWindow_Executed, Command_CanExecute);
            #endregion Close

            add(Quit, Quit_Executed, Command_CanExecute);
            #endregion File

            #region Process
            #region Copy
            add(Copy, Copy_Executed, Copy_CanExecute);
            add(Cut, Cut_Executed, Copy_CanExecute);
            add(Paste, Paste_Executed, Paste_CanExecute);
            #endregion Copy

            #region Deletion
            add(Delete, Recycle_Executed, Recycle_CanExecute);
            add(Empty, Empty_Executed, Empty_CanExecute);
            add(DeletePermanently, Delete_Executed, Delete_CanExecute);
            #endregion Deletion
            #endregion Process

            #region Help
            add(Help, About_Executed, Command_CanExecute);

            add(SubmitABug, SubmitABug_Executed, Command_CanExecute);
            #endregion Help
        }

        protected IExplorerControlViewModel GetCurrentExplorerControlViewModel() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem;

        protected System.Collections.IList GetSelectedItems() => GetCurrentExplorerControlViewModel().SelectedItems;

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            IExplorerControlViewModel selectedItem = GetCurrentExplorerControlViewModel();

            e.CanExecute = selectedItem.SelectedItems?.Count > 0 && selectedItem.SelectedItems[0] is IBrowsableObjectInfoViewModel browsableObjectInfo && selectedItem.ItemClickCommand.CanExecute(browsableObjectInfo);
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() =>
        {
            IExplorerControlViewModel selectedItem = GetCurrentExplorerControlViewModel();
            IList _selectedItems = selectedItem.SelectedItems;

            if (_selectedItems?.Count > 0)
            {
                System.Collections.IList selectedItems = new ArrayList(_selectedItems.Count);

                foreach (object item in _selectedItems)

                    _ = selectedItems.Add(item);

                Action<IBrowsableObjectInfoViewModel> action;

                void _openItem(IBrowsableObjectInfoViewModel browsableObjectInfo)
                {
                    IBrowsableObjectInfoViewModel item = ExplorerControlViewModel.GetBrowsableObjectInfoOrLaunchItem(browsableObjectInfo);

                    if (item != null)

                        AddNewDefaultTab(GetDefaultExplorerControlViewModel(item), false);
                }

                void openItem(IBrowsableObjectInfoViewModel browsableObjectInfo)
                {
                    action = _openItem;

                    if (selectedItem.ItemClickCommand == null)

                        selectedItem.OnItemClick(browsableObjectInfo);

                    else

                        _ = selectedItem.ItemClickCommand.TryExecute(browsableObjectInfo);
                }

                action = openItem;

                foreach (object item in selectedItems)

                    if (item is IBrowsableObjectInfoViewModel _item)

                        action(_item);
            }
        }, e);

        public bool AreSelectedItemsBrowsableByDefault(out System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> selectedItems)
        {
            System.Collections.IList _selectedItems = GetSelectedItems();

            selectedItems = _selectedItems.As<IBrowsableObjectInfoViewModel>();

            if (_selectedItems?.Count > 0)
            {
                foreach (IBrowsableObjectInfoViewModel item in selectedItems)

                    if (item.Browsability.Browsability != Browsability.BrowsableByDefault)

                        return false;

                return true;
            }

            return false;
        }

        private void OpenInNewTabOrWindow_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = AreSelectedItemsBrowsableByDefault(out _);

        protected void OpenInNewTabOrWindow(in ActionIn<IBrowsableObjectInfoViewModel> action)
        {
            if (AreSelectedItemsBrowsableByDefault(out IEnumerable<IBrowsableObjectInfoViewModel> selectedItems))

                foreach (IBrowsableObjectInfoViewModel item in selectedItems)

                    action(item);
        }

        protected void OpenInNewTabOrWindow(ActionIn<IExplorerControlViewModel, bool> action) => OpenInNewTabOrWindow((in IBrowsableObjectInfoViewModel item) => action(GetDefaultExplorerControlViewModel(item), false));

        private void OpenInNewTab_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() => OpenInNewTabOrWindow(AddNewDefaultTab), e);

        private void OpenInNewWindow_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() =>
        {
            BrowsableObjectInfoCollectionViewModel items;

            ActionIn<IExplorerControlViewModel> action = addItem;

            void _addItem(in IExplorerControlViewModel item) => items.Paths.Add(item);

            void addItem(in IExplorerControlViewModel item)
            {
                items = new BrowsableObjectInfoCollectionViewModel();

                action = _addItem;

                _addItem(item);

                GetNewBrowsableObjectInfoWindow(new BrowsableObjectInfoWindowViewModel(items)).Show();
            }

            OpenInNewTabOrWindow((in IExplorerControlViewModel item, in bool selected) => action(item));
        }, e);

        private void AddNewDefaultTab(in IExplorerControlViewModel viewModel, in bool selected)
        {
            viewModel.IsSelected = selected;

            ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths.Add(viewModel);
        }

        private void AddNewTab(IExplorerControlViewModel viewModel, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() => AddNewDefaultTab(viewModel, true), e);

        // todo: replace by the WinCopies.Util's same method.

        public static bool RemoveAll<T>(in IList<T> collection, in T itemToKeep, in bool onlyOne, in bool throwIfMultiple) where T : class
        {
            while (collection.Count != 1)
            {
                if (collection[0] == itemToKeep)
                {
                    if (onlyOne)

                        while (collection.Count != 1)
                        {
                            if (collection[1] == itemToKeep)
                            {
                                if (throwIfMultiple) throw new InvalidOperationException("More than one occurence were found.");

                                else

                                    while (collection.Count != 1)

                                        collection.RemoveAt(1);

                                return false;
                            }

                            collection.RemoveAt(1);

                            return true;
                        }

                    while (collection.Count != 1)

                        collection.RemoveAt(1);

                    return true;
                }

                collection.RemoveAt(0);
            }

            return false;
        }

        private void NewTab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlViewModel(), e);

        private void NewRegistryTab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlViewModel(new RegistryItemInfo()), e);

        private void NewWMITab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlViewModel(new WMIItemInfo()), e);

        protected abstract BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow();
        protected abstract BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow(in IBrowsableObjectInfoWindowViewModel dataContext);

        private void NewWindow_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() => GetNewBrowsableObjectInfoWindow().Show(), e);

        private void CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths.Count > 1;

            e.Handled = true;
        }

        private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths.Remove((IExplorerControlViewModel)(e.Parameter is Func func ? func() : e.Parameter)), e);

        private void CloseOtherTabs_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() => RemoveAll(((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths, ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem, false, false), e);
    }

    public abstract class BrowsableObjectInfoWindow2 : BrowsableObjectInfoWindow
    {
        protected BrowsableObjectInfoWindow2() : base() { /* Left empty. */ }

        protected BrowsableObjectInfoWindow2(in IBrowsableObjectInfoWindowViewModel dataContext) : base(dataContext) { /* Left empty. */ }

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => GetNewBrowsableObjectInfoWindow(GetDefaultDataContext());
    }
}
