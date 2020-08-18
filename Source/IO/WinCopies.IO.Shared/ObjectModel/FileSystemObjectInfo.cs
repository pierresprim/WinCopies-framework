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
using Microsoft.WindowsAPICodePack.Win32Native.Shell;

using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using TsudaKageyu;

using WinCopies.Collections;
using WinCopies.IO.ObjectModel;

namespace WinCopies.IO
{

    public class FileSystemObjectInfoProperties<T> : BrowsableObjectInfoProperties<T>, IFileSystemObjectInfoProperties where T : IFileSystemObjectInfo
    {
        ///// <summary>
        ///// The file type of this <see cref="FileSystemObject"/>.
        ///// </summary>
        public FileType FileType => BrowsableObjectInfo.FileType;

        public FileSystemObjectInfoProperties(T fileSystemObjectInfo) : base(fileSystemObjectInfo)
        {
            // Left empty.
        }
    }

    //public class FileSystemObjectInfoPropertiesCommon : FileSystemObjectInfoProperties
    //{
    //    private IFileSystemObjectInfo _fileSystemObjectInfo;

    //    public sealed override FileType FileType => _fileSystemObjectInfo.FileType;

    //    public FileSystemObjectInfoPropertiesCommon(IFileSystemObjectInfo fileSystemObjectInfo) => _fileSystemObjectInfo = fileSystemObjectInfo;
    //}

    namespace ObjectModel
    {
        public abstract class FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> : BrowsableObjectInfo<TObjectProperties, TEncapsulatedObject>, IFileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> where TObjectProperties : IFileSystemObjectInfoProperties
        {
            public override bool IsRecursivelyBrowsable { get; } = true;

            public sealed override bool HasProperties => true;

            public abstract FileType FileType { get; }

            protected FileSystemObjectInfo(in string path) : base(path)
            {
                // Left empty.
            }

            // /// <param name="fileType">The <see cref="FileType"/> of this <see cref="BrowsableObjectInfo"/>.</param>
            protected FileSystemObjectInfo(in string path, in ClientVersion? clientVersion) : base(path, clientVersion)
            {
                // Left empty.
            }

            #region Methods
            #region Helpers
            /*/// <summary>
            /// Gets a default comparer for <see cref="FileSystemObjectInfo{T}"/>s.
            /// </summary>
            /// <returns>A default comparer for <see cref="FileSystemObjectInfo{T}"/>s.</returns>
            public static FileSystemObjectInfoComparer<IFileSystemObjectInfo> GetDefaultComparer() => new FileSystemObjectInfoComparer<IFileSystemObjectInfo>();*/

            public static string GetItemTypeName(string extension, FileType fileType) => fileType == FileType.Folder
                        ? FileOperation.GetFileInfo(string.Empty, FileAttributes.Directory, GetFileInfoOptions.TypeName).TypeName
                        : FileOperation.GetFileInfo(extension, FileAttributes.Normal, GetFileInfoOptions.TypeName).TypeName;
            #endregion

            #region TryGetIcon/BitmapSource
            #region Helpers
            private static Icon TryGetIcon(in int index, in System.Drawing.Size size) => TryGetIcon(index, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);

            public static Icon TryGetIcon(in string extension, in FileType fileType, in System.Drawing.Size size) =>

               // if (System.IO.Path.HasExtension(Path))

               fileType == FileType.Folder ? TryGetIcon(3, size) : FileOperation.GetFileInfo(extension, FileAttributes.Normal, GetFileInfoOptions.Icon | GetFileInfoOptions.UseFileAttributes).Icon?.TryGetIcon(size, 32, true, true) ?? TryGetIcon(0, size);// else// return TryGetIcon(FileType == FileType.Folder ? 3 : 0, "SHELL32.dll", size);

            public static BitmapSource TryGetBitmapSource(in string extension, in FileType fileType, in int size)
            {
#if NETFRAMEWORK

            using (Icon icon = TryGetIcon(extension, fileType, new System.Drawing.Size(size, size)))

#else

                using Icon icon = TryGetIcon(extension, fileType, new System.Drawing.Size(size, size));

#endif
                return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            #endregion
            #endregion

            public Icon TryGetIcon(in int size) => TryGetIcon(System.IO.Path.GetExtension(Path), ObjectPropertiesGeneric.FileType, new System.Drawing.Size(size, size));

            public BitmapSource TryGetBitmapSource(in int size) => TryGetBitmapSource(System.IO.Path.GetExtension(Path), ObjectPropertiesGeneric.FileType, size);

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

            public override IEqualityComparer<IFileSystemObject> GetDefaultEqualityComparer() => new FileSystemObjectInfoEqualityComparer<IFileSystemObject>();

            public override System.Collections.Generic.IComparer<IFileSystemObject> GetDefaultComparer() => new FileSystemObjectInfoComparer<IFileSystemObject>();
            #endregion
            #endregion

            #region Operators
            /// <summary>
            /// Checks if two <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/>s are equal.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the two <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/>s are equal.</returns>
            public static bool operator ==(in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> left, in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> right) => left is null ? right is null : left.Equals(right);

            /// <summary>
            /// Checks if two <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/>s are different.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the two <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/>s are different.</returns>
            public static bool operator !=(in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> left, in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> right) => !(left == right);

            /// <summary>
            /// Checks if a given <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> is lesser than an other <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/>.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> is lesser than the <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> to compare with.</returns>
            public static bool operator <(in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> left, in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> right) => left is null ? right is object : left.CompareTo(right) < 0;

            /// <summary>
            /// Checks if a given <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> is lesser or equal to an other <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/>.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> is lesser or equal to the <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> to compare with.</returns>
            public static bool operator <=(in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> left, in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> right) => left is null || left.CompareTo(right) <= 0;

            /// <summary>
            /// Checks if a given <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> is greater than an other <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/>.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> is greater than the <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> to compare with.</returns>
            public static bool operator >(in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> left, in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> right) => left is object && left.CompareTo(right) > 0;

            /// <summary>
            /// Checks if a given <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> is greater or equal to an other <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/>.
            /// </summary>
            /// <param name="left">Left operand.</param>
            /// <param name="right">Right operand.</param>
            /// <returns>A <see cref="bool"/> value that indicates whether the given <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> is greater or equal to the <see cref="FileSystemObjectInfo{TObjectProperties, TEncapsulatedObject}"/> to compare with.</returns>
            public static bool operator >=(in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> left, in FileSystemObjectInfo<TObjectProperties, TEncapsulatedObject> right) => left is null ? right is null : left.CompareTo(right) >= 0;
            #endregion
        }
    }
}
