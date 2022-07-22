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

#region Usings
#region Namespaces
#region WAPICP
using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.Win32Native;
using Microsoft.WindowsAPICodePack.Win32Native.Menus;
using Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager;
#endregion

#region System
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
#endregion

#region WinCopies
using WinCopies.Desktop;
using WinCopies.GUI.IO.Controls;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.Windows;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.Linq;
#endregion
#endregion

#region Static Usings
#region System
using static System.Windows.Clipboard;
using static System.Windows.Input.ApplicationCommands;

using static System.Windows.MessageBoxButton;
using static System.Windows.MessageBoxImage;
using static System.Windows.MessageBoxResult;
#endregion

#region WinCopies
using static WinCopies.Commands.ApplicationCommands;
using static WinCopies.Commands.Commands;
using static WinCopies.GUI.Icons.Properties.Resources;
#endregion
#endregion

using IOResources = WinCopies.GUI.IO.Properties.Resources;
using ListViewItem = WinCopies.GUI.Controls.ListViewItem;
using TabItem = WinCopies.GUI.Controls.TabItem;
#endregion

namespace WinCopies.GUI.IO
{
    public abstract class BrowsableObjectInfoWindow : Windows.Window
    {
        private class HookRegistration
        {
            private System.Windows.Interop.HwndSourceHook _hook;
            private readonly BrowsableObjectInfoWindow _window;

            public Microsoft.WindowsAPICodePack.HookRegistration InnerStruct { get; }

            public FuncRef<uint?, string
#if CS8
                ?
#endif
                > Func
            { get; }

            public HookRegistration(IntPtr hwnd, in FuncRef<uint?, string
#if CS8
                ?
#endif
                > func, in BrowsableObjectInfoWindow window)
            {
                _window = window;

                Func = func;

                InnerStruct = new Microsoft.WindowsAPICodePack.HookRegistration(hook => HwndSource.FromHwnd(hwnd).AddHook(_hook = (IntPtr _hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
                {
                    if (msg == (int)WindowMessage.MenuSelect)
                    {
                        uint? value = Core.GetLoWord((ulong)wParam);

                        string result = Func(ref value);

                        if (result == null && value.HasValue)

                            switch ((ContextMenuCommand)value.Value)
                            {
                                case ContextMenuCommand.None:

                                    break;

                                case ContextMenuCommand.Open:

                                    result = IOResources.OpenOrLaunchStatusBarLabel;

                                    break;

                                case ContextMenuCommand.OpenInNewTab:

                                    result = IOResources.OpenInNewTabStatusBarLabel;

                                    break;

                                case ContextMenuCommand.OpenInNewWindow:

                                    result = IOResources.OpenInNewWindowStatusBarLabel;

                                    break;

                                case ContextMenuCommand.CopyName:

                                    result = IOResources.CopyNameStatusBarLabel;

                                    break;

                                case ContextMenuCommand.CopyPath:

                                    result = IOResources.CopyPathStatusBarLabel;

                                    break;
                            }

                        _window.StatusBarLabel = result;
                    }

                    return hook((WindowMessage)msg, wParam, lParam, ref handled);
                }), hook => HwndSource.FromHwnd(hwnd).RemoveHook(_hook));
            }
        }

        private delegate IBrowsableObjectInfoWindowViewModel GetPathsFunc<T>(in RoutedEventArgs e, out T sender, out BrowsableObjectInfoWindow window);

        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, BrowsableObjectInfoWindow>(propertyName);

        public abstract ClientVersion ClientVersion { get; }

        public static RoutedCommand Quit { get; } = GetRoutedCommand(IOResources.Quit, nameof(Quit));

        public static RoutedCommand SubmitABug { get; } = GetRoutedCommand(IOResources.SubmitABug, nameof(SubmitABug));

        public static DependencyProperty ViewModelProperty = Register<IBrowsableObjectInfoWindowViewModel>(nameof(ViewModel));

        public IBrowsableObjectInfoWindowViewModel ViewModel { get => (IBrowsableObjectInfoWindowViewModel)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

        public static DependencyProperty StatusBarLabelProperty = Register<string>(nameof(StatusBarLabel));

        public string StatusBarLabel { get => (string)GetValue(StatusBarLabelProperty); set => SetValue(StatusBarLabelProperty, value); }

        static BrowsableObjectInfoWindow()
        {
            DefaultStyleKeyProperty.OverrideDefaultStyleKey<BrowsableObjectInfoWindow>();

            Microsoft.WindowsAPICodePack.Shell.Window
#if CS8
                    ?
#endif
                    getWindow(RoutedEventArgs e)
            {
                Microsoft.WindowsAPICodePack.Shell.Window
#if CS8
                    ?
#endif
                    _getWindow()
                {
                    Microsoft.WindowsAPICodePack.Shell.Window
#if CS8
                        ?
#endif
                        __getWindow(in DependencyObject __d, in Predicate<Microsoft.WindowsAPICodePack.Shell.Window> func) => UtilHelpers.GetValue((Microsoft.WindowsAPICodePack.Shell.Window)GetWindow(__d), func);

#if CS8
                    return
#else
                    switch (
#endif
                    e.OriginalSource
#if CS8
                    switch
#else
                    )
#endif
                    {
#if !CS8
                        case
#endif
                        DockPanel d
#if CS8
                            =>
#else
                        :
                            return
#endif
                            __getWindow(d, _window => _window?.GetChild<Grid>(true, out _)?.GetChild<DockPanel>(true, out _) == d)
#if CS8
                            ,
#else
                            ;
                        case
#endif
                        TabPanel d
#if CS8
                            =>
#else
                        :
                            return
#endif
                            __getWindow(d, _window => d.Parent is Grid grid && VisualTreeHelper.GetParent(grid) is ExplorerControlTabControl)
#if CS8
                            ,
#else
                            ;
                        case
#endif
                        Border d
#if CS8
                            =>
#else
                        :
                            return
#endif
                            __getWindow(d, _window => _window?.GetChild<Grid>(true, out _)?.GetChild<StatusBar>(true, out _) == d.GetParent<StatusBar>(false))
#if CS8
                            ,
                        _ =>
#else
                            ;
                        default:

                            return
#endif
                    null
#if CS8
                    };
#else
                        ;
                    }
#endif
                }

                Microsoft.WindowsAPICodePack.Shell.Window
#if CS8
                    ?
#endif
                    window = _getWindow();

                UtilHelpers.PerformActionIfNotNull(window, () => e.Handled = true);

                return window;
            }

            void registerClassHandler(in RoutedEvent @event, in Action<object, MouseButtonEventArgs> action) => RegisterClassHandler(@event, new MouseButtonEventHandler(action));

            registerClassHandler(MouseLeftButtonDownEvent, (object sender, MouseButtonEventArgs e) =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)

                        getWindow(e)?.DragMove();
                });

            registerClassHandler(MouseRightButtonDownEvent, (object sender, MouseButtonEventArgs e) =>
            {
                var window = getWindow(e);

                if (window != null)
                {
                    var point = ((Visual)e.OriginalSource).PointToScreen(e.GetPosition(window));

                    var command = (SystemCommand)Menus.TrackPopupMenu(Menus.GetSystemMenu(window.Handle, false), TrackPopupMenuFlags.ReturnCommand, new System.Drawing.Point((int)point.X, (int)point.Y), window.Handle);

                    if (command > 0)

                        Core.SendMessage(window.Handle, WindowMessage.SystemCommand, (IntPtr)command, IntPtr.Zero);
                }
            });

            RegisterClassHandler(MouseMoveEvent, new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    var window = getWindow(e);

                    if (window?.WindowState == WindowState.Maximized)
                    {
                        window.WindowState = WindowState.Normal;

                        window.Top = 0;

                        window.DragMove();
                    }
                }
            }));

            registerClassHandler(MouseDoubleClickEvent, (object sender, MouseButtonEventArgs e) =>
            {
                var window = getWindow(e);

                if (window != null)

                    window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            });

            IBrowsableObjectInfoWindowViewModel
#if CS8
                ?
#endif
                _getDataContext<T>(in RoutedEventArgs e, out T
#if CS8
                ?
#endif
                sender, out BrowsableObjectInfoWindow
#if CS8
                ?
#endif
                window) where T : DependencyObject
            {
                if (e.OriginalSource is T _sender)
                {
                    sender = _sender;

                    window = _sender.GetParent<BrowsableObjectInfoWindow>(false);

                    return window?.DataContext as IBrowsableObjectInfoWindowViewModel;
                }

                sender = null;
                window = null;

                return null;
            }

            IBrowsableObjectInfoWindowViewModel
#if CS8
                ?
#endif
                getDataContext(in RoutedEventArgs e, out ExplorerControlListViewItem
#if CS8
                ?
#endif
                sender, out BrowsableObjectInfoWindow
#if CS8
                ?
#endif
                window) => _getDataContext(e, out sender, out window);

            void _openInNewTab(in IBrowsableObjectInfoWindowViewModel dataContext, in IBrowsableObjectInfoViewModel _path, in bool selected) => dataContext.Paths.Paths.Add(ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel(_path, selected));

            RegisterClassHandler(ExtraEvents.ClickEvent, new ClickEventHandler((object sender, ClickEventArgs e) =>
                    {
                        if (e.Button == MouseButton.Middle)
                        {
                            IBrowsableObjectInfoCollectionViewModel getPaths<T>(in GetPathsFunc<T> func) => func(e, out _, out _).Paths;

                            if (e.OriginalSource is ListViewItem)
                            {
                                IBrowsableObjectInfoCollectionViewModel paths = getPaths<ExplorerControlListViewItem>(getDataContext);
                                IList<IExplorerControlViewModel> dataContext = paths.Paths;

                                foreach (IBrowsableObjectInfoViewModel item in paths.SelectedItem.SelectedItems.Where(path => path.IsBrowsable()))

                                    dataContext.Add(ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel(item));
                            }

                            else if (e.OriginalSource is TabItem tabItem)

                                getPaths<TabItem>(_getDataContext).Paths.Remove((IExplorerControlViewModel)tabItem.DataContext);
                        }
                    }));

            RegisterClassHandler(ExplorerControlListViewItem.ContextMenuRequestedEvent, new ContextMenuRequestedEventHandler((object sender, ContextMenuRequestedEventArgs e) =>
           {
               var dataContext = getDataContext(e, out ExplorerControlListViewItem
#if CS8
                ?
#endif
                _sender, out BrowsableObjectInfoWindow
#if CS8
                ?
#endif
                window);

               if (dataContext == null)

                   return;

               IExplorerControlViewModel parent = dataContext.Paths.SelectedItem;

               IBrowsableObjectInfoViewModel[] selectedItems = new Collections.Generic.ArrayBuilder<IBrowsableObjectInfoViewModel>(parent.SelectedItems).ToArray();

               IBrowsableObjectInfoViewModel parentPath = parent.Path;

               void process(IBrowsableObjectInfoViewModel path, in bool renameVerb, in PredicateIn<IBrowsableObjectInfo> browsabilityVerbs)
               {
                   if (path.ContextCommands != null)

                       return;

                   var hwnd = new WindowInteropHelper(window).Handle;

                   System.Drawing.Point getPoint()
                   {
                       System.Windows.Point _point = _sender.PointToScreen(new System.Windows.Point(0, 0));

                       return new System.Drawing.Point((int)_point.X, (int)_point.Y);
                   }

                   System.Drawing.Point point = getPoint();

                   bool extendedVerbs = e.Shift;

                   IContextMenu contextMenu = renameVerb ? path.GetContextMenu(extendedVerbs) : parentPath.GetContextMenu(selectedItems.Select(obj => obj.Model), extendedVerbs);

                   if (contextMenu == null)

                       return;

#if CS8
                   static
#endif
                   MenuItemInfo getMenuItemInfoBase() => new MenuItemInfo() { cbSize = (uint)Marshal.SizeOf<MenuItemInfo>() };

#if CS8
                   static
#endif
                   MenuItemInfo getMenuItemInfo(in string header, in Bitmap bitmap)
                   {
                       MenuItemInfo menuItemInfo = getMenuItemInfoBase();

                       menuItemInfo.fMask = MenuItemInfoFlags.String | MenuItemInfoFlags.Bitmap;
                       menuItemInfo.dwTypeData = header;
                       menuItemInfo.hbmpItem = bitmap.GetHbitmap();
                       menuItemInfo.cch = (uint)menuItemInfo.dwTypeData.Length;

                       return menuItemInfo;
                   }

                   FuncRef<uint?, string> func;

                   string getCommandTooltip(ref uint? command) => contextMenu.GetCommandTooltip(ref command);

                   Func<ContextMenuCommand> showMenuFunc;

                   ContextMenuCommand showMenu() => contextMenu.Show(hwnd, new HookRegistration(hwnd, func, window).InnerStruct, point, e.Ctrl, e.Shift);

                   if (browsabilityVerbs(path))
                   {
                       contextMenu.AddCommands(new MenuItemInfo[] { getMenuItemInfo("&Open in WinCopies", folder), getMenuItemInfo("Open in new &tab", tab_add), getMenuItemInfo("Open in new &window", application_add) });

                       func = getCommandTooltip;

                       showMenuFunc = showMenu;
                   }

                   else
                   {
                       func = (ref uint? _command) =>
                       {
                           string _result = getCommandTooltip(ref _command);

                           if (_result == null && _command.HasValue)

                               _command = _command.Value + (uint)ContextMenuCommand.LastDelegatedCommand;

                           return _result;
                       };

                       showMenuFunc = () =>
                       {
                           ContextMenuCommand _result = showMenu();

                           if (_result > ContextMenuCommand.LastDelegatedCommand)

                               _result += (sbyte)ContextMenuCommand.LastDelegatedCommand;

                           return _result;
                       };
                   }

                   if (renameVerb)
                   {
#if CS8
                       static
#endif
                       KeyValuePair<ExtensionCommand, MenuItemInfo> getKeyValuePair(in ExtensionCommand extensionCommand, in string header, in Bitmap bitmap) => new KeyValuePair<ExtensionCommand, MenuItemInfo>(extensionCommand, getMenuItemInfo(header, bitmap));

                       contextMenu.AddExtensionCommands(new KeyValuePair<ExtensionCommand, MenuItemInfo>[] { getKeyValuePair(ExtensionCommand.CopyPath, "Copy path", folder_edit), getKeyValuePair(ExtensionCommand.CopyName, "Copy name", textfield) });
                   }

                   ContextMenuCommand result = showMenuFunc();

                   void openInNewTab(in IBrowsableObjectInfoViewModel _path, in bool selected = true) => _openInNewTab(dataContext, _path, selected);

                   void open()
                   {
                       IBrowsableObjectInfo[] items = new Collections.Generic.ArrayBuilder<IBrowsableObjectInfo>(selectedItems.Where(item => !item.IsBrowsable())).ToArray();

                       if (items.Length > 0)

                           contextMenu.Open(items, point, e.Ctrl, e.Shift);
                   }

                   void openSelectedItemsInNewTabs(in ActionIn<IBrowsableObjectInfoViewModel> firstAction)
                   {
                       int i = 0;

                       for (; i < selectedItems.Length; i++)

                           if ((path = selectedItems[i]).IsBrowsable())
                           {
                               firstAction(path);

                               break;
                           }

                       for (i++; i < selectedItems.Length; i++)
                       {
                           if ((path = selectedItems[i]).IsBrowsable())

                               openInNewTab(path, false);
                       }

                       open();
                   }

                   switch (result)
                   {
                       case ContextMenuCommand.None:

                           break;

                       case ContextMenuCommand.NewFolder:

                           break;

                       case ContextMenuCommand.Rename:

                           if (renameVerb)
                           {
                               IProcessCommand renameItemProcessCommand = parentPath.ItemSources?.SelectedItem.ProcessSettings?.ProcessFactory.RenameItemProcessCommand;

                               if (InputBox.ShowDialog(renameItemProcessCommand.Name, DialogButton.OKCancel, renameItemProcessCommand.Caption, path.Name, null, out string _result, (path.IsLocalRoot == true ? drive_rename : textfield_rename).ToImageSource()) == true && !renameItemProcessCommand.TryExecute(_result, Enumerable.Repeat(path, 1), out _))

                                   _ = MessageBox.Show("An error occurred while renaming the selected item.", "Error", MessageBoxButton.OK, Error);
                           }

                           break;

                       case ContextMenuCommand.Delete:

                           RunCommand(window.OnRecycle, null);

                           break;

                       case ContextMenuCommand.Open:

                           if (renameVerb)

                               parent.Path = path;

                           else

                               openSelectedItemsInNewTabs((in IBrowsableObjectInfoViewModel obj) => parent.Path = obj);

                           break;

                       case ContextMenuCommand.OpenInNewTab:

                           if (renameVerb)

                               openInNewTab(path);

                           else

                               openSelectedItemsInNewTabs((in IBrowsableObjectInfoViewModel obj) => openInNewTab(obj));

                           break;

                       case ContextMenuCommand.OpenInNewWindow:

                           foreach (IBrowsableObjectInfoViewModel item in selectedItems.Where(item => item.IsBrowsable()))

                               window.GetNewBrowsableObjectInfoWindow(new BrowsableObjectInfoWindowViewModel(window.GetDefaultBrowsableObjectInfoCollection(ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel(item))));

                           open();

                           break;

                       case ContextMenuCommand.CopyName:

                           SetText(path.Name);

                           break;

                       case ContextMenuCommand.CopyPath:

                           SetText(path.Path);

                           break;
                   }
               }

               if (selectedItems.Length == 1)

                   process(selectedItems[0], true, (in IBrowsableObjectInfo selectedItem) => selectedItem.IsBrowsable());

               else

                   process(parentPath, false, (in IBrowsableObjectInfo selectedItem) =>
                   {
                       for (int i = 0; i < selectedItems.Length; i++)

                           if (selectedItems[i].IsBrowsable())

                               return true;

                       return false;
                   });
           }));

            /*EventManager.RegisterClassHandler(typeof(BrowsableObjectInfoWindow), ExplorerControlListView.ContextMenuRequestedEvent, new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                var listView = (ExplorerControlListView)e.OriginalSource;

                BrowsableObjectInfoWindow window = listView.GetParent<BrowsableObjectInfoWindow>(false);

                IExplorerControlViewModel selectedItem = ((BrowsableObjectInfoWindowViewModel)window.DataContext).Paths.SelectedItem;

                if (selectedItem.SelectedItems == null || selectedItem.SelectedItems.Count != 1)

                    return;

                if (selectedItem.SelectedItems[0].InnerObject is ShellObject shellObject)
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
            }));*/
        }

        protected BrowsableObjectInfoWindow(IBrowsableObjectInfoWindowViewModel
#if CS8
            ?
#endif
            dataContext = null)
        {
            /*dataContext.HookRegistration = new HookRegistration(hook =>
    {
    _hook = (IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) => hook((WindowMessage)msg, wParam, lParam, ref handled);

    HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(_hook);
    }, hook => HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).RemoveHook(_hook));*/

            SetResourceReference(StyleProperty, typeof(BrowsableObjectInfoWindow));

            ViewModel = (dataContext
#if CS8
                ??=
#else
                ?? (dataContext =
#endif
                GetDefaultDataContext()
#if !CS8
                )
#endif
                );

            if (dataContext.Paths.Paths.Count == 0)

                dataContext.Paths.Paths.Add(GetDefaultExplorerControlViewModel());

            AddDefaultCommandBindings();
        }

        private static void RegisterClassHandler(in RoutedEvent routedEvent, in Delegate handler) => Util.Desktop.UtilHelpers.RegisterClassHandler<BrowsableObjectInfoWindow>(routedEvent, handler);

        //private System.Windows.Interop.HwndSourceHook _hook;

        private static RoutedUICommand GetRoutedCommand(in string text, in string name) => new
#if !CS9
            RoutedUICommand
#endif
            (text, name, typeof(BrowsableObjectInfoWindow));

        protected abstract IBrowsableObjectInfoWindowViewModel GetDefaultDataContextOverride();

        public IBrowsableObjectInfoWindowViewModel GetDefaultDataContext()
        {
            IBrowsableObjectInfoWindowViewModel dataContext = GetDefaultDataContextOverride();

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

        protected virtual bool OnCanCancelClose(IList<IExplorerControlViewModel> paths) => /* !Current.IsClosing && */ paths.Count > 1 && MessageBox.Show(this, IOResources.WindowClosingMessage, "WinCopies", YesNo, Question, No) != Yes;

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

        protected IEnumerable<IBrowsableObjectInfo> GetEnumerable() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.Path.ItemSources?.SelectedItem.Items?.WhereSelect(item => item.IsSelected, item => item.Model);

        private void CanRunCommand(in IProcessFactoryProcessInfo
#if CS8
            ?
#endif
            processFactory, in CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = processFactory?.CanRun(GetEnumerable()) == true;

            e.Handled = true;
        }

        private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>

            // todo:
            //e.CanExecute = new EmptyCheckEnumerator<IBrowsableObjectInfoViewModel>(((MainWindowViewModel)DataContext).SelectedItem.Path.Items.Where(item => item.IsSelected).GetEnumerator()).HasItems;

            CanRunCommand(GetProcessFactory()?.Copy, e);

        protected IProcessFactory GetProcessFactory() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.Path.ItemSources?.SelectedItem.ProcessSettings?.ProcessFactory;

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
            e.CanExecute = GetProcessFactory()?.CanPaste(10u) == true;

            e.Handled = true;
        }

        protected abstract void OnPaste(ExecutedRoutedEventArgs e); // => StartInstance(GetProcessFactory().Copy.TryGetProcessParameters(10u));

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnPaste, e);

        private void Recycle_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory()?.Recycling, e);

        protected abstract void OnRecycle(ExecutedRoutedEventArgs e); // => StartInstance(GetProcessFactory().Recycling.TryGetProcessParameters(GetEnumerable()));

        private void Recycle_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnRecycle, e);

        private void Empty_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory()?.Clearing, e);

        protected abstract void OnEmpty(ExecutedRoutedEventArgs e); // => StartInstance(GetProcessFactory().Clearing.TryGetProcessParameters(GetEnumerable()));

        private void Empty_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnEmpty, e);

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory()?.Deletion, e);

        protected abstract void OnDelete(ExecutedRoutedEventArgs e); // => StartInstance(GetProcessFactory().Deletion.TryGetProcessParameters(GetEnumerable()));

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(OnDelete, e);

        private static InvalidOperationException GetInvalidParameterException() => new
#if !CS9
            InvalidOperationException
#endif
            ("The given parameter is not valid.");

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

            void enqueue(in TitleBarMenuItem item)
            {
                item.Icon.MakeTransparent();

                menuItems.Enqueue(item);
            }

            enqueue(new TitleBarMenuItem() { Command = NewTab, Icon = tab_add });
            enqueue(new TitleBarMenuItem() { Command = CloseTab, CommandParameter = new Func(() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem), Icon = tab_delete });

            TitleBarMenuItems = menuItems;
        }

        private void Window_PreviousCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.History.CanMoveBack;

            e.Handled = true;
        }

        private void Window_NextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.History.CanMoveForward;

            e.Handled = true;
        }

        private void Window_BrowseToParentCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.BrowseToParent.CanExecute(null);

            e.Handled = true;
        }

        private void Window_PreviousExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.History.MoveBack();

            CommandManager.InvalidateRequerySuggested();

            e.Handled = true;
        }

        private void Window_NextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.History.MoveForward();

            CommandManager.InvalidateRequerySuggested();

            e.Handled = true;
        }

        private void Window_BrowseToParentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _ = ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem.BrowseToParent.TryExecute(null);

            CommandManager.InvalidateRequerySuggested();

            e.Handled = true;
        }

        protected virtual void AddDefaultCommandBindings()
        {
            void add(in ICommand command, in ExecutedRoutedEventHandler executedRoutedEventHandler, in CanExecuteRoutedEventHandler canExecuteRoutedEventHandler) => CommandBindings.Add(new CommandBinding(command, executedRoutedEventHandler, canExecuteRoutedEventHandler));

            add(NavigationCommands.BrowseBack, Window_PreviousExecuted, Window_PreviousCanExecute);
            add(NavigationCommands.BrowseForward, Window_NextExecuted, Window_NextCanExecute);
            add(Commands.NavigationCommands.BrowseToParent, Window_BrowseToParentExecuted, Window_BrowseToParentCanExecute);

            #region File
            #region New
            add(NewTab, NewTab_Executed, Command_CanExecute);
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

        protected IList GetSelectedItems() => GetCurrentExplorerControlViewModel().SelectedItems;

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
                IList selectedItems = new ArrayList(_selectedItems.Count);

                foreach (object item in _selectedItems)

                    _ = selectedItems.Add(item);

                Action<IBrowsableObjectInfoViewModel> action;

                void _openItem(IBrowsableObjectInfoViewModel browsableObjectInfo)
                {
                    IBrowsableObjectInfoViewModel item = ExplorerControlViewModel.GetBrowsableObjectInfoOrLaunchItem(browsableObjectInfo);

                    if (item != null)

                        AddNewDefaultTab(IO.ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel(item), false);
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

        public bool AreSelectedItemsBrowsableByDefault(out IEnumerable<IBrowsableObjectInfoViewModel> selectedItems)
        {
            IList _selectedItems = GetSelectedItems();

            selectedItems = _selectedItems.OfType<IBrowsableObjectInfoViewModel>();

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

        protected void OpenInNewTabOrWindow(ActionIn<IExplorerControlViewModel, bool> action) => OpenInNewTabOrWindow((in IBrowsableObjectInfoViewModel item) => action(IO.ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel(item), false));

        private void OpenInNewTab_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() => OpenInNewTabOrWindow(AddNewDefaultTab), e);

        private void OpenInNewWindow_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() =>
        {
            IBrowsableObjectInfoCollectionViewModel items;

            ActionIn<IExplorerControlViewModel> action = addItem;

            void _addItem(in IExplorerControlViewModel item) => items.Paths.Add(item);

            void addItem(in IExplorerControlViewModel item)
            {
                items = GetDefaultBrowsableObjectInfoCollection();

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

        protected abstract IBrowsableObjectInfoCollectionViewModel GetDefaultBrowsableObjectInfoCollection();
        protected abstract IBrowsableObjectInfoCollectionViewModel GetDefaultBrowsableObjectInfoCollection(in IEnumerable<IExplorerControlViewModel> items);
        protected IBrowsableObjectInfoCollectionViewModel GetDefaultBrowsableObjectInfoCollection(params IExplorerControlViewModel[] items) => GetDefaultBrowsableObjectInfoCollection(items.AsEnumerable());

        public virtual IExplorerControlViewModel GetDefaultExplorerControlViewModel(in bool selected = false) => ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel(ViewModel.Paths.GetDefaultBrowsableObjectInfo(), selected);

        private void NewTab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlViewModel(), e);

        protected abstract BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow();
        protected abstract BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow(in IBrowsableObjectInfoWindowViewModel dataContext);

        private void NewWindow_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() => GetNewBrowsableObjectInfoWindow().Show(), e);

        private void CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.Paths.Paths.Count > 1;

            e.Handled = true;
        }

        private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() => ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths.Remove((IExplorerControlViewModel)(e.Parameter is Func func ? func() : e.Parameter)), e);

        private void CloseOtherTabs_Executed(object sender, ExecutedRoutedEventArgs e) => TryExecuteRoutedCommand(() => RemoveAll(((BrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths, ((BrowsableObjectInfoWindowViewModel)DataContext).Paths.SelectedItem, false, false), e);
    }

    public abstract class BrowsableObjectInfoWindow2 : BrowsableObjectInfoWindow
    {
        protected BrowsableObjectInfoWindow2(in IBrowsableObjectInfoWindowViewModel
#if CS8
            ?
#endif
            dataContext = null) : base(dataContext) { /* Left empty. */ }

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => GetNewBrowsableObjectInfoWindow(GetDefaultDataContext());
    }
}
