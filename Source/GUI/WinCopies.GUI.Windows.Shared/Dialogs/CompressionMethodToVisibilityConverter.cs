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

//namespace WinCopies.GUI.Windows.Dialogs
//{
//    public class CompressionMethodToVisibilityConverter : WinCopies.Util.Data.MultiConverterBase
//    {
//        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//        {
//            SevenZip.CompressionMethod currentCompressionMethod = SevenZip.CompressionMethod.Default;

//            currentCompressionMethod = (SevenZip.CompressionMethod)((KeyValuePair)values[1]).Key;

//            switch ((SevenZip.OutArchiveFormat)values[0])

//            {

//                case SevenZip.OutArchiveFormat.Zip:

//                    return currentCompressionMethod == SevenZip.CompressionMethod.Copy || currentCompressionMethod == SevenZip.CompressionMethod.Deflate || currentCompressionMethod == SevenZip.CompressionMethod.Deflate64 || currentCompressionMethod == SevenZip.CompressionMethod.BZip2 || currentCompressionMethod == SevenZip.CompressionMethod.Lzma || currentCompressionMethod == SevenZip.CompressionMethod.Ppmd || currentCompressionMethod == SevenZip.CompressionMethod.Default ?

//                        System.Windows.Visibility.Visible :

//                        System.Windows.Visibility.Collapsed;

//                case SevenZip.OutArchiveFormat.SevenZip:

//                    return currentCompressionMethod == SevenZip.CompressionMethod.Copy || currentCompressionMethod == SevenZip.CompressionMethod.BZip2 || currentCompressionMethod == SevenZip.CompressionMethod.Lzma || currentCompressionMethod == SevenZip.CompressionMethod.Lzma2 || currentCompressionMethod == SevenZip.CompressionMethod.Ppmd || currentCompressionMethod == SevenZip.CompressionMethod.Default ?

//                        System.Windows.Visibility.Visible :

//                        System.Windows.Visibility.Collapsed;

//                default:

//                    return currentCompressionMethod == SevenZip.CompressionMethod.Default ?

//                        System.Windows.Visibility.Visible :

//                        System.Windows.Visibility.Collapsed;

//            }
//        }

//        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
//    }
//}
