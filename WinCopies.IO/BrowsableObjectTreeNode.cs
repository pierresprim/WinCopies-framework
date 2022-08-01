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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCopies.Collections;

namespace WinCopies.IO
{

    public interface IBrowsableObjectTreeNode<TValue, TItems> where TValue : IBrowsableObjectInfo where TItems : IBrowsableObjectInfo
    {

        /// <summary>
        /// Gets a value that indicates whether the items of this <see cref="IBrowsableObjectInfo"/> are currently loaded.
        /// </summary>
        bool AreItemsLoaded { get; }

        IBrowsableObjectInfoLoader<TValue, TItems> ItemsLoader { get; }

        ///// <summary>
        ///// Loads the items of this <see cref="IBrowsableObjectInfo"/>.
        ///// </summary>
        //void LoadItems();

        ///// <summary>
        ///// Loads the items of this <see cref="IBrowsableObjectInfo"/> using custom worker behavior options.
        ///// </summary>
        ///// <param name="workerReportsProgress">Whether the worker reports progress</param>
        ///// <param name="workerSupportsCancellation">Whether the worker supports cancellation.</param>
        //void LoadItems(bool workerReportsProgress, bool workerSupportsCancellation);

        /////// <summary>
        /////// Loads the items of this <see cref="IBrowsableObjectInfo"/> asynchronously using a given items loader.
        /////// </summary>
        /////// <param name="itemsLoader">A custom items loader.</param>
        ////void LoadItems(IBrowsableObjectInfoLoader itemsLoader);

        ///// <summary>
        ///// Loads the items of this <see cref="IBrowsableObjectInfo"/> asynchronously.
        ///// </summary>
        //void LoadItemsAsync();

        ///// <summary>
        ///// Loads the items of this <see cref="IBrowsableObjectInfo"/> asynchronously using custom worker behavior options.
        ///// </summary>
        ///// <param name="workerReportsProgress">Whether the worker reports progress</param>
        ///// <param name="workerSupportsCancellation">Whether the worker supports cancellation.</param>
        //void LoadItemsAsync(bool workerReportsProgress, bool workerSupportsCancellation);

    }

    // ///<remarks>The <see cref="Parent"/> property is set directly on first call.</remarks>
    public class BrowsableObjectTreeNode<TValue, TItems, TFactory> : ReadOnlyTreeNode<TValue, TItems>, IBrowsableObjectTreeNode<TValue, TItems> where TValue : BrowsableObjectInfo where TItems : BrowsableObjectInfo where TFactory : BrowsableObjectInfoFactory
    {

        internal void Insert(int index, ReadOnlyTreeNode<TItems> item) => InsertItem(index, item);

        internal void RemoveAt(int index) => RemoveItem(index);

        public IBrowsableObjectInfoLoader<TValue, TItems> ItemsLoader { get; internal set; }

        private ITreeNode _parent;

        //private Func<ITreeNode> _getParentDelegate;

        //public override ITreeNode Parent
        //{

        //    get
        //    {

        //        if (_parent is null)

        //        {

        //            _parent = _getParentDelegate();

        //            _getParentDelegate = null;

        //        }

        //        return _parent;

        //    }

        //    protected set => _parent = value;

        //}

        private TFactory _factory;

        ///// <summary>
        ///// Gets or sets the factory for this <see cref="BrowsableObjectInfo"/>. This factory is used to create new <see cref="IBrowsableObjectInfo"/>s from the current <see cref="BrowsableObjectInfo"/> and its associated <see cref="BrowsableObjectInfo.ItemsLoader"/>.
        ///// </summary>
        ///// <exception cref="InvalidOperationException">The old <see cref="BrowsableObjectInfo.ItemsLoader"/> is running. OR The given factory has already been added to a <see cref="BrowsableObjectInfo"/>.</exception>
        ///// <exception cref="ArgumentNullException">value is null.</exception>
        public TFactory Factory
        {

            get => _factory;

            set

            {

                ThrowOnInvalidFactoryUpdateOperation(value, nameof(value));

                _factory = value;

            }

        }

        /// <summary>
        /// Checks if an <see cref="IBrowsableObjectInfoFactory"/> can be added to this <see cref="BrowsableObjectInfo"/> and throw an exception if the validation failed.
        /// </summary>
        /// <param name="newFactory">The new factory to use in this <see cref="BrowsableObjectInfo"/> and in its associated <see cref="BrowsableObjectInfo.ItemsLoader"/>.</param>
        /// <param name="paramName">The parameter name to include in error messages.</param>
        /// <exception cref="InvalidOperationException">The <see cref="BrowsableObjectInfo.ItemsLoader"/> is busy. OR The given factory has already been added to a <see cref="BrowsableObjectInfo"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="newFactory"/> is null.</exception>
        protected virtual void ThrowOnInvalidFactoryUpdateOperation(IBrowsableObjectInfoFactory newFactory, string paramName)

        {

            if (ItemsLoader?.IsBusy == true)

                throw new InvalidOperationException($"The {nameof(ItemsLoader)} is busy.");

            if (newFactory is null)

                throw new ArgumentNullException(paramName);

        }

        /// <summary>
        /// Gets a value that indicates if the items of this <see cref="BrowsableObjectInfo"/> are currently loaded.
        /// </summary>
        public bool AreItemsLoaded { get; internal set; }

        public BrowsableObjectTreeNode(TValue value, TFactory factory) : base(value) => Factory = factory;

        public BrowsableObjectTreeNode(TValue value, System.Collections.Generic.IList<ReadOnlyTreeNode<TItems>> items, TFactory factory) : base(value, items) => Factory = factory;

        ///// <summary>
        ///// Loads the items of this <see cref="BrowsableObjectInfo"/>.
        ///// </summary>
        //public virtual void LoadItems()

        //{

        //if (ItemsLoader == null)

        //    LoadItems(true, true);

        //else

        //        ItemsLoader.LoadItems();

        //}

        ///// <summary>
        ///// When overridden in a derived class, loads the items of this <see cref="BrowsableObjectInfo"/> using custom worker behavior options.
        ///// </summary>
        ///// <param name="workerReportsProgress">Whether the worker reports progress</param>
        ///// <param name="workerSupportsCancellation">Whether the worker supports cancellation.</param>
        //public abstract void LoadItems(bool workerReportsProgress, bool workerSupportsCancellation);

        ///// <summary>
        ///// Loads the items of this <see cref="BrowsableObjectInfo"/> asynchronously using a given items loader.
        ///// </summary>
        ///// <param name="itemsLoader">A custom items loader.</param>
        //public virtual void LoadItems(IBrowsableObjectInfoLoader itemsLoader)

        //{

        //    if (!IsBrowsable)

        //        throw new InvalidOperationException(string.Format(Generic.NotBrowsableObject));

        //    ItemsLoader = itemsLoader;

        //    ItemsLoader.LoadItems();

        //}

        ///// <summary>
        ///// Loads the items of this <see cref="BrowsableObjectInfo"/> asynchronously.
        ///// </summary>
        //public virtual void LoadItemsAsync()

        //{

        //    if (ItemsLoader == null)

        //        LoadItemsAsync(true, true);

        //    else

        //        ItemsLoader.LoadItemsAsync();

        //}

        ///// <summary>
        ///// When overridden in a derived class, loads the items of this <see cref="BrowsableObjectInfo"/> asynchronously using custom worker behavior options.
        ///// </summary>
        ///// <param name="workerReportsProgress">Whether the worker reports progress</param>
        ///// <param name="workerSupportsCancellation">Whether the worker supports cancellation.</param>
        //public abstract void LoadItemsAsync(bool workerReportsProgress, bool workerSupportsCancellation);

        ///// <summary>
        ///// Loads the items of this <see cref="BrowsableObjectInfo"/> asynchronously using a given items loader.
        ///// </summary>
        ///// <param name="itemsLoader">A custom items loader.</param>
        //public virtual void LoadItemsAsync(IBrowsableObjectInfoLoader itemsLoader)

        //{

        //    if (!IsBrowsable)

        //        throw new InvalidOperationException(string.Format(Generic.NotBrowsableObject));

        //    ItemsLoader = itemsLoader;

        //    ItemsLoader.LoadItemsAsync();

        //}

    }

}
