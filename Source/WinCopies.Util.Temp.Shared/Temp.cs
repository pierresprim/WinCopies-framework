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
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;

namespace WinCopies.Temp
{
    public struct ConversionStruct<TIn, TOut>
    {
        public Converter<TIn, TOut> Converter { get; }

        public Converter<TOut, TIn> BackConverter { get; }

        public ConversionStruct(in Converter<TIn, TOut> converter, Converter<TOut, TIn> backConverter)
        {
            Converter = converter;

            BackConverter = backConverter;
        }
    }

    public class Conversion<TIn, TOut>
    {
        public Converter<TIn, TOut> Converter { get; }

        public Converter<TOut, TIn> BackConverter { get; }

        public Conversion(in Converter<TIn, TOut> converter, Converter<TOut, TIn> backConverter)
        {
            Converter = converter;

            BackConverter = backConverter;
        }
    }

    public interface ICommandSource<T>:ICommandSource
    {
        new T CommandParameter { get; }

#if CS8
        object ICommandSource.CommandParameter => CommandParameter;
#endif
    }

    [Flags]
    public enum MenuFlags : uint
    {
        Insert = 0x00000000,
        Change = 0x00000080,
        Append = 0x00000100,
        Delete = 0x00000200,
        Remove = 0x00001000,

        ByCommand = 0x00000000,
        ByPosition = 0x00000400,

        Separator = 0x00000800,

        Enabled = 0x00000000,
        Grayed = 0x00000001,
        Disabled = 0x00000002,

        Unchecked = 0x00000000,
        Checked = 0x00000008,
        UseCheckBitmaps = 0x00000200,

        String = 0x00000000,
        Bitmap = 0x00000004,
        OwnerDraw = 0x00000100,

        Popup = 0x00000010,
        MenuBarBreak = 0x00000020,
        MenuBreak = 0x00000040,

        Unhilite = 0x00000000,
        Hilite = 0x00000080,

        Default = 0x00001000,

        SystemMenu = 0x00002000,
        Help = 0x00004000,

        RightJustify = 0x00004000,



        MouseSelect = 0x00008000,

        End = 0x00000080,  /* Obsolete -- only used by old RES files */



        RadioCheck = 0x00000200,
        RightOrder = 0x00002000,

        /* Menu flags for Add/Check/EnableMenuItem() */
        MFS_Grayed = 0x00000003,
        MFS_Disabled = MFS_Grayed,
    }

    [Flags]
    public enum SystemMenuCommands : uint
    {
        Size = 0xF000,
        Move = 0xF010,
        Minimize = 0xF020,
        Maximize = 0xF030,
        NextWindow = 0xF040,
        PrevWindow = 0xF050,
        Close = 0xF060,
        VScroll = 0xF070,
        HScroll = 0xF080,
        MouseMenu = 0xF090,
        KeyMenu = 0xF100,
        Arrange = 0xF110,
        Restore = 0xF120,
        TaskList = 0xF130,
        ScreenSave = 0xF140,
        HotKey = 0xF150,
        Default = 0xF160,
        MonitorPower = 0xF170,
        ContextHelp = 0xF180,
        Separator = 0xF00F,

        IsSecure = 0x00000001,

        // Obsolete names
        Icon = Minimize,
        Zoom = Maximize
    }

    public class Enumerator<T> : WinCopies.Collections.Generic. Enumerator<ILinkedListNode<T>>
    {
        private ILinkedList<T> _list;
        private Action _action;
        private Func<bool> _moveNext;
        private ILinkedListNode<T> _currentNode;
        private Action _reset;
        private readonly uint _version;

        protected ILinkedList<T> List => GetOrThrowIfDisposed(_list);

        public EnumerationDirection EnumerationDirection { get; }

        public ILinkedListNode<T> Start { get; }

        public ILinkedListNode<T> End { get; }

        public override bool? IsResetSupported => true;

        /// <summary>
        /// When overridden in a derived class, gets the element in the collection at the current position of the enumerator.
        /// </summary>
        protected override ILinkedListNode<T> CurrentOverride => _currentNode;

        public Enumerator(in ILinkedList<T> list, in EnumerationDirection enumerationDirection, in ILinkedListNode<T> start = null, in ILinkedListNode<T> end = null)
        {
            // todo: (list ?? throw GetArgumentNullException(nameof(list)))._enumeratorsCount++;

            _list = list;

            // todo: _version = list.EnumerableVersion;

            EnumerationDirection = enumerationDirection;

            Start = start;

            End = end;

            switch (enumerationDirection)
            {
                case EnumerationDirection.FIFO:

                    _action = () => _currentNode = _currentNode.Next;

                    _reset = () => _currentNode = Start ?? _list.First;

                    break;

                case EnumerationDirection.LIFO:

                    _action = () => _currentNode = _currentNode.Previous;

                    _reset = () => _currentNode = Start ?? _list.Last;

                    break;
            }

            ResetMoveNext();
        }

        protected void ResetMoveNext() => _moveNext = () =>
        {
            if (_list.First == null)

                return false;

            _reset();

            if (End == null)

                _moveNext = () =>
                {
                    _action();

                    return _MoveNext();
                };

            _moveNext = () =>
            {
                if (_currentNode == End)

                    return false;

                _action();

                return _MoveNext();
            };

            return true;
        };

        private bool _MoveNext()
        {
            if (_currentNode == null)
            {
                ResetCurrent();

                return false;
            }

            // The new node has already been updated in the _action delegate.

            return true;
        }

        protected override void ResetOverride()
        {
            base.ResetOverride();

            // todo: ThrowIfVersionHasChanged(_list.EnumerableVersion, _version);

            _reset();

            ResetMoveNext();
        }

        protected override bool MoveNextOverride()
        {
            // todo: ThrowIfVersionHasChanged(_list.EnumerableVersion, _version);

            return _moveNext();
        }

        protected override void ResetCurrent() => _currentNode = null;

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _action = null;

            ResetCurrent();

            _reset = null;
        }

        protected override void Dispose(bool disposing)
        {
            // todo: _list.DecrementEnumeratorsCount();

            _list = null;

            base.Dispose(disposing);
        }

        ~Enumerator() => Dispose(false);
    }

    public class UIntCountableEnumeratorInfo<T> : Enumerator<T>, IUIntCountableEnumeratorInfo<ILinkedListNode<T>>
    {
        public uint Count => List.Count;

        public UIntCountableEnumeratorInfo(in LinkedList<T> list, in EnumerationDirection enumerationDirection) : base(list, enumerationDirection, null, null)
        {
            // Left empty.
        }
    }

    public static class Temp
    {
        public static System.Collections.Generic.IEnumerator<ILinkedListNode<T>> GetNodeEnumerator<T>(in ILinkedList<T> list, in EnumerationDirection enumerationDirection, in ILinkedListNode<T> start, ILinkedListNode<T> end) => new Enumerator<T>(list, enumerationDirection, start, end);

        public static int GetSystemCommandWParam(in IntPtr wParam) => (int)wParam & 0xFFF0;

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu([In] IntPtr hWnd, [In, MarshalAs(UnmanagedType.Bool)] bool bRevert);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableMenuItem([In] IntPtr hMenu, [In, MarshalAs(UnmanagedType.U4)] SystemMenuCommands uIDEnableItem, [In, MarshalAs(UnmanagedType.U4)] MenuFlags uEnable);
        public static bool EnableCloseMenuItemFromMenuHandle(in IntPtr hMenu) => EnableMenuItem(hMenu, SystemMenuCommands.Close, MenuFlags.ByCommand | MenuFlags.Enabled);

        public static bool EnableCloseMenuItemFromWindowHandle(in IntPtr hwnd)
        {
            IntPtr hMenu = GetSystemMenu(hwnd, false);

            return hMenu != IntPtr.Zero && EnableCloseMenuItemFromMenuHandle(hMenu);
        }

        public static bool EnableCloseMenuItem(in System.Windows.Window window) => EnableCloseMenuItemFromWindowHandle(new WindowInteropHelper(window).Handle);

        public static bool DisableCloseMenuItemFromMenuHandle(in IntPtr hMenu) => EnableMenuItem(hMenu, SystemMenuCommands.Close, MenuFlags.ByCommand | MenuFlags.Grayed);

        public static bool DisableCloseMenuItemFromWindowHandle(in IntPtr hwnd)
        {
            IntPtr hMenu = GetSystemMenu(hwnd, false);

            return hMenu != IntPtr.Zero && DisableCloseMenuItemFromMenuHandle(hMenu);
        }

        public static bool DisableCloseMenuItem(in System.Windows.Window window) => DisableCloseMenuItemFromWindowHandle(new WindowInteropHelper(window).Handle);

        public static DependencyProperty Register<TValue, TOwnerType>(in string propertyName) => DependencyProperty.Register(propertyName, typeof(TValue), typeof(TOwnerType));
    }
}
#endif
