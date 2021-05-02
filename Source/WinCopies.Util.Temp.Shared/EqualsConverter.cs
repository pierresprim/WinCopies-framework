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

using System;
using System.Globalization;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Temp.Util.Data;
using WinCopies.Util.Data;

using static WinCopies.Util.Data.ConverterHelper;

namespace WinCopies.Temp
{
    public class EqualsConverter : MultiConverterBase2<bool, bool, bool, bool>
    {
        public override ConversionOptions ConvertOptions => ParameterCanBeNull;

        public override ConversionOptions ConvertBackOptions => AllowNull;

        public override ConversionWays Direction => ConversionWays.OneWay;

        protected override IMultiConverterConverters<bool, bool, bool, bool> Converters => new MultiConverterConverters<bool, bool>();

        protected override bool ConvertOverride(object[] values, bool parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 1)

                return false;

            Func<object, object, bool> func;

            if (parameter is bool _parameter && _parameter)

                func = (x, y) => x == y;

            else

                func = (x, y) => x != y;

            for (int i = 0; i < values.Length; i++)

                for (int j = 1; j < values.Length; j++)

                    if (func(values[i], values[j]))

                        return false;

            return true;
        }

        protected override object[] ConvertBack(bool _value, bool _parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
