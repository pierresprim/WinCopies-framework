﻿/* Copyright © Pierre Sprimont, 2020
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
using System;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public interface IBrowsableObjectInfoFactory
    {
        ClientVersion ClientVersion { get; }

        IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo);

        IBrowsableObjectInfo GetBrowsableObjectInfo(string path);
    }

    public class BrowsableObjectInfoFactory : IBrowsableObjectInfoFactory
    {
        public ClientVersion ClientVersion { get; }

        public BrowsableObjectInfoFactory(ClientVersion clientVersion) => ClientVersion = clientVersion;

        public virtual IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) => new BrowsableObjectInfoViewModel(browsableObjectInfo);

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(string path)
        {
            if (Path.IsFileSystemPath(path))

                return ShellObjectInfo.From(ShellObject.FromParsingName(path), ClientVersion);

            else if (Path.IsRegistryPath(path))

                return new RegistryItemInfo(path);

            throw new ArgumentException("The factory cannot create an object for the given path.");
        }
    }
}
