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
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Media.Imaging;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;
using WinCopies.PropertySystem;
using WinCopies.Util;

using static Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames;

using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// Represents a Windows registry item.
    /// </summary>
    public abstract class RegistryItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>/*<TItems, TFactory>*/ : BrowsableObjectInfo<TObjectProperties, RegistryKey, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>/*<TItems, TFactory>*/, IRegistryItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IRegistryItemInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems> // where TItems : BrowsableObjectInfo, IRegistryItemInfo where TFactory : IRegistryItemInfoFactory
    {
        #region Fields
        internal RegistryKey _registryKey;
        private IBrowsableObjectInfo _parent;
        private IBrowsabilityOptions _browsability;
        #endregion

        #region Properties
        public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> RootItems => RegistryItemInfo.DefaultRootItems;

        ///// <summary>
        ///// Gets a value that indicates whether this object needs to reconstruct objects on deep cloning.
        ///// </summary>
        //public override bool NeedsObjectsOrValuesReconstruction => _registryKey is object; // If _registryKey is null, reconstructing registry does not make sense, so we return false.

        //public static DeepClone<RegistryKey> DefaultRegistryKeyDeepClone { get; } = registryKey => Registry.OpenRegistryKey(registryKey.Name);

        public override bool IsSpecialItem => false;

        private string _itemTypeName;

        public override string ItemTypeName
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

        public override string Description => NotApplicable;

        /// <summary>
        /// Gets the localized path of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override string LocalizedName => Name;

        /// <summary>
        /// Gets the name of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override string Name { get; }

        #region BitmapSources
        /// <summary>
        /// Gets the small <see cref="BitmapSource"/> of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override BitmapSource SmallBitmapSource => TryGetBitmapSource(SmallIconSize);

        /// <summary>
        /// Gets the medium <see cref="BitmapSource"/> of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override BitmapSource MediumBitmapSource => TryGetBitmapSource(MediumIconSize);

        /// <summary>
        /// Gets the large <see cref="BitmapSource"/> of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override BitmapSource LargeBitmapSource => TryGetBitmapSource(LargeIconSize);

        /// <summary>
        /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(ExtraLargeIconSize);
        #endregion

        /// <summary>
        /// Gets a value that indicates whether this <see cref="RegistryItemInfo"/> is browsable.
        /// </summary>
        public override IBrowsabilityOptions Browsability
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

        public override bool IsRecursivelyBrowsable { get; } = true;

        public override IBrowsableObjectInfo Parent => _parent
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

        ///// <summary>
        ///// The <see cref="RegistryKey"/> that this <see cref="RegistryItemInfo"/> represents.
        ///// </summary>
        public sealed override RegistryKey InnerObjectGeneric
        {
            get
            {
                if (_registryKey == null)

                    OpenKey();

                return _registryKey;
            }
        }

        // public override FileSystemType ItemFileSystemType => FileSystemType.Registry;
        #endregion // Properties

        #region Constructors
        public RegistryItemInfo() : this(GetDefaultClientVersion()) { /* Left empty. */ }

        public RegistryItemInfo(in ClientVersion clientVersion) : base(Properties.Resources.RegistryRoot, clientVersion) => Name = Path;

        public RegistryItemInfo(in RegistryKey registryKey, in ClientVersion clientVersion) : base(GetRegistryKeyName(registryKey), clientVersion)
        {
            Name = registryKey.Name.Split(WinCopies.IO.Path.PathSeparator).GetLast();

            _registryKey = registryKey;
        }

        public RegistryItemInfo(in string path, in ClientVersion clientVersion) : base(IsNullEmptyOrWhiteSpace(path) ? throw GetNullEmptyOrWhiteSpaceStringException(nameof(path)) : path, clientVersion) => Name = path.Split(WinCopies.IO.Path.PathSeparator).GetLast();

        public RegistryItemInfo(in RegistryKey registryKey, in string valueName, in ClientVersion clientVersion) : base($"{GetRegistryKeyName(registryKey)}{WinCopies.IO.Path.PathSeparator}{(IsNullEmptyOrWhiteSpace(valueName) ? throw GetNullEmptyOrWhiteSpaceStringException(nameof(valueName)) : valueName)}", clientVersion)
        {
            Name = valueName;

            _registryKey = registryKey;
        }

        public RegistryItemInfo(in string registryKeyPath, in string valueName, in ClientVersion clientVersion) : this(Registry.OpenRegistryKey(registryKeyPath), valueName, clientVersion)
        {
            // Left empty.
        }
        #endregion // Constructors

        #region Methods
        private static string GetRegistryKeyName(in RegistryKey registryKey) => (registryKey ?? throw GetArgumentNullException(nameof(registryKey))).Name;

        ///// <summary>
        ///// Gets a default comparer for <see cref="FileSystemObject"/>s.
        ///// </summary>
        ///// <returns>A default comparer for <see cref="FileSystemObject"/>s.</returns>
        //public static RegistryItemInfoComparer<IRegistryItemInfo> GetDefaultRegistryItemInfoComparer() => new RegistryItemInfoComparer<IRegistryItemInfo>();

        ///// <summary>
        ///// Determines whether the specified object is equal to the current object by calling <see cref="FileSystemObject.Equals(object)"/> and then, if the result is <see langword="true"/>, by testing the following things, in order: <paramref name="obj"/> implements the <see cref="IRegistryItemInfo"/> interface and <see cref="RegistryItemType"/> are equal.
        ///// </summary>
        ///// <param name="obj">The object to compare with the current object.</param>
        ///// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
        //public override bool Equals(object obj) => base.Equals(obj) && (obj is IRegistryItemInfo _obj ? RegistryItemType == _obj.RegistryItemType : false);

        ///// <summary>
        ///// Compares the current object to a given <see cref="FileSystemObject"/>.
        ///// </summary>
        ///// <param name="registryItemInfo">The <see cref="FileSystemObject"/> to compare with.</param>
        ///// <returns>The comparison result. See <see cref="IComparable{T}.CompareTo(T)"/> for more details.</returns>
        //public int CompareTo(IRegistryItemInfo registryItemInfo) => GetDefaultRegistryItemInfoComparer().Compare(this, registryItemInfo);

        ///// <summary>
        ///// Determines whether the specified <see cref="IRegistryItemInfo"/> is equal to the current object by calling the <see cref="Equals(object)"/> method.
        ///// </summary>
        ///// <param name="registryItemInfo">The <see cref="IRegistryItemInfo"/> to compare with the current object.</param>
        ///// <returns><see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>.</returns>
        //public bool Equals(IRegistryItemInfo registryItemInfo) => Equals(registryItemInfo as object);

        ///// <summary>
        ///// Gets an hash code for this <see cref="RegistryItemInfo"/>.
        ///// </summary>
        ///// <returns>The hash code returned by the <see cref="FileSystemObject.GetHashCode"/> and the hash code of the <see cref="RegistryItemType"/>.</returns>
        //public override int GetHashCode() => base.GetHashCode() ^ RegistryItemType.GetHashCode();

        /// <summary>
        /// Opens the <see cref="RegistryKey"/> that this <see cref="RegistryItemInfo"/> represents.
        /// </summary>
        public void OpenKey() => _registryKey = ObjectPropertiesGeneric.RegistryItemType == RegistryItemType.Key ? Registry.OpenRegistryKey(Path) : throw new InvalidOperationException("This item does not represent a registry key.");

        /// <summary>
        /// Opens the <see cref="RegistryKey"/> that this <see cref="RegistryItemInfo"/> represents using custom <see cref="RegistryKeyPermissionCheck"/> and <see cref="RegistryRights"/>.
        /// </summary>
        /// <param name="registryKeyPermissionCheck">Specifies whether security checks are performed when opening the registry key and accessing its name/value pairs.</param>
        /// <param name="registryRights">Specifies the access control rights that can be applied to the registry objects in registry key's scope.</param>
        public void OpenKey(RegistryKeyPermissionCheck registryKeyPermissionCheck, RegistryRights registryRights) => _registryKey = Registry.OpenRegistryKey(Path, registryKeyPermissionCheck, registryRights);

        /// <summary>
        /// Opens the <see cref="RegistryKey"/> that this <see cref="RegistryItemInfo"/> represents with a <see cref="bool"/> value that indicates whether the registry key has to be opened with write-rights.
        /// </summary>
        /// <param name="writable">A <see cref="bool"/> value that indicates whether the registry key has to be opened with write-rights</param>
        public void OpenKey(bool writable) => _registryKey = Registry.OpenRegistryKey(Path, writable);

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
                        _ = stringBuilder.Append(WinCopies.IO.Path.PathSeparator);

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
            base.DisposeManaged();

            if (_registryKey != null)
            {
                _registryKey.Dispose();

                _registryKey = null;
            }
        }

        private BitmapSource TryGetBitmapSource(in int size)
        {
            int iconIndex = FileIcon;

            switch (ObjectPropertiesGeneric.RegistryItemType)
            {
                case RegistryItemType.Root:

                    iconIndex = ComputerIcon;

                    break;

                case RegistryItemType.Key:

                    iconIndex = FolderIcon;

                    break;
            }

            return TryGetBitmapSource(iconIndex, Shell32, size);
        }

        public override WinCopies.Collections.Generic.IEqualityComparer<IBrowsableObjectInfoBase> GetDefaultEqualityComparer() => new RegistryItemInfoEqualityComparer<IBrowsableObjectInfoBase>();

        public override WinCopies.Collections.Generic.IComparer<IBrowsableObjectInfoBase> GetDefaultComparer() => new RegistryItemInfoComparer<IBrowsableObjectInfoBase>();
        #endregion // Methods
    }

    public class RegistryItemInfo : RegistryItemInfo<IRegistryItemInfoProperties, RegistryItemInfoItemProvider, IBrowsableObjectInfoSelectorDictionary<RegistryItemInfoItemProvider>, RegistryItemInfoItemProvider>, IRegistryItemInfo
    {
        private static System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> _defaultRootItems;
        private IRegistryItemInfoProperties _objectProperties;

        #region Properties
        public static System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> DefaultRootItems => _defaultRootItems
#if CS8
            ??=
#else
            ?? (_defaultRootItems =
#endif
            GetRootItems()
#if !CS8
            )
#endif
            ;

        //public override Predicate<RegistryItemInfoItemProvider> RootItemsPredicate => item => item.RegistryKey != null && item.ValueName == null;

        //public override Predicate<IBrowsableObjectInfo> RootItemsBrowsableObjectInfoPredicate => null;

        public static IBrowsableObjectInfoSelectorDictionary<RegistryItemInfoItemProvider> DefaultItemSelectorDictionary { get; } = new RegistryItemInfoSelectorDictionary();

        public sealed override IRegistryItemInfoProperties ObjectPropertiesGeneric => IsDisposed ? throw GetExceptionForDispose(false) : _objectProperties;

        public override IPropertySystemCollection<PropertyId,ShellPropertyGroup> ObjectPropertySystem => null;
        #endregion // Properties

        #region Constructors
        public RegistryItemInfo() : base() { /* Left empty. */ }

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
        #endregion // Constructors

        #region Methods
        public static System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetRootItems() => new IBrowsableObjectInfo[] { new RegistryItemInfo(GetDefaultClientVersion()) };

        public override IBrowsableObjectInfoSelectorDictionary<RegistryItemInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

        #region GetItems
        protected override System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> GetItemProviders() => GetItemProviders(null);

        //#if NETFRAMEWORK
        //        {
        //            switch (RegistryItemType)
        //            {
        //                case RegistryItemType.Root:

        //                    return typeof(Microsoft.Win32.Registry).GetFields().Select(f => new RegistryItemInfo((RegistryKey)f.GetValue(null)));

        //                case RegistryItemType.Key:

        //                    return GetItems(null, false);

        //                default:

        //                    throw new InvalidOperationException("The current item cannot be browsed.");
        //            }
        //        }
        //#else
        //            => ObjectPropertiesGeneric.RegistryItemType switch
        //            {
        //                RegistryItemType.Root => typeof(Microsoft.Win32.Registry).GetFields().Select(f => new RegistryItemInfo((RegistryKey)f.GetValue(null))),
        //                RegistryItemType.Key => GetItems(null),
        //                _ => throw new InvalidOperationException("The current item cannot be browsed."),
        //            };
        //#endif

        //protected override System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> GetItemProviders(Predicate<RegistryKey> predicate)
        //{
        //    if (ObjectPropertiesGeneric.RegistryItemType == RegistryItemType.Root)

        //        //{

        //        //if (RegistryItemTypes.HasFlag(RegistryItemTypes.RegistryKey))

        //        //{

        //        /*FieldInfo[] _registryKeyFields = */

        //        return typeof(Microsoft.Win32.Registry).GetFields().Select(f => (RegistryKey)f.GetValue(null)).WherePredicate(predicate).Select(item => new RegistryItemInfo(item));

        //    //string name;

        //    //foreach (FieldInfo fieldInfo in _registryKeyFields)

        //    //{  

        //    //checkAndAppend(name, name, false);

        //    //}

        //    //}

        //    //}

        //    else

        //        throw new ArgumentException("The given predicate is not valid for the current RegistryItemInfo.");
        //}

        protected override System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> GetItemProviders(Predicate<RegistryItemInfoItemProvider> predicate)
        {
            System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> enumerable;

            switch (ObjectPropertiesGeneric.RegistryItemType)
            {
                case RegistryItemType.Key:

                    //string[] items;

                    System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> keys = null;
                    System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> values = null;

                    // if (catchExceptions)

                    try
                    {
                        keys = InnerObjectGeneric.GetSubKeyNames().Select(item => new RegistryItemInfoItemProvider(item, ClientVersion));

                        values = _registryKey.GetValueNames().Select(s => new RegistryItemInfoItemProvider(_registryKey, s, ClientVersion) /*new RegistryItemInfo(Path, s)*/);

                        // foreach (string item in items)

                        // item.Substring(0, item.LastIndexOf(IO.Path.PathSeparator)), item.Substring(item.LastIndexOf(IO.Path.PathSeparator) + 1), false
                    }

                    catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException))) { /* Left empty. */ }

                    //else

                    //    enumerate();

                    enumerable = (keys ?? GetEmptyEnumerable()).AppendValues(values ?? GetEmptyEnumerable());

                    break;

                case RegistryItemType.Root:

                    enumerable = typeof(Microsoft.Win32.Registry).GetFields().Select(f => new RegistryItemInfoItemProvider((RegistryKey)f.GetValue(null), ClientVersion));

                    break;

                default:

                    return enumerable = GetEmptyEnumerable();
            }

            return predicate == null ? enumerable : enumerable.WherePredicate(predicate);
        }

        //public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<RegistryItemInfoItemProvider> predicate)
        //{
        //    //protected override void OnDoWork(DoWorkEventArgs e)
        //    //{

        //    //if (RegistryItemTypes == RegistryItemTypes.None)

        //    //    return;

        //    // todo: 'if' to remove if not necessary:

        //    // if (Path is IRegistryItemInfo registryItemInfo)

        //    // {

        //    // var paths = new ArrayBuilder<PathInfo>();

        //    // PathInfo pathInfo;

        //    //void checkAndAppend(string pathWithoutName, string name, bool isValue)

        //    //{

        //    //string path = pathWithoutName + IO.Path.PathSeparator + name;

        //    //if (CheckFilter(path))

        //    //    _ = paths.AddLast(pathInfo = new PathInfo(path, path.RemoveAccents(), name, null, RegistryItemInfo.DefaultRegistryKeyDeepClone, isValue));

        //    //}

        //    if (ObjectPropertiesGeneric.RegistryItemType == RegistryItemType.Key)
        //    {
        //        //string[] items;

        //        System.Collections.Generic.IEnumerable<RegistryItemInfo> keys;

        //        System.Collections.Generic.IEnumerable<RegistryItemInfo> values;

        //        void enumerate()
        //        {
        //            if (predicate == null)
        //            {
        //                keys = InnerObjectGeneric.GetSubKeyNames().Select(item => new RegistryItemInfo($"{Path}\\{item}"));

        //                values = _registryKey.GetValueNames().Select(s => new RegistryItemInfo(Path, s));
        //            }

        //            else
        //            {
        //                keys = InnerObjectGeneric.GetSubKeyNames().Where(item => predicate(new RegistryItemInfoItemProvider(item, RegistryItemType.Key))).Select(item => new RegistryItemInfo($"{Path}\\{item}"));

        //                values = _registryKey.GetValueNames().Where(s => predicate(new RegistryItemInfoItemProvider(s, RegistryItemType.Value))).Select(s => new RegistryItemInfo(Path, s));
        //            }
        //        }

        //        if (catchExceptions)

        //            try
        //            {
        //                enumerate();

        //                // foreach (string item in items)

        //                // item.Substring(0, item.LastIndexOf(IO.Path.PathSeparator)), item.Substring(item.LastIndexOf(IO.Path.PathSeparator) + 1), false
        //            }

        //            catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException))) { keys = null; values = null; }

        //        else

        //            enumerate();

        //        return values == null ? keys : keys == null ? values : keys.AppendValues(values);
        //    }

        //    else

        //        throw new ArgumentException("The current predicate type is not valid with this RegistryItemInfo.");



        //    //System.Collections.Generic.IEnumerable<PathInfo> pathInfos;



        //    //if (FileSystemObjectComparer == null)

        //    //    pathInfos = (System.Collections.Generic.IEnumerable<PathInfo>)paths;

        //    //else

        //    //{

        //    //    var _paths = paths.ToList();

        //    //    _paths.Sort(FileSystemObjectComparer);

        //    //    pathInfos = (System.Collections.Generic.IEnumerable<PathInfo>)_paths;

        //    //}



        //    //using (IEnumerator<PathInfo> pathsEnum = pathInfos.GetEnumerator())



        //    //while (pathsEnum.MoveNext())

        //    //try

        //    //{

        //    //    do

        //    //ReportProgress(0, new BrowsableObjectTreeNode<TItems, TSubItems, TItemsFactory>((TItems)(pathsEnum.Current.IsValue ? ((IRegistryItemInfoFactory)Path.Factory).GetBrowsableObjectInfo(pathsEnum.Current.Path.Substring(0, pathsEnum.Current.Path.Length - pathsEnum.Current.Name.Length - 1 /* We remove one more character to remove the backslash between the registry key path and the registry key value name. */ ), pathsEnum.Current.Name) : Path.Factory.GetBrowsableObjectInfo(pathsEnum.Current.Path)), (TItemsFactory)Path.Factory.DeepClone()));

        //    //    while (pathsEnum.MoveNext());

        //    //}

        //    //catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException))) { }



        //    // }
        //}

        public override IEnumerable<IBrowsableObjectInfo> GetSubRootItems() => GetItems(GetItemProviders(item => item.RegistryKey != null && item.ValueName == null));
        #endregion // GetItems
        #endregion // Methods
    }
}