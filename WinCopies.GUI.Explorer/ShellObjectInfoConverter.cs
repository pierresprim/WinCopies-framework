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

using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Globalization;

namespace WinCopies.GUI.Windows.Dialogs
{
    public class ShellObjectInfoConverter : Util.Data.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IO.ShellObjectInfo shellObject = null;

            if (value is IO.ShellObjectInfo)

                shellObject = (IO.ShellObjectInfo)value;

            switch ((string)parameter)

            {

                case "FileName":

                    return System.IO.Path.GetFileName((string)value);

                case "FileType":

                    return shellObject == null ? null : shellObject.ShellObject.Properties.System.ContentType.ValueAsObject + " (" + System.IO.Path.GetExtension(shellObject.Path) + ")";

                case "Path":

                    return System.IO.Path.GetDirectoryName((string)value);

                case "PathBitmapSource":

                    return ShellObject.FromParsingName(System.IO.Path.GetDirectoryName((string)value)).Thumbnail.BitmapSource;

                case "Size":

                    if (shellObject == null)

                        return null;

                    // todo : 'size' and 'sizeOnDisk' have both the same context ; it would be better having one variable above the 'switch' for the gesture of these two ones.

                    ulong? size = (ulong?)shellObject.ShellObject.Properties.System.Size.ValueAsObject;

                    return size.HasValue ?

                        IO.Size.Create(size.Value).ToString() :

                        null;

                case "SizeOnDisk":

                    if (shellObject == null)

                        return null;

                    ulong? sizeOnDisk = (ulong?)shellObject.ShellObject.Properties.System.FileAllocationSize.ValueAsObject;

                    return sizeOnDisk.HasValue ?

                        IO.Size.Create(sizeOnDisk.Value).ToString() :

                        null;

                case "CreationTime":

                    return shellObject?.ShellObject.Properties.System.DateCreated.ValueAsObject;

                case "LastWriteTime":

                    return shellObject?.ShellObject.Properties.System.DateModified.ValueAsObject;

                case "LastAccessTime":

                    return shellObject?.ShellObject.Properties.System.DateAccessed.ValueAsObject;

                default:

                    return null;

            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
