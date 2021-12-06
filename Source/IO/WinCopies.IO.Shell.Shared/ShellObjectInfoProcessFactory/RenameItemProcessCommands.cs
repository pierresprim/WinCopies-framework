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
using WinCopies.Util.Shared.Delegates;

using static WinCopies.IO.Shell.Resources.ExceptionMessages;
using static WinCopies.IO.FileType;

namespace WinCopies.IO
{
    public partial class ShellObjectInfoProcessFactory
    {
        protected class _RenameItemProcessCommands : IProcessCommand
        {
            private IShellObjectInfo<IFileSystemObjectInfoProperties> _shellObjectInfo;

            public bool IsDisposed => _shellObjectInfo == null;

            public string Name { get; } = "Rename";

            public string Caption { get; } = "New name:";

            public _RenameItemProcessCommands(in IShellObjectInfo<IFileSystemObjectInfoProperties> shellObjectInfo) => _shellObjectInfo = shellObjectInfo;

            private static bool RunCommand(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items, in Func<IBrowsableObjectInfo, bool> action) => ActionDelegates.RunFuncIfTRUE((out IBrowsableObjectInfo _obj) =>
           {
               _obj = null;

               if (items == null)

                   return false;

               foreach (IBrowsableObjectInfo item in items)
               {
                   if (_obj == null)

                       _obj = item;

                   else

                       return false;
               }

               return true;
           }, action, false, out _);

            public bool CanExecute(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items) => RunCommand(items, obj =>
            {
                if (obj is IShellObjectInfoBase2 shellObjectInfo && shellObjectInfo.ObjectProperties is IFileSystemObjectInfoProperties properties)

                    switch (properties.FileType)
                    {
                        case FileType.File:
                        case Folder:
                        case KnownFolder:
                        case Archive:
                        case Link:
                        case Drive:

                            return shellObjectInfo.InnerObject.IsFileSystemObject;
                    }

                return false;
            });

            public bool TryExecute(string parameter, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items, out IProcessParameters result)
            {
                result = null;

                return RunCommand(items, obj => Microsoft.WindowsAPICodePack.Win32Native.Shell.Shell.MoveFileW(obj.Path, $"{_shellObjectInfo.Path}\\{parameter}"));
            }

            public IProcessParameters Execute(string name, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items) => TryExecute(name, items, out IProcessParameters result)
                    ? result
                    : throw new InvalidOperationException(CouldNotCreateItem);

            public void Dispose() => _shellObjectInfo = null;

            ~_RenameItemProcessCommands() => Dispose();
        }
    }
}
