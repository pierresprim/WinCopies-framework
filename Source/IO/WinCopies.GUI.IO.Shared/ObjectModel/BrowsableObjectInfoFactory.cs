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

using Microsoft.WindowsAPICodePack.Shell;

using System;

using WinCopies.IO;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public interface IBrowsableObjectInfoFactory
    {
        ClientVersion ClientVersion { get; }

        Comparison<IBrowsableObjectInfo> SortComparison { get; set; }

        IBrowsableObjectInfo GetBrowsableObjectInfo(string path);

        IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo);

        IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(string path);

        IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo, IBrowsableObjectInfoViewModel parent);
    }

    public class BrowsableObjectInfoFactory : IBrowsableObjectInfoFactory
    {
        /// <summary>
        /// Gets the <see cref="Microsoft.WindowsAPICodePack.PortableDevices.ClientVersion"/> value associated to this factory. This value is used for <see cref="PortableDeviceInfo"/> and <see cref="PortableDeviceItemInfo"/> creation when browsing the Computer folder with a <see cref="ShellObjectInfo"/> item.
        /// </summary>
        public WinCopies.IO.ClientVersion ClientVersion { get; }

        public Comparison<IBrowsableObjectInfo> SortComparison { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfoFactory"/> class.
        /// </summary>
        /// <param name="clientVersion">The <see cref="Microsoft.WindowsAPICodePack.PortableDevices.ClientVersion"/> value for PortableDevice items creation. See <see cref="ClientVersion"/>.</param>
        public BrowsableObjectInfoFactory(in WinCopies.IO.ClientVersion clientVersion) => ClientVersion = clientVersion;

        public BrowsableObjectInfoFactory() : this(WinCopies.IO.ObjectModel.BrowsableObjectInfo.GetDefaultClientVersion()) { /* Left empty. */ }

        /// <summary>
        /// Creates an <see cref="IBrowsableObjectInfo"/> for a given path. See Remarks section.
        /// </summary>
        /// <param name="path">The path of the <see cref="IBrowsableObjectInfo"/> to create.</param>
        /// <returns>An <see cref="IBrowsableObjectInfo"/> for <paramref name="path"/>.</returns>
        /// <remarks>This method cannot create <see cref="IBrowsableObjectInfo"/> for WMI paths.</remarks>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a Shell or a Registry path.</exception>
        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(string path)
        {
            if (Path.IsFileSystemPath(path))

                return ShellObjectInfo.From(ShellObjectFactory.Create(path), ClientVersion);

            else if (WinCopies.IO.Shell.Path.IsRegistryPath(path))

                return new RegistryItemInfo(path, WinCopies.IO.ObjectModel.BrowsableObjectInfo.GetDefaultClientVersion());

            throw new ArgumentException("The factory cannot create an object for the given path.");
        }

        protected virtual IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo, bool rootParentIsRootNode) => new BrowsableObjectInfoViewModel(browsableObjectInfo, rootParentIsRootNode) { SortComparison = SortComparison, Factory = this };

        public virtual IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(string path) => GetBrowsableObjectInfoViewModel(GetBrowsableObjectInfo(path), false);

        public virtual IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) => GetBrowsableObjectInfoViewModel(browsableObjectInfo, true);

        public virtual IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo, IBrowsableObjectInfoViewModel parent) => GetBrowsableObjectInfoViewModel(browsableObjectInfo, parent.RootParentIsRootNode);
    }
}
