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
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native;

using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinCopies.IO.ObjectModel;
using WinCopies.Linq;

using static WinCopies.
#if WinCopies2
    Util.Util
#else
    ThrowHelper
#endif
    ;

namespace WinCopies.IO
{
    public class PortableDeviceInfoProperties<T> : FileSystemObjectInfoProperties<T>, IPortableDeviceInfoProperties where T : IPortableDeviceInfo
    {
        public PortableDeviceOpeningOptions OpeningOptions => BrowsableObjectInfo.OpeningOptions;

        public PortableDeviceInfoProperties(T browsableObjectInfo) : base(browsableObjectInfo)
        {
            // Left empty.
        }
    }

    namespace ObjectModel
    {
        public class PortableDeviceInfo : FileSystemObjectInfo<IPortableDeviceInfoProperties, IPortableDevice>, IPortableDeviceInfo<IPortableDeviceInfoProperties>
        {
            private const int PortableDeviceIcon = 42;
            private const string PortableDeviceIconDllName = "imageres.dll";

            #region Properties
            public override FileType FileType => FileType.Folder;

            public sealed override IPortableDevice EncapsulatedObjectGeneric { get; }

            public sealed override IPortableDeviceInfoProperties ObjectPropertiesGeneric { get; }

            public override bool IsSpecialItem => false;

            public override BitmapSource SmallBitmapSource => TryGetBitmapSource(SmallIconSize);

            public override BitmapSource MediumBitmapSource => TryGetBitmapSource(MediumIconSize);

            public override BitmapSource LargeBitmapSource => TryGetBitmapSource(LargeIconSize);

            public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(ExtraLargeIconSize);

            public override bool IsBrowsable => true;

            public override string ItemTypeName => "Portable device";

            public override string Description => "N/A";

            public override Size? Size => null;

            public override IBrowsableObjectInfo Parent => ShellObjectInfo.From(ShellObject.FromParsingName(KnownFolders.Computer.ParsingName), ClientVersion.Value);

            public override string LocalizedName => Name;

            public override string Name => EncapsulatedObjectGeneric.DeviceFriendlyName;

            public override FileSystemType ItemFileSystemType => FileSystemType.PortableDevice;

            public PortableDeviceOpeningOptions OpeningOptions { get; set; }
            #endregion

            public PortableDeviceInfo(in IPortableDevice portableDevice, in ClientVersion clientVersion) : this(portableDevice, clientVersion, new PortableDeviceOpeningOptions(GenericRights.Read, FileShareOptions.Read, true))
            {
                // Left empty.
            }

            public PortableDeviceInfo(in IPortableDevice portableDevice, in ClientVersion clientVersion, in PortableDeviceOpeningOptions openingOptions) : base((portableDevice ?? throw GetArgumentNullException(nameof(portableDevice))).DeviceFriendlyName, clientVersion)
            {
                EncapsulatedObjectGeneric = portableDevice;

                OpeningOptions = openingOptions;

                ObjectPropertiesGeneric = new PortableDeviceInfoProperties<IPortableDeviceInfo>(this);
            }

            private BitmapSource TryGetBitmapSource(in int size)
            {
                using
#if NETFRAMEWORK
            (
#endif

                Icon icon = TryGetIcon(PortableDeviceIcon, PortableDeviceIconDllName, new System.Drawing.Size(size, size))

#if NETFRAMEWORK
            )
#else
                ;
#endif

                return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }

            public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(null);

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(in Predicate<IPortableDeviceObject> predicate)
            {
                EncapsulatedObjectGeneric.Open(ClientVersion.Value, OpeningOptions);

                return (predicate == null ? EncapsulatedObjectGeneric : EncapsulatedObjectGeneric.WherePredicate(predicate)).Select(portableDeviceObject => new PortableDeviceObjectInfo(portableDeviceObject, this));
            }
        }
    }
}
