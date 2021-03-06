﻿/* Copyright © Pierre Sprimont, 2020
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework. If not, see <https://www.gnu.org/licenses/>. */

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;
using WinCopies.PropertySystem;
using WinCopies.Util;

using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;
using static WinCopies.IO.Resources.ExceptionMessages;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// Represents a Windows registry item.
    /// </summary>
    public abstract class RegistryItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>/*<TItems, TFactory>*/ : BrowsableObjectInfo<TObjectProperties, RegistryKey, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>/*<TItems, TFactory>*/, IRegistryItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IRegistryItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo> // where TItems : BrowsableObjectInfo, IRegistryItemInfo where TFactory : IRegistryItemInfoFactory
    {
        private class BrowsableObjectInfoBitmapSources : BrowsableObjectInfoBitmapSources<RegistryItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>>
        {
            protected override BitmapSource SmallBitmapSourceOverride => InnerObject.TryGetBitmapSource(SmallIconSize);

            protected override BitmapSource MediumBitmapSourceOverride => InnerObject.TryGetBitmapSource(MediumIconSize);

            protected override BitmapSource LargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(LargeIconSize);

            protected override BitmapSource ExtraLargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(ExtraLargeIconSize);

            public BrowsableObjectInfoBitmapSources(in RegistryItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> registryItemInfo) : base(registryItemInfo) { /* Left empty. */ }
        }

        #region Fields
        internal NullableReference<RegistryKey> _registryKey;
        private IBrowsableObjectInfo _parent;
        private IBrowsabilityOptions _browsability;
        private IBrowsableObjectInfoBitmapSources _bitmapSources;
        private string _itemTypeName;
        #endregion

        #region Properties
        ///// <summary>
        ///// Gets a value that indicates whether this object needs to reconstruct objects on deep cloning.
        ///// </summary>
        //public override bool NeedsObjectsOrValuesReconstruction => _registryKey is object; // If _registryKey is null, reconstructing registry does not make sense, so we return false.

        //public static DeepClone<RegistryKey> DefaultRegistryKeyDeepClone { get; } = registryKey => Registry.OpenRegistryKey(registryKey.Name);

        protected override bool IsSpecialItemOverride => false;

        protected override string ItemTypeNameOverride
        {
            get
            {
                if (string.IsNullOrEmpty(_itemTypeName))

                    switch (ObjectPropertiesGeneric.RegistryItemType)
                    {
                        case RegistryItemType.Root:

                            _itemTypeName = Properties.Resources.RegistryRoot;

                            break;

                        case RegistryItemType.Key:

                            _itemTypeName = Properties.Resources.RegistryKey;

                            break;

                        case RegistryItemType.Value:

                            _itemTypeName = Properties.Resources.RegistryValue;

                            break;
                    }

                return _itemTypeName;
            }
        }

        protected override string DescriptionOverride => NotApplicable;

        /// <summary>
        /// Gets the localized path of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override string LocalizedName => Name;

        /// <summary>
        /// Gets the name of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override string Name { get; }

        protected override IBrowsableObjectInfoBitmapSources BitmapSourcesOverride => _bitmapSources
#if CS8
            ??=
#else
            ?? (_bitmapSources =
#endif
            new BrowsableObjectInfoBitmapSources(this)
#if !CS8
            )
#endif
            ;

        /// <summary>
        /// Gets a value that indicates whether this <see cref="RegistryItemInfo"/> is browsable.
        /// </summary>
        protected override IBrowsabilityOptions BrowsabilityOverride
        {
            get
            {
                if (_browsability == null)
                {
#if CS8
                    _browsability = ObjectPropertiesGeneric.RegistryItemType switch
                    {
#if CS9
                        RegistryItemType.Key or RegistryItemType.Root => BrowsabilityOptions.BrowsableByDefault,
#else
                        RegistryItemType.Key => BrowsabilityOptions.BrowsableByDefault,
                        RegistryItemType.Root => BrowsabilityOptions.BrowsableByDefault,
#endif
                        _ => BrowsabilityOptions.NotBrowsable,
                    };
#else

                    switch (ObjectPropertiesGeneric.RegistryItemType)
                    {
                        case RegistryItemType.Root:
                        case RegistryItemType.Key:

                            _browsability = BrowsabilityOptions.BrowsableByDefault;

                            break;

                        default:

                            _browsability = BrowsabilityOptions.NotBrowsable;

                            break;
                    }
#endif
                }

                return _browsability;
            }
        }

        protected override bool IsRecursivelyBrowsableOverride => true;

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

        protected sealed override RegistryKey InnerObjectGenericOverride => (_registryKey
#if CS8
            ??=
#else
            ?? (_registryKey =
#endif
            TryOpenKey()
#if !CS8
            )
#endif
            )?.Value;

        // public override FileSystemType ItemFileSystemType => FileSystemType.Registry;
        #endregion // Properties

        #region Constructors
        public RegistryItemInfo() : this(GetDefaultClientVersion()) { /* Left empty. */ }

        public RegistryItemInfo(in ClientVersion clientVersion) : base(Properties.Resources.RegistryRoot, clientVersion) => Name = Path;

        public RegistryItemInfo(in RegistryKey registryKey, in ClientVersion clientVersion) : base(GetRegistryKeyName(registryKey), clientVersion) => Name = registryKey.Name.Split(WinCopies.IO.Path.PathSeparator).GetLast();

        public RegistryItemInfo(in string path, in ClientVersion clientVersion) : base(IsNullEmptyOrWhiteSpace(path) ? throw GetNullEmptyOrWhiteSpaceStringException(nameof(path)) : path, clientVersion) => Name = path.Split(WinCopies.IO.Path.PathSeparator).GetLast();

        public RegistryItemInfo(in RegistryKey registryKey, in string valueName, in ClientVersion clientVersion) : base($"{GetRegistryKeyName(registryKey)}{WinCopies.IO.Path.PathSeparator}{(IsNullEmptyOrWhiteSpace(valueName) ? throw GetNullEmptyOrWhiteSpaceStringException(nameof(valueName)) : valueName)}", clientVersion) => Name = valueName;

        public RegistryItemInfo(in string registryKeyPath, in string valueName, in ClientVersion clientVersion) : this(Registry.OpenRegistryKey(registryKeyPath), valueName, clientVersion)
        {
            // Left empty.
        }
        #endregion Constructors

        #region Methods
        protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => RegistryItemInfo.GetDefaultRootItems();

        private static string GetRegistryKeyName(in RegistryKey registryKey) => (registryKey ?? throw GetArgumentNullException(nameof(registryKey))).Name;

        public NullableReference<RegistryKey> TryOpenKey() => ObjectPropertiesGeneric.RegistryItemType == RegistryItemType.Key
            ? new NullableReference<RegistryKey>(Registry.OpenRegistryKey(Path))
            : new NullableReference<RegistryKey>(null);

        /// <summary>
        /// Opens the <see cref="RegistryKey"/> that this <see cref="RegistryItemInfo"/> represents.
        /// </summary>
        public void OpenKey()
        {
            if ((_registryKey = TryOpenKey()).Value == null)

                throw new InvalidOperationException(ItemDoesNotRepresentARegistryKey);
        }

        /// <summary>
        /// Opens the <see cref="RegistryKey"/> that this <see cref="RegistryItemInfo"/> represents using custom <see cref="RegistryKeyPermissionCheck"/> and <see cref="RegistryRights"/>.
        /// </summary>
        /// <param name="registryKeyPermissionCheck">Specifies whether security checks are performed when opening the registry key and accessing its name/value pairs.</param>
        /// <param name="registryRights">Specifies the access control rights that can be applied to the registry objects in registry key's scope.</param>
        public void OpenKey(RegistryKeyPermissionCheck registryKeyPermissionCheck, RegistryRights registryRights) => _registryKey = new NullableReference<RegistryKey>(Registry.OpenRegistryKey(Path, registryKeyPermissionCheck, registryRights));

        /// <summary>
        /// Opens the <see cref="RegistryKey"/> that this <see cref="RegistryItemInfo"/> represents with a <see cref="bool"/> value that indicates whether the registry key has to be opened with write-rights.
        /// </summary>
        /// <param name="writable">A <see cref="bool"/> value that indicates whether the registry key has to be opened with write-rights</param>
        public void OpenKey(bool writable) => _registryKey = new NullableReference<RegistryKey>(Registry.OpenRegistryKey(Path, writable));

        public void CloseKey()
        {
            if (_registryKey != null)
            {
                _registryKey.Value?.Dispose();
                _registryKey = null;
            }
        }

        /// <summary>
        /// Returns the parent of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        /// <returns>The parent of this <see cref="RegistryItemInfo"/>.</returns>
        private IBrowsableObjectInfo GetParent()
        {
            switch (ObjectPropertiesGeneric.RegistryItemType)
            {
                case RegistryItemType.Root:

                    return new ShellObjectInfo(KnownFolders.Computer, ClientVersion);

                case RegistryItemType.Key:

                    string[] path = InnerObjectGeneric.Name.Split(IO.Path.PathSeparator);

                    if (path.Length == 1)

                        return new RegistryItemInfo(ClientVersion);

                    var stringBuilder = new StringBuilder();

                    void append(in int _i) => _ = stringBuilder.Append(path[_i]);

                    append(0);

                    for (int i = 1; i < path.Length - 1; i++)
                    {
                        _ = stringBuilder.Append(IO.Path.PathSeparator);

                        append(i);
                    }

                    return new RegistryItemInfo(stringBuilder.ToString(), ClientVersion);

                case RegistryItemType.Value:

                    return new RegistryItemInfo(InnerObjectGeneric, ClientVersion);
            }

            return null;
        }

        ///// <summary>
        ///// Disposes the current <see cref="RegistryItemInfo"/> and its parent and items recursively.
        ///// </summary>
        ///// <exception cref="InvalidOperationException">The <see cref="BrowsableObjectInfo.ItemsLoader"/> is busy and does not support cancellation.</exception>
        protected override void DisposeManaged()
        {
            _itemTypeName = null;

            base.DisposeManaged();
        }

        private BitmapSource TryGetBitmapSource(in int size)
        {
            switch (ObjectPropertiesGeneric.RegistryItemType)
            {
                case RegistryItemType.Root:

                    return Icons.Computer.TryGetComputerBitmapSource(size);

                case RegistryItemType.Key:

                    return Icons.Folder.TryGetFolderBitmapSource(size);
            }

            return Icons.File.TryGetFileBitmapSource(size);
        }

        public override IEqualityComparer<IBrowsableObjectInfoBase> GetDefaultEqualityComparer() => new RegistryItemInfoEqualityComparer<IBrowsableObjectInfoBase>();

        public override IComparer<IBrowsableObjectInfoBase> GetDefaultComparer() => new RegistryItemInfoComparer<IBrowsableObjectInfoBase>();

        protected override void DisposeUnmanaged()
        {
            CloseKey();

            if (_bitmapSources != null)
            {
                _bitmapSources.Dispose();
                _bitmapSources = null;
            }

            base.DisposeUnmanaged();
        }
        #endregion // Methods
    }

    public class RegistryItemInfo : RegistryItemInfo<IRegistryItemInfoProperties, RegistryItemInfoItemProvider, IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo>, RegistryItemInfoItemProvider>, IRegistryItemInfo
    {
        protected class _ProcessFactory : IProcessFactory
        {
            protected class _NewItemProcessCommands : IProcessCommands
            {
                private IRegistryItemInfo _registryItemInfo;

                public bool IsDisposed => _registryItemInfo == null;

                public string Name { get; } = "New key";

                public string Caption { get; } = "Key name:";

                public _NewItemProcessCommands(in IRegistryItemInfo registryItemInfo) => _registryItemInfo = registryItemInfo;

                public bool CanCreateNewItem() => _registryItemInfo.ObjectProperties.RegistryItemType == RegistryItemType.Key && !_registryItemInfo.InnerObject.Name.StartsWith(Microsoft.Win32.Registry.LocalMachine.Name);

                public bool TryCreateNewItem(string parameter, out IProcessParameters result)
                {
                    result = null;

                    try
                    {
                        _ = _registryItemInfo.InnerObject.CreateSubKey(parameter);

                        return true;
                    }

                    catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(UnauthorizedAccessException), typeof(System.IO.IOException)))
                    {
                        return false;
                    }
                }

                public IProcessParameters CreateNewItem(string parameter) => TryCreateNewItem(parameter, out IProcessParameters result)
                        ? result
                        : throw new InvalidOperationException(CouldNotCreateItem);

                public void Dispose() => _registryItemInfo = null;

                ~_NewItemProcessCommands() => Dispose();
            }

            public bool IsDisposed { get; private set; }

            public IProcessCommands NewItemProcessCommands { get; }

            public _ProcessFactory(in IRegistryItemInfo registryItemInfo) => NewItemProcessCommands = new _NewItemProcessCommands(registryItemInfo);

            bool IProcessFactory.CanPaste(uint count) => false;

            IProcess IProcessFactory.GetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => throw new NotSupportedException();

            IProcess IProcessFactory.TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => null;

            bool IProcessFactory.CanCopy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => false;

            bool IProcessFactory.TryCopy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => false;

            void IProcessFactory.Copy(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => throw new NotSupportedException();

            IProcessParameters IProcessFactory.GetCopyProcessParameters(uint count) => throw new NotSupportedException();

            IProcessParameters IProcessFactory.TryGetCopyProcessParameters(uint count) => null;

            protected virtual void Dispose(bool disposing) => IsDisposed = true;

            public void Dispose()
            {
                if (IsDisposed)

                    return;

                Dispose(true);

                GC.SuppressFinalize(this);
            }

            ~_ProcessFactory() => Dispose(false);
        }

        protected class Enumerable : DisposableEnumerable<RegistryItemInfoItemProvider>
        {
            private RegistryKey _registryKey;
            private System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> _enumerable;

            public Enumerable(in RegistryKey registryKey, in System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> enumerable)
            {
                _registryKey = registryKey;

                _enumerable = enumerable;
            }

            protected override System.Collections.Generic.IEnumerator<RegistryItemInfoItemProvider> GetEnumeratorOverride() => _enumerable.GetEnumerator();

            protected override void Dispose(bool disposing)
            {
                _registryKey.Dispose();
                _registryKey = null;

                base.Dispose(disposing);
            }
        }

        #region Fields
        private IRegistryItemInfoProperties _objectProperties;
        private IProcessFactory _processFactory;
        private static readonly BrowsabilityPathStack<IRegistryItemInfo> __browsabilityPathStack = new BrowsabilityPathStack<IRegistryItemInfo>();
        private static readonly WriteOnlyBrowsabilityPathStack<IRegistryItemInfo> _browsabilityPathStack = __browsabilityPathStack.AsWriteOnly();
        #endregion

        #region Properties
        public static IBrowsabilityPathStack<IRegistryItemInfo> BrowsabilityPathStack => _browsabilityPathStack;

        protected override IProcessFactory ProcessFactoryOverride => _processFactory
#if CS8
            ??=
#else
            ?? (_processFactory =
#endif
            new _ProcessFactory(this)
#if !CS8
            )
#endif
            ;

        protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => DefaultCustomProcessesSelectorDictionary.Select(this);

        public static IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new RegistryItemInfoSelectorDictionary();

        public static ISelectorDictionary<IRegistryItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IRegistryItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

        protected sealed override IRegistryItemInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);
        #endregion Properties

        #region Constructors
        public RegistryItemInfo() { /* Left empty. */ }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryItemInfo"/> class as the Registry root.
        /// </summary>
        public RegistryItemInfo(in ClientVersion clientVersion) : base(clientVersion) => _objectProperties = new RegistryItemInfoProperties<IRegistryItemInfoBase>(this, RegistryItemType.Root);

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKey">The <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in RegistryKey registryKey, in ClientVersion clientVersion) : base(registryKey, clientVersion) => _objectProperties = new RegistryItemInfoProperties<IRegistryItemInfoBase>(this, RegistryItemType.Key);

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="path">The path of the <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) => _objectProperties = new RegistryItemInfoProperties<IRegistryItemInfoBase>(this, RegistryItemType.Key);

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKey">The <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        ///// <param name="valueName">The name of the value that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in RegistryKey registryKey, in string valueName, in ClientVersion clientVersion) : base(registryKey, valueName, clientVersion) => _objectProperties = new RegistryItemInfoProperties<IRegistryItemInfoBase>(this, RegistryItemType.Value);

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKeyPath">The path of the <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        ///// <param name="valueName">The name of the value that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in string registryKeyPath, in string valueName, in ClientVersion clientVersion) : base(registryKeyPath, valueName, clientVersion) => _objectProperties = new RegistryItemInfoProperties<IRegistryItemInfoBase>(this, RegistryItemType.Value);
        #endregion Constructors

        #region Methods
        public static ArrayBuilder<IBrowsableObjectInfo> GetDefaultRootItems()
        {
            var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

            _ = arrayBuilder.AddLast(new RegistryItemInfo(GetDefaultClientVersion()));

            return arrayBuilder;
        }

        protected override IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        #region GetItems
        protected override System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> GetItemProviders() => GetItemProviders(null);

        protected override System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> GetItemProviders(Predicate<RegistryItemInfoItemProvider> predicate)
        {
            System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> enumerable;

            switch (ObjectPropertiesGeneric.RegistryItemType)
            {
                case RegistryItemType.Key:

                    //string[] items;

                    System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> keys = null;
                    System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> values = null;

                    RegistryKey registryKey;

                    if ((registryKey = TryOpenKey().Value) != null)

                        try
                        {
                            keys = registryKey.GetSubKeyNames().Select(item => new RegistryItemInfoItemProvider(item, ClientVersion));

                            values = registryKey.GetValueNames().Select(s => new RegistryItemInfoItemProvider(registryKey, s, ClientVersion) /*new RegistryItemInfo(Path, s)*/);

                            // foreach (string item in items)

                            // item.Substring(0, item.LastIndexOf(IO.Path.PathSeparator)), item.Substring(item.LastIndexOf(IO.Path.PathSeparator) + 1), false
                        }

                        catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException))) { CloseKey(); return GetEmptyEnumerable(); }

                    //else

                    //    enumerate();

                    enumerable = new Enumerable(registryKey, (keys ?? GetEmptyEnumerable()).AppendValues(values ?? GetEmptyEnumerable()));

                    break;

                case RegistryItemType.Root:

                    enumerable = typeof(Microsoft.Win32.Registry).GetFields().Select(f => new RegistryItemInfoItemProvider((RegistryKey)f.GetValue(null), ClientVersion));

                    break;

                default:

                    return GetEmptyEnumerable();
            }

            return predicate == null ? enumerable : enumerable.WherePredicate(predicate);
        }

        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => GetItems(GetItemProviders(item => item.RegistryKey != null && item.ValueName == null));
        #endregion // GetItems

        protected override void DisposeUnmanaged()
        {
            if (_objectProperties != null)
            {
                _objectProperties.Dispose();
                _objectProperties = null;
            }

            base.DisposeUnmanaged();
        }
        #endregion Methods
    }
}
