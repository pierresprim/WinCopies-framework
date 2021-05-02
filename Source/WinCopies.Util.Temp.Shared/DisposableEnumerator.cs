using WinCopies.Collections.DotNetFix.Generic;

namespace WinCopies.Temp.Collections.DotNetFix.Generic
{
    public sealed class DisposableEnumerator<T> : IDisposableEnumerator<T>
    {
        private System.Collections.Generic.IEnumerator<T> _enumerator;

        private TValue GetOrThrowIfDisposed<TValue>(in TValue value) => ThrowHelper.GetOrThrowIfDisposed(this, value);

        public T Current => GetOrThrowIfDisposed(_enumerator).Current;

        object System.Collections.IEnumerator.Current => Current;

        public bool IsDisposed => _enumerator == null;

        public DisposableEnumerator(in System.Collections.Generic.IEnumerator<T> enumerator) => _enumerator = enumerator;

        public DisposableEnumerator(in System.Collections.Generic.IEnumerable<T> enumerable) : this(enumerable.GetEnumerator()) { /* Left empty. */ }

        public bool MoveNext() => GetOrThrowIfDisposed(_enumerator).MoveNext();

        public void Reset() => GetOrThrowIfDisposed(_enumerator).Reset();

        public void Dispose()               => _enumerator = null;
    }
}
