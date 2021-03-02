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

using System;
using System.Globalization;

using WinCopies.PropertySystem;
using WinCopies.Util.Data;

using static WinCopies.Util.Data.ConverterHelper;

namespace WinCopies.GUI.Controls.PropertySystem
{
    public class PropertyConverter : AlwaysConvertibleTwoWayConverter<IProperty, object, PropertyViewModel>
    {
        public override ConversionOptions ConvertOptions => AllowNull;

        public override ConversionOptions ConvertBackOptions => ParameterCanBeNull;

        public static PropertyViewModel TryConvert(IProperty property)
        {
            if (property == null)

                return null;

            Type type = property.Type;

            if (type == typeof(string))

                return new TextBoxProperty(property);

            if (typeof(Array).IsAssignableFrom(type))
            {
                var _value = (Array)property.Value;

                return _value == null ? null : _value.Rank != 1 ? null : (PropertyViewModel)ArrayPropertyViewModel.CreateFromArrayProperty(property);
            }

            return typeof(System.Collections.IEnumerable).IsAssignableFrom(type) ? ArrayPropertyViewModel.CreateFromEnumerableProperty(property)
                : type == typeof(bool) || type == typeof(bool?) ? new CheckBoxProperty(property)
                : (PropertyViewModel)new TextBoxProperty(property);
        }

        protected override PropertyViewModel Convert(IProperty value, object parameter, CultureInfo culture) => TryConvert(value);

        protected override IProperty ConvertBack(PropertyViewModel value, object parameter, CultureInfo culture) => value.Model;
    }
}
