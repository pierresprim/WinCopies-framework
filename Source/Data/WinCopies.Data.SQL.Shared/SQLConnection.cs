using WinCopies.EntityFramework;
using WinCopies.Temp;

namespace WinCopies.Data.SQL
{
    public struct SelectParameters<T> : ISelectParameters<T> where T : ISQLColumn
    {
        public IExtensibleEnumerable<string>? Tables { get; set; }

        public IExtensibleEnumerable<T>? Columns { get; set; }

        public IConditionGroup? ConditionGroup { get; set; }
    }

    public abstract class SQLConnection : ISQLConnection
    {
        public abstract bool IsClosed { get; }

        public bool IsDisposed { get; private set; }

        public abstract string EqualityOperator { get; }

        public abstract string NullityOperator { get; }

        public abstract string NullityOperand { get; }

        public abstract string ColumnDecorator { get; }

        public string? DBName { get; private set; }

        public SQLConnection() { /* Left empty. */ }

        public SQLConnection(in string dbName) => _ = UseDB(dbName);

        public ISQLConnection GetConnection(bool autoDispose = true)
        {
            if (DBName == null)

                throw new InvalidOperationException("No DB open.");

            ISQLConnection? connection = Clone(autoDispose);

            connection.Open();

            _ = connection.UseDB(DBName);

            return connection;
        }

        public abstract void Open();

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

        public SQLColumn GetColumn(string columnName) => new(columnName, ColumnDecorator);

        public abstract ISelect GetSelect(SQLItemCollection<string> defaultTables, SQLItemCollection<SQLColumn> defaultColumns);

        public abstract ISelect GetCountSelect(string tableName, IConditionGroup conditionGroup);

        public abstract IInsert GetInsert(string tableName, SQLItemCollection<StringSQLColumn>? columns, SQLItemCollection<SQLItemCollection<IParameter>> values);

        public abstract ISQLTableRequest2 GetDelete(SQLItemCollection<string> defaultTables);

        public abstract IUpdate GetUpdate(string tableName, SQLItemCollection<ICondition> columns);

        public abstract ICondition GetCondition<T>(string columnName, string paramName, T value, string? tableName = null, string @operator = "=");

        public abstract ICondition GetNullityCondition(string columnName, string? tableName = null, string @operator = "IS");

        public abstract IParameter<T> GetParameter<T>(string name, T value);

        public IEntityCollection<T> GetEntities<T>() where T : IEntity => new DBEntityCollection<T>(this);

        public abstract string? GetOperator(ConditionGroupOperator @operator);

        public abstract ISQLConnection Clone(bool autoDispose = true);

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
