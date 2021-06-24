﻿/* Copyright © Pierre Sprimont, 2019
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
using System.Windows.Data;

using WinCopies.Util.Data;

namespace WinCopies.GUI.Windows
{
    [ValueConversion(typeof(HorizontalAlignment), typeof(System.Windows.HorizontalAlignment))]
    public class ButtonAlignmentToHorizontalAlignmentConverter : AlwaysConvertibleTwoWayConverter<HorizontalAlignment, object, System.Windows.HorizontalAlignment>
    {
        public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.ParameterCanBeNull;

        public override IReadOnlyConversionOptions ConvertBackOptions => ConverterHelper.ParameterCanBeNull;

        protected override System.Windows.HorizontalAlignment Convert(HorizontalAlignment value, object parameter, CultureInfo culture)
        {
#if NETFRAMEWORK
            switch (value)
            {
                case HorizontalAlignment.Left:

                    return System.Windows.HorizontalAlignment.Left;

                case HorizontalAlignment.Right:

                    return System.Windows.HorizontalAlignment.Right;

                default:

                    throw new ArgumentException("Invalid value for HorizontalAlignment.");
            }
#else
            return value switch
            {
                HorizontalAlignment.Left => System.Windows.HorizontalAlignment.Left,

                HorizontalAlignment.Right => System.Windows.HorizontalAlignment.Right,

                _ => throw new ArgumentException("Invalid value for HorizontalAlignment.")
            };
#endif
        }

        protected override HorizontalAlignment ConvertBack(System.Windows.HorizontalAlignment value, object parameter, CultureInfo culture)
        {
#if NETFRAMEWORK
            switch (value)
            {
                case System.Windows.HorizontalAlignment.Left:

                    return HorizontalAlignment.Left;

                case System.Windows.HorizontalAlignment.Right:

                    return HorizontalAlignment.Right;

                default:

                    throw new ArgumentException("Invalid value for HorizontalAlignment.");
            }
#else
            return value switch
            {
                System.Windows.HorizontalAlignment.Left => HorizontalAlignment.Left,

                System.Windows.HorizontalAlignment.Right => HorizontalAlignment.Right,

                _ => throw new ArgumentException("Invalid value for HorizontalAlignment.")
            };
#endif
        }
    }
}
