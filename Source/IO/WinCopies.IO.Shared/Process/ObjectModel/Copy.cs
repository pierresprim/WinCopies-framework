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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Util;
using WinCopies.Util.Commands;
using WinCopies.Util.Commands.Primitives;
using static WinCopies.IO.File;
using static WinCopies.IO.Resources.ExceptionMessages;

namespace WinCopies.IO.Process
{
    public enum CopyErrorAction
    {
        None = ErrorAction.None,

        Ignore = ErrorAction.Ignore,

        Rename = 2,

        Replace = 3,

        Parse = 4
    }

    public class CopyProcessErrorFactory : ProcessErrorFactory<IPathInfo, CopyErrorAction>
    {
        public override CopyErrorAction IgnoreAction => CopyErrorAction.Ignore;
    }
}

namespace WinCopies.IO.Process.ObjectModel
{
    [ProcessGuid(Guids.Process.Shell.Copy)]
    public class CopyProcess<T> : ProcessObjectModelTypes<IPathInfo, IPathInfo, T, ProcessError, CopyErrorAction, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates>, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates, IProcessProgressDelegateParameter>.DefaultDestinationProcess where T : ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError, CopyErrorAction>.IProcessErrorFactories
    {
        private class Dictionary : Dictionary<string, ICommand<IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction>>>
        {
            public static IReadOnlyDictionary<string, ICommand<IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction>>> Instance { get; } = new Dictionary();

            private Dictionary()
            {
                void add(in string name, in string description, ProcessError error, CopyErrorAction action) => Add(name, new DelegateCommand<IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction>>(name, description, path => path.Error.Error == error, _path => _path.Error.Action = action));

                add("Rename", "Renames the selected paths and copies them.", ProcessError.FileSystemEntryAlreadyExists, CopyErrorAction.Rename);

                add("Replace", "Replaces the paths on disk by the pending paths for all selected paths.", ProcessError.FileSystemEntryAlreadyExists, CopyErrorAction.Replace);

                add("Parse", "Reads the paths on disk and compares them with pending paths for all selected paths. For each pair, if paths are equal, no action is performed else, renames the selected path of the pair and copies it.", ProcessError.FileSystemEntryAlreadyExists, CopyErrorAction.Parse);
            }
        }

        protected struct NewPathStruct
        {
            public string OldSourcePath { get; }

            public string OldDestinationPath { get; }

            public string NewPath { get; }

            public NewPathStruct(in string oldSourcePath, in string oldDestinationPath, in string newPath)
            {
                OldSourcePath = oldSourcePath;

                OldDestinationPath = oldDestinationPath;

                NewPath = newPath;
            }
        }

        #region Fields
        private int _bufferLength;
        #endregion

        #region Properties
        public override string Guid => Guids.Process.Shell.Copy;

        public override string Name => Properties.Resources.Copy;

        public override IReadOnlyDictionary<string, ICommand<IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction>>> Actions => Dictionary.Instance;

        protected ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError, CopyErrorAction>.ProcessOptions Options { get; }

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

        protected NewPathStruct NewPath { get; set; }

        public bool IgnoreFolderFileNameConflicts { get; set; }
        #endregion Properties

        public CopyProcess(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, CopyErrorAction>.ProcessErrorItem, CopyErrorAction> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, in ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError, CopyErrorAction>.ProcessOptions processOptions, T factory) : base(initialPaths, sourcePath, destinationPath.IsDirectory ? destinationPath : throw new ArgumentException($"{nameof(destinationPath)} must be a directory."), paths, errorsQueue, progressDelegate, factory) => Options = processOptions;

        #region Methods
        #region Checks
        protected IProcessError<ProcessError, CopyErrorAction> CheckDrivesAndSpace()
        {
            if (Paths.TotalSize.ValueInBytes.IsNaN)

                return Factory.GetError(ProcessError.NotEnoughSpace, NotEnoughSpace, ErrorCode.DiskFull);

            string drive = System.IO.Path.GetPathRoot(SourcePath.Path);

            if (System.IO.Directory.Exists(drive))
            {
                var driveInfo = new DriveInfo(drive);

                if ((driveInfo.IsReady && drive == (drive = System.IO.Path.GetPathRoot(DestinationPath.Path))) || (System.IO.Directory.Exists(drive) && new DriveInfo(drive).IsReady))

                    return driveInfo.TotalFreeSpace >= Paths.TotalSize.ValueInBytes ? new ProcessError<ProcessError, CopyErrorAction>(ProcessErrorFactoryData.NoError, NoError, ErrorCode.NoError) : new ProcessError<ProcessError, CopyErrorAction>(ProcessError.NotEnoughSpace, NotEnoughSpace, ErrorCode.DiskFull);
            }

            return Factory.GetError(ProcessError.DriveNotReady, DriveNotReady, ErrorCode.NotReady);
        }
        #endregion

        protected virtual bool OnPathLoaded(in IPathInfo path) => ProcessHelper.OnPathLoaded(path, Options, ProcessDelegates, null, AddPath);

        protected override bool LoadPathsOverride(out IProcessError<ProcessError, CopyErrorAction> error, out bool clearOnError)
        {
            clearOnError = true;

            void setParameters(out IProcessError<ProcessError, CopyErrorAction> _error, out bool _clearOnError)
            {
                _error = Factory.GetError(ProcessError.CancelledByUser, CancelledByUser, ErrorCode.Cancelled);

                _clearOnError = Options.ClearOnError;
            }

            foreach (IPathInfo path in InitialPaths)
            {
                if (OnPathLoaded(path))

                    foreach (IPathInfo _path in new RecursivelyEnumerablePath<IPathInfo>(InitialPaths.Peek(), null, null
#if CS8
                    , null
#endif
                    , FileSystemEntryEnumerationOrder.DirectoriesThenFiles, __path => __path, true
#if DEBUG
                    , null
#endif
                    ))
                    {
                        if (OnPathLoaded(_path))

                            continue;

                        setParameters(out error, out clearOnError);

                        return false;
                    }

                else

                    setParameters(out error, out clearOnError);
            }

            error = Factory.GetError(Factory.NoError, NoError, ErrorCode.NoError);

            clearOnError = false;

            return true;
        }

        protected override bool Check(out IProcessError<ProcessError, CopyErrorAction> error)
        {
            error = CheckDrivesAndSpace();

            return error.Error == ProcessErrorFactoryData.NoError;
        }

        protected virtual void RenameOnDuplicate(in IPathInfo path, ref string destPath, out bool alreadyRenamed)
        {
            string oldPath = destPath;

            destPath = Path.RenameDuplicate(destPath);

            NewPath = new NewPathStruct(path.Path + '\\', oldPath, destPath);

            alreadyRenamed = true;
        }

        protected virtual CopyProgressResult GetCopyProgressResult() => ProcessDelegates.CancellationPendingDelegate.RaiseEvent(null) ? CopyProgressResult.Cancel : CopyProgressResult.Continue;

        protected delegate CopyProgressResult Func(long bytesTransferred /*, CopyProgressCallbackReason*/, ref long l, ref long _l);

        protected delegate CopyProgressResult Func2(in long totalFileSize, in long bytesTransferred);

        protected virtual CopyProgressResult CopyProgressRoutine(in long totalFileSize, long bytesTransferred /*, CopyProgressCallbackReason*/, ref long l, ref long _l) // => _bytesTransferred/*, CopyProgressCallbackReason __copyProgressCallbackReason*/ =>
        {
            _l = bytesTransferred;

            bytesTransferred -= l;

            l = _l;

            DecrementActualRemainingSize(new Size((ulong)bytesTransferred));

            // _Paths.DecrementSize((ulong)___totalBytesTransferred);

            float f = (float)((float)ActualRemainingSize / (float)InitialTotalSize);

            float _f = (float)((float)l / (float)totalFileSize);

            return ProcessDelegates.CommonDelegate.RaiseEvent(new ProcessProgressDelegateParameter((uint)(100 - (f * 100)), (uint)(_f * 100))) ? CopyProgressResult.Continue : CopyProgressResult.Cancel;
        }

        protected virtual CopyFileFlags GetDefaultCopyFileFlags() => CopyFileFlags.FailIfExists;

        protected virtual void CopyFileOrCreateDirectory(IPathInfo path, ref string destPath, ref bool alreadyRenamed, ref CopyFileFlags copyFileFlags, out IProcessError<ProcessError, CopyErrorAction> _error, out bool _isErrorGlobal)
        {
            bool cancel = false;
            bool result;

            if (path.Size.HasValue)
            {
                copyFileFlags |= CopyFileFlags.NoBuffering;

                if (path.Path.EndsWith(".lnk", true, CultureInfo.InvariantCulture))

                    copyFileFlags |= CopyFileFlags.CopySymLink;

                long l = 0;
                long _l = 0;

                Func2 _copyProgressRoutine = (in long totalFileSize, in long bytesTransferred) =>
                {
                    if (Paths.TotalSize.ValueInBytes.IsNaN || path.Size.Value.ValueInBytes.IsNaN)

                        _copyProgressRoutine = (in long _totalFileSize, in long _bytesTransferred) => GetCopyProgressResult();

                    else

                        _copyProgressRoutine = (in long _totalFileSize, in long _bytesTransferred) => CopyProgressRoutine(_totalFileSize, _bytesTransferred, ref l, ref _l);

                    return GetCopyProgressResult();
                };

                CopyProgressResult copyProgressRoutine(long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, uint streamNumber, CopyProgressCallbackReason copyProgressCallbackReason, IntPtr sourceFile, IntPtr destinationFile, IntPtr data) => _copyProgressRoutine( totalFileSize, totalBytesTransferred/*, copyProgressCallbackReason*/);

                result = Shell.CopyFileEx(path.Path, destPath, copyProgressRoutine, IntPtr.Zero, ref cancel, copyFileFlags);
            }

            else

                result = Shell.CreateDirectoryW(destPath, IntPtr.Zero);

            //void reportProgressCommon() => TryReportProgress((int)((_Paths.Count / InitialItemCount) * 100));

            //Action reportProgress = () =>
            //{
            //    if (Paths.TotalSize.ValueInBytes.IsNaN || Paths.TotalSize.ValueInBytes == 0)
            //    {
            //        reportProgress = reportProgressCommon;

            //        reportProgressCommon();
            //    }
            //};

            void setError(out IProcessError<ProcessError, CopyErrorAction> __error, out bool __isErrorGlobal)
            {
                __error = Factory.GetNoErrorError();

                __isErrorGlobal = false;
            }

            if (result)
            {
                setError(out _error, out _isErrorGlobal);

                //_ = _Paths.Dequeue();

                //if (path.Size.HasValue)

                //    reportProgressCommon();

                //else

                //    reportProgress();

                return;
            }

            var e = (ErrorCode)Marshal.GetLastWin32Error();

            if (path.IsDirectory && IgnoreFolderFileNameConflicts && e == ErrorCode.AlreadyExists)
            {
                setError(out _error, out _isErrorGlobal);

                return;
            }

            _error = ProcessHelper.GetIOError(Factory, e);

            if (path.Size.HasValue) // We do not try to rename folders, because folder name conflicts are handled the same way as file name conflicts.
            {
                if (alreadyRenamed)
                {
                    _error = Factory.GetError(ProcessError.FileRenamingFailed, FileRenamingFailed, e);

                    _isErrorGlobal = false;

                    return;
                }

                if (AutoRenameFiles)
                {
                    RenameOnDuplicate(path, ref destPath, out alreadyRenamed);

                    copyFileFlags = GetDefaultCopyFileFlags();

                    CopyFileOrCreateDirectory(path, ref destPath, ref alreadyRenamed, ref copyFileFlags, out _error, out _isErrorGlobal);

                    return;
                }
            }

            _isErrorGlobal = false;
        }

        protected virtual bool Parse(in IPathInfo path, ref string destPath, ref bool alreadyRenamed, out IProcessError<ProcessError, CopyErrorAction> _error, out bool _isErrorGlobal)
        {
            FileStream sourceFileStream = ProcessHelper.TryGetFileStream(Factory, path.Path, _bufferLength, out _error);

            if (sourceFileStream != null)

                using (sourceFileStream)
                {
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

                                RenameOnDuplicate(path, ref destPath, out alreadyRenamed);

                            else
                            {
                                _error = new ProcessError<ProcessError, CopyErrorAction>(ProcessErrorFactoryData.NoError, NoError, ErrorCode.NoError); // _ = _Paths.Dequeue();

                                _isErrorGlobal = false;

                                return true;
                            }

                        else
                        {
                            _error = new ProcessError<ProcessError, CopyErrorAction>(ProcessError.FileSystemEntryAlreadyExists, FileSystemEntryAlreadyExists, ErrorCode.AlreadyExists);

                            _isErrorGlobal = false;

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
                        _error = Factory.GetError(ProcessError.DestinationReadProtection, string.Format(ReadProtection, Destination), ErrorCode.ReadFault);
                    }
                }

            _isErrorGlobal = false;

            return false;
        }

        protected override bool DoWork(IPathInfo path, out IProcessError<ProcessError, CopyErrorAction> error, out bool isErrorGlobal)
        {
            string destPath;
            bool alreadyRenamed;

            alreadyRenamed = false;

            // CurrentPath = _Paths.Peek();

            // sourcePath = $"{SourcePath}{WinCopies.IO.Path.PathSeparator}{CurrentPath.Path}";

            destPath = GetDestinationPath(path);

            CopyFileFlags _copyFileFlags = GetDefaultCopyFileFlags();

            if (path is IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction> _path && _path.Error != null)
            {
                void setError(out bool _isErrorGlobal) => _isErrorGlobal = false;

                switch (_path.Error.Action)
                {
                    case CopyErrorAction.Ignore:

                        setError(out isErrorGlobal);

                        error = Factory.GetNoErrorError();

                        return true;

                    case CopyErrorAction.Rename:

                        RenameOnDuplicate(path, ref destPath, out alreadyRenamed);

                        break;

                    case CopyErrorAction.Replace:

                        _copyFileFlags ^= CopyFileFlags.FailIfExists;

                        break;

                    case CopyErrorAction.Parse:

                        return Parse(path, ref destPath, ref alreadyRenamed, out error, out isErrorGlobal);

                    case CopyErrorAction.None:

                        setError(out isErrorGlobal);

                        error = _path.Error;

                        return true;
                }
            }

            else if (NewPath.OldSourcePath != null)

                if (path.Path.StartsWith(NewPath.OldSourcePath, StringComparison.CurrentCultureIgnoreCase))

                    destPath = NewPath.NewPath + destPath.Substring(NewPath.OldDestinationPath.Length);

                else

                    NewPath = new NewPathStruct();

            if (!path.IsDirectory && Path.Exists(destPath))

                if (AutoRenameFiles)

                    if (_bufferLength == 0)

                        RenameOnDuplicate(path, ref destPath, out alreadyRenamed);

                    else
                    {
                        if (Parse(path, ref destPath, ref alreadyRenamed, out error, out isErrorGlobal))

                            return true;
                    }

                else
                {
                    error = Factory.GetError(ProcessError.FileSystemEntryAlreadyExists, FileSystemEntryAlreadyExists, ErrorCode.AlreadyExists);

                    isErrorGlobal = false;

                    return true;
                }

            CopyFileOrCreateDirectory(path, ref destPath, ref alreadyRenamed, ref _copyFileFlags, out error, out isErrorGlobal);

            return true;
        }

        protected override bool DoWork(IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction> path, out IProcessError<ProcessError, CopyErrorAction> error, out bool isErrorGlobal) => DoWork((IPathInfo)path, out error, out isErrorGlobal);

        protected override IProcessProgressDelegateParameter GetNotifyCompletionParameters() => new ProcessProgressDelegateParameter(100u, null);

        protected override void ResetStatus() => NewPath = new NewPathStruct();
        #endregion Methods
    }
}
