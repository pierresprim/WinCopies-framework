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

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.PortableDevices;

namespace WinCopies.IO.AbstractionInterop
{
    public class RegistryItemInfoItemProvider
    {
        public RegistryKey RegistryKey { get; }

        public string Path { get; }

        public string ValueName { get; }

        public IProcessPathCollectionFactory ShellProcessPathCollectionFactory { get; }

        public ClientVersion ClientVersion { get; }

        public RegistryItemInfoItemProvider(in RegistryKey registryKey, in IProcessPathCollectionFactory shellProcessPathCollectionFactory, in ClientVersion clientVersion)
        {
            RegistryKey = registryKey;

            ShellProcessPathCollectionFactory = shellProcessPathCollectionFactory;

            ClientVersion = clientVersion;
        }

        public RegistryItemInfoItemProvider(in RegistryKey registryKey, in string valueName, in IProcessPathCollectionFactory shellProcessPathCollectionFactory, in ClientVersion clientVersion) : this(registryKey, shellProcessPathCollectionFactory, clientVersion) => ValueName = valueName;

        public RegistryItemInfoItemProvider(in string path, in IProcessPathCollectionFactory shellProcessPathCollectionFactory, in ClientVersion clientVersion)
        {
            Path = path;

            ShellProcessPathCollectionFactory = shellProcessPathCollectionFactory;

            ClientVersion = clientVersion;
        }

        public RegistryItemInfoItemProvider(in string registryKeyPath, in string valueName, in IProcessPathCollectionFactory shellProcessPathCollectionFactory, in ClientVersion clientVersion) : this(registryKeyPath, shellProcessPathCollectionFactory , clientVersion)
        {
            ValueName = valueName;

            ShellProcessPathCollectionFactory = shellProcessPathCollectionFactory;
        }
    }
}
