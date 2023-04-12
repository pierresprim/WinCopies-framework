using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using WinCopies.EntityFramework;
using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.Data.SQL.SQLConstants;
using static WinCopies.Data.SQL.SQLItemCollection;

namespace WinCopies.Data.SQL
{
    /*public struct SelectParameters<T> : ISelectParameters<T> where T : ISQLColumn
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
    }*/

    public abstract class SQLColumnInfo<TConnection, TTable> : SQLItem<TConnection, TTable>, ISQLColumnInfo<TConnection> where TConnection : ISQLConnection where TTable : ISQLTable<TConnection>
    {
        public ISQLTable Table => Parent;

        public ISQLColumnType ColumnType { get; }

        public abstract bool AllowNull { get; }

        public abstract object
#if CS8
            ?
#endif
            Default
        { get; }

        public abstract bool AutoIncrement { get; }

        public abstract bool IsPrimaryKey { get; }

        public abstract string
#if CS8
            ?
#endif
            Collation
        { get; }

        // public ISQLForeignKey ForeignKey { get; }

        protected SQLColumnInfo(in TTable table, in string name) : base(table, name) { /* Left empty. */ }

        public abstract string GetSQL();

        public abstract ISelect GetProperties();
    }

    /*public abstract class SQLUnicityIndex
    {
        public string Name { get; }
    }

    public abstract class SQLUnicityIndex2 : SQLUnicityIndex, IUnicityIndex
    {
        public IEnumerable<string> Columns { get; }

        public abstract string GetSQL();

        protected IEnumerator<string> GetEnumerator() => Columns.GetEnumerator();
        IEnumerator<string> IEnumerable<string>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }*/

    public abstract class SQLTable<TConnection, TDatabase> : SQLItem<TConnection, TDatabase> where TConnection : ISQLConnection where TDatabase : ISQLDatabase<TConnection>
    {
        public abstract string Engine { get; }

        public abstract string
#if CS8
            ?
#endif
            Collation
        { get; }

        public abstract ulong AutoIncrement { get; }

        protected SQLTable(in TDatabase database, in string name) : base(database, name) { /* Left empty. */ }

        //public abstract IEnumerable<IUnicityIndex> UnicityKeys { get; }
    }

    public abstract class SQLTable2<TConnection, TDatabase> : SQLTable<TConnection, TDatabase>, ISQLTable where TConnection : ISQLConnection where TDatabase : ISQLDatabase<TConnection>
    {
        ISQLDatabase ISQLTable.Database => Parent;

        public SQLTable2(in TDatabase database, in string name) : base(database, name) { /* Left empty. */ }

        public abstract ISelect GetProperties();

        public abstract string GetCreateTableSQL(bool ifNotExists, string characterSet);

        public abstract string GetRemoveTableSQL(bool ifExists);

        public abstract IDatabaseSelect GetColumns();

        public abstract IEnumerable<ISQLColumnInfo> AsEnumerable();
    }

    public abstract class SQLTable<TConnection, TDatabase, TColumn> : SQLTable2<TConnection, TDatabase>, ISQLTable<TConnection, TDatabase, TColumn> where TConnection : ISQLConnection where TDatabase : ISQLDatabase<TConnection> where TColumn : ISQLColumnInfo
    {
        protected SQLTable(in TDatabase database, in string name) : base(database, name) { /* Left empty. */ }

        protected abstract TColumn GetColumn(string columnName);

        public IEnumerable<TColumn> GetEnumerable() => GetColumns().ExecuteQuery(true).Select(GetColumn);

        public IEnumerator<TColumn> GetEnumerator() => GetEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class SQLConstants
    {
        public const string EQUAL = "=";

        public const string IS = nameof(IS);
    }

    public readonly
#if CS10
        record
#endif
        struct SQLConnectionParametersStruct
#if CS10
        (
#else
    {
        public
#endif
        string ServerName
#if CS10
        ,
#else
        {
            get;
#if CS9
            init;
#endif
        }

        public
#endif
        string UserName
#if CS10
        ,
#else
        {
            get;
#if CS9
            init;
#endif
        }

        public
#endif
        NetworkCredential Credential
#if CS10
        ,
#else
        {
            get;
#if CS9
            init;
#endif
        }

        public
#endif
        string
#if CS8
            ?
#endif
            DBName
#if CS10
        = null,
#else
        {
            get;
#if CS9
            init;
#endif
        }

        public
#endif
        Dictionary<string, string>
#if CS8
            ?
#endif
            Parameters
#if CS10
        = null);
#else
        {
            get;
#if CS9
            init;
#endif
#if !CS10
        }

        public SQLConnectionParametersStruct(in string serverName, in string userName, in NetworkCredential credential, in string
#if CS8
            ?
#endif
            dbName = null, in Dictionary<string, string>
#if CS8
            ?
#endif
            parameters = null)
        {
            ServerName = serverName;

            UserName = userName;

            Credential = credential;

            DBName = dbName;

            Parameters = parameters;
#endif
        }
    }
#endif

    public interface ISQLConnectionParameters<T>
    {
        SQLConnectionParametersStruct Parameters { get; }

        T GetConnection(string dbName);
    }

    public abstract class SQLConnectionParameters<T> : ISQLConnectionParameters<T>
    {
        public SQLConnectionParametersStruct Parameters { get; }

        public SQLConnectionParameters(in SQLConnectionParametersStruct parameters) => Parameters = parameters;

        public abstract T GetConnection(string
#if CS8
            ?
#endif
            dbName);
    }

    public abstract class SQLConnection : ISQLConnection
    {
        #region Properties
        #region Common Properties
        public abstract string ServerName { get; }

        public abstract string UserName { get; }

        public abstract bool IsClosed { get; }

        public abstract string EqualityOperator { get; }

        public abstract string NullityOperator { get; }

        public abstract string NullityOperand { get; }

        public abstract string ColumnDecorator { get; }

        public abstract string
#if CS8
            ?
#endif
            DBName
        { get; }

        public bool IsDisposed { get; private set; }
        #endregion Common Properties

        #region Granted Actions
        public abstract bool CanUseDB { get; }
        public abstract bool CanCreateDB { get; }
        public abstract bool CanSelect { get; }
        public abstract bool CanCount { get; }
        public abstract bool CanInsert { get; }
        public abstract bool CanUpdate { get; }
        public abstract bool CanDelete { get; }
        #endregion Granted Actions

        public abstract string Normalization { get; }
        #endregion Properties

        public event EventHandler<ISQLConnection> Opened;
        public event EventHandler<ISQLConnection> Closed;

        public SQLConnection() { /* Left empty. */ }

        public SQLConnection(in string
#if CS8
            ?
#endif
            dbName)
        {
            if (dbName == null)

                return;

            _ = UseDB(dbName);
        }

        #region Methods
        #region Misc
        public ISQLConnection GetConnection(bool autoDispose = true)
        {
            ISQLConnection
#if CS8
            ?
#endif
            connection = Clone(autoDispose);

            connection.Open();

            if (!UtilHelpers.IsNullEmptyOrWhiteSpace(DBName))

                _ = connection.UseDB(DBName);

            return connection;
        }

        #region UseDB
        protected abstract uint? UseDBOverride(string dbName);

        public uint? UseDB(string dbName) => UseDBOverride(dbName) ??
#if !CS9
            (uint?)
#endif
            0;
        #endregion UseDB

        private void TryInvokeEvent(in EventHandler<ISQLConnection> eventHandler) => eventHandler?.Invoke(this, EventArgs.Empty);

        public abstract ISQLConnection Clone(bool autoDispose = true);

#if !CS8
        object ICloneable.Clone() => Clone();
#endif
        #endregion

        #region Open
        protected abstract bool OpenOverride();

        public bool Open()
        {
            if (OpenOverride())
            {
                TryInvokeEvent(Opened);

                return true;
            }

            return false;
        }

        protected abstract Task<bool> OpenAsyncOverride();

        public async Task<bool> OpenAsync()
        {
            if (await OpenAsyncOverride())
            {
                TryInvokeEvent(Opened);

                return true;
            }

            return false;
        }

        protected abstract Task<bool> OpenAsyncOverride(CancellationToken cancellationToken);

        public async Task<bool> OpenAsync(CancellationToken cancellationToken)
        {
            if (await OpenAsyncOverride(cancellationToken))
            {
                TryInvokeEvent(Opened);

                return true;
            }

            return false;
        }
        #endregion Open

        #region Transaction
        public abstract string GetBeginTransactionSQL();

        public uint? BeginTransaction() => ExecuteNonQuery(GetBeginTransactionSQL);

        public abstract string GetEndTransactionSQL();

        public uint? EndTransaction() => ExecuteNonQuery(GetEndTransactionSQL);

        public abstract string GetResetTransactionSQL();

        public uint? ResetTransaction() => ExecuteNonQuery(GetResetTransactionSQL);

        protected uint? PerformTransaction(in IReadOnlyList<Func<uint?>> funcs, out bool reset)
        {
            // TODO: should also exists as a foreach version.

            uint? result = UtilHelpers.While(new Collections.Generic.ReadOnlyArrayArray2<Func<uint?>>(UtilHelpers.GetArrayEnumerable<Func<uint?>, IReadOnlyList<Func<uint?>>>(BeginTransaction, EndTransaction, item => new Func<uint?>[] { item }, funcs)), Delegates.HasValue, ResetTransaction, out _, out bool error);

            if (error)
            {
                reset = result.HasValue;

                return null;
            }

            reset = false;

            return result;
        }

        protected uint? PerformTransaction(out bool reset, params Func<uint?>[] funcs) => PerformTransaction(funcs.AsFromType<IReadOnlyList<Func<uint?>>>(), out reset);
        #endregion Transaction

        #region Table
        public uint? CreateTable(ISQLTable table, bool ifNotExists, string characterSet) => ExecuteNonQuery(table.GetCreateTableSQL(ifNotExists, characterSet));

        public uint? RemoveTable(ISQLTable table, bool ifExists) => ExecuteNonQuery(table.GetRemoveTableSQL(ifExists));

        #region UpdateTable
        public uint? UpdateTable(ISQLTable table, TableDropping dropping, bool ifNotExists, string characterSet)
        {
            bool removeTable(in bool ifExists) => RemoveTable(table, ifExists).HasValue;

            bool process()
#if CS8
                =>
#else
            {
                switch (
#endif
                dropping
#if CS8
                switch
#else
                )
#endif
                {
#if !CS8
                    case
#endif
                    TableDropping.Drop
#if CS8
                    =>
#else
                    :
                        return
#endif
                        removeTable(false)
#if CS8
                    ,
#else
                        ;
                    case
#endif
                    TableDropping.DropIfExists
#if CS8
                    =>
#else
                    :
                        return
#endif
                        removeTable(true)
#if CS8
                    ,
                    _ =>
#else
                        ;
                }

                return
#endif
                    true
#if CS8
                };
#else
                ;
            }
#endif

            return process() ? CreateTable(table, ifNotExists, characterSet) : null;
        }

        public uint? UpdateTable(ISQLTable table, TableDropping dropping, bool ifNotExists, string characterSet, out bool reset) => PerformTransaction(out reset, () => UpdateTable(table, dropping, ifNotExists, characterSet));
        #endregion UpdateTable
        #endregion Table

        #region DB
        #region AlterDB
        public uint? AlterDB(ISQLDatabase database, TableDropping dropping, bool ifNotExists, string characterSet, out ISQLTable
#if CS8
            ?
#endif
            errorTable)
        {
            uint? tmp;
            uint result = 0;

            foreach (ISQLTable table in database.AsEnumerable())

                if ((tmp = UpdateTable(table, dropping, ifNotExists, characterSet)).HasValue)

                    result += tmp.Value;

                else
                {
                    errorTable = table;

                    return null;
                }

            errorTable = null;

            return result;
        }

        public uint? AlterDBTransacted(ISQLDatabase database, TableDropping dropping, bool ifNotExists, string characterSet, out ISQLTable
#if CS8
            ?
#endif
            errorTable)
        {
            ISQLTable
#if CS8
            ?
#endif
            _errorTable = null;

            uint? result = PerformTransaction(out _, () => AlterDB(database, dropping, ifNotExists, characterSet, out _errorTable));

            errorTable = _errorTable;

            return result;
        }
        #endregion AlterDB

        #region CreateDB
        public abstract string GetCreateDBSQL(ISQLDatabase database, bool ifNotExists);

        public uint? CreateDB(ISQLDatabase database, bool ifNotExists) => ExecuteNonQuery(GetCreateDBSQL(database, ifNotExists));

        public uint? CreateDB(ISQLDatabase database, TableDropping dropping, bool ifNotExists, string characterSet, out bool reset) => PerformTransaction(out reset, () => CreateDB(database, ifNotExists), () => AlterDB(database, dropping, ifNotExists, characterSet, out _));
        #endregion CreateDB
        #endregion DB

        #region ExecuteNonQuery
        public abstract uint? ExecuteNonQuery(string sql);

        public uint? ExecuteNonQuery2(string sql) => ExecuteNonQuery(sql + ';');

        public uint? ExecuteNonQuery(Func<string> getSQL) => ExecuteNonQuery2(getSQL());
        #endregion ExecuteNonQuery

        #region Get Statements
        public abstract IDatabaseSelect GetDatabases();

        public ISelect GetSelect(string table, IEnumerable<ICondition> conditions) =>
#if CS8
            this.
#endif
            GetSelect(GetCollection(table), null, @operator: "AND", conditions: conditions);

        public abstract ISelect GetSelect(SQLItemCollection<string> defaultTables, SQLItemCollection<SQLColumn> defaultColumns);

        public abstract ISelect GetCountSelect(string tableName, IConditionGroup conditionGroup);

        public abstract IInsert GetInsert(string tableName, SQLItemCollection<StringSQLColumn>
#if CS8
            ?
#endif
            columns, SQLItemCollection<SQLItemCollection<IParameter>> values);

        public abstract IUpdate GetUpdate(string tableName, SQLItemCollection<ICondition> columns);

        public abstract ISQLTableRequest2 GetDelete(SQLItemCollection<string> defaultTables);

#if !CS8
        public ISelect GetSelect(IEnumerable<string> defaultTables, IEnumerable<SQLColumn>
#if CS8
            ?
#endif
            defaultColumns, string @operator = null, IEnumerable<ICondition> conditions = null, IEnumerable<IConditionGroup> conditionGroups = null, IEnumerable<KeyValuePair<SQLColumn, ISelect>> selects = null) => DBEntityCollection.GetSelect(this, defaultTables, defaultColumns, @operator, conditions, conditionGroups, selects);

        public ISQLTableRequest2 GetDelete(IEnumerable<string> defaultTables, string @operator = null, IEnumerable<ICondition> conditions = null) => DBEntityCollection.GetDelete(this, defaultTables, @operator, conditions);
#endif
        #endregion Get Statements

        #region GetItems
        public SQLColumn GetColumn(string columnName) => new
#if !CS9
            SQLColumn
#endif
            (columnName, ColumnDecorator);

        public abstract string GetItemName(string itemName);

        public abstract ICondition GetCondition<T>(string columnName, string paramName, T value, string
#if CS8
            ?
#endif
            tableName = null, string @operator = EQUAL);

        public abstract ICondition GetNullityCondition(string columnName, string
#if CS8
            ?
#endif
            tableName = null, string @operator = IS);

        public abstract IParameter<T> GetParameter<T>(string name, T value);

        public IEntityCollection<T> GetEntities<T>() where T : IEntity => new DBEntityCollection<T>(this);

        public abstract string
#if CS8
            ?
#endif
            GetOperator(ConditionGroupOperator @operator);

#if !CS8
        public IOrderByColumns GetOrderByColumns(OrderBy orderBy, params string[] columns) => GetOrderByColumns(columns.AsEnumerable(), orderBy);

        public IOrderByColumns GetOrderByColumns(IEnumerable<string> columns, OrderBy orderBy) => new OrderByColumns(columns.Select(column => GetColumn(column)), orderBy);
#endif
        #endregion GetItems

        #region Disposing
        protected abstract void CloseOverride();

        public void Close()
        {
            CloseOverride();

            TryInvokeEvent(Closed);
        }

        protected virtual void Dispose(bool disposing) => IsDisposed = true;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~SQLConnection() => Dispose(false);
        #endregion Disposing

        public abstract IEnumerable<ISQLDatabase> AsEnumerable();
        #endregion Methods
    }

    public abstract class SQLConnection<TConnection, TDatabase> : SQLConnection, ISQLConnection<TDatabase> where TConnection : ICloneable, System.IDisposable where TDatabase : ISQLDatabase
    {
        protected TConnection Connection { get; }

        protected bool AutoDispose { get; }

        protected SQLConnection(in TConnection connection, in bool autoDispose)
        {
            AutoDispose = autoDispose;

            Connection = connection;
        }

        protected SQLConnection(in TConnection connection, in string
#if CS8
            ?
#endif
            dbName = null, in bool autoDispose = true) : base(dbName)
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

        protected abstract TDatabase GetDatabase(string dbName);

        public IEnumerable<TDatabase> GetEnumerable() => GetDatabases().ExecuteQuery(true).Select(GetDatabase);

        public IEnumerator<TDatabase> GetEnumerator() => GetEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
