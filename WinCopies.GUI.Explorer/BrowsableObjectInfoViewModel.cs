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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WinCopies.IO;

namespace WinCopies.GUI.Explorer
{
    public class BrowsableObjectInfoViewModel : WinCopies.Util.Data.ViewModel<Explorer.IBrowsableObjectInfo>, Explorer.IBrowsableObjectInfoViewModel
    {
        #region IBrowsableObjectInfoViewModel implementation

        /// <summary>
        /// Gets the small <see cref="BitmapSource"/> of this <see cref="IO. IBrowsableObjectInfo"/>.
        /// </summary>
        public BitmapSource SmallBitmapSource => Model.SmallBitmapSource;

        /// <summary>
        /// Gets the medium <see cref="BitmapSource"/> of this <see cref="IO. IBrowsableObjectInfo"/>.
        /// </summary>
        public BitmapSource MediumBitmapSource => Model.MediumBitmapSource;

        /// <summary>
        /// Gets the large <see cref="BitmapSource"/> of this <see cref="IO. IBrowsableObjectInfo"/>.
        /// </summary>
        public BitmapSource LargeBitmapSource => Model.LargeBitmapSource;

        /// <summary>
        /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="IO. IBrowsableObjectInfo"/>.
        /// </summary>
        public BitmapSource ExtraLargeBitmapSource => Model.ExtraLargeBitmapSource;

        /// <summary>
        /// Gets a value that indicates whether this <see cref="IO. IBrowsableObjectInfo"/> is browsable.
        /// </summary>
        public bool IsBrowsable => Model.IsBrowsable;

        /// <summary>
        /// Gets a value that indicates whether this <see cref="IO. IBrowsableObjectInfo"/> is disposing.
        /// </summary>
        public bool IsDisposing => Model.IsDisposing;

        /// <summary>
        /// Gets the items of this <see cref="IO. IBrowsableObjectInfo"/>.
        /// </summary>
        public Collections.ReadOnlyObservableCollection<IO.IBrowsableObjectInfo> Items => Model.Items;

        // todo: really needed? :

        /// <summary>
        /// Gets a value that indicates if the items of this <see cref="IO. IBrowsableObjectInfo"/> are currently loaded.
        /// </summary>
        public bool AreItemsLoaded => Model.AreItemsLoaded;

        /// <summary>
        /// Gets the <see cref="IO. IBrowsableObjectInfo"/> parent of this <see cref="IO. IBrowsableObjectInfo"/>. Returns <see langword="null"/> if this object is the root object of a hierarchy.
        /// </summary>
        public IO.IBrowsableObjectInfo Parent => Model.Parent;

        /// <summary>
        /// Gets or sets the items loader for this <see cref="Explorer.IBrowsableObjectInfo"/>.
        /// </summary>
        public BrowsableObjectInfoItemsLoader ItemsLoader { get => Model.ItemsLoader; set => OnPropertyChanged(nameof(ItemsLoader), value, typeof(IO.IBrowsableObjectInfo)); }

        ReadOnlyObservableCollection<Explorer.IBrowsableObjectInfo> Explorer.IBrowsableObjectInfo.SelectedItems => Model.SelectedItems;

        public bool IsSelected { get => Model.IsSelected; set => OnPropertyChanged(nameof(IsSelected), value, typeof(Explorer.IBrowsableObjectInfo)); }

        public Explorer.IBrowsableObjectInfo SelectedItem { get => Model.SelectedItem; set => OnPropertyChanged(nameof(SelectedItem), value, typeof(Explorer.IBrowsableObjectInfo)); }

        public bool IsCheckBoxEnabled { get => Model.IsCheckBoxEnabled; set => OnPropertyChanged(nameof(IsCheckBoxEnabled), value, typeof(Explorer.IBrowsableObjectInfo)); }

        public string Path => Model.Path;

        public string LocalizedName => Model.LocalizedName;

        public string Name => Model.Name;

        public FileType FileType => Model.FileType;

        public void LoadItems() => Model.LoadItems();

        public void LoadItems(BrowsableObjectInfoItemsLoader browsableObjectInfoItemsLoader) => Model.LoadItems(browsableObjectInfoItemsLoader);

        public void LoadItems(bool workerReportsProgress, bool workerSupportsCancellation, FileTypesFlags fileTypes) => Model.LoadItems(workerReportsProgress, workerSupportsCancellation, fileTypes);

        public void LoadItemsAsync() => Model.LoadItemsAsync();

        public void LoadItemsAsync(BrowsableObjectInfoItemsLoader browsableObjectInfoItemsLoader) => Model.LoadItemsAsync(browsableObjectInfoItemsLoader);

        public void LoadItemsAsync(bool workerReportsProgress, bool workerSupportsCancellation, FileTypesFlags fileTypes) => Model.LoadItemsAsync(workerReportsProgress, workerSupportsCancellation, fileTypes);

        public void Rename(string newName) => Model.Rename(newName);

        public IO.IBrowsableObjectInfo Clone() => Model.Clone();

        public void Dispose() => Model.Dispose();

        #endregion

        Explorer.IBrowsableObjectInfo Explorer.IBrowsableObjectInfoViewModel.Model => Model;

        public BrowsableObjectInfoViewModel(Explorer.IBrowsableObjectInfo model) : base(model) { }
    }
}
