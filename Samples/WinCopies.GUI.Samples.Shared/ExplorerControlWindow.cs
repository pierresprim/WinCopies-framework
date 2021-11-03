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

using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.GUI.Shell;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;

namespace WinCopies.GUI.Samples
{
    public class ProcessWindow : IO.Process.ProcessWindow
    {
        protected override NotificationIconData GetNotificationIconData() => new(Properties.Resources.WinCopies, "WinCopies");

        protected override bool ValidateClosing() => true;
    }

    public partial class ExplorerControlWindow : BrowsableObjectInfoWindow2
    {
        private static ProcessWindow _processWindow;

        public static ProcessWindow ProcessWindow
        {
            get
            {
                if (_processWindow == null)

                    (_processWindow = new ProcessWindow()).Show();

                return _processWindow;
            }
        }

        public static IProcessPathCollectionFactory DefaultProcessPathCollectionFactory { get; } = new ProcessPathCollectionFactory();

        public ExplorerControlWindow() : base(GetDefaultDataContext())
        {
            GetPathCollection().CollectionChanged += ExplorerControlWindow_CollectionChanged;

            var paths = GetPathCollection();

            foreach (IExplorerControlViewModel item in paths)

                AddEventHandler(item);
        }

        public ExplorerControlWindow(in IBrowsableObjectInfoWindowViewModel dataContext) : base(dataContext) { /* Left empty. */ }

        private void AddEventHandler(in IExplorerControlViewModel item) => item.CustomProcessParametersGenerated += ExplorerControlWindow_CustomProcessParametersGeneratedEventHandler;

        private void ExplorerControlWindow_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)

                foreach (object item in e.NewItems)

                    AddEventHandler((IExplorerControlViewModel)item);

            if (e.OldItems != null)

                foreach (object item in e.OldItems)

                    ((IExplorerControlViewModel)item).CustomProcessParametersGenerated -= ExplorerControlWindow_CustomProcessParametersGeneratedEventHandler;
        }

        private void ExplorerControlWindow_CustomProcessParametersGeneratedEventHandler(object sender, IO.CustomProcessParametersGeneratedEventArgs e) => AddProcess(e.Process, f => e.ProcessParameters);

        public ObservableCollection<IExplorerControlViewModel> GetPathCollection() => (ObservableCollection<IExplorerControlViewModel>)((IBrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths;

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => new ExplorerControlWindow();

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow(in IBrowsableObjectInfoWindowViewModel dataContext) => new ExplorerControlWindow(dataContext);

        //protected void StartProcess(in IProcessFactoryProcessInfo processInfo)
        //{
        //    if (processInfo.UserConfirmationRequired)

        //        if (MessageBox.Show(processInfo.GetUserConfirmationText(), Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)

        //            return;

        //    if (processInfo is IRunnableProcessInfo runnableProcessInfo)

        //        runnableProcessInfo.Run(GetEnumerable(), 10u);

        //    else

        //        AddProcess(((IDirectProcessInfo)processInfo).TryGetProcessParameters(GetEnumerable()));
        //}

        private static void AddProcess<T>(in T factory, in Converter<T, IProcessParameters> parameters) where T : IProcessFactoryProcessInfoBase
        {
            if (!(factory.UserConfirmationRequired && MessageBox.Show(factory.GetUserConfirmationText(), "Process confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No))
            {
                IProcess result = new Process(BrowsableObjectInfo.DefaultProcessSelectorDictionary.Select(new ProcessFactorySelectorDictionaryParameters(parameters(factory), DefaultProcessPathCollectionFactory)));

                ((ProcessManager<IProcess>)ProcessWindow.Content).Processes.Add(result);
            }
        }

        private void AddProcess<T>(in Converter<IProcessFactory, T> func1, in Converter<T, IProcessParameters> func2) where T : IProcessFactoryProcessInfo => AddProcess(func1(GetProcessFactory()), func2);

        private void AddProcess(Converter<IProcessFactory, IDirectProcessInfo> func) => AddProcess(f => func(f), f => f.TryGetProcessParameters(GetEnumerable()));

        protected override void OnAboutWindowRequested(ExecutedRoutedEventArgs e) { }

        protected override void OnDelete(ExecutedRoutedEventArgs e) => AddProcess(f => f.Deletion);

        protected override void OnEmpty(ExecutedRoutedEventArgs e) => AddProcess(f => f.Clearing);

        protected override void OnPaste(ExecutedRoutedEventArgs e) => AddProcess(f => f.Copy, f => f.TryGetProcessParameters(10u));

        protected override void OnQuit(ExecutedRoutedEventArgs e) { }

        protected override void OnRecycle(ExecutedRoutedEventArgs e) => AddProcess(f => f.Recycling);

        protected override void OnSubmitABug(ExecutedRoutedEventArgs e)
        {
            string url = "https://github.com/pierresprim/WinCopies/issues";

            _ = UtilHelpers.StartProcessNetCore(url);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            GetPathCollection().CollectionChanged -= ExplorerControlWindow_CollectionChanged;
        }
    }
}
