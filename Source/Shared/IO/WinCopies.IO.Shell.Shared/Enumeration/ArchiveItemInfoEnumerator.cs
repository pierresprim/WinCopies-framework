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
using System.Security;

using WinCopies.Collections;
using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ObjectModel;
using WinCopies.Util;

using static System.IO.Path;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Enumeration
{
    public sealed class ArchiveItemInfoEnumerator :
#if !WinCopies3
IEnumerator
#else
        Enumerator
#endif
        <ArchiveItemInfoItemProvider>,
#if !WinCopies3
        Util.
#endif
        DotNetFix.IDisposable
    {
        #region Private fields
        private const int DefaultIndex = 0;
        private int _index = DefaultIndex;
        private IArchiveItemInfoProvider _archiveItemInfoProvider;
        private SevenZipExtractor _archiveExtractor;
        private
#if !WinCopies3
WinCopies.Collections.Generic.Queue
#else
            EnumerableHelper
#endif
            <string>
#if WinCopies3
            .IEnumerableQueue
#endif
            _paths =
#if !WinCopies3
new WinCopies.Collections.Generic.Queue
#else
            EnumerableHelper
#endif
            <string>
#if WinCopies3
            .GetEnumerableQueue
#endif
            ();
        private ArchiveItemInfoItemProvider _current;
        private Predicate<ArchiveFileInfoEnumeratorStruct> _func;
        #endregion

#if !WinCopies3
        public bool IsDisposed { get; private set; }

        public ArchiveItemInfo Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

        object IEnumerator.Current => Current;
#else
        protected override ArchiveItemInfoItemProvider CurrentOverride => _current;

        public override bool? IsResetSupported => true;
#endif

        public Predicate<ArchiveFileInfoEnumeratorStruct> Func => IsDisposed ? throw GetExceptionForDispose(false) : _func;

        public ArchiveItemInfoEnumerator(in IArchiveItemInfoProvider archiveItemInfoProvider, in Predicate<ArchiveFileInfoEnumeratorStruct> func)
        {
            ThrowIfNull(archiveItemInfoProvider, nameof(archiveItemInfoProvider));

            _archiveItemInfoProvider = archiveItemInfoProvider;

            if (archiveItemInfoProvider.ArchiveShellObject != null && !archiveItemInfoProvider.ArchiveShellObject.IsArchiveOpen)

                archiveItemInfoProvider.ArchiveShellObject.OpenArchive(System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, 4096, System.IO.FileOptions.None);

            _archiveExtractor = new SevenZipExtractor(archiveItemInfoProvider.ArchiveShellObject.GetArchiveFileStream());

            if (func == null)

                _func = item => true;

            else

                _func = func;
        }

#if !WinCopies3
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

            if (!_archiveItemInfoProvider.IsBrowsable())

                return false;

            try
            {
                //try
                //{

                // archiveShellObject.ArchiveFileStream = archiveFileStream;
                void AddArchiveFileInfo(in ArchiveFileInfo _archiveFileInfo) =>

                    //if (fileType == FileType.Other || (FileTypes != GetAllEnumFlags<FileTypes>() && !FileTypes.HasFlag(FileTypeToFileTypeFlags(fileType)))) return;

                    // // We only make a normalized path if we add the path to the paths to load.

                    // string __path = string.Copy(_path);

                    _current = new ArchiveItemInfoItemProvider(_archiveItemInfoProvider.ArchiveShellObject, _archiveFileInfo); // ArchiveItemInfo.From(_archiveItemInfoProvider.ArchiveShellObject, _archiveFileInfo);

                void AddPath(in string archiveFilePath) =>

                    _current = new ArchiveItemInfoItemProvider(_archiveItemInfoProvider.ArchiveShellObject, archiveFilePath); // ArchiveItemInfo.From(_archiveItemInfoProvider.ArchiveShellObject, archiveFilePath);

                //void AddDirectory(string _path, ArchiveFileInfo? archiveFileInfo) =>

                // if (FileTypes.HasFlag(FileTypesFlags.All) || (FileTypes.HasFlag(FileTypesFlags.Folder) && System.IO.Path.GetPathRoot(pathInfo.Path) != pathInfo.Path) || (FileTypes.HasFlag(FileTypesFlags.Drive) && System.IO.Path.GetPathRoot(pathInfo.Path) == pathInfo.Path))

                //AddPath(ref _path, FileType.Folder, ref archiveFileInfo);

                //void AddFile(string _path, ArchiveFileInfo? archiveFileInfo) =>

                // We only make a normalized path if we add the path to the paths to load.

                //AddPath(ref _path, _path.Substring(_path.Length).EndsWith(".lnk")
                //    ? FileType.Link
                //    : IO.Path.IsSupportedArchiveFormat(System.IO.Path.GetExtension(_path)) ? FileType.Archive : FileType.File, ref archiveFileInfo);



                string fileName;

                string relativePath = _archiveItemInfoProvider.Path.Length > _archiveItemInfoProvider.ArchiveShellObject.Path.Length && _archiveItemInfoProvider.Path.StartsWith(_archiveItemInfoProvider.ArchiveShellObject.Path) ? _archiveItemInfoProvider.Path
#if CS8
                    [(
#else
                    .Substring(
#endif
                _archiveItemInfoProvider.ArchiveShellObject.Path.Length + 1)
#if CS8
                ..]
#endif
                : string.Empty;

                // PathInfo path;

                //#if DEBUG

                //                foreach (ArchiveFileInfo archiveFileInfo in archiveFileData)

                //                    Debug.WriteLine(archiveFileInfo.FileName);

                //#endif

                Action addValue;

                bool addPath(in ArchiveFileInfoEnumeratorStruct archiveFileInfoEnumeratorStruct)
                {
                    if (Func is object && !Func(archiveFileInfoEnumeratorStruct))

                        return false;

                    if (!archiveFileInfoEnumeratorStruct.ArchiveFileInfo.HasValue || archiveFileInfoEnumeratorStruct.ArchiveFileInfo.Value.ArchiveFileInfo.IsDirectory)
                    {
                        foreach (string path in _paths)

                            if (path == fileName)

                                return false;

                        _paths.Enqueue(archiveFileInfoEnumeratorStruct.GetFileName());
                    }

                    addValue();

                    _index++;

                    return true;
                }

                ArchiveFileInfo archiveFileInfo;

                for (; _index < _archiveExtractor.ArchiveFileData.Count; _index++)
                {
                    archiveFileInfo = _archiveExtractor.ArchiveFileData[_index];

                    // _path = archiveFileInfo.FileName.Replace('/', IO.Path.PathSeparator);

                    if (archiveFileInfo.FileName.StartsWith(relativePath, StringComparison.OrdinalIgnoreCase) && archiveFileInfo.FileName.Length > relativePath.Length)
                    {
                        fileName = archiveFileInfo.FileName
#if CS8
                            [relativePath.Length..];
#else
                            .Substring(relativePath.Length);
#endif

                        if (fileName.StartsWith(DirectorySeparatorChar))

                            fileName = fileName
#if CS8
                            [1..];
#else
                            .Substring(1);
#endif

                        if (fileName.Contains(DirectorySeparatorChar
#if CS8
                            , StringComparison.OrdinalIgnoreCase
#endif
                            ))

                            fileName = fileName.Substring(0, fileName.IndexOf(DirectorySeparatorChar
#if CS8
                                , StringComparison.OrdinalIgnoreCase
#endif
                                ));

                        /*if (!archiveFileInfo.FileName.Substring(archiveFileInfo.FileName.Length).Contains(IO.Path.PathSeparator))*/

                        // {

                        ArchiveFileInfoEnumeratorStruct archiveFileInfoEnumeratorStruct;

                        var _fileName = new ArchiveFileInfoEnumeratorStruct2(relativePath, fileName);

                        if (_fileName.Path.ToUpperInvariant() == archiveFileInfo.FileName.ToUpperInvariant())
                        {
                            archiveFileInfoEnumeratorStruct = new ArchiveFileInfoEnumeratorStruct(new ArchiveFileInfoEnumeratorStruct3(archiveFileInfo, fileName));

                            addValue = () => AddArchiveFileInfo(archiveFileInfoEnumeratorStruct.ArchiveFileInfo.Value.ArchiveFileInfo);
                        }

                        else
                        {
                            archiveFileInfoEnumeratorStruct = new ArchiveFileInfoEnumeratorStruct(_fileName);

                            addValue = () => AddPath(archiveFileInfoEnumeratorStruct.RelativePath.Value.Path);
                        }

                        if (addPath(archiveFileInfoEnumeratorStruct))
                            //{
                            //if (archiveFileInfo.IsDirectory)

                            //    AddDirectory(fileName, archiveFileInfo);

                            //else /*if (CheckFilter(archiveFileInfo.FileName))*/

                            //    AddFile(fileName, archiveFileInfo);

                            return true;
                        //}
                        // }
                    }
                }

                //#if NETFRAMEWORK

                //                        }

                //#endif
            }

            catch (Exception ex) when (ex.Is(false, typeof(IOException), typeof(SecurityException), typeof(UnauthorizedAccessException), typeof(SevenZipException)))
            {
                _index++;
            }

            return false;
        }

        protected override void ResetCurrent() => _current = null;

        protected override void ResetOverride2()
        {
            _index = DefaultIndex;

            _paths.Clear();
        }

        #region IDisposable Support

#if !WinCopies3
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
}
