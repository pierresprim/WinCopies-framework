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

using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.PropertySystem;
using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.PropertySystem
{
    public interface IPortableDeviceInfoProperties : IFileSystemObjectInfoProperties
    {
        PortableDeviceOpeningOptions OpeningOptions { get; }
    }

    public class PortableDeviceInfoProperties<T> : FileSystemObjectInfoProperties<T>, IPortableDeviceInfoProperties where T : IPortableDeviceInfoBase
    {
        public PortableDeviceOpeningOptions OpeningOptions { get; }

        public override Size? Size => null;

        public PortableDeviceInfoProperties(T browsableObjectInfo, PortableDeviceOpeningOptions openingOptions) : base(browsableObjectInfo, FileType.Other) => OpeningOptions = openingOptions;
    }

    public class PortableDeviceObjectInfoProperties<T> : FileSystemObjectInfoProperties<T> where T : IPortableDeviceObjectInfoBase
    {
        private bool _isSizeLoaded;
        private Size? _size;

        public override Size? Size
        {
            get
            {
                if (_isSizeLoaded)

                    return _size;

                if (InnerObject.InnerObject.Properties.TryGetValue(Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Legacy.Object.Common.Size, out Property value) && value.TryGetValue(out ulong _value))

                    _size = new Size(_value);

                _isSizeLoaded = true;

                return _size;
            }
        }

        public PortableDeviceObjectInfoProperties(in T browsableObjectInfo):base(browsableObjectInfo, GetFileType(browsableObjectInfo.InnerObject.FileType, browsableObjectInfo. Path))
        {
            // Left empty.
        }

        private static FileType GetFileType(in PortableDeviceFileType portableDeviceFileType, in string path)
        {
            string extension = System.IO.Path.GetExtension(path);

            return portableDeviceFileType == PortableDeviceFileType.Folder ? FileType.Folder : extension == ".lnk" ? FileType.Link : extension == ".library.ms" ? FileType.Library : FileType.File;
        }
    }
}
