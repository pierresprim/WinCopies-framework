/* Copyright © Pierre Sprimont, 2021
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

using System.Globalization;
using System.Windows;

using WinCopies.Util.Data;

namespace WinCopies.GUI
{
    public class FlowDirectionToHorizontalAlignment : AlwaysConvertibleTwoWayConverter<FlowDirection, object, HorizontalAlignment>
    {
        public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.ParameterCanBeNull;

        public override IReadOnlyConversionOptions ConvertBackOptions => ConverterHelper.ParameterCanBeNull;

        protected override HorizontalAlignment Convert(FlowDirection value, object parameter, CultureInfo culture) => value == FlowDirection.LeftToRight ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        protected override FlowDirection ConvertBack(HorizontalAlignment value, object parameter, CultureInfo culture) => value == HorizontalAlignment.Right ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
    }
}
