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
using WinCopies.Util.DotNetFix;
using static WinCopies.Util.Util;

namespace WinCopies.Util
{

    public static class _Util
    {
        public static bool IsEnumeratorNotStartedOrDisposed(in WinCopies.Collections.IDisposableEnumeratorInfo enumerator) => (enumerator ?? throw GetArgumentNullException(nameof(enumerator))).IsDisposed || !enumerator.IsStarted;
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

            // private bool _firstLaunch = true;
            private bool _completed = false;

            // private Func<string, IEnumerable<string>> _enumerateFunc;

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

                //                void _markAsCompleted()
                //                {
                //                    _stack = null;

                //                    _completed = true;
                //                }

                //                bool dequeueDirectory()
                //                {
                IEnumerator<ITreeEnumerable<T>> enumerator;

                void push(in ITreeEnumerable<T> enumerable)
                {
                    enumerator = enumerable.GetRecursiveEnumerator();

                    Current = enumerable.Value;

                    _stack.Push(enumerator);
                }

                //new FileSystemEntryEnumerator(
                //#if DEBUG
                //                    Current,
                //#endif
                //_enumerateDirectoriesFunc(Current.Path), _enumerateFilesFunc(Current.Path)));

                while (true)
                {
                    if (_stack.Count == 0)
                    {
                        //                            if (_directories == null)
                        //                            {
                        //                                _directories = null;

                        //                                _markAsCompleted();

                        //                                return false;
                        //                            }

                        if (InnerEnumerator.MoveNext())
                        {
                            push(InnerEnumerator.Current);

                            return true;
                        }

                        _completed = true;

                        return false;

                        //                            if (_directories.Count == 0)

                        //                                _directories = null;
                    }

                    enumerator = _stack.Peek();

                    //                        //#if DEBUG
                    //                        //                        SimulationParameters?.WriteLogAction($"Peeked enumerator: {enumerator.PathInfo.Path}");
                    //                        //#endif

                    if (enumerator.MoveNext())
                    {
                        //Current = enumerator.Current;

                        //                            //#if DEBUG
                        //                            //                            SimulationParameters?.WriteLogAction($"Peeked enumerator: {enumerator.PathInfo.Path}; Peeked enumerator current: {enumerator.Current.Path}");
                        //                            //#endif

                        //if (enumerator.Current.IsDirectory)

                        push(enumerator.Current);

                        return true;
                    }

                    else

                        _ = _stack.Pop();

                    //                        //#if DEBUG
                    //                        //                        SimulationParameters?.WriteLogAction($"Peeked enumerator: {enumerator.PathInfo.Path}; Peeked enumerator move next failed.");
                    //                        //#endif

                    //                        _ = _stack.Pop();
                }

                //}

                //if (_firstLaunch)
                //{
                //_firstLaunch = false;

                //IPathInfo path;

                //while (InnerEnumerator.MoveNext())
                //{
                //    path = InnerEnumerator.Current;

                //    (path.IsDirectory ? _directories : _files).Enqueue(path);
                //}

                //if (_files.Count == 0)
                //{
                //    _files = null;

                //    if (_directories.Count == 0)
                //    {
                //        _directories = null;

                //        _markAsCompleted();

                //        return false;
                //    }

                //    _ = dequeueDirectory();

                //    return true;
                //}

                //if (_directories.Count == 0)

                //    _directories = null;

                //Current = _files.Dequeue();

                //if (_files.Count == 0)

                //    _files = null;

                //return true;
                //}

                //if (_files == null)
                //{
                //    if (_directories == null && _stack.Count == 0)
                //    {
                //        _markAsCompleted();

                //        return false;
                //    }

                //    if (dequeueDirectory())

                //        return true;

                //    _markAsCompleted();

                //    return false;
                //}

                //Current = _files.Dequeue();

                //if (_files.Count == 0)

                //    _files = null;

                //return true;
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

//    public static class Temp
//    {
//        // public static IEnumerable<T> GetEmptyEnumerable<T>() => new Enumerable<T>(() => new EmptyEnumerator<T>());

//        // todo: replace with WinCopies.Util's implementation.

//        //private static void Split(string s, in bool skipEmptyValues, in StringBuilder stringBuilder, in Action<string> action, params char[] separators)
//        //{
//        //    ThrowIfNull(s, nameof(s));
//        //    ThrowIfNull(stringBuilder, nameof(stringBuilder));
//        //    ThrowIfNull(separators, nameof(separators));

//        //    if (separators.Length == 0)

//        //        throw new ArgumentException($"{nameof(separators)} does not contain values.");

//        //    Debug.Assert(action != null, $"{nameof(action)} must be not null.");

//        //    Predicate<char> getPredicate()
//        //    {
//        //        Predicate<char> predicate;

//        //        if (separators.Length == 1)

//        //            predicate = __c => __c == separators[0];

//        //        else

//        //            predicate = __c => separators.Contains(__c);

//        //        return predicate;
//        //    }

//        //    if (skipEmptyValues)

//        //        if (s.Length == 0)

//        //            return;

//        //        else if (s.Length == 1)

//        //            if ((separators.Length == 1 && s[0] == separators[0]) || separators.Contains(s[0]))

//        //                return;

//        //            else

//        //                action(s);

//        //        else
//        //        {
//        //            Predicate<char> predicate = getPredicate();

//        //            foreach (char _c in s)

//        //                if (predicate(_c) && stringBuilder.Length > 0)
//        //                {
//        //                    action(stringBuilder.ToString());

//        //                    _ = stringBuilder.Clear();
//        //                }

//        //                else

//        //                    _ = stringBuilder.Append(_c);

//        //            if (stringBuilder.Length > 0)

//        //                action(stringBuilder.ToString());
//        //        }

//        //    else if (s.Length == 0)

//        //        action("");

//        //    else if (s.Length == 1)

//        //        if ((separators.Length == 1 && s[0] == separators[0]) || separators.Contains(s[0]))
//        //        {
//        //            action("");

//        //            action("");
//        //        }

//        //        else

//        //            action(s);

//        //    else
//        //    {
//        //        Predicate<char> predicate = getPredicate();

//        //        foreach (char _c in s)

//        //            if (predicate(_c))

//        //                if (stringBuilder.Length == 0)

//        //                    action("");

//        //                else
//        //                {
//        //                    action(stringBuilder.ToString());

//        //                    _ = stringBuilder.Clear();
//        //                }

//        //            else

//        //                _ = stringBuilder.Append(_c);

//        //        if (stringBuilder.Length > 0)

//        //            action(stringBuilder.ToString());
//        //    }
//        ////}

//        //public static Queue<string> SplitToQueue(string s, in bool skipEmptyValues, params char[] separators)
//        //{
//        //    var queue = new Queue<string>();

//        //    SplitToQueue(s, skipEmptyValues, new StringBuilder(), queue, separators);

//        //    return queue;
//        //}

//        //public static void SplitToQueue(string s, in bool skipEmptyValues, in StringBuilder stringBuilder, Queue<string> queue, params char[] separators)
//        //{
//        //    ThrowIfNull(queue, nameof(queue));

//        //    Split(s, skipEmptyValues, stringBuilder, _s => queue.Enqueue(_s), separators);
//        //}
//    }

//    //public sealed class EmptyCheckEnumerator<T> : IEnumerator<T>, WinCopies.Util.DotNetFix.IDisposable
//    //{
//    //    #region Fields
//    //    private IEnumerator<T> _enumerator;
//    //    private Func<bool> _moveNext;
//    //    private bool? _hasItems = null;
//    //    private Func<T> _current;
//    //    #endregion

//    //    #region Properties
//    //    public bool IsDisposed { get; private set; }

//    //    public bool HasItems
//    //    {
//    //        get
//    //        {
//    //            ThrowIfDisposed();

//    //            if (!_hasItems.HasValue)

//    //                _hasItems = _enumerator.MoveNext();

//    //            return _hasItems.Value;
//    //        }
//    //    }

//    //    public T Current
//    //    {
//    //        get
//    //        {
//    //            ThrowIfDisposed();

//    //            return _current();
//    //        }
//    //    }
//    //    #endregion

//    //    public EmptyCheckEnumerator(IEnumerator<T> enumerator)
//    //    {
//    //        _enumerator = enumerator;

//    //        ResetMoveNext();
//    //    }

//    //    #region Methods
//    //    private void ResetCurrent() => _current = () => throw new InvalidOperationException("The enumeration has not been started or has completed.");

//    //    private void ResetMoveNext()
//    //    {
//    //        ResetCurrent();

//    //        void resetMoveNext()
//    //        {
//    //            _moveNext = () => false;

//    //            ResetCurrent();
//    //        }

//    //        bool enumerate()
//    //        {
//    //            if (_enumerator.MoveNext())

//    //                return true;

//    //            resetMoveNext();

//    //            return false;
//    //        }

//    //        _moveNext = () =>
//    //        {
//    //            if (_hasItems.HasValue)
//    //            {
//    //                if (_hasItems.Value)
//    //                {
//    //                    _current = () => _enumerator.Current;

//    //                    _moveNext = enumerate;

//    //                    return true;
//    //                }

//    //                else
//    //                {
//    //                    resetMoveNext();

//    //                    return false;
//    //                }
//    //            }

//    //            else
//    //            {
//    //                _moveNext = enumerate;

//    //                return enumerate();
//    //            }
//    //        };
//    //    }

//    //    private void ThrowIfDisposed()
//    //    {
//    //        if (IsDisposed)

//    //            throw GetExceptionForDispose(false);
//    //    }
//    //    #endregion

//    //    #region Interface implementations
//    //    #region IEnumerator implementation
//    //    object IEnumerator.Current => Current;

//    //    public bool MoveNext()
//    //    {
//    //        ThrowIfDisposed();

//    //        return _moveNext();
//    //    }

//    //    public void Reset()
//    //    {
//    //        ThrowIfDisposed();

//    //        _enumerator.Reset();

//    //        _hasItems = null;

//    //        ResetMoveNext();
//    //    }
//    //    #endregion

//    //    #region IDisposable implementation
//    //    public void Dispose()
//    //    {
//    //        if (IsDisposed) return;

//    //        _enumerator.Dispose();

//    //        ResetMoveNext();
//    //    }
//    //    #endregion
//    //    #endregion
//    //}

//    //public sealed class EmptyEnumerator<T> : IEnumerator<T>
//    //{
//    //    private readonly T _current = default;

//    //    T IEnumerator<T>.Current => _current;

//    //    object IEnumerator.Current => _current;

//    //    bool IEnumerator.MoveNext() => false;

//    //    void IEnumerator.Reset() { }

//    //    void System.IDisposable.Dispose() { }
//    //}

//    //public sealed class Enumerable<T> : IEnumerable<T>
//    //{
//    //    private readonly Func<IEnumerator<T>> _enumeratorFunc;

//    //    public Enumerable(Func<IEnumerator<T>> enumeratorFunc) => _enumeratorFunc = enumeratorFunc;

//    //    public IEnumerator<T> GetEnumerator() => _enumeratorFunc();

//    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//    ////}

//    //public abstract class Enumerator<TSource, TDestination> : IEnumerator<TDestination>, WinCopies.Util.DotNetFix.IDisposable
//    //{
//    //    public bool IsDisposed { get; private set; }

//    //    private IEnumerator<TSource> _innerEnumerator;

//    //    protected IEnumerator<TSource> InnerEnumerator => IsDisposed ? throw GetExceptionForDispose(false) : _innerEnumerator;

//    //    private TDestination _current;

//    //    public TDestination Current { get => IsDisposed ? throw GetExceptionForDispose(false) : _current; protected set => _current = IsDisposed ? throw GetExceptionForDispose(false) : value; }

//    //    object IEnumerator.Current => Current;

//    //    public Enumerator(IEnumerable<TSource> enumerable) => _innerEnumerator = (enumerable ?? throw GetArgumentNullException(nameof(enumerable))).GetEnumerator();

//    //    public bool MoveNext()
//    //    {
//    //        if (IsDisposed ? throw GetExceptionForDispose(false) : MoveNextOverride()) return true;

//    //        _current = default;

//    //        return false;
//    //    }

//    //    protected abstract bool MoveNextOverride();

//    //    public void Reset()
//    //    {
//    //        if (IsDisposed)

//    //            throw GetExceptionForDispose(false);

//    //        ResetOverride();
//    //    }

//    //    protected virtual void ResetOverride()
//    //    {
//    //        _current = default;

//    //        InnerEnumerator.Reset();
//    //    }

//    //    protected virtual void Dispose(bool disposing)
//    //    {
//    //        if (disposing)

//    //            _innerEnumerator = null;

//    //        IsDisposed = true;
//    //    }

//    //    public void Dispose()
//    //    {
//    //        if (!IsDisposed)
//    //        {
//    //            Dispose(disposing: true);

//    //            GC.SuppressFinalize(this);
//    //        }
//    //    }
//    //}

//    //// todo: moved to WinCopies.Util

//    //public class PausableBackgroundWorker : System.ComponentModel.BackgroundWorker
//    //{
//    //    public bool PausePending { get; private set; }

//    //    private bool _workerSupportsPausing = false;

//    //    public bool WorkerSupportsPausing { get => _workerSupportsPausing; set => _workerSupportsPausing = IsBusy ? throw new InvalidOperationException("The BackgroundWorker is running.") : value; }

//    //    public void PauseAsync()
//    //    {
//    //        if (!_workerSupportsPausing)

//    //            throw new InvalidOperationException("The BackgroundWorker does not support pausing.");

//    //        if (IsBusy)

//    //            PausePending = true;
//    //    }

//    //    protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
//    //    {
//    //        base.OnRunWorkerCompleted(e);

//    //        PausePending = false;
//    //    }
//    //}

////    public static class Extensions// todo: replace by WinCopies.Util's implementation.
////    {
////        public static string Join(this IEnumerable<string> enumerable, in bool keepEmptyValues, params char[] join) => Join(enumerable, keepEmptyValues, new string(join));

////        public static string Join(this IEnumerable<string> enumerable, in bool keepEmptyValues, in string join, StringBuilder stringBuilder = null)
////        {
////            IEnumerator<string> enumerator = (enumerable ?? throw GetArgumentNullException(nameof(enumerable))).GetEnumerator();

////#if CS7
////            if (stringBuilder == null)

////                stringBuilder = new StringBuilder();
////#else
////            stringBuilder ??= new StringBuilder();
////#endif

////            try
////            {
////                void append() => _ = stringBuilder.Append(enumerator.Current);

////                bool moveNext() => enumerator.MoveNext();

////                if (moveNext())

////                    append();

////                while (moveNext() && (keepEmptyValues || enumerator.Current.Length > 0))
////                {
////                    _ = stringBuilder.Append(join);

////                    append();
////                }
////            }
////            finally
////            {
////                enumerator.Dispose();
////            }

////            return stringBuilder.ToString();
////        }
////    }

//    //public class ReadOnlyObservableQueueCollection<T, U> : INotifyPropertyChanged where T : ObservableQueueCollection<U> // todo: remove when CopyProcessQueueCollection has been updated.
//    //{
//    //    private readonly T _innerCollection;

//    //    public event PropertyChangedEventHandler PropertyChanged;

//    //    public ReadOnlyObservableQueueCollection(T innerCollection)
//    //    {
//    //        _innerCollection = innerCollection ?? throw GetArgumentNullException(nameof(innerCollection));

//    //        innerCollection.PropertyChanged += PropertyChanged;
//    //    }

//    //    public U Peek() => _innerCollection.Peek();
//    //}

//    //public class ReadOnlyCopyProcessQueueCollection // todo: remove when CopyProcessQueueCollection has been updated.
//    //{
//    //    private readonly ProcessQueueCollection _innerCollection;

//    //    public Size Size => _innerCollection.Size;

//    //    public event PropertyChangedEventHandler PropertyChanged;

//    //    public ReadOnlyCopyProcessQueueCollection(ProcessQueueCollection innerCollection)
//    //    {
//    //        _innerCollection = innerCollection ?? throw GetArgumentNullException(nameof(innerCollection));

//    //        innerCollection.PropertyChanged += PropertyChanged;
//    //    }

//    //    public IPathInfo Peek() => _innerCollection.Peek();
//    //}
//}

#endif
