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

using SevenZip;

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

#region WinCopies
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.PropertySystem;
#endregion WinCopies

using static WinCopies.IO.Consts.Guids.Shell.Process.Archive;

namespace WinCopies.GUI.Shell
{
    public class ArchiveProcessInitializer<T> : ProcessInitialization<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer2<T> where T : IArchiveProcessParameters
    {
        public ArchiveProcessInitializer(in ProcessInitialization<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer processInitializer, in T extraParameters) : base(processInitializer, extraParameters) { /* Left empty. */ }

        public override PathTypes<IPathInfo>.RootPath GetDestPath() => new PathTypes<IPathInfo>.RootPath(ExtraParameters.DestinationPath, true);
    }

    namespace ObjectModel
    {
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

                  WinCopies.IO.ObjectModel.BrowsableObjectInfo.DefaultProcessSelectorDictionary.Push(item => WinCopies.IO.ObjectModel.BrowsableObjectInfo.Predicate(item, typeof(WinCopies.IO.Consts.Guids.Shell.Process.Archive)), TryGetArchiveProcess);
              });
            }

            public static WinCopies.IO.Process.ObjectModel.IProcess Get<TInnerProcess, TParameters>(in ArchiveProcessInitializer<TParameters> processInitializer, in ProcessConverter<TInnerProcess, ProcessErrorTypes<IPathInfo, ProcessError, object>.IProcessErrorFactories> func) where TParameters : IArchiveProcessParameters<TInnerProcess>
            {
                ProcessInitialization<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer _processInitializer = processInitializer.ProcessInitializer;

                return func(_processInitializer.GetInitialPaths(), _processInitializer.SourcePath, processInitializer.GetDestPath(), _processInitializer.GetPathsQueue(), _processInitializer.GetErrorsQueue(), ProcessHelper<WinCopies.IO.IPathInfo>.GetDefaultProcessDelegates(), processInitializer.ExtraParameters.ToArchiveProcess());
            }

            public static WinCopies.IO.Process.ObjectModel.IProcess GetArchiveCompressionProcess(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in WinCopies.IO.Process.IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, in SevenZipCompressor archiveCompressor) =>
                new WinCopies.IO.Process.ObjectModel.Compression<ProcessErrorFactory<IPathInfo, object>>(
                                initialPaths,
                                sourcePath,
                                destinationPath,
                                paths,
                                errorsQueue,
                                progressDelegate,
                                new CompressionProcessErrorFactory(),
                                archiveCompressor);

            public static WinCopies.IO.Process.ObjectModel.IProcess GetArchiveExtractionProcess(in IEnumerableQueue<IPathInfo> initialPaths, in IPathInfo sourcePath, in IPathInfo destinationPath, in ProcessTypes<IPathInfo>.IProcessQueue paths, in WinCopies.IO.Process.IProcessLinkedList<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object> errorsQueue, in ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessDelegates<ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.IProcessEventDelegates> progressDelegate, in Converter<string, SevenZipExtractor> archiveExtractor) =>
                new WinCopies.IO.Process.ObjectModel.Extraction<ProcessErrorFactory<IPathInfo, object>>(
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
                ProcessInitialization<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer processInitializer = new ProcessInitialization<IPathInfo, ProcessError, ProcessTypes<IPathInfo, ProcessError, object>.ProcessErrorItem, object>.ProcessInitializer(processParameters);

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

        public abstract class AppBrowsableObjectInfo<T> : BrowsableObjectInfo3<T>
        {
            public sealed override string Protocol => "about";

            public override string URI => Path;

            protected AppBrowsableObjectInfo(in T innerObject, in string path, in ClientVersion clientVersion) : base(innerObject, path, clientVersion) { /* Left empty. */ }
        }

        public abstract class PluginInfo<T> : BrowsableObjectInfo3<T>, IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin> where T : IBrowsableObjectInfoPlugin
        {
            public override string LocalizedName => GetName(InnerObjectGenericOverride.GetType().Assembly);

            public override string Name => LocalizedName;

            protected override bool IsLocalRootOverride => false;

            protected override IBitmapSourceProvider BitmapSourceProviderOverride => InnerObjectGenericOverride.BitmapSourceProvider;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => null;

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

            protected override string DescriptionOverride => InnerObjectGeneric.GetType().Assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault()?.Description;

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride => "Plug-in Start Page";

            protected override object ObjectPropertiesOverride => null;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

            protected override IProcessFactory ProcessFactoryOverride => null;

            public override string Protocol => "plugin";

            public override string URI { get; }

            IBrowsableObjectInfoPlugin IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin>.InnerObject => InnerObjectGeneric;

            private PluginInfo(in T plugin, in ClientVersion clientVersion, Assembly assembly) : base(plugin, GetName(assembly), clientVersion) => URI = GetURI(assembly);

            protected PluginInfo(in T plugin, in ClientVersion clientVersion) : this(plugin, clientVersion, (plugin
#if CS8
                ??
#else
                == null ?
#endif
                throw ThrowHelper.GetArgumentNullException(nameof(plugin))
#if !CS8
                : plugin
#endif
                ).GetType().Assembly)
            { /* Left empty. */ }

            public static string GetName(in Assembly assembly)
            {
                AssemblyName name = (assembly ?? throw ThrowHelper.GetArgumentNullException(nameof(assembly))).GetName();

                return name.Name ?? name.FullName;
            }

            public static string GetURI(in Assembly assembly) => assembly.GetCustomAttributes<GuidAttribute>().FirstOrDefault()?.Value ?? GetName(assembly);

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItemsOverride() => InnerObjectGenericOverride.GetStartPages(ClientVersion).AppendValues(InnerObjectGenericOverride.GetProtocols(this, ClientVersion));

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;
        }

        public abstract class BrowsableObjectInfoStartPage<T> : AppBrowsableObjectInfo<System.Collections.Generic.IEnumerable<T>> where T : IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin>
        {
            public override string LocalizedName => "Start Here";

            public override string Name => LocalizedName;

            protected override bool IsLocalRootOverride => true;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => null;

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

            protected override string DescriptionOverride => "This is the start page of the Explorer window. Here you can find all the root browsable items from the plug-ins you have installed.";

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride => "Start Page";

            protected override object ObjectPropertiesOverride => null;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

            protected override IBrowsableObjectInfo ParentOverride => null;

            protected override IProcessFactory ProcessFactoryOverride => null;

            protected BrowsableObjectInfoStartPage(in System.Collections.Generic.IEnumerable<T> plugins, in ClientVersion clientVersion) : base(plugins, "start", clientVersion) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItemsOverride() => InnerObjectGenericOverride.As<IBrowsableObjectInfo>();

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;
        }

        public static class BrowsableObjectInfo
        {
            public static ClientVersion ClientVersion { get; } = new ClientVersion(Assembly.GetExecutingAssembly().GetName());

            public static IExplorerControlViewModel GetDefaultExplorerControlViewModel(in IBrowsableObjectInfoViewModel browsableObjectInfo, in bool selected = false)
            {
                IExplorerControlViewModel viewModel = ExplorerControlViewModel.From(browsableObjectInfo);

                if (selected)

                    viewModel.IsSelected = true;

                return viewModel;
            }

            public static IExplorerControlViewModel GetDefaultExplorerControlViewModel(in IBrowsableObjectInfo browsableObjectInfo, in bool selected = false) => GetDefaultExplorerControlViewModel(browsableObjectInfo is IBrowsableObjectInfoViewModel viewModel ? viewModel : new BrowsableObjectInfoViewModel(browsableObjectInfo), selected);

            public static IExplorerControlViewModel GetDefaultExplorerControlViewModel(in bool selected = false) => GetDefaultExplorerControlViewModel(ShellObjectInfo.GetDefault(ClientVersion), selected);

            public static IBrowsableObjectInfoPlugin GetPluginParameters() => new BrowsableObjectInfoPlugin();
        }
    }
}
