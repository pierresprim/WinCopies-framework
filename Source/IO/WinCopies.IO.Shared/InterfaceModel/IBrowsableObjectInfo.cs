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

using WinCopies.Collections.Generic;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;

namespace WinCopies.IO
{

    namespace ObjectModel
    {
        /// <summary>
        /// Provides interoperability for interacting with browsable items.
        /// </summary>
        public interface IBrowsableObjectInfo : IBrowsableObjectInfoBase, IRecursiveEnumerable<IBrowsableObjectInfo>, DotNetFix.IDisposable
        {
            IBrowsabilityOptions Browsability { get; }

            System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPaths { get; }

            IProcessFactory ProcessFactory { get; }

            System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcesses { get; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> is recursively browsable.
            /// </summary>
            bool IsRecursivelyBrowsable { get; }

            /// <summary>
            /// Gets the underlying object of this abstraction interface.
            /// </summary>
            object InnerObject { get; }

            /// <summary>
            /// Gets the common properties of <see cref="InnerObject"/>. These properties may be specific to the underlying object type.
            /// </summary>
            object ObjectProperties { get; }

#if WinCopies3
            /// <summary>
            /// Gets the specific properties of <see cref="InnerObject"/>. These properties are specific to this object.
            /// </summary>
            IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystem { get; }
#endif

            /// <summary>
            /// Gets the <see cref="IBrowsableObjectInfo"/> parent of this <see cref="IBrowsableObjectInfo"/>. Returns <see langword="null"/> if this object is the root object of a hierarchy.
            /// </summary>
            IBrowsableObjectInfo Parent { get; }

            IBitmapSourceProvider BitmapSourceProvider { get; }

            IBitmapSources BitmapSources { get; }

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

            System.IDisposable RegisterCallback(Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason> callback);

            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems();

            ArrayBuilder<IBrowsableObjectInfo> GetRootItems();

            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItems();

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

        public interface IBrowsableObjectInfo<out T> : IBrowsableObjectInfo
        {
            T ObjectProperties { get; }
        }

        public interface IEncapsulatorBrowsableObjectInfo<out
#if WinCopies3
            T
#else
            TInnerObject
#endif
            > : IBrowsableObjectInfo
        {
#if WinCopies3
            T
#else
TInnerObject
#endif
                InnerObject
            { get; }
        }

        public interface IBrowsableObjectInfo<out TObjectProperties, out TInnerObject, out TPredicateTypeParameter, out TSelectorDictionary, out TDictionaryItems> : IBrowsableObjectInfo<TObjectProperties>, IEncapsulatorBrowsableObjectInfo<TInnerObject> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<TPredicateTypeParameter> predicate);

            TSelectorDictionary GetSelectorDictionary();
        }
    }
}
