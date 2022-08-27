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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
#endregion System

#region WinCopies
using WinCopies;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Windows;
using WinCopies.IO;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.Linq;
using WinCopies.Util;
using WinCopies.Util.Data;
#endregion WinCopies

using static WinCopies.Extensions.UtilHelpers;

using Enumerable = System.Linq.Enumerable;
using IEnumerator = System.Collections.IEnumerator;
#endregion Usings

namespace WinCopies.GUI.IO.ObjectModel
{
    public interface IItemSourceViewModel : IItemSource
    {
        bool IsSelected { get; set; }

        IBrowsableObjectInfoViewModel SelectedItem { get; set; }

        int SelectedIndex { get; set; }

        ObservableCollection<IBrowsableObjectInfoViewModel>
#if CS8
            ?
#endif
            Items
        { get; }

        void LoadItems();

        void UpdateItems();

        void ClearItems();
    }

    public class ItemSourceViewModel : ViewModel<IItemSource>, IItemSourceViewModel
    {
        private struct MultiInput : IMultiInput
        {
            private struct Input : IInput
            {
                private readonly KeyValuePair<string, ConnectionParameter> _keyValuePair;

                public string Name => _keyValuePair.Key;

                public string
#if CS8
                    ?
#endif
                    Description => _keyValuePair.Value.Description;

                public string
#if CS8
                    ?
#endif
                    Placeholder => _keyValuePair.Value.Placeholder;

                public string Text { get => _keyValuePair.Value.Value; set => _keyValuePair.Value.Value = value; }

                public Input(in KeyValuePair<string, ConnectionParameter> keyValuePair) => _keyValuePair = keyValuePair;
            }

            private readonly IReadOnlyDictionary<string, ConnectionParameter> _connectionParameters;

            public string Name => "Connection Parameters";

            public string Description => "Enter the connection parameters that will be used to connect to the server.";

            public MultiInput(in IReadOnlyDictionary<string, ConnectionParameter> connectionParameters) => _connectionParameters = connectionParameters;

            public IEnumerator<IInput> GetEnumerator() => _connectionParameters.WhereSelect(keyValuePair => keyValuePair.Value != null, keyValuePair => (IInput)new Input(keyValuePair)).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private bool _isSelected;
        private ICollectionView _collectionView;
        private IBrowsableObjectInfoViewModel _selectedItem;
        private int _selectedIndex = -1;

        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                if (UpdateValue(ref _isSelected, value, nameof(IsSelected)) && !value)

                    ClearItems();
            }
        }

        internal ref ICollectionView CollectionView => ref _collectionView;

        public ItemSourcesProviderViewModel ItemSourcesViewModel { get; }

        public BrowsableObjectInfoViewModel BrowsableObjectInfoViewModel => ItemSourcesViewModel.BrowsableObjectInfoViewModel;

        public IBrowsableObjectInfoViewModel SelectedItem { get => _selectedItem; set => UpdateValue(ref _selectedItem, Items == null ? throw ItemSourceViewModel.GetNoItemException() : value == null ? null : Items.Contains(value) ? value : throw new ArgumentOutOfRangeException(nameof(value)), nameof(SelectedItem)); }

        public int SelectedIndex { get => _selectedIndex; set => UpdateValue(ref _selectedIndex, Items == null ? throw ItemSourceViewModel.GetNoItemException() : value < 0 ? value : value.Between(0, Items.Count - 1) ? value : throw new IndexOutOfRangeException(), nameof(SelectedIndex)); }

        internal ObservableCollection<IBrowsableObjectInfoViewModel>
#if CS8
            ?
#endif
            ItemCollection
        { get; private set; }

        public ObservableCollection<IBrowsableObjectInfoViewModel>
#if CS8
            ?
#endif
            Items
        {
            get
            {
                LoadItems();

                return ItemCollection;
            }
        }

        public string Name => ModelGeneric.Name;

        public string Description => ModelGeneric.Description;

        public bool IsPaginationSupported => ModelGeneric.IsPaginationSupported;

        public Interval? Interval { get => ModelGeneric.Interval; set { ModelGeneric.Interval = value; OnPropertyChanged(nameof(Interval)); } }

        public IProcessSettings
#if CS8
            ?
#endif
            ProcessSettings => ModelGeneric.ProcessSettings;

        public bool IsDisposed => ModelGeneric.IsDisposed;

        protected internal ItemSourceViewModel(in ItemSourcesProviderViewModel itemSourcesViewModel, IItemSource itemSource) : base(itemSource) => ItemSourcesViewModel = itemSourcesViewModel;

        public void ClearItems()
        {
            ItemCollection = null;
            BrowsableObjectInfoViewModel.Bools.AreItemsLoaded = false;
        }

        protected static InvalidOperationException GetNoItemException() => new InvalidOperationException("No item.");

        public void Dispose() => ModelGeneric.Dispose();

        public IEnumerator<IBrowsableObjectInfo> GetEnumerator() => ModelGeneric.GetEnumerator();

        protected void ThrowIfDisposed() => ThrowHelper.ThrowIfDisposed(this);

        private void LoadItems2()
        {
            ThrowIfDisposed();

            BrowsableObjectInfoViewModel browsableObjectInfoViewModel = BrowsableObjectInfoViewModel;

            if (!BrowsableObjectInfoViewModel.Bools.AreItemsLoaded && browsableObjectInfoViewModel.IsBrowsable())
            {
                IReadOnlyDictionary<string, ConnectionParameter>
#if CS8
                    ?
#endif
                    connectionParameters = browsableObjectInfoViewModel.ConnectionParameters;

                if (connectionParameters != null)
                {
                    var multiInput = new MultiInput(connectionParameters);

                    if (new DialogWindow(multiInput.Name) { Content = multiInput, ContentTemplateSelector = new InterfaceDataTemplateSelector(), DialogButton = DialogButton.OKCancel }.ShowDialog() != true)

                        return;
                }

                bool changed = false;

                ObservableCollection<IBrowsableObjectInfoViewModel> oldItems
#if DEBUG
                    = null
#endif
                    ;

#if DEBUG
                try
                {
#endif
                    var __items = new Collections.Generic.ArrayBuilder<IBrowsableObjectInfoViewModel>((browsableObjectInfoViewModel.RootParentIsRootNode ? browsableObjectInfoViewModel.Model.GetSubRootItems() : ItemSourcesViewModel.SelectedItem?.ModelGeneric).Select(_browsableObjectInfo => browsableObjectInfoViewModel.Factory == null ? new BrowsableObjectInfoViewModel(_browsableObjectInfo/*, _filter*/, browsableObjectInfoViewModel.RootParentIsRootNode) { SortComparison = browsableObjectInfoViewModel.SortComparison, Factory = browsableObjectInfoViewModel.Factory } : browsableObjectInfoViewModel.Factory.GetBrowsableObjectInfoViewModel(_browsableObjectInfo, browsableObjectInfoViewModel)));

                    List<IBrowsableObjectInfoViewModel> itemsList = __items.ToList();

                    __items.Clear();

                    Sort(itemsList);

                    oldItems = ItemCollection;

                    ItemCollection = new ObservableCollection<IBrowsableObjectInfoViewModel>(itemsList);

                    changed = true;
#if DEBUG
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                finally
                {
#endif
                    BrowsableObjectInfoViewModel.Bools.AreItemsLoaded = true;
#if DEBUG
                }
#endif
                if (changed)
                {
                    var newSelectedItems = new Collections.Generic.ArrayBuilder<IBrowsableObjectInfoViewModel>();

                    foreach (IBrowsableObjectInfoViewModel item in ItemCollection)

                        if (item.IsSelected)

                            _ = newSelectedItems.AddLast(item);

                    foreach (IBrowsableObjectInfoViewModel item in ItemCollection)

                        item.SelectionChanged += Item_SelectionChanged;

                    ItemCollection.CollectionChanged += Items_CollectionChanged;

                    if (oldItems != null)
                    {
                        oldItems.CollectionChanged -= Items_CollectionChanged;

                        foreach (IBrowsableObjectInfoViewModel item in oldItems)

                            item.SelectionChanged -= Item_SelectionChanged;

                        var oldSelectedItems = new Collections.Generic.ArrayBuilder<IBrowsableObjectInfoViewModel>();

                        foreach (IBrowsableObjectInfoViewModel item in oldItems)

                            if (!ItemCollection.Contains(item))

                                _ = oldSelectedItems.AddLast(item);

                        if (oldSelectedItems.Count > 0)

                            RaiseSelectedItemsChanged(oldSelectedItems.ToList(), null);
                    }

                    UpdateCollectionView(ItemCollection, ref _collectionView);

                    OnPropertyChanged(nameof(Items));

                    if (newSelectedItems.Count > 0)

                        RaiseSelectedItemsChanged(null, newSelectedItems.ToList());
                }
            }
        }

        public void LoadItems() => LoadItems2();

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnItemCollectionChanged(e);

        private void UpdateCollectionView(in ObservableCollection<IBrowsableObjectInfoViewModel> items, ref System.ComponentModel.ICollectionView collectionView)
        {
            collectionView = CollectionViewSource.GetDefaultView(items);

            BrowsableObjectInfoViewModel browsableObjectInfoViewModel = BrowsableObjectInfoViewModel;

            collectionView.Filter = browsableObjectInfoViewModel.Filter == null ? null :
#if !CS9
                (Predicate<object>)
#endif
                browsableObjectInfoViewModel.Predicate;

            var groupDescription = new PropertyGroupDescription(nameof(IBrowsableObjectInfo.ItemTypeName));

            collectionView.GroupDescriptions.Add(groupDescription);
        }

        public void UpdateItems()
        {
            LoadItems2();

            var list = new List<IBrowsableObjectInfoViewModel>(ItemCollection.Count);

            foreach (var item in ItemCollection)

                list.Add(item);

            ItemCollection.Clear();

            Sort(list);

            _collectionView.Filter = null;

            UpdateCollectionView(ItemCollection, ref _collectionView);

            ItemCollection = new ObservableCollection<IBrowsableObjectInfoViewModel>(list);

            OnPropertyChanged(nameof(Items));
        }

        private void Item_SelectionChanged(IBrowsableObjectInfoViewModel sender, System.EventArgs e) => OnItemSelectionChanged(sender);

        protected virtual void OnItemSelectionChanged(IBrowsableObjectInfoViewModel item)
        {
            var list = new List<IBrowsableObjectInfoViewModel>(1) { item };

            if (item.IsSelected)

                RaiseSelectedItemsChanged(null, list);

            else

                RaiseSelectedItemsChanged(list, null);
        }

        protected virtual void RaiseSelectedItemsChanged(ItemsChangedEventArgs<IBrowsableObjectInfoViewModel> e) => BrowsableObjectInfoViewModel.RaiseSelectedItemsChanged(e);

        protected virtual void RaiseSelectedItemsChanged(IEnumerable<IBrowsableObjectInfoViewModel>
#if CS8
            ?
#endif
            oldItems, IEnumerable<IBrowsableObjectInfoViewModel>
#if CS8
            ?
#endif
            newItems) => RaiseSelectedItemsChanged(new ItemsChangedEventArgs<IBrowsableObjectInfoViewModel>(oldItems, newItems));

        public void Sort(in List<IBrowsableObjectInfoViewModel> list)
        {
            BrowsableObjectInfoViewModel browsableObjectInfoViewModel = BrowsableObjectInfoViewModel;

            if (browsableObjectInfoViewModel.SortComparison == null)

                list.Sort();

            else

                list.Sort(browsableObjectInfoViewModel.SortComparison);
        }

        protected virtual void OnItemCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var oldSelectedItems = new Collections.Generic.ArrayBuilder<IBrowsableObjectInfoViewModel>();
            var newSelectedItems = new Collections.Generic.ArrayBuilder<IBrowsableObjectInfoViewModel>();

#if CS8
            static
#endif
                void runAction(in IEnumerable enumerable, in Action<IBrowsableObjectInfoViewModel> action) => RunActionIfNotNull(enumerable, action);


#if CS8
            static
#endif
                IList<IBrowsableObjectInfoViewModel>
#if CS8
            ?
#endif
                getList(in Collections.Generic.ArrayBuilder<IBrowsableObjectInfoViewModel> arrayBuilder) => arrayBuilder.Count > 0 ? arrayBuilder.ToList() : null;

            runAction(e.OldItems, item =>
            {
                if (item.IsSelected)

                    _ = oldSelectedItems.AddLast(item);

                item.SelectionChanged -= Item_SelectionChanged;
            });

            runAction(e.NewItems, item =>
            {
                if (item.IsSelected)

                    _ = newSelectedItems.AddLast(item);

                item.SelectionChanged += Item_SelectionChanged;
            });

            if (oldSelectedItems.Count > 0 || newSelectedItems.Count > 0)

                RaiseSelectedItemsChanged(getList(oldSelectedItems), getList(newSelectedItems));
        }

#if !(WinCopies3 && CS8)
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
    }

    public interface IItemSourcesProviderViewModel : IItemSourcesProvider, INotifyPropertyChanged
    {
        IItemSourceViewModel Default { get; }

        IReadOnlyList<IItemSourceViewModel> ItemSources { get; }

        IItemSourceViewModel SelectedItem { get; }

        int SelectedIndex { get; set; }
    }

    public class ItemSourcesProviderViewModel : ViewModel<IItemSourcesProvider>, IItemSourcesProviderViewModel
    {
        private int _selectedIndex;

        public IReadOnlyList<ItemSourceViewModel> ItemSources { get; }

        IReadOnlyList<IItemSourceViewModel> IItemSourcesProviderViewModel.ItemSources => ItemSources;

        public ItemSourceViewModel
#if CS8
            ?
#endif
            SelectedItem => ItemSources[_selectedIndex];

        public int SelectedIndex { get => _selectedIndex; set => UpdateValue(ref _selectedIndex, value < 0 ? 0 : value.Between(0, ItemSources.Count - 1) ? value : throw new IndexOutOfRangeException(), nameof(SelectedIndex)); }

        public ItemSourceViewModel Default { get; }

        public IEnumerable<ItemSourceViewModel>
#if CS8
            ?
#endif
            ExtraItemSources
        { get; }

        public BrowsableObjectInfoViewModel BrowsableObjectInfoViewModel
        {
            get; protected
#if CS9
            init;
#else
            set;
#endif
        }

        IItemSourceViewModel
#if CS8
            ?
#endif
            IItemSourcesProviderViewModel.SelectedItem => SelectedItem;

        IItemSourceViewModel IItemSourcesProviderViewModel.Default => Default;
        IItemSource IItemSourcesProvider.Default => Default;

        IEnumerable<IItemSource>
#if CS8
            ?
#endif
            IItemSourcesProvider.ExtraItemSources => ExtraItemSources;

        protected ItemSourcesProviderViewModel(in IItemSourcesProvider itemSources) : base(itemSources)
        {
            if ((ItemSources = new Collections.Generic.ReadOnlyArray<ItemSourceViewModel>(new Collections.Generic.ArrayBuilder<ItemSourceViewModel>((itemSources.ExtraItemSources?.Select(item => new ItemSourceViewModel(this, item)) ?? System.Linq.Enumerable.Empty<ItemSourceViewModel>()).Prepend(Default = new ItemSourceViewModel(this, itemSources.Default))).ToArray())).Count > 1)

                ExtraItemSources = new Collections.Generic.SubReadOnlyList<ItemSourceViewModel>(ItemSources, 1);

            _selectedIndex = 0;
        }

        public static ItemSourcesProviderViewModel
#if CS8
            ?
#endif
            Construct(in BrowsableObjectInfoViewModel browsableObjectInfoViewModel) => new ItemSourcesProviderViewModel(browsableObjectInfoViewModel.Model.ItemSources ?? throw new ArgumentException("No item sources provider.", nameof(browsableObjectInfoViewModel))) { BrowsableObjectInfoViewModel = browsableObjectInfoViewModel };

        IEnumerator<IItemSource> IEnumerable<IItemSource>.GetEnumerator() => ModelGeneric.GetEnumerator();

        public IEnumerator<IItemSourceViewModel> GetEnumerator() => ItemSources.GetEnumerator();

#if !(WinCopies3 && CS8)
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
    }

    public interface IBrowsableObjectInfoViewModel : IBrowsableObjectInfo, IBrowsableObjectInfoViewModelCommon, IEquatable<IBrowsableObjectInfoViewModel>, IComparable<IBrowsableObjectInfoViewModel>
    {
        Predicate<IBrowsableObjectInfo> Filter { get; set; }

        bool RootParentIsRootNode { get; }

        IBrowsableObjectInfo Model { get; }

        bool IsSelected { get; set; }

        IItemSourcesProviderViewModel
#if CS8
            ?
#endif
            ItemSources
        { get; }

        Comparison<IBrowsableObjectInfo> SortComparison { get; set; }

        IBrowsableObjectInfoFactory Factory { get; set; }

        event EventHandler<IBrowsableObjectInfoViewModel> SelectionChanged;

        event ItemsChangedEventHandler<IBrowsableObjectInfoViewModel> SelectedItemsChanged;
    }
}
