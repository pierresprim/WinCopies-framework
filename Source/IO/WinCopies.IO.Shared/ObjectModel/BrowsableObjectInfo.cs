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

using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.GUI.Drawing;
using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;

using static WinCopies.Temp;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// The base class for all IO browsable objects of the WinCopies framework.
    /// </summary>
    public abstract class BrowsableObjectInfo : BrowsableObjectInfoBase, IBrowsableObjectInfo
    {
        #region Consts
        public const ushort SmallIconSize = 16;
        public const ushort MediumIconSize = 48;
        public const ushort LargeIconSize = 128;
        public const ushort ExtraLargeIconSize = 256;

        public const int FileIcon = 0;
        public const int ComputerIcon = 15;
        public const int FolderIcon = 3;
        #endregion

        #region Properties
        public static Action RegisterDefaultSelectors { get; private set; } = () =>
        {
            DotNetAssemblyInfo.RegisterSelectors();

            RegisterDefaultSelectors = () => { /* Left empty. */ };
        };

        IBrowsableObjectInfo Collections.Generic.IRecursiveEnumerable<IBrowsableObjectInfo>.Value => this;

        public abstract object InnerObject { get; }

#if WinCopies3
        public abstract IPropertySystemCollection ObjectPropertySystem { get; }
#endif

        public abstract object ObjectProperties { get; }

        /// <summary>
        /// When overridden in a derived class, gets a value that indicates whether this <see cref="BrowsableObjectInfo"/> is browsable.
        /// </summary>
        public abstract bool IsBrowsable { get; }

        public abstract bool IsRecursivelyBrowsable { get; }

        public abstract bool IsBrowsableByDefault { get; }

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

        public abstract bool IsSpecialItem { get; }

        public ClientVersion ClientVersion { get; }

        public abstract System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> RootItems { get; }

        public abstract Predicate<IBrowsableObjectInfo> RootItemsBrowsableObjectInfoPredicate { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfo"/> class.
        /// </summary>
        /// <param name="path">The path of the new item.</param>
        /// <param name="clientVersion">The <see cref="Microsoft.WindowsAPICodePack.PortableDevices.ClientVersion"/> that will be used to initialize new <see cref="PortableDeviceInfo"/>s and <see cref="PortableDeviceObjectInfo"/>s.</param>
        protected BrowsableObjectInfo(in string path, in ClientVersion clientVersion) : base(path) => ClientVersion = clientVersion;
        #endregion

        #region Methods
        public static ClientVersion GetDefaultClientVersion()
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

            Version assemblyVersion = assemblyName.Version;

            // todo: use the new constructor

            return new ClientVersion(assemblyName.Name, (uint)assemblyVersion.Major, (uint)assemblyVersion.Minor, (uint)assemblyVersion.Revision);
        }

        public static Icon TryGetIcon(in int iconIndex, in string dll, in System.Drawing.Size size) => new IconExtractor(IO.Path.GetRealPathFromEnvironmentVariables(IO.Path.System32Path + dll)).GetIcon(iconIndex).Split()?.TryGetIcon(size, 32, true, true);

        public static BitmapSource TryGetBitmapSource(in int iconIndex, in string dllName, in int size)
        {
            using
#if !CS8
            (
#endif
                Icon icon = TryGetIcon(iconIndex, dllName, new System.Drawing.Size(size, size))
#if CS8
            ;
#else
            )
#endif

            return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        System.Collections.Generic.IEnumerator<Collections.Generic.IRecursiveEnumerable<IBrowsableObjectInfo>> IRecursiveEnumerableProviderEnumerable<IBrowsableObjectInfo>.GetRecursiveEnumerator() => GetItems().GetEnumerator();

        RecursiveEnumerator<IBrowsableObjectInfo> IRecursiveEnumerable<IBrowsableObjectInfo>.GetEnumerator() => IsRecursivelyBrowsable ? new RecursiveEnumerator<IBrowsableObjectInfo>(this) : throw new NotSupportedException("The current BrowsableObjectInfo does not support recursive browsing.");

        System.Collections.Generic.IEnumerator<IBrowsableObjectInfo> System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>.GetEnumerator() => GetItems().GetEnumerator();

        IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetItems().GetEnumerator();

        /// <summary>
        /// When overridden in a derived class, returns the items of this <see cref="BrowsableObjectInfo"/>.
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{IBrowsableObjectInfo}"/> that enumerates through the items of this <see cref="BrowsableObjectInfo"/>.</returns>
        public abstract System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems();

        #region IDisposable
        /// <summary>
        /// Gets a value that indicates whether the current object is disposed.
        /// </summary>
        public bool IsDisposed { get; internal set; }

        public void Dispose()
        {
            if (IsDisposed)

                return;

            DisposeManaged();

            Dispose(true);

            GC.SuppressFinalize(this);

#if !WinCopies3
            IsDisposed = true;
#endif
        }

        /// <summary>
        /// In WinCopies 3, sets <see cref="IsDisposed"/> to <see langword="true"/>. This method does nothing in WinCopies 2. This method is called from the <see cref="Dispose()"/> method.
        /// </summary>
        protected virtual void DisposeManaged()
#if WinCopies3
            => IsDisposed = true;
#else
        {
            // Left empty.
        }
#endif

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

    public abstract class BrowsableObjectInfo<T> : BrowsableObjectInfo, IBrowsableObjectInfo<T>
    {
        #region Properties
        public abstract T ObjectPropertiesGeneric { get; }

        public sealed override object ObjectProperties => ObjectPropertiesGeneric;

        T IBrowsableObjectInfo<T>.ObjectProperties => ObjectPropertiesGeneric;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfo"/> class.
        /// </summary>
        /// <param name="path">The path of the new item.</param>
        /// <param name="clientVersion">The <see cref="ClientVersion"/> that will be used to initialize new <see cref="PortableDeviceInfo"/>s and <see cref="PortableDeviceObjectInfo"/>s.</param>
        protected BrowsableObjectInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }
        #endregion
    }

    public abstract class BrowsableObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<TObjectProperties>, IBrowsableObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Properties
        public abstract TInnerObject InnerObjectGeneric { get; }

        TInnerObject IEncapsulatorBrowsableObjectInfo<TInnerObject>.InnerObject => InnerObjectGeneric;

        public sealed override object InnerObject => InnerObjectGeneric;

        public abstract Predicate<TPredicateTypeParameter> RootItemsPredicate { get; }
        #endregion

        /// <summary>
        /// When called from a derived class, initializes a new instance of the <see cref="BrowsableObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> class with a custom <see cref="ClientVersion"/>.
        /// </summary>
        /// <param name="path">The path of the new <see cref="BrowsableObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>.</param>
        /// <param name="clientVersion">A custom <see cref="ClientVersion"/>. This parameter can be null for non-file system and portable devices-related types.</param>
        protected BrowsableObjectInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }

        public abstract TSelectorDictionary GetSelectorDictionary();

        protected System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(System.Collections.Generic.IEnumerable<TDictionaryItems> items) => GetSelectorDictionary().Select(items);

        protected abstract System.Collections.Generic.IEnumerable<TDictionaryItems> GetItemProviders();

        public sealed override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(GetItemProviders());

        protected abstract System.Collections.Generic.IEnumerable<TDictionaryItems> GetItemProviders(Predicate<TPredicateTypeParameter> predicate);

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<TPredicateTypeParameter> predicate)
        {
            if (predicate == null)

                return GetItems(GetItemProviders(item => true));

            else

                return GetItems(GetItemProviders(predicate));
        }

        protected System.Collections.Generic.IEnumerable<TDictionaryItems> GetEmptyEnumerable() => GetEmptyEnumerable<TDictionaryItems>();
    }
}
