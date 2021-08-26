/* Copyright © Pierre Sprimont, 2021
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

#if DEBUG
using Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager;
using Microsoft.Xaml.Behaviors;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using static Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames;

using static System.Runtime.InteropServices.UnmanagedType;

namespace WinCopies.Temp
{
    public enum MenuItemInfoFlags : uint
    {
        /// <summary>
        /// Retrieves or sets the <see cref="MenuItemInfo.fState"/> member.
        /// </summary>
        State = 0x00000001,

        /// <summary>
        /// Retrieves or sets the <see cref="MenuItemInfo.wID"/> member.
        /// </summary>
        ID = 0x00000002,

        /// <summary>
        /// Retrieves or sets the <see cref="MenuItemInfo.hSubMenu"/> member.
        /// </summary>
        SubMenu = 0x00000004,

        /// <summary>
        /// Retrieves or sets the <see cref="MenuItemInfo.hbmpChecked"/> and <see cref="MenuItemInfo.hbmpUnchecked"/> members.
        /// </summary>
        CheckMarks = 0x00000008,

        /// <summary>
        /// <para>Retrieves or sets the <see cref="MenuItemInfo.fType"/> and <see cref="MenuItemInfo.dwTypeData"/> members.</para>
        /// <para>MIIM_TYPE is replaced by <see cref="Bitmap"/>, <see cref="FType"/>, and <see cref="String"/>.</para>
        /// </summary>
        Type = 0x00000010,

        /// <summary>
        /// Retrieves or sets the <see cref="MenuItemInfo.dwItemData"/> member.
        /// </summary>
        Data = 0x00000020,

        /// <summary>
        /// Retrieves or sets the <see cref="MenuItemInfo.dwTypeData"/> member.
        /// </summary>
        String = 0x00000040,

        /// <summary>
        /// Retrieves or sets the <see cref="MenuItemInfo.hbmpItem"/> member.
        /// </summary>
        Bitmap = 0x00000080,

        /// <summary>
        /// Retrieves or sets the <see cref="MenuItemInfo.fType"/> member.
        /// </summary>
        FType = 0x00000100
    }

    public struct MenuItemInfo
    {
        [MarshalAs(U4)]
        public uint cbSize;
        [MarshalAs(U4)]
        public MenuItemInfoFlags fMask;
        [MarshalAs(U4)]
        public uint fType;         // used if MIIM_TYPE (4.0) or MIIM_FTYPE (>4.0)
        [MarshalAs(U4)]
        public uint fState;        // used if MIIM_STATE
        [MarshalAs(U4)]
        public uint wID;           // used if MIIM_ID
        public IntPtr hSubMenu;      // used if MIIM_SUBMENU
        public IntPtr hbmpChecked;   // used if MIIM_CHECKMARKS
        public IntPtr hbmpUnchecked; // used if MIIM_CHECKMARKS
        public UIntPtr dwItemData;   // used if MIIM_DATA
        [MarshalAs(LPWStr)]
        public string dwTypeData;    // used if MIIM_TYPE (4.0) or MIIM_STRING (>4.0)
        [MarshalAs(U4)]
        public uint cch;           // used if MIIM_TYPE (4.0) or MIIM_STRING (>4.0)
        public IntPtr hbmpItem;      // used if MIIM_BITMAP
    }

    public static class Temp
    {
        [DllImport(User32, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(I4)]
        public static extern int GetMenuItemCount([In] IntPtr hMenu);

        [DllImport(User32, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(U4)]
        public static extern uint GetMenuItemID([In] IntPtr hMenu, [In, MarshalAs(I4)] int nPos);

        [DllImport(User32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr GetSubMenu([In] IntPtr hMenu, [In, MarshalAs(I4)] int nPos);

        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMenuItemInfoW([In] IntPtr hmenu, [In, MarshalAs(U4)] uint item, [In, MarshalAs(UnmanagedType.Bool)] bool fByPosition, [In, Out] ref MenuItemInfo lpmii);

        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyMenu([In] IntPtr hMenu);

        public static IntPtr GetSystemMenu(in System.Windows.Window window, in bool bRevert) => DesktopWindowManager.GetSystemMenu(new WindowInteropHelper(window).Handle, bRevert);

        [DllImport(User32, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AppendMenuW([In] IntPtr hMenu, [In, MarshalAs(U4)] MenuFlags uFlags, [In] UIntPtr uIDNewItem, [In, MarshalAs(LPWStr)] string lpNewItem);

        public static bool AppendMenuW(in IntPtr hMenu, in MenuFlags uFlags, in uint uIDNewItem, in string lpNewItem) => AppendMenuW(hMenu, uFlags, (UIntPtr)uIDNewItem, lpNewItem);

        [DllImport(User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableMenuItem([In] IntPtr hMenu, [In, MarshalAs(U4)] uint uIDEnableItem, [In, MarshalAs(U4)] MenuFlags uEnable);

        public static bool EnableMenuItemByCommand(in IntPtr hMenu, in SystemMenuCommands uIDEnableItem, in MenuFlags uEnable) => EnableMenuItem(hMenu, (uint)uIDEnableItem, MenuFlags.ByCommand | uEnable);

        public static bool EnableMenuItemByPosition(in IntPtr hMenu, in uint uIDEnableItem, in MenuFlags uEnable) => EnableMenuItem(hMenu, uIDEnableItem, MenuFlags.ByPosition | uEnable);

        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenuItemInfoW([In] IntPtr hmenu, [In, MarshalAs(U4)] uint item, [In, MarshalAs(UnmanagedType.Bool)] bool fByPositon, [In] ref MenuItemInfo lpmii);

        public static bool SetMenuItemInfo(IntPtr hmenu, uint item, ref MenuItemInfo lpmii) => SetMenuItemInfoW(hmenu, item, true, ref lpmii);

        public static bool SetMenuItemInfo(IntPtr hmenu, SystemMenuCommands item, ref MenuItemInfo lpmii) => SetMenuItemInfoW(hmenu, (uint)item, false, ref lpmii);

        public static RoutedUICommand BrowseToParent { get; } = new RoutedUICommand("Browse to parent", nameof(BrowseToParent), typeof(Temp), new InputGestureCollection() { new KeyGesture(Key.Up, ModifierKeys.Alt) });

        public static bool BetweenA(this ushort i, ushort x, ushort y, in bool bx, bool by)
        {
            bool between(in Func<bool> func)
            {
                bool b = func();

                return b && (by ? i <= y : i < y);
            }

            return between(bx ?
#if !CS9
                (Func<bool>)(
#endif
                () => x <= i
#if !CS9
                )
#endif
                : () => x < i);
        }
        public static int LOWORD(IntPtr i) => (int)(UIntPtr)(int)i & 0xffff;

        public static int HIWORD(IntPtr i) => ((int)(UIntPtr)(int)i >> 16) & 0xffff;
    }
}
#endif
