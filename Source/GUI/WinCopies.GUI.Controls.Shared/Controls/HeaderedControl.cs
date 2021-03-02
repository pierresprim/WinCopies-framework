using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WinCopies.GUI.Controls
{
    /// <summary>
    /// Represents a WPF control that can display a header.
    /// </summary>
    public class HeaderedControl : System.Windows.Controls.Control
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
}
