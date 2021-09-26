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

using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.Shell;

using System;
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
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.Linq;
using WinCopies.Temp;

using static System.Windows.Input.ApplicationCommands;

using static System.Windows.MessageBoxButton;
using static System.Windows.MessageBoxImage;
using static System.Windows.MessageBoxResult;

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

                var window = listView.GetParent<BrowsableObjectInfoWindow>(true);

                IExplorerControlBrowsableObjectInfoViewModel selectedItem = ((BrowsableObjectInfoWindowViewModel)window.DataContext).Paths.SelectedItem;

                if (selectedItem.SelectedItems == null || selectedItem.SelectedItems.Count != 1)

                    return;

                if (((IBrowsableObjectInfoViewModel)selectedItem.SelectedItems[0]).InnerObject is ShellObject shellObject)
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

        public BrowsableObjectInfoWindow(in BrowsableObjectInfoWindowViewModel dataContext)
        {
            SetResourceReference(StyleProperty, typeof(BrowsableObjectInfoWindow));

            // _ = Current._OpenWindows.AddFirst(this);

            DataContext = dataContext;

            if (dataContext.Paths.Paths.Count == 0)

                dataContext.Paths.Paths.Add(GetDefaultExplorerControlBrowsableObjectInfoViewModel());

            AddDefaultCommandBindings();

            // InitializeComponent();
        }

        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void CloseAllTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Collections.ObjectModel.ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> paths = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths;

            paths.Clear();

            IExplorerControlBrowsableObjectInfoViewModel explorerControlBrowsableObjectInfo = GetDefaultExplorerControlBrowsableObjectInfoViewModel();

            explorerControlBrowsableObjectInfo.IsSelected = true;

            paths.Add(explorerControlBrowsableObjectInfo);

            e.Handled = true;
        }

        protected virtual bool OnCanCancelClose(System.Collections.ObjectModel.ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> paths) => /* !Current.IsClosing && */ paths.Count > 1 && MessageBox.Show(this, Shell.Properties.Resources.WindowClosingMessage, "WinCopies", YesNo, Question, No) != Yes;

        protected override void OnClosing(CancelEventArgs e)
        {
            if (OnCanCancelClose(((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths))

                e.Cancel = true;

            base.OnClosing(e);
        }

        //protected override void OnClosed(EventArgs e)
        //{
        //    base.OnClosed(e);

        //    _ = Current._OpenWindows.Remove2(this);
        //}

        private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();

            e.Handled = true;
        }

        protected abstract void OnQuit();

        //{
        //    ObservableLinkedCollection<System.Windows.Window> openWindows = Current._OpenWindows;

        //    if (openWindows.Count == 1u || MessageBox.Show(this, Shell.Properties.Resources.ApplicationClosingMessage, "WinCopies", YesNo, Question, No) == Yes)
        //    {
        //        Current.IsClosing = true;

        //        while (openWindows.Count > 0)
        //        {
        //            openWindows.First.Value.Close();

        //            openWindows.RemoveFirst();
        //        }
        //    }
        //}

        private void Quit_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnQuit, e);

        protected abstract void OnAboutWindowRequested(); // => _ = new About().ShowDialog();

        private void About_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnAboutWindowRequested, e);

        protected abstract void OnSubmitABug();

        private void SubmitABug_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnSubmitABug, e);

        protected System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetEnumerable() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.Path.Items.WhereSelect(item => item.IsSelected, item => item.Model);

        private void CanRunCommand(in IProcessFactoryProcessInfo processFactory, in CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = processFactory.CanRun(GetEnumerable());

            e.Handled = true;
        }

        private static void RunCommand(in Action action, in ExecutedRoutedEventArgs e)
        {
            action();

            e.Handled = true;
        }

        private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>

            // todo:
            //e.CanExecute = new EmptyCheckEnumerator<IBrowsableObjectInfoViewModel>(((MainWindowViewModel)DataContext).SelectedItem.Path.Items.Where(item => item.IsSelected).GetEnumerator()).HasItems;

            CanRunCommand(GetProcessFactory().Copy, e);

        protected IProcessFactory GetProcessFactory() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.Path.ProcessFactory;

        private static void RunCommand(in Action action, in IRunnableProcessInfo processFactory)
        {
            if (processFactory.UserConfirmationRequired && MessageBox.Show(processFactory.GetUserConfirmationText(), Assembly.GetExecutingAssembly().GetName().Name, YesNo, Question, No) == No)

                return;

            action();
        }

        private void RunProcess(in ExecutedRoutedEventArgs e, in FuncIn<IProcessFactory, IRunnableProcessInfo> func)
        {
            IRunnableProcessInfo processFactory = func(GetProcessFactory());

            RunCommand(() => RunCommand(() => processFactory.Run(GetEnumerable(), 10u), processFactory), e);
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e) => RunProcess(e, (in IProcessFactory processFactory) => processFactory.Copy);

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e) => RunProcess(e, (in IProcessFactory processFactory) => processFactory.Cut);

        private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GetProcessFactory().CanPaste(10u);

            e.Handled = true;
        }

        protected abstract void OnPaste(); // => StartInstance(GetProcessFactory().Copy.TryGetProcessParameters(10u));

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnPaste, e);

        private void Recycle_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory().Recycling, e);

        protected abstract void OnRecycle(); // => StartInstance(GetProcessFactory().Recycling.TryGetProcessParameters(GetEnumerable()));

        private void Recycle_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnRecycle, e);

        private void Empty_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory().Clearing, e);

        protected abstract void OnEmpty(); // => StartInstance(GetProcessFactory().Clearing.TryGetProcessParameters(GetEnumerable()));

        private void Empty_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnEmpty, e);

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory().Deletion, e);

        protected abstract void OnDelete(); // => StartInstance(GetProcessFactory().Deletion.TryGetProcessParameters(GetEnumerable()));

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
                : e.CanExecute =
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
                : e.CanExecute =
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

            add(Commands.ApplicationCommands.NewTab, NewTab_Executed, Command_CanExecute);
            add(NewRegistryTab, NewRegistryTab_Executed, Command_CanExecute);
            add(NewWMITab, NewWMITab_Executed, Command_CanExecute);

            add(Commands.ApplicationCommands.NewWindow, NewWindow_Executed, Command_CanExecute);
            add(Commands.ApplicationCommands.CloseTab, CloseTab_Executed, CloseTab_CanExecute);
            add(Commands.ApplicationCommands.CloseOtherTabs, CloseOtherTabs_Executed, CloseTab_CanExecute);
            add(Commands.ApplicationCommands.CloseAllTabs, CloseAllTabs_Executed, CloseTab_CanExecute);
            add(Commands.ApplicationCommands.CloseTabsToTheLeftOrRight, CloseTabsTo_Executed, CloseTabsTo_CanExecute);

            add(ApplicationCommands.Close, CloseWindow_Executed, Command_CanExecute);

            add(Quit, Quit_Executed, Command_CanExecute);

            add(Copy, Copy_Executed, Copy_CanExecute);
            add(Cut, Cut_Executed, Copy_CanExecute);
            add(Paste, Paste_Executed, Paste_CanExecute);

            add(Delete, Recycle_Executed, Recycle_CanExecute);
            add(Commands.ApplicationCommands.Empty, Empty_Executed, Empty_CanExecute);
            add(Commands.ApplicationCommands.DeletePermanently, Delete_Executed, Delete_CanExecute);

            add(Help, About_Executed, Command_CanExecute);

            add(SubmitABug, SubmitABug_Executed, Command_CanExecute);
        }

        private void AddNewDefaultTab(in IExplorerControlBrowsableObjectInfoViewModel viewModel)
        {
            viewModel.IsSelected = true;

            ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths.Add(viewModel);
        }

        private void AddNewTab(IExplorerControlBrowsableObjectInfoViewModel viewModel, ExecutedRoutedEventArgs e)
        {
            AddNewDefaultTab(viewModel);

            e.Handled = true;
        }

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

        private void NewTab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlBrowsableObjectInfoViewModel(), e);

        private void NewRegistryTab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlBrowsableObjectInfoViewModel(new RegistryItemInfo()), e);

        private void NewWMITab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlBrowsableObjectInfoViewModel(new WMIItemInfo()), e);

        protected abstract BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow();

        private void NewWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GetNewBrowsableObjectInfoWindow().Show();

            e.Handled = true;
        }

        private void CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths.Count > 1;

            e.Handled = true;
        }

        private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths.Remove((IExplorerControlBrowsableObjectInfoViewModel)(e.Parameter is Func func ? func() : e.Parameter));

            e.Handled = true;
        }

        private void CloseOtherTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = RemoveAll(((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths, ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem, false, false);

            e.Handled = true;
        }
    }
}
