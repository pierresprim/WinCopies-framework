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
using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native.Menus;
#endregion WAPICP

#region System
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
#endregion System

#region WinCopies
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Enumeration;
using WinCopies.IO.ObjectModel.Reflection;
using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Shell.Process;
using WinCopies.Linq;
using WinCopies.PropertySystem;
#endregion WinCopies
#endregion Namespaces

#region Static Usings
using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

#region WinCopies
using static WinCopies.IO.FileType;
using static WinCopies.IO.Consts.Guids.Shell.Process.Shell;
using static WinCopies.IO.ObjectModel.ShellObjectInfo;
using static WinCopies.IO.Shell.Path;
using static WinCopies.IO.Shell.Resources.ExceptionMessages;
using static WinCopies.ThrowHelper;
#endregion
#endregion Static Usings

using SystemPath = System.IO.Path;
#endregion

namespace WinCopies.IO
{
    namespace ObjectModel
    {
        /// <summary>
        /// Represents a file system item.
        /// </summary>
        public abstract class ShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ArchiveItemInfoProvider<TObjectProperties, ShellObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            #region Subtypes
            private struct GetStreamStruct : DotNetFix.IDisposable
            {
                private Func<StreamInfo> _getStreamDelegate;
                private StreamInfo _archiveFileStream;

                public StreamInfo ArchiveFileStream => _archiveFileStream.IsDisposed ? (_archiveFileStream = _getStreamDelegate()) : _archiveFileStream;

                public bool IsDisposed
                {
                    get
                    {
                        if (_getStreamDelegate == null)

                            return true;

                        else if (_archiveFileStream.IsDisposed)
                        {
                            _archiveFileStream = null;
                            _getStreamDelegate = null;

                            return true;
                        }

                        return false;
                    }
                }

                public GetStreamStruct(Func<FileStream> func) => _archiveFileStream = (_getStreamDelegate = () => new StreamInfo(func()))();

                public void Dispose()
                {
                    if (!IsDisposed)
                    {
                        if (!_archiveFileStream.IsDisposed)

                            _archiveFileStream.Dispose();

                        _archiveFileStream = null;

                        _getStreamDelegate = null;
                    }
                }
            }

            private class ContextMenu : IContextMenu
            {
                private const uint FIRST = 1u;
                private const uint LAST = 30000u;
                private const ContextMenuFlags DEFAULT_FLAG = ContextMenuFlags.Explore;

                private uint _last = LAST;
                private bool _hasDefaultCommands = false;
                private readonly Menu _menu;
                private readonly ShellContextMenu _contextMenu;
                private readonly IBrowsableObjectInfo _parent;
                private readonly ShellContainer __parent;

                private ContextMenu(in IShellObjectInfoBase2 shellObjectInfo, in bool extendedVerbs, in ShellContainer parent, params ShellObject[] so)
                {
                    _parent = shellObjectInfo.Parent;

                    ContextMenuFlags flags = DEFAULT_FLAG;

                    if (extendedVerbs)

                        flags |= ContextMenuFlags.ExtendedVerbs;

                    _menu = new Menu((_contextMenu = new ShellContextMenu(__parent = parent, null, so /*_so = so*/)).Query(FIRST, _last, _parent == null || so.Length > 1 ? flags : flags | ContextMenuFlags.CanRename));

                    /*_ = _contextMenu.Query(1u, uint.MaxValue, ContextMenuFlags.Explore | ContextMenuFlags.CanRename);

                    System.Windows.Point point = Mouse.GetPosition(null);
                    var _point = new System.Drawing.Point((int)point.X, (int)point.Y);

                    _contextMenu.Show(new WindowInteropHelper(window).Handle, _point);*/

                    /*window.ContextMenu = new ContextMenu() { UsesItemContainerTemplate = true, ItemContainerTemplateSelector = new MenuItemTemplateSelector(), ItemsSource =

                    };*/
                }

                public static ContextMenu GetNewContextMenu(in IShellObjectInfoBase2 shellObjectInfo, in bool extendedVerbs)
                {
                    //ShellObject so = ShellObjectFactory.Create(shellObjectInfo.InnerObject.ParsingName);
                    //var parent = (ShellContainer)so.Parent;

                    var parent = (ShellContainer)shellObjectInfo.InnerObject.Parent;

                    return parent == null ? null : new ContextMenu(shellObjectInfo, extendedVerbs, parent, shellObjectInfo.InnerObject /*so, parent*/);
                }

                public static ContextMenu GetNewContextMenu(in IShellObjectInfoBase2 shellObjectInfo, in IEnumerable<IBrowsableObjectInfo> items, in bool extendedVerbs)
                {
                    //ShellObject so = ShellObjectFactory.Create(shellObjectInfo.InnerObject.ParsingName);
                    //var parent = (ShellContainer)so.Parent;

                    var so = new Collections.Generic.ArrayBuilder<ShellObject>();

                    foreach (IBrowsableObjectInfo item in items)

                        if (item is IShellObjectInfoBase2 _item)

                            _ = so.AddLast(_item.InnerObject);

                        else

                            return null;

                    return new ContextMenu(shellObjectInfo, extendedVerbs, (ShellContainer)shellObjectInfo.InnerObject, so.ToArray() /*so, parent*/);
                }

                private static ContextMenuCommand GetContextMenuCommand(in uint selected) => (sbyte)(selected - LAST) + ContextMenuCommand.LastDelegatedCommand;

                public string
#if CS8
                    ?
#endif
                    GetCommandTooltip(ref uint? command)
                {
                    uint _command = command.Value;

                    command = null;

                    string
#if CS8
                    ?
#endif
                    tryGetCommandTooltip(in GetCommandStringFlags flags, in bool unicode) => _contextMenu.TryGetCommandString(_command, flags, unicode);

                    if (_command > FIRST)

                        if (_command > LAST)

                            command = (uint)GetContextMenuCommand(_command);

                        else
                        {
                            _command--;

                            return tryGetCommandTooltip(GetCommandStringFlags.HelpTextW, true) ?? tryGetCommandTooltip(GetCommandStringFlags.HelpTextA, false);
                        }

                    return null;
                }

                private MenuItemInfo UpdateMenuItemInfo(MenuItemInfo mi)
                {
                    mi.wID = ++_last;

                    mi.fMask |= MenuItemInfoFlags.ID;

                    return mi;
                }

                private static MenuItemInfo GetSeparator() => new MenuItemInfo() { cbSize = (uint)Marshal.SizeOf<MenuItemInfo>(), fMask = MenuItemInfoFlags.Type, fType = MenuFlags.Separator };

                private bool TryInsertMenu(in uint i, MenuItemInfo menuItemInfo) => _contextMenu.TryInsertMenu(i, true, ref menuItemInfo);

                public void AddCommands(IEnumerable<MenuItemInfo> menuItems)
                {
                    uint i = 0;

                    bool add(in MenuItemInfo _mi) => TryInsertMenu(i, _mi);

                    PredicateIn<MenuItemInfo> action = (in MenuItemInfo __mi) =>
                    {
                        action = (in MenuItemInfo ___mi) =>
                        {
                            i++;

                            return add(___mi);
                        };

                        _hasDefaultCommands = true;

                        return add(__mi);
                    };

                    foreach (MenuItemInfo mi in menuItems)

                        if (!action(UpdateMenuItemInfo(mi)))
                        {
                            _last--;

                            break;
                        }

                    _ = action(GetSeparator());
                }

                private string
#if CS8
                    ?
#endif
                    TryGetCommandString(in uint command) => _contextMenu.TryGetCommandString(command, GetCommandStringFlags.VerbA, false);

                public void AddExtensionCommands(IEnumerable<KeyValuePair<ExtensionCommand, MenuItemInfo>> menuItems)
                {
                    MenuItemInfo _mi;
                    int count;
                    uint i = _last - LAST;

                    bool tryInsertMenu() => _contextMenu.TryInsertMenu(i, true, ref _mi);

                    void addAll()
                    {
                        foreach (KeyValuePair<ExtensionCommand, MenuItemInfo> mi in menuItems)
                        {
                            _mi = UpdateMenuItemInfo(mi.Value);

                            count = _menu.Count;

                            if (!tryInsertMenu())

                                break;

                            /*commandCanonicalName = mi.Key.GetCanonicalName();

                            void add()
                            {
                                i = 0;

                                foreach (MenuItem menuItem in _menu)
                                {
                                    i++;

                                    if (menuItem.Id.HasValue && TryGetCommandString(menuItem.Id.Value) == commandCanonicalName)
                                    {
                                        _ = _contextMenu.TryInsertMenu(i + 1, true, ref _mi);

                                        return;
                                    }
                                }

                                TryPrependMenu(_mi);
                            }

                            add();*/
                        }
                    }

                    void addSeparator()
                    {
                        _mi = GetSeparator();

                        _ = tryInsertMenu();
                    }

                    if (_last == LAST)
                    {
                        addSeparator();

                        addAll();
                    }

                    else
                    {
                        addAll();

                        addSeparator();
                    }
                }

                public ContextMenuCommand Show(IntPtr hwnd, HookRegistration hookRegistration, System.Drawing.Point point, bool ctrl = false, bool shift = false)
                {
                    _ = _contextMenu.TrySetMenuDefaultItem(_hasDefaultCommands ? 0 : _last - LAST, true);

                    _contextMenu.UpdateHookRegistration(hookRegistration);

                    uint selected = _contextMenu.Show(hwnd, point);

                    if (selected > FIRST)

                        if (selected > LAST)

                            return GetContextMenuCommand(selected);

                        else if (TryGetCommandString(--selected) == "rename")

                            return ContextMenuCommand.Rename;

                        else if (TryGetCommandString(selected) == "delete")

                            return ContextMenuCommand.Delete;

                        else

                            _contextMenu.InvokeCommand(selected, point);

                    return ContextMenuCommand.None;
                }

                public void Open(IBrowsableObjectInfo[] items, System.Drawing.Point point, bool ctrl = false, bool shift = false)
                {
                    if (items.Length > 0)
#if CS8
                    {
#endif
                        using
#if !CS8
                        (
#endif
                        var cm = new ShellContextMenu(__parent, null, items.Select(item => (ShellObject)item.InnerObject) /*_so = so*/)
#if CS8
                        ;
#else
                        )
                        {
#endif

                        _ = cm.Query(FIRST, LAST, DEFAULT_FLAG);

                        cm.InvokeCommand("open", point, ctrl, shift);
                    }
                }

                private void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _contextMenu.Dispose();
                        _menu.Dispose();
                        //_so?.Dispose();
                        __parent.Dispose();
                    }
                }

                ~ContextMenu() => Dispose(false);

                public void Dispose()
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }

                /*public override void Execute(object value)
                {
                    var command = MenuItem.Id.Value - 1;

                    if (_menu.ContextMenu.TryGetCommandString(command, GetCommandStringFlags.VerbW, true) == "rename")

                    else
                    {
                        Point _point = Mouse.GetPosition(null);
                        var point = new System.Drawing.Point((int)_point.X, (int)_point.Y);

                        ContextMenuInvokeCommandInfo ci = _menu.ContextMenu.GetDefaultInvokeCommandInfo(command, point);

                        _menu.ContextMenu.InvokeCommand(ref ci);
                    }
                }*/
            }
            #endregion

            #region Fields
            private ShellObject _shellObject;
            private IBrowsableObjectInfo _parent;
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
            public override IShellObjectInfoBase ArchiveShellObject => ObjectPropertiesGeneric.FileType == Archive ? this : null;

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
            protected override bool IsSpecialItemOverride
            {
                get
                {
                    uint? value = _shellObject.Properties.System.FileAttributes.Value;

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
            /// Gets the localized name of this <see cref="ShellObjectInfo"/> depending the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
            /// </summary>
            public override string LocalizedName => _shellObject.GetDisplayName(DisplayNameType.Default);

            /// <summary>
            /// Gets the name of this <see cref="ShellObjectInfo"/> depending of the associated <see cref="ShellObject"/> (see the <see cref="ShellObject"/> property for more details.
            /// </summary>
            public override string Name => _shellObject.Name;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => ShellObjectPropertySystemCollection._GetShellObjectPropertySystemCollection(this);

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

            protected override IEnumerable<IProcessInfo> CustomProcessesOverride => DefaultCustomProcessesSelectorDictionary.Select(this);

            public override string Protocol
#if CS8
                =>
#else
            {
                get
                {
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
            }

            #region Methods
            protected override IContextMenu GetContextMenuOverride(bool extendedVerbs) => ContextMenu.GetNewContextMenu(this, extendedVerbs);

            protected override IContextMenu GetContextMenuOverride(IEnumerable<IBrowsableObjectInfo> items, bool extendedVerbs) => ContextMenu.GetNewContextMenu(this, items, extendedVerbs);

            protected internal new void RaiseCallbacks(in BrowsableObjectInfoCallbackArgs a) => base.RaiseCallbacks(a);

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
                IBrowsableObjectInfo getDefault() => From(SystemPath.GetDirectoryName(_shellObject.ParsingName), ClientVersion);

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
            public StreamInfo GetArchiveFileStream()
            {
                ThrowArchiveOpenStatusException(true);

                return _getStreamStruct?.ArchiveFileStream;
            }

            protected void ThrowItemIsNotAnArchiveException() => throw new InvalidOperationException("The current item is not an archive.");

            protected void ThrowArchiveOpenStatusException(in bool mustBeOpen)
            {
                string msg = null;

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

                    var getStreamStruct = new GetStreamStruct(() => new FileStream(Path, fileMode, fileAccess, fileShare, bufferSize ?? 4096, fileOptions));

                    _getStreamStruct = getStreamStruct; // The field has to remain uninitialized if the struct constructor fails to initialize the stream.
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
            protected override IEnumerable<Util.Commands.Primitives.ICommand> GetCommandsOverride(IEnumerable<IBrowsableObjectInfo> items) => null;
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

        public class ShellObjectInfo : ShellObjectInfo<IFileSystemObjectInfoProperties, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>, IShellObjectInfo
        {
            #region Fields
            private static readonly BrowsabilityPathStack<IShellObjectInfo> __browsabilityPathStack = new
#if !CS9
                BrowsabilityPathStack<IShellObjectInfo>
#endif
                ();

            private IFileSystemObjectInfoProperties _objectProperties;
            private IProcessFactory _processFactory;
            #endregion

            #region Properties
            #region Static Properties
            public static IBrowsabilityPathStack<IShellObjectInfo> BrowsabilityPathStack { get; } = __browsabilityPathStack.AsWriteOnly();

            public static ISelectorDictionary<IShellObjectInfoBase2, IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IShellObjectInfoBase2, System.Collections.Generic.IEnumerable<IProcessInfo>>();

            public static IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new ShellObjectInfoSelectorDictionary();

            public static Action RegisterDefaultBrowsabilityPaths { get; private set; } = () =>
              {
                  BrowsabilityPathStack.Push(new MultiIconInfo.BrowsabilityPath());
                  BrowsabilityPathStack.Push(new DotNetAssemblyInfo.BrowsabilityPath());

                  RegisterDefaultBrowsabilityPaths = EmptyVoid;
              };

            public static Action RegisterDefaultProcessSelectors { get; private set; } = () =>
            {
                DefaultProcessSelectorDictionary.Push(item => Predicate(item, typeof(Consts.Guids.Shell.Process.Shell)), TryGetProcess

                // System.Reflection.Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(t => t.Namespace.StartsWith(typeof(Process.ObjectModel.IProcess).Namespace) && t.GetCustomAttribute<ProcessGuidAttribute>().Guid == guid);
                );

                RegisterDefaultProcessSelectors = EmptyVoid;
            };
            #endregion Static Properties

            #region Overrides
            protected override IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

            protected sealed override IFileSystemObjectInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

            protected sealed override IProcessFactory ProcessFactoryOverride => _processFactory;
            #endregion Overrides
            #endregion Properties

            #region Constructors
            private ShellObjectInfo(in BrowsableObjectInfoURL url, FileType fileType, in ShellObject shellObject, in ClientVersion clientVersion) : base(url, shellObject, clientVersion)
            {
                _objectProperties = GetDefaultProperties(this, fileType);

                _processFactory = new ShellObjectInfoProcessFactory(this);
            }

            public ShellObjectInfo(in string path, FileType fileType, in ShellObject shellObject, in ClientVersion clientVersion) : this(new BrowsableObjectInfoURL(path), fileType, shellObject, clientVersion) { /* Left empty. */ }

            public ShellObjectInfo(in IKnownFolder knownFolder, in ClientVersion clientVersion) : this(GetURL(knownFolder), KnownFolder, ShellObjectFactory.Create(knownFolder.ParsingName), clientVersion) { /* Left empty. */ }
            #endregion

            #region Methods
            private static BrowsableObjectInfoURL GetURL(in IKnownFolder knownFolder)
            {
                string path = knownFolder.Path;

                bool updatePath() => string.IsNullOrEmpty(path);

                if (updatePath())

                    path = knownFolder.LocalizedName;

                return new BrowsableObjectInfoURL(updatePath() ? knownFolder.CanonicalName : path, knownFolder.ParsingName);
            }

            public static ShellObjectInfo GetDefault(in ClientVersion clientVersion) => new ShellObjectInfo(KnownFolders.Desktop, clientVersion);

            public static IFileSystemObjectInfoProperties GetDefaultProperties(IShellObjectInfoBase2 shellObjectInfo, FileType fileType)
            {
                IFileSystemObjectInfoProperties getFolderProperties() => new FolderShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType);
                IFileSystemObjectInfoProperties getFileSystemObjectInfoProperties() => new FileSystemObjectInfoProperties(shellObjectInfo, fileType);
#if CS9
                return
#else
                switch (
#endif
                    fileType
#if CS9
                    switch
#else
                    )
#endif
                {
#if !CS9
                    case
#endif
                        Folder
#if CS9
                        =>
#else
                        :
                        return
#endif
                        getFolderProperties()
#if CS9
                        ,
#else
                        ;
                    case
#endif
                        FileType.File
#if CS9
                        or
#else
                        :
                    case
#endif
                        Archive
#if CS9
                        or
#else
                        :
                    case
#endif
                        Library
#if CS9
                        or
#else
                        :
                    case
#endif
                        Link
#if CS9
                        =>
#else
                        :
                        return
#endif
                        new FileShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType)
#if CS9
                        ,
#else
                        ;
                    case
#endif
                        KnownFolder
#if CS9
                        =>
#else
                        :
                        return
#endif
                        shellObjectInfo.InnerObject.IsFileSystemObject ? getFolderProperties() : getFileSystemObjectInfoProperties()
#if CS9
                        ,
#else
                        ;
                    case
#endif
                        Drive
#if CS9
                        =>
#else
                        :
                        return
#endif
                        new DriveShellObjectInfoProperties<IShellObjectInfoBase2>(shellObjectInfo, fileType)
#if CS9
                        ,
                        _ =>
#else
                        ;
                    default:
                        return
#endif
                        getFileSystemObjectInfoProperties()
#if CS9
                    };
#else
                    ;
                }
#endif
            }

            public static ShellObjectInitInfo GetInitInfo(in ShellObject shellObject)
            {
#if CS8
                static
#endif
                FileType getFileType(in string path, in FileType defaultFileType) => IsSupportedArchiveFormat(SystemPath.GetExtension(path)) ? Archive : defaultFileType;

                switch (shellObject ?? throw GetArgumentNullException(nameof(shellObject)))
                {
                    case ShellFolder _:

                        switch (shellObject)
                        {
                            case ShellFileSystemFolder shellFileSystemFolder:

                                (string path, FileType fileType) = shellFileSystemFolder is FileSystemKnownFolder ? (shellObject.ParsingName, KnownFolder) : (shellFileSystemFolder.Path, Folder);

                                return new ShellObjectInitInfo(path, System.IO.Directory.GetParent(path) is null ? Drive : getFileType(path, fileType));

                            case NonFileSystemKnownFolder nonFileSystemKnownFolder:

                                return new ShellObjectInitInfo(nonFileSystemKnownFolder.Path, KnownFolder);

                            case ShellNonFileSystemFolder _:

                                return new ShellObjectInitInfo(shellObject.ParsingName, KnownFolder);
                        }

                        break;

                    case ShellLink shellLink:

                        return new ShellObjectInitInfo(shellLink.Path, Link);

                    case ShellFile shellFile:

                        return new ShellObjectInitInfo(shellFile.Path, shellFile.IsLink ? Link : SystemPath.GetExtension(shellFile.Path) == ".library-ms" ? Library : getFileType(shellFile.Path, FileType.File));
                }

                throw new ArgumentException(ShellObjectIsNotSupported);
            }

            public static IProcess TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters)
            {
                string guid = processParameters.ProcessParameters.Guid.ToString();

                IEnumerator<string> enumerator = null;

                try
                {
                    enumerator = processParameters.ProcessParameters.Parameters.GetEnumerator();

                    PathTypes<IPathInfo>.RootPath getParameter() => enumerator.MoveNext()
                                        ? new PathTypes<IPathInfo>.RootPath(enumerator.Current, true)
                                        : throw new InvalidOperationException(Resources.ExceptionMessages.ProcessParametersCouldNotBeParsedCorrectly);

                    ProcessTypes<T>.IProcessQueue getProcessCollection<T>() where T : IPathInfo => processParameters.Factory.GetProcessCollection<T>();

                    IProcessLinkedList<TPath,
                        ProcessError,
                        ProcessTypes<TPath,
                            ProcessError,
                            TAction>.
                        ProcessErrorItem,
                        TAction> getProcessLinkedList<TPath, TAction>() where TPath : IPathInfo => processParameters.Factory.GetProcessLinkedList<
                                    TPath,
                                    ProcessError,
                                    ProcessTypes<TPath, ProcessError, TAction>.ProcessErrorItem,
                                    TAction>();

                    PathTypes<IPathInfo>.PathInfoBase sourcePath;

                    switch (guid)
                    {
                        case Copy:

                            PathTypes<IPathInfo>.PathInfoBase destinationPath;

                            sourcePath = getParameter();
                            destinationPath = getParameter();

                            IProcess getCopyProcess()
                            {
                                _ = enumerator.MoveNext();

                                bool? option
#if CS8
                                    =
#else
                                    ;
                                bool? getOption()
                                {
                                    switch (
#endif
                                    enumerator.Current
#if CS8
                                    switch
#else
                                    )
#endif
                                    {
#if !CS8
                                        case
#endif
                                        "1"
#if CS8
                                        =>
#else
                                        :
                                            return
#endif
                                        true
#if CS8
                                        ,
#else
                                        ;
                                        case
#endif
                                        "0"
#if CS8
                                        =>
#else
                                        :
                                            return
#endif
                                        false
#if CS8
                                        ,
                                        _ =>
#else
                                        ;
                                        default:
                                            return
#endif
                                        null
#if CS8
                                    };
#else
                                        ;
                                    }
                                }

                                option = getOption();
#endif

                                return option.HasValue
                                    ? new Copy<ProcessErrorFactory<IPathInfo, CopyErrorAction>>(ProcessHelper<IPathInfo>.GetInitialPaths(enumerator, sourcePath, path => path),
                                        sourcePath,
                                        destinationPath,
                                        getProcessCollection<IPathInfo>(),
                                        getProcessLinkedList<IPathInfo, CopyErrorAction>(),
                                        ProcessHelper<IPathInfo>.GetDefaultProcessDelegates(),
                                        new CopyOptions<IPathInfo>(Bool.True, true, option.Value) { IgnoreFolderFileNameConflicts = true },
                                        new CopyProcessErrorFactory<IPathInfo>())
                                    : null;
                            }

                            return getCopyProcess();

                        case Deletion:

                            sourcePath = getParameter();

                            IProcess getDeletionProcess()
                            {
                                _ = enumerator.MoveNext();

                                return Enum.TryParse(enumerator.Current, out RemoveOption option)
                                    ? new Deletion<ProcessErrorFactory<IPathInfo, ErrorAction>>(ProcessHelper<IPathInfo>.GetInitialPaths(enumerator, sourcePath, path => path),
                                            sourcePath,
                                            getProcessCollection<IPathInfo>(),
                                            getProcessLinkedList<IPathInfo, ErrorAction>(),
                                            ProcessHelper<IPathInfo>.GetDefaultProcessDelegates(),
                                            new DeletionOptions<IPathInfo>(Bool.True, true, option),
                                            new DefaultProcessErrorFactory<IPathInfo>())
                                    : null;
                            }

                            return getDeletionProcess();
                    }
                }

                finally
                {
                    enumerator?.Dispose();
                }

                return null;
            }

            public static IProcess GetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => TryGetProcess(processParameters) ?? throw new InvalidOperationException(NoProcessCouldBeGenerated);

            #region Construction Helpers
            public static ShellObjectInfo From(in ShellObject shellObject, in ClientVersion clientVersion)
            {
                ShellObjectInitInfo initInfo = GetInitInfo(shellObject);

                return new ShellObjectInfo(initInfo.Path, initInfo.FileType, shellObject, clientVersion);
            }

            public static ShellObjectInfo From(in ShellObject shellObject) => From(shellObject, GetDefaultClientVersion());

            public static ShellObjectInfo From(in string path, in ClientVersion clientVersion) => From(ShellObjectFactory.Create(path), clientVersion);

            public static ShellObjectInfo From(in string path) => From(path, GetDefaultClientVersion());
            #endregion

            protected override IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

            protected override void DisposeUnmanaged()
            {
                base.DisposeUnmanaged();

                _ = UtilHelpers.TryDispose(ref _processFactory);
                _ = UtilHelpers.TryDispose(ref _objectProperties);
            }

            #region GetItems
            public IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ArchiveFileInfoEnumeratorStruct> func) => func == null ? GetItems(GetItemProviders(item => true)) : GetItems(GetItemProviders(func));

            protected override IEnumerable<ShellObjectInfoItemProvider> GetItemProviders(Predicate<ShellObjectInfoEnumeratorStruct> predicate) => Browsability.Browsability.IsBrowsable()
                ? ObjectPropertiesGeneric?.FileType == Archive
                    ? GetItemProviders(item => predicate(new ShellObjectInfoEnumeratorStruct(item)))
                    : ShellObjectInfoEnumeration.From(this, predicate)
                : GetEmptyEnumerable();

            protected virtual IEnumerable<ShellObjectInfoItemProvider> GetItemProviders(Predicate<ArchiveFileInfoEnumeratorStruct> func)
#if CS8
             =>
#else
            {
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
                    Archive
#if CS8
                    =>
#else
                    :
                        return
#endif
                    ArchiveItemInfo.GetArchiveItemInfoItems(this, func).Select(ShellObjectInfoItemProvider.ToShellObjectInfoItemProvider)
#if CS8
                    ,
                    _ =>
#else
                    ;
                    default:
                        return
#endif
                    GetEmptyEnumerable()
#if CS8
                };
#else
                    ;
                }
            }
#endif

            protected override IEnumerable<ShellObjectInfoItemProvider> GetItemProviders() => GetItemProviders((Predicate<ShellObjectInfoEnumeratorStruct>)(obj => true));
            #endregion // GetItems
            #endregion // Methods
        }
    }
}
