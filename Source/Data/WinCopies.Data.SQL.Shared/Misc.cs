using System.Text;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Temp;

using static WinCopies.ThrowHelper;

namespace WinCopies.Data.SQL
{
    public static class SQLHelper
    {
        public static void AddConditions(in IConditionGroup? conditionGroup, in StringBuilder stringBuilder)
        {
            if (conditionGroup != null && conditionGroup.HasConditions)

                _ = stringBuilder.Append($" WHERE {conditionGroup}");
        }

        public static string GetSQL(in string sql, in IConditionGroup? conditionGroup)
        {
            StringBuilder sb = new();

            _ = sb.Append(sql);

            AddConditions(conditionGroup, sb);

            return sb.ToString();
        }

        public static TO_BE_DELETED.LinkedListTEMP<T> GetEnumerable<T>(in System.Collections.Generic.IEnumerable<T> items) => new(new TO_BE_DELETED.LinkedList<T>(items));

        public static TO_BE_DELETED.LinkedListTEMP<T> GetEnumerable<T>(params T[] items) => GetEnumerable((System.Collections.Generic.IEnumerable<T>)items);

        public static T ExecuteNonQuery<T>(in Func<T> action, out long? lastInsertedId)
        {
            ThrowIfNull(action, nameof(action));

            lastInsertedId = null;

            return action();
        }
    }

    //public interface IAsyncEnumerableProvider<T>
    //{
    //    IAsyncResult Result { get; }

    //    IEnumerable<T> GetEnumerable();
    //}

    public interface ISQLGetter : DotNetFix.IDisposable
    {
        bool IsIntIndexable { get; }

        object this[int index] { get; }

        bool IsStringIndexable { get; }

        object this[string columnName] { get; }
    }

    public interface IConnection<T>
    {
        T GetCommand(string sql);
    }

    public interface ISQLRequest2
    {
        string TableName { get; }
    }

    public interface ISQLRequest3 : ISQLRequest, ISQLRequest2
    {
        // Left empty.
    }

    public struct StringSQLColumn : ISQLColumn
    {
        public string Name { get; set; }

        public StringSQLColumn(in string name) => Name = name;

        SQLColumn ISQLColumn.ToSQLColumn() => new(Name);
    }

    public interface IInsert : ISQLRequest3, ISQLColumnRequest<StringSQLColumn>
    {
        IExtensibleEnumerable<System.Collections.Generic.IEnumerable<IParameter>> Values { get; }
    }

    public abstract class Insert<TConnection, TCommand> : SQLRequest2<TConnection, TCommand>, IInsert where TConnection : IConnection<TCommand>
    {
        private SQLItemCollection<SQLItemCollection<IParameter>> _values;

        public SQLItemCollection<StringSQLColumn> Columns { get; set; }

        IExtensibleEnumerable<StringSQLColumn>? ISQLColumnRequest<StringSQLColumn>.Columns => Columns;

        public SQLItemCollection<SQLItemCollection<IParameter>> Values { get => _values; set => _values = value ?? throw GetArgumentNullException(nameof(value)); }

        IExtensibleEnumerable<System.Collections.Generic.IEnumerable<IParameter>> IInsert.Values => new ExtensibleEnumerable<SQLItemCollection<IParameter>, System.Collections.Generic.IEnumerable<IParameter>>(_values);

        protected Insert(in TConnection connection, in string tableName, in SQLItemCollection<StringSQLColumn> columns, in SQLItemCollection<SQLItemCollection<IParameter>> values) : base(connection, tableName)
        {
            Columns = columns;

            Values = values;
        }

        protected abstract Action<TCommand> GetPrepareCommandAction();

        protected override TCommand GetCommand()
        {
            TCommand command = base.GetCommand();

            GetPrepareCommandAction()(command);

            return command;
        }
    }

    public interface IUpdate : ISQLRequest3
    {
        public IExtensibleEnumerable<ICondition> Columns { get; }

        public IConditionGroup? ConditionGroup { get; set; }
    }

    public abstract class Update<TConnection, TCommand> : SQLRequest2<TConnection, TCommand>, IUpdate where TConnection : IConnection<TCommand>
    {
        private SQLItemCollection<ICondition> _columns;

        public SQLItemCollection<ICondition> Columns { get => _columns; set => _columns = value ?? throw GetArgumentNullException(nameof(value)); }

        public IConditionGroup? ConditionGroup { get; set; }

        IExtensibleEnumerable<ICondition> IUpdate.Columns { get => Columns; }

        protected Update(in TConnection connection, in string tableName, in SQLItemCollection<ICondition> columns) : base(connection, tableName) => Columns = columns;

        protected abstract Action<TCommand, IConditionGroup?> GetPrepareCommandAction();

        protected override TCommand GetCommand()
        {
            TCommand command = base.GetCommand();

            GetPrepareCommandAction()(command, ConditionGroup);

            return command;
        }
    }
}
