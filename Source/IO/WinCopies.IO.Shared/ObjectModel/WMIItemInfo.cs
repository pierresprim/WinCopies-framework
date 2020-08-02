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
 * along with the WinCopies Framework.If not, see<https://www.gnu.org/licenses/>. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Management;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinCopies.Collections;
using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.Util.Util;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// The WMI item type.
    /// </summary>
    public enum WMIItemType
    {
        /// <summary>
        /// The WMI item is a namespace.
        /// </summary>
        Namespace,

        /// <summary>
        /// The WMI item is a class.
        /// </summary>
        Class,

        /// <summary>
        /// The WMI item is an instance.
        /// </summary>
        Instance
    }

    public class WMIItemInfo : BrowsableObjectInfo, IWMIItemInfo
    {
        #region Consts

        public const string RootPath = @"\\.\";
        public const string NamespacePath = ":__NAMESPACE";
        public const string NameConst = "Name";
        public const string RootNamespace = "root:__namespace";
        public const string ROOT = "ROOT";

        #endregion

        private IBrowsableObjectInfo _parent;

        #region Properties

        public override bool IsSpecialItem => false;

        private string _itemTypeName;

        public override string ItemTypeName
        {
            get
            {
                if (string.IsNullOrEmpty(_itemTypeName))

                    switch (WMIItemType)
                    {
                        case WMIItemType.Namespace:
                            _itemTypeName = "WMI Namespace";
                            break;
                        case WMIItemType.Class:
                            _itemTypeName = "WMI Class";
                            break;
                        case WMIItemType.Instance:
                            _itemTypeName = "WMI Instance";
                            break;
                    }

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

                    object value = ManagementObject.Qualifiers["Description"].Value;

                    _description = value == null ? "N/A" : (string)value;

                }

                return _description;
            }
        }

        public override Size? Size => null;

        public override FileSystemType ItemFileSystemType => FileSystemType.WMI;

#if NETCORE

        public override IBrowsableObjectInfo Parent => _parent ??= GetParent();

#else

        public override IBrowsableObjectInfo Parent => _parent ?? (_parent = GetParent());

#endif

        /// <summary>
        /// Gets a value that indicates whether this <see cref="WMIItemInfo"/> represents a root node.
        /// </summary>
        public bool IsRootNode { get; }

        /// <summary>
        /// Gets the localized path of this <see cref="WMIItemInfo"/>.
        /// </summary>
        public override string LocalizedName => Name;

        /// <summary>
        /// Gets the name of this <see cref="WMIItemInfo"/>.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Gets the small <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
        /// </summary>
        public override BitmapSource SmallBitmapSource => TryGetBitmapSource(new System.Drawing.Size(16, 16));

        /// <summary>
        /// Gets the medium <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
        /// </summary>
        public override BitmapSource MediumBitmapSource => TryGetBitmapSource(new System.Drawing.Size(48, 48));

        /// <summary>
        /// Gets the large <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
        /// </summary>
        public override BitmapSource LargeBitmapSource => TryGetBitmapSource(new System.Drawing.Size(128, 128));

        /// <summary>
        /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="WMIItemInfo"/>.
        /// </summary>
        public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(new System.Drawing.Size(256, 256));

        /// <summary>
        /// Gets a value that indicates whether this <see cref="WMIItemInfo"/> is browsable.
        /// </summary>
        public override bool IsBrowsable => WMIItemType == WMIItemType.Namespace || WMIItemType == WMIItemType.Class;

        public WMIItemType WMIItemType { get; }

        //public static ConnectionOptionsDeepClone DefaultConnectionOptionsDeepClone { get; } = (ConnectionOptions connectionOptions, SecureString password) => new ConnectionOptions()
        //{
        //    Locale = connectionOptions.Locale,
        //    Username = connectionOptions.Username,
        //    SecurePassword = password,
        //    Authority = connectionOptions.Authority,
        //    Impersonation = connectionOptions.Impersonation,
        //    Authentication = connectionOptions.Authentication,
        //    EnablePrivileges = connectionOptions.EnablePrivileges,
        //    Timeout = connectionOptions.Timeout
        //};

        //    public static DeepClone<ManagementPath> DefaultManagementPathDeepClone { get; } = managementPath => new ManagementPath() { Path = managementPath.Path, ClassName = managementPath.ClassName, NamespacePath = managementPath.NamespacePath, RelativePath = managementPath.RelativePath, Server = managementPath.Server };

        //    public static DeepClone<ObjectGetOptions> DefaultObjectGetOptionsDeepClone { get; } = objectGetOptions => new ObjectGetOptions() { Timeout = objectGetOptions.Timeout, UseAmendedQualifiers = objectGetOptions.UseAmendedQualifiers };

        //    public static ManagementObjectDeepClone DefaultManagementObjectDeepClone { get; } = (ManagementObject managementObject, SecureString password) =>

        //    {

        //        ManagementObject _managementObject = managementObject as ManagementClass ?? managementObject as ManagementObject ?? throw new ArgumentException("managementObject must be a ManagementClass or a ManagementObject.", nameof(managementObject));

        //        ManagementPath path = DefaultManagementPathDeepClone(_managementObject.Scope?.Path ?? _managementObject.Path);

        //        return _managementObject is ManagementClass managementClass ? DefaultManagementClassDeepCloneDelegate(managementClass, null) : new ManagementObject(
        //            new ManagementScope(
        //path,
        //                _managementObject.Scope?.Options is null ? null : DefaultConnectionOptionsDeepClone(_managementObject.Scope?.Options, password)
        //                ), path, _managementObject.Options is null ? null : DefaultObjectGetOptionsDeepClone(_managementObject.Options));

        //    };

        //    public static ManagementClassDeepClone DefaultManagementClassDeepCloneDelegate { get; } = (ManagementClass managementClass, SecureString password) =>

        //    {

        //        ManagementPath path = DefaultManagementPathDeepClone(managementClass.Scope?.Path ?? managementClass.Path);

        //        return new ManagementClass(
        //            new ManagementScope(
        //path,
        //                managementClass?.Scope?.Options is null ? null : DefaultConnectionOptionsDeepClone(managementClass?.Scope?.Options, password)
        //                ), path, managementClass.Options is null ? null : DefaultObjectGetOptionsDeepClone(managementClass.Options));

        //    };

        /// <summary>
        /// Gets the <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.
        /// </summary>
        public ManagementBaseObject ManagementObject { get; private set; }

        //public override bool NeedsObjectsOrValuesReconstruction => true;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WMIItemInfo"/> class as the root WMI item.
        /// </summary>
        public WMIItemInfo() : this($"{RootPath}{ROOT}{NamespacePath}", WMIItemType.Namespace, new ManagementClass($"{RootPath}{ROOT}{NamespacePath}")) => IsRootNode = true;

        public WMIItemInfo(WMIItemType wmiItemType, ManagementBaseObject managementBaseObject) : this(GetPath(managementBaseObject, wmiItemType), wmiItemType, managementBaseObject) { }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="WMIItemInfo"/> class. If you want to initialize this class in order to represent the root WMI item, you can also use the <see cref="WMIItemInfo()"/> constructor.
        ///// </summary>
        ///// <param name="path">The path of this <see cref="WMIItemInfo"/></param>.
        ///// <param name="wmiItemType">The type of this <see cref="WMIItemInfo"/>.</param>
        ///// <param name="managementObjectDelegate">The delegate that will be used by the <see cref="BrowsableObjectInfo.DeepClone()"/> method to get a new <see cref="ManagementBaseObject"/>.</param>
        ///// <param name="managementObject">The <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.</param>
        public WMIItemInfo(string path, WMIItemType wmiItemType, ManagementBaseObject managementObject) : base(path)
        {
            //ThrowIfNull(managementObjectDelegate, nameof(managementObjectDelegate));

            ThrowIfNull(managementObject, nameof(managementObject));

            // wmiItemType.ThrowIfInvalidEnumValue(true, WMIItemType.Namespace, WMIItemType.Class);

            //_managementObjectDelegate = managementObjectDelegate;

            ManagementObject = managementObject;

            if (wmiItemType != WMIItemType.Instance)

                Name = GetName(ManagementObject, wmiItemType);

            WMIItemType = wmiItemType;

            if (wmiItemType == WMIItemType.Namespace && Path.ToUpper().EndsWith("ROOT:__NAMESPACE"))

                IsRootNode = true;
        }

        #endregion

        #region Public methods

        //public static WMIItemInfoComparer<IWMIItemInfo> GetDefaultWMIItemInfoComparer() => new WMIItemInfoComparer<IWMIItemInfo>();

        /// <summary>
        /// Gets the name of the given <see cref="ManagementBaseObject"/>.
        /// </summary>
        /// <param name="managementObject">The <see cref="ManagementBaseObject"/> for which get the name.</param>
        /// <param name="wmiItemType">The <see cref="IO.WMIItemType"/> of <paramref name="managementObject"/>.</param>
        /// <returns>The name of the given <see cref="ManagementBaseObject"/>.</returns>
        public static string GetName(ManagementBaseObject managementObject, WMIItemType wmiItemType)
        {
            (managementObject as ManagementClass)?.Get();

            const string name = NameConst;

            return wmiItemType == WMIItemType.Namespace ? (string)managementObject[name] : managementObject.ClassPath.ClassName;
        }

        /// <summary>
        /// Gets the path of the given <see cref="ManagementBaseObject"/>.
        /// </summary>
        /// <param name="managementObject">The <see cref="ManagementBaseObject"/> for which get the path.</param>
        /// <param name="wmiItemType">The <see cref="IO.WMIItemType"/> of <paramref name="managementObject"/>.</param>
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

        #endregion

        private IBrowsableObjectInfo GetParent()
        {
            if (IsRootNode) return null;

            string path;

            switch (WMIItemType)
            {
                case WMIItemType.Namespace:

                    path = Path.Substring(0, Path.LastIndexOf(WinCopies.IO.Path.PathSeparator)) + NamespacePath;

                    return path.EndsWith(RootNamespace, true, CultureInfo.InvariantCulture)
                        ? new WMIItemInfo()
                        : new WMIItemInfo(path, WMIItemType.Namespace, null);

                case WMIItemType.Class:

                    return Path.EndsWith("root:" + Name, true, CultureInfo.InvariantCulture)
                        ? new WMIItemInfo()
                        : new WMIItemInfo(Path.Substring(0, Path.IndexOf(':')) + NamespacePath, WMIItemType.Namespace, null);

                case WMIItemType.Instance:

                    path = Path.Substring(0, Path.IndexOf(':'));

                    int splitIndex = path.LastIndexOf(WinCopies.IO.Path.PathSeparator);

                    path = $"{path.Substring(0, splitIndex)}:{path.Substring(splitIndex + 1)}";

                    return new WMIItemInfo(path, WMIItemType.Class, null);

                default: // We souldn't reach this point.

                    return null;
            }
        }

        protected override void Dispose(in bool disposing)
        {
            base.Dispose(disposing);

            ManagementObject.Dispose();

            if (disposing)
                //{
                ManagementObject = null;

            //_managementObjectDelegate = null;
            //}
        }

        private BitmapSource TryGetBitmapSource(System.Drawing.Size size)
        {
            int iconIndex = 0;

            if (IsRootNode)

                iconIndex = 15;

            else if (WMIItemType == WMIItemType.Namespace || WMIItemType == WMIItemType.Class)

                iconIndex = 3;

#if NETFRAMEWORK

            using (Icon icon = TryGetIcon(iconIndex, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size))

#else

            using Icon icon = TryGetIcon(iconIndex, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);

#endif

            return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        // public override bool CheckFilter(string path) => throw new NotImplementedException();

        private static IEnumerable<ManagementBaseObject> Enumerate(ManagementObjectCollection collection)
        {
            foreach (ManagementBaseObject value in collection)

                yield return value;
        }

        private static IEnumerable<ManagementBaseObject> EnumerateInstances(ManagementClass managementClass, IWMIItemInfoFactory factory)
        {
            ManagementObjectCollection collection = factory.Options?.EnumerationOptions == null ? managementClass.GetInstances() : managementClass.GetInstances(factory.Options?.EnumerationOptions);

            return collection == null || collection.Count == 0 ? null : Enumerate(collection);
        }

        private static IEnumerable<ManagementBaseObject> EnumerateSubClasses(ManagementClass managementClass, IWMIItemInfoFactory factory)
        {
            ManagementObjectCollection collection = factory?.Options?.EnumerationOptions == null ? managementClass.GetSubclasses() : managementClass.GetSubclasses(factory?.Options?.EnumerationOptions);

            return collection == null || collection.Count == 0 ? null : Enumerate(collection);
        }

        public override IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(new WMIItemInfoFactory(), null, false);

        public IEnumerable<IBrowsableObjectInfo> GetItems(IWMIItemInfoFactory factory, Predicate<ManagementBaseObject> predicate, bool catchExceptionsDuringEnumeration)
        {
            // var paths = new ArrayBuilder<PathInfo>();

            // string _path;

            bool dispose = false;

#pragma warning disable IDE0019 // Pattern Matching
            var managementClass = ManagementObject as ManagementClass;
#pragma warning restore IDE0019 // Pattern Matching

            if (managementClass == null)
            {
                dispose = true;

                // #pragma warning disable IDE0067 // Dispose objects before losing scope
                managementClass = new ManagementClass(new ManagementScope(Path, factory.Options?.ConnectionOptions), new ManagementPath(Path), factory.Options?.ObjectGetOptions);
                // #pragma warning restore IDE0067 // Dispose objects before losing scope
            }

            managementClass.Get();

            try
            {
                if (WMIItemType == WMIItemType.Namespace)
                {
                    IEnumerable<ManagementBaseObject> namespaces = EnumerateInstances(managementClass, factory);

                    IEnumerable<ManagementBaseObject> classes = EnumerateSubClasses(managementClass, factory);

                    if (predicate != null)
                    {
                        if (namespaces != null)

                            namespaces = namespaces.WherePredicate(predicate);

                        if (classes != null)

                            classes = classes.WherePredicate(predicate);
                    }

                    if (namespaces == null) return new Enumerable<WMIItemInfo>(() => new WMIItemInfoEnumerator(classes, false, WMIItemType.Class, catchExceptionsDuringEnumeration));

                    else if (classes == null) return new Enumerable<WMIItemInfo>(() => new WMIItemInfoEnumerator(namespaces, false, WMIItemType.Namespace, catchExceptionsDuringEnumeration));

                    else return new Enumerable<IBrowsableObjectInfo>(() => new WMIItemInfoEnumerator(namespaces, false, WMIItemType.Namespace, catchExceptionsDuringEnumeration)).AppendValues(new Enumerable<IBrowsableObjectInfo>(() => new WMIItemInfoEnumerator(classes, false, WMIItemType.Class, catchExceptionsDuringEnumeration)));
                }

                else if (WMIItemType == WMIItemType.Class /*&& WMIItemTypes.HasFlag(WMIItemTypes.Instance)*/)
                {
                    managementClass.Get();

                    IEnumerable<ManagementBaseObject> items = predicate == null ? EnumerateInstances(managementClass, factory) : EnumerateInstances(managementClass, factory).WherePredicate(predicate);

                    return items == null ? null : new Enumerable<IBrowsableObjectInfo>(() => new WMIItemInfoEnumerator(items, false, WMIItemType.Instance, catchExceptionsDuringEnumeration));
                }

                return null;
            }

            finally
            {
                if (dispose)

                    managementClass.Dispose();
            }
        }

        public override Collections.IEqualityComparer<IFileSystemObject> GetDefaultEqualityComparer() => new WMIItemInfoEqualityComparer<IFileSystemObject>();

        public override System.Collections.Generic.IComparer<IFileSystemObject> GetDefaultComparer() => new WMIItemInfoComparer<IFileSystemObject>();
    }
}
