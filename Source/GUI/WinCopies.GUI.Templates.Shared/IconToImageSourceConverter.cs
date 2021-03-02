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

#if !WinCopies4

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinCopies.Util;
using WinCopies.Util.Data;

#if WinCopies3
using WinCopies.Desktop;
#endif

namespace WinCopies.GUI.Templates
{
    [ValueConversion(typeof(Icon), typeof(ImageSource))]
    public class IconToImageSourceConverter : ConverterBase
    {
        public override object Convert(object value, Type type, object parameter, CultureInfo culture) => value is Icon _value ? _value.ToImageSource() : null;

        public override object ConvertBack(object value, Type type, object parameter, CultureInfo culture) => value is ImageSource _value ? Icon.FromHandle(((BitmapSource)_value).ToBitmap().GetHicon()) : null;
    }
}

#endif
