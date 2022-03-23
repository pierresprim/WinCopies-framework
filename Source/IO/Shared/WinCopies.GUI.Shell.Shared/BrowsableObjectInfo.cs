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

using Microsoft.WindowsAPICodePack.Shell;

using SevenZip;

using System;
using System.Reflection;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;

using static WinCopies.IO.Guids.Shell.Process.Archive;

namespace WinCopies.GUI.Shell.ObjectModel
{
    public class ArchiveProcessInitializer<T> : ProcessInitialization<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer2<T> where T : IArchiveProcessParameters
    {
        public ArchiveProcessInitializer(in ProcessInitialization<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer processInitializer, in T extraParameters) : base(processInitializer, extraParameters) { /* Left empty. */ }

        public override PathTypes<IPathInfo>.RootPath GetDestPath() => new PathTypes<WinCopies.IO.IPathInfo>.RootPath(ExtraParameters.DestinationPath, true);
    }

    public delegate WinCopies.IO.Process.ObjectModel.IProcess ProcessConverter<TInnerProcess, TFactory>(
        in IEnumerableQueue<IPathInfo> initialPaths,
        in IPathInfo sourcePath,
        in IPathInfo destinationPath,
        in ProcessTypes<IPathInfo>.IProcessQueue paths,
        in WinCopies.IO.Process.IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object> errorsQueue,
        in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate,
        in TInnerProcess innerProcess) where TFactory : ProcessErrorTypes<IPathInfo, ProcessError, object>.IProcessErrorFactories;

    public class BrowsableObjectInfoPlugin : WinCopies.IO.Shell.BrowsableObjectInfoPlugin
    {
        public BrowsableObjectInfoPlugin()
        {
            RegisterProcessSelectorsStack.Push(() =>
          {
              ShellObjectInfo.DefaultCustomProcessesSelectorDictionary.Push(item => item.InnerObject.IsFileSystemObject, item => new IProcessInfo[] { new ArchiveCompressionProcessInfo(), new ArchiveExtractionProcessInfo() });

              WinCopies.IO.ObjectModel.BrowsableObjectInfo.DefaultProcessSelectorDictionary.Push(item => WinCopies.IO.ObjectModel.BrowsableObjectInfo.Predicate(item, typeof(WinCopies.IO.Guids.Shell.Process.Archive)), TryGetArchiveProcess);
          });
        }

        public static WinCopies.IO.Process.ObjectModel.IProcess Get<TInnerProcess, TParameters>(in ArchiveProcessInitializer<TParameters> processInitializer, in ProcessConverter<TInnerProcess, ProcessErrorTypes<IPathInfo, ProcessError, object>.IProcessErrorFactories> func) where TParameters : IArchiveProcessParameters<TInnerProcess>
        {
            ProcessInitialization<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer _processInitializer = processInitializer.ProcessInitializer;

            return func(_processInitializer.GetInitialPaths(), _processInitializer.SourcePath, processInitializer.GetDestPath(), _processInitializer.GetPathsQueue(), _processInitializer.GetErrorsQueue(), ProcessHelper<WinCopies.IO.IPathInfo>.GetDefaultProcessDelegates(), processInitializer.ExtraParameters.ToArchiveProcess());
        }

        public static WinCopies.IO.Process.ObjectModel.IProcess GetArchiveCompressionProcess(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in WinCopies.IO.Process.IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, in SevenZipCompressor archiveCompressor) =>
            new WinCopies.IO.Process.ObjectModel.Compression<ProcessErrorFactory<WinCopies.IO.IPathInfo, object>>(
                            initialPaths,
                            sourcePath,
                            destinationPath,
                            paths,
                            errorsQueue,
                            progressDelegate,
                            new CompressionProcessErrorFactory(),
                            archiveCompressor);

        public static WinCopies.IO.Process.ObjectModel.IProcess GetArchiveExtractionProcess(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in WinCopies.IO.Process.IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, in Converter<string, SevenZipExtractor> archiveExtractor) =>
            new WinCopies.IO.Process.ObjectModel.Extraction<ProcessErrorFactory<WinCopies.IO.IPathInfo, object>>(
                            initialPaths,
                            sourcePath,
                            destinationPath,
                            paths,
                            errorsQueue,
                            progressDelegate,
                            new CompressionProcessErrorFactory(),
                            archiveExtractor);

        public static WinCopies.IO.Process.ObjectModel.IProcess TryGetArchiveProcess(ProcessFactorySelectorDictionaryParameters processParameters)
        {
            ProcessInitialization<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer processInitializer = new ProcessInitialization<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer(processParameters);

            WinCopies.IO.Process.ObjectModel.IProcess get<TInnerProcess, TParameters>(in TParameters parameters, in ProcessConverter<TInnerProcess, ProcessErrorTypes<IPathInfo, ProcessError, object>.IProcessErrorFactories> func) where TParameters : IArchiveProcessParameters<TInnerProcess> => Get<TInnerProcess, TParameters>(new ArchiveProcessInitializer<TParameters>(processInitializer, parameters), func);

            try
            {
                switch (processInitializer.Guid)
                {
                    case Extraction:

                        return get<Converter<string, SevenZipExtractor>, IArchiveExtractionParameters>(ArchiveExtractionParameters.FromProcessParameters(processInitializer.Enumerator), GetArchiveExtractionProcess);

                    case Compression:

                        return get<SevenZipCompressor, IArchiveCompressionParameters>(ArchiveCompressionParameters.FromProcessParameters(processInitializer.Enumerator), GetArchiveCompressionProcess);
                }
            }

            finally
            {
                processInitializer.Enumerator?.Dispose();
            }

            return null;
        }

        public static WinCopies.IO.Process.ObjectModel.IProcess GetArchiveProcess(ProcessFactorySelectorDictionaryParameters processParameters) => TryGetArchiveProcess(processParameters) ?? throw new InvalidOperationException("No process could be generated.");
    }

    public static class BrowsableObjectInfo
    {
        public static ClientVersion ClientVersion { get; } = new ClientVersion(Assembly.GetExecutingAssembly().GetName());

        public static IBrowsableObjectInfo GetBrowsableObjectInfo(string path) => ShellObjectInfo.From(ShellObjectFactory.Create(path));

        public static IExplorerControlViewModel GetDefaultExplorerControlViewModel(in IBrowsableObjectInfo browsableObjectInfo) => GetDefaultExplorerControlViewModel(browsableObjectInfo is IBrowsableObjectInfoViewModel viewModel ? viewModel : new BrowsableObjectInfoViewModel(browsableObjectInfo));

        public static IExplorerControlViewModel GetDefaultExplorerControlViewModel(in IBrowsableObjectInfoViewModel browsableObjectInfo)
        {
            IExplorerControlViewModel viewModel = ExplorerControlViewModel.From(browsableObjectInfo, GetBrowsableObjectInfo);

            return viewModel;
        }

        public static IExplorerControlViewModel GetDefaultExplorerControlViewModel() => GetDefaultExplorerControlViewModel(new ShellObjectInfo(KnownFolders.Desktop, ClientVersion));

        public static IBrowsableObjectInfoPlugin GetPluginParameters() => new BrowsableObjectInfoPlugin();
    }
}
