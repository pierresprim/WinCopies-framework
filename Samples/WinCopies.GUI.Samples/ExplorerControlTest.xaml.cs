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

using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.Samples
{
    /// <summary>
    /// Interaction logic for ExplorerControlTest.xaml
    /// </summary>
    public partial class ExplorerControlTest : Window
    {
        private static ClientVersion GetClientVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            return new ClientVersion("WinCopies Framework Test App", (uint)version.Major, (uint)version.Minor, (uint)version.Revision);
        }

        public static ClientVersion ClientVersion { get; } = GetClientVersion();

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IEnumerable<IExplorerControlBrowsableObjectInfoViewModel>), typeof(ExplorerControlTest));

        public IEnumerable<IExplorerControlBrowsableObjectInfoViewModel> Items { get => (IEnumerable<IExplorerControlBrowsableObjectInfoViewModel>)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        public ExplorerControlTest()
        {
            InitializeComponent();

            DataContext = this;
        }

        public static ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> GetShellItems() => new ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>() { { GetExplorerControlBrowsableObjectInfoViewModel(GetBrowsableObjectInfoViewModel(ShellObjectInfo.From(ShellObjectFactory.Create("C:\\"), ClientVersion)), true, SelectionMode.Extended, true) } };

        private static IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) => new BrowsableObjectInfoViewModel(browsableObjectInfo) { Factory = new BrowsableObjectInfoFactory(ClientVersion) { SortComparison = BrowsableObjectInfoViewModel.DefaultComparison }, SortComparison = BrowsableObjectInfoViewModel.DefaultComparison };

        public static IExplorerControlBrowsableObjectInfoViewModel GetExplorerControlBrowsableObjectInfoViewModel(in IBrowsableObjectInfoViewModel browsableObjectInfo, in bool isSelected, in SelectionMode selectionMode, in bool isCheckBoxVisible)
        {
            IExplorerControlBrowsableObjectInfoViewModel result = ExplorerControlBrowsableObjectInfoViewModel.From(browsableObjectInfo);

            result.IsSelected = isSelected;
            result.SelectionMode = selectionMode;
            result.IsCheckBoxVisible = isCheckBoxVisible;

            return result;
        }

        public static ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> GetRegistryItems() => new ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>() { { GetExplorerControlBrowsableObjectInfoViewModel(new BrowsableObjectInfoViewModel(new RegistryItemInfo()), true, SelectionMode.Extended, true) } };

        public static ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> GetWMIItems() => new ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>() { { GetExplorerControlBrowsableObjectInfoViewModel(new BrowsableObjectInfoViewModel(new WMIItemInfo()), true, SelectionMode.Extended, true) } };

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
        }
    }
}
