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
using System.Collections.Generic;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;

namespace WinCopies.IO
{
#if !WinCopies3
    public interface IBrowsableObjectEncapsulator
    {
        object EncapsulatedObject { get; }
    }

    public interface IBrowsableObjectEncapsulator<T> : IBrowsableObjectEncapsulator
    {
        T EncapsulatedObject { get; }
    }

    public abstract class BrowsableObjectEncapsulator<T> : IBrowsableObjectEncapsulator<T>
    {
        public T EncapsulatedObject { get; }

        object IBrowsableObjectEncapsulator.EncapsulatedObject => EncapsulatedObject;

        public BrowsableObjectEncapsulator(T obj) => EncapsulatedObject = obj;
    }
#endif

    public interface IRecursiveEnumerable<T> : WinCopies.Collections.Generic.IRecursiveEnumerable<T>
    {
        RecursiveEnumerator<T> GetEnumerator();
    }

    namespace ObjectModel
    {
        /// <summary>
        /// Provides interoperability for interacting with browsable items.
        /// </summary>
        public interface IBrowsableObjectInfo : IBrowsableObjectInfoBase, IRecursiveEnumerable<IBrowsableObjectInfo>,
WinCopies.
#if !WinCopies3
    Util.
#endif
    DotNetFix.IDisposable
        {
            /// <summary>
            /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> is browsable.
            /// </summary>
            bool IsBrowsable { get; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> is recursively browsable.
            /// </summary>
            bool IsRecursivelyBrowsable { get; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> is browsable by default.
            /// </summary>
            bool IsBrowsableByDefault { get; }

            /// <summary>
            /// Gets the underlying object of this abstraction interface.
            /// </summary>
            object EncapsulatedObject { get; }

            /// <summary>
            /// Gets the common properties of <see cref="EncapsulatedObject"/>. These properties may be specific to the underlying object type.
            /// </summary>
            object ObjectProperties { get; }

#if WinCopies3
            /// <summary>
            /// Gets the specific properties of <see cref="EncapsulatedObject"/>. These properties are specific to this object.
            /// </summary>
            IPropertySystemCollection ObjectPropertySystem { get; }
#endif

            /// <summary>
            /// Gets the <see cref="IBrowsableObjectInfo"/> parent of this <see cref="IBrowsableObjectInfo"/>. Returns <see langword="null"/> if this object is the root object of a hierarchy.
            /// </summary>
            IBrowsableObjectInfo Parent { get; }

            /// <summary>
            /// Gets the small <see cref="BitmapSource"/> of this <see cref="IBrowsableObjectInfo"/>.
            /// </summary>
            BitmapSource SmallBitmapSource { get; }

            /// <summary>
            /// Gets the medium <see cref="BitmapSource"/> of this <see cref="IBrowsableObjectInfo"/>.
            /// </summary>
            BitmapSource MediumBitmapSource { get; }

            /// <summary>
            /// Gets the large <see cref="BitmapSource"/> of this <see cref="IBrowsableObjectInfo"/>.
            /// </summary>
            BitmapSource LargeBitmapSource { get; }

            /// <summary>
            /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="IBrowsableObjectInfo"/>.
            /// </summary>
            BitmapSource ExtraLargeBitmapSource { get; }

            string ItemTypeName { get; }

            string Description { get; }

            bool IsSpecialItem { get; }

            ClientVersion ClientVersion { get; }

            ///// <summary>
            ///// Gets or sets the factory for this <see cref="BrowsableObjectInfo{TParent, TItems, TFactory}"/>. This factory is used to create new <see cref="IBrowsableObjectInfo"/>s from the current <see cref="BrowsableObjectInfo{TParent, TItems, TFactory}"/>.
            ///// </summary>
            ///// <exception cref="InvalidOperationException">The old <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> is running. OR The given items loader has already been added to a <see cref="BrowsableObjectInfo{TParent, TItems, TFactory}"/>.</exception>
            ///// <exception cref="ArgumentNullException">value is null.</exception>
            //IBrowsableObjectInfoFactory Factory { get; }

            ///// <summary>
            ///// Gets or sets the items loader for this <see cref="BrowsableObjectInfo"/>.
            ///// </summary>
            ///// <exception cref="InvalidOperationException">The old <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> is running. OR The given items loader has already been added to a <see cref="BrowsableObjectInfo"/>.</exception>
            //IBrowsableObjectInfoLoader ItemsLoader { get; }

            ///// <summary>
            ///// Gets the items of this <see cref="IBrowsableObjectInfo"/>.
            ///// </summary>
            //IReadOnlyCollection<IBrowsableObjectInfo> Items { get; }

            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems();

            // IBrowsableObjectInfo GetBrowsableObjectInfo(IBrowsableObjectInfo browsableObjectInfo);

            // IPathModifier<IBrowsableObjectInfo, IBrowsableObjectInfo> RegisterLoader(IBrowsableObjectInfoLoader browsableObjectInfoLoader);

            // void UnregisterLoader();

            ///// <summary>
            ///// Loads the items of this <see cref="IBrowsableObjectInfo"/> asynchronously using a given items loader.
            ///// </summary>
            ///// <param name="itemsLoader">A custom items loader.</param>
            //void LoadItemsAsync(IBrowsableObjectInfoLoader itemsLoader);

            // bool IsRenamingSupported { get; }

            ///// <summary>
            ///// Renames or move to a relative path, or both, the current <see cref="IBrowsableObjectInfo"/> with the specified name.
            ///// </summary>
            ///// <param name="newValue">The new name or relative path for this <see cref="IBrowsableObjectInfo"/>.</param>
            //void Rename(string newValue);

            // string ToString();

            ///// <summary>
            ///// Gets a new <see cref="IBrowsableObjectInfo"/> that represents the same item that the current <see cref="IBrowsableObjectInfo"/>.
            ///// </summary>
            ///// <returns>A new <see cref="IBrowsableObjectInfo"/> that represents the same item that the current <see cref="IBrowsableObjectInfo"/>.</returns>
            //IBrowsableObjectInfo Clone();

            ///// <summary>
            ///// Disposes the current <see cref="IBrowsableObjectInfo"/> and its parent and items recursively.
            ///// </summary>
            ///// <param name="disposeItemsLoader">Whether to dispose the items loader of the current path.</param>
            ///// <param name="disposeParent">Whether to dispose the parent of the current path.</param>
            ///// <param name="disposeItems">Whether to dispose the items of the current path.</param>
            ///// <param name="recursively">Whether to dispose recursively.</param>
            ///// <exception cref="InvalidOperationException">The <see cref="ItemsLoader"/> is busy and does not support cancellation.</exception>
            //void Dispose(bool disposeItemsLoader, bool disposeParent, bool disposeItems, bool recursively);
        }

        public interface IBrowsableObjectInfo<T> : IBrowsableObjectInfo
        {
            T ObjectProperties { get; }
        }

        public interface IEncapsulatorBrowsableObjectInfo<
#if WinCopies3
            T
#else
            TEncapsulatedObject
#endif
            > : IBrowsableObjectInfo
        {
#if WinCopies3
            T
#else
TEncapsulatedObject
#endif
                EncapsulatedObject
            { get; }
        }

        public interface IBrowsableObjectInfo<TObjectProperties, TEncapsulatedObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IBrowsableObjectInfo<TObjectProperties>, IEncapsulatorBrowsableObjectInfo<TEncapsulatedObject> where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
        {
            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<TPredicateTypeParameter> predicate);

            TSelectorDictionary GetSelectorDictionary();
        }

        //public interface IBrowsableObjectInfo<TFactory> : IBrowsableObjectInfo where TFactory : IBrowsableObjectInfoFactory
        //{

        //}
    }
}
