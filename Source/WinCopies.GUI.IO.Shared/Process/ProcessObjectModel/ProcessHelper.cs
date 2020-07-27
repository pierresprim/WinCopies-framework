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

using System;
using System.Collections.Generic;
using System.Security;

using WinCopies.Util;

using static WinCopies.Util.Util;

namespace WinCopies.GUI.IO.Process
{
    public static class ProcessHelper
    {
        public static ProcessError LoadFileSystemEnumerationProcessPaths<T
#if DEBUG
             , TSimulationParameters
#endif
            >(Process<T
#if DEBUG
             , TSimulationParameters
#endif
           > process, IEnumerator<T> pathsToLoadEnumerator) where T : WinCopies.IO.IPathInfo
#if DEBUG
            where TSimulationParameters : ProcessSimulationParameters
#endif

             => _LoadFileSystemEnumerationProcessPaths(process ?? throw GetArgumentNullException(nameof(process)), pathsToLoadEnumerator ?? throw GetArgumentNullException(nameof(pathsToLoadEnumerator)));

        internal static ProcessError _LoadFileSystemEnumerationProcessPaths<T
#if DEBUG
             , TSimulationParameters
#endif
            >(Process<T
#if DEBUG
             , TSimulationParameters
#endif
           > process, IEnumerator<T> pathsToLoadEnumerator) where T : WinCopies.IO.IPathInfo
#if DEBUG
            where TSimulationParameters : ProcessSimulationParameters
#endif
        {
            if (process.CheckIfDriveIsReady())
            {
                while (true)

                    try
                    {
                        if (process.CheckIfPauseOrCancellationPending())

                            return process.Error;

                        if (pathsToLoadEnumerator.MoveNext())

                            process._Paths.Enqueue(new PathInfo(pathsToLoadEnumerator.Current.Path, process.PathCollection.GetPathSizeDelegate(pathsToLoadEnumerator.Current))); // todo: use Windows API Code Pack's Shell's implementation instead.

                        else break;
                    }

                    catch (Exception ex) when (ex.Is(false, typeof(System.IO.IOException), typeof(SecurityException))) { }

                return process.Error;
            }

            return process.Error;
        }
    }
}
