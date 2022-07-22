using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Collections.Generic;
using WinCopies.Desktop;

using static System.Windows.Input.Key;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Controls.Samples
{
    public class NavigationMenuRoutedEventArgs : RoutedEventArgs<BooleanEventArgs>
    {
        public NavigationMenu Menu { get; }

        public NavigationMenuRoutedEventArgs(in NavigationMenu menu, in RoutedEvent @event, in BooleanEventArgs e) : base(@event, e) => Menu = menu;

        public NavigationMenuRoutedEventArgs(in NavigationMenu menu, in RoutedEvent @event, in bool value) : this(menu, @event, new BooleanEventArgs(value)) { }
    }

    public partial class NavigationMenuBar : ItemsControl
    {
        private Window _window;

        private static readonly DependencyPropertyKey OpenItemPropertyKey = RegisterReadOnly<NavigationMenu, NavigationMenuBar>(nameof(OpenItem), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            Debug.WriteLine("OpenItem");

            var menuBar = (NavigationMenuBar)d;

            if (e.OldValue != null)

                menuBar.OnMenuClosing(e);

            if (e.NewValue != null)

                menuBar.OnMenuOpening(e, null);
        }));

        public static readonly DependencyProperty OpenItemProperty = OpenItemPropertyKey.DependencyProperty;

        public NavigationMenu OpenItem { get => (NavigationMenu)GetValue(OpenItemProperty); internal set => SetValue(OpenItemPropertyKey, value); }

        public static readonly RoutedEvent MenuOpenStateChangedEvent = Register<RoutedEventHandler<BooleanEventArgs>, NavigationMenuBar>(nameof(MenuOpenStateChanged), RoutingStrategy.Bubble);

        public event RoutedEventHandler MenuOpenStateChanged { add => AddHandler(MenuOpenStateChangedEvent, value); remove => RemoveHandler(MenuOpenStateChangedEvent, value); }

        public NavigationMenuBar()
        {
            InitializeComponent();

            LostMouseCapture += NavigationMenuBar_LostMouseCapture;
        }

        public new bool CaptureMouse() => _ = Mouse.Capture(this, CaptureMode.SubTree);

        private void NavigationMenuBar_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource != this)

                _ = CaptureMouse();
        }

        protected static NavigationMenuRoutedEventArgs GetMenuOpenedStateChanged(in NavigationMenu menu, in bool value) => new(menu, MenuOpenStateChangedEvent, value);

        private static NavigationMenuRoutedEventArgs GetMenuOpenedStateChanged(in object menu, in bool value) => GetMenuOpenedStateChanged((NavigationMenu)menu, value);

        protected virtual void OnMenuOpening(DependencyPropertyChangedEventArgs? e, NavigationMenu menu)
        {
            _ = CaptureMouse();

            NavigationMenu openItem = OpenItem;

            if (openItem != null)
            {
                System.Collections.IEnumerable items = Items;

                if (items != null)

                    foreach (object item in items)

                        if (item is INavigableMenu _menu && _menu != openItem.DataContext)
                        {
                            Debug.WriteLine(_menu.Header);

                            _menu.IsOpen = false;
                        }

                _ = openItem.Focus();
            }

            RaiseEvent(GetMenuOpenedStateChanged(menu, true));
        }

        protected virtual void OnMenuClosing(DependencyPropertyChangedEventArgs e)
        {
            ReleaseMouseCapture();

            RaiseEvent(GetMenuOpenedStateChanged(e.OldValue, false));
        }

        public override void OnApplyTemplate()
        {
            if (_window != null)
            {
                _window.Deactivated -= Window_Deactivated;

                _window = null;
            }

            _window = Window.GetWindow(this);

            if (_window != null)

                _window.Deactivated += Window_Deactivated;
        }

        private delegate NavigationMenu TryGetFirst<T>(T itemsControl, int index, ref bool rollBack, ref bool checkContent, out bool found);

        private delegate void TryOpenItemRelativeDelegate(in Panel panel, in int index);

        protected virtual bool OnNavigate(object source, Key key)
        {
            void tryOpenItemRelative(in Panel panel, in int index, bool rollBack, in TryGetFirst<ItemsControl> funcC, in TryGetFirst<Panel> funcP)
            {
                bool checkContent;
                NavigationMenu menu;

                if (ItemsSource == null)
                {
                    checkContent = false;

                    menu = funcC(this, index, ref rollBack, ref checkContent, out _);
                }

                else
                {
                    checkContent = true;

                    menu = funcP(panel, index, ref rollBack, ref checkContent, out _);
                }

                if (menu != null)

                    menu.IsOpen = true;
            }

            void tryOpenItemAfter(in Panel panel, in int index) => tryOpenItemRelative(panel, index, true, Desktop.Extensions.TryGetFirstAfter<NavigationMenu>, Desktop.Extensions.TryGetFirstAfter<NavigationMenu>);

            void tryOpenItemBefore(in Panel panel, in int index) => tryOpenItemRelative(panel, index, true, Desktop.Extensions.TryGetFirstBefore<NavigationMenu>, Desktop.Extensions.TryGetFirstAfter<NavigationMenu>);

            void _action(in IList list, in Func<object, int> func, in Panel itemsHost, in TryOpenItemRelativeDelegate ltr, in TryOpenItemRelativeDelegate rtl)
            {
                if (list.Count < 2)

                    return;

                int index = func(source);

                if (index > -1)

                    switch (FlowDirection)
                    {
                        case FlowDirection.LeftToRight:

                            ltr(itemsHost, index);

                            break;

                        case FlowDirection.RightToLeft:

                            rtl(itemsHost, index);

                            break;
                    }
            }

            void action(in TryOpenItemRelativeDelegate ltr, in TryOpenItemRelativeDelegate rtl)
            {
                if (ItemsSource == null)

                    _action(Items, Items.IndexOf, null, ltr,rtl);

                else
                {
                    Panel itemsHost = this.GetItemsPanel();

                    if (itemsHost != null && itemsHost.HasHorizontalOrientation())

                        _action(itemsHost.Children, obj => source is DependencyObject d && (d = d.GetParent<ContentPresenter>(false)) != null ? ItemContainerGenerator.IndexFromContainer(d) : -1, itemsHost, ltr, rtl);
                }
            }

            switch (key)
            {
                case Left:

                    action(tryOpenItemBefore, tryOpenItemAfter);

                    break;

                case Right:

                    action(tryOpenItemAfter, tryOpenItemBefore);

                    break;

                default:

                    return false;
            }

            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.OriginalSource is UIElement source && source.IsFocused)

                e.Handled = OnNavigate(e.OriginalSource, e.Key);

            base.OnKeyDown(e);
        }

        protected virtual void CloseInItems()
        {
            foreach (object item in Items)

                if (item is NavigationMenu menu)

                    menu.IsOpen = false;
        }

        protected virtual void CloseInItemsSource()
        {
            foreach (object item in ItemsSource)

                if (item is INavigableMenu menu)

                    menu.IsOpen = false;
        }

        protected virtual void CloseAll()
        {
            if (ItemsSource is Collections.Generic.NavigationMenu menuBar)

                menuBar.CloseAll();

            else if (ItemsSource == null)

                CloseInItems();

            else

                CloseInItemsSource();
        }

        protected virtual void OnWindowDeactivated() => CloseAll();

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (IsMouseDirectlyOver)
            {
                Debug.WriteLine("items");

                foreach (object item in Items)

                    if (item is INavigableMenu menu && menu.IsOpen)

                        menu.IsOpen = false;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e) => OnWindowDeactivated();
    }
}
