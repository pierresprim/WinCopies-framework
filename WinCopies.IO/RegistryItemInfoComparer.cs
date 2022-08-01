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

using WinCopies.Collections;
using WinCopies.Util;
using static WinCopies.Util.Util;

namespace WinCopies.IO
{
    public class RegistryItemInfoComparer<T> : FileSystemObjectComparer<T> where T : IRegistryItemInfo

    {

#pragma warning disable CS0649 // Set up using reflection
        private readonly IFileSystemObjectComparer<IFileSystemObject> _fileSystemObjectComparer;
#pragma warning restore CS0649

        public IFileSystemObjectComparer<IFileSystemObject> FileSystemObjectComparer { get => _fileSystemObjectComparer; set => this.SetField(nameof(_fileSystemObjectComparer), value, typeof(RegistryItemInfoComparer<T>), paramName: nameof(value), setOnlyIfNotNull: true, throwIfNull: true); }

        public RegistryItemInfoComparer() : this(FileSystemObject.GetDefaultComparer()) { }

        public RegistryItemInfoComparer(IFileSystemObjectComparer<IFileSystemObject> fileSystemObjectComparer) => FileSystemObjectComparer = fileSystemObjectComparer;

        protected override int CompareOverride( T x, T y)
        {

            int result = GetIf(x.RegistryItemType, y.RegistryItemType, (RegistryItemType _x, RegistryItemType _y) => _x.CompareTo(_y), () => -1, () => 0, () => 1);

            return result == 0 ? FileSystemObjectComparer.Compare(x, y) : result;

        }

    }
}
