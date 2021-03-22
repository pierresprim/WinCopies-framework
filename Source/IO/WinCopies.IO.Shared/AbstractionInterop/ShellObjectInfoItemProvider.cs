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
using Microsoft.WindowsAPICodePack.Shell;

using SevenZip;

using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.AbstractionInterop
{
    public enum NonShellObjectRootItemType : byte
    {
        None = 0,

        Registry = 1,

        WMI = 2
    }

    public class ShellObjectInfoItemProvider : ArchiveItemInfoProviderItemProvider
    {
        public ClientVersion? ClientVersion { get; }

        public ShellObject ShellObject { get; }

        public IPortableDevice PortableDevice { get; }

        public NonShellObjectRootItemType NonShellObjectRootItemType { get; }

        public IProcessPathCollectionFactory ProcessPathCollectionFactory { get; }

        public ShellObjectInfoItemProvider(in ShellObject shellObject, in IProcessPathCollectionFactory processPathCollectionFactory, in ClientVersion clientVersion)
        {
            ShellObject = shellObject;

            ProcessPathCollectionFactory = processPathCollectionFactory;

            ClientVersion = clientVersion;
        }

        public ShellObjectInfoItemProvider(in IShellObjectInfoBase archiveShellObject, in ArchiveFileInfo? archiveFileInfo) : base(archiveShellObject, archiveFileInfo) => ProcessPathCollectionFactory = archiveShellObject.ProcessPathCollectionFactory;

        public ShellObjectInfoItemProvider(in IShellObjectInfoBase archiveShellObject, in string archiveFilePath) : base(archiveShellObject, archiveFilePath) => ProcessPathCollectionFactory = archiveShellObject.ProcessPathCollectionFactory;

        public ShellObjectInfoItemProvider(in IPortableDevice portableDevice, in IProcessPathCollectionFactory processPathCollectionFactory, in ClientVersion clientVersion)
        {
            PortableDevice = portableDevice;

            ProcessPathCollectionFactory = processPathCollectionFactory;

            ClientVersion = clientVersion;
        }

        public ShellObjectInfoItemProvider(in NonShellObjectRootItemType nonShellObjectRootItemType, in IProcessPathCollectionFactory processPathCollectionFactory, in ClientVersion clientVersion)
        {
            NonShellObjectRootItemType = nonShellObjectRootItemType;

            ProcessPathCollectionFactory = processPathCollectionFactory;

            ClientVersion = clientVersion;
        }

        public static ShellObjectInfoItemProvider ToShellObjectInfoItemProvider(ArchiveItemInfoItemProvider archiveItemInfoItemProvider) => (archiveItemInfoItemProvider ?? throw GetArgumentNullException(nameof(archiveItemInfoItemProvider))).ArchiveFileInfo.HasValue ? new ShellObjectInfoItemProvider(archiveItemInfoItemProvider.ShellObjectInfo, archiveItemInfoItemProvider.ArchiveFileInfo) : new ShellObjectInfoItemProvider(archiveItemInfoItemProvider.ShellObjectInfo, archiveItemInfoItemProvider.ArchiveFilePath);
    }
}
