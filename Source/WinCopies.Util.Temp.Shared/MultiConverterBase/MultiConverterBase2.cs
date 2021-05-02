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

using WinCopies.Util.Data;

using static WinCopies.Util.Data.ConverterHelper;

#if !WinCopies3
using static WinCopies.Util.Util;
#endif

namespace WinCopies.Temp.Util.Data
{
#if WinCopies3
    public interface IMultiConverterConverters<TParamIn, TParamOut, TDestinationIn, TDestinationOut>
    {
        TParamOut GetDefaultParameter();

        TParamOut ConvertParameter(in TParamIn parameter);

        TDestinationOut GetDefaultDestinationValue();

        TDestinationOut ConvertDestinationValue(in TDestinationIn value);
    }

    public sealed class MultiConverterConverters<TParam, TDestination> : IMultiConverterConverters<TParam, TParam, TDestination, TDestination>
    {
        public TParam GetDefaultParameter() => default;

        public TParam ConvertParameter(in TParam parameter) => parameter;

        public TDestination GetDefaultDestinationValue() => default;

        public TDestination ConvertDestinationValue(in TDestination value) => value;
    }
#endif

    public abstract class MultiConverterBase<TParamIn,
#if WinCopies3
        TParamOut,
#endif
        TDestinationIn
#if WinCopies3
        , TDestinationOut
#endif
        > : MultiConverterBase
    {
        public abstract ConversionOptions ConvertOptions { get; }

        public abstract ConversionOptions ConvertBackOptions { get; }

        public abstract ConversionWays Direction { get; }

#if WinCopies3
        protected abstract IMultiConverterConverters<TParamIn, TParamOut, TDestinationIn, TDestinationOut> Converters { get; }
#endif

        protected abstract object Convert(object[] values, TParamOut _parameter, CultureInfo culture);

        public sealed override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (Direction.HasFlag(ConversionWays.OneWay))
            {
                Check(values, ConvertOptions.AllowNullValue, nameof(values));

                Check<TParamIn>(parameter, ConvertOptions.AllowNullParameter, nameof(parameter));

                return Convert(values,
#if WinCopies3
                    parameter == null ? Converters.GetDefaultParameter() : Converters.ConvertParameter((TParamIn)parameter)
#else
                    parameter == null ? default : (TParam)parameter
#endif
                    , culture);
            }

            throw new InvalidOperationException("The OneWay conversion direction is not supported.");
        }

        protected abstract object[] ConvertBack(TDestinationOut _value, TParamOut _parameter, CultureInfo culture);

        public sealed override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (Direction.HasFlag(ConversionWays.OneWayToSource))
            {
                Check<TDestinationIn>(value, ConvertOptions.AllowNullValue, nameof(value));

                Check<TParamIn>(parameter, ConvertOptions.AllowNullParameter, nameof(parameter));

                return ConvertBack(value == null ?
#if WinCopies3
                    Converters.GetDefaultDestinationValue() : Converters.ConvertDestinationValue((TDestinationIn)value)
#else
                    default : (TDestinationIn)value
#endif
                    , parameter == null ?
#if WinCopies3
                    Converters.GetDefaultParameter() : Converters.ConvertParameter((TParamIn)parameter)
#else
                    default : (TParamIn)parameter
#endif
                    , culture);
            }

            throw new InvalidOperationException("The OneWayToSource conversion direction is not supported.");
        }
    }

    public abstract class MultiConverterBase2<TParamIn,
#if WinCopies3
        TParamOut,
#endif
        TDestinationIn
#if WinCopies3
        , TDestinationOut
#endif
      > : MultiConverterBase<TParamIn,
#if WinCopies3
        TParamOut,
#endif
        TDestinationIn
#if WinCopies3
        , TDestinationOut
#endif
      >
    {
        protected abstract TDestinationOut ConvertOverride(object[] values, TParamOut _parameter, CultureInfo culture);

        protected sealed override object Convert(object[] values, TParamOut _parameter, CultureInfo culture) => ConvertOverride(values, _parameter, culture);
    }
}
