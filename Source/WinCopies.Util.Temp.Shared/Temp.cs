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
using System.Collections.Generic;
using System.Globalization;

using WinCopies.Util.Data;
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
    Util.Util;

using static WinCopies.Util.ThrowHelper
#endif
    ;
using System.Diagnostics;
using WinCopies.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Linq;

using WinCopies.Collections;
using System.Windows.Markup;
using System.Text;

#if !WinCopies3
using System.Collections;
using WinCopies.Util;
#endif

namespace WinCopies
{
    namespace Linq
    {
        public static class Temp
        {
            public static System.Collections.Generic.IEnumerable<TOut> SelectConverter<TIn, TOut>(this System.Collections.Generic.IEnumerable<TIn> enumerable, Converter<TIn, TOut> selector) => enumerable.Select(() => selector());
        }
    }

    // Already implemented in WinCopies.Util.

    public interface IReadOnlyList<out T> : System.Collections.Generic.IReadOnlyList<T>, ICountableEnumerable<T>
    {
        // Left empty.
    }

    namespace Markup
    {
        public abstract class ValueMarkupExtension<T> : MarkupExtension
        {
            public T Value { get; }

            public ValueMarkupExtension(in T value) => Value = value;

            public override object ProvideValue(IServiceProvider serviceProvider) => Value;
        }

        public class Boolean : ValueMarkupExtension<bool>
        {
            public Boolean(in bool value) : base(value) { /* Left empty. */ }

            public Boolean(in string value) : base(bool.Parse(value)) { /* Left empty. */ }
        }

        public class TrueValue : Boolean
        {
            public TrueValue() : base(true) { /* Left empty. */ }
        }

        public class FalseValue : Boolean
        {
            public FalseValue() : base(false) { /* Left empty. */ }
        }
    }

    public class InterfaceDataTemplateSelector : DataTemplateSelector
    {
        public static System.Collections.Generic.IEnumerable<Type> GetDirectInterfaces(Type t, bool ignoreGenerics, bool directTypeOnly)
        {
            var interfaces = new ArrayBuilder<Type>();
            var subInterfaces = new ArrayBuilder<Type>();

            void _addInterface(in Type i) => _ = interfaces.AddLast(i);

            void _addSubInterface(in Type i) => _ = subInterfaces.AddLast(i);

            void addNonGenericSubInterfaces(Type _t)
            {
                foreach (Type i in _t.GetInterfaces())

                    if (!i.IsGenericType)

                        _addSubInterface(i);
            }

            void addNonGenericInterfaces()
            {
                foreach (Type i in t.GetInterfaces())

                    if (!i.IsGenericType)
                    {
                        _addInterface(i);

                        addNonGenericSubInterfaces(i);
                    }
            }

            void addSubInterfaces(Type _t)
            {
                foreach (Type i in _t.GetInterfaces())

                    _addSubInterface(i);
            }

            void addInterfaces()
            {
                foreach (Type i in t.GetInterfaces())
                {
                    _addInterface(i);

                    addSubInterfaces(i);
                }
            }

            void addBaseTypesInterfaces(Action<Type> _action)
            {
                Type _t = t.BaseType;

                while (_t != null)
                {
                    _action(_t);

                    _t = _t.BaseType;
                }
            }

            Action action;

            if (ignoreGenerics)

                if (directTypeOnly)

                    action = () =>
                    {
                        addNonGenericInterfaces();

                        addBaseTypesInterfaces(addNonGenericSubInterfaces);
                    };

                else

                    action = addNonGenericInterfaces;

            else if (directTypeOnly)

                action = () =>
                {
                    addInterfaces();

                    addBaseTypesInterfaces(addSubInterfaces);
                };


            else

                action = addInterfaces;

            action();

            return ((System.Collections.Generic.IEnumerable<Type>)interfaces).Except(subInterfaces);
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null ||
#if !CS9
                !(
#endif
                container is
#if CS9
                not
#endif
                FrameworkElement containerElement
#if !CS9
                )
#endif
                )

                return base.SelectTemplate(item, container);

            Type itemType = item.GetType();

            foreach (var i in GetDirectInterfaces(itemType, true, true))

                Debug.WriteLine(item.GetType().ToString() + " " + i.Name);

            return System.Linq.Enumerable.Repeat(itemType, 1).Concat(GetDirectInterfaces(itemType, true, true))
                    .FirstOrDefault<DataTemplate>(t => containerElement.TryFindResource(new DataTemplateKey(t))) ?? base.SelectTemplate(item, container);
        }
    }

    public class TypeConverterEnumerator<T> : Enumerator<T>
    {
        private T _current;

        protected System.Collections.IEnumerator InnerEnumerator { get; }

        protected override T CurrentOverride => _current;

        public override bool? IsResetSupported => null;

        public TypeConverterEnumerator(in System.Collections.IEnumerator enumerator) => InnerEnumerator = enumerator;

        public TypeConverterEnumerator(in System.Collections.IEnumerable enumerable) : this(enumerable.GetEnumerator()) { /* Left empty. */ }

        protected override bool MoveNextOverride()
        {
            while (InnerEnumerator.MoveNext())

                if (InnerEnumerator.Current is T current)
                {
                    _current = current;

                    return true;
                }

            return false;
        }
    }

    public delegate T Converter<T>(object obj);

    public static class Extensions
    {
        public static System.Collections.Generic.IEnumerable<T> AppendValues<T>(this System.Collections.Generic.IEnumerable<T> enumerable, System.Collections.Generic.IEnumerable<T> values)
        {
            foreach (T value in enumerable)

                yield return value;

            foreach (T _value in values)

                yield return _value;
        }

        public static System.Collections.Generic.IEnumerable<T> AppendValues<T>(this System.Collections.Generic.IEnumerable<T> enumerable, params T[] values) => enumerable.AppendValues((System.Collections.Generic.IEnumerable<T>)values);

        public static bool HasFlag(this Enum @enum, System.Collections.Generic.IEnumerable<Enum> values)
        {
            foreach (Enum value in values)

                if (@enum.HasFlag(value))

                    return true;

            return false;
        }

        public static bool HasFlag(this Enum @enum, params Enum[] values) => @enum.HasFlag((System.Collections.Generic.IEnumerable<Enum>)values);

        public static bool HasAllFlags(this Enum @enum, System.Collections.Generic.IEnumerable<Enum> values)
        {
            foreach (Enum value in values)

                if (!@enum.HasFlag(value))

                    return false;

            return true;
        }

        public static bool HasAllFlags(this Enum @enum, params Enum[] values) => @enum.HasAllFlags((System.Collections.Generic.IEnumerable<Enum>)values);

        /// <summary>
        /// Iterates through a given <see cref="System.Collections.IEnumerable"/> and tries to convert the items to a given generic type parameter. If an item cannot be converted, it is ignored in the resulting enumerable.
        /// </summary>
        /// <typeparam name="T">The generic type parameter for the resulting enumerable. Only the items that can be converted to this type will be present in the resulting enumerable.</typeparam>
        /// <param name="enumerable">The source enumerable.</param>
        /// <returns>An enumerable containing all the items from <paramref name="enumerable"/> that could be converted to <typeparamref name="T"/>.</returns>
        /// <seealso cref="To{T}(System.Collections.IEnumerable)"/>
        public static System.Collections.Generic.IEnumerable<T> As<T>(this System.Collections.IEnumerable enumerable) => new Enumerable<T>(() => new TypeConverterEnumerator<T>(enumerable));

        public static System.Collections.Generic.IEnumerable<T> SelectConverter<T>(this System.Collections.IEnumerable enumerable, Converter<T> converter)
        {
            foreach (object value in enumerable)

                yield return converter(value);
        }

        /// <summary>
        /// Iterates through a given <see cref="System.Collections.IEnumerable"/> and directly converts the items to a given generic type parameter. An <see cref="InvalidCastException"/> is thrown when an item cannot be converted.
        /// </summary>
        /// <typeparam name="T">The generic type parameter for the resulting enumerable. All items in <paramref name="enumerable"/> will be converted to this type.</typeparam>
        /// <param name="enumerable">The source enumerable.</param>
        /// <returns>An enumerable containing the same items as they from <paramref name="enumerable"/>, with these items converted to <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidCastException">An item could not be converted.</exception>
        /// <seealso cref="As{T}(System.Collections.IEnumerable)"/>
        public static System.Collections.Generic.IEnumerable<T> To<T>(this System.Collections.IEnumerable enumerable) => SelectConverter(enumerable, value => (T)value);

        public static bool EndsWith(this string s, params char[] values)
        {
            foreach (char value in values)

                if (s.EndsWith(value))

                    return true;

            return false;
        }

        public static bool EndsWith(this string s, params string[] values)
        {
            foreach (string value in values)

                if (s.EndsWith(value))

                    return true;

            return false;
        }

        // Already implemented in WinCopies.Util.

        public static bool IsAssignableFrom<T>(this Type t)
        {
            ThrowIfNull(t, nameof(t));

            Type from = typeof(T);

            if (from == t)

                return true;

            if (t.IsInterface)

                return t.IsType(from.GetInterfaces());

            if (from.IsInterface)

                return false;

            from = from.BaseType;

            while (from != null)
            {
                if (from == t)

                    return true;

                from = from.BaseType;
            }

            return false;
        }

        public static
#if !WinCopies3
System.Collections.Generic.IEnumerator
#else
            IEnumeratorInfo2
#endif
            <TDestination> Select<TSource, TDestination>(this System.Collections.Generic.IEnumerator<TSource> enumerator, Converter<TSource, TDestination> func) => new SelectEnumerator<TSource, TDestination>(enumerator, value => func(value));
    }

    namespace Util
    {
        public static class ExtensionsTemp
        {
            // Already implemented in WinCopies.Util.

            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool DeleteObject([In] IntPtr hObject);

            // Already implemented in WinCopies.Util.

            private static ImageSource _ToImageSource(Bitmap bitmap)
            {
                bitmap.MakeTransparent();

                IntPtr hBitmap = bitmap.GetHbitmap();

                ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                if (!DeleteObject(hBitmap))

                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                return wpfBitmap;

                //            //using (MemoryStream stream = new MemoryStream())
                //            //{
                //            //    bitmap.Save(stream, ImageFormat.Png); // Was .Bmp, but this did not show a transparent background.

                //            //    stream.Position = 0;
                //            //    BitmapImage result = new BitmapImage();
                //            //    result.BeginInit();
                //            //    // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                //            //    // Force the bitmap to load right now so we can dispose the stream.
                //            //    result.CacheOption = BitmapCacheOption.OnLoad;
                //            //    result.StreamSource = stream;
                //            //    result.EndInit();
                //            //    result.Freeze();
                //            //    return result;
                //            //}

                //            return wpfBitmap;
            }

            // Already implemented in WinCopies.Util.

            public static ImageSource ToImageSource(this Icon icon) => _ToImageSource((icon ?? throw GetArgumentNullException(nameof(icon))).ToBitmap());

            // https://stackoverflow.com/questions/5689674/c-sharp-convert-wpf-image-source-to-a-system-drawing-bitmap

            // Already implemented in WinCopies.Util.

            public static Bitmap ToBitmap(this BitmapSource bitmapSource)
            {
                ThrowIfNull(bitmapSource, nameof(bitmapSource));

                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;
                int stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
                IntPtr ptr = IntPtr.Zero;

                try
                {
                    ptr = Marshal.AllocHGlobal(height * stride);
                    bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);

                    using (var bitmap = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, ptr))

                        // Clone the bitmap so that we can dispose it and
                        // release the unmanaged memory at ptr
                        return new Bitmap(bitmap);
                }

                finally
                {
                    if (ptr != IntPtr.Zero)

                        Marshal.FreeHGlobal(ptr);
                }
            }
        }
    }

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

        public interface IList<T> : System.Collections.Generic.IReadOnlyList<T>, System.Collections.Generic.IList<T>, ICountableEnumerable<T>, IReadOnlyCollection<T>
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
        public static System.Collections.Generic.IEnumerable<T> GetEmptyEnumerable<T>() => new WinCopies.Collections.Generic.Enumerable<T>(() => new EmptyEnumerator<T>());

        // https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/

        // Already implemented in WinCopies.Util.

        public static Process StartProcessNetCore(in string url) =>

             // hack because of this: https://github.com/dotnet/corefx/issues/10361
             // if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))

             Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}") { CreateNoWindow = true });

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

        public static TDestination[] ToArray<TSource, TDestination>(this System.Collections.Generic.IReadOnlyList<TSource> list, Converter<TSource, TDestination> selector) => ToArray(list, 0, list.Count, selector);

        // Already implemented in WinCopies.Util.

        public static ArgumentException GetArrayHasNotEnoughSpaceException(in string arrayArgumentName) => new ArgumentException("", arrayArgumentName);

        // Already implemented in WinCopies.Util.

        public static void ThrowIfArrayHasNotEnoughSpace<T>(in IReadOnlyCollection<T> array, in int arrayIndex, in int count, in string arrayArgumentName)
        {
            if (count <= array.Count - arrayIndex)

                throw GetArrayHasNotEnoughSpaceException(arrayArgumentName);
        }

        public static void ThrowIfIndexIsLowerThanZero(in int index, in string indexArgumentName)
        {
            if (index < 0)

                throw new
#if !WinCopies3
                    ArgumentOutOfRangeException
#else
                    IndexOutOfRangeException
#endif
                    (indexArgumentName);
        }

        public static void ThrowOnInvalidCopyToArrayOperation<T>(in IReadOnlyCollection<T> array, in int arrayIndex, in int count, in string arrayArgumentName, in string arrayIndexArgumentName)
        {
            ThrowIfNull(array, nameof(array));

            ThrowIfIndexIsLowerThanZero(arrayIndex, arrayIndexArgumentName);

            //if (array.GetLowerBound(0) != 0)

            //    throw GetArrayHasNonZeroLowerBoundException(arrayArgumentName);

            ThrowIfArrayHasNotEnoughSpace(array, arrayIndex, count, arrayArgumentName);
        }

        public static TDestination[] ToArray<TSource, TDestination>(this System.Collections.Generic.IReadOnlyList<TSource> list, int startIndex, int length, Converter<TSource, TDestination> selector)
        {
            ThrowIfNull(list, nameof(list));
            ThrowIfNull(selector, nameof(selector));
            ThrowOnInvalidCopyToArrayOperation(list, startIndex, length, nameof(list), nameof(startIndex));

            var result = new TDestination[list.Count];

            for (int i = 0; i < length; i++)

                result[i + startIndex] = selector(list[i]);

            return result;
        }

        public interface IReadOnlyProperty
        {
            // bool IsEnabled { get; }

            string Name { get; }

            string DisplayName { get; }

            string Description { get; }

            string EditInvitation { get; }

            object PropertyGroup { get; }

            object Value { get; }

            Type Type { get; }

            // string GetDisplayGroupName();
        }

        public interface IProperty : IReadOnlyProperty
        {
            bool IsReadOnly { get; }
        }

        public interface IReadOnlyProperty<T> : IReadOnlyProperty
        {
            new T PropertyGroup { get; }
        }

        public interface IProperty<T> : IReadOnlyProperty<T>, IProperty
        {
            new T PropertyGroup { get; }
        }

        public class ReadOnlyList<TEnumerable, TSource, TDestination> : CountableEnumerableSelector<TEnumerable, TSource, TDestination>, System.Collections.Generic.IReadOnlyList<TDestination> where TEnumerable : System.Collections.Generic.IReadOnlyList<TSource>
        {
            public TDestination this[int index] => Selector(InnerEnumerable[index]);

            public ReadOnlyList(TEnumerable innerList, Converter<TSource, TDestination> selector) : base(innerList, selector) { /* Left empty. */ }
        }

        public class ReadOnlyList<TSource, TDestination> : ReadOnlyList<System.Collections.Generic.IReadOnlyList<TSource>, TSource, TDestination>
        {
            public ReadOnlyList(System.Collections.Generic.IReadOnlyList<TSource> innerList, Converter<TSource, TDestination> selector) : base(innerList, selector) { /* Left empty. */ }
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

                return value == null
                    ? parameter == null ? convert(default, default) : (object)convert(default, (TParam)parameter)
                    : parameter == null ? convert((TSource)value, default) : (object)convert((TSource)value, (TParam)parameter);
            }

            public sealed override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value != null && !(value is TDestination))

                    throw new ArgumentException($"{nameof(value)} must be null or from {nameof(TDestination)}.");

                if (parameter != null && !(parameter is TParam))

                    throw new ArgumentException($"{nameof(parameter)} must be null or from {nameof(TParam)}.");

                TSource convertBack(in TDestination _value, in TParam _parameter) => ConvertBack(_value, _parameter, culture);

                return value == null
                    ? parameter == null ? convertBack(default, default) : (object)convertBack(default, (TParam)parameter)
                    : parameter == null ? convertBack((TDestination)value, default) : (object)convertBack((TDestination)value, (TParam)parameter);
            }
        }

        public static class ThrowHelper
        {
            public static ArgumentException GetArgumentException(in object obj, in string argumentName, in Type t) => new ArgumentException($"{argumentName} must be an instance of {t.Name}. {argumentName} is {(obj == null ? "null" : obj.GetType().Name)}.");

            public static ArgumentException GetArgumentException<T>(in object obj, in string argumentName) => GetArgumentException(obj, argumentName, typeof(T));
        }
    }
}
#endif
