/* Copyright © Pierre Sprimont, 2020
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

using WinCopies.Util.Data;
using WinCopies.Collections;
using WinCopies.Collections.DotNetFix
#if WinCopies3
    .Generic
#endif
    ;
using WinCopies.Collections.Abstract.Generic;
using WinCopies.Linq;
using static WinCopies.
#if WinCopies3
    ThrowHelper
#else
    Util.Util
#endif
    ;
using System.Globalization;

namespace WinCopies
{
    namespace Collections.Abstract.Generic
    {
        public abstract class Countable<TEnumerable, TItems> : ICountable where TEnumerable : IReadOnlyCollection<TItems>
        {
            protected TEnumerable InnerEnumerable { get; }

            public int Count => InnerEnumerable.Count;

            protected Countable(TEnumerable enumerable) => InnerEnumerable = enumerable;
        }

        public abstract class CountableEnumerable<TEnumerable, TSourceItems, TDestinationItems> : Countable<TEnumerable, TSourceItems>, ICountableEnumerable<TDestinationItems> where TEnumerable : IReadOnlyCollection<TSourceItems>
        {
            protected CountableEnumerable(TEnumerable enumerable) : base(enumerable) { /* Left empty. */ }

            public abstract System.Collections.Generic.IEnumerator<TDestinationItems> GetEnumerator();

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public abstract class CountableEnumerable<TEnumerable, TItems> : CountableEnumerable<TEnumerable, TItems, TItems> where TEnumerable : IReadOnlyCollection<TItems>
        {
            protected CountableEnumerable(TEnumerable enumerable) : base(enumerable) { /* Left empty. */ }

            public override System.Collections.Generic.IEnumerator<TItems> GetEnumerator() => InnerEnumerable.GetEnumerator();
        }

        public abstract class CountableEnumerableSelector<TEnumerable, TSourceItems, TDestinationItems> : CountableEnumerable<TEnumerable, TSourceItems, TDestinationItems> where TEnumerable : IReadOnlyCollection<TSourceItems>
        {
            protected Converter<TSourceItems, TDestinationItems> Selector { get; }

            protected CountableEnumerableSelector(TEnumerable enumerable, Converter<TSourceItems, TDestinationItems> selector) : base(enumerable) => Selector = selector;

            public override System.Collections.Generic.IEnumerator<TDestinationItems> GetEnumerator() => InnerEnumerable.GetEnumerator().Select(Selector);
        }

        public interface IList<T> : IReadOnlyList<T>, System.Collections.Generic.IList<T>, ICountableEnumerable<T>, IReadOnlyCollection<T>
        {
            new T this[int index] { get; set; }
        }

        public class List<T> : IList<T>
        {
            #region Properties
            protected System.Collections.Generic.IList<T> InnerList { get; }

            public int Count => InnerList.Count;

            public T this[int index] { get => InnerList[index]; set => InnerList[index] = value; }

            public bool IsReadOnly => InnerList.IsReadOnly;
            #endregion

            public List(System.Collections.Generic.IList<T> innerList) => InnerList = innerList;

            #region Methods
            public void Add(T item) => InnerList.Add(item);

            public bool Contains(T item) => InnerList.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => InnerList.CopyTo(array, arrayIndex);

            public System.Collections.Generic.IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            public int IndexOf(T item) => InnerList.IndexOf(item);

            public void Insert(int index, T item) => InnerList.Insert(index, item);

            public void RemoveAt(int index) => InnerList.RemoveAt(index);

            public bool Remove(T item) => InnerList.Remove(item);

            public void Clear() => InnerList.Clear();
            #endregion
        }
    }

    public static class Temp
    {
        public static TValue GetValue<TKey, TValue>(KeyValuePair<TKey, TValue> keyValuePair) => keyValuePair.Value;

        public static TDestination[] ToArray<TSource, TDestination>(this TSource[] array, Converter<TSource, TDestination> selector) => ToArray(array, 0, array.Length, selector);

        public static TDestination[] ToArray<TSource, TDestination>(this TSource[] array, int startIndex, int length, Converter<TSource, TDestination> selector)
        {
            ThrowIfNull(array, nameof(array));
            ThrowIfNull(selector, nameof(selector));
            ThrowOnInvalidCopyToArrayOperation(array, startIndex, length, nameof(array), nameof(startIndex));

            var result = new TDestination[array.Length];

            for (int i = 0; i < length; i++)

                result[i + startIndex] = selector(array[i]);

            return result;
        }

        public static TDestination[] ToArray<TSource, TDestination>(this IReadOnlyList<TSource> list, Converter<TSource, TDestination> selector) => ToArray(list, 0, list.Count, selector);

        public static TDestination[] ToArray<TSource, TDestination>(this IReadOnlyList<TSource> list, int startIndex, int length, Converter<TSource, TDestination> selector)
        {
            ThrowIfNull(list, nameof(list));
            ThrowIfNull(selector, nameof(selector));
            ThrowOnInvalidCopyToArrayOperation(array, startIndex, length, nameof(array), nameof(startIndex));

            var result = new TDestination[list.Count];

            for (int i = 0; i < length; i++)

                result[i + startIndex] = selector(list[i]);

            return result;
        }

        public interface IProperty
        {
            bool IsEnabled { get; }

            string Name { get; }

            string DisplayName { get; }

            string Description { get; }

            string EditInvitation { get; }

            object PropertyGroup { get; }

            object Value { get; }

            Type Type { get; }

            // string GetDisplayGroupName();
        }

        public interface IProperty<T> : IProperty
        {
            new T PropertyGroup { get; }
        }

        public class ReadOnlyList<TEnumerable, TSource, TDestination> : CountableEnumerableSelector<TEnumerable, TSource, TDestination>, IReadOnlyList<TDestination> where TEnumerable : IReadOnlyList<TSource>
        {
            public TDestination this[int index] => Selector(InnerEnumerable[index]);

            public ReadOnlyList(TEnumerable innerList, Converter<TSource, TDestination> selector) : base(innerList, selector) { /* Left empty. */ }
        }

        public class ReadOnlyList<TSource, TDestination> : ReadOnlyList<IReadOnlyList<TSource>, TSource, TDestination>
        {
            public ReadOnlyList(IReadOnlyList<TSource> innerList, Converter<TSource, TDestination> selector) : base(innerList, selector) { /* Left empty. */ }
        }

        public class List<TSource, TDestination> : ReadOnlyList<WinCopies.Collections.Abstract.Generic.IList<TSource>, TSource, TDestination>, System.Collections.Generic.IList<TDestination>
        {
            protected Converter<TDestination, TSource> ReversedSelector { get; }

            public new TDestination this[int index] { get => base[index]; set => InnerEnumerable[index] = ReversedSelector(value); }

            public bool IsReadOnly => InnerEnumerable.IsReadOnly;

            public List(WinCopies.Collections.Abstract.Generic.IList<TSource> innerList, Converter<TSource, TDestination> selector, Converter<TDestination, TSource> reversedSelector) : base(innerList, selector) => ReversedSelector = reversedSelector;

            public void Add(TDestination item) => InnerEnumerable.Add(ReversedSelector(item));

            public void Clear() => InnerEnumerable.Clear();

            public bool Contains(TDestination item) => InnerEnumerable.Contains(ReversedSelector(item));

            public void CopyTo(TDestination[] array, int arrayIndex) => InnerEnumerable.ToArray(arrayIndex, Count, Selector);

            public int IndexOf(TDestination item) => InnerEnumerable.IndexOf(ReversedSelector(item));

            public void Insert(int index, TDestination item) => InnerEnumerable.Insert(index, ReversedSelector(item));

            public bool Remove(TDestination item) => InnerEnumerable.Remove(ReversedSelector(item));

            public void RemoveAt(int index) => InnerEnumerable.RemoveAt(index);
        }

        public abstract class ConverterBase<TSource, TParam, TDestination> : ConverterBase
        {
            public abstract TDestination Convert(TSource value, TParam parameter, CultureInfo culture);

            public abstract TSource ConvertBack(TDestination value, TParam parameter, CultureInfo culture);

            public sealed override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value != null && !(value is TSource))

                    throw new ArgumentException($"{nameof(value)} must be null or from {nameof(TSource)}.");

                if (parameter != null && !(parameter is TParam))

                    throw new ArgumentException($"{nameof(parameter)} must be null or from {nameof(TParam)}.");

                TDestination convert(in TSource _value, in TParam _parameter) => Convert(_value, _parameter, culture);

                if (value == null)

                    if (parameter == null)

                        return convert(default, default);

                    else

                        return convert(default, (TParam)parameter);

                else if (parameter == null)

                    return convert((TSource)value, default);

                else

                    return convert((TSource)value, (TParam)parameter);
            }

            public sealed override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value != null && !(value is TDestination))

                    throw new ArgumentException($"{nameof(value)} must be null or from {nameof(TDestination)}.");

                if (parameter != null && !(parameter is TParam))

                    throw new ArgumentException($"{nameof(parameter)} must be null or from {nameof(TParam)}.");

                TSource convertBack(in TDestination _value, in TParam _parameter) => ConvertBack(_value, _parameter, culture);

                if (value == null)

                    if (parameter == null)

                        return convertBack(default, default);

                    else

                        return convertBack(default, (TParam)parameter);

                else if (parameter == null)

                    return convertBack((TDestination)value, default);

                else

                    return convertBack((TDestination)value, (TParam)parameter);
            }
        }
    }
}
#endif
