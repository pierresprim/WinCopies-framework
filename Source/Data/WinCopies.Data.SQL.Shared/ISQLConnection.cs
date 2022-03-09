using WinCopies.EntityFramework;

using static WinCopies.Data.SQL.SQLHelper;

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

        ICondition GetCondition<T>(string columnName, string paramName, T value, string? tableName = null, string @operator = "=");

        ICondition GetNullityCondition(string columnName, string? tableName = null, string @operator = "IS");

        ISelect GetSelect(SQLItemCollection<string> defaultTables, SQLItemCollection<SQLColumn> defaultColumns);

        ISelect GetSelect(IEnumerable<string> defaultTables, IEnumerable<SQLColumn> defaultColumns, string? @operator = null, IEnumerable<ICondition>? conditions = null, IEnumerable<IConditionGroup>? conditionGroups = null, IEnumerable<KeyValuePair<SQLColumn, ISelect>>? selects = null)
        {
            ISelect select = GetSelect(new SQLItemCollection<string>(defaultTables), new SQLItemCollection<SQLColumn>(defaultColumns));

            IConditionGroup initConditionGroup() => select.ConditionGroup = new ConditionGroup(@operator);

            void initConditionGroup2(Action<IConditionGroup> action) => action(select.ConditionGroup ?? initConditionGroup());

            if (conditions != null)

                initConditionGroup().Conditions = GetEnumerable(conditions);

            if (conditionGroups != null)

                initConditionGroup2(conditionGroup => conditionGroup.ConditionGroups = GetEnumerable(conditionGroups));

            if (selects != null)

                initConditionGroup2(conditionGroup => conditionGroup.Selects = GetEnumerable(selects));

            return select;
        }

        ISQLTableRequest2 GetDelete(IEnumerable<string> defaultTables, string? @operator = null, IEnumerable<ICondition>? conditions = null)
        {
            ISQLTableRequest2? delete = GetDelete(new SQLItemCollection<string>(defaultTables));

            if (conditions != null)

                delete.ConditionGroup = new ConditionGroup(@operator)
                {
                    Conditions = GetEnumerable(conditions)
                };

            return delete;
        }

        ISelect GetCountSelect(string tableName, IConditionGroup conditionGroup);

        IInsert GetInsert(string tableName, SQLItemCollection<StringSQLColumn>? columns, SQLItemCollection<SQLItemCollection<IParameter>> values);

        ISQLTableRequest2 GetDelete(SQLItemCollection<string> defaultTables);

        IUpdate GetUpdate(string tableName, SQLItemCollection<ICondition> columns);

        IParameter<T> GetParameter<T>(string name, T value);

        IEntityCollection<T> GetEntities<T>() where T : IEntity;

        string? GetOperator(ConditionGroupOperator @operator);

        void Close();

        ISQLConnection Clone(bool autoDispose = true);

        object ICloneable.Clone() => Clone();
    }
}
