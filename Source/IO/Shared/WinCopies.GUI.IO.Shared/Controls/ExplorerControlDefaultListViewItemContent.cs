using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WinCopies.Desktop;

namespace WinCopies.GUI.IO.Controls
{
    public class ExplorerControlDefaultListViewItemContent : Control
    {
        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, ExplorerControlDefaultListViewItemContent>(propertyName);

        /// <summary>
        /// Identifies the <see cref="IsCheckBoxVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckBoxVisibleProperty = Register<bool>(nameof(IsCheckBoxVisible));

        public bool IsCheckBoxVisible { get => (bool)GetValue(IsCheckBoxVisibleProperty); set => SetValue(IsCheckBoxVisibleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = Register<ImageSource>(nameof(Icon));

        public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ImageSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSizeProperty = Register<double>(nameof(ImageSize));

        public double ImageSize { get => (double)GetValue(ImageSizeProperty); set => SetValue(ImageSizeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ItemName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemNameProperty = Register<string>(nameof(ItemName));

        public string ItemName { get => (string)GetValue(ItemNameProperty); set => SetValue(ItemNameProperty, value); }

        static ExplorerControlDefaultListViewItemContent() => DefaultStyleKeyProperty.OverrideFrameworkPropertyMetadata<ExplorerControlDefaultListViewItemContent>();
    }
}
