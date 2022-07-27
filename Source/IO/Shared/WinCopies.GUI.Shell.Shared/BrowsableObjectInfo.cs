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

#region Usings
using SevenZip;

using System;
using System.Linq;
using System.Reflection;

#region WinCopies
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.GUI.IO;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Shell;
using WinCopies.IO.Shell.ObjectModel;
using WinCopies.Util;
#endregion WinCopies

using static WinCopies.IO.Consts.Guids.Shell.Process.Archive;
#endregion Usings

namespace WinCopies.GUI.Shell
{
    public class ArchiveProcessInitializer<T> : ProcessInitialization<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer2<T> where T : IArchiveProcessParameters
    {
        public ArchiveProcessInitializer(in ProcessInitialization<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer processInitializer, in T extraParameters) : base(processInitializer, extraParameters) { /* Left empty. */ }

        public override PathTypes<WinCopies.IO.IPathInfo>.RootPath GetDestPath() => new PathTypes<WinCopies.IO.IPathInfo>.RootPath(ExtraParameters.DestinationPath, true);
    }

    public delegate WinCopies.IO.Process.ObjectModel.IProcess ProcessConverter<TInnerProcess, TFactory>(
        in IEnumerableQueue<WinCopies.IO.IPathInfo> initialPaths,
        in WinCopies.IO.IPathInfo sourcePath,
        in WinCopies.IO.IPathInfo destinationPath,
        in ProcessTypes<WinCopies.IO.IPathInfo>.IProcessQueue paths,
        in WinCopies.IO.Process.IProcessLinkedList<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object> errorsQueue,
        in ProcessDelegateTypes<WinCopies.IO.IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<WinCopies.IO.IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate,
        in TInnerProcess innerProcess) where TFactory : ProcessErrorTypes<WinCopies.IO.IPathInfo, ProcessError, object>.IProcessErrorFactories;

    namespace ObjectModel
    {
        public class BrowsableObjectInfoPlugin : WinCopies.IO.Shell.BrowsableObjectInfoPlugin
        {
            public BrowsableObjectInfoPlugin() => RegisterProcessSelectorsStack.Push(() =>
                                                             {
                                                                 ShellObjectInfo.DefaultCustomProcessesSelectorDictionary.Push(item => item.InnerObject.IsFileSystemObject, item => new IProcessInfo[] { new ArchiveCompressionProcessInfo(), new ArchiveExtractionProcessInfo() });

                                                                 WinCopies.IO.ObjectModel.BrowsableObjectInfo.DefaultProcessSelectorDictionary.Push(item => WinCopies.IO.ObjectModel.BrowsableObjectInfo.Predicate(item, typeof(WinCopies.IO.Consts.Guids.Shell.Process.Archive)), TryGetArchiveProcess);
                                                             });

            public static WinCopies.IO.Process.ObjectModel.IProcess Get<TInnerProcess, TParameters>(in ArchiveProcessInitializer<TParameters> processInitializer, in ProcessConverter<TInnerProcess, ProcessErrorTypes<WinCopies.IO.IPathInfo, ProcessError, object>.IProcessErrorFactories> func) where TParameters : IArchiveProcessParameters<TInnerProcess>
            {
                ProcessInitialization<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer _processInitializer = processInitializer.ProcessInitializer;

                return func(_processInitializer.GetInitialPaths(), _processInitializer.SourcePath, processInitializer.GetDestPath(), _processInitializer.GetPathsQueue(), _processInitializer.GetErrorsQueue(), ProcessHelper<WinCopies.IO.IPathInfo>.GetDefaultProcessDelegates(), processInitializer.ExtraParameters.ToArchiveProcess());
            }

            public static WinCopies.IO.Process.ObjectModel.IProcess GetArchiveCompressionProcess(in IEnumerableQueue<WinCopies.IO.IPathInfo> initialPaths, in WinCopies.IO.IPathInfo sourcePath, in WinCopies.IO.IPathInfo destinationPath, in ProcessTypes<WinCopies.IO.IPathInfo>.IProcessQueue paths, in WinCopies.IO.Process.IProcessLinkedList<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object> errorsQueue, in ProcessDelegateTypes<WinCopies.IO.IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<WinCopies.IO.IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, in SevenZipCompressor archiveCompressor) =>
                new WinCopies.IO.Process.ObjectModel.Compression<ProcessErrorFactory<WinCopies.IO.IPathInfo, object>>(
                                initialPaths,
                                sourcePath,
                                destinationPath,
                                paths,
                                errorsQueue,
                                progressDelegate,
                                new CompressionProcessErrorFactory(),
                                archiveCompressor);

            public static WinCopies.IO.Process.ObjectModel.IProcess GetArchiveExtractionProcess(in IEnumerableQueue<WinCopies.IO.IPathInfo> initialPaths, in WinCopies.IO.IPathInfo sourcePath, in WinCopies.IO.IPathInfo destinationPath, in ProcessTypes<WinCopies.IO.IPathInfo>.IProcessQueue paths, in WinCopies.IO.Process.IProcessLinkedList<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object> errorsQueue, in ProcessDelegateTypes<WinCopies.IO.IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<WinCopies.IO.IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, in Converter<string, SevenZipExtractor> archiveExtractor) =>
                new WinCopies.IO.Process.ObjectModel.Extraction<ProcessErrorFactory<WinCopies.IO.IPathInfo, object>>(
                                initialPaths,
                                sourcePath,
                                destinationPath,
                                paths,
                                errorsQueue,
                                progressDelegate,
                                new CompressionProcessErrorFactory(),
                                archiveExtractor);

            public static WinCopies.IO.Process.ObjectModel.IProcess
#if CS8
                ?
#endif
                TryGetArchiveProcess(ProcessFactorySelectorDictionaryParameters processParameters)
            {
                ProcessInitialization<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer processInitializer = new ProcessInitialization<WinCopies.IO.IPathInfo, ProcessError, ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer(processParameters);

                WinCopies.IO.Process.ObjectModel.IProcess get<TInnerProcess, TParameters>(in TParameters parameters, in ProcessConverter<TInnerProcess, ProcessErrorTypes<WinCopies.IO.IPathInfo, ProcessError, object>.IProcessErrorFactories> func) where TParameters : IArchiveProcessParameters<TInnerProcess> => Get<TInnerProcess, TParameters>(new ArchiveProcessInitializer<TParameters>(processInitializer, parameters), func);

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
            public static IBrowsableObjectInfoPlugin GetPluginParameters() => new BrowsableObjectInfoPlugin();

            public static IExplorerControlViewModel GetDefaultExplorerControlViewModel() => IO.ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel(ShellObjectInfo.GetDefault());
        }
    }
}
