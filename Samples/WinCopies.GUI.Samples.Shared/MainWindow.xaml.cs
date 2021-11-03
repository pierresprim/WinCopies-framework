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

using System;
using System.Windows;
using System.Windows.Input;

using WinCopies.Commands;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.Shell;
using WinCopies.GUI.Windows;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Windows.Window
    {
        private TitleBarMenuItem _menuItem;

        public MainWindow()
        {
            InitializeComponent();

            HelpButton = true;

            CloseButton = false;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var menuItems = new TitleBarMenuItemQueue();

            menuItems.Enqueue(_menuItem = new TitleBarMenuItem() { Header = "Menu 1" });

            _menuItem.Click += (object sender, EventArgs _e) => MessageBox.Show("You clicked on the menu item 1.");

            menuItems.Enqueue(new TitleBarMenuItem()
            {
                Header = "Menu 2",
                Command = new DelegateCommand(o => true, o =>
{
    _menuItem.IsEnabled = !_menuItem.IsEnabled;

    _ = MessageBox.Show($"The menu item 1 has been {(_menuItem.IsEnabled ? "enabled" : "disabled")}.");
})
            });

            menuItems.Enqueue(new TitleBarMenuItem()
            {
                Header = "Menu 3",
                Command = new DelegateCommand(o => true, o =>
                {
                    _menuItem.Header = _menuItem.Header == "Menu 1" ? "Alternative Menu 1 Header" : "Menu 1";

                    _ = MessageBox.Show("The menu item 1 header has been changed.");
                })
            });

            TitleBarMenuItems = menuItems;
        }

        private void Window_HelpButtonClick(object sender, RoutedEventArgs e) => _ = MessageBox.Show($"The window is currently in help mode: { IsInHelpMode }.");

        private void Button_Click(object sender, RoutedEventArgs e)
        {
#if CS8
            static
#endif
            void action(string s) => MessageBox.Show($"You clicked the Button{s}!");

            ICommand command = new DelegateCommand<string>(s => true, s => action(s));

            RoutedCommand routedCommand = new RoutedUICommand("ButtonCommand", "ButtonCommand", typeof(MainWindow));

            Func<DialogWindow>[] dialogWindows =
                {
                () => new DialogWindow() { Content = "1 - This is a sample DialogWindow.", HelpButton = true },
            () => new DialogWindow() { Content = "2 - This is a sample DialogWindow.", HelpButton = true },
            () => new DialogWindow() { Content = "3 - This is a sample DialogWindow.", ShowHelpButtonAsCommandButton = true },
            () => new DialogWindow() { Content = "4 - This is a sample DialogWindow.", ShowHelpButtonAsCommandButton = true },
            () => new DialogWindow() { Content = "5 - This is a sample DialogWindow.", DialogButton = DialogButton.YesNoCancel, DefaultButton = DefaultButton.Cancel, ShowHelpButtonAsCommandButton = true },
            () => new DialogWindow() { Content = "6 - This is a sample DialogWindow.", DialogButton = null, CustomButtonTemplateSelector = new AttributeDataTemplateSelector(), CustomButtonsSource = new ButtonModel[] { new ButtonModel("Button1") { CommandParameter = "1", Command = command }, new ButtonModel("Button2") { CommandParameter = "2", Command = command } } },
            () =>
            {
                var _dialogWindow = new DialogWindow() { Content = "7 - This is a sample DialogWindow.", DialogButton = null, CustomButtonTemplateSelector = new AttributeDataTemplateSelector(), CustomButtonsSource = new ButtonModel[] { new ButtonModel("Button1") { CommandParameter = "1", Command = routedCommand }, new ButtonModel("Button2") { CommandParameter = "2", Command = routedCommand } } };

                _ = _dialogWindow.CommandBindings.Add(new CommandBinding(routedCommand, (object _sender, ExecutedRoutedEventArgs _e) => MessageBox.Show($"You clicked the Button{ (string) _e.Parameter}!")));

                return _dialogWindow;

            }
        };

            int i = 0;

            DialogWindow dialogWindow = dialogWindows[0]();

            dialogWindow.Closed += (object _sender, EventArgs _e) => OnDialogWindowClosed(dialogWindows, i);

            dialogWindow.Show();
        }

        private void OnDialogWindowClosed(Func<DialogWindow>[] dialogWindows, int i)
        {
            i++;

            if (i == dialogWindows.Length)

                return;

            if (i % 2 == 0)
            {
                DialogWindow dialogWindow = dialogWindows[i]();

                dialogWindow.Closed += (object sender, EventArgs e) => OnDialogWindowClosed(dialogWindows, i);

                dialogWindow.Show();
            }

            else
            {
                DialogWindow dialogWindow = dialogWindows[i]();

                dialogWindow.Closed += (object sender, EventArgs e) => OnDialogWindowClosed(dialogWindows, i);

                _ = dialogWindow.ShowDialog();
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!CloseButton)
            {
                CloseButton = true;

                return;
            }

            Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) => new Window1().Show();

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) => new ExplorerControlWindow /*ExplorerControlTest*/().Show();

        public class NamedObject : NamedObject<string>
        {
            public NamedObject(in string name, in string value) : base(name, value) { /* Left empty. */ }

            public override string ToString() => Name;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            static void showDialog(in FileSystemDialogBoxMode mode, in INamedObject<string>[] filters)
            {
                var dialog = new FileSystemDialogBox(new FileSystemDialog(mode), true);

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

            var filters = new INamedObject<string>[] { new NamedObject("Text file", "*.txt"), new NamedObject("CS file", "*.cs") };

            showDialog(FileSystemDialogBoxMode.OpenFile, filters);

            showDialog(FileSystemDialogBoxMode.OpenFile, filters);
        }
    }
}
