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

using System;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Windows.Media.Imaging;

using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;

using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;
using WinCopies.PropertySystem;
using WinCopies.Util;

using static WinCopies.IO.ObjectModel.WMIItemInfo;
using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;

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
        public abstract class WMIItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<TObjectProperties, ManagementBaseObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IWMIItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IWMIItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            protected class BrowsableObjectInfoBitmapSources : BrowsableObjectInfoBitmapSources<WMIItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>>
            {
                /// <summary>
                /// Gets the small <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
                /// </summary>
                protected override BitmapSource SmallBitmapSourceOverride => InnerObject.TryGetBitmapSource(SmallIconSize);

                /// <summary>
                /// Gets the medium <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
                /// </summary>
                protected override BitmapSource MediumBitmapSourceOverride => InnerObject.TryGetBitmapSource(MediumIconSize);

                /// <summary>
                /// Gets the large <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
                /// </summary>
                protected override BitmapSource LargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(LargeIconSize);

                /// <summary>
                /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
                /// </summary>
                protected override BitmapSource ExtraLargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(ExtraLargeIconSize);

                public BrowsableObjectInfoBitmapSources(in WMIItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> wmiItemInfo) : base(wmiItemInfo) { /* Left empty. */ }
            }

            #region Fields
            private ManagementBaseObject _managementObject;
            private IBrowsableObjectInfo _parent;
            private string _itemTypeName;
            private IBrowsabilityOptions _browsability;
            private string _description;
            private IBrowsableObjectInfoBitmapSources _bitmapSources;
            #endregion

            #region Properties
            protected override IBrowsableObjectInfoBitmapSources BitmapSourcesOverride => _bitmapSources;

            protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => GetDefaultRootItems();

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride
            {
                get
                {
                    if (string.IsNullOrEmpty(_itemTypeName))
#if CS8
                        _itemTypeName = ObjectPropertiesGeneric.ItemType switch
                        {
                            WMIItemType.Namespace => Properties.Resources.WMINamespace,
                            WMIItemType.Class => Properties.Resources.WMIClass,
                            WMIItemType.Instance => Properties.Resources.WMIInstance,
                            _ => throw new InvalidOperationException("Invalid item type."),
                        };
#else

                        switch (ObjectPropertiesGeneric.ItemType)
                        {
                            case WMIItemType.Namespace:
                                _itemTypeName = Properties.Resources.WMINamespace;
                                break;
                            case WMIItemType.Class:
                                _itemTypeName = Properties.Resources.WMIClass;
                                break;
                            case WMIItemType.Instance:
                                _itemTypeName = Properties.Resources.WMIInstance;
                                break;
                            default:
                                throw new InvalidOperationException($"Invalid item type.");
                        }
#endif
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

                        _description = value == null ? NotApplicable : (string)value;
                    }

                    return _description;
                }
            }

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
                    {
#if CS8
                        _browsability = ObjectPropertiesGeneric.ItemType switch
                        {
#if CS9
                            WMIItemType.Namespace or WMIItemType.Class => BrowsabilityOptions.BrowsableByDefault,
#else
                            WMIItemType.Namespace => BrowsabilityOptions.BrowsableByDefault,
                            WMIItemType.Class => BrowsabilityOptions.BrowsableByDefault,
#endif
                            _ => BrowsabilityOptions.NotBrowsable
                        };
#else
                        switch (ObjectPropertiesGeneric.ItemType)
                        {
                            case WMIItemType.Namespace:
                            case WMIItemType.Class:
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

            /// <summary>
            /// Gets the <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.
            /// </summary>
            protected sealed override ManagementBaseObject InnerObjectGenericOverride => _managementObject;

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => DefaultCustomProcessesSelectorDictionary.Select(this);
            #endregion // Properties

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

            protected WMIItemInfo(in WMIItemInfoInitializer initializer, in string name, in ClientVersion clientVersion) : base((initializer ?? throw GetArgumentNullException(nameof(initializer))).Path, clientVersion)
            {
                //ThrowIfNull(managementObjectDelegate, nameof(managementObjectDelegate));

                // wmiItemType.ThrowIfInvalidEnumValue(true, WMIItemType.Namespace, WMIItemType.Class);

                //_managementObjectDelegate = managementObjectDelegate;

                _managementObject = initializer.ManagementObject;

                Name = IsNullEmptyOrWhiteSpace(name) ? Path : name;
            }
            #endregion Constructors

            #region Methods
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

            public WMIItemInfo GetWMIItemInfo(in string serverName, in string serverRelativePath) => WMIItemInfo.GetWMIItemInfo(serverName, serverRelativePath, ObjectPropertiesGeneric.Options, ClientVersion);

            public WMIItemInfo GetWMIItemInfo(in WMIItemType itemType, in ManagementBaseObject managementObject) => new WMIItemInfo(itemType, managementObject, ObjectPropertiesGeneric.Options, ClientVersion);

            public WMIItemInfo GetWMIItemInfo(in string path, in WMIItemType itemType) => new WMIItemInfo(path, itemType, ObjectPropertiesGeneric.Options, ClientVersion);

            public WMIItemInfo GetRootWMIItemInfo() => new WMIItemInfo(ObjectPropertiesGeneric.Options, ClientVersion);

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

            private IBrowsableObjectInfo GetParent()
            {
                if (ObjectPropertiesGeneric.IsRootNode) return new ShellObjectInfo(KnownFolders.Computer, ClientVersion);

                string path;

                switch (ObjectPropertiesGeneric.ItemType)
                {
                    case WMIItemType.Namespace:

                        path = Path.Substring(0, Path.LastIndexOf(WinCopies.IO.Path.PathSeparator)) + NamespacePath;

                        return path.EndsWith(RootNamespace, true, CultureInfo.InvariantCulture)
                            ? GetRootWMIItemInfo()
                            : GetWMIItemInfo(path, WMIItemType.Namespace);

                    case WMIItemType.Class:

                        return Path.EndsWith("root:" + Name, true, CultureInfo.InvariantCulture)
                            ? GetRootWMIItemInfo()
                            : GetWMIItemInfo(Path.Substring(0, Path.IndexOf(':')) + NamespacePath, WMIItemType.Namespace);

                    case WMIItemType.Instance:

                        path = Path.Substring(0, Path.IndexOf(':'));

                        int splitIndex = path.LastIndexOf(WinCopies.IO.Path.PathSeparator);

                        path =
#if CS8
                        $"{path.Substring(0, splitIndex)}:{path[(splitIndex + 1)..]}";
#else
                        $"{path.Substring(0, splitIndex)}:{path.Substring(splitIndex + 1)}";
#endif

                        return GetWMIItemInfo(path, WMIItemType.Class);

                    default:

                        return null;
                }
            }

            protected override void DisposeUnmanaged()
            {
                if (_bitmapSources != null)
                {
                    _bitmapSources.Dispose();
                    _bitmapSources = null;
                }

                base.DisposeUnmanaged();
            }

            protected override void DisposeManaged()
            {
                _managementObject.Dispose();
                _managementObject = null;

                base.DisposeManaged();
            }

            private BitmapSource TryGetBitmapSource(in ushort size) => TryGetBitmapSource(IsBrowsable ? FolderIcon : FileIcon, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);

            // public override bool CheckFilter(string path) => throw new NotImplementedException();

            public override WinCopies.Collections.Generic.IEqualityComparer<IBrowsableObjectInfoBase> GetDefaultEqualityComparer() => new WMIItemInfoEqualityComparer<IBrowsableObjectInfoBase>();

            public override WinCopies.Collections.Generic.IComparer<IBrowsableObjectInfoBase> GetDefaultComparer() => new WMIItemInfoComparer<IBrowsableObjectInfoBase>();

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => GetItems().Where(item => item.Browsability?.Browsability == IO.Browsability.BrowsableByDefault);
            #endregion Methods
        }

        public class WMIItemInfo : WMIItemInfo<IWMIItemInfoProperties, ManagementBaseObject, IEnumerableSelectorDictionary<WMIItemInfoItemProvider, IBrowsableObjectInfo>, WMIItemInfoItemProvider>, IWMIItemInfo
        {
            #region Consts
            public const string RootPath = @"\\.\";
            public const string NamespacePath = ":__NAMESPACE";
            public const string NameConst = "Name";
            public const string RootNamespace = "root:__namespace";
            public const string ROOT = "ROOT";
            #endregion

            private static readonly BrowsabilityPathStack<IWMIItemInfo> __browsabilityPathStack = new BrowsabilityPathStack<IWMIItemInfo>();

            private IWMIItemInfoProperties _objectProperties;

            #region Properties
            public static IBrowsabilityPathStack<IWMIItemInfo> BrowsabilityPathStack { get; } = __browsabilityPathStack.AsWriteOnly();

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

            public static ISelectorDictionary<IWMIItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IWMIItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

            public static IEnumerableSelectorDictionary<WMIItemInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new WMIItemInfoSelectorDictionary();

            protected sealed override IWMIItemInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="WMIItemInfo"/> class as the WMI root item.
            /// </summary>
            public WMIItemInfo() : base() => _objectProperties = GetProperties(this, new WMIItemInfoOptions());

            /// <summary>
            /// Initializes a new instance of the <see cref="WMIItemInfo"/> class as the WMI root item.
            /// </summary>
            public WMIItemInfo(in IWMIItemInfoOptions options, in ClientVersion clientVersion) : base(clientVersion) => _objectProperties = GetProperties(this, options);

            ///// <summary>
            ///// Initializes a new instance of the <see cref="WMIItemInfo"/> class. If you want to initialize this class in order to represent the root WMI item, you can also use the <see cref="WMIItemInfo()"/> constructor.
            ///// </summary>
            ///// <param name="path">The path of this <see cref="WMIItemInfo"/></param>.
            ///// <param name="wmiItemType">The type of this <see cref="WMIItemInfo"/>.</param>
            ///// <param name="managementObjectDelegate">The delegate that will be used by the <see cref="BrowsableObjectInfo.DeepClone()"/> method to get a new <see cref="ManagementBaseObject"/>.</param>
            ///// <param name="managementObject">The <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.</param>
            protected internal WMIItemInfo(in string path, in WMIItemType wmiItemType, in ManagementBaseObject managementObject, in IWMIItemInfoOptions options, in ClientVersion clientVersion) : base(new WMIItemInfoInitializer(path, managementObject), wmiItemType == WMIItemType.Instance ? null : GetName(managementObject, wmiItemType), clientVersion) => _objectProperties = new WMIItemInfoProperties(this, wmiItemType, false, options);

            public WMIItemInfo(in WMIItemType itemType, in ManagementBaseObject managementObject, in IWMIItemInfoOptions options, in ClientVersion clientVersion) : this(GetPath(managementObject, itemType), itemType, managementObject, options, clientVersion)
            {
                // Left empty.
            }

            public WMIItemInfo(in string path, in WMIItemType itemType, in IWMIItemInfoOptions options, in ClientVersion clientVersion) : this(path, itemType, GetManagementClassFromPath(path, options), options, clientVersion)
            {
                // Left empty.
            }
            #endregion // Constructors

            #region Methods
            //public static WMIItemInfoComparer<IWMIItemInfo> GetDefaultWMIItemInfoComparer() => new WMIItemInfoComparer<IWMIItemInfo>();

            public static IWMIItemInfoProperties GetProperties(in IWMIItemInfoBase item, in IWMIItemInfoOptions options) => new WMIItemInfoProperties(item, WMIItemType.Namespace, true, options);

            /// <summary>
            /// Gets the name of the given <see cref="ManagementBaseObject"/>.
            /// </summary>
            /// <param name="managementObject">The <see cref="ManagementBaseObject"/> for which get the name.</param>
            /// <param name="wmiItemType">The <see cref="IO.PropertySystem.WMIItemType"/> of <paramref name="managementObject"/>.</param>
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
                string path = $"{WinCopies.IO.Path.PathSeparator.Repeat(2)}{managementObject.ClassPath.Server}{WinCopies.IO.Path.PathSeparator}{managementObject.ClassPath.NamespacePath}";

                string name = GetName(managementObject, wmiItemType);

                string _getPath(in string format) => $"{path}{format}:{managementObject.ClassPath.ClassName}";

                path = name == null ? _getPath(string.Empty) : _getPath($"{WinCopies.IO.Path.PathSeparator}{name}");

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
            public static WMIItemInfo GetWMIItemInfo(in string serverName, in string serverRelativePath, in IWMIItemInfoOptions options, in ClientVersion clientVersion)
            {
                string path = $"{WinCopies.IO.Path.PathSeparator}{WinCopies.IO.Path.PathSeparator}{serverName}{WinCopies.IO.Path.PathSeparator}{(IsNullEmptyOrWhiteSpace(serverRelativePath) ? ROOT : serverRelativePath)}{NamespacePath}";

                return new WMIItemInfo(path, WMIItemType.Namespace, new ManagementClass(path), options, clientVersion  /*, managementObject => DefaultManagementClassDeepCloneDelegate((ManagementClass)managementObject, null)*/);
            }

            public static ManagementClass GetManagementClassFromPath(in string path, in IWMIItemInfoOptions options) =>

                    // #pragma warning disable IDE0067 // Dispose objects before losing scope
                    new ManagementClass(options?.ConnectionOptions == null ? new ManagementScope(path) : new ManagementScope(path, options.ConnectionOptions), new ManagementPath(path), options?.ObjectGetOptions);
            // #pragma warning restore IDE0067 // Dispose objects before losing scope

            protected override IEnumerableSelectorDictionary<WMIItemInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

            protected override void DisposeManaged()
            {
                base.DisposeManaged();

                _objectProperties = null;
            }

            #region GetItems
            protected override System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider> GetItemProviders(Predicate<ManagementBaseObject> predicate) => GetItemProviders(predicate, ObjectPropertiesGeneric.Options);

            protected override System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider> GetItemProviders() => GetItemProviders(item => true, ObjectPropertiesGeneric.Options);

            protected virtual System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider> GetItemProviders(Predicate<ManagementBaseObject> predicate, IWMIItemInfoOptions options)
            {
                // var paths = new ArrayBuilder<PathInfo>();

                // string _path;

                bool dispose = false;

                var managementClass = InnerObjectGeneric as ManagementClass;

                if (managementClass == null)
                {
                    dispose = true;

                    managementClass = GetManagementClassFromPath(Path, options);
                }

                managementClass.Get();

                try
                {
#if CS8
                    static
#endif
                    System.Collections.Generic.IEnumerable<ManagementBaseObject> _as(in ManagementObjectCollection collection) => collection.As<ManagementBaseObject>();

#if CS8
                    static
#endif
                    System.Collections.Generic.IEnumerable<ManagementBaseObject> enumerateInstances(in ManagementClass _managementClass, in IWMIItemInfoOptions _options) => _as(_options?.EnumerationOptions == null ? _managementClass.GetInstances() : _managementClass.GetInstances(_options?.EnumerationOptions));

                    System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider> getEnumerable(in System.Collections.Generic.IEnumerable<ManagementBaseObject> enumerable, WMIItemType itemType) => enumerable.SelectConverter(item => new WMIItemInfoItemProvider(null, item, itemType, ObjectPropertiesGeneric.Options, ClientVersion));

                    if (ObjectPropertiesGeneric.ItemType == WMIItemType.Namespace)
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

                    else if (ObjectPropertiesGeneric.ItemType == WMIItemType.Class /*&& WMIItemTypes.HasFlag(WMIItemTypes.Instance)*/)
                    {
                        managementClass.Get();

                        System.Collections.Generic.IEnumerable<ManagementBaseObject> items = enumerateInstances(managementClass, options);

                        if (predicate != null)

                            items = items.WherePredicate(predicate);

                        return getEnumerable(items, WMIItemType.Instance);
                    }

                    return GetEmptyEnumerable();
                }

                finally
                {
                    if (dispose)

                        managementClass.Dispose();
                }
            }

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(in Predicate<ManagementBaseObject> predicate, in IWMIItemInfoOptions options) => GetItems(GetItemProviders(predicate, options));

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(in IWMIItemInfoOptions options) => GetItems(item => true, options);
            #endregion GetItems
            #endregion Methods
        }
    }
}
