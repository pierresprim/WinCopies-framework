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

using System;
using System.Collections.Generic;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Util.Commands.Primitives;

namespace WinCopies.IO.Process
{
    public class ArchiveCompressionPathInfo : PathTypes<IPathInfo>.PathInfo
    {
        public string[] FileNames { get; }

        public ArchiveCompressionPathInfo(in string[] fileNames) : base(new PathTypes<IPathInfo>.RootPath("..\\", true)) => FileNames = fileNames;
    }

    public class CompressionProcessErrorFactory : ProcessErrorFactory<IPathInfo, object>
    {
        public override object IgnoreAction => null;
    }

    namespace ObjectModel
    {
        [ProcessGuid(Guids.Process.ArchiveCompression)]
        public class Compression<T> : // ArchiveProcess<WinCopies.IO.IPathInfo, ProcessQueueCollection, ReadOnlyProcessQueueCollection, ProcessErrorPathQueueCollection, ReadOnlyProcessErrorPathQueueCollection
                                      // #if DEBUG
                                      // , ProcessSimulationParameters
                                      // #endif
                                      // >
           ArchiveProcess<T> where T : ProcessErrorTypes<IPathInfo, ProcessError, object>.IProcessErrorFactories
        {
            public override IReadOnlyDictionary<string, ICommand<IProcessErrorItem<IPathInfo, ProcessError, object>>> Actions => null;

            protected SevenZipCompressor ArchiveCompressor { get; }

            public override string Guid => Guids.Process.ArchiveCompression;

            public override string Name => Shell.Properties.Resources.Compression;

            public Compression(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, T factory, in SevenZipCompressor archiveCompressor) : base(initialPaths, sourcePath, destinationPath, paths, errorsQueue, progressDelegate, factory) => ArchiveCompressor = archiveCompressor;

            protected override IRecursiveEnumerable<IPathInfo> GetEnumerable(in IPathInfo path) => throw new NotSupportedException();

            protected override bool OnPathLoaded(in IPathInfo path) => throw new NotSupportedException();

            protected override RecursiveEnumerationOrder GetRecursiveEnumerationOrder() => throw new NotSupportedException();

            protected override Predicate<IPathInfo> GetAddAsDuplicatePredicate() => throw new NotSupportedException();

            protected override void GetPathsLoadingErrorParameters(in ProcessError error, in string message, in ErrorCode errorCode, out IProcessError<ProcessError, object> _error, out bool clearOnError) => throw new NotSupportedException();

            protected override bool LoadPathsOverride(out IProcessError<ProcessError, object> error, out bool clearOnError)
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

            protected override bool Check(out IProcessError<ProcessError, object> error)
            {
                try
                {
                    if (System.IO.File.Exists(DestinationPath.Path) || System.IO.Directory.Exists(DestinationPath.Path))
                    {
                        error = new ProcessError<ProcessError, object>(ProcessError.FileSystemEntryAlreadyExists, "An item already exists with the same path.", ErrorCode.FileExists);

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

            protected override bool DoWork(IPathInfo path, out IProcessError<ProcessError, object> error, out bool isErrorGlobal)
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

            protected override bool DoWork(IProcessErrorItem<IPathInfo, ProcessError, object> path, out IProcessError<ProcessError, object> error, out bool isErrorGlobal) => DoWork(path.Item, out error, out isErrorGlobal);

            protected override bool OnRunWorkerCompleted(bool? result)
            {
                ArchiveCompressor.FileCompressionStarted -= ArchiveCompressor_FileCompressionStarted;

                ArchiveCompressor.Compressing -= ArchiveCompressor_Compressing;

                ArchiveCompressor.FileCompressionFinished -= ArchiveCompressor_FileCompressionFinished;

                return base.OnRunWorkerCompleted(result);
            }

            private void ArchiveCompressor_Compressing(object sender, ProgressEventArgs e) => CancellationPending = ProcessDelegates.CommonDelegate.RaiseEvent(new ProcessProgressDelegateParameter(e.PercentDone, null));

            private void ArchiveCompressor_FileCompressionStarted(object sender, FileNameEventArgs e)
            {
                if (OnFileProcessStarted(e.PercentDone))

                    e.Cancel = true;
            }

            private void ArchiveCompressor_FileCompressionFinished(object sender, System.EventArgs e) => _ = OnFileProcessCompleted();

            protected override void ResetStatus() { /* Left empty. */ }

            protected override IPathInfo Convert(in IPathInfo path) => path;
        }
    }
}
