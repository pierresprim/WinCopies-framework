﻿/* Copyright © Pierre Sprimont, 2021
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

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.PortableDevices;

using WinCopies.IO.Process;

namespace WinCopies.IO.AbstractionInterop
{
    public class RegistryItemInfoItemProvider
    {
        public RegistryKey RegistryKey { get; }

        public string Path { get; }

        public string ValueName { get; }

        public ClientVersion ClientVersion { get; }

        public RegistryItemInfoItemProvider(in RegistryKey registryKey,  in ClientVersion clientVersion)
        {
            RegistryKey = registryKey;

            ClientVersion = clientVersion;
        }

        public RegistryItemInfoItemProvider(in RegistryKey registryKey, in string valueName,  in ClientVersion clientVersion) : this(registryKey, clientVersion) => ValueName = valueName;

        public RegistryItemInfoItemProvider(in string path, in ClientVersion clientVersion)
        {
            Path = path;

            ClientVersion = clientVersion;
        }

        public RegistryItemInfoItemProvider(in string registryKeyPath, in string valueName, in ClientVersion clientVersion) : this(registryKeyPath,  clientVersion) => ValueName = valueName;
    }
}
