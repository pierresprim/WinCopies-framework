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

using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;

namespace WinCopies.IO
{
    public struct ShellObjectInfoEnumeratorStruct
    {
        public ShellObject ShellObject { get; }

        public IPortableDevice PortableDevice { get; }

        public ArchiveFileInfoEnumeratorStruct? ArchiveFileInfoEnumeratorStruct { get; }

        internal ShellObjectInfoEnumeratorStruct(ShellObject shellObject)
        {
            ShellObject = shellObject;

            PortableDevice = null;

            ArchiveFileInfoEnumeratorStruct = null;
        }

        internal ShellObjectInfoEnumeratorStruct(IPortableDevice portableDevice)
        {
            ShellObject = null;

            PortableDevice = portableDevice;

            ArchiveFileInfoEnumeratorStruct = null;
        }

        internal ShellObjectInfoEnumeratorStruct(ArchiveFileInfoEnumeratorStruct archiveFileInfoEnumeratorStruct)
        {
            ShellObject = null;

            PortableDevice = null;

            ArchiveFileInfoEnumeratorStruct = archiveFileInfoEnumeratorStruct;
        }
    }
}
