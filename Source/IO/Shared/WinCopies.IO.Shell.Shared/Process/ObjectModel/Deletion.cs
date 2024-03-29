﻿/* Copyright © Pierre Sprimont, 2020
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
using Microsoft.WindowsAPICodePack.Win32Native;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.IO.Shell.Process;
using WinCopies.Linq;
using WinCopies.Util.Commands.Primitives;

using static Microsoft.WindowsAPICodePack.COMNative.Shell.ShellOperationFlags;

using static WinCopies.IO.Process.ProcessHelper;
using static WinCopies.IO.Resources.ExceptionMessages;
using static WinCopies.IO.Shell.Process.ProcessHelper;
using static WinCopies.IO.Shell.Resources.ExceptionMessages;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Process.ObjectModel
{
    [ProcessGuid(Consts.Guids.Shell.Process.Shell.Deletion)]
    public class Deletion<T> :
        ProcessObjectModelTypes<
            IPathInfo,
            IPathInfo,
            T,
            ProcessError,
            ErrorAction,
            ProcessDelegateTypes<
                IPathInfo,
                IProcessProgressDelegateParameter>.IProcessDelegates<
                    ProcessDelegateTypes<
                        IPathInfo,
                        IProcessProgressDelegateParameter>.IProcessEventDelegates>,
            ProcessDelegateTypes<
                IPathInfo,
                IProcessProgressDelegateParameter>.IProcessEventDelegates,
            IProcessProgressDelegateParameter>.DefaultProcesses<
                DeletionOptions<
                    IPathInfo>>.DefaultProcess2
        where T : ProcessErrorTypes<IPathInfo, ProcessError, ErrorAction>.IProcessErrorFactories
    {
        private delegate bool Func(IPathInfo path, out IProcessError<ProcessError, ErrorAction> error, out bool isErrorGlobal);

        private FileOperation _fileOperation;
        private Func _func;

        #region Properties
        public override string Guid => Consts.Guids.Shell.Process.Shell.Deletion;

        public override string Name
        {
            get
            {
                switch (Options.RemoveOption)
                {
                    case RemoveOption.Recycle:

                        return Shell.Properties.Resources.Recycling;

                    case RemoveOption.Clear:

                        return Shell.Properties.Resources.EmptyingRecycleBin;
                }

                return Shell.Properties.Resources.Deletion;
            }
        }

        public override System.Collections.Generic.IReadOnlyDictionary<string, ICommand<IProcessErrorItem<IPathInfo, ProcessError, ErrorAction>>> Actions => null;
        #endregion Properties

        public Deletion(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, ErrorAction>.ProcessErrorItem, ErrorAction> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, DeletionOptions<IPathInfo> options, T factory) : base(options.RemoveOption == RemoveOption.Clear && ((IUIntCountable)(initialPaths ?? throw GetArgumentNullException(nameof(initialPaths)))).Count != 0 ? throw new ArgumentException($"The initial paths collection must be empty (not null) when {nameof(options)}.{nameof(DeletionOptions<IPathInfo>.RemoveOption)} is set to {nameof(RemoveOption.Clear)}.") : initialPaths, sourcePath, paths, errorsQueue, progressDelegate, factory, options)
        {
            // Left empty.
        }

        protected override RecursiveEnumerationOrder GetRecursiveEnumerationOrder() => RecursiveEnumerationOrder.ChildrenThenParent;

        protected override Predicate<IPathInfo> GetAddAsDuplicatePredicate() => throw new NotSupportedException();

        protected virtual void RemoveDirectory(IPathInfo path, out IProcessError<ProcessError, ErrorAction> error) => ProcessHelper<IPathInfo, ErrorAction>.RemoveDirectory(path.Path, Factory, out error);

        protected virtual bool Recycle(IPathInfo path, out IProcessError<ProcessError, ErrorAction> error, out bool isErrorGlobal)
        {
            isErrorGlobal = false;

            void setError(in ProcessError processError, in string message, in HResult hr, out IProcessError<ProcessError, ErrorAction> _error) => _error = Factory.GetError(processError, message, hr);

            void setCancelledError(in HResult hr, out IProcessError<ProcessError, ErrorAction> _error) => _error = Factory.GetError(ProcessError.CancelledByUser, hr);

            bool check(in HResult hr, out IProcessError<ProcessError, ErrorAction> _error)
            {
                if (CoreErrorHelper.Succeeded(hr))
                {
                    _error = null;

                    return true;
                }

                switch (hr)
                {
                    case HResult.Abort:
                    case HResult.Canceled:

                        setCancelledError(hr, out _error);

                        break;

                    case HResult.AccessDenied:

                        setError(ProcessError.AccessDenied, AccessDenied, hr, out _error);

                        break;

                    default:

                        setError(Factory.UnknownError, UnknownError, hr, out _error);

                        break;
                }

                return false;
            }

            if (check(_fileOperation.TryDeleteItem(ShellObjectFactory.Create(path.Path.Replace("\\\\", "\\")), provider: null), out error) && check(_fileOperation.TryPerformOperations(), out error))

                if (_fileOperation.GetAnyOperationsAborted())

                    setCancelledError(HResult.Canceled, out error);

                else

                    error = Factory.GetNoErrorError();

            return true;
        }

        protected virtual bool Delete(IPathInfo path, out IProcessError<ProcessError, ErrorAction> error, out bool isErrorGlobal)
        {
            isErrorGlobal = false;

            if (path.IsDirectory)

                RemoveDirectory(path, out error);

            else if (Microsoft.WindowsAPICodePack.Win32Native.Shell.Shell.DeleteFileW(path.Path))

                error = Factory.GetNoErrorError();

            else
            {
                var errorCode = (ErrorCode)Marshal.GetLastWin32Error();

                IProcessError<ProcessError, ErrorAction> getError(in ProcessError processError, in string message) => Factory.GetError(processError, message, errorCode);

                switch (errorCode)
                {
                    case ErrorCode.SharingViolation:

                        string errorMessage;

                        try
                        {
                            List<System.Diagnostics.Process> processes = FileLockFinder.FindFileLock(path.Path);

                            errorMessage = processes.Count == 1
                                ? string.Format(CultureInfo.InvariantCulture, SharingViolationFileOpenInProcess, processes[0].ProcessName)
                                : processes.Count == 0 ? SharingViolation : SharingViolationFileOpenInMultipleProcesses;
                        }

                        catch
                        {
                            errorMessage = SharingViolation;
                        }

                        error = getError(ProcessError.SharingViolation, errorMessage);

                        break;

                    case ErrorCode.FileReadOnly:

                        error = getError(ProcessError.FileReadOnly, FileReadOnly);

                        break;

                    case ErrorCode.AccessDenied:

                        bool fileExists = true;

                        try
                        {
                            fileExists = System.IO.File.Exists(path.Path);
                        }

                        catch { }

                        IProcessError<ProcessError, ErrorAction> _getError()
                        {
                            if (fileExists)

                                return getError(ProcessError.AccessDenied, AccessDenied);

                            errorCode = ErrorCode.FileNotFound;

                            return getError(ProcessError.PathNotFound, PathNotFound);
                        }

                        error = _getError();

                        break;

                    default:

                        error = getError(ProcessError.UnknownError, UnknownError);

                        break;
                }

            }

            return true;
        }

        protected override bool DoWork(IPathInfo path, out IProcessError<ProcessError, ErrorAction> error, out bool isErrorGlobal) => _func(path, out error, out isErrorGlobal);

        protected override bool Check(out IProcessError<ProcessError, ErrorAction> error)
        {
            if (Options.RemoveOption == RemoveOption.Clear)
            {
                error = Factory.GetNoErrorError();

                _func = Delete;

                return true;
            }

            if (DefaultCheck<IPathInfo, ProcessError, ErrorAction, T>(Paths, SourcePath, null, false, Factory, ProcessErrorFactoryData, out error))
            {
                switch (Options.RemoveOption)
                {
                    case RemoveOption.Delete:

                        _func = Delete;

                        break;

                    case RemoveOption.Recycle:

                        _fileOperation = new FileOperation();

                        _fileOperation.SetOperationFlags(RecycleOnDelete | Silent | EarlyFailure | NoErrorUI);

                        _func = Recycle;

                        break;
                }

                return true;
            }

            return false;
        }

        protected override bool DoWork(IProcessErrorItem<IPathInfo, ProcessError, ErrorAction> path, out IProcessError<ProcessError, ErrorAction> error, out bool isErrorGlobal) => DoWork((IPathInfo)path, out error, out isErrorGlobal);

        protected override IProcessProgressDelegateParameter GetNotifyCompletionParameters() => GetDefaultNotifyCompletionParameters();

        protected override bool LoadPathsOverride(out IProcessError<ProcessError, ErrorAction> error, out bool clearOnError)
        {
            switch (Options.RemoveOption)
            {
                case RemoveOption.Recycle:

                    return LoadPathsDirectly(out error, out clearOnError);

                case RemoveOption.Clear:

                    return LoadPaths(KnownFolders.RecycleBin.Select(path => new PathTypes<IPathInfo>.PathInfo(path.ParsingName, null)), out error, out clearOnError);

                case RemoveOption.Delete:

                    return base.LoadPathsOverride(out error, out clearOnError);
            }

            SetCancelErrorParameters(out error, out clearOnError);

            return false;
        }

        protected override void ResetStatus()
        {
            if (_func == null)

                return;

            if (Options.Recycle)
            {
                _fileOperation.Dispose();
                _fileOperation = null;
            }

            _func = null;
        }

        protected override IPathInfo ConvertCommon(IPathInfo path) => path;
    }
}
