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

using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.IO;
using System.Linq;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Enumeration;
using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Shell.Process;
using WinCopies.Linq;
using WinCopies.PropertySystem;

using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.IO.ObjectModel.ShellObjectInfo;
using static WinCopies.IO.Shell.Resources.ExceptionMessages;
using static WinCopies.IO.FileType;
using static WinCopies.IO.Shell.Path;
using static WinCopies.ThrowHelper;

using SystemPath = System.IO.Path;

#if !CS8
using WinCopies.Collections;
#endif

namespace WinCopies.IO
{
    namespace ObjectModel
    {
        /// <summary>
        /// Represents a file system item.
        /// </summary>
        public abstract class ShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ArchiveItemInfoProvider<TObjectProperties, ShellObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            private ShellObject _shellObject;
            private IBrowsableObjectInfo _parent;
            private IBrowsabilityOptions _browsability;
            private IBitmapSourceProvider _bitmapSourceProvider;

            #region Properties
            public System.IO.Stream ArchiveFileStream { get; private set; }

            /// <summary>
            /// The parent <see cref="IShellObjectInfoBase"/> of the current archive item. If the current <see cref="ShellObjectInfo"/> represents an archive file, this property returns the current <see cref="ShellObjectInfo"/>, or <see langword="null"/> otherwise.
            /// </summary>
            public override IShellObjectInfoBase ArchiveShellObject => ObjectPropertiesGeneric.FileType == Archive ? this : null;

            public bool IsArchiveOpen => ArchiveFileStream is
#if CS9
                not null;
#else
                object;
#endif

            #region Overrides
            /// <summary>
            /// Gets a <see cref="ShellObject"/> that represents this <see cref="ShellObjectInfo"/>.
            /// </summary>
            protected sealed override ShellObject InnerObjectGenericOverride => _shellObject;

            protected override IBitmapSourceProvider BitmapSourceProviderOverride => _bitmapSourceProvider
#if CS8
                ??=
#else
                ?? (_bitmapSourceProvider =
#endif
                new Shell.BitmapSourceProvider(InnerObjectGeneric, true)
#if !CS8
                )
#endif
                ;

            protected override IBrowsabilityOptions BrowsabilityOverride
            {
                get
                {
                    if (_browsability == null)

                        if (InnerObjectGeneric is ShellLink shellLink)
                        {
                            ShellObjectInfo targetShellObjectInfo = From(shellLink.TargetShellObject, ClientVersion);

                            if (targetShellObjectInfo.InnerObjectGeneric is ShellLink)
                            {
                                targetShellObjectInfo.Dispose();

                                _browsability = BrowsabilityOptions.NotBrowsable;
                            }

                            else

                                _browsability = new ShellLinkBrowsabilityOptions(targetShellObjectInfo);
                        }

                        else

                            _browsability = _shellObject is System.Collections.Generic.IEnumerable<ShellObject> ? BrowsabilityOptions.BrowsableByDefault : BrowsabilityOptions.NotBrowsable;

                    return _browsability;
                }
            }

            protected override string DescriptionOverride => _shellObject.Properties.System.FileDescription.Value;

            /// <summary>
            /// Gets the type name of the current <see cref="ShellObjectInfo"/>. This value corresponds to the description of the file's extension.
            /// </summary>
            protected override string ItemTypeNameOverride => _shellObject.Properties.System.ItemTypeText.Value;

            /// <summary>
            /// Gets a value that indicates whether the current item has particularities.
            /// </summary>
            protected override bool IsSpecialItemOverride
            {
                get
                {
                    uint? value = _shellObject.Properties.System.FileAttributes.Value;

                    if (value.HasValue)
                    {
                        var _value = (Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes)value.Value;

                        return _value.HasFlag(Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes.Hidden) || _value.HasFlag(Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes.System);
                    }

                    else

                        return false;
                }
            }

            /// <summary>
            /// Gets the localized name of this <see cref="ShellObjectInfo"/> depending the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
            /// </summary>
            public override string LocalizedName => _shellObject.GetDisplayName(DisplayNameType.Default);

            /// <summary>
            /// Gets the name of this <see cref="ShellObjectInfo"/> depending of the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
            /// </summary>
            public override string Name => _shellObject.Name;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => ShellObjectPropertySystemCollection._GetShellObjectPropertySystemCollection(this);

            protected override IBrowsableObjectInfo ParentOverride => _parent
#if CS8
                ??=
#else
                ?? (_parent =
#endif
                GetParent()
#if !CS8
                )
#endif
                ;

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => DefaultCustomProcessesSelectorDictionary.Select(this);
            #endregion
            #endregion

            ///// <summary>
            ///// Initializes a new instance of the <see cref="ShellObjectInfo"/> class with a given <see cref="FileType"/> and <see cref="SpecialFolder"/> using custom factories for <see cref="ShellObjectInfo"/>s and <see cref="ArchiveItemInfo"/>s.
            ///// </summary>
            ///// <param name="path">The path of this <see cref="ShellObjectInfo"/>.</param>
            ///// <param name="fileType">The file type of this <see cref="ShellObjectInfo"/>.</param>
            ///// <param name="specialFolder">The special folder type of this <see cref="ShellObjectInfo"/>. <see cref="WinCopies.IO.SpecialFolder.None"/> if this <see cref="ShellObjectInfo"/> is a casual file system item.</param>
            ///// <param name="shellObject">The <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/> that this <see cref="ShellObjectInfo"/> represents.</param>
            protected ShellObjectInfo(in string path, in ShellObject shellObject, in ClientVersion clientVersion) : base(path, clientVersion) => _shellObject = shellObject;

            #region Methods
            #region Archive
            public void OpenArchive(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int? bufferSize, FileOptions fileOptions) => ArchiveFileStream = ObjectPropertiesGeneric.FileType == Archive
                    ? ArchiveFileStream == null
                        ? new FileStream(Path, fileMode, fileAccess, fileShare, bufferSize ?? 4096, fileOptions)
                        : throw new InvalidOperationException("The archive is not open.")
                    : throw new InvalidOperationException("The current item is not an archive.");

            public void CloseArchive()
            {
                if (ObjectPropertiesGeneric.FileType == Archive)
                {
                    if (ArchiveFileStream == null)

                        return;

                    ArchiveFileStream.Dispose();

                    ArchiveFileStream = null;
                }

                else

                    throw new InvalidOperationException("The current item is not an archive.");
            }
            #endregion

            #region Overrides
            protected override void DisposeUnmanaged()
            {
                _parent = null;

                if (IsArchiveOpen)

                    CloseArchive();

                if (_bitmapSourceProvider != null)
                {
                    _bitmapSourceProvider.Dispose();
                    _bitmapSourceProvider = null;
                }

                base.DisposeUnmanaged();
            }
            protected override void DisposeManaged()
            {
                _shellObject.Dispose();
                _shellObject = null;

                _browsability = null;

                base.DisposeManaged();
            }

            /// <summary>
            /// Gets a string representation of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            /// <returns>The <see cref="LocalizedName"/> of this <see cref="ShellObjectInfo"/>.</returns>
            public override string ToString() => string.IsNullOrEmpty(Path) ? _shellObject.GetDisplayName(DisplayNameType.Default) : SystemPath.GetFileName(Path);
            #endregion

            /// <summary>
            /// Returns the parent of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            /// <returns>The parent of this <see cref="ShellObjectInfo"/>.</returns>
            private IBrowsableObjectInfo GetParent()
            {
                bool equalsFileType()
                {
                    switch (ObjectPropertiesGeneric.FileType)
                    {
                        case Folder:
                        case Archive:

                            return true;

                        case KnownFolder:

                            return _shellObject.IsFileSystemObject || ((ShellNonFileSystemFolder)_shellObject).ParsingName != Computer.ParsingName;
                    }

                    return false;
                }

                return equalsFileType()
                    ? From(_shellObject.Parent, ClientVersion)
                    : ObjectPropertiesGeneric.FileType == Drive
                        ? new ShellObjectInfo(Computer.Path, KnownFolder, ShellObjectFactory.Create(Computer.ParsingName), ClientVersion)
                        : null;
            }
            #endregion
        }

        public class ShellObjectInfo : ShellObjectInfo<IFileSystemObjectInfoProperties, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>, IShellObjectInfo
        {
            #region Fields
            private static readonly BrowsabilityPathStack<IShellObjectInfo> __browsabilityPathStack = new
#if !CS9
                BrowsabilityPathStack<IShellObjectInfo>
#endif
                ();

            private IFileSystemObjectInfoProperties _objectProperties;
            private IProcessFactory _processFactory;
            #endregion

            #region Properties
            #region Static Properties
            public static IBrowsabilityPathStack<IShellObjectInfo> BrowsabilityPathStack { get; } = __browsabilityPathStack.AsWriteOnly();

            public static ISelectorDictionary<IShellObjectInfoBase2, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IShellObjectInfoBase2, System.Collections.Generic.IEnumerable<IProcessInfo>>();

            public static IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new ShellObjectInfoSelectorDictionary();

            public static Action RegisterProcessSelectors { get; private set; } = () =>
            {
                DefaultProcessSelectorDictionary.Push(item => Predicate(item, typeof(Guids.Process.Shell))
                                    , TryGetProcess

                // System.Reflection.Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(t => t.Namespace.StartsWith(typeof(Process.ObjectModel.IProcess).Namespace) && t.GetCustomAttribute<ProcessGuidAttribute>().Guid == guid);
                );

                RegisterProcessSelectors = EmptyVoid;
            };
            #endregion Static Properties

            #region Overrides
            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

            protected sealed override IFileSystemObjectInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

            protected sealed override IProcessFactory ProcessFactoryOverride => _processFactory;
            #endregion Overrides
            #endregion Properties

            #region Constructors
            public ShellObjectInfo(in string path, FileType fileType, in ShellObject shellObject, in ClientVersion clientVersion) : base(path, shellObject, clientVersion)
            {
                _objectProperties = GetDefaultProperties(this, fileType);

                _processFactory = new ShellObjectInfoProcessFactory(this);
            }

            public ShellObjectInfo(in IKnownFolder knownFolder, in ClientVersion clientVersion) : this(string.IsNullOrEmpty(knownFolder.Path) ? knownFolder.ParsingName : knownFolder.Path, FileType.KnownFolder, ShellObjectFactory.Create(knownFolder.ParsingName), clientVersion)
            {
                // Left empty.
            }
            #endregion

            #region Methods
            public static IFileSystemObjectInfoProperties GetDefaultProperties(IShellObjectInfoBase2 shellObjectInfo, FileType fileType)
            {
                IFileSystemObjectInfoProperties getFolderProperties() => new FolderShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType);
                IFileSystemObjectInfoProperties getFileSystemObjectInfoProperties() => new FileSystemObjectInfoProperties(shellObjectInfo, fileType);
#if CS9
                return fileType switch
                {
                    Folder => getFolderProperties(),
                    FileType.File or Archive or Library or Link => new FileShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType),
                    KnownFolder => shellObjectInfo.InnerObject.IsFileSystemObject ? getFolderProperties() : getFileSystemObjectInfoProperties(),
                    Drive => new DriveShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType),
                    _ => getFileSystemObjectInfoProperties()
                };
#else
                switch (fileType)
                {
                    case Folder:

                        return getFolderProperties();

                    case FileType.File:
                    case Archive:
                    case Library:
                    case Link:

                        return new FileShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType);

                    case KnownFolder:

                        return shellObjectInfo.InnerObject.IsFileSystemObject ? getFolderProperties() : getFileSystemObjectInfoProperties();

                    case Drive:

                        return new DriveShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType);

                    default:

                        return getFileSystemObjectInfoProperties();
                }
#endif
            }

            public static ShellObjectInitInfo GetInitInfo(in ShellObject shellObject)
            {
                switch (shellObject ?? throw GetArgumentNullException(nameof(shellObject)))
                {
                    case ShellFolder _:

                        switch (shellObject)
                        {
                            case ShellFileSystemFolder shellFileSystemFolder:

                                (string path, FileType fileType) = shellFileSystemFolder is FileSystemKnownFolder ? (shellObject.ParsingName, KnownFolder) : (shellFileSystemFolder.Path, FileType.Folder);

                                return new ShellObjectInitInfo(path, System.IO.Directory.GetParent(path) is null ? Drive : fileType);

                            case NonFileSystemKnownFolder nonFileSystemKnownFolder:

                                return new ShellObjectInitInfo(nonFileSystemKnownFolder.Path, KnownFolder);

                            case ShellNonFileSystemFolder _:

                                return new ShellObjectInitInfo(shellObject.ParsingName, KnownFolder);
                        }

                        break;

                    case ShellLink shellLink:

                        return new ShellObjectInitInfo(shellLink.Path, Link);

                    case ShellFile shellFile:

                        return new ShellObjectInitInfo(shellFile.Path, IsSupportedArchiveFormat(SystemPath.GetExtension(shellFile.Path)) ? Archive : shellFile.IsLink ? Link : SystemPath.GetExtension(shellFile.Path) == ".library-ms" ? Library : FileType.File);
                }

                throw new ArgumentException(ShellObjectIsNotSupported);
            }

            public static IProcess GetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => TryGetProcess(processParameters) ?? throw new InvalidOperationException(NoProcessCouldBeGenerated);

            public static IProcess TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters)
            {
                string guid = processParameters.ProcessParameters.Guid.ToString();

                System.Collections.Generic.IEnumerator<string> enumerator = null;

                try
                {
                    enumerator = processParameters.ProcessParameters.Parameters.GetEnumerator();

                    PathTypes<IPathInfo>.RootPath getParameter() => enumerator.MoveNext()
                                        ? new PathTypes<IPathInfo>.RootPath(enumerator.Current, true)
                                        : throw new InvalidOperationException(ProcessParametersCouldNotBeParsedCorrectly);

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
                        case Guids.Process.Shell.Copy:

                            PathTypes<IPathInfo>.PathInfoBase destinationPath;

                            sourcePath = getParameter();
                            destinationPath = getParameter();

                            IProcess getCopyProcess()
                            {
                                _ = enumerator.MoveNext();

                                bool? option;

                                switch (enumerator.Current)
                                {
                                    case "1":

                                        option = true;

                                        break;

                                    case "0":

                                        option = false;

                                        break;

                                    default:

                                        option = null;

                                        break;
                                }

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

                        case Guids.Process.Shell.Deletion:

                            sourcePath = getParameter();

                            IProcess getDeletionProcess()
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

                if (_processFactory != null)
                {
                    _processFactory.Dispose();
                    _processFactory = null;
                }

                if (_objectProperties != null)
                {
                    _objectProperties.Dispose();
                    _objectProperties = null;
                }
            }

            #region GetItems
            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ArchiveFileInfoEnumeratorStruct> func) => func == null ? GetItems(GetItemProviders(item => true)) : GetItems(GetItemProviders(func));

            protected override System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetItemProviders(Predicate<ShellObjectInfoEnumeratorStruct> predicate) => Browsability.Browsability.IsBrowsable()
                ? ObjectPropertiesGeneric?.FileType == Archive
                    ? GetItemProviders(item => predicate(new ShellObjectInfoEnumeratorStruct(item)))
                    : ShellObjectInfoEnumeration.From(this, predicate)
                : GetEmptyEnumerable();

            protected virtual System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetItemProviders(Predicate<ArchiveFileInfoEnumeratorStruct> func)
#if CS8
             => ObjectPropertiesGeneric.FileType switch
             {
                 Archive => ArchiveItemInfo.GetArchiveItemInfoItems(this, func).Select(ShellObjectInfoItemProvider.ToShellObjectInfoItemProvider),
                 _ => GetEmptyEnumerable()
             };
#else
            {
                switch (ObjectPropertiesGeneric.FileType)
                {
                    case Archive:
                        return ArchiveItemInfo.GetArchiveItemInfoItems(this, func).Select(ShellObjectInfoItemProvider.ToShellObjectInfoItemProvider);
                    default:
                        return GetEmptyEnumerable();
                }
            }
#endif

            protected override System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetItemProviders() => GetItemProviders((Predicate<ShellObjectInfoEnumeratorStruct>)(obj => true));
            #endregion // GetItems
            #endregion // Methods
        }
    }
}
