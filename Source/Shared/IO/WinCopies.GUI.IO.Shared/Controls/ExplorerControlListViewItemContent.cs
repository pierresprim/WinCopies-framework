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

using WinCopies.Desktop;

namespace WinCopies.GUI.IO.Controls
{
    public class ExplorerControlListViewItemContent : Control
    {
        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, ExplorerControlListViewItemContent>(propertyName);

        /// <summary>
        /// Identifies the <see cref="IsCheckBoxVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckBoxVisibleProperty = Register<bool>(nameof(IsCheckBoxVisible));

        public bool IsCheckBoxVisible { get => (bool)GetValue(IsCheckBoxVisibleProperty); set => SetValue(IsCheckBoxVisibleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SmallIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallIconProperty = Register<ImageSource>(nameof(SmallIcon));

        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        /// <summary>
        /// Identifies the <see cref="MediumIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MediumIconProperty = Register<ImageSource>(nameof(MediumIcon));

        public ImageSource MediumIcon { get => (ImageSource)GetValue(MediumIconProperty); set => SetValue(MediumIconProperty, value); }

        /// <summary>
        /// Identifies the <see cref="LargeIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty = Register<ImageSource>(nameof(LargeIcon));

        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ExtraLargeIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtraLargeIconProperty = Register<ImageSource>(nameof(ExtraLargeIcon));

        public ImageSource ExtraLargeIcon { get => (ImageSource)GetValue(ExtraLargeIconProperty); set => SetValue(ExtraLargeIconProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ItemName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemNameProperty = Register<string>(nameof(ItemName));

        public string ItemName { get => (string)GetValue(ItemNameProperty); set => SetValue(ItemNameProperty, value); }

        //public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(nameof(Properties), typeof(IEnumerable<KeyValuePair<string, string>>), typeof(ExplorerControlListViewItemContent));

        //public IEnumerable<KeyValuePair<string, string>> Properties { get => (IEnumerable<KeyValuePair<string, string>>)GetValue(PropertiesProperty); set => SetValue(PropertiesProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HasTransparency"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasTransparencyProperty = Register<bool>(nameof(HasTransparency));

        public bool HasTransparency { get => (bool)GetValue(HasTransparencyProperty); set => SetValue(HasTransparencyProperty, value); }

        static ExplorerControlListViewItemContent() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<ExplorerControlListViewItemContent>();
    }
}
