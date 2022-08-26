using System.Windows;

using WinCopies.Desktop;

namespace WinCopies.GUI.Controls
{
    public class PropertyEditor : HeaderedContentItemsControl
    {
        public static readonly DependencyProperty DescriptionProperty = Util.Desktop.UtilHelpers.Register<object, PropertyEditor>(nameof(Description));

        public object Description { get => GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

        static PropertyEditor() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<PropertyEditor>();
    }
}
