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
using Microsoft.WindowsAPICodePack.Shell;

#region System
using System;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Windows.Media.Imaging;
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

using static WinCopies.IO.ObjectModel.WMIItemInfo;
using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;
#endregion Static Usings
#endregion Usings

namespace WinCopies.IO
{
    public class WMIItemInfoInitializer
    {
        public string Path { get; }

        public ManagementBaseObject ManagementObject { get; }

        public WMIItemInfoInitializer(in string path, in ManagementBaseObject managementObject)
        {
            Path = path ?? throw GetArgumentNullException(nameof(path));

            ManagementObject = managementObject ?? throw GetArgumentNullException(nameof(managementObject));
        }
    }

    namespace ObjectModel
    {
        public abstract class WMIItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<IBrowsableObjectInfo, TObjectProperties, ManagementBaseObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IWMIItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IWMIItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            protected class BrowsableObjectInfoBitmapSources : BitmapSources<WMIItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>>
            {
                /// <summary>
                /// Gets the small <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
                /// </summary>
                protected override BitmapSource SmallOverride => InnerObject.TryGetBitmapSource(SmallIconSize);

                /// <summary>
                /// Gets the medium <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
                /// </summary>
                protected override BitmapSource MediumOverride => InnerObject.TryGetBitmapSource(MediumIconSize);

                /// <summary>
                /// Gets the large <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
                /// </summary>
                protected override BitmapSource LargeOverride => InnerObject.TryGetBitmapSource(LargeIconSize);

                /// <summary>
                /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
                /// </summary>
                protected override BitmapSource ExtraLargeOverride => InnerObject.TryGetBitmapSource(ExtraLargeIconSize);

                public BrowsableObjectInfoBitmapSources(in WMIItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> wmiItemInfo) : base(wmiItemInfo) { /* Left empty. */ }
            }

            #region Fields
            private ManagementBaseObject _managementObject;
            private IBrowsableObjectInfo
#if CS8
                ?
#endif
                _parent;
            private string _itemTypeName;
            private IBrowsabilityOptions _browsability;
            private string _description;
            private IBitmapSourceProvider _bitmapSourceProvider;
            #endregion

            #region Properties
            protected override bool IsLocalRootOverride => ObjectPropertiesGenericOverride.IsRootNode;

            protected override IBitmapSourceProvider BitmapSourceProviderOverride => _bitmapSourceProvider
#if CS8
                ??=
#else
                ?? (_bitmapSourceProvider =
#endif
                new Shell.ComponentSources.Bitmap.BitmapSourceProvider(this, new BrowsableObjectInfoBitmapSources(this), true)
#if !CS8
                )
#endif
                ;

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride
            {
                get
                {
                    if (string.IsNullOrEmpty(_itemTypeName))
#if CS8
                        _itemTypeName =
#else
                        switch (
#endif
                            ObjectPropertiesGeneric.ItemType
#if CS8
                            switch
#else
                            )
#endif
                            {
#if !CS8
                                case
#endif
                                WMIItemType.Namespace
#if CS8
                                    =>
#else
                                    :

                                _itemTypeName =
#endif
                                    Shell.Properties.Resources.WMINamespace
#if CS8
                                ,
#else
                                ;

                                break;

                                case
#endif
                                WMIItemType.Class
#if CS8
                                    =>
#else
                                    :

                                _itemTypeName =
#endif
                                    Shell.Properties.Resources.WMIClass
#if CS8
                                ,
#else
                                ;

                                break;

                                case
#endif
                                WMIItemType.Instance
#if CS8
                                    =>
#else
                                    :

                                _itemTypeName =
#endif
                                    Shell.Properties.Resources.WMIInstance
#if CS8
                                ,

                                _ =>
#else
                                ;

                                break;

                                default:
#endif
                                throw new InvalidOperationException("Invalid item type.")
#if CS8
                                ,
#else
                                ;
#endif
                            };

                    return _itemTypeName;
                }
            }

            protected override string DescriptionOverride
            {
                get
                {
                    if (_description == null)
                    {
                        object value = _managementObject.Qualifiers[nameof(Description)].Value;

                        _description = value == null ? WinCopies.Consts.NotApplicable : (string)value;
                    }

                    return _description;
                }
            }

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

            /// <summary>
            /// Gets the localized path of this <see cref="WMIItemInfo"/>.
            /// </summary>
            public override string LocalizedName => Name;

            /// <summary>
            /// Gets the name of this <see cref="WMIItemInfo"/>.
            /// </summary>
            public override string Name { get; }

            /// <summary>
            /// Gets a value that indicates whether this <see cref="WMIItemInfo"/> is browsable.
            /// </summary>
            protected override IBrowsabilityOptions BrowsabilityOverride
            {
                get
                {
                    if (_browsability == null)

#if CS9
                        _browsability =
#else
                        switch (
#endif
                            ObjectPropertiesGeneric.ItemType
#if CS9
                            switch
#else
                        )
#endif
                            {
#if !CS9
                            case
#endif
                                WMIItemType.Namespace
#if CS9
                                or
#else
                                :
                            case
#endif
                                WMIItemType.Class
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
                                ;

                                break;

                            default:

                                _browsability =
#endif
                                BrowsabilityOptions.NotBrowsable
#if !CS9
                                ; break;
#endif
                            };

                    return _browsability;
                }
            }

            protected override bool IsRecursivelyBrowsableOverride => true;

            /// <summary>
            /// Gets the <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.
            /// </summary>
            protected sealed override ManagementBaseObject InnerObjectGenericOverride => _managementObject;
            #endregion Properties

            #region Constructors
            protected WMIItemInfo() : this(GetDefaultClientVersion()) { /* Left empty. */ }

            /// <summary>
            /// Initializes a new instance of the <see cref="WMIItemInfo"/> class as the WMI root item.
            /// </summary>
            protected WMIItemInfo(in ClientVersion clientVersion) : this(GetRootInitializer(), null, clientVersion) { /* Left empty. */ }

            ///// <summary>
            ///// Initializes a new instance of the <see cref="WMIItemInfo"/> class. If you want to initialize this class in order to represent the root WMI item, you can also use the <see cref="WMIItemInfo()"/> constructor.
            ///// </summary>
            ///// <param name="path">The path of this <see cref="WMIItemInfo"/></param>.
            ///// <param name="wmiItemType">The type of this <see cref="WMIItemInfo"/>.</param>
            ///// <param name="managementObjectDelegate">The delegate that will be used by the <see cref="BrowsableObjectInfo.DeepClone()"/> method to get a new <see cref="ManagementBaseObject"/>.</param>
            ///// <param name="managementObject">The <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.</param>
            //public WMIItemInfo(in string path, in ManagementBaseObject managementObject) : this(path, wmiItemType, wmiItemType == WMIItemType.Namespace && path.ToUpper().EndsWith("ROOT:__NAMESPACE"), managementObject)
            //{
            //    // Left empty.
            //}

            protected WMIItemInfo(in WMIItemInfoInitializer initializer, in string
#if CS8
                ?
#endif
                name, in ClientVersion clientVersion) : base((initializer ?? throw GetArgumentNullException(nameof(initializer))).Path, clientVersion)
            {
                //ThrowIfNull(managementObjectDelegate, nameof(managementObjectDelegate));

                // wmiItemType.ThrowIfInvalidEnumValue(true, WMIItemType.Namespace, WMIItemType.Class);

                //_managementObjectDelegate = managementObjectDelegate;

                _managementObject = initializer.ManagementObject;

                Name = IsNullEmptyOrWhiteSpace(name) ? Path : name;
            }
            #endregion Constructors

            #region Methods
            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => GetDefaultRootItems();

            public static ArrayBuilder<IBrowsableObjectInfo> GetDefaultRootItems()
            {
                var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

                _ = arrayBuilder.AddLast(new WMIItemInfo(null, GetDefaultClientVersion()));

                return arrayBuilder;
            }

            private static WMIItemInfoInitializer GetRootInitializer()
            {
                string path = $"{RootPath}{ROOT}{NamespacePath}";

                return new WMIItemInfoInitializer(path, new ManagementClass(path));
            }

            public WMIItemInfo GetWMIItemInfo(in string serverName, in string serverRelativePath) => WMIItemInfo.GetWMIItemInfo(serverName, serverRelativePath, ObjectPropertiesGeneric?.Options, ClientVersion);

            public WMIItemInfo GetWMIItemInfo(in WMIItemType itemType, in ManagementBaseObject managementObject) => new
#if !CS9
                WMIItemInfo
#endif
                (itemType, managementObject, ObjectPropertiesGeneric.Options, ClientVersion);

            public WMIItemInfo GetWMIItemInfo(in string path, in WMIItemType itemType) => new
#if !CS9
                WMIItemInfo
#endif
                (path, itemType, ObjectPropertiesGeneric.Options, ClientVersion);

            public WMIItemInfo GetRootWMIItemInfo() => new
#if !CS9
                WMIItemInfo
#endif
                (ObjectPropertiesGeneric.Options, ClientVersion);

            /*public override bool Equals(object obj) => ReferenceEquals(this, obj) || (obj is IWMIItemInfo _obj && WMIItemType == _obj.WMIItemType && Path.ToLower() == _obj.Path.ToLower());

            //public int CompareTo(IWMIItemInfo other) => GetDefaultWMIItemInfoComparer().Compare(this, other);

            /// <summary>
            /// Determines whether the specified <see cref="IWMIItemInfo"/> is equal to the current object by calling the <see cref="Equals(object)"/> method.
            /// </summary>
            /// <param name="wmiItemInfo">The <see cref="IWMIItemInfo"/> to compare with the current object.</param>
            /// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
            public bool Equals(IWMIItemInfo wmiItemInfo) => Equals(wmiItemInfo as object);

            /// <summary>
            /// Gets an hash code for this <see cref="WMIItemInfo"/>.
            /// </summary>
            /// <returns>The hash code returned by the <see cref="FileSystemObject.GetHashCode"/> and the hash code of the <see cref="WMIItemType"/>.</returns>
            public override int GetHashCode() => base.GetHashCode() ^ WMIItemType.GetHashCode();*/

            private IBrowsableObjectInfo
#if CS8
                ?
#endif
                GetParent()
            {
                if (ObjectPropertiesGeneric != null)
                {
                    if (ObjectPropertiesGeneric.IsRootNode) return new ShellObjectInfo(KnownFolders.Computer, ClientVersion);

                    string path;

                    switch (ObjectPropertiesGeneric.ItemType)
                    {
                        case WMIItemType.Namespace:

                            path =
#if CS8
                                string.Concat(
#endif
                                    Path.
#if CS8
                                    AsSpan
#else
                                Substring
#endif
                                    (0, Path.LastIndexOf(DirectorySeparatorChar))
#if CS8
                                    ,
#else
                                +
#endif
                                    NamespacePath
#if CS8
                                    )
#endif
                                ;

                            return path.EndsWith(RootNamespace, true, CultureInfo.InvariantCulture)
                                ? GetRootWMIItemInfo()
                                : GetWMIItemInfo(path, WMIItemType.Namespace);

                        case WMIItemType.Class:

                            return Path.EndsWith("root:" + Name, true, CultureInfo.InvariantCulture)
                                ? GetRootWMIItemInfo()
                                : GetWMIItemInfo(
#if CS8
                                    string.Concat(
#endif
                                        Path.
#if CS8
                                        AsSpan
#else
                                    Substring
#endif
                                        (0, Path.IndexOf(':'))
#if CS8
                                        ,
#else
                                    +
#endif
                                        NamespacePath
#if CS8
                                        )
#endif
                                    , WMIItemType.Namespace)
                                ;

                        case WMIItemType.Instance:

                            path = Path.Substring(0, Path.IndexOf(':'));

                            int splitIndex = path.LastIndexOf(DirectorySeparatorChar);

                            path =
#if CS8
                            $"{path.Substring(0, splitIndex)}:{path[(splitIndex + 1)..]}";
#else
                        $"{path.Substring(0, splitIndex)}:{path.Substring(splitIndex + 1)}";
#endif

                            return GetWMIItemInfo(path, WMIItemType.Class);
                    }
                }

                return null;
            }

            protected override void DisposeUnmanaged()
            {
                if (_bitmapSourceProvider != null)
                {
                    _bitmapSourceProvider.Dispose();
                    _bitmapSourceProvider = null;
                }

                base.DisposeUnmanaged();
            }

            protected override void DisposeManaged()
            {
                _managementObject.Dispose();
                _managementObject = null;

                base.DisposeManaged();
            }

            private BitmapSource
#if CS8
                ?
#endif
                TryGetBitmapSource(in ushort size) => TryGetBitmapSource(IsBrowsable ? FolderIcon : FileIcon, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);

            // public override bool CheckFilter(string path) => throw new NotImplementedException();

            public override IEqualityComparer<IBrowsableObjectInfoBase> GetDefaultEqualityComparer() => new WMIItemInfoEqualityComparer<IBrowsableObjectInfoBase>();

            public override IComparer<IBrowsableObjectInfoBase> GetDefaultComparer() => new WMIItemInfoComparer<IBrowsableObjectInfoBase>();

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => GetDefaultEnumerable().Where(item => item.Browsability?.Browsability == IO.Browsability.BrowsableByDefault);
            #endregion Methods
        }

        public class WMIItemInfo : WMIItemInfo<IWMIItemInfoProperties, ManagementBaseObject, IEnumerableSelectorDictionary<WMIItemInfoItemProvider, IBrowsableObjectInfo>, WMIItemInfoItemProvider>, IWMIItemInfo
        {
            public class ItemSource : ItemSourceBase4<IWMIItemInfo, ManagementBaseObject, IEnumerableSelectorDictionary<WMIItemInfoItemProvider, IBrowsableObjectInfo>, WMIItemInfoItemProvider>
            {
                private const string EXCEPTION_MESSAGE = "No properties found.";

                public override bool IsPaginationSupported => false;

                protected override IProcessSettings
#if CS8
                    ?
#endif
                    ProcessSettingsOverride
                { get; }

                public ItemSource(in IWMIItemInfo browsableObjectInfo) : base(browsableObjectInfo)
                {
                    _ = ValidateProperties(() => new ArgumentException(EXCEPTION_MESSAGE, nameof(browsableObjectInfo)));

                    ProcessSettingsOverride = new ProcessSettings(null, DefaultCustomProcessesSelectorDictionary.Select(BrowsableObjectInfo));
                }

                private IWMIItemInfoProperties ValidateProperties(in Func<Exception> func) => BrowsableObjectInfo.ObjectProperties ?? throw func();
                protected IWMIItemInfoProperties ValidateProperties() => ValidateProperties(() => new InvalidOperationException(EXCEPTION_MESSAGE));

                protected internal virtual System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider>
#if CS8
                    ?
#endif
                    GetItemProviders(Predicate<ManagementBaseObject> predicate, IWMIItemInfoOptions
#if CS8
                    ?
#endif
                    options)
                {
                    IWMIItemInfoProperties objectProperties = ValidateProperties();

                    // var paths = new ArrayBuilder<PathInfo>();

                    // string _path;

                    bool dispose = false;

                    IWMIItemInfo browsableObjectInfo = BrowsableObjectInfo;

                    if (
#if !CS9
                        !(
#endif
                        browsableObjectInfo.InnerObject is
#if CS9
                        not
#endif
                        ManagementClass managementClass
#if !CS9
                        )
#endif
                        )
                    {
                        dispose = true;

                        managementClass = GetManagementClassFromPath(browsableObjectInfo.Path, options);
                    }

                    managementClass.Get();

                    try
                    {
#if CS8
                        static
#endif
                        System.Collections.Generic.IEnumerable<ManagementBaseObject> _as(in ManagementObjectCollection collection) => collection.OfType<ManagementBaseObject>();

#if CS8
                        static
#endif
                        System.Collections.Generic.IEnumerable<ManagementBaseObject> enumerateInstances(in ManagementClass _managementClass, in IWMIItemInfoOptions
#if CS8
                        ?
#endif
                        _options) => _as(_options?.EnumerationOptions == null ? _managementClass.GetInstances() : _managementClass.GetInstances(_options?.EnumerationOptions));

                        System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider> getEnumerable(in System.Collections.Generic.IEnumerable<ManagementBaseObject> enumerable, WMIItemType itemType) => enumerable.SelectConverter(item => new WMIItemInfoItemProvider(null, item, itemType, objectProperties.Options, browsableObjectInfo.ClientVersion));

                        if (objectProperties.ItemType == WMIItemType.Namespace)
                        {
                            System.Collections.Generic.IEnumerable<ManagementBaseObject> namespaces = enumerateInstances(managementClass, options);

                            System.Collections.Generic.IEnumerable<ManagementBaseObject> classes = _as(options?.EnumerationOptions == null ? managementClass.GetSubclasses() : managementClass.GetSubclasses(options?.EnumerationOptions));

                            if (predicate != null)
                            {
                                namespaces = namespaces.WherePredicate(predicate);

                                classes = classes.WherePredicate(predicate);
                            }

                            return getEnumerable(namespaces, WMIItemType.Namespace).AppendValues(getEnumerable(classes, WMIItemType.Class));
                        }

                        else if (objectProperties.ItemType == WMIItemType.Class /*&& WMIItemTypes.HasFlag(WMIItemTypes.Instance)*/)
                        {
                            managementClass.Get();

                            System.Collections.Generic.IEnumerable<ManagementBaseObject> items = enumerateInstances(managementClass, options);

                            if (predicate != null)

                                items = items.WherePredicate(predicate);

                            return getEnumerable(items, WMIItemType.Instance);
                        }

                        return null;
                    }

                    finally
                    {
                        if (dispose)

                            managementClass.Dispose();
                    }
                }

                protected override System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider>
#if CS8
                    ?
#endif
                    GetItemProviders(Predicate<ManagementBaseObject> predicate) => GetItemProviders(predicate, BrowsableObjectInfo.ObjectProperties?.Options);

                protected override System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider>
#if CS8
                    ?
#endif
                    GetItemProviders() => GetItemProviders(Bool.True, ValidateProperties().Options);

                internal new System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                    ?
#endif
                    GetItems(System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider>
#if CS8
                    ?
#endif
                    items) => base.GetItems(items);
            }

            #region Consts
            public const string RootPath = @"\\.\";
            public const string NamespacePath = ":__NAMESPACE";
            public const string NameConst = "Name";
            public const string RootNamespace = "root:__namespace";
            public const string ROOT = "ROOT";
            #endregion

            private static readonly BrowsabilityPathStack<IWMIItemInfo> __browsabilityPathStack = new
#if !CS9
                BrowsabilityPathStack<IWMIItemInfo>
#endif
                ();
            private IWMIItemInfoProperties _objectProperties;
            private
#if CS9
                readonly
#endif
                ItemSource _itemSource;
            private
#if CS9
                readonly
#endif
                IItemSourcesProvider<ManagementBaseObject> _itemSources;

            #region Properties
            public override string Protocol => "wmi";

            public static IBrowsabilityPathStack<IWMIItemInfo> BrowsabilityPathStack { get; } = __browsabilityPathStack.AsWriteOnly();

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

            public static ISelectorDictionary<IWMIItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IWMIItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

            public static IEnumerableSelectorDictionary<WMIItemInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new WMIItemInfoSelectorDictionary();

            protected sealed override IWMIItemInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride => null;

            protected override IItemSourcesProvider<ManagementBaseObject>
#if CS8
                ?
#endif
                ItemSourcesGenericOverride => _itemSources;

            private IWMIItemInfoProperties ObjectProperties
            {
#if CS9
                init
#else
                set
#endif
                {
                    _objectProperties = value;
                    _itemSources = ItemSourcesProvider.Construct(_itemSource = new ItemSource(this));
                }
            }
            #endregion Properties

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="WMIItemInfo"/> class as the WMI root item.
            /// </summary>
            public WMIItemInfo() : base() => ObjectProperties = GetProperties(this, new WMIItemInfoOptions());

            /// <summary>
            /// Initializes a new instance of the <see cref="WMIItemInfo"/> class as the WMI root item.
            /// </summary>
            public WMIItemInfo(in IWMIItemInfoOptions
#if CS8
                ?
#endif
                options, in ClientVersion clientVersion) : base(clientVersion) => ObjectProperties = GetProperties(this, options);

            ///// <summary>
            ///// Initializes a new instance of the <see cref="WMIItemInfo"/> class. If you want to initialize this class in order to represent the root WMI item, you can also use the <see cref="WMIItemInfo()"/> constructor.
            ///// </summary>
            ///// <param name="path">The path of this <see cref="WMIItemInfo"/></param>.
            ///// <param name="wmiItemType">The type of this <see cref="WMIItemInfo"/>.</param>
            ///// <param name="managementObjectDelegate">The delegate that will be used by the <see cref="BrowsableObjectInfo.DeepClone()"/> method to get a new <see cref="ManagementBaseObject"/>.</param>
            ///// <param name="managementObject">The <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.</param>
            protected internal WMIItemInfo(in string path, in WMIItemType wmiItemType, in ManagementBaseObject managementObject, in IWMIItemInfoOptions
#if CS8
                ?
#endif
                options, in ClientVersion clientVersion) : base(new WMIItemInfoInitializer(path, managementObject), wmiItemType == WMIItemType.Instance ? null : GetName(managementObject, wmiItemType), clientVersion) => ObjectProperties = new WMIItemInfoProperties(this, wmiItemType, false, options);

            public WMIItemInfo(in WMIItemType itemType, in ManagementBaseObject managementObject, in IWMIItemInfoOptions options, in ClientVersion clientVersion) : this(GetPath(managementObject, itemType), itemType, managementObject, options, clientVersion) { /* Left empty. */ }

            public WMIItemInfo(in string path, in WMIItemType itemType, in IWMIItemInfoOptions options, in ClientVersion clientVersion) : this(path, itemType, GetManagementClassFromPath(path, options), options, clientVersion) { /* Left empty. */ }
            #endregion Constructors

            #region Methods
            //public static WMIItemInfoComparer<IWMIItemInfo> GetDefaultWMIItemInfoComparer() => new WMIItemInfoComparer<IWMIItemInfo>();

            public static IWMIItemInfoProperties GetProperties(in IWMIItemInfoBase item, in IWMIItemInfoOptions
#if CS8
                ?
#endif
                options) => new WMIItemInfoProperties(item, WMIItemType.Namespace, true, options);

            /// <summary>
            /// Gets the name of the given <see cref="ManagementBaseObject"/>.
            /// </summary>
            /// <param name="managementObject">The <see cref="ManagementBaseObject"/> for which get the name.</param>
            /// <param name="wmiItemType">The <see cref="WMIItemType"/> of <paramref name="managementObject"/>.</param>
            /// <returns>The name of the given <see cref="ManagementBaseObject"/>.</returns>
            public static string GetName(ManagementBaseObject managementObject, WMIItemType wmiItemType)
            {
                (managementObject as ManagementClass)?.Get();

                return wmiItemType == WMIItemType.Namespace ? (string)managementObject[NameConst] : managementObject.ClassPath.ClassName;
            }

            /// <summary>
            /// Gets the path of the given <see cref="ManagementBaseObject"/>.
            /// </summary>
            /// <param name="managementObject">The <see cref="ManagementBaseObject"/> for which get the path.</param>
            /// <param name="wmiItemType">The <see cref="IO.PropertySystem.WMIItemType"/> of <paramref name="managementObject"/>.</param>
            /// <returns>The path of the given <see cref="ManagementBaseObject"/>.</returns>
            public static string GetPath(ManagementBaseObject managementObject, WMIItemType wmiItemType)
            {
                string path = $"{DirectorySeparatorChar.Repeat(2)}{managementObject.ClassPath.Server}{DirectorySeparatorChar}{managementObject.ClassPath.NamespacePath}";

                string name = GetName(managementObject, wmiItemType);

                string _getPath(in string format) => $"{path}{format}:{managementObject.ClassPath.ClassName}";

                path = name == null ? _getPath(string.Empty) : _getPath($"{DirectorySeparatorChar}{name}");

                return path;
            }

            ///// <summary>
            ///// Gets a new <see cref="WMIItemInfo"/> that corresponds to the given server name and relative path.
            ///// </summary>
            ///// <param name="serverName">The server name.</param>
            ///// <param name="serverRelativePath">The server relative path.</param>
            ///// <returns>A new <see cref="WMIItemInfo"/> that corresponds to the given server name and relative path.</returns>
            ///// <seealso cref="WMIItemInfo()"/>
            ///// <seealso cref="WMIItemInfo(string, WMIItemType, ManagementBaseObject, DeepClone{ManagementBaseObject})"/>
            public static WMIItemInfo GetWMIItemInfo(in string serverName, in string serverRelativePath, in IWMIItemInfoOptions
#if CS8
                ?
#endif
                options, in ClientVersion clientVersion)
            {
                string path = $"{DirectorySeparatorChar}{DirectorySeparatorChar}{serverName}{DirectorySeparatorChar}{(IsNullEmptyOrWhiteSpace(serverRelativePath) ? ROOT : serverRelativePath)}{NamespacePath}";

                return new WMIItemInfo(path, WMIItemType.Namespace, new ManagementClass(path), options, clientVersion  /*, managementObject => DefaultManagementClassDeepCloneDelegate((ManagementClass)managementObject, null)*/);
            }

            public static ManagementClass GetManagementClassFromPath(in string path, in IWMIItemInfoOptions
#if CS8
                ?
#endif
                options) =>

                    // #pragma warning disable IDE0067 // Dispose objects before losing scope
                    new
#if !CS9
                ManagementClass
#endif
                (options?.ConnectionOptions == null ? new ManagementScope(path) : new ManagementScope(path, options.ConnectionOptions), new ManagementPath(path), options?.ObjectGetOptions);
            // #pragma warning restore IDE0067 // Dispose objects before losing scope

            protected override IEnumerableSelectorDictionary<WMIItemInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(in Predicate<ManagementBaseObject> predicate, in IWMIItemInfoOptions options) => _itemSource.GetItems(_itemSource.GetItemProviders(predicate, options));

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(in IWMIItemInfoOptions options) => GetItems(Bool.True, options);

            protected override void DisposeManaged()
            {
                base.DisposeManaged();

                _objectProperties = null;
            }
            #endregion Methods
        }
    }
}
