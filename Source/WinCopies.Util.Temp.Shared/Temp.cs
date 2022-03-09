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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WinCopies.Linq;

using static WinCopies.ThrowHelper;
using static WinCopies.Temp.ForLoop;
using System.Reflection;
using System.Globalization;

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

    public class ItemsChangedEventArgs<T> : ItemsChangedAbstractEventArgs<System.Collections.Generic.IEnumerable<T>>, IItemsChangedEventArgs
    {
        IEnumerable IItemsChangedEventArgs.OldItems => OldItems;

        IEnumerable IItemsChangedEventArgs.NewItems => NewItems;

        public ItemsChangedEventArgs(in System.Collections.Generic.IEnumerable<T> oldItems, in System.Collections.Generic.IEnumerable<T> newItems) : base(oldItems, newItems) { /* Left empty. */ }
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

        public StreamInfo(in System.IO.Stream stream) => Stream = stream ?? throw GetArgumentNullException(nameof(stream));

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

    public class BooleanEventArgs : EventArgs
    {
        public bool Value { get; }

        public BooleanEventArgs(in bool value) => Value = value;
    }

    public interface IForLoopAction : DotNetFix.IDisposable
    {
        void Loop(int index, int count, Func<int, bool> action);
    }

    public interface IForLoopFunc<T> : DotNetFix.IDisposable
    {
        T Loop(int index, int count, FuncOut<int, T, bool> func, out bool ok);
    }

    internal class _ForLoop : DotNetFix.IDisposable
    {
        protected class PredicateClass
        {
            private readonly FuncIn<int, bool> _defaultPredicate;

            public bool Continue { get; set; } = true;

            public FuncIn<int, bool> Predicate { get; private set; }

            public PredicateClass(FuncIn<int, bool> defaultPredicate)
            {
                _defaultPredicate = defaultPredicate;

                Predicate = (in int i) =>
                {
                    Predicate = (in int _i) => Continue && _defaultPredicate(_i);

                    return _defaultPredicate(i);
                };
            }
        }

        protected FuncIn<int, int, bool> Predicate { get; private set; }

        protected ActionRef<int> Updater { get; private set; }

        public bool IsDisposed => Updater == null;

        public _ForLoop(in FuncIn<int, int, bool> predicate, in ActionRef<int> updater)
        {
            Predicate = predicate;
            Updater = updater;
        }

        protected PredicateClass GetPredicate(int count) => new
#if !CS9
            PredicateClass
#endif
            ((in int index) => Predicate(index, count));

        public void Dispose() => Updater = null;
    }

    internal class ForLoopAction : _ForLoop, IForLoopAction
    {
        public ForLoopAction(in FuncIn<int, int, bool> predicate, in ActionRef<int> updater) : base(predicate, updater) { /* Left empty. */ }

        public void Loop(int index, int count, Func<int, bool> action)
        {
            ThrowIfNull(action, nameof(action));
            ThrowIfDisposed(this);

            PredicateClass predicate = GetPredicate(count);

            for (; predicate.Predicate(index); Updater(ref index))

                predicate.Continue = action(index);
        }
    }

    internal class ForLoopFunc<T> : _ForLoop, IForLoopFunc<T>
    {
        public ForLoopFunc(in FuncIn<int, int, bool> predicate, in ActionRef<int> updater) : base(predicate, updater) { /* Left empty. */ }

        public T Loop(int index, int count, FuncOut<int, T, bool> func, out bool ok)
        {
            ThrowIfNull(func, nameof(func));
            ThrowIfDisposed(this);

            PredicateClass predicate = GetPredicate(count);
            T item = default;

            for (; predicate.Predicate(index); Updater(ref index))

                predicate.Continue = func(index, out item);

            // We can't just return item because it would be possible that func did set item to a value different from the default one and returned true at the end.

            return (ok = !predicate.Continue) ? item : default;
        }
    }

    public static class ForLoop
    {
        private static void LoopAction(in int index, in int count, in Func<int, bool> action, in Func<IForLoopAction> forLoopFunc) => forLoopFunc().Loop(index, count, action);

        public static IForLoopAction GetForLoopActionASC() => new ForLoopAction((in int _index, in int _count) => _index < _count, (ref int _index) => _index++);

        public static void LoopActionASC(in int index, in int count, in Func<int, bool> action) => LoopAction(index, count, action, GetForLoopActionASC);

        public static IForLoopAction GetForLoopActionDESC() => new ForLoopAction((in int _index, in int _count) => _index >= _count, (ref int _index) => _index--);

        public static void LoopActionDESC(in int index, in int count, in Func<int, bool> action) => LoopAction(index, count, action, GetForLoopActionDESC);



        private static T LoopFunc<T>(in int index, in int count, out bool ok, in FuncOut<int, T, bool> func, in Func<IForLoopFunc<T>> forLoopFunc) => forLoopFunc().Loop(index, count, func, out ok);

        public static IForLoopFunc<T> GetForLoopFuncASC<T>() => new ForLoopFunc<T>((in int _index, in int _count) => _index < _count, (ref int _index) => _index++);

        public static T LoopFuncASC<T>(in int index, in int count, out bool ok, in FuncOut<int, T, bool> func) => LoopFunc(index, count, out ok, func, GetForLoopFuncASC<T>);

        public static IForLoopFunc<T> GetForLoopFuncDESC<T>() => new ForLoopFunc<T>((in int _index, in int _count) => _index >= _count, (ref int _index) => _index--);

        public static T LoopFuncDESC<T>(in int index, in int count, out bool ok, in FuncOut<int, T, bool> func) => LoopFunc(index, count, out ok, func, GetForLoopFuncDESC<T>);
    }

    public static class Extensions
    {
        public static ConstructorInfo? TryGetConstructor(this Type t, params Type[] types) => t.GetConstructor(types);

        public static ConstructorInfo AssertGetConstructor(this Type t, params Type[] types) => t.TryGetConstructor(types) ?? throw new InvalidOperationException("There is no such constructor for this type.");

        public static System.Collections.Generic.IEnumerable<T> AsReadOnlyEnumerable<T>(this System.Collections.Generic.IEnumerable<T> enumerable)
        {
            foreach (T? item in enumerable)

                yield return item;
        }

        public static System.Collections.Generic.IEnumerable<U> As<T, U>(this System.Collections.Generic.IEnumerable<T> enumerable) where T : U
        {
            foreach (T? item in enumerable)

                yield return item;
        }

        public static bool Any(this IEnumerable enumerable, Func<object, bool> func)
        {
            ThrowIfNull(enumerable, nameof(enumerable));
            ThrowIfNull(func, nameof(func));

            foreach (object value in enumerable)

                if (func(value))

                    return true;

            return false;
        }

        public static bool AnyPredicate(this IEnumerable enumerable, Predicate func)
        {
            ThrowIfNull(enumerable, nameof(enumerable));
            ThrowIfNull(func, nameof(func));

            foreach (object value in enumerable)

                if (func(value))

                    return true;

            return false;
        }

        public static bool Any<T>(this IEnumerable enumerable) => enumerable.Any(item => item is T);

        public static bool AllPredicate<T>(this System.Collections.Generic.IEnumerable<T> source, Predicate<T> predicate)
        {
            foreach (T item in source)

                if (!predicate(item))

                    return false;

            return true;
        }

        public static bool AnyPredicate<T>(this System.Collections.Generic.IEnumerable<T> source, Predicate<T> predicate)
        {
            foreach (T item in source)

                if (predicate(item))

                    return true;

            return false;
        }


        /*private static System.Collections.Generic.IEnumerable<PropertyInfo> _GetAllProperties(this Type t, Predicate<Type?> predicate, bool include)
        {
            Type? type = t;

            System.Collections.Generic.IEnumerable<PropertyInfo> loop() => type.GetProperties();

            do
            {
                foreach (PropertyInfo p in loop())

                    yield return p;

                type = type.BaseType;
            }
            while (type != null && predicate(type));

            if (include && type != null)

                foreach (PropertyInfo p in loop())

                    yield return p;
        }

        public static System.Collections.Generic.IEnumerable<PropertyInfo> GetAllProperties(this Type t, Predicate<Type?> predicate, bool include = true) => (t ?? throw GetArgumentNullException(nameof(t))).GetAllProperties(predicate ?? throw GetArgumentNullException(nameof(predicate)), include);

        private static System.Collections.Generic.IEnumerable<PropertyInfo> __GetAllProperties(this Type t, Type u, bool include) => t._GetAllProperties(t => t != u, include);

        internal static System.Collections.Generic.IEnumerable<PropertyInfo> _GetAllProperties(this Type t, in Type u, in bool include) => t == u ? include ? t.GetProperties() : Enumerable.Empty<PropertyInfo>() : t.__GetAllProperties(u, include);

        internal static System.Collections.Generic.IEnumerable<PropertyInfo> GetAllProperties(this Type t, in Type u, in string tName, in string uName, in bool include = true) => (t ?? throw GetArgumentNullException(nameof(t))).IsAssignableTo(u ?? throw GetArgumentNullException(nameof(u))) ? t._GetAllProperties(u, include) : throw new ArgumentException($"{tName} must inherit from {uName}.");

        public static System.Collections.Generic.IEnumerable<PropertyInfo> GetAllProperties(this Type t, in Type u, in bool include = true) => t.GetAllProperties(u, nameof(t), nameof(u), include);

        public static System.Collections.Generic.IEnumerable<PropertyInfo> GetAllProperties<U>(this Type t, in bool include = true) => t.GetAllProperties(typeof(U), nameof(t), nameof(U), include);*/

        public static T? FirstOrNull<T>(this System.Collections.Generic.IEnumerable<T> enumerable) where T : struct
        {
            foreach (T item in enumerable)

                return item;

            return null;
        }

        public static T? FirstOrNull<T>(this System.Collections.Generic.IEnumerable<T> enumerable, Func<T, bool> predicate) where T : struct
        {
            ThrowIfNull(predicate, nameof(predicate));

            foreach (T item in enumerable)

                if (predicate(item))

                    return item;

            return null;
        }

        public static T? FirstOrNullPredicate<T>(this System.Collections.Generic.IEnumerable<T> enumerable, Predicate<T> predicate) where T : struct
        {
            ThrowIfNull(predicate, nameof(predicate));

            foreach (T item in enumerable)

                if (predicate(item))

                    return item;

            return null;
        }

        public static TResult? FirstOrNull<TItems, TResult>(this System.Collections.Generic.IEnumerable<TItems> enumerable) where TResult : struct
        {
            foreach (TItems item in enumerable)

                if (item is TResult result)

                    return result;

            return null;
        }

        public static string Surround(this string? value, in char left, in char right) => Surround(value, left.ToString(), right.ToString());
        public static string Surround(this string? value, in char decorator) => Surround(value, decorator.ToString());
        public static string Surround(this string? value, in string? left, in string? right) => $"{left}{value}{right}";
        public static string Surround(this string? value, in string? decorator) => Surround(value, decorator, decorator);

        private static string? FirstCharTo(this string? value, Converter<char, char> charConverter, Converter<string, string> stringConverter) => value == null ? null : value.Length > 1 ? charConverter(value[0]) + value[1..] : stringConverter(value);
        public static string? FirstCharToLower(this string? value) => value.FirstCharTo(c => char.ToLower(c), s => s.ToLower());
        public static string? FirstCharToLowerInvariant(this string? value) => value.FirstCharTo(c => char.ToLowerInvariant(c), s => s.ToLowerInvariant());
        public static string? FirstCharToLower(this string? value, CultureInfo culture) => value.FirstCharTo(c => char.ToLower(c, culture), s => s.ToLower(culture));

        public static string? FirstCharToUpper(this string? value) => value.FirstCharTo(c => char.ToLower(c), s => s.ToUpper());
        public static string? FirstCharToUpperInvariant(this string? value) => value.FirstCharTo(c => char.ToUpperInvariant(c), s => s.ToUpperInvariant());
        public static string? FirstCharToUpper(this string? value, CultureInfo culture) => value.FirstCharTo(c => char.ToUpper(c, culture), s => s.ToUpper(culture));

        private static string FirstCharOfEachWordToUpper(this string s, in Converter<char, char> converter, params char[] separators)
        {
            string[] text = s.Split(separators);

            char[] c = new char[s.Length];

            int _j;

            string _text;

            for (int i = 0, j = 0; i < text.Length; i++)
            {
                _text = text[i];

                c[j] = converter(_text[0]);

                for (j++, _j = 1; _j < _text.Length; j++, _j++)

                    c[j] = _text[_j];
            }

            return new string(c);
        }
        public static string FirstCharOfEachWordToUpper(this string s, params char[] separators) => s.FirstCharOfEachWordToUpper(c => char.ToUpper(c), separators);
        public static string FirstCharOfEachWordToUpperInvariant(this string s, params char[] separators) => s.FirstCharOfEachWordToUpper(c => char.ToUpperInvariant(c), separators);
        public static string FirstCharOfEachWordToUpper(this string s, CultureInfo culture, params char[] separators) => s.FirstCharOfEachWordToUpper(c => char.ToUpper(c, culture), separators);

        public static string Reverse(this string s)
        {
            char[] c = new char[s.Length];

            for (int i = 0; i < s.Length; i++)

                c[i] = s[s.Length - i - 1];

            return new string(c);
        }

        public static System.Collections.Generic.IEnumerable<TOut> ForEach<TIn, TOut>(this System.Collections.Generic.IEnumerable<TIn> enumerable, Func<TIn, System.Collections.Generic.IEnumerable<TOut>> func)
        {
            foreach (TIn? item in enumerable)

                foreach (TOut? _item in func(item))

                    yield return _item;
        }

        public static bool IsAssignableFrom<T>(this Type t) => t.IsAssignableFrom(typeof(T));

        public static bool IsAssignableTo<T>(this Type t) => t.IsAssignableTo(typeof(T));

        public static T GetChild<T>(this DependencyObject parent, in bool lookForDirectChildOnly, out bool isDirectChild) where T : Visual
        {
            T child = default;
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                var visual = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = visual as T;

                if (child == null)
                {
                    if (!lookForDirectChildOnly && child == null)

                        child = GetChild<T>(visual, false, out _);
                }

                else
                {
                    isDirectChild = true;

                    return child;
                }
            }

            isDirectChild = false;

            return child;
        }

        public static Panel GetItemsPanel(this DependencyObject itemsControl)
        {
            ItemsPresenter itemsPresenter = itemsControl.GetChild<ItemsPresenter>(false, out _);

            return itemsPresenter == null ? null : VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;
        }

        public static bool HasHorizontalOrientation(this Panel panel) => panel.HasLogicalOrientationPublic && panel.LogicalOrientationPublic == Orientation.Horizontal;

        public static bool HasVerticalOrientation(this Panel panel) => panel.HasLogicalOrientationPublic && panel.LogicalOrientationPublic == Orientation.Vertical;

        private struct TryGetFirstParams
        {
            public int Index { get; }

            public int Count { get; }

            public TryGetFirstParams(in int index, in int count)
            {
                Index = index;
                Count = count;
            }
        }

        private static bool TryGet<T>(in IList itemCollection, in int i, ref bool checkContent, out T item) where T : Visual
        {
#if CS8
            static
#endif
                bool onItemFound(in T __item, out T ___item)
            {
                ___item = __item;

                return false;
            }

            object obj = itemCollection[i];

            if (obj is T _item)

                return onItemFound(_item, out item);

            else if (checkContent && obj is ContentPresenter contentPresenter)
            {
                _item = contentPresenter.GetChild<T>(true, out _);

                if (_item != null)
                {
                    checkContent = true;

                    return onItemFound(_item, out item);
                }
            }

            item = default;

            return true;
        }

        private delegate T LoopFunc<T>(in int index, in int count, out bool ok, in FuncOut<int, T, bool> func);

        private static T TryGetFirst<T>(IList itemCollection, in TryGetFirstParams initParams, in TryGetFirstParams? rollBackParams, out bool rollBack, ref bool checkContent, LoopFunc<T> loopFunc, out bool found) where T : Visual
        {
            T loop(in TryGetFirstParams @params, ref bool _checkContent, out bool _found)
            {
                bool __checkContent = _checkContent;

                T result = loopFunc(@params.Index, @params.Count, out _found, (int i, out T _item) => TryGet(itemCollection, i, ref __checkContent, out _item));

                _checkContent = __checkContent;

                return result;
            }

            T item = loop(initParams, ref checkContent, out found);

            rollBack = false;

            if (!found && rollBackParams.HasValue)
            {
                item = loop(rollBackParams.Value, ref checkContent, out found);

                rollBack = true;
            }

            return item;
        }

        private struct TGFParams
        {
            public FuncIn<int, TryGetFirstParams> GetInitParams { get; }

            public FuncIn<int, TryGetFirstParams> GetRollBackParams { get; }

            public TGFParams(in int index, in FuncIn<int, int, TryGetFirstParams> getInitParams, in FuncIn<int, int, TryGetFirstParams> getRollBackParams)
            {
                GetInitParams = GetTGFParamsFunc(index, getInitParams);
                GetRollBackParams = GetTGFParamsFunc(index, getRollBackParams);
            }

            private static FuncIn<int, TryGetFirstParams> GetTGFParamsFunc(int index, FuncIn<int, int, TryGetFirstParams> func) => (in int count) => func(index, count);
        }

        private static TOut TryGetFirst<TIn, TOut>(in TIn itemsControl, in FuncIn<TIn, IList> func, in TGFParams @params, ref bool rollBack, ref bool checkContent, in LoopFunc<TOut> loopFunc, out bool found) where TOut : Visual
        {
            if (itemsControl == null)

                throw GetArgumentNullException(nameof(itemsControl));

            IList itemCollection = func(itemsControl);

            int count = itemCollection.Count;

            TryGetFirstParams? _params;

            if (rollBack)

                _params = @params.GetRollBackParams(count);

            else

                _params = null;

            return TryGetFirst(itemCollection, @params.GetInitParams(count), _params, out rollBack, ref checkContent, loopFunc, out found);
        }

        private static TGFParams GetTGFParams(in int index, in FuncIn<int, int, TryGetFirstParams> funcInitParams, in FuncIn<int, int, TryGetFirstParams> funcRollBackParams) => new
#if !CS9
            TGFParams
#endif
            (index, funcInitParams, funcRollBackParams);

        private static TGFParams GetTGFAParams(in int index) => GetTGFParams(index, (in int _index, in int count) => new
#if !CS9
            TryGetFirstParams
#endif
            (_index + 1, count), (in int _index, in int _count) => new
#if !CS9
            TryGetFirstParams
#endif
            (0, _index));

        private static TGFParams GetTGFBParams(in int index) => GetTGFParams(index, (in int _index, in int count) => new
#if !CS9
            TryGetFirstParams
#endif
            (_index - 1, 0), (in int _index, in int _count) => new
#if !CS9
            TryGetFirstParams
#endif
            (_count - 1, _index + 1));

        private static T TryGetFirst<T>(in ItemsControl itemsControl, int index, in FuncIn<int, TGFParams> func, ref bool rollBack, ref bool checkContent, in LoopFunc<T> loopFunc, out bool found) where T : Visual => TryGetFirst(itemsControl, (in ItemsControl _itemsControl) => _itemsControl.Items, func(index), ref rollBack, ref checkContent, loopFunc, out found);

        public static T TryGetFirstAfter<T>(this ItemsControl itemsControl, int index, ref bool rollBack, ref bool checkContent, out bool found) where T : Visual => TryGetFirst<T>(itemsControl, index, GetTGFAParams, ref rollBack, ref checkContent, LoopFuncASC, out found);

        public static T TryGetFirstBefore<T>(this ItemsControl itemsControl, int index, ref bool rollBack, ref bool checkContent, out bool found) where T : Visual => TryGetFirst<T>(itemsControl, index, GetTGFBParams, ref rollBack, ref checkContent, LoopFuncDESC, out found);

        private static T TryGetFirst<T>(in Panel itemsControl, int index, in FuncIn<int, TGFParams> func, ref bool rollBack, ref bool checkContent, in LoopFunc<T> loopFunc, out bool found) where T : Visual => TryGetFirst(itemsControl, (in Panel _itemsControl) => _itemsControl.Children, func(index), ref rollBack, ref checkContent, loopFunc, out found);

        public static T TryGetFirstAfter<T>(this Panel itemsControl, int index, ref bool rollBack, ref bool checkContent, out bool found) where T : Visual => TryGetFirst<T>(itemsControl, index, GetTGFAParams, ref rollBack, ref checkContent, LoopFuncASC, out found);

        public static T TryGetFirstBefore<T>(this Panel itemsControl, int index, ref bool rollBack, ref bool checkContent, out bool found) where T : Visual => TryGetFirst<T>(itemsControl, index, GetTGFBParams, ref rollBack, ref checkContent, LoopFuncDESC, out found);
    }

    public static class Temp
    {
        public static RoutedEventArgs<BooleanEventArgs> GetRoutedBooleanEventArgs(in RoutedEvent @event, in bool value) => new
#if !CS9
            RoutedEventArgs<BooleanEventArgs>
#endif
            (@event, new BooleanEventArgs(value));

        public static void RegisterClassHandler<T>(RoutedEvent routedEvent, Delegate handler) => EventManager.RegisterClassHandler(typeof(T), routedEvent, handler);

        public static void RegisterClassHandler<T>(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo) => EventManager.RegisterClassHandler(typeof(T), routedEvent, handler, handledEventsToo);

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
