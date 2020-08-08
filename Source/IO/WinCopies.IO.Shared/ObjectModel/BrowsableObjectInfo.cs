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


using Microsoft.WindowsAPICodePack.PortableDevices;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

using TsudaKageyu;

using WinCopies.Collections.Generic;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// The base class for all IO browsable objects of the WinCopies framework.
    /// </summary>
    public abstract class BrowsableObjectInfo : FileSystemObject, IBrowsableObjectInfo
    {
        #region Consts
        public const ushort SmallIconSize = 16;
        public const ushort MediumIconSize = 48;
        public const ushort LargeIconSize = 128;
        public const ushort ExtraLargeIconSize = 256;
        #endregion

        #region Properties
        IBrowsableObjectInfo ITreeEnumerable<IBrowsableObjectInfo>.Value => this;

        /// <summary>
        /// When overridden in a derived class, gets a value that indicates whether this <see cref="BrowsableObjectInfo"/> is browsable.
        /// </summary>
        public abstract bool IsBrowsable { get; }

        /// <summary>
        /// Gets the <see cref="IBrowsableObjectInfo"/> parent of this <see cref="BrowsableObjectInfo"/>. Returns <see langword="null"/> if this object is the root object of a hierarchy.
        /// </summary>
        public abstract IBrowsableObjectInfo Parent { get; }

        #region BitmapSources
        /// <summary>
        /// When overridden in a derived class, gets the small <see cref="BitmapSource"/> of this <see cref="BrowsableObjectInfo"/>.
        /// </summary>
        public abstract BitmapSource SmallBitmapSource { get; }

        /// <summary>
        /// When overridden in a derived class, gets the medium <see cref="BitmapSource"/> of this <see cref="BrowsableObjectInfo"/>.
        /// </summary>
        public abstract BitmapSource MediumBitmapSource { get; }

        /// <summary>
        /// When overridden in a derived class, gets the large <see cref="BitmapSource"/> of this <see cref="BrowsableObjectInfo"/>.
        /// </summary>
        public abstract BitmapSource LargeBitmapSource { get; }

        /// <summary>
        /// When overridden in a derived class, gets the extra large <see cref="BitmapSource"/> of this <see cref="BrowsableObjectInfo"/>.
        /// </summary>
        public abstract BitmapSource ExtraLargeBitmapSource { get; }
        #endregion

        public abstract string ItemTypeName { get; }

        public abstract string Description { get; }

        /// <summary>
        /// Gets the size for this <see cref="IBrowsableObjectInfo"/>.
        /// </summary>
        public abstract Size? Size { get; }

        public abstract bool IsSpecialItem { get; }

        public ClientVersion? ClientVersion { get; private set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfo"/> class.
        /// </summary>
        /// <param name="path">The path of the new item.</param>
        protected BrowsableObjectInfo(in string path) : this(path, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfo"/> class.
        /// </summary>
        /// <param name="path">The path of the new item.</param>
        /// <param name="clientVersion">The <see cref="Microsoft.WindowsAPICodePack.PortableDevices.ClientVersion"/> that will be used to initialize new <see cref="PortableDeviceInfo"/>s and <see cref="PortableDeviceObjectInfo"/>s.</param>
        protected BrowsableObjectInfo(in string path, in ClientVersion? clientVersion) : base(path) => ClientVersion = clientVersion;

        #region Methods
        internal static Icon TryGetIcon(in int iconIndex, in string dll, in System.Drawing.Size size) => new IconExtractor(IO.Path.GetRealPathFromEnvironmentVariables(WinCopies.IO.Path.System32Path + dll)).GetIcon(iconIndex).Split()?.TryGetIcon(size, 32, true, true);

        public IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => EnumerablePath.GetFileSystemEntryEnumerable(Path, searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
#if DEBUG
                , simulationParameters
#endif
                );

        public IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
#if NETCORE
            , EnumerationOptions enumerationOptions
#endif
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) => EnumerablePath.GetDirectoryEnumerable(Path, searchPattern, searchOption
#if NETCORE
                , enumerationOptions
#endif
#if DEBUG
                , simulationParameters
#endif
                );

        public IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption, EnumerationOptions enumerationOptions, FileSystemEntryEnumeratorProcessSimulation simulationParameters) => EnumerablePath.GetFileEnumerable(Path, searchPattern, searchOption
#if NETCORE
            , enumerationOptions
#endif
#if DEBUG
            , simulationParameters
#endif
            );

        IEnumerator<ITreeEnumerable<IBrowsableObjectInfo>> ITreeEnumerableProviderEnumerable<IBrowsableObjectInfo>.GetRecursiveEnumerator() => GetItems().GetEnumerator();

        public TreeEnumerator<IBrowsableObjectInfo> GetEnumerator() => new TreeEnumerator<IBrowsableObjectInfo>(this);

        IEnumerator<IBrowsableObjectInfo> IEnumerable<IBrowsableObjectInfo>.GetEnumerator() => GetItems().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetItems().GetEnumerator();

        /// <summary>
        /// When overridden in a derived class, returns the items of this <see cref="BrowsableObjectInfo"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{IBrowsableObjectInfo}"/> that enumerates through the items of this <see cref="BrowsableObjectInfo"/>.</returns>
        public abstract IEnumerable<IBrowsableObjectInfo> GetItems();

        #region IDisposable
        /// <summary>
        /// Gets a value that indicates whether the current object is disposed.
        /// </summary>
        public bool IsDisposed { get; internal set; }

        public void Dispose()
        {
            if (IsDisposed)

                return;

            Dispose(true);

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }

        /// <summary>
        /// Not used in this class.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(in bool disposing)
        {
            //if (ItemsLoader != null)

            //{

            //    if (ItemsLoader.IsBusy)

            //        ItemsLoader.Cancel();

            //    // ItemsLoader.Path = null;

            //}

            //if (disposing)

            //    _parent = null;
        }
        #endregion
        #endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        ~BrowsableObjectInfo() => Dispose(false);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
