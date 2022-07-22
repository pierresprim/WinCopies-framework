using System.Windows;
using System.Windows.Controls;

using WinCopies.Desktop;

namespace WinCopies.GUI.Controls
{
    /// <summary>
    /// Represents a WPF control that can display a header.
    /// </summary>
    public class HeaderedControl : Control
    {
        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(HeaderedControl));

        /// <summary>
        /// Gets or sets the header of the control. This is a dependency property.
        /// </summary>
        public object Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    }

    public class HeaderedContentControl : System.Windows.Controls.HeaderedContentControl
    {
        public static readonly DependencyProperty DockProperty = Util.Desktop.UtilHelpers.Register<Dock, HeaderedContentControl>(nameof(Dock));

        public Dock Dock { get => (Dock)GetValue(DockProperty); set => SetValue(DockProperty, value); }

        static HeaderedContentControl() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<HeaderedContentControl>();
    }
}
