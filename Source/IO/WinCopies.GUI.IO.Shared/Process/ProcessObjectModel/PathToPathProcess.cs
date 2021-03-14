﻿///* Copyright © Pierre Sprimont, 2020
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
//using System.IO;
//using WinCopies.Collections.DotNetFix;

//namespace WinCopies.GUI.IO.Process
//{

//    public abstract class PathToPathProcess<T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
//#if DEBUG
//         , TSimulationParameters
//#endif
//        > : Process<T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
//#if DEBUG
//         , TSimulationParameters
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
//        /// <summary>
//        /// Gets the destination root path.
//        /// </summary>
//        public string DestPath { get; }

//        protected PathToPathProcess(in PathCollection<T> paths, in string destPath, in TCollection pathCollection, in TReadOnlyCollection readOnlyPathCollection, in TErrorPathCollection errorPathCollection, TReadOnlyErrorPathCollection readOnlyErrorPathCollection
//#if DEBUG
//            ,    in TSimulationParameters simulationParameters
//#endif
//            ) : base(paths, pathCollection, readOnlyPathCollection, errorPathCollection, readOnlyErrorPathCollection
//#if DEBUG
//                , simulationParameters
//#endif
//                ) => DestPath = WinCopies.IO.Path.IsFileSystemPath(destPath) && System.IO.Path.IsPathRooted(destPath) ? destPath : throw new ArgumentException($"{nameof(destPath)} is not a valid path.");
//    }
//}
