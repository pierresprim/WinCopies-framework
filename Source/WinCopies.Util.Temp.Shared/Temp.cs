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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using WinCopies.Linq;

namespace WinCopies.Temp
{
    //public interface IMinimalLinkedList<T> : System.Collections.Generic.IEnumerable<T>
    //{
    //    IReadOnlyLinkedListNodeBase<T> Add(T item);

    //    void Remove(IReadOnlyLinkedListNodeBase<T> item);
    //}

    //public interface IMinimalList<T> : System.Collections.Generic.IEnumerable<T>
    //{
    //    void Add(T item);

    //    void RemoveAt(int index);
    //}

    //public interface IIntMinimalLinkedList<T> : IMinimalLinkedList<T>, ICountable
    //{

    //}

    //public interface IIntMinimalList<T> : IMinimalList<T>, ICountable
    //{

    //}

    //public interface IUIntMinimalLinkedList<T> : IMinimalLinkedList<T>, IUIntCountable
    //{

    //}

    //public interface IUIntMinimalList<T> : IMinimalList<T>, IUIntCountable
    //{

    //}

    //public struct BoolArray
    //{
    //    private const byte And = 1;
    //    private byte _b;

    //    public bool this[byte index]
    //    {
    //        get => index < 8 ? GetAt(index) : throw new ArgumentOutOfRangeException(nameof(index));

    //        set
    //        {
    //            if (GetAt(index) == value)

    //                return;

    //            _b ^= (byte)((index < 8 ? And : throw new ArgumentOutOfRangeException(nameof(index))) << index);
    //        }
    //    }

    //    public BoolArray(in byte b) => _b = b;

    //    private bool GetAt(in byte index) => ((_b >> index) & And) == And;

    //    public static explicit operator byte(BoolArray b) => b._b;
    //}

    public interface IItemsChangedEventArgs
    {
        IEnumerable OldItems { get; }

        IEnumerable NewItems { get; }
    }

    public abstract class ItemsChangedAbstractEventArgs<T>
    {
        public T OldItems { get; }

        public T NewItems { get; }

        public ItemsChangedAbstractEventArgs(in T oldItems, in T newItems)
        {
            OldItems = oldItems;

            NewItems = newItems;
        }
    }

    public class ItemsChangedEventArgs : ItemsChangedAbstractEventArgs<IEnumerable>, IItemsChangedEventArgs
    {
        public ItemsChangedEventArgs(in IEnumerable oldItems, in IEnumerable newItems) : base(oldItems, newItems) { /* Left empty. */ }
    }

    public class ItemsChangedEventArgs<T> : ItemsChangedAbstractEventArgs<IEnumerable<T>>, IItemsChangedEventArgs
    {
        IEnumerable IItemsChangedEventArgs.OldItems => OldItems;

        IEnumerable IItemsChangedEventArgs.NewItems => NewItems;

        public ItemsChangedEventArgs(in IEnumerable<T> oldItems, in IEnumerable<T> newItems) : base(oldItems, newItems) { /* Left empty. */ }
    }

    public delegate void ItemsChangedEventHandler(object sender, ItemsChangedEventArgs e);

    public delegate void ItemsChangedEventHandler<T>(object sender, ItemsChangedEventArgs<T> e);

    public class StreamInfo : System.IO.Stream, DotNetFix.IDisposable
    {
        protected System.IO.Stream Stream { get; }

        public override bool CanRead => Stream.CanRead;

        public override bool CanSeek => Stream.CanSeek;

        public override bool CanWrite => Stream.CanWrite;

        public override long Length => Stream.Length;

        public override long Position { get => Stream.Position; set => Stream.Position = value; }

        public bool IsDisposed { get; private set; }

        public StreamInfo(in System.IO.Stream stream) => Stream = stream ?? throw ThrowHelper.GetArgumentNullException(nameof(stream));

        public override void Flush() => Stream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => Stream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => Stream.Seek(offset, origin);

        public override void SetLength(long value) => Stream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => Stream.Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            IsDisposed = true;
        }
    }

    public static class Temp
    {
        private static void _RunAction<T>(in System.Collections.Generic.IEnumerable<T> enumerable, in Action<T> action)
        {
            foreach (var item in enumerable)

                action(item);
        }

        public static void RunActionIfNotNull<T>(in System.Collections.Generic.IEnumerable<T> enumerable, in Action<T> action)
        {
            if (enumerable == null)

                return;

            _RunAction(enumerable, action);
        }

        public static void RunActionIfNotNull<T>(in System.Collections.IEnumerable enumerable, in Action<T> action)
        {
            if (enumerable == null)

                return;

            _RunAction(enumerable.To<T>(), action);
        }
    }

    //    public class NativeShellMenu : IRecursiveEnumerable<NativeShellMenu>
    //    {
    //        public const RecursiveEnumerationOrder EnumerationOrder = RecursiveEnumerationOrder.ParentThenChildren;

    //        public IntPtr Menu { get; }

    //        public uint Id { get; private set; }

    //        public int Position { get; private set; }

    //        public int Count { get; }

    //        public NativeShellMenu Value => this;

    //        public NativeShellMenu(in IntPtr menu) => Count = Menus.GetMenuItemCount(Menu);

    //        public RecursiveEnumerator<NativeShellMenu> GetEnumerator() => new RecursiveEnumerator<NativeShellMenu>(this, EnumerationOrder);

    //        System.Collections.Generic.IEnumerator<NativeShellMenu> System.Collections.Generic.IEnumerable<NativeShellMenu>.GetEnumerator() => GetEnumerator();

    //        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //        public System.Collections.Generic.IEnumerator<IRecursiveEnumerable<NativeShellMenu>> GetRecursiveEnumerator() => new Enumerator(this);

    //        public sealed class Enumerator : Enumerator<IRecursiveEnumerable<NativeShellMenu>>
    //        {
    //            private NativeShellMenu _menu;
    //            private NativeShellMenu _current;

    //            public int CurrentIndex { get; private set; } = -1;

    //            public override bool? IsResetSupported => true;

    //            protected override IRecursiveEnumerable<NativeShellMenu> CurrentOverride => _current;

    //            public Enumerator(in NativeShellMenu menu) => _menu = menu;

    //            protected override bool MoveNextOverride()
    //            {
    //                if (++CurrentIndex == _menu.Count)

    //                    return false;

    //                _current = new NativeShellMenu(Menus.GetSubMenu(_menu.Menu, CurrentIndex))
    //                {
    //                    Id = (uint)Menus.GetMenuItemID(_menu.Menu, CurrentIndex),

    //                    Position = CurrentIndex
    //                };

    //                return true;
    //            }

    //            protected override void ResetCurrent()
    //            {
    //                base.ResetCurrent();

    //                CurrentIndex = -1;

    //                _current = null;
    //            }

    //            protected override void DisposeManaged()
    //            {
    //                _menu = null;

    //                base.DisposeManaged();
    //            }
    //        }
    //    }

    //    public interface ISeparator
    //    {

    //    }

    //    public class Separator : ISeparator
    //    {

    //    }

    //    public class NativeMenuItem<TItems, TItemCollection> : Command<uint> where TItemCollection : System.Collections.Generic.IEnumerable<TItems>
    //    {
    //        public struct NativeMenuItemConverters
    //        {
    //            public Converter<System.Collections.Generic.IEnumerable<TItems>, TItemCollection> ItemCollectionConverter { get; }

    //            public Converter<ISeparator, TItems> SeparatorConverter { get; }

    //            public Converter<NativeMenuItem<TItems, TItemCollection>, TItems> ItemConverter { get; }

    //            public NativeMenuItemConverters(in Converter<System.Collections.Generic.IEnumerable<TItems>, TItemCollection> itemCollectionConverter, in Converter<ISeparator, TItems> separatorConverter, in Converter<NativeMenuItem<TItems, TItemCollection>, TItems> itemConverter)
    //            {
    //                ItemCollectionConverter = itemCollectionConverter;

    //                SeparatorConverter = separatorConverter;

    //                ItemConverter = itemConverter;
    //            }
    //        }

    //        public NativeMenuItemConverters _converters;
    //        public NativeShellMenu _shellMenu;
    //        private System.Collections.Generic.IEnumerable<TItems> _items;

    //        public ICommand<uint> Command { get; }

    //        public uint CommandParameter { get; }

    //        public System.Collections.Generic.IEnumerable<TItems> Items => _items
    //#if CS8
    //            ??=
    //#else
    //            ?? (_items =     
    //#endif
    //            GetItemCollection()
    //#if !CS8
    //            )
    //#endif
    //            ;

    //        protected NativeMenuItem(in NativeShellMenu shellMenu, in string header, in ICommand<uint> command, in uint commandParameter, in NativeMenuItemConverters converters) : base(header, null)
    //        {
    //            Command = command;

    //            CommandParameter = commandParameter;

    //            _converters = converters;

    //            _shellMenu = shellMenu;
    //        }

    //        public static TItems Create(in NativeShellMenu menu, in ICommand<uint> command, in NativeMenuItemConverters converters)
    //        {
    //            ThrowIfNull(menu, nameof(menu));
    //            ThrowIfNull(command, nameof(command));

    //            if (If(Or, Logical, Equal, null, converters.ItemConverter, converters.SeparatorConverter, converters.ItemCollectionConverter))

    //                throw new ArgumentException("One or more of the given converters is null.");

    //            var data = new MenuItemInfo() { cbSize = (uint)Marshal.SizeOf<MenuItemInfo>(), fMask = MenuItemInfoFlags.State | MenuItemInfoFlags.String | MenuItemInfoFlags.FType };

    //            GetMenuItemInfo(menu, ref data);

    //            MenuFlags menuFlags = (MenuFlags)data.fType;

    //            return menuFlags.HasFlag(MenuFlags.Separator)
    //                ? converters.SeparatorConverter(new Separator())
    //                : converters.ItemConverter(new NativeMenuItem<TItems, TItemCollection>(menu, data.dwTypeData, command, menu.Id, converters));
    //        }

    //        public static void GetMenuItemInfo(in NativeShellMenu shellMenu, ref MenuItemInfo data)
    //        {
    //            if (!Menus.GetMenuItemInfoW(shellMenu.Menu, (uint)shellMenu.Position, true, ref data))

    //                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    //        }

    //        public void GetMenuItemInfo(ref MenuItemInfo data) => GetMenuItemInfo(_shellMenu, ref data);

    //        protected TItemCollection GetItemCollection()
    //        {
    //            TItemCollection result = _converters.ItemCollectionConverter(new Enumerable<TItems>(() => _shellMenu.GetRecursiveEnumerator().SelectConverter(_menu => Create((NativeShellMenu)_menu, Command, _converters))));

    //            _converters = default;

    //            return result;
    //        }

    //        public override bool CanExecute(uint value)
    //        {
    //            var data = new MenuItemInfo() { cbSize = (uint)Marshal.SizeOf<MenuItemInfo>(), fMask = MenuItemInfoFlags.State };

    //            GetMenuItemInfo(ref data);

    //            return ((MenuStates)data.fState).HasFlag(MenuStates.Enabled);
    //        }

    //        public override void Execute(uint value) => Command?.Execute(CommandParameter);
    //    }
}

//using System;
//using System.Collections.Generic;

//using WinCopies.Collections;
//using WinCopies.Collections.DotNetFix.Generic;
//using WinCopies.Linq;

//using static WinCopies.Temp.Delegates;
//using static
//#if WinCopies3
//    WinCopies.ThrowHelper;
//#else
//    WinCopies.Util.Util;
//#endif

//namespace WinCopies.Temp
//{
//    public static class Delegates
//    {
//        public static void EmptyVoid<T>(T parameter) { }

//        public static TOut Null<TIn, TOut>(TIn parameter) where TOut : class => null;
//    }

//    public interface IActionDictionaryBase<TPredicate, TAction> : DotNetFix.IDisposable
//    {
//        TAction DefaultAction { get; }

//        void Push(Predicate<TPredicate> predicate, TAction action);
//    }

//    public abstract class ActionDictionaryBase<TPredicate, TAction>
//#if WinCopies3
//        : IActionDictionaryBase<TPredicate, TAction>
//#endif
//    {
//        private IEnumerableStack<KeyValuePair<Predicate<TPredicate>, TAction>> _stack;

//        protected
//#if WinCopies3
//            IEnumerableStack
//#else
//            EnumerableStack
//#endif
//            <KeyValuePair<Predicate<TPredicate>, TAction>> Stack => IsDisposed ? throw GetExceptionForDispose(false) : _stack;

//        protected abstract TAction
//#if WinCopies3
//            DefaultActionOverride
//#else
//DefaultSelectorOverride
//#endif
//        { get; }

//        public TAction
//#if WinCopies3
//            DefaultAction
//#else
//            DefaultSelector
//#endif
//            => IsDisposed ? throw GetExceptionForDispose(false) :
//#if WinCopies3
//            DefaultActionOverride;

//#else
//        DefaultSelectorOverride;
//#endif

//        public bool IsDisposed { get; private set; }

//        public ActionDictionaryBase(IEnumerableStack<KeyValuePair<Predicate<TPredicate>, TAction>> stack) => _stack = stack ?? throw GetArgumentNullException(nameof(stack));

//        public ActionDictionaryBase() : this(new EnumerableStack<KeyValuePair<Predicate<TPredicate>, TAction>>()) { }

//        public void Push(Predicate<TPredicate> predicate, TAction selector) => Stack.Push(new KeyValuePair<Predicate<TPredicate>, TAction>(predicate ?? throw GetArgumentNullException(nameof(predicate)), selector
//#if CS8
//            ??
//#else
//            == null ?
//#endif
//            throw GetArgumentNullException(nameof(selector))
//#if !CS8
//            : selector
//#endif
//            ));

//        /// <summary>
//        /// This method is called by <see cref="Dispose"/> and by the deconstructor. This overload does nothing and it is not necessary to call this base overload in derived classes.
//        /// </summary>
//        protected virtual void DisposeUnmanaged() { /* Left empty. */ }

//        protected virtual void DisposeManaged()
//        {
//            _stack.Clear();

//            _stack = null;

//            IsDisposed = true;
//        }

//        public void Dispose()
//        {
//            if (IsDisposed)

//                return;

//            DisposeUnmanaged();

//            DisposeManaged();

//            GC.SuppressFinalize(this);
//        }

//        ~ActionDictionaryBase() => DisposeUnmanaged();
//    }

//    public interface IActionDictionary<T> : IActionDictionaryBase<T, Action<T>>
//    {
//        void Run(T item);
//    }

//    public interface IEnumerableActionDictionary<T> : IActionDictionary<T>
//    {
//        void Run(System.Collections.Generic.IEnumerable<T> items);
//    }

//    public abstract class ActionDictionary<T> : ActionDictionaryBase<T, Action<T>>, IActionDictionary<T>
//    {
//        protected void _Run(T item)
//        {
//            foreach (KeyValuePair<Predicate<T>, Action<T>> _item in Stack)

//                if (_item.Key(item))
//                {
//                    _item.Value(item);

//                    return;
//                }

//#if WinCopies3
//            DefaultActionOverride
//#else
//            DefaultSelectorOverride
//#endif
//                (item);
//        }

//        public void Run(T item)
//        {
//            ThrowIfDisposed(this);

//            _Run(item
//#if CS8
//                ??
//#else
//                == null ?
//#endif
//                throw GetArgumentNullException(nameof(item))
//#if !CS8
//                : item
//#endif
//                );
//        }
//    }

//    public abstract class EnumerableActionDictionary<T> : ActionDictionary<T>, IEnumerableActionDictionary<T>
//    {
//        public void Run(System.Collections.Generic.IEnumerable<T> items)
//        {
//            ThrowIfDisposed(this);

//            foreach (T item in items)

//                (((IUIntCountable)Stack).Count == 0 ?
//#if WinCopies3
//            DefaultActionOverride
//#else
//            DefaultSelectorOverride
//#endif
//            : _Run)(item);
//        }
//    }

//    public class DefaultNullableValueActionDictionary<T> : ActionDictionary<T>
//    {
//        protected override Action<T>
//#if WinCopies3
//            DefaultActionOverride
//#else
//            DefaultSelectorOverride
//#endif
//            =>
//#if !WinCopies3
//            Util.
//#endif
//            EmptyVoid;
//    }

//    public class DefaultNullableValueEnumerableActionDictionary<T> : EnumerableActionDictionary<T>
//    {
//        protected override Action<T>
//#if WinCopies3
//            DefaultActionOverride
//#else
//            DefaultSelectorOverride
//#endif
//            =>
//#if !WinCopies3
//            Util.
//#endif
//            EmptyVoid;
//    }

//    public interface ISelectorDictionary<TIn, TOut> :
//#if WinCopies3
//        IActionDictionaryBase<TIn, Converter<TIn, TOut>>
//    {
//#else
//        Util.DotNetFix.IDisposable
//    {
//        void Push(Predicate<TIn> predicate, Converter<TIn, TOut> selector);

//        Converter<TIn, TOut> DefaultSelector { get; }
//#endif
//        TOut Select(TIn item);
//    }

//    public interface IEnumerableSelectorDictionary<TIn, TOut> : ISelectorDictionary<TIn, TOut>
//    {
//        System.Collections.Generic.IEnumerable<TOut> Select(System.Collections.Generic.IEnumerable<TIn> items);
//    }

//    public static class SelectorDictionary
//    {
//        public static ArgumentException GetInvalidItemException() => new ArgumentException("The given item or its current configuration is not supported.");
//    }

//    public abstract class SelectorDictionary<TIn, TOut> : ActionDictionaryBase<TIn, Converter<TIn, TOut>>, ISelectorDictionary<TIn, TOut>
//    {
//        protected TOut _Select(TIn item)
//        {
//            foreach (KeyValuePair<Predicate<TIn>, Converter<TIn, TOut>> _item in Stack)

//                if (_item.Key(item))

//                    return _item.Value(item);

//            return
//#if WinCopies3
//                DefaultActionOverride
//#else
//                DefaultSelectorOverride
//#endif
//                (item);
//        }

//        public TOut Select(TIn item)
//        {
//            ThrowIfDisposed(this);

//            return _Select(item
//#if CS8
//                ??
//#else
//                == null ?
//#endif
//                throw GetArgumentNullException(nameof(item))
//#if !CS8
//                : item
//#endif
//                );
//        }
//    }

//    public abstract class EnumerableSelectorDictionary<TIn, TOut> : SelectorDictionary<TIn, TOut>, IEnumerableSelectorDictionary<TIn, TOut>
//    {
//        public System.Collections.Generic.IEnumerable<TOut> Select(System.Collections.Generic.IEnumerable<TIn> items) => IsDisposed ? throw GetExceptionForDispose(false) : items.
//#if WinCopies3
//            SelectConverter
//#else
//            Select
//#endif
//            (((IUIntCountable)Stack).Count == 0 ?
//#if WinCopies3
//            DefaultActionOverride
//#else
//            DefaultSelectorOverride
//#endif
//            : _Select);
//    }

//    public class DefaultNullableValueSelectorDictionary<TIn, TOut> : SelectorDictionary<TIn, TOut> where TOut : class
//    {
//        protected override Converter<TIn, TOut>
//#if WinCopies3
//            DefaultActionOverride
//#else
//            DefaultSelectorOverride
//#endif
//            => Null<TIn, TOut>;
//    }

//    public class DefaultNullableValueEnumerableSelectorDictionary<TIn, TOut> : EnumerableSelectorDictionary<TIn, TOut> where TOut : class
//    {
//        protected override Converter<TIn, TOut>
//#if WinCopies3
//            DefaultActionOverride
//#else
//            DefaultSelectorOverride
//#endif
//            =>            Null<TIn, TOut>;
//    }
//}
#endif
