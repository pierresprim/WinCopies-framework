﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Input;
using static WinCopies.Util.Generic;

namespace WinCopies.Util
{
    public static class Util
    {

        public const BindingFlags DefaultBindingFlagsForPropertySet = BindingFlags.Public | BindingFlags.NonPublic |
                         BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static (bool propertyChanged, object oldValue) SetPropertyWhenNotBusy<T>(T bgWorker, string propertyName, string fieldName, object newValue, Type declaringType, bool performIntegrityCheck = true, BindingFlags bindingFlags = DefaultBindingFlagsForPropertySet, bool throwIfBusy = true) where T : IBackgroundWorker, INotifyPropertyChanged

        {

            if (bgWorker.IsBusy)

                if (throwIfBusy)

                    throw new InvalidOperationException("Cannot change property value when BackgroundWorker is busy.");

                else

                    return (false, Extensions.GetField(fieldName, declaringType, bindingFlags).GetValue(bgWorker));

            else

                return bgWorker.SetProperty(propertyName, fieldName, newValue, declaringType, performIntegrityCheck, bindingFlags);

        }

        public static Predicate<T> GetCommonPredicate<T>() => (T value) => true;

        public static void ThrowOnNotValidEnumValue(params Enum[] values)

        {

            foreach (Enum value in values)

                value.ThrowIfNotValidEnumValue();

        }

        // public static KeyValuePair<TKey, Func<bool>>[] GetIfKeyValuePairPredicateArray<TKey>(params KeyValuePair<TKey, Func<bool>>[] keyValuePairs) => keyValuePairs;

        #region 'If' methods

        public static KeyValuePair<TKey, TValue> GetKeyValuePair<TKey, TValue>(TKey key, TValue value) => new KeyValuePair<TKey, TValue>(key, value);

        public static KeyValuePair<TKey, Func<bool>> GetIfKeyValuePairPredicate<TKey>(TKey key, Func<bool> predicate) => new KeyValuePair<TKey, Func<bool>>(key, predicate);

        #region Enums

        /// <summary>
        /// Comparison types for the If functions.
        /// </summary>
        public enum ComparisonType

        {

            /// <summary>
            /// Check if all conditions are checked.
            /// </summary>
            And = 0,

            /// <summary>
            /// Check if at least one condition is checked.
            /// </summary>
            Or = 1,

            /// <summary>
            /// Check if exactly one condition is checked.
            /// </summary>
            Xor = 2

        }

        /// <summary>
        /// Comparison modes for the If functions.
        /// </summary>
        public enum ComparisonMode
        {

            /// <summary>
            /// Use a binary comparison
            /// </summary>
            Binary = 0,

            /// <summary>
            /// Use a logical comparison
            /// </summary>
            Logical = 1

        }

        /// <summary>
        /// Comparison to perform.
        /// </summary>
        public enum Comparison

        {

            /// <summary>
            /// Check for values equality
            /// </summary>
            Equal = 0,

            /// <summary>
            /// Check for values non-equality
            /// </summary>
            NotEqual = 1,

            /// <summary>
            /// Check if a value is lesser than a given value. This field only works for methods that use lesser/greater/equal comparers.
            /// </summary>
            Lesser = 2,

            /// <summary>
            /// Check if a value is lesser than or equal to a given value. This field only works for methods that use lesser/greater/equal comparers.
            /// </summary>
            LesserOrEqual = 3,

            /// <summary>
            /// Check if a value is greater than a given value. This field only works for methods that use lesser/greater/equal comparers.
            /// </summary>
            Greater = 4,

            /// <summary>
            /// Check if a value is greater than or equal to a given value. This field only works for methods that use lesser/greater/equal comparers.
            /// </summary>
            GreaterOrEqual = 5,

            /// <summary>
            /// Check if an object reference is equal to a given object reference. This field only works for methods that use equality comparers (not lesser/greater ones).
            /// </summary>
            ReferenceEqual = 6

        }

        #endregion

        #region 'Throw' methods

        private static void ThrowOnInvalidIfMethodArg(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison)

        {

            ThrowOnNotValidEnumValue(comparisonType, comparisonMode, comparison);

            if (comparison == Comparison.ReferenceEqual)

                throw new InvalidEnumArgumentException(nameof(comparison), (int)Comparison.ReferenceEqual, typeof(Comparison));

        }

        private static void ThrowOnInvalidEqualityIfMethodEnumValue(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison)

        {

            ThrowOnNotValidEnumValue(comparisonType, comparisonMode);

            if (!(comparison == Comparison.Equal || comparison == Comparison.NotEqual || comparison == Comparison.ReferenceEqual))

                // todo:

                throw new ArgumentException($"{comparison} must be equal to {nameof(Comparison.Equal)}, {nameof(Comparison.NotEqual)} or {nameof(Comparison.ReferenceEqual)}");

        }

        private static void ThrowOnInvalidEqualityIfMethodArg(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, Type valueType, IEqualityComparer equalityComparer)

        {

            ThrowOnInvalidEqualityIfMethodEnumValue(comparisonType, comparisonMode, comparison);

            if (comparison == Comparison.ReferenceEqual && equalityComparer != null)

                throw new ArgumentException($"{nameof(equalityComparer)} have to be set to null in order to use this method with the {nameof(Comparison.ReferenceEqual)} enum value.");

            if (comparison == Comparison.ReferenceEqual && !valueType.GetType().IsClass) throw new InvalidOperationException("ReferenceEqual comparison is only valid with class types.");

        }

        private static void ThrowOnInvalidEqualityIfMethodArg<T>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, IEqualityComparer<T> equalityComparer)

        {

            ThrowOnInvalidEqualityIfMethodEnumValue(comparisonType, comparisonMode, comparison);

            if (comparison == Comparison.ReferenceEqual && equalityComparer != null)

                throw new ArgumentException($"{nameof(equalityComparer)} have to be set to null in order to use this method with the {nameof(Comparison.ReferenceEqual)} enum value.");

            if (comparison == Comparison.ReferenceEqual && !typeof(T).IsClass) throw new InvalidOperationException("ReferenceEqual comparison is only valid with class types.");

        }

        #endregion

        #region 'Check comparison' methods

        private static bool CheckIfComparison(Comparison comparison, bool predicateResult, int result)
        {
            switch (comparison)

            {

                case Comparison.Equal:
                case Comparison.ReferenceEqual:

                    return predicateResult && result == 0;

                case Comparison.LesserOrEqual:

                    return result <= 0;

                case Comparison.GreaterOrEqual:

                    return result >= 0;

                case Comparison.Lesser:

                    return !predicateResult && result < 0;

                case Comparison.Greater:

                    return !predicateResult && result > 0;

                case Comparison.NotEqual:

                    return !predicateResult && result != 0;

                default:

                    return false;//: comparisonType == ComparisonType.Or ?//(result == 0 && (comparison == Comparison.Equals || comparison == Comparison.LesserOrEquals || comparison == Comparison.GreaterOrEquals)) ||//    (result < 0 && (comparison == Comparison.DoesNotEqual || comparison == Comparison.LesserThan || comparison == Comparison.LesserOrEquals)) ||//    (result > 0 && (comparison == Comparison.DoesNotEqual || comparison == Comparison.GreaterThan || comparison == Comparison.GreaterOrEquals))

            }
        }

        private static bool CheckEqualityComparison(Comparison comparison, object value, object valueToCompare, bool predicateResult, IEqualityComparer equalityComparer)
        {

            if (comparison == Comparison.ReferenceEqual && !value.GetType().IsClass) throw new InvalidOperationException("ReferenceEqual comparison is only valid with class types.");

            switch (comparison)

            {

                case Comparison.Equal:

                    return predicateResult && equalityComparer.Equals(value, valueToCompare);

                case Comparison.NotEqual:

                    return !predicateResult && !equalityComparer.Equals(value, valueToCompare);

                case Comparison.ReferenceEqual:

#pragma warning disable IDE0002
                    return object.ReferenceEquals(value, valueToCompare);
#pragma warning restore IDE0002

                default:

                    return false;

            }

        }

        private static bool CheckEqualityComparison<T>(Comparison comparison, T value, T valueToCompare, bool predicateResult, IEqualityComparer<T> equalityComparer)
        {

            // Because we've already checked that for the 'T' type in the 'If' method and assuming that 'T' is the base type of all the values to test, if 'T' is actually a class, we don't need to check here if the type of the current value is actually a class when comparison is set to ReferenceEqual.

            switch (comparison)

            {

                case Comparison.Equal:

                    return predicateResult && equalityComparer.Equals(value, valueToCompare);

                case Comparison.NotEqual:

                    return !predicateResult && !equalityComparer.Equals(value, valueToCompare);

                case Comparison.ReferenceEqual:

#pragma warning disable IDE0002
                    return object.ReferenceEquals(value, valueToCompare);
#pragma warning restore IDE0002

                default:

                    return false;

            }

        }

        private delegate bool CheckIfComparisonDelegate(object value, Func<bool> predicate);

        private delegate bool CheckIfComparisonDelegate<T>(T value, Func<bool> predicate);

        #endregion

        #region Enumerables

        private interface IIfValuesEnumerable : IEnumerable<KeyValuePair<object, Func<bool>>>
        {

            Array Array { get; }

            KeyValuePair<object, Func<bool>> GetValue(int index);

        }

        private class IfValuesEnumerable : IIfValuesEnumerable
        {

            private static KeyValuePair<object, Func<bool>> GetValue(object[] array, int index, Predicate<object> predicate)

            {

                object result = array[index];

                return new KeyValuePair<object, Func<bool>>(result, () => predicate(result));

            }

            private class IfValuesEnumerator : IEnumerator<KeyValuePair<object, Func<bool>>>
            {

                private int _currentIndex;

                public KeyValuePair<object, Func<bool>> Current => GetValue(_array, _currentIndex, _predicate);

                object IEnumerator.Current => Current;

                private object[] _array;

                private Predicate<object> _predicate;

                public IfValuesEnumerator(object[] array) => _array = array;

                public bool MoveNext()
                {

                    if (_currentIndex == _array.Length)

                        return false;

                    _currentIndex++;

                    return true;

                }

                public void Reset() => _currentIndex = -1;

                public void Dispose() { }
            }

            public object[] Array { get; }

            Array IIfValuesEnumerable.Array => Array;

            public Predicate<object> Predicate { get; }

            public IfValuesEnumerable(object[] array, Predicate<object> predicate)
            {

                Array = array;

                Predicate = predicate;

            }

            public IEnumerator<KeyValuePair<object, Func<bool>>> GetEnumerator() => new IfValuesEnumerator((object[])Array);

            IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

            public KeyValuePair<object, Func<bool>> GetValue(int index) => GetValue(Array, index, Predicate);

        }

        private class IfKeyValuePairEnumerable : IIfValuesEnumerable
        {

            private class IfKeyValuePairEnumerator : IEnumerator<KeyValuePair<object, Func<bool>>>
            {

                private int _currentIndex;

                private KeyValuePair<object, Func<bool>>[] _array;

                public KeyValuePair<object, Func<bool>> Current => _array[_currentIndex];

                public IfKeyValuePairEnumerator(KeyValuePair<object, Func<bool>>[] array) => _array = array;

                object IEnumerator.Current => Current;

                public void Dispose() { }

                public bool MoveNext()
                {

                    if (_currentIndex == _array.Length)

                        return false;

                    _currentIndex++;

                    return true;

                }

                public void Reset() => _currentIndex = -1;

            }

            public KeyValuePair<object, Func<bool>>[] Array { get; }

            Array IIfValuesEnumerable.Array => Array;

            public IfKeyValuePairEnumerable(KeyValuePair<object, Func<bool>>[] array) => Array = array;

            public IEnumerator<KeyValuePair<object, Func<bool>>> GetEnumerator() => new IfKeyValuePairEnumerator(Array);

            IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

            public KeyValuePair<object, Func<bool>> GetValue(int index) => Array[index];

        }

        private interface IIfKeyValuesEnumerable : IEnumerable<KeyValuePair<object, KeyValuePair<object, Func<bool>>>>
        {

            Array Array { get; }

            KeyValuePair<object, KeyValuePair<object, Func<bool>>> GetValue(int index);

        }

        private class IfKeyValuesEnumerable : IIfKeyValuesEnumerable
        {

            private static KeyValuePair<object, KeyValuePair<object, Func<bool>>> GetValue(KeyValuePair<object, object>[] array, int index, Predicate<object> predicate)

            {

                KeyValuePair<object, object> result = array[index];

                return new KeyValuePair<object, KeyValuePair<object, Func<bool>>>(result.Key, new KeyValuePair<object, Func<bool>>(result.Value, () => predicate(result.Value)));

            }

            private class IfKeyValuesEnumerator : IEnumerator<KeyValuePair<object, KeyValuePair<object, Func<bool>>>>
            {

                private int _currentIndex;

                public KeyValuePair<object, KeyValuePair<object, Func<bool>>> Current => GetValue(_array, _currentIndex, _predicate);

                object IEnumerator.Current => Current;

                private KeyValuePair<object, object>[] _array;

                private Predicate<object> _predicate;

                public IfKeyValuesEnumerator(KeyValuePair<object, object>[] array) => _array = array;

                public bool MoveNext()
                {

                    if (_currentIndex == _array.Length)

                        return false;

                    _currentIndex++;

                    return true;

                }

                public void Reset() => _currentIndex = -1;

                public void Dispose() { }
            }

            public KeyValuePair<object, object>[] Array { get; }

            Array IIfKeyValuesEnumerable.Array => Array;

            public Predicate<object> Predicate { get; }

            public IfKeyValuesEnumerable(KeyValuePair<object, object>[] array, Predicate<object> predicate)
            {

                Array = array;

                Predicate = predicate;

            }

            public IEnumerator<KeyValuePair<object, KeyValuePair<object, Func<bool>>>> GetEnumerator() => new IfKeyValuesEnumerator((KeyValuePair<object, object>[])Array);

            IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

            public KeyValuePair<object, KeyValuePair<object, Func<bool>>> GetValue(int index) => GetValue(Array, index, Predicate);

        }

        private class IfKeyKeyValuePairEnumerable : IIfKeyValuesEnumerable
        {

            private class IfKeyKeyValuePairEnumerator : IEnumerator<KeyValuePair<object, KeyValuePair<object, Func<bool>>>>
            {

                private int _currentIndex;

                private KeyValuePair<object, KeyValuePair<object, Func<bool>>>[] _array;

                public KeyValuePair<object, KeyValuePair<object, Func<bool>>> Current => _array[_currentIndex];

                public IfKeyKeyValuePairEnumerator(KeyValuePair<object, KeyValuePair<object, Func<bool>>>[] array) => _array = array;

                object IEnumerator.Current => Current;

                public void Dispose() { }

                public bool MoveNext()
                {

                    if (_currentIndex == _array.Length)

                        return false;

                    _currentIndex++;

                    return true;

                }

                public void Reset() => _currentIndex = -1;

            }

            public KeyValuePair<object, KeyValuePair<object, Func<bool>>>[] Array { get; }

            Array IIfKeyValuesEnumerable.Array => Array;

            public IfKeyKeyValuePairEnumerable(KeyValuePair<object, KeyValuePair<object, Func<bool>>>[] array) => Array = array;

            public IEnumerator<KeyValuePair<object, KeyValuePair<object, Func<bool>>>> GetEnumerator() => new IfKeyKeyValuePairEnumerator(Array);

            IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

            public KeyValuePair<object, KeyValuePair<object, Func<bool>>> GetValue(int index) => Array[index];

        }

        private interface IIfValuesEnumerable<T> : IEnumerable<KeyValuePair<T, Func<bool>>>
        {

            Array Array { get; }

            KeyValuePair<T, Func<bool>> GetValue(int index);

        }

        private class IfValuesEnumerable<T> : IIfValuesEnumerable<T>
        {

            private static KeyValuePair<T, Func<bool>> GetValue(T[] array, int index, Predicate<T> predicate)

            {

                T result = array[index];

                return new KeyValuePair<T, Func<bool>>(result, () => predicate(result));

            }

            private class IfValuesEnumerator : IEnumerator<KeyValuePair<T, Func<bool>>>
            {

                private int _currentIndex;

                public KeyValuePair<T, Func<bool>> Current => GetValue((T[])_array, _currentIndex, _predicate);

                object IEnumerator.Current => Current;

                private T[] _array;

                private Predicate<T> _predicate;

                public IfValuesEnumerator(T[] array) => _array = array;

                public bool MoveNext()
                {

                    if (_currentIndex == _array.Length)

                        return false;

                    _currentIndex++;

                    return true;

                }

                public void Reset() => _currentIndex = -1;

                public void Dispose() { }
            }

            public T[] Array { get; }

            Array IIfValuesEnumerable<T>.Array => Array;

            public Predicate<T> Predicate { get; }

            public IfValuesEnumerable(T[] array, Predicate<T> predicate)
            {

                Array = array;

                Predicate = predicate;

            }

            public IEnumerator<KeyValuePair<T, Func<bool>>> GetEnumerator() => new IfValuesEnumerator((T[])Array);

            IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

            public KeyValuePair<T, Func<bool>> GetValue(int index) => GetValue(Array, index, Predicate);

        }

        private class IfKeyValuePairEnumerable<T> : IIfValuesEnumerable<T>
        {

            private class IfKeyValuePairEnumerator : IEnumerator<KeyValuePair<T, Func<bool>>>
            {

                private int _currentIndex;

                private KeyValuePair<T, Func<bool>>[] _array;

                public KeyValuePair<T, Func<bool>> Current => _array[_currentIndex];

                public IfKeyValuePairEnumerator(KeyValuePair<T, Func<bool>>[] array) => _array = array;

                object IEnumerator.Current => Current;

                public void Dispose() { }

                public bool MoveNext()
                {

                    if (_currentIndex == _array.Length)

                        return false;

                    _currentIndex++;

                    return true;

                }

                public void Reset() => _currentIndex = -1;

            }

            public KeyValuePair<T, Func<bool>>[] Array { get; }

            Array IIfValuesEnumerable<T>.Array => Array;

            public IfKeyValuePairEnumerable(KeyValuePair<T, Func<bool>>[] array) => Array = array;

            public IEnumerator<KeyValuePair<T, Func<bool>>> GetEnumerator() => new IfKeyValuePairEnumerator(Array);

            IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

            public KeyValuePair<T, Func<bool>> GetValue(int index) => Array[index];

        }

        private interface IIfKeyValuesEnumerable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>>
        {

            Array Array { get; }

            KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> GetValue(int index);

        }

        private class IfKeyValuesEnumerable<TKey, TValue> : IIfKeyValuesEnumerable<TKey, TValue>
        {

            private static KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> GetValue(KeyValuePair<TKey, TValue>[] array, int index, Predicate<TValue> predicate)

            {

                KeyValuePair<TKey, TValue> result = array[index];

                return new KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>(result.Key, new KeyValuePair<TValue, Func<bool>>(result.Value, () => predicate(result.Value)));

            }

            private class IfKeyValuesEnumerator : IEnumerator<KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>>
            {

                private int _currentIndex;

                public KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> Current => GetValue(_array, _currentIndex, _predicate);

                object IEnumerator.Current => Current;

                private KeyValuePair<TKey, TValue>[] _array;

                private Predicate<TValue> _predicate;

                public IfKeyValuesEnumerator(KeyValuePair<TKey, TValue>[] array) => _array = array;

                public bool MoveNext()
                {

                    if (_currentIndex == _array.Length)

                        return false;

                    _currentIndex++;

                    return true;

                }

                public void Reset() => _currentIndex = -1;

                public void Dispose() { }
            }

            public KeyValuePair<TKey, TValue>[] Array { get; }

            Array IIfKeyValuesEnumerable<TKey, TValue>.Array => Array;

            public Predicate<TValue> Predicate { get; }

            public IfKeyValuesEnumerable(KeyValuePair<TKey, TValue>[] array, Predicate<TValue> predicate)
            {

                Array = array;

                Predicate = predicate;

            }

            public IEnumerator<KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>> GetEnumerator() => new IfKeyValuesEnumerator((KeyValuePair<TKey, TValue>[])Array);

            IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

            public KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> GetValue(int index) => GetValue(Array, index, Predicate);

        }

        private class IfKeyKeyValuePairEnumerable<TKey, TValue> : IIfKeyValuesEnumerable<TKey, TValue>
        {

            private class IfKeyKeyValuePairEnumerator : IEnumerator<KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>>
            {

                private int _currentIndex;

                private KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>[] _array;

                public KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> Current => _array[_currentIndex];

                public IfKeyKeyValuePairEnumerator(KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>[] array) => _array = array;

                object IEnumerator.Current => Current;

                public void Dispose() { }

                public bool MoveNext()
                {

                    if (_currentIndex == _array.Length)

                        return false;

                    _currentIndex++;

                    return true;

                }

                public void Reset() => _currentIndex = -1;

            }

            public KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>[] Array { get; }

            Array IIfKeyValuesEnumerable<TKey, TValue>.Array => Array;

            public IfKeyKeyValuePairEnumerable(KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>[] array) => Array = array;

            public IEnumerator<KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>> GetEnumerator() => new IfKeyKeyValuePairEnumerator(Array);

            IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

            public KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> GetValue(int index) => Array[index];

        }

        #endregion

        private static bool IfInternal(ComparisonType comparisonType, ComparisonMode comparisonMode, CheckIfComparisonDelegate comparisonDelegate, IIfValuesEnumerable values)

        {

            bool checkIfComparison(KeyValuePair<object, Func<bool>> value) => comparisonDelegate(value.Key, value.Value);

            bool result;

            if (comparisonMode == ComparisonMode.Binary)

            {

                if (comparisonType == ComparisonType.And)

                {

                    result = true;

                    foreach (KeyValuePair<object, Func<bool>> _value in values)

                        result &= checkIfComparison(_value);

                }

                else

                {

                    result = false;

                    if (comparisonType == ComparisonType.Or)

                        foreach (KeyValuePair<object, Func<bool>> _value in values)

                            result |= checkIfComparison(_value);

                    else // Xor

                    {

                        bool alreadyTrue = false;

                        KeyValuePair<object, Func<bool>> _value;

                        int i;

                        for (i = 0; i < values.Array.Length; i++)
                        {

                            _value = values.GetValue(i);

                            if (checkIfComparison(_value))

                                if (alreadyTrue)

                                    break;

                                else

                                {

                                    result |= true;

                                    alreadyTrue = true;

                                }

                            else

                                result |= false;

                        }

                        if (alreadyTrue)

                            for (i += 1; i < values.Array.Length; i++)

                            {

                                _value = values.GetValue(i);

                                checkIfComparison(_value);

                            }
                    }

                }

            }

            // We check the comparison type for the 'and' comparison.

            else if (comparisonType == ComparisonType.And)

            {

                result = true;

                foreach (KeyValuePair<object, Func<bool>> _value in values)

                    if (!checkIfComparison(_value))

                    {

                        result = false;

                        break;

                    }

            }

            // We check the comparison type for the 'or' comparison.

            else if (comparisonType == ComparisonType.Or)

            {

                result = false;

                foreach (KeyValuePair<object, Func<bool>> _value in values)

                    if (checkIfComparison(_value))

                    {

                        result = true;

                        break;

                    }

            }

            else

            {

                result = false;

                KeyValuePair<object, Func<bool>> _value;

                KeyValuePair<object, Func<bool>> __value;

                for (int i = 0; i < values.Array.Length; i++)
                {

                    _value = values.GetValue(i);

                    result = true;

                    if (checkIfComparison(_value))

                    {

                        for (int j = i + 1; j < values.Array.Length; j++)

                        {

                            __value = values.GetValue(j);

                            if (checkIfComparison(__value))

                            {

                                result = false;

                                break;

                            }

                        }

                        break;

                    }

                }

            }

            return result;

        }

        private static bool IfInternal(ComparisonType comparisonType, ComparisonMode comparisonMode, CheckIfComparisonDelegate comparisonDelegate, out object key, IIfKeyValuesEnumerable values)

        {

            bool checkIfComparison(KeyValuePair<object, Func<bool>> value) => comparisonDelegate(value.Key, value.Value);

            object _key = null;

            bool result;

            if (comparisonMode == ComparisonMode.Binary)

            {

                if (comparisonType == ComparisonType.And)

                {

                    result = true;

                    foreach (KeyValuePair<object, KeyValuePair<object, Func<bool>>> _value in values)

                    {

                        result &= checkIfComparison(_value.Value);

                        if (_key == null && !result)

                            _key = _value.Key;

                    }

                }

                else

                {

                    result = false;

                    if (comparisonType == ComparisonType.Or)

                        foreach (KeyValuePair<object, KeyValuePair<object, Func<bool>>> _value in values)

                        {

                            result |= checkIfComparison(_value.Value);

                            if (_key == null && result)

                                _key = _value.Key;

                        }

                    else // Xor

                    {

                        bool alreadyTrue = false;

                        KeyValuePair<object, KeyValuePair<object, Func<bool>>> _value;

                        int i;

                        for (i = 0; i < values.Array.Length; i++)
                        {

                            _value = values.GetValue(i);

                            if (checkIfComparison(_value.Value))

                                if (alreadyTrue)

                                {

                                    _key = null;

                                    break;

                                }

                                else

                                {

                                    result |= true;

                                    _key = _value.Key;

                                    alreadyTrue = true;

                                }

                            else

                                result |= false;

                        }

                        if (alreadyTrue)

                            for (i += 1; i < values.Array.Length; i++)

                            {

                                _value = values.GetValue(i);

                                checkIfComparison(_value.Value);

                            }
                    }

                }

            }

            // We check the comparison type for the 'and' comparison.

            else if (comparisonType == ComparisonType.And)

            {

                result = true;

                foreach (KeyValuePair<object, KeyValuePair<object, Func<bool>>> _value in values)

                    if (!checkIfComparison(_value.Value))

                    {

                        result = false;

                        _key = _value.Key;

                        break;

                    }

            }

            // We check the comparison type for the 'or' comparison.

            else if (comparisonType == ComparisonType.Or)

            {

                result = false;

                foreach (KeyValuePair<object, KeyValuePair<object, Func<bool>>> _value in values)

                    if (checkIfComparison(_value.Value))

                    {

                        result = true;

                        _key = _value.Key;

                        break;

                    }

            }

            else

            {

                result = false;

                KeyValuePair<object, KeyValuePair<object, Func<bool>>> _value;

                KeyValuePair<object, KeyValuePair<object, Func<bool>>> __value;

                for (int i = 0; i < values.Array.Length; i++)
                {

                    _value = values.GetValue(i);

                    result = true;

                    if (checkIfComparison(_value.Value))

                    {

                        for (int j = i + 1; j < values.Array.Length; j++)

                        {

                            __value = values.GetValue(j);

                            if (checkIfComparison(__value.Value))

                            {

                                result = false;

                                break;

                            }

                        }

                        if (result)

                            _key = _value.Key;

                        break;

                    }

                }

            }

            key = _key;

            return result;

        }

        private static bool IfInternal<T>(ComparisonType comparisonType, ComparisonMode comparisonMode, CheckIfComparisonDelegate<T> comparisonDelegate, IIfValuesEnumerable<T> values)

        {

            bool checkIfComparison(KeyValuePair<T, Func<bool>> value) => comparisonDelegate(value.Key, value.Value);

            bool result;

            if (comparisonMode == ComparisonMode.Binary)

            {

                if (comparisonType == ComparisonType.And)

                {

                    result = true;

                    foreach (KeyValuePair<T, Func<bool>> _value in values)

                        result &= checkIfComparison(_value);

                }

                else

                {

                    result = false;

                    if (comparisonType == ComparisonType.Or)

                        foreach (KeyValuePair<T, Func<bool>> _value in values)

                            result |= checkIfComparison(_value);

                    else // Xor

                    {

                        bool alreadyTrue = false;

                        KeyValuePair<T, Func<bool>> _value;

                        int i;

                        for (i = 0; i < values.Array.Length; i++)
                        {

                            _value = values.GetValue(i);

                            if (checkIfComparison(_value))

                                if (alreadyTrue)

                                    break;

                                else

                                {

                                    result |= true;

                                    alreadyTrue = true;

                                }

                            else

                                result |= false;

                        }

                        if (alreadyTrue)

                            for (i += 1; i < values.Array.Length; i++)

                            {

                                _value = values.GetValue(i);

                                checkIfComparison(_value);

                            }
                    }

                }

            }

            // We check the comparison type for the 'and' comparison.

            else if (comparisonType == ComparisonType.And)

            {

                result = true;

                foreach (KeyValuePair<T, Func<bool>> _value in values)

                    if (!checkIfComparison(_value))

                    {

                        result = false;

                        break;

                    }

            }

            // We check the comparison type for the 'or' comparison.

            else if (comparisonType == ComparisonType.Or)

            {

                result = false;

                foreach (KeyValuePair<T, Func<bool>> _value in values)

                    if (checkIfComparison(_value))

                    {

                        result = true;

                        break;

                    }

            }

            else

            {

                result = false;

                KeyValuePair<T, Func<bool>> _value;

                KeyValuePair<T, Func<bool>> __value;

                for (int i = 0; i < values.Array.Length; i++)
                {

                    _value = values.GetValue(i);

                    result = true;

                    if (checkIfComparison(_value))

                    {

                        for (int j = i + 1; j < values.Array.Length; j++)

                        {

                            __value = values.GetValue(j);

                            if (checkIfComparison(__value))

                            {

                                result = false;

                                break;

                            }

                        }

                        break;

                    }

                }

            }

            return result;

        }

        private static bool IfInternal<TKey, TValue>(ComparisonType comparisonType, ComparisonMode comparisonMode, CheckIfComparisonDelegate<TValue> comparisonDelegate, out TKey key, IIfKeyValuesEnumerable<TKey, TValue> values)

        {

            bool checkIfComparison(KeyValuePair<TValue, Func<bool>> value) => comparisonDelegate(value.Key, value.Value);

            object _key = null;

            bool result;

            if (comparisonMode == ComparisonMode.Binary)

            {

                if (comparisonType == ComparisonType.And)

                {

                    result = true;

                    foreach (KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> _value in values)

                    {

                        result &= checkIfComparison(_value.Value);

                        if (_key == null && !result)

                            _key = _value.Key;

                    }

                }

                else

                {

                    result = false;

                    if (comparisonType == ComparisonType.Or)

                        foreach (KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> _value in values)

                        {

                            result |= checkIfComparison(_value.Value);

                            if (_key == null && result)

                                _key = _value.Key;

                        }

                    else // Xor

                    {

                        bool alreadyTrue = false;

                        KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> _value;

                        int i;

                        for (i = 0; i < values.Array.Length; i++)
                        {

                            _value = values.GetValue(i);

                            if (checkIfComparison(_value.Value))

                                if (alreadyTrue)

                                {

                                    _key = null;

                                    break;

                                }

                                else

                                {

                                    result |= true;

                                    _key = _value.Key;

                                    alreadyTrue = true;

                                }

                            else

                                result |= false;

                        }

                        if (alreadyTrue)

                            for (i += 1; i < values.Array.Length; i++)

                            {

                                _value = values.GetValue(i);

                                checkIfComparison(_value.Value);

                            }
                    }

                }

            }

            // We check the comparison type for the 'and' comparison.

            else if (comparisonType == ComparisonType.And)

            {

                result = true;

                foreach (KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> _value in values)

                    if (!checkIfComparison(_value.Value))

                    {

                        result = false;

                        _key = _value.Key;

                        break;

                    }

            }

            // We check the comparison type for the 'or' comparison.

            else if (comparisonType == ComparisonType.Or)

            {

                result = false;

                foreach (KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> _value in values)

                    if (checkIfComparison(_value.Value))

                    {

                        result = true;

                        _key = _value.Key;

                        break;

                    }

            }

            else

            {

                result = false;

                KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> _value;

                KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>> __value;

                for (int i = 0; i < values.Array.Length; i++)
                {

                    _value = values.GetValue(i);

                    result = true;

                    if (checkIfComparison(_value.Value))

                    {

                        for (int j = i + 1; j < values.Array.Length; j++)

                        {

                            __value = values.GetValue(j);

                            if (checkIfComparison(__value.Value))

                            {

                                result = false;

                                break;

                            }

                        }

                        if (result)

                            _key = _value.Key;

                        break;

                    }

                }

            }

            key = (TKey)_key;

            return result;

        }

        // todo: factoriser au maximum

        #region Non generic methods

        #region Comparisons without key notification

        /// <summary>
        /// Performs a comparison by testing a value compared to an array of values.
        /// </summary>
        /// <param name="comparisonType">Whether to perform an 'and', 'or' or 'xor' comparison.</param>
        /// <param name="comparisonMode">Whether to perform a binary or a logical comparison</param>
        /// <param name="comparison">The comparison type</param>
        /// <param name="value">The value to compare the values of the table with.</param>
        /// <param name="values">The values to compare.</param>
        /// <returns><see langword="true"/> if the comparison has succeeded for all values, otherwise <see langword="false"/>.</returns>
        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, object value, params object[] values) => If(comparisonType, comparisonMode, comparison, (IEqualityComparer)EqualityComparer<object>.Default, GetCommonPredicate<object>(), value, values);

        /// <summary>
        /// Performs a comparison by testing a value compared to an array of objects or values using a custom <see cref="IComparer{Object}"/> and <see cref="Predicate{Object}"/>.
        /// </summary>
        /// <param name="comparisonType">Whether to perform an 'and', 'or' or 'xor' comparison.</param>
        /// <param name="comparison">The comparison type</param>
        /// <param name="comparer">The comparer used to compare the values.</param>
        /// <param name="value">The value to compare with.</param>
        /// <param name="values">The values to compare.</param>
        /// <param name="predicate">The comparison predicate</param>
        /// <returns><see langword="true"/> if the comparison has succeeded for all values, otherwise <see langword="false"/>.</returns>
        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, IComparer comparer, Predicate<object> predicate, object value, params object[] values) => If(comparisonType, comparisonMode, comparison, (object x, object y) => comparer.Compare(x, y), predicate, value, values);

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, Comparison<object> comparisonDelegate, Predicate<object> predicate, object value, params object[] values)

        {

            // First, we check if comparisonType and comparison are in the required value range.

            ThrowOnInvalidIfMethodArg(comparisonType, comparisonMode, comparison);

            return IfInternal(comparisonType, comparisonMode, (object _value, Func<bool> _predicate) => CheckIfComparison(comparison, _predicate(), comparisonDelegate(value, _value)), new IfValuesEnumerable(values, predicate));

        }

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, IEqualityComparer equalityComparer, Predicate<object> predicate, object value, params object[] values)

        {

            // First, we check if comparisonType and comparison are in the required value range.

            ThrowOnInvalidEqualityIfMethodArg(comparisonType, comparisonMode, comparison, value.GetType(), equalityComparer);

            return IfInternal(comparisonType, comparisonMode, (object _value, Func<bool> _predicate) => CheckEqualityComparison(comparison, _value, value, _predicate(), equalityComparer), new IfValuesEnumerable(values, predicate));

        }

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, IComparer comparer, object value, params KeyValuePair<object, Func<bool>>[] values) => If(comparisonType, comparisonMode, comparison, (object x, object y) => comparer.Compare(x, y), value, values);

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, Comparison<object> comparisonDelegate, object value, params KeyValuePair<object, Func<bool>>[] values)

        {

            // First, we check if comparisonType and comparison are in the required value range.

            ThrowOnInvalidIfMethodArg(comparisonType, comparisonMode, comparison);

            return IfInternal(comparisonType, comparisonMode, (object _value, Func<bool> _predicate) => CheckIfComparison(comparison, _predicate(), comparisonDelegate(value, _value)), new IfKeyValuePairEnumerable(values));

        }

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, IEqualityComparer equalityComparer, object value, params KeyValuePair<object, Func<bool>>[] values)

        {

            // First, we check if comparisonType and comparison are in the required value range.

            ThrowOnInvalidEqualityIfMethodArg(comparisonType, comparisonMode, comparison, value.GetType(), equalityComparer);

            return IfInternal(comparisonType, comparisonMode, (object _value, Func<bool> _predicate) => CheckEqualityComparison(comparison, _value, value, _predicate(), equalityComparer), new IfKeyValuePairEnumerable(values));

        }

        #endregion

        #region Comparisons with key notification

        /// <summary>
        /// Performs a comparison by testing a value compared to an array of objects or values.
        /// </summary>
        /// <param name="comparisonType">Whether to perform an 'and', 'or' or 'xor' comparison.</param>
        /// <param name="comparison">The comparison type</param>
        /// <param name="value">The value to compare the values of the table with.</param>
        /// <param name="values">The values to compare.</param>
        /// <returns><see langword="true"/> if the comparison has succeeded for all values, otherwise <see langword="false"/>.</returns>
        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out object key, object value, params KeyValuePair<object, object>[] values) => If(comparisonType, comparisonMode, comparison, out key, (IEqualityComparer)EqualityComparer<object>.Default, GetCommonPredicate<object>(), value, values);

        /// <summary>
        /// Performs a comparison by testing a value compared to an array of objects or values using a custom <see cref="IComparer{Object}"/> and <see cref="Predicate{Object}"/>.
        /// </summary>
        /// <param name="comparisonType">Whether to perform an 'and', 'or' or 'xor' comparison.</param>
        /// <param name="comparison">The comparison type</param>
        /// <param name="comparer">The comparer used to compare the values.</param>
        /// <param name="value">The value to compare with.</param>
        /// <param name="values">The values to compare.</param>
        /// <param name="predicate">The comparison predicate</param>
        /// <returns><see langword="true"/> if the comparison has succeeded for all values, otherwise <see langword="false"/>.</returns>
        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out object key, IComparer comparer, Predicate<object> predicate, object value, params KeyValuePair<object, object>[] values) => If(comparisonType, comparisonMode, comparison, out key, (object x, object y) => comparer.Compare(x, y), predicate, value, values);

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out object key, Comparison<object> comparisonDelegate, Predicate<object> predicate, object value, params KeyValuePair<object, object>[] values)

        {

            // First, we check if comparisonType and comparison are in the required value range.

            ThrowOnInvalidIfMethodArg(comparisonType, comparisonMode, comparison);

            return IfInternal(comparisonType, comparisonMode, (object _value, Func<bool> _predicate) => CheckIfComparison(comparison, _predicate(), comparisonDelegate(value, _value)), out key, new IfKeyValuesEnumerable(values, predicate));

        }

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out object key, IEqualityComparer equalityComparer, Predicate<object> predicate, object value, params KeyValuePair<object, object>[] values)

        {

            ThrowOnInvalidEqualityIfMethodArg(comparisonType, comparisonMode, comparison, value.GetType(), equalityComparer);

            return IfInternal(comparisonType, comparisonMode, (object _value, Func<bool> _predicate) => CheckEqualityComparison(comparison, _value, value, _predicate(), equalityComparer), out key, new IfKeyValuesEnumerable(values, predicate));

        }

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out object key, IComparer comparer, object value, params KeyValuePair<object, KeyValuePair<object, Func<bool>>>[] values) => If(comparisonType, comparisonMode, comparison, out key, (object x, object y) => comparer.Compare(x, y), value, values);

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out object key, Comparison<object> comparisonDelegate, object value, params KeyValuePair<object, KeyValuePair<object, Func<bool>>>[] values)

        {

            // First, we check if comparisonType and comparison are in the required value range.

            ThrowOnInvalidIfMethodArg(comparisonType, comparisonMode, comparison);

            return IfInternal(comparisonType, comparisonMode, (object _value, Func<bool> _predicate) => CheckIfComparison(comparison, _predicate(), comparisonDelegate(value, _value)), out key, new IfKeyKeyValuePairEnumerable(values));

        }

        public static bool If(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out object key, IEqualityComparer equalityComparer, object value, params KeyValuePair<object, KeyValuePair<object, Func<bool>>>[] values)

        {

            ThrowOnInvalidEqualityIfMethodArg(comparisonType, comparisonMode, comparison, value.GetType(), equalityComparer);

            return IfInternal(comparisonType, comparisonMode, (object _value, Func<bool> _predicate) => CheckEqualityComparison(comparison, _value, value, _predicate(), equalityComparer), out key, new IfKeyKeyValuePairEnumerable(values));

        }

        #endregion

        #endregion

        #region Generic methods

        #region Comparisons without key notification

        /// <summary>
        /// Performs a comparison by testing a value compared to an array of objects or values.
        /// </summary>
        /// <param name="comparisonType">Whether to perform an 'and', 'or' or 'xor' comparison.</param>
        /// <param name="comparison">The comparison type</param>
        /// <param name="value">The value to compare the values of the table with.</param>
        /// <param name="values">The values to compare.</param>
        /// <returns><see langword="true"/> if the comparison has succeeded for all values, otherwise <see langword="false"/>.</returns>
        public static bool If<T>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, T value, params T[] values) => If(comparisonType, comparisonMode, comparison, EqualityComparer<T>.Default, GetCommonPredicate<T>(), value, values);

        /// <summary>
        /// Performs a comparison by testing a value compared to an array of objects or values using a custom <see cref="IComparer{Object}"/> and <see cref="Predicate{Object}"/>.
        /// </summary>
        /// <param name="comparisonType">Whether to perform an 'and', 'or' or 'xor' comparison.</param>
        /// <param name="comparison">The comparison type</param>
        /// <param name="comparer">The comparer used to compare the values.</param>
        /// <param name="value">The value to compare with.</param>
        /// <param name="values">The values to compare.</param>
        /// <param name="predicate">The comparison predicate</param>
        /// <returns><see langword="true"/> if the comparison has succeeded for all values, otherwise <see langword="false"/>.</returns>
        public static bool If<T>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, IComparer<T> comparer, Predicate<T> predicate, T value, params T[] values) => If(comparisonType, comparisonMode, comparison, (T x, T y) => comparer.Compare(x, y), predicate, value, values);

        public static bool If<T>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, Comparison<T> comparisonDelegate, Predicate<T> predicate, T value, params T[] values)

        {

            // First, we check if comparisonType and comparison are in the required value range.

            ThrowOnInvalidIfMethodArg(comparisonType, comparisonMode, comparison);

            return IfInternal(comparisonType, comparisonMode, (T _value, Func<bool> _predicate) => CheckIfComparison(comparison, _predicate(), comparisonDelegate(value, _value)), new IfValuesEnumerable<T>(values, predicate));

        }

        public static bool If<T>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, IEqualityComparer<T> equalityComparer, Predicate<T> predicate, T value, params T[] values)

        {

            ThrowOnInvalidEqualityIfMethodArg(comparisonType, comparisonMode, comparison, equalityComparer);

            return IfInternal(comparisonType, comparisonMode, (T _value, Func<bool> _predicate) => CheckEqualityComparison(comparison, _value, value, _predicate(), equalityComparer), new IfValuesEnumerable<T>(values, predicate));

        }

        public static bool If<T>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, IComparer<T> comparer, T value, params KeyValuePair<T, Func<bool>>[] values) => If(comparisonType, comparisonMode, comparison, (T x, T y) => comparer.Compare(x, y), value, values);

        public static bool If<T>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, Comparison<T> comparisonDelegate, T value, params KeyValuePair<T, Func<bool>>[] values)

        {

            // First, we check if comparisonType and comparison are in the required value range.

            ThrowOnInvalidIfMethodArg(comparisonType, comparisonMode, comparison);

            return IfInternal(comparisonType, comparisonMode, (T _value, Func<bool> _predicate) => CheckIfComparison(comparison, _predicate(), comparisonDelegate(value, _value)), new IfKeyValuePairEnumerable<T>(values));

        }

        public static bool If<T>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, IEqualityComparer<T> equalityComparer, T value, params KeyValuePair<T, Func<bool>>[] values)

        {

            ThrowOnInvalidEqualityIfMethodArg(comparisonType, comparisonMode, comparison, equalityComparer);

            return IfInternal(comparisonType, comparisonMode, (T _value, Func<bool> _predicate) => CheckEqualityComparison(comparison, _value, value, _predicate(), equalityComparer), new IfKeyValuePairEnumerable<T>(values));

        }

        #endregion

        #region Comparisons with key notification

        /// <summary>
        /// Performs a comparison by testing a value compared to an array of objects or values.
        /// </summary>
        /// <param name="comparisonType">Whether to perform an 'and', 'or' or 'xor' comparison.</param>
        /// <param name="comparison">The comparison type</param>
        /// <param name="value">The value to compare the values of the table with.</param>
        /// <param name="values">The values to compare.</param>
        /// <returns><see langword="true"/> if the comparison has succeeded for all values, otherwise <see langword="false"/>.</returns>
        public static bool If<TKey, TValue>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out TKey key, TValue value, params KeyValuePair<TKey, TValue>[] values) => If(comparisonType, comparisonMode, comparison, out key, EqualityComparer<TValue>.Default, GetCommonPredicate<TValue>(), value, values);

        /// <summary>
        /// Performs a comparison by testing a value compared to an array of objects or values using a custom <see cref="IComparer{Object}"/> and <see cref="Predicate{Object}"/>.
        /// </summary>
        /// <param name="comparisonType">Whether to perform an 'and', 'or' or 'xor' comparison.</param>
        /// <param name="comparison">The comparison type</param>
        /// <param name="comparer">The comparer used to compare the values.</param>
        /// <param name="value">The value to compare with.</param>
        /// <param name="values">The values to compare.</param>
        /// <param name="predicate">The comparison predicate</param>
        /// <returns><see langword="true"/> if the comparison has succeeded for all values, otherwise <see langword="false"/>.</returns>
        public static bool If<TKey, TValue>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out TKey key, IComparer<TValue> comparer, Predicate<TValue> predicate, TValue value, params KeyValuePair<TKey, TValue>[] values) => If(comparisonType, comparisonMode, comparison, out key, (TValue x, TValue y) => comparer.Compare(x, y), predicate, value, values);

        public static bool If<TKey, TValue>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out TKey key, Comparison<TValue> comparisonDelegate, Predicate<TValue> predicate, TValue value, params KeyValuePair<TKey, TValue>[] values)

        {

            // First, we check if comparisonType and comparison are in the required value range.

            ThrowOnInvalidIfMethodArg(comparisonType, comparisonMode, comparison);

            return IfInternal(comparisonType, comparisonMode, (TValue _value, Func<bool> _predicate) => CheckIfComparison(comparison, _predicate(), comparisonDelegate(value, _value)), out key, new IfKeyValuesEnumerable<TKey, TValue>(values, predicate));

        }

        public static bool If<TKey, TValue>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out TKey key, IEqualityComparer<TValue> equalityComparer, Predicate<TValue> predicate, TValue value, params KeyValuePair<TKey, TValue>[] values)

        {

            ThrowOnInvalidEqualityIfMethodArg(comparisonType, comparisonMode, comparison, equalityComparer);

            return IfInternal(comparisonType, comparisonMode, (TValue _value, Func<bool> _predicate) => CheckEqualityComparison(comparison, _value, value, _predicate(), equalityComparer), out key, new IfKeyValuesEnumerable<TKey, TValue>(values, predicate));

        }

        public static bool If<TKey, TValue>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out TKey key, IComparer<TValue> comparer, TValue value, params KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>[] values) => If(comparisonType, comparisonMode, comparison, out key, (TValue x, TValue y) => comparer.Compare(x, y), value, values);

        public static bool If<TKey, TValue>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out TKey key, Comparison<TValue> comparisonDelegate, TValue value, params KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>[] values)

        {

            ThrowOnInvalidIfMethodArg(comparisonType, comparisonMode, comparison);

            return IfInternal(comparisonType, comparisonMode, (TValue _value, Func<bool> _predicate) => CheckIfComparison(comparison, _predicate(), comparisonDelegate(value, _value)), out key, new IfKeyKeyValuePairEnumerable<TKey, TValue>(values));

        }

        public static bool If<TKey, TValue>(ComparisonType comparisonType, ComparisonMode comparisonMode, Comparison comparison, out TKey key, IEqualityComparer<TValue> equalityComparer, TValue value, params KeyValuePair<TKey, KeyValuePair<TValue, Func<bool>>>[] values)

        {

            ThrowOnInvalidEqualityIfMethodArg(comparisonType, comparisonMode, comparison, equalityComparer);

            return IfInternal(comparisonType, comparisonMode, (TValue _value, Func<bool> _predicate) => CheckEqualityComparison(comparison, _value, value, _predicate(), equalityComparer), out key, new IfKeyKeyValuePairEnumerable<TKey, TValue>(values));

        }

        #endregion

        #endregion

        #endregion

        public static bool IsNullEmptyOrWhiteSpace(string value) => string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Concatenates multiple arrays from a same item type. Arrays must have only one dimension.
        /// </summary>
        /// <param name="arrays">The different tables to concatenate.</param>
        /// <returns></returns>
        public static T[] Concatenate<T>(params T[][] arrays)

        {
            // /// <param name="elementType">The type of the items inside the tables.</param>

            T[] newArray;

            int totalArraysLength = 0;

            int totalArraysIndex = 0;

            foreach (T[] array in arrays)

            {

                // todo : in a newer version, instead, get the maximum rank of arrays in params Array[] arrays and add a gesture of this in the process (also for the ConcatenateLong method) ; and not forgetting to change the comments of the xmldoc about this.

                if (array.Rank != 1) throw new ArgumentException(ArrayWithMoreThanOneDimension);

                totalArraysLength += array.Length;

            }

            newArray = new T[totalArraysLength];

            for (int i = 0; i < arrays.Length - 1; i++)

            {

                T[] array = arrays[i];

                array.CopyTo(newArray, totalArraysIndex);

                totalArraysIndex += array.Length;

            }

            arrays[arrays.Length - 1].CopyTo(newArray, totalArraysIndex);

            return newArray;

        }

        /// <summary>
        /// Concatenates multiple arrays from a same item type using the <see cref="Array.LongLength"/> length property. Arrays must have only one dimension.
        /// </summary>
        /// <param name="arrays">The different tables to concatenate.</param>
        /// <returns></returns>
        public static T[] ConcatenateLong<T>(params T[][] arrays)

        {

            // /// <param name="elementType">The type of the items inside the tables.</param>

            T[] newArray;

            long totalArraysLength = 0;

            long totalArraysIndex = 0;

            foreach (T[] array in arrays)

            {

                // todo:

                if (array.Rank != 1) throw new ArgumentException("Arrays must have only one dimension.");

                totalArraysLength += array.LongLength;

            }

            newArray = new T[totalArraysLength];

            for (long i = 0; i < arrays.LongLength - 1; i++)

            {

                T[] array = arrays[i];

                array.CopyTo(newArray, totalArraysIndex);

                totalArraysIndex += array.LongLength;

            }

            arrays[arrays.LongLength - 1].CopyTo(newArray, totalArraysIndex);

            return newArray;

        }

        /// <summary>
        /// Checks if a object is numeric.
        /// </summary>
        /// <remarks>This function makes a check at the object type. For a string-parsing-checking for numerical value, look at the <see cref="IsNumeric(string, out decimal)"/> function.</remarks>
        /// <param name="value">The object to check</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the object given is a numerical type.</returns>
        public static bool IsNumber(object value) => value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;

        /// <summary>
        /// Checks if a <see cref="string"/> is a numerical value.
        /// </summary>
        /// <remarks>This function tries to parse a <see cref="string"/> value to a <see cref="decimal"/> value. Given that <see cref="decimal"/> type is the greatest numerical type in the .NET framework, all the numbers can be supported in the .NET framework can be set in a <see cref="decimal"/> object.</remarks>
        /// <param name="s">The <see cref="string"/> to check</param>
        /// <param name="d">The <see cref="decimal"/> in which one set the <see cref="decimal"/> value</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the <see cref="string"/> given is a <see cref="decimal"/>.</returns>
        public static bool IsNumeric(string s, out decimal d) => decimal.TryParse(s, out d);

        public static T GetEnumAllFlags<T>() where T : Enum

        {

            Type enumType = typeof(T);

            if (enumType.GetCustomAttributes<FlagsAttribute>().ToList().Count == 0)

                throw new ArgumentException("Enum is not a 'flags' enum.");

            Array array = Enum.GetValues(enumType);

            long values = 0;

            foreach (object value in array)

                values |= (long)Convert.ChangeType(value, TypeCode.Int64);

            return (T)Enum.ToObject(enumType, values);

        }

    }
}
