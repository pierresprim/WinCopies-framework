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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;
using System.Windows.Data;

using WinCopies.Util.Data;
using WinCopies.Collections.DotNetFix
#if WinCopies3
    .Generic
#endif
    ;
using static WinCopies.
#if WinCopies3
    ThrowHelper
#else
    Util.Util;

using static WinCopies.Util.ThrowHelper
#endif
    ;

using WinCopies.Collections;
using WinCopies.Collections.Abstraction.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Linq;

using static WinCopies.Util.Data.ConverterHelper;
using WinCopies.Collections.DotNetFix;

#if !WinCopies3
using System.Collections;
using WinCopies.Util;
#endif

namespace WinCopies
{
    public delegate TResult FuncOut<in T1, T2, out TResult>(T1 p1, out T2 p2);
    public delegate TResult FuncOut<in T1, in T2, T3, out TResult>(T1 p1, T2 p2, out T3 p3);
    public delegate TResult FuncOut<in T1, in T2, in T3, T4, out TResult>(T1 p1, T2 p2, T3 p3, out T4 p4);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, T5, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, out T5 p5);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, T6, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, out T6 p6);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, T7, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, out T7 p7);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, in T7, T8, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, out T8 p8);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, T9, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, out T9 p9);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, T10, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, out T10 p10);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, T11, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, out T11 p11);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, T12, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, out T12 p12);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, T13, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, out T13 p13);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, T14, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, out T14 p14);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, T15, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, out T15 p15);
    public delegate TResult FuncOut<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, T16, out TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, out T16 p16);

    public delegate TResult FuncIn<T1, out TResult>(in T1 p1);
    public delegate TResult FuncIn<T1, T2, out TResult>(in T1 p1, in T2 p2);
    public delegate TResult FuncIn<T1, T2, T3, out TResult>(in T1 p1, in T2 p2, in T3 p3);
    public delegate TResult FuncIn<T1, T2, T3, T4, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, T8, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13, in T14 p14);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13, in T14 p14, in T15 p15);
    public delegate TResult FuncIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13, in T14 p14, in T15 p15, in T16 p16);

    public delegate TResult FuncInOut<T1, T2, out TResult>(in T1 p1, out T2 p2);
    public delegate TResult FuncInOut<T1, T2, T3, out TResult>(in T1 p1, in T2 p2, out T3 p3);
    public delegate TResult FuncInOut<T1, T2, T3, T4, out TResult>(in T1 p1, in T2 p2, in T3 p3, out T4 p4);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, out T5 p5);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, out T6 p6);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, out T7 p7);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, T8, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, out T8 p8);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, T8, T9, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, out T9 p9);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, out T10 p10);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, out T11 p11);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, out T12 p12);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, out T13 p13);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13, out T14 p14);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13, in T14 p14, out T15 p15);
    public delegate TResult FuncInOut<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, out TResult>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13, in T14 p14, in T15 p15, out T16 p16);

    public delegate void ActionIn<T>(in T parameter);
    public delegate void ActionIn<T1, T2>(in T1 p1, in T2 p2);
    public delegate void ActionIn<T1, T2, T3>(in T1 p1, in T2 p2, in T3 p3);
    public delegate void ActionIn<T1, T2, T3, T4>(in T1 p1, in T2 p2, in T3 p3, in T4 p4);
    public delegate void ActionIn<T1, T2, T3, T4, T5>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7, T8, T9>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13, in T14 p14);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13, in T14 p14, in T15 p15);
    public delegate void ActionIn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(in T1 p1, in T2 p2, in T3 p3, in T4 p4, in T5 p5, in T6 p6, in T7 p7, in T8 p8, in T9 p9, in T10 p10, in T11 p11, in T12 p12, in T13 p13, in T14 p14, in T15 p15, in T16 p16);

    public interface IValueProvider<T1, T2>
    {
        T1 ProvideValues(out T2 result);
    }

    public abstract class ValueProvider<T1, T2> : IValueProvider<T1, T2>
    {
        private readonly T1 _value;

        public ValueProvider(in T1 value) => _value = value;

        protected abstract T2 ProvideSecondValue(in T1 value);

        protected virtual void ValidateValue(in T1 value) { /* Left empty. */ }

        protected virtual void ValidateResultValue(in T2 value) { /* Left empty. */ }

        public T1 ProvideValues(out T2 result)
        {
            ValidateValue(_value);

            T2 _result = ProvideSecondValue(_value);

            ValidateResultValue(_result);

            result = _result;

            return _value;
        }

    }

    public class DelegateValueProvider<T1, T2> : ValueProvider<T1, T2>
    {
        private readonly Func<T1, T2> _func;

        public DelegateValueProvider(in T1 value, in Func<T1, T2> func) : base(value) => _func = func ?? throw GetArgumentNullException(nameof(func));

        protected sealed override T2 ProvideSecondValue(in T1 value) => _func(value);
    }

    public sealed class NullableGeneric<T>
    {
        public T Value { get; }

        public NullableGeneric(T value) => Value = value;
    }

    // TODO: ICountableEnumerableInfo should have a GetReversedEnumerator() method that returns an ICountableEnumeratorInfo<T>.

    public class StringCharArray : ICountableEnumerableInfo<char>, System.Collections.Generic.IReadOnlyList<char>
    {
        private readonly string _s;

        public int Count => _s.Length;

        public char this[int index] => _s[index];

        public bool SupportsReversedEnumeration => true;

        public StringCharArray(in string s) => _s = s;

        public ICountableEnumeratorInfo<char> GetEnumerator() => new CountableEnumeratorInfo<char>(new EnumeratorInfo<char>(_s), () => Count);

        System.Collections.Generic.IEnumerator<char> System.Collections.Generic.IEnumerable<char>.GetEnumerator() => GetEnumerator();

        System.Collections.IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        ICountableEnumerator<char> ICountableEnumerable<char>.GetEnumerator() => GetEnumerator();

        public IEnumeratorInfo2<char> GetReversedEnumerator() => new ArrayEnumerator<char>(this, true);

        IEnumeratorInfo2<char> Collections.DotNetFix.Generic.IEnumerable<char, IEnumeratorInfo2<char>>.GetEnumerator() => GetEnumerator();

        System.Collections.Generic.IEnumerator<char> Collections.Generic.IEnumerable<char>.GetReversedEnumerator() => GetReversedEnumerator();
    }

    public abstract class UIntCountableEnumerable<TEnumerable, TItems> : IUIntCountableEnumerable<TItems> where TEnumerable : ICountableEnumerable<TItems>
    {
        protected TEnumerable Enumerable { get; }

        public uint Count => (uint)Enumerable.Count;

        public UIntCountableEnumerable(in TEnumerable enumerable) => Enumerable = enumerable;

        public IUIntCountableEnumeratorInfo<TItems> GetEnumerator() => new UIntCountableEnumeratorInfo<TItems>(new EnumeratorInfo<TItems>(Enumerable), () => (uint)Enumerable.Count);

        System.Collections.Generic.IEnumerator<TItems> System.Collections.Generic.IEnumerable<TItems>.GetEnumerator() => GetEnumerator();

        System.Collections.IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IUIntCountableEnumerator<TItems> IUIntCountableEnumerable<TItems>.GetEnumerator() => GetEnumerator();
    }

    public class UIntCountableEnumerable<T> : UIntCountableEnumerable<ICountableEnumerable<T>, T>
    {
        public UIntCountableEnumerable(in ICountableEnumerable<T> enumerable) : base(enumerable)
        {
            // Left empty.
        }
    }

    public class UIntCountableEnumerableInfo<T> : UIntCountableEnumerable<ICountableEnumerableInfo<T>, T>, IUIntCountableEnumerableInfo<T>
    {
        public bool SupportsReversedEnumeration => Enumerable.SupportsReversedEnumeration;

        public UIntCountableEnumerableInfo(in ICountableEnumerableInfo<T> enumerable) : base(enumerable)
        {
            // Left empty.
        }

        public IEnumeratorInfo2<T> GetReversedEnumerator() => Enumerable.GetReversedEnumerator();

        IEnumeratorInfo2<T> Collections.DotNetFix.Generic.IEnumerable<T, IEnumeratorInfo2<T>>.GetEnumerator() => GetEnumerator();

        System.Collections.Generic.IEnumerator<T> Collections.Generic.IEnumerable<T>.GetReversedEnumerator() => GetReversedEnumerator();
    }

    public sealed class RepeatEnumerator<T> : Enumerator<T>, IUIntCountableEnumerator<T>, ICountableEnumerator<T>, IUIntCountableEnumerableInfo<T>, ICountableEnumerableInfo<T>
    {
        private NullableGeneric<T> _value;
        private readonly uint _count;
        private uint _i;

        public override bool? IsResetSupported => GetValueIfNotDisposed(true);

        private TValue GetValueIfNotDisposed<TValue>(in TValue value) => _value == null ? throw GetExceptionForDispose(false) : value;

        protected override T CurrentOverride => GetValueIfNotDisposed(_value.Value);

        uint IUIntCountable.Count => GetValueIfNotDisposed(_count);

        private int Count => GetValueIfNotDisposed(_count > int.MaxValue ? throw new InvalidOperationException($"The number of items is greater than {nameof(Int32)}.{nameof(int.MaxValue)}.") : (int)_count);

        int ICountable.Count => Count;

        int IReadOnlyCollection<T>.Count => Count;

        int ICountableEnumerable<T>.Count => Count;

        bool Collections.Generic.IEnumerable<T>.SupportsReversedEnumeration => GetValueIfNotDisposed(true);

        public RepeatEnumerator(in NullableGeneric<T> value, in uint count)
        {
            _value = value;

            _count = count;
        }

        public RepeatEnumerator(in T value, in uint count) : this(new NullableGeneric<T>(value), count)
        {
            // Left empty.
        }

        protected override bool MoveNextOverride()
        {
            if (_value == null)

                throw GetExceptionForDispose(false);

            if (_i == _count)

                return false;

            else
            {
                _i++;

                return true;
            }
        }

        protected override void ResetOverride()
        {
            base.ResetOverride();

            _i = _value == null ? throw GetExceptionForDispose(false) : 0u;
        }

        void System.IDisposable.Dispose() => _value = null;

        private RepeatEnumerator<T> GetEnumerator() => GetValueIfNotDisposed(this);

        private ICountableEnumerator<T> GetCountableEnumerator() => GetEnumerator();

        private IEnumeratorInfo2<T> GetEnumeratorInfo() => new EnumeratorInfo<T>(GetCountableEnumerator());

        ICountableEnumerator<T> ICountableEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumeratorInfo2<T> Collections.Generic.IEnumerable<T, IEnumeratorInfo2<T>>.GetReversedEnumerator() => GetEnumeratorInfo();

        IEnumeratorInfo2<T> Collections.DotNetFix.Generic.IEnumerable<T, IEnumeratorInfo2<T>>.GetEnumerator() => GetEnumeratorInfo();

        System.Collections.Generic.IEnumerator<T> Collections.Generic.IEnumerable<T>.GetReversedEnumerator() => GetEnumerator();

        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => GetEnumerator();

        System.Collections.IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IUIntCountableEnumerator<T> IUIntCountableEnumerable<T>.GetEnumerator() => GetEnumerator();
    }

    public abstract class ValueManager<T> : DotNetFix.IDisposable
    {
        private ILinkedList<T> _values;

        protected ILinkedList<T> Values => this.GetIfNotDisposed(_values);

        public bool IsDisposed { get; private set; }

        public void Add(in T action) => _values.Add(action);

        public void Remove(in T action)
        {
            ILinkedListNode<T> node = _values.First;

            if (node == null)

                return;

            do
            {
                if (Equals(node.Value, action))
                {
                    _values.Remove(node);

                    break;
                }
            } while ((node = node.Next) != null);
        }

        protected virtual void Dispose(in bool disposing)
        {
            if (IsDisposed)

                return;

            if (disposing)
            {
                _values.Clear();

                _values = null;
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }

    public class EventDelegate<T> : ValueManager<Action<T>>
    {
        public void RaiseEvent(in T param)
        {
            foreach (Action<T> action in Values)

                action(param);
        }
    }

    public interface IQueryDelegateDelegate<TIn, TOut>
    {
        Func<TIn, TOut> FirstAction { get; }

        Func<TIn, TOut, TOut> OtherAction { get; }
    }

    public class QueryDelegateDelegate<TIn, TOut> : IQueryDelegateDelegate<TIn, TOut>
    {
        public Func<TIn, TOut> FirstAction { get; }

        public Func<TIn, TOut, TOut> OtherAction { get; }

        public QueryDelegateDelegate(in Func<TIn, TOut> firstAction, in Func<TIn, TOut, TOut> otherAction)
        {
            FirstAction = firstAction;

            OtherAction = otherAction;
        }
    }

    public abstract class EventAndQueryDelegate<TIn, TOut> : ValueManager<IQueryDelegateDelegate<TIn, TOut>>
    {
        protected abstract bool Check(in TOut outParam);

        protected abstract bool Merge(in TOut x, in TOut y, out TOut result);

        public TOut RaiseEvent(TIn param)
        {
            NullableGeneric<TOut> result = null;
            bool mergeResult;

            Values.ForEach<IQueryDelegateDelegate<TIn, TOut>>(func =>
            {
                result = new NullableGeneric<TOut>(func.FirstAction(param));

                return Check(result.Value);
            }, func =>
            {
                mergeResult = Merge(result.Value, func.OtherAction(param, result.Value), out TOut mergeResultParam);

                result = new NullableGeneric<TOut>(mergeResultParam);

                return mergeResult;
            });

            return result == null ? default : result.Value;
        }
    }

    public abstract class EventAndQueryDelegate2<TIn, TOut> : EventAndQueryDelegate<TIn, TOut>
    {
        protected abstract TOut Merge(in TOut x, in TOut y);

        protected sealed override bool Merge(in TOut x, in TOut y, out TOut result)
        {
            result = Merge(x, y);

            return Check(result);
        }
    }

    public class AND_EventAndQueryDelegate<TIn> : EventAndQueryDelegate2<TIn, bool>
    {
        protected override bool Check(in bool outParam) => true;

        protected override bool Merge(in bool x, in bool y) => x && y;
    }

    public class ANDALSO_EventAndQueryDelegate<TIn> : EventAndQueryDelegate2<TIn, bool>
    {
        protected override bool Check(in bool outParam) => outParam;

        protected override bool Merge(in bool x, in bool y) => x && y;
    }

    public class OR_EventAndQueryDelegate<TIn> : EventAndQueryDelegate2<TIn, bool>
    {
        protected override bool Check(in bool outParam) => true;

        protected override bool Merge(in bool x, in bool y) => x || y;
    }

    public class ORELSE_EventAndQueryDelegate<TIn> : EventAndQueryDelegate2<TIn, bool>
    {
        protected override bool Check(in bool outParam) => !outParam;

        protected override bool Merge(in bool x, in bool y) => x || y;
    }

    public class XOR_EventAndQueryDelegate<TIn> : EventAndQueryDelegate2<TIn, bool>
    {
        protected override bool Check(in bool outParam) => true;

        protected override bool Merge(in bool x, in bool y) => x ^ y;
    }

    public class XORELSE_EventAndQueryDelegate<TIn> : EventAndQueryDelegate<TIn, bool>
    {
        protected override bool Check(in bool outParam) => true;

        protected override bool Merge(in bool x, in bool y, out bool result)
        {
            if (x && y)
            {
                result = false;

                return false;
            }

            result = x ^ y;

            return true;
        }
    }

    public interface IPropertyObservable : DotNetFix.IDisposable
    {
        void AddPropertyChangedDelegate(Action<string> action);

        void RemovePropertyChangedDelegate(Action<string> action);
    }

    public static class Temp
    {
        public static void ThrowIfNullOrReadOnly(in ISimpleLinkedListBase linkedList, in string argumentName)
        {
            if ((linkedList ?? throw GetArgumentNullException(argumentName)).IsReadOnly)

                throw new ArgumentException($"{argumentName} is read-only.");
        }

        public static void ThrowIfNullOrReadOnly<T>(in ICollection<T> linkedList, in string argumentName)
        {
            if ((linkedList ?? throw GetArgumentNullException(argumentName)).IsReadOnly)

                throw new ArgumentException($"{argumentName} is read-only.");
        }

        public static T GetIfNotDisposed<T>(this WinCopies.DotNetFix.IDisposable obj, in T value) => obj.IsDisposed ? throw GetExceptionForDispose(false) : value;

        public static T GetIfNotDisposedOrDisposing<T>(this WinCopies.IDisposable obj, in T value) => obj.IsDisposed ? throw GetExceptionForDispose(false) : obj.IsDisposing ? throw GetExceptionForDispose(true) : value;

        public static void ForEach<T>(this System.Collections.Generic.IEnumerable<T> enumerable, in Func<T, bool> firstAction, Func<T, bool> otherAction)
        {
            System.Collections.Generic.IEnumerator<T> enumerator = enumerable.GetEnumerator();

            try
            {
                if (enumerator.MoveNext())

                    if (firstAction(enumerator.Current))

                        while (enumerator.MoveNext() && otherAction(enumerator.Current))
                        {
                            // Left empty.
                        }
            }

            finally
            {
                enumerator.Dispose();
            }
        }

        [DllImport(Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateDirectoryW([In, MarshalAs(UnmanagedType.LPWStr)] string lpPathName, [In] IntPtr lpSecurityAttributes);
    }
}
#endif
