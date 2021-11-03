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

#region System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
#endregion

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
using WinCopies.Util.Commands.Primitives;
using WinCopies.Util.Data;
#endregion

using static WinCopies.
#if !WinCopies3
    Util.Util
#else
    ThrowHelper
#endif
    ;

using IEnumerable = System.Collections.IEnumerable;

#if CS8
using System.Diagnostics.CodeAnalysis;
#endif

namespace WinCopies.GUI.IO.ObjectModel
{
    //public class TreeViewItemBrowsableObjectInfoViewModelFactory : IBrowsableObjectInfoViewModelFactory
    //{
    //    public IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) => new BrowsableObjectInfoViewModel(browsableObjectInfo, Predicate) { Factory = this };
    //}

    public partial class BrowsableObjectInfoViewModel : ViewModel<IBrowsableObjectInfo>, IBrowsableObjectInfoViewModel
    {
        #region Private fields
        private Selection _selection = Selection.GetInstance();
        private _Browsability _browsability = new
#if !CS9
            _Browsability
#endif
            ();
        private IBrowsableObjectInfoFactory _factory;
        #endregion

        #region Properties
        //public static Predicate<IBrowsableObjectInfo> Predicate { get; } = browsableObjectInfo => browsableObjectInfo.IsBrowsable;

        public bool IsMonitoringSupported => ModelGeneric.IsMonitoringSupported;

        public bool IsMonitoring => ModelGeneric.IsMonitoring;

        public bool IsLocalRoot => ModelGeneric.IsLocalRoot;

        public IProcessFactory ProcessFactory => ModelGeneric.ProcessFactory;

        public bool RootParentIsRootNode { get; }

        public new IBrowsableObjectInfo Model => ModelGeneric;

        public static Comparison<IBrowsableObjectInfo> DefaultComparison { get; } = (left, right) => left.CompareTo(right);

        public Predicate<IBrowsableObjectInfo> Filter
        {
            get => _browsability._items._itemManagement._filter;

            set
            {
                ThrowIfDisposed();

                ref _Browsability.Items items = ref _browsability._items;
                ref Predicate<IBrowsableObjectInfo> filter = ref items._itemManagement._filter;

                if (UpdateValue(ref filter, value))
                {
                    filter = GetIfNotDisposed(value);

                    if (items._items != null)

                        UpdatePredicate();

                    OnPropertyChanged(nameof(Filter));
                }
            }
        }

        public IBrowsableObjectInfoFactory Factory { get => _factory; set => UpdateValue2(ref _factory, value, nameof(Factory)); }

        public bool IsSpecialItem => ModelGeneric.IsSpecialItem;

        public IBitmapSourceProvider BitmapSourceProvider => ModelGeneric.BitmapSourceProvider;

        public IBitmapSources BitmapSources => ModelGeneric.BitmapSources;

        public object InnerObject => ModelGeneric.InnerObject;

        public object ObjectProperties => ModelGeneric.ObjectProperties;

        public IBrowsabilityOptions Browsability => ModelGeneric.Browsability;

        public string ItemTypeName => ModelGeneric.ItemTypeName;

        public string Description => ModelGeneric.Description;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> is recursively browsable.
        /// </summary>
        public bool IsRecursivelyBrowsable => ModelGeneric.IsRecursivelyBrowsable;

        public IBrowsableObjectInfo Value => ModelGeneric.Value;

        public bool HasTransparency => ModelGeneric.IsSpecialItem;

        public Comparison<IBrowsableObjectInfo> SortComparison { get => _browsability._items._itemManagement._sortComparison; set => UpdateValue2(ref _browsability._items._itemManagement._sortComparison, value, nameof(SortComparison)); }

        public ObservableCollection<IBrowsableObjectInfoViewModel> Items
        {
            get
            {
                LoadItems();

                return _browsability._items._items;
            }
        }

        IBrowsableObjectInfo IBrowsableObjectInfo.Parent => ModelGeneric.Parent;

        public IBrowsableObjectInfoViewModel Parent
        {
            get
            {
                ThrowIfDisposed();

                ref _Browsability.Parent parent = ref _browsability._parent;
                ref bool parentLoaded = ref parent._parentLoaded;
                ref IBrowsableObjectInfoViewModel _parent = ref parent._parent;

                if (parentLoaded)

                    return _parent;

                if (ModelGeneric.Parent is object)

                    _parent = new BrowsableObjectInfoViewModel(ModelGeneric.Parent) { SortComparison = SortComparison, Factory = Factory };

                parentLoaded = true;

                return _parent;
            }
        }

        /// <summary>
        /// Gets the path of this <see cref="IBrowsableObjectInfoBase"/>.
        /// </summary>
        public string Path => ModelGeneric.Path;

        /// <summary>
        /// Gets the localized name of this <see cref="IBrowsableObjectInfoBase"/>.
        /// </summary>
        public string LocalizedName => ModelGeneric.LocalizedName;

        /// <summary>
        /// Gets the name of this <see cref="IBrowsableObjectInfoBase"/>.
        /// </summary>
        public string Name => ModelGeneric.Name;

        public bool IsSelected { get => _selection._isSelected; set => UpdateValue2(ref _selection._isSelected, value, nameof(IsSelected)); }

        public ClientVersion? ClientVersion => ModelGeneric.ClientVersion;

        /// <summary>
        /// The model for this view model instance.
        /// </summary>
        protected internal new IBrowsableObjectInfo ModelGeneric => base.ModelGeneric;

        public IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystem => ModelGeneric.ObjectPropertySystem;

        ClientVersion IBrowsableObjectInfo.ClientVersion => ModelGeneric.ClientVersion;

        public int SelectedIndex { get => _selection._selectedIndex; set => UpdateValue2(ref _selection._selectedIndex, value, nameof(SelectedIndex)); }

        public IBrowsableObjectInfo SelectedItem { get => _selection._selectedItem; set => UpdateValue2(ref _selection._selectedItem, value, nameof(SelectedItem)); }

        public System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPaths => ModelGeneric.BrowsabilityPaths;

        public System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcesses => ModelGeneric.CustomProcesses;
        #endregion

        #region Constructors
        internal BrowsableObjectInfoViewModel(in IBrowsableObjectInfo browsableObjectInfo, in bool rootParentIsRootNode) : base(browsableObjectInfo ?? throw GetArgumentNullException(nameof(browsableObjectInfo))) => RootParentIsRootNode = rootParentIsRootNode;

        public BrowsableObjectInfoViewModel(in IBrowsableObjectInfo browsableObjectInfo) : this(browsableObjectInfo, false) =>

            Debug.Assert(!(browsableObjectInfo is IBrowsableObjectInfoViewModel));
        #endregion

        #region Methods
        public System.Collections.Generic.IEnumerable<ICommand> GetCommands(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items) => ModelGeneric.GetCommands(items);

        protected T GetIfNotDisposed<T>(in T value) => GetOrThrowIfDisposed(this, value);

        protected void ThrowIfDisposed() => ThrowHelper.ThrowIfDisposed(this);

        protected bool UpdateValue<T>(ref T value, in T newValue) => UtilHelpers.UpdateValue(ref value, GetIfNotDisposed(newValue));

        protected bool UpdateValue2<T>(ref T value, in T newValue, in string propertyName)
        {
            bool result = UpdateValue(ref value, newValue);

            if (result)

                OnPropertyChanged(propertyName);

            return result;
        }

        private bool Predicate(object obj) => Filter((IBrowsableObjectInfo)obj);

        private void UpdatePredicate()
        {
            ref _Browsability.Items _items = ref _browsability._items;
            ref _Browsability.Items.ItemManagement itemManagement = ref _items._itemManagement;
            ref System.ComponentModel.ICollectionView collectionView = ref itemManagement._collectionView;

            if (collectionView == null)

                collectionView = CollectionViewSource.GetDefaultView(_items._items);

            if (itemManagement._filter == null)
            {
                collectionView.Filter = null;

                collectionView = null;
            }

            else

                collectionView.Filter = Predicate;
        }

        public void Sort(in List<IBrowsableObjectInfoViewModel> list)
        {
            if (SortComparison == null)

                list.Sort();

            else

                list.Sort(SortComparison);
        }

        private ref _Browsability.Items LoadItems2()
        {
            ThrowIfDisposed();

            ref _Browsability.Items items = ref _browsability._items;
            ref bool itemsLoaded = ref items._itemsLoaded;
            ref ObservableCollection<IBrowsableObjectInfoViewModel> _items = ref items._items;
            ref _Browsability.Items.ItemManagement itemManagement = ref items._itemManagement;
            ref System.ComponentModel.ICollectionView collectionView = ref itemManagement._collectionView;

            if (itemsLoaded)

                return ref items;

            if (ModelGeneric.IsBrowsable())
            {
                bool changed = false;

                try
                {
                    var __items = new ArrayBuilder<IBrowsableObjectInfoViewModel>((RootParentIsRootNode ? ModelGeneric.GetSubRootItems() : ModelGeneric.GetItems()).Select(

                        _browsableObjectInfo => _factory == null ? new BrowsableObjectInfoViewModel(_browsableObjectInfo/*, _filter*/, RootParentIsRootNode) { SortComparison = SortComparison, Factory = Factory } : _factory.GetBrowsableObjectInfoViewModel(_browsableObjectInfo, this)));

                    List<IBrowsableObjectInfoViewModel> itemsList = __items.ToList();

                    __items.Clear();

                    Sort(itemsList);

                    _items = new ObservableCollection<IBrowsableObjectInfoViewModel>(itemsList);

                    changed = true;
                }

                catch
#if DEBUG
                    (Exception ex)
#endif
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }

                finally
                {
                    itemsLoaded = true;
                }

                if (changed)
                {
                    ref Predicate<IBrowsableObjectInfo> filter = ref itemManagement._filter;

                    collectionView = CollectionViewSource.GetDefaultView(items._items);

                    if (filter == null)

                        collectionView.Filter = null;

                    else

                        collectionView.Filter = Predicate;

                    OnPropertyChanged(nameof(Items));
                }
            }

            return ref items;
        }

        public void LoadItems() => LoadItems2();

        public void UpdateItems()
        {
            ref _Browsability.Items _items = ref LoadItems2();
            ref ObservableCollection<IBrowsableObjectInfoViewModel> items = ref _items._items;
            ref var collectionView = ref _items._itemManagement._collectionView;

            var list = new List<IBrowsableObjectInfoViewModel>(items.Count);

            foreach (var item in items)

                list.Add(item);

            items.Clear();

            Sort(list);

            collectionView.Filter = null;

            collectionView = CollectionViewSource.GetDefaultView(items);

            if (Filter == null)

                collectionView.Filter = null;

            else

                collectionView.Filter = Predicate;

            items = new ObservableCollection<IBrowsableObjectInfoViewModel>(list);

            OnPropertyChanged(nameof(Items));
        }

        public void StartMonitoring() => ModelGeneric.StartMonitoring();

        public void StopMonitoring() => ModelGeneric.StopMonitoring();

        public IBrowsableObjectInfoCallback RegisterCallback(Action<BrowsableObjectInfoCallbackArgs> callback) => ModelGeneric.RegisterCallback(callback);

        ArrayBuilder<IBrowsableObjectInfo> IBrowsableObjectInfo.GetRootItems() => ModelGeneric.GetRootItems();

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems() => Items;

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItems() => RootParentIsRootNode ? Items : ModelGeneric.GetSubRootItems();

        IEnumerator<IBrowsableObjectInfo> System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>.GetEnumerator() => ((System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>)ModelGeneric).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ModelGeneric).GetEnumerator();

        public RecursiveEnumeratorAbstract<IBrowsableObjectInfo> GetEnumerator() => ModelGeneric.GetEnumerator();

        public IEnumerator<Collections.Generic.IRecursiveEnumerable<IBrowsableObjectInfo>> GetRecursiveEnumerator() => ModelGeneric.GetRecursiveEnumerator();

        public int CompareTo(
#if !NETFRAMEWORK
            [AllowNull]
#endif
        IBrowsableObjectInfoBase other) => ModelGeneric.CompareTo(other);

        public bool Equals(
#if !NETFRAMEWORK
            [AllowNull]
#endif
        IBrowsableObjectInfoBase other) => ModelGeneric.Equals(other);

        public Collections.Generic.IEqualityComparer<IBrowsableObjectInfoBase> GetDefaultEqualityComparer() => ModelGeneric.GetDefaultEqualityComparer();

        public Collections.Generic.IComparer<IBrowsableObjectInfoBase> GetDefaultComparer() => ModelGeneric.GetDefaultComparer();

        public override bool Equals(object obj) => obj is null ? false : ReferenceEquals(this, obj) || ModelGeneric.Equals(obj);

        public override int GetHashCode() => ModelGeneric.GetHashCode();
        #endregion

        #region Operators
        public static bool operator ==(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is null ? right is null : left.Equals(right);

        public static bool operator !=(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => !(left == right);

        public static bool operator <(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is null ? right is object : left.CompareTo(right) < 0;

        public static bool operator <=(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is null || left.CompareTo(right) <= 0;

        public static bool operator >(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is object && left.CompareTo(right) > 0;

        public static bool operator >=(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is null ? right is null : left.CompareTo(right) >= 0;
        #endregion

        #region IDisposable Support
        public bool IsDisposed => ModelGeneric.IsDisposed;

        protected virtual void Dispose(in bool disposing)
        {
            if (disposing)

                ModelGeneric.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public bool Equals(IBrowsableObjectInfoViewModel other) => ModelGeneric.Equals(other.Model);

        public int CompareTo(IBrowsableObjectInfoViewModel other) => ModelGeneric.CompareTo(other.Model);
        #endregion
    }
}
