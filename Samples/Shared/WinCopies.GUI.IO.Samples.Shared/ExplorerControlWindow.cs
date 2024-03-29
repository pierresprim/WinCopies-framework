﻿/* Copyright © Pierre Sprimont, 2021
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
#region System
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
#endregion System

#region WinCopies
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
#endregion WinCopies
#endregion Usings

namespace WinCopies.GUI.IO.Samples
{
    public class ProcessWindow : Process.ProcessWindow
    {
        protected override NotificationIconData GetNotificationIconData() => new(Properties.Resources.WinCopies, "WinCopies");

        protected override bool ValidateClosing() => true;
    }

    public class PluginInfo : ObjectModel.PluginInfo
    {
        public PluginInfo(in IBrowsableObjectInfoPlugin plugin, in ClientVersion clientVersion) : base(plugin, clientVersion) { /* Left empty. */ }

        protected override ObjectModel.BrowsableObjectInfoStartPage GetBrowsableObjectInfoStartPage() => new BrowsableObjectInfoStartPage(ClientVersion);

        public override IBrowsableObjectInfo Clone() => new PluginInfo(InnerObjectGeneric, ClientVersion);
    }

    public class BrowsableObjectInfoStartPage : ObjectModel.BrowsableObjectInfoStartPage
    {
        public BrowsableObjectInfoStartPage(ClientVersion clientVersion) : base(App.Current.PluginParameters.Select(plugin => new PluginInfo(plugin, clientVersion)), clientVersion) { /* Left empty. */ }
        public BrowsableObjectInfoStartPage() : this(DefaultClientVersion) { /* Left empty. */ }

        public override IBrowsableObjectInfo Clone() => new BrowsableObjectInfoStartPage(ClientVersion);

        protected override Icon GetIcon() => Properties.Resources.WinCopies;
    }

    public class BrowsableObjectInfoCollectionViewModel : ObjectModel.BrowsableObjectInfoCollectionViewModel
    {
        public BrowsableObjectInfoCollectionViewModel() { /* Left empty. */ }

        public BrowsableObjectInfoCollectionViewModel(in IEnumerable<IExplorerControlViewModel> items) : base(items) { /* Left empty. */ }

        public BrowsableObjectInfoCollectionViewModel(params IExplorerControlViewModel[] items) : base(items.AsEnumerable()) { /* Left empty. */ }

        public override IBrowsableObjectInfo GetDefaultBrowsableObjectInfo() => new BrowsableObjectInfoStartPage();
    }

    public partial class ExplorerControlWindow : BrowsableObjectInfoWindow2
    {
        private static ProcessWindow? _processWindow;
        private ClientVersion? _clientVersion;

        public static ProcessWindow ProcessWindow
        {
            get
            {
                (_processWindow ??= new ProcessWindow()).Show();

                return _processWindow;
            }
        }

        public static IProcessPathCollectionFactory DefaultProcessPathCollectionFactory { get; } = new ProcessPathCollectionFactory();

        public override ClientVersion ClientVersion => _clientVersion ??= WinCopies.IO.ObjectModel.BrowsableObjectInfo.GetDefaultClientVersion();

        public ExplorerControlWindow(in IBrowsableObjectInfoWindowViewModel
#if CS8
            ?
#endif
            dataContext = null) : base(dataContext)
        {
            ObservableCollection<IExplorerControlViewModel> paths = GetPathCollection();

            paths.CollectionChanged += ExplorerControlWindow_CollectionChanged;

            foreach (IExplorerControlViewModel item in paths)

                AddEventHandler(item);
        }

        private void AddEventHandler(in IExplorerControlViewModel item) => item.CustomProcessParametersGenerated += ExplorerControlWindow_CustomProcessParametersGeneratedEventHandler;

        private void ExplorerControlWindow_CollectionChanged(object
#if CS8
            ?
#endif
            sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)

                foreach (object item in e.NewItems)

                    AddEventHandler((IExplorerControlViewModel)item);

            if (e.OldItems != null)

                foreach (object item in e.OldItems)

                    ((IExplorerControlViewModel)item).CustomProcessParametersGenerated -= ExplorerControlWindow_CustomProcessParametersGeneratedEventHandler;
        }

        private void ExplorerControlWindow_CustomProcessParametersGeneratedEventHandler(object
#if CS8
            ?
#endif
            sender, CustomProcessParametersGeneratedEventArgs e) => AddProcess(e.Process, f => e.ProcessParameters);

        public ObservableCollection<IExplorerControlViewModel> GetPathCollection() => (ObservableCollection<IExplorerControlViewModel>)ViewModel.Paths.Paths;

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => new ExplorerControlWindow();

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow(in IBrowsableObjectInfoWindowViewModel dataContext) => new ExplorerControlWindow(dataContext);

        private static void AddProcess<T>(in T factory, in Converter<T, IProcessParameters> parameters) where T : IProcessFactoryProcessInfoBase
        {
            if (!(factory.UserConfirmationRequired && MessageBox.Show(factory.GetUserConfirmationText(), "Process confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No))
            {
                IProcess result = new Process.Process(WinCopies.IO.ObjectModel.BrowsableObjectInfo.DefaultProcessSelectorDictionary.Select(new ProcessFactorySelectorDictionaryParameters(parameters(factory), DefaultProcessPathCollectionFactory)));

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

        protected override IBrowsableObjectInfoWindowViewModel GetDefaultDataContextOverride() => new BrowsableObjectInfoWindowViewModel(GetDefaultBrowsableObjectInfoCollection());
        protected override IBrowsableObjectInfoCollectionViewModel GetDefaultBrowsableObjectInfoCollection() => new BrowsableObjectInfoCollectionViewModel();
        protected override IBrowsableObjectInfoCollectionViewModel GetDefaultBrowsableObjectInfoCollection(in IEnumerable<IExplorerControlViewModel> items) => new BrowsableObjectInfoCollectionViewModel(items);
    }
}
