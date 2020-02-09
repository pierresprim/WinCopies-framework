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
using WinCopies.IO;
using ShellObjectInfo = WinCopies.GUI.Explorer.ShellObjectInfo;

namespace WinCopies.GUI.Windows.Dialogs
{
    public class FolderBrowserDialogCommandButtonContentConverter : Util.Data.MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values[0] == System.Windows.DependencyProperty.UnsetValue) return null;



            Explorer.IBrowsableObjectInfo selectedItem = (Explorer.IBrowsableObjectInfo)values[0];



            if (selectedItem != null && (selectedItem.FileType == FileType.Folder || (selectedItem.FileType == FileType.SpecialFolder && selectedItem is IShellObjectInfo so && so.ShellObject.IsFileSystemObject) || selectedItem.FileType == FileType.Drive))

                return Explorer.Themes.Generic.Open;



            switch ((FolderBrowserDialogMode)values[1])
            {

                case FolderBrowserDialogMode.OpenFiles:

                    return Explorer.Themes. Generic.Open;

                case FolderBrowserDialogMode.OpenFolder:

                    return GUI.Themes.Generic.Cancel;

                case FolderBrowserDialogMode.Save:

                    return Explorer.Themes.Generic.Save;

                default:

                    return null;

            }

        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
