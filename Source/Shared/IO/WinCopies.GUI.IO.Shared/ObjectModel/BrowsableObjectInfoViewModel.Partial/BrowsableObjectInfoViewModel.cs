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

#region Usings
#region System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Data;

#if CS8
using System.Diagnostics.CodeAnalysis;
#endif
#endregion

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.IO;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
using WinCopies.Util;
using WinCopies.Util.Commands.Primitives;
using WinCopies.Util.Data;
#endregion

using static WinCopies.
#if WinCopies3
    ThrowHelper
#else
    Util.Util
#endif
;

using IEnumerable = System.Collections.IEnumerable;
#endregion Usings

namespace WinCopies.GUI.IO.ObjectModel
{
    //public class TreeViewItemBrowsableObjectInfoViewModelFactory : IBrowsableObjectInfoViewModelFactory
    //{
    //    public IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) => new BrowsableObjectInfoViewModel(browsableObjectInfo, Predicate) { Factory = this };
    //}

    public static class BrowsableObjectInfo
    {
        public static IExplorerControlViewModel GetDefaultExplorerControlViewModel(in IBrowsableObjectInfoViewModel browsableObjectInfo, in IBrowsableObjectInfoFactory factory, in bool selected = false)
        {
            IExplorerControlViewModel viewModel = new ExplorerControlViewModel(browsableObjectInfo, factory);

            if (selected)

                viewModel.IsSelected = true;

            return viewModel;
        }

        public static IExplorerControlViewModel GetDefaultExplorerControlViewModel(in IBrowsableObjectInfo browsableObjectInfo, in IBrowsableObjectInfoFactory factory, in bool selected = false) => GetDefaultExplorerControlViewModel(browsableObjectInfo is IBrowsableObjectInfoViewModel viewModel ? viewModel : new BrowsableObjectInfoViewModel(browsableObjectInfo), factory, selected);

        public static IBrowsableObjectInfoFactory GetFactory(in IBrowsableObjectInfo path) => new BrowsableObjectInfoFactory(path.ClientVersion);

        public static IBrowsableObjectInfoFactory GetFactory(in IBrowsableObjectInfoViewModel path)
        {
            IBrowsableObjectInfoFactory factory = GetFactory(path.AsFromType<IBrowsableObjectInfo>());

            factory.SortComparison = path.SortComparison;

            return factory;
        }

        public static IExplorerControlViewModel GetDefaultExplorerControlViewModel(in IBrowsableObjectInfoViewModel browsableObjectInfo, in bool selected = false) => GetDefaultExplorerControlViewModel(browsableObjectInfo, GetFactory(browsableObjectInfo), selected);

        public static IExplorerControlViewModel GetDefaultExplorerControlViewModel(in IBrowsableObjectInfo browsableObjectInfo, in bool selected = false) => GetDefaultExplorerControlViewModel(browsableObjectInfo is IBrowsableObjectInfoViewModel viewModel ? viewModel : new BrowsableObjectInfoViewModel(browsableObjectInfo), selected);
    }

    [DebuggerDisplay("{Name}")]
    public partial class BrowsableObjectInfoViewModel : ViewModel<IBrowsableObjectInfo>, IBrowsableObjectInfoViewModel
    {
        internal struct BoolsStruct
        {
            private byte _bools;

            public bool AreItemsLoaded { get => GetBit(0); set => SetBit(0, value); }

            public bool IsParentLoaded { get => GetBit(1); set => SetBit(1, value); }

            private bool GetBit(in byte pos) => _bools.GetBit(pos);
            private void SetBit(in byte pos, in bool value) => UtilHelpers.SetBit(ref _bools, pos, value);
        }

        #region Private fields
        private BoolsStruct _bools;
        private IBrowsableObjectInfoViewModel _parent;
        private Predicate<IBrowsableObjectInfo> _filter;
        private Comparison<IBrowsableObjectInfo> _sortComparison;
        private bool _isSelected;
        private IBrowsableObjectInfoFactory _factory;
        #endregion

        #region Properties
        //public static Predicate<IBrowsableObjectInfo> Predicate { get; } = browsableObjectInfo => browsableObjectInfo.IsBrowsable;

        internal ref BoolsStruct Bools => ref _bools;

        public IReadOnlyDictionary<string, ConnectionParameter>
#if CS8
            ?
#endif
            ConnectionParameters => ModelGeneric.ConnectionParameters;

        public string
#if CS8
            ?
#endif
            Protocol => ModelGeneric.Protocol;

        public string URI => ModelGeneric.URI;

        public bool IsMonitoringSupported => ModelGeneric.IsMonitoringSupported;

        public bool IsMonitoring => ModelGeneric.IsMonitoring;

        public bool IsLocalRoot => ModelGeneric.IsLocalRoot;

        public ItemSourcesProviderViewModel
#if CS8
            ?
#endif
            ItemSources
        { get; }

        IItemSourcesProvider
#if CS8
            ?
#endif
            IBrowsableObjectInfo.ItemSources => ItemSources;

        public bool RootParentIsRootNode { get; }

        //public new IBrowsableObjectInfo Model => ModelGeneric;

        IBrowsableObjectInfo IBrowsableObjectInfoViewModel.Model => ModelGeneric;

        IItemSourcesProviderViewModel
#if CS8
            ?
#endif
            IBrowsableObjectInfoViewModel.ItemSources => ItemSources;

        public static Comparison<IBrowsableObjectInfo> DefaultComparison { get; } = (left, right) => left.CompareTo(right);

        public Predicate<IBrowsableObjectInfo> Filter
        {
            get => _filter;

            set
            {
                ThrowIfDisposed();

                if (UpdateValue(ref _filter, value))
                {
                    _filter = GetIfNotDisposed(value);

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

        public string
#if CS8
            ?
#endif
            Description => ModelGeneric.Description;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> is recursively browsable.
        /// </summary>
        public bool IsRecursivelyBrowsable => ModelGeneric.IsRecursivelyBrowsable;

        public IBrowsableObjectInfo Value => ModelGeneric.Value;

        public bool HasTransparency => ModelGeneric.IsSpecialItem;

        public Comparison<IBrowsableObjectInfo> SortComparison { get => _sortComparison; set => UpdateValue2(ref _sortComparison, value, nameof(SortComparison)); }

        IBrowsableObjectInfo IBrowsableObjectInfo.Parent => ModelGeneric.Parent;

        public IBrowsableObjectInfoViewModel Parent
        {
            get
            {
                ThrowIfDisposed();

                if (_bools.IsParentLoaded)

                    return _parent;

                if (ModelGeneric.Parent is object)

                    _parent = new BrowsableObjectInfoViewModel(ModelGeneric.Parent) { SortComparison = SortComparison, Factory = Factory };

                _bools.IsParentLoaded = true;

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

        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                if (UpdateValue2(ref _isSelected, value, nameof(IsSelected)))

                    SelectionChanged?.Invoke(this, System.EventArgs.Empty);
            }
        }

        public ClientVersion? ClientVersion => ModelGeneric.ClientVersion;

        /// <summary>
        /// The model for this view model instance.
        /// </summary>
        internal new IBrowsableObjectInfo Model => ModelGeneric;

        public IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystem => ModelGeneric.ObjectPropertySystem;

        ClientVersion IBrowsableObjectInfo.ClientVersion => ModelGeneric.ClientVersion;

        public System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPaths => ModelGeneric.BrowsabilityPaths;

        public IBrowsableObjectInfoContextCommandEnumerable
#if CS8
            ?
#endif
            ContextCommands => ModelGeneric.ContextCommands;

        public DisplayStyle DisplayStyle => ModelGeneric.DisplayStyle;
        #endregion

        public event EventHandler<IBrowsableObjectInfoViewModel> SelectionChanged;
        public event ItemsChangedEventHandler<IBrowsableObjectInfoViewModel> SelectedItemsChanged;

        #region Constructors
        internal BrowsableObjectInfoViewModel(in IBrowsableObjectInfo browsableObjectInfo, in bool rootParentIsRootNode) : base(browsableObjectInfo ?? throw GetArgumentNullException(nameof(browsableObjectInfo)))
        {
            RootParentIsRootNode = rootParentIsRootNode;

            ItemSources = ItemSourcesProviderViewModel.Construct(this);
        }

        public BrowsableObjectInfoViewModel(in IBrowsableObjectInfo browsableObjectInfo) : this(browsableObjectInfo, false) =>

            Debug.Assert(
#if !CS9
                !(
#endif
                browsableObjectInfo is
#if CS9
                not
#endif
                IBrowsableObjectInfoViewModel
#if !CS9
                )
#endif
                );
        #endregion

        #region Methods
        protected internal virtual void RaiseSelectedItemsChanged(ItemsChangedEventArgs<IBrowsableObjectInfoViewModel> e) => SelectedItemsChanged?.Invoke(this, e);

        public IContextMenu
#if CS8
            ?
#endif
            GetContextMenu(bool extendedVerbs) => ModelGeneric.GetContextMenu(extendedVerbs);

        public IContextMenu
#if CS8
            ?
#endif
            GetContextMenu(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> children, bool extendedVerbs) => ModelGeneric.GetContextMenu(children, extendedVerbs);

        public System.Collections.Generic.IEnumerable<ICommand>
#if CS8
            ?
#endif
            GetCommands(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items) => ModelGeneric.GetCommands(items);

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

        internal bool Predicate(object obj) => Filter((IBrowsableObjectInfo)obj);

        private void UpdatePredicate()
        {
            ItemSourceViewModel
#if CS8
                ?
#endif
                selectedItem = ItemSources?.SelectedItem;

            if (selectedItem?.ItemCollection == null)

                return;

            ref var collectionView = ref selectedItem.CollectionView;

#if !CS8
            if (
#endif
            collectionView
#if CS8
                ??=
#else
                == null)
#endif
                CollectionViewSource.GetDefaultView(selectedItem.ItemCollection);

            if (_filter == null)
            {
                collectionView.Filter = null;

                collectionView = null;
            }

            else

                collectionView.Filter = Predicate;
        }

        public void StartMonitoring() => ModelGeneric.StartMonitoring();

        public void StopMonitoring() => ModelGeneric.StopMonitoring();

        public IBrowsableObjectInfoCallback RegisterCallback(Action<BrowsableObjectInfoCallbackArgs> callback) => ModelGeneric.RegisterCallback(callback);

        ArrayBuilder<IBrowsableObjectInfo> IBrowsableObjectInfo.GetRootItems() => ModelGeneric.GetRootItems();

        //public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems() => Items;

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItems() => RootParentIsRootNode ? ItemSources.Default.Items : ModelGeneric.GetSubRootItems();

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

        public override bool Equals(object obj) => !(obj is null) && (ReferenceEquals(this, obj) || ModelGeneric.Equals(obj));

        public override int GetHashCode() => ModelGeneric.GetHashCode();

        public bool Equals(IBrowsableObjectInfoViewModel other) => ModelGeneric.Equals(other.Model);

        public int CompareTo(IBrowsableObjectInfoViewModel other) => ModelGeneric.CompareTo(other.Model);
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
#endregion
    }
}
