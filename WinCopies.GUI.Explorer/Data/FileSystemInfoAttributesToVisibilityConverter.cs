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
using System.IO;
using System.Windows;
using WinCopies.GUI.Explorer.Themes;
using WinCopies.Util.Data;
using static WinCopies.Util.Util;
using IfCT = WinCopies.Util.Util.ComparisonType;
using IfCM = WinCopies.Util.Util.ComparisonMode;
using IfComp = WinCopies.Util.Util.Comparison;

namespace WinCopies.GUI.Explorer.Data
{
    public class FileSystemInfoAttributesToVisibilityConverter : MultiConverterBase
    {

        private BooleanToVisibilityConverter booleanToVisibilityConverter = null;

        public BooleanToVisibilityConverter BooleanToVisibilityConverter

        {

            get => booleanToVisibilityConverter ?? (booleanToVisibilityConverter = Themes. Generic.BooleanToVisibilityConverter);

            set => booleanToVisibilityConverter = value;

        }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => If(IfCT.Or, IfCM.Logical, IfComp.Equal, DependencyProperty.UnsetValue, values) ? null :

            BooleanToVisibilityConverter.Convert(values[0] is IO.ShellObjectInfo && ((ShellObjectInfo)values[0]).ShellObject.IsFileSystemObject
               ? !((FileAttributes)values[1]).HasFlag(FileAttributes.Hidden) && !((FileAttributes)values[1]).HasFlag(FileAttributes.System)
                   ? true
                   : ((FileAttributes)values[1]).HasFlag(FileAttributes.Hidden) && (bool)values[2]
                   ? (bool)values[2]
                       ? true
                       : ((IO.ShellObjectInfo)values[0]).FileType == IO.FileType.Drive
                       ? ((IO.ShellObjectInfo)values[0]).DriveInfoProperties.IsReady ? true : false
                       : false
                   : ((FileAttributes)values[1]).HasFlag(FileAttributes.System)
                   ? (bool)values[3]
                       ? true
                       : ((IO.ShellObjectInfo)values[0]).FileType == IO.FileType.Drive
                       ? ((IO.ShellObjectInfo)values[0]).DriveInfoProperties.IsReady ? true : false
                       : false
                   : false
               : true, targetType, parameter, culture);// else return Visibility.Visible;

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
