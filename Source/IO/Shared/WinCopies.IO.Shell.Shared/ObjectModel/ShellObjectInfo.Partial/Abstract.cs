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

#region Usings
#region Namespaces
#region WAPICP
using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.Shell;
#endregion WAPICP

#region System
using System;
using System.Collections.Generic;
using System.IO;
using WinCopies.ActionRunners;
#endregion System

#region WinCopies
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
#endregion WinCopies
#endregion Namespaces

#region Static Usings
using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

#region WinCopies
using static WinCopies.IO.FileType;
using static WinCopies.IO.ObjectModel.ShellObjectInfo;
#endregion WinCopies
#endregion Static Usings

using SystemPath = System.IO.Path;
#endregion

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// Represents a file system item.
    /// </summary>
    public abstract partial class ShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ArchiveItemInfoProvider<TObjectProperties, ShellObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        #region Fields
        private ShellObject _shellObject;
        private IBrowsableObjectInfo
#if CS8
            ?
#endif
            _parent;
        private bool _loadParent = true;
        private IBrowsabilityOptions _browsability;
        private IBitmapSourceProvider _bitmapSourceProvider;
        private ShellObjectInfoMonitoring<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>? _monitoring;
        private GetStreamStruct? _getStreamStruct;
        private readonly string _uri;
        #endregion

        #region Properties
        /// <summary>
        /// The parent <see cref="IShellObjectInfoBase"/> of the current archive item. If the current <see cref="ShellObjectInfo"/> represents an archive file, this property returns the current <see cref="ShellObjectInfo"/>, or <see langword="null"/> otherwise.
        /// </summary>
        public override IShellObjectInfoBase
#if CS8
            ?
#endif
            ArchiveShellObject => ObjectPropertiesGeneric.FileType == Archive ? this : null;

        public bool IsArchiveOpen => _getStreamStruct.HasValue;

        #region Overrides
        protected override bool IsMonitoringSupportedOverride => InnerObjectGenericOverride is ShellFileSystemFolder || InnerObjectGenericOverride is ShellFile || InnerObjectGenericOverride.ParsingName == Computer.ParsingName;

        protected override bool IsLocalRootOverride => ObjectPropertiesGenericOverride.FileType == KnownFolder && InnerObjectGenericOverride.ParsingName == Computer.ParsingName;

        /// <summary>
        /// Gets a <see cref="ShellObject"/> that represents this <see cref="ShellObjectInfo"/>.
        /// </summary>
        protected sealed override ShellObject InnerObjectGenericOverride => _shellObject;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => _bitmapSourceProvider
#if CS8
            ??=
#else
            ?? (_bitmapSourceProvider =
#endif
           FileSystemObjectInfo.GetDefaultBitmapSourcesProvider(this, new ShellObjectBitmapSources(InnerObjectGenericOverride))
#if !CS8
                )
#endif
            ;

        protected override IBrowsabilityOptions BrowsabilityOverride
        {
            get
            {
                IBrowsabilityOptions getBrowsabilityOptions()
                {
                    IBrowsabilityOptions getFromShellLink(in ShellLink _shellLink)
                    {
                        IBrowsabilityOptions getFromShellLinkTargetLocation(in ShellLink __shellLink)
                        {
                            ShellObjectInfo targetShellObjectInfo = From(__shellLink.TargetShellObject, ClientVersion);

                            IBrowsabilityOptions getFromShellLinkTargetObject()
                            {
                                targetShellObjectInfo.Dispose();

                                return BrowsabilityOptions.NotBrowsable;
                            }

                            return targetShellObjectInfo.InnerObjectGeneric is ShellLink ? getFromShellLinkTargetObject() : new ShellLinkBrowsabilityOptions(targetShellObjectInfo);
                        }

                        return IO.Path.Exists(_shellLink.TargetLocation) ? getFromShellLinkTargetLocation(_shellLink) : BrowsabilityOptions.NotBrowsable;
                    }

                    return InnerObjectGeneric is ShellLink shellLink ? getFromShellLink(shellLink) : _shellObject is IEnumerable<ShellObject> ? BrowsabilityOptions.BrowsableByDefault : BrowsabilityOptions.NotBrowsable;
                }

                return _browsability
#if CS8
                    ??=
#else
                    ?? (_browsability =
#endif
                    getBrowsabilityOptions()
#if !CS8
                        )
#endif
                    ;
            }
        }

        protected override string DescriptionOverride => _shellObject.Properties.System.FileDescription.Value;

        /// <summary>
        /// Gets the type name of the current <see cref="ShellObjectInfo"/>. This value corresponds to the description of the file's extension.
        /// </summary>
        protected override string ItemTypeNameOverride => _shellObject.Properties.System.ItemTypeText.Value;

        /// <summary>
        /// Gets a value that indicates whether the current item has particularities.
        /// </summary>
        protected override bool IsSpecialItemOverride => ValueChecks.Run(_shellObject.Properties.System.FileAttributes.Value, value => value.HasFlag(Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes.Hidden) || value.HasFlag(Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes.System));

        /// <summary>
        /// Gets the localized name of this <see cref="ShellObjectInfo"/> depending the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
        /// </summary>
        public override string LocalizedName => _shellObject.GetDisplayName(DisplayNameType.Default) ?? Name;

        /// <summary>
        /// Gets the name of this <see cref="ShellObjectInfo"/> depending of the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
        /// </summary>
        public override string Name => _shellObject.Name;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride { get; }

        protected override IBrowsableObjectInfo
#if CS8
            ?
#endif
            ParentGenericOverride
        {
            get
            {
                if (_loadParent)
                {
                    _parent = GetParent();

                    _loadParent = false;
                }

                return _parent;
            }
        }

        public override string Protocol
#if CS8
            =>
#else
            {
                get
                {
                    if (
#endif
                        ObjectPropertiesGeneric == null
#if CS8
        ?
#else
                    )

                    return
#endif
            "unknown"
#if CS8
            :
#else
                    ;

                    switch (
#endif
            ObjectPropertiesGeneric.FileType
#if CS8
            switch
#else
                    )
#endif
            {
#if !CS8
                        case
#endif
                KnownFolder
#if CS8
                =>
#else
                    :
                            return
#endif
                InnerObjectGeneric.IsFileSystemObject ? "file" : "shell"
#if CS8
                ,
#else
                    ;
                        case
#endif
                Other
#if CS8
                =>
#else
                    :
                            return
#endif
                "unknown"
#if CS8
                ,
                _ =>
#else
                    ;
                        default:
                            return
#endif
                "file"
#if CS8
            };
#else
                        ;
                    }
                }
            }
#endif

        public override string URI => _uri;
        #endregion
        #endregion

        ///// <summary>
        ///// Initializes a new instance of the <see cref="ShellObjectInfo"/> class with a given <see cref="FileType"/> and <see cref="SpecialFolder"/> using custom factories for <see cref="ShellObjectInfo"/>s and <see cref="ArchiveItemInfo"/>s.
        ///// </summary>
        ///// <param name="path">The path of this <see cref="ShellObjectInfo"/>.</param>
        ///// <param name="fileType">The file type of this <see cref="ShellObjectInfo"/>.</param>
        ///// <param name="specialFolder">The special folder type of this <see cref="ShellObjectInfo"/>. <see cref="WinCopies.IO.SpecialFolder.None"/> if this <see cref="ShellObjectInfo"/> is a casual file system item.</param>
        ///// <param name="shellObject">The <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/> that this <see cref="ShellObjectInfo"/> represents.</param>
        protected ShellObjectInfo(in BrowsableObjectInfoURL url, in ShellObject shellObject, in ClientVersion clientVersion) : base(url.Path, clientVersion)
        {
            _shellObject = shellObject;
            _uri = url.URI;
            ObjectPropertySystemOverride = ShellObjectPropertySystemCollection._GetShellObjectPropertySystemCollection(this);
        }

        #region Methods
        protected override IContextMenu
#if CS8
                ?
#endif
                GetContextMenuOverride(bool extendedVerbs) => ContextMenu.GetNewContextMenu(this, extendedVerbs);

        protected override IContextMenu
#if CS8
                ?
#endif
                GetContextMenuOverride(IEnumerable<IBrowsableObjectInfo> items, bool extendedVerbs) => ContextMenu.GetNewContextMenu(this, items, extendedVerbs);

        internal new void RaiseCallbacks(in BrowsableObjectInfoCallbackArgs a) => base.RaiseCallbacks(a);

        /// <summary>
        /// Returns the parent of this <see cref="BrowsableObjectInfo"/>.
        /// </summary>
        /// <returns>The parent of this <see cref="BrowsableObjectInfo"/>.</returns>
        private IBrowsableObjectInfo
#if CS8
            ?
#endif
            GetParent()
        {
            IBrowsableObjectInfo getDefault() => UtilHelpers.PerformActionIfNotNull(SystemPath.GetDirectoryName(_shellObject.ParsingName), parsingName => From(parsingName, ClientVersion));

            IBrowsableObjectInfo getComputer() => new ShellObjectInfo(Computer.Path, KnownFolder, ShellObjectFactory.Create(Computer.ParsingName), ClientVersion);
#if CS8
            return
#else
                switch (
#endif
            ObjectPropertiesGeneric.FileType
#if CS8
                switch
#else
                    )
#endif
            {
#if !CS8
                    case
#endif
                Other
#if CS8
                            =>
#else
                                :
                        return
#endif
                                null
#if CS8
                                ,
#else
                        ;
                    case
#endif
                KnownFolder
#if CS8
                            =>
#else
                                :
                        return
#endif
                                _shellObject.IsFileSystemObject ? getDefault() : _shellObject.ParsingName == Computer.ParsingName ? null : getComputer()
#if CS8
                                ,
#else
                                    ;
                    case
#endif
                Drive
#if CS8
                    =>
#else
                        :
                        return
#endif
                    getComputer()
#if CS8
                                ,
                _ =>
#else
                            ;
                    default:
                        return
#endif
                            getDefault()
#if CS8
            };
#else
                        ;
                }
#endif
        }

        #region Archive
        public StreamInfo
#if CS8
            ?
#endif
            GetArchiveFileStream()
        {
            ThrowArchiveOpenStatusException(true);

            return _getStreamStruct?.ArchiveFileStream;
        }

        protected void ThrowItemIsNotAnArchiveException() => throw new InvalidOperationException("The current item is not an archive.");

        protected void ThrowArchiveOpenStatusException(in bool mustBeOpen)
        {
            string
#if CS8
            ?
#endif
            msg = null;

            if (mustBeOpen)
            {
                if (!IsArchiveOpen)

                    msg = "closed";
            }

            else if (IsArchiveOpen)

                msg = "already open";

            if (msg != null)

                throw new InvalidOperationException($"The archive is {msg}.");
        }

        public void OpenArchive(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int? bufferSize, FileOptions fileOptions)
        {
            if (ObjectPropertiesGeneric.FileType == Archive)
            {
                ThrowArchiveOpenStatusException(false);

                _getStreamStruct = new GetStreamStruct(() => new FileStream(Path, fileMode, fileAccess, fileShare, bufferSize ?? 4096, fileOptions));
            }

            else ThrowItemIsNotAnArchiveException();
        }

        public void CloseArchive()
        {
            if (ObjectPropertiesGeneric.FileType == Archive)
            {
                ThrowArchiveOpenStatusException(false);

                _getStreamStruct.Value.Dispose();
                _getStreamStruct = null;
            }

            else ThrowItemIsNotAnArchiveException();
        }
        #endregion

        #region Overrides
        protected override IEnumerable<Util.Commands.Primitives.ICommand>
#if CS8
            ?
#endif
            GetCommandsOverride(IEnumerable<IBrowsableObjectInfo> items) => null;
        /*{
            if (InnerObject is ShellFolder shellFolder)
            {
                IEnumerableQueue<ShellObject> shellObjects = new EnumerableQueue<ShellObject>();

                foreach (IBrowsableObjectInfo _item in items)
                {
                    if (_item.InnerObject is ShellObject shellObject)
                    {
                        if (((IUIntCountable)shellObjects).Count == 0u)

                            shellObjects.Enqueue(shellObject);

                        else
                        {
                            shellObjects.Clear();

                            return null;
                        }

                        if (!Equals(_item.Parent, this))

                            return null;
                    }

                    else

                        return null;
                }

                IntPtr menu = Menus.CreatePopupMenu();

                Guid riid = new Guid(Microsoft.WindowsAPICodePack.NativeAPI.Guids.Shell.IContextMenu);
                IntPtr ptr = shellFolder.GetUIObjectOf(IntPtr.Zero, ShellContainer.GetPIDLs(shellObjects), ref riid);
                IContextMenu contextMenu = CoreHelpers.GetTypedObjectForIUnknown<IContextMenu>(in ptr);

                HResult hresult = contextMenu.QueryContextMenu(menu, 0u, 1u, uint.MaxValue, ContextMenuFlags.Explore | ContextMenuFlags.CanRename);

                IContextMenu3 contextMenu3;

                contextMenu3.HandleMenuMsg2((uint)WindowMessage.MenuChar, wParam, IntPtr.Zero) == HResult.Ok)
                {
                    handled = true;
                }

                var menuItem = Temp.NativeMenuItem<Command<uint>, System.Collections.Generic.IEnumerable<Command<uint>>>.Create(new Temp.NativeShellMenu(ptr));
            }
        }*/

        protected override void StartMonitoringOverride()
        {
            var monitoring = new ShellObjectInfoMonitoring<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>(this);

            monitoring.Start();

            _monitoring = monitoring;
        }

        protected override void StopMonitoringOverride()
        {
            if (_monitoring.HasValue)
            {
                _monitoring.Value.Stop();
                _monitoring = null;
            }
        }

        protected override void DisposeUnmanaged()
        {
            _parent = null;

            if (IsArchiveOpen)

                CloseArchive();

            if (_bitmapSourceProvider != null)
            {
                _bitmapSourceProvider.Dispose();
                _bitmapSourceProvider = null;
            }

            base.DisposeUnmanaged();
        }
        protected override void DisposeManaged()
        {
            _shellObject.Dispose();
            _shellObject = null;

            _browsability = null;

            base.DisposeManaged();
        }

        /// <summary>
        /// Gets a string representation of this <see cref="ShellObjectInfo"/>.
        /// </summary>
        /// <returns>The <see cref="LocalizedName"/> of this <see cref="ShellObjectInfo"/>.</returns>
        public override string ToString() => string.IsNullOrEmpty(Path) ? _shellObject.GetDisplayName(DisplayNameType.Default) : SystemPath.GetFileName(Path);
        #endregion
        #endregion
    }
}
