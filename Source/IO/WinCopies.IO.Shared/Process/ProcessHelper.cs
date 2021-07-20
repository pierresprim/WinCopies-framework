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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Util;

using static WinCopies.IO.Resources.ExceptionMessages;

namespace WinCopies.IO.Process
{
    public static class ProcessHelper
    {
        public static IProcessProgressDelegateParameter GetDefaultNotifyCompletionParameters() => new ProcessProgressDelegateParameter(100u, null);

        public static IRecursiveEnumerable<IPathInfo> GetDefaultEnumerable(in IPathInfo path, in RecursiveEnumerationOrder recursiveEnumerationOrder) => ProcessHelper<IPathInfo>.GetDefaultEnumerable(path, recursiveEnumerationOrder, __path => __path);

        public static string GetDestinationPath(in IPathInfo x, in IPathInfo y) => $"{x.Path}{Path.PathSeparator}{y.GetPath(true)}";

        public static IProcessError<ProcessError, TErrorAction> CheckDrivesAndSpace<TPath, TErrorAction, TFactory>(in ProcessTypes<TPath>.IProcessQueue paths, in IPathCommon sourcePath, in IPathCommon destPath, in bool checkSize, TFactory factory, in IProcessErrorFactoryData<ProcessError> processErrorFactoryData) where TPath : IPath where TFactory : ProcessErrorTypes<TPath, ProcessError, TErrorAction>.IProcessErrorFactories
        {
            if (paths.TotalSize.ValueInBytes.IsNaN)

                return factory.GetError(ProcessError.NotEnoughSpace, NotEnoughSpace, ErrorCode.DiskFull);

            string drive = System.IO.Path.GetPathRoot(sourcePath.Path);

            IProcessError<ProcessError, TErrorAction> getError() => factory.GetError(ProcessError.DriveNotReady, DriveNotReady, ErrorCode.NotReady);

            if (System.IO.Directory.Exists(drive))
            {
                var driveInfo = new DriveInfo(drive);

                if (driveInfo.IsReady)

                    if (destPath != null)
                    {
                        string destPathDrive = System.IO.Path.GetPathRoot(destPath.Path);

                        if (!(drive == destPathDrive || (System.IO.Directory.Exists(destPathDrive) && new DriveInfo(destPathDrive).IsReady)))

                            return getError();
                    }

                return checkSize && driveInfo.TotalFreeSpace < paths.TotalSize.ValueInBytes ? new ProcessError<ProcessError, TErrorAction>(ProcessError.NotEnoughSpace, NotEnoughSpace, ErrorCode.DiskFull) : new ProcessError<ProcessError, TErrorAction>(processErrorFactoryData.NoError, NoError, ErrorCode.NoError);
            }

            return getError();
        }

        public static bool DefaultCheck<TPath, TError, TErrorAction, TFactory>(in ProcessTypes<TPath>.IProcessQueue paths, in IPathCommon sourcePath, in IPathCommon destPath, in bool checkSize, in TFactory factory, in IProcessErrorFactoryData<ProcessError> processErrorFactoryData, out IProcessError<ProcessError, TErrorAction> error) where TPath : IPath where TFactory : ProcessErrorTypes<TPath, ProcessError, TErrorAction>.IProcessErrorFactories
        {
            error = CheckDrivesAndSpace<TPath, TErrorAction, TFactory>(paths, sourcePath, destPath, checkSize, factory, processErrorFactoryData);

            return error.Error == processErrorFactoryData.NoError;
        }

        public static void GetDefaultPathsLoadingErrorParameters<TPath, TError, TErrorAction, TFactory>(in TError error, in string message, in ErrorCode errorCode, in ProcessOptions<TPath> options, in TFactory factory, out IProcessError<TError, TErrorAction> _error, out bool clearOnError) where TPath : IPath where TFactory : ProcessErrorTypes<TPath, TError, TErrorAction>.IProcessErrorFactories
        {
            _error = factory.GetError(error, message, errorCode);

            clearOnError = options.ClearOnError;
        }
    }

    public static class ProcessHelper<T> where T : IPathInfo
    {
        public static ProcessDelegateTypes<T, IProcessProgressDelegateParameter>.ProcessDelegates GetDefaultProcessDelegates() => new ProcessDelegateTypes<T, IProcessProgressDelegateParameter>
                                .ProcessDelegates(
                                    new EventDelegate<T>(),
                                    EventAndQueryDelegate<bool>.GetANDALSO_Delegate(true),
                                    EventAndQueryDelegate<object>.GetANDALSO_Delegate(false),
                                    EventAndQueryDelegate<IProcessProgressDelegateParameter>.GetANDALSO_Delegate(true));

        public static EnumerableQueueCollection<T> GetInitialPaths(System.Collections.Generic.IEnumerator<string> enumerator, IPathInfo sourcePath, Converter<PathTypes<IPathInfo>.PathInfoBase, T> converter) => new Collections.DotNetFix.Generic.EnumerableQueueCollection<T>(new Collections.DotNetFix.Generic.QueueCollection<IEnumerableQueue<T>, T>(
                new Enumerable<string>(
                    () => enumerator
                    ).Select(
                        path => path.EndsWith(":\\") || path.EndsWith(":\\\\")
                                ? converter(new PathTypes<IPathInfo>.RootPath(path, true))
                                : converter(new PathTypes<IPathInfo>.PathInfo(System.IO.Path.GetFileName(path), sourcePath)))
                    .ToEnumerableQueue()));

        public static IRecursiveEnumerable<T> GetDefaultEnumerable(in T path, in RecursiveEnumerationOrder recursiveEnumerationOrder, in Func<PathTypes<IPathInfo>.PathInfo, T> func) => new RecursivelyEnumerablePath<T>(path, null, null
#if CS8
                    , null
#endif
                    , FileSystemEntryEnumerationOrder.FilesThenDirectories, recursiveEnumerationOrder, func, true
#if DEBUG
                    , null
#endif
                    );

        public static class ProcessHelper2<TError, TAction, TProcessDelegateParam, TProcessEventDelegates> where TProcessEventDelegates : ProcessDelegateTypes<T, TProcessDelegateParam>.IProcessEventDelegates where TProcessDelegateParam : IProcessProgressDelegateParameter
        {
            public static bool OnPathLoaded(in T path, in ProcessOptions<T> options, in ProcessDelegateTypes<T, TProcessDelegateParam>.IProcessDelegates<TProcessEventDelegates> processDelegates, in object cancellationPendingDelegateParam, in Action<T> action)
            {
                if ((options ?? throw ThrowHelper.GetArgumentNullException(nameof(options))).PathLoadedDelegate(path) && !processDelegates.CancellationPendingDelegate.RaiseEvent(cancellationPendingDelegateParam))
                {
                    action(path);

                    return true;
                }

                return false;
            }

            public static bool OnPathLoaded(in T path, in ProcessOptions<T> options, in ProcessDelegateTypes<T, TProcessDelegateParam>.IProcessDelegates<TProcessEventDelegates> processDelegates, in Action<T> action) => OnPathLoaded(path, options, processDelegates, null, action);
        }
    }

    public static class ProcessHelper<TPath, TAction> where TPath : IPath
    {
        public static FileStream TryGetFileStream(in ProcessErrorTypes<TPath, ProcessError, TAction>.IProcessErrorFactories factory, in string path, in int bufferLength, out IProcessError<ProcessError, TAction> error) 
        {
            try
            {
                FileStream result = File.GetFileStream(path, bufferLength);

                error = factory.GetNoErrorError();

                return result;
            }

            catch (System.IO.IOException ex) when (ex.Is(false, typeof(System.IO.FileNotFoundException), typeof(System.IO.DirectoryNotFoundException)))
            {
                error = factory.GetError(ProcessError.PathNotFound, PathNotFound, ErrorCode.PathNotFound);
            }

            catch (System.IO.PathTooLongException)
            {
                error = factory.GetError(ProcessError.PathTooLong, PathTooLong, ErrorCode.InvalidName);
            }

            catch (System.IO.IOException)
            {
                error = factory.GetError(ProcessError.UnknownError, UnknownError, (ErrorCode)(-1));
            }

            catch (Exception ex) when (ex.Is(false, typeof(UnauthorizedAccessException), typeof(SecurityException)))
            {
                error = factory.GetError(ProcessError.ReadProtection, string.Format(ReadProtection, Source), ErrorCode.ReadFault);
            }

            return null;
        }

        public static void RemoveDirectory(in string path, in ProcessErrorTypes<TPath, ProcessError, TAction>.IProcessErrorFactories factory, out IProcessError<ProcessError, TAction> error) => error = Shell.RemoveDirectoryW(path)
                ? factory.GetNoErrorError()
                : GetRemoveDirectoryError(factory, (ErrorCode)Marshal.GetLastWin32Error());

        public static IProcessError<ProcessError, TAction> GetCopyError(ProcessErrorTypes<TPath, ProcessError, TAction>.IProcessErrorFactories factory, ErrorCode errorCode)
        {
            IProcessError<ProcessError, TAction> getError(in ProcessError error, in string message) => factory.GetError(error, message, errorCode);

            switch (errorCode)
            {
                case ErrorCode.AlreadyExists:

                    return getError(ProcessError.FileSystemEntryAlreadyExists, FileSystemEntryAlreadyExists);

                case ErrorCode.PathNotFound:

                    return getError(ProcessError.PathNotFound, PathNotFound);

                case ErrorCode.AccessDenied:

                    return getError(ProcessError.ReadProtection, string.Format(ReadProtection, Source));

                case ErrorCode.DiskFull:

                    return getError(ProcessError.NotEnoughSpace, NotEnoughSpace);

                case ErrorCode.DiskOperationFailed:

                    return getError(ProcessError.DiskError, DiskError);

                default:

                    return getError(ProcessError.UnknownError, UnknownError);
            }
        }

        public static IProcessError<ProcessError, TAction> GetRemoveDirectoryError(ProcessErrorTypes<TPath, ProcessError, TAction>.IProcessErrorFactories factory, ErrorCode errorCode)
        {
            IProcessError<ProcessError, TAction> getError(in ProcessError processError, in string message) => factory.GetError(processError, message, errorCode);

            switch (errorCode)
            {
                case ErrorCode.FileNotFound:

                    return getError(ProcessError.PathNotFound, PathNotFound);

                case ErrorCode.DirNotEmpty:

                    return factory.GetNoErrorError();

                case ErrorCode.Directory:

                    return getError(ProcessError.ItemIsNotDirectory, ItemIsNotDirectory);

                default:

                    return getError(ProcessError.UnknownError, UnknownError);
            }
        }
    }
}
