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
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;

namespace WinCopies.Temp
{
    public static class Temp
    {
        public static RoutedUICommand BrowseParent { get; } = new RoutedUICommand("Browse to parent", nameof(BrowseParent), typeof(Temp), new InputGestureCollection() { new KeyGesture(Key.Up, ModifierKeys.Alt) });

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
