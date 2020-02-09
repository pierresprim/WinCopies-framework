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

using AttachedCommandBehavior;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WinCopies.IO;
using WinCopies.Util;
using BackgroundWorker = WinCopies.Util.BackgroundWorker;
using NotifyCollectionChangedEventArgs = System.Collections.Specialized.NotifyCollectionChangedEventArgs;
using Generic = WinCopies.GUI.Explorer.Themes.Generic;
using WinCopies.GUI.Controls;
using TextBox = System.Windows.Controls.TextBox;
using System.Windows.Interop;
using System.ComponentModel;
using WinCopies.Util.Commands;
using static WinCopies.Util.Util;
using WinCopies.Collections;
using WinCopies.Util.Data;
using MenuItem = WinCopies.Util.Data.MenuItem;
using ApplicationCommands = System.Windows.Input.ApplicationCommands;

namespace WinCopies.GUI.Explorer
{

    public delegate void PasteHandler(bool isAFileMoving, StringCollection sc, string destPath);

    public delegate void RenameHandler(Explorer.IBrowsableObjectInfo path, string newName);

    public delegate void DeleteHandler(Explorer.IBrowsableObjectInfo[] paths);

    public class ExplorerControl : Control
    {

        // todo: to replace with data binding

        /// <summary>
        /// Gets this <see cref="ExplorerControl"/>'s <see cref="TreeView"/>.
        /// </summary>
        public TreeView TreeView { get; private set; } = null;

        /// <summary>
        /// Gets this <see cref="ExplorerControl"/>'s <see cref="ListView"/>.
        /// </summary>
        public ListView ListView { get; private set; } = null;

        private TextBox PART_TextBox = null;

        private BackgroundWorker fileOpeningBgWorker = null;

        private List<ShellFile> pathsToOpen = null;

        private readonly FileSystemWatcher fsw = new FileSystemWatcher();

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ExplorerControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the text of this <see cref="ExplorerControl"/>. This is a dependency property.
        /// </summary>
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        private static readonly DependencyPropertyKey PathPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Path), typeof(Explorer.IBrowsableObjectInfo), typeof(ExplorerControl), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ExplorerControl)d).RaiseEvent(new RoutedEventArgs<ValueChangedEventArgs<Explorer.IBrowsableObjectInfo>>(PathChangedEvent, d, new ValueChangedEventArgs<Explorer.IBrowsableObjectInfo>((Explorer.IBrowsableObjectInfo)e.OldValue, (Explorer.IBrowsableObjectInfo)e.NewValue)))));

        /// <summary>
        /// Identifies the <see cref="Path"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathProperty = PathPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="Explorer.IBrowsableObjectInfo"/> which represents the path of this <see cref="ExplorerControl"/>. This is a dependency property.
        /// </summary>
        public Explorer.IBrowsableObjectInfo Path => (Explorer.IBrowsableObjectInfo)GetValue(PathProperty);

        private static readonly DependencyPropertyKey IsLoadingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsLoading), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsLoading"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty = IsLoadingPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets a value that indicates if this <see cref="ExplorerControl"/> is loading items. The <see cref="FrameworkElement.IsLoaded"/> property and the <see cref="FrameworkElement.Loaded"/> event are not concerned with this property. This is a dependency property.
        /// </summary>
        public bool IsLoading { get => (bool)GetValue(IsLoadingProperty); set => SetValue(IsLoadingPropertyKey, value); }

        private static readonly DependencyPropertyKey HeaderPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Header), typeof(string), typeof(ExplorerControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = HeaderPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="Path"/> header of this <see cref="ExplorerControl"/>. This is a dependency property.
        /// </summary>
        public string Header => (string)GetValue(HeaderProperty);

        /// <summary>
        /// Identifies the <see cref="SelectionMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionMode), typeof(ExplorerControl), new PropertyMetadata(SelectionMode.Extended));

        /// <summary>
        /// Gets or sets whether activate multiple selection. This is dependency property.
        /// </summary>
        public SelectionMode SelectionMode { get => (SelectionMode)GetValue(SelectionModeProperty); set => SetValue(SelectionModeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ShowItemsCheckBox"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowItemsCheckBoxProperty = DependencyProperty.Register(nameof(ShowItemsCheckBox), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that indicates whether to show the items check boxes when multi-selection is available. This is a dependency property.
        /// </summary>
        public bool ShowItemsCheckBox { get => (bool)GetValue(ShowItemsCheckBoxProperty); set => SetValue(ShowItemsCheckBoxProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ShowHiddenItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowHiddenItemsProperty = DependencyProperty.Register(nameof(ShowHiddenItems), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that indicates whether to show the hidden items.
        /// </summary>
        public bool ShowHiddenItems { get => (bool)GetValue(ShowHiddenItemsProperty); set => SetValue(ShowHiddenItemsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ShowSystemItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowSystemItemsProperty = DependencyProperty.Register(nameof(ShowSystemItems), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that indicates whether to show the system items.
        /// </summary>
        public bool ShowSystemItems { get => (bool)GetValue(ShowSystemItemsProperty); set => SetValue(ShowSystemItemsProperty, value); }

        //public static readonly DependencyPropertyKey TreeViewSelectedItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof(TreeViewSelectedItem), typeof(Explorer.IBrowsableObjectInfo), typeof(ExplorerControl), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        //{

        //    Debug.WriteLine(((ShellObjectInfo)e.NewValue).Path + " " + ((ShellObjectInfo)e.NewValue).ShellObject.ParsingName);

        //}));

        ///// <summary>
        ///// Identifies the <see cref="TreeViewSelectedItem"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty TreeViewSelectedItemProperty = TreeViewSelectedItemPropertyKey.DependencyProperty;

        ///// <summary>
        ///// Gets or sets the tree view selected item.
        ///// </summary>
        //public Explorer.IBrowsableObjectInfo TreeViewSelectedItem { get => (Explorer.IBrowsableObjectInfo)GetValue(TreeViewSelectedItemProperty); internal set => SetValue(TreeViewSelectedItemPropertyKey, value); }

        ///// <summary>
        ///// Identifies the <see cref="SelectedItem"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(IO.Explorer.IBrowsableObjectInfo), typeof(ExplorerControl), new PropertyMetadata(null));

        ///// <summary>
        ///// Gets or sets the list view selected item.
        ///// </summary>
        //public Explorer.IBrowsableObjectInfo SelectedItem { get => (Explorer.IBrowsableObjectInfo)GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        //private static readonly DependencyPropertyKey SelectedItemsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedItems), typeof(ObservableListBoxSelectedItems), typeof(ExplorerControl), new PropertyMetadata(null));

        ///// <summary>
        ///// Identifies the <see cref="SelectedItems"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;

        ///// <summary>
        ///// Gets the list view selected items.
        ///// </summary>
        //public ObservableListBoxSelectedItems SelectedItems => (ObservableListBoxSelectedItems)GetValue(SelectedItemsProperty);

        private System.Collections.ObjectModel.ObservableCollection<IHistoryItemData> history = new System.Collections.ObjectModel.ObservableCollection<IHistoryItemData>();

        private static readonly DependencyPropertyKey HistoryPropertyKey = DependencyProperty.RegisterReadOnly(nameof(History), typeof(System.Collections.ObjectModel.ReadOnlyObservableCollection<IHistoryItemData>), typeof(ExplorerControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="History"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HistoryProperty = HistoryPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the history of the browsed paths.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyObservableCollection<IHistoryItemData> History { get => (System.Collections.ObjectModel.ReadOnlyObservableCollection<IHistoryItemData>)GetValue(HistoryProperty); }

        private static readonly DependencyPropertyKey HistorySelectedIndexPropertyKey = DependencyProperty.RegisterReadOnly(nameof(HistorySelectedIndex), typeof(int), typeof(ExplorerControl), new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="HistorySelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HistorySelectedIndexProperty = HistorySelectedIndexPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the history selected index.
        /// </summary>
        public int HistorySelectedIndex => (int)GetValue(HistorySelectedIndexProperty);

        /// <summary>
        /// Identifies the <see cref="ViewStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewStyleProperty = DependencyProperty.Register(nameof(ViewStyle), typeof(ViewStyles), typeof(ExplorerControl), new PropertyMetadata(ViewStyles.Tiles, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

        {

            ((ExplorerControl)d).IsAutomaticItemContainerGeneratorStatusChange = true;

        }));

        /// <summary>
        /// Gets or sets the view style of the ListView.
        /// </summary>
        public ViewStyles ViewStyle { get => (ViewStyles)GetValue(ViewStyleProperty); set => SetValue(ViewStyleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="TreeViewItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TreeViewItemsProperty = DependencyProperty.Register(nameof(TreeViewItems), typeof(System.Collections.ObjectModel.ObservableCollection<Explorer.IBrowsableObjectInfo>), typeof(ExplorerControl), new PropertyMetadata(

            GetDefaultTreeViewItems(), (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

            {

                foreach (Explorer.IBrowsableObjectInfo item in (ObservableCollection<Explorer.IBrowsableObjectInfo>)e.OldValue)

                    item.Dispose();

            }

            ));

        public System.Collections.ObjectModel.ObservableCollection<Explorer.IBrowsableObjectInfo> TreeViewItems { get => (System.Collections.ObjectModel.ObservableCollection<Explorer.IBrowsableObjectInfo>)GetValue(TreeViewItemsProperty); set => SetValue(TreeViewItemsProperty, value); }

        private static readonly DependencyPropertyKey CanMoveToPreviousPathPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanMoveToPreviousPath), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanMoveToPreviousPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToPreviousPathProperty = CanMoveToPreviousPathPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether it is possible to move to the previous path of the history.
        /// </summary>
        public bool CanMoveToPreviousPath => (bool)GetValue(CanMoveToPreviousPathProperty);

        private static readonly DependencyPropertyKey CanMoveToNextPathPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanMoveToNextPath), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanMoveToNextPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToNextPathProperty = CanMoveToNextPathPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether it is possible to move to the next path of the history.
        /// </summary>
        public bool CanMoveToNextPath => (bool)GetValue(CanMoveToNextPathProperty);

        private static readonly DependencyPropertyKey CanMoveToParentPathPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanMoveToParentPath), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanMoveToParentPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToParentPathProperty = CanMoveToParentPathPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether it is possible to move to the parent path.
        /// </summary>
        public bool CanMoveToParentPath => (bool)GetValue(CanMoveToParentPathProperty);

        private static readonly DependencyPropertyKey VisibleItemsCountPropertyKey = DependencyProperty.RegisterReadOnly(nameof(VisibleItemsCount), typeof(int), typeof(ExplorerControl), new PropertyMetadata(0, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ExplorerControl)d).RaiseEvent(new RoutedEventArgs<ValueChangedEventArgs<int>>(VisibleItemsCountChangedEvent, d, new ValueChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue)))));

        /// <summary>
        /// Identifies the <see cref="VisibleItemsCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleItemsCountProperty = VisibleItemsCountPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the visible items count.
        /// </summary>
        public int VisibleItemsCount => (int)GetValue(VisibleItemsCountProperty);

        /// <summary>
        /// Identifies the <see cref="ItemContextMenu"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContextMenuProperty = DependencyProperty.Register(nameof(ItemContextMenu), typeof(ContextMenu), typeof(ExplorerControl));

        public ContextMenu ItemContextMenu { get => (ContextMenu)GetValue(ItemContextMenuProperty); set => SetValue(ItemContextMenuProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Filter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter), typeof(IEnumerable<string>), typeof(ExplorerControl));

        public IEnumerable<string> Filter { get => (IEnumerable<string>)GetValue(FilterProperty); set => SetValue(FilterProperty, value); }

        private static readonly DependencyPropertyKey CanCopyPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanCopy), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanCopy"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanCopyProperty = CanCopyPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether a copy can be made.
        /// </summary>
        public bool CanCopy => (bool)GetValue(CanCopyProperty);

        private static readonly DependencyPropertyKey CanCutPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanCut), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanCut"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanCutProperty = CanCutPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether a cut can be made.
        /// </summary>
        public bool CanCut => (bool)GetValue(CanCutProperty);

        private static readonly DependencyPropertyKey CanPastePropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanPaste), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanPaste"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanPasteProperty = CanPastePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether a paste action can be made.
        /// </summary>
        public bool CanPaste => (bool)GetValue(CanPasteProperty);

        private static readonly DependencyPropertyKey CanRenamePropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanRename), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanRename"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanRenameProperty = CanRenamePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether a rename can be made.
        /// </summary>
        public bool CanRename => (bool)GetValue(CanRenameProperty);

        private static readonly DependencyPropertyKey CanDeletePropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanDelete), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanDelete"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanDeleteProperty = CanDeletePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether a deletion can be made.
        /// </summary>
        public bool CanDelete => (bool)GetValue(CanDeleteProperty);

        // todo: to add a default implementation:

        /// <summary>
        /// Identifies the <see cref="PasteAction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PasteActionProperty = DependencyProperty.Register(nameof(PasteAction), typeof(PasteHandler), typeof(ExplorerControl));

        public PasteHandler PasteAction { get => (PasteHandler)GetValue(PasteActionProperty); set => SetValue(PasteActionProperty, value); }

        // todo: to add a default implementation:

        /// <summary>
        /// Identifies the <see cref="RenameAction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenameActionProperty = DependencyProperty.Register(nameof(RenameAction), typeof(RenameHandler), typeof(ExplorerControl));

        public RenameHandler RenameAction { get => (RenameHandler)GetValue(RenameActionProperty); set => SetValue(RenameActionProperty, value); }

        // todo: to add a default implementation:
        /// <summary>
        /// Identifies the <see cref="DeleteAction"/> dependency property.
        /// </summary>

        public static readonly DependencyProperty DeleteActionProperty = DependencyProperty.Register(nameof(DeleteAction), typeof(DeleteHandler), typeof(ExplorerControl));

        public DeleteHandler DeleteAction { get => (DeleteHandler)GetValue(DeleteActionProperty); set => SetValue(DeleteActionProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ArchiveFormatsToOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ArchiveFormatsToOpenProperty = DependencyProperty.Register(nameof(ArchiveFormatsToOpen), typeof(InArchiveFormats), typeof(ExplorerControl), new PropertyMetadata(GetAllEnumFlags<InArchiveFormats>()));

        public InArchiveFormats ArchiveFormatsToOpen { get => (InArchiveFormats)GetValue(ArchiveFormatsToOpenProperty); set => SetValue(ArchiveFormatsToOpenProperty, value); }

        /// <summary>
        /// Identifies the <see cref="OpenDirectoriesDirectly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenDirectoriesDirectlyProperty = DependencyProperty.Register(nameof(OpenDirectoriesDirectly), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(true));

        public bool OpenDirectoriesDirectly { get => (bool)GetValue(OpenDirectoriesDirectlyProperty); set => SetValue(OpenDirectoriesDirectlyProperty, value); }

        /// <summary>
        /// Identifies the <see cref="OpenFilesDirectly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenFilesDirectlyProperty = DependencyProperty.Register(nameof(OpenFilesDirectly), typeof(bool), typeof(ExplorerControl), new PropertyMetadata(true));

        public bool OpenFilesDirectly { get => (bool)GetValue(OpenFilesDirectlyProperty); set => SetValue(OpenFilesDirectlyProperty, value); }

        /// <summary>
        /// Identifies the <see cref="OpenMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenModeProperty = DependencyProperty.Register(nameof(OpenMode), typeof(OpenMode), typeof(ExplorerControl), new PropertyMetadata(OpenMode.OnDoubleClick, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

        {

            OpenMode openMode = (OpenMode)e.NewValue;

            openMode.ThrowIfNotValidEnumValue();

            ExplorerControl explorerControl = (ExplorerControl)d;

            ListViewItem listViewItem = null;

            for (int i = 0; i < explorerControl.Path.Items.Count; i++)

            {

                listViewItem = explorerControl.ListView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;

                void tryRemove()

                {

                    for (int _i = 0; _i < listViewItem.InputBindings.Count;)

                        if (listViewItem.InputBindings[_i].Command == Commands.Open)

                            listViewItem.InputBindings.RemoveAt(_i);

                        else

                            _i++;

                }

                tryRemove();

                if (openMode == OpenMode.OnFirstClick)

                    listViewItem.InputBindings.Add(new InputBinding(Commands.Open, new MouseGesture(MouseAction.LeftClick)));

                else if (openMode == OpenMode.OnDoubleClick)

                {

                    explorerControl.UnderliningMode = LinkUnderliningMode.None;

                    listViewItem.InputBindings.Add(new InputBinding(Commands.Open, new MouseGesture(MouseAction.LeftDoubleClick)));

                }

            }

        }));

        public OpenMode OpenMode { get => (OpenMode)GetValue(OpenModeProperty); set => SetValue(OpenModeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="UnderliningMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnderliningModeProperty = DependencyProperty.Register(nameof(LinkUnderliningMode), typeof(LinkUnderliningMode), typeof(ExplorerControl), new PropertyMetadata(LinkUnderliningMode.None, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

        {

            ExplorerControl explorerControl = d as ExplorerControl;

            if (explorerControl != null)

            {

                if ((LinkUnderliningMode)e.NewValue != LinkUnderliningMode.None && explorerControl.OpenMode == OpenMode.OnDoubleClick)

                    throw new ArgumentException("UnderliningMode must be LinkUnderliningMode.None when OpenMode is OnDoubleClick");

            }

        }));

        public LinkUnderliningMode UnderliningMode { get => (LinkUnderliningMode)GetValue(UnderliningModeProperty); set => SetValue(UnderliningModeProperty, value); }

        // public static readonly DependencyProperty BrowsableObjectInfoItemsLoaderProperty = DependencyProperty.Register("BrowsableObjectInfoItemsLoader", typeof(BrowsableObjectInfoItemsLoader), typeof(ExplorerControl), new PropertyMetadata()));

        // private WinCopies.IO.BrowsableObjectInfoItemsLoader BrowsableObjectInfoItemsLoader = null;

        // todo: to turn all events to routed events

        /// <summary>
        /// Identifies the <see cref="PathChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PathChangedEvent = EventManager.RegisterRoutedEvent(nameof(PathChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueChangedEventArgs<Explorer.IBrowsableObjectInfo>>), typeof(ExplorerControl));

        /// <summary>
        /// Occurs when the <see cref="Path"/> property has changed.
        /// </summary>
        public event RoutedEventHandler<ValueChangedEventArgs<Explorer.IBrowsableObjectInfo>> PathChanged
        {

            add => AddHandler(PathChangedEvent, value);

            remove => RemoveHandler(PathChangedEvent, value);

        }

        /// <summary>
        /// Identifies the <see cref="NavigationRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent NavigationRequestedEvent = EventManager.RegisterRoutedEvent(nameof(NavigationRequested), RoutingStrategy.Bubble, typeof(RoutedEventHandler<EventArgs<Explorer.IBrowsableObjectInfo>>), typeof(ExplorerControl));

        /// <summary>
        /// Occurs when a navigation is requested. This event occurs only if the <see cref="OpenDirectoriesDirectly"/> is set to <see langword="false"/>.
        /// </summary>
        public event RoutedEventHandler<EventArgs<Explorer.IBrowsableObjectInfo>> NavigationRequested
        {

            add => AddHandler(NavigationRequestedEvent, value);

            remove => RemoveHandler(NavigationRequestedEvent, value);

        }

        /// <summary>
        /// Identifies the <see cref="MultiplePathsOpeningRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MultiplePathsOpeningRequestedEvent = EventManager.RegisterRoutedEvent(nameof(MultiplePathsOpeningRequested), RoutingStrategy.Bubble, typeof(RoutedEventHandler<EventArgs<Explorer.IBrowsableObjectInfo[]>>), typeof(ExplorerControl));

        /// <summary>
        /// Occurs when an array of paths have been requested for opening. When the <see cref="ExplorerControl"/> raises this event, if the <see cref="OpenDirectoriesDirectly"/> is set to <see langword="true"/>, it has also tried to open the first path of the <see cref="EventArgs{T}.Value"/> array. If this property is set to <see langword="false"/>, the <see cref="NavigationRequested"/> event is raised instead.
        /// </summary>
        public event RoutedEventHandler<EventArgs<Explorer.IBrowsableObjectInfo[]>> MultiplePathsOpeningRequested
        {

            add => AddHandler(MultiplePathsOpeningRequestedEvent, value);

            remove => RemoveHandler(MultiplePathsOpeningRequestedEvent, value);

        }

        /// <summary>
        /// Identifies the <see cref="FilesOpeningRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent FilesOpeningRequestedEvent = EventManager.RegisterRoutedEvent(nameof(FilesOpeningRequested), RoutingStrategy.Bubble, typeof(RoutedEventHandler<EventArgs<Explorer.IBrowsableObjectInfo[]>>), typeof(ExplorerControl));

        public event RoutedEventHandler<EventArgs<Explorer.IBrowsableObjectInfo[]>> FilesOpeningRequested
        {

            add => AddHandler(FilesOpeningRequestedEvent, value);

            remove => RemoveHandler(FilesOpeningRequestedEvent, value);

        }

        /// <summary>
        /// Identifies the <see cref="TextChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(nameof(TextChanged), RoutingStrategy.Bubble, typeof(TextChangedEventHandler), typeof(ExplorerControl));

        /// <summary>
        /// Occurs when the <see cref="Text"/> property has changed.
        /// </summary>
        public event TextChangedEventHandler TextChanged
        {

            add => AddHandler(TextChangedEvent, value);

            remove => RemoveHandler(TextChangedEvent, value);

        }

        /// <summary>
        /// Identifies the <see cref="SelectionChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(ExplorerControl));

        /// <summary>
        /// Occurs when the selection has changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged
        {

            add => AddHandler(SelectionChangedEvent, value);

            remove => RemoveHandler(SelectionChangedEvent, value);

        }

        /// <summary>
        /// Identifies the <see cref="VisibleItemsCountChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent VisibleItemsCountChangedEvent = EventManager.RegisterRoutedEvent(nameof(VisibleItemsCountChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueChangedEventArgs<int>>), typeof(ExplorerControl));

        /// <summary>
        /// Occurs when the visible items count changed.
        /// </summary>
        public event RoutedEventHandler<ValueChangedEventArgs<int>> VisibleItemsCountChanged
        {

            add => AddHandler(VisibleItemsCountChangedEvent, value);

            remove => RemoveHandler(VisibleItemsCountChangedEvent, value);

        }

        static ExplorerControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerControl), new FrameworkPropertyMetadata(typeof(ExplorerControl)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplorerControl"/> class.
        /// </summary>
        public ExplorerControl() : this((Explorer.IBrowsableObjectInfo)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplorerControl"/> class using a custom shell object path.
        /// </summary>
        public ExplorerControl(string path) : this(new ShellObjectInfo(ShellObject.FromParsingName(path), path)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplorerControl"/> class using a custom path.
        /// </summary>
        public ExplorerControl(Explorer.IBrowsableObjectInfo path) => Init(path);

        private void Init(Explorer.IBrowsableObjectInfo path)

        {

            CommandBindings.Add(new CommandBinding(Commands.Open, Open_Executed, Open_CanExecute));

            CommandBindings.Add(new CommandBinding(Commands.SingleClickSelection, SingleClickSelection_Executed, SingleClickSelection_CanExecute));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_CanExecute));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_CanExecute));

            CommandBindings.Add(new CommandBinding(FileSystemCommands.Rename, Rename_Executed, Rename_CanExecute));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_CanExecute));

            InputBindings.Add(new InputBinding(ApplicationCommands.SelectAll, new KeyGesture(Key.A, ModifierKeys.Control)));

            // todo: to move the 'e.CanExecute = true' handler to the WinCopies.Util dll

            // CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, (object sender, ExecutedRoutedEventArgs e) => { ListView.Focus(); ListView.SelectAll(); }, ));

            SetValue(HistoryPropertyKey, new System.Collections.ObjectModel.ReadOnlyObservableCollection<IHistoryItemData>(history));

            history.CollectionChanged += History_CollectionChanged;

            PathChanged += ExplorerControl_PathChanged;

            TextChanged += ExplorerControl_TextChanged;

            if (path == null)

            {

                string _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                path = new ShellObjectInfo(ShellObject.FromParsingName(_path), _path);

            }

            Open(path);

            // InputBindings.Add(new MouseBinding(Commands.Open, new MouseGesture(MouseAction.LeftDoubleClick)));

            // CommandBindings.Add(new CommandBinding(Util.Util.CommonCommand, Open_Execute, Open_CanExecute));

            SelectionChanged += ExplorerControl_SelectionChanged;

            #region Comments

            //CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Command_CanExecute));

            //CommandBindings.Add(new CommandBinding(Commands.CreateShortcut, CreateShortcut_Executed, CanWrite_CanExecute));

            //CommandBindings.Add(new CommandBinding(Commands.Properties, Properties_Executed, Command_CanExecute));

            #endregion

        }

        private void SingleClickSelection_CanExecute(object sender, CanExecuteRoutedEventArgs e)

        {

            Window window = Window.GetWindow(this);

            //       

            //Window window = Window.GetWindow(this);

            e.CanExecute = window == null ? false : Microsoft.WindowsAPICodePack.ShellExtensions.WindowUtilities.IsOnForeground(new WindowInteropHelper(window).Handle) /*window == null || window.IsFocused*/;

        }

        private void SingleClickSelection_Executed(object sender, ExecutedRoutedEventArgs e)

        {

            Explorer.IBrowsableObjectInfo _obj = (Explorer.IBrowsableObjectInfo)e.Parameter;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))

                _obj.IsSelected = !_obj.IsSelected;

            else

            {

                IO.IBrowsableObjectInfo p = _obj.Parent;

                foreach (IO.IBrowsableObjectInfo browsableObjectInfo in p.Items)

                    if (browsableObjectInfo is Explorer.IBrowsableObjectInfo _browsableObjectInfo)

                        _browsableObjectInfo.IsSelected = false;

                _obj.IsSelected = true;

            }

        }

        private void ExplorerControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            bool selectedItemIsBrowsableObjectInfo;

            bool selectedItemIsShellObjectInfo;

            if (ListView.SelectedItem is Explorer.IBrowsableObjectInfo browsableObjectInfo)

            {

                selectedItemIsBrowsableObjectInfo = true;

                selectedItemIsShellObjectInfo = browsableObjectInfo is IShellObjectInfo shellObjectInfo && shellObjectInfo.ShellObject.IsFileSystemObject;

            }

            else

                selectedItemIsBrowsableObjectInfo = selectedItemIsShellObjectInfo = false;

            SetValue(CanRenamePropertyKey, selectedItemIsBrowsableObjectInfo);

            SetValue(CanDeletePropertyKey, selectedItemIsBrowsableObjectInfo);

            SetValue(CanCopyPropertyKey, selectedItemIsShellObjectInfo);

            SetValue(CanCutPropertyKey, selectedItemIsShellObjectInfo);

        }

        internal void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e) => OnOpenCanExecute(e);

        internal void Open_Executed(object sender, ExecutedRoutedEventArgs e) => OnOpening(ListView.SelectedItems.OfType<Explorer.IBrowsableObjectInfo>().ToArray());

        protected virtual void OnCopyCanExecute(CanExecuteRoutedEventArgs e) => e.CanExecute = CanCopy;//e.ContinueRouting = false;//e.Handled = true;//raise

        private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e) => OnCopyCanExecute(e);

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e) =>

            // if (sender == this)

            Copy(ActionsFromObjects.ListView);

        protected virtual void OnCutCanExecute(CanExecuteRoutedEventArgs e) => e.CanExecute = CanCut;

        private void Cut_CanExecute(object sender, CanExecuteRoutedEventArgs e) => OnCutCanExecute(e);

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e) =>

            // if (sender == this)

            Cut(ActionsFromObjects.ListView);

        protected virtual void OnRenameCanExecute(CanExecuteRoutedEventArgs e) => e.CanExecute = CanRename;

        private void Rename_CanExecute(object sender, CanExecuteRoutedEventArgs e) => OnRenameCanExecute(e);

        private void Rename_Executed(object sender, ExecutedRoutedEventArgs e) =>

            // if (sender == this)

            RenameAction?.Invoke(ListView.SelectedItem as Explorer.IBrowsableObjectInfo, e.Parameter as string);

        protected virtual void OnDeleteCanExecute(CanExecuteRoutedEventArgs e) => e.CanExecute = CanDelete;

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => OnDeleteCanExecute(e);

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e) =>

            // if (sender == this)

            DeleteAction?.Invoke(ListView.SelectedItems.OfType<Explorer.IBrowsableObjectInfo>().ToArray());

        private bool IsAutomaticItemContainerGeneratorStatusChange = false;

        // internal void RaiseTextChangedEvent(TextChangedEventArgs e) => RaiseEvent(e);

        internal void RaiseSelectionChangedEvent(SelectionChangedEventArgs e) => RaiseEvent(e);

        protected virtual void OnPathChanged(Explorer.IBrowsableObjectInfo oldValue, Explorer.IBrowsableObjectInfo newValue)

        {
            // Console.WriteLine((PART_TextBox != null).ToString()+" "+(!PART_TextBox.IsFocused).ToString());
            if (PART_TextBox == null || PART_TextBox != null && !PART_TextBox.IsFocused)

                Text = newValue.Path;

            // if (value != null)

            // if (e.Action == NotifyCollectionChangedAction.Reset)    

            if (oldValue != null)

            {

                // ((INotifyCollectionChanging)oldValue.Items).CollectionChanging -= Path_CollectionChanging;

                // foreach (Explorer.IBrowsableObjectInfo value in oldValue.Items)

                // {

                // object content = ((ListViewItem)ListView.ItemContainerGenerator.ContainerFromItem(value)).Content;

                // ((ListViewItem)ListView.ItemContainerGenerator.ContainerFromItem(value)).IsVisibleChanged -= Value_IsVisibleChanged;

                // }

                if (oldValue is BrowsableObjectInfo)

                    oldValue.Dispose();

                // oldValue.Items.CollectionChanging -= Items_CollectionChanging;

                listViewItems.Clear();

                // SetValue(VisibleItemsCountPropertyKey, 0);

            }

            // ((INotifyCollectionChanging)newValue.Items).CollectionChanging += Path_CollectionChanging;

            // newValue.Items.CollectionChanging += Items_CollectionChanging;

        }

        // private void Path_CollectionChanging(object sender, WinCopies.Util.NotifyCollectionChangedEventArgs e)

        // {

        // if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset) && e.ResetItems != null)

        // foreach (Explorer.IBrowsableObjectInfo value in e.ResetItems)

        // object value = ((ListViewItem)ListView.ItemContainerGenerator.ContainerFromItem(ListView.ItemContainerGenerator.Items[i]));

        // ((ListViewItem)ListView.ItemContainerGenerator.ContainerFromItem(value)).IsVisibleChanged -= Value_IsVisibleChanged;

        // }

        protected virtual void OnTextChanged(TextChangedEventArgs e)

        {

            // todo: checking if a display path is available, and so showing the list of the available sub-paths

        }

        protected internal virtual void OnItemsControlContextMenuOpening(FrameworkElement sender, ISelectionIndexableSettableSelector source, ContextMenuEventArgs e)

        {

            // if (ItemContextMenu.PlacementTarget == null) return;
            //focused
            //if (ListView.IsAncestorOf(((FrameworkElement)ContextMenu.PlacementTarget)))

            //void callBack()

            //{

            if (source is ListView)

                if (((ListView)source).SelectedItems.Count > 1) { }

                else if (((ListView)source).SelectedItem != null)

                    sender.ContextMenu.ItemsSource = GetDefaultListViewItemContextMenu();

            //}

            //if (!Dispatcher.CheckAccess())

            //    Dispatcher.InvokeAsync(callBack);

            //else

            //    callBack();

        }

        private void Open(object value) => Open();

        public List<MenuItem> GetDefaultListViewItemContextMenu()

        {

            List<MenuItem> menuItems = new List<MenuItem>();

            MenuItem menuItem = new MenuItem(Themes.Generic.Open, null, new DelegateCommand { ExecuteDelegate = Open }, Path.SelectedItem, null);

            string extension = System.IO.Path.GetExtension(Path.SelectedItem.Path);

            if (!(Path.SelectedItem.FileType == FileType.Archive && ArchiveLoader.IsSupportedArchiveFormat(extension)) && extension.Length > 0)

            {

                string command = Registry.GetCommandFromExtension("open", System.IO.Path.GetExtension(Path.SelectedItem.Path));

                if (command != null) menuItem.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Registry.GetOpenWithSoftwarePathFromCommand(command)).ToBitmap().ToImageSource();

            }

            menuItems.AddRange(new MenuItem[] {menuItem,
                new MenuItem(Themes. Generic.Copy, Properties.Resources.page_copy.ToImageSource(), new DelegateCommand{ ExecuteDelegate =    o => Copy(ActionsFromObjects.ListView) }, null, null),
                new MenuItem(Themes. Generic.Cut, Properties.Resources.cut.ToImageSource(), new DelegateCommand{ ExecuteDelegate =    o => Cut(ActionsFromObjects.ListView) }, null, null),
                new MenuItem(Themes. Generic.CreateShortcut, null, new DelegateCommand<ShellObjectInfo>{ ExecuteDelegate =    o => new ShellLink(o.Path, System.IO.Path.GetDirectoryName(o.Path) + "\\" + System.IO.Path.GetFileNameWithoutExtension(o.Path) + ".lnk") }, Path.SelectedItem, null),
                new MenuItem(Themes. Generic.Rename),
                new MenuItem(Themes. Generic.Delete),
                new MenuItem(Themes. Generic.Properties) });

            if (System.IO.Path.HasExtension(Path.SelectedItem.Path))

            {

                menuItem = new MenuItem("Open with");

                menuItems.Insert(1, menuItem);

                WinShellAppInfoInterop winShellAppInfoInterop = new WinShellAppInfoInterop(System.IO.Path.GetExtension(Path.SelectedItem.Path));

                winShellAppInfoInterop.OpenWithAppInfosLoaded += (object sender, EventArgs e) =>

                {

                    void callBack()

                    {

                        foreach (AppInfo value in winShellAppInfoInterop.OpenWithAppInfos)

                            menuItem.Items.Add(new MenuItem(value.DisplayName, null, null, null, null));

                    }

                    if (Dispatcher.CheckAccess())

                        callBack();

                    else

                        Dispatcher.InvokeAsync(callBack);


                };

                winShellAppInfoInterop.GetAppInfos();

            }

            return menuItems;

        }

        private void History_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == NotifyCollectionChangedAction.Remove)

                foreach (var oldItem in e.OldItems.OfType<HistoryItemData>())

                    ((Explorer.IBrowsableObjectInfo)oldItem.Path).Dispose();

        }

        private static System.Collections.ObjectModel.ObservableCollection<Explorer.IBrowsableObjectInfoViewModel> GetDefaultTreeViewItems()

        {

            object[][] defaultTreeViewFolders = { new object[] { KnownFolders.Desktop, SpecialFolders.Desktop }, new object[] { KnownFolders.Computer, SpecialFolders.Computer }, new object[] { KnownFolders.Libraries, SpecialFolders.Libraries }, new object[] { KnownFolders.RecycleBin, SpecialFolders.RecycleBin } };

            List<BrowsableObjectInfoViewModel> browsableObjectInfos = new List<BrowsableObjectInfoViewModel>(defaultTreeViewFolders.Length);

            ShellObject shellObject;

            foreach (object[] defaultTreeViewFolder in defaultTreeViewFolders)

            {

                shellObject = ShellObject.FromParsingName(((IKnownFolder)defaultTreeViewFolder[0]).ParsingName);

                browsableObjectInfos.Add(new BrowsableObjectInfoViewModel(new ShellObjectInfo(
                            shellObject, shellObject.IsFileSystemObject ? shellObject.ParsingName : shellObject.GetDisplayName(DisplayNameType.Default), FileType.SpecialFolder, (SpecialFolders)defaultTreeViewFolder[1])));

            };

            return new System.Collections.ObjectModel.ObservableCollection<Explorer.IBrowsableObjectInfoViewModel>(browsableObjectInfos);

        }

        private void ExplorerControl_PathChanged(object sender, RoutedEventArgs<ValueChangedEventArgs<Explorer.IBrowsableObjectInfo>> e) => OnPathChanged(e.OriginalEventArgs.OldValue, e.OriginalEventArgs.NewValue);

        private void ExplorerControl_TextChanged(object sender, TextChangedEventArgs e) => OnTextChanged(e);

        private ObservableListBoxSelectedItems _observableListBoxSelectedItems = null;

        //private void TryAddCommandBinding()

        //{

        //}

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();

            TreeView = (TreeView)Template.FindName("PART_TreeView", this);

            //if (TreeView != null)

            //    TreeView.ParentExplorerControl = this;

            ListView = (ListView)Template.FindName("PART_ListView", this);

            if (ListView != null)

            {

                ListView.ParentExplorerControl = this;

                // TryAddCommandBinding();

                // ListView.SelectionChanged += (object sender, System.Windows.Controls.SelectionChangedEventArgs e) => OnListViewSelectionChanged(e);

                (_observableListBoxSelectedItems = new ObservableListBoxSelectedItems(ListView)).CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>

                {

                    if (IsAutomaticItemContainerGeneratorStatusChange) return;

                    Explorer.IBrowsableObjectInfoInternal _path = (Explorer.IBrowsableObjectInfoInternal)Path;

                    if (e.Action == NotifyCollectionChangedAction.Add)

                        foreach (Explorer.IBrowsableObjectInfo item in e.NewItems.OfType<Explorer.IBrowsableObjectInfo>())

                            _path.SelectedItems.Add(item);

                    else if (e.Action == NotifyCollectionChangedAction.Remove)

                        foreach (Explorer.IBrowsableObjectInfo item in e.OldItems.OfType<Explorer.IBrowsableObjectInfo>())

                            _path.SelectedItems.Remove(item);

                    else if (e.Action == NotifyCollectionChangedAction.Reset)

                        _path.SelectedItems.Clear();

                };

                // ListView.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;

                ListView.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;

            }

            PART_TextBox = (TextBox)Template.FindName("PART_TextBox", this);

        }

        private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)

        {
            Debug.WriteLine("ici: " + e.Action.ToString() + " " + e.ItemCount);
            if (e.Action == NotifyCollectionChangedAction.Add)

                SetValue(VisibleItemsCountPropertyKey, VisibleItemsCount + e.ItemCount);

            else if (e.Action == NotifyCollectionChangedAction.Remove)

                SetValue(VisibleItemsCountPropertyKey, VisibleItemsCount - e.ItemCount);

            else if (e.Action == NotifyCollectionChangedAction.Reset)

                SetValue(VisibleItemsCountPropertyKey, ListView.ItemContainerGenerator.Items.Count);

        }

        //protected virtual void OnItemsCollectionChanging(Util.NotifyCollectionChangedEventArgs e)
        //{
        //    Debug.WriteLine("OnItemsCollectionChanging: 1");
        //    Debug.WriteLine("OnItemsCollectionChanging: " + e.Action.ToString());
        //    if (ListView != null && ListView.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)

        //    {
        //        Debug.WriteLine("OnItemsCollectionChanging: 2");
        //        ListViewItem item;

        //        if (e.OldItems != null)

        //            for (int i = 0; i < e.OldItems.Count; i++)

        //            {
        //                Debug.WriteLine("OnItemsCollectionChanging: 3");
        //                item = (ListViewItem)ListView.ItemContainerGenerator.ContainerFromIndex(e.OldStartingIndex + i);

        //                if (listViewItems.Contains(item))

        //                {

        //                    listViewItems.Remove(item);

        //                    SetValue(VisibleItemsCountPropertyKey, VisibleItemsCount - 1);

        //                }

        //            }

        //        if (e.NewItems != null)

        //            UpdateVisibleItemsCount();

        //    }

        //}

        //private void Items_CollectionChanging(object sender, Util.NotifyCollectionChangedEventArgs e) => OnItemsCollectionChanging(e);

        private readonly List<ListViewItem> listViewItems = new List<ListViewItem>();

        private void UpdateVisibleItemsCount()

        {

            if (ListView.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)

            {

                if (IsAutomaticItemContainerGeneratorStatusChange)

                {

                    IsAutomaticItemContainerGeneratorStatusChange = false;

                    return;

                }

                // SetValue(VisibleItemsCountPropertyKey, 0);

                // int count = 0;

                object item;

                ListViewItem value;

                for (int i = 0; i < listViewItems.Count; i++)

                    if ((item = ListView.ItemContainerGenerator.ItemFromContainer(listViewItems[i])) == null || !ListView.ItemContainerGenerator.Items.Contains(item))

                    {

                        listViewItems.RemoveAt(i);

                        // SetValue(VisibleItemsCountPropertyKey, VisibleItemsCount - 1);

                    }

                foreach (object _item in ListView.ItemContainerGenerator.Items)

                {

                    if ((value = (ListViewItem)ListView.ItemContainerGenerator.ContainerFromItem(_item)) == null)

                        return;

                    if (!listViewItems.Contains(value))

                    {

                        listViewItems.Add(value);

                        SetValue(VisibleItemsCountPropertyKey, VisibleItemsCount + 1);

                        // value.IsVisibleChanged += Value_IsVisibleChanged;

                    }

                }

                // SetValue(VisibleItemsCountPropertyKey, count);

            }

        }

        internal void Value_IsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)

        {

            if ((Visibility)e.NewValue == Visibility.Visible)

                SetValue(VisibleItemsCountPropertyKey, VisibleItemsCount + 1);

            else

            {

                ((ListViewItem)d).IsSelected = false;

                SetValue(VisibleItemsCountPropertyKey, VisibleItemsCount - 1);

            }

        }

        protected virtual void OnItemContainerGeneratorStatusChanged(EventArgs e) => UpdateVisibleItemsCount();

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e) => OnItemContainerGeneratorStatusChanged(e);

        private void FileOpeningBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)

        {

            pathsToOpen = null;

            fileOpeningBgWorker = null;

            Window.GetWindow(this).Cursor = Cursors.Arrow;

        }

        private void FileOpeningBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            foreach (ShellFile item in pathsToOpen)

                Process.Start(item.Path);

        }

        internal bool? OpenInternal(Explorer.IBrowsableObjectInfo path, bool firstItem)

        {

            if (firstItem && path is IArchiveItemInfo && path.FileType == FileType.Archive)

            {

                MessageBox.Show("The 'Open Archive in Archive' feature isn't currently supported by WinCopies. This feature will come with next updates.");

                return false;

            }

            if (path is IShellObjectInfo || path is IArchiveItemInfo)

            {

                if (path.FileType == FileType.Folder || path.FileType == FileType.Drive || path.FileType == FileType.SpecialFolder)

                    if (firstItem)

                        if (OpenDirectoriesDirectly)

                        {

                            Navigate(path);

                            return true;

                        }

                        else

                        {

                            RaiseEvent(new RoutedEventArgs<EventArgs<Explorer.IBrowsableObjectInfo>>(NavigationRequestedEvent, this, new EventArgs<Explorer.IBrowsableObjectInfo>(path)));

                            return null;

                        }

                    else return null;

                else if (path.FileType == FileType.Archive)

                {

                    foreach (KeyValuePair<SevenZip.InArchiveFormat, string[]> items in ArchiveLoader.InArchiveFormats)

                        if (items.Value.Contains(System.IO.Path.GetExtension(path.Path)))

                            if (ArchiveFormatsToOpen.HasFlag((Enum)Enum.Parse(typeof(InArchiveFormats), items.Key.ToString())))

                                if (firstItem)

                                    if (OpenDirectoriesDirectly)

                                    {

                                        Navigate(path);

                                        return true;

                                    }

                                    else

                                    {

                                        RaiseEvent(new RoutedEventArgs<EventArgs<Explorer.IBrowsableObjectInfo>>(NavigationRequestedEvent, this, new EventArgs<Explorer.IBrowsableObjectInfo>(path)));

                                        return null;

                                    }

                                else return null;

                            else if (!(path is IO.ShellObjectInfo) || !(((IO.ShellObjectInfo)path).ShellObject is ShellFile))

                                // todo:

                                throw new ArgumentException("path isn't a ShellObjectInfo or its ShellObject property value isn't a ShellFile.");

                            else

                            {

                                OpenFile((ShellFile)((IO.ShellObjectInfo)path).ShellObject);

                                return true;

                            }

                    if (!(path is IO.ShellObjectInfo) || !(((IO.ShellObjectInfo)path).ShellObject is ShellFile))

                        // todo:

                        throw new ArgumentException("path isn't a ShellObjectInfo or its ShellObject property value isn't a ShellFile.");

                    else

                    {

                        OpenFile((ShellFile)((IO.ShellObjectInfo)path).ShellObject);

                        return true;

                    }

                }

                else if (path.FileType == FileType.File)

                    if (!(path is IO.ShellObjectInfo) || !(((IO.ShellObjectInfo)path).ShellObject is ShellFile))

                        // todo:

                        throw new ArgumentException("path isn't a ShellObjectInfo or its ShellObject property value isn't a ShellFile.");

                    else

                    {

                        OpenFile((ShellFile)((IO.ShellObjectInfo)path).ShellObject);

                        return true;

                    }

            }

            return false;

        }

        internal bool? OpenLinkInternal(ShellLink path, bool firstItem)

        {

            ShellObjectInfo _path = new ShellObjectInfo(path.TargetShellObject, path.TargetLocation);

            if (_path.FileType == FileType.Folder || _path.FileType == FileType.Drive || _path.FileType == FileType.SpecialFolder || _path.FileType == FileType.Archive)

                if (firstItem)

                    if (OpenDirectoriesDirectly)

                    {

                        Navigate(_path);

                        return true;

                    }

                    else

                    {

                        RaiseEvent(new RoutedEventArgs<EventArgs<Explorer.IBrowsableObjectInfo>>(NavigationRequestedEvent, this, new EventArgs<Explorer.IBrowsableObjectInfo>(_path)));

                        return null;

                    }

                else return false;

            else if (_path.FileType == FileType.File)

                if (!(_path is IO.ShellObjectInfo) || !(_path.ShellObject is ShellFile))

                    // todo:

                    throw new ArgumentException("path isn't a ShellObjectInfo or its ShellObject property value isn't a ShellFile.");

                else

                    OpenFile((ShellFile)_path.ShellObject);

            return true;

        }

        /// <summary>
        /// Opens a path array.
        /// </summary>
        /// <param name="paths">The <see cref="Explorer.IBrowsableObjectInfo"/> paths array to open.</param>
        public void Open(params Explorer.IBrowsableObjectInfo[] paths) => OnOpening(paths);

        public void Open()

        {

            //bool areFoldersSelected = false;

            //bool areFilesSelected = false;

            //bool areOtherObjectsSelected = false;

            //foreach (var item in ListViewSelectedItems.ListBox.SelectedItems)

            //    if (item is Explorer.IBrowsableObjectInfo)

            //        if (((Explorer.IBrowsableObjectInfo)item).FileType == FileTypes.Folder || ((Explorer.IBrowsableObjectInfo)item).FileType == FileTypes.Drive || ((Explorer.IBrowsableObjectInfo)item).FileType == FileTypes.SpecialFolder || ((Explorer.IBrowsableObjectInfo)item).FileType == FileTypes.Archive)

            //        {

            //            areFoldersSelected = true;

            //            break;
            //        }

            //        else if (((Explorer.IBrowsableObjectInfo)item).FileType == FileTypes.File)

            //        {

            //            areFilesSelected = true;

            //            break;

            //        }

            //        else

            //        {

            //            areOtherObjectsSelected = true;

            //            break;

            //        }

            //if (areFoldersSelected || areFilesSelected || areOtherObjectsSelected)

            OnOpening((Explorer.IBrowsableObjectInfo[])_observableListBoxSelectedItems.ListBox.SelectedItems.ToArray());

        }

        public void Navigate(Explorer.IBrowsableObjectInfo path) => Navigate(path, null);

        public virtual void Navigate(Explorer.IBrowsableObjectInfo path, BrowsableObjectInfoItemsLoader browsableObjectInfoItemsLoader) => NavigateInternal(path, browsableObjectInfoItemsLoader, true);

        public void NavigateToHistoryIndex(int index) => NavigateToHistoryIndex(index, null);

        public virtual void NavigateToHistoryIndex(int index, BrowsableObjectInfoItemsLoader browsableObjectInfoItemsLoader) => NavigateInternal((Explorer.IBrowsableObjectInfo)((HistoryItemData)History[index]).Path, browsableObjectInfoItemsLoader, false);

        private void NavigateInternal(Explorer.IBrowsableObjectInfo path, BrowsableObjectInfoItemsLoader browsableObjectInfoItemsLoader, bool addPathToHistory)

        {

            // SetValue(PathPropertyKey, path);

            if (Path != null && Path.ItemsLoader.IsBusy)

                // if (Path.ItemsLoader.WorkerSupportsCancellation)

                Path.ItemsLoader.Cancel();

            // else

            // throw new InvalidOperationException("This worker doesn't supports cancellation.");

            SetValue(IsLoadingPropertyKey, true);

            // if (path is IShellObjectInfo && (((ShellObjectInfo)path).FileType == FileTypes.Folder || ((ShellObjectInfo)path).FileType == FileTypes.Drive))

            // if (BrowsableObjectInfoItemsLoader == null || BrowsableObjectInfoItemsLoader.GetType() != typeof(FolderLoader))

            // BrowsableObjectInfoItemsLoader = new FolderLoader((ShellObjectInfo)path);


            // FolderLoader.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>

            SetValue(PathPropertyKey, path);

            SetValue(HeaderPropertyKey, path.LocalizedName);

            SetValue(CanMoveToParentPathPropertyKey, path.Parent != null);

            if (path is IShellObjectInfo shellObjectInfo && shellObjectInfo.ShellObject.IsFileSystemObject)

                SetValue(CanPastePropertyKey, true);



            if (browsableObjectInfoItemsLoader == null)

                browsableObjectInfoItemsLoader = path is IArchiveItemInfo || path.FileType == FileType.Archive
                    ? new ArchiveLoader(true, true, FileTypesFlags.All)
                    : (BrowsableObjectInfoItemsLoader)new FolderLoader(true, true, FileTypesFlags.All);

            browsableObjectInfoItemsLoader.RunWorkerCompleted += BrowsableObjectInfoItemsLoader_RunWorkerCompleted;

            path.LoadItems(browsableObjectInfoItemsLoader);



            if (addPathToHistory)

            {

                if (HistorySelectedIndex > 0)

                    history.RemoveRange(0, HistorySelectedIndex);

                history.Insert(0, new HistoryItemData(Header, path.Clone(), ListView == null ? new ScrollViewerOffset(0, 0) : new ScrollViewerOffset(ListView.ScrollHost.HorizontalOffset, ListView.ScrollHost.VerticalOffset), ListView?.SelectedItems.OfType<Explorer.IBrowsableObjectInfo>()));

            }

            //else

            //{

            //    bool currentPathIsInHistory = false;

            //    foreach (IHistoryItemData historyItem in history)

            //        if (historyItem is HistoryItemData && path.Path == ((HistoryItemData)historyItem).Path.Path)

            //        {

            //            SetValue(HistorySelectedIndexPropertyKey, HistorySelectedIndex + 1);

            //            currentPathIsInHistory = true;

            //        }

            //    if (!currentPathIsInHistory)

            //    {

            //        history.RemoveRange(0, HistorySelectedIndex + 1);

            //        SetValue(HistorySelectedIndexPropertyKey, 0);

            //    }

            //}

            SetValue(CanMoveToPreviousPathPropertyKey, history.Count > 1 && HistorySelectedIndex < history.Count - 1);

            SetValue(CanMoveToNextPathPropertyKey, history.Count > 1 && HistorySelectedIndex > 0);

        }

        protected internal virtual void OnItemsLoadException(string path, Exception ex)

        {

            if (ex != null)

                MessageBox.Show((ex is IOException || ex is UnauthorizedAccessException ? "The load can't be performed because the path was not found or you don't have access rights to this path." : "The load can't be performed because of an unkown exception.") + " Path is: " + path);

        }

        protected virtual void OnBrowsableObjectInfoItemsLoaderRunWorkerCompleted(RunWorkerCompletedEventArgs e) => OnItemsLoadException(Path.Path, e.Error);

        private void BrowsableObjectInfoItemsLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)

        {

            ((BrowsableObjectInfoItemsLoader)sender).RunWorkerCompleted -= BrowsableObjectInfoItemsLoader_RunWorkerCompleted;

            OnBrowsableObjectInfoItemsLoaderRunWorkerCompleted(e);

        }

        /// <summary>
        /// Opens a <see cref="ShellFile"/> asynchronously.
        /// </summary>
        /// <param name="path">The <see cref="ShellFile"/> to open.</param>
        public void OpenFile(ShellFile path)

        {

            if (fileOpeningBgWorker == null)

            {

                fileOpeningBgWorker = new BackgroundWorker();

                fileOpeningBgWorker.DoWork += FileOpeningBgWorker_DoWork;

                fileOpeningBgWorker.RunWorkerCompleted += FileOpeningBgWorker_RunWorkerCompleted;

            }

            if (pathsToOpen == null)

                pathsToOpen = new List<ShellFile>();

            pathsToOpen.Add(path);

            if (!fileOpeningBgWorker.IsBusy)

                fileOpeningBgWorker.RunWorkerAsync();

            Window.GetWindow(this).Cursor = Cursors.Wait;

        }

        protected internal virtual void OnOpening(params Explorer.IBrowsableObjectInfo[] paths)
        {

            System.Collections.Generic.LinkedList<Explorer.IBrowsableObjectInfo> directories = new System.Collections.Generic.LinkedList<Explorer.IBrowsableObjectInfo>();

            System.Collections.Generic.LinkedList<Explorer.IBrowsableObjectInfo> files = new System.Collections.Generic.LinkedList<Explorer.IBrowsableObjectInfo>();

            foreach (Explorer.IBrowsableObjectInfo path in paths)

            {

#if DEBUG

                Debug.WriteLine("ExplorerControl Open");

                if (path is IO.ShellObjectInfo)

                    Debug.WriteLine((!(path is IO.ShellObjectInfo)).ToString() + " " + ((IO.ShellObjectInfo)path).ShellObject.GetType().ToString());

#endif

                if (!OpenFilesDirectly && If(ComparisonType.And, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, path.FileType, FileType.File, FileType.Link, FileType.Archive))

                {

                    files.Append(path);

                    continue;

                }

                bool? result = OpenInternal(path, directories.Count == 0);

                if (result.HasValue)

                {

                    if (!result.Value)

                        if (path.FileType == FileType.Link)

                            if (!(path is IO.ShellObjectInfo) || !((IO.ShellObjectInfo)path).ShellObject.IsLink)

                                // todo:

                                throw new ArgumentException("path isn't a ShellObjectInfo or path isn't a link.");

                            else

                            {

                                ShellLink shellLink = (ShellLink)ShellObject.FromParsingName(((IO.ShellObjectInfo)path).ShellObject.ParsingName);

                                if (shellLink.TargetShellObject.IsLink)

                                    // todo:

                                    throw new InvalidOperationException("Shell link target shell object is also a link.");

                                else

                                {

                                    OpenLinkInternal(shellLink, directories.Count == 0);

                                    directories.Append(path);

                                }

                            }

                }

                else

                    directories.Append(path);

            }

            if (directories.Count > 1)

                RaiseEvent(new RoutedEventArgs<EventArgs<Explorer.IBrowsableObjectInfo[]>>(MultiplePathsOpeningRequestedEvent, this, new EventArgs<Explorer.IBrowsableObjectInfo[]>(directories.ToArray())));

            if (files.Count > 0)

                RaiseEvent(new RoutedEventArgs<EventArgs<Explorer.IBrowsableObjectInfo[]>>(FilesOpeningRequestedEvent, this, new EventArgs<Explorer.IBrowsableObjectInfo[]>(files.ToArray())));

        }

        protected internal virtual void OnOpenCanExecute(CanExecuteRoutedEventArgs e) => e.CanExecute = TreeView?.SelectedItem != null || ListView?.SelectedItem != null;

        internal void OnOpeningInternal(params Explorer.IBrowsableObjectInfo[] paths) => OnOpening(paths);

        public StringCollection GetFileDropList(ActionsFromObjects copyFrom)

        {

            StringCollection sc = null;

            if (copyFrom == ActionsFromObjects.TreeView)

                sc = new StringCollection() { ((IO.ShellObjectInfo)TreeView.SelectedItem).Path };

            else if (copyFrom == ActionsFromObjects.ListView)

            {

                sc = new StringCollection();

                foreach (IO.ShellObjectInfo shellObjectInfo in _observableListBoxSelectedItems.ListBox.SelectedItems)

                    sc.Add(shellObjectInfo.Path);

                // Clipboard.SetFileDropList(sc);

            }

            return sc;

        }

        /// <summary>
        /// Set the selected items of the TreeView or the ListView in the system clipboard.
        /// </summary>
        /// <param name="copyFrom">Whether to look for selected items in the TreeView or the ListView.</param>
        public void Copy(ActionsFromObjects copyFrom)

        {

            StringCollection sc = GetFileDropList(copyFrom);

            if (sc != null)

            {

                Clipboard.Clear();

                //while (true)

                //    try
                //    {

                Clipboard.SetFileDropList(sc);

                //    break;

                //}

                //catch (Win32Exception ex)

                //{



                //}

            }

        }

        //public void Copy()
        //{

        //    if (PART_TreeView.IsFocused)

        //        Copy(ActionsFromObjects.FromTreeView);

        //    else if (PART_ListView.IsFocused)

        //        Copy(ActionsFromObjects.FromListView);

        //}

        public void Cut(ActionsFromObjects cutFrom)
        {

            StringCollection sc = GetFileDropList(cutFrom);

            MemoryStream dropEffect = new MemoryStream();
            dropEffect.WriteByte((byte)DragDropEffects.Move /* new byte[] { 2, 0, 0, 0 }; */);

            DataObject data = new DataObject();
            data.SetFileDropList(sc);
            data.SetData("Preferred DropEffect", dropEffect);

            // Clipboard.Clear();
            System.Windows.Clipboard.SetDataObject(data, true);

        }

        //public void Cut()

        //{

        //    if (PART_TreeView.IsFocused)

        //        Cut(ActionsFromObjects.FromTreeView);

        //    else if (PART_ListView.IsFocused)

        //        Cut(ActionsFromObjects.FromListView);

        //} 

        protected virtual void OnPaste(bool isAFileMoving, StringCollection sc, string destPath) => PasteAction?.Invoke(isAFileMoving, sc, destPath);

        public void Paste(ActionsFromObjects pasteTo)

        {

            // todo: handle clipboard exceptions

            pasteTo.ThrowIfNotValidEnumValue();

            WindowInteropHelper windowHandle = new WindowInteropHelper(Window.GetWindow(this));

            if (!Clipboard.ContainsFileDropList()) return;

            string path = null;

            if (pasteTo == ActionsFromObjects.TreeView)

                path = ((BrowsableObjectInfo)TreeView.SelectedItem).Path;

            else if (pasteTo == ActionsFromObjects.ListView)

                path = Path.Path;

            bool isAFileMoving = false;

            if (Clipboard.ContainsData("Preferred DropEffect"))

                using (MemoryStream dropEffect = (MemoryStream)Clipboard.GetData("Preferred DropEffect"))

                    isAFileMoving = (DragDropEffects)dropEffect.ReadByte() == DragDropEffects.Move;

            OnPaste(isAFileMoving, Clipboard.GetFileDropList(), path);

        }

        //public void Paste()

        //{

        //    if (PART_TreeView.IsFocused)

        //        Paste(ActionsFromObjects.FromTreeView);

        //    else if (PART_ListView.IsFocused)

        //        Paste(ActionsFromObjects.FromListView);

        //}

        private void CreateShortcut_Executed(object sender, ExecutedRoutedEventArgs e)

        {

            // todo

        }

        private void Properties_Executed(object sender, ExecutedRoutedEventArgs e)

        {

            // todo

        }

    }

    public static class Commands

    {

        public static RoutedUICommand Open { get; } = new RoutedUICommand(Themes.Generic.Open, nameof(Open), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.Enter) });

        public static RoutedUICommand EditProperty { get; } = new RoutedUICommand(Themes.Generic.EditProperty, nameof(EditProperty), typeof(Commands));

        // todo:

        public static DelegateCommand TrucMuche { get; } = new DelegateCommand
        {
            ExecuteDelegate = (object obj) =>
{
    Explorer.IBrowsableObjectInfo _obj = (Explorer.IBrowsableObjectInfo)obj;

    if (!_obj.AreItemsLoaded)

        _obj.LoadItems(true, false, FileTypesFlags.Folder | FileTypesFlags.Drive | FileTypesFlags.Archive);

    // MessageBox.Show(obj.ToString());
}
        };

        public static RoutedUICommand SingleClickSelection { get; } = new RoutedUICommand();

        //public static readonly RoutedUICommand OpenInNewTab = new RoutedUICommand((string)Application.Current.Resources["OpenInNewTab"], "OpenInNewTab", typeof(Commands));

        //public static readonly RoutedUICommand OpenInNewWindow = new RoutedUICommand((string)Application.Current.Resources["OpenInNewWindow"], "OpenInNewWindow", typeof(Commands));

        //public static readonly RoutedUICommand OpenInNewInstanceWindow = new RoutedUICommand((string)Application.Current.Resources["OpenInNewInstanceWindow"], "OpenInNewInstanceWindow", typeof(Commands));

        //public static readonly RoutedUICommand CreateShortcut = new RoutedUICommand((string)Application.Current.Resources["CreateShortcut"], "CreateShortcut", typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.L, ModifierKeys.Control) });

        //public static readonly RoutedUICommand PasteShortcut = new RoutedUICommand((string)Application.Current.Resources["PasteShortcut"], "PasteShortcut", typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.P, ModifierKeys.Control | ModifierKeys.Alt) });

        //public static readonly RoutedUICommand Rename = new RoutedUICommand((string)Application.Current.Resources["Rename"], "Rename", typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.F2) });

        //public static readonly RoutedUICommand Delete = new RoutedUICommand((string)Application.Current.Resources["Delete"], "Delete", typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.Delete) });

        //public static readonly RoutedUICommand Properties = new RoutedUICommand((string)Application.Current.Resources["Properties"], "Properties", typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.Enter, ModifierKeys.Alt) });

    }
}
