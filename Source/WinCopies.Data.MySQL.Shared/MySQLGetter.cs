using MySql.Data.MySqlClient;
using System.Data.Common;
using WinCopies.Data.SQL;

using static WinCopies.ThrowHelper;

namespace WinCopies.Data.MySQL
{
    public class MySQLGetter : ISQLGetter
    {
        private DbDataReader? _reader;

        public bool IsDisposed => _reader == null;

        bool ISQLGetter.IsIntIndexable => true;

        bool ISQLGetter.IsStringIndexable => true;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        // _reader is null only when disposed.

        public object this[int index] => IsDisposed ? throw GetExceptionForDispose(false) : _reader[index];

        public object this[string columnName] => IsDisposed ? throw GetExceptionForDispose(false) : _reader[columnName];
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        public MySQLGetter(DbDataReader reader) => _reader = reader;

        protected virtual void DisposeManaged() { /* Left empty. */ }

        protected virtual void DisposeUnmanaged() => _reader = null;

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)

                return;

            if (disposing)

                DisposeManaged();

            DisposeUnmanaged();
        }

        ~MySQLGetter() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
