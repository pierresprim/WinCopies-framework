/* Copyright © Pierre Sprimont, 2019
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.GUI.IO;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.GUI.Windows;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.Linq;

namespace WinCopies.GUI.Samples
{
    /// <summary>
    /// Interaction logic for ExplorerControlTest.xaml
    /// </summary>
    public partial class ExplorerControlTest : Windows.Window
    {
        private static Windows.Window _processWindow;

        public static IProcessPathCollectionFactory DefaultProcessPathCollectionFactory { get; } = new ProcessPathCollectionFactory();

        public static WinCopies.IO.ClientVersion ClientVersion { get; } = GetClientVersion();

        public static Windows.Window ProcessWindow
        {
            get
            {
                if (_processWindow == null)
                {
                    _processWindow = new Windows.Window() { ContentTemplateSelector = new InterfaceDataTemplateSelector(), Content = new ProcessManager<IProcess>() { Processes = new ObservableCollection<IProcess>() } };

                    _processWindow.Show();
                }

                return _processWindow;
            }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>), typeof(ExplorerControlTest), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            if (e.NewValue != null)

                foreach (IExplorerControlBrowsableObjectInfoViewModel item in (System.Collections.Generic.IEnumerable<IExplorerControlBrowsableObjectInfoViewModel>)e.NewValue)

                    AddHandlers(item);
        }));

        public ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> Items { get => (ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(IExplorerControlBrowsableObjectInfoViewModel), typeof(ExplorerControlTest));

        public IExplorerControlBrowsableObjectInfoViewModel SelectedItem { get => (IExplorerControlBrowsableObjectInfoViewModel)GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        static ExplorerControlTest() => Shell.ObjectModel.BrowsableObjectInfo.RegisterDefaultSelectors();

        public ExplorerControlTest()
        {
            InitializeComponent();

            DataContext = this;

            void addCommandBinding(in ICommand command, Func<IProcessFactoryProcessInfo> getProcessInfo) => CommandBindings.Add(new CommandBinding(command, (object sender, ExecutedRoutedEventArgs e) => StartProcess(getProcessInfo()), (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = getProcessInfo().CanRun(GetEnumerable())));

            IProcessFactory getProcessFactory() => SelectedItem.Path.ProcessFactory;

            addCommandBinding(ApplicationCommands.Copy, () => getProcessFactory().Copy);

            addCommandBinding(ApplicationCommands.Cut, () => getProcessFactory().Cut);

            _ = CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, (object sender, ExecutedRoutedEventArgs e) => AddProcess(getProcessFactory().Copy.TryGetProcessParameters(10u)), (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = SelectedItem.Path.ProcessFactory.CanPaste(10u)));

            addCommandBinding(ApplicationCommands.Delete, () => getProcessFactory().Recycling);

            addCommandBinding(Commands.ApplicationCommands.Empty, () => getProcessFactory().Clearing);

            addCommandBinding(Commands.ApplicationCommands.DeletePermanently, () => getProcessFactory().Deletion);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var menuItems = new TitleBarMenuItemQueue();

            menuItems.Enqueue(new TitleBarMenuItem() { Header = "New tab", Command = Commands.ApplicationCommands.NewTab });

            menuItems.Enqueue(new TitleBarMenuItem() { Header = "Close tab", Command = Commands.ApplicationCommands.CloseTab, CommandParameter = SelectedItem });

            TitleBarMenuItems = menuItems;
        }

        private IEnumerable<IBrowsableObjectInfo> GetEnumerable() => SelectedItem.Path.Items.WhereSelect(item => item.IsSelected, item => item.Model);

        protected void StartProcess(in IProcessFactoryProcessInfo processInfo)
        {
            if (processInfo.UserConfirmationRequired)

                if (MessageBox.Show(processInfo.GetUserConfirmationText(), Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.YesNo, MessageBoxImage.Question, System.Windows.MessageBoxResult.No) == System.Windows.MessageBoxResult.No)

                    return;

            if (processInfo is IRunnableProcessInfo runnableProcessInfo)

                runnableProcessInfo.Run(GetEnumerable(), 10u);

            else

                AddProcess(((IDirectProcessInfo)processInfo).TryGetProcessParameters(GetEnumerable()));
        }

        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void AddNewTab(IExplorerControlBrowsableObjectInfoViewModel viewModel, ExecutedRoutedEventArgs e)
        {
            viewModel.IsSelected = true;

            Items.Add(viewModel);

            e.Handled = true;
        }

        public static IExplorerControlBrowsableObjectInfoViewModel GetDefaultExplorerControlBrowsableObjectInfoViewModel() => GetDefaultExplorerControlBrowsableObjectInfoViewModel(new ShellObjectInfo(KnownFolders.Desktop, ClientVersion));

        public static IExplorerControlBrowsableObjectInfoViewModel GetDefaultExplorerControlBrowsableObjectInfoViewModel(in IBrowsableObjectInfo browsableObjectInfo)
        {
            IExplorerControlBrowsableObjectInfoViewModel viewModel = Shell.ObjectModel.ExplorerControlBrowsableObjectInfoViewModel.From(new BrowsableObjectInfoViewModel(browsableObjectInfo), path => ShellObjectInfo.From(ShellObjectFactory.Create(path), ClientVersion));

            return viewModel;
        }

        private void NewTab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlBrowsableObjectInfoViewModel(), e);

        private void CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Items.Count > 1;

            e.Handled = true;
        }

        private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = Items.Remove((IExplorerControlBrowsableObjectInfoViewModel)(e.Parameter is Func func ? func() : e.Parameter));

            e.Handled = true;
        }

        private static void AddProcess(in IProcessParameters parameters)
        {
            IProcess result = new Process(BrowsableObjectInfo.DefaultProcessSelectorDictionary.Select(new ProcessFactorySelectorDictionaryParameters(parameters, DefaultProcessPathCollectionFactory)));

            ((ProcessManager<IProcess>)ProcessWindow.Content).Processes.Add(result);

            result.RunWorkerAsync();
        }

        private static WinCopies.IO.ClientVersion GetClientVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            return new WinCopies.IO.ClientVersion("WinCopies Framework Test App", (uint)version.Major, (uint)version.Minor, (uint)version.Revision);
        }

        public static ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> GetShellItems() => new
#if !CS9
            ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>
#endif
            ()
        { { GetExplorerControlBrowsableObjectInfoViewModel(GetBrowsableObjectInfoViewModel(ShellObjectInfo.From(ShellObjectFactory.Create("C:\\"), ClientVersion)), true, SelectionMode.Extended, true) } };

        private static IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) => new BrowsableObjectInfoViewModel(browsableObjectInfo);

        public static IExplorerControlBrowsableObjectInfoViewModel GetExplorerControlBrowsableObjectInfoViewModel(in IBrowsableObjectInfoViewModel browsableObjectInfo, in bool isSelected, in SelectionMode selectionMode, in bool isCheckBoxVisible)
        {
            IExplorerControlBrowsableObjectInfoViewModel result = Shell.ObjectModel.ExplorerControlBrowsableObjectInfoViewModel.From(browsableObjectInfo, path => ShellObjectInfo.From(ShellObjectFactory.Create(path), ClientVersion));

            result.IsSelected = isSelected;
            result.SelectionMode = selectionMode;
            result.IsCheckBoxVisible = isCheckBoxVisible;

            return result;
        }

        public static ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> GetRegistryItems() => new
#if !CS9
            ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>
#endif
            ()
        { { GetExplorerControlBrowsableObjectInfoViewModel(new BrowsableObjectInfoViewModel(new RegistryItemInfo()), true, SelectionMode.Extended, true) } };

        public static ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> GetWMIItems() => new
#if !CS9
            ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>
#endif
            ()
        { { GetExplorerControlBrowsableObjectInfoViewModel(new BrowsableObjectInfoViewModel(new WMIItemInfo()), true, SelectionMode.Extended, true) } };

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string radioButton = (string)((RadioButton)e.Source).Content;

            switch (radioButton)
            {
                case "Shell":

                    Items = GetShellItems();

                    break;

                case "Registry":

                    Items = GetRegistryItems();

                    break;

                case "WMI":

                    Items = GetWMIItems();

                    break;
            }

            Items.CollectionChanged += Items_CollectionChanged;
        }

        private static void AddHandlers(in IExplorerControlBrowsableObjectInfoViewModel item) => item.CustomProcessParametersGeneratedEventHandler += ExplorerControlTest_CustomProcessParametersGeneratedEventHandler;

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:

                    foreach (object _item in e.NewItems)

                        AddHandlers((IExplorerControlBrowsableObjectInfoViewModel)_item);

                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:

                    IExplorerControlBrowsableObjectInfoViewModel item;

                    foreach (object _item in e.OldItems)
                    {
                        item = (IExplorerControlBrowsableObjectInfoViewModel)_item;

                        item.CustomProcessParametersGeneratedEventHandler -= ExplorerControlTest_CustomProcessParametersGeneratedEventHandler;

                        item.Dispose();
                    }

                    break;
            }
        }

        private static void ExplorerControlTest_CustomProcessParametersGeneratedEventHandler(object sender, CustomProcessParametersGeneratedEventArgs e) => AddProcess(e.ProcessParameters);

        protected override void OnClosing(CancelEventArgs e)
        {
            ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> items = Items;

            items.CollectionChanged -= Items_CollectionChanged;

            Items = null;

            items.Clear();

            base.OnClosing(e);
        }

        private void Window_PreviousCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedItem.History.CanMovePreviousFromCurrent;

            e.Handled = true;
        }

        private void Window_NextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedItem.History.CanMoveNextFromCurrent;

            e.Handled = true;
        }

        private void Window_PreviousExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedItem.History.CurrentIndex++;

            CommandManager.InvalidateRequerySuggested();

            e.Handled = true;
        }

        private void Window_NextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedItem.History.CurrentIndex--;

            CommandManager.InvalidateRequerySuggested();

            e.Handled = true;
        }
    }
}
