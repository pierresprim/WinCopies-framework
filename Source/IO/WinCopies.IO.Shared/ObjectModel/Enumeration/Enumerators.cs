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

using SevenZip;

using System;
using System.Management;
using System.Security;

using WinCopies.IO.ObjectModel;

#if WinCopies2
using System.Collections;
using System.Collections.Generic;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Util;

using static WinCopies.Util.Util;
#else
using WinCopies.Collections.Generic;

using static WinCopies.ThrowHelper;
#endif

namespace WinCopies.IO
{
    public sealed class ArchiveItemInfoEnumerator :
#if WinCopies2
IEnumerator
#else
        Enumerator
#endif
        <ArchiveItemInfo>,
#if WinCopies2
        Util.
#endif
        DotNetFix.IDisposable
    {
        #region Private fields
        private int _index = -1;
        private IArchiveItemInfoProvider _archiveItemInfoProvider;
        private SevenZipExtractor _archiveExtractor;
        private
#if WinCopies2
WinCopies.Collections.Generic.Queue
#else
            EnumerableHelper
#endif
            <IFileSystemObject>
#if !WinCopies2
            .IEnumerableQueue
#endif
            _paths =
#if WinCopies2
new WinCopies.Collections.Generic.Queue
#else
            EnumerableHelper
#endif
            <IFileSystemObject>
#if !WinCopies2
            .GetEnumerableQueue
#endif
            ();
        private ArchiveItemInfo _current;
        private Predicate<ArchiveFileInfoEnumeratorStruct> _func;
        #endregion

#if WinCopies2
        public bool IsDisposed { get; private set; }

        public ArchiveItemInfo Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

        object IEnumerator.Current => Current;
#else
        protected override ArchiveItemInfo CurrentOverride => _current;

        public override bool? IsResetSupported => true;
#endif

        public Predicate<ArchiveFileInfoEnumeratorStruct> Func => IsDisposed ? throw GetExceptionForDispose(false) : _func;

        public ArchiveItemInfoEnumerator(IArchiveItemInfoProvider archiveItemInfoProvider, Predicate<ArchiveFileInfoEnumeratorStruct> func)
        {
            ThrowIfNull(archiveItemInfoProvider, nameof(archiveItemInfoProvider));
            ThrowIfNull(func, nameof(func));

            _archiveItemInfoProvider = archiveItemInfoProvider;

            _archiveExtractor = new SevenZipExtractor(archiveItemInfoProvider.ArchiveShellObject.ArchiveFileStream);

            _func = func;
        }

#if WinCopies2
        public bool MoveNext
#else
        protected override bool MoveNextOverride
#endif
            ()
        {
            #region Old

            // if (FileTypes == FileTypes.None) return;

            //else if (FileTypes.HasFlag(GetAllEnumFlags<FileTypes>()) && FileTypes.HasMultipleFlags())

            //    throw new InvalidOperationException("FileTypes cannot have the All flag in combination with other flags.");

            //        #if DEBUG

            //                                // Debug.WriteLine("Dowork event started.");

            //                                // Debug.WriteLine(FileTypes);

            //                                try
            //                                {

            //                                    Debug.WriteLine("Path == null: " + (Path == null).ToString());

            //                                    Debug.WriteLine("Path: " + Path);

            //                                    Debug.WriteLine("ShellObject: " + ArchiveShellObject.ToString());

            //                                }
            //        #pragma warning disable CA1031 // Do not catch general exception types
            //                                catch (Exception) { }
            //#pragma warning restore CA1031 // Do not catch general exception types

            //#endif

            //#if DEBUG

            //Debug.WriteLine("Dowork event started.");

            //#endif

            //List<FolderLoader.IPathInfo> directories = new List<FolderLoader.IPathInfo>();

            //List<FolderLoader.IPathInfo> files = new List<FolderLoader.IPathInfo>();

            // var paths = new ArrayBuilder<ArchiveItemInfo>();

            //#if DEBUG

            //Debug.WriteLine("Path == null: " + (Path == null).ToString());

            //                                Debug.WriteLine("Path: " + Path);

            //                                Debug.WriteLine("ShellObject: " + ArchiveShellObject.ToString());

            //        #endif

            // ShellObjectInfo archiveShellObject = Path is ShellObjectInfo ? (ShellObjectInfo)Path : ((ArchiveItemInfo)Path).ArchiveShellObject;

            #endregion

            try
            {
                //try
                //{

                // archiveShellObject.ArchiveFileStream = archiveFileStream;
                void AddArchiveFileInfo(in ArchiveFileInfo _archiveFileInfo) =>

                    //if (fileType == FileType.Other || (FileTypes != GetAllEnumFlags<FileTypes>() && !FileTypes.HasFlag(FileTypeToFileTypeFlags(fileType)))) return;

                    // // We only make a normalized path if we add the path to the paths to load.

                    // string __path = string.Copy(_path);

                    _current = ArchiveItemInfo.From(_archiveItemInfoProvider.ArchiveShellObject, _archiveFileInfo);

                void AddPath(in string archiveFilePath) =>

                    _current = ArchiveItemInfo.From(_archiveItemInfoProvider.ArchiveShellObject, archiveFilePath);

                //void AddDirectory(string _path, ArchiveFileInfo? archiveFileInfo) =>

                // if (FileTypes.HasFlag(FileTypesFlags.All) || (FileTypes.HasFlag(FileTypesFlags.Folder) && System.IO.Path.GetPathRoot(pathInfo.Path) != pathInfo.Path) || (FileTypes.HasFlag(FileTypesFlags.Drive) && System.IO.Path.GetPathRoot(pathInfo.Path) == pathInfo.Path))

                //AddPath(ref _path, FileType.Folder, ref archiveFileInfo);

                //void AddFile(string _path, ArchiveFileInfo? archiveFileInfo) =>

                // We only make a normalized path if we add the path to the paths to load.

                //AddPath(ref _path, _path.Substring(_path.Length).EndsWith(".lnk")
                //    ? FileType.Link
                //    : IO.Path.IsSupportedArchiveFormat(System.IO.Path.GetExtension(_path)) ? FileType.Archive : FileType.File, ref archiveFileInfo);



                string fileName;

                string relativePath = _archiveItemInfoProvider.Path.Substring(_archiveItemInfoProvider.ArchiveShellObject.Path.Length + 1);

                // PathInfo path;

                //#if DEBUG

                //                foreach (ArchiveFileInfo archiveFileInfo in archiveFileData)

                //                    Debug.WriteLine(archiveFileInfo.FileName);

                //#endif

                bool addPath(ArchiveFileInfoEnumeratorStruct archiveFileInfoEnumeratorStruct)
                {
                    if (Func is object && !Func(archiveFileInfoEnumeratorStruct))

                        return false;

                    foreach (IFileSystemObject pathInfo in _paths)

                        if (pathInfo.Path == fileName)

                            return false;

                    return true;
                }

                ArchiveFileInfo archiveFileInfo;

                for (int i = _index; i < _archiveExtractor.ArchiveFileData.Count; i++)
                {
                    archiveFileInfo = _archiveExtractor.ArchiveFileData[i];

                    // _path = archiveFileInfo.FileName.Replace('/', IO.Path.PathSeparator);

                    if (archiveFileInfo.FileName.StartsWith(relativePath, StringComparison.OrdinalIgnoreCase) && archiveFileInfo.FileName.Length > relativePath.Length)
                    {
                        fileName = archiveFileInfo.FileName.Substring(relativePath.Length);

                        if (fileName.StartsWith(WinCopies.IO.Path.PathSeparator))

                            fileName = fileName.Substring(1);

                        if (fileName.Contains(WinCopies.IO.Path.PathSeparator, StringComparison.OrdinalIgnoreCase))

                            fileName = fileName.Substring(0, fileName.IndexOf(WinCopies.IO.Path.PathSeparator
#if !NETFRAMEWORK
                                , StringComparison.OrdinalIgnoreCase
#endif
                                ));

                        /*if (!archiveFileInfo.FileName.Substring(archiveFileInfo.FileName.Length).Contains(IO.Path.PathSeparator))*/

                        // {

                        Action addValue;

                        ArchiveFileInfoEnumeratorStruct archiveFileInfoEnumeratorStruct;

                        if (fileName.ToUpperInvariant() == archiveFileInfo.FileName.ToUpperInvariant())
                        {
                            archiveFileInfoEnumeratorStruct = new ArchiveFileInfoEnumeratorStruct(archiveFileInfo);

                            addValue = () => AddArchiveFileInfo(archiveFileInfoEnumeratorStruct.ArchiveFileInfo.Value);
                        }

                        else
                        {
                            archiveFileInfoEnumeratorStruct = new ArchiveFileInfoEnumeratorStruct(fileName);

                            addValue = () => AddPath(archiveFileInfoEnumeratorStruct.Path);
                        }

                        if (addPath(archiveFileInfoEnumeratorStruct))
                        {
                            //if (archiveFileInfo.IsDirectory)

                            //    AddDirectory(fileName, archiveFileInfo);

                            //else /*if (CheckFilter(archiveFileInfo.FileName))*/

                            //    AddFile(fileName, archiveFileInfo);

                            addValue();

                            _index = i;

                            _paths.Enqueue(Current);

                            return true;
                        }
                        // }
                    }
                }

                //#if NETFRAMEWORK

                //                        }

                //#endif
            }

            catch (Exception ex) when (ex.Is(false, typeof(IOException), typeof(SecurityException), typeof(UnauthorizedAccessException), typeof(SevenZipException))) { }

            return false;
        }

#if WinCopies2
        public void Reset()
        {
#else
        protected override void ResetOverride()
        {
            base.ResetOverride();
#endif
            _index = -1;

            _current = null;

            _paths.Clear();
        }

        #region IDisposable Support

#if WinCopies2
        public void Dispose()
        {
#else
        protected override void DisposeManaged()
        {
            base.DisposeManaged();
#endif
            Reset();

            _archiveItemInfoProvider = null;

            _archiveExtractor.Dispose();

            _archiveExtractor = null;

            _paths = null;

            _func = null;
        }

        #endregion

        //private void Load()

        //{



        // for (int i = 0; i < paths.Count; i++)

        // {

        // PathInfo directory = (PathInfo)paths[i];

        // string CurrentFile_Normalized = "";

        // CurrentFile_Normalized = Util.GetNormalizedPath(directory.Path);

        // directory.Normalized_Path = CurrentFile_Normalized;

        // paths[i] = directory;

        // }

        // System.Collections.Generic.IEnumerable<PathInfo> pathInfos;

        //if (FileSystemObjectComparer == null)

        //    pathInfos = (System.Collections.Generic.IEnumerable<PathInfo>)paths;

        //else

        //{

        //    var sortedPaths = paths.ToList();

        //    sortedPaths.Sort(FileSystemObjectComparer);

        //    pathInfos = (System.Collections.Generic.IEnumerable<PathInfo>)paths;

        //}

        // pathInfos = paths;

        // for (int i = 0; i < files.Count; i++)

        // {

        // var file = (PathInfo)files[i];

        // string CurrentFile_Normalized = "";

        // CurrentFile_Normalized = FolderLoader.PathInfo.NormalizePath(file.Path);

        // file.Normalized_Path = CurrentFile_Normalized;

        // files[i] = file;

        // }

        // files.Sort(comp);
    }

    public sealed class WMIItemInfoEnumerator : Enumerator<ManagementBaseObject, WMIItemInfo>
    {
        private readonly bool _resetInnerEnumerator = false;

        private Func<bool> _func;

#if !WinCopies2
        private WMIItemInfo _current = null;

        protected override WMIItemInfo CurrentOverride => _current;

        public override bool? IsResetSupported => _resetInnerEnumerator;

        protected override void ResetCurrent() => _current = null;
#endif

        public WMIItemType ItemWMIItemType { get; }

        public WMIItemInfoEnumerator(System.Collections.Generic.IEnumerable<ManagementBaseObject> enumerable, bool resetEnumerator, WMIItemType itemWMIItemType, bool catchExceptions) : base(enumerable)
        {
            _resetInnerEnumerator = resetEnumerator;

            ItemWMIItemType = itemWMIItemType;

            if (catchExceptions)

                _func = () =>
                  {
                      while (InnerEnumerator.MoveNext())

                          try
                          {
                              _MoveNext();

                              return true;
                          }
                          catch (Exception) { }

                      return false;
                  };

            else

                _func = () =>
                  {
                      if (InnerEnumerator.MoveNext())
                      {
                          _MoveNext();

                          return true;
                      }

                      return false;
                  };
        }

        private void _MoveNext() =>

            // if (CheckFilter(_path))

#if WinCopies2
Current 
#else
            _current
#endif
            = new WMIItemInfo(ItemWMIItemType, InnerEnumerator.Current);

        protected override bool MoveNextOverride() => _func();

        protected override void ResetOverride()
        {
            base.ResetOverride();

            if (_resetInnerEnumerator)

                InnerEnumerator.Reset();
        }

        #region IDisposable Support

        protected override void
#if WinCopies2
            Dispose(bool disposing)
#else
            DisposeManaged()
#endif
        {
            Reset();

            _func = null;

#if WinCopies2
            base.Dispose(disposing);
#else
            base.DisposeManaged();
#endif
        }
        #endregion
    }

    //    public sealed class RecursiveSubEnumerator<T> : IEnumerator<T>
    //    {
    //        private IEnumerator<IEnumerator<T>> _enumerator;
    //        // private EmptyCheckEnumerator<string> _fileEnumerable;

    //        private bool _completed = false;

    //        private T _current;

    //        public T Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

    //        object IEnumerator.Current => Current;

    //#if DEBUG
    //        public IPathInfo PathInfo { get; }
    //#endif 

    //        public FileSystemEntryEnumerator(
    //#if DEBUG
    //            IPathInfo pathInfo,
    //#endif 
    //            System.Collections.Generic.IEnumerable<string> directoryEnumerable, System.Collections.Generic.IEnumerable<string> fileEnumerable)
    //        {
    //#if DEBUG
    //            ThrowIfNull(pathInfo, nameof(pathInfo));

    //            PathInfo = pathInfo;
    //#endif
    //            ThrowIfNull(directoryEnumerable, nameof(directoryEnumerable));
    //            ThrowIfNull(fileEnumerable, nameof(fileEnumerable));

    //            _directoryEnumerable = directoryEnumerable.GetEnumerator();

    //            _fileEnumerable = new EmptyCheckEnumerator<string>(fileEnumerable.GetEnumerator());
    //        }

    //        public bool MoveNext()
    //        {
    //            if (_completed) return false;

    //                if (_enumerator.MoveNext())
    //                {
    //                    _current = new PathInfo(_enumerator.Current, true);

    //                    return true;
    //                }

    //                _directoryEnumerable = null;

    //                _completed = true;

    //                return false;
    //        }

    //        public void Reset() => throw new NotSupportedException();

    //        public void Dispose()
    //        {
    //            if (_completed)

    //                _current = null;

    //            else
    //            {
    //                if (_directoryEnumerable != null)
    //                {
    //                    _directoryEnumerable.Dispose();

    //                    _directoryEnumerable = null;
    //                }

    //                if (_fileEnumerable != null)
    //                {
    //                    _fileEnumerable.Dispose();

    //                    _fileEnumerable = null;
    //                }
    //            }

    //            _current = null;

    //            IsDisposed = true;
    //        }

    //        public bool IsDisposed { get; private set; }
    //    }

#if DEBUG

    public

#else
        
        internal 
        
#endif

        enum PathType
    {
        All = 1,

        Directories = 2,

        Files = 3
    }

#if DEBUG

    public class FileSystemEntryEnumeratorProcessSimulation
    {
        private Func<string, PathType, System.Collections.Generic.IEnumerable<string>> _enumerateFunc;
        private Action<string> _writeLogAction;

        public Func<string, PathType, System.Collections.Generic.IEnumerable<string>> EnumerateFunc { get => _enumerateFunc ?? throw GetInvalidOperationException(); set => _enumerateFunc = value ?? throw GetInvalidOperationException(); }

        public Action<string> WriteLogAction { get => _writeLogAction ?? throw GetInvalidOperationException(); set => _writeLogAction = value ?? throw GetInvalidOperationException(); }

        private InvalidOperationException GetInvalidOperationException() => new InvalidOperationException("Value cannot be null.");
    }

#endif
}
