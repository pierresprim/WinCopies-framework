/* Copyright © Pierre Sprimont, 2019
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

using System;
using System.Text;
using System.Windows.Media.Imaging;
using static WinCopies.Util.Util;
using System.Management;
using System.Windows;
using System.Windows.Interop;
using System.Drawing;
using System.Globalization;
using WinCopies.Util;
using System.Security;
using static WinCopies.IO.WMIItemInfo;
using static WinCopies.IO.Path;

namespace WinCopies.IO
{

    public delegate ManagementObject ManagementObjectDeepClone(ManagementObject managementObject, SecureString password);

    public delegate ManagementClass ManagementClassDeepClone(ManagementClass managementClass, SecureString password);

    public delegate ConnectionOptions ConnectionOptionsDeepClone(ConnectionOptions connectionOptions, SecureString password);

    public class WMIItemInfo/*<TItems, TFactory>*/ : BrowsableObjectInfo/*<TItems, TFactory>*/, IWMIItemInfo // where TItems : BrowsableObjectInfo<TItems, TFactory>, IWMIItemInfo where TFactory : BrowsableObjectInfoFactory, IWMIItemInfoFactory
    {

        // public override bool IsRenamingSupported => false;

        #region Consts

        public const string RootPath = @"\\.\ROOT:__NAMESPACE";
        public const string NamespacePath = ":__NAMESPACE";
        public const string NameConst = "Name";
        public const string RootNamespace = "root:__namespace";
        public const string ROOT = "ROOT";

        #endregion

        #region Fields

        private DeepClone<ManagementBaseObject> _managementObjectDelegate;

        #endregion

        #region Properties

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

        public static ConnectionOptionsDeepClone DefaultConnectionOptionsDeepClone { get; } = (ConnectionOptions connectionOptions, SecureString password) => new ConnectionOptions()
        {
            Locale = connectionOptions.Locale,
            Username = connectionOptions.Username,
            SecurePassword = password,
            Authority = connectionOptions.Authority,
            Impersonation = connectionOptions.Impersonation,
            Authentication = connectionOptions.Authentication,
            EnablePrivileges = connectionOptions.EnablePrivileges,
            Timeout = connectionOptions.Timeout
        };

        public static DeepClone<ManagementPath> DefaultManagementPathDeepClone { get; } = managementPath => new ManagementPath() { Path = managementPath.Path, ClassName = managementPath.ClassName, NamespacePath = managementPath.NamespacePath, RelativePath = managementPath.RelativePath, Server = managementPath.Server };

        public static DeepClone<ObjectGetOptions> DefaultObjectGetOptionsDeepClone { get; } = objectGetOptions => new ObjectGetOptions() { Timeout = objectGetOptions.Timeout, UseAmendedQualifiers = objectGetOptions.UseAmendedQualifiers };

        public static ManagementObjectDeepClone DefaultManagementObjectDeepClone { get; } = (ManagementObject managementObject, SecureString password) =>

        {

            ManagementObject _managementObject = managementObject as ManagementClass ?? managementObject as ManagementObject ?? throw new ArgumentException("managementObject must be a ManagementClass or a ManagementObject.", nameof(managementObject));

            ManagementPath path = DefaultManagementPathDeepClone(_managementObject.Scope?.Path ?? _managementObject.Path);

            return _managementObject is ManagementClass managementClass ? DefaultManagementClassDeepCloneDelegate(managementClass, null) : new ManagementObject(
                new ManagementScope(
    path,
                    _managementObject.Scope?.Options is null ? null : DefaultConnectionOptionsDeepClone(_managementObject.Scope?.Options, password)
                    ), path, _managementObject.Options is null ? null : DefaultObjectGetOptionsDeepClone(_managementObject.Options));

        };

        public static ManagementClassDeepClone DefaultManagementClassDeepCloneDelegate { get; } = (ManagementClass managementClass, SecureString password) =>

        {

            ManagementPath path = DefaultManagementPathDeepClone(managementClass.Scope?.Path ?? managementClass.Path);

            return new ManagementClass(
                new ManagementScope(
    path,
                    managementClass?.Scope?.Options is null ? null : DefaultConnectionOptionsDeepClone(managementClass?.Scope?.Options, password)
                    ), path, managementClass.Options is null ? null : DefaultObjectGetOptionsDeepClone(managementClass.Options));

        };

        /// <summary>
        /// Gets the <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.
        /// </summary>
        public ManagementBaseObject ManagementObject { get; private set; }

        public override bool NeedsObjectsOrValuesReconstruction => true;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WMIItemInfo"/> class as the root WMI item.
        /// </summary>
        public WMIItemInfo() : this(RootPath, WMIItemType.Namespace, new ManagementClass(RootPath), (ManagementBaseObject managementObject) => DefaultManagementClassDeepCloneDelegate((ManagementClass)managementObject, null)) => IsRootNode = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="WMIItemInfo"/> class. If you want to initialize this class in order to represent the root WMI item, you can also use the <see cref="WMIItemInfo()"/> constructor.
        /// </summary>
        /// <param name="path">The path of this <see cref="WMIItemInfo"/></param>.
        /// <param name="wmiItemType">The type of this <see cref="WMIItemInfo"/>.</param>
        /// <param name="managementObjectDelegate">The delegate that will be used by the <see cref="BrowsableObjectInfo.DeepClone()"/> method to get a new <see cref="ManagementBaseObject"/>.</param>
        /// <param name="managementObject">The <see cref="ManagementBaseObject"/> that this <see cref="WMIItemInfo"/> represents.</param>
        public WMIItemInfo(string path, WMIItemType wmiItemType, ManagementBaseObject managementObject, DeepClone<ManagementBaseObject> managementObjectDelegate) : base(path)
        {

            ThrowIfNull(managementObjectDelegate, nameof(managementObjectDelegate));

            ThrowIfNull(managementObject, nameof(managementObject));

            ThrowOnEnumNotValidEnumValue(wmiItemType, WMIItemType.Namespace, WMIItemType.Class);

            _managementObjectDelegate = managementObjectDelegate;

            ManagementObject = managementObject;

            if (wmiItemType != WMIItemType.Instance)

                Name = GetName(ManagementObject, wmiItemType);

            WMIItemType = wmiItemType;

            if (wmiItemType == WMIItemType.Namespace && Path.ToUpper().EndsWith("ROOT:__NAMESPACE"))

                IsRootNode = true;

        }

        #endregion

        #region Public methods

        public static WMIItemInfoComparer<IWMIItemInfo> GetDefaultWMIItemInfoComparer() => new WMIItemInfoComparer<IWMIItemInfo>();

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

            string path = PathSeparator + managementObject.ClassPath.Server + PathSeparator + managementObject.ClassPath.NamespacePath;

            string name = GetName(managementObject, wmiItemType);

            if (name != null)

                path += PathSeparator + name;

            path += ":" + managementObject.ClassPath.ClassName;

            return path;

        }

        /// <summary>
        /// Gets a new <see cref="WMIItemInfo"/> that corresponds to the given server name and relative path.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        /// <param name="serverRelativePath">The server relative path.</param>
        /// <returns>A new <see cref="WMIItemInfo"/> that corresponds to the given server name and relative path.</returns>
        /// <seealso cref="WMIItemInfo()"/>
        /// <seealso cref="WMIItemInfo(string, WMIItemType, ManagementBaseObject, DeepClone{ManagementBaseObject})"/>
        public static WMIItemInfo GetWMIItemInfo(string serverName, string serverRelativePath)

        {

            var stringBuilder = new StringBuilder();

            _ = stringBuilder.Append(PathSeparator);

            _ = stringBuilder.Append(PathSeparator);

            _ = stringBuilder.Append(serverName);

            _ = stringBuilder.Append(PathSeparator);

            _ = stringBuilder.Append(IsNullEmptyOrWhiteSpace(serverRelativePath) ? ROOT : serverRelativePath);

            _ = stringBuilder.Append(NamespacePath);

            string path = stringBuilder.ToString();

            return new WMIItemInfo(path, WMIItemType.Namespace, new ManagementClass(path), managementObject => DefaultManagementClassDeepCloneDelegate((ManagementClass)managementObject, null));

        }

        public override bool Equals(object obj) => ReferenceEquals(this, obj)
                ? true : obj is IWMIItemInfo _obj ? WMIItemType == _obj.WMIItemType && Path.ToLower() == _obj.Path.ToLower()
                : false;

        public int CompareTo(IWMIItemInfo other) => GetDefaultWMIItemInfoComparer().Compare(this, other);

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
        public override int GetHashCode() => base.GetHashCode() ^ WMIItemType.GetHashCode();

        #endregion

        #region Protected methods

        //public new WMIItemInfoFactory Factory { get => (WMIItemInfoFactory)base.Factory; set => base.Factory = value; }

        protected override BrowsableObjectInfo DeepCloneOverride() => IsRootNode ? new WMIItemInfo() : new WMIItemInfo(Path, WMIItemType, _managementObjectDelegate(ManagementObject), _managementObjectDelegate);

        public static WMIItemInfo GetWMIItemInfo(string path, WMIItemType wmiItemType, ConnectionOptions connectionOptions, ObjectGetOptions objectGetOptions) => new WMIItemInfo(path, wmiItemType, new ManagementObject(
        new ManagementScope(
            path,
            connectionOptions is null
                ? null
                : WMIItemInfo.DefaultConnectionOptionsDeepClone(
                    connectionOptions, null)),
                    new ManagementPath(path),
                    objectGetOptions is null
                        ? null
                        : WMIItemInfo.DefaultObjectGetOptionsDeepClone(objectGetOptions)
    ),
    _managementObject => _managementObject is ManagementClass managementClass
        ? WMIItemInfo.DefaultManagementClassDeepCloneDelegate(
            managementClass,
            null)
        : _managementObject is ManagementObject __managementObject
            ? WMIItemInfo.DefaultManagementObjectDeepClone(
                __managementObject,
                null)
            : throw new ArgumentException("The given object must be a ManagementClass or a ManagementObject.", "managementObject"));

        protected override IBrowsableObjectInfo GetParent()
        {

            if (IsRootNode) return null;

            string path;

            switch (WMIItemType)

            {

                case WMIItemType.Namespace:

                    path = Path.Substring(0, Path.LastIndexOf(PathSeparator)) + NamespacePath;

                    return path.EndsWith(RootNamespace, true, CultureInfo.InvariantCulture)
                        ? new WMIItemInfo()
                        : GetWMIItemInfo(path, WMIItemType.Namespace, null, null);

                case WMIItemType.Class:

                    return Path.EndsWith("root:" + Name, true, CultureInfo.InvariantCulture)
                        ? new WMIItemInfo()
                        : GetWMIItemInfo(Path.Substring(0, Path.IndexOf(':')) + NamespacePath, WMIItemType.Namespace, null, null);

                case WMIItemType.Instance:

                    path = Path.Substring(0, Path.IndexOf(':'));

                    path = path.Substring(0, path.LastIndexOf(PathSeparator)) + ':' + path.Substring(path.LastIndexOf(PathSeparator) + 1);

                    return GetWMIItemInfo(path, WMIItemType.Class, null, null);

                default: // We souldn't reach this point.

                    return null;

            }

        }

        //#pragma warning disable IDE0067 // Dispose objects before losing scope
        //        public override void LoadItems(bool workerReportsProgress, bool workerSupportsCancellation) => LoadItems(GetDefaultWMIItemsLoader(workerReportsProgress, workerSupportsCancellation));

        //        public override void LoadItemsAsync(bool workerReportsProgress, bool workerSupportsCancellation) => LoadItemsAsync(GetDefaultWMIItemsLoader(workerReportsProgress, workerSupportsCancellation));
        //#pragma warning restore IDE0067 // Dispose objects before losing scope

        ///// <summary>
        ///// Not implemented.
        ///// </summary>
        ///// <param name="newValue"></param>
        //public override void Rename(string newValue) => throw new NotImplementedException();

        /// <summary>
        /// Disposes the current <see cref="WMIItemInfo"/> and its parent and items recursively.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="BrowsableObjectInfo.ItemsLoader"/> is busy and does not support cancellation.</exception>
        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);

            ManagementObject.Dispose();

            if (disposing)

            {

                ManagementObject = null;

                _managementObjectDelegate = null;

            }

        }

        #endregion

        private BitmapSource TryGetBitmapSource(System.Drawing.Size size)

        {

            int iconIndex = 0;

            if (IsRootNode)

                iconIndex = 15;

            else if (WMIItemType == WMIItemType.Namespace || WMIItemType == WMIItemType.Class)

                iconIndex = 3;

            using (Icon icon = TryGetIcon(iconIndex, "shell32.dll", size))

                return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        }

    }

}
