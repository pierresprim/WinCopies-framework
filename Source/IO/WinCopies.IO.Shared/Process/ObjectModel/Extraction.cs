///*Copyright © Pierre Sprimont, 2021
// *
// * This file is part of the WinCopies Framework.
// *
// * The WinCopies Framework is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * The WinCopies Framework is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using Microsoft.WindowsAPICodePack.Shell;

//using SevenZip;

//using System;
//using System.IO;
//using System.Linq;

//using WinCopies.Collections.DotNetFix.Generic;
//using WinCopies.Collections.Generic;
//using WinCopies.IO.ObjectModel;

//namespace WinCopies.IO.Process.ObjectModel
//{
//    //public interface IExtractionProcessPathInfo : WinCopies.IO.IPathInfo
//    //{
//    //    Func<SevenZipExtractor> GetArchiveExtractorDelegate { get; }
//    //}

//    //public struct ExtractionProcessPathInfo : IExtractionProcessPathInfo
//    //{
//    //    public string Path { get; }

//    //    public bool IsDirectory => false;

//    //    public Func<SevenZipExtractor> GetArchiveExtractorDelegate { get; }

//    //    public ExtractionProcessPathInfo(string path, Func<SevenZipExtractor> getArchiveExtractorDelegate)
//    //    {
//    //        Path = path;

//    //        GetArchiveExtractorDelegate = getArchiveExtractorDelegate;
//    //    }
//    //}

//    [ProcessGuid(Guids.Process.ArchiveExtraction)]
//    public class Extraction<T> : // ArchiveProcess<IPathInfo, ProcessQueueCollection, ReadOnlyProcessQueueCollection, ProcessErrorPathQueueCollection, ReadOnlyProcessErrorPathQueueCollection
//                                 // #if DEBUG
//                                 // , ProcessSimulationParameters
//                                 // #endif
//                                 // >
//        ArchiveProcess<T> where T : ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError>.IProcessErrorFactories
//    {
//        protected SevenZipExtractor ArchiveExtractor { get; }

//        public override string Guid => Guids.Process.ArchiveExtraction;

//        public override string Name => Properties.Resources.Extraction;

//        public string SourceShellPath { get; }

//        public int BufferLength { get; }

//        public Extraction(in string sourceShellPath, in int bufferLength, in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in IProcessLinkedList<IPathInfo, ProcessError> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, T factory, in SevenZipExtractor archiveExtractor) : base(initialPaths, sourcePath, destinationPath, paths, errorsQueue, progressDelegate, factory)
//        {
//            SourceShellPath = sourceShellPath;

//            BufferLength = bufferLength;

//            ArchiveExtractor = archiveExtractor;
//        }

//        protected virtual bool LoadTreeNode(LinkedTreeNode<IPathInfo> node, out IProcessError<ProcessError> error, out bool clearOnError)
//        {
//            void setParameters(out IProcessError<ProcessError> _error, out bool _clearOnError)
//            {
//                _error = Factory.GetNoErrorError();

//                _clearOnError = false;
//            }

//            if ((node ?? throw ThrowHelper.GetArgumentNullException(nameof(node))).Count == 0)
//            {
//                setParameters(out error, out clearOnError);

//                return true;
//            }

//            ArchiveItemInfo archive;

//            node = node.First;

//            do
//            {
//                if (node.Value.IsDirectory)
//                {
//                    archive = new ArchiveItemInfo(node.Value.Path, FileType.Folder, ShellObjectInfo.From(ShellObjectFactory.Create(SourceShellPath)), null, BrowsableObjectInfo.GetDefaultClientVersion());

//                    foreach (ArchiveItemInfo item in archive.GetItems().OrderBy(item => ((ArchiveItemInfo)item).ObjectPropertiesGeneric.FileType, (x, y) => x.CompareTo(y)))

//                        _ = node.AddLast(new PathTypes<IPathInfo>.PathInfo(item.Name, node.Value));

//                    archive = null;

//                    node = node.Count == 0 || !node.First.Value.IsDirectory ? node.Next ?? node.Parent : node.First;
//                }

//                else

//                    node = node.Parent.Next;

//            } while (node != null);

//            node = node.First;

//            LinkedTreeNode<IPathInfo> _node;

//            do
//            {
//                AddPath(node.Value);

//                if (node.First == null)
//                {
//                    _node = node.Parent.Next;

//                    node.Parent.Remove(node);

//                    node = _node;
//                }

//                else

//                    node = node.First;

//            } while (node != null);

//            setParameters(out error, out clearOnError);

//            return true;
//        }

//        protected override bool LoadPathsOverride(out IProcessError<ProcessError> error, out bool clearOnError)
//        {
//            var paths = new LinkedTreeNode<IPathInfo>();

//            foreach (IPathInfo path in InitialPaths.OrderBy(_path => _path.IsDirectory, (x, y) => (!x).CompareTo(!y)))

//                _ = paths.AddLast(path);

//            return LoadTreeNode(paths, out error, out clearOnError);
//        }

//        protected override bool Check(out IProcessError<ProcessError> error)
//        {
//            ArchiveExtractor.FileExtractionStarted += ArchiveExtractor_FileExtractionStarted;

//            ArchiveExtractor.Extracting += ArchiveExtractor_Extracting; ;

//            ArchiveExtractor.FileExtractionFinished += ArchiveExtractor_FileExtractionFinished;

//            error = Factory.GetNoErrorError();

//            return true;
//        }

//        protected override bool OnRunWorkerCompleted()
//        {
//            ArchiveExtractor.FileExtractionStarted -= ArchiveExtractor_FileExtractionStarted;

//            ArchiveExtractor.Extracting -= ArchiveExtractor_Extracting; ;

//            ArchiveExtractor.FileExtractionFinished -= ArchiveExtractor_FileExtractionFinished;

//            return true;
//        }

//        protected override bool DoWork(IPathInfo path, out IProcessError<ProcessError> error, out bool isErrorGlobal)
//        {
//            isErrorGlobal = false;

//            if (path.IsDirectory)
//            {
//                bool result = Microsoft.WindowsAPICodePack.Win32Native.Shell.Shell.CreateDirectoryW(GetDestinationPath(path), IntPtr.Zero);

//                if (result)
//                {
//                    error = Factory.GetNoErrorError();

//                    isErrorGlobal = false;

//                    return true;
//                }

//                error = ProcessHelper.GetIOError(Factory, out _);
//            }

//            else
//            {
//                FileStream fileStream = ProcessHelper.TryGetFileStream(Factory, GetDestinationPath(path), BufferLength, out error);

//                if (fileStream != null)

//                    using (fileStream)

//                        ArchiveExtractor.ExtractFile(path.Path, fileStream); // TODO: The Process class should be updated to allow a child class to process multiple paths at once.
//            }

//            return CancellationPending ? (CancellationPending = false) : true;
//        }

//        private void ArchiveExtractor_FileExtractionStarted(object sender, FileInfoEventArgs e)
//        {
//            if (OnFileProcessStarted(e.PercentDone))

//                e.Cancel = true;
//        }

//        protected override bool DoWork(IProcessErrorItem<IPathInfo, ProcessError> path, out IProcessError<ProcessError> error, out bool isErrorGlobal) => DoWork(path.Item, out error, out isErrorGlobal);

//        private void ArchiveExtractor_Extracting(object sender, ProgressEventArgs e) => CancellationPending = ProcessDelegates.CommonDelegate.RaiseEvent(new ProcessProgressDelegateParameter(e.PercentDone));

//        private void ArchiveExtractor_FileExtractionFinished(object sender, FileInfoEventArgs e) => _ = OnFileProcessCompleted();

//        protected override void ResetStatus() { /* Left empty. */ }
//    }
//}
