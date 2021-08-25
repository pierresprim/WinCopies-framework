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

using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native;
using Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager;

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Commands;
using WinCopies.Temp;
using WinCopies.Util.Data;

using static Microsoft.WindowsAPICodePack.Shell.DesktopWindowManager;
using static Microsoft.WindowsAPICodePack.Win32Native.Menus.Menus;
using static Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager.DesktopWindowManager;

using static WinCopies.Delegates;
using static WinCopies.Util.Desktop.UtilHelpers;
using static WinCopies.ThrowHelper;
using static WinCopies.Temp.Temp;
using WinCopies.Linq;

namespace WinCopies.GUI.Windows
{
    public enum XButton : sbyte
    {
        One = 1,

        Two = 2
    }

    public enum XButtonClick : sbyte
    {
        Down = 1,

        Up = 2,

        DoubleClick = 3
    }

    public class TitleBarMenuItem : ViewModelBase, ICommandSource, DotNetFix.IDisposable
    {
        private ICommand _command;
        private object _commandParameter;
        private IInputElement _commandTarget;
        private string _header;

        public uint Id { get; internal set; }

        public TitleBarMenuItemQueue Collection { get; internal set; }

        public string Header { get => _header; set => UpdateValue(ref _header, value, nameof(Header)); }

        public ICommand Command { get => _command; set => UpdateValue(ref _command, value, nameof(Command)); }

        public object CommandParameter { get => _commandParameter; set => UpdateValue(ref _commandParameter, value, nameof(CommandParameter)); }

        public IInputElement CommandTarget { get => _commandTarget; set => UpdateValue(ref _commandTarget, value, nameof(CommandTarget)); }

        public bool IsDisposed => Command == null;

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)

                return;

            if (disposing)
            {
                if (Collection == null)

                    Id = 0u;

                _command = null;
                _commandParameter = null;
                _commandTarget = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }

    public class TitleBarMenuItemQueue : Collections.DotNetFix.Generic.TEMP.EnumerableQueueCollection<TitleBarMenuItem>
    {
        public uint LastId { get; private set; }

        protected override void EnqueueItem(TitleBarMenuItem item)
        {
            ThrowIfNull(item, nameof(item));

            if (item.Collection == null)
            {
                item.Collection = this;
                item.Id = ++LastId;

                base.EnqueueItem(item);

                LastId = item.Id;
            }

            else

                throw new ArgumentException("The given item is already in a collection.");
        }

        private static void ResetItem(in TitleBarMenuItem item)
        {
            item.Collection = null;
            item.Id = 0u;
        }

        protected override bool TryDequeueItem([MaybeNullWhen(false)] out TitleBarMenuItem result)
        {
            if (base.TryDequeueItem(out result))
            {
                ResetItem(result);

                return true;
            }

            return false;
        }

        protected override TitleBarMenuItem DequeueItem()
        {
            TitleBarMenuItem item = base.DequeueItem();

            ResetItem(item);

            return item;
        }

        protected override void ClearItems()
        {
            while (((IUIntCountable)InnerQueue).Count > 0)

                DequeueItem();
        }
    }

    public class Window : System.Windows.Window
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, Window>(propertyName);

        private static DependencyProperty Register<T>(in string propertyName, in PropertyMetadata propertyMetadata) => Register<T, Window>(propertyName, propertyMetadata);

        public static readonly DependencyProperty CloseButtonProperty = Register<bool>(nameof(CloseButton), new PropertyMetadata(true, (DependencyObject d, DependencyPropertyChangedEventArgs e) => _ = (bool)e.NewValue ? EnableCloseMenuItem((Window)d) : DisableCloseMenuItem((Window)d)));

        public bool CloseButton { get => (bool)GetValue(CloseButtonProperty); set => SetValue(CloseButtonProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HelpButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HelpButtonProperty = Register<bool>(nameof(HelpButton));

        public bool HelpButton { get => (bool)GetValue(HelpButtonProperty); set => SetValue(HelpButtonProperty, value); }

        private static readonly DependencyPropertyKey IsInHelpModePropertyKey = RegisterReadOnly<bool, Window>(nameof(IsInHelpMode), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsInHelpMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInHelpModeProperty = IsInHelpModePropertyKey.DependencyProperty;

        public bool IsInHelpMode => (bool)GetValue(IsInHelpModeProperty);

        public static readonly DependencyProperty NotInHelpModeCursorProperty = Register<Cursor>(nameof(NotInHelpModeCursor), new PropertyMetadata(Cursors.Arrow));

        public Cursor NotInHelpModeCursor { get => (Cursor)GetValue(NotInHelpModeCursorProperty); set => SetValue(NotInHelpModeCursorProperty, value); }

        public static readonly DependencyProperty TitleBarMenuItemsProperty = Register<TitleBarMenuItemQueue>(nameof(TitleBarMenuItem), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((Window)d).OnTitleBarMenuItemsCollectionChanged((TitleBarMenuItemQueue)e.OldValue, (TitleBarMenuItemQueue)e.NewValue)));

        public TitleBarMenuItemQueue TitleBarMenuItems { get => (TitleBarMenuItemQueue)GetValue(TitleBarMenuItemsProperty); set => SetValue(TitleBarMenuItemsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HelpButtonClick"/> routed event.
        /// </summary>
        public static readonly RoutedEvent HelpButtonClickEvent = EventManager.RegisterRoutedEvent(nameof(HelpButtonClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Window));

        public event RoutedEventHandler HelpButtonClick
        {
            add => AddHandler(HelpButtonClickEvent, value);

            remove => RemoveHandler(HelpButtonClickEvent, value);
        }

        static Window() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));

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

            IntPtr menu = GetSystemMenu(new WindowInteropHelper(this).Handle, false);

            uint id;
            int count = GetMenuItemCount(menu);
            uint i;
            MenuItemInfo menuItemInfo;

            foreach (TitleBarMenuItem item in menuItems)
            {
                id = item.Id == 0u ? item.Collection.LastId + 1 : item.Id;

                menuItemInfo = new MenuItemInfo() { cbSize = (uint)Marshal.SizeOf<MenuItemInfo>() };

                bool ok = true;

                while (true)
                {
                    for (i = 0u; i < count; i++)
                    {
                        GetMenuItemInfoW(menu, i, true, ref menuItemInfo);

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

                AppendMenuW(menu, MenuFlags.String, id, item.Header ?? (item.Command is RoutedCommand command ? command.Name : null));

                item.PropertyChanged += onPropertyChanged;
            }
        }

        private void OnRemoveMenuItems(in System.Collections.Generic.IEnumerable<TitleBarMenuItem> menuItems)
        {
            void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => TitleBarMenuItem_PropertyChanged((TitleBarMenuItem)sender, e);

            IntPtr menu = GetSystemMenu(new WindowInteropHelper(this).Handle, false);

            foreach (TitleBarMenuItem item in menuItems)
            {
                item.PropertyChanged -= onPropertyChanged;

                DeleteMenu(menu, (SystemMenuCommands)item.Id, MenuFlags.ByCommand);
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

        protected void TitleBarMenuItem_PropertyChanged(TitleBarMenuItem sender, System.ComponentModel.PropertyChangedEventArgs e) => ;

        protected virtual void OnSourceInitialized(HwndSource hwndSource)
        {
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

            hwndSource.AddHook(OnSourceHook);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

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
            if (Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager.DesktopWindowManager.GetSystemCommandWParam(wParam) == (int)SystemCommand.ContextHelp)
            {
                OnHelpButtonClick();

                return true;
            }

            return false;
        }

        protected virtual bool OnShowWindowMessage()
        {
            if (!CloseButton)

                _ = DisableCloseMenuItem(this);

            return false;
        }

        protected virtual IntPtr OnSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (handled)

                return IntPtr.Zero;

            var _msg = (WindowMessage)msg;

            IntPtr result = OnSourceHook(hwnd, _msg, wParam, lParam, out bool _handled);

            if (_handled)

                handled = true;

            return result;
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

        protected virtual IntPtr OnSourceHook(IntPtr hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            if (((ushort)msg).BetweenA((ushort)WindowMessage.XBUTTONDOWN, (ushort)WindowMessage.XBUTTONDBLCLK, true, true))

                handled = OnXButtonClick((XButton)Temp.Temp.HIWORD(wParam), (XButtonClick)((ushort)msg - ((ushort)WindowMessage.XBUTTONDOWN - 1)));

            else

                switch (msg)
                {
                    case WindowMessage.SystemCommand:

                        handled = OnSystemCommandMessage(wParam);

                        break;

                    case WindowMessage.ShowWindow:

                        handled = OnShowWindowMessage();

                        break;

                    case WindowMessage.Close:

                        handled = !CloseButton;

                        break;

                    default:

                        handled = false;

                        break;
                }

            return IntPtr.Zero;
        }
    }
}
