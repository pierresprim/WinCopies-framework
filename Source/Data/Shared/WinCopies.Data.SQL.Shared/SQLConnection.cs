using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WinCopies.EntityFramework;
using WinCopies.Util;

using static WinCopies.Data.SQL.SQLConstants;

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

    public abstract class SQLColumnInfo : ISQLColumnInfo
    {
        public string Name { get; set; }

        public ISQLColumnType ColumnType { get; set; }

        public bool AllowNull { get; set; }

        public object
#if CS8
            ?
#endif
            Default
        { get; set; }

        public bool AutoIncrement { get; set; }

        public bool IsPrimaryKey { get; set; }

        // public ISQLForeignKey ForeignKey { get; set; }

        public abstract string GetSQL();
    }

    public abstract class SQLUnicityIndex
    {
        public string Name { get; set; }
    }

    public abstract class SQLUnicityIndex2 : SQLUnicityIndex, IUnicityIndex
    {
        public IEnumerable<string> Columns { get; set; }

        public abstract string GetSQL();

        protected IEnumerator<string> GetEnumerator() => Columns.GetEnumerator();
        IEnumerator<string> IEnumerable<string>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public abstract class SQLTable
    {
        public string Name { get; set; }

        public string Engine { get; set; }

        public string CharacterSet { get; set; }

        public ulong AutoIncrement { get; set; }

        public IEnumerable<IUnicityIndex> UnicityKeys { get; set; }
    }

    public abstract class SQLTable2 : SQLTable, ISQLTable
    {
        public IEnumerable<ISQLColumnInfo> Columns { get; set; }

        public abstract string GetCreateTableSQL(bool ifNotExists);

        public abstract string GetRemoveTableSQL(bool ifExists);

        protected IEnumerator<ISQLColumnInfo> GetEnumerator() => Columns.GetEnumerator();
        IEnumerator<ISQLColumnInfo> IEnumerable<ISQLColumnInfo>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class SQLConstants
    {
        public const string EQUAL = "=";

        public const string IS = nameof(IS);
    }

    public abstract class SQLConnection : ISQLConnection
    {
        #region Common Properties
        public abstract bool IsClosed { get; }

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

        public SQLConnection() { /* Left empty. */ }

        public SQLConnection(in string dbName) => _ = UseDB(dbName);

        #region Misc
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

        #region UseDB
        protected abstract uint? UseDBOverride(string dbName);

        public uint? UseDB(string dbName)
        {
            uint? result = UseDBOverride(dbName);

            if (result.HasValue)
            {
                DBName = dbName;

                return result.Value;
            }

            return 0;
        }
        #endregion UseDB

        public abstract ISQLConnection Clone(bool autoDispose = true);

#if !CS8
        object ICloneable.Clone() => Clone();
#endif
        #endregion

        #region Open
        public abstract bool Open();

        public abstract Task OpenAsync();

        public abstract Task OpenAsync(CancellationToken cancellationToken);
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
        public uint? CreateTable(ISQLTable table, bool ifNotExists) => ExecuteNonQuery(table.GetCreateTableSQL(ifNotExists));

        public uint? RemoveTable(ISQLTable table, bool ifExists) => ExecuteNonQuery(table.GetRemoveTableSQL(ifExists));

        #region UpdateTable
        public uint? UpdateTable(ISQLTable table, TableDropping dropping, bool ifNotExists)
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

            return process() ? CreateTable(table, ifNotExists) : null;
        }

        public uint? UpdateTable(ISQLTable table, TableDropping dropping, bool ifNotExists, out bool reset) => PerformTransaction(out reset, () => UpdateTable(table, dropping, ifNotExists));
        #endregion UpdateTable
        #endregion Table

        #region DB
        #region AlterDB
        public uint? AlterDB(ISQLDatabase database, TableDropping dropping, bool ifNotExists, out ISQLTable
#if CS8
            ?
#endif
            errorTable)
        {
            uint? tmp;
            uint result = 0;

            foreach (ISQLTable table in database)

                if ((tmp = UpdateTable(table, dropping, ifNotExists)).HasValue)

                    result += tmp.Value;

                else
                {
                    errorTable = table;

                    return null;
                }

            errorTable = null;

            return result;
        }

        public uint? AlterDBTransacted(ISQLDatabase database, TableDropping dropping, bool ifNotExists, out ISQLTable
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

            uint? result = PerformTransaction(out _, () => AlterDB(database, dropping, ifNotExists, out _errorTable));

            errorTable = _errorTable;

            return result;
        }
        #endregion AlterDB

        #region CreateDB
        public abstract string GetCreateDBSQL(ISQLDatabase database, bool ifNotExists);

        public uint? CreateDB(ISQLDatabase database, bool ifNotExists) => ExecuteNonQuery(GetCreateDBSQL(database, ifNotExists));

        public uint? CreateDB(ISQLDatabase database, TableDropping dropping, bool ifNotExists, out bool reset) => PerformTransaction(out reset, () => CreateDB(database, ifNotExists), () => AlterDB(database, dropping, ifNotExists, out _));
        #endregion CreateDB
        #endregion DB

        #region ExecuteNonQuery
        public abstract uint? ExecuteNonQuery(string sql);

        public uint? ExecuteNonQuery2(string sql) => ExecuteNonQuery(sql + ';');

        public uint? ExecuteNonQuery(Func<string> getSQL) => ExecuteNonQuery2(getSQL());
        #endregion ExecuteNonQuery

        #region Get Statements
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
        public ISelect GetSelect(IEnumerable<string> defaultTables, IEnumerable<SQLColumn> defaultColumns, string @operator = null, IEnumerable<ICondition> conditions = null, IEnumerable<IConditionGroup> conditionGroups = null, IEnumerable<KeyValuePair<SQLColumn, ISelect>> selects = null) => DBEntityCollection.GetSelect(this, defaultTables, defaultColumns, @operator, conditions, conditionGroups, selects);

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
        public abstract void Close();

        protected virtual void Dispose(bool disposing) => IsDisposed = true;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~SQLConnection() => Dispose(false);
        #endregion Disposing
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
