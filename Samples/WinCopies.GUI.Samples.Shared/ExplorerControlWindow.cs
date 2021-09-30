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

using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using WinCopies.GUI.IO.Process;
using WinCopies.GUI.Shell;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;

namespace WinCopies.GUI.Samples
{
    public partial class ExplorerControlWindow : BrowsableObjectInfoWindow
    {
        private static Windows.Window _processWindow;

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

        public static IProcessPathCollectionFactory DefaultProcessPathCollectionFactory { get; } = new ProcessPathCollectionFactory();

        public ExplorerControlWindow() : base(GetDefaultDataContext()) { /* Left empty. */ }

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => new ExplorerControlWindow();

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

        private static void AddProcess(in IProcessParameters parameters)
        {
            IProcess result = new Process(BrowsableObjectInfo.DefaultProcessSelectorDictionary.Select(new ProcessFactorySelectorDictionaryParameters(parameters, DefaultProcessPathCollectionFactory)));

            ((ProcessManager<IProcess>)ProcessWindow.Content).Processes.Add(result);

            result.RunWorkerAsync();
        }

        protected override void OnAboutWindowRequested() { }

        protected override void OnDelete() { }

        protected override void OnEmpty() { }

        protected override void OnPaste() => GetProcessFactory().Copy.TryGetProcessParameters(10u);

        protected override void OnQuit() { }

        protected override void OnRecycle() { }

        protected override void OnSubmitABug()
        {
            string url = "https://github.com/pierresprim/WinCopies/issues";

            _ = UtilHelpers.StartProcessNetCore(url);
        }
    }
}
