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

using System;
using System.Globalization;
using WinCopies.Collections;
using WinCopies.Util;
using IDisposable = WinCopies.Util.IDisposable;

namespace WinCopies.IO
{
    public class FileSystemObjectComparer<T> : Comparer<T>, IFileSystemObjectComparer<T> where T : IFileSystemObject

    {

        public virtual bool NeedsObjectsOrValuesReconstruction => true; // True because of the StirngComparer property.

        protected virtual void OnDeepClone(FileSystemObjectComparer<T> fileSystemObjectComparer) { }

        protected virtual FileSystemObjectComparer<T> DeepCloneOverride() => new FileSystemObjectComparer<T>(_stringComparerDelegate);

        public object DeepClone()

        {

            FileSystemObjectComparer<T> fileSystemObjectComparer = DeepCloneOverride();

            OnDeepClone(fileSystemObjectComparer);

            return fileSystemObjectComparer;

        }

        private readonly DeepClone<StringComparer> _stringComparerDelegate;

        public StringComparer StringComparer { get; }

        public FileSystemObjectComparer() : this( stringComparer => StringComparer.Create(CultureInfo.CurrentCulture, true)) { }

        public FileSystemObjectComparer(DeepClone<StringComparer> stringComparerDelegate)
        {

            _stringComparerDelegate = stringComparerDelegate;

            StringComparer = stringComparerDelegate(null);

        }

        protected override int CompareOverride(T x, T y) => StringComparer.Compare(x.LocalizedName.RemoveAccents(), y.LocalizedName.RemoveAccents());

    }

    public class FileSystemObjectInfoComparer<T> : FileSystemObjectComparer<T>, IFileSystemObjectComparer<T> where T : IFileSystemObjectInfo

    {

        public virtual bool NeedsObjectsOrValuesReconstruction => true; // True because of the StirngComparer property.

        protected virtual void OnDeepClone(FileSystemObjectComparer<T> fileSystemObjectComparer) { }

        protected virtual FileSystemObjectComparer<T> DeepCloneOverride() => new FileSystemObjectComparer<T>(_stringComparerDelegate);

        public object DeepClone()

        {

            FileSystemObjectComparer<T> fileSystemObjectComparer = DeepCloneOverride();

            OnDeepClone(fileSystemObjectComparer);

            return fileSystemObjectComparer;

        }

        private readonly DeepClone<StringComparer> _stringComparerDelegate;

        public StringComparer StringComparer { get; }

        public FileSystemObjectInfoComparer() : this(stringComparer => StringComparer.Create(CultureInfo.CurrentCulture, true)) { }

        public FileSystemObjectInfoComparer(DeepClone<StringComparer> stringComparerDelegate)
        {

            _stringComparerDelegate = stringComparerDelegate;

            StringComparer = stringComparerDelegate(null);

        }

        protected override int CompareOverride(T x, T y) => x.FileType == y.FileType || (x.FileType == FileType.File && (y.FileType == FileType.Link || y.FileType == FileType.Archive)) || (y.FileType == FileType.File && (x.FileType == FileType.Link || x.FileType == FileType.Archive))
                ? StringComparer.Compare(x.LocalizedName.RemoveAccents(), y.LocalizedName.RemoveAccents())
                : (x.FileType == FileType.Folder || x.FileType == FileType.Drive) && (y.FileType == FileType.File || y.FileType == FileType.Archive || y.FileType == FileType.Link)
                ? -1
                : (x.FileType == FileType.File || x.FileType == FileType.Archive || x.FileType == FileType.Link) && (y.FileType == FileType.Folder || y.FileType == FileType.Drive)
                ? 1
                : 0;

    }
}
