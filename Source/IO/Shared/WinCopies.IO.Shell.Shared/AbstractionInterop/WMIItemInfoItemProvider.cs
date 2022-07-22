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

using System.Management;

using WinCopies.IO.PropertySystem;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.AbstractionInterop
{
    public class WMIItemInfoItemProvider
    {
        public string
#if CS8
            ?
#endif
            Path
        { get; }

        public ManagementBaseObject ManagementObject { get; }

        public WMIItemType ItemType { get; }

        public IWMIItemInfoOptions
#if CS8
            ?
#endif
            Options
        { get; }

        public ClientVersion ClientVersion { get; }

        public WMIItemInfoItemProvider(in string
#if CS8
            ?
#endif
            path, in ManagementBaseObject managementObject, in WMIItemType itemType, in IWMIItemInfoOptions
#if CS8
            ?
#endif
            options, in ClientVersion clientVersion)
        {
            Path = path;

            ManagementObject = managementObject ?? throw GetArgumentNullException(nameof(managementObject));

            ItemType = itemType;

            Options = options;

            ClientVersion = clientVersion;
        }
    }
}
