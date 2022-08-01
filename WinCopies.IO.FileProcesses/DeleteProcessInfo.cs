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

using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WinCopies.Util;
using static WinCopies.Util.Util;

namespace WinCopies.IO.FileProcesses
{
    public class DeleteProcessInfo : Process
    {
        /// <summary>
        /// Gets the type of this process.
        /// </summary>
        public override ActionType ActionType { get; }

        private long _deletedFiles = 0;

        /// <summary>
        /// Gets the number of deleted files.
        /// </summary>
        public long DeletedFiles

        {

            get => _deletedFiles;

            private set => OnPropertyChanged(nameof(DeletedFiles), nameof(_deletedFiles), value, typeof(DeleteProcessInfo));

        }

        public FileSystemInfo _currentDeletedFile = null;

        /// <summary>
        /// Gets the file which is being deleted.
        /// </summary>
        public FileSystemInfo CurrentDeletedFile

        {

            get => _currentDeletedFile;

            set => OnPropertyChanged(nameof(CurrentDeletedFile), nameof(_currentDeletedFile), value, typeof(DeleteProcessInfo));

        }

        private Size _current_Deleted_Size = new Size(0, SizeUnit.Byte);

        /// <summary>
        /// Gets the total deleted size.
        /// </summary>
        public Size CurrentDeletedSize { get => _current_Deleted_Size; set => OnPropertyChanged(nameof(CurrentDeletedSize), nameof(_current_Deleted_Size), value, typeof(DeleteProcessInfo)); }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteProcessInfo"/> class.
        /// </summary>
        public DeleteProcessInfo(bool tryRecycle) => ActionType = tryRecycle ? ActionType.Recycling : ActionType.Deletion;

        protected override void OnDoWork(DoWorkEventArgs e)
        {

            OnPropertyChanged(nameof(IsBusy), false, true);

            bool onlyFirstFile = (bool)e.Argument;

            if (PausedFiles.Count == 0 && HowToRetryWhenExceptionOccured == HowToRetry.Cancel)

            {

                FileSystemInfo item = null;

                void delete()

                {

                    item = ExceptionsProtected[0];

                    item._exception = FileProcesses.Exceptions.None;

                    DeletedFiles += 1;

                    if (ActionType != ActionType.Recycling && item.FileSystemInfoProperties.GetType() == typeof(FileInfo))

                        CurrentDeletedSize += ((FileInfo)item.FileSystemInfoProperties).Length;

                    ExceptionsProtected.RemoveAt(0);

                }

                if (onlyFirstFile)

                    delete();

                else

                    while (ExceptionsProtected.Count > 0)

                        delete();

                ExceptionsOccurred = ExceptionsProtected.Count == 0;

                return;

            }



            // List<string> folder_Path = new List<string>(); // Liste des répertoires à supprimer lors du déplacement une fois celui-ci effectué (déplacements uniquement).

            void getStartAndLength(Exceptions flag, out int _start, out int _length)

            {

                if (flag == FileProcesses.Exceptions.None)

                {

                    _start = StartItemIndex;

                    _length = FilesInfoLoader.PathsLoaded.Count;

                }

                else

                {

                    _start = ExceptionsProtected.IndexOf(ExceptionsProtected.First((FileSystemInfo f) => f.Exception == flag));

                    _length = onlyFirstFile ? 1 : ExceptionsProtected.IndexOf(ExceptionsProtected.Last((FileSystemInfo f) => f.Exception == flag)) + 1;

                }

            }

            int start;

            int length;

            if (PausedFiles.Count > 0)

                this_all_Files_DoWork(PausedIndex, PausedFiles.Count, _PausedFiles, _PausedFiles == ExceptionsProtected, true);

            else if (ExceptionsToRetry == FileProcesses.Exceptions.None)
            {

                getStartAndLength(FileProcesses.Exceptions.None, out start, out length);

                this_all_Files_DoWork(start, length, FilesInfoLoader.PathsLoaded);

            }

            else

            {

                Type type = ExceptionsToRetry.GetType();

                Enum enumValue = null;



                foreach (string s in type.GetEnumNames())

                {

                    enumValue = (Enum)Enum.Parse(type, s);



                    if (enumValue.GetNumValue(s).Equals(0)) continue;



                    if (ExceptionsToRetry.HasFlag(enumValue))

                    {

                        getStartAndLength((Exceptions)enumValue, out start, out length);

                        this_all_Files_DoWork(start, length, Exceptions, true);

                        ExceptionsOccurred = ExceptionsProtected.Count > 0; // If we are in a re-try process, we check if there are still paths in the exceptions list.

                    }

                }

            }

        }

        private void this_all_Files_DoWork(int start, int length, IList<FileSystemInfo> items, bool isARetry = false, bool isResuming = false)
        {

            Exceptions deleteFile(FileSystemInfo _path, FileOperation _fileOperation)

            {

                if (!_path.FileSystemInfoProperties.Exists)

                    return FileProcesses.Exceptions.PathNotFound;

                if (_path.FileType == FileType.Drive)

                    return FileProcesses.Exceptions.NotAllowedOnDrives;

                if (_fileOperation != null)

                {

                    ShellObject shellObject = ShellObject.FromParsingName(_path.FileSystemInfoProperties.FullName);

                    Guid guid = KnownFolders.RecycleBin.FolderId;

                    // todo: to optimize

                    while ((shellObject = shellObject.Parent) != null)

                    {

                        if ((shellObject is FileSystemKnownFolder knownFolder && knownFolder.FolderId == guid) || (shellObject is NonFileSystemKnownFolder folder && folder.FolderId == guid))

                            return FileProcesses.Exceptions.FileIsAlreadyInRecycleBin;

                    }

                    if (FileOperation.QueryRecycleBinInfo(System.IO.Path.GetPathRoot(_path.FileSystemInfoProperties.FullName), out RecycleBinInfo recycleBinInfo))

                    {

                        _fileOperation.DeleteItem(ShellObject.FromParsingName(_path.FileSystemInfoProperties.FullName), null);

                        try

                        {

                            _fileOperation.PerformOperations();

                        }

                        catch (Win32Exception)

                        {

                            return FileProcesses.Exceptions.Unknown;

                        }

                        return FileProcesses.Exceptions.None;

                    }

                    else return FileProcesses.Exceptions.NotAllowedOnCurrentFileSystem;

                }

                else

                {

                    if (If(ComparisonType.Or, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, _path.FileType, FileType.Folder, FileType.SpecialFolder))

                        Directory.Delete(_path.FileSystemInfoProperties.FullName);

                    else if (If(ComparisonType.Or, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, _path.FileType, FileType.File, FileType.Link, FileType.Archive))

                        File.Delete(_path.FileSystemInfoProperties.FullName);

                    return FileProcesses.Exceptions.None;

                }

            }



            FileOperation getFileOperation()

            {

                FileOperation fileOperation = new FileOperation();

                fileOperation.Advise(new FileOperationProgressSink());

                fileOperation.SetOperationFlags(ShellOperationFlags.FOF_ALLOWUNDO | ShellOperationFlags.FOF_NOERRORUI | ShellOperationFlags.FOFX_EARLYFAILURE | ShellOperationFlags.FOF_SILENT);

                return fileOperation;

            }



            void onException(FileSystemInfo path, int progress, Exceptions exception)

            {

                ExceptionsOccurred = true;

                if (path._exception != exception)

                {

                    path._exception = exception;

                    if (!isARetry)

                        ExceptionsProtected.Add(path);

                }

                ReportProgress(progress);

            }



            if (isResuming && PausedFile != null)

            {

                Exceptions e = FileProcesses.Exceptions.None;

                FileSystemInfo path = null;

                int index = -1;

                if (ActionType == ActionType.Recycling)

                    using (FileOperation fileOperation = getFileOperation())

                        e = deleteFile(PausedFile, fileOperation);

                path = PausedFile;

                PausedFile = null;

                _PausedFiles = new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>();

                index = PausedIndex;

                PausedIndex = -1;

                if (e != FileProcesses.Exceptions.None)

                    onException(path, index / length * 100, e);

            }

            else if (!isResuming)

            {

                if (length == 1 && HowToRetryWhenExceptionOccured == HowToRetry.None && items[start].HowToRetryToProcess == HowToRetry.None)

                    return;



                ExceptionsOccurred = false;

                FileSystemInfo path = null;

                int i = start;

                    try

                    {

                while (i < length && !CancellationPending)

                {

                        #region item pre-processing actions

                        path = items[i];

                        if (PausePending)

                        {

                            PausedFile = path;

                            _PausedFiles = isARetry ? (System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>)items : new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>(items);

                            PausedIndex = i;



                            if (StopProcessOnPause) return;

                            Pause();

                        }

                        if (isARetry && path.HowToRetryToProcess != HowToRetry.Retry && HowToRetryWhenExceptionOccured != HowToRetry.Retry && path.HowToRetryToProcess != HowToRetry.Rename && HowToRetryWhenExceptionOccured != HowToRetry.Rename && path.HowToRetryToProcess != HowToRetry.Replace && HowToRetryWhenExceptionOccured != HowToRetry.Replace)

                        {

                            if (path.HowToRetryToProcess != HowToRetry.None || HowToRetryWhenExceptionOccured != HowToRetry.None)

                            {

                                ExceptionsProtected.RemoveAt(i);

                                // items.RemoveAt(i);    

                                // todo : HowToRetry ...

                                //if (path.HowToRetryToProcess == HowToRetry.None)

                                //{

                                //    onException(path.Exception);

                                //    continue;

                                //}

                                if (path.HowToRetryToProcess == HowToRetry.Ignore || HowToRetryWhenExceptionOccured == HowToRetry.Ignore)

                                {

                                    path._exception = FileProcesses.Exceptions.None;

                                    DeletedFiles += 1;

                                    if (path.FileSystemInfoProperties.GetType() == typeof(FileInfo))

                                        CurrentDeletedSize += ((FileInfo)path.FileSystemInfoProperties).Length;

                                }

                            }

                            // i++;

                            length--;

                            ReportProgress(0);

                            ExceptionsOccurred = i == length && ExceptionsProtected.Count > 0;

                            continue;

                        }



                        CurrentDeletedFile = path;

                        #endregion

                        Exceptions e = FileProcesses.Exceptions.None;

                        using (FileOperation fileOperation = getFileOperation())

                            if ((e = deleteFile(path, fileOperation)) == FileProcesses.Exceptions.None)

                                DeletedFiles += 1;

                            else

                                onException(path, i / length * 100, e);

                        //else if (ex == Exceptions.FileAlreadyExists)

                        //{

                        //    _file_Already_Exists.Add(path);

                        //}

                        //else if (ex == Exceptions.DirectoryNotFound)

                        //{

                        //    _directory_Not_Found.Add(path);

                        //}



                        //TODO:

                        // if (start_Item_Index + 1 == pathsToCopy.Count) start_Item_Index = 0; else start_Item_Index += 1;

                        //TODO:

                        ReportProgress(i / length * 100);

                    if (isARetry)

                    {

                        ExceptionsProtected.RemoveAt(0);

                        length--;

                    }

                    else

                        i++;

                    }

                }

                catch (DirectoryNotFoundException)
                {

                    onException(path, i / length * 100, FileProcesses.Exceptions.PathNotFound);

                }
                catch (PathTooLongException)
                {

                    onException(path, i / length * 100, FileProcesses.Exceptions.FileNameTooLong);

                }
                // catch (IOException) { ex = FileProcesses.Exceptions.FileAlreadyExists; } 
                catch (UnauthorizedAccessException)
                {

                    onException(path, i / length * 100, FileProcesses.Exceptions.AccessDenied);

                }

                catch (IOException)

                {

                    if (new DriveInfo(System.IO.Path.GetPathRoot(FilesInfoLoader.SourcePath)).IsReady)

                    {

                        ExceptionsOccurred = true;

                        ExceptionsProtected.Clear();

                        foreach (FileSystemInfo _path in items)

                        {

                            _path._exception = FileProcesses.Exceptions.DiskNotReady;

                            ExceptionsProtected.Add(_path);

                        }

                    }

                    else

                        onException(path, i / length * 100, FileProcesses.Exceptions.Unknown);

                }

                catch (Win32Exception)

                {

                    onException(path, i / length * 100, FileProcesses.Exceptions.Unknown);

                }

                // todo : maybe more functional with a dictionary which would create a collection corresponding to an exception when it'd be needed (this case would mean no sorting needed)

                ExceptionsProtected.Sort(new FileSystemInfoComparer());

            }

        }
    }
}
