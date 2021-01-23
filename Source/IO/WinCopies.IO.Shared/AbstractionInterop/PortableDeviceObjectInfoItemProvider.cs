/* Copyright © Pierre Sprimont, 2021
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
using WinCopies.IO.ObjectModel;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO.AbstractionInterop
{
    public class PortableDeviceObjectInfoItemProvider
    {
        public IPortableDeviceObject PortableDeviceObject { get; }

        public IPortableDeviceInfoBase ParentPortableDevice { get; }

        public IPortableDeviceObjectInfoBase ParentPortableDeviceObject { get; }

        public ClientVersion ClientVersion { get; }

        private PortableDeviceObjectInfoItemProvider(in IPortableDeviceObject portableDeviceObject, in ClientVersion clientVersion)
        {
            PortableDeviceObject = portableDeviceObject ?? throw GetArgumentNullException(nameof(portableDeviceObject));

            ClientVersion = clientVersion;
        }

        public PortableDeviceObjectInfoItemProvider(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceInfoBase parentPortableDevice, in ClientVersion clientVersion) : this(portableDeviceObject, clientVersion) => ParentPortableDevice = parentPortableDevice ?? throw GetArgumentNullException(nameof(parentPortableDevice));

        public PortableDeviceObjectInfoItemProvider(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceObjectInfoBase parentPortableDeviceObject, in ClientVersion clientVersion) : this(portableDeviceObject, clientVersion) => ParentPortableDeviceObject = parentPortableDeviceObject ?? throw GetArgumentNullException(nameof(parentPortableDeviceObject));
    }
}
