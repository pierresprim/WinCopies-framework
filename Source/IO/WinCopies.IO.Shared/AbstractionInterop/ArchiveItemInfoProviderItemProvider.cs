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

using SevenZip;

using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.AbstractionInterop
{
    public abstract class ArchiveItemInfoProviderItemProvider
    {
        public IShellObjectInfoBase ShellObjectInfo { get; }

        public ArchiveFileInfo? ArchiveFileInfo { get; }

        public string ArchiveFilePath { get; }

        public ArchiveItemInfoProviderItemProvider(in IShellObjectInfoBase archiveShellObject, in ArchiveFileInfo? archiveFileInfo)
        {
            ShellObjectInfo = archiveShellObject;

            ArchiveFileInfo = archiveFileInfo;
        }

        public ArchiveItemInfoProviderItemProvider(in IShellObjectInfoBase archiveShellObject, in string archiveFilePath)
        {
            ShellObjectInfo = archiveShellObject;

            ArchiveFilePath = archiveFilePath;
        }

        public ArchiveItemInfoProviderItemProvider() { /* Left empty. */ }
    }
}
