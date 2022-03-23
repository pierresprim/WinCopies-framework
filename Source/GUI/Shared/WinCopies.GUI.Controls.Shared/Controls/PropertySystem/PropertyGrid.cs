/*Copyright © Pierre Sprimont, 2020
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

using System.Windows;
using System.Windows.Media;
using WinCopies.PropertySystem;

namespace WinCopies.GUI.Controls
{
    public class PropertyGrid : System.Windows.Controls.DataGrid
    {
        /// <summary>
        /// Identifies the <see cref="Header1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Header1Property = DependencyProperty.Register(nameof(Header1), typeof(object), typeof(PropertyGrid));

        public object Header1 { get => GetValue(Header1Property); set => SetValue(Header1Property, value); }

        /// <summary>
        /// Identifies the <see cref="Header2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Header2Property = DependencyProperty.Register(nameof(Header2), typeof(object), typeof(PropertyGrid));

        public object Header2 { get => (ImageSource)GetValue(Header2Property); set => SetValue(Header2Property, value); }

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(PropertyGrid));

        public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }

        public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(nameof(Properties), typeof(System.Collections.Generic.IEnumerable<IProperty>), typeof(PropertyGrid));

        public System.Collections.Generic.IEnumerable<IProperty> Properties { get => (System.Collections.Generic.IEnumerable<IProperty>)GetValue(PropertiesProperty); set => SetValue(PropertiesProperty, value); }

        static PropertyGrid() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
    }
}
