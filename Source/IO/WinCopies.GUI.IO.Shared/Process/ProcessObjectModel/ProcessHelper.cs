//* Copyright © Pierre Sprimont, 2020
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

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Security;
//using WinCopies.Util;

//#if !WinCopies3
//using WinCopies.Util;

//using static WinCopies.Util.Util;
//#else
//using static WinCopies.ThrowHelper;
//#endif

//namespace WinCopies.GUI.IO.Process
//{
//    public static class ProcessHelper
//    {
//        public static ProcessError LoadFileSystemEnumerationProcessPaths<T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
//#if DEBUG
//             , TSimulationParameters
//#endif
//            >(Process<T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
//#if DEBUG
//             , TSimulationParameters
//#endif
//           > process, IEnumerator<T> pathsToLoadEnumerator)

//            where T : WinCopies.IO.IPathInfo
//        where TCollection : IProcessCollection
//        where TReadOnlyCollection : IReadOnlyProcessCollection
//        where TErrorPathCollection : IProcessErrorPathCollection
//        where TReadOnlyErrorPathCollection : IReadOnlyProcessErrorPathCollection
//#if DEBUG
//            where TSimulationParameters : ProcessSimulationParameters
//#endif

//             => _LoadFileSystemEnumerationProcessPaths(process ?? throw GetArgumentNullException(nameof(process)), pathsToLoadEnumerator ?? throw GetArgumentNullException(nameof(pathsToLoadEnumerator)));

//        internal static ProcessError _LoadFileSystemEnumerationProcessPaths<T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
//#if DEBUG
//             , TSimulationParameters
//#endif
//            >(Process<T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
//#if DEBUG
//             , TSimulationParameters
//#endif
//           > process, IEnumerator<T> pathsToLoadEnumerator)

//            where T : WinCopies.IO.IPathInfo
//        where TCollection : IProcessCollection
//        where TReadOnlyCollection : IReadOnlyProcessCollection
//        where TErrorPathCollection : IProcessErrorPathCollection
//        where TReadOnlyErrorPathCollection : IReadOnlyProcessErrorPathCollection
//#if DEBUG
//            where TSimulationParameters : ProcessSimulationParameters
//#endif
//        {
//            if (process.CheckIfDriveIsReady())
//            {
//                while (true)

//                    try
//                    {
//                        if (process.CheckIfPauseOrCancellationPending())

//                            return process.Error;

//                        if (pathsToLoadEnumerator.MoveNext())

//                            process._Paths.Add(new PathInfo(pathsToLoadEnumerator.Current.Path, process.PathCollection.GetPathSizeDelegate(pathsToLoadEnumerator.Current))); // todo: use Windows API Code Pack's Shell's implementation instead.

//                        else break;
//                    }

//                    catch (Exception ex) when (ex.Is(false, typeof(IOException), typeof(SecurityException))) { }

//                return process.Error;
//            }

//            return process.Error;
//        }

//        public static ProcessError TryGetFileInfo(in string path, out FileInfo fileInfo)
//        {
//            try
//            {
//                fileInfo = new FileInfo(path);

//                return ProcessError.None;
//            }

//            catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(UnauthorizedAccessException)))
//            {
//                return ProcessError.ReadProtection;
//            }

//            catch (PathTooLongException)
//            {
//                return ProcessError.PathTooLong;
//            }

//            finally
//            {
//                fileInfo = null;
//            }
//        }
//    }
//}
