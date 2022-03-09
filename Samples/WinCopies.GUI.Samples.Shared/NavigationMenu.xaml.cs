using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Collections.Generic;
using WinCopies.Desktop;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Samples
{
    public partial class NavigationMenu : IconControl
    {
        private NavigationMenuBar _menuBar;

        private static DependencyProperty Register<T>(in string propertyName) => Register<T, NavigationMenu>(propertyName);

        public static readonly DependencyProperty HeaderProperty = Register<string>(nameof(Header));

        public string Header { get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }

        public static readonly DependencyProperty IsOpenProperty = Register<bool, NavigationMenu>(nameof(IsOpen), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) => ((NavigationMenu)d).OnOpenStatusChanged(e)));

        public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }

        public static readonly DependencyProperty ItemsProperty = Register<System.Collections.IEnumerable>(nameof(Items));

        public System.Collections.IEnumerable Items { get => (System.Collections.IEnumerable)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        public static readonly RoutedEvent OpeningEvent = RegisterRoutedEvent<RoutedEventHandler, NavigationMenu>(nameof(Opening), RoutingStrategy.Bubble);

        public event RoutedEventHandler Opening { add => AddHandler(OpeningEvent, value); remove => RemoveHandler(OpeningEvent, value); }

        public static readonly RoutedEvent ClosingEvent = RegisterRoutedEvent<RoutedEventHandler, NavigationMenu>(nameof(Closing), RoutingStrategy.Bubble);

        public event RoutedEventHandler Closing { add => AddHandler(ClosingEvent, value); remove => RemoveHandler(ClosingEvent, value); }

        public NavigationMenu()
        {
            InitializeComponent();

            AddHandler(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, new RoutedEventHandler((object sender, RoutedEventArgs e) => OnClick(e)));
        }

        protected virtual void OnClick(RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button)
            {
                NavigationMenuItem menuItem = button.GetParent<NavigationMenuItem>(false);

                if (!(menuItem == null || menuItem.StaysOpenOnClick))

                    IsOpen = false;
            }
        }

        public override void OnApplyTemplate()
        {
            if (_menuBar != null)
            {
                _menuBar.OpenItem = null;

                _menuBar = null;
            }

            base.OnApplyTemplate();

            _menuBar = this.GetParent<NavigationMenuBar>(false);

            if (IsOpen)

                _menuBar.OpenItem = this;
        }

        protected virtual void OnOpening(RoutedEventArgs e)
        {
            if (_menuBar != null)

                _menuBar.OpenItem = this;

            RaiseEvent(e);
        }

        protected virtual void OnClosing(RoutedEventArgs e)
        {
            if (_menuBar != null && _menuBar.OpenItem == this)

                _menuBar.OpenItem = null;

            Debug.WriteLine(Items);

            if (Items is INavigableMenuItem menuItem)
            {
                INavigableMenuItem parent;

                do
                {
                    parent = menuItem;

                    menuItem = menuItem.Parent as INavigableMenuItem;
                }
                while (menuItem != null);

                Items = parent.Parent is INavigableMenu menu ? menu : parent;
            }

            RaiseEvent(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (_menuBar != null && _menuBar.OpenItem != null)

                IsOpen = true;
        }

        protected virtual void OnOpenStatusChanged(DependencyPropertyChangedEventArgs e)
        {
            void call(Action<RoutedEventArgs> method, RoutedEvent @event) => method(new RoutedEventArgs(@event));

            if ((bool)e.NewValue)

                call(OnOpening, OpeningEvent);

            else

                call(OnClosing, ClosingEvent);
        }
    }
}
