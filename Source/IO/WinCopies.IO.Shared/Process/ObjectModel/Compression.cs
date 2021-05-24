/*Copyright © Pierre Sprimont, 2021
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
using SevenZip;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;

namespace WinCopies.IO.Process
{
    public class ArchiveCompressionPathInfo : PathTypes<IPathInfo>.PathInfo
    {
        public string[] FileNames { get; }

        public ArchiveCompressionPathInfo(in string[] fileNames) : base(new PathTypes<IPathInfo>.RootPath("..\\", true)) => FileNames = fileNames;
    }

    namespace ObjectModel
    {
        [ProcessGuid(Guids.Process.ArchiveCompression)]
        public class Compression<T> : // ArchiveProcess<WinCopies.IO.IPathInfo, ProcessQueueCollection, ReadOnlyProcessQueueCollection, ProcessErrorPathQueueCollection, ReadOnlyProcessErrorPathQueueCollection
                                      // #if DEBUG
                                      // , ProcessSimulationParameters
                                      // #endif
                                      // >
           ArchiveProcess<T> where T : ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError>.IProcessErrorFactories
        {
            protected SevenZipCompressor ArchiveCompressor { get; }

            public override string Guid => Guids.Process.ArchiveCompression;

            public override string Name => Properties.Resources.Compression;

            public Compression(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in IProcessLinkedList<IPathInfo, ProcessError> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, T factory, in SevenZipCompressor archiveCompressor) : base(initialPaths, sourcePath, destinationPath, paths, errorsQueue, progressDelegate, factory) => ArchiveCompressor = archiveCompressor;

            protected override bool LoadPathsOverride(out IProcessError<ProcessError> error, out bool clearOnError)
            {
                var fileNames = new ArrayBuilder<string>();

                foreach (IPathInfo path in InitialPaths)

                    if (path.IsDirectory)

                        AddPath(path);

                    else

                        _ = fileNames.AddLast(path.Path);

                if (fileNames.Count != 0)

                    AddPath(new ArchiveCompressionPathInfo(fileNames.ToArray()));

                error = Factory.GetNoErrorError();

                clearOnError = false;

                return true;
            }

            protected override bool Check(out IProcessError<ProcessError> error)
            {
                try
                {
                 if (System.IO.File.Exists(DestinationPath.Path) || System.IO.Directory.Exists(DestinationPath.Path))
                    {
                        error = new ProcessError<ProcessError>(ProcessError.FileSystemEntryAlreadyExists, "An item already exists with the same path.", ErrorCode.FileExists);

                        return false;
                    }
                }

                catch
                {
                    // Left empty.
                }

                ArchiveCompressor.CompressionMode = CompressionMode.Create;

                ArchiveCompressor.FileCompressionStarted += ArchiveCompressor_FileCompressionStarted;

                ArchiveCompressor.Compressing += ArchiveCompressor_Compressing;

                ArchiveCompressor.FileCompressionFinished += ArchiveCompressor_FileCompressionFinished;

                error = Factory.GetNoErrorError();

                return true;
            }

            protected override bool DoWork(IPathInfo path, out IProcessError<ProcessError> error, out bool isErrorGlobal)
            {
                isErrorGlobal = false;

                if (path.IsDirectory)

                    ArchiveCompressor.CompressDirectory(path.Path, DestinationPath.Path);

                else if (path is ArchiveCompressionPathInfo _path)

                    ArchiveCompressor.CompressFiles(DestinationPath.Path, _path.FileNames);

                else

                    ArchiveCompressor.CompressFiles(DestinationPath.Path, path.Path);

                error = Factory.GetNoErrorError();

                return CancellationPending ? (CancellationPending = false) : true;
            }

            protected override bool DoWork(IProcessErrorItem<IPathInfo, ProcessError> path, out IProcessError<ProcessError> error, out bool isErrorGlobal) => DoWork(path.Item, out error, out isErrorGlobal);

            protected override bool OnRunWorkerCompleted()
            {
                ArchiveCompressor.FileCompressionStarted -= ArchiveCompressor_FileCompressionStarted;

                ArchiveCompressor.Compressing -= ArchiveCompressor_Compressing;

                ArchiveCompressor.FileCompressionFinished -= ArchiveCompressor_FileCompressionFinished;

                return true;
            }

            private void ArchiveCompressor_Compressing(object sender, ProgressEventArgs e) => CancellationPending = ProcessDelegates.CommonDelegate.RaiseEvent(new ProcessProgressDelegateParameter(e.PercentDone));

            private void ArchiveCompressor_FileCompressionStarted(object sender, FileNameEventArgs e)
            {
                if (OnFileProcessStarted(e.PercentDone))

                    e.Cancel = true;
            }

            private void ArchiveCompressor_FileCompressionFinished(object sender, System.EventArgs e) => _ = OnFileProcessCompleted();

            protected override void ResetStatus() { /* Left empty. */ }
        }
    }
}
