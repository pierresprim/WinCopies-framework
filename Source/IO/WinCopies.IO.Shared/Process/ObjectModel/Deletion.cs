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
using System.Globalization;
using System.Runtime.InteropServices;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Util.Commands.Primitives;

using static WinCopies.IO.Process.ProcessHelper;
using static WinCopies.IO.Resources.ExceptionMessages;

namespace WinCopies.IO.Process.ObjectModel
{
    [ProcessGuid(Guids.Process.Shell.Deletion)]
    public class Deletion<T> : ProcessObjectModelTypes<IPathInfo, IPathInfo, T, ProcessError, ErrorAction, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates>, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates, IProcessProgressDelegateParameter>.Process where T : ProcessErrorTypes<IPathInfo, ProcessError, ErrorAction>.IProcessErrorFactories
    {
        #region Properties
        public override string Guid => Guids.Process.Shell.Deletion;

        public override string Name => Properties.Resources.Deletion;

        public ProcessOptions<IPathInfo> Options { get; }

        public override IReadOnlyDictionary<string, ICommand<IProcessErrorItem<IPathInfo, ProcessError, ErrorAction>>> Actions => null;

        public override IPathInfo SourcePath { get; }
        #endregion Properties

        public Deletion(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, ErrorAction>.ProcessErrorItem, ErrorAction> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, ProcessOptions<IPathInfo> options, T factory) : base(initialPaths, paths, errorsQueue, progressDelegate, factory)
        {
            SourcePath = sourcePath;

            Options = options;
        }

        protected override IRecursiveEnumerable<IPathInfo> GetEnumerable(in IPathInfo path) => GetDefaultEnumerable(path, RecursiveEnumerationOrder.ChildrenThenParent);

        protected override RecursiveEnumerationOrder GetRecursiveEnumerationOrder() => RecursiveEnumerationOrder.ChildrenThenParent;

        protected override Predicate<IPathInfo> GetAddAsDuplicatePredicate() => throw new NotSupportedException();

        protected virtual void RemoveDirectory(IPathInfo path, out IProcessError<ProcessError, ErrorAction> error) => ProcessHelper<IPathInfo, ErrorAction>.RemoveDirectory(path.Path, Factory, out error);

        protected override bool DoWork(IPathInfo path, out IProcessError<ProcessError, ErrorAction> error, out bool isErrorGlobal)
        {
            isErrorGlobal = false;

            IProcessError<ProcessError, ErrorAction> getError(in ProcessError processError, in string message, in ErrorCode errorCode) => Factory.GetError(processError, message, errorCode);

            if (path.IsDirectory)

                RemoveDirectory(path, out error);

            else if (Shell.DeleteFileW(path.Path))

                error = Factory.GetNoErrorError();

            else
            {
                var errorCode = (ErrorCode)Marshal.GetLastWin32Error();

                switch (errorCode)
                {
                    case ErrorCode.SharingViolation:

                        string errorMessage;

                        try
                        {
                            System.Collections.Generic.List<System.Diagnostics.Process> processes = Microsoft.WindowsAPICodePack.Shell.FileLockFinder.FindFileLock(path.Path);

                            errorMessage = processes.Count == 1
                                ? string.Format(CultureInfo.InvariantCulture, SharingViolationFileOpenInProcess, processes[0].ProcessName)
                                : processes.Count == 0 ? SharingViolation : SharingViolationFileOpenInMultipleProcesses;
                        }

                        catch
                        {
                            errorMessage = SharingViolation;
                        }

                        error = getError(ProcessError.SharingViolation, errorMessage, errorCode);

                        break;

                    case ErrorCode.FileReadOnly:

                        error = getError(ProcessError.FileReadOnly, FileReadOnly, errorCode);

                        break;

                    case ErrorCode.AccessDenied:

                        bool fileExists = true;

                        try
                        {
                            fileExists = System.IO.File.Exists(path.Path);
                        }

                        catch { }

                        error = fileExists
                            ? getError(ProcessError.AccessDenied, AccessDenied, errorCode)
                            : getError(ProcessError.PathNotFound, PathNotFound, ErrorCode.FileNotFound);

                        break;

                    default:

                        error = getError(ProcessError.UnknownError, UnknownError, errorCode);

                        break;
                }

            }

            return true;
        }

        protected override bool Check(out IProcessError<ProcessError, ErrorAction> error) => DefaultCheck<IPathInfo, ProcessError, ErrorAction, T>(Paths, SourcePath, null, false, Factory, ProcessErrorFactoryData, out error);

        protected override void GetPathsLoadingErrorParameters(in ProcessError error, in string message, in ErrorCode errorCode, out IProcessError<ProcessError, ErrorAction> _error, out bool clearOnError) => GetDefaultPathsLoadingErrorParameters(error, message, errorCode, Options, Factory, out _error, out clearOnError);

        protected override bool OnPathLoaded(in IPathInfo path) => ProcessHelper<IPathInfo>.ProcessHelper2<ProcessError, ErrorAction, IProcessProgressDelegateParameter, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates>.OnPathLoaded(path, Options, ProcessDelegates, null, AddPath);

        protected override bool DoWork(IProcessErrorItem<IPathInfo, ProcessError, ErrorAction> path, out IProcessError<ProcessError, ErrorAction> error, out bool isErrorGlobal) => DoWork((IPathInfo)path, out error, out isErrorGlobal);

        protected override IProcessProgressDelegateParameter GetNotifyCompletionParameters() => GetDefaultNotifyCompletionParameters();

        protected override void ResetStatus() { }

        protected override IPathInfo Convert(in IPathInfo path) => path;
    }
}
