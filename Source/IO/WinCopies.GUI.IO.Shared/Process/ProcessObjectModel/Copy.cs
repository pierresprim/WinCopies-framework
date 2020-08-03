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

using Microsoft.WindowsAPICodePack.Win32Native;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Globalization;

using WinCopies.Util;

using Size = WinCopies.IO.Size;

namespace WinCopies.GUI.IO.Process
{
    public sealed class CopyProcessPathCollection : PathCollection<WinCopies.IO.IPathInfo>
    {
        protected override Func<WinCopies.IO.IPathInfo> GetNewEmptyEnumeratorPathInfoDelegate { get; }

        protected override Func<WinCopies.IO.IPathInfo, WinCopies.IO.IPathInfo> GetNewEnumeratorPathInfoDelegate { get; }

        protected override Func<WinCopies.IO.IPathInfo, WinCopies.IO.IPathInfo> GetNewPathInfoDelegate { get; } = path => path;

        public override Func<WinCopies.IO.IPathInfo, Size?> GetPathSizeDelegate { get; } = item =>
         {
             if (item.IsDirectory) return null;

             else return (Size)new FileInfo(item.Path).Length;
         };

        public CopyProcessPathCollection(string path) : this(path, new List<WinCopies.IO.IPathInfo>()) { }

        public CopyProcessPathCollection(string path, IList<WinCopies.IO.IPathInfo> list) : base(path, list)
        {
            GetNewEmptyEnumeratorPathInfoDelegate = () => new WinCopies.IO.PathInfo(Path, System.IO.Directory.Exists(Path));

            GetNewEnumeratorPathInfoDelegate = current => new WinCopies.IO.PathInfo(GetConcatenatedPath(current), current.IsDirectory);
        }
    }

    public class Copy : PathToPathProcess<WinCopies.IO.IPathInfo, ProcessQueueCollection, ReadOnlyProcessQueueCollection, ProcessErrorPathQueueCollection, ReadOnlyProcessErrorPathQueueCollection
#if DEBUG
         , CopyProcessSimulationParameters
#endif
        >
    {
        #region Private fields

        private bool _autoRenameFiles;
        private IEnumerator<WinCopies.IO.IPathInfo> _pathsToLoadEnumerator;
        private int _bufferLength;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates whether files are automatically renamed when they conflict with existing paths.
        /// </summary>
        public bool AutoRenameFiles
        {
            get => _autoRenameFiles;

            set
            {
                ThrowIfIsBusy();

                if (value != _autoRenameFiles)
                {
                    _autoRenameFiles = value;

                    OnPropertyChanged(nameof(AutoRenameFiles));
                }
            }
        }

        public int BufferLength
        {
            get => _bufferLength;

            set
            {
                ThrowIfIsBusy();

                if (value < 0 ? throw new ArgumentOutOfRangeException($"{nameof(value)} cannot be less than zero.") : value != _bufferLength)
                {
                    _bufferLength = value;

                    OnPropertyChanged(nameof(BufferLength));
                }
            }
        }

        #endregion

        public static Copy From(in CopyProcessPathCollection pathsToLoad, in string destPath
#if DEBUG
             , in CopyProcessSimulationParameters simulationParameters
#endif
            )
        {
            var processQueueCollection = new ProcessQueueCollection();
            var processErrorPathQueueCollection = new ProcessErrorPathQueueCollection();

            return new Copy(pathsToLoad, destPath, processQueueCollection, new ReadOnlyProcessQueueCollection(processQueueCollection), processErrorPathQueueCollection, new ReadOnlyProcessErrorPathQueueCollection(processErrorPathQueueCollection)
#if DEBUG
                 , simulationParameters
#endif
                );
        }

        private Copy(in CopyProcessPathCollection pathsToLoad, in string destPath, in ProcessQueueCollection pathCollection, in ReadOnlyProcessQueueCollection readOnlyPathCollection, in ProcessErrorPathQueueCollection errorPathCollection, ReadOnlyProcessErrorPathQueueCollection readOnlyErrorPathCollection
#if DEBUG
            , in CopyProcessSimulationParameters simulationParameters
#endif
            ) : base(pathsToLoad, destPath, pathCollection, readOnlyPathCollection, errorPathCollection, readOnlyErrorPathCollection
#if DEBUG
                 , simulationParameters
#endif
                ) =>
            // {
            // BackgroundWorker.DoWork += (object sender, DoWorkEventArgs e) => OnDoWork(e);

            // BackgroundWorker.RunWorkerAsync(pathsToLoad);

            _pathsToLoadEnumerator = pathsToLoad.GetEnumerator(null, null
#if NETCORE
                , null
#endif
                , WinCopies.IO.FileSystemEntryEnumerationOrder.FilesThenDirectories
#if DEBUG
                , simulationParameters.FileSystemEntryEnumeratorProcessSimulation
#endif
                );
        // }

        #region Method overrides

        protected override ProcessError OnLoadPaths(DoWorkEventArgs e)
        {
            ProcessError result = ProcessHelper._LoadFileSystemEnumerationProcessPaths(this, _pathsToLoadEnumerator);

            _pathsToLoadEnumerator.Dispose();

            _pathsToLoadEnumerator = null;

            return result;
        }

        protected override ProcessError OnProcessDoWork(DoWorkEventArgs e)
        {
            ProcessError error = CheckIfDrivesAreReady(
#if DEBUG
                 SimulationParameters
#endif
                );

            if (error == ProcessError.None)
            {
                // string sourcePath;
                string destPath;
                bool alreadyRenamed;

                void renameOnDuplicate()
                {
                    destPath =
#if DEBUG
                                SimulationParameters?.RenameOnDuplicateAction(destPath) ??
#endif
                                WinCopies.IO.Path.RenameDuplicate(destPath);

                    alreadyRenamed = true;
                }

                void copyFileOrCreateDirectory()
                {
                    bool cancel = false;
                    bool result;

                    if (CurrentPath.Size.HasValue)
                    {
                        CopyFileFlags copyFileFlags = CopyFileFlags.FailIfExists | CopyFileFlags.NoBuffering;

                        if (CurrentPath.Path.EndsWith(".lnk", true, CultureInfo.InvariantCulture))

                            copyFileFlags |= CopyFileFlags.CopySymLink;

                        CopyProgressResult getCopyProgressResult() => CancellationPending ? CopyProgressResult.Cancel : CopyProgressResult.Continue;

                        Func<long/*, CopyProgressCallbackReason*/, CopyProgressResult> _copyProgressRoutine = __totalBytesTransferred =>
                        {
                            if (Paths.Size.ValueInBytes.IsNaN || CurrentPath.Size.Value.ValueInBytes.IsNaN)

                                _copyProgressRoutine = ___totalBytesTransferred => getCopyProgressResult();

                            else

                                _copyProgressRoutine = ___totalBytesTransferred/*, CopyProgressCallbackReason __copyProgressCallbackReason*/ =>
                                {
                                    _Paths.DecrementSize((ulong)___totalBytesTransferred);

                                    TryReportProgress(((int)Paths.Size / (int)InitialItemSize) * 100);

                                    return getCopyProgressResult();
                                };

                            return getCopyProgressResult();
                        };

                        CopyProgressResult copyProgressRoutine(long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, uint streamNumber, CopyProgressCallbackReason copyProgressCallbackReason, IntPtr sourceFile, IntPtr destinationFile, IntPtr data) => _copyProgressRoutine(totalBytesTransferred/*, copyProgressCallbackReason*/);

                        result =
#if DEBUG
                                    (SimulationParameters?.CopyFileExAction ??
#endif
                                        Shell.CopyFileEx
#if DEBUG
                                    )
#endif
                                    (CurrentPath.Path, destPath, copyProgressRoutine, IntPtr.Zero, ref cancel, copyFileFlags);
                    }

                    else

                        result =
#if DEBUG
                                    SimulationParameters?.CreateDirectoryWAction?.Invoke(destPath) ??
#endif
                                    Directory.CreateDirectoryW(destPath, IntPtr.Zero);

                    void reportProgressCommon() => TryReportProgress((_Paths.Count / InitialItemCount) * 100);

                    Action reportProgress = () =>
                    {
                        if (Paths.Size.ValueInBytes.IsNaN || Paths.Size.ValueInBytes == 0)
                        {
                            reportProgress = reportProgressCommon;

                            reportProgressCommon();
                        }
                    };

                    if (result)
                    {
                        _ = _Paths.Dequeue();

                        if (CurrentPath.Size.HasValue)

                            reportProgressCommon();

                        else

                            reportProgress();

                        return;
                    }

                    switch ((ErrorCode)Marshal.GetLastWin32Error())
                    {
                        // todo: the current version of this process is not optimized: when a file name conflict occurs when we want to create a folder, we know that all the subpaths won't be able to be copied neither. So, we should have a tree structure, so we can dequeue all the path in conflict with all of its subpaths at one time.
                        case ErrorCode.AlreadyExists:

                            if (CurrentPath.Size.HasValue) // We do not try to rename folders, because folder name conflicts are handled the same way as file name conflicts.
                            {
                                if (alreadyRenamed)
                                {
                                    RemoveErrorPath(ProcessError.FileRenamingFailed);

                                    return;
                                }

                                if (_autoRenameFiles)
                                {
                                    renameOnDuplicate();

                                    copyFileOrCreateDirectory();

                                    return;
                                }
                            }

                            RemoveErrorPath(ProcessError.FileSystemEntryAlreadyExists);

                            break;

                        case ErrorCode.PathNotFound:

                            RemoveErrorPath(ProcessError.PathNotFound);

                            break;

                        case ErrorCode.AccessDenied:

                            RemoveErrorPath(ProcessError.ReadProtection);

                            break;

                        case ErrorCode.DiskFull:

                            RemoveErrorPath(ProcessError.NotEnoughSpace);

                            break;

                        case ErrorCode.DiskOperationFailed:

                            RemoveErrorPath(ProcessError.DiskError);

                            break;

                        case (ErrorCode)6000: // Encryption failed

                            RemoveErrorPath(ProcessError.EncryptionFailed);

                            break;

                        default:

                            RemoveErrorPath(ProcessError.UnknownError);

                            break;
                    }
                }

                int splitLength = 0;

                string getDestPathCommon() => $"{DestPath}{WinCopies.IO.Path.PathSeparator}{CurrentPath.Path.Substring(splitLength)}";

                Func<string> getDestPath = () =>
                {
                    getDestPath = getDestPathCommon;

                    if (PathCollection.Count == 0)
                    {
                        splitLength = PathCollection.Path.Length - Path.GetFileName(PathCollection.Path).Length;

                        return $"{DestPath}{WinCopies.IO.Path.PathSeparator}{Path.GetFileName(CurrentPath.Path)}";
                    }

                    else
                    {
                        splitLength = PathCollection.Path.Length + 1;

                        return getDestPathCommon();
                    }
                };

                while (_Paths.Count > 0)
                {
                    if (CheckIfPauseOrCancellationPending())

                        return Error;

                    alreadyRenamed = false;

                    CurrentPath = _Paths.Peek();

                    // sourcePath = $"{SourcePath}{WinCopies.IO.Path.PathSeparator}{CurrentPath.Path}";

                    destPath = getDestPath();

                    if ((!CurrentPath.IsDirectory &&

#if DEBUG
                                        SimulationParameters == null &&
#endif
                                            WinCopies.IO.Path.Exists(destPath))
#if DEBUG
                                             || SimulationParameters?.DestPathExistsAction(destPath) == true
#endif
                                            )

                        if (_autoRenameFiles)

                            if (_bufferLength == 0)

                                renameOnDuplicate();

                            else
                            {
#if DEBUG
                                FileStream sourceFileStream = null;
#endif
                                try
                                {
#if DEBUG
                                    if (SimulationParameters == null)
#else
                                    using
#if CS7
                                                        (
#endif
                                                        FileStream
#endif
                                        sourceFileStream = ProcessHelper.GetFileStream(CurrentPath.Path, _bufferLength)
#if CS7 && !DEBUG
                                                        )
                                                        {
#else
                                    ;
#endif

#if DEBUG
                                    else
                                    {
                                        Exception exception = SimulationParameters.CreatingFileStreamSucceedsAction(CurrentPath.Path, PathDirectoryType.Source);

                                        if (exception == null)

                                            sourceFileStream = null;

                                        else

                                            throw exception;
                                    }

                                    FileStream destFileStream = null;
#endif

                                    try
                                    {
#if DEBUG
                                        if (SimulationParameters == null)
#else
                                        using
#if CS7
                                                            (
#endif
                                                            FileStream
#endif
                                            destFileStream = ProcessHelper.GetFileStream(destPath, _bufferLength)
#if CS7 && !DEBUG
                                                            )
                                                                {
#else
                                                            ;
#endif

#if DEBUG

                                        else
                                        {
                                            Exception exception = SimulationParameters.CreatingFileStreamSucceedsAction(destPath, PathDirectoryType.Destination);

                                            if (exception != null)

                                                throw exception;
                                        }
#endif

                                        bool? _result;
#if DEBUG
                                        if (SimulationParameters == null)
#endif

                                        _result = WinCopies.IO.File.IsDuplicate(sourceFileStream, destFileStream, _bufferLength, () => CheckIfPauseOrCancellationPending());

#if DEBUG

                                        else

                                            _result = SimulationParameters.IsDuplicateAction(CurrentPath.Path, destPath, () => CheckIfPauseOrCancellationPending());

#endif

                                        if (_result.HasValue)

                                            if (_result.Value)

                                                _ = _Paths.Dequeue();

                                            else

                                                renameOnDuplicate();

                                        else

                                            return Error;
#if CS7 && !DEBUG
                                                                }
#endif
                                    }

                                    catch (System.IO.FileNotFoundException)
                                    {
                                        // Left empty because this is a check for duplicate.
                                    }

                                    catch (Exception ex) when (ex.Is(false, typeof(System.UnauthorizedAccessException), typeof(System.Security.SecurityException)))
                                    {
                                        RemoveErrorPath(ProcessError.DestinationReadProtection);

                                        continue;
                                    }
#if DEBUG
                                    finally
                                    {
                                        destFileStream?.Dispose();
                                    }
#endif

#if NETFRAMEWORK && !DEBUG
                                                        }
#endif
                                }

                                catch (System.IO.IOException ex) when (ex.Is(false, typeof(System.IO.FileNotFoundException), typeof(System.IO.DirectoryNotFoundException)))
                                {
                                    RemoveErrorPath(ProcessError.PathNotFound);

                                    continue;
                                }

                                catch (System.IO.PathTooLongException)
                                {
                                    RemoveErrorPath(ProcessError.PathTooLong);

                                    continue;
                                }

                                catch (System.IO.IOException)
                                {
                                    RemoveErrorPath(ProcessError.UnknownError);

                                    continue;
                                }

                                catch (Exception ex) when (ex.Is(false, typeof(System.UnauthorizedAccessException), typeof(System.Security.SecurityException)))
                                {
                                    RemoveErrorPath(ProcessError.ReadProtection);

                                    continue;
                                }
#if DEBUG
                                finally
                                {
                                    sourceFileStream?.Dispose();
                                }
#endif
                            }

                        else
                        {
                            RemoveErrorPath(ProcessError.FileSystemEntryAlreadyExists);

                            continue;
                        }

                    copyFileOrCreateDirectory();
                }
            }

            return error;
        }

        #endregion
    }
}
