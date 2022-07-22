using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        uint ExecuteNonQuery()
#if CS8
            => ExecuteNonQuery(out _)
#endif
            ;

#if CS8
        async
#endif
        Task<uint> ExecuteNonQueryAsync2()
#if CS8
            => (await ExecuteNonQueryAsync()).Rows
#endif
            ;
    }

    public interface ISQLColumnRequest
    {
        IEnumerable<SQLColumn>
#if CS8
            ?
#endif
            Columns
        { get; }
    }

    public interface ISQLColumn
    {
        SQLColumn ToSQLColumn();
    }

    public interface ISQLColumnRequest<T> : ISQLColumnRequest where T : ISQLColumn
    {
        new Collections.Generic.IExtensibleEnumerable<T>
#if CS8
            ?
#endif
            Columns
        { get; }

#if CS8
        IEnumerable<SQLColumn>? ISQLColumnRequest.Columns => Columns?.Select(column => column.ToSQLColumn());
#endif
    }

    public interface ISQLTableRequestBase
    {

        IConditionGroup
#if CS8
            ?
#endif
            ConditionGroup
        { get; set; }
    }

    public interface ISQLTableRequest: ISQLTableRequestBase
    {
        Collections.Generic.IExtensibleEnumerable<string>
#if CS8
            ?
#endif
            Tables
        { get; }
    }

    public interface ISQLTableRequest2 : ISQLTableRequest, ISQLRequest
    {
        // Left empty.
    }

    public abstract class SQLRequest<TConnection, TCommand> where TConnection : IConnection<TCommand>
    {
        protected TConnection Connection { get; }

        public SQLRequest(in TConnection connection) => Connection = connection;

        public abstract string GetSQL();

        public virtual TCommand GetCommand() => Connection.GetCommand(GetSQL());

        public sealed override string ToString() => GetSQL();
    }

    public abstract class NonQueryRequest<TConnection, TCommand> : SQLRequest<TConnection, TCommand>, ISQLRequest where TConnection : IConnection<TCommand>
    {
        protected NonQueryRequest(in TConnection connection) : base(connection) { /* Left empty. */ }

        public abstract uint ExecuteNonQuery(out long? lastInsertedId);

        public abstract Task<SQLAsyncNonQueryRequestResult> ExecuteNonQueryAsync();

        public uint ExecuteNonQuery() => ExecuteNonQuery(out _);

        public async Task<uint> ExecuteNonQueryAsync2() => (await ExecuteNonQueryAsync()).Rows;
    }

    public abstract class SQLRequest2<TConnection, TCommand> : NonQueryRequest<TConnection, TCommand>, ISQLRequest3 where TConnection : IConnection<TCommand>
    {
        private string _tableName;

        public string TableName { get => _tableName; set => _tableName = value ?? throw GetArgumentNullException(nameof(value)); }

        protected SQLRequest2(in TConnection connection, in string tableName) : base(connection) => TableName = tableName;

        protected T ExecuteNonQuery<T>(in Func<TCommand, T> action, in Converter<TCommand, long?>
#if CS8
            ?
#endif
            converter, out long? lastInsertedId)
        {
            TCommand
#if CS9
            ?
#endif
            command = GetCommand();

            T
#if CS9
            ?
#endif
            result = action(command);

            lastInsertedId = converter == null ? null : converter(command);

            return result;
        }

        protected T ExecuteNonQuery<T>(in Func<TCommand, T> action, out long? lastInsertedId) => ExecuteNonQuery(action, null, out lastInsertedId);
    }

    public abstract class SQLTableRequest<TConnection, TCommand> : SQLRequest<TConnection, TCommand>, ISQLTableRequest where TConnection : IConnection<TCommand>
    {
        private SQLItemCollection<string> _tables;

        public SQLItemCollection<string> Tables { get => _tables; set => _tables = value ?? throw GetArgumentNullException(nameof(value)); }

        Collections.Generic.IExtensibleEnumerable<string> ISQLTableRequest.Tables => Tables;

        public IConditionGroup
#if CS8
            ?
#endif
            ConditionGroup
        { get; set; }

        protected SQLTableRequest(in TConnection connection, in SQLItemCollection<string> tables) : base(connection) => Tables = tables;

        protected abstract Action<TCommand, IConditionGroup
#if CS8
            ?
#endif
            > GetPrepareCommandAction();

        public override TCommand GetCommand()
        {
            TCommand command = base.GetCommand();

            GetPrepareCommandAction()(command, ConditionGroup);

            return command;
        }
    }
}
