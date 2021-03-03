///* Copyright © Pierre Sprimont, 2020
//*
//* This file is part of the WinCopies Framework.
//*
//* The WinCopies Framework is free software: you can redistribute it and/or modify
//* it under the terms of the GNU General Public License as published by
//* the Free Software Foundation, either version 3 of the License, or
//* (at your option) any later version.
//*
//* The WinCopies Framework is distributed in the hope that it will be useful,
//* but WITHOUT ANY WARRANTY; without even the implied warranty of
//* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//* GNU General Public License for more details.
//*
//* You should have received a copy of the GNU General Public License
//* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using Microsoft.WindowsAPICodePack.Win32Native;
//using SevenZip;

//using System;
//using System.ComponentModel;
//using System.Security;

//using WinCopies.Util;

//namespace WinCopies.GUI.IO.Process
//{
//    public abstract class ArchiveProcess<T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
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
//    {
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

//        protected virtual bool OnFileProcessStarted(in int percentDone)
//        {
//            if (CheckIfPauseOrCancellationPending())

//                return true;

//            CurrentPath = _Paths.Peek();

//            if (WorkerReportsProgress)

//                _ = TryReportProgress(percentDone);

//            return false;
//        }

//        protected virtual bool OnFileProcessCompleted()
//        {
//            WinCopies.IO.Size? size = _Paths.Remove().Size;

//            if (size.HasValue && !size.Value.ValueInBytes.IsNaN)

//                _Paths.DecrementSize(size.Value.ValueInBytes.Value);

//            return false;
//        }

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
//    }
//}
