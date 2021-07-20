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
using WinCopies.Collections.Generic;
using WinCopies.Util;
using WinCopies.Util.Commands.Primitives;

using static WinCopies.IO.Process.ProcessHelper;
using static WinCopies.IO.File;
using static WinCopies.IO.Resources.ExceptionMessages;
using static WinCopies.ThrowHelper;

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

    public class CopyProcessErrorFactory<T> : ProcessErrorFactory<T, CopyErrorAction> where T : IPathInfo
    {
        public override CopyErrorAction IgnoreAction => CopyErrorAction.Ignore;
    }

    public delegate CopyProgressResult CopyProgressRoutine(long bytesTransferred /*, CopyProgressCallbackReason*/, ref long l, ref long _l);

    public delegate CopyProgressResult CopyProgressRoutine2(in long totalFileSize, in long bytesTransferred);

    public delegate CopyProgressResult CopyProgressRoutine3(long totalFileSize, long bytesTransferred, ref long l, ref long _l);

    public sealed class CopyProgressRoutineStruct<T> : DotNetFix.IDisposable where T : IPath
    {
        private CopyProgressRoutine2 _copyProgressRoutine = (in long totalFileSize, in long bytesTransferred) => throw new InvalidOperationException("The current object is not initialized.");
        private long l;
        private long _l;

        public bool IsDisposed { get; private set; }

        public void Initialize(T path, ProcessTypes<T>.IProcessQueue paths, Func<CopyProgressResult> getCopyProgressResultFunc, CopyProgressRoutine3 copyProgressRoutine)
        {
            if (IsDisposed)

                throw GetExceptionForDispose(false);

            _copyProgressRoutine = (in long totalFileSize, in long bytesTransferred) =>
  {
      if (paths.TotalSize.ValueInBytes.IsNaN || path.Size.Value.ValueInBytes.IsNaN)

          _copyProgressRoutine = (in long _totalFileSize, in long _bytesTransferred) => getCopyProgressResultFunc();

      else

          _copyProgressRoutine = (in long _totalFileSize, in long _bytesTransferred) => copyProgressRoutine(_totalFileSize, _bytesTransferred, ref l, ref _l);

      return getCopyProgressResultFunc();
  };
        }

        public CopyProgressResult CopyProgressRoutine(in long totalFileSize, in long bytesTransferred) => _copyProgressRoutine(totalFileSize, bytesTransferred);

        public void Reset()
        {
            l = 0;
            _l = 0;
        }

        public void Dispose()
        {
            _copyProgressRoutine = null;

            Reset();

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }

        ~CopyProgressRoutineStruct() => Dispose();
    }

    namespace ObjectModel
    {
        [ProcessGuid(Guids.Process.Shell.Copy)]
        public class Copy<T> : ProcessObjectModelTypes<IPathInfo, IPathInfo, T, ProcessError, CopyErrorAction, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates>, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates, IProcessProgressDelegateParameter>.DefaultDestinationProcess where T : ProcessErrorTypes<IPathInfo, ProcessError, CopyErrorAction>.IProcessErrorFactories
        {
            private class Dictionary : Dictionary<string, ICommand<IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction>>>
            {
                public static IReadOnlyDictionary<string, ICommand<IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction>>> Instance { get; } = new Dictionary();

                private Dictionary()
                {
                    void add(in string name, in string description, ProcessError error, CopyErrorAction action) => Add(name, new DelegateCommand<IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction>>(name, description, path => path.Error.Error == error && !path.IsDirectory, _path => _path.Error.Action = action));

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

            private CopyProgressRoutineStruct<IPathInfo> _copyProgressRoutineStruct;
            private FileDoWork _fileDoWork;
            private Func<PathTypes<IPathInfo>.PathInfoBase, string, bool> _directoryDoWork;

            #region Properties
            public override string Guid => Guids.Process.Shell.Copy;

            public override string Name => Options.Move ? Properties.Resources.Move : Properties.Resources.Copy;

            public override IReadOnlyDictionary<string, ICommand<IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction>>> Actions => Dictionary.Instance;

            protected CopyProcessOptions<IPathInfo> Options { get; }

            protected NewPathStruct NewPath { get; set; }
            #endregion Properties

            protected delegate bool FileDoWork(IPathInfo path, string destPath, uint flags);

            public Copy(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, CopyErrorAction>.ProcessErrorItem, CopyErrorAction> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, in CopyProcessOptions<IPathInfo> processOptions, T factory) : base(initialPaths, sourcePath, destinationPath.IsDirectory ? destinationPath : throw new ArgumentException($"{nameof(destinationPath)} must be a directory."), paths, errorsQueue, progressDelegate, factory) => Options = processOptions ?? throw GetArgumentNullException(nameof(processOptions));

            #region Methods
            protected override IPathInfo Convert(in IPathInfo path) => path;

            protected override RecursiveEnumerationOrder GetRecursiveEnumerationOrder() => Options.Move ? RecursiveEnumerationOrder.Both : RecursiveEnumerationOrder.ParentThenChildren;

            protected override Predicate<IPathInfo> GetAddAsDuplicatePredicate() => path => path.IsDirectory;

            protected override bool OnPathLoaded(in IPathInfo path) => ProcessHelper<IPathInfo>.ProcessHelper2<ProcessError, CopyErrorAction, IProcessProgressDelegateParameter, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates>.OnPathLoaded(path, Options, ProcessDelegates, null, AddPath);

            protected override IRecursiveEnumerable<IPathInfo> GetEnumerable(in IPathInfo path) => Options.Move ? new MovingRecursivelyEnumerablePath<IPathInfo>(path, null, null
#if CS8
                , null
#endif
                , FileSystemEntryEnumerationOrder.FilesThenDirectories, _path => _path, true
#if DEBUG
                , null
#endif
                ) : ProcessHelper<IPathInfo>.GetDefaultEnumerable(path, RecursiveEnumerationOrder.ParentThenChildren, _path => _path);

            protected override void GetPathsLoadingErrorParameters(in ProcessError error, in string message, in ErrorCode errorCode, out IProcessError<ProcessError, CopyErrorAction> _error, out bool clearOnError) => GetDefaultPathsLoadingErrorParameters(error, message, errorCode, Options, Factory, out _error, out clearOnError);

            protected override bool Check(out IProcessError<ProcessError, CopyErrorAction> error)
            {
                error = CheckDrivesAndSpace<IPathInfo, CopyErrorAction, T>(Paths, SourcePath, DestinationPath, true, Factory, ProcessErrorFactoryData);

                if (error.Error == ProcessErrorFactoryData.NoError)
                {
                    _copyProgressRoutineStruct = new CopyProgressRoutineStruct<IPathInfo>();

                    bool createDirectory(in string destPath) => Shell.CreateDirectoryW(destPath, IntPtr.Zero);

                    if (Options.Move)
                    {
                        _fileDoWork = (IPathInfo path, string destPath, uint flags) =>
                        {
                            bool result;

                            if (path.AlreadyPushed)

                                result = Shell.RemoveDirectoryW(path.Path);

                            else
                            {
                                var _flags = (MoveFileFlags)flags;

                                result = MoveFile(path, ref destPath, ref _flags);
                            }

                            _copyProgressRoutineStruct.Reset();

                            return result;
                        };

                        _directoryDoWork = (path, destPath) =>
                        {
                            if (createDirectory(destPath))
                            {
                                path.AlreadyPushed = true;

                                return true;
                            }

                            else

                                return false;
                        };
                    }

                    else
                    {
                        _fileDoWork = (IPathInfo path, string destPath, uint flags) =>
                        {
                            var _flags = (CopyFileFlags)flags;

                            bool result = CopyFile(path, ref destPath, ref _flags);

                            _copyProgressRoutineStruct.Reset();

                            return result;

                        };

                        _directoryDoWork = (path, destPath) => createDirectory(destPath);
                    }

                    return true;
                }

                else return false;
            }

            protected virtual void RenameOnDuplicate(in IPathInfo path, ref string destPath, out bool alreadyRenamed)
            {
                ThrowIfNull(path, nameof(path));

                string oldPath = destPath;

                destPath = Path.RenameDuplicate(destPath);

                NewPath = new NewPathStruct(path.Path + WinCopies.IO.Path.PathSeparator, oldPath, destPath);

                alreadyRenamed = true;
            }

            protected virtual CopyProgressResult GetCopyProgressResult() => ProcessDelegates.CancellationPendingDelegate.RaiseEvent(null) ? CopyProgressResult.Cancel : CopyProgressResult.Continue;

            protected virtual CopyProgressResult CopyProgressRoutine(long totalFileSize, long bytesTransferred /*, CopyProgressCallbackReason*/, ref long l, ref long _l) // => _bytesTransferred/*, CopyProgressCallbackReason __copyProgressCallbackReason*/ =>
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

            protected virtual Microsoft.WindowsAPICodePack.Win32Native.Shell.CopyProgressRoutine GetDefaultProgressRoutine(IPathInfo path)
            {
                _copyProgressRoutineStruct.Initialize(path, Paths, GetCopyProgressResult, CopyProgressRoutine);

                return (long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, uint streamNumber, CopyProgressCallbackReason copyProgressCallbackReason, IntPtr sourceFile, IntPtr destinationFile, IntPtr data) => _copyProgressRoutineStruct.CopyProgressRoutine(totalFileSize, totalBytesTransferred/*, copyProgressCallbackReason*/);
            }

            protected virtual bool CopyFile(IPathInfo path, ref string destPath, ref CopyFileFlags copyFileFlags)
            {
                ThrowIfNull(path, nameof(path));

                copyFileFlags |= CopyFileFlags.NoBuffering;

                bool cancel = false;

                if (path.Path.EndsWith(".lnk", true, CultureInfo.InvariantCulture))

                    copyFileFlags |= CopyFileFlags.CopySymLink;

                return Shell.CopyFileEx(path.Path, destPath, GetDefaultProgressRoutine(path), IntPtr.Zero, ref cancel, copyFileFlags);
            }

            protected virtual bool MoveFile(IPathInfo path, ref string destPath, ref MoveFileFlags moveFileFlags)
            {
                if (System.IO.Path.GetPathRoot((path ?? throw GetArgumentNullException(nameof(path))).Path) != System.IO.Path.GetPathRoot(destPath))

                    moveFileFlags |= MoveFileFlags.CopyAllowed | MoveFileFlags.WriteThrough;

                return Shell.MoveFileWithProgressW(path.Path, destPath, GetDefaultProgressRoutine(path), IntPtr.Zero, moveFileFlags);
            }

            protected virtual void CopyFileOrCreateDirectory(IPathInfo path, ref string destPath, ref bool alreadyRenamed, ref uint flags, out IProcessError<ProcessError, CopyErrorAction> _error, out bool _isErrorGlobal)
            {
                bool result = (path ?? throw GetArgumentNullException(nameof(path))).Size.HasValue || path.AlreadyPushed ? _fileDoWork(path, destPath, flags) : _directoryDoWork(path is PathTypes<IPathInfo>.PathInfoBase _path ? _path : (PathTypes<IPathInfo>.PathInfoBase)((IProcessErrorItem)path).Item, destPath);

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

                if (path.IsDirectory && Options.IgnoreFolderFileNameConflicts && e == ErrorCode.AlreadyExists)
                {
                    setError(out _error, out _isErrorGlobal);

                    return;
                }

                _error = ProcessHelper<IPathInfo, CopyErrorAction>.GetCopyError(Factory, e);

                if (path.Size.HasValue) // We do not try to rename folders, because folder name conflicts are handled the same way as file name conflicts.
                {
                    if (alreadyRenamed)
                    {
                        _error = Factory.GetError(ProcessError.FileRenamingFailed, FileRenamingFailed, e);

                        _isErrorGlobal = false;

                        return;
                    }

                    if (Options.AutoRenameFiles)
                    {
                        RenameOnDuplicate(path, ref destPath, out alreadyRenamed);

                        if (!Options.Move)

                            flags = (uint)GetDefaultCopyFileFlags();

                        CopyFileOrCreateDirectory(path, ref destPath, ref alreadyRenamed, ref flags, out _error, out _isErrorGlobal);

                        return;
                    }
                }

                _isErrorGlobal = false;
            }

            protected virtual bool Parse(in IPathInfo path, ref string destPath, ref bool alreadyRenamed, out IProcessError<ProcessError, CopyErrorAction> error, out bool isErrorGlobal)
            {
                FileStream sourceFileStream = ProcessHelper<IPathInfo, CopyErrorAction>.TryGetFileStream(Factory, (path ?? throw GetArgumentNullException(nameof(path))).Path, Options.BufferLength, out error);

                if (sourceFileStream != null)

                    using (sourceFileStream)
                    {
                        try
                        {
                            using
#if !CS8
                                (
#endif
                                    FileStream destFileStream = GetFileStream(destPath, Options.BufferLength)
#if !CS8
                                )
                            {
#else
                                ;
#endif

                            bool? _result;

                            _result = IsDuplicate(sourceFileStream, destFileStream, Options.BufferLength, () => ProcessDelegates.CancellationPendingDelegate.RaiseEvent(null));

                            if (_result.HasValue)

                                if (_result.Value)

                                    RenameOnDuplicate(path, ref destPath, out alreadyRenamed);

                                else
                                {
                                    error = new ProcessError<ProcessError, CopyErrorAction>(ProcessErrorFactoryData.NoError, NoError, ErrorCode.NoError); // _ = _Paths.Dequeue();

                                    isErrorGlobal = false;

                                    return true;
                                }

                            else
                            {
                                error = new ProcessError<ProcessError, CopyErrorAction>(ProcessError.FileSystemEntryAlreadyExists, FileSystemEntryAlreadyExists, ErrorCode.AlreadyExists);

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
                            error = Factory.GetError(ProcessError.DestinationReadProtection, string.Format(CultureInfo.InvariantCulture, ReadProtection, Destination), ErrorCode.ReadFault);
                        }
                    }

                isErrorGlobal = false;

                return false;
            }

            protected override bool DoWork(IPathInfo path, out IProcessError<ProcessError, CopyErrorAction> error, out bool isErrorGlobal)
            {
                ThrowIfNull(path, nameof(path));

                string destPath;
                bool alreadyRenamed;

                alreadyRenamed = false;

                // CurrentPath = _Paths.Peek();

                // sourcePath = $"{SourcePath}{WinCopies.IO.Path.PathSeparator}{CurrentPath.Path}";

                destPath = GetDestinationPath(path);

                uint flags = Options.Move ? 0u : (uint)GetDefaultCopyFileFlags();

                if (path is IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction> _path && _path.Error != null)
                {
#if CS8
                    static
#endif
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

                            if (Options.Move)

                                flags |= (uint)MoveFileFlags.ReplaceExisting;

                            else

                                flags ^= (uint)CopyFileFlags.FailIfExists;

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

                    if (Options.AutoRenameFiles)

                        if (Options.BufferLength == 0)

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

                CopyFileOrCreateDirectory(path, ref destPath, ref alreadyRenamed, ref flags, out error, out isErrorGlobal);

                return true;
            }

            protected override bool DoWork(IProcessErrorItem<IPathInfo, ProcessError, CopyErrorAction> path, out IProcessError<ProcessError, CopyErrorAction> error, out bool isErrorGlobal) => DoWork((IPathInfo)path, out error, out isErrorGlobal);

            protected override IProcessProgressDelegateParameter GetNotifyCompletionParameters() => GetDefaultNotifyCompletionParameters();

            protected override void ResetStatus()
            {
                NewPath = new NewPathStruct();

                if (_copyProgressRoutineStruct != null)
                {
                    _copyProgressRoutineStruct.Dispose();
                    _copyProgressRoutineStruct = null;
                }

                _fileDoWork = null;
                _directoryDoWork = null;
            }
            #endregion Methods
        }
    }
}
