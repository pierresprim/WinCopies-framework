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
using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Extensions;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Enumeration;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;
using WinCopies.PropertySystem;
using WinCopies.Util;

using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.IO.Path;
using static WinCopies.ThrowHelper;
using static WinCopies.UtilHelpers;

namespace WinCopies.IO
{
    public struct ShellObjectInitInfo
    {
        public string Path { get; }

        public FileType FileType { get; }

        public ShellObjectInitInfo(string path, FileType fileType)
        {
            Path = path;

            FileType = fileType;
        }
    }

    public sealed class ShellLinkBrowsabilityOptions : IBrowsabilityOptions
    {
        private readonly IShellObjectInfoBase _shellObjectInfo;

        public Browsability Browsability => Browsability.RedirectsToBrowsableItem;

        public ShellLinkBrowsabilityOptions(in IShellObjectInfoBase shellObjectInfo) => _shellObjectInfo = shellObjectInfo ?? throw GetArgumentNullException(nameof(shellObjectInfo));

        public IBrowsableObjectInfo RedirectToBrowsableItem() => _shellObjectInfo;
    }

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

            #region Properties
            /// <summary>
            /// Gets a <see cref="ShellObject"/> that represents this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public sealed override ShellObject InnerObjectGeneric => _shellObject;

            public Stream ArchiveFileStream { get; private set; }

            public bool IsArchiveOpen => ArchiveFileStream is object;

            #region Overrides
            // public override FileSystemType ItemFileSystemType => FileSystemType.CurrentDeviceFileSystem;

            public override IBrowsabilityOptions Browsability
            {
                get
                {
                    ThrowIfDisposed(this);

                    if (_browsability == null)

                        if (InnerObjectGeneric is ShellLink shellLink)
                        {
                            ShellObjectInfo targetShellObjectInfo = ShellObjectInfo.From(shellLink.TargetShellObject, ClientVersion);

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

#if NETFRAMEWORK
            public override IBrowsableObjectInfo Parent => _parent ?? (_parent = GetParent());
#else
            public override IBrowsableObjectInfo Parent => _parent ??= GetParent();
#endif

            /// <summary>
            /// Gets the localized name of this <see cref="ShellObjectInfo"/> depending the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
            /// </summary>
            public override string LocalizedName => _shellObject.GetDisplayName(DisplayNameType.Default);

            /// <summary>
            /// Gets the name of this <see cref="ShellObjectInfo"/> depending of the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
            /// </summary>
            public override string Name => _shellObject.Name;

            #region BitmapSources
            /// <summary>
            /// Gets the small <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public override BitmapSource SmallBitmapSource => _shellObject.Thumbnail.SmallBitmapSource;

            /// <summary>
            /// Gets the medium <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public override BitmapSource MediumBitmapSource => _shellObject.Thumbnail.MediumBitmapSource;

            /// <summary>
            /// Gets the large <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public override BitmapSource LargeBitmapSource => _shellObject.Thumbnail.LargeBitmapSource;

            /// <summary>
            /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            public override BitmapSource ExtraLargeBitmapSource => _shellObject.Thumbnail.ExtraLargeBitmapSource;
            #endregion

            /// <summary>
            /// Gets the type name of the current <see cref="ShellObjectInfo"/>. This value corresponds to the description of the file's extension.
            /// </summary>
            public override string ItemTypeName => _shellObject.Properties.System.ItemTypeText.Value;

            public override string Description => _shellObject.Properties.System.FileDescription.Value;

            /// <summary>
            /// Gets a value that indicates whether the current item has particularities.
            /// </summary>
            public override bool IsSpecialItem
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
            /// The parent <see cref="IShellObjectInfoBase"/> of the current archive item. If the current <see cref="ShellObjectInfo"/> represents an archive file, this property returns the current <see cref="ShellObjectInfo"/>, or <see langword="null"/> otherwise.
            /// </summary>
            public override IShellObjectInfoBase ArchiveShellObject => ObjectPropertiesGeneric.FileType == FileType.Archive ? this : null;

#if WinCopies3
            public override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystem => ShellObjectPropertySystemCollection._GetShellObjectPropertySystemCollection(this);
#endif
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
            public void OpenArchive(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int? bufferSize, FileOptions fileOptions)
            {
                if (ObjectPropertiesGeneric.FileType == FileType.Archive)

                    ArchiveFileStream = ArchiveFileStream == null
                        ? (Stream)new FileStream(Path, fileMode, fileAccess, fileShare, bufferSize.HasValue ? bufferSize.Value : 4096, fileOptions)
                        : throw new InvalidOperationException("The archive is not open.");

                else

                    throw new InvalidOperationException("The current item is not an archive.");
            }

            public void CloseArchive()
            {
                if (ObjectPropertiesGeneric.FileType == FileType.Archive)
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
            protected override void DisposeManaged()
            {
                base.DisposeManaged();

                _shellObject.Dispose();

                _shellObject = null;

                if (IsArchiveOpen)

                    CloseArchive();

                _browsability = null;
            }

            /// <summary>
            /// Gets a string representation of this <see cref="ShellObjectInfo"/>.
            /// </summary>
            /// <returns>The <see cref="LocalizedName"/> of this <see cref="ShellObjectInfo"/>.</returns>
            public override string ToString() => string.IsNullOrEmpty(Path) ? _shellObject.GetDisplayName(DisplayNameType.Default) : System.IO.Path.GetFileName(Path);
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
                        case FileType.Folder:
                        case FileType.Archive:

                            return true;

                        case FileType.KnownFolder:

                            return _shellObject.IsFileSystemObject || ((IKnownFolder)_shellObject).FolderId.ToString() != Microsoft.WindowsAPICodePack.Shell.Guids.KnownFolders.Computer;
                    }

                    return false;
                }

                return equalsFileType()
                    ? ShellObjectInfo.From(_shellObject.Parent, ClientVersion)
                    : ObjectPropertiesGeneric.FileType == FileType.Drive
                        ? new ShellObjectInfo(Computer.Path, FileType.KnownFolder, ShellObjectFactory.Create(Computer.ParsingName), ClientVersion)
                        : null;
            }
            #endregion
        }

        public class ShellObjectInfo : ShellObjectInfo<IFileSystemObjectInfoProperties, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>, IShellObjectInfo
        {
            public static IProcess TryGetProcess(in ProcessFactorySelectorDictionaryParameters processParameters, uint count)
            {
                string guid = processParameters.ProcessParameters.Guid.ToString();

                System.Collections.Generic.IEnumerator<string> enumerator = null;

                try
                {
                    switch (guid)
                    {
                        case Guids.ShellCopyProcessGuid:

                            enumerator = processParameters.ProcessParameters.Parameters.GetEnumerator();

                            PathTypes<IPathInfo>.RootPath getParameter()
                            {
                                _ = enumerator.MoveNext();

                                return new PathTypes<IPathInfo>.RootPath(enumerator.Current, true);
                            }

                            IPathInfo sourcePath, destinationPath;

                            sourcePath = getParameter();
                            destinationPath = getParameter();

                            return new CopyProcess<ProcessErrorFactory<IPathInfo>>(
                                new ReadOnlyEnumerableQueueCollection<IPathInfo>(
                                    new Enumerable<string>(
                                        () => enumerator
                                        ).Select(
                                            path =>
                                            {
                                                if (path.EndsWith(":\\") || path.EndsWith(":\\\\"))

                                                    return new PathTypes<IPathInfo>.RootPath(path, true);

                                                return (IPathInfo)new PathTypes<IPathInfo>.PathInfo(System.IO.Path.GetFileName(path), sourcePath);
                                            })
                                        .ToEnumerableQueue()),
                                sourcePath,
                                destinationPath,
                                processParameters.Factory.GetProcessCollection<IPathInfo>(),
                                processParameters.Factory.GetProcessLinkedList<
                                    IPathInfo,
                                    ProcessError>(),
                                new ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>
                                .ProcessDelegates(
                                    new EventDelegate<IPathInfo>(),
                                    EventAndQueryDelegate<bool>.GetANDALSO_Delegate(true),
                                    EventAndQueryDelegate<object>.GetANDALSO_Delegate(false),
                                    EventAndQueryDelegate<IProcessProgressDelegateParameter>.GetANDALSO_Delegate(true)),
                                new ProcessTypes<IPathInfo>.ProcessErrorTypes<ProcessError>.ProcessOptions(path => true, true),
                                new ProcessErrorFactory<IPathInfo>());
                    }
                }

                finally
                {
                    enumerator?.Dispose();
                }

                return null;
            }

            public static IProcess GetProcess(ProcessFactorySelectorDictionaryParameters processParameters, uint count) => TryGetProcess(processParameters, count) ?? throw new InvalidOperationException("No process could be generated.");

            public static class Guids
            {
                public const string ShellCopyProcessGuid = "084ff8d5-c66e-40bc-8ea4-7fa2dd30bd21";
            }

            public static Action RegisterProcessSelectors { get; private set; } = () =>
            {
                DefaultProcessSelectorDictionary.Push(item =>
                {
                    string guid = item.ProcessParameters.Guid.ToString();

                    foreach (FieldInfo f in typeof(Guids).GetFields())

                        if ((string)f.GetValue(null) == guid)

                            return true;

                    return false;
                }, _item =>
                {
                    string guid = _item.ToString();

                    return TryGetProcess(_item, 10);

                    // System.Reflection.Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(t => t.Namespace.StartsWith(typeof(Process.ObjectModel.IProcess).Namespace) && t.GetCustomAttribute<ProcessGuidAttribute>().Guid == guid);
                });

                RegisterProcessSelectors = EmptyVoid;
            };


            private class _ProcessFactory : IProcessFactory
            {
                private class ProcessParameters : IProcessParameters
                {
                    public Guid Guid { get; }

                    public System.Collections.Generic.IEnumerable<string> Parameters { get; }

                    public ProcessParameters(in string processGuid, in string sourcePath, in string destinationPath, StringCollection sc)
                    {
                        Guid = new Guid(processGuid);

                        Parameters = new Enumerable<string>(() => new Collections.StringEnumerator(sc, Collections.DotNetFix.EnumerationDirection.FIFO)).PrependValues(sourcePath, destinationPath);
                    }
                }

                private readonly IShellObjectInfoBase2 _path;

                public _ProcessFactory(in IShellObjectInfoBase2 path) => _path = path;

                public bool CanCopy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths)
                {
                    var enumerator = new EmptyCheckEnumerator<IBrowsableObjectInfo>((paths ?? throw GetArgumentNullException(nameof(paths))).GetEnumerator());

                    if (enumerator.HasItems)
                    {
                        var enumerable = new CustomEnumeratorEnumerable<IBrowsableObjectInfo, System.Collections.Generic.IEnumerator<IBrowsableObjectInfo>>(enumerator);

                        foreach (IBrowsableObjectInfo path in enumerable)

                            if (!(path is IShellObjectInfoBase2 shellObjectInfo
                                && shellObjectInfo.InnerObject.IsFileSystemObject
                                && shellObjectInfo.Path.Validate(_path.Path, _path.Path.EndsWith('\\') ? 1 : 0, null, null, 1, "\\")))

                                return false;

                        return true;
                    }

                    return false;
                }

                private static bool _Copy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, in uint count)
                {
                    ThrowIfNull(paths, nameof(paths));

                    var sc = new StringCollection();

                    sc.AddRange(new ArrayBuilder<string>(paths.As<IShellObjectInfoBase2>().Select(path => path.Path)).ToArray());

                    for (int i = 0; i < count; i++)

                        try
                        {
                            System.Windows.Clipboard.SetFileDropList(sc);

                            return true;
                        }

                        catch
                        {
                            // Left empty.
                        }

                    return false;
                }

                public void Copy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count)
                {
                    if (!(CanCopy(paths) && _Copy(paths, count)))

                        throw new InvalidOperationException("The copy operation has not succeeded.");
                }

                public bool TryCopy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => CanCopy(paths) && _Copy(paths, count);

                public static StringCollection TryGetFileDropList(uint count)
                {
                    uint i = 0;

                    StringCollection sc = null;

                    _ = For(() => i < count, () => sc = System.Windows.Clipboard.GetFileDropList(), () => i++);

                    return sc;
                }

                public static string CanPaste(in uint count, out StringCollection sc)
                {
                    sc = TryGetFileDropList(count);

                    if (sc == null || sc.Count == 0)

                        return null;

                    string sourcePath = null;

                    Predicate<string> predicate;

                    StringCollection _sc = sc;

                    string getSourcePath(in bool updatePredicate)
                    {
                        if (_sc[0].EndsWith(":\\") || _sc[0].EndsWith(":\\\\"))
                        {
                            if (updatePredicate)

                                predicate = _path => sourcePath == From(_path).Parent.Path;

                            return From(ShellObjectFactory.Create(Computer.ParsingName)).Path;
                        }

                        else
                        {
                            if (updatePredicate)

                                predicate = _path => sourcePath == System.IO.Path.GetDirectoryName(_path);

                            return System.IO.Path.GetDirectoryName(_sc[0]);
                        }

                    }

                    if (sc.Count == 1)

                        return getSourcePath(false);

                    predicate = path =>
                    {
                        sourcePath = getSourcePath(true);

                        return true;
                    };

                    System.Collections.Generic.IEnumerable<string> paths = new Enumerable<string>(() => new Collections.StringEnumerator(_sc, Collections.DotNetFix.EnumerationDirection.FIFO));

                    return paths.ForEachANDALSO(predicate) ? sourcePath : null;
                }

                bool IProcessFactory.CanPaste(uint count) => CanPaste(count, out _) != null;

                public IProcessParameters GetCopyProcessParameters(uint count) => TryGetCopyProcessParameters(count) ?? throw new InvalidOperationException("An unknown error occurred during copy process parameters generation.");

                public IProcessParameters TryGetCopyProcessParameters(uint count)
                {
                    string sourcePath = CanPaste(count, out StringCollection sc);

                    return sourcePath == null
                        ? null
                     : new ProcessParameters(Guids.ShellCopyProcessGuid, sourcePath, _path.Path, sc);
                }

                IProcess IProcessFactory.GetProcess(ProcessFactorySelectorDictionaryParameters processParameters, uint count) => GetProcess(processParameters, count);

                IProcess IProcessFactory.TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters, uint count) => TryGetProcess(processParameters, count);
            }

            private IFileSystemObjectInfoProperties _objectProperties;

            #region Properties
            public override IProcessFactory ProcessFactory { get; }

            public static IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new ShellObjectInfoSelectorDictionary();

            public sealed override IFileSystemObjectInfoProperties ObjectPropertiesGeneric => IsDisposed ? throw GetExceptionForDispose(false) : _objectProperties;
            #endregion // Properties

            #region Constructors
            public ShellObjectInfo(in string path, in FileType fileType, in ShellObject shellObject, in ClientVersion clientVersion) : base(path, shellObject, clientVersion)
            {
#if CS9
                _objectProperties = fileType switch
                {
                    FileType.Folder or FileType.KnownFolder => new FolderShellObjectInfoProperties<IShellObjectInfoBase2>(this, fileType),
                    FileType.File or FileType.Archive or FileType.Library or FileType.Link => new FileShellObjectInfoProperties<IShellObjectInfoBase2>(this, fileType),
                    FileType.Drive => new DriveShellObjectInfoProperties<IShellObjectInfoBase2>(this, fileType),
                    _ => new FileSystemObjectInfoProperties(this, fileType),
                };
#else
                switch (fileType)
                {
                    case FileType.Folder:
                    case FileType.KnownFolder:

                        _objectProperties = new FolderShellObjectInfoProperties<IShellObjectInfoBase2>(this, fileType);

                        break;

                    case FileType.File:
                    case FileType.Archive:
                    case FileType.Library:
                    case FileType.Link:

                        _objectProperties = new FileShellObjectInfoProperties<IShellObjectInfoBase2>(this, fileType);

                        break;

                    case FileType.Drive:

                        _objectProperties = new DriveShellObjectInfoProperties<IShellObjectInfoBase2>(this, fileType);

                        break;

                    default:

                        _objectProperties = new FileSystemObjectInfoProperties(this, fileType);

                        break;
                }
#endif

                ProcessFactory = new _ProcessFactory(this);
            }

            public ShellObjectInfo(in IKnownFolder knownFolder, in ClientVersion clientVersion) : this(string.IsNullOrEmpty(knownFolder.Path) ? knownFolder.ParsingName : knownFolder.Path, FileType.KnownFolder, ShellObjectFactory.Create(knownFolder.ParsingName), clientVersion)
            {
                // Left empty.
            }
            #endregion

            #region Methods
            public static ShellObjectInitInfo GetInitInfo(in ShellObject shellObject)
            {
#if DEBUG
                if (shellObject.ParsingName == Computer.ParsingName)

                    Debug.WriteLine(shellObject.ParsingName);
#endif

                switch (shellObject ?? throw GetArgumentNullException(nameof(shellObject)))
                {
                    case ShellFolder shellFolder:

                        switch (shellObject)
                        {
                            case ShellFileSystemFolder shellFileSystemFolder:

                                (string path, FileType fileType) = shellFileSystemFolder is FileSystemKnownFolder ? (shellObject.ParsingName, FileType.KnownFolder) : (shellFileSystemFolder.Path, FileType.Folder);

                                return new ShellObjectInitInfo(path, System.IO.Directory.GetParent(path) is null ? FileType.Drive : fileType);

                            case NonFileSystemKnownFolder nonFileSystemKnownFolder:

                                return new ShellObjectInitInfo(nonFileSystemKnownFolder.Path, FileType.KnownFolder);

                            case ShellNonFileSystemFolder _:

                                return new ShellObjectInitInfo(shellObject.ParsingName, FileType.Folder);
                        }

                        break;

                    case ShellLink shellLink:

                        return new ShellObjectInitInfo(shellLink.Path, FileType.Link);

                    case ShellFile shellFile:

                        return new ShellObjectInitInfo(shellFile.Path, IsSupportedArchiveFormat(System.IO.Path.GetExtension(shellFile.Path)) ? FileType.Archive : shellFile.IsLink ? FileType.Link : System.IO.Path.GetExtension(shellFile.Path) == ".library-ms" ? FileType.Library : FileType.File);
                }

                throw new ArgumentException($"The given {nameof(ShellObject)} is not supported.");
            }

            public static ShellObjectInfo From(in ShellObject shellObject, in ClientVersion clientVersion)
            {
                ShellObjectInitInfo initInfo = GetInitInfo(shellObject);

                return new ShellObjectInfo(initInfo.Path, initInfo.FileType, shellObject, clientVersion);
            }

            public static ShellObjectInfo From(in ShellObject shellObject) => From(shellObject, GetDefaultClientVersion());

            public static ShellObjectInfo From(in string path, in ClientVersion clientVersion) => From(ShellObjectFactory.Create(path), clientVersion);

            public static ShellObjectInfo From(in string path) => From(path, GetDefaultClientVersion());

            public override IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionary() => DefaultItemSelectorDictionary;

            protected override void DisposeManaged()
            {
                base.DisposeManaged();

                _objectProperties = null;
            }

            #region GetItems
            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ArchiveFileInfoEnumeratorStruct> func) => func == null ? GetItems(GetItemProviders(item => true)) : GetItems(GetItemProviders(func));

            protected override System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetItemProviders(Predicate<ShellObjectInfoEnumeratorStruct> predicate) => Browsability.Browsability.IsBrowsable()
                ? ObjectPropertiesGeneric.FileType == FileType.Archive
                    ? GetItemProviders(item => predicate(new ShellObjectInfoEnumeratorStruct(item)))
                    : ShellObjectInfoEnumeration.From(this, predicate)
                : GetEmptyEnumerable();

            protected virtual System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetItemProviders(Predicate<ArchiveFileInfoEnumeratorStruct> func)
#if NETFRAMEWORK
            {
                switch (ObjectPropertiesGeneric.FileType)
                {
                    case FileType.Archive:
                        return ArchiveItemInfo.GetArchiveItemInfoItems(this, func).Select(ShellObjectInfoItemProvider.ToShellObjectInfoItemProvider);
                    default:
                        return null;
                }
            }
#else
             => ObjectPropertiesGeneric.FileType switch
             {
                 FileType.Archive => ArchiveItemInfo.GetArchiveItemInfoItems(this, func).Select(ShellObjectInfoItemProvider.ToShellObjectInfoItemProvider),
                 _ => GetEmptyEnumerable()
             };
#endif

            protected override System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetItemProviders() => GetItemProviders((Predicate<ShellObjectInfoEnumeratorStruct>)(obj => true));
            #endregion // GetItems
            #endregion // Methods
        }
    }
}
