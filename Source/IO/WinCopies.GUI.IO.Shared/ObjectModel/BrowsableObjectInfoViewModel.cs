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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media.Imaging;

using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.Linq;
using WinCopies.Util.Data;

using static WinCopies.Util.Util;

namespace WinCopies.GUI.IO.ObjectModel
{
    //public class TreeViewItemBrowsableObjectInfoViewModelFactory : IBrowsableObjectInfoViewModelFactory
    //{
    //    public IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) => new BrowsableObjectInfoViewModel(browsableObjectInfo, Predicate) { Factory = this };
    //}

    public class BrowsableObjectInfoViewModel : ViewModel<IBrowsableObjectInfo>, IBrowsableObjectInfoViewModel
    {
        public static Predicate<IBrowsableObjectInfo> Predicate { get; } = browsableObjectInfo => browsableObjectInfo.IsBrowsable;

        public static Comparison<IBrowsableObjectInfo> DefaultComparison { get; } = (left, right) => left.CompareTo(right);

        private Predicate<IBrowsableObjectInfo> _filter;

        public Predicate<IBrowsableObjectInfo> Filter { get => _filter; set { _filter = value; OnPropertyChanged(nameof(Filter)); } }

        private IBrowsableObjectInfoFactory _factory;

        public IBrowsableObjectInfoFactory Factory { get => _factory; set { _factory = value; OnPropertyChanged(nameof(_factory)); } }

        public IBrowsableObjectInfo InnerBrowsableObjectInfo => ModelGeneric;

        public bool IsSpecialItem => InnerBrowsableObjectInfo.IsSpecialItem;

        public BitmapSource SmallBitmapSource => InnerBrowsableObjectInfo.SmallBitmapSource;

        public BitmapSource MediumBitmapSource => InnerBrowsableObjectInfo.MediumBitmapSource;

        public BitmapSource LargeBitmapSource => InnerBrowsableObjectInfo.LargeBitmapSource;

        public BitmapSource ExtraLargeBitmapSource => InnerBrowsableObjectInfo.ExtraLargeBitmapSource;

        public bool IsBrowsable => InnerBrowsableObjectInfo.IsBrowsable;

        public string ItemTypeName => InnerBrowsableObjectInfo.ItemTypeName;

        public string Description => InnerBrowsableObjectInfo.Description;

        public Size? Size => InnerBrowsableObjectInfo.Size;

        public bool HasTransparency => InnerBrowsableObjectInfo.IsSpecialItem;

        private ObservableCollection<IBrowsableObjectInfoViewModel> _items;

        private bool _itemsLoaded = false;

        public IEnumerable<IBrowsableObjectInfo> GetItems() => Items;

        public Comparison<IBrowsableObjectInfo> SortComparison { get; set; }

        public ObservableCollection<IBrowsableObjectInfoViewModel> Items
        {
            get
            {
                if (_itemsLoaded)

                    return _items;

                if (InnerBrowsableObjectInfo.IsBrowsable)

                    try
                    {
                        IEnumerable<IBrowsableObjectInfo> items = _filter == null ? InnerBrowsableObjectInfo.GetItems() : InnerBrowsableObjectInfo.GetItems().WherePredicate(_filter);

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

        private IBrowsableObjectInfoViewModel _parent;

        private bool _parentLoaded = false;

        IBrowsableObjectInfo IBrowsableObjectInfo.Parent => InnerBrowsableObjectInfo.Parent;

        public IBrowsableObjectInfoViewModel Parent
        {
            get
            {
                if (_parentLoaded)

                    return _parent;

                if (InnerBrowsableObjectInfo.Parent is object)

                    _parent = new BrowsableObjectInfoViewModel(InnerBrowsableObjectInfo.Parent);

                _parentLoaded = true;

                return _parent;
            }
        }

        public string Path => InnerBrowsableObjectInfo.Path;

        public string LocalizedName => InnerBrowsableObjectInfo.LocalizedName;

        public string Name => InnerBrowsableObjectInfo.Name;

        private bool _isSelected = false;

        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }

        public FileSystemType ItemFileSystemType => InnerBrowsableObjectInfo.ItemFileSystemType;

        public ClientVersion? ClientVersion => InnerBrowsableObjectInfo.ClientVersion;

        public BrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) : base(browsableObjectInfo ?? throw GetArgumentNullException(nameof(browsableObjectInfo))) =>

            Debug.Assert(!(browsableObjectInfo is IBrowsableObjectInfoViewModel));

        public BrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo, Predicate<IBrowsableObjectInfo> filter) : this(browsableObjectInfo)
        {
            Debug.Assert(!(browsableObjectInfo is IBrowsableObjectInfoViewModel));

            _filter = filter;
        }

        public int CompareTo(
#if !NETFRAMEWORK
            [AllowNull]
        #endif
        IFileSystemObject other) => InnerBrowsableObjectInfo.CompareTo(other);

        public bool Equals(
#if !NETFRAMEWORK
            [AllowNull]
        #endif
        IFileSystemObject other) => InnerBrowsableObjectInfo.Equals(other);

        public Collections.IEqualityComparer<IFileSystemObject> GetDefaultEqualityComparer() => InnerBrowsableObjectInfo.GetDefaultEqualityComparer();

        public IComparer<IFileSystemObject> GetDefaultComparer() => InnerBrowsableObjectInfo.GetDefaultComparer();

        #region IDisposable Support

        public bool IsDisposed => InnerBrowsableObjectInfo.IsDisposed;

        protected virtual void Dispose(in bool disposing)
        {
            if (disposing)

                InnerBrowsableObjectInfo.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public override bool Equals(object obj) => ReferenceEquals(this, obj) ? true : obj is null ? false : InnerBrowsableObjectInfo.Equals(obj);

        public override int GetHashCode() => InnerBrowsableObjectInfo.GetHashCode();

        public static bool operator ==(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is null ? right is null : left.Equals(right);

        public static bool operator !=(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => !(left == right);

        public static bool operator <(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is null ? right is object : left.CompareTo(right) < 0;

        public static bool operator <=(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is null || left.CompareTo(right) <= 0;

        public static bool operator >(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is object && left.CompareTo(right) > 0;

        public static bool operator >=(BrowsableObjectInfoViewModel left, BrowsableObjectInfoViewModel right) => left is null ? right is null : left.CompareTo(right) >= 0;
        #endregion
    }
}
