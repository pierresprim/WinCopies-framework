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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using Microsoft.WindowsAPICodePack.COMNative.PortableDevices.PropertySystem;
using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

using WinCopies.Collections;
using WinCopies.Util;

using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.IO.Path;
using static WinCopies.Util.Util;

using IfComp = WinCopies.Util.Util.Comparison;
using IfCM = WinCopies.Util.Util.ComparisonMode;
using IfCT = WinCopies.Util.Util.ComparisonType;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// Represents a file system item.
    /// </summary>
    public class ShellObjectInfo : ArchiveItemInfoProvider, IShellObjectInfo
    {
        private IBrowsableObjectInfo _parent;

        #region Properties
        /// <summary>
        /// Gets a <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/> that represents this <see cref="ShellObjectInfo"/>.
        /// </summary>
        public ShellObject ShellObject { get; private set; } = null;

        public Stream ArchiveFileStream { get; private set; }

        public bool IsArchiveOpen => ArchiveFileStream is object;

        #region Overrides
        public override FileSystemType ItemFileSystemType => FileSystemType.CurrentDeviceFileSystem;

        /// <summary>
        /// Gets a value that indicates whether this <see cref="ShellObjectInfo"/> is browsable.
        /// </summary>
        public override bool IsBrowsable => ShellObject is IEnumerable<ShellObject>;

#if NETFRAMEWORK

        public override IBrowsableObjectInfo Parent => _parent ?? (_parent = GetParent());

#else

        public override IBrowsableObjectInfo Parent => _parent ??= GetParent();

#endif

        /// <summary>
        /// Gets the localized name of this <see cref="ShellObjectInfo"/> depending the associated <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
        /// </summary>
        public override string LocalizedName => ShellObject.GetDisplayName(DisplayNameType.Default);

        /// <summary>
        /// Gets the name of this <see cref="ShellObjectInfo"/> depending of the associated <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
        /// </summary>
        public override string Name => ShellObject.Name;

        #region BitmapSources
        /// <summary>
        /// Gets the small <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
        /// </summary>
        public override BitmapSource SmallBitmapSource => ShellObject.Thumbnail.SmallBitmapSource;

        /// <summary>
        /// Gets the medium <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
        /// </summary>
        public override BitmapSource MediumBitmapSource => ShellObject.Thumbnail.MediumBitmapSource;

        /// <summary>
        /// Gets the large <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
        /// </summary>
        public override BitmapSource LargeBitmapSource => ShellObject.Thumbnail.LargeBitmapSource;

        /// <summary>
        /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="ShellObjectInfo"/>.
        /// </summary>
        public override BitmapSource ExtraLargeBitmapSource => ShellObject.Thumbnail.ExtraLargeBitmapSource;
        #endregion

        /// <summary>
        /// Gets the type name of the current <see cref="ShellObjectInfo"/>. This value corresponds to the description of the file's extension.
        /// </summary>
        public override string ItemTypeName => ShellObject.Properties.System.ItemTypeText.Value;

        public override string Description => ShellObject.Properties.System.FileDescription.Value;

        public override Size? Size
        {
            get
            {
                ulong? value = ShellObject.Properties.System.Size.Value;

                if (value.HasValue)

                    return new Size(value.Value);

                else

                    return null;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the current item has particularities.
        /// </summary>
        public override bool IsSpecialItem
        {
            get
            {
                uint? value = ShellObject.Properties.System.FileAttributes.Value;

                if (value.HasValue)
                {
                    var _value = (Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes)value.Value;

                    return _value.HasFlag(Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes.Hidden) || _value.HasFlag(Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes.System);
                }

                else

                    return false;
            }
        }

        /// <summary>
        /// The parent <see cref="IShellObjectInfo"/> of the current archive item. If the current <see cref="ShellObjectInfo"/> represents an archive file, this property returns the current <see cref="ShellObjectInfo"/>, or <see langword="null"/> otherwise.
        /// </summary>
        public override IShellObjectInfo ArchiveShellObject => FileType == FileType.Archive ? this : null;
        #endregion
        #endregion

        ///// <summary>
        ///// Initializes a new instance of the <see cref="ShellObjectInfo"/> class with a given <see cref="FileType"/> and <see cref="SpecialFolder"/> using custom factories for <see cref="ShellObjectInfo"/>s and <see cref="ArchiveItemInfo"/>s.
        ///// </summary>
        ///// <param name="path">The path of this <see cref="ShellObjectInfo"/>.</param>
        ///// <param name="fileType">The file type of this <see cref="ShellObjectInfo"/>.</param>
        ///// <param name="specialFolder">The special folder type of this <see cref="ShellObjectInfo"/>. <see cref="WinCopies.IO.SpecialFolder.None"/> if this <see cref="ShellObjectInfo"/> is a casual file system item.</param>
        ///// <param name="shellObject">The <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/> that this <see cref="ShellObjectInfo"/> represents.</param>
        private ShellObjectInfo(in string path, in FileType fileType, in ShellObject shellObject, in ClientVersion clientVersion) : base(path, fileType, clientVersion) => ShellObject = shellObject;

        #region Methods
        public static ShellObjectInfo From(in ShellObject shellObject, in ClientVersion clientVersion)
        {
            if ((shellObject ?? throw GetArgumentNullException(nameof(shellObject))) is ShellFolder shellFolder)
            {
                if (shellObject is ShellFileSystemFolder shellFileSystemFolder)
                {
                    (string path, FileType fileType) = shellFileSystemFolder is FileSystemKnownFolder ? (shellObject.ParsingName, FileType.KnownFolder) : (shellFileSystemFolder.Path, FileType.Folder);

                    return new ShellObjectInfo(path, System.IO.Directory.GetParent(path) is null ? FileType.Drive : fileType, shellObject, clientVersion);
                }

                switch (shellObject)
                {
                    case NonFileSystemKnownFolder nonFileSystemKnownFolder:

                        return new ShellObjectInfo(nonFileSystemKnownFolder.Path, FileType.KnownFolder, shellObject, clientVersion);

                    case ShellNonFileSystemFolder _:

                        return new ShellObjectInfo(shellObject.ParsingName, FileType.Folder, shellObject, clientVersion);
                }
            }

            if (shellObject is ShellLink shellLink)

                return new ShellObjectInfo(shellLink.Path, FileType.Link, shellObject, clientVersion);

            if (shellObject is ShellFile shellFile)

                return new ShellObjectInfo(shellFile.Path, IsSupportedArchiveFormat(System.IO.Path.GetExtension(shellFile.Path)) ? FileType.Archive : shellFile.IsLink ? FileType.Link : System.IO.Path.GetExtension(shellFile.Path) == ".library-ms" ? FileType.Library : FileType.File, shellObject, clientVersion);

            throw new ArgumentException($"The given {nameof(Microsoft.WindowsAPICodePack.Shell.ShellObject)} is not supported.");
        }

        #region Archive
        public void OpenArchive(Stream stream)
        {
            CloseArchive();

            FileType.ThrowIfInvalidEnumValue(true, FileType.Archive);

            ArchiveFileStream = stream;
        }

        public void CloseArchive()
        {
            ArchiveFileStream.Flush();

            ArchiveFileStream.Dispose();

            ArchiveFileStream = null;
        }
        #endregion

#if DEBUG

        static int count = 0;

#endif

        #region GetItems
        private bool PortableDevicePredicate(in IPortableDevice portableDevice, in Predicate<ShellObjectInfoEnumeratorStruct> func)
        {
            Debug.Assert(portableDevice != null);
            Debug.Assert(func != null);

            try
            {
                portableDevice.Open(ClientVersion.Value, new PortableDeviceOpeningOptions(Microsoft.WindowsAPICodePack.Win32Native.GenericRights.Read, Microsoft.WindowsAPICodePack.Win32Native.FileShareOptions.Read, false));

                //#if DEBUG 

                //                Queue<PropertyKey> queue = new Queue<PropertyKey>();

                //                foreach (var key in portableDevice.Properties.Keys)

                //                    queue.Enqueue(key);

                //                string id=portableDevice.DeviceId;

                //                if (count++>0&&portableDevice.Properties.TryGetValue(Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Category, out Property _value))
                //                {
                //                    object ____value = _value.GetValue(out _);

                //                    string guid = ((Guid)____value).ToString();

                //                    string guid1 = Microsoft.WindowsAPICodePack.PortableDevices.Guids.PropertySystem.FunctionalCategory.Device;

                //                    bool bool1 = guid.ToLower() == guid1.ToLower();

                //                    string guid2 = Microsoft.WindowsAPICodePack.PortableDevices.Guids.PropertySystem.FunctionalCategory.Storage;

                //                    bool bool2 = guid.ToLower() == guid2.ToLower();
                //                }

                //                bool value1 = portableDevice.Properties.TryGetValue(Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Device.Type, out _value);

                //                object ___value = _value.GetValue(out _);

                //                bool value2 = (DeviceTypeValues)___value == DeviceTypeValues.Generic;

                //                bool __value = !(value1 && value2);

                //#endif

                return !(portableDevice.Properties.TryGetValue(Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Device.Type, out Property value) && (DeviceTypeValues)value.GetValue(out Type _) == DeviceTypeValues.Generic) && func(new ShellObjectInfoEnumeratorStruct(portableDevice));
            }

            catch (Exception)
            {
                return true;
            }
        }

        public virtual IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ShellObjectInfoEnumeratorStruct> func)
        {
            ThrowIfNull(func, nameof(func));

            if (IsBrowsable)

                switch (FileType)
                {
                    case FileType.Archive:

                        return GetItems(item => func(new ShellObjectInfoEnumeratorStruct(item)));

                    default:

                        IEnumerable<ShellObject> shellObjects = null;

                        IEnumerable<IPortableDevice> portableDevices = null;

                        IEnumerable<IBrowsableObjectInfo> getShellObjects() => shellObjects.Where(item =>
                          {
                              if (portableDevices != null)

                                  foreach (IPortableDevice portableDevice in portableDevices)

                                      if (item.ParsingName.EndsWith(portableDevice.DeviceId, StringComparison.InvariantCultureIgnoreCase))

                                          return false;

                              return func(new ShellObjectInfoEnumeratorStruct(item));
                          }).Select(shellObject => From(shellObject, ClientVersion.Value));

                        IEnumerable<IBrowsableObjectInfo> getPortableDevices() => portableDevices.Where(item =>
                          {
                              if (shellObjects != null)
                              {
                                  foreach (ShellObject shellObject in shellObjects)

                                      if (shellObject.ParsingName.EndsWith(item.DeviceId, StringComparison.InvariantCultureIgnoreCase))

                                          return _where(item);

                                  return false;
                              }

                              bool _where(IPortableDevice _item) => PortableDevicePredicate(_item, func);

                              return _where(item);
                          }).Select(portableDevice => new PortableDeviceInfo(portableDevice, ClientVersion.Value));

                        shellObjects = (IEnumerable<ShellObject>)ShellObject;

                        if (ShellObject.ParsingName == Computer.ParsingName)
                        {
                            var portableDeviceManager = new PortableDeviceManager();

                            portableDeviceManager.GetDevices();

                            portableDevices = portableDeviceManager.PortableDevices;

                            if (shellObjects == null) return getPortableDevices();

                            else if (portableDevices == null) return getShellObjects();

                            return getShellObjects().AppendValues(getPortableDevices());
                        }

                        else return getShellObjects();
                }

            else return null;
        }

        public virtual IEnumerable<IBrowsableObjectInfo> GetItems(in Predicate<ArchiveFileInfoEnumeratorStruct> func)
        {
            ThrowIfNull(func, nameof(func));

#if NETFRAMEWORK
            switch (FileType)
            {
                case FileType.Archive:
                    return GetArchiveItemInfoItems(func);
                default:
                    return null;
            }
#else
            return FileType switch
            {
                FileType.Archive => GetArchiveItemInfoItems(func),
                _ => null,
            };
#endif
        }

        public override IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems((Predicate<ShellObjectInfoEnumeratorStruct>)(obj => true));

        private IEnumerable<IBrowsableObjectInfo> GetArchiveItemInfoItems(Predicate<ArchiveFileInfoEnumeratorStruct> func) => new Enumerable<IBrowsableObjectInfo>(() => new ArchiveItemInfoEnumerator(this, func));
        #endregion

        #region Overrides
        protected override void Dispose(in bool disposing)
        {
            base.Dispose(disposing);

            ShellObject.Dispose();

            if (IsArchiveOpen)

                CloseArchive();

            if (disposing)

                ShellObject = null;
        }

        /// <summary>
        /// Gets a string representation of this <see cref="ShellObjectInfo"/>.
        /// </summary>
        /// <returns>The <see cref="LocalizedName"/> of this <see cref="ShellObjectInfo"/>.</returns>
        public override string ToString() => string.IsNullOrEmpty(Path) ? ShellObject.GetDisplayName(DisplayNameType.Default) : System.IO.Path.GetFileName(Path);
        #endregion

        /// <summary>
        /// Returns the parent of this <see cref="ShellObjectInfo"/>.
        /// </summary>
        /// <returns>The parent of this <see cref="ShellObjectInfo"/>.</returns>
        private IBrowsableObjectInfo GetParent()
        {
            if (If(IfCT.Or, IfCM.Logical, IfComp.Equal, FileType, FileType.Folder, FileType.Archive) || (FileType == FileType.KnownFolder && ShellObject.IsFileSystemObject) || (FileType == FileType.KnownFolder && ((IKnownFolder)ShellObject).FolderId.ToString() != Microsoft.WindowsAPICodePack.Shell.Guids.KnownFolders.Computer))

                return From(ShellObject.Parent, ClientVersion.Value);

            else if (FileType == FileType.Drive)

                return new ShellObjectInfo(Computer.Path, FileType.KnownFolder, ShellObject.FromParsingName(Computer.ParsingName), ClientVersion.Value);

            else return null;
        }
        #endregion
    }
}
