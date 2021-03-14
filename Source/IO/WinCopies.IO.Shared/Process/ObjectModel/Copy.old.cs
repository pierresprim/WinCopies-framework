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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Globalization;

using WinCopies.Util;

using Size = WinCopies.IO.Size;

namespace WinCopies.GUI.IO.Process
{
    public sealed class CopyProcessPathCollection : PathCollection<WinCopies.IO.IPathInfo>
    {
        protected override Func<WinCopies.IO.IPathInfo> GetNewEmptyEnumeratorPathInfoDelegate { get; }

        protected override Func<WinCopies.IO.IPathInfo, WinCopies.IO.IPathInfo> GetNewEnumeratorPathInfoDelegate { get; }

        protected override Func<WinCopies.IO.IPathInfo, WinCopies.IO.IPathInfo> GetNewPathInfoDelegate { get; } = path => path;

        public override Func<WinCopies.IO.IPathInfo, Size?> GetPathSizeDelegate { get; } = item =>
         {
             if (item.IsDirectory) return null;

             else return (Size)new FileInfo(item.Path).Length;
         };

        public CopyProcessPathCollection(string path) : this(path, new List<WinCopies.IO.IPathInfo>()) { }

        public CopyProcessPathCollection(string path, IList<WinCopies.IO.IPathInfo> list) : base(path, list)
        {
            GetNewEmptyEnumeratorPathInfoDelegate = () => new WinCopies.IO.PathInfo(Path, System.IO.Directory.Exists(Path));

            GetNewEnumeratorPathInfoDelegate = current => new WinCopies.IO.PathInfo(GetConcatenatedPath(current), current.IsDirectory);
        }
    }

    public class Copy : PathToPathProcess<WinCopies.IO.IPathInfo, ProcessQueueCollection, ReadOnlyProcessQueueCollection, ProcessErrorPathQueueCollection, ReadOnlyProcessErrorPathQueueCollection
#if DEBUG
         , CopyProcessSimulationParameters
#endif
        >
    {
        #region Private fields

        private IEnumerator<WinCopies.IO.IPathInfo> _pathsToLoadEnumerator;

        #endregion

        #region Properties

        #endregion

        public static Copy From(in CopyProcessPathCollection pathsToLoad, in string destPath
#if DEBUG
             , in CopyProcessSimulationParameters simulationParameters
#endif
            )
        {
            var processQueueCollection = new ProcessQueueCollection();
            var processErrorPathQueueCollection = new ProcessErrorPathQueueCollection();

            return new Copy(pathsToLoad, destPath, processQueueCollection, new ReadOnlyProcessQueueCollection(processQueueCollection), processErrorPathQueueCollection, new ReadOnlyProcessErrorPathQueueCollection(processErrorPathQueueCollection)
#if DEBUG
                 , simulationParameters
#endif
                );
        }

        private Copy(in CopyProcessPathCollection pathsToLoad, in string destPath, in ProcessQueueCollection pathCollection, in ReadOnlyProcessQueueCollection readOnlyPathCollection, in ProcessErrorPathQueueCollection errorPathCollection, ReadOnlyProcessErrorPathQueueCollection readOnlyErrorPathCollection
#if DEBUG
            , in CopyProcessSimulationParameters simulationParameters
#endif
            ) : base(pathsToLoad, destPath, pathCollection, readOnlyPathCollection, errorPathCollection, readOnlyErrorPathCollection
#if DEBUG
                 , simulationParameters
#endif
                ) =>
            // {
            // BackgroundWorker.DoWork += (object sender, DoWorkEventArgs e) => OnDoWork(e);

            // BackgroundWorker.RunWorkerAsync(pathsToLoad);

            _pathsToLoadEnumerator = pathsToLoad.GetEnumerator(null, null
#if NETCORE
                , null
#endif
                , WinCopies.IO.FileSystemEntryEnumerationOrder.FilesThenDirectories
#if DEBUG
                , simulationParameters.FileSystemEntryEnumeratorProcessSimulation
#endif
                );
        // }

        #region Method overrides

        protected override ProcessError OnLoadPaths(DoWorkEventArgs e)
        {
            ProcessError result = ProcessHelper._LoadFileSystemEnumerationProcessPaths(this, _pathsToLoadEnumerator);

            _pathsToLoadEnumerator.Dispose();

            _pathsToLoadEnumerator = null;

            return result;
        }

        #endregion
    }
}
