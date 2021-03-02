/* Copyright © Pierre Sprimont, 2020
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
using System.Windows.Controls;
using System.Windows.Media;

namespace WinCopies.GUI.IO.Controls
{
    public class ExplorerControlListViewItemContent : Control
    {
        /// <summary>
        /// Identifies the <see cref="IsCheckBoxVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckBoxVisibleProperty = DependencyProperty.Register(nameof(IsCheckBoxVisible), typeof(bool), typeof(ExplorerControlListViewItemContent));

        public bool IsCheckBoxVisible { get => (bool)GetValue(IsCheckBoxVisibleProperty); set => SetValue(IsCheckBoxVisibleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SmallIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallIconProperty = DependencyProperty.Register(nameof(SmallIcon), typeof(ImageSource), typeof(ExplorerControlListViewItemContent));

        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        /// <summary>
        /// Identifies the <see cref="LargeIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty = DependencyProperty.Register(nameof(LargeIcon), typeof(ImageSource), typeof(ExplorerControlListViewItemContent));

        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ItemName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemNameProperty = DependencyProperty.Register(nameof(ItemName), typeof(string), typeof(ExplorerControlListViewItemContent));

        public ImageSource ItemName { get => (ImageSource)GetValue(ItemNameProperty); set => SetValue(ItemNameProperty, value); }

        //public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(nameof(Properties), typeof(IEnumerable<KeyValuePair<string, string>>), typeof(ExplorerControlListViewItemContent));

        //public IEnumerable<KeyValuePair<string, string>> Properties { get => (IEnumerable<KeyValuePair<string, string>>)GetValue(PropertiesProperty); set => SetValue(PropertiesProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HasTransparency"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasTransparencyProperty = DependencyProperty.Register(nameof(HasTransparency), typeof(bool), typeof(ExplorerControlListViewItemContent));

        public bool HasTransparency { get => (bool)GetValue(HasTransparencyProperty); set => SetValue(HasTransparencyProperty, value); }

        static ExplorerControlListViewItemContent() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerControlListViewItemContent), new FrameworkPropertyMetadata(typeof(ExplorerControlListViewItemContent)));
    }
}
