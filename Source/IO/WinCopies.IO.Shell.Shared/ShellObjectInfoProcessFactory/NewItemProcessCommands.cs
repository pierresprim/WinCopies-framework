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

using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;

using static WinCopies.IO.Shell.Resources.ExceptionMessages;
using static WinCopies.IO.FileType;

namespace WinCopies.IO
{
    public partial class ShellObjectInfoProcessFactory
    {
        protected class _NewItemProcessCommands : IProcessCommands
        {
            private IShellObjectInfo<IFileSystemObjectInfoProperties> _shellObjectInfo;

            public bool IsDisposed => _shellObjectInfo == null;

            public string Name { get; } = "New folder";

            public string Caption { get; } = "Folder name:";

            public _NewItemProcessCommands(in IShellObjectInfo<IFileSystemObjectInfoProperties> shellObjectInfo) => _shellObjectInfo = shellObjectInfo;

            public bool CanCreateNewItem()
            {
                switch (_shellObjectInfo.ObjectProperties.FileType)
                {
                    case Folder:
                    case KnownFolder:
                    case Drive:

                        return _shellObjectInfo.InnerObject.IsFileSystemObject;
                }

                return false;
            }

            public bool TryCreateNewItem(string parameter, out IProcessParameters result)
            {
                result = null;

                return Microsoft.WindowsAPICodePack.Win32Native.Shell.Shell.CreateDirectoryW($"{_shellObjectInfo.Path}\\{parameter}", IntPtr.Zero);
            }

            public IProcessParameters CreateNewItem(string name) => TryCreateNewItem(name, out IProcessParameters result)
                    ? result
                    : throw new InvalidOperationException(CouldNotCreateItem);

            public void Dispose() => _shellObjectInfo = null;

            ~_NewItemProcessCommands() => Dispose();
        }
    }
}
