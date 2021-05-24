/*Copyright © Pierre Sprimont, 2020
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

//using Microsoft.WindowsAPICodePack.Win32Native;
//using SevenZip;

//using System;
//using System.ComponentModel;
//using System.Security;

//using WinCopies.Util;

using System;
using WinCopies.Collections.DotNetFix.Generic;

namespace WinCopies.IO.Process.ObjectModel
{
    public abstract class ArchiveProcess<T> : ProcessObjectModelTypes<IPathInfo, IPathInfo, T, ProcessError, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates>, ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates, IProcessProgressDelegateParameter>.DefaultDestinationProcess where T : ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError>.IProcessErrorFactories
        // <T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
        //#if DEBUG
        //         , TSimulationParameters
        //#endif
        //        > : PathInfoPathToPathProcess<T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
        //#if DEBUG
        //             , TSimulationParameters
        //#endif
        //            >

        //        where T : WinCopies.IO.IPathInfo
        //        where TCollection : IProcessCollection
        //        where TReadOnlyCollection : IReadOnlyProcessCollection
        //        where TErrorPathCollection : IProcessErrorPathCollection
        //        where TReadOnlyErrorPathCollection : IReadOnlyProcessErrorPathCollection
        //#if DEBUG
        //        where TSimulationParameters : ProcessSimulationParameters
        //#endif
    {
        public bool CancellationPending { get; protected set; }

        protected ArchiveProcess(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in IProcessLinkedList<IPathInfo, ProcessError> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, T factory) : base(initialPaths, sourcePath, destinationPath.IsDirectory ? destinationPath : throw new ArgumentException($"{nameof(destinationPath)} must be a directory."), paths, errorsQueue, progressDelegate, factory)
        {
            // Left empty.
        }

        protected virtual bool OnFileProcessStarted(int percentDone) => ProcessDelegates.CancellationPendingDelegate.RaiseEvent(null);

        protected virtual bool OnFileProcessCompleted() => CancellationPending = ProcessDelegates.CommonDelegate.RaiseEvent(new ProcessProgressDelegateParameter(100));

        //        protected ArchiveProcess(in PathCollection<T> pathsToExtract, in string destPath, in TCollection pathCollection, in TReadOnlyCollection readOnlyPathCollection, in TErrorPathCollection errorPathCollection, TReadOnlyErrorPathCollection readOnlyErrorPathCollection
        //#if DEBUG
        //             , in TSimulationParameters simulationParameters
        //#endif
        //            ) : base(pathsToExtract, destPath, pathCollection, readOnlyPathCollection, errorPathCollection, readOnlyErrorPathCollection
        //#if DEBUG
        //                 , simulationParameters
        //#endif
        //                )
        //        { }

        //        protected virtual ProcessError OnPreProcess(DoWorkEventArgs e) => CheckIfDrivesAreReady(
        //#if DEBUG
        //                null
        //#endif
        //                );

        //        protected abstract ProcessError OnProcess(DoWorkEventArgs e);

        //        protected override ProcessError OnProcessDoWork(DoWorkEventArgs e)
        //        {
        //            if (CheckIfPauseOrCancellationPending())

        //                return Error;

        //            ProcessError error = OnPreProcess(e);

        //            if (error != ProcessError.None)

        //                return error;

        //            try
        //            {
        //                return OnProcess(e);
        //            }

        //            catch (SecurityException)
        //            {
        //                return ProcessError.ReadProtection;
        //            }

        //            catch (Exception ex) when (ex.Is(false, typeof(System.IO.IOException), typeof(SevenZipException)))
        //            {
        //                return ProcessError.UnknownError;
        //            }
        //        }
    }
}
