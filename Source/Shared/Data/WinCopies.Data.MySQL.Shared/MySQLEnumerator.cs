using MySql.Data.MySqlClient;

using System.Data.Common;
using System.Threading.Tasks;

using WinCopies.Data.SQL;

namespace WinCopies.Data.MySQL
{
    public class MySQLEnumerator : SQLEnumerator<MySQLConnection>
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

        protected internal MySQLEnumerator(in DbCommand command, in DbDataReader reader, in MySQLConnection connection, in bool closeReader) : base(command, reader, connection, closeReader) { /* Left empty. */ }

        public MySQLEnumerator(in DbCommand command, in MySQLConnection connection, in bool closeReader) : base(command, connection, closeReader) { /* Left empty. */ }

        public MySQLEnumerator(in string sql, in MySQLConnection connection, in bool closeReader) : this(new MySqlCommand(sql, connection.InnerConnection), connection, closeReader) { /* Left empty. */ }

        protected override MySQLConnection CloneConnection(MySQLConnection connection) => connection.GetConnection(false);

        protected override DbConnection GetConnection(MySQLConnection connection) => connection.InnerConnection;
    }

    public class MySQLEnumerable : SQLEnumerable<MySQLConnection>
    {
        public MySQLEnumerable(in DbCommand command, in MySQLConnection connection, in bool closeReader) : base(command, connection, closeReader) { /* Left empty. */ }
        public MySQLEnumerable(in SQLRequest<MySQLConnection, MySqlCommand> request, in MySQLConnection connection, in bool closeReader) : this(request.GetCommand(), connection, closeReader) { /* Left empty. */ }

        protected override SQLEnumerator<MySQLConnection> GetSQLEnumerator() => new MySQLEnumerator(Command, Connection, CloseReader);

        protected override async Task<SQLEnumerator<MySQLConnection>> GetSQLEnumeratorAsync() => new MySQLEnumerator(Command, await Command.ExecuteReaderAsync(), Connection, CloseReader);
    }
}
