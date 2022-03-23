//* Copyright © Pierre Sprimont, 2021
//*
//* This file is part of the WinCopies Framework.
//*
//* The WinCopies Framework is free software: you can redistribute it and/or modify
//* it under the terms of the GNU General Public License as published by
//* the Free Software Foundation, either version 3 of the License, or
//* (at your option) any later version.
//*
//* The WinCopies Framework is distributed in the hope that it will be useful,
//* but WITHOUT ANY WARRANTY; without even the implied warranty of
//* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//* GNU General Public License for more details.
//*
//* You should have received a copy of the GNU General Public License
//* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using WinCopies.IO.Process;
using WinCopies.Util.Data;

namespace WinCopies.GUI.IO.Process
{
    [ValueConversion(typeof(ProcessTypes<IProcessErrorItem>.IProcessQueue), typeof(bool))]
    public class ErrorPathsToBooleanConverter : AlwaysConvertibleOneWayConverter<ProcessTypes<IProcessErrorItem>.IProcessQueue, object, bool>
    {
        public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.AllowNull;

        protected override bool Convert(ProcessTypes<IProcessErrorItem>.IProcessQueue value, object parameter, CultureInfo culture) => value?.HasItems == true;
    }

    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ObjectToVisibilityConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null || ((string)value).Length == 0 ? Visibility.Collapsed : Visibility.Visible;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
