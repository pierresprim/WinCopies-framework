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

using System;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.Selectors
{
    public class ShellObjectInfoSelectorDictionary : BrowsableObjectInfoSelectorDictionary<ShellObjectInfoItemProvider>
    {
        public static IBrowsableObjectInfo Convert(ShellObjectInfoItemProvider item)
        {
            if (item.ShellObject != null)

                return ShellObjectInfo.From(item.ShellObject, item.ClientVersion.Value);

            if (item.ArchiveFileInfo.HasValue)

                return ArchiveItemInfo.From(item.ShellObjectInfo, item.ArchiveFileInfo.Value);

            if (!UtilHelpers.IsNullEmptyOrWhiteSpace(item.ArchiveFilePath))

                return ArchiveItemInfo.From(item.ShellObjectInfo, item.ArchiveFilePath);

            if (item.PortableDevice != null)

                return new PortableDeviceInfo(item.PortableDevice, item.ClientVersion.Value);

            if (item.NonShellObjectRootItemType == NonShellObjectRootItemType.Registry)

                return new RegistryItemInfo(item.ClientVersion.Value);

            return item.NonShellObjectRootItemType == NonShellObjectRootItemType.WMI
                ? new WMIItemInfo(null, item.ClientVersion.Value)
                : throw new ArgumentException("The given item provider or its current configuration is not supported.");
        }

        protected override Converter<ShellObjectInfoItemProvider, IBrowsableObjectInfo> DefaultSelectorOverride => Convert;

        public ShellObjectInfoSelectorDictionary() { /* Left empty. */ }
    }
}
