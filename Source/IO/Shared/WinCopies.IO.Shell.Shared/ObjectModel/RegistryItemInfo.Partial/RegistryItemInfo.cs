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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework. If not, see <https://www.gnu.org/licenses/>. */

#region Usings
using Microsoft.Win32;

using Microsoft.WindowsAPICodePack.Shell;

#region System
using System;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Text;
#endregion System

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;
using WinCopies.PropertySystem;
using WinCopies.Util;
#endregion WinCopies

#region Static Usings
using static System.IO.Path;

using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;
using static WinCopies.IO.Shell.Resources.ExceptionMessages;
#endregion Static Usings
#endregion Usings

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// Represents a Windows registry item.
    /// </summary>
    public abstract class RegistryItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>/*<TItems, TFactory>*/ : BrowsableObjectInfo<IBrowsableObjectInfo, TObjectProperties, RegistryKey, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>/*<TItems, TFactory>*/, IRegistryItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IRegistryItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo> // where TItems : BrowsableObjectInfo, IRegistryItemInfo where TFactory : IRegistryItemInfoFactory
    {
        #region Fields
        internal NullableReference<RegistryKey> _registryKey;
        private IBrowsableObjectInfo _parent;
        private IBrowsabilityOptions _browsability;
        private IBitmapSourceProvider _bitmapSourceProvider;
        private string _itemTypeName;
        #endregion

        #region Properties
        ///// <summary>
        ///// Gets a value that indicates whether this object needs to reconstruct objects on deep cloning.
        ///// </summary>
        //public override bool NeedsObjectsOrValuesReconstruction => _registryKey is object; // If _registryKey is null, reconstructing registry does not make sense, so we return false.

        //public static DeepClone<RegistryKey> DefaultRegistryKeyDeepClone { get; } = registryKey => Registry.OpenRegistryKey(registryKey.Name);

        protected override bool IsLocalRootOverride => ObjectPropertiesGenericOverride.RegistryItemType == RegistryItemType.Root;

        protected override bool IsSpecialItemOverride => false;

        protected override string ItemTypeNameOverride
        {
            get
            {
                if (string.IsNullOrEmpty(_itemTypeName))

                    switch (ObjectPropertiesGeneric.RegistryItemType)
                    {
                        case RegistryItemType.Root:

                            _itemTypeName = Shell.Properties.Resources.RegistryRoot;

                            break;

                        case RegistryItemType.Key:

                            _itemTypeName = Shell.Properties.Resources.RegistryKey;

                            break;

                        case RegistryItemType.Value:

                            _itemTypeName = Shell.Properties.Resources.RegistryValue;

                            break;
                    }

                return _itemTypeName;
            }
        }

        protected override string DescriptionOverride => WinCopies.Consts.NotApplicable;

        /// <summary>
        /// Gets the localized path of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override string LocalizedName => Name;

        /// <summary>
        /// Gets the name of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override string Name { get; }

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => _bitmapSourceProvider
#if CS8
            ??=
#else
            ?? (_bitmapSourceProvider =
#endif
            new Shell.ComponentSources.Bitmap.BitmapSourceProvider(this)
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
#if CS9
                    _browsability =
#else
                    switch (
#endif
                        ObjectPropertiesGeneric.RegistryItemType
#if CS9
                        switch
#else
                        )
#endif
                        {
#if !CS9
                        case
#endif
                            RegistryItemType.Key
#if CS9
                            or
#else
                        :

                        case
#endif
                                RegistryItemType.Root
#if CS9
                                =>
#else
                            :

                            _browsability =
#endif
                                BrowsabilityOptions.BrowsableByDefault
#if CS9
                            ,

                            _ =>
#else
                            ; break;

                        default:

                            _browsability =
#endif
                            BrowsabilityOptions.NotBrowsable
#if !CS9
                                ;

                            break;
#endif
                        };
                }

                return _browsability;
            }
        }

        protected override bool IsRecursivelyBrowsableOverride => true;

        protected override IBrowsableObjectInfo
#if CS8
            ?
#endif
            ParentGenericOverride => _parent
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

        protected sealed override RegistryKey
#if CS8
            ?
#endif
            InnerObjectGenericOverride => (_registryKey
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

        public override string Protocol => "registry";

        // public override FileSystemType ItemFileSystemType => FileSystemType.Registry;
        #endregion // Properties

        #region Constructors
        protected RegistryItemInfo() : this(GetDefaultClientVersion()) { /* Left empty. */ }

        protected RegistryItemInfo(in ClientVersion clientVersion) : base(Shell.Properties.Resources.RegistryRoot, clientVersion) => Name = Path;

        protected RegistryItemInfo(in RegistryKey registryKey, in ClientVersion clientVersion) : base(GetRegistryKeyName(registryKey), clientVersion) => Name = registryKey.Name.Split(DirectorySeparatorChar).GetLast();

        protected RegistryItemInfo(in string path, in ClientVersion clientVersion) : base(IsNullEmptyOrWhiteSpace(path) ? throw GetNullEmptyOrWhiteSpaceStringException(nameof(path)) : path, clientVersion) => Name = path.Split(DirectorySeparatorChar).GetLast();

        protected RegistryItemInfo(in RegistryKey registryKey, in string valueName, in ClientVersion clientVersion) : base($"{GetRegistryKeyName(registryKey)}{DirectorySeparatorChar}{(IsNullEmptyOrWhiteSpace(valueName) ? throw GetNullEmptyOrWhiteSpaceStringException(nameof(valueName)) : valueName)}", clientVersion) => Name = valueName;

        protected RegistryItemInfo(in string registryKeyPath, in string valueName, in ClientVersion clientVersion) : this(Registry.OpenRegistryKey(registryKeyPath), valueName, clientVersion) { /* Left empty. */ }
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
        private IBrowsableObjectInfo
#if CS8
            ?
#endif
            GetParent()
        {
            switch (ObjectPropertiesGeneric.RegistryItemType)
            {
                case RegistryItemType.Root:

                    return new ShellObjectInfo(KnownFolders.Computer, ClientVersion);

                case RegistryItemType.Key:

                    string[] path = InnerObjectGeneric.Name.Split(DirectorySeparatorChar);

                    if (path.Length == 1)

                        return new RegistryItemInfo(ClientVersion);

                    var stringBuilder = new StringBuilder();

                    void append(in int _i) => _ = stringBuilder.Append(path[_i]);

                    append(0);

                    for (int i = 1; i < path.Length - 1; i++)
                    {
                        _ = stringBuilder.Append(DirectorySeparatorChar);

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

        public override IEqualityComparer<IBrowsableObjectInfoBase> GetDefaultEqualityComparer() => new RegistryItemInfoEqualityComparer<IBrowsableObjectInfoBase>();

        public override IComparer<IBrowsableObjectInfoBase> GetDefaultComparer() => new RegistryItemInfoComparer<IBrowsableObjectInfoBase>();

        protected override void DisposeUnmanaged()
        {
            CloseKey();

            if (_bitmapSourceProvider != null)
            {
                _bitmapSourceProvider.Dispose();
                _bitmapSourceProvider = null;
            }

            base.DisposeUnmanaged();
        }
        #endregion Methods
    }

    public partial class RegistryItemInfo : RegistryItemInfo<IRegistryItemInfoProperties, RegistryItemInfoItemProvider, IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo>, RegistryItemInfoItemProvider>, IRegistryItemInfo
    {
        public class ItemSource : ItemSourceBase4<IRegistryItemInfo, RegistryItemInfoItemProvider, IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo>, RegistryItemInfoItemProvider>
        {
            private IProcessSettings _processSettings;

            public override bool IsPaginationSupported => false;

            protected override IProcessSettings ProcessSettingsOverride => _processSettings
#if CS8
                ??=
#else
                ?? (_processSettings =
#endif
                new ProcessSettings(new _ProcessFactory(BrowsableObjectInfo), DefaultCustomProcessesSelectorDictionary.Select(BrowsableObjectInfo))
#if !CS8
            )
#endif
                ;

            public ItemSource(in IRegistryItemInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                ?
#endif
                GetItemProviders(Predicate<RegistryItemInfoItemProvider>
#if CS8
                    ?
#endif
                    predicate)
            {
                System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> enumerable;

                switch (BrowsableObjectInfo.ObjectProperties.RegistryItemType)
                {
                    case RegistryItemType.Key:

                        //string[] items;

                        System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                            ?
#endif
                            keys = null;
                        System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                            ?
#endif
                            values = null;

                        RegistryKey
#if CS8
                            ?
#endif
                            registryKey;

                        if ((registryKey = BrowsableObjectInfo.TryOpenKey().Value) != null)

                            try
                            {
                                keys = registryKey.GetSubKeyNames().Select(item => new RegistryItemInfoItemProvider(item, BrowsableObjectInfo.ClientVersion));

                                values = registryKey.GetValueNames().Select(s => new RegistryItemInfoItemProvider(registryKey, s, BrowsableObjectInfo.ClientVersion) /*new RegistryItemInfo(Path, s)*/);

                                // foreach (string item in items)

                                // item.Substring(0, item.LastIndexOf(IO.Path.PathSeparator)), item.Substring(item.LastIndexOf(IO.Path.PathSeparator) + 1), false
                            }

                            catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException)))
                            {
                                BrowsableObjectInfo.CloseKey();

                                return null;
                            }

                        //else

                        //    enumerate();

                        if (keys == null)

                            if (values == null)

                                return null;

                            else

                                enumerable = values;

                        else

                            enumerable = values == null ? keys : keys.AppendValues(values);

                        break;

                    case RegistryItemType.Root:

                        enumerable = typeof(Microsoft.Win32.Registry).GetFields().Select(f => new RegistryItemInfoItemProvider((RegistryKey)f.GetValue(null), BrowsableObjectInfo.ClientVersion));

                        break;

                    default:

                        return null;
                }

                return predicate == null ? enumerable : enumerable.WherePredicate(predicate);
            }

            protected override System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                ?
#endif
                GetItemProviders() => GetItemProviders(null);

            internal new System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetItems(System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                ?
#endif
                items) => base.GetItems(items);

            internal System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                ?
#endif
                _GetItemProviders(Predicate<RegistryItemInfoItemProvider>
#if CS8
                    ?
#endif
                    predicate) => GetItemProviders(predicate);

            protected override void DisposeManaged()
            {
                _processSettings = null;

                base.DisposeManaged();
            }
        }

        #region Fields
        private ItemSource _itemSource;
        private IItemSourcesProvider<RegistryItemInfoItemProvider> _itemSources;
        private IRegistryItemInfoProperties _objectProperties;
        private static readonly BrowsabilityPathStack<IRegistryItemInfo> __browsabilityPathStack = new
#if !CS9
            BrowsabilityPathStack<IRegistryItemInfo>
#endif
            ();
        private static readonly WriteOnlyBrowsabilityPathStack<IRegistryItemInfo> _browsabilityPathStack = __browsabilityPathStack.AsWriteOnly();
        #endregion

        #region Properties
        public static IBrowsabilityPathStack<IRegistryItemInfo> BrowsabilityPathStack => _browsabilityPathStack;

        protected override IItemSourcesProvider<RegistryItemInfoItemProvider> ItemSourcesGenericOverride => _itemSources;

        public static IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new RegistryItemInfoSelectorDictionary();

        public static ISelectorDictionary<IRegistryItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IRegistryItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

        protected sealed override IRegistryItemInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystemOverride => null;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

        private RegistryItemType Properties
        {
#if CS9
            init
#else
            set
#endif
            {
                _objectProperties = new RegistryItemInfoProperties<IRegistryItemInfoBase>(this, value);
                SetItemSources();
            }
        }
        #endregion Properties

        #region Constructors
        public RegistryItemInfo() : this(GetDefaultClientVersion()) => SetItemSources();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryItemInfo"/> class as the Registry root.
        /// </summary>
        public RegistryItemInfo(in ClientVersion clientVersion) : base(clientVersion) => Properties = RegistryItemType.Root;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKey">The <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in RegistryKey registryKey, in ClientVersion clientVersion) : base(registryKey, clientVersion) => Properties = RegistryItemType.Key;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="path">The path of the <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) => Properties = RegistryItemType.Key;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKey">The <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        ///// <param name="valueName">The name of the value that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in RegistryKey registryKey, in string valueName, in ClientVersion clientVersion) : base(registryKey, valueName, clientVersion) => Properties = RegistryItemType.Value;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKeyPath">The path of the <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        ///// <param name="valueName">The name of the value that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in string registryKeyPath, in string valueName, in ClientVersion clientVersion) : base(registryKeyPath, valueName, clientVersion) => Properties = RegistryItemType.Value;
        #endregion Constructors

        #region Methods
        private void SetItemSources() => _itemSources = ItemSourcesProvider.Construct(_itemSource = new ItemSource(this));

        public static ArrayBuilder<IBrowsableObjectInfo> GetDefaultRootItems()
        {
            var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

            _ = arrayBuilder.AddLast(new RegistryItemInfo(GetDefaultClientVersion()));

            return arrayBuilder;
        }

        protected override IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetSubRootItemsOverride() => _itemSource.GetItems(_itemSource._GetItemProviders(item => item.RegistryKey != null && item.ValueName == null));

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
