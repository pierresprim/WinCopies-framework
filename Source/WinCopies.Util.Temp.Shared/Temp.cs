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
using System.Text;

namespace WinCopies.Temp
{
    public static class Regex
    {
        public const string LikeSnippet = ".*?";

        public static string FromPattern(in string filter, in char c, in string pattern) => filter == null
                ? ""
                : $"^{filter.Format(c, value => System.Text.RegularExpressions.Regex.Escape(value)).Replace(c.ToString(), pattern)}$";

        public static string FromPathFilter(in string filter) => FromPattern(filter, Temp.PathFilterChar, LikeSnippet);

        public static string FromLikeStatement(in string filter) => FromPattern(filter, Temp.LikeStatementChar, LikeSnippet);
    }

    public static class Temp
    {
        public const char PathFilterChar = '*';
        public const char LikeStatementChar = '%';

        public static string Format(this string value, in char except, in Func<string, string> func)
        {
            string[] split = value.Split(except);

            for (int i = 0; i < split.Length; i++)

                split[i] = func(split[i]);

            var result = new StringBuilder();

            return result.AppendJoin(except, split).ToString();
        }

        public static T First<T>(this System.Collections.Generic.IReadOnlyList<T> list) => list[0];

        public static T Last<T>(this System.Collections.Generic.IReadOnlyList<T> list) => list[list.Count - 1];

#if !CS8
        public static StringBuilder AppendJoin(this StringBuilder stringBuilder, in string s, in System.Collections.Generic.IEnumerable<string> values)
        {
            if (values is System.Collections.Generic.IReadOnlyList<string> collection)

                return stringBuilder.AppendJoin(s, collection);

            foreach (string value in values)

                _ = stringBuilder.Append(value).Append(s);

            _ = stringBuilder.Remove(stringBuilder.Length - s.Length, s.Length);

            return stringBuilder;
        }

        public static StringBuilder AppendJoin(this StringBuilder stringBuilder, in char c, in System.Collections.Generic.IEnumerable<string> values) => stringBuilder.AppendJoin(c.ToString(), values);

        public static StringBuilder AppendJoin(this StringBuilder stringBuilder, in char c, in System.Collections.Generic.IReadOnlyList<string> values) => stringBuilder.AppendJoin(c.ToString(), values);

        public static StringBuilder AppendJoin(this StringBuilder stringBuilder, in string s, in System.Collections.Generic.IReadOnlyList<string> values)
        {
            for (int i = 0; i < values.Count - 1; i++)

                _ = stringBuilder.Append(values[i]).Append(s);

            _ = stringBuilder.Append(values.Last());

            return stringBuilder;
        }

        public static StringBuilder AppendJoin(this StringBuilder stringBuilder, in char c, params string[] values) => stringBuilder.AppendJoin(c.ToString(), values);

        public static StringBuilder AppendJoin(this StringBuilder stringBuilder, in string s, params string[] values) => stringBuilder.AppendJoin(s, (System.Collections.Generic.IReadOnlyList<string>)values);
#endif
    }
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
