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

using System;
using System.Globalization;

using WinCopies.Collections;
using WinCopies.IO.ObjectModel;
using WinCopies.Util;

using static WinCopies.Util.Util;

namespace WinCopies.IO
{
    public class FileSystemObjectComparer<T> : Comparer<T>, IFileSystemObjectComparer<T> where T : IFileSystemObject
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
            if (x.ItemFileSystemType != y.ItemFileSystemType) return CompareLocalizedNames(x, y);

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

    public class FileSystemObjectInfoComparer<T> : FileSystemObjectComparer<T>, IFileSystemObjectComparer<T> where T : IFileSystemObject
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

        public FileSystemObjectInfoComparer() : base() { }

        public FileSystemObjectInfoComparer(StringComparer stringComparer) : base(stringComparer) { }

        protected override int CompareOverride(T x, T y)
        {
            int? result = Validate(x, y);

            if (result.HasValue)

                return result.Value;

            if (x is IFileSystemObjectInfo _x && y is IFileSystemObjectInfo _y)
            {
                if (_x.FileType == _y.FileType) return CompareLocalizedNames(x, y);

                if (_x.FileType.IsValidEnumValue())
                {
#if !CS7
                    static
#endif
                        FileType[] getFileItemTypes() => new FileType[] { FileType.File, FileType.Archive, FileType.Library, FileType.Link };

                    if (_y.FileType.IsValidEnumValue())
                    {
                        FileType[] fileTypes = getFileItemTypes();

                        if (_x.FileType.IsValidEnumValue(true, fileTypes))

                            return _y.FileType.IsValidEnumValue(true, fileTypes) ? CompareLocalizedNames(x, y) : 1;

                        fileTypes = new FileType[] { FileType.Folder, FileType.KnownFolder, FileType.Drive, FileType.Other };

                        if (_x.FileType.IsValidEnumValue(true, fileTypes))

                            return _y.FileType.IsValidEnumValue(true, fileTypes) ? CompareLocalizedNames(x, y) : -1;
                    }

                    return 1;
                }

                if (_y.FileType.IsValidEnumValue()) return -1;
            }

            return CompareLocalizedNames(x, y);
        }
    }

    public class RegistryItemInfoComparer<T> : FileSystemObjectComparer<T> where T : IFileSystemObject
    {
        protected override int CompareOverride(T x, T y)
        {
            int? result = Validate(x, y);

            if (result.HasValue)

                return result.Value;

            if (x is IRegistryItemInfo _x && y is IRegistryItemInfo _y)
            {
                if (_x.RegistryItemType == _y.RegistryItemType) return CompareLocalizedNames(x, y);

                if (_x.RegistryItemType.IsValidEnumValue())
                {
                    if (_y.RegistryItemType.IsValidEnumValue())

                        switch (_x.RegistryItemType)
                        {
                            case RegistryItemType.Key:

                                return _y.RegistryItemType == RegistryItemType.Value ? -1 : 1;

                            case RegistryItemType.Value:

                                return _y.RegistryItemType == RegistryItemType.Key ? 1 : -1;

                            case RegistryItemType.Root:

                                return -1;
                        }

                    return 1;
                }

                if (_y.RegistryItemType.IsValidEnumValue()) return -1;
            }

            return CompareLocalizedNames(x, y);
        }
    }

    public class WMIItemInfoComparer<T> : FileSystemObjectComparer<T> where T : IFileSystemObject
    {
        protected override int CompareOverride(T x, T y)
        {
            int? result = Validate(x, y);

            if (result.HasValue)

                return result.Value;

            if (x is IWMIItemInfo _x && y is IWMIItemInfo _y)
            {
                if (_x.WMIItemType == _y.WMIItemType) return CompareLocalizedNames(x, y);

                if (_x.WMIItemType.IsValidEnumValue())
                {
                    if (_y.WMIItemType.IsValidEnumValue())

                        switch (_x.WMIItemType)
                        {
                            case WMIItemType.Class:

                                return _y.WMIItemType == WMIItemType.Instance ? -1 : 1;

                            case WMIItemType.Instance:

                                return _y.WMIItemType == WMIItemType.Class ? 1 : -1;

                            case WMIItemType.Namespace:

                                return -1;
                        }

                    return 1;
                }

                if (_y.WMIItemType.IsValidEnumValue()) return -1;
            }

            return CompareLocalizedNames(x, y);
        }
    }
}
