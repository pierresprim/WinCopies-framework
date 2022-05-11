using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WinCopies.EntityFramework;

namespace WinCopies.Data.SQL
{
    public struct SelectParameters<T> : ISelectParameters<T> where T : ISQLColumn
    {
        public Collections.Generic.IExtensibleEnumerable<string>
#if CS8
            ?
#endif
            Tables
        { get; set; }

        public Collections.Generic.IExtensibleEnumerable<T>
#if CS8
            ?
#endif
            Columns
        { get; set; }

        public IConditionGroup
#if CS8
            ?
#endif
            ConditionGroup
        { get; set; }

#if !CS8
        IEnumerable<SQLColumn> ISQLColumnRequest.Columns => Columns?.Select(column => column.ToSQLColumn());
#endif
    }

    public abstract class SQLConnection : ISQLConnection
    {
        public abstract bool IsClosed { get; }

        public bool IsDisposed { get; private set; }

        public abstract string EqualityOperator { get; }

        public abstract string NullityOperator { get; }

        public abstract string NullityOperand { get; }

        public abstract string ColumnDecorator { get; }

        public string
#if CS8
            ?
#endif
            DBName
        { get; private set; }

        public SQLConnection() { /* Left empty. */ }

        public SQLConnection(in string dbName) => _ = UseDB(dbName);

        public ISQLConnection GetConnection(bool autoDispose = true)
        {
            if (DBName == null)

                throw new InvalidOperationException("No DB open.");

            ISQLConnection
#if CS8
            ?
#endif
            connection = Clone(autoDispose);

            connection.Open();

            _ = connection.UseDB(DBName);

            return connection;
        }

#if !CS8
        public IOrderByColumns GetOrderByColumns(IEnumerable<string> columns, OrderBy orderBy) => new OrderByColumns(columns.Select(column => GetColumn(column)), orderBy);

        public IOrderByColumns GetOrderByColumns(OrderBy orderBy, params string[] columns) => GetOrderByColumns(columns.AsEnumerable(), orderBy);

        public ISelect GetSelect(IEnumerable<string> defaultTables, IEnumerable<SQLColumn> defaultColumns, string @operator = null, IEnumerable<ICondition> conditions = null, IEnumerable<IConditionGroup> conditionGroups = null, IEnumerable<KeyValuePair<SQLColumn, ISelect>> selects = null) => DBEntityCollection.GetSelect(this, defaultTables, defaultColumns, @operator, conditions, conditionGroups, selects);

        public ISQLTableRequest2 GetDelete(IEnumerable<string> defaultTables, string @operator = null, IEnumerable<ICondition> conditions = null) => DBEntityCollection.GetDelete(this, defaultTables, @operator, conditions);
#endif

        public abstract bool Open();

        public abstract Task OpenAsync();

        public abstract Task OpenAsync(CancellationToken cancellationToken);

        protected abstract int? UseDBOverride(string dbName);

        public int UseDB(string dbName)
        {
            int? result = UseDBOverride(dbName);

            if (result.HasValue)
            {
                DBName = dbName;

                return result.Value;
            }

            return 0;
        }

        public SQLColumn GetColumn(string columnName) => new
#if !CS9
            SQLColumn
#endif
            (columnName, ColumnDecorator);

        public abstract ISelect GetSelect(SQLItemCollection<string> defaultTables, SQLItemCollection<SQLColumn> defaultColumns);

        public abstract ISelect GetCountSelect(string tableName, IConditionGroup conditionGroup);

        public abstract IInsert GetInsert(string tableName, SQLItemCollection<StringSQLColumn>
#if CS8
            ?
#endif
            columns, SQLItemCollection<SQLItemCollection<IParameter>> values);

        public abstract ISQLTableRequest2 GetDelete(SQLItemCollection<string> defaultTables);

        public abstract IUpdate GetUpdate(string tableName, SQLItemCollection<ICondition> columns);

        public abstract ICondition GetCondition<T>(string columnName, string paramName, T value, string
#if CS8
            ?
#endif
            tableName = null, string @operator = "=");

        public abstract ICondition GetNullityCondition(string columnName, string
#if CS8
            ?
#endif
            tableName = null, string @operator = "IS");

        public abstract IParameter<T> GetParameter<T>(string name, T value);

        public IEntityCollection<T> GetEntities<T>() where T : IEntity => new DBEntityCollection<T>(this);

        public abstract string
#if CS8
            ?
#endif
            GetOperator(ConditionGroupOperator @operator);

        public abstract ISQLConnection Clone(bool autoDispose = true);

#if !CS8
        object ICloneable.Clone() => Clone();
#endif

        public abstract void Close();

        protected virtual void Dispose(bool disposing) => IsDisposed = true;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~SQLConnection() => Dispose(false);
    }

    public abstract class SQLConnection<T> : SQLConnection where T : ICloneable, System.IDisposable
    {
        protected T Connection { get; }

        protected bool AutoDispose { get; }

        protected SQLConnection(in T connection, in bool autoDispose = true)
        {
            AutoDispose = autoDispose;

            Connection = connection;
        }

        protected SQLConnection(in T connection, in string dbName, in bool autoDispose = true) : base(dbName)
        {
            AutoDispose = autoDispose;

            Connection = connection;
        }

        protected virtual void DisposeManaged() { /* Left empty. */ }

        protected virtual void DisposeUnmanaged() => Connection.Dispose();

        protected sealed override void Dispose(bool disposing)
        {
            void disposeBase() => base.Dispose(disposing);

            if (AutoDispose)
            {
                if (disposing)

                    DisposeManaged();

                disposeBase();
            }

            else if (disposing)
            {
                DisposeManaged();

                disposeBase();
            }
        }
    }
}
