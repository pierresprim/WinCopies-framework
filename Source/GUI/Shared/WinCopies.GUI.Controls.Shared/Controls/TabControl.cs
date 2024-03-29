﻿/* Copyright © Pierre Sprimont, 2021
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

namespace WinCopies.GUI.Controls
{
    public class TabControl : System.Windows.Controls.TabControl
    {
        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, TabControl>(propertyName);

        public static readonly DependencyProperty LeftItemsProperty = Register<Style>(nameof(LeftItems));

        public Style LeftItems { get => (Style)GetValue(LeftItemsProperty); set => SetValue(LeftItemsProperty, value); }

        public static readonly DependencyProperty RightItemsProperty = Register<Style>(nameof(RightItems));

        public Style RightItems { get => (Style)GetValue(LeftItemsProperty); set => SetValue(LeftItemsProperty, value); }
    }
}
