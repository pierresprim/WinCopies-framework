/* Copyright © Pierre Sprimont, 2021
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
using System.IO;
using System.Linq;
using System.Security;

using WinCopies.Collections.Generic;
using WinCopies.IO.Enumeration;
using WinCopies.Util;

using static WinCopies.Collections.Util;

namespace WinCopies.IO
{
    public static class Directory
    {
        #region Safe Enumeration
        public static Type[] GetIOSafeEnumerationExceptionTypes() => new Type[] { typeof(DirectoryNotFoundException), typeof(IOException), typeof(PathTooLongException), typeof(SecurityException), typeof(UnauthorizedAccessException) };

        public static System.Collections.Generic.IEnumerable<string> EnumerateFileSystemEntriesIOSafe(string path)
        {
            try
            {
                return System.IO.Directory.EnumerateFileSystemEntries(path);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }

        public static System.Collections.Generic.IEnumerable<string> EnumerateFileSystemEntriesIOSafe(in string path, in string searchPattern)
        {
            try
            {
                return System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }

#if CS8
        public static System.Collections.Generic.IEnumerable<string> EnumerateFileSystemEntriesIOSafe(in string path, in string searchPattern, in EnumerationOptions enumerationOptions)
        {
            try
            {
                return System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }
#endif

        public static System.Collections.Generic.IEnumerable<string> EnumerateFileSystemEntriesIOSafe(string path, in string searchPattern, in SearchOption searchOption)
        {
            try
            {
                return System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }



        public static System.Collections.Generic.IEnumerable<string> EnumerateDirectoriesIOSafe(string path)
        {
            try
            {
                return System.IO.Directory.EnumerateDirectories(path);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }

        public static System.Collections.Generic.IEnumerable<string> EnumerateDirectoriesIOSafe(string path, string searchPattern)
        {
            try
            {
                return System.IO.Directory.EnumerateDirectories(path, searchPattern);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }

#if CS8
        public static System.Collections.Generic.IEnumerable<string> EnumerateDirectoriesIOSafe(string path, string searchPattern, EnumerationOptions enumerationOptions)
        {
            try
            {
                return System.IO.Directory.EnumerateDirectories(path, searchPattern, enumerationOptions);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }
#endif

        public static System.Collections.Generic.IEnumerable<string> EnumerateDirectoriesIOSafe(string path, string searchPattern, SearchOption searchOption)
        {
            try
            {
                return System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOption);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }



        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesIOSafe(string path)
        {
            try
            {
                return System.IO.Directory.EnumerateFiles(path);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }

        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesIOSafe(string path, string searchPattern)
        {
            try
            {
                return System.IO.Directory.EnumerateFiles(path, searchPattern);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }

#if CS8
        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesIOSafe(string path, string searchPattern, EnumerationOptions enumerationOptions)
        {
            try
            {
                return System.IO.Directory.EnumerateFiles(path, searchPattern, enumerationOptions);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }
#endif

        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesIOSafe(string path, string searchPattern, SearchOption searchOption)
        {
            try
            {
                return System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption);
            }

            catch (Exception ex) when (ex.Is(false, GetIOSafeEnumerationExceptionTypes()))
            {
                return GetEmptyEnumerable<string>();
            }
        }
        #endregion Safe Enumeration



        public static System.Collections.Generic.IEnumerable<T> Enumerate<T>(in System.Collections.Generic.IEnumerable<T> paths, RecursiveEnumerationOrder recursiveEnumerationOrder, in Func<IPathInfo, T> getNewPathInfoDelegate, in bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) where T : IPathInfo => Enumerate(paths, null, null
#if CS8
            , null
#endif
                , FileSystemEntryEnumerationOrder.None, recursiveEnumerationOrder, getNewPathInfoDelegate, safeEnumeration
#if DEBUG
                , simulationParameters
#endif
            );

        ///// <summary>
        ///// Loads the items.
        ///// </summary>
        public static System.Collections.Generic.IEnumerable<T> Enumerate<T>(System.Collections.Generic.IEnumerable<T> paths, string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
            , FileSystemEntryEnumerationOrder enumerationOrder, RecursiveEnumerationOrder recursiveEnumerationOrder, Func<IPathInfo, T> getNewPathInfoDelegate, bool safeEnumeration
#if DEBUG
            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
#endif
            ) where T : IPathInfo => new Enumerable<T>(() => new RecursiveEnumerator<T>(paths.Select(item => new RecursivelyEnumerablePath<T>(item, searchPattern, searchOption
#if CS8
                , enumerationOptions
#endif
                , enumerationOrder, recursiveEnumerationOrder, getNewPathInfoDelegate, safeEnumeration
#if DEBUG
                , simulationParameters
#endif
                )), recursiveEnumerationOrder));

        #region Old

        // _pathsLoaded = new ObservableCollection<FileSystemInfo>();

        //string parent_Path = "";

        //if (((System.IO.DirectoryInfo)path.FileSystemInfoProperties).Parent != null) parent_Path = ((System.IO.DirectoryInfo)path.FileSystemInfoProperties).Parent.FullName;


        //    else
        //    {

        //        if (new DriveInfo(((System.IO.DirectoryInfo)path.FileSystemInfoProperties).Root.FullName).VolumeLabel == string.Empty)
        //        {

        //            parent_Path = ((DirectoryInfo)path.FileSystemInfoProperties).Root.FullName;

        //            parent_Path = parent_Path.Substring(0, parent_Path.Length - 2);

        //        } // end if

        //        else

        //            parent_Path = new DriveInfo(((DirectoryInfo)path.FileSystemInfoProperties).Root.FullName).VolumeLabel;


        //    } // end if



        //if (actionType == ActionType.Recycling)

        //    _pathsLoaded = new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>(Paths);

        //else

        //foreach (IPathInfo path in paths)

        //{

        //    Debug.WriteLine("FilesInfoLoader log: " + path.FileSystemInfoProperties.FullName);

        // parent_Path = "";



        //LinkedList<IPathInfo> files = new LinkedList<IPathInfo>();



        //switch (path.FileType)
        //{

        //    case FileType.Folder:
        //    case FileType.Drive:



        //        List<int> pathsIndexes = new List<int>();

        //        Type t = path.FileSystemInfoProperties.GetType();

        //        DirectoryInfo directoriesInfo = (DirectoryInfo)path.FileSystemInfoProperties;

        //        Debug.WriteLine("FilesInfoLoader log: " + (ActionType != ActionType.Deletion /*&& SearchMethods.AddFile(path.FileSystemInfoProperties, path.FileType, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/).ToString());

        //        // Console.WriteLine("FilesInfoLoader log: "+ActionType.ToString() + " " + path.FileSystemInfoProperties.FullName + " " + path.FileType.ToString() + " " + LoadOnlyItemsWithSearchTermsForAllActions.ToString() + " " + Search_Terms.ToString());

        //        if (ActionType != ActionType.Deletion /*&& SearchMethods.AddFile(path.FileSystemInfoProperties, path.FileType, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/)
        //        {

        //            _pathsLoaded.Add(path);

        //            // System.Windows.Forms.MessageBox.Show("a" + IO.Path.Return_A_Path_With_One_Backslash_Per_Path(path.FileSystemInfoProperties.FullName) + "a" + " " + "b" + IO.Path.Return_A_Path_With_One_Backslash_Per_Path(new System.IO.DirectoryInfo(path.FileSystemInfoProperties.FullName).Root.FullName) + "b" + " " + (IO.Path.Return_A_Path_With_One_Backslash_Per_Path(path.FileSystemInfoProperties.FullName) != IO.Path.Return_A_Path_With_One_Backslash_Per_Path(new System.IO.DirectoryInfo(path.FileSystemInfoProperties.FullName).Root.FullName)).ToString());
        //            //if (path.FileSystemInfoProperties.FullName != ((DirectoryInfo)path.FileSystemInfoProperties).Root.FullName)

        //            //    if (directoriesInfo.Attributes.HasFlag(FileAttributes.Hidden) && (directoriesInfo.GetDirectories().Length > 0 || directoriesInfo.GetFiles().Length > 0))

        //            //        Hidden_Folders_With_Subpaths.Add(path.FileSystemInfoProperties.FullName);

        //            //else
        //            //{

        //            //    _pathsLoaded.Add(new FileSystemInfo(path.FileSystemInfoProperties, FileTypes.Drive));

        //            //    ReportProgress(0);

        //            //}


        //            //FileSystemInfoLoaded = pathsLoaded[pathsLoaded.Count - 1];

        //            // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileSystemInfoThatIsLoading)));

        //            //ReportProgress(0);

        //        } // end if

        //        //TODO : vraiment utile ?

        //        // ReportProgress(0);

        //        try
        //        {

        //            foreach (FileInfo file in directoriesInfo.GetFiles())

        //            {

        //                //if (SearchMethods.AddFile(file, FileType.File, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms))
        //                //{

        //                _pathsLoaded.Add(new FileSystemInfo(file, FileType.File));

        //                TotalSize += file.Length;

        //                //} // end if

        //            } // next file

        //        } // end try

        //        catch (Exception)
        //        { }

        //        int pathSubdirectoriesCount = 0;

        //        try
        //        {

        //            pathSubdirectoriesCount = ((DirectoryInfo)path.FileSystemInfoProperties).GetDirectories().Length;

        //        } // end try

        //        catch (Exception)
        //        { }

        //        pathsIndexes.Add(0);

        //        int findIndex = 0;

        //        while (pathsIndexes[0] < pathSubdirectoriesCount)
        //        {

        //            try
        //            {

        //                DirectoryInfo[] directories = directoriesInfo.GetDirectories();

        //                while (directories.Length > 0)
        //                {



        //                    DirectoryInfo directory = directories[pathsIndexes[findIndex]];

        //                    directories = directory.GetDirectories();



        //                    //if (directory.Attributes.HasFlag(FileAttributes.Hidden) && (directory.GetDirectories().Length > 0 || directory.GetFiles().Length > 0))

        //                    //    Hidden_Folders_With_Subpaths.Add(directory.FullName);




        //                    if (ActionType != ActionType.Deletion /*&& SearchMethods.AddFile(directory, FileType.Folder, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/)
        //                    {

        //                        _pathsLoaded.Add(new FileSystemInfo(directory, FileType.Folder));

        //                    } // end if



        //                    foreach (FileInfo file in directory.GetFiles())

        //                    {

        //                        //if (SearchMethods.AddFile(file, FileType.File, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms))
        //                        //{

        //                        _pathsLoaded.Add(new FileSystemInfo(file, FileType.File));

        //                        TotalSize += file.Length;

        //                        //} // end if

        //                    } // next file

        //                    if (ActionType == ActionType.Deletion /*&& SearchMethods.AddFile(directory, FileType.Folder, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/)
        //                    {

        //                        _pathsLoaded.Add(new FileSystemInfo(directory, FileType.Folder));

        //                        //FileSystemInfoLoaded = pathsLoaded[pathsLoaded.Count - 1];

        //                        // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileSystemInfoThatIsLoading)));

        //                        //ReportProgress(0);

        //                    } // end if

        //                    pathsIndexes.Add(0);

        //                    findIndex++;

        //                } // end while

        //            } // end try

        //            catch (Exception)
        //            { findIndex++; }

        //            do
        //            {



        //                directoriesInfo = directoriesInfo.Parent;

        //                findIndex--;

        //                pathsIndexes[findIndex] += 1;



        //                for (int i = findIndex + 1; i < pathsIndexes.Count; i++)

        //                    pathsIndexes.RemoveAt(findIndex + 1);



        //            } while (directoriesInfo.GetDirectories().Length == pathsIndexes[findIndex] && pathsIndexes[0] != pathSubdirectoriesCount);

        //        } // end while

        //        if (ActionType == ActionType.Deletion /*&& SearchMethods.AddFile(path.FileSystemInfoProperties, path.FileType, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/)
        //        {

        //            _pathsLoaded.Add(path);

        //        } // end if

        //        break;

        //    case FileType.File:

        //        files.AddLast(path);



        //        break;

        //} // end switch

        //while (files.Count > 0)
        //{

        //    var _path = files.RemoveAndGetFirstValue();

        //    try
        //    {

        //        Debug.WriteLine("FilesInfoLoader log: " + _path.FileSystemInfoProperties.FullName + " (1)");

        //        //if (SearchMethods.AddFile(path.FileSystemInfoProperties, FileType.File, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms))
        //        //{

        //        Debug.WriteLine("FilesInfoLoader log: " + _path.FileSystemInfoProperties.FullName + " (2)");

        //        _pathsLoaded.Add(_path);

        //        TotalSize += ((FileInfo)_path.FileSystemInfoProperties).Length;

        //        //} // end if

        //    } // end try

        //    catch (Exception) { }

        //}

        //System.Windows.Forms.MessageBox.Show(path.FileSystemInfoProperties.FullName);

        //} // next

        #endregion
    }
}
