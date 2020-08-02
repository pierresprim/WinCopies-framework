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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

using WinCopies.Linq;

using static Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Legacy.Object.Common;

namespace WinCopies.IO.ObjectModel
{
    public interface IPortableDeviceObjectInfo : IFileSystemObjectInfo
    {
        IPortableDeviceObject PortableDeviceObject { get; }
    }

    public class PortableDeviceObjectInfo : FileSystemObjectInfo, IPortableDeviceObjectInfo
    {
        public IPortableDeviceObject PortableDeviceObject { get; }

        private bool? _isSpecialItem;

        public override bool IsSpecialItem
        {
            get
            {
                if (_isSpecialItem.HasValue)

                    return _isSpecialItem.Value;

                bool result = (PortableDeviceObject.Properties.TryGetValue(IsSystem, out Property value) && value.TryGetValue(out bool _value) && _value) || (PortableDeviceObject.Properties.TryGetValue(IsHidden, out Property __value) && __value.TryGetValue(out bool ___value) && ___value);

                _isSpecialItem = result;

                return result;
            }
        }

        public override BitmapSource SmallBitmapSource => TryGetBitmapSource(SmallIconSize);

        public override BitmapSource MediumBitmapSource => TryGetBitmapSource(MediumIconSize);

        public override BitmapSource LargeBitmapSource => TryGetBitmapSource(LargeIconSize);

        public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(ExtraLargeIconSize);

        private bool? _isBrowsable;

        public override bool IsBrowsable
        {
            get
            {
                if (_isBrowsable.HasValue)

                    return _isBrowsable.Value;

                bool result = PortableDeviceObject is IEnumerablePortableDeviceObject;

                _isBrowsable = result;

                return result;
            }
        }

        public override string ItemTypeName => GetItemTypeName(System.IO.Path.GetExtension(Path), FileType);

        public override string Description => "N/A";

        private bool _isSizeLoaded;

        private Size? _size;

        public override Size? Size
        {
            get
            {
                if (_isSizeLoaded)

                    return _size;

                if (PortableDeviceObject.Properties.TryGetValue(Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Legacy.Object.Common.Size, out Property value) && value.TryGetValue(out ulong _value))

                    _size = new Size(_value);

                _isSizeLoaded = true;

                return _size;
            }
        }

        public override IBrowsableObjectInfo Parent { get; }

        public override string LocalizedName => Name;

        private bool _isNameLoaded;

        private string _name;

        public override string Name
        {
            get
            {
                if (_isNameLoaded)

                    return _name;

                _name = PortableDeviceObject.Name;

                _isNameLoaded = true;

                return _name;
            }
        }

        public override FileType FileType { get; }

        public override FileSystemType ItemFileSystemType => FileSystemType.PortableDevice;

        private static FileType GetFileType(in PortableDeviceFileType portableDeviceFileType, in string path)
        {
            string extension = System.IO.Path.GetExtension(path);

            return portableDeviceFileType == PortableDeviceFileType.Folder ? FileType.Folder : extension == ".lnk" ? FileType.Link : extension == ".library.ms" ? FileType.Library : FileType.File;
        }

        internal PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceInfo parentPortableDevice) : this($"{parentPortableDevice.Path}{IO.Path.PathSeparator}{portableDeviceObject.Name}", portableDeviceObject) => Parent = parentPortableDevice;

        private PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceObjectInfo parent) : this($"{parent.Path}{IO.Path.PathSeparator}{portableDeviceObject.Name}", portableDeviceObject) => Parent = parent;

        private PortableDeviceObjectInfo(in string path, in IPortableDeviceObject portableDeviceObject) : base(path)
        {
            PortableDeviceObject = portableDeviceObject;

            FileType = GetFileType(portableDeviceObject.FileType, Path);
        }

        public override IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(null);

        public IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<IPortableDeviceObject> predicate)
        {
            if (PortableDeviceObject is IEnumerablePortableDeviceObject enumerablePortableDeviceObject)

                return (predicate == null ? enumerablePortableDeviceObject : enumerablePortableDeviceObject.WherePredicate(predicate)).Select(portableDeviceObject => new PortableDeviceObjectInfo(portableDeviceObject, this));

            return null;
        }
    }
}
