/* Copyright © Pierre Sprimont, 2019
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

using static WinCopies.Util.Util;
using WinCopies.Collections;
using WinCopies.Util;

namespace WinCopies.IO
{
    public class WMIItemInfoComparer<T> : FileSystemObjectComparer<T> where T : IWMIItemInfo

    {

#pragma warning disable CS0649 // Set up using reflection
        private readonly FileSystemObjectComparer<IFileSystemObject> _fileSystemObjectComparer;
#pragma warning restore

        public FileSystemObjectComparer<IFileSystemObject> FileSystemObjectComparer { get => _fileSystemObjectComparer; set => this.SetField(nameof(_fileSystemObjectComparer), value, typeof(WMIItemInfoComparer<T>), paramName: nameof(value), setOnlyIfNotNull: true, throwIfNull: true); }

        public WMIItemInfoComparer() : this(FileSystemObject.GetDefaultComparer()) { }

        public WMIItemInfoComparer(FileSystemObjectComparer<IFileSystemObject> fileSystemObjectComparer) => FileSystemObjectComparer = fileSystemObjectComparer;

        protected override int CompareOverride(T x, T y)
        {

            int result = GetIf(x.WMIItemType, y.WMIItemType, (WMIItemType _x, WMIItemType _y) => _x.CompareTo(_y), () => -1, () => 1, () => 0);

            return result == 0 ? FileSystemObjectComparer.Compare(x, y) : result;

        }

    }

}
