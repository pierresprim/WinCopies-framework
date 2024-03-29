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

using System;
using System.Globalization;

using WinCopies.Collections;
using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.Util;

namespace WinCopies.IO
{
    public class FileSystemObjectComparer<T> : Comparer<T>, IFileSystemObjectComparer<T> where T : IBrowsableObjectInfoBase
    {
        //public virtual bool NeedsObjectsOrValuesReconstruction => true; // True because of the StirngComparer property.

        //protected virtual void OnDeepClone(FileSystemObjectComparer<T> fileSystemObjectComparer) { }

        //protected virtual FileSystemObjectComparer<T> DeepCloneOverride() => new FileSystemObjectComparer<T>(_stringComparerDelegate);

        //public object DeepClone()
        //{
        //    FileSystemObjectComparer<T> fileSystemObjectComparer = DeepCloneOverride();

        //    OnDeepClone(fileSystemObjectComparer);

        //    return fileSystemObjectComparer;
        //}

        //private readonly DeepClone<StringComparer> _stringComparerDelegate;

        public StringComparer StringComparer { get; }

        public FileSystemObjectComparer() : this(StringComparer.Create(CultureInfo.CurrentCulture, true)) { }

        public FileSystemObjectComparer(StringComparer stringComparer) => StringComparer = stringComparer;

        public int? Validate(in T x, in T y)
        {
            if (x == null) return y == null ? 0 : -1;

            if (y == null) return 1;

            return null;
        }

        public int CompareLocalizedNames(in T x, in T y) => StringComparer.Compare(x.LocalizedName.RemoveAccents(), y.LocalizedName.RemoveAccents());

        // public int Compare(T x, IFileSystemObject y) => y is T _y ? CompareOverride(x, _y) : CompareFileSystemTypesLocalizedNames(x, y);

        protected override int CompareOverride(T x, T y)
        {
            int? result = Validate(x, y);

            return result.HasValue ? result.Value : CompareLocalizedNames(x, y);
        }
    }

    public class FileSystemObjectInfoComparer<T> : FileSystemObjectComparer<T>, IFileSystemObjectComparer<T> where T : IBrowsableObjectInfoBase
    {
        //public virtual bool NeedsObjectsOrValuesReconstruction => true; // True because of the StirngComparer property.

        //protected virtual void OnDeepClone(FileSystemObjectComparer<T> fileSystemObjectComparer) { }

        //protected virtual FileSystemObjectComparer<T> DeepCloneOverride() => new FileSystemObjectComparer<T>(_stringComparerDelegate);

        //public object DeepClone()
        //{
        //    FileSystemObjectComparer<T> fileSystemObjectComparer = DeepCloneOverride();

        //    OnDeepClone(fileSystemObjectComparer);

        //    return fileSystemObjectComparer;
        //}

        //private readonly DeepClone<StringComparer> _stringComparerDelegate;

        //public StringComparer StringComparer { get; }

        public static FileType[] FolderItemTypes { get; } = new FileType[] { FileType.Folder, FileType.KnownFolder, FileType.Other };

        public static FileType[] FileItemTypes { get; } = new FileType[] { FileType.File, FileType.Archive, FileType.Library, FileType.Link };

        public FileSystemObjectInfoComparer() : base() { }

        public FileSystemObjectInfoComparer(StringComparer stringComparer) : base(stringComparer) { }

        protected override int CompareOverride(T x, T y)
        {
            int? result = Validate(x, y);

            if (result.HasValue)

                return result.Value;

            if (x is IBrowsableObjectInfo<IFileSystemObjectInfoProperties> _x && y is IBrowsableObjectInfo<IFileSystemObjectInfoProperties> _y)
            {
                FileType xFileType = _x.ObjectProperties.FileType;
                FileType yFileType = _y.ObjectProperties.FileType;

                if (xFileType == yFileType) return CompareLocalizedNames(x, y);

                if (xFileType.IsValidEnumValue())

                    return yFileType.IsValidEnumValue()
                        ? xFileType.IsValidEnumValue(true, FileItemTypes)
                            ? yFileType.IsValidEnumValue(true, FileItemTypes) ? CompareLocalizedNames(x, y) : 1
                            : xFileType.IsValidEnumValue(true, FolderItemTypes)
                            ? yFileType.IsValidEnumValue(true, FolderItemTypes) ? CompareLocalizedNames(x, y) : -1
                            : yFileType == FileType.Drive ? CompareLocalizedNames(x, y) : 1
                        : 1;

                if (yFileType.IsValidEnumValue()) return -1;
            }

            return CompareLocalizedNames(x, y);
        }
    }
}
