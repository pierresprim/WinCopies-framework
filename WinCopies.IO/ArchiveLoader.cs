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

using SevenZip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security;
using WinCopies.Util;
using static WinCopies.Util.Util;
using static WinCopies.IO.FolderLoader;
using WinCopies.Collections;

namespace WinCopies.IO
{

    // todo: does not work with encrypted archives

    public class ArchiveLoader<TPath, TItems, TSubItems, TFactory, TItemsFactory> : FileSystemObjectLoader<TPath, TItems, TSubItems, TFactory> where TPath : ArchiveItemInfoProvider where TItems : ArchiveItemInfo where TSubItems : ArchiveItemInfo where TFactory : BrowsableObjectInfoFactory, IArchiveItemInfoFactory where TItemsFactory : BrowsableObjectInfoFactory, IArchiveItemInfoFactory
    {

        protected override BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory> DeepCloneOverride() => new ArchiveLoader<TPath, TItems, TSubItems, TFactory, TItemsFactory>(default, FileTypes, (IFileSystemObjectComparer<IFileSystemObject>)FileSystemObjectComparer.DeepClone(), WorkerReportsProgress, WorkerSupportsCancellation);

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveLoader{TPath, TItems, TFactory}"/> class.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        /// <param name="fileTypes">The file types to load.</param>
        public ArchiveLoader( BrowsableObjectTreeNode< TPath, TItems, TFactory > path, FileTypes fileTypes, bool workerReportsProgress, bool workerSupportsCancellation) : this(path, fileTypes, new FileSystemObjectComparer<IFileSystemObject>(), workerReportsProgress, workerSupportsCancellation) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveLoader{TPath, TItems, TFactory}"/> class using a custom comparer.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        /// <param name="fileSystemObjectComparer">The comparer used to sort the loaded items.</param>
        /// <param name="fileTypes">The file types to load.</param>
        public ArchiveLoader( BrowsableObjectTreeNode< TPath, TItems, TFactory > path, FileTypes fileTypes, IFileSystemObjectComparer<IFileSystemObject> fileSystemObjectComparer, bool workerReportsProgress, bool workerSupportsCancellation) : base(path, fileTypes, (IFileSystemObjectComparer<IFileSystemObject>)fileSystemObjectComparer, workerReportsProgress, workerSupportsCancellation) { }

        protected override void OnPathChanging( BrowsableObjectTreeNode< TPath, TItems, TFactory > path )

        {

            // if ((path is ShellObjectInfo && ((ShellObjectInfo)path).FileType == FileTypes.Archive) || path is ArchiveItemInfo)

            // {

            // this.path = path;

            // PropertyChanged?.Invoke(this, new WinCopies.Util.PropertyChangedEventArgs(nameof(Path), null, path));

            // }

            /*else*/
            if (!(path is null) && path.Value.FileType != FileType.Archive)

                throw new ArgumentException("'Path' is not an Archive or a Folder.");

            // _Paths = new ObservableCollection<IBrowsableObjectInfo>();

        }

        // protected override void OnProgressChanged(object sender, ProgressChangedEventArgs e) => PathsOverride.Add((ArchiveItemInfo)e.UserState);

        protected override void OnDoWork(DoWorkEventArgs e)

        {

            if (FileTypes == FileTypes.None) return;

            //else if (FileTypes.HasFlag(GetAllEnumFlags<FileTypes>()) && FileTypes.HasMultipleFlags())

            //    throw new InvalidOperationException("FileTypes cannot have the All flag in combination with other flags.");

#if DEBUG

            Debug.WriteLine("Dowork event started.");

            Debug.WriteLine(FileTypes);

            try
            {

                Debug.WriteLine("Path == null: " + (Path == null).ToString());

                Debug.WriteLine("Path.Path: " + Path?. Value. Path);

                Debug.WriteLine("Path.ShellObject: " + (Path as IShellObjectInfo)?.ShellObject.ToString());

            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception) { }
#pragma warning restore CA1031 // Do not catch general exception types

#endif

#if DEBUG

            Debug.WriteLine("Dowork event started.");

#endif

            //List<FolderLoader.IPathInfo> directories = new List<FolderLoader.IPathInfo>();

            //List<FolderLoader.IPathInfo> files = new List<FolderLoader.IPathInfo>();

            var paths = new ArrayBuilder<PathInfo>();

#if DEBUG

            Debug.WriteLine("Path == null: " + (Path == null).ToString());

            Debug.WriteLine("Path.Path: " + Path.Value.Path);

            if (Path is IShellObjectInfo) Debug.WriteLine("Path.ShellObject: " + ((IShellObjectInfo)Path).ShellObject.ToString());

#endif

            // ShellObjectInfo archiveShellObject = Path is ShellObjectInfo ? (ShellObjectInfo)Path : ((ArchiveItemInfo)Path).ArchiveShellObject;

            string archiveFileName = (Path is IShellObjectInfo ? (IShellObjectInfo)Path : ((IArchiveItemInfo)Path).ArchiveShellObject).Path;

            try

            {

                using (var archiveFileStream = new FileStream(archiveFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {

                    //try
                    //{

                    // archiveShellObject.ArchiveFileStream = archiveFileStream;

                    using (var archiveExtractor = new SevenZipExtractor(archiveFileStream))
                    {

                        void AddPath(ref string _path, FileType fileType, ref ArchiveFileInfo? archiveFileInfo)

                        {

                            if (fileType == FileType.Other || (FileTypes != GetAllEnumFlags<FileTypes>() && !FileTypes.HasFlag(FileTypeToFileTypeFlags(fileType)))) return;

                            // We only make a normalized path if we add the path to the paths to load.

                            string __path = string.Copy(_path) ; 

                            paths.AddLast(new PathInfo(_path, _path.RemoveAccents(), fileType, archiveFileInfo, _archiveFileInfo =>     ArchiveItemInfo.DefaultArchiveFileInfoDeepClone(_archiveFileInfo, __path)));

                        }

                        void AddDirectory(string _path, ArchiveFileInfo? archiveFileInfo) =>

                            // if (FileTypes.HasFlag(FileTypesFlags.All) || (FileTypes.HasFlag(FileTypesFlags.Folder) && System.IO.Path.GetPathRoot(pathInfo.Path) != pathInfo.Path) || (FileTypes.HasFlag(FileTypesFlags.Drive) && System.IO.Path.GetPathRoot(pathInfo.Path) == pathInfo.Path))

                            AddPath(ref _path, FileType.Folder, ref archiveFileInfo);

                        void AddFile(string _path, ArchiveFileInfo? archiveFileInfo) =>

                            // We only make a normalized path if we add the path to the paths to load.

                            AddPath(ref _path, _path.Substring(_path.Length).EndsWith(".lnk")
                                ? FileType.Link
                                : IO.Path.IsSupportedArchiveFormat(System.IO.Path.GetExtension(_path)) ? FileType.Archive : FileType.File, ref archiveFileInfo);

                        System.Collections.ObjectModel.ReadOnlyCollection<ArchiveFileInfo> archiveFileData = archiveExtractor.ArchiveFileData;

                        string fileName = "";

                        string relativePath = Path is IShellObjectInfo ? "" : Path.Value.Path.Substring(archiveFileName.Length + 1);

                        // PathInfo path;

#if DEBUG

                        foreach (ArchiveFileInfo archiveFileInfo in archiveFileData)

                            Debug.WriteLine(archiveFileInfo.FileName);

#endif

                        void addPath(ArchiveFileInfo archiveFileInfo)

                        {

                            if (archiveFileInfo.FileName.StartsWith(relativePath) && archiveFileInfo.FileName.Length > relativePath.Length)

                            {

                                fileName = archiveFileInfo.FileName.Substring(relativePath.Length);

                                if (fileName.StartsWith(IO.Path.PathSeparator))

                                    fileName = fileName.Substring(1);

                                if (fileName.Contains(IO.Path.PathSeparator))

                                    fileName = fileName.Substring(0, fileName.IndexOf(IO.Path.PathSeparator));

                                /*if (!archiveFileInfo.FileName.Substring(archiveFileInfo.FileName.Length).Contains(IO.Path.PathSeparator))*/

                                // {

                                foreach (IFileSystemObject pathInfo in (IEnumerable<IFileSystemObject>) paths)

                                    if (pathInfo.Path == fileName)

                                        return;

                                if (fileName.ToLower() == archiveFileInfo.FileName.ToLower())

                                {

                                    if (archiveFileInfo.IsDirectory)

                                        AddDirectory(fileName, archiveFileInfo);

                                    else if (CheckFilter(archiveFileInfo.FileName))

                                        AddFile(fileName, archiveFileInfo);

                                }

                                else

                                    AddDirectory(fileName, archiveFileInfo);

                                // }

                            }

                        }

                        foreach (ArchiveFileInfo archiveFileInfo in archiveFileData)

                            // _path = archiveFileInfo.FileName.Replace('/', IO.Path.PathSeparator);

                            addPath(archiveFileInfo);

                        //if (Path is ArchiveItemInfo)

                        //{

                        //    if (relativePath != "")

                        //        relativePath = IO.Path.PathSeparator;

                        //    relativePath += ((ArchiveItemInfo)Path).Path/*.Replace('/', IO.Path.PathSeparator)*/;

                        //}

#if DEBUG

                        Debug.WriteLine(relativePath);

#endif

                    }

                    //}

                    //catch (Exception)

                    //{

                    //    paths = null;

                    //    return;

                    //}
                }

            }
            catch (Exception ex) when (ex.Is(false, typeof(IOException), typeof(SecurityException), typeof(UnauthorizedAccessException), typeof(SevenZipException))) { return; }

            // for (int i = 0; i < paths.Count; i++)

            // {

            // PathInfo directory = (PathInfo)paths[i];

            // string CurrentFile_Normalized = "";

            // CurrentFile_Normalized = Util.GetNormalizedPath(directory.Path);

            // directory.Normalized_Path = CurrentFile_Normalized;

            // paths[i] = directory;

            // }

            IEnumerable<PathInfo> pathInfos;

            if (FileSystemObjectComparer == null)

                pathInfos = (IEnumerable<PathInfo>)paths;

            else

            {

                var sortedPaths = paths.ToList();

                sortedPaths.Sort(FileSystemObjectComparer);

                pathInfos = (IEnumerable<PathInfo>)paths;

            }

            // for (int i = 0; i < files.Count; i++)

            // {

            // var file = (PathInfo)files[i];

            // string CurrentFile_Normalized = "";

            // CurrentFile_Normalized = FolderLoader.PathInfo.NormalizePath(file.Path);

            // file.Normalized_Path = CurrentFile_Normalized;

            // files[i] = file;

            // }

            // files.Sort(comp);



#if DEBUG

            void reportProgress(PathInfo path)

            {

                Debug.WriteLine("Current thread is background: " + System.Threading.Thread.CurrentThread.IsBackground);
                Debug.WriteLine("path_.Path: " + path.Path);
                Debug.WriteLine("path_.Normalized_Path: " + path.NormalizedPath);
                // Debug.WriteLine("path_.Shell_Object: " + path.ArchiveShellObject);

                // var new_Path = ((ArchiveItemInfo)Path).ArchiveShellObject;
                // new_Path.LoadThumbnail();

                ReportProgress(0, new BrowsableObjectTreeNode<TItems, TSubItems, TItemsFactory>(    (TItems)Path.Factory.GetBrowsableObjectInfo(Path.Value.Path + IO.Path.PathSeparator + path.Path, path.FileType, Path.Value.ArchiveShellObject, path.ArchiveFileInfo, archiveFileInfo => ArchiveItemInfo.DefaultArchiveFileInfoDeepClone(archiveFileInfo, Path.Value.ArchiveShellObject.Path)), (TItemsFactory) Path.Factory.DeepClone()));

                // #if DEBUG

                // Debug.WriteLine("Ceci est un " + new_Path.GetType().ToString());

                // #endif

            }

#endif

            // this._Paths = new ObservableCollection<IBrowsableObjectInfo>();



            PathInfo path_;



            using (IEnumerator<PathInfo> _paths = pathInfos.GetEnumerator())

                while (_paths.MoveNext())

                    try

                    {

                        do

                        {

                            path_ = _paths.Current;

#if DEBUG

                            reportProgress(path_);

#else

                            ReportProgress(0, ((IArchiveItemInfoProvider)Path).Factory.GetBrowsableObjectInfo(((IArchiveItemInfoProvider)Path).ArchiveShellObject, path.ArchiveFileInfo, Path.Path + IO.Path.PathSeparator + path.Path, path.FileType));

#endif

                        } while (_paths.MoveNext());

                    }
                    catch (Exception ex) when (HandleIOException(ex)) { }

            //foreach (FolderLoader.PathInfo path_ in files)

            //    reportProgressAndAddNewPathToObservableCollection(path_);

        }

        protected class PathInfo : IO.PathInfo

        {

            public FileType FileType { get; }

            public ArchiveFileInfo? ArchiveFileInfo { get; }

            public DeepClone<ArchiveFileInfo?> ArchiveFileInfoDelegate { get; }

            /// <summary>
            /// Gets the localized name of this <see cref="PathInfo"/>.
            /// </summary>
            public override string LocalizedName => Name;

            /// <summary>
            /// Gets the name of this <see cref="PathInfo"/>.
            /// </summary>
            public override string Name => System.IO.Path.GetFileName(Path);

            public PathInfo(string path, string normalizedPath, FileType fileType, ArchiveFileInfo? archiveFileInfo, DeepClone<ArchiveFileInfo?> archiveFileInfoDelegate) : base(path, normalizedPath)
            {

                ArchiveFileInfo = archiveFileInfo;

                ArchiveFileInfoDelegate = archiveFileInfoDelegate;

                FileType = fileType;

            }

            //public bool Equals(IFileSystemObject fileSystemObject) => ReferenceEquals(this, fileSystemObject)
            //        ? true : fileSystemObject is IBrowsableObjectInfo _obj ? FileType == _obj.FileType && Path.ToLower() == _obj.Path.ToLower()
            //        : false;

            //public int CompareTo(IFileSystemObject fileSystemObject) => GetDefaultComparer().Compare(this, fileSystemObject);

        }

    }

}
