#region Usings
using System;
using System.Management;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.PropertySystem;

#region Static Usings
using static System.IO.Path;

using static WinCopies.UtilHelpers;
using WinCopies.Util;
#endregion Static Usings
#endregion Usings

namespace WinCopies.IO.ObjectModel
{
    public partial class WMIItemInfo : WMIItemInfo<IWMIItemInfoProperties, ManagementBaseObject, IEnumerableSelectorDictionary<WMIItemInfoItemProvider, IBrowsableObjectInfo>, WMIItemInfoItemProvider>, IWMIItemInfo
    {
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
