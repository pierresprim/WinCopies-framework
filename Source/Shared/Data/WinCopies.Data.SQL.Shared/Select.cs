using System.Collections.Generic;
using System.Linq;

using WinCopies.EntityFramework;

namespace WinCopies.Data.SQL
{
    public interface ISelectParametersBase : ISQLTableRequestBase
    {
        IConditionGroup
#if CS8
            ?
#endif
            ConditionGroup
        { get; set; }
    }

    public interface ISelectParameters : ISelectParametersBase, ISQLTableRequest
    {
        // Left empty.
    }

    public struct OrderByColumns : IOrderByColumns
    {
        public SQLItemCollection<SQLColumn> Columns { get; }

        public OrderBy OrderBy { get; }

        IEnumerable<string> IOrderByColumns.Columns => Columns.Select(column => column.ToString());

        public OrderByColumns(in SQLItemCollection<SQLColumn> columns, in OrderBy orderBy)
        {
            Columns = columns;

            OrderBy = orderBy;
        }

        public OrderByColumns(in IEnumerable<SQLColumn> columns, in OrderBy orderBy) : this(columns is SQLItemCollection<SQLColumn> _columns ? _columns : new SQLItemCollection<SQLColumn>(columns), orderBy) { /* Left empty. */ }

        public OrderByColumns(in OrderBy orderBy, params SQLColumn[] columns) : this(columns.AsEnumerable(), orderBy) { /* Left empty. */ }
    }

    public interface ISelectBase<T> : ISelectParametersBase
    {
        OrderByColumns? OrderBy { get; set; }

        Interval? Interval { get; set; }

        Collections.Generic.IDisposableEnumerable<T> ExecuteQuery(in bool dispose);
    }

    public interface ISelect : ISelectBase<ISQLGetter>, ISQLColumnRequest
    {

    }

    public interface IDatabaseSelect : ISelectBase<string>
    {

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
        public SQLItemCollection<T>
#if CS8
            ?
#endif
            Columns
        { get; set; }

        Collections.Generic.IExtensibleEnumerable<T>
#if CS8
            ?
#endif
            ISQLColumnRequest<T>.Columns => Columns;

        public OrderByColumns? OrderBy { get; set; }

        public Interval? Interval { get; set; }

#if !CS8
        IEnumerable<SQLColumn> ISQLColumnRequest.Columns => Columns?.Select(column => column.ToSQLColumn());
#endif

        protected Select(in TConnection connection, in SQLItemCollection<string> tables, in SQLItemCollection<T> defaultColumns) : base(connection, tables) => Columns = defaultColumns;

        public abstract Collections.Generic.IDisposableEnumerable<ISQLGetter> ExecuteQuery(in bool dispose);
    }
}
