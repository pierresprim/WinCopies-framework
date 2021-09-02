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

using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;

using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.GUI.Drawing;
using WinCopies.IO.PropertySystem;

namespace WinCopies.IO
{
    //public class FileSystemObjectInfoPropertiesCommon : FileSystemObjectInfoProperties
    //{
    //    private IFileSystemObjectInfo _fileSystemObjectInfo;

    //    public sealed override FileType FileType => _fileSystemObjectInfo.FileType;

    //    public FileSystemObjectInfoPropertiesCommon(IFileSystemObjectInfo fileSystemObjectInfo) => _fileSystemObjectInfo = fileSystemObjectInfo;
    //}

    namespace ObjectModel
    {
        public abstract class FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IFileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            #region Properties
            protected override bool IsRecursivelyBrowsableOverride => true;

            // public override Predicate<TPredicateTypeParameter> RootItemsPredicate => null;

            //public override Predicate<IBrowsableObjectInfo> RootItemsBrowsableObjectInfoPredicate => item => item.Browsability?.Browsability == IO.Browsability.BrowsableByDefault;
            #endregion

            // /// <param name="fileType">The <see cref="FileType"/> of this <see cref="BrowsableObjectInfo"/>.</param>
            protected FileSystemObjectInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }

            #region Methods
            #region Helpers
            /*/// <summary>
            /// Gets a default comparer for <see cref="FileSystemObjectInfo{T}"/>s.
            /// </summary>
            /// <returns>A default comparer for <see cref="FileSystemObjectInfo{T}"/>s.</returns>
            public static FileSystemObjectInfoComparer<IFileSystemObjectInfo> GetDefaultComparer() => new FileSystemObjectInfoComparer<IFileSystemObjectInfo>();*/

            #region TryGetIcon/BitmapSource
            public Icon TryGetIcon(in int size) => FileSystemObjectInfo.TryGetIcon(System.IO.Path.GetExtension(Path), ObjectPropertiesGeneric.FileType, new System.Drawing.Size(size, size));

            public BitmapSource TryGetBitmapSource(in int size) => FileSystemObjectInfo.TryGetBitmapSource(System.IO.Path.GetExtension(Path), ObjectPropertiesGeneric.FileType, size);
            #endregion
            #endregion

            /*#region Equatable methods
            /// <summary>
            /// Determines whether the specified <see cref="IFileSystemObjectInfo"/> is equal to the current object by calling the <see cref="Equals(object)"/> method.
            /// </summary>
            /// <param name="FileSystemObjectInfo{T}">The <see cref="IFileSystemObjectInfo"/> to compare with the current object.</param>
            /// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
            public virtual bool Equals(IFileSystemObjectInfo fileSystemObjectInfo) => fileSystemObjectInfo is null ? false : ReferenceEquals(this, fileSystemObjectInfo) || (FileType == fileSystemObjectInfo.FileType && Path.ToLower(CultureInfo.CurrentCulture) == fileSystemObjectInfo.Path.ToLower(CultureInfo.CurrentCulture));

            public override bool Equals(IFileSystemObject fileSystemObject) => fileSystemObject is IFileSystemObjectInfo fileSystemObjectInfo && Equals(fileSystemObjectInfo) ;
#endregion

            /// <summary>
            /// Compares the current object to a given <see cref="FileSystemObjectInfo{T}"/>.
            /// </summary>
            /// <param name="FileSystemObjectInfo{T}">The <see cref="FileSystemObjectInfo{T}"/> to compare with.</param>
            /// <returns>The comparison result. See <see cref="IComparable{T}.CompareTo(T)"/> for more details.</returns>
            public virtual int CompareTo(IFileSystemObjectInfo fileSystemObjectInfo) => GetDefaultComparer().Compare(this, fileSystemObjectInfo);*/

            #region Overrides
            /*/// <summary>
            /// Gets an hash code for this <see cref="FileSystemObjectInfo{T}"/>.
            /// </summary>
            /// <returns>The hash code of the <see cref="FileType"/> and the <see cref="FileSystemObject.Path"/> property.</returns>
            public override int GetHashCode() => FileType.GetHashCode() ^ Path.ToLower().GetHashCode();

            /// <summary>
            /// Gets a string representation of this <see cref="FileSystemObjectInfo{T}"/>.
            /// </summary>
            /// <returns>The <see cref="FileSystemObject.LocalizedName"/> of this <see cref="FileSystemObjectInfo{T}"/>.</returns>
            public override string ToString() => IsNullEmptyOrWhiteSpace(LocalizedName) ? Path : LocalizedName;*/

            public override IEqualityComparer<IBrowsableObjectInfoBase> GetDefaultEqualityComparer() => new FileSystemObjectInfoEqualityComparer<IBrowsableObjectInfoBase>();

            public override IComparer<IBrowsableObjectInfoBase> GetDefaultComparer() => new FileSystemObjectInfoComparer<IBrowsableObjectInfoBase>();

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => FileSystemObjectInfo.GetRootItems();

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => GetItems().Where(item => item.Browsability?.Browsability == IO.Browsability.BrowsableByDefault);
            #endregion
            #endregion

            #region Operators
            /// <summary>
            /// Checks if two <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>s are equal.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the two <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>s are equal.</returns>
            public static bool operator ==(in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> left, in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> right) => left is null ? right is null : left.Equals(right);

            /// <summary>
            /// Checks if two <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>s are different.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the two <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>s are different.</returns>
            public static bool operator !=(in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> left, in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> right) => !(left == right);

            /// <summary>
            /// Checks if a given <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> is lesser than an other <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> is lesser than the <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> to compare with.</returns>
            public static bool operator <(in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> left, in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> right) => left is null ? right is object : left.CompareTo(right) < 0;

            /// <summary>
            /// Checks if a given <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> is lesser or equal to an other <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> is lesser or equal to the <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> to compare with.</returns>
            public static bool operator <=(in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> left, in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> right) => left is null || left.CompareTo(right) <= 0;

            /// <summary>
            /// Checks if a given <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> is greater than an other <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> is greater than the <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> to compare with.</returns>
            public static bool operator >(in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> left, in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> right) => left is object && left.CompareTo(right) > 0;

            /// <summary>
            /// Checks if a given <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> is greater or equal to an other <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> is greater or equal to the <see cref="FileSystemObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> to compare with.</returns>
            public static bool operator >=(in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> left, in FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> right) => left is null ? right is null : left.CompareTo(right) >= 0;
            #endregion
        }

        public static class FileSystemObjectInfo
        {
            public static ArrayBuilder<IBrowsableObjectInfo> GetRootItems()
            {
                var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

                ClientVersion clientVersion = BrowsableObjectInfo.GetDefaultClientVersion();

                void add(in IKnownFolder knownFolder) => arrayBuilder.AddLast(new ShellObjectInfo(knownFolder, clientVersion));

                add(KnownFolders.UserPinned);
                add(KnownFolders.Desktop);
                add(KnownFolders.Libraries);
                add(KnownFolders.Profile);
                add(KnownFolders.Computer);
                add(KnownFolders.RecycleBin);

                return arrayBuilder;
            }

            private static Icon TryGetIcon(in int index, in System.Drawing.Size size) => Shell.ObjectModel.BrowsableObjectInfo.TryGetIcon(index, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);

            public static string GetItemTypeName(in string extension, in FileType fileType) => fileType == FileType.Folder
                        ? FileOperation.GetFileInfo(string.Empty, FileAttributes.Directory, GetFileInfoOptions.TypeName).TypeName
                        : FileOperation.GetFileInfo(extension, FileAttributes.Normal, GetFileInfoOptions.TypeName).TypeName;

            public static Icon TryGetIcon(in string extension, in FileType fileType, in System.Drawing.Size size) =>

               // if (System.IO.Path.HasExtension(Path))

               fileType == FileType.Folder ? TryGetIcon(3, size) : FileOperation.GetFileInfo(extension, FileAttributes.Normal, GetFileInfoOptions.Icon | GetFileInfoOptions.UseFileAttributes).Icon?.TryGetIcon(size, 32, true, true) ?? TryGetIcon(0, size);// else// return TryGetIcon(FileType == FileType.Folder ? 3 : 0, "SHELL32.dll", size);

            public static BitmapSource TryGetBitmapSource(in string extension, in FileType fileType, in int size)
            {
                using
#if !CS8
                    (
#endif

                Icon icon = TryGetIcon(extension, fileType, new System.Drawing.Size(size, size))

#if CS8
                    ;
#else
                    )
#endif
                return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }
    }
}
