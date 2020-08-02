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

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.Util.Util;

namespace WinCopies.IO.ObjectModel
{
    public struct RegistryItemInfoEnumeratorStruct
    {
        public string Value { get; }

        public RegistryItemType RegistryItemType { get; }

        public RegistryItemInfoEnumeratorStruct(string value, RegistryItemType registryItemType)
        {
            Value = value;

            RegistryItemType = registryItemType;
        }
    }

    /// <summary>
    /// The Windows registry item type.
    /// </summary>
    public enum RegistryItemType
    {
        /// <summary>
        /// The current instance represents the Windows registry root node.
        /// </summary>
        Root,

        /// <summary>
        /// The current instance represents a Windows registry key.
        /// </summary>
        Key,

        /// <summary>
        /// The current instance represents a Windows registry value.
        /// </summary>
        Value
    }

    /// <summary>
    /// Represents a Windows registry item.
    /// </summary>
    public class RegistryItemInfo/*<TItems, TFactory>*/ : BrowsableObjectInfo/*<TItems, TFactory>*/, IRegistryItemInfo // where TItems : BrowsableObjectInfo, IRegistryItemInfo where TFactory : IRegistryItemInfoFactory
    {
        // public override bool IsRenamingSupported => false;

        #region Fields

        private const int FileIcon = 0;
        private const int ComputerIcon = 15;
        private const int FolderIcon = 3;

        private RegistryKey _registryKey;

        private IBrowsableObjectInfo _parent;

        #endregion

        #region Properties

        ///// <summary>
        ///// Gets a value that indicates whether this object needs to reconstruct objects on deep cloning.
        ///// </summary>
        //public override bool NeedsObjectsOrValuesReconstruction => _registryKey is object; // If _registryKey is null, reconstructing registry does not make sense, so we return false.

        //public static DeepClone<RegistryKey> DefaultRegistryKeyDeepClone { get; } = registryKey => Registry.OpenRegistryKey(registryKey.Name);

        public override bool IsSpecialItem => false;

        public override Size? Size => null;

        private string _itemTypeName;

        public override string ItemTypeName
        {

            get
            {

                if (string.IsNullOrEmpty(_itemTypeName))

                    switch (RegistryItemType)
                    {
                        case RegistryItemType.Root:
                            _itemTypeName = "Registry root";
                            break;
                        case RegistryItemType.Key:
                            _itemTypeName = "Registry key";
                            break;
                        case RegistryItemType.Value:
                            _itemTypeName = "Registry value";
                            break;
                    }

                return _itemTypeName;

            }

        }

        public override string Description => "N/A";

        /// <summary>
        /// The Windows registry item type of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public RegistryItemType RegistryItemType { get; }

        /// <summary>
        /// The <see cref="Microsoft.Win32.RegistryKey"/> that this <see cref="RegistryItemInfo"/> represents.
        /// </summary>
        public RegistryKey RegistryKey
        {
            get
            {
                if (_registryKey == null && RegistryItemType == RegistryItemType.Key)

                    OpenKey();

                return _registryKey;
            }
        }

        /// <summary>
        /// Gets the localized path of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override string LocalizedName => Name;

        /// <summary>
        /// Gets the name of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        public override string Name { get; }

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

        /// <summary>
        /// Gets a value that indicates whether this <see cref="RegistryItemInfo"/> is browsable.
        /// </summary>
        public override bool IsBrowsable => RegistryItemType == RegistryItemType.Root || RegistryItemType == RegistryItemType.Key;

#if NETCORE

        public override IBrowsableObjectInfo Parent => _parent ??= GetParent();

#else

        public override IBrowsableObjectInfo Parent => _parent ?? (_parent = GetParent());

#endif

        #endregion

        public override FileSystemType ItemFileSystemType => FileSystemType.Registry;

        #region Constructors

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        public RegistryItemInfo() : base(ShellObject.FromParsingName(KnownFolders.Computer.ParsingName).GetDisplayName(DisplayNameType.Default))
        {

            Name = Path;

            RegistryItemType = RegistryItemType.Root;

        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKey">The <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(RegistryKey registryKey) : base((registryKey ?? throw GetArgumentNullException(nameof(registryKey))).Name)
        {

            string[] name = registryKey.Name.Split(WinCopies.IO.Path.PathSeparator);

            Name =

#if NETFRAMEWORK

                name[name.Length-1];

#else

                name[^1];

#endif

            RegistryItemType = RegistryItemType.Key;

            _registryKey = registryKey;

        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="path">The path of the <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(string path) : base(path)
        {

            ThrowIfNullEmptyOrWhiteSpace(path);

            string[] name = path.Split(WinCopies.IO.Path.PathSeparator);

            Name =

#if NETFRAMEWORK
                
                name[name.Length-1];
                
#else

                name[^1];

#endif

            RegistryItemType = RegistryItemType.Key;

            _registryKey = Registry.OpenRegistryKey(path);

        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKey">The <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        ///// <param name="valueName">The name of the value that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(RegistryKey registryKey, string valueName) : base(registryKey.Name)
        {

            Name = valueName;

            RegistryItemType = RegistryItemType.Value;

            _registryKey = registryKey;

        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKeyPath">The path of the <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        ///// <param name="valueName">The name of the value that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(string registryKeyPath, string valueName) : this(Registry.OpenRegistryKey(registryKeyPath), valueName) { }

        #endregion

        #region Public methods

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
        public void OpenKey()
        {

            if (RegistryItemType != RegistryItemType.Key)

                throw new InvalidOperationException("This item does not represent a registry key.");

            _registryKey = Registry.OpenRegistryKey(Path);

        }

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

        #endregion

        #region Protected methods

        /// <summary>
        /// Returns the parent of this <see cref="RegistryItemInfo"/>.
        /// </summary>
        /// <returns>The parent of this <see cref="RegistryItemInfo"/>.</returns>
        private IBrowsableObjectInfo GetParent()
        {

            switch (RegistryItemType)

            {

                case RegistryItemType.Key:

                    string[] path = RegistryKey.Name.Split(WinCopies.IO.Path.PathSeparator);

                    if (path.Length == 1)

                        return new RegistryItemInfo();

                    var stringBuilder = new StringBuilder();

                    for (int i = 0; i < path.Length - 1; i++)

                        _ = stringBuilder.Append(path);

                    return new RegistryItemInfo(stringBuilder.ToString());

                case RegistryItemType.Value:

                    return new RegistryItemInfo(RegistryKey);

                default:

                    return null;

            }

        }

        ///// <summary>
        ///// Disposes the current <see cref="RegistryItemInfo"/> and its parent and items recursively.
        ///// </summary>
        ///// <exception cref="InvalidOperationException">The <see cref="BrowsableObjectInfo.ItemsLoader"/> is busy and does not support cancellation.</exception>
        protected override void Dispose(in bool disposing)
        {
            base.Dispose(disposing);

            _registryKey?.Dispose();

            if (disposing)

            {

                _registryKey = null;

            }
        }

        #endregion

        private BitmapSource TryGetBitmapSource(int size)
        {
            int iconIndex = FileIcon;

            switch (RegistryItemType)
            {
                case RegistryItemType.Root:

                    iconIndex = ComputerIcon;

                    break;

                case RegistryItemType.Key:

                    iconIndex = FolderIcon;

                    break;
            }

#if NETFRAMEWORK

            using (Icon icon = TryGetIcon(iconIndex, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, new System.Drawing.Size(size, size)))

#else

            using Icon icon = TryGetIcon(iconIndex, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, new System.Drawing.Size(size, size));

#endif

            return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public override IEnumerable<IBrowsableObjectInfo> GetItems()
#if NETFRAMEWORK
        {
            switch (RegistryItemType)
            {
                case RegistryItemType.Root:

                    return typeof(Microsoft.Win32.Registry).GetFields().Select(f => new RegistryItemInfo((RegistryKey)f.GetValue(null)));

                case RegistryItemType.Key:

                    return GetItems(null, false);

                default:

                    throw new InvalidOperationException("The current item cannot be browsed.");
            }
        }
#else
            => RegistryItemType switch
            {
                RegistryItemType.Root => typeof(Microsoft.Win32.Registry).GetFields().Select(f => new RegistryItemInfo((RegistryKey)f.GetValue(null))),
                RegistryItemType.Key => GetItems(null, false),
                _ => throw new InvalidOperationException("The current item cannot be browsed."),
            };
#endif

        public IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<RegistryKey> predicate)
        {
            if (RegistryItemType == RegistryItemType.Root)

                //{

                //if (RegistryItemTypes.HasFlag(RegistryItemTypes.RegistryKey))

                //{

                /*FieldInfo[] _registryKeyFields = */
                return typeof(Microsoft.Win32.Registry).GetFields().Select(f => (RegistryKey)f.GetValue(null)).WherePredicate(predicate).Select(item => new RegistryItemInfo(item));

            //string name;

            //foreach (FieldInfo fieldInfo in _registryKeyFields)

            //{  

            //checkAndAppend(name, name, false);

            //}

            //}

            //}

            else

                throw new ArgumentException("The given predicate is not valid for the current RegistryItemInfo.");
        }

        public IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<RegistryItemInfoEnumeratorStruct> predicate, bool catchExceptions)
        {
            //protected override void OnDoWork(DoWorkEventArgs e)
            //{

            //if (RegistryItemTypes == RegistryItemTypes.None)

            //    return;

            // todo: 'if' to remove if not necessary:

            // if (Path is IRegistryItemInfo registryItemInfo)

            // {

            // var paths = new ArrayBuilder<PathInfo>();

            // PathInfo pathInfo;

            //void checkAndAppend(string pathWithoutName, string name, bool isValue)

            //{

            //string path = pathWithoutName + IO.Path.PathSeparator + name;

            //if (CheckFilter(path))

            //    _ = paths.AddLast(pathInfo = new PathInfo(path, path.RemoveAccents(), name, null, RegistryItemInfo.DefaultRegistryKeyDeepClone, isValue));

            //}

            if (RegistryItemType == RegistryItemType.Key)
            {
                //string[] items;

                IEnumerable<RegistryItemInfo> keys;

                IEnumerable<RegistryItemInfo> values;

                void enumerate()
                {
                    if (predicate == null)
                    {
                        keys = RegistryKey.GetSubKeyNames().Select(item => new RegistryItemInfo($"{Path}\\{item}"));

                        values = RegistryKey.GetValueNames().Select(s => new RegistryItemInfo(Path, s));
                    }

                    else
                    {
                        keys = RegistryKey.GetSubKeyNames().Where(item => predicate(new RegistryItemInfoEnumeratorStruct(item, RegistryItemType.Key))).Select(item => new RegistryItemInfo($"{Path}\\{item}"));

                        values = RegistryKey.GetValueNames().Where(s => predicate(new RegistryItemInfoEnumeratorStruct(s, RegistryItemType.Value))).Select(s => new RegistryItemInfo(Path, s));
                    }
                }

                if (catchExceptions)

                    try
                    {
                        enumerate();

                        // foreach (string item in items)

                        // item.Substring(0, item.LastIndexOf(IO.Path.PathSeparator)), item.Substring(item.LastIndexOf(IO.Path.PathSeparator) + 1), false
                    }

                    catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException))) { keys = null; values = null; }

                else

                    enumerate();

                return values == null ? keys : keys == null ? values : keys.AppendValues(values);
            }

            else

                throw new ArgumentException("The current predicate type is not valid with this RegistryItemInfo.");



            //IEnumerable<PathInfo> pathInfos;



            //if (FileSystemObjectComparer == null)

            //    pathInfos = (IEnumerable<PathInfo>)paths;

            //else

            //{

            //    var _paths = paths.ToList();

            //    _paths.Sort(FileSystemObjectComparer);

            //    pathInfos = (IEnumerable<PathInfo>)_paths;

            //}



            //using (IEnumerator<PathInfo> pathsEnum = pathInfos.GetEnumerator())



            //while (pathsEnum.MoveNext())

            //try

            //{

            //    do

            //ReportProgress(0, new BrowsableObjectTreeNode<TItems, TSubItems, TItemsFactory>((TItems)(pathsEnum.Current.IsValue ? ((IRegistryItemInfoFactory)Path.Factory).GetBrowsableObjectInfo(pathsEnum.Current.Path.Substring(0, pathsEnum.Current.Path.Length - pathsEnum.Current.Name.Length - 1 /* We remove one more character to remove the backslash between the registry key path and the registry key value name. */ ), pathsEnum.Current.Name) : Path.Factory.GetBrowsableObjectInfo(pathsEnum.Current.Path)), (TItemsFactory)Path.Factory.DeepClone()));

            //    while (pathsEnum.MoveNext());

            //}

            //catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException))) { }



            // }
        }

        public override Collections.IEqualityComparer<IFileSystemObject> GetDefaultEqualityComparer() => new RegistryItemInfoEqualityComparer<IFileSystemObject>();

        public override IComparer<IFileSystemObject> GetDefaultComparer() => new RegistryItemInfoComparer<IFileSystemObject>();
    }
}
