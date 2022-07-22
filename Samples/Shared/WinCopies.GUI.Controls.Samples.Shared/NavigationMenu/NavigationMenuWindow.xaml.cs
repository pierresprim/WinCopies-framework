using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using WinCopies.Collections.Generic;
using WinCopies.Desktop;

namespace WinCopies.GUI.Controls.Samples
{
    public partial class NavigationMenuWindow : Window
    {
        public class DefaultCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter) => Debug.WriteLine($"{parameter} was clicked.");
        }

        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, NavigationMenuWindow>(propertyName);

        public static readonly DependencyProperty ItemsProperty = Register<IList>(nameof(Items));

        public IList Items { get => (IList)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        public NavigationMenuWindow()
        {
            NavigableMenu<string> getMenu(in string header, in bool isOpen = false) => new() { Header = header, IsOpen = isOpen, Icon = Icon };

            NavigableMenuItemGroup<string> getItemGroup(in string header, in bool isExpanded = false) => new() { Header = header, IsExpanded = isExpanded, Icon = Icon };

            NavigableMenuItem<string> getItem(in string header) => new() { Header = header, Command = new DefaultCommand(), CommandParameter = header, Icon = Icon };

            InitializeComponent();

            DataContext = this;

            Icon = Properties.Resources.WinCopies.ToImageSource();

            NavigableMenu<string> fileMenu = getMenu("_File", true);

            NavigableMenuItemGroup<string> newMenu = getItemGroup("_New", true);

            NavigableMenuItem<string> newTabMenu = getItem("_New tab");

            newTabMenu.StaysOpenOnClick = true;

            NavigableMenuItemGroup<string> newTabMenuGroup = getItemGroup("Common");

            newTabMenuGroup.Add(getItem("New file system tab"));

            newTabMenuGroup.Add(getItem("New registry tab"));

            newTabMenu.Add(newTabMenuGroup);

            newMenu.Add(newTabMenu);

            newMenu.Add(getItem("New window"));

            fileMenu.Add(newMenu);

            fileMenu.Add(getItemGroup("Save"));

            NavigableMenu<string> editMenu = getMenu("Edit");

            NavigableMenuItemGroup<string> _newMenu = getItemGroup("Clipboard", true);

            _newMenu.Add(getItem("Copy"));

            editMenu.Add(_newMenu);

            editMenu.Add(getItemGroup("Selection"));

            var items = new Collections.Generic.NavigationMenu
            {
                fileMenu,editMenu
            };

            Items = items;
        }
    }
}
