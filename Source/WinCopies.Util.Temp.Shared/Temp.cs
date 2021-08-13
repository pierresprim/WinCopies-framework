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
using System;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Win32Native;

namespace WinCopies.Temp
{
    public static class Temp
    {
        public static TOut Convert<TIn, TOut>(TIn value) where TOut : TIn => (TOut)value;

        public static TOut ConvertIn<TIn, TOut>(in TIn value) where TOut : TIn => (TOut)value;

        public static TOut ConvertBack<TIn, TOut>(TIn value) where TIn : TOut => value;

        public static TOut ConvertBackIn<TIn, TOut>(in TIn value) where TIn : TOut => value;

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SHQUERYRBINFO
        {
            public int cbSize;
            public long i64Size;
            public long i64NumItems;
        }

        [DllImport("shell32.dll")]
        public static extern HResult SHQueryRecycleBin(string pszRootPath, ref SHQUERYRBINFO
 pSHQueryRBInfo);

        #region Delegates
        public static object GetValue(Func func) => func == null ? default : func();

        public static T GetValue<T>(Func<T> func) => func == null ? default : func();
        public static TResult GetValue<TParam, TResult>(in Func<TParam, TResult> func, in TParam param) => func == null ? default : func(param);
        public static TResult GetValue<T1, T2, TResult>(in Func<T1, T2, TResult> func, in T1 param1, in T2 param2) => func == null ? default : func(param1, param2);
        public static TResult GetValue<T1, T2, T3, TResult>(in Func<T1, T2, T3, TResult> func, in T1 param1, in T2 param2, in T3 param3) => func == null ? default : func(param1, param2, param3);
        public static TResult GetValue<T1, T2, T3, T4, TResult>(in Func<T1, T2, T3, T4, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4) => func == null ? default : func(param1, param2, param3, param4);
        public static TResult GetValue<T1, T2, T3, T4, T5, TResult>(in Func<T1, T2, T3, T4, T5, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5) => func == null ? default : func(param1, param2, param3, param4, param5);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, TResult>(in Func<T1, T2, T3, T4, T5, T6, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6) => func == null ? default : func(param1, param2, param3, param4, param5, param6);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7, in T8 param8) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7, param8);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7, in T8 param8, in T9 param9) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7, param8, param9);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7, in T8 param8, in T9 param9, in T10 param10) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7, in T8 param8, in T9 param9, in T10 param10, in T11 param11) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7, in T8 param8, in T9 param9, in T10 param10, in T11 param11, in T12 param12) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7, in T8 param8, in T9 param9, in T10 param10, in T11 param11, in T12 param12, in T13 param13) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7, in T8 param8, in T9 param9, in T10 param10, in T11 param11, in T12 param12, in T13 param13, in T14 param14) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7, in T8 param8, in T9 param9, in T10 param10, in T11 param11, in T12 param12, in T13 param13, in T14 param14, in T15 param15) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15);
        public static TResult GetValue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(in Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, in T1 param1, in T2 param2, in T3 param3, in T4 param4, in T5 param5, in T6 param6, in T7 param7, in T8 param8, in T9 param9, in T10 param10, in T11 param11, in T12 param12, in T13 param13, in T14 param14, in T15 param15, in T16 param16) => func == null ? default : func(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16);
        #endregion

        public static string GetAbsolutePath(string path) => Uri.IsWellFormedUriString(path, UriKind.Absolute) ? path : System.IO.Path.GetFullPath(path);

        // public static void PerformAction(in WinCopies.DotNetFix.IDisposable obj, in Action action);

        public static bool UpdateValue<T>(ref T value, in T newValue)
        {
            if (object.Equals(value, newValue))

                return false;

            value = newValue;

            return true;
        }

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, SystemMenuCommands nPosition, MenuFlags wFlags);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        public static void DeleteConsoleMenu(in System.Collections.Generic.IEnumerable<KeyValuePair<SystemMenuCommands, MenuFlags>> enumerable)
        {
            foreach (KeyValuePair<SystemMenuCommands, MenuFlags> item in enumerable)

                DeleteMenu(DesktopWindowManager.GetSystemMenu(GetConsoleWindow(), false), item.Key, item.Value);
        }

        public static void DeleteConsoleMenu(params KeyValuePair<SystemMenuCommands, MenuFlags>[] menus) => DeleteConsoleMenu((System.Collections.Generic.IEnumerable<KeyValuePair<SystemMenuCommands, MenuFlags>>)menus);

        //public static new KeyValuePair<TKey, TValue> GetKeyValuePair<TKey, TValue>(in TKey key, in TValue value) => new KeyValuePair<TKey, TValue>(key, value);
    }
}

namespace WinCopies.Commands.TEMP
{
    public static class ApplicationCommands
    {
        public static RoutedUICommand Empty { get; } = new RoutedUICommand("Empty", nameof(Empty), typeof(ApplicationCommands));

        public static RoutedUICommand DeletePermanently { get; } = new RoutedUICommand("Delete permanently", nameof(DeletePermanently), typeof(ApplicationCommands), new InputGestureCollection() { new System.Windows.Input.KeyGesture(Key.Delete, ModifierKeys.Alt) });
    }
}
#endif
