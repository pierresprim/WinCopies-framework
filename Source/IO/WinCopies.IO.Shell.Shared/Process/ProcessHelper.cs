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

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

using WinCopies.Collections.Generic;
using WinCopies.Extensions;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Resources;
using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.IO.Resources.ExceptionMessages;
using static WinCopies.IO.Shell.Resources.ExceptionMessages;

namespace WinCopies.IO.Shell.Process
{
    public static class ProcessErrorHelper
    {
        public static IProcessError<ProcessError, TAction> GetError<TAction>(in ProcessError error, in ErrorCode errorCode, in IProcessErrorFactory<ProcessError, TAction> factory) => factory.GetError(error, GetErrorMessageFromProcessError(error), errorCode);

        public static IProcessError<ProcessError, TAction> GetError<TAction>(in ProcessError error, in HResult hResult, in IProcessErrorFactory<ProcessError, TAction> factory) => factory.GetError(error, GetErrorMessageFromProcessError(error), hResult);

        public static string GetErrorMessageFromProcessError(ProcessError error)
        {
            System.Reflection.PropertyInfo property = typeof(Shell.Resources.ExceptionMessages).GetProperties().AppendValues(typeof(ExceptionMessages).GetProperties()).FirstOrDefault(p => p.Name == error.ToString());

            return property == null ? error.ToString() : (string)property.GetValue(null);
        }
    }

    public static class ProcessHelper
    {
        public static bool CanRun(in string path, in System.Collections.Generic.IEnumerator<IBrowsableObjectInfo> enumerator)
        {
            var enumerable = new CustomEnumeratorEnumerable<IBrowsableObjectInfo, System.Collections.Generic.IEnumerator<IBrowsableObjectInfo>>(enumerator);

            foreach (IBrowsableObjectInfo _path in enumerable)

                if (!(_path is IShellObjectInfoBase2 shellObjectInfo
                    && shellObjectInfo.InnerObject.IsFileSystemObject
                    && shellObjectInfo.Path.Validate(path, path.EndsWith(WinCopies.IO.Path.PathSeparator
#if !CS8
                                .ToString()
#endif
                                ) ? 1 : 0, null, null, 1, "\\")))

                    return false;

            return true;
        }

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

        public static void RemoveDirectory(in string path, in ProcessErrorTypes<TPath, ProcessError, TAction>.IProcessErrorFactories factory, out IProcessError<ProcessError, TAction> error) => error = Microsoft.WindowsAPICodePack.Win32Native.Shell.Shell.RemoveDirectoryW(path)
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
