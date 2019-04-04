﻿using System;
using System.Globalization;
using System.Windows;

namespace WinCopies.Util.DataConverters
{
    public class ReversedBooleanToVisibilityConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter != null && !(parameter is Visibility))

                throw new ArgumentException("parameter must be a value of the System.Windows.Visibility enum.");

            return (bool)value ? parameter ?? Visibility.Collapsed : Visibility.Visible;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)value == Visibility.Visible;
    }
}