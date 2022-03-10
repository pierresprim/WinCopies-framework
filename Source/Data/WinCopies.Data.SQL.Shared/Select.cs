using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinCopies.Temp;

using static WinCopies.ThrowHelper;

namespace WinCopies.Data.SQL
{
    public interface ISelectParameters : ISQLTableRequest
    {
        IConditionGroup
#if CS8
            ?
#endif
            ConditionGroup
        { get; set; }
    }

    public enum OrderBy
    {
        Asc = 0,

        Desc = 1
    }

    public struct OrderByColumns
    {
        public SQLItemCollection<SQLColumn> Columns { get; }

        public OrderBy OrderBy { get; }

        public OrderByColumns(in SQLItemCollection<SQLColumn> columns, in OrderBy orderBy)
        {
            Columns = columns;

            OrderBy = orderBy;
        }
    }

    public interface ISelect : ISelectParameters, ISQLColumnRequest
    {
        OrderByColumns? OrderBy { get; set; }

        IEnumerable<ISQLGetter> ExecuteQuery();

        Task<IEnumerable<ISQLGetter>> ExecuteQueryAsync();
    }

    public interface ISelectParameters<T> : ISelectParameters, ISQLColumnRequest<T> where T : ISQLColumn
    {
        // Left empty.
    }

    public interface ISelect<T> : ISelect, ISelectParameters<T> where T : ISQLColumn
    {
        // Left empty.
    }

    public abstract class Select<TConnection, TCommand, T> : SQLTableRequest<TConnection, TCommand>, ISelect<T> where TConnection : IConnection<TCommand> where T : ISQLColumn
    {
        private SQLItemCollection<T> _columns;

        public SQLItemCollection<T> Columns { get => _columns; set => _columns = value ?? throw GetArgumentNullException(nameof(value)); }

        IExtensibleEnumerable<T> ISQLColumnRequest<T>.Columns => Columns;

        public OrderByColumns? OrderBy { get; set; }

#if !CS8
        IEnumerable<SQLColumn> ISQLColumnRequest.Columns => Columns?.Select(column => column.ToSQLColumn());
#endif

        protected Select(in TConnection connection, in SQLItemCollection<string> tables, in SQLItemCollection<T> defaultColumns) : base(connection, tables) => Columns = defaultColumns;

        public abstract IEnumerable<ISQLGetter> ExecuteQuery();

        public abstract Task<IEnumerable<ISQLGetter>> ExecuteQueryAsync();
    }
}
