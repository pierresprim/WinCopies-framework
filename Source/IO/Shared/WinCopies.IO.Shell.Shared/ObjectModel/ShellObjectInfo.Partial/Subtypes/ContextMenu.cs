using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native.Menus;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using WinCopies.Linq;

namespace WinCopies.IO.ObjectModel
{
    public abstract partial class ShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>
    {
        private class ContextMenu : IContextMenu
        {
            private const uint FIRST = 1u;
            private const uint LAST = 30000u;
            private const ContextMenuFlags DEFAULT_FLAG = ContextMenuFlags.Explore;

            private uint _last = LAST;
            private bool _hasDefaultCommands = false;
            private readonly Menu _menu;
            private readonly ShellContextMenu _contextMenu;
            private readonly IBrowsableObjectInfo
#if CS8
                ?
#endif
                _parent;
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

            public static ContextMenu
#if CS8
                ?
#endif
                GetNewContextMenu(in IShellObjectInfoBase2 shellObjectInfo, in bool extendedVerbs)
            {
                //ShellObject so = ShellObjectFactory.Create(shellObjectInfo.InnerObject.ParsingName);
                //var parent = (ShellContainer)so.Parent;

                var parent = shellObjectInfo.InnerObject.Parent as ShellContainer;

                return parent == null ? null : new ContextMenu(shellObjectInfo, extendedVerbs, parent, shellObjectInfo.InnerObject /*so, parent*/);
            }

            public static ContextMenu
#if CS8
                ?
#endif
                GetNewContextMenu(in IShellObjectInfoBase2 shellObjectInfo, in IEnumerable<IBrowsableObjectInfo> items, in bool extendedVerbs)
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
                if (command == null)

                    return null;

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

            private static MenuItemInfo GetSeparator() => new
#if !CS9
                MenuItemInfo
#endif
                ()
            { cbSize = (uint)Marshal.SizeOf<MenuItemInfo>(), fMask = MenuItemInfoFlags.Type, fType = MenuFlags.Separator };

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
    }
}
