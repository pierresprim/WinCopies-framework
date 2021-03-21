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
using System.Windows.Media.Imaging;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.PropertySystem;

namespace WinCopies.IO
{
#if !WinCopies3
    public interface IBrowsableObjectEncapsulator
    {
        object InnerObject { get; }
    }

    public interface IBrowsableObjectEncapsulator<T> : IBrowsableObjectEncapsulator
    {
        T InnerObject { get; }
    }

    public abstract class BrowsableObjectEncapsulator<T> : IBrowsableObjectEncapsulator<T>
    {
        public T InnerObject { get; }

        object IBrowsableObjectEncapsulator.InnerObject => InnerObject;

        public BrowsableObjectEncapsulator(T obj) => InnerObject = obj;
    }
#endif

    public interface IRecursiveEnumerable<T> : WinCopies.Collections.Generic.IRecursiveEnumerable<T>
    {
        RecursiveEnumerator<T> GetEnumerator();
    }

    /// <summary>
    /// Indicates the browsing ability for an <see cref="IBrowsableObjectInfo"/>.
    /// </summary>
    public enum Browsability : byte
    {
        /// <summary>
        /// The item is not browsable.
        /// </summary>
        NotBrowsable = 0,

        /// <summary>
        /// The item is browsable by default.
        /// </summary>
        BrowsableByDefault = 1,

        /// <summary>
        /// The item is browsable but browsing should not be the default action to perform on this item.
        /// </summary>
        Browsable = 2,

        /// <summary>
        /// The item redirects to an other item, with this item browsable.
        /// </summary>
        RedirectsToBrowsableItem = 3
    }

    public interface IBrowsabilityOptions
    {
        Browsability Browsability { get; }

        IBrowsableObjectInfo RedirectToBrowsableItem();
    }

    public static class BrowsabilityOptions
    {
        private class _Browsability : IBrowsabilityOptions
        {
            public Browsability Browsability { get; }

            public IBrowsableObjectInfo RedirectToBrowsableItem() => null;

            internal _Browsability(Browsability browsability) => Browsability = browsability;
        }

        public static IBrowsabilityOptions NotBrowsable { get; } = new _Browsability(Browsability.NotBrowsable);

        public static IBrowsabilityOptions BrowsableByDefault { get; } = new _Browsability(Browsability.BrowsableByDefault);

        public static IBrowsabilityOptions Browsable { get; } = new _Browsability(Browsability.Browsable);

        public static bool IsBrowsable(this Browsability browsability)
#if CS8
            => browsability switch
            {
#if CS9
                Browsability.BrowsableByDefault or Browsability.Browsable => true,
#else
                Browsability.BrowsableByDefault => true,
                Browsability.Browsable => true,
#endif
                _ => false,
            };
#else
        {
            switch (browsability)
            {
                case Browsability.BrowsableByDefault:
                case Browsability.Browsable:

                    return true;
            }

            return false;
        }
#endif
    }

    public interface IProcessParameters
    {
        Guid Guid { get; }

        System.Collections.Generic.IEnumerable<string> Parameters { get; }
    }

    public interface IProcessFactory
    {
        bool CanCopy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);

        bool TryCopy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

        void Copy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

        bool CanPaste(uint count, out string sourcePath);

        IProcessParameters GetCopyProcessParameters(uint count);

        IProcessParameters TryGetCopyProcessParameters(uint count);

        IProcess GetProcess(IProcessParameters processParameters, uint count);

        IProcess TryGetProcess(IProcessParameters processParameters, uint count);
    }

    public class DefaultProcessFactory : IProcessFactory
    {
        public static DefaultProcessFactory Instance { get; } = new DefaultProcessFactory();

        private DefaultProcessFactory() { /* Left empty. */ }

        bool IProcessFactory.CanCopy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => false;

        bool IProcessFactory.CanPaste(uint count, out string sourcePath)
        {
            sourcePath = null;

            return false;
        }

        void IProcessFactory.Copy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => throw new InvalidOperationException();

        IProcessParameters IProcessFactory.GetCopyProcessParameters(uint count) => throw new InvalidOperationException();

        IProcess IProcessFactory.GetProcess(IProcessParameters processParameters, uint count) => throw new InvalidOperationException();

        bool IProcessFactory.TryCopy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => false;

        IProcessParameters IProcessFactory.TryGetCopyProcessParameters(uint count) => null;

        IProcess IProcessFactory.TryGetProcess(IProcessParameters processParameters, uint count) => null;
    }

    public interface IProcessPathCollectionFactory
    {
        ProcessTypes<T>.IProcessCollection GetProcessCollection<T>() where T : IPathInfo;

        ProcessTypes<T>.IProcessCollection GetReadOnlyProcessCollection<T>(ProcessTypes<T>.IProcessCollection collection) where T : IPathInfo;

        ProcessObjectModelTypes<TItems, TFactory, TError, TProcessDelegates, TProcessEventDelegates, TProcessProgressDelegateParameter>.Process.IEnumerableInfoLinkedList GetEnumerableInfoLinkedList<TItems, TFactory, TError, TProcessDelegates, TProcessEventDelegates, TProcessProgressDelegateParameter>() where TItems : IPathInfo where TFactory : ProcessTypes<TItems>.ProcessErrorTypes<TError>.IProcessErrorFactories where TProcessDelegates : ProcessDelegateTypes<TItems, TProcessProgressDelegateParameter>.IProcessDelegates<TProcessEventDelegates> where TProcessEventDelegates : ProcessDelegateTypes<TItems, TProcessProgressDelegateParameter>.IProcessEventDelegates where TProcessProgressDelegateParameter : IProcessProgressDelegateParameter;

        ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessCollection GetReadOnlyEnumerableInfoLinkedList<TItems, TError>(IEnumerableInfoLinkedList<IProcessErrorItem<TItems, TError>> collection) where TItems : IPathInfo;
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
            IBrowsabilityOptions Browsability { get; }

            IProcessFactory ProcessFactory { get; }

            IProcessPathCollectionFactory ProcessPathCollectionFactory { get; }

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

            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> RootItems { get; }

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

        public interface IBrowsableObjectInfo<T> : IBrowsableObjectInfo
        {
            T ObjectProperties { get; }
        }

        public interface IEncapsulatorBrowsableObjectInfo<
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

        public interface IBrowsableObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IBrowsableObjectInfo<TObjectProperties>, IEncapsulatorBrowsableObjectInfo<TInnerObject> where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
        {
            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<TPredicateTypeParameter> predicate);

            TSelectorDictionary GetSelectorDictionary();
        }

        //public interface IBrowsableObjectInfo<TFactory> : IBrowsableObjectInfo where TFactory : IBrowsableObjectInfoFactory
        //{

        //}
    }
}
