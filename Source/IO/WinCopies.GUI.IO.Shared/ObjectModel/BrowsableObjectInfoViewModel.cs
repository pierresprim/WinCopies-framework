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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using WinCopies.Collections.Generic;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
using WinCopies.Util.Data;

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

    public class BrowsableObjectInfoViewModel : ViewModel<IBrowsableObjectInfo>, IBrowsableObjectInfoViewModel
    {
        #region Private fields
        // private Predicate<IBrowsableObjectInfo> _filter;
        private IBrowsableObjectInfoFactory _factory;
        private ObservableCollection<IBrowsableObjectInfoViewModel> _items;
        private bool _itemsLoaded;
        private IBrowsableObjectInfoViewModel _parent;
        private bool _parentLoaded;
        private bool _isSelected;
        private int _selectedIndex = -1;
        private IBrowsableObjectInfo _selectedItem;
        #endregion

        #region Properties
        //public static Predicate<IBrowsableObjectInfo> Predicate { get; } = browsableObjectInfo => browsableObjectInfo.IsBrowsable;

        public IProcessFactory ProcessFactory => ModelGeneric.ProcessFactory;

        public bool RootParentIsRootNode { get; }

        public new IBrowsableObjectInfo Model => ModelGeneric;

        public static Comparison<IBrowsableObjectInfo> DefaultComparison { get; } = (left, right) => left.CompareTo(right);

        //public Predicate<IBrowsableObjectInfo> Filter { get => _filter; set { _filter = value; OnPropertyChanged(nameof(Filter)); } }

        public IBrowsableObjectInfoFactory Factory { get => _factory; set { _factory = value; OnPropertyChanged(nameof(_factory)); } }

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

        public Comparison<IBrowsableObjectInfo> SortComparison { get; set; }

        public ObservableCollection<IBrowsableObjectInfoViewModel> Items
        {
            get
            {
                if (_itemsLoaded)

                    return _items;

                if (ModelGeneric.IsBrowsable())

                    try
                    {
                        var __items = new ArrayBuilder<IBrowsableObjectInfoViewModel>((RootParentIsRootNode ? ModelGeneric.GetSubRootItems() : ModelGeneric.GetItems()).Select(

                            _browsableObjectInfo => _factory == null ? new BrowsableObjectInfoViewModel(_browsableObjectInfo/*, _filter*/, RootParentIsRootNode) { SortComparison = SortComparison, Factory = Factory } : _factory.GetBrowsableObjectInfoViewModel(_browsableObjectInfo, this)));

                        var itemsList = __items.ToList();

                        __items.Clear();

                        if (SortComparison == null)

                            itemsList.Sort();

                        else

                            itemsList.Sort(SortComparison);

                        return (_items = new ObservableCollection<IBrowsableObjectInfoViewModel>(itemsList));
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
                        _itemsLoaded = true;
                    }

                return null;
            }
        }

        IBrowsableObjectInfo IBrowsableObjectInfo.Parent => ModelGeneric.Parent;

        public IBrowsableObjectInfoViewModel Parent
        {
            get
            {
                if (_parentLoaded)

                    return _parent;

                if (ModelGeneric.Parent is object)

                    _parent = new BrowsableObjectInfoViewModel(ModelGeneric.Parent) { SortComparison = SortComparison, Factory = Factory };

                _parentLoaded = true;

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

        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }

        public ClientVersion? ClientVersion => ModelGeneric.ClientVersion;

        /// <summary>
        /// The model for this view model instance.
        /// </summary>
        protected internal new IBrowsableObjectInfo ModelGeneric => base.ModelGeneric;

        public IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystem => ModelGeneric.ObjectPropertySystem;

        ClientVersion IBrowsableObjectInfo.ClientVersion => ModelGeneric.ClientVersion;

        public int SelectedIndex { get => _selectedIndex; set { _selectedIndex = value; OnPropertyChanged(nameof(SelectedIndex)); } }

        public IBrowsableObjectInfo SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); } }

        public System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPaths => ModelGeneric.BrowsabilityPaths;

        public System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcesses => ModelGeneric.CustomProcesses;
        #endregion

        #region Constructors
        internal BrowsableObjectInfoViewModel(in IBrowsableObjectInfo browsableObjectInfo, in bool rootParentIsRootNode) : base(browsableObjectInfo ?? throw GetArgumentNullException(nameof(browsableObjectInfo))) => RootParentIsRootNode = rootParentIsRootNode;

        public BrowsableObjectInfoViewModel(in IBrowsableObjectInfo browsableObjectInfo) : this(browsableObjectInfo, false) =>

            Debug.Assert(!(browsableObjectInfo is IBrowsableObjectInfoViewModel));
        #endregion

        #region Methods
        public System.IDisposable RegisterCallback(Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason> callback) => ModelGeneric.RegisterCallback(callback);

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
