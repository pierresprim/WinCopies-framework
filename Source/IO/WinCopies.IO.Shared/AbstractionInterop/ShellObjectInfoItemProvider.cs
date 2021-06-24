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

using System.Drawing;

using WinCopies.GUI.Drawing;
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

        public ShellObjectInfoItemProvider(in ShellObject shellObject, in ClientVersion clientVersion)
        {
            ShellObject = shellObject;

            ClientVersion = clientVersion;
        }

        public ShellObjectInfoItemProvider(in IShellObjectInfoBase archiveShellObject, in ArchiveFileInfo? archiveFileInfo) : base(archiveShellObject, archiveFileInfo)
        {
            // Left empty.
        }

        public ShellObjectInfoItemProvider(in IShellObjectInfoBase archiveShellObject, in string archiveFilePath) : base(archiveShellObject, archiveFilePath)
        {
            // Left empty.
        }

        public ShellObjectInfoItemProvider(in IPortableDevice portableDevice, in ClientVersion clientVersion)
        {
            PortableDevice = portableDevice;

            ClientVersion = clientVersion;
        }

        public ShellObjectInfoItemProvider(in NonShellObjectRootItemType nonShellObjectRootItemType, in ClientVersion clientVersion)
        {
            NonShellObjectRootItemType = nonShellObjectRootItemType;

            ClientVersion = clientVersion;
        }

        public static ShellObjectInfoItemProvider ToShellObjectInfoItemProvider(ArchiveItemInfoItemProvider archiveItemInfoItemProvider) => (archiveItemInfoItemProvider ?? throw GetArgumentNullException(nameof(archiveItemInfoItemProvider))).ArchiveFileInfo.HasValue ? new ShellObjectInfoItemProvider(archiveItemInfoItemProvider.ShellObjectInfo, archiveItemInfoItemProvider.ArchiveFileInfo) : new ShellObjectInfoItemProvider(archiveItemInfoItemProvider.ShellObjectInfo, archiveItemInfoItemProvider.ArchiveFilePath);
    }

    public class MultiIconInfoItemProvider
    {
        public SingleIcon Icon { get; }

        public MultiIconInfo MultiIconInfo { get; }

        public MultiIconInfoItemProvider(in SingleIcon icon, in MultiIconInfo multiIconInfo)
        {
            Icon = icon;

            MultiIconInfo = multiIconInfo;
        }
    }

    public class SingleIconInfoItemProvider
    {
        public IconImage Icon { get; }

        public SingleIconInfo Parent { get; }

        public SingleIconInfoItemProvider(in IconImage icon, in SingleIconInfo parent)
        {
            Icon = icon;

            Parent = parent;
        }
    }

    public class IconImageInfoItemProvider
    {
        public Bitmap Bitmap { get; }

        public Icon Icon { get; }

        public string Name { get; } 

        public IconImageInfo Parent { get; }

        private IconImageInfoItemProvider(in string name, in IconImageInfo parent)
        {
            Name = name;

            Parent = parent;
        }

        public IconImageInfoItemProvider( in string name, in Bitmap bitmap, in IconImageInfo iconImageInfo) : this(name, iconImageInfo) => Bitmap = bitmap;

        public IconImageInfoItemProvider(in string name, in Icon icon, in IconImageInfo iconImageInfo) : this(name, iconImageInfo) => Icon = icon;
    }
}
