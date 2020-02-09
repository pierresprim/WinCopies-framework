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
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace WinCopies.GUI.Controls
{
    [Flags]
    public enum PlaceholderPropertiesConverterEnum

    {

        None = 0,

        Text = 1,

        AcceptsReturn = 2,

        AcceptsTab = 4,

        FontFamily = 8,

        FontSize = 16,

        FontStretch = 32,

        FontStyle = 64,

        FontWeight = 128,

        Foreground = 256,

        TextAlignment = 512,

        TextDecorations = 1024,

        TextWrapping = 2048

    }

    public class PlaceholderPropertiesConverter : WinCopies.Util.Data.MultiConverterBase
    {
        public PlaceholderProperties PlaceholderProperties { get; } = null;

        public PlaceholderPropertiesConverter() => PlaceholderProperties = new PlaceholderProperties();

        public PlaceholderPropertiesConverter(PlaceholderProperties placeholderProperties) => PlaceholderProperties = placeholderProperties;

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //            PlaceholderProperties _value = ((PlaceholderProperties)value);

            //            switch ((String)parameter)
            //            {
            //                case "Text":

            //#if DEBUG 

            //                    Console.WriteLine("PlaceholderPropertiesConverter message: " + _value);

            //#endif

            //                    return _value.Text;

            //                default:

            //                    return null;

            //            }

            if (parameter == null || parameter.GetType() != typeof(PlaceholderPropertiesConverterEnum)) throw new ArgumentException(string.Format("parameter must be {0} and cannot be null.", nameof(PlaceholderPropertiesConverterEnum)));

            PlaceholderPropertiesConverterEnum _parameter = (PlaceholderPropertiesConverterEnum)parameter;

            short i = -1;

            string[] enumNames = typeof(PlaceholderPropertiesConverterEnum).GetEnumNames();

            for (ushort _i = 1; _i <= enumNames.Length - 1; _i++)

            {

                string enumName = enumNames[_i];

                if (_parameter.HasFlag((PlaceholderPropertiesConverterEnum)Enum.Parse(typeof(PlaceholderPropertiesConverterEnum), enumName)))

                {

                    Debug.WriteLine(enumName);

                    i++;

                    typeof(PlaceholderProperties).GetProperty(enumName).SetValue(this.PlaceholderProperties, values[i]);

                }

            }

            return this.PlaceholderProperties;
        }

        // todo : not tested yet.

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (parameter == null || parameter.GetType() != typeof(PlaceholderPropertiesConverterEnum)) throw new ArgumentException(string.Format("parameter must be {0} and cannot be null.", nameof(PlaceholderPropertiesConverterEnum)));

            PlaceholderPropertiesConverterEnum _parameter = (PlaceholderPropertiesConverterEnum)parameter;

            System.Collections.Generic.IEnumerable<byte> values = Enum.GetValues(typeof(PlaceholderPropertiesConverterEnum)).Cast<byte>();

            int length = values.Count(_value => ((byte)_parameter & _value) == _value);

            object[] objectsToReturn = new object[length];

            short i = -1;

            string enumName = null;

            foreach (byte enumValue in values)

            {

                i++;

                enumName = Enum.GetName(typeof(PlaceholderPropertiesConverterEnum), enumValue);

                Debug.WriteLine(enumName);

                objectsToReturn[i] = typeof(PlaceholderProperties).GetProperty(enumName).GetValue(this.PlaceholderProperties);

            }

            return objectsToReturn;
        }
    }
}
