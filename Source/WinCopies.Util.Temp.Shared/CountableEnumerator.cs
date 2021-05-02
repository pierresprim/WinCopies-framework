using System;

using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;

using static WinCopies.ThrowHelper;

namespace WinCopies.Temp.Collections.Enumeration
{
    public abstract class CountableEnumerator<TEnumerator, TCount> : System.Collections.IEnumerator where TEnumerator : System.Collections.IEnumerator
    {
        private readonly Func<TCount> _func;

        protected TEnumerator Enumerator { get; }

        object System.Collections.IEnumerator.Current => Enumerator.Current;

        public TCount Count => _func();

        protected CountableEnumerator(in TEnumerator enumerator, in Func<TCount> func)
        {
            Enumerator = enumerator == null ? throw GetArgumentNullException(nameof(enumerator)) : enumerator;

            _func = func ?? throw GetArgumentNullException(nameof(func));
        }

        protected virtual void OnEnumerationStarting() { /* Left empty. */ }

        protected virtual void OnEnumerationCompleted() { /* Left empty. */ }

        public bool MoveNext()
        {
            OnEnumerationStarting();

            bool result = Enumerator.MoveNext();

            OnEnumerationCompleted();

            return result;
        }

        public void Reset() => Enumerator.Reset();
    }

    public class CountableEnumerator<TEnumerator> : CountableEnumerator<TEnumerator, int>, ICountableEnumerator where TEnumerator : System.Collections.IEnumerator
    {
        public CountableEnumerator(in TEnumerator enumerator, in Func<int> func) : base(enumerator, func) { /* Left empty. */ }
    }

    namespace Generic
    {
        public abstract class CountableEnumerator<TEnumerator, TItems, TCount> : WinCopies.Collections.Enumeration.CountableEnumerator<TEnumerator, TCount>, System.Collections.Generic.IEnumerator<TItems>, WinCopies.DotNetFix.IDisposable where TEnumerator : System.Collections.Generic.IEnumerator<TItems>, WinCopies.DotNetFix.IDisposable
        {
            TItems System.Collections.Generic.IEnumerator<TItems>.Current => Enumerator.Current;

            bool WinCopies.DotNetFix.IDisposable.IsDisposed => Enumerator.IsDisposed;

            protected CountableEnumerator(in TEnumerator enumerator, in Func<TCount> func) : base(enumerator, func) { /* Left empty. */ }

            protected virtual void Dispose(in bool disposing)
            {
                if (!Enumerator.IsDisposed)

                    Enumerator.Dispose();
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }
        }

        public class CountableEnumerator<TEnumerator, TItems> : CountableEnumerator<TEnumerator, TItems, int>, ICountableDisposableEnumerator<TItems> where TEnumerator : System.Collections.Generic.IEnumerator<TItems>, WinCopies.DotNetFix.IDisposable
        {
            public CountableEnumerator(in TEnumerator enumerator, in Func<int> func) : base(enumerator, func) { /* Left empty. */ }
        }
    }
}
