using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinCopies.GUI.Explorer.Data;
using BooleanToVisibilityConverter = WinCopies.Util.Data.BooleanToVisibilityConverter;
using WinCopies.Util;
using System;
using System.IO;
using Microsoft.WindowsAPICodePack.Shell;
using System.Collections;
using System.Windows.Media;

namespace WinCopies.GUI.Explorer.Themes
{
    public partial class Generic
    {

        public static ResourceDictionary ResourceDictionary { get; } = Util.Generic.AddNewDictionary("/WinCopies.GUI.Explorer;component/Themes/Generic.xaml");

        public static T GetResource<T>(object key) => (T)ResourceDictionary[key];

        #region Resources

        /// <summary>
        /// Gets the <see cref="Open"/> resource.
        /// </summary>
        public static string Open => GetResource<string>(nameof(Open));

        /// <summary>
        /// Gets the <see cref="OpenInNewTab"/> resource.
        /// </summary>
        public static string OpenInNewTab => GetResource<string>(nameof(OpenInNewTab));

        /// <summary>
        /// Gets the <see cref="OpenInNewWindow"/> resource.
        /// </summary>
        public static string OpenInNewWindow => GetResource<string>(nameof(OpenInNewWindow));

        /// <summary>
        /// Gets the <see cref="OpenInNewInstanceWindow"/> resource.
        /// </summary>
        public static string OpenInNewInstanceWindow => GetResource<string>(nameof(OpenInNewInstanceWindow));

        /// <summary>
        /// Gets the <see cref="Copy"/> resource.
        /// </summary>
        public static string Copy => GetResource<string>(nameof(Copy));

        /// <summary>
        /// Gets the <see cref="Cut"/> resource.
        /// </summary>
        public static string Cut => GetResource<string>(nameof(Cut));

        /// <summary>
        /// Gets the <see cref="Paste"/> resource.
        /// </summary>
        public static string Paste => GetResource<string>(nameof(Paste));

        /// <summary>
        /// Gets the <see cref="CreateShortcut"/> resource.
        /// </summary>
        public static string CreateShortcut => GetResource<string>(nameof(CreateShortcut));

        /// <summary>
        /// Gets the <see cref="Rename"/> resource.
        /// </summary>
        public static string Rename => GetResource<string>(nameof(Rename));

        /// <summary>
        /// Gets the <see cref="Delete"/> resource.
        /// </summary>
        public static string Delete => GetResource<string>(nameof(Delete));

        /// <summary>
        /// Gets the <see cref="Properties"/> resource.
        /// </summary>
        public static string Properties => GetResource<string>(nameof(Properties));

        /// <summary>
        /// Gets the CommonProperties resource.
        /// </summary>
        public static string CommonProperties => GetResource<string>(nameof(CommonProperties));

        /// <summary>
        /// Gets the CreationTime resource.
        /// </summary>
        public static string CreationTime => GetResource<string>(nameof(CreationTime));

        /// <summary>
        /// Gets the LastAccessTime resource.
        /// </summary>
        public static string LastAccessTime => GetResource<string>(nameof(LastAccessTime));

        /// <summary>
        /// Gets the LastWriteTime resource.
        /// </summary>
        public static string LastWriteTime => GetResource<string>(nameof(LastWriteTime));

        /// <summary>
        /// Gets the Size resource.
        /// </summary>
        public static string Size => GetResource<string>(nameof(Size));

        /// <summary>
        /// Gets the SpecificProperties resource.
        /// </summary>
        public static string SpecificProperties => GetResource<string>(nameof(SpecificProperties));

        /// <summary>
        /// Gets the EditProperty resource.
        /// </summary>
        public static string EditProperty => GetResource<string>(nameof(EditProperty));

        /// <summary>
        /// Gets the Common resource.
        /// </summary>
        public static string Common => GetResource<string>(nameof(Common));

        /// <summary>
        /// Gets the FileType resource.
        /// </summary>
        public static string FileType => GetResource<string>(nameof(FileType));

        /// <summary>
        /// Gets the OpenWith resource.
        /// </summary>
        public static string OpenWith => GetResource<string>(nameof(OpenWith));

        /// <summary>
        /// Gets the Define resource.
        /// </summary>
        public static string Define => GetResource<string>(nameof(Define));

        /// <summary>
        /// Gets the Path resource.
        /// </summary>
        public static string Path => GetResource<string>(nameof(Path));

        /// <summary>
        /// Gets the SizeOnDisk resource.
        /// </summary>
        public static string SizeOnDisk => GetResource<string>(nameof(SizeOnDisk));

        /// <summary>
        /// Gets the Details resource.
        /// </summary>
        public static string Details => GetResource<string>(nameof(Details));

        /// <summary>
        /// Gets the Property resource.
        /// </summary>
        public static string Property => GetResource<string>(nameof(Property));

        /// <summary>
        /// Gets the Value resource.
        /// </summary>
        public static string Value => GetResource<string>(nameof(Value));

        /// <summary>
        /// Gets the DeletePersonalProperties resource.
        /// </summary>
        public static string DeletePersonalProperties => GetResource<string>(nameof(DeletePersonalProperties));

        /// <summary>
        /// Gets the OpenFolder resource.
        /// </summary>
        public static string OpenFolder => GetResource<string>(nameof(OpenFolder));

        /// <summary>
        /// Gets the OpenFile resource.
        /// </summary>
        public static string OpenFile => GetResource<string>(nameof(OpenFile));

        /// <summary>
        /// Gets the OpenFiles resource.
        /// </summary>
        public static string OpenFiles => GetResource<string>(nameof(OpenFiles));

        /// <summary>
        /// Gets the Save resource.
        /// </summary>
        public static string Save => GetResource<string>(nameof(Save));

        public static BooleanToVisibilityConverter BooleanToVisibilityConverter => GetResource<BooleanToVisibilityConverter>(nameof(BooleanToVisibilityConverter));

        public static FileSystemInfoAttributesToVisibilityConverter FileSystemInfoAttributesToVisibilityConverter => GetResource<FileSystemInfoAttributesToVisibilityConverter>(nameof(FileSystemInfoAttributesToVisibilityConverter));

        public static FileSystemInfoAttributesToOpacityConverter FileSystemInfoAttributesToOpacityConverter => GetResource<FileSystemInfoAttributesToOpacityConverter>(nameof(FileSystemInfoAttributesToOpacityConverter));

        #endregion

        private void PreviousButton_Click(object sender, RoutedEventArgs e) => ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).NavigateToHistoryIndex(((ExplorerControl)((FrameworkElement)sender).TemplatedParent).HistorySelectedIndex + 1);

        private void ForwardButton_Click(object sender, RoutedEventArgs e) => ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).NavigateToHistoryIndex(((ExplorerControl)((FrameworkElement)sender).TemplatedParent).HistorySelectedIndex - 1);

        private void ListView_MouseDoubleClick(object sender, RoutedEventArgs e)

        {

            if (((ExplorerControl)((FrameworkElement)sender).TemplatedParent).Path.SelectedItem is IO.IBrowsableObjectInfo)

                ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).OnOpeningInternal();

        }

        private void Navigate(object sender)

        {

            ExplorerControl explorerControl = (ExplorerControl)((FrameworkElement)sender).TemplatedParent;

            try

            {

                IO.BrowsableObjectInfo path = IO.Path.GetBrowsableObjectInfoFromPath(explorerControl.Text);

                Explorer.IBrowsableObjectInfo _path = path is IO.ShellObjectInfo shellObjectInfo
                    ? new ShellObjectInfo(shellObjectInfo)
                    : (Explorer.IBrowsableObjectInfo)new ArchiveItemInfo((IO.ArchiveItemInfo)path);

                explorerControl.Open(_path /*new ShellObjectInfo[] { new ShellObjectInfo(Microsoft.WindowsAPICodePack.Shell.ShellObject.FromParsingName(
                    explorerControl.Text)
                    , explorerControl.Text)

                }*/);

            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)

            {

                explorerControl.OnItemsLoadException(explorerControl.Text, ex);

            }

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)

            {

                Navigate(sender);

                e.Handled = true;

            }
            // SelectedItem.ExplorerControl.Open(WinCopies.IO.Util.GetNormalizedOSPath(SelectedItem.ExplorerControl.CurrentPath));)
            // new WinCopies.IO.ShellObjectInfo(Microsoft.WindowsAPICodePack.Shell.ShellObject.FromParsingName(WinCopies.IO.Util.GetNormalizedOSPath(ExplorerControl.CurrentPath)), WinCopies.IO.Util.GetNormalizedOSPath(ExplorerControl.CurrentPath), WinCopies.IO.SpecialFolders)
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Navigate(sender);

        // todo:

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            // if (!IsLoaded || DataContext == null) return;

            // ((ExplorerControl)sender).RaisePropertyChangedEvent("SelectedItems");

#if DEBUG
            Debug.WriteLine(string.Format("Added items: {0}, Removed items: {1}", e.AddedItems.Count, e.RemovedItems.Count));

            if (((ExplorerControl)((FrameworkElement)sender).TemplatedParent).Path.SelectedItems != null)

            {

                Debug.WriteLine(((ExplorerControl)((FrameworkElement)sender).TemplatedParent).Path.SelectedItems.Count.ToString());

                foreach (object @object in ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).Path.SelectedItems)

                    Debug.WriteLine("@object.GetType().ToString(): " + @object.GetType().ToString());

            }

#endif

            ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).RaiseEvent(new SelectionChangedEventArgs(ExplorerControl.SelectionChangedEvent, ActionsFromObjects.ListView, e.AddedItems, e.RemovedItems));

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).Text = ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).Path.Path;

        private void PART_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes == null)

                ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).RaiseEvent(new TextChangedEventArgs(ExplorerControl.TextChangedEvent, e.UndoAction));

            else

                ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).RaiseEvent(new TextChangedEventArgs(ExplorerControl.TextChangedEvent, e.UndoAction, e.Changes));
        }

        // private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ((ExplorerControl)((FrameworkElement)sender).TemplatedParent).OnListViewMouseDoubleClickInternal(e);

        private void ListViewItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

            ListView listView = ((FrameworkElement)sender).GetParent<ListView>(false);

            listView.GetParent<ExplorerControl>(false).OnItemsControlContextMenuOpening((FrameworkElement)sender, listView, e);

        }

        private void PART_ListView_MouseEnter(object sender, MouseEventArgs e)
        {

            ListView listView = (ListView)sender;

            if (listView.GetParent<ExplorerControl>(false)?.OpenMode == OpenMode.OnFirstClick)

                listView.Focus();

        }

        // private void PART_TreeView_KeyDown(object sender, KeyEventArgs e) => ((TreeView)sender).TryRaiseCommandsByKeyDown(e);

        // private void PART_ListView_KeyDown(object sender, KeyEventArgs e) => ((ListView)sender).TryRaiseCommandsByKeyDown(e);

        // private void Button_Click_1(object sender, RoutedEventArgs e) => ((DependencyObject)sender).GetParent<TreeViewItem>(false).IsSelected = true;

        //private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        //{

        //   Explorer.IBrowsableObjectInfo browsableObjectInfo = (Explorer.IBrowsableObjectInfo)((TreeViewItem)sender).DataContext;

        //   ExplorerControl explorerControl = ((DependencyObject)sender).GetParent<ExplorerControl>(false);

        //   // // todo: to use data binding

        //   // // explorerControl.TreeViewSelectedItem = browsableObjectInfo;

        //   browsableObjectInfo = (Explorer.IBrowsableObjectInfo)(browsableObjectInfo is IShellObjectInfo shellObjectInfo ? shellObjectInfo.GetBrowsableObjectInfo(shellObjectInfo.ShellObject, shellObjectInfo.Path) : browsableObjectInfo is ArchiveItemInfo archiveItemInfo ? archiveItemInfo.GetBrowsableObjectInfo(archiveItemInfo.ArchiveShellObject, archiveItemInfo.ArchiveFileInfo, archiveItemInfo.Path, archiveItemInfo.FileType) : null);

        //   explorerControl.Navigate(browsableObjectInfo, true);

        //   e.Handled = true;

        //}

        //private void TreeViewItem_Selected(object sender, MouseButtonEventArgs e)
        //{

        //    Explorer.IBrowsableObjectInfo browsableObjectInfo = (Explorer.IBrowsableObjectInfo)((TreeViewItem)sender).DataContext;

        //    ExplorerControl explorerControl = ((DependencyObject)sender).GetParent<ExplorerControl>(false);

        //    // // todo: to use data binding

        //    // // explorerControl.TreeViewSelectedItem = browsableObjectInfo;

        //    browsableObjectInfo = (Explorer.IBrowsableObjectInfo)(browsableObjectInfo is IShellObjectInfo shellObjectInfo ? shellObjectInfo.GetBrowsableObjectInfo(shellObjectInfo.ShellObject, shellObjectInfo.Path) : browsableObjectInfo is ArchiveItemInfo archiveItemInfo ? archiveItemInfo.GetBrowsableObjectInfo(archiveItemInfo.ArchiveShellObject, archiveItemInfo.ArchiveFileInfo, archiveItemInfo.Path, archiveItemInfo.FileType) : null);

        //    explorerControl.Navigate(browsableObjectInfo, true);

        //    e.Handled = true;

        //}
    }

}
