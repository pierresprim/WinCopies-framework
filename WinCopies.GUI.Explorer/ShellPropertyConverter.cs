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

using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Diagnostics;
using System.Globalization;

namespace WinCopies.GUI.Explorer
{
    public class ShellPropertyConverter : WinCopies.Util.Data.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is ShellPropertyContainer _value)

                switch ((string)parameter)

                {

                    case "Visibility":

                        return !string.IsNullOrEmpty(_value.Description.DisplayName)
                            ? System.Windows.Visibility.Visible
                            : (object)System.Windows.Visibility.Collapsed;

                    case "IsReadOnly":

#if DEBUG

                        Debug.WriteLine(_value.CanonicalName + " " + _value.Description.DisplayName + " " + _value.Description.TypeFlags.HasFlag(PropertyTypeOptions.IsInnate));

#endif

                        return _value.Description.TypeFlags.HasFlag(PropertyTypeOptions.IsInnate);

                    case "IsEnum":

#if DEBUG

                        Debug.WriteLine(_value.Description.DisplayName);

#endif

                        return _value.Description.DisplayType == PropertyDisplayType.Enumerated;

                    case "EnumValues":

                        return _value.Description.PropertyEnumTypes;

                    case "IsArray":

                        return _value.ValueAsObject is Array;

                }

            return null;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
