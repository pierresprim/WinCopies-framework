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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.Linq;
using WinCopies.Util.Data;

using static WinCopies.
    #if WinCopies2
    Util.Util
#else
    ThrowHelper
    #endif
    ;

namespace WinCopies.GUI.IO.ObjectModel
{
    //public class TreeViewItemBrowsableObjectInfoViewModelFactory : IBrowsableObjectInfoViewModelFactory
    //{
    //    public IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) => new BrowsableObjectInfoViewModel(browsableObjectInfo, Predicate) { Factory = this };
    //}

    public class BrowsableObjectInfoViewModel : ViewModel<IBrowsableObjectInfo>, IBrowsableObjectInfoViewModel
    {
#region Private fields
        private Predicate<IBrowsableObjectInfo> _filter;
        private IBrowsableObjectInfoFactory _factory;
        private ObservableCollection<IBrowsableObjectInfoViewModel> _items;
        private bool _itemsLoaded = false;
        private IBrowsableObjectInfoViewModel _parent;
        private bool _parentLoaded = false;
        private bool _isSelected = false;
#endregion

#region Properties
        public static Predicate<IBrowsableObjectInfo> Predicate { get; } = browsableObjectInfo => browsableObjectInfo.IsBrowsable;

        public static Comparison<IBrowsableObjectInfo> DefaultComparison { get; } = (left, right) => left.CompareTo(right);

        public Predicate<IBrowsableObjectInfo> Filter { get => _filter; set { _filter = value; OnPropertyChanged(nameof(Filter)); } }

        public IBrowsableObjectInfoFactory Factory { get => _factory; set { _factory = value; OnPropertyChanged(nameof(_factory)); } }

        public bool IsSpecialItem => ModelGeneric.IsSpecialItem;

#region Bitmap sources
        public BitmapSource SmallBitmapSource => ModelGeneric.SmallBitmapSource;

        public BitmapSource MediumBitmapSource => ModelGeneric.MediumBitmapSource;

        public BitmapSource LargeBitmapSource => ModelGeneric.LargeBitmapSource;

        public BitmapSource ExtraLargeBitmapSource => ModelGeneric.ExtraLargeBitmapSource;
#endregion

        public object EncapsulatedObject => ModelGeneric.EncapsulatedObject;

        public object ObjectProperties => ModelGeneric.ObjectProperties;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> is browsable.
        /// </summary>
        public bool IsBrowsable => ModelGeneric.IsBrowsable;

        public string ItemTypeName => ModelGeneric.ItemTypeName;

        public string Description => ModelGeneric.Description;

        /// <summary>
        /// Gets the size for this <see cref="IBrowsableObjectInfo"/>.
        /// </summary>
        public Size? Size => ModelGeneric.Size;

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

                if (ModelGeneric.IsBrowsable)

                    try
                    {
                        System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items = _filter == null ? ModelGeneric.GetItems() : ModelGeneric.GetItems().WherePredicate(_filter);

                        var __items = new List<IBrowsableObjectInfoViewModel>(items.Select(

                            _browsableObjectInfo => _factory == null ? new BrowsableObjectInfoViewModel(_browsableObjectInfo, _filter) : _factory.GetBrowsableObjectInfoViewModel(_browsableObjectInfo)));

                        if (SortComparison != null)

                            __items.Sort(SortComparison);

                        _items = new ObservableCollection<IBrowsableObjectInfoViewModel>(__items);
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

                _itemsLoaded = true;

                return _items;
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

                    _parent = new BrowsableObjectInfoViewModel(ModelGeneric.Parent);

                _parentLoaded = true;

                return _parent;
            }
        }

        /// <summary>
        /// Gets the path of this <see cref="IFileSystemObject"/>.
        /// </summary>
        public string Path => ModelGeneric.Path;

        /// <summary>
        /// Gets the localized name of this <see cref="IFileSystemObject"/>.
        /// </summary>
        public string LocalizedName => ModelGeneric.LocalizedName;

        /// <summary>
        /// Gets the name of this <see cref="IFileSystemObject"/>.
        /// </summary>
        public string Name => ModelGeneric.Name;

        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }

        public FileSystemType ItemFileSystemType => ModelGeneric.ItemFileSystemType;

        public ClientVersion? ClientVersion => ModelGeneric.ClientVersion;

        /// <summary>
        /// The model for this view model instance.
        /// </summary>
        protected internal new IBrowsableObjectInfo ModelGeneric => base.ModelGeneric;
#endregion

#region Constructors
        public BrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) : base(browsableObjectInfo ?? throw GetArgumentNullException(nameof(browsableObjectInfo))) =>

            Debug.Assert(!(browsableObjectInfo is IBrowsableObjectInfoViewModel));

        public BrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo, Predicate<IBrowsableObjectInfo> filter) : this(browsableObjectInfo)
        {
            Debug.Assert(!(browsableObjectInfo is IBrowsableObjectInfoViewModel));

            _filter = filter;
        }
#endregion

#region Methods
        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems() => Items;

        IEnumerator<IBrowsableObjectInfo> System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>.GetEnumerator() => ((System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>)ModelGeneric).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ModelGeneric).GetEnumerator();

        public RecursiveEnumerator<IBrowsableObjectInfo> GetEnumerator() => ((WinCopies.IO.IRecursiveEnumerable<IBrowsableObjectInfo>)ModelGeneric).GetEnumerator();

        public IEnumerator<Collections.Generic.IRecursiveEnumerable<IBrowsableObjectInfo>> GetRecursiveEnumerator() => ((WinCopies.IO.IRecursiveEnumerable<IBrowsableObjectInfo>)ModelGeneric).GetRecursiveEnumerator();

        public int CompareTo(
#if !NETFRAMEWORK
            [AllowNull]
#endif
        IFileSystemObject other) => ModelGeneric.CompareTo(other);

        public bool Equals(
#if !NETFRAMEWORK
            [AllowNull]
#endif
        IFileSystemObject other) => ModelGeneric.Equals(other);

        public Collections.IEqualityComparer<IFileSystemObject> GetDefaultEqualityComparer() => ModelGeneric.GetDefaultEqualityComparer();

        public IComparer<IFileSystemObject> GetDefaultComparer() => ModelGeneric.GetDefaultComparer();

        public override bool Equals(object obj) => ReferenceEquals(this, obj) ? true : obj is null ? false : ModelGeneric.Equals(obj);

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
#endregion
    }
}
