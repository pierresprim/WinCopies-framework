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
using System.Windows.Controls;
using WinCopies.Util.Data;
using static WinCopies.Util.Util;
using IfCT = WinCopies.Util.Util.ComparisonType;
using IfCM = WinCopies.Util.Util.ComparisonMode;
using IfComp = WinCopies.Util.Util.Comparison;

namespace WinCopies.GUI.Explorer.Data
{
    public class ViewStyleConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            //#if DEBUG
            //            Console.WriteLine("ViewStyleConverter value: "+value);
            //#endif

            string result = If(IfCT.Or, IfCM.Logical, IfComp.Equal, (ViewStyles)value, ViewStyles.SizeOne,
                ViewStyles.SizeTwo,
                ViewStyles.SizeThree,
                ViewStyles.SizeFour,
                ViewStyles.Tiles) ? "SizeOneToFourOrTiles" : (ViewStyles)value == ViewStyles.List ? "ListItems" : null;

            string _parameter = (string)parameter;
            object r = _parameter == "ScrollBarsVisibility" ? result : _parameter == "WrapPanelOrientation" ? result == "SizeOneToFourOrTiles" ? Orientation.Horizontal : result == "ListItems" ? (object)Orientation.Vertical : null : null;
            return _parameter == "ScrollBarsVisibility" ? result : _parameter == "WrapPanelOrientation" ? result == "SizeOneToFourOrTiles" ? Orientation.Horizontal : result == "ListItems" ? (object)Orientation.Vertical : null : null;

        }

        //#if DEBUG//                    Console.WriteLine("ViewStyleConverter return value: " + "SizeOneToFour");//#endif//#if DEBUG//            Console.WriteLine("ViewStyleConverter return value: " + "ListItems");//#endif

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
