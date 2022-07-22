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

#region System
using System;
using System.Drawing;
using System.Reflection;
using System.Security;
using System.Windows.Media.Imaging;
#endregion System

#region WinCopies
using WinCopies.Collections;
using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Enumeration;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.PropertySystem;
using WinCopies.Util;
#endregion WinCopies

#region Static Usings
using static WinCopies.IO.DotNetAssemblyInfoHandledExtensions;
using static WinCopies.IO.Shell.Resources.ExceptionMessages;
using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
#endregion Static Usings
#endregion Usings

namespace WinCopies.IO
{
    public static class DotNetAssemblyInfoHandledExtensions
    {
        public const string Exe = ".exe";
        public const string Dll = ".dll";
    }

    namespace ObjectModel.Reflection
    {
        public class BrowsableObjectInfoPlugin : IO.BrowsableObjectInfoPlugin
        {
            public override IBitmapSourceProvider BitmapSourceProvider => null;

            public BrowsableObjectInfoPlugin() => RegisterBrowsabilityPathsStack.Push(DotNetAssemblyInfo.RegisterDefaultBrowsabilityPaths);

            public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetProtocols(IBrowsableObjectInfo parent, ClientVersion clientVersion) => null;
            public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetStartPages(ClientVersion clientVersion) => null;
        }

        public static class BrowsableObjectInfo
        {
            public static IBrowsableObjectInfoPlugin GetPlugInParameters() => new BrowsableObjectInfoPlugin();
        }

        public class DotNetAssemblyInfo : BrowsableObjectInfo<IBrowsableObjectInfo, Shell.PropertySystem.IFileSystemObjectInfoProperties, Assembly, DotNetNamespaceInfoItemProvider, IEnumerableSelectorDictionary<DotNetNamespaceInfoItemProvider, IBrowsableObjectInfo>, DotNetNamespaceInfoItemProvider>, IDotNetAssemblyInfo // <IFileSystemObjectInfoProperties>
        {
            #region Subtypes
            protected internal class BrowsabilityPath : IBrowsabilityPath<IShellObjectInfo>
            {
                string IBrowsabilityPathBase.Name => ".Net Parser";

                IBrowsableObjectInfo IBrowsabilityPath<IShellObjectInfo>.GetPath(IShellObjectInfo browsableObjectInfo) => new DotNetAssemblyInfo(browsableObjectInfo, browsableObjectInfo.ClientVersion);

                bool IBrowsabilityPath<IShellObjectInfo>.IsValidFor(IShellObjectInfo browsableObjectInfo)
                {
                    if (browsableObjectInfo.InnerObject is ShellFile shellFile && shellFile.Path.EndsWithOR(Exe, Dll))

                        try
                        {
                            _ = Assembly.LoadFrom(browsableObjectInfo.Path);

                            return true;
                        }

                        catch (Exception ex) when (ex.Is(false, typeof(FileNotFoundException), typeof(FileLoadException), typeof(BadImageFormatException), typeof(SecurityException), typeof(PathTooLongException)))
                        {

                        }

                    return false;
                }
            }

            public class ItemSource : ItemSourceBase3<DotNetAssemblyInfo, DotNetNamespaceInfoItemProvider, IEnumerableSelectorDictionary<DotNetNamespaceInfoItemProvider, IBrowsableObjectInfo>, DotNetNamespaceInfoItemProvider>
            {
                private IProcessSettings
#if CS8
                    ?
#endif
                    _processSettings;

                public override bool IsPaginationSupported => false;

                public override bool IsDisposed => _processSettings == null;

                protected sealed override IProcessSettings
#if CS8
                    ?
#endif
                    ProcessSettingsOverride => _processSettings;

                public ItemSource(in DotNetAssemblyInfo browsableObjectInfo) : base(browsableObjectInfo) => _processSettings = browsableObjectInfo.ItemSources?.Default.ProcessSettings;

                protected internal virtual System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoItemProvider> func) => DotNetNamespaceInfoEnumeration.From(BrowsableObjectInfo, typesToEnumerate, func);

                protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders(Predicate<DotNetNamespaceInfoItemProvider> predicate) => BrowsableObjectInfo.GetItemProviders(predicate);

                protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders() => GetItemProviders(GetDefaultItemTypes(), null);


                internal new System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                        ?
#endif
                    GetItems(System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider>
#if CS8
                        ?
#endif
                    items) => base.GetItems(items);

                protected override void DisposeManaged() => TryDispose(ref _processSettings);
            }
            #endregion

            private Assembly _assembly;
            private ShellObjectInfo _shellObjectInfo;
            private readonly ItemSource _itemSource;

            #region Properties
            //public override Predicate<DotNetNamespaceInfoItemProvider> RootItemsPredicate => item => !IsNullEmptyOrWhiteSpace(item.NamespaceName);

            //public override Predicate<IBrowsableObjectInfo> RootItemsBrowsableObjectInfoPredicate => null;

            public static Action RegisterDefaultBrowsabilityPaths { get; private set; } = () =>
            {
                ObjectModel.ShellObjectInfo.BrowsabilityPathStack.Push(new BrowsabilityPath());

                RegisterDefaultBrowsabilityPaths = Delegates.EmptyVoid;
            };

            protected override IItemSourcesProvider<DotNetNamespaceInfoItemProvider>
#if CS8
                ?
#endif
                ItemSourcesGenericOverride
            { get; }

            /// <inheritdoc/>
            protected override bool IsLocalRootOverride => false;

            protected IShellObjectInfo ShellObjectInfo => GetValueIfNotDisposed(_shellObjectInfo);

            protected override Assembly InnerObjectGenericOverride => _assembly;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.Browsable;

            protected sealed override bool IsRecursivelyBrowsableOverride => false;

            protected override Shell.PropertySystem.IFileSystemObjectInfoProperties ObjectPropertiesGenericOverride { get; }

            protected override IBitmapSourceProvider BitmapSourceProviderOverride => _shellObjectInfo.BitmapSourceProvider;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => _shellObjectInfo.BrowsabilityPaths;

            protected override string
#if CS8
                ?
#endif
                DescriptionOverride => _shellObjectInfo.Description;

            protected override bool IsSpecialItemOverride => _shellObjectInfo.IsSpecialItem;

            protected override string ItemTypeNameOverride => _shellObjectInfo.ItemTypeName;

            /// <inheritdoc/>
            public override string LocalizedName => _shellObjectInfo.LocalizedName;

            /// <inheritdoc/>
            public override string Name => _shellObjectInfo.Name;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride => _shellObjectInfo.ObjectPropertySystem;

            protected override IBrowsableObjectInfo ParentGenericOverride => _shellObjectInfo.Parent;

            IShellObjectInfoBase IArchiveItemInfoProvider.ArchiveShellObject => ShellObjectInfo.ArchiveShellObject;

            IFileSystemObjectInfoProperties IBrowsableObjectInfo<IFileSystemObjectInfoProperties>.ObjectProperties => ShellObjectInfo.ObjectProperties;

            ShellObject IEncapsulatorBrowsableObjectInfo<ShellObject>.InnerObject => ShellObjectInfo.InnerObject;

            bool IShellObjectInfoBase.IsArchiveOpen => false;

            IItemSourcesProvider<ShellObjectInfoEnumeratorStruct>
#if CS8
                ?
#endif
                IBrowsableObjectInfo2<ShellObjectInfoEnumeratorStruct>.ItemSources => ShellObjectInfo.ItemSources;
            #endregion Properties

            protected DotNetAssemblyInfo(in IShellObjectInfo shellObjectInfo, in ClientVersion clientVersion) : base(shellObjectInfo.Path, clientVersion)
            {
                ObjectPropertiesGenericOverride = new FileShellObjectInfoProperties<IShellObjectInfoBase2>(this, FileType.File);

                ItemSourcesGenericOverride = ItemSourcesProvider.Construct(_itemSource = new ItemSource(this));
            }

            #region Methods
            public static DotNetAssemblyInfo From(in ShellObject shellObject, in ClientVersion clientVersion)
            {
                ShellObjectInitInfo initInfo = ObjectModel.ShellObjectInfo.GetInitInfo(shellObject);

                return initInfo.FileType == FileType.File ? initInfo.Path.EndsWithOR(Exe, Dll) ? new DotNetAssemblyInfo(new ShellObjectInfo(initInfo.Path, initInfo.FileType, shellObject, clientVersion), clientVersion) : throw new ArgumentException($"{nameof(shellObject)} must be an exe (.exe) or a dll (.dll).") : throw new ArgumentException(GivenShellObjectIsNotAFile, nameof(shellObject));
            }

            StreamInfo IShellObjectInfoBase.GetArchiveFileStream() => ShellObjectInfo.GetArchiveFileStream();

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride()
            {
                var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

                _ = arrayBuilder.AddLast(new DotNetAssemblyInfo(ObjectModel.ShellObjectInfo.From(_shellObjectInfo.InnerObjectGeneric, _shellObjectInfo.ClientVersion), ClientVersion));

                return arrayBuilder;
            }

            protected override IEnumerableSelectorDictionary<DotNetNamespaceInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DotNetNamespaceInfo.DefaultItemSelectorDictionary;

            public void OpenAssembly()
            {
                ThrowIfDisposed(this);

                try
                {
                    _assembly = Assembly.LoadFrom(Path);
                }

                catch (Exception ex) when (ex.Is(false, typeof(FileNotFoundException), typeof(FileLoadException), typeof(BadImageFormatException), typeof(SecurityException)))
                {
                    _assembly = null;
                }
            }

            public void CloseAssembly()
            {
                ThrowIfDisposed(this);

                _assembly = null;
            }

            public static System.Collections.Generic.IEnumerable<DotNetItemType> GetDefaultItemTypes() => new DotNetItemType[] { DotNetItemType.Namespace, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate };

            protected internal virtual System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders(Predicate<DotNetNamespaceInfoItemProvider> predicate) => _itemSource.GetItemProviders(GetDefaultItemTypes(), predicate);

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetSubRootItemsOverride() => _itemSource.GetItems(GetItemProviders(item => !IsNullEmptyOrWhiteSpace(item.NamespaceName)));

            protected override void DisposeManaged()
            {
                CloseAssembly();

                base.DisposeManaged();
            }

            protected override void DisposeUnmanaged()
            {
                _shellObjectInfo = null;

                base.DisposeUnmanaged();
            }

            void IShellObjectInfoBase.OpenArchive(System.IO.FileMode fileMode, System.IO.FileAccess fileAccess, System.IO.FileShare fileShare, int? bufferSize, System.IO.FileOptions fileOptions) => ShellObjectInfo.OpenArchive(fileMode, fileAccess, fileShare, bufferSize, fileOptions);

            void IShellObjectInfoBase.CloseArchive() => ShellObjectInfo.CloseArchive();

            Icon IFileSystemObjectInfo.TryGetIcon(in int size) => ShellObjectInfo.TryGetIcon(size);

            BitmapSource IFileSystemObjectInfo.TryGetBitmapSource(in int size) => ShellObjectInfo.TryGetBitmapSource(size);

            IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo> IBrowsableObjectInfo<ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>.GetSelectorDictionary() => ShellObjectInfo.GetSelectorDictionary();

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetItems(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoItemProvider> func) => _itemSource.GetItems(_itemSource.GetItemProviders(typesToEnumerate, func));
            #endregion
        }
    }
}
