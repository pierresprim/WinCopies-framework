using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Samples
{
    public class IconControl : Control
    {
        public static readonly DependencyProperty IconProperty = Register<ImageSource, IconControl>(nameof(Icon));

        public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }
    }

    public partial class NavigationMenuItemGroup : IconControl
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, NavigationMenuItemGroup>(propertyName);

        public static readonly DependencyProperty HeaderProperty = Register<string>(nameof(Header));

        public string Header { get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }

        public static readonly DependencyProperty IsExpandedProperty = Register<bool, NavigationMenuItemGroup>(nameof(IsExpanded), new PropertyMetadata(true));

        public bool IsExpanded { get => (bool)GetValue(IsExpandedProperty); set => SetValue(IsExpandedProperty, value); }

        public static readonly DependencyProperty ItemsProperty = Register<System.Collections.IEnumerable>(nameof(Items));

        public System.Collections.IEnumerable Items { get => (System.Collections.IEnumerable)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        public NavigationMenuItemGroup() => InitializeComponent();
    }
}
