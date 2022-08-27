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

#region Usings
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections.Generic;

#region WinCopies
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Enumeration;
using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Shell.Process;
#endregion WinCopies

#region Static Usings
using static WinCopies.Delegates;
using static WinCopies.IO.FileType;
using static WinCopies.IO.Consts.Guids.Shell.Process.Shell;
using static WinCopies.IO.Shell.Path;
using static WinCopies.IO.Shell.Resources.ExceptionMessages;
using static WinCopies.ThrowHelper;
#endregion Static Usings

using SystemPath = System.IO.Path;
#endregion Usings

namespace WinCopies.IO.ObjectModel
{
    public partial class ShellObjectInfo : ShellObjectInfo<IFileSystemObjectInfoProperties, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>, IShellObjectInfo
    {
        #region Fields
        private readonly ItemSource _itemSource;
        private IFileSystemObjectInfoProperties _objectProperties;
        private static readonly BrowsabilityPathStack<IShellObjectInfo> __browsabilityPathStack = new
#if !CS9
            BrowsabilityPathStack<IShellObjectInfo>
#endif
    ();
        #endregion Fields

        #region Properties
        #region Static Properties
        public static IBrowsabilityPathStack<IShellObjectInfo> BrowsabilityPathStack { get; } = __browsabilityPathStack.AsWriteOnly();

        public static ISelectorDictionary<IShellObjectInfoBase2, IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IShellObjectInfoBase2, IEnumerable<IProcessInfo>>();

        public static IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new ShellObjectInfoSelectorDictionary();

        public static Action RegisterDefaultProcessSelectors { get; private set; } = () =>
        {
            DefaultProcessSelectorDictionary.Push(item => Predicate(item, typeof(Consts.Guids.Shell.Process.Shell)), TryGetProcess

            // System.Reflection.Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(t => t.Namespace.StartsWith(typeof(Process.ObjectModel.IProcess).Namespace) && t.GetCustomAttribute<ProcessGuidAttribute>().Guid == guid);
            );

            RegisterDefaultProcessSelectors = EmptyVoid;
        };
        #endregion Static Properties

        #region Overrides
        protected override IItemSourcesProvider<ShellObjectInfoEnumeratorStruct> ItemSourcesGenericOverride { get; }

        protected override IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

        protected sealed override IFileSystemObjectInfoProperties ObjectPropertiesGenericOverride => _objectProperties;
        #endregion Overrides
        #endregion Properties

        #region Constructors
        private ShellObjectInfo(in BrowsableObjectInfoURL url, FileType fileType, in ShellObject shellObject, in ClientVersion clientVersion) : base(url, shellObject, clientVersion)
        {
            _objectProperties = GetDefaultProperties(this, fileType);
            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(_itemSource = new ItemSource(this));
        }

        public ShellObjectInfo(in string path, FileType fileType, in ShellObject shellObject, in ClientVersion clientVersion) : this(new BrowsableObjectInfoURL(path), fileType, shellObject, clientVersion) { /* Left empty. */ }

        public ShellObjectInfo(in IKnownFolder knownFolder, in ClientVersion clientVersion) : this(GetURL(knownFolder), KnownFolder, ShellObjectFactory.Create(knownFolder.ParsingName), clientVersion) { /* Left empty. */ }
        #endregion

        #region Methods
        public override IBrowsableObjectInfo Clone() => new ShellObjectInfo(new BrowsableObjectInfoURL(Path, URI), ObjectPropertiesGeneric.FileType, InnerObjectGenericOverride, ClientVersion);

        private static BrowsableObjectInfoURL GetURL(in IKnownFolder knownFolder)
        {
            string path = knownFolder.Path;

            bool updatePath() => string.IsNullOrEmpty(path);

            if (updatePath())

                path = knownFolder.LocalizedName;

            return new BrowsableObjectInfoURL(updatePath() ? knownFolder.CanonicalName : path, knownFolder.ParsingName);
        }

        public static ShellObjectInfo GetDefault(in ClientVersion clientVersion) => new ShellObjectInfo(KnownFolders.Desktop, clientVersion);
        public static ShellObjectInfo GetDefault() => GetDefault(DefaultClientVersion);

        public static IFileSystemObjectInfoProperties GetDefaultProperties(IShellObjectInfoBase2 shellObjectInfo, FileType fileType)
        {
            IFileSystemObjectInfoProperties getFolderProperties() => new FolderShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType);
            IFileSystemObjectInfoProperties getFileSystemObjectInfoProperties() => new FileSystemObjectInfoProperties(shellObjectInfo, fileType);
#if CS9
                return
#else
            switch (
#endif
                fileType
#if CS9
                    switch
#else
                )
#endif
            {
#if !CS9
                case
#endif
                    Folder
#if CS9
                        =>
#else
                    :
                    return
#endif
                    getFolderProperties()
#if CS9
                        ,
#else
                    ;
                case
#endif
                    FileType.File
#if CS9
                        or
#else
                    :
                case
#endif
                    Archive
#if CS9
                        or
#else
                    :
                case
#endif
                    Library
#if CS9
                        or
#else
                    :
                case
#endif
                    Link
#if CS9
                        =>
#else
                    :
                    return
#endif
                    new FileShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType)
#if CS9
                        ,
#else
                    ;
                case
#endif
                    KnownFolder
#if CS9
                        =>
#else
                    :
                    return
#endif
                    shellObjectInfo.InnerObject.IsFileSystemObject ? getFolderProperties() : getFileSystemObjectInfoProperties()
#if CS9
                        ,
#else
                    ;
                case
#endif
                    Drive
#if CS9
                        =>
#else
                    :
                    return
#endif
                    new DriveShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType)
#if CS9
                        ,
                        _ =>
#else
                    ;
                default:
                    return
#endif
                    getFileSystemObjectInfoProperties()
#if CS9
                    };
#else
                ;
            }
#endif
        }

        public static ShellObjectInitInfo GetInitInfo(in ShellObject shellObject)
        {
#if CS8
            static
#endif
            FileType getFileType(in string path, in FileType defaultFileType) => IsSupportedArchiveFormat(SystemPath.GetExtension(path)) ? Archive : defaultFileType;

            switch (shellObject ?? throw GetArgumentNullException(nameof(shellObject)))
            {
                case ShellFolder _:

                    switch (shellObject)
                    {
                        case ShellFileSystemFolder shellFileSystemFolder:

                            (string path, FileType fileType) = shellFileSystemFolder is FileSystemKnownFolder ? (shellObject.ParsingName, KnownFolder) : (shellFileSystemFolder.Path, Folder);

                            return new ShellObjectInitInfo(path, System.IO.Directory.GetParent(path) is null ? Drive : getFileType(path, fileType));

                        case NonFileSystemKnownFolder nonFileSystemKnownFolder:

                            return new ShellObjectInitInfo(nonFileSystemKnownFolder.Path, KnownFolder);

                        case ShellNonFileSystemFolder _:

                            return new ShellObjectInitInfo(shellObject.ParsingName, KnownFolder);
                    }

                    break;

                case ShellLink shellLink:

                    return new ShellObjectInitInfo(shellLink.Path, Link);

                case ShellFile shellFile:

                    return new ShellObjectInitInfo(shellFile.Path, shellFile.IsLink ? Link : SystemPath.GetExtension(shellFile.Path) == ".library-ms" ? Library : getFileType(shellFile.Path, FileType.File));
            }

            throw new ArgumentException(ShellObjectIsNotSupported);
        }

        public static IProcess
#if CS8
            ?
#endif
            TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters)
        {
            string guid = processParameters.ProcessParameters.Guid.ToString();

            IEnumerator<string> enumerator = null;

            try
            {
                enumerator = processParameters.ProcessParameters.Parameters.GetEnumerator();

                PathTypes<IPathInfo>.RootPath getParameter() => enumerator.MoveNext()
                                    ? new PathTypes<IPathInfo>.RootPath(enumerator.Current, true)
                                    : throw new InvalidOperationException(Resources.ExceptionMessages.ProcessParametersCouldNotBeParsedCorrectly);

                ProcessTypes<T>.IProcessQueue getProcessCollection<T>() where T : IPathInfo => processParameters.Factory.GetProcessCollection<T>();

                IProcessLinkedList<TPath,
                    ProcessError,
                    ProcessTypes<TPath,
                        ProcessError,
                        TAction>.
                    ProcessErrorItem,
                    TAction> getProcessLinkedList<TPath, TAction>() where TPath : IPathInfo => processParameters.Factory.GetProcessLinkedList<
                                TPath,
                                ProcessError,
                                ProcessTypes<TPath, ProcessError, TAction>.ProcessErrorItem,
                                TAction>();

                PathTypes<IPathInfo>.PathInfoBase sourcePath;

                switch (guid)
                {
                    case Copy:

                        PathTypes<IPathInfo>.PathInfoBase destinationPath;

                        sourcePath = getParameter();
                        destinationPath = getParameter();

                        IProcess
#if CS8
                            ?
#endif
                            getCopyProcess()
                        {
                            _ = enumerator.MoveNext();

                            bool? option
#if CS8
                                =
#else
                                    ;
                                bool? getOption()
                                {
                                    switch (
#endif
                                enumerator.Current
#if CS8
                                switch
#else
                                    )
#endif
                                {
#if !CS8
                                        case
#endif
                                    "1"
#if CS8
                                    =>
#else
                                        :
                                            return
#endif
                                    true
#if CS8
                                    ,
#else
                                        ;
                                        case
#endif
                                    "0"
#if CS8
                                    =>
#else
                                        :
                                            return
#endif
                                    false
#if CS8
                                    ,
                                    _ =>
#else
                                        ;
                                        default:
                                            return
#endif
                                    null
#if CS8
                                };
#else
                                        ;
                                    }
                                }

                                option = getOption();
#endif

                            return option.HasValue
                                ? new Copy<ProcessErrorFactory<IPathInfo, CopyErrorAction>>(ProcessHelper<IPathInfo>.GetInitialPaths(enumerator, sourcePath, path => path),
                                    sourcePath,
                                    destinationPath,
                                    getProcessCollection<IPathInfo>(),
                                    getProcessLinkedList<IPathInfo, CopyErrorAction>(),
                                    ProcessHelper<IPathInfo>.GetDefaultProcessDelegates(),
                                    new CopyOptions<IPathInfo>(Bool.True, true, option.Value) { IgnoreFolderFileNameConflicts = true },
                                    new CopyProcessErrorFactory<IPathInfo>())
                                : null;
                        }

                        return getCopyProcess();

                    case Deletion:

                        sourcePath = getParameter();

                        IProcess
#if CS8
                            ?
#endif
                            getDeletionProcess()
                        {
                            _ = enumerator.MoveNext();

                            return Enum.TryParse(enumerator.Current, out RemoveOption option)
                                ? new Deletion<ProcessErrorFactory<IPathInfo, ErrorAction>>(ProcessHelper<IPathInfo>.GetInitialPaths(enumerator, sourcePath, path => path),
                                        sourcePath,
                                        getProcessCollection<IPathInfo>(),
                                        getProcessLinkedList<IPathInfo, ErrorAction>(),
                                        ProcessHelper<IPathInfo>.GetDefaultProcessDelegates(),
                                        new DeletionOptions<IPathInfo>(Bool.True, true, option),
                                        new DefaultProcessErrorFactory<IPathInfo>())
                                : null;
                        }

                        return getDeletionProcess();
                }
            }

            finally
            {
                enumerator?.Dispose();
            }

            return null;
        }

        public static IProcess GetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => TryGetProcess(processParameters) ?? throw new InvalidOperationException(NoProcessCouldBeGenerated);

        #region Construction Helpers
        public static ShellObjectInfo From(in ShellObject shellObject, in ClientVersion clientVersion)
        {
            ShellObjectInitInfo initInfo = GetInitInfo(shellObject);

            return new ShellObjectInfo(initInfo.Path, initInfo.FileType, shellObject, clientVersion);
        }

        public static ShellObjectInfo From(in ShellObject shellObject) => From(shellObject, GetDefaultClientVersion());

        public static ShellObjectInfo From(in string path, in ClientVersion clientVersion) => From(ShellObjectFactory.Create(path), clientVersion);

        public static ShellObjectInfo From(in string path) => From(path, GetDefaultClientVersion());
        #endregion

        protected override IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            _ = UtilHelpers.TryDispose(ref _objectProperties);
        }

        #region GetItems
        public IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetItems(Predicate<ArchiveFileInfoEnumeratorStruct> func)
        {
            IEnumerable<IBrowsableObjectInfo>
#if CS8
                    ?
#endif
                getItems(in IEnumerable<ShellObjectInfoItemProvider>
#if CS8
                    ?
#endif
                items) => _itemSource.GetItems(items);

            return func == null ? getItems(_itemSource.GetItemProviders()) : _itemSource.GetItems(_itemSource.GetItemProviders(func));
        }
        #endregion GetItems
        #endregion Methods
    }
}
