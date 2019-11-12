﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WinCopies.Collections;
using static WinCopies.Util.Generic;
using IList = System.Collections.IList;
using static WinCopies.Util.Util;

namespace WinCopies.Util
{

    public delegate (bool result, Exception ex) FieldValidateValueCallback(object obj, object value, FieldInfo field, string paramName);

    public delegate void FieldValueChangedCallback(object obj, object value, FieldInfo field, string paramName);

    public delegate (bool result, Exception ex) PropertyValidateValueCallback(object obj, object value, PropertyInfo property, string paramName);

    public delegate void PropertyValueChangedCallback(object obj, object value, PropertyInfo property, string paramName);

    /// <summary>
    /// Provides some static extension methods.
    /// </summary>
    public static class Extensions
    {

        #region Enumerables extension methods

        // todo:

        //#region Contains methods

        //// public static bool Contains(this IEnumerable array, object value) => array.Contains(value, EqualityComparer<object>.Default);

        //public static bool Contains(this IEnumerable array, object value, IEqualityComparer comparer)

        //{

        //    foreach (object _value in array)

        //        if (comparer.Equals(_value, value)) return true;

        //    return false;

        //}

        //public static bool Contains(this IEnumerable array, object value, System.Collections.Generic.IComparer comparer) => array.Contains(value, (object x, object y) => comparer.Compare(x, y));

        //public static bool Contains(this IEnumerable array, object value, Comparison comparison)

        //{

        //    foreach (object _value in array)

        //        if (comparison(_value, value) == 0) return true;

        //    return false;

        //}

        //// todo: to add methods for the LongLength's Array property gesture.

        //public static bool Contains(this object[] array, object value, out int index) => array.Contains(value, EqualityComparer<object>.Default, out index);

        //public static bool Contains(this object[] array, object value, IEqualityComparer comparer, out int index)

        //{

        //    for (int i = 0; i < array.Length; i++)

        //        if (comparer.Equals(array[i], value))

        //        {

        //            index = i;

        //            return true;

        //        }

        //    index = -1;

        //    return false;

        //}

        //public static bool Contains(this object[] array, object value, System.Collections.Generic.IComparer comparer, out int index) => array.Contains(value, (object x, object y) => comparer.Compare(x, y), out index);

        //public static bool Contains(this object[] array, object value, Comparison comparison, out int index)

        //{

        //    for (int i = 0; i < array.Length; i++)

        //        if (comparison(array[i], value) == 0)

        //        {

        //            index = i;

        //            return true;

        //        }

        //    index = -1;

        //    return false;

        //}

        //public static bool ContainsRange(this IEnumerable array, params object[] values);

        //public static bool ContainsRange(this IEnumerable array, IEnumerable values);

        //public static bool ContainsRange(this IEnumerable array, IEqualityComparer comparer, params object[] values);

        //public static bool ContainsRange(this IEnumerable array, IEqualityComparer comparer, IEnumerable values);

        //public static bool ContainsRange(this IEnumerable array, System.Collections.Generic.IComparer comparer, params object[] values);

        //public static bool ContainsRange(this IEnumerable array, System.Collections.Generic.IComparer comparer, IEnumerable values);

        //public static bool ContainsRange(this IEnumerable array, Comparison comparison, params object[] values);

        //public static bool ContainsRange(this IEnumerable array, Comparison comparison, IEnumerable values);

        //public static bool ContainsRange(this IEnumerable array, out int index, params object[] values);

        //public static bool ContainsRange(this IEnumerable array, out int index, IEnumerable values);

        //public static bool ContainsRange(this IEnumerable array, IEqualityComparer comparer, out int index, params object[] values);

        //public static bool ContainsRange(this IEnumerable array, IEqualityComparer comparer, out int index, IEnumerable values);

        //public static bool ContainsRange(this IEnumerable array, System.Collections.Generic.IComparer comparer, out int index, params object[] values);

        //public static bool ContainsRange(this IEnumerable array, System.Collections.Generic.IComparer comparer, out int index, IEnumerable values);

        //public static bool ContainsRange(this IEnumerable array, Comparison comparison, out int index, params object[] values);

        //public static bool ContainsRange(this IEnumerable array, Comparison comparison, out int index, IEnumerable values);

        //public static bool Contains<T>(this IEnumerable<T> array, T value);

        //public static bool Contains<T>(this IEnumerable<T> array, T value, IEqualityComparer comparer);

        //public static bool Contains<T>(this IEnumerable<T> array, T value, System.Collections.Generic.IComparer comparer);

        //public static bool Contains<T>(this IEnumerable<T> array, T value, Comparison comparison);

        //public static bool Contains<T>(this IEnumerable<T> array, T value, out int index);

        //public static bool Contains<T>(this IEnumerable<T> array, T value, IEqualityComparer comparer, out int index);

        //public static bool Contains<T>(this IEnumerable<T> array, T value, System.Collections.Generic.IComparer comparer, out int index);

        //public static bool Contains<T>(this IEnumerable<T> array, T value, Comparison comparison, out int index);

        //public static bool ContainsRange<T>(this IEnumerable<T> array, params T[] values);

        //public static bool ContainsRange<T>(this IEnumerable<T> array, IEqualityComparer comparer, params T[] values);

        //public static bool ContainsRange<T>(this IEnumerable<T> array, System.Collections.Generic.IComparer comparer, params T[] values);

        //public static bool ContainsRange<T>(this IEnumerable<T> array, Comparison comparison, params T[] values);

        //public static bool ContainsRange<T>(this IEnumerable<T> array, out int index, params T[] values);

        //public static bool ContainsRange<T>(this IEnumerable<T> array, IEqualityComparer comparer, out int index, params T[] values);

        //public static bool ContainsRange<T>(this IEnumerable<T> array, System.Collections.Generic.IComparer comparer, out int index, params T[] values);

        //public static bool ContainsRange<T>(this IEnumerable<T> array, Comparison comparison, out int index, params T[] values);

        //#endregion

        // todo: Add-, Insert-, Remove-If(Not)Contains methods: add parameters like the Contains methods

        #region Add(Range)IfNotContains methods

        /// <summary>
        /// Tries to add a value to an <see cref="IList"/> if it does not contain it already.
        /// </summary>
        /// <param name="collection">The collection to which try to add the value</param>
        /// <param name="value">The value to try to add to the collection</param>
        /// <returns><see langword="true"/> if the value has been added to the collection, otherwise <see langword="false"/>.</returns>
        public static bool AddIfNotContains(this IList collection, in object value)

        {

            ThrowIfNull(collection, nameof(collection));

            if (collection.Contains(value)) return false;

            _ = collection.Add(value);

            return true;

        }

        /// <summary>
        /// Tries to add multiple values to an <see cref="System.Collections.ICollection"/> if it does not contain them already.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="collection">The collection to which try to add the value</param>
        /// <param name="values">The values to try to add to the collection</param>
        /// <returns><see langword="true"/> if the value has been added to the collection, otherwise <see langword="false"/>.</returns>
        public static object[] AddRangeIfNotContains(this System.Collections.ICollection collection, params object[] values) => collection.AddRangeIfNotContains((IEnumerable)values);

        /// <summary>
        /// Tries to add multiple values to an <see cref="System.Collections.IList"/> if it does not contain them already.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="collection">The collection to which try to add the value</param>
        /// <param name="values">The values to try to add to the collection</param>
        /// <returns><see langword="true"/> if the value has been added to the collection, otherwise <see langword="false"/>.</returns>
        public static object[] AddRangeIfNotContains(this System.Collections.IList collection, in IEnumerable values)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(values, nameof(values));

            var addedValues = new ArrayAndListBuilder<object>();

            foreach (object value in values)

            {

                if (collection.Contains(value)) continue;

                _ = collection.Add(value);

                _ = addedValues.AddLast(value);

            }

            return addedValues.ToArray();

        }



        /// <summary>
        /// Tries to add a value to an <see cref="ICollection{T}"/> if it does not contain it already.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="collection">The collection to which try to add the value</param>
        /// <param name="value">The value to try to add to the collection</param>
        /// <returns><see langword="true"/> if the value has been added to the collection, otherwise <see langword="false"/>.</returns>
        public static bool AddIfNotContains<T>(this ICollection<T> collection, in T value)

        {

            ThrowIfNull(collection, nameof(collection));

            if (collection.Contains(value)) return false;

            collection.Add(value);

            return true;

        }

        /// <summary>
        /// Tries to add multiple values to an <see cref="ICollection{T}"/> if it does not contain them already.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="collection">The collection to which try to add the value</param>
        /// <param name="values">The values to try to add to the collection</param>
        /// <returns><see langword="true"/> if the value has been added to the collection, otherwise <see langword="false"/>.</returns>
        public static T[] AddRangeIfNotContains<T>(this ICollection<T> collection, params T[] values) => collection.AddRangeIfNotContains((IEnumerable<T>)values);

        /// <summary>
        /// Tries to add multiple values to an <see cref="ICollection{T}"/> if it does not contain them already.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="collection">The collection to which try to add the value</param>
        /// <param name="values">The values to try to add to the collection</param>
        /// <returns><see langword="true"/> if the value has been added to the collection, otherwise <see langword="false"/>.</returns>
        public static T[] AddRangeIfNotContains<T>(this ICollection<T> collection, in IEnumerable<T> values)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(values, nameof(values));

            var addedValues = new ArrayAndListBuilder<T>();

            foreach (T value in values)

            {

                if (collection.Contains(value)) continue;

                collection.Add(value);

                _ = addedValues.AddLast(value);

            }

            return addedValues.ToArray();

        }

        #endregion

        #region Insert(Range)IfNotContains methods

        public static bool InsertIfNotContains(this IList collection, in int index, in object value)

        {

            ThrowIfNull(collection, nameof(collection));

            if (collection.Contains(value)) return false;

            collection.Insert(index, value);

            return true;

        }

        public static object[] InsertRangeIfNotContains(this IList collection, in int index, params object[] values) => collection.InsertRangeIfNotContains(index, (IEnumerable)values);

        public static object[] InsertRangeIfNotContains(this IList collection, in int index, in IEnumerable values)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(values, nameof(values));

            var addedValues = new ArrayAndListBuilder<object>();

            foreach (object value in values)

            {

                if (collection.Contains(value)) continue;

                collection.Insert(index, value);

                _ = addedValues.AddLast(value);

            }

            return addedValues.ToArray();

        }



        public static bool InsertIfNotContains<T>(this System.Collections.Generic.IList<T> collection, in int index, in T value)

        {

            ThrowIfNull(collection, nameof(collection));

            if (collection.Contains(value)) return false;

            collection.Insert(index, value);

            return true;

        }

        public static T[] InsertRangeIfNotContains<T>(this System.Collections.Generic.IList<T> collection, in int index, params T[] values) => collection.InsertRangeIfNotContains(index, (IEnumerable<T>)values);

        public static T[] InsertRangeIfNotContains<T>(this System.Collections.Generic.IList<T> collection, in int index, in IEnumerable<T> values)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(values, nameof(values));

            var addedValues = new ArrayAndListBuilder<T>();

            foreach (T value in values)

            {

                if (collection.Contains(value)) continue;

                collection.Insert(index, value);

                _ = addedValues.AddLast(value);

            }

            return addedValues.ToArray();

        }

        #endregion

        #region Remove(Range)IfContains methods

        public static bool RemoveIfContains(this IList collection, in object value)

        {

            ThrowIfNull(collection, nameof(collection));

            if (collection.Contains(value))

            {

                collection.Remove(value);

                return true;

            }

            return false;

        }

        public static object[] RemoveRangeIfContains(this IList collection, params object[] values) => collection.RemoveRangeIfContains((IEnumerable)values);

        public static object[] RemoveRangeIfContains(this IList collection, in IEnumerable values)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(values, nameof(values));

            var removedValues = new ArrayAndListBuilder<object>();

            foreach (object value in values)

                if (collection.Contains(value))

                {

                    // todo: RemoveAt()

                    collection.Remove(value);

                    _ = removedValues.AddLast(value);

                }

            return removedValues.ToArray();

        }



        public static bool RemoveIfContains<T>(this ICollection<T> collection, in T value)

        {

            ThrowIfNull(collection, nameof(collection));

            if (collection.Contains(value))

            {

                _ = collection.Remove(value);

                return true;

            }

            return false;

        }

        public static T[] RemoveRangeIfContains<T>(this ICollection<T> collection, params T[] values) => collection.RemoveRangeIfContains((IEnumerable<T>)values);

        public static T[] RemoveRangeIfContains<T>(this ICollection<T> collection, in IEnumerable<T> values)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(values, nameof(values));

            var removedValues = new ArrayAndListBuilder<T>();

            foreach (T value in values)

                if (collection.Contains(value))

                {

                    // todo: RemoveAt()

                    _ = collection.Remove(value);

                    _ = removedValues.AddLast(value);

                }

            return removedValues.ToArray();

        }

        #endregion

        #region AddRange methods

        public static void AddRange(this IList collection, params object[] values) => collection.AddRange((IEnumerable)values);

        public static void AddRange(this IList collection, in IEnumerable array)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(array, nameof(array));

            foreach (object item in array)

                _ = collection.Add(item);

        }

        public static void AddRange(this IList collection, in int start, in int length, params object[] values)

        {

            ThrowIfNull(collection, nameof(collection));

            for (int i = start; i < length; i++)

                _ = collection.Add(values[i]);

        }

        // todo: to add a version of the methods like this one with a 'contains' check:

        public static void AddRange(this IList collection, in IList values, in int start, in int length)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(values, nameof(values));

            for (int i = start; i < length; i++)

                _ = collection.Add(values[i]);

        }

        public static void AddRange(this IList collection, in IEnumerable array, in int start, in int length) => collection.AddRange(array.ToArray(), start, length);



        public static void AddRange<T>(this ICollection<T> collection, params T[] values) => collection.AddRange((IEnumerable<T>)values);

        public static void AddRange<T>(this ICollection<T> collection, in IEnumerable<T> array)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(array, nameof(array));

            foreach (T item in array)

                collection.Add(item);

        }

        public static void AddRange<T>(this ICollection<T> collection, in int start, in int length, params T[] values)

        {

            ThrowIfNull(collection, nameof(collection));

            for (int i = start; i < length; i++)

                collection.Add(values[i]);

        }

        public static void AddRange<T>(this ICollection<T> collection, in System.Collections.Generic.IList<T> values, in int start, in int length)

        {

            ThrowIfNull(collection, nameof(collection));

            for (int i = start; i < length; i++)

                collection.Add(values[i]);

        }

        public static void AddRange<T>(this ICollection<T> collection, in IEnumerable<T> array, in int start, in int length) => collection.AddRange(array.ToArray<T>(), start, length);



        /// <summary>
        /// Add multiple values at the top of a <see cref="LinkedList{T}"/>. For better performance, use the <see cref="ArrayAndListBuilder{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="values">The values to add to this <see cref="LinkedList{T}"/></param>
        /// <returns>The added <see cref="LinkedListNode{T}"/>'s.</returns>
        public static LinkedListNode<T>[] AddRangeFirst<T>(this LinkedList<T> collection, params T[] values)
        {
            ThrowIfNull(collection, nameof(collection));

            return collection.First == null ? collection.AddRangeLast(values) : collection.AddRangeBefore(collection.First, values);
        }

        /// <summary>
        /// Add multiple values at the top of a <see cref="LinkedList{T}"/>. For better performance, use the <see cref="ArrayAndListBuilder{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="array">The values to add to this <see cref="LinkedList{T}"/></param>
        /// <returns>The added <see cref="LinkedListNode{T}"/>'s.</returns>
        public static LinkedListNode<T>[] AddRangeFirst<T>(this LinkedList<T> collection, in IEnumerable<T> array)
        {
            ThrowIfNull(collection, nameof(collection));

            return collection.First == null ? collection.AddRangeLast(array) : collection.AddRangeBefore(collection.First, array);
        }

        /// <summary>
        /// Add multiple <see cref="LinkedListNode{T}"/>'s at the top of a <see cref="LinkedList{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="nodes">The <see cref="LinkedListNode{T}"/>'s to add to a <see cref="LinkedList{T}"/></param>
        public static void AddRangeFirst<T>(this LinkedList<T> collection, params LinkedListNode<T>[] nodes)
        {
            ThrowIfNull(collection, nameof(collection));

            if (collection.First == null)

                collection.AddRangeLast(nodes);

            else

                collection.AddRangeBefore(collection.First, nodes);
        }

        /// <summary>
        /// Add multiple <see cref="LinkedListNode{T}"/>'s at the top of a <see cref="LinkedList{T}"/>. For better performance, use the <see cref="ArrayAndListBuilder{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="array">The <see cref="LinkedListNode{T}"/>'s to add to a <see cref="LinkedList{T}"/></param>
        public static void AddRangeFirst<T>(this LinkedList<T> collection, in IEnumerable<LinkedListNode<T>> array)
        {
            ThrowIfNull(collection, nameof(collection));

            if (collection.First == null)

                collection.AddRangeLast(array);

            else

                collection.AddRangeBefore(collection.First, array);
        }

        /// <summary>
        /// Add multiple values at the end of a <see cref="LinkedList{T}"/>. For better performance, use the <see cref="ArrayAndListBuilder{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="values">The values to add to a <see cref="LinkedList{T}"/></param>
        /// <returns>The added <see cref="LinkedListNode{T}"/>'s.</returns>
        public static LinkedListNode<T>[] AddRangeLast<T>(this LinkedList<T> collection, params T[] values) => collection.AddRangeLast((IEnumerable<T>)values);

        /// <summary>
        /// Add multiple values at the end of a <see cref="LinkedList{T}"/>. For better performance, use the <see cref="ArrayAndListBuilder{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="array">The values to add to a <see cref="LinkedList{T}"/></param>
        /// <returns>The added <see cref="LinkedListNode{T}"/>'s.</returns>
        public static LinkedListNode<T>[] AddRangeLast<T>(this LinkedList<T> collection, in IEnumerable<T> array)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(array, nameof(array));

            var result = new LinkedList<LinkedListNode<T>>();

            foreach (T item in array)

                _ = result.AddLast(collection.AddLast(item));

            return result.ToArray<LinkedListNode<T>>();

        }

        /// <summary>
        /// Add multiple <see cref="LinkedListNode{T}"/>'s at the end of a <see cref="LinkedList{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="nodes">The <see cref="LinkedListNode{T}"/>'s to add to a <see cref="LinkedList{T}"/></param>
        /// <returns>The added <see cref="LinkedListNode{T}"/>'s.</returns>
        public static void AddRangeLast<T>(this LinkedList<T> collection, params LinkedListNode<T>[] nodes) => collection.AddRangeLast((IEnumerable<LinkedListNode<T>>)nodes);

        /// <summary>
        /// Add multiple <see cref="LinkedListNode{T}"/>'s at the end of a <see cref="LinkedList{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="array">The <see cref="LinkedListNode{T}"/>'s to add to a <see cref="LinkedList{T}"/></param>
        public static void AddRangeLast<T>(this LinkedList<T> collection, in IEnumerable<LinkedListNode<T>> array)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(array, nameof(array));

            foreach (LinkedListNode<T> item in array)

                collection.AddLast(item);

        }

        /// <summary>
        /// Add multiple values before a specified node in a <see cref="LinkedList{T}"/>. For better performance, use the <see cref="ArrayAndListBuilder{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="node">The node before which to add the values</param>
        /// <param name="values">The values to add to a <see cref="LinkedList{T}"/></param>
        /// <returns>The added <see cref="LinkedListNode{T}"/>'s.</returns>
        public static LinkedListNode<T>[] AddRangeBefore<T>(this LinkedList<T> collection, in LinkedListNode<T> node, params T[] values) => collection.AddRangeBefore(node, (IEnumerable<T>)values);

        /// <summary>
        /// Add multiple values before a specified node in a <see cref="LinkedList{T}"/>. For better performance, use the <see cref="ArrayAndListBuilder{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="node">The node before which to add the values</param>
        /// <param name="array">The values to add to a <see cref="LinkedList{T}"/></param>
        /// <returns>The added <see cref="LinkedListNode{T}"/>'s.</returns>
        public static LinkedListNode<T>[] AddRangeBefore<T>(this LinkedList<T> collection, in LinkedListNode<T> node, in IEnumerable<T> array)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(array, nameof(array));

            var result = new LinkedList<LinkedListNode<T>>();

            foreach (T item in array)

                _ = result.AddLast(collection.AddBefore(node, item));

            return result.ToArray<LinkedListNode<T>>();

        }

        /// <summary>
        /// Add multiple values before a specified node in a <see cref="LinkedList{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="node">The node before which to add the values</param>
        /// <param name="nodes">The <see cref="LinkedListNode{T}"/>'s to add to a <see cref="LinkedList{T}"/></param>
        public static void AddRangeBefore<T>(this LinkedList<T> collection, in LinkedListNode<T> node, params LinkedListNode<T>[] nodes) => collection.AddRangeBefore(node, (IEnumerable<LinkedListNode<T>>)nodes);

        /// <summary>
        /// Add multiple values before a specified node in a <see cref="LinkedList{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="node">The node before which to add the values</param>
        /// <param name="array">The values to add to a <see cref="LinkedList{T}"/></param>
        public static void AddRangeBefore<T>(this LinkedList<T> collection, in LinkedListNode<T> node, in IEnumerable<LinkedListNode<T>> array)

        {

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(array, nameof(array));

            foreach (LinkedListNode<T> item in array)

                collection.AddBefore(node, item);

        }

        /// <summary>
        /// Add multiple values after a specified node in a <see cref="LinkedList{T}"/>. For better performance, use the <see cref="ArrayAndListBuilder{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="node">The node after which to add the values</param>
        /// <param name="values">The values to add to a <see cref="LinkedList{T}"/></param>
        /// <returns>The added <see cref="LinkedListNode{T}"/>'s.</returns>
        public static LinkedListNode<T>[] AddRangeAfter<T>(this LinkedList<T> collection, in LinkedListNode<T> node, params T[] values)
        {
            ThrowIfNull(node, nameof(node));

            return node.Next == null ? collection.AddRangeLast(values) : collection.AddRangeBefore(node.Next, values);
        }

        /// <summary>
        /// Add multiple values after a specified node in a <see cref="LinkedList{T}"/>. For better performance, use the <see cref="ArrayAndListBuilder{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="node">The node after which to add the values</param>
        /// <param name="array">The values to add to a <see cref="LinkedList{T}"/></param>
        /// <returns>The added <see cref="LinkedListNode{T}"/>'s.</returns>
        public static LinkedListNode<T>[] AddRangeAfter<T>(this LinkedList<T> collection, in LinkedListNode<T> node, in IEnumerable<T> array)
        {
            ThrowIfNull(node, nameof(node));

            return node.Next == null ? collection.AddRangeLast(array) : collection.AddRangeBefore(node.Next, array);
        }

        /// <summary>
        /// Add multiple values after a specified node in a <see cref="LinkedList{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="node">The node after which to add the values</param>
        /// <param name="nodes">The values to add to a <see cref="LinkedList{T}"/></param>
        public static void AddRangeAfter<T>(this LinkedList<T> collection, in LinkedListNode<T> node, params LinkedListNode<T>[] nodes)

        {
            ThrowIfNull(node, nameof(node));

            if (node.Next == null)

                collection.AddRangeLast(nodes);

            else

                collection.AddRangeBefore(node.Next, nodes);
        }

        /// <summary>
        /// Add multiple values after a specified node in a <see cref="LinkedList{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="LinkedList{T}"/> into which add the values.</param>
        /// <param name="node">The node after which to add the values</param>
        /// <param name="array">The values to add to a <see cref="LinkedList{T}"/></param>
        public static void AddRangeAfter<T>(this LinkedList<T> collection, in LinkedListNode<T> node, in IEnumerable<LinkedListNode<T>> array)

        {
            ThrowIfNull(node, nameof(node));

            if (node.Next == null)

                collection.AddRangeLast(array);

            else

                collection.AddRangeBefore(node.Next, array);
        }

        #endregion

        public static ArrayList ToList(this IEnumerable array) => array.ToList(0);

        /// <summary>
        /// Converts an <see cref="IEnumerable"/> to an <see cref="ArrayList"/> from a given index for a given length.
        /// </summary>
        /// <param name="array">The <see cref="IEnumerable"/> to convert</param>
        /// <param name="startIndex">The index from which start the conversion.</param>
        /// <param name="length">The length of items to copy in the out <see cref="ArrayList"/>. Leave this parameter to null if you want to copy all the source <see cref="IEnumerable"/>.</param>
        /// <returns>The result <see cref="ArrayList"/>.</returns>
        public static ArrayList ToList(this IEnumerable array, in int startIndex, in int? length = null)

        {

            ThrowIfNull(array, nameof(array));

            int i = 0;

            if (length == null)

            {

                var arrayBuilder = new ArrayAndListBuilder<object>();

                foreach (object value in array)

                    if (i < startIndex) i++;

                    else // We don't need to increment i anymore when we are here

                        _ = arrayBuilder.AddLast(value);

                return arrayBuilder.ToArrayList();

            }

            else

            {

                var arrayList = new ArrayList(length.Value);

                int count = 0;

                foreach (object value in array)

                {

                    if (i < startIndex)

                        i++;

                    else

                    {

                        _ = arrayList.Add(value);

                        count++;

                    }

                    if (count == length)

                        break;

                }

                return arrayList;

            }

        }

        //public static List<T> ToList<T>(this IEnumerable<T> array)

        //{

        //    List<T> arrayList = new List<T>();

        //    foreach (T value in array)

        //        arrayList.Add(value);

        //    return arrayList;

        //}

        /// <summary>
        /// Converts an <see cref="IEnumerable"/> to a <see cref="List{T}"/> from a given index for a given length.
        /// </summary>
        /// <param name="array">The <see cref="IEnumerable"/> to convert</param>
        /// <param name="startIndex">The index from which start the conversion.</param>
        /// <param name="length">The length of items to copy in the out <see cref="List{T}"/>. Leave this parameter to null if you want to copy all the source <see cref="IEnumerable"/>.</param>
        /// <returns>The result <see cref="List{T}"/>.</returns>
        public static List<T> ToList<T>(this IEnumerable<T> array, in int startIndex, in int? length = null)

        {

            ThrowIfNull(array, nameof(array));

            int i = 0;

            if (length == null)

            {

                var arrayBuilder = new ArrayAndListBuilder<T>();

                foreach (T value in array)

                    if (i < startIndex) i++;

                    else    // We don't need to increment i anymore when we are here

                        _ = arrayBuilder.AddLast(value);

                return arrayBuilder.ToList();

            }

            else

            {

                var arrayList = new List<T>(length.Value);

                int count = 0;

                foreach (T value in array)

                {

                    if (i < startIndex)

                        i++;

                    else

                    {

                        arrayList.Add(value);

                        count++;

                    }

                    if (count == length)

                        break;

                }

                return arrayList;

            }

        }

        public static object[] ToArray(this IEnumerable array)

        {

            ThrowIfNull(array, nameof(array));

            var _array = new LinkedList<object>();

            foreach (object value in array)

                _ = _array.AddLast(value);

            return _array.ToArray<object>();

        }

        //public static T[] ToArray<T>(this IEnumerable<T> array)

        //{

        //    T[] _array = new T[length];

        //    int i = 0;

        //    int count = 0;

        //    foreach (T value in array)

        //    {

        //        if (i < startIndex)

        //            i++;

        //        else

        //            _array[count++] = value;

        //        if (count == length)

        //            break;

        //    }

        //    return _array;

        //}

        public static object[] ToArray(this IEnumerable array, in int startIndex, in int length)

        {

            ThrowIfNull(array, nameof(array));

            object[] _array = new object[length];

            int i = 0;

            int count = 0;

            foreach (object value in array)

            {

                if (i < startIndex)

                    i++;

                else

                    _array[count++] = value;

                if (count == length)

                    break;

            }

            return _array;

        }

        public static T[] ToArray<T>(this IEnumerable<T> array, in int startIndex, in int length)

        {

            ThrowIfNull(array, nameof(array));

            var _array = new T[length];

            int i = 0;

            int count = 0;

            foreach (T value in array)

            {

                if (i < startIndex)

                    i++;

                else

                    _array[count++] = value;

                if (count == length)

                    break;

            }

            return _array;

        }

        public static ArrayList ToList(this object[] array, in int startIndex, in int length)

        {

            ThrowIfNull(array, nameof(array));

            var arrayList = new ArrayList(length);

            int count = startIndex + length;

            int i;

            for (i = startIndex; i < count; i++)

                _ = arrayList.Add(array[i]);

            return arrayList;

        }

        public static List<T> ToList<T>(this T[] array, in int startIndex, in int length)

        {

            ThrowIfNull(array, nameof(array));

            var arrayList = new List<T>(length);

            int count = startIndex + length;

            int i;

            for (i = startIndex; i < count; i++)

                arrayList.Add(array[i]);

            return arrayList;

        }

        public static object[] ToArray(this IList arrayList, in int startIndex, in int length)

        {

            ThrowIfNull(arrayList, nameof(arrayList));

            object[] array = new object[length];

            int i;

            for (i = 0; i < length; i++)

                array[i] = arrayList[i + startIndex];

            return array;

        }

        public static T[] ToArray<T>(this System.Collections.Generic.IList<T> arrayList, in int startIndex, in int length)

        {

            ThrowIfNull(arrayList, nameof(arrayList));

            var array = new T[length];

            int i;

            for (i = 0; i < length; i++)

                array[i] = arrayList[i + startIndex];

            return array;

        }

        // todo: add null checks, out-of-range checks, ...

        // todo: add same methods for arrays

        /// <summary>
        /// Removes multiple items in an <see cref="IList"/> collection, from a given start index for a given length.
        /// </summary>
        /// <param name="collection">The collection from which remove the items.</param>
        /// <param name="start">The start index in the collection from which delete the items.</param>
        /// <param name="length">The length to remove.</param>
        public static void RemoveRange(this IList collection, in int start, in int length)

        {

            ThrowIfNull(collection, nameof(collection));

            for (int i = 0; i < length; i++)

                collection.RemoveAt(start);

        }

        /// <summary>
        /// Appends data to the table. Arrays must have only one dimension.
        /// </summary>
        /// <param name="array">The source table.</param>
        /// <param name="arrays">The tables to concatenate.</param>
        /// <returns></returns>
        public static object[] Append(this Array array, params Array[] arrays) => Util.Concatenate((object[])array, arrays);

        /// <summary>
        /// Appends data to the table using the <see cref="Array.LongLength"/> length property. Arrays must have only one dimension.
        /// </summary>
        /// <param name="array">The source table.</param>
        /// <param name="arrays">The tables to concatenate.</param>
        /// <returns></returns>
        public static object[] AppendLong(this Array array, params Array[] arrays) => Util.ConcatenateLong((object[])array, arrays);

        /// <summary>
        /// Sorts an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the values in the <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>.</typeparam>
        /// <param name="oc">The <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> to sort.</param>
        public static void Sort<T>(this System.Collections.ObjectModel.ObservableCollection<T> oc)

        {

            ThrowIfNull(oc, nameof(oc));

            System.Collections.Generic.IList<T> sorted = oc.OrderBy(x => x).ToList<T>();

            for (int i = 0; i < sorted.Count; i++)

                oc.Move(oc.IndexOf(sorted[i]), i);

        }

        /// <summary>
        /// Sorts an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> with a user-defined comparer.
        /// </summary>
        /// <typeparam name="T">The type of the values in the <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>.</typeparam>
        /// <param name="oc">The <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> to sort.</param>
        /// <param name="comparer">An <see cref="System.Collections.Generic.IComparer{T}"/> providing comparison for sorting the <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>.</param>
        public static void Sort<T>(this System.Collections.ObjectModel.ObservableCollection<T> oc, in System.Collections.Generic.IComparer<T> comparer)

        {

            ThrowIfNull(oc, nameof(oc));

            System.Collections.Generic.IList<T> sorted = oc.OrderBy(x => x, comparer).ToList<T>();

            for (int i = 0; i < sorted.Count; i++)

                oc.Move(oc.IndexOf(sorted[i]), i);

        }

        #region Contains methods

        #region Non generic methods

        #region ContainsOneValue overloads

        public static bool ContainsOneValue(this IEnumerable array, in EqualityComparison comparison, out bool containsMoreThanOneValue, in object[] values)

        {

            ThrowIfNull(array, nameof(array));

            bool matchFound = false;

            foreach (object value in array)

                foreach (object _value in values)

                    if (comparison(value, _value))

                    {

                        if (matchFound)

                        {

                            containsMoreThanOneValue = true;

                            return false;

                        }

                        matchFound = true;

                    }

            containsMoreThanOneValue = false;

            return matchFound;

        }

        /// <summary>
        /// Checks whether an array contains <i>exactly</i> one value of a given array.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if <i>exactly</i> one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneValue(this IEnumerable array, out bool containsMoreThanOneValue, params object[] values) => ContainsOneValue(array, (object value, object _value) => object.Equals(value, _value), out containsMoreThanOneValue, values);

        /// <summary>
        /// Checks whether an array contains <i>exactly</i> one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparer">The <see cref="System.Collections.IComparer"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if <i>exactly</i> one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneValue(this IEnumerable array, System.Collections.IComparer comparer, out bool containsMoreThanOneValue, params object[] values)

        {

            ThrowIfNull(comparer, nameof(comparer));

            return ContainsOneValue(array, (object value, object _value) => comparer.Compare(value, _value) == 0, out containsMoreThanOneValue, values);

        }

        /// <summary>
        /// Checks whether an array contains <i>exactly</i> one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if <i>exactly</i> one value has been found, otherwise <see langword="false"/>.</returns>
        [Obsolete("This method has been replaced by the overload with the comparison parameter from WinCopies.Collections.Comparison.")]
        public static bool ContainsOneValue(this IEnumerable array, Comparison<object> comparison, out bool containsMoreThanOneValue, params object[] values) => ContainsOneValue(array, new WinCopies.Collections.Comparison((object x, object y) => comparison(x, y)), out containsMoreThanOneValue, values);

        /// <summary>
        /// Checks whether an array contains <i>exactly</i> one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparison">The <see cref="WinCopies.Collections.Comparison"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if <i>exactly</i> one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneValue(this IEnumerable array, WinCopies.Collections.Comparison comparison, out bool containsMoreThanOneValue, params object[] values)

        {

            ThrowIfNull(comparison, nameof(comparison));

            return ContainsOneValue(array, (object value, object _value) => comparison(value, _value) == 0, out containsMoreThanOneValue, values);

        }

        /// <summary>
        /// Checks whether an array contains <i>exactly</i> one value of a given array using a custom equality comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if <i>exactly</i> one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneValue(this IEnumerable array, IEqualityComparer equalityComparer, out bool containsMoreThanOneValue, params object[] values)

        {

            ThrowIfNull(equalityComparer, nameof(equalityComparer));

            return ContainsOneValue(array, (object value, object _value) => equalityComparer.Equals(value, _value), out containsMoreThanOneValue, values);

        }

        #endregion

        #region ContainsOneOrMoreValues with notification whether contains more than one values overloads

        public static bool ContainsOneOrMoreValues(IEnumerable array, in EqualityComparison comparison, out bool containsMoreThanOneValue, object[] values)

        {

            ThrowIfNull(array, nameof(array));

            bool matchFound = false;

            foreach (object value in array)

                foreach (object _value in values)

                    if (comparison(value, _value))

                    {

                        if (matchFound)

                        {

                            containsMoreThanOneValue = true;

                            return true;

                        }

                        matchFound = true;

                    }

            containsMoreThanOneValue = false;

            return matchFound;

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues(this IEnumerable array, out bool containsMoreThanOneValue, params object[] values) => ContainsOneOrMoreValues(array, (object value, object _value) => object.Equals(value, _value), out containsMoreThanOneValue, values);

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparer">The <see cref="System.Collections.IComparer"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues(this IEnumerable array, System.Collections.IComparer comparer, out bool containsMoreThanOneValue, params object[] values)

        {

            ThrowIfNull(comparer, nameof(comparer));

            return ContainsOneOrMoreValues(array, (object value, object _value) => comparer.Compare(value, _value) == 0, out containsMoreThanOneValue, values);

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues(this IEnumerable array, Comparison<object> comparison, out bool containsMoreThanOneValue, params object[] values)

        {

            ThrowIfNull(comparison, nameof(comparison));

            return ContainsOneOrMoreValues(array, (object value, object _value) => comparison(value, _value) == 0, out containsMoreThanOneValue, values);

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom equality comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues(this IEnumerable array, IEqualityComparer equalityComparer, out bool containsMoreThanOneValue, params object[] values)

        {

            ThrowIfNull(equalityComparer, nameof(equalityComparer));

            return ContainsOneOrMoreValues(array, (object value, object _value) => equalityComparer.Equals(value, _value), out containsMoreThanOneValue, values);

        }

        #endregion

        #region ContainsOneOrMoreValues without notification whether contains more than one values overloads

        public static bool ContainsOneOrMoreValues(IEnumerable array, in Func<object, object, bool> comparison, object[] values)

        {

            ThrowIfNull(array, nameof(array));

            foreach (object value in array)

                foreach (object _value in values)

                    if (comparison(value, _value))

                        return true;

            return false;

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues(this IEnumerable array, params object[] values) => ContainsOneOrMoreValues(array, (object value, object _value) => object.Equals(value, _value), values);

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparer">The <see cref="System.Collections.IComparer"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues(this IEnumerable array, System.Collections.IComparer comparer, params object[] values)

        {

            ThrowIfNull(comparer, nameof(comparer));

            return ContainsOneOrMoreValues(array, (object value, object _value) => comparer.Compare(value, _value) == 0, values);

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues(this IEnumerable array, Comparison<object> comparison, params object[] values)

        {

            ThrowIfNull(comparison, nameof(comparison));

            return ContainsOneOrMoreValues(array, (object value, object _value) => comparison(value, _value) == 0, values);

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues(this IEnumerable array, IEqualityComparer equalityComparer, params object[] values)

        {

            ThrowIfNull(equalityComparer, nameof(equalityComparer));

            return ContainsOneOrMoreValues(array, (object value, object _value) => equalityComparer.Equals(value, _value), values);

        }

        #endregion

        #region Contains array overloads

        public static bool Contains(IEnumerable array, in EqualityComparison comparison, object[] values)

        {

            ThrowIfNull(array, nameof(array));

            bool matchFound;

            foreach (object value in array)

            {

                matchFound = false;

                foreach (object _value in values)

                    if (comparison(value, _value))

                    {

                        matchFound = true;

                        break;

                    }

                if (!matchFound)

                    return false;

            }

            return true;

        }

        /// <summary>
        /// Checks whether an array contains all values of a given array.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool Contains(this IEnumerable array, params object[] values) => Contains(array, (object value, object _value) => object.Equals(value, _value), values);

        /// <summary>
        /// Checks whether an array contains all values of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparer">The <see cref="System.Collections.IComparer"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool Contains(this IEnumerable array, System.Collections.IComparer comparer, params object[] values)

        {

            ThrowIfNull(comparer, nameof(comparer));

            return Contains(array, (object value, object _value) => comparer.Compare(value, _value) == 0, values);

        }

        /// <summary>
        /// Checks whether an array contains all values of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool Contains(this IEnumerable array, Comparison<object> comparison, params object[] values)

        {

            ThrowIfNull(comparison, nameof(comparison));

            return Contains(array, (object value, object _value) => comparison(value, _value) == 0, values);

        }

        /// <summary>
        /// Checks whether an array contains all values of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool Contains(this IEnumerable array, IEqualityComparer equalityComparer, params object[] values)

        {

            ThrowIfNull(equalityComparer, nameof(equalityComparer));

            return Contains(array, (object value, object _value) => equalityComparer.Equals(value, _value), values);

        }

        #endregion

        #endregion

        #region Generic methods

        #region ContainsOneValue overloads

        public static bool ContainsOneValue<T>(IEnumerable<T> array, in EqualityComparison<T> comparison, out bool containsMoreThanOneValue, in T[] values)

        {

            ThrowIfNull(array, nameof(array));

            bool matchFound = false;

            foreach (T value in array)

                foreach (T _value in values)

                    if (comparison(value, _value))

                    {

                        if (matchFound)

                        {

                            containsMoreThanOneValue = true;

                            return false;

                        }

                        matchFound = true;

                    }

            containsMoreThanOneValue = false;

            return matchFound;

        }

        /// <summary>
        /// Checks whether an array contains <i>exactly</i> one value of a given array.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if <i>exactly</i> one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneValue<T>(this IEnumerable<T> array, out bool containsMoreThanOneValue, params T[] values) => ContainsOneValue(array, (T value, T _value) => object.Equals(value, _value), out containsMoreThanOneValue, values);

        /// <summary>
        /// Checks whether an array contains <i>exactly</i> one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if <i>exactly</i> one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneValue<T>(this IEnumerable<T> array, System.Collections.Generic.IComparer<T> comparer, out bool containsMoreThanOneValue, params T[] values)

        {

            ThrowIfNull(comparer, nameof(comparer));

            return ContainsOneValue(array, (T value, T _value) => comparer.Compare(value, _value) == 0, out containsMoreThanOneValue, values);

        }

        /// <summary>
        /// Checks whether an array contains <i>exactly</i> one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if <i>exactly</i> one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneValue<T>(this IEnumerable<T> array, Comparison<T> comparison, out bool containsMoreThanOneValue, params T[] values)

        {

            ThrowIfNull(comparison, nameof(comparison));

            return ContainsOneValue(array, (T value, T _value) => comparison(value, _value) == 0, out containsMoreThanOneValue, values);

        }

        /// <summary>
        /// Checks whether an array contains <i>exactly</i> one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if <i>exactly</i> one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneValue<T>(this IEnumerable<T> array, IEqualityComparer<T> equalityComparer, out bool containsMoreThanOneValue, params T[] values)

        {

            ThrowIfNull(equalityComparer, nameof(equalityComparer));

            return ContainsOneValue(array, (T value, T _value) => equalityComparer.Equals(value, _value), out containsMoreThanOneValue, values); ;

        }

        #endregion

        #region ContainsOneOrMoreValues with notification whether contains more than one values overloads

        public static bool ContainsOneOrMoreValues<T>(IEnumerable<T> array, in EqualityComparison<T> comparison, out bool containsMoreThanOneValue, in T[] values)

        {

            ThrowIfNull(array, nameof(array));

            bool matchFound = false;

            foreach (T value in array)

                foreach (T _value in values)

                    if (comparison(value, _value))

                    {

                        if (matchFound)

                        {

                            containsMoreThanOneValue = true;

                            return true;

                        }

                        matchFound = true;

                    }

            containsMoreThanOneValue = false;

            return matchFound;

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues<T>(this IEnumerable<T> array, out bool containsMoreThanOneValue, params T[] values) => ContainsOneOrMoreValues(array, (T value, T _value) => object.Equals(value, _value), out containsMoreThanOneValue, values);

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues<T>(this IEnumerable<T> array, System.Collections.Generic.IComparer<T> comparer, out bool containsMoreThanOneValue, params T[] values)

        {

            ThrowIfNull(comparer, nameof(comparer));

            return ContainsOneOrMoreValues(array, (T value, T _value) => comparer.Compare(value, _value) == 0, out containsMoreThanOneValue, values);

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues<T>(this IEnumerable<T> array, Comparison<T> comparison, out bool containsMoreThanOneValue, params T[] values)

        {

            ThrowIfNull(comparison, nameof(comparison));

            return ContainsOneOrMoreValues(array, (T value, T _value) => comparison(value, _value) == 0, out containsMoreThanOneValue, values);

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> used to compare the values</param>
        /// <param name="containsMoreThanOneValue"><see langword="true"/> if more than one value has been found, otherwise <see langword="false"/></param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues<T>(this IEnumerable<T> array, IEqualityComparer<T> equalityComparer, out bool containsMoreThanOneValue, params T[] values)

        {

            ThrowIfNull(equalityComparer, nameof(equalityComparer));

            return ContainsOneOrMoreValues(array, (T value, T _value) => equalityComparer.Equals(value, _value), out containsMoreThanOneValue, values);

        }

        #endregion

        #region ContainsOneOrMoreValues without notification whether contains more than one values overloads

        public static bool ContainsOneOrMoreValues<T>(IEnumerable<T> array, in EqualityComparison<T> comparison, in T[] values)

        {

            ThrowIfNull(array, nameof(array));

            foreach (T value in array)

                foreach (T _value in values)

                    if (comparison(value, _value))

                        return true;

            return false;

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues<T>(this IEnumerable<T> array, params T[] values) => ContainsOneOrMoreValues(array, (T value, T _value) => object.Equals(value, _value), values);

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues<T>(this IEnumerable<T> array, System.Collections.Generic.IComparer<T> comparer, params T[] values)

        {

            ThrowIfNull(comparer, nameof(comparer));

            return ContainsOneOrMoreValues(array, (T value, T _value) => comparer.Compare(value, _value) == 0, values);

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues<T>(this IEnumerable<T> array, Comparison<T> comparison, params T[] values)

        {

            ThrowIfNull(comparison, nameof(comparison));

            return ContainsOneOrMoreValues(array, (T value, T _value) => comparison(value, _value) == 0, values);

        }

        /// <summary>
        /// Checks whether an array contains at least one value of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool ContainsOneOrMoreValues<T>(this IEnumerable<T> array, IEqualityComparer<T> equalityComparer, params T[] values)

        {

            ThrowIfNull(equalityComparer, nameof(equalityComparer));

            return ContainsOneOrMoreValues(array, (T value, T _value) => equalityComparer.Equals(value, _value), values);

        }

        #endregion

        #region Contains array overloads

        public static bool Contains<T>(IEnumerable<T> array, in EqualityComparison<T> comparison, in T[] values)

        {

            ThrowIfNull(array, nameof(array));

            bool matchFound;

            foreach (T value in array)

            {

                matchFound = false;

                foreach (T _value in values)

                    if (comparison(value, _value))

                    {

                        matchFound = true;

                        break;

                    }

                if (!matchFound)

                    return false;

            }

            return true;

        }

        /// <summary>
        /// Checks whether an array contains all values of a given array.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool Contains<T>(this IEnumerable<T> array, params T[] values) => Contains(array, (T value, T _value) => object.Equals(value, _value), values);

        /// <summary>
        /// Checks whether an array contains all values of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool Contains<T>(this IEnumerable<T> array, System.Collections.Generic.IComparer<T> comparer, params T[] values)

        {

            ThrowIfNull(comparer, nameof(comparer));

            return Contains(array, (T value, T _value) => comparer.Compare(value, _value) == 0, values);

        }

        /// <summary>
        /// Checks whether an array contains all values of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool Contains<T>(this IEnumerable<T> array, Comparison<T> comparison, params T[] values)

        {

            ThrowIfNull(comparison, nameof(comparison));

            return Contains(array, (T value, T _value) => comparison(value, _value) == 0, values);

        }

        /// <summary>
        /// Checks whether an array contains all values of a given array using a custom comparer.
        /// </summary>
        /// <param name="array">The array to browse</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> used to compare the values</param>
        /// <param name="values">The values to compare</param>
        /// <returns><see langword="true"/> if at least one value has been found, otherwise <see langword="false"/>.</returns>
        public static bool Contains<T>(this IEnumerable<T> array, IEqualityComparer<T> equalityComparer, params T[] values)

        {

            ThrowIfNull(equalityComparer, nameof(equalityComparer));

            return Contains(array, (T value, T _value) => equalityComparer.Equals(value, _value), values);

        }

        #endregion

        #endregion

        #endregion

        public static string ToString(this IEnumerable array, in bool parseSubEnumerables, in bool parseStrings = false)

        {

            ThrowIfNull(array, nameof(array));

            var stringBuilder = new StringBuilder();

            array.ToString(ref stringBuilder, parseSubEnumerables, parseStrings);

            return stringBuilder.ToString(0, stringBuilder.Length - 2);

        }

        static void Append(object _value, ref StringBuilder stringBuilder, in bool parseStrings, in bool parseSubEnumerables)

        {

            ThrowIfNull(stringBuilder, nameof(stringBuilder));

            if ((_value is string && parseStrings) || (!(_value is string) && _value is IEnumerable && parseSubEnumerables))

                ((IEnumerable)_value).ToString(ref stringBuilder, true);

            else

                _ = stringBuilder.AppendFormat("{0}, ", _value?.ToString());

        }

        private static void ToString(this IEnumerable array, ref StringBuilder stringBuilder, in bool parseSubEnumerables, in bool parseStrings = false)

        {

            _ = stringBuilder.Append("{");

            IEnumerator enumerator = array.GetEnumerator();

            bool atLeastOneLoop = false;

            if (enumerator.MoveNext())

            {

                atLeastOneLoop = true;

                Append(enumerator.Current, ref stringBuilder, parseStrings, parseSubEnumerables);

            }

            while (enumerator.MoveNext())

                Append(enumerator.Current, ref stringBuilder, parseStrings, parseSubEnumerables);

            _ = atLeastOneLoop ? stringBuilder.Insert(stringBuilder.Length - 2, "}") : stringBuilder.Append("}");

        }

        public static T FirstOrDefault<T>(this IEnumerable enumerable)

        {

            foreach (var obj in enumerable)

                if (obj is T _obj) return _obj;

            return default;

        }

        #endregion

        /// <summary>
        /// Checks if the current object is assignable from at least one type of a given <see cref="Type"/> array.
        /// </summary>
        /// <param name="obj">The object from which check the type</param>
        /// <param name="typeEquality"><see langword="true"/> to preserve type equality, regardless of the type inheritance, otherwise <see langword="false"/></param>
        /// <param name="types">The types to compare</param>
        /// <returns><see langword="true"/> if the current object is assignable from at least one of the given types, otherwise <see langword="false"/>.</returns>
        public static bool Is(this object obj, in bool typeEquality, params Type[] types)

        {

            ThrowIfNull(obj, nameof(obj));

            Type objType = obj.GetType();

            foreach (Type type in types)

                if (typeEquality ? objType == type : type.IsAssignableFrom(objType))

                    return true;

            return false;

        }

        public static IEnumerable<TKey> GetKeys<TKey, TValue>(this KeyValuePair<TKey, TValue>[] array)

        {

            ThrowIfNull(array, nameof(array));

            foreach (KeyValuePair<TKey, TValue> value in array)

                yield return value.Key;

        }

        public static IEnumerable<TValue> GetValues<TKey, TValue>(this KeyValuePair<TKey, TValue>[] array)

        {

            ThrowIfNull(array, nameof(array));

            foreach (KeyValuePair<TKey, TValue> value in array)

                yield return value.Value;

        }

        public static bool CheckIntegrity<TKey, TValue>(this KeyValuePair<TKey, TValue>[] array)

        {

            static bool predicateByVal(TKey keyA, TKey keyB) => Equals(keyA, keyB);

            static bool predicateByRef(TKey keyA, TKey keyB) => ReferenceEquals(keyA, keyB);

            Func<TKey, TKey, bool> predicate = typeof(TKey).IsClass ? predicateByRef : (Func<TKey, TKey, bool>)predicateByVal;

            IEnumerable<TKey> keys = array.GetKeys();

            IEnumerable<TKey> _keys = array.GetKeys();

            bool foundOneOccurrence = false;

            foreach (TKey key in keys)

            {

                if (key == null)

                    throw new ArgumentException("One or more key is null.");

                foreach (TKey _key in _keys)

                {

                    if (predicate(key, _key))

                        if (foundOneOccurrence)

                            return false;

                        else

                            foundOneOccurrence = true;

                }

                foundOneOccurrence = false;

            }

            return true;

        }

        public static bool CheckPropertySetIntegrity(Type propertyObjectType, in string propertyName, out string methodName, in int skipFrames, in BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet)

        {

            ThrowIfNull(propertyObjectType, nameof(propertyObjectType));

            PropertyInfo property = propertyObjectType.GetProperty(propertyName, bindingFlags);

            if (property == null)

                throw new ArgumentException(string.Format(FieldOrPropertyNotFound, propertyName, propertyObjectType));

            MethodBase method = new StackFrame(skipFrames).GetMethod();

            methodName = method.Name;

            //#if DEBUG 

            //            Debug.WriteLine("Property: " + property.Name + ", " + property.DeclaringType);

            //            Debug.WriteLine("Method: " + method.Name + ", " + method.DeclaringType);

            //#endif 

            // todo: tuple and check DeclaringTypeNotCorrespond throws

            return (property.CanWrite && property.GetSetMethod() != null) || property.DeclaringType == method.DeclaringType;

        }

        internal static FieldInfo GetField( in string fieldName, in Type objectType, in BindingFlags bindingFlags)

        {

            BindingFlags flags = bindingFlags;

            // var objectType = obj.GetType(); 

            FieldInfo field = objectType.GetField(fieldName, flags);

            if (field == null)

                throw new ArgumentException(string.Format(FieldOrPropertyNotFound, fieldName, objectType));

            return field;

        }

        internal static PropertyInfo GetProperty( in string propertyName, in Type objectType, in BindingFlags bindingFlags)

        {

            BindingFlags flags = bindingFlags;

            // var objectType = obj.GetType(); 

            PropertyInfo property = objectType.GetProperty(propertyName, flags);

            if (property == null)

                throw new ArgumentException(string.Format(FieldOrPropertyNotFound, propertyName, objectType));

            return property;

        }

        // todo: use attributes

        private static (bool fieldChanged, object oldValue) SetField(this object obj, in FieldInfo field, in object previousValue, in object newValue, in string paramName, in bool setOnlyIfNotNull, in bool throwIfNull, in bool disposeOldValue, in FieldValidateValueCallback validateValueCallback, in bool throwIfValidationFails, in FieldValueChangedCallback valueChangedCallback)

        {

            if (newValue is null)

                if (throwIfNull)

                    throw new ArgumentNullException(nameof(paramName));

                else if (setOnlyIfNotNull)

                    return (false, previousValue);

            (bool validationResult, Exception validationException) = validateValueCallback?.Invoke(obj, newValue, field, paramName) ?? (true, null);

            if (validationResult)

                if ((newValue == null && previousValue != null) || (newValue != null && !newValue.Equals(previousValue)))

                {

                    if (disposeOldValue)

                        ((IDisposable)previousValue).Dispose();

                    field.SetValue(obj, newValue);

                    valueChangedCallback?.Invoke(obj, newValue, field, paramName);

                    return (true, previousValue);

                    //BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                    //             BindingFlags.Static | BindingFlags.Instance |
                    //             BindingFlags.DeclaredOnly;
                    //this.GetType().GetField(fieldName, flags).SetValue(this, newValue);

                }

                else

                    return (false, previousValue);

            else

                return throwIfValidationFails ? throw (validationException ?? new ArgumentException("Validation error.", paramName)) : (false, previousValue);

        }

        public static (bool fieldChanged, object oldValue) SetField(this object obj, in string fieldName, in object newValue, in Type declaringType, in BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, in string paramName = null, in bool setOnlyIfNotNull = false, in bool throwIfNull = false, in FieldValidateValueCallback validateValueCallback = null, in bool throwIfValidationFails = false, in FieldValueChangedCallback valueChangedCallback = null)

        {

            ThrowIfNull(declaringType, nameof(declaringType));

            FieldInfo field = GetField(fieldName, declaringType, bindingFlags);

            return obj.SetField(field, field.GetValue(obj), newValue, paramName, setOnlyIfNotNull, throwIfNull, false, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        }

        public static (bool fieldChanged, IDisposable oldValue) DisposeAndSetField(this object obj, in string fieldName, in IDisposable newValue, in Type declaringType, in BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, in string paramName = null, in bool setOnlyIfNotNull = false, in bool throwIfNull = false, in FieldValidateValueCallback validateValueCallback = null, in bool throwIfValidationFails = false, in FieldValueChangedCallback valueChangedCallback = null)

        {

            ThrowIfNull(declaringType, nameof(declaringType));

            FieldInfo field = GetField(fieldName, declaringType, bindingFlags);

            return ((bool, IDisposable))obj.SetField(field, field.GetValue(obj), newValue, paramName, setOnlyIfNotNull, throwIfNull, true, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        }

        // todo: update code (in, throw if null)

        /// <summary>
        /// Sets a value to a property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="fieldName">The field related to the property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The declaring types of the given property and field name doesn't correspond. OR The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, object oldValue) SetProperty(this object obj, string propertyName, string fieldName, object newValue, Type declaringType, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, FieldValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, FieldValueChangedCallback valueChangedCallback = null)

        {

            //BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
            //             BindingFlags.Static | BindingFlags.Instance |
            //             BindingFlags.DeclaredOnly;
            //this.GetType().GetField(fieldName, flags).SetValue(this, newValue);

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, previousValue, newValue)); 

            // if (declaringType == null) 

            // {

            //while (objectType != declaringType && objectType != typeof(object))

            //    objectType = objectType.BaseType;

            //if (objectType != declaringType)

            //    throw new ArgumentException(string.Format((string)ResourcesHelper.GetResource("DeclaringTypeIsNotInObjectInheritanceHierarchyException"), declaringType, objectType));

            // }

            //#if DEBUG

            //            var fields = objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            //            foreach (var _field in fields)

            //                Debug.WriteLine("Object type: " + objectType + " " + _field.Name);

            //#endif

            // var objectType = obj.GetType();

            FieldInfo field = GetField(fieldName, declaringType, bindingFlags);

            object previousValue = field.GetValue(obj);

            if (!CheckPropertySetIntegrity(declaringType, propertyName, out string methodName, 3, bindingFlags))

                throw new InvalidOperationException(string.Format(DeclaringTypesNotCorrespond, propertyName, methodName));

            PropertyInfo property = GetProperty(propertyName, declaringType, bindingFlags);

            return !property.CanWrite || property.SetMethod == null
                ? throwIfReadOnly ? throw new InvalidOperationException(string.Format("This property is read-only. Property name: {0}, declaring type: {1}.", propertyName, declaringType)) : (false, previousValue)
                : obj.SetField(field, previousValue, newValue, paramName, setOnlyIfNotNull, throwIfNull, false, validateValueCallback, throwIfValidationFails, valueChangedCallback);
        }

        /// <summary>
        /// Sets a value to a property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, object oldValue) SetProperty(this object obj, string propertyName, object newValue, Type declaringType, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, PropertyValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, PropertyValueChangedCallback valueChangedCallback = null)

        {

            PropertyInfo property = GetProperty(propertyName, declaringType, bindingFlags);

            object previousValue = property.GetValue(obj);

            if (!property.CanWrite || property.SetMethod == null)

                return throwIfReadOnly ? throw new InvalidOperationException(string.Format("This property is read-only. Property name: {0}, declaring type: {1}.", propertyName, declaringType)) : (false, previousValue);

            if (newValue is null)

                if (throwIfNull)

                    throw new ArgumentNullException(nameof(paramName));

                else if (setOnlyIfNotNull)

                    return (false, previousValue);

            (bool validationResult, Exception validationException) = validateValueCallback?.Invoke(obj, newValue, property, paramName) ?? (true, null);

            if (validationResult)

                if ((newValue == null && previousValue != null) || (newValue != null && !newValue.Equals(previousValue)))

                {

                    property.SetValue(obj, newValue);

                    valueChangedCallback?.Invoke(obj, newValue, property, paramName);

                    return (true, previousValue);

                    //BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                    //             BindingFlags.Static | BindingFlags.Instance |
                    //             BindingFlags.DeclaredOnly;
                    //this.GetType().GetField(fieldName, flags).SetValue(this, newValue);

                }

                else

                    return (false, previousValue);

            else

                return throwIfValidationFails ? throw (validationException ?? new ArgumentException("Validation error.", paramName)) : (false, previousValue);

        }


        /// <summary>
        /// Disposes an old value of a property then sets a new value to the given property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="fieldName">The field related to the property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The declaring types of the given property and field name doesn't correspond. OR The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, IDisposable oldValue) DisposeAndSetProperty(this object obj, string propertyName, string fieldName, IDisposable newValue, Type declaringType, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, FieldValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, FieldValueChangedCallback valueChangedCallback = null)

        {

            //BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
            //             BindingFlags.Static | BindingFlags.Instance |
            //             BindingFlags.DeclaredOnly;
            //this.GetType().GetField(fieldName, flags).SetValue(this, newValue);

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, previousValue, newValue)); 

            // if (declaringType == null) 

            // {

            //while (objectType != declaringType && objectType != typeof(object))

            //    objectType = objectType.BaseType;

            //if (objectType != declaringType)

            //    throw new ArgumentException(string.Format((string)ResourcesHelper.GetResource("DeclaringTypeIsNotInObjectInheritanceHierarchyException"), declaringType, objectType));

            // }

            //#if DEBUG

            //            var fields = objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            //            foreach (var _field in fields)

            //                Debug.WriteLine("Object type: " + objectType + " " + _field.Name);

            //#endif

            // var objectType = obj.GetType();

            FieldInfo field = GetField(fieldName, declaringType, bindingFlags);

            var previousValue = (IDisposable)field.GetValue(obj);

            if (!CheckPropertySetIntegrity(declaringType, propertyName, out string methodName, 3, bindingFlags))

                throw new InvalidOperationException(string.Format(DeclaringTypesNotCorrespond, propertyName, methodName));

            PropertyInfo property = GetProperty(propertyName, declaringType, bindingFlags);

            return !property.CanWrite || property.SetMethod == null
                ? throwIfReadOnly ? throw new InvalidOperationException(string.Format("This property is read-only. Property name: {0}, declaring type: {1}.", propertyName, declaringType)) : (false, previousValue)
                : ((bool, IDisposable))obj.SetField(field, previousValue, newValue, paramName, setOnlyIfNotNull, throwIfNull, true, validateValueCallback, throwIfValidationFails, valueChangedCallback);
        }

        /// <summary>
        /// Disposes an old value of a property then sets a new value to the given property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, IDisposable oldValue) DisposeAndSetProperty(this object obj, string propertyName, IDisposable newValue, Type declaringType, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, PropertyValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, PropertyValueChangedCallback valueChangedCallback = null)

        {

            PropertyInfo property = GetProperty(propertyName, declaringType, bindingFlags);

            var previousValue = (IDisposable)property.GetValue(obj);

            if (!property.CanWrite || property.SetMethod == null)

                return throwIfReadOnly ? throw new InvalidOperationException(string.Format("This property is read-only. Property name: {0}, declaring type: {1}.", propertyName, declaringType)) : (false, previousValue);

            if (newValue is null)

                if (throwIfNull)

                    throw new ArgumentNullException(nameof(paramName));

                else if (setOnlyIfNotNull)

                    return (false, previousValue);

            (bool validationResult, Exception validationException) = validateValueCallback?.Invoke(obj, newValue, property, paramName) ?? (true, null);

            if (validationResult)

                if ((newValue == null && previousValue != null) || (newValue != null && !newValue.Equals(previousValue)))

                {

                    previousValue.Dispose();

                    property.SetValue(obj, newValue);

                    valueChangedCallback?.Invoke(obj, newValue, property, paramName);

                    return (true, previousValue);

                    //BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                    //             BindingFlags.Static | BindingFlags.Instance |
                    //             BindingFlags.DeclaredOnly;
                    //this.GetType().GetField(fieldName, flags).SetValue(this, newValue);

                }

                else

                    return (false, previousValue);

            else

                return throwIfValidationFails ? throw (validationException ?? new ArgumentException("Validation error.", paramName)) : (false, previousValue);

        }

        /// <summary>
        /// Sets a value to a property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="fieldName">The field related to the property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfBusy">Whether to throw if <paramref name="obj"/> is busy.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The declaring types of the given property and field name doesn't correspond. OR The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, object oldValue) SetBackgroundWorkerProperty(this System.ComponentModel.BackgroundWorker obj, string propertyName, string fieldName, object newValue, Type declaringType, bool throwIfBusy, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, FieldValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, FieldValueChangedCallback valueChangedCallback = null) => obj.IsBusy
                ? throwIfBusy ? throw new InvalidOperationException(BackgroundWorkerIsBusy) : (false, GetField(fieldName, declaringType, bindingFlags).GetValue(obj))
                : obj.SetProperty(propertyName, fieldName, newValue, declaringType, throwIfReadOnly, bindingFlags, paramName, setOnlyIfNotNull, throwIfNull, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        /// <summary>
        /// Sets a value to a property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfBusy">Whether to throw if <paramref name="obj"/> is busy.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, object oldValue) SetBackgroundWorkerProperty(this System.ComponentModel.BackgroundWorker obj, string propertyName, object newValue, Type declaringType, bool throwIfBusy, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, PropertyValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, PropertyValueChangedCallback valueChangedCallback = null) => obj.IsBusy
                ? throwIfBusy ? throw new InvalidOperationException(BackgroundWorkerIsBusy) : (false, GetProperty(propertyName, declaringType, bindingFlags).GetValue(obj))
                : obj.SetProperty(propertyName, newValue, declaringType, throwIfReadOnly, bindingFlags, paramName, setOnlyIfNotNull, throwIfNull, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        /// <summary>
        /// Sets a value to a property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="fieldName">The field related to the property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfBusy">Whether to throw if <paramref name="obj"/> is busy.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The declaring types of the given property and field name doesn't correspond.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, object oldValue) SetBackgroundWorkerProperty(this IBackgroundWorker obj, string propertyName, string fieldName, object newValue, Type declaringType, bool throwIfBusy, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, FieldValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, FieldValueChangedCallback valueChangedCallback = null) => obj.IsBusy
                ? throwIfBusy ? throw new InvalidOperationException(BackgroundWorkerIsBusy) : (false, GetField(fieldName, declaringType, bindingFlags).GetValue(obj))
                : obj.SetProperty(propertyName, fieldName, newValue, declaringType, throwIfReadOnly, bindingFlags, paramName, setOnlyIfNotNull, throwIfNull, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        /// <summary>
        /// Sets a value to a property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfBusy">Whether to throw if <paramref name="obj"/> is busy.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, object oldValue) SetBackgroundWorkerProperty(this IBackgroundWorker obj, string propertyName, object newValue, Type declaringType, bool throwIfBusy, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, PropertyValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, PropertyValueChangedCallback valueChangedCallback = null) => obj.IsBusy
                ? throwIfBusy ? throw new InvalidOperationException(BackgroundWorkerIsBusy) : (false, GetProperty(propertyName, declaringType, bindingFlags).GetValue(obj))
                : obj.SetProperty(propertyName, newValue, declaringType, throwIfReadOnly, bindingFlags, paramName, setOnlyIfNotNull, throwIfNull, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        /// <summary>
        /// Disposes an old value of a property then sets a new value to the given property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="fieldName">The field related to the property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfBusy">Whether to throw if <paramref name="obj"/> is busy.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The declaring types of the given property and field name doesn't correspond. OR The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, IDisposable oldValue) DisposeAndSetBackgroundWorkerProperty(this System.ComponentModel.BackgroundWorker obj, string propertyName, string fieldName, IDisposable newValue, Type declaringType, bool throwIfBusy, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, FieldValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, FieldValueChangedCallback valueChangedCallback = null) => obj.IsBusy
                ? throwIfBusy ? throw new InvalidOperationException(BackgroundWorkerIsBusy) : (false, (IDisposable)GetField(fieldName, declaringType, bindingFlags).GetValue(obj))
                : obj.DisposeAndSetProperty(propertyName, fieldName, newValue, declaringType, throwIfReadOnly, bindingFlags, paramName, setOnlyIfNotNull, throwIfNull, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        /// <summary>
        /// Disposes an old value of a property then sets a new value to the given property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfBusy">Whether to throw if <paramref name="obj"/> is busy.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, IDisposable oldValue) DisposeAndSetBackgroundWorkerProperty(this System.ComponentModel.BackgroundWorker obj, string propertyName, IDisposable newValue, Type declaringType, bool throwIfBusy, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, PropertyValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, PropertyValueChangedCallback valueChangedCallback = null) => obj.IsBusy
                ? throwIfBusy ? throw new InvalidOperationException(BackgroundWorkerIsBusy) : (false, (IDisposable)GetProperty(propertyName, declaringType, bindingFlags).GetValue(obj))
                : obj.DisposeAndSetProperty(propertyName, newValue, declaringType, throwIfReadOnly, bindingFlags, paramName, setOnlyIfNotNull, throwIfNull, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        /// <summary>
        /// Disposes an old value of a property then sets a new value to the given property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="fieldName">The field related to the property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfBusy">Whether to throw if <paramref name="obj"/> is busy.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The declaring types of the given property and field name doesn't correspond.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, IDisposable oldValue) DisposeAndSetBackgroundWorkerProperty(this IBackgroundWorker obj, string propertyName, string fieldName, IDisposable newValue, Type declaringType, bool throwIfBusy, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, FieldValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, FieldValueChangedCallback valueChangedCallback = null) => obj.IsBusy
                ? throwIfBusy ? throw new InvalidOperationException(BackgroundWorkerIsBusy) : (false, (IDisposable)GetField(fieldName, declaringType, bindingFlags).GetValue(obj))
                : obj.DisposeAndSetProperty(propertyName, fieldName, newValue, declaringType, throwIfReadOnly, bindingFlags, paramName, setOnlyIfNotNull, throwIfNull, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        /// <summary>
        /// Disposes an old value of a property then sets a new value to the given property if the new value is different.
        /// </summary>
        /// <param name="obj">The object in which to set the property.</param>
        /// <param name="propertyName">The name of the given property.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="declaringType">The actual declaring type of the property.</param>
        /// <param name="throwIfBusy">Whether to throw if <paramref name="obj"/> is busy.</param>
        /// <param name="throwIfReadOnly">Whether to throw if the given property is read-only.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to get the property.</param>
        /// <param name="paramName">The parameter from which the value was passed to this method.</param>
        /// <param name="setOnlyIfNotNull">Whether to set only if the given value is not null.</param>
        /// <param name="throwIfNull">Whether to throw if the given value is null.</param>
        /// <param name="validateValueCallback">The callback used to validate the given value. You can leave this parameter to null if you don't want to perform validation.</param>
        /// <param name="throwIfValidationFails">Whether to throw if the validation of <paramref name="validateValueCallback"/> fails.</param>
        /// <param name="valueChangedCallback">The callback used to perform actions after the property is set. You can leave this parameter to null if you don't want to perform actions after the property is set.</param>
        /// <returns>A <see cref="bool"/> value that indicates whether the setting succeeded and the old value of the given property (or <see langword="null"/> if the property does not contain any value nor reference).</returns>
        /// <exception cref="InvalidOperationException">The given property is read-only and <paramref name="throwIfReadOnly"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="ArgumentNullException">The new value is null and <paramref name="throwIfNull"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="Exception"><paramref name="validateValueCallback"/> failed and <paramref name="throwIfValidationFails"/> is set to <see langword="true"/>. This exception is the exception that was returned by <paramref name="validateValueCallback"/> if it was not null or an <see cref="ArgumentException"/> otherwise.</exception>
        public static (bool propertyChanged, IDisposable oldValue) DisposeAndSetBackgroundWorkerProperty(this IBackgroundWorker obj, string propertyName, IDisposable newValue, Type declaringType, bool throwIfBusy, bool throwIfReadOnly = true, BindingFlags bindingFlags = Util.DefaultBindingFlagsForPropertySet, string paramName = null, bool setOnlyIfNotNull = false, bool throwIfNull = false, PropertyValidateValueCallback validateValueCallback = null, bool throwIfValidationFails = false, PropertyValueChangedCallback valueChangedCallback = null) => obj.IsBusy
                ? throwIfBusy ? throw new InvalidOperationException(BackgroundWorkerIsBusy) : (false, (IDisposable)GetProperty(propertyName, declaringType, bindingFlags).GetValue(obj))
                : obj.DisposeAndSetProperty(propertyName, newValue, declaringType, throwIfReadOnly, bindingFlags, paramName, setOnlyIfNotNull, throwIfNull, validateValueCallback, throwIfValidationFails, valueChangedCallback);

        /// <summary>
        /// Gets the numeric value for an enum.
        /// </summary>
        /// <param name="enum">The enum for which get the corresponding numeric value.</param>
        /// <param name="enumName">Not used.</param>
        /// <returns>The numeric value corresponding to this enum, in the given enum type underlying type.</returns>
        [Obsolete("This method has been replaced by the GetNumValue(this Enum @enum) and the WinCopies.Util.GetNumValue(Type enumType, string fieldName) methods and will be removed in later versions.")]
        public static object GetNumValue(this Enum @enum, string enumName) => @enum.GetNumValue();

        // todo: IFormatProvider

        /// <summary>
        /// Gets the numeric value for an enum.
        /// </summary>
        /// <param name="enum">The enum for which get the corresponding numeric value.</param>
        /// <returns>The numeric value corresponding to this enum, in the given enum type underlying type.</returns>
        public static object GetNumValue(this Enum @enum) => Convert.ChangeType(@enum, Enum.GetUnderlyingType(@enum.GetType()));

        // public static object GetNumValue(this Enum @enum) => GetNumValue(@enum, @enum.ToString());

        // todo : to test if Math.Log(Convert.ToInt64(flagsEnum), 2) == 'SomeInt64'; (no float, double ...) would be faster.

        /// <summary>
        /// Determines whether an enum has multiple flags.
        /// </summary>
        /// <param name="flagsEnum">The enum to check.</param>
        /// <returns><see langword="true"/> if <paramref name="flagsEnum"/> type has the <see cref="FlagsAttribute"/> and has multiple flags; otherwise, <see langword="false"/>.</returns>
        /// <remarks><paramref name="flagsEnum"/> type must have the <see cref="FlagsAttribute"/>.</remarks>
        public static bool HasMultipleFlags(this Enum flagsEnum)

        {

            Type type = flagsEnum.GetType();

            if (type.GetCustomAttributes(typeof(FlagsAttribute)).Count() == 0)

                return false; // throw new ArgumentException(string.Format("This enum does not implement the {0} attribute.", typeof(FlagsAttribute).Name));



            bool alreadyFoundAFlag = false;

            Enum enumValue;

            // FieldInfo field = null;



            foreach (string s in type.GetEnumNames())

            {

                enumValue = (Enum)Enum.Parse(type, s);



                if (enumValue.GetNumValue().Equals(0)) continue;



                if (flagsEnum.HasFlag(enumValue))

                    if (alreadyFoundAFlag) return true;

                    else alreadyFoundAFlag = true;

            }

            return false;

        }

        /// <summary>
        /// Determines whether the current enum value is within the enum values range delimited by the first and the last fields; see the Remarks section for more information.
        /// </summary>
        /// <param name="enum">The enum value to check.</param>
        /// <returns><see langword="true"/> if the given value is in the enum values range, otherwise <see langword="false"/>.</returns>
        /// <remarks>This method doesn't read all the enum fields, but only takes care of the first and last numeric enum fields, so if the value is 1, and the enum has only defined fields for 0 and 2, this method still returns <see langword="true"/>. For a method that actually reads all the enum fields, see the <see cref="Type.IsEnumDefined(object)"/> method.</remarks>
        /// <seealso cref="ThrowIfNotValidEnumValue(Enum)"/>
        /// <seealso cref="ThrowIfNotDefinedEnumValue(Enum)"/>
        /// <seealso cref="ThrowIfNotValidEnumValue(Enum, string)"/>
        /// <seealso cref="ThrowIfNotDefinedEnumValue(Enum, string)"/>
        public static bool IsValidEnumValue(this Enum @enum)

        {

            var values = new ArrayList(@enum.GetType().GetEnumValues());

            values.Sort();

            // object _value = Convert.ChangeType(value, value.GetType().GetEnumUnderlyingType());

            return @enum.CompareTo(values[0]) >= 0 && @enum.CompareTo(values[values.Count - 1]) <= 0;

        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the enum value is not in the required enum value range. See the Remarks section.
        /// </summary>
        /// <param name="enum">The enum value to check.</param>
        /// <remarks>This method doesn't read all the enum fields, but only takes care of the first and last numeric enum fields, so if the value is 1, and the enum has only defined fields for 0 and 2, this method still doesn't throw. For a method that actually reads all the enum fields, see the <see cref="ThrowIfNotDefinedEnumValue(Enum)"/> method.</remarks>
        /// <seealso cref="IsValidEnumValue(Enum)"/>
        /// <seealso cref="ThrowIfNotValidEnumValue(Enum, string)"/>
        public static void ThrowIfNotValidEnumValue(this Enum @enum)

        {

            if (!@enum.IsValidEnumValue()) throw new InvalidOperationException(string.Format(InvalidEnumValue, @enum.ToString()));

        }

        /// <summary>
        /// Throws an <see cref="InvalidEnumArgumentException"/> if the enum value is not in the required enum value range. See the Remarks section.
        /// </summary>
        /// <param name="enum">The enum value to check.</param>
        /// <param name="argumentName">The parameter name.</param>
        /// <remarks>This method doesn't read all the enum fields, but only takes care of the first and last numeric enum fields, so if the value is 1, and the enum has only defined fields for 0 and 2, this method still doesn't throw. For a method that actually reads all the enum fields, see the <see cref="ThrowIfNotDefinedEnumValue(Enum)"/> method.</remarks>
        /// <seealso cref="IsValidEnumValue(Enum)"/>
        /// <seealso cref="ThrowIfNotValidEnumValue(Enum)"/>
        public static void ThrowIfNotValidEnumValue(this Enum @enum, string argumentName)

        {

            if (!@enum.IsValidEnumValue()) throw new InvalidEnumArgumentException(argumentName, (int)Convert.ChangeType(@enum, TypeCode.Int32), @enum.GetType());
            // .GetType().IsEnumDefined(@enum)

        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the enum value is not in the required enum value range.
        /// </summary>
        /// <param name="enum">The enum value to check.</param>
        /// <seealso cref="Type.IsEnumDefined(object)"/>
        /// <seealso cref="ThrowIfNotDefinedEnumValue(Enum, string)"/>
        public static void ThrowIfNotDefinedEnumValue(this Enum @enum)

        {

            if (!@enum.GetType().IsEnumDefined(@enum)) throw new InvalidOperationException(string.Format(InvalidEnumValue, @enum.ToString()));

        }

        /// <summary>
        /// Throws an <see cref="InvalidEnumArgumentException"/> if the enum value is not in the required enum value range. See the Remarks section.
        /// </summary>
        /// <param name="enum">The enum value to check.</param>
        /// <param name="argumentName">The parameter name.</param>
        /// <remarks>This method doesn't read all the enum fields, but only takes care of the first and last numeric enum fields, so if the value is 1, and the enum has only defined fields for 0 and 2, this method still doesn't throw. For a method that actually reads all the enum fields, see the <see cref="ThrowIfNotDefinedEnumValue(Enum)"/> method.</remarks>
        /// <seealso cref="IsValidEnumValue(Enum)"/>
        /// <seealso cref="ThrowIfNotDefinedEnumValue(Enum)"/>
        public static void ThrowIfNotDefinedEnumValue(this Enum @enum, string argumentName)

        {

            if (!@enum.GetType().IsEnumDefined(@enum)) throw new InvalidEnumArgumentException(argumentName, @enum);

        }

        /// <summary>
        /// Determines whether the current enum value is within the enum values range.
        /// </summary>
        /// <param name="enum">The enum value to check.</param>
        /// <param name="throwIfNotFlagsEnum">Whether to throw if the given enum does not have the <see cref="FlagsAttribute"/> attribute.</param>
        /// <param name="throwIfZero">Whether to throw if the given enum is zero.</param>
        /// <returns><see langword="true"/> if the given value is in the enum values range, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="enum"/> does not have the <see cref="FlagsAttribute"/> and <paramref name="throwIfNotFlagsEnum"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="enum"/> is equal to zero and the <paramref name="throwIfZero"/> parameter is set to true or <paramref name="enum"/> is lesser than zero.</exception>
        /// <seealso cref="ThrowIfNotValidFlagsEnumValue( Enum, bool, bool)"/>
        /// <seealso cref="ThrowIfNotValidFlagsEnumValue(Enum, string, bool, bool)"/>
        public static bool IsValidFlagsEnumValue(this Enum @enum, bool throwIfNotFlagsEnum, bool throwIfZero)

        {

            Type enumType = @enum.GetType();

            var enumComparer = new EnumComparer();

            int comparisonResult = enumComparer.CompareToObject(@enum, 0);

            object value = @enum.GetNumValue();

            // If the value is lesser than zero, this is not a flags enum value.

            if (comparisonResult < 0 || (comparisonResult == 0 && throwIfZero))

                throw new InvalidEnumArgumentException("The given value must be greater than zero if the 'throwIfZero' parameter is set to true, or greater or equal to zero otherwise.", nameof(@enum), value is long ? (long)value : (int)value, enumType);

            if (enumType.GetCustomAttribute<FlagsAttribute>() == null)

                return throwIfNotFlagsEnum ? throw new ArgumentException("The given enum does not have the FlagsAttribute.", nameof(@enum)) : false;

            // Now, we have to check if the given value is directly defined in the enum.

            if (enumType.IsEnumDefined(@enum))

                return true;

            // If not, we have to check if the given value is a power of 2.

            double valueDouble = (double)Convert.ChangeType(value, TypeCode.Double);

            // If yes and if we reached this point, that means that the value is a power of 2 -- and therefore represents a flag in the enum --, but is not defined in the enum.

            double log = Math.Log(valueDouble, 2);

            if (Math.Truncate(log) == log) return false;

            // If not, we have to check if all the flags represented by the given value are actually set in the enum.

            double _value = Math.Pow(2, Math.Ceiling(log));

            double valueToCheck;

            do

            {

                valueToCheck = _value - valueDouble;

                if (valueToCheck > long.MaxValue && !enumType.IsEnumDefined(Enum.ToObject(enumType, (ulong)valueToCheck))) return false;

                else if (!enumType.IsEnumDefined(Enum.ToObject(enumType, (long)valueToCheck))) return false;

                valueDouble -= valueToCheck;

            }

            while (valueDouble > 0);

            return true;

        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the enum value is not in the required enum value range.
        /// </summary>
        /// <param name="enum">The enum value to check.</param>
        /// <param name="throwIfNotFlagsEnum">Whether to throw if the given enum does not have the <see cref="FlagsAttribute"/> attribute.</param>
        /// <param name="throwIfZero">Whether to throw if the given enum is zero.</param>
        /// <exception cref="ArgumentException"><paramref name="enum"/> does not have the <see cref="FlagsAttribute"/> and <paramref name="throwIfNotFlagsEnum"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="enum"/> is equal to zero and the <paramref name="throwIfZero"/> parameter is set to true or <paramref name="enum"/> is lesser than zero.</exception>
        /// <seealso cref="IsValidEnumValue(Enum)"/>
        /// <seealso cref="ThrowIfNotValidEnumValue(Enum, string)"/>
        public static void ThrowIfNotValidFlagsEnumValue(this Enum @enum, bool throwIfNotFlagsEnum, bool throwIfZero)

        {

            if (!@enum.IsValidFlagsEnumValue(throwIfNotFlagsEnum, throwIfZero))

                throw new InvalidOperationException(string.Format(InvalidEnumValue, @enum.ToString()));

        }

        /// <summary>
        /// Throws an <see cref="InvalidEnumArgumentException"/> if the enum value is not in the required enum value range.
        /// </summary>
        /// <param name="enum">The enum value to check.</param>
        /// <param name="argumentName">The parameter name.</param>
        /// <param name="throwIfNotFlagsEnum">Whether to throw if the given enum does not have the <see cref="FlagsAttribute"/> attribute.</param>
        /// <param name="throwIfZero">Whether to throw if the given enum is zero.</param>
        /// <exception cref="ArgumentException"><paramref name="enum"/> does not have the <see cref="FlagsAttribute"/> and <paramref name="throwIfNotFlagsEnum"/> is set to <see langword="true"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="enum"/> is equal to zero and the <paramref name="throwIfZero"/> parameter is set to true or <paramref name="enum"/> is lesser than zero.</exception>
        /// <seealso cref="IsValidEnumValue(Enum)"/>
        /// <seealso cref="ThrowIfNotValidEnumValue(Enum)"/>
        public static void ThrowIfNotValidFlagsEnumValue(this Enum @enum, string argumentName, bool throwIfNotFlagsEnum, bool throwIfZero)

        {

            if (!@enum.IsValidFlagsEnumValue(throwIfNotFlagsEnum, throwIfZero))

                throw new InvalidEnumArgumentException(argumentName, (int)Convert.ChangeType(@enum, TypeCode.Int32), @enum.GetType());

        }

        //public static ImageSource ToImageSource(this Icon icon)

        //{

        //    IntPtr hIcon = icon.Handle;

        //    BitmapSource wpfIcon = Imaging.CreateBitmapSourceFromHIcon(
        //        hIcon,
        //        Int32Rect.Empty,
        //        BitmapSizeOptions.FromEmptyOptions());

        //    //if (!Util.DeleteObject(hIcon))

        //    //    throw new Win32Exception();

        //    //using (MemoryStream memoryStream = new MemoryStream())

        //    //{

        //    //    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);

        //    //    IconBitmapDecoder iconBitmapDecoder = new IconBitmapDecoder(memoryStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);

        //    //    return (ImageSource) new ImageSourceConverter().ConvertFrom( iconBitmapDecoder);

        //    //}

        //    ImageSource imageSource;

        //    // Icon icon = Icon.ExtractAssociatedIcon(path);

        //    using (Bitmap bmp = icon.ToBitmap())
        //    {
        //        var stream = new MemoryStream();
        //        bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        //        imageSource = BitmapFrame.Create(stream);
        //    }

        //    return imageSource;

        //    return icon.ToBitmap().ToImageSource();

        //    return wpfIcon;

        //}

        /// <summary>
        /// Converts a <see cref="Bitmap"/> to an <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="bitmap">The <see cref="Bitmap"/> to convert.</param>
        /// <returns>The <see cref="ImageSource"/> obtained from the given <see cref="Bitmap"/>.</returns>
        public static ImageSource ToImageSource(this Bitmap bitmap)

        {

            bitmap.MakeTransparent();

            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!Microsoft.WindowsAPICodePack.Win32Native.Shell.ShellNativeMethods.DeleteObject(hBitmap))

                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    bitmap.Save(stream, ImageFormat.Png); // Was .Bmp, but this did not show a transparent background.

            //    stream.Position = 0;
            //    BitmapImage result = new BitmapImage();
            //    result.BeginInit();
            //    // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
            //    // Force the bitmap to load right now so we can dispose the stream.
            //    result.CacheOption = BitmapCacheOption.OnLoad;
            //    result.StreamSource = stream;
            //    result.EndInit();
            //    result.Freeze();
            //    return result;
            //}

            return wpfBitmap;

        }

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="b">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="b"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this sbyte b, sbyte x, sbyte y) => b >= x && b <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="b">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="b"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this byte b, byte x, byte y) => b >= x && b <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="s">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this short s, short x, short y) => s >= x && s <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="s">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this ushort s, ushort x, ushort y) => s >= x && s <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="i">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="i"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this int i, int x, int y) => i >= x && i <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="i">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="i"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this uint i, uint x, uint y) => i >= x && i <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="l">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="l"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this long l, long x, long y) => l >= x && l <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="l">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="l"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this ulong l, ulong x, ulong y) => l >= x && l <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="f">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="f"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this float f, float x, float y) => f >= x && f <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="d">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="d"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this double d, double x, double y) => d >= x && d <= y;

        /// <summary>
        /// Checks if a number is between two given numbers.
        /// </summary>
        /// <param name="d">The number to check.</param>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if <paramref name="d"/> is between <paramref name="x"/> and <paramref name="y"/>, otherwise <see langword="false"/>.</returns>
        public static bool Between(this decimal d, decimal x, decimal y) => d >= x && d <= y;

        public static void Execute(this ICommand command, object commandParameter, IInputElement commandTarget)

        {

            if (command is RoutedCommand)

                ((RoutedCommand)command).Execute(commandParameter, commandTarget);

            else

                command.Execute(commandParameter);

        }

        public static bool TryExecute(this ICommand command, object commandParameter, IInputElement commandTarget) => command is RoutedCommand
                ? ((RoutedCommand)command).TryExecute(commandParameter, commandTarget)
                : command.TryExecute(commandParameter);

        /// <summary>
        /// Check if the command can be executed, and executes it if available. See the remarks section.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandParameter">The parameter of your command.</param>
        /// <remarks>
        /// This method only evaluates the commands of the common <see cref="ICommand"/> type. To evaluate a command of the <see cref="RoutedCommand"/> type, consider using the <see cref="TryExecute(RoutedCommand, object, IInputElement)"/> method. If you are not sure of the type of your command, so consider using the <see cref="TryExecute(ICommand, object, IInputElement)"/> method.
        /// </remarks>
        public static bool TryExecute(this ICommand command, object commandParameter)

        {

            if (command != null && command.CanExecute(commandParameter))

            {

                command.Execute(commandParameter);

                return true;

            }

            return false;

        }

        public static bool TryExecute(this RoutedCommand command, object commandParameter, IInputElement commandTarget)

        {

            if (command.CanExecute(commandParameter, commandTarget))

            {

                // try
                // {

                command.Execute(commandParameter, commandTarget);

                // }
                // catch (InvalidOperationException ex)
                // {

                // Debug.WriteLine(ex.Message);

                // }

                return true;

            }

            return false;

        }

        public static bool CanExecute(this ICommand command, object commandParameter, IInputElement commandTarget) => command is RoutedCommand routedCommand
                ? routedCommand.CanExecute(commandParameter, commandTarget)
                : command.CanExecute(commandParameter);

        ///// <summary>
        ///// Checks if an object is a <see cref="FrameworkElement.Parent"/> or a <see cref="FrameworkElement.TemplatedParent"/> of an another object.
        ///// </summary>
        ///// <param name="source">The source object</param>
        ///// <param name="obj">The object to search in</param>
        ///// <returns><see langword="true"/> if 'obj' is a parent of the current object, otherwise <see langword="false"/>.</returns>
        //public static bool IsParent(this DependencyObject source, FrameworkElement obj)

        //{

        //    DependencyObject parent = obj.Parent ?? obj.TemplatedParent;

        //    while (parent != null && parent is FrameworkElement)

        //    {

        //        if (parent == source)

        //            return true;

        //        parent = ((FrameworkElement)parent).Parent ?? ((FrameworkElement)parent).TemplatedParent;

        //    }

        //    return false;

        //}

        /// <summary>
        /// Searches for the first parent of an object which is assignable from a given type.
        /// </summary>
        /// <typeparam name="T">The type to search</typeparam>
        /// <param name="source">The source object</param>
        /// <param name="typeEquality">Indicates whether to check for the exact type equality. <see langword="true"/> to only search for objects with same type than the given type, <see langword="false"/> to search for all objects of type for which the given type is assignable from.</param>
        /// <returns>The first object that was found, if any, otherwise null.</returns>
        public static T GetParent<T>(this DependencyObject source, bool typeEquality) where T : DependencyObject

        {

            Type type = typeof(T);

            //if (!typeof(DependencyObject).IsAssignableFrom(type))

            //    throw new InvalidOperationException($"The DependencyObject type must be assignable from the type parameter.");

            do

                source = (source is FrameworkElement frameworkElement ? frameworkElement.Parent ?? frameworkElement.TemplatedParent : null) ?? VisualTreeHelper.GetParent(source);

            while (source != null && (typeEquality ? source.GetType() != type : !type.IsAssignableFrom(source.GetType())));

            return (T)source;

        }

        #region String extension methods

        // todo: add other methods and overloads for StringComparison, IEqualityComparer<char>, Comparer<char>, Comparison<char>, ignore case and CultureInfo parameters

        [Obsolete("This method has been replaced by the Contains(this string, string, IEqualityComparer<char>) method.")]
        public static bool Contains(this string s, IEqualityComparer<char> comparer, string value) => s.Contains(value, comparer);

        public static bool Contains(this string s, string value, IEqualityComparer<char> comparer)

        {

            bool contains(ref int i)

            {

                for (int j = 0; j < value.Length; j++)

                    if (!comparer.Equals(s[i + j], value[j]))

                        return false;

                return true;

            }

            for (int i = 0; i < s.Length; i++)

            {

                if (value.Length > s.Length - i)

                    return false;

                if (contains(ref i))

                    return true;

            }

            return false;

        }

        // todo: To replace by arrays-common methods

        public static bool Contains(this string s, char value, out int index)

        {

            for (int i = 0; i < s.Length; i++)

                if (s[i] == value)

                {

                    index = i;

                    return true;

                }

            index = default;

            return false;

        }

        public static bool Contains(this string s, string value, out int index)

        {

            bool contains(ref int i)

            {

                for (int j = 0; j < value.Length; j++)

                    if (s[i + j] != value[j])

                        return false;

                return true;

            }

            for (int i = 0; i < s.Length; i++)

            {

                if (value.Length > s.Length - i)

                {

                    index = -1;

                    return false;

                }

                if (contains(ref i))

                {

                    index = i;

                    return true;

                }

            }

            index = -1;

            return false;

        }

        [Obsolete("This method has been replaced by arrays-common methods.")]
        public static bool Contains(this string s, char value, IEqualityComparer<char> comparer, out int index)

        {

            for (int i = 0; i < s.Length; i++)

                if (comparer.Equals(s[i], value))

                {

                    index = i;

                    return true;

                }

            index = default;

            return false;

        }

        public static bool Contains(this string s, string value, IEqualityComparer<char> comparer, out int index)

        {

            bool contains(ref int i)

            {

                for (int j = 0; j < value.Length; j++)

                    if (!comparer.Equals(s[i + j], value[j]))

                        return false;

                return true;

            }

            for (int i = 0; i < s.Length; i++)

            {

                if (value.Length > s.Length - i)

                {

                    index = -1;

                    return false;

                }

                if (contains(ref i))

                {

                    index = i;

                    return true;

                }

            }

            index = -1;

            return false;

        }

        public static bool StartsWith(this string s, char value) => s[0] == value;

        public static string RemoveAccents(this string s)

        {

            var stringBuilder = new StringBuilder();

            s = s.Normalize(System.Text.NormalizationForm.FormD);

            foreach (char c in s)

                if (char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)

                    _ = stringBuilder.Append(c);

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);

        }

        #endregion

        public static void ThrowIfDisposingOrDisposed(this IDisposable obj)

        {

            if (obj.IsDisposing)

                throw new InvalidOperationException("The current object or value is disposing.");

            if (obj.IsDisposed)

                throw new InvalidOperationException("The current object or value is disposed.");

        }

    }
}
