#region Usings
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Management;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.PropertySystem;

#region Static Usings
using static System.IO.Path;

using static WinCopies.IO.ObjectModel.WMIItemInfo;
using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;
using System.Globalization;
using System.Linq;
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
                Shell.ComponentSources.Bitmap.BitmapSourceProvider.Create(this, new BrowsableObjectInfoBitmapSources(this), true)
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

                        _description = value == null ? WinCopies.Consts.
#if WinCopies4
                        Common.
#endif
                        NotApplicable : (string)value;
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
            public override IBrowsableObjectInfo Clone()
            {
                IWMIItemInfoProperties properties = ObjectPropertiesGeneric;

                return properties.IsRootNode
                    ? new WMIItemInfo()
                    : (IBrowsableObjectInfo)new WMIItemInfo(Path, properties.ItemType, InnerObjectGenericOverride, properties.Options, ClientVersion);
            }

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
    }
}
