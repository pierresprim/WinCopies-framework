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

using System.Drawing;
using System.Windows.Media.Imaging;

namespace WinCopies.IO
{
    public interface IFileSystemObjectInfoProperties
    {
        ///// <summary>
        ///// Gets the <see cref="WinCopies.IO.FileType"/> of this <see cref="IFileSystemObject"/>.
        ///// </summary>
        FileType FileType { get; }
    }

    namespace ObjectModel
    {
        // public interface IFileSystemObjectInfoFactory : IBrowsableObjectInfoFactory { }

        public interface IFileSystemObjectInfo : IFileSystemObjectInfoProperties, IBrowsableObjectInfo
        {
            Icon TryGetIcon(in int size);

            BitmapSource TryGetBitmapSource(in int size);
        }

        public interface IFileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> : IFileSystemObjectInfo, IBrowsableObjectInfo<TObjectProperties, TEncapsulatedObject> where TObjectProperties : IFileSystemObjectInfoProperties
        {
            // Left empty.
        }
    }
}
