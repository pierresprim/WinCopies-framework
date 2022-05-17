using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WinCopies.EntityFramework;

using static WinCopies.Data.SQL.SQLConstants;

#if CS8
using System.Linq;
#endif

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

    public interface ISQLColumnInfo : ISQLProvider
    {
        string Name { get; set; }

        ISQLColumnType ColumnType { get; set; }

        bool AllowNull { get; set; }

        object
#if CS8
            ?
#endif
            Default
        { get; set; }

        bool AutoIncrement { get; set; }

        bool IsPrimaryKey { get; set; }

        // ISQLForeignKey ForeignKey { get; set; }
    }

    public interface IUnicityIndex : IEnumerable<string>, ISQLProvider
    {
        string Name { get; set; }
    }

    public interface ISQLTable : IEnumerable<ISQLColumnInfo>
    {
        string Name { get; set; }

        string Engine { get; set; }

        string CharacterSet { get; set; }

        ulong AutoIncrement { get; set; }

        IEnumerable<IUnicityIndex> UnicityKeys { get; set; }

        string GetCreateTableSQL(bool ifNotExists);

        string GetRemoveTableSQL(bool ifExists);
    }

    public interface ISQLDatabase : IEnumerable<ISQLTable>
    {
        string Name { get; }

        string CharacterSet { get; }

        string Collate { get; }
    }

    public interface ISQLConnection : ICloneable, DotNetFix.IDisposable
    {
        #region Common Properties
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

        #region Misc
        ISQLConnection GetConnection(bool autoDispose = true);

        uint? UseDB(string dbName);

        ISQLConnection Clone(bool autoDispose = true);
        #endregion

        #region Open
        bool Open();

        Task OpenAsync();

        Task OpenAsync(CancellationToken cancellationToken);
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
        uint? CreateTable(ISQLTable table, bool ifNotExists);

        uint? RemoveTable(ISQLTable table, bool ifExists);

        #region UpdateTable
        uint? UpdateTable(ISQLTable table, TableDropping dropping, bool ifNotExists);

        uint? UpdateTable(ISQLTable table, TableDropping dropping, bool ifNotExists, out bool reset);
        #endregion UpdateTable
        #endregion Table

        #region DB
        #region AlterDB
        uint? AlterDB(ISQLDatabase database, TableDropping dropping, bool ifNotExists, out ISQLTable
#if CS8
            ?
#endif
            errorTable);

        uint? AlterDBTransacted(ISQLDatabase database, TableDropping dropping, bool ifNotExists, out ISQLTable
#if CS8
            ?
#endif
            errorTable);
        #endregion AlterDB

        #region CreateDB
        string GetCreateDBSQL(ISQLDatabase database, bool ifNotExists);

        uint? CreateDB(ISQLDatabase database, bool ifNotExists);

        uint? CreateDB(ISQLDatabase database, TableDropping dropping, bool ifNotExists, out bool reset);
        #endregion CreateDB
        #endregion DB

        #region ExecuteNonQuery
        uint? ExecuteNonQuery(string sql);

        uint? ExecuteNonQuery2(string sql);

        uint? ExecuteNonQuery(Func<string> getSQL);
        #endregion ExecuteNonQuery

        #region Get Statements
        ISelect GetSelect(SQLItemCollection<string> defaultTables, SQLItemCollection<SQLColumn> defaultColumns);

        ISelect GetSelect(IEnumerable<string> defaultTables, IEnumerable<SQLColumn> defaultColumns, string
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

        ISelect GetCountSelect(string tableName, IConditionGroup conditionGroup);

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
    }
}
