using System;
using System.Windows;
using System.Windows.Input;

using WinCopies.Commands;
using WinCopies.Desktop;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Windows;
using WinCopies.Util.Data;

using EventArgs = System.EventArgs;

namespace WinCopies.GUI.Controls.Samples
{
    public partial class MainWindow : Windows.Window
    {
        private TitleBarMenuItem _menuItem;

        public static readonly DependencyProperty SwitchCommandProperty = Util.Desktop.UtilHelpers.Register<IQueryCommand<bool>, MainWindow>(nameof(SwitchCommand), new PropertyMetadata(new DelegateQueryCommand<bool>(o => true, o =>
        {
            _ = MessageBox.Show("Ok");

            return true;
        })));

        public static readonly DependencyProperty SwitchCommand2Property = Util.Desktop.UtilHelpers.Register<IQueryCommand<bool>, MainWindow>(nameof(SwitchCommand2), new PropertyMetadata(new DelegateQueryCommand<bool>(o => true, o =>
        {
            _ = MessageBox.Show("Error");

            return false;
        })));

        public IQueryCommand<bool> SwitchCommand { get => (IQueryCommand<bool>)GetValue(SwitchCommandProperty); set => SetValue(SwitchCommandProperty, value); }

        public IQueryCommand<bool> SwitchCommand2 { get => (IQueryCommand<bool>)GetValue(SwitchCommand2Property); set => SetValue(SwitchCommand2Property, value); }

        public MainWindow()
        {
            InitializeComponent();

            void add(in ICommand command, ActionIn<string?> action) => CommandBindings.Add(command, (object sender, ExecutedRoutedEventArgs e) => action(e.Parameter as string), Desktop.Delegates.CanExecute);

            add(Commands.Commands.CommonCommand, (in string? param) => MessageBox.Show(param));
            add(System.Windows.Input.ApplicationCommands.Close, (in string? param) => Close());

            var dw = new DialogWindow() { Content = new FontPicker() }.ShowDialog();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var menuItems = new TitleBarMenuItemQueue();

            menuItems.Enqueue(_menuItem = new TitleBarMenuItem() { Header = "Menu 1" });

            _menuItem.Click += (object? sender, EventArgs _e) => MessageBox.Show("You clicked on the menu item 1.");

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

        private void Window_HelpButtonClick(object sender, RoutedEventArgs e) => _ = MessageBox.Show($"The window is currently in help mode: {IsInHelpMode}.");

        private new void Close()
        {
            if (!CloseButton)
            {
                CloseButton = true;

                return;
            }

            base.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            static void action(string? s) => MessageBox.Show($"You clicked the Button{s}!");

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

                    _ = _dialogWindow.CommandBindings.Add(new CommandBinding(routedCommand, (object _sender, ExecutedRoutedEventArgs _e) => MessageBox.Show($"You clicked the Button{(string)_e.Parameter}!")));

                    return _dialogWindow;
                }
            };

            int i = 0;

            DialogWindow dialogWindow = dialogWindows[0]();

            dialogWindow.Closed += (object? _sender, EventArgs _e) => OnDialogWindowClosed(dialogWindows, i);

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

                dialogWindow.Closed += (object? sender, EventArgs e) => OnDialogWindowClosed(dialogWindows, i);

                dialogWindow.Show();
            }

            else
            {
                DialogWindow dialogWindow = dialogWindows[i]();

                dialogWindow.Closed += (object? sender, EventArgs e) => OnDialogWindowClosed(dialogWindows, i);

                _ = dialogWindow.ShowDialog();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) => new NavigationMenuWindow().ShowDialog();

        private void NumericUpDown_ValueChanged(object sender, ValueChangedRoutedEventArgs<decimal> e) => MessageBox.Show($"{e.OldValue} {e.NewValue}");
    }
}
