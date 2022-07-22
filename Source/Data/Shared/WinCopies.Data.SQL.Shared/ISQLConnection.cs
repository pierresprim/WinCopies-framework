using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WinCopies.EntityFramework;

using static WinCopies.Data.SQL.SQLConstants;

namespace WinCopies.Data.SQL
{
    public enum ConditionGroupOperator
    {
        And = 0,

        Or = 1,
    }

    public enum TableDropping
    {
        NoDrop = 0,

        Drop = 1,

        DropIfExists = 2
    }

    public interface ISQLColumnType
    {
        string Name { get; }

        uint? Length { get; }

        IEnumerable<string>
#if CS8
            ?
#endif
            Attributes
        { get; }
    }

    /*public interface ISQLForeignKey
    {
        string Table { get; }

        string Column { get; }
    }*/

    public interface ISQLProvider
    {
        string GetSQL();
    }

    public interface ISQLColumnInfo : ISQLProvider, ISQLPropertiesProvider, ISQLItem, ISQLItemProperties
    {
        ISQLTable Table { get; }

        ISQLColumnType ColumnType { get; }

        bool AllowNull { get; }

        object
#if CS8
            ?
#endif
            Default
        { get; }

        bool AutoIncrement { get; }

        bool IsPrimaryKey { get; }

        // ISQLForeignKey ForeignKey { get; set; }
    }

    public interface ISQLColumnInfo<T> : ISQLColumnInfo, ISQLItem<T> where T : ISQLConnection
    {
        // Left empty.
    }

    public interface ISQLColumnInfo<TConnection, TTable> : ISQLColumnInfo, ISQLItem<TConnection, TTable> where TConnection : ISQLConnection where TTable : ISQLTable
    {
        // Left empty.
    }

    /*public interface IUnicityIndex : IEnumerable<string>, ISQLProvider
    {
        string Name { get; }
    }*/

    public interface ISQLPropertiesProvider
    {
        ISelect GetProperties();
    }

    public interface ISQLItemProperties
    {
        string Collation { get; }
    }

    public interface ISQLTable : ISQLItemProperties, IAsEnumerable<ISQLColumnInfo>, ISQLPropertiesProvider, ISQLItem
    {
        ISQLDatabase Database { get; }

        string Engine { get; }

        ulong AutoIncrement { get; }

        //IEnumerable<IUnicityIndex> UnicityKeys { get; }

        string GetCreateTableSQL(bool ifNotExists, string characterSet);

        string GetRemoveTableSQL(bool ifExists);
    }

    public interface ISQLTable<T> : ISQLTable, ISQLItem<T> where T : ISQLConnection
    {
        // Left empty.
    }

    public interface ISQLTable<TConnection, TDatabase, TColumn> : IEnumerable<TColumn>, ISQLTable<TConnection>, ISQLItem<TConnection, TDatabase> where TConnection : ISQLConnection where TDatabase : ISQLDatabase where TColumn : ISQLColumnInfo
    {
        // Left empty.
    }

    public interface ISQLDatabase : ISQLItemProperties, IAsEnumerable<ISQLTable>, ISQLPropertiesProvider, ISQLItem
    {
        string CharacterSet { get; }

        IDatabaseSelect GetTables();
    }

    public interface ISQLDatabase<T> : ISQLDatabase, ISQLItem<T> where T : ISQLConnection
    {
        // Left empty.
    }

    public interface ISQLDatabase<TConnection, TTable> : ISQLDatabase<TConnection>, IEnumerable<TTable> where TConnection : ISQLConnection where TTable : ISQLTable
    {
        // Left empty.
    }

    public abstract class SQLItem<T> : ISQLItem<T> where T : ISQLConnection
    {
        public string Name { get; }

        public abstract T Connection { get; }
        ISQLConnection ISQLItemBase.Connection => Connection;

        protected SQLItem(in string name) => Name = name;
    }

    public abstract class SQLItem<TConnection, TParent> : SQLItem<TConnection>, ISQLItem<TConnection> where TConnection : ISQLConnection where TParent : ISQLItem<TConnection>
    {
        public TParent Parent { get; }

        public sealed override TConnection Connection => Parent.Connection;

        protected SQLItem(in TParent parent, in string name) : base(name) => Parent = parent;
    }

    public abstract class SQLDatabase<T> : SQLItem<T>, ISQLDatabase where T : ISQLConnection
    {
        public override T Connection { get; }

        public abstract string CharacterSet { get; }

        public abstract string
#if CS8
            ?
#endif
            Collation
        { get; }

        public SQLDatabase(in T connection, in string name) : base(name) => Connection = connection;

        public abstract ISelect GetProperties();

        public abstract IDatabaseSelect GetTables();

        public abstract IEnumerable<ISQLTable> AsEnumerable();
    }

    public abstract class SQLDatabase<TConnection, TTable> : SQLDatabase<TConnection>, ISQLDatabase<TConnection, TTable> where TConnection : ISQLConnection where TTable : ISQLTable
    {
        public SQLDatabase(in TConnection connection, in string dbName) : base(connection, dbName) { /* Left empty. */ }

        protected abstract TTable GetTable(string tableName);

        public IEnumerable<TTable> GetEnumerable() => GetTables().ExecuteQuery(true).Select(GetTable);

        public IEnumerator<TTable> GetEnumerator() => GetEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public interface ISQLItemBase
    {
        ISQLConnection Connection { get; }
    }

    public interface ISQLItemBase<T> : ISQLItemBase
    {
        new T Connection { get; }
    }

    public interface ISQLItem : ISQLItemBase
    {
        string Name { get; }
    }

    public interface ISQLItem<T> : ISQLItem, ISQLItemBase<T> where T : ISQLConnection
    {
        // Left empty.
    }

    public interface ISQLItem<TConnection, TParent> : ISQLItem<TConnection> where TConnection : ISQLConnection where TParent : ISQLItemBase
    {
        TParent Parent { get; }
    }

    public interface ISQLConnection : IAsEnumerable<ISQLDatabase>, ICloneable, DotNetFix.IDisposable
    {
        #region Common Properties
        string Normalization { get; }

        string ServerName { get; }

        string UserName { get; }

        bool IsClosed { get; }

        string EqualityOperator { get; }

        string NullityOperator { get; }

        string NullityOperand { get; }

        string ColumnDecorator { get; }

        string
#if CS8
            ?
#endif
            DBName
        { get; }
        #endregion

        #region Granted Actions

        // These properties should also indicate the permissions of the connected user.

        bool CanUseDB { get; }
        bool CanCreateDB { get; }
        bool CanSelect { get; }
        bool CanCount { get; }
        bool CanInsert { get; }
        bool CanUpdate { get; }
        bool CanDelete { get; }
        #endregion

        event EventHandler<ISQLConnection> Opened;
        event EventHandler<ISQLConnection> Closed;

        #region Methods
        #region Misc
        ISQLConnection GetConnection(bool autoDispose = true);

        uint? UseDB(string dbName);

        ISQLConnection Clone(bool autoDispose = true);
        #endregion

        #region Open
        bool Open();

        Task<bool> OpenAsync();

        Task<bool> OpenAsync(CancellationToken cancellationToken);
        #endregion

        #region Transaction
        string GetBeginTransactionSQL();

        uint? BeginTransaction();

        string GetEndTransactionSQL();

        uint? EndTransaction();

        string GetResetTransactionSQL();

        uint? ResetTransaction();
        #endregion Transaction

        #region Table
        uint? CreateTable(ISQLTable table, bool ifNotExists, string characterSet);

        uint? RemoveTable(ISQLTable table, bool ifExists);

        #region UpdateTable
        uint? UpdateTable(ISQLTable table, TableDropping dropping, bool ifNotExists, string characterSet);

        uint? UpdateTable(ISQLTable table, TableDropping dropping, bool ifNotExists, string characterSet, out bool reset);
        #endregion UpdateTable
        #endregion Table

        #region DB
        #region AlterDB
        uint? AlterDB(ISQLDatabase database, TableDropping dropping, bool ifNotExists, string characterSet, out ISQLTable
#if CS8
            ?
#endif
            errorTable);

        uint? AlterDBTransacted(ISQLDatabase database, TableDropping dropping, bool ifNotExists, string characterSet, out ISQLTable
#if CS8
            ?
#endif
            errorTable);
        #endregion AlterDB

        #region CreateDB
        string GetCreateDBSQL(ISQLDatabase database, bool ifNotExists);

        uint? CreateDB(ISQLDatabase database, bool ifNotExists);

        uint? CreateDB(ISQLDatabase database, TableDropping dropping, bool ifNotExists, string characterSet, out bool reset);
        #endregion CreateDB
        #endregion DB

        #region ExecuteNonQuery
        uint? ExecuteNonQuery(string sql);

        uint? ExecuteNonQuery2(string sql);

        uint? ExecuteNonQuery(Func<string> getSQL);
        #endregion ExecuteNonQuery

        #region Get Statements
        IDatabaseSelect GetDatabases();

        ISelect GetSelect(string table, IEnumerable<ICondition> conditions);

        ISelect GetSelect(SQLItemCollection<string> defaultTables, SQLItemCollection<SQLColumn>
#if CS8
            ?
#endif
            defaultColumns);

        ISelect GetSelect(IEnumerable<string> defaultTables, IEnumerable<SQLColumn>
#if CS8
            ?
#endif
            defaultColumns, string
#if CS8
            ?
#endif
            @operator = null, IEnumerable<ICondition>
#if CS8
            ?
#endif
            conditions = null, IEnumerable<IConditionGroup>
#if CS8
            ?
#endif
            conditionGroups = null, IEnumerable<KeyValuePair<SQLColumn, ISelect>>
#if CS8
            ?
#endif
            selects = null)
#if CS8
            => DBEntityCollection.GetSelect(this, defaultTables, defaultColumns, @operator, conditions, conditionGroups, selects)
#endif
            ;

        ISelect GetCountSelect(string tableName, IConditionGroup
#if CS8
            ?
#endif
            conditionGroup = null);

        IInsert GetInsert(string tableName, SQLItemCollection<StringSQLColumn>
#if CS8
            ?
#endif
            columns, SQLItemCollection<SQLItemCollection<IParameter>> values);

        IUpdate GetUpdate(string tableName, SQLItemCollection<ICondition> columns);

        ISQLTableRequest2 GetDelete(SQLItemCollection<string> defaultTables);

        ISQLTableRequest2 GetDelete(IEnumerable<string> defaultTables, string
#if CS8
            ?
#endif
            @operator = null, IEnumerable<ICondition>
#if CS8
            ?
#endif
            conditions = null)
#if CS8
            => DBEntityCollection.GetDelete(this, defaultTables, @operator, conditions)
#endif
            ;
        #endregion Get Statements

        #region GetItems
        SQLColumn GetColumn(string columnName);

        string GetItemName(string itemName);

        ICondition GetCondition<T>(string columnName, string paramName, T value, string
#if CS8
            ?
#endif
            tableName = null, string @operator = EQUAL);

        ICondition GetNullityCondition(string columnName, string
#if CS8
            ?
#endif
            tableName = null, string @operator = IS);

        IParameter<T> GetParameter<T>(string name, T value);

        IEntityCollection<T> GetEntities<T>() where T : IEntity;

        string
#if CS8
            ?
#endif
            GetOperator(ConditionGroupOperator @operator);

        IOrderByColumns GetOrderByColumns(OrderBy orderBy, params string[] columns)
#if CS8
            => GetOrderByColumns(columns.AsEnumerable(), orderBy)
#endif
            ;

        IOrderByColumns GetOrderByColumns(IEnumerable<string> columns, OrderBy orderBy)
#if CS8
            => new OrderByColumns(columns.Select(column => GetColumn(column)), orderBy)
#endif
            ;
        #endregion GetItems

        #region Disposing
        void Close();

#if CS8
        object ICloneable.Clone() => Clone();
#endif
        #endregion Disposing
        #endregion Methods
    }

    public interface ISQLConnection<T> : ISQLConnection, IEnumerable<T> where T : ISQLDatabase
    {
        IEnumerable<T> GetEnumerable();
    }
}
