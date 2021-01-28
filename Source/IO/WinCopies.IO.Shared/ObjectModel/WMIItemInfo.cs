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
using System.Management;
using System.Windows.Media.Imaging;

using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;

using static WinCopies.IO.ObjectModel.WMIItemInfo;
using System.Collections.Generic;

using WinCopies.Util;

using static WinCopies.Util.Util;
#else
using WinCopies.Collections.Generic;

using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;

        public WMIItemInfoInitializer(in string path, in ManagementBaseObject managementObject)
        {
            Path = path ?? throw GetArgumentNullException(nameof(path));
        /// The WMI item is an instance.
            ManagementObject = managementObject ?? throw GetArgumentNullException(nameof(managementObject));
    {
        public WMIItemType WMIItemType => BrowsableObjectInfo.WMIItemType;

        public bool IsRootNode => BrowsableObjectInfo.IsRootNode;

        public WMIItemInfoProperties(in IWMIItemInfo browsableObjectInfo) : base(browsableObjectInfo)
        {
            public const string RootPath = @"\\.\";
            public const string NamespacePath = ":__NAMESPACE";
            public const string NameConst = "Name";
            public const string RootNamespace = "root:__namespace";
            public const string ROOT = "ROOT";

            #endregion

            private static System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> _defaultRootItems;

            public static System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> DefaultRootItems => _defaultRootItems ??= GetRootItems();

            public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> RootItems => DefaultRootItems;

            public override Predicate<TPredicateTypeParameter> RootItemsPredicate => null;

            public override Predicate<IBrowsableObjectInfo> RootItemsBrowsableObjectInfoPredicate => item => item.IsBrowsable && item.IsBrowsableByDefault;

            #region Properties
            /// <summary>
            /// Gets a value that indicates whether this <see cref="WMIItemInfo"/> represents a root node.
            /// </summary>
            public bool IsRootNode { get; }

            public WMIItemType WMIItemType { get; }

#if CS8
                        _itemTypeName = ObjectPropertiesGeneric.ItemType switch

            public override string ItemTypeName
            {
                get
                {
                    if (string.IsNullOrEmpty(_itemTypeName))

                        switch (ObjectPropertiesGeneric.WMIItemType)
                        {
                            WMIItemType.Namespace => Properties.Resources.WMINamespace,
                            WMIItemType.Class => Properties.Resources.WMIClass,
                            WMIItemType.Instance => Properties.Resources.WMIInstance,
                            _ => throw new InvalidOperationException($"Invalid item type."),
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

            private string _description;

            public override string Description
            {
                get
                {
                    if (_description == null)
                    {
                        object value = _managementObject.Qualifiers[nameof(Description)].Value;
            public override IBrowsableObjectInfo Parent => _parent
#if CS8
                ??=
            public override Size? Size => null;
            ?? (_parent = 
#if NETCORE

            public override IBrowsableObjectInfo Parent => _parent ??= GetParent();

#else

            public override IBrowsableObjectInfo Parent => _parent ?? (_parent = GetParent());

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

            #region BitmapSources
            /// <summary>
            /// Gets the small <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
            /// </summary>
            public override BitmapSource SmallBitmapSource => TryGetBitmapSource(SmallIconSize);

            /// <summary>
            /// Gets the medium <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
            /// </summary>
            public override BitmapSource MediumBitmapSource => TryGetBitmapSource(MediumIconSize);

            /// <summary>
            /// Gets the large <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
            /// </summary>
            public override BitmapSource LargeBitmapSource => TryGetBitmapSource(LargeIconSize);

            /// <summary>
            /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
            /// </summary>
            public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(ExtraLargeIconSize);
            #endregion

            /// <summary>
            /// Gets a value that indicates whether this <see cref="WMIItemInfo"/> is browsable.
            /// </summary>
#if CS8
                    _isBrowsable = ObjectPropertiesGeneric.ItemType switch
            {
                get
                {
                    if (_isBrowsable.HasValue)

                        return _isBrowsable.Value;

                    switch (ObjectPropertiesGeneric.WMIItemType)
                    {
                        WMIItemType.Namespace or WMIItemType.Class => true,
                        _ => false,
                    };
#else

                    switch (ObjectPropertiesGeneric.ItemType)
                    {
                        case WMIItemType.Namespace:
                        case WMIItemType.Class:
                            _isBrowsable = true;
                            break;
                        default:
                            _isBrowsable = false;
                            break;
                    }
#endif

                    return _isBrowsable.Value;
                }
            }

            #endregion // Properties

            /// Gets the <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.
            /// </summary>
            public sealed override ManagementBaseObject InnerObjectGeneric => _managementObject;

            protected WMIItemInfo(in ClientVersion clientVersion) : this(GetRootInitializer(), null, clientVersion)
            {
                // Left empty.
            }

                // Left empty.
            }

            public WMIItemInfo(in WMIItemType wmiItemType, in ManagementBaseObject managementBaseObject) : this(GetPath(managementBaseObject, wmiItemType), wmiItemType, managementBaseObject)
            {
                // Left empty.
            }

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

                _managementObject = initializer.ManagementObject;

                if (!IsNullEmptyOrWhiteSpace(name))

                    Name = name;

            #endregion // Constructors


            public static System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetRootItems() => new IBrowsableObjectInfo[] { new WMIItemInfo(null, GetDefaultClientVersion()) };

            private static WMIItemInfoInitializer GetRootInitializer()

                string path = $"{RootPath}{ROOT}{NamespacePath}";

                return new WMIItemInfoInitializer(path, new ManagementClass(path));
            /// <returns>The name of the given <see cref="ManagementBaseObject"/>.</returns>
            public static string GetName(ManagementBaseObject managementObject, WMIItemType wmiItemType)
            public WMIItemInfo GetWMIItemInfo(in string serverName, in string serverRelativePath) => WMIItemInfo.GetWMIItemInfo(serverName, serverRelativePath, ObjectPropertiesGeneric.Options, ClientVersion);
            {
            public WMIItemInfo GetWMIItemInfo(in WMIItemType itemType, in ManagementBaseObject managementObject) => new WMIItemInfo(itemType, managementObject, ObjectPropertiesGeneric.Options, ClientVersion);
                string name = GetName(managementObject, wmiItemType);
            public WMIItemInfo GetWMIItemInfo(in string path, in WMIItemType itemType) => new WMIItemInfo(path, itemType, ObjectPropertiesGeneric.Options, ClientVersion);
            ///// <param name="serverName">The server name.</param>
            public WMIItemInfo GetRootWMIItemInfo() => new WMIItemInfo(ObjectPropertiesGeneric.Options, ClientVersion);

            ///// <seealso cref="WMIItemInfo(string, WMIItemType, ManagementBaseObject, DeepClone{ManagementBaseObject})"/>
            public static WMIItemInfo GetWMIItemInfo(string serverName, string serverRelativePath)
            {
                string path = $"{WinCopies.IO.Path.PathSeparator}{WinCopies.IO.Path.PathSeparator}{serverName}{WinCopies.IO.Path.PathSeparator}{(IsNullEmptyOrWhiteSpace(serverRelativePath) ? ROOT : serverRelativePath)}{NamespacePath}";

                return new WMIItemInfo(path, WMIItemType.Namespace, new ManagementClass(path)/*, managementObject => DefaultManagementClassDeepCloneDelegate((ManagementClass)managementObject, null)*/);
            }

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

            protected override void Dispose(in bool disposing)
            {
                base.Dispose(disposing);

                _managementObject.Dispose();

                if (disposing)
                    //{
                    _managementObject = null;

                //_managementObjectDelegate = null;
                //}
            }

            private BitmapSource TryGetBitmapSource(in ushort size)
            {
                int iconIndex = 0;

                return TryGetBitmapSource(iconIndex, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);
            }

            // public override bool CheckFilter(string path) => throw new NotImplementedException();

            public override Collections.IEqualityComparer<IBrowsableObjectInfoBase> GetDefaultEqualityComparer() => new WMIItemInfoEqualityComparer<IBrowsableObjectInfoBase>();

            public override System.Collections.Generic.IComparer<IBrowsableObjectInfoBase> GetDefaultComparer() => new WMIItemInfoComparer<IBrowsableObjectInfoBase>();
            #endregion // Methods
        }

        public class WMIItemInfo : WMIItemInfo<IWMIItemInfoProperties, ManagementBaseObject, IBrowsableObjectInfoSelectorDictionary<WMIItemInfoItemProvider>, WMIItemInfoItemProvider>, IWMIItemInfo
        {
            #region Consts
            public const string RootPath = @"\\.\";
            public const string NamespacePath = ":__NAMESPACE";
            public const string NameConst = "Name";
            public const string RootNamespace = "root:__namespace";
            public const string ROOT = "ROOT";
            #endregion

            #region Properties
            public static IBrowsableObjectInfoSelectorDictionary<WMIItemInfoItemProvider> DefaultItemSelectorDictionary { get; } = new WMIItemInfoSelectorDictionary();

            public override bool IsBrowsableByDefault => true;

            public sealed override IWMIItemInfoProperties ObjectPropertiesGeneric { get; }

            public override IPropertySystemCollection ObjectPropertySystem => null;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="WMIItemInfo"/> class as the WMI root item.
            /// </summary>
            public WMIItemInfo(in IWMIItemInfoOptions options, in ClientVersion clientVersion) : base(clientVersion) => ObjectPropertiesGeneric = new WMIItemInfoProperties(this, WMIItemType.Namespace, true, options);

            ///// <summary>
            ///// Initializes a new instance of the <see cref="WMIItemInfo"/> class. If you want to initialize this class in order to represent the root WMI item, you can also use the <see cref="WMIItemInfo()"/> constructor.
            ///// </summary>
            ///// <param name="path">The path of this <see cref="WMIItemInfo"/></param>.
            ///// <param name="wmiItemType">The type of this <see cref="WMIItemInfo"/>.</param>
            ///// <param name="managementObjectDelegate">The delegate that will be used by the <see cref="BrowsableObjectInfo.DeepClone()"/> method to get a new <see cref="ManagementBaseObject"/>.</param>
            ///// <param name="managementObject">The <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.</param>
            protected internal WMIItemInfo(in string path, in WMIItemType wmiItemType, in ManagementBaseObject managementObject, in IWMIItemInfoOptions options, in ClientVersion clientVersion) : base(new WMIItemInfoInitializer(path, managementObject), wmiItemType == WMIItemType.Instance ? null : GetName(managementObject, wmiItemType), clientVersion) => ObjectPropertiesGeneric = new WMIItemInfoProperties(this, wmiItemType, false, options);

            public WMIItemInfo(in WMIItemType itemType, in ManagementBaseObject managementObject, in IWMIItemInfoOptions options, in ClientVersion clientVersion) : this(GetPath(managementObject, itemType), itemType, managementObject, options, clientVersion)
            {
                // Left empty.
            }

            public WMIItemInfo(in string path, in WMIItemType itemType, in IWMIItemInfoOptions options, in ClientVersion clientVersion) : this(path, itemType, GetManagementClassFromPath(path, options), options, clientVersion)
            {
                // Left empty.

#else

            #region Methods
            //public static WMIItemInfoComparer<IWMIItemInfo> GetDefaultWMIItemInfoComparer() => new WMIItemInfoComparer<IWMIItemInfo>();

            /// <summary>
            /// Gets the name of the given <see cref="ManagementBaseObject"/>.
            /// </summary>
            /// <param name="managementObject">The <see cref="ManagementBaseObject"/> for which get the name.</param>
            /// <param name="wmiItemType">The <see cref="IO.PropertySystem.WMIItemType"/> of <paramref name="managementObject"/>.</param>
            /// <returns>The name of the given <see cref="ManagementBaseObject"/>.</returns>
            public static string GetName(ManagementBaseObject managementObject, WMIItemType wmiItemType)

                (managementObject as ManagementClass)?.Get();

                const string name = NameConst;
            }
                return wmiItemType == WMIItemType.Namespace ? (string)managementObject[name] : managementObject.ClassPath.ClassName;

            // public override bool CheckFilter(string path) => throw new NotImplementedException();

            private static System.Collections.Generic.IEnumerable<ManagementBaseObject> Enumerate(ManagementObjectCollection collection)
            {
                foreach (ManagementBaseObject value in collection)

                    yield return value;
            }

            /// <summary>
            /// Gets the path of the given <see cref="ManagementBaseObject"/>.
            /// </summary>
            /// <param name="managementObject">The <see cref="ManagementBaseObject"/> for which get the path.</param>
            /// <param name="wmiItemType">The <see cref="IO.PropertySystem.WMIItemType"/> of <paramref name="managementObject"/>.</param>
            /// <returns>The path of the given <see cref="ManagementBaseObject"/>.</returns>
            public static string GetPath(ManagementBaseObject managementObject, WMIItemType wmiItemType)
            {
                string path = $"{WinCopies.IO.Path.PathSeparator}{managementObject.ClassPath.Server}{WinCopies.IO.Path.PathSeparator}{managementObject.ClassPath.NamespacePath}";

                string name = GetName(managementObject, wmiItemType);

                string _getPath() => $"{path}:{managementObject.ClassPath.ClassName}";

                path = name == null ? _getPath() : $"{WinCopies.IO.Path.PathSeparator}{name}" + _getPath();

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

            public override IBrowsableObjectInfoSelectorDictionary<WMIItemInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

            #region GetItems
            protected override System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider> GetItemProviders(Predicate<ManagementBaseObject> predicate) => GetItemProviders(predicate, ObjectPropertiesGeneric.Options);

            protected override System.Collections.Generic.IEnumerable<WMIItemInfoItemProvider> GetItemProviders() => GetItemProviders(item => true, ObjectPropertiesGeneric.Options);

                var managementClass = InnerObjectGeneric as ManagementClass;

                // string _path;

                bool dispose = false;

#pragma warning disable IDE0019 // Pattern Matching
                var managementClass = _managementObject as ManagementClass;
#pragma warning restore IDE0019 // Pattern Matching

                if (managementClass == null)
                {
                    dispose = true;

                    managementClass = GetManagementClassFromPath(Path, options);
                }

                managementClass.Get();

                try
                {
                    static System.Collections.Generic.IEnumerable<ManagementBaseObject> _as(in ManagementObjectCollection collection) => collection.As<ManagementBaseObject>();

                    static System.Collections.Generic.IEnumerable<ManagementBaseObject> enumerateInstances(in ManagementClass managementClass, in IWMIItemInfoOptions options) => _as(options?.EnumerationOptions == null ? managementClass.GetInstances() : managementClass.GetInstances(options?.EnumerationOptions));

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
            #endregion // GetItems
            #endregion // Methods
        }
    }
}
