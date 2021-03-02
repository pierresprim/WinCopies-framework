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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Windows.Dialogs;
using WinCopies.Util;
using WinCopies.Commands;
using WinCopies.GUI.Windows;

namespace WinCopies.GUI.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WinCopies.GUI.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ShowHelpButton = true;
        }

        private void Window_HelpButtonClick(object sender, RoutedEventArgs e) => _ = MessageBox.Show($"The window is currently in help mode: { IsInHelpMode }.");

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            static void action(string s) => MessageBox.Show($"You clicked the Button{s}!");

            ICommand command = new DelegateCommand<string>(s => true, s => action(s));

            RoutedCommand routedCommand = new RoutedUICommand("ButtonCommand", "ButtonCommand", typeof(MainWindow));

            Func<DialogWindow>[] dialogWindows =
                {
                () => new DialogWindow() { Content = "1 - This is a sample DialogWindow.", ShowHelpButton = true },
            () => new DialogWindow() { Content = "2 - This is a sample DialogWindow.", ShowHelpButton = true },
            () => new DialogWindow() { Content = "3 - This is a sample DialogWindow.", ShowHelpButtonAsCommandButton = true },
            () => new DialogWindow() { Content = "4 - This is a sample DialogWindow.", ShowHelpButtonAsCommandButton = true },
            () => new DialogWindow() { Content = "5 - This is a sample DialogWindow.", DialogButton = DialogButton.YesNoCancel, DefaultButton = DefaultButton.Cancel, ShowHelpButtonAsCommandButton = true },
            () => new DialogWindow() { Content = "6 - This is a sample DialogWindow.", DialogButton = null, CustomButtonTemplateSelector = new AttributeDataTemplateSelector(), CustomButtonsSource = new ButtonModel[] { new ButtonModel("Button1") { CommandParameter = "1", Command = command }, new ButtonModel("Button2") { CommandParameter = "2", Command = command } } },
            () =>
            {
                var _dialogWindow = new DialogWindow() { Content = "7 - This is a sample DialogWindow.", DialogButton = null, CustomButtonTemplateSelector = new AttributeDataTemplateSelector(), CustomButtonsSource = new ButtonModel[] { new ButtonModel("Button1") { CommandParameter = "1", Command = routedCommand }, new ButtonModel("Button2") { CommandParameter = "2", Command = routedCommand } } };

                _dialogWindow.CommandBindings.Add(new CommandBinding(routedCommand, (object _sender, ExecutedRoutedEventArgs _e) => MessageBox.Show($"You clicked the Button{ (string) _e.Parameter}!")));

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

                dialogWindow.ShowDialog();

            }

        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

        private void MenuItem_Click(object sender, RoutedEventArgs e) => new Window1().Show();

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) => new ExplorerControlTest().Show();

        private void MenuItem_Click_2(object sender, RoutedEventArgs e) => new CopyTest().Show();
    }
}
