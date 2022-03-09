using WinCopies.Temp;

using static WinCopies.ThrowHelper;

namespace WinCopies.Data.SQL
{
    public struct SQLAsyncNonQueryRequestResult
    {
        public uint Rows { get; }

        public long? LastInsertedId { get; }

        public SQLAsyncNonQueryRequestResult(in uint rows, in long? lastInsertedId)
        {
            Rows = rows;

            LastInsertedId = lastInsertedId;
        }
    }

    public interface ISQLRequest
    {
        uint ExecuteNonQuery(out long? lastInsertedId);

        Task<SQLAsyncNonQueryRequestResult> ExecuteNonQueryAsync();

        uint ExecuteNonQuery() => ExecuteNonQuery(out _);

        async Task<uint> ExecuteNonQueryAsync2() => (await ExecuteNonQueryAsync()).Rows;
    }

    public interface ISQLColumnRequest
    {
        IEnumerable<SQLColumn>? Columns { get; }
    }

    public interface ISQLColumn
    {
        SQLColumn ToSQLColumn();
    }

    public interface ISQLColumnRequest<T> : ISQLColumnRequest where T : ISQLColumn
    {
        new IExtensibleEnumerable<T>? Columns { get; }

        IEnumerable<SQLColumn>? ISQLColumnRequest.Columns => Columns?.Select(column => column.ToSQLColumn());
    }

    public interface ISQLTableRequest
    {
        IExtensibleEnumerable<string>? Tables { get; }

        IConditionGroup? ConditionGroup { get; set; }
    }

    public interface ISQLTableRequest2 : ISQLTableRequest, ISQLRequest
    {
        // Left empty.
    }

    public abstract class SQLRequest<TConnection, TCommand> where TConnection : IConnection<TCommand>
    {
        protected TConnection Connection { get; }

        public SQLRequest(in TConnection connection) => Connection = connection;

        protected abstract string GetSQL();

        protected virtual TCommand GetCommand() => Connection.GetCommand(GetSQL());

        public sealed override string ToString() => GetSQL();
    }

    public abstract class NonQueryRequest<TConnection, TCommand> : SQLRequest<TConnection, TCommand>, ISQLRequest where TConnection : IConnection<TCommand>
    {
        protected NonQueryRequest(in TConnection connection) : base(connection) { /* Left empty. */ }

        public abstract uint ExecuteNonQuery(out long? lastInsertedId);

        public abstract Task<SQLAsyncNonQueryRequestResult> ExecuteNonQueryAsync();
    }

    public abstract class SQLRequest2<TConnection, TCommand> : NonQueryRequest<TConnection, TCommand>, ISQLRequest3 where TConnection : IConnection<TCommand>
    {
        private string _tableName;

        public string TableName { get => _tableName; set => _tableName = value ?? throw GetArgumentNullException(nameof(value)); }

        protected SQLRequest2(in TConnection connection, in string tableName) : base(connection) => TableName = tableName;

        protected T ExecuteNonQuery<T>(in Func<TCommand, T> action, in Converter<TCommand, long?>? converter, out long? lastInsertedId)
        {
            TCommand? command = GetCommand();

            T? result = action(command);

            lastInsertedId = converter == null ? null : converter(command);

            return result;
        }

        protected T ExecuteNonQuery<T>(in Func<TCommand, T> action, out long? lastInsertedId) => ExecuteNonQuery(action, null, out lastInsertedId);
    }

    public abstract class SQLTableRequest<TConnection, TCommand> : SQLRequest<TConnection, TCommand>, ISQLTableRequest where TConnection : IConnection<TCommand>
    {
        private SQLItemCollection<string> _tables;

        public SQLItemCollection<string> Tables { get => _tables; set => _tables = value ?? throw GetArgumentNullException(nameof(value)); }

        IExtensibleEnumerable<string> ISQLTableRequest.Tables => Tables;

        public IConditionGroup? ConditionGroup { get; set; }

        protected SQLTableRequest(in TConnection connection, in SQLItemCollection<string> tables) : base(connection) => Tables = tables;

        protected abstract Action<TCommand, IConditionGroup?> GetPrepareCommandAction();

        protected override TCommand GetCommand()
        {
            TCommand command = base.GetCommand();

            GetPrepareCommandAction()(command, ConditionGroup);

            return command;
        }
    }
}
