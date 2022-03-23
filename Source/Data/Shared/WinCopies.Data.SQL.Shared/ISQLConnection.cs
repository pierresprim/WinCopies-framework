using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WinCopies.EntityFramework;

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

    public interface ISQLConnection : ICloneable, DotNetFix.IDisposable
    {
        bool IsClosed { get; }

        string EqualityOperator { get; }

        string NullityOperator { get; }

        string NullityOperand { get; }

        string ColumnDecorator { get; }

        bool Open();

        Task OpenAsync();

        Task OpenAsync(CancellationToken cancellationToken);

        int UseDB(string dbName);

        ISQLConnection GetConnection(bool autoDispose = true);

        SQLColumn GetColumn(string columnName);

        ICondition GetCondition<T>(string columnName, string paramName, T value, string
#if CS8
            ?
#endif
            tableName = null, string @operator = "=");

        ICondition GetNullityCondition(string columnName, string
#if CS8
            ?
#endif
            tableName = null, string @operator = "IS");

        IOrderByColumns GetOrderByColumns(IEnumerable<string> columns, OrderBy orderBy)
#if CS8
            => new OrderByColumns(columns.Select(column => GetColumn(column)), orderBy)
#endif
            ;

        IOrderByColumns GetOrderByColumns(OrderBy orderBy, params string[] columns)
#if CS8
            => GetOrderByColumns(columns.AsEnumerable(), orderBy)
#endif
            ;

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

        ISelect GetCountSelect(string tableName, IConditionGroup conditionGroup);

        IInsert GetInsert(string tableName, SQLItemCollection<StringSQLColumn>
#if CS8
            ?
#endif
            columns, SQLItemCollection<SQLItemCollection<IParameter>> values);

        ISQLTableRequest2 GetDelete(SQLItemCollection<string> defaultTables);

        IUpdate GetUpdate(string tableName, SQLItemCollection<ICondition> columns);

        IParameter<T> GetParameter<T>(string name, T value);

        IEntityCollection<T> GetEntities<T>() where T : IEntity;

        string
#if CS8
            ?
#endif
            GetOperator(ConditionGroupOperator @operator);

        void Close();

        ISQLConnection Clone(bool autoDispose = true);

#if CS8
        object ICloneable.Clone() => Clone();
#endif
    }
}
