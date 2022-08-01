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

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Explorer.Data
{
    public class ShowItemsCheckBoxConverter : MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue &&  (SelectionMode)values[0] != SelectionMode.Single && (bool)values[1];

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
