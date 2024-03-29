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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

#region Usings
#region Namespaces
#region WAPICP
using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native.Menus;
using Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager;
#endregion WAPICP

#region System
using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WinCopies.Desktop;
#endregion System

using WinCopies.Linq;
#endregion Namespaces

#region Static Usings
#region WAPICP
using static Microsoft.WindowsAPICodePack.Shell.DesktopWindowManager;
using static Microsoft.WindowsAPICodePack.Win32Native.InteropTools;
using static Microsoft.WindowsAPICodePack.Win32Native.Menus.Menus;
using static Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager.DesktopWindowManager;
#endregion WAPICP

using static WinCopies.Delegates;
using static WinCopies.Util.Desktop.UtilHelpers;
#endregion Static Usings
#endregion Usings

namespace WinCopies.GUI.Windows
{
    public class Window : GlassWindow
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, Window>(propertyName);

        private static DependencyProperty Register<T>(in string propertyName, in PropertyMetadata propertyMetadata) => Register<T, Window>(propertyName, propertyMetadata);

        private static DependencyPropertyKey RegisterReadOnly<T>(in string propertyName, in PropertyMetadata propertyMetadata) => RegisterReadOnly<T, Window>(propertyName, propertyMetadata);

        private static RoutedEvent Register<T>(in string eventName, in RoutingStrategy routingStrategy) => Register<T, Window>(eventName, routingStrategy);

        public static readonly DependencyProperty CloseButtonProperty = Register<bool>(nameof(CloseButton), new PropertyMetadata(true, (DependencyObject d, DependencyPropertyChangedEventArgs e) => _ = (bool)e.NewValue ? EnableCloseMenuItem((Window)d) : DisableCloseMenuItem((Window)d)));

        public bool CloseButton { get => (bool)GetValue(CloseButtonProperty); set => SetValue(CloseButtonProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HelpButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HelpButtonProperty = Register<bool>(nameof(HelpButton));

        public bool HelpButton { get => (bool)GetValue(HelpButtonProperty); set => SetValue(HelpButtonProperty, value); }

        private static readonly DependencyPropertyKey IsInHelpModePropertyKey = RegisterReadOnly<bool>(nameof(IsInHelpMode), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsInHelpMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInHelpModeProperty = IsInHelpModePropertyKey.DependencyProperty;

        public bool IsInHelpMode => (bool)GetValue(IsInHelpModeProperty);

        public static readonly DependencyProperty NotInHelpModeCursorProperty = Register<Cursor>(nameof(NotInHelpModeCursor), new PropertyMetadata(Cursors.Arrow));

        public Cursor NotInHelpModeCursor { get => (Cursor)GetValue(NotInHelpModeCursorProperty); set => SetValue(NotInHelpModeCursorProperty, value); }

        public static readonly DependencyProperty TitleBarMenuItemsProperty = Register<TitleBarMenuItemQueue>(nameof(TitleBarMenuItem), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            var window = (Window)d;

            (window.IsSourceInitialized ? window : throw new InvalidOperationException("The window's source is not initialized.")).OnTitleBarMenuItemsCollectionChanged((TitleBarMenuItemQueue)e.OldValue, (TitleBarMenuItemQueue)e.NewValue);
        }));

        public TitleBarMenuItemQueue TitleBarMenuItems { get => (TitleBarMenuItemQueue)GetValue(TitleBarMenuItemsProperty); set => SetValue(TitleBarMenuItemsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HelpButtonClick"/> routed event.
        /// </summary>
        public static readonly RoutedEvent HelpButtonClickEvent = Register<RoutedEventHandler>(nameof(HelpButtonClick), RoutingStrategy.Bubble);

        public event RoutedEventHandler HelpButtonClick
        {
            add => AddHandler(HelpButtonClickEvent, value);

            remove => RemoveHandler(HelpButtonClickEvent, value);
        }

        static Window() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Alt) && e.Key == Key.Up)

                Commands.NavigationCommands.BrowseToParent.Execute(null, this);
        }

        protected virtual void OnTitleBarMenuItemsCollectionChanged(TitleBarMenuItemQueue oldValue, TitleBarMenuItemQueue newValue)
        {
            void onCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnTitleBarMenuItemsChanged(e);

            if (oldValue != null)
            {
                if (oldValue is INotifyCollectionChanged _oldValue)

                    _oldValue.CollectionChanged -= onCollectionChanged;

                OnRemoveMenuItems(oldValue);
            }

            if (newValue != null)
            {
                OnAddMenuItems(newValue);

                if (newValue is INotifyCollectionChanged _newValue)

                    _newValue.CollectionChanged += onCollectionChanged;
            }
        }

        private void OnAddMenuItems(in System.Collections.Generic.IEnumerable<TitleBarMenuItem> menuItems)
        {
            void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => TitleBarMenuItem_PropertyChanged((TitleBarMenuItem)sender, e);

            IntPtr menu = GetSystemMenu();

            uint id;
            int count = GetMenuItemCount(menu);
            uint i;
            MenuItemInfo menuItemInfo;

            foreach (TitleBarMenuItem item in menuItems)
            {
                id = item.Id == 0u ? item.Collection.LastId + 1 : item.Id;

                menuItemInfo = new MenuItemInfo() { cbSize = (uint)Marshal.SizeOf<MenuItemInfo>(), fMask = MenuItemInfoFlags.ID };

                bool ok = true;

                while (true)
                {
                    for (i = 0u; i < count; i++)
                    {
                        _ = GetMenuItemInfoW(menu, i, true, ref menuItemInfo);

                        if (menuItemInfo.wID == id)

                            if (id == uint.MaxValue)

                                throw new InvalidOperationException("Too much items.");

                            else
                            {
                                id++;

                                ok = false;

                                break;
                            }
                    }

                    if (ok)

                        break;
                }

                item.Id = id;

                _ = AppendMenu(menu, Microsoft.WindowsAPICodePack.Win32Native.Menus.MenuFlags.String, id, item.Header ?? (item.Command is RoutedUICommand command ? command.Text : null));

                if (!item.IsEnabled)

                    _ = EnableMenuItemByCommand(menu, (SystemMenuCommands)id, Microsoft.WindowsAPICodePack.Win32Native.Menus.MenuFlags.Disabled);

                if (item.Icon != null)
                {
                    menuItemInfo = new MenuItemInfo() { cbSize = (uint)Marshal.SizeOf<MenuItemInfo>(), fMask = MenuItemInfoFlags.Bitmap, hbmpItem = item.Icon.GetHbitmap() };

                    _ = SetMenuItemInfoW(menu, id, false, ref menuItemInfo);
                }

                item.PropertyChanged += onPropertyChanged;
            }
        }

        public IntPtr GetSystemMenu() => Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager.DesktopWindowManager.GetSystemMenu(new WindowInteropHelper(this).Handle, false);

        private void OnRemoveMenuItems(in System.Collections.Generic.IEnumerable<TitleBarMenuItem> menuItems)
        {
            void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => TitleBarMenuItem_PropertyChanged((TitleBarMenuItem)sender, e);

            IntPtr menu = GetSystemMenu();

            foreach (TitleBarMenuItem item in menuItems)
            {
                item.PropertyChanged -= onPropertyChanged;

                _ = DeleteMenu(menu, (SystemMenuCommands)item.Id);
            }
        }

        protected virtual void OnTitleBarMenuItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    OnAddMenuItems(e.NewItems.Select(Convert<object, TitleBarMenuItem>));

                    break;

                case NotifyCollectionChangedAction.Remove:

                    OnRemoveMenuItems(e.OldItems.Select(Convert<object, TitleBarMenuItem>));

                    break;
            }
        }

        protected void TitleBarMenuItem_PropertyChanged(TitleBarMenuItem sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TitleBarMenuItem.IsEnabled):

                    _ = EnableMenuItemByCommand(GetSystemMenu(), (SystemMenuCommands)sender.Id, sender.IsEnabled ? Microsoft.WindowsAPICodePack.Win32Native.Menus.MenuFlags.Enabled : Microsoft.WindowsAPICodePack.Win32Native.Menus.MenuFlags.Disabled);

                    break;

                case nameof(TitleBarMenuItem.Header):

                    var menuItemInfo = new MenuItemInfo() { cbSize = (uint)Marshal.SizeOf<MenuItemInfo>(), fMask = MenuItemInfoFlags.String, dwTypeData = sender.Header };

                    _ = SetMenuItemInfo(GetSystemMenu(), (SystemMenuCommands)sender.Id, ref menuItemInfo);

                    break;
            }
        }

        protected virtual void OnSourceInitialized(HwndSource hwndSource)
        {
            base.OnSourceInitialized(hwndSource);

            if (HelpButton)
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;

                SetWindow(hwnd, IntPtr.Zero, 0, 0, 0, 0,
                    (WindowStyles)(((long)GetWindowStyles(hwnd, GetWindowLongEnum.Style) & 0xFFFFFFFF) ^ ((uint)WindowStyles.MinimizeBox | (uint)WindowStyles.MaximizeBox)),
                    (WindowStyles)((uint)GetWindowStyles(hwnd, GetWindowLongEnum.ExStyle) | (uint)WindowStyles.ContextHelp), SetWindowPositionOptions.NoMove | SetWindowPositionOptions.NoSize | SetWindowPositionOptions.NoZOrder | SetWindowPositionOptions.FrameChanged);

                //IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                //uint styles = GetWindowLongPtr(hwnd, GWL_STYLE);
                //styles &= 0xFFFFFFFF ^ (WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
                //SetWindowLongPtr(hwnd, GWL_STYLE, styles);
                //styles = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
                //styles |= WS_EX_CONTEXTHELP;
                //SetWindowLongPtr(hwnd, GWL_EXSTYLE, styles);
                //SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
                //((HwndSource)PresentationSource.FromVisual(this)).AddHook(OnHelpButtonClickHook);
            }
        }

        protected override void OnSourceInitializing(EventArgs e)
        {
            base.OnSourceInitializing(e);

            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)

                OnSourceInitialized(hwndSource);
        }

        // todo: really needed?

        protected void RaiseHelpButtonClickEvent() => RaiseEvent(new RoutedEventArgs(HelpButtonClickEvent));

        protected virtual void OnHelpButtonClick()
        {
            // if (IsInHelpMode)
            // {
            SetValue(IsInHelpModePropertyKey, !IsInHelpMode);

            // Cursor = (bool)IsInHelpMode ? Cursors.Help : Cursors.Arrow;
            // }

            RaiseHelpButtonClickEvent();
        }

        protected virtual bool OnSystemCommandMessage(IntPtr wParam)
        {
            if (GetSystemCommandWParam(wParam) == (int)SystemCommand.ContextHelp)
            {
                OnHelpButtonClick();

                return true;
            }

            else if (TitleBarMenuItems != null)
            {
                int menuId = Microsoft.WindowsAPICodePack.Win32Native.Core.GetLoWord(wParam);

                var menuItems = (System.Collections.Generic.IEnumerable<TitleBarMenuItem>)TitleBarMenuItems;

                foreach (TitleBarMenuItem menuItem in menuItems)

                    if (menuItem.Id == menuId && menuItem.IsEnabled)
                    {
                        menuItem.OnClick();

                        return true;
                    }
            }

            return false;
        }

        protected virtual bool OnShowWindowMessage()
        {
            if (!CloseButton)

                _ = DisableCloseMenuItem(this);

            return false;
        }

        protected virtual bool OnXButtonClick(XButton button, XButtonClick buttonClick)
        {
            if (buttonClick == XButtonClick.Up)

                switch (button)
                {
                    case XButton.One:

                        NavigationCommands.BrowseBack.Execute(null, null);

                        break;

                    case XButton.Two:

                        NavigationCommands.BrowseForward.Execute(null, null);

                        break;
                }

            return true;
        }

        protected override IntPtr OnSourceHook2(WindowMessage msg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            base.OnSourceHook2(msg, wParam, lParam, out handled);

            if (!handled)
            {
#if CS8
                handled =
#else
            if (
#endif
                ((ushort)msg).Between((ushort)WindowMessage.XBUTTONDOWN, (ushort)WindowMessage.XBUTTONDBLCLK, true, true)
#if CS8
                ?
#else
                )

                handled =
#endif
                OnXButtonClick((XButton)HIWORD(wParam), (XButtonClick)((ushort)msg - ((ushort)WindowMessage.XBUTTONDOWN - 1)))
#if CS8
                :
#else
                ;

            else

                switch (
#endif
                msg
#if CS8
                switch
#else
                )
#endif
                {
#if !CS8
                    case
#endif
                    WindowMessage.SystemCommand
#if CS8
                        =>
#else
                        :

                        handled =
#endif
                        OnSystemCommandMessage(wParam)
#if CS8
                        ,
#else
                        ; break;
                    case
#endif
                    WindowMessage.ShowWindow
#if CS8
                        =>
#else
                        :

                        handled =
#endif
                        OnShowWindowMessage()
#if CS8
                        ,
#else
                        ; break;
                    case
#endif
                    WindowMessage.Close
#if CS8
                        =>
#else
                        :

                        handled =
#endif
                        !CloseButton
#if CS8
                        ,
                    _ =>
#else
                        ; break;
                    default:

                        handled =
#endif
                    false
#if CS8
                };
#else
                    ; break;
                }
#endif
            }

            return IntPtr.Zero;
        }
    }
}
