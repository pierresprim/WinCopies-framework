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

using System.Windows;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.Util.Data;

namespace WinCopies.GUI.IO.Samples
{
    public partial class MainWindow : Windows.Window
    {
        public MainWindow() => InitializeComponent();

        private void MenuItem_Click(object sender, RoutedEventArgs e) => new ExplorerControlWindow().Show();

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            static void showDialog(in FileSystemDialogBoxMode mode, in INamedObject<string>[]
#if CS8
                ?
#endif
                filters)
            {
                var dialog = new FileSystemDialogBox(new FileSystemDialog(mode, new BrowsableObjectInfoStartPage()));

                if (filters != null)

                    dialog.Dialog.Filters = filters;

                _ = dialog.ShowDialog();

                IExplorerControlViewModel path = dialog.Dialog.Path;

                switch (dialog.MessageBoxResult)
                {
                    case Windows.MessageBoxResult.OK:

                        _ = MessageBox.Show($"You've selected: {path.Path.Path}");

                        break;

                    case Windows.MessageBoxResult.Cancel:

                        _ = MessageBox.Show($"You've canceled the path selection.");

                        break;
                }

                path.Dispose();
            }

            showDialog(FileSystemDialogBoxMode.SelectFolder, null);

            var filters = new INamedObject<string>[] { new NamedObject<string>("Text file", "*.txt"), new NamedObject<string>("CS file", "*.cs") };

            showDialog(FileSystemDialogBoxMode.OpenFile, filters);

            showDialog(FileSystemDialogBoxMode.OpenFile, filters);
        }
    }
}
