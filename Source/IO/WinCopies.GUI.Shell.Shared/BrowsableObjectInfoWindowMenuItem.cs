using System.Linq;
using System.Windows;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Shell
{
    public partial class BrowsableObjectInfoWindowMenuItem : System.Windows.Controls.MenuItem
    {
        private static readonly DependencyPropertyKey IsSelectedPropertyKey = RegisterReadOnly<bool, BrowsableObjectInfoWindowMenuItem>(nameof(IsSelected),  new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BrowsableObjectInfoWindowMenuItemViewModel)((BrowsableObjectInfoWindowMenuItem)d).DataContext).IsSelected = (bool)e.NewValue));

        public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); internal set => SetValue(IsSelectedPropertyKey, value); }

        public static DependencyProperty StatusBarLabelProperty = Register<string, BrowsableObjectInfoWindowMenuItem>(nameof(StatusBarLabel));

        public string StatusBarLabel { get => (string)GetValue(StatusBarLabelProperty); set => SetValue(StatusBarLabelProperty, value); }

        public static readonly DependencyProperty ResourceKeyProperty = Register<string, BrowsableObjectInfoWindowMenuItem>(nameof(ResourceKey), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BrowsableObjectInfoWindowMenuItem)d).OnResourceKeyChanged((string)e.NewValue)));

        public string ResourceKey { get => (string)GetValue(ResourceKeyProperty); set => SetValue(ResourceKeyProperty, value); }

        protected void OnResourceKeyChanged(string newResourceKey)
        {
            DataContext = DataContext is BrowsableObjectInfoWindowMenuItemViewModel menuItemViewModel ? new BrowsableObjectInfoWindowMenuItemViewModel(menuItemViewModel) : new BrowsableObjectInfoWindowMenuItemViewModel((BrowsableObjectInfoWindowMenuViewModel)DataContext);

            ((BrowsableObjectInfoWindowMenuItemViewModel)DataContext).ResourceId = newResourceKey;

            Header = (string)typeof(Properties.Resources).GetProperties().FirstOrDefault(p => p.Name == newResourceKey)?.GetValue(null);

            //StatusBarLabel = getResource($"{newResourceKey}StatusBarLabel");
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            IsSelected = true;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            IsSelected = false;
        }
    }
}
