//* Copyright © Pierre Sprimont, 2020
// *
// * This file is part of the WinCopies Framework.
// *
// * The WinCopies Framework is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * The WinCopies Framework is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using Microsoft.WindowsAPICodePack.Shell;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Security;
//using System.Windows;
//using WinCopies.Util;

//using static WinCopies.Util.Util;
//using IFCT = WinCopies.Util.Util.ComparisonType;
//using IfCM = WinCopies.Util.Util.ComparisonMode;
//using IfComp = WinCopies.Util.Util.Comparison;
//using static WinCopies.IO.FolderLoader;
//using WinCopies.Collections;

using System;
using System.IO;
using System.Security;
using WinCopies.Util;

namespace WinCopies.IO
{

    //    public interface IFolderLoader : IFileSystemObjectLoader
    //    {

    //        FolderLoaderFileSystemWatcher FileSystemWatcher { get; }

    //    }

    internal static class FolderLoader
    {
        internal static bool HandleIOException(Exception ex) => ex.Is(false, typeof(IOException), typeof(UnauthorizedAccessException), typeof(SecurityException));
    }

    //    /// <summary>
    //    /// Provides a background process that can be used to load items of a folder. See the Remarks section.
    //    /// </summary>
    //    /// <remarks>
    //    /// This loader is not designed for <see cref="ShellObjectInfo"/> that have their <see cref="FileSystemObject.FileType"/> property set up with an other value than <see cref="FileType.Folder"/>, <see cref="FileType.Drive"/> or <see cref="FileType.SpecialFolder"/>, even if they can be browsable (e.g. <see cref="FileType.Archive"/>). If the file type of the given <see cref="BrowsableObjectInfoLoader.Path"/> is not supported by this loader, you'll have to use a specific loader or to inherit from this loader.
    //    /// </remarks>
    //    public class FolderLoader<TPath, TItems, TSubItems, TFactory, TItemsFactory> : FileSystemObjectLoader<TPath, TItems, TSubItems, TFactory>, IFolderLoader where TPath : ShellObjectInfo where TItems : FileSystemObjectInfo where TSubItems : FileSystemObjectInfo where TFactory : BrowsableObjectInfoFactory, IShellObjectInfoFactory where TItemsFactory : BrowsableObjectInfoFactory
    //    {

    //        public override bool NeedsObjectsOrValuesReconstruction => true;

    //        protected override BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory> DeepCloneOverride() => new FolderLoader<TPath, TItems, TSubItems, TFactory, TItemsFactory>(null, FileTypes, (IFileSystemObjectComparer<IFileSystemObject>)FileSystemObjectComparer.DeepClone(), WorkerReportsProgress, WorkerSupportsCancellation);

    //        // todo: to turn on ShellObjectWatcher for better compatibility

    //#pragma warning disable CS0649 // Set up using reflection
    //        private FolderLoaderFileSystemWatcher _fileSystemWatcher;
    //#pragma warning restore CS0649

    //        public FolderLoaderFileSystemWatcher FileSystemWatcher
    //        {
    //            get => _fileSystemWatcher ?? (FileSystemWatcher = GetFolderLoaderFileSystemWatcher()); private set
    //            {

    //                if (IsBusy)

    //                    throw new InvalidOperationException("The items loader is busy.");

    //                value.Created += FileSystemWatcher_Created;

    //                value.Renamed += FileSystemWatcher_Renamed;

    //                value.Deleted += FileSystemWatcher_Deleted;

    //                _fileSystemWatcher = value;

    //            }
    //        }

    //        /// <summary>
    //        /// Initializes a new instance of the <see cref="FolderLoader"/> class.
    //        /// </summary>
    //        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
    //        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
    //        /// <param name="fileTypes">The file types to load.</param>
    //        public FolderLoader( BrowsableObjectTreeNode< TPath, TItems, TFactory > path, FileTypes fileTypes, bool workerReportsProgress, bool workerSupportsCancellation) : this(path, fileTypes, new FileSystemObjectComparer<IFileSystemObject>(), workerReportsProgress, workerSupportsCancellation) { }

    //        /// <summary>
    //        /// Initializes a new instance of the <see cref="FolderLoader"/> class using a custom comparer.
    //        /// </summary>
    //        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
    //        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
    //        /// <param name="fileSystemObjectComparer">The comparer used to sort the loaded items.</param>
    //        /// <param name="fileTypes">The file types to load.</param>
    //        public FolderLoader( BrowsableObjectTreeNode< TPath, TItems, TFactory > path, FileTypes fileTypes, IFileSystemObjectComparer<IFileSystemObject> fileSystemObjectComparer, bool workerReportsProgress, bool workerSupportsCancellation) : base(path, fileTypes, (IFileSystemObjectComparer<IFileSystemObject>)fileSystemObjectComparer, workerReportsProgress, workerSupportsCancellation) { }

    //        protected override void OnPathChanging( BrowsableObjectTreeNode< TPath, TItems, TFactory > path)
    //        {

    //            if (path is null)

    //                return;

    //            FileSystemWatcher.EnableRaisingEvents = false;

    //            FileSystemWatcher.Path = null;

    //            if (Path is null) return;

    //            if (If(IFCT.Or, IfCM.Logical, IfComp.NotEqual, path.Value.FileType, FileType.Folder, FileType.Drive, FileType.SpecialFolder))

    //                throw new ArgumentException("'Path' isn't a folder, a drive or a special folder. 'Path': " + path.ToString());

    //            if ((Path.Value.FileType == FileType.Drive && new DriveInfo(Path.Value.Path).IsReady) || Path.Value.ShellObject.IsFileSystemObject)

    //            {

    //                FileSystemWatcher.Path = Path.Value.Path;

    //                FileSystemWatcher.EnableRaisingEvents = true;

    //            }

    //        }

    //        /// <summary>
    //        /// <para>Gets the <see cref="FolderLoaderFileSystemWatcher"/> used to listen to the file system events for the current <see cref="BrowsableObjectInfoLoader.Path"/> property.</para>
    //        /// <para>When overridden in a derived class, provides a custom <see cref="FolderLoaderFileSystemWatcher"/>.</para>
    //        /// </summary>
    //        /// <returns>An instance of the <see cref="FolderLoaderFileSystemWatcher"/> class.</returns>
    //        protected virtual FolderLoaderFileSystemWatcher GetFolderLoaderFileSystemWatcher() => new FolderLoaderFileSystemWatcher();

    //        /// <summary>
    //        /// Frees all resources used by this <see cref="FolderLoader"/>.
    //        /// </summary>
    //        protected override void Dispose(bool disposing)

    //        {

    //            base.Dispose(disposing);

    //            if (FileSystemWatcher != null)

    //                FileSystemWatcher.Dispose();

    //        }

    //        protected virtual void OnNewShellObjectCreated(string path)

    //        {

    //            // todo:

    //            if (!Application.Current.Dispatcher.CheckAccess())

    //                Application.Current.Dispatcher.InvokeAsync(() => OnNewShellObjectCreated(path));

    //            else
    //            {

    //                try
    //                {

    //                    // todo: may not work with ShellObjectWatcher

    //                    Path.Insert( Path.Count, new BrowsableObjectTreeNode<TItems, TSubItems, TItemsFactory>( (TItems)Path.Factory.GetBrowsableObjectInfo(path, FileType.File, SpecialFolder.OtherFolderOrFile, ShellObject.FromParsingName(path), ShellObjectInfo.DefaultShellObjectDeepClone), (TItemsFactory) Path.Factory.DeepClone()));

    //                }
    //#if DEBUG
    //#pragma warning disable CA1031 // Do not catch general exception types
    //                catch (Exception ex) { Debug.WriteLine(ex.Message); }
    //#pragma warning restore CA1031 // Do not catch general exception types
    //#else
    //            catch { }
    //#endif

    //                // todo:

    //                //if (FileSystemObjectComparer != null)

    //                //    Path.Items.Sort( 0, Path.Items.Count,     FileSystemObjectComparer);

    //            }

    //        }

    //        protected virtual void OnShellObjectRenamed(string oldPath, string newPath)

    //        {

    //            if (!Application.Current.Dispatcher.CheckAccess())

    //                _ = Application.Current.Dispatcher.InvokeAsync(() => OnShellObjectRenamed(oldPath, newPath));

    //            else
    //            {

    //                OnShellObjectDeleted(oldPath);

    //                OnNewShellObjectCreated(newPath);

    //            }

    //        }

    //        protected virtual void OnShellObjectDeleted(string path)

    //        {

    //            if (!Application.Current.Dispatcher.CheckAccess())

    //                _ = Application.Current.Dispatcher.InvokeAsync(() => OnShellObjectDeleted(path));

    //            else

    //                for (int i = 0; i < Path.Count; i++)

    //                    if (Path[i].Value.Path == path)

    //                    {

    //                        Path.RemoveAt(i);

    //                        return;

    //                    }

    //        }

    //        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e) => OnNewShellObjectCreated(e.FullPath);

    //        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e) => OnShellObjectRenamed(e.OldFullPath, e.FullPath);

    //        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e) => OnShellObjectDeleted(e.FullPath);

}
