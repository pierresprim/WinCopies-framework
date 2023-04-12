using System;
using System.Data.Common;

using static WinCopies.ThrowHelper;

namespace WinCopies.Data.SQL
{
    public interface ISQLGetter : DotNetFix.IDisposable
    {
        bool IsIntIndexable { get; }

        object this[int index] { get; }

        bool IsStringIndexable { get; }

        object this[string columnName] { get; }

        void Dispose(bool disposing);
    }

    public class SQLGetter : ISQLGetter
    {
        private DbDataReader
#if CS8
            ?
#endif
            _reader;

        internal Action
#if CS8
            ?
#endif
            OnDispose
        { private get; set; }

        public bool IsDisposed => _reader == null;

        bool ISQLGetter.IsIntIndexable => true;

        bool ISQLGetter.IsStringIndexable => true;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        // _reader is null only when disposed.

        public object this[int index] => IsDisposed ? throw GetExceptionForDispose(false) : _reader[index];

        public object this[string columnName] => IsDisposed ? throw GetExceptionForDispose(false) : _reader[columnName];
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        public SQLGetter(DbDataReader reader) => _reader = reader;

        protected virtual void DisposeManaged()
        {
            _reader.Close();
            _reader.Dispose();
        }

        protected virtual void DisposeUnmanaged() => _reader = null;

        public virtual void Dispose(bool disposing)
        {
            if (IsDisposed)

                return;

            if (disposing)

                DisposeManaged();

            DisposeUnmanaged();

            OnDispose?.Invoke();
        }

        ~SQLGetter() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
