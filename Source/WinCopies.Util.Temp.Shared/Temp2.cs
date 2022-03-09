using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.ThrowHelper;
using static WinCopies.Temp.Util;
using System;
using System.Collections.Generic;

namespace WinCopies.Temp
{
    public interface IPopable<TIn, TOut>
    {
        TOut Pop(TIn key);
    }

    public interface IAsEnumerable<T>
    {
        System.Collections.Generic.IEnumerable<T> AsEnumerable();
    }

    public interface IMultiTypeEnumerable<T, U> : System.Collections.Generic.IEnumerable<T>, IAsEnumerable<U> where T : U
    {
        System.Collections.Generic.IEnumerable<U> IAsEnumerable<U>.AsEnumerable() => this.As<T, U>();
    }

    public class ArrayEnumerator : Enumerator<object?>
    {
        protected Array Array { get; }

        public int CurrentIndex { get; private set; }

        public override bool? IsResetSupported => true;

        protected override object? CurrentOverride => Array.GetValue(CurrentIndex);

        public ArrayEnumerator(in Array array)
        {
            Array = array ?? throw GetArgumentNullException(nameof(array));

            ResetIndex();
        }

        protected void ResetIndex() => CurrentIndex = -1;

        protected override void ResetCurrent()
        {
            base.ResetCurrent();

            ResetIndex();
        }

        protected override bool MoveNextOverride() => ++CurrentIndex < Array.Length;

        protected override void ResetOverride2() => ResetIndex();
    }

    public class UIntCountableEnumerable<T> : Collections.Enumeration.Generic.UIntCountableEnumerable<IUIntCountableEnumerable<T>, T>
    {
        public UIntCountableEnumerable(in IUIntCountableEnumerable<T> enumerable) : base(enumerable) { /* Left empty. */ }
    }

    public interface IPrependableExtensibleEnumerable<T> : System.Collections.Generic.IEnumerable<T>
    {
        void Prepend(T item);

        void PrependRange(System.Collections.Generic.IEnumerable<T> items);
    }

    public interface IAppendableExtensibleEnumerable<T>
    {
        void Append(T item);

        void AppendRange(System.Collections.Generic.IEnumerable<T> items);
    }

    public interface IExtensibleEnumerable<T> : IPrependableExtensibleEnumerable<T>, IAppendableExtensibleEnumerable<T>
    {
        // Left empty.
    }

    public abstract class ExtensibleEnumerableBase<TIn, TOut> : IExtensibleEnumerable<TOut>
    {
        protected IExtensibleEnumerable<TIn> InnerEnumerable { get; }

        protected ExtensibleEnumerableBase(IExtensibleEnumerable<TIn> enumerable) => InnerEnumerable = enumerable;

        public abstract void Prepend(TOut item);
        public abstract void PrependRange(System.Collections.Generic.IEnumerable<TOut> items);

        public abstract void Append(TOut item);
        public abstract void AppendRange(System.Collections.Generic.IEnumerable<TOut> items);

        public abstract System.Collections.Generic.IEnumerator<TOut> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ExtensibleEnumerable<TIn, TOut> : ExtensibleEnumerableBase<TIn, TOut>
    {
        public ExtensibleEnumerable(IExtensibleEnumerable<TIn> enumerable) : base(enumerable) { /* Left empty. */ }

        protected void PerformAction(in TOut parameter, in string paramName, in Action<TIn> action) => PerformAction<TIn, TOut>(parameter, paramName, action);

        protected void PerformAction(in System.Collections.Generic.IEnumerable<TOut> parameters, in Action<System.Collections.Generic.IEnumerable<TIn>> action) => PerformAction<TIn, TOut>(parameters, action);

        public override void Append(TOut item) => PerformAction(item, nameof(item), InnerEnumerable.Append);

        public override void AppendRange(System.Collections.Generic.IEnumerable<TOut> items) => PerformAction(items, InnerEnumerable.AppendRange);

        public override void Prepend(TOut item) => PerformAction(item, nameof(item), InnerEnumerable.Prepend);

        public override void PrependRange(System.Collections.Generic.IEnumerable<TOut> items) => PerformAction(items, InnerEnumerable.PrependRange);

        public override System.Collections.Generic.IEnumerator<TOut> GetEnumerator() => InnerEnumerable.To<TOut>().GetEnumerator();
    }

    public static class Delegates
    {
        public static string? ToString(object obj) => obj.ToString();
        public static string? ToStringIn(in object obj) => obj.ToString();
        public static string? ToStringRef(ref object obj) => obj.ToString();
        public static string? ToStringT<T>(T value) => (value ?? throw GetArgumentNullException(nameof(value))).ToString();
        public static string? ToStringInT<T>(in T value) => (value ?? throw GetArgumentNullException(nameof(value))).ToString();
        public static string? ToStringRefT<T>(ref T value) => (value ?? throw GetArgumentNullException(nameof(value))).ToString();

        public static Converter<T, string?> GetSurrounder<T>(in char decorator) => GetSurrounder<T>(decorator.ToString());
        public static Converter<T, string?> GetSurrounder<T>(string decorator) => value => value?.ToString().Surround(decorator);

        private static T _GetIn<T>(in Func<bool> func, in T ifTrue, in T ifFalse) => GetIn(func, ifTrue, ifFalse);

        public static T Get<T>(Func<bool> func, T ifTrue, T ifFalse) => GetIn(func, ifTrue, ifFalse);
        public static T GetIn<T>(in Func<bool> func, in T ifTrue, in T ifFalse) => _GetIn(func ?? throw GetArgumentNullException(nameof(func)), ifTrue, ifFalse);
        public static T Get<T>(bool value, T ifTrue, T ifFalse) => GetIn(value, ifTrue, ifFalse);
        public static T GetIn<T>(in bool value, in T ifTrue, in T ifFalse) => value ? ifTrue : ifFalse;

        public static TOut GetIfNull<TIn, TOut>(Func<TIn> func, TOut ifTrue, TOut ifFalse) where TIn : class? => GetIfNullIn(func, ifTrue, ifFalse);
        public static TOut GetIfNullIn<TIn, TOut>(in Func<TIn> func, in TOut ifTrue, in TOut ifFalse) where TIn : class? => GetIfNullIn((func ?? throw GetArgumentNullException(nameof(func)))(), ifTrue, ifFalse);
        public static TOut GetIfNull<TIn, TOut>(TIn value, TOut ifTrue, TOut ifFalse) where TIn : class? => GetIfNullIn(value, ifTrue, ifFalse);
        public static TOut GetIfNullIn<TIn, TOut>(TIn value, in TOut ifTrue, in TOut ifFalse) where TIn : class? => _GetIn(() => value == null, ifTrue, ifFalse);
    }

    public static class Bool
    {
        public static bool FirstBool(bool ok, object? obj) => FirstBoolIn(ok, obj);
        public static bool FirstBoolIn(in bool ok, in object? obj) => ok;
        public static bool FirstBoolGeneric<T>(bool ok, T value) => FirstBoolIn(ok, value);
        public static bool FirstBoolInGeneric<T>(in bool ok, in T value) => FirstBoolIn(ok, value);

        private static T2 PrependPredicate<T1, T2>(T1 x, Func<T2> func) where T1 : class? => x == null ? throw GetArgumentNullException(nameof(x)) : func();

        private static Predicate<T> GetPredicate<T>(Predicate<T> x, Predicate<T> y) => value => x(value) && y(value);
        private static PredicateIn<T> GetPredicateIn<T>(Predicate<T> x, PredicateIn<T> y) => (in T value) => x(value) && y(value);
        private static PredicateIn<T> GetPredicateIn<T>(PredicateIn<T> x, PredicateIn<T> y) => (in T value) => x(value) && y(value);

        public static Predicate<object> PrependPredicateNULL(Predicate<object> x, Predicate<object>? y) => PrependPredicate(x, () => y == null ? x : GetPredicate(x, y));
        public static Predicate<T> PrependPredicateNULL<T>(Predicate<T> x, Predicate<T>? y) => PrependPredicate(x, () => y == null ? x : value => x(value) && y(value));
        public static PredicateIn<object> PrependPredicateInNULL(PredicateIn<object> x, PredicateIn<object>? y) => PrependPredicate(x, () => y == null ? x : (in object obj) => x(obj) && y(obj));
        public static PredicateIn<T> PrependPredicateInNULL<T>(PredicateIn<T> x, PredicateIn<T>? y) => PrependPredicate(x, () => y == null ? x : (in T value) => x(value) && y(value));

        public static Predicate<object> PrependPredicate(Predicate<object> x, Predicate<object> y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicate(x, y));
        public static Predicate<T> PrependPredicate<T>(Predicate<T> x, Predicate<T> y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicate(x, y));
        public static PredicateIn<object> PrependPredicateIn(PredicateIn<object> x, PredicateIn<object> y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicateIn(x, y));
        public static PredicateIn<T> PrependPredicateIn<T>(PredicateIn<T> x, PredicateIn<T> y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicateIn(x, y));

        public static PredicateIn<object> PrependPredicateIn(Predicate<object> x, PredicateIn<object> y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicateIn(x, y));
        public static PredicateIn<T> PrependPredicateIn<T>(Predicate<T> x, PredicateIn<T> y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicateIn(x, y));

        public static Func<bool, object, bool> PrependPredicate(Predicate<object>? predicate) => predicate == null ? FirstBool : (bool ok, object value) => ok && predicate(value);
        public static Func<bool, T, bool> PrependPredicate<T>(Predicate<T>? predicate) => predicate == null ? FirstBoolGeneric : (bool ok, T value) => ok && predicate(value);
        public static FuncIn<bool, object, bool> PrependPredicateIn(Predicate<object>? predicate) => predicate == null ? FirstBoolIn : (in bool ok, in object value) => ok && predicate(value);
        public static FuncIn<bool, T, bool> PrependPredicateIn<T>(Predicate<T>? predicate) => predicate == null ? FirstBoolInGeneric : (in bool ok, in T value) => ok && predicate(value);
    }

    public static class ThrowHelper
    {

        public static void ThrowIfNull(object? obj, in string paramName)
        {
            if (obj == null)

                throw GetArgumentNullException(paramName);
        }

        public static T GetResultOrThrowIfNull<T>(Func<T?> func, string errorMessage) where T : class => func() ?? throw new InvalidOperationException(errorMessage);

        public static Func<T> FuncAsGetResultOrThrowIfNull<T>(Func<T?> func, string errorMessage) where T : class => () => GetResultOrThrowIfNull(func, errorMessage);

        public static Exception GetExceptionForInvalidType<T>(in object? obj, in string argumentName) => WinCopies.ThrowHelper.GetExceptionForInvalidType<T>(obj?.GetType(), argumentName);
    }

    public static class Util
    {
        public static ConstructorInfo? TryGetConstructor<T>(params Type[] types) => typeof(T).TryGetConstructor(types);

        public static ConstructorInfo? GetConstructor<T>(params Type[] types) => typeof(T).AssertGetConstructor(types);

        /*public static System.Collections.Generic.IEnumerable<PropertyInfo> GetAllProperties<T, U>(in bool include = true) where T : U => typeof(T)._GetAllProperties(typeof(U), include);

        public static System.Collections.Generic.IEnumerable<PropertyInfo> GetAllProperties<T>(in Type u, in bool include = true) => typeof(T).GetAllProperties(u, nameof(T), nameof(u), include);*/

        public static bool IsSigned(object value) => (value ?? throw GetArgumentNullException(nameof(value))).Is(true, typeof(int), typeof(short), typeof(long), typeof(sbyte))
                || (value.Is(true, typeof(uint), typeof(ushort), typeof(ulong), typeof(byte))
                ? false
                : throw new ArgumentException("The given value is neither from a signed nor an unsigned type."));

        public static unsafe void ReadLines(params IValueObject<string?>[] @params)
        {
            ThrowIfNull(@params, nameof(@params));

            foreach (IValueObject<string?>? param in @params)
            {
                if (param == null)

                    throw new ArgumentException("The given array contains one or more null values.", nameof(@params));

                param.Value = Console.ReadLine();
            }
        }

        public static unsafe void ReadLines(params KeyValuePair<string, IValueObject<string?>>[] @params)
        {
            ThrowIfNull(@params, nameof(@params));

            foreach (KeyValuePair<string, IValueObject<string?>> param in @params)

                param.Value.Value = ReadLine(param.Key);
        }

        public static T[] GetArray<T>(params T[] items) => items;

        public static string? ReadLine(in string msg)
        {
            Console.WriteLine(msg);

            return Console.ReadLine();
        }

        [return: NotNull]
        public static T ReadLine<T>(in string msg, in Converter<string?, T?> converter, in string errorMessage)
        {
            T? result;

            while ((result = converter(ReadLine(msg))) == null)

                Console.WriteLine(errorMessage);

            return result;
        }

        public static T ReadValue<T>(in string msg, in Converter<string?, T?> converter, in string errorMessage) where T : struct => ReadLine(msg, converter, errorMessage).Value;

        public static void PerformAction<TIn, TOut>(in TOut parameter, in string paramName, in Action<TIn> action) => action(parameter is TIn _parameter ? _parameter : throw new InvalidArgumentException(paramName));

        public static void PerformAction<TIn, TOut>(in System.Collections.Generic.IEnumerable<TOut> parameters, in Action<System.Collections.Generic.IEnumerable<TIn>> action) => action(parameters.To<TIn>());

        public static void WriteMenu<T>(in List<KeyValuePair<string, T>> menu)
        {
            for (int i = 0; i < menu.Count; i++)

                Console.WriteLine($"{i}: {menu[i].Key}");
        }

        public static void HandleMenu(List<KeyValuePair<string, Func<bool>>> menu, in bool clear)
        {
            do
            {
                if (clear)

                    Console.Clear();

                WriteMenu(menu);
            }

            while (menu[ReadValue<int>("Please make a choice to continue: ", s => int.TryParse(s, out int packageId) && packageId.Between(0, menu.Count - 1, true, true) ? packageId : null, "Invalid value.")].Value());
        }
    }
}
