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

using Microsoft.WindowsAPICodePack.Win32Native;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;

using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.IO.Resources;
using WinCopies.Util;

using static WinCopies.IO.File;
using static WinCopies.IO.Resources.ExceptionMessages;
using static WinCopies.Temp;

namespace WinCopies.IO.Process.ObjectModel
{
    public class CopyProcess<T> : ProcessObjectModelTypes<IPathInfo, T, ProcessError, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.ProcessDelegatesAbstract<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.ProcessEventDelegates>, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.ProcessEventDelegates, IProcessProgressDelegateParameter>.DefaultDestinationProcess where T : ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError>.IProcessErrorFactories
    {
        #region Fields
        private int _bufferLength;
        #endregion

        #region Properties
        public override string Name => Properties.Resources.Copy;

        protected ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError>.ProcessOptions Options { get; }

        /// <summary>
        /// Gets a value that indicates whether files are automatically renamed when they conflict with existing paths.
        /// </summary>
        public bool AutoRenameFiles { get; set; }

        public int BufferLength
        {
            get => _bufferLength;

            set
            {
                if (value < 0 ? throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} cannot be less than zero.") : value != _bufferLength)

                    _bufferLength = value;
            }
        }
        #endregion Properties

        public CopyProcess(in IQueueBase<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in QueueParameter paths, in LinkedListParameter errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.ProcessDelegatesAbstract<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.ProcessEventDelegates> progressDelegate, in ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError>.ProcessOptions processOptions) : base(initialPaths, sourcePath, destinationPath.IsDirectory ? destinationPath : throw new ArgumentException($"{nameof(destinationPath)} must be a directory."), paths, errorsQueue, progressDelegate) => Options = processOptions;

        #region Methods
        #region Checks
        protected IProcessError<ProcessError> CheckDrivesAndSpace()
        {
            if (Paths.TotalSize.ValueInBytes.IsNaN)

                return Factory.GetError(ProcessError.NotEnoughSpace, NotEnoughSpace);

            string drive = System.IO.Path.GetPathRoot(SourcePath.Path);

            if (System.IO.Directory.Exists(drive))
            {
                var driveInfo = new DriveInfo(drive);

                if ((driveInfo.IsReady && drive == (drive = System.IO.Path.GetPathRoot(DestinationPath.Path))) || (System.IO.Directory.Exists(drive) && new DriveInfo(drive).IsReady))

                    return driveInfo.TotalFreeSpace >= Paths.TotalSize.ValueInBytes ? new ProcessError<ProcessError>(ProcessErrorFactoryData.NoError, NoError) : new ProcessError<ProcessError>(ProcessError.NotEnoughSpace, NotEnoughSpace);
            }

            return Factory.GetError(ProcessError.DriveNotReady, DriveNotReady);
        }
        #endregion

        protected override bool LoadPathsOverride(out IProcessError<ProcessError> error, out bool clearOnError)
        {
            clearOnError = true;

            while (InitialPaths.HasItems)

                foreach (IRecursivelyEnumerablePath<IPathInfo> _path in new RecursivelyEnumerablePath<IPathInfo>(InitialPaths.Peek(), null, null
#if CS8
                    , null
#endif
                    , FileSystemEntryEnumerationOrder.DirectoriesThenFiles, __path => __path, true, null))
                {
                    AddPath(_path.Value);

                    if (Options.PathLoadedDelegate(_path.Value) && !ProcessDelegates.CancellationPendingDelegate.RaiseEvent(null))

                        continue;

                    error = Factory.GetError(ProcessError.AbortedByUser, AbortedByUser);

                    clearOnError = Options.ClearOnError;

                    return false;
                }

            error = null;

            clearOnError = false;

            return true;
        }

        protected override bool Check(out IProcessError<ProcessError> error)
        {
            error = CheckDrivesAndSpace();

            return error.Error == ProcessErrorFactoryData.NoError;
        }

        protected override bool DoWork(IPathInfo path, out IProcessError<ProcessError> error, out bool isErrorGlobal)
        {
            // string sourcePath;
            string destPath;
            bool alreadyRenamed;

            void renameOnDuplicate()
            {
                destPath = Path.RenameDuplicate(destPath);

                alreadyRenamed = true;
            }

            string getDestPath(in IPathInfo _path) => $"{DestinationPath.Path}{Path.PathSeparator}{_path.GetPath(true)}";

            void copyFileOrCreateDirectory(out IProcessError<ProcessError> _error, out bool _isErrorGlobal)
            {
                bool cancel = false;
                bool result;

                if (path.Size.HasValue)
                {
                    CopyFileFlags copyFileFlags = CopyFileFlags.FailIfExists | CopyFileFlags.NoBuffering;

                    if (path.Path.EndsWith(".lnk", true, CultureInfo.InvariantCulture))

                        copyFileFlags |= CopyFileFlags.CopySymLink;

                    CopyProgressResult getCopyProgressResult() => ProcessDelegates.CancellationPendingDelegate.RaiseEvent(null) ? CopyProgressResult.Cancel : CopyProgressResult.Continue;

                    Func<long/*, CopyProgressCallbackReason*/, CopyProgressResult> _copyProgressRoutine = __totalBytesTransferred =>
                    {
                        if (Paths.TotalSize.ValueInBytes.IsNaN || path.Size.Value.ValueInBytes.IsNaN)

                            _copyProgressRoutine = ___totalBytesTransferred => getCopyProgressResult();

                        else

                            _copyProgressRoutine = ___totalBytesTransferred/*, CopyProgressCallbackReason __copyProgressCallbackReason*/ =>
                                // _Paths.DecrementSize((ulong)___totalBytesTransferred);

                                ProcessDelegates.CommonDelegate.RaiseEvent(new ProcessProgressDelegateParameter(((int)Paths.TotalSize / (int)InitialTotalSize) * 100)) ? CopyProgressResult.Continue : CopyProgressResult.Cancel;

                        return getCopyProgressResult();
                    };

                    CopyProgressResult copyProgressRoutine(long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, uint streamNumber, CopyProgressCallbackReason copyProgressCallbackReason, IntPtr sourceFile, IntPtr destinationFile, IntPtr data) => _copyProgressRoutine(totalBytesTransferred/*, copyProgressCallbackReason*/);

                    result = Shell.CopyFileEx(path.Path, destPath, copyProgressRoutine, IntPtr.Zero, ref cancel, copyFileFlags);
                }

                else

                    result = CreateDirectoryW(destPath, IntPtr.Zero);

                //void reportProgressCommon() => TryReportProgress((int)((_Paths.Count / InitialItemCount) * 100));

                //Action reportProgress = () =>
                //{
                //    if (Paths.TotalSize.ValueInBytes.IsNaN || Paths.TotalSize.ValueInBytes == 0)
                //    {
                //        reportProgress = reportProgressCommon;

                //        reportProgressCommon();
                //    }
                //};

                if (result)
                {
                    //_ = _Paths.Dequeue();

                    //if (path.Size.HasValue)

                    //    reportProgressCommon();

                    //else

                    //    reportProgress();

                    _error = Factory.GetError(ProcessErrorFactoryData.NoError, NoError);

                    _isErrorGlobal = false;

                    return;
                }

                switch ((ErrorCode)Marshal.GetLastWin32Error())
                {
                    // todo: the current version of this process is not optimized: when a file name conflict occurs when we want to create a folder, we know that all the subpaths won't be able to be copied neither. So, we should have a tree structure, so we can dequeue all the path in conflict with all of its subpaths at one time.
                    case ErrorCode.AlreadyExists:

                        if (path.Size.HasValue) // We do not try to rename folders, because folder name conflicts are handled the same way as file name conflicts.
                        {
                            if (alreadyRenamed)
                            {
                                _error = Factory.GetError(ProcessError.FileRenamingFailed, FileRenamingFailed);

                                _isErrorGlobal = false;

                                return;
                            }

                            if (AutoRenameFiles)
                            {
                                renameOnDuplicate();

                                copyFileOrCreateDirectory(out _error, out _isErrorGlobal);

                                return;
                            }
                        }

                        _error = Factory.GetError(ProcessError.FileSystemEntryAlreadyExists, FileSystemEntryAlreadyExists);

                        break;

                    case ErrorCode.PathNotFound:

                        _error = Factory.GetError(ProcessError.PathNotFound, PathNotFound);

                        break;

                    case ErrorCode.AccessDenied:

                        _error = Factory.GetError(ProcessError.ReadProtection, string.Format(ReadProtection, Source));

                        break;

                    case ErrorCode.DiskFull:

                        _error = Factory.GetError(ProcessError.NotEnoughSpace, NotEnoughSpace);

                        break;

                    case ErrorCode.DiskOperationFailed:

                        _error = Factory.GetError(ProcessError.DiskError, DiskError);

                        break;

                    case ErrorCode.EncryptionFailed:

                        _error = Factory.GetError(ProcessError.EncryptionFailed, EncryptionFailed);

                        break;

                    default:

                        _error = Factory.GetError(ProcessErrorFactoryData.UnknownError, UnknownError);

                        break;
                }

                _isErrorGlobal = false;
            }

            alreadyRenamed = false;

            // CurrentPath = _Paths.Peek();

            // sourcePath = $"{SourcePath}{WinCopies.IO.Path.PathSeparator}{CurrentPath.Path}";

            destPath = getDestPath(path);

            if (!path.IsDirectory && Path.Exists(destPath))

                if (AutoRenameFiles)

                    if (_bufferLength == 0)

                        renameOnDuplicate();

                    else
                    {
                        try
                        {
                            using
#if !CS8
                            (
#endif
                                FileStream sourceFileStream = GetFileStream(path.Path, _bufferLength)
#if !CS8
                            )
                            {
#else
                            ;
#endif

                                try
                                {
                                    using
#if !CS8
                                (
#endif
                                    FileStream destFileStream = GetFileStream(destPath, _bufferLength)
#if !CS8
                                )
                                    {
#else
                                ;
#endif

                                        bool? _result;

                                        _result = IsDuplicate(sourceFileStream, destFileStream, _bufferLength, () => ProcessDelegates.CancellationPendingDelegate.RaiseEvent(null));

                                        if (_result.HasValue)

                                            if (_result.Value)

                                                renameOnDuplicate();

                                            else
                                            {
                                                error = new ProcessError<ProcessError>(ProcessErrorFactoryData.NoError, NoError); // _ = _Paths.Dequeue();

                                                isErrorGlobal = false;

                                                return true;
                                            }

                                        else
                                        {
                                            error = new ProcessError<ProcessError>(ProcessError.FileSystemEntryAlreadyExists, FileSystemEntryAlreadyExists);

                                            isErrorGlobal = false;

                                            return true;
                                        }
#if !CS8
                                    }
#endif
                                }

                                catch (System.IO.FileNotFoundException)
                                {
                                    // Left empty because this is a check for duplicate.
                                }

                                catch (Exception ex) when (ex.Is(false, typeof(UnauthorizedAccessException), typeof(SecurityException)))
                                {
                                    error = Factory.GetError(ProcessError.DestinationReadProtection, string.Format(ReadProtection, Destination));
                                }

#if NETFRAMEWORK
                            }
#endif
                        }

                        catch (System.IO.IOException ex) when (ex.Is(false, typeof(System.IO.FileNotFoundException), typeof(System.IO.DirectoryNotFoundException)))
                        {
                            error = Factory.GetError(ProcessError.PathNotFound, PathNotFound);
                        }

                        catch (System.IO.PathTooLongException)
                        {
                            error = Factory.GetError(ProcessError.PathTooLong, PathTooLong);
                        }

                        catch (System.IO.IOException)
                        {
                            error = Factory.GetError(ProcessError.UnknownError, ExceptionMessages.UnknownError);
                        }

                        catch (Exception ex) when (ex.Is(false, typeof(UnauthorizedAccessException), typeof(SecurityException)))
                        {
                            error = Factory.GetError(ProcessError.ReadProtection, string.Format(ReadProtection, Source));
                        }
                    }

                else

                    error = Factory.GetError(ProcessError.FileSystemEntryAlreadyExists, FileSystemEntryAlreadyExists);

            copyFileOrCreateDirectory(out error, out isErrorGlobal);

            isErrorGlobal = false;

            return true;
        }

        protected override bool DoWork(IProcessErrorItem<IPathInfo, ProcessError> path, out IProcessError<ProcessError> error, out bool isErrorGlobal) => DoWork(path, out error, out isErrorGlobal);

        protected override void ResetStatus() { /* Left empty. */ }
        #endregion Methods
    }
}
