using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.ThrowHelper;
using static WinCopies.Temp.Util;

namespace WinCopies.Temp
{
    public delegate bool PredicateIn(in object obj);

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
#if CS8
        System.Collections.Generic.IEnumerable<U> IAsEnumerable<U>.AsEnumerable() => this.As<T, U>();
#else
        // Left empty.
#endif
    }

    public class ArrayEnumerator : Enumerator<object
#if CS8
                ?
#endif
                >
    {
        protected Array Array { get; }

        public int CurrentIndex { get; private set; }

        public override bool? IsResetSupported => true;

        protected override object
#if CS8
                ?
#endif
                CurrentOverride => Array.GetValue(CurrentIndex);

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
        public static string
#if CS8
                ?
#endif
                ToString(object obj) => obj.ToString();
        public static string
#if CS8
                ?
#endif
                ToStringIn(in object obj) => obj.ToString();
        public static string
#if CS8
                ?
#endif
                ToStringRef(ref object obj) => obj.ToString();
        public static string
#if CS8
                ?
#endif
                ToStringT<T>(T value) => (value
#if CS8
            ??
#else
        == null ?
#endif
            throw GetArgumentNullException(nameof(value))
#if !CS8
             : value
#endif
            ).ToString();
        public static string
#if CS8
                ?
#endif
                ToStringInT<T>(in T value) => (value
#if CS8
            ??
#else
        == null ?
#endif
             throw GetArgumentNullException(nameof(value))
#if !CS8
             : value
#endif
            ).ToString();
        public static string
#if CS8
                ?
#endif
                ToStringRefT<T>(ref T value) => (value
#if CS8
            ??
#else
        == null ?
#endif
             throw GetArgumentNullException(nameof(value))
#if !CS8
             : value
#endif
            ).ToString();

        public static Converter<T, string
#if CS8
                ?
#endif
                > GetSurrounder<T>(in char decorator) => GetSurrounder<T>(decorator.ToString());
        public static Converter<T, string
#if CS8
                ?
#endif
                > GetSurrounder<T>(string decorator) => value => value?.ToString().Surround(decorator);

        private static T _GetIn<T>(in Func<bool> func, in T ifTrue, in T ifFalse) => GetIn(func, ifTrue, ifFalse);

        public static T Get<T>(Func<bool> func, T ifTrue, T ifFalse) => GetIn(func, ifTrue, ifFalse);
        public static T GetIn<T>(in Func<bool> func, in T ifTrue, in T ifFalse) => _GetIn(func ?? throw GetArgumentNullException(nameof(func)), ifTrue, ifFalse);
        public static T Get<T>(bool value, T ifTrue, T ifFalse) => GetIn(value, ifTrue, ifFalse);
        public static T GetIn<T>(in bool value, in T ifTrue, in T ifFalse) => value ? ifTrue : ifFalse;

        public static TOut GetIfNull<TIn, TOut>(Func<TIn> func, TOut ifTrue, TOut ifFalse) where TIn : class
#if CS8
                ?
#endif
                => GetIfNullIn(func, ifTrue, ifFalse);
        public static TOut GetIfNullIn<TIn, TOut>(in Func<TIn> func, in TOut ifTrue, in TOut ifFalse) where TIn : class
#if CS8
                ?
#endif
                => GetIfNullIn((func ?? throw GetArgumentNullException(nameof(func)))(), ifTrue, ifFalse);
        public static TOut GetIfNull<TIn, TOut>(TIn value, TOut ifTrue, TOut ifFalse) where TIn : class
#if CS8
                ?
#endif
                => GetIfNullIn(value, ifTrue, ifFalse);
        public static TOut GetIfNullIn<TIn, TOut>(TIn value, in TOut ifTrue, in TOut ifFalse) where TIn : class
#if CS8
                ?
#endif
                => _GetIn(() => value == null, ifTrue, ifFalse);
    }

    public static class Bool
    {
        public static bool FirstBool(bool ok, object
#if CS8
                ?
#endif
                obj) => FirstBoolIn(ok, obj);
        public static bool FirstBoolIn(in bool ok, in object
#if CS8
                ?
#endif
                obj) => ok;
        public static bool FirstBoolGeneric<T>(bool ok, T value) => FirstBoolIn(ok, value);
        public static bool FirstBoolInGeneric<T>(in bool ok, in T value) => FirstBoolIn(ok, value);

        private static T2 PrependPredicate<T1, T2>(T1 x, Func<T2> func) where T1 : class
#if CS8
                ?
#endif
                => x == null ? throw GetArgumentNullException(nameof(x)) : func();

        private static Predicate GetPredicate(Predicate x, Predicate y) => value => x(value) && y(value);
        private static Predicate<T> GetPredicate<T>(Predicate<T> x, Predicate<T> y) => value => x(value) && y(value);
        private static PredicateIn GetPredicateIn(Predicate x, PredicateIn y) => (in object value) => x(value) && y(value);
        private static PredicateIn GetPredicateIn(PredicateIn x, PredicateIn y) => (in object value) => x(value) && y(value);
        private static PredicateIn<T> GetPredicateIn<T>(Predicate<T> x, PredicateIn<T> y) => (in T value) => x(value) && y(value);
        private static PredicateIn<T> GetPredicateIn<T>(PredicateIn<T> x, PredicateIn<T> y) => (in T value) => x(value) && y(value);

        public static Predicate PrependPredicateNULL(Predicate x, Predicate
#if CS8
                ?
#endif
                y) => PrependPredicate(x, () => y == null ? x : GetPredicate(x, y));
        public static Predicate<T> PrependPredicateNULL<T>(Predicate<T> x, Predicate<T>
#if CS8
                ?
#endif
                y) => PrependPredicate(x, () => y == null ? x : value => x(value) && y(value));
        public static PredicateIn PrependPredicateInNULL(PredicateIn x, PredicateIn
#if CS8
                ?
#endif
                y) => PrependPredicate(x, () => y == null ? x : (in object obj) => x(obj) && y(obj));
        public static PredicateIn<T> PrependPredicateInNULL<T>(PredicateIn<T> x, PredicateIn<T>
#if CS8
                ?
#endif
                y) => PrependPredicate(x, () => y == null ? x : (in T value) => x(value) && y(value));

        public static Predicate PrependPredicate(Predicate x, Predicate y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicate(x, y));
        public static Predicate<T> PrependPredicate<T>(Predicate<T> x, Predicate<T> y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicate(x, y));
        public static PredicateIn PrependPredicateIn(PredicateIn x, PredicateIn y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicateIn(x, y));
        public static PredicateIn<T> PrependPredicateIn<T>(PredicateIn<T> x, PredicateIn<T> y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicateIn(x, y));

        public static PredicateIn PrependPredicateIn(Predicate x, PredicateIn y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicateIn(x, y));
        public static PredicateIn<T> PrependPredicateIn<T>(Predicate<T> x, PredicateIn<T> y) => PrependPredicate(x, () => y == null ? throw GetArgumentNullException(nameof(y)) : GetPredicateIn(x, y));

        public static Func<bool, object, bool> PrependPredicate(Predicate
#if CS8
                ?
#endif
                predicate)
#if CS9
            =>
#else
        {
            if (
#endif
                predicate == null
#if CS9
            ?
#else
            ) return
#endif
                FirstBool
#if CS9
                : 
#else
                ;

            else return
#endif
            (bool ok, object value) => ok && predicate(value);
#if !CS9
        }
#endif

        public static Func<bool, T, bool> PrependPredicate<T>(Predicate<T>
#if CS8
                ?
#endif
                predicate)
#if CS9
            =>
#else
        {
            if (
#endif
                predicate == null
#if CS9
                ?
#else
                ) return
#endif
                FirstBoolGeneric

#if CS9
                :
#else
                ;

            else return
#endif
                    (bool ok, T value) => ok && predicate(value);
#if !CS9
        }
#endif

        public static FuncIn<bool, object, bool> PrependPredicateIn(Predicate
#if CS8
                ?
#endif
                predicate)
#if CS9
            =>
#else
        {
            if (
#endif
         predicate == null
#if CS9
        ?
#else
        ) return
#endif
        FirstBoolIn
#if CS9
        
            :
#else
            ;

            else return
#endif
            (in bool ok, in object value) => ok && predicate(value);
#if !CS9
        }
#endif

        public static FuncIn<bool, T, bool> PrependPredicateIn<T>(Predicate<T>
#if CS8
                ?
#endif
                predicate)
#if CS9
            =>
#else
        {
            if (
#endif
         predicate == null
#if CS9
         ?
#else
         )

                return
#endif
            FirstBoolInGeneric
#if CS9
                :
#else
                ;

            else return
#endif
                (in bool ok, in T value) => ok && predicate(value);
#if !CS9
        }
#endif
    }

    public static class ThrowHelper
    {

        public static void ThrowIfNull(object
#if CS8
                ?
#endif
                obj, in string paramName)
        {
            if (obj == null)

                throw GetArgumentNullException(paramName);
        }

        public static T GetResultOrThrowIfNull<T>(Func<T
#if CS8
                ?
#endif
                > func, string errorMessage) where T : class => func() ?? throw new InvalidOperationException(errorMessage);

        public static Func<T> FuncAsGetResultOrThrowIfNull<T>(Func<T
#if CS8
                ?
#endif
                > func, string errorMessage) where T : class => () => GetResultOrThrowIfNull(func, errorMessage);

        public static Exception GetExceptionForInvalidType<T>(in object
#if CS8
                ?
#endif
                obj, in string argumentName) => WinCopies.ThrowHelper.GetExceptionForInvalidType<T>(obj?.GetType(), argumentName);
    }

    public static class Util
    {
        public static ConstructorInfo
#if CS8
                ?
#endif
                TryGetConstructor<T>(params Type[] types) => typeof(T).TryGetConstructor(types);

        public static ConstructorInfo
#if CS8
                ?
#endif
                GetConstructor<T>(params Type[] types) => typeof(T).AssertGetConstructor(types);

        /*public static System.Collections.Generic.IEnumerable<PropertyInfo> GetAllProperties<T, U>(in bool include = true) where T : U => typeof(T)._GetAllProperties(typeof(U), include);

        public static System.Collections.Generic.IEnumerable<PropertyInfo> GetAllProperties<T>(in Type u, in bool include = true) => typeof(T).GetAllProperties(u, nameof(T), nameof(u), include);*/

        public static bool IsSigned(object value) => (value ?? throw GetArgumentNullException(nameof(value))).Is(true, typeof(int), typeof(short), typeof(long), typeof(sbyte))
                || (value.Is(true, typeof(uint), typeof(ushort), typeof(ulong), typeof(byte))
                ? false
                : throw new ArgumentException("The given value is neither from a signed nor an unsigned type."));

        public static unsafe void ReadLines(params IValueObject<string
#if CS8
                ?
#endif
                >[] @params)
        {
            ThrowIfNull(@params, nameof(@params));

            foreach (IValueObject<string
#if CS8
                ?
#endif
                >
#if CS8
                ?
#endif
                param in @params)
            {
                if (param == null)

                    throw new ArgumentException("The given array contains one or more null values.", nameof(@params));

                param.Value = Console.ReadLine();
            }
        }

        public static unsafe void ReadLines(params KeyValuePair<string, IValueObject<string
#if CS8
                ?
#endif
                >>[] @params)
        {
            ThrowIfNull(@params, nameof(@params));

            foreach (KeyValuePair<string, IValueObject<string
#if CS8
                ?
#endif
                >> param in @params)

                param.Value.Value = ReadLine(param.Key);
        }

        public static T[] GetArray<T>(params T[] items) => items;

        public static string
#if CS8
                ?
#endif
                ReadLine(in string msg)
        {
            Console.WriteLine(msg);

            return Console.ReadLine();
        }

#if CS8
        [return: NotNull]
#endif
        public static T ReadLine<T>(in string msg, in Converter<string
#if CS8
                ?
#endif
                , T
#if CS9
                ?
#endif
                > converter, in string errorMessage)
        {
            T
#if CS9
            ?
#endif
                result;

            while ((result = converter(ReadLine(msg))) == null)

                Console.WriteLine(errorMessage);

            return result;
        }

        public static T ReadValue<T>(in string msg, in Converter<string
#if CS8
                ?
#endif
                , T?> converter, in string errorMessage) where T : struct => ReadLine(msg, converter, errorMessage).Value;

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

            while (menu[ReadValue<int>("Please make a choice to continue: ", s => int.TryParse(s, out int packageId) && packageId.Between(0, menu.Count - 1, true, true) ?
#if !CS9
            (int?)
#endif
            packageId : null, "Invalid value.")].Value());
        }
    }
}
