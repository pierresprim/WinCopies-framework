/* Copyright © Pierre Sprimont, 2019
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using WinCopies.Desktop;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Controls
{
    public class HeaderedButton : Button
    {
        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = Register<object, HeaderedButton>(nameof(Header));

        public object Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HeaderDock"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderDockProperty = Register<Dock, HeaderedButton>(nameof(HeaderDock));

        public object HeaderDock { get => GetValue(HeaderDockProperty); set => SetValue(HeaderDockProperty, value); }

        static HeaderedButton() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<HeaderedButton>();
    }

    /// <summary>
    /// Represents a WPF control that can display a header.
    /// </summary>
    public class HeaderedControl : Control
    {
        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = Register<object, HeaderedControl>(nameof(Header));

        /// <summary>
        /// Gets or sets the header of the control. This is a dependency property.
        /// </summary>
        public object Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    }

    public class HeaderedContentControl : System.Windows.Controls.HeaderedContentControl
    {
        public static readonly DependencyProperty DockProperty = Register<Dock, HeaderedContentControl>(nameof(Dock));

        public Dock Dock { get => (Dock)GetValue(DockProperty); set => SetValue(DockProperty, value); }

        static HeaderedContentControl() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<HeaderedContentControl>();
    }

    [DefaultProperty(nameof(Content))]
    [ContentProperty(nameof(Content))]
    public class HeaderedContentItemsControl : HeaderedItemsControl
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, HeaderedContentItemsControl>(propertyName);

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = Register<object>(nameof(Content));

        /// <summary>
        /// Gets or sets the content of the control. This is a dependency property.
        /// </summary>
        public object Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = Register<DataTemplate>(nameof(ContentTemplate));

        public object ContentTemplate { get => GetValue(ContentTemplateProperty); set => SetValue(ContentTemplateProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ContentTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = Register<DataTemplateSelector>(nameof(ContentTemplateSelector));

        public object ContentTemplateSelector { get => GetValue(ContentTemplateSelectorProperty); set => SetValue(ContentTemplateSelectorProperty, value); }
    }

    public class Label : System.Windows.Controls.Label
    {
        /// <summary>
        /// Identifies the <see cref="RecognizesAccessKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RecognizesAccessKeyProperty = Register<bool, Label>(nameof(RecognizesAccessKey), new PropertyMetadata(true));

        public bool RecognizesAccessKey { get => (bool)GetValue(RecognizesAccessKeyProperty); set => SetValue(RecognizesAccessKeyProperty, value); }

        static Label() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<Label>();
    }
}
