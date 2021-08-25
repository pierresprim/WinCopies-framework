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

using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native;
using Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager;

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WinCopies.Temp;

using static Microsoft.WindowsAPICodePack.Shell.DesktopWindowManager;

using static WinCopies.Util.Desktop.UtilHelpers;

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

    public class Window : System.Windows.Window
    {
        private static DependencyProperty Register<T>(in string propertyName, in PropertyMetadata propertyMetadata) => Register<T, Window>(propertyName, propertyMetadata);

        public static readonly DependencyProperty CloseButtonProperty = Register<bool>(nameof(CloseButton), new PropertyMetadata(true, (DependencyObject d, DependencyPropertyChangedEventArgs e) => _ = (bool)e.NewValue ? EnableCloseMenuItem((Window)d) : DisableCloseMenuItem((Window)d)));

        public bool CloseButton { get => (bool)GetValue(CloseButtonProperty); set => SetValue(CloseButtonProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HelpButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HelpButtonProperty = Register<bool>(nameof(HelpButton), new PropertyMetadata(false));

        public bool HelpButton { get => (bool)GetValue(HelpButtonProperty); set => SetValue(HelpButtonProperty, value); }

        private static readonly DependencyPropertyKey IsInHelpModePropertyKey = RegisterReadOnly<bool, Window>(nameof(IsInHelpMode), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsInHelpMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInHelpModeProperty = IsInHelpModePropertyKey.DependencyProperty;

        public bool IsInHelpMode => (bool)GetValue(IsInHelpModeProperty);

        public static readonly DependencyProperty NotInHelpModeCursorProperty = Register<Cursor>(nameof(NotInHelpModeCursor), new PropertyMetadata(Cursors.Arrow));

        public Cursor NotInHelpModeCursor { get => (Cursor)GetValue(NotInHelpModeCursorProperty); set => SetValue(NotInHelpModeCursorProperty, value); }

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
