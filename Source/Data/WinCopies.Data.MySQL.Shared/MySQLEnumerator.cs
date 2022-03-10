using MySql.Data.MySqlClient;

using System;
using System.Data.Common;
using System.Threading.Tasks;
using WinCopies.Collections.Generic;
using WinCopies.Data.SQL;

using static WinCopies.ThrowHelper;

namespace WinCopies.Data.MySQL
{
    public class MySQLEnumerator : Enumerator<ISQLGetter>
    {
        //private struct Async : IAsyncEnumerableProvider<ISQLGetter>
        //{
        //    private readonly MySqlCommand _command;

        //    public IAsyncResult Result { get; }

        //    public Async(in MySqlCommand command)
        //    {
        //        _command = command ?? throw GetArgumentNullException(nameof(command));

        //        Result = command.ExecuteReaderAsync.BeginExecuteReader();
        //    }

        //    public System.Collections.Generic.IEnumerable<ISQLGetter> GetEnumerable()
        //    {
        //        Async async = this;

        //        return new Enumerable<ISQLGetter>(() => new MySQLEnumerator(async._command.EndExecuteReader(async.Result)));
        //    }
        //}

        private DbDataReader
#if CS8
            ?
#endif
            _reader;
        private MySQLGetter
#if CS8
            ?
#endif
            _current;
        private Action
#if CS8
            ?
#endif
            _moveNextAction;
        private Func<bool>
#if CS8
            ?
#endif
            _moveNext;
        //ulong i = 0;
        MySQLConnection _connection;
        MySqlCommand _command;
        bool _dispose;

        protected override
#if CS9
            MySQLGetter
#else
            ISQLGetter
#endif
            CurrentOverride => _current;

        public override bool? IsResetSupported => false;

        private MySQLEnumerator(in MySqlCommand command, in DbDataReader
#if CS8
            ?
#endif
            reader, MySQLConnection connection /*Func<MySqlConnection> func*/  )
        {
            _connection = connection;

            _command = command;

            _reader = reader;

            _moveNextAction = () => _moveNextAction = () => _current.Dispose();

            bool moveNext()
            {
                //if (_reader.IsClosed)
                //{
                //    //command.Connection = func();

                //    if (_connection.IsDisposed)

                //        initCollection();

                //    _reader = command.ExecuteReader();

                //    for (ulong _i = 0; _i < i; _i++)

                //        _reader.Read();
                //}

                if (_reader.Read())
                {
                    //i++;

                    _moveNextAction();

                    _current = new MySQLGetter(_reader);

                    return true;
                }

                //ResetOverride2();

                return false;
            }

            _moveNext = () =>
            {
                if (_reader == null)

                    return false;

                ResetConnection();

                return (_moveNext = moveNext)();
            };
        }

        public MySQLEnumerator(in MySqlCommand command, in MySQLConnection connection) : this(command ?? throw GetArgumentNullException(nameof(command)), command.ExecuteReader(), connection) { /* Left empty. */ }

        public static async Task<MySQLEnumerator> GetEnumerableAsync(MySqlCommand command, MySQLConnection connection) => new MySQLEnumerator(command
#if CS8
            ??
#else
            == null ?
#endif
            throw GetArgumentNullException(nameof(command))
#if !CS8
            : command
#endif
            , await command.ExecuteReaderAsync(), connection);

        protected void ResetConnection()
        {
            if (_connection.IsDisposed && !_connection.Open())
            {
                _command.Connection = (_connection = (MySQLConnection)_connection.GetConnection(false)).InnerConnection;

                _dispose = true;
            }
        }

        protected override void ResetCurrent()
        {
            base.ResetCurrent();

            if (_current != null)
            {
                _current.Dispose();
                _current = null;
            }
        }

        protected override void ResetOverride2()
        {
            _moveNextAction = null;

            _moveNext = null;

            //i = 0;
        }

        protected override bool MoveNextOverride() => _moveNext();

        protected virtual void DisposeUnmanaged()
        {
            ResetCurrent();

            if (_reader != null)
            {
                if (_dispose)

                    _connection.Dispose();

                _connection = null;

                _command.Dispose();

                _command = null;

                _reader.Close();
                _reader.Dispose();
                _reader = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            DisposeUnmanaged();

            base.Dispose(disposing);
        }
    }
}
