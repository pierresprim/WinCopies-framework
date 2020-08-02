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
using WinCopies.Collections.DotNetFix.Generic;
using static WinCopies.Util.Util;

namespace WinCopies.Util
{
    /// <summary>
    /// This enum is designed as an extension of the <see cref="bool"/> type.
    /// </summary>
    public enum Result : sbyte
    {
        /// <summary>
        /// An error as occurred.
        /// </summary>
        Error = -3,

        /// <summary>
        /// The operation has been canceled.
        /// </summary>
        Canceled = -2,

        /// <summary>
        /// The operation did not return any particular value. This value is the same as returning a <see langword="null"/> <see cref="bool?"/>.
        /// </summary>
        None = -1,

        /// <summary>
        /// The operation returned False. This value is the same number as <see langword="false"/>.
        /// </summary>
        False = 0,

        /// <summary>
        /// The operation returned True. This value is the same number as <see langword="true"/>.
        /// </summary>
        True = 1
    }

    public static class _Util
    {
        public static bool IsEnumeratorNotStartedOrDisposed(in WinCopies.Collections.IDisposableEnumeratorInfo enumerator) => (enumerator ?? throw GetArgumentNullException(nameof(enumerator))).IsDisposed || !enumerator.IsStarted;
    }

    public static class _Extensions
    {
        public static Result ToResultEnum(this bool? value) => value.HasValue ? value.Value.ToResultEnum() : Result.None;

        public static Result ToResultEnum(this bool value) => value ? Result.True : Result.False;
    }

    public static class _ThrowHelper
    {
        public static InvalidOperationException GetEnumeratorNotStartedOrDisposedException() => new InvalidOperationException("The enumeration is not started or the enumerator is disposed.");

        public static void ThrowIfEnumeratorNotStartedOrDisposedException(in WinCopies.Collections.IDisposableEnumeratorInfo enumerator)
        {
            if (_Util.IsEnumeratorNotStartedOrDisposed(enumerator))

                throw GetEnumeratorNotStartedOrDisposedException();
        }
    }
}

namespace WinCopies.Collections
{
    public interface ICountable
    {
        int Count { get; }
    }

    public interface IEnumeratorInfo : IEnumerator
    {
        bool? IsResetSupported { get; }

        bool IsStarted { get; }

        bool IsCompleted { get; }
    }

    public interface IDisposableEnumeratorInfo : IEnumeratorInfo, WinCopies.Util.DotNetFix.IDisposable
    {
        // Left empty.
    }

    namespace DotNetFix.Generic
    {
        public interface ICountableEnumerator<out T> : IEnumerator<T>, ICountable
        {
            // Left empty.
        }

        public interface ICountableDisposableEnumerator<out T> : ICountableEnumerator<T>, WinCopies.Util.DotNetFix.IDisposable
        {
            // Left empty.
        }

        public class ArrayEnumerator<T> : ICountableDisposableEnumerator<T>
        {
            private T[] _array;
            private T _current;
            private int _currentIndex = 0;

            protected T[] Array => IsDisposed ? throw GetExceptionForDispose(false) : _array;

            public int Count => IsDisposed ? throw GetExceptionForDispose(false) : _array.Length;

            public bool IsStarted { get; private set; }

            public bool IsCompleted { get; private set; }

            public T Current => IsStarted && !IsDisposed ? _current : throw new InvalidOperationException("The enumeration is not started or the enumerator is disposed.");

            object System.Collections.IEnumerator.Current => Current;

            protected int CurrentIndex => IsDisposed ? throw GetExceptionForDispose(false) : _currentIndex;

            public bool IsDisposed { get; private set; }

            public ArrayEnumerator(T[] array) => _array = array ?? throw GetArgumentNullException(nameof(array));

            public bool MoveNext()
            {
                if (IsDisposed)

                    throw GetExceptionForDispose(false);

                if (IsCompleted)

                    return false;

                if (_currentIndex < _array.Length)
                {
                    IsStarted = true;

                    _current = _array[_currentIndex++];

                    return true;
                }

                _Reset();

                IsCompleted = true;

                return false;
            }

            private void _Reset()
            {
                _currentIndex = 0;

                _current = default;

                IsStarted = false;
            }

            public void Reset()
            {
                if (IsDisposed)

                    throw GetExceptionForDispose(false);

                _Reset();

                IsCompleted = false;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (IsDisposed)

                    return;

                if (disposing)
                {
                    _array = null;

                    Reset();

                    IsDisposed = true;
                }
            }

            public void Dispose() => Dispose(true);
        }
    }

    namespace Generic
    {
        public interface ITreeEnumerableProviderEnumerable<out T> : IEnumerable<T>
        {
            IEnumerator<ITreeEnumerable<T>> GetRecursiveEnumerator();
        }

        public interface ITreeEnumerable<out T> : ITreeEnumerableProviderEnumerable<T>
        {
            T Value { get; }
        }

        public class TreeEnumerator<T> : Enumerator<ITreeEnumerable<T>, T>
        {
            private Stack<IEnumerator<ITreeEnumerable<T>>> _stack = new Stack<IEnumerator<ITreeEnumerable<T>>>();

            private bool _completed = false;

            public TreeEnumerator(IEnumerable<ITreeEnumerable<T>> enumerable) : base(enumerable ?? throw GetArgumentNullException(nameof(enumerable)))
            {
                // Left empty.
            }

            public TreeEnumerator(ITreeEnumerableProviderEnumerable<T> enumerable) : base(new Enumerable<ITreeEnumerable<T>>(() => (enumerable ?? throw GetArgumentNullException(nameof(enumerable))).GetRecursiveEnumerator()))
            {
                // Left empty.
            }

            protected override bool MoveNextOverride()
            {
                if (_completed) return false;

                IEnumerator<ITreeEnumerable<T>> enumerator;

                void push(in ITreeEnumerable<T> enumerable)
                {
                    enumerator = enumerable.GetRecursiveEnumerator();

                    Current = enumerable.Value;

                    _stack.Push(enumerator);
                }

                while (true)
                {
                    if (_stack.Count == 0)
                    {
                        if (InnerEnumerator.MoveNext())
                        {
                            push(InnerEnumerator.Current);

                            return true;
                        }

                        _completed = true;

                        return false;
                    }

                    enumerator = _stack.Peek();

                    if (enumerator.MoveNext())
                    {
                        push(enumerator.Current);

                        return true;
                    }

                    else

                        _ = _stack.Pop();
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                    // {
                    _stack = null;

                // _enumerateFunc = null;
                // }
            }
        }

        public interface IEnumeratorInfo<out T> : System.Collections.Generic.IEnumerator<T>, IEnumeratorInfo
        {
            // Left empty.
        }

        public interface IDisposableEnumeratorInfo<out T> : IEnumeratorInfo<T>, IDisposableEnumeratorInfo
        {
            // Left empty.
        }

        public interface ICountableEnumeratorInfo<out T> : IEnumeratorInfo<T>, ICountableEnumerator<T>
        {
            // Left empty.
        }

        public interface ICountableDisposableEnumeratorInfo<out T> : ICountableDisposableEnumerator<T>, IDisposableEnumeratorInfo<T>, ICountableEnumeratorInfo<T>
        {
            // Left empty.
        }
    }
}

#endif
