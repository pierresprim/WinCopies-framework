using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WinCopies.Temp;

using static WinCopies.ThrowHelper;

namespace WinCopies.Data.SQL
{
    public static class SQLHelper
    {
        public static void AddConditions(in IConditionGroup
#if CS8
            ?
#endif
            conditionGroup, in StringBuilder stringBuilder)
        {
            if (conditionGroup != null && conditionGroup.HasConditions
#if !CS8
                ()
#endif
                )

                _ = stringBuilder.Append($" WHERE {conditionGroup}");
        }

        public static string GetSQL(in string sql, in IConditionGroup
#if CS8
            ?
#endif
            conditionGroup)
        {
            StringBuilder sb = new
#if !CS9
                StringBuilder
#endif
                ();

            _ = sb.Append(sql);

            AddConditions(conditionGroup, sb);

            return sb.ToString();
        }

        public static TO_BE_DELETED.LinkedListTEMP<T> GetEnumerable<T>(in IEnumerable<T> items) => new
#if !CS9
            TO_BE_DELETED.LinkedListTEMP<T>
#endif
            (new TO_BE_DELETED.LinkedList<T>(items));

        public static TO_BE_DELETED.LinkedListTEMP<T> GetEnumerable<T>(params T[] items) => GetEnumerable((IEnumerable<T>)items);

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

#if !CS8
        public
#endif
        SQLColumn
#if CS8
            ISQLColumn.
#endif
            ToSQLColumn() => new
#if !CS9
            SQLColumn
#endif
            (Name);
    }

    public interface IInsert : ISQLRequest3, ISQLColumnRequest<StringSQLColumn>
    {
        bool Ignore { get; set; }

        IExtensibleEnumerable<IEnumerable<IParameter>> Values { get; }
    }

    public abstract class Insert<TConnection, TCommand> : SQLRequest2<TConnection, TCommand>, IInsert where TConnection : IConnection<TCommand>
    {
        private SQLItemCollection<SQLItemCollection<IParameter>> _values;

        public bool Ignore { get; set; }

        public SQLItemCollection<StringSQLColumn> Columns { get; set; }

        IExtensibleEnumerable<StringSQLColumn>
#if CS8
            ?
#endif
            ISQLColumnRequest<StringSQLColumn>.Columns => Columns;

#if !CS8
        IEnumerable<SQLColumn> ISQLColumnRequest.Columns => Columns?.Select(column => column.ToSQLColumn());
#endif

        public SQLItemCollection<SQLItemCollection<IParameter>> Values { get => _values; set => _values = value ?? throw GetArgumentNullException(nameof(value)); }

        IExtensibleEnumerable<IEnumerable<IParameter>> IInsert.Values => new ExtensibleEnumerable<SQLItemCollection<IParameter>, IEnumerable<IParameter>>(_values);

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
        IExtensibleEnumerable<ICondition> Columns { get; }

        IConditionGroup
#if CS8
            ?
#endif
            ConditionGroup
        { get; set; }
    }

    public abstract class Update<TConnection, TCommand> : SQLRequest2<TConnection, TCommand>, IUpdate where TConnection : IConnection<TCommand>
    {
        private SQLItemCollection<ICondition> _columns;

        public SQLItemCollection<ICondition> Columns { get => _columns; set => _columns = value ?? throw GetArgumentNullException(nameof(value)); }

        public IConditionGroup
#if CS8
            ?
#endif
            ConditionGroup
        { get; set; }

        IExtensibleEnumerable<ICondition> IUpdate.Columns { get => Columns; }

        protected Update(in TConnection connection, in string tableName, in SQLItemCollection<ICondition> columns) : base(connection, tableName) => Columns = columns;

        protected abstract Action<TCommand, IConditionGroup
#if CS8
            ?
#endif
            > GetPrepareCommandAction();

        protected override TCommand GetCommand()
        {
            TCommand command = base.GetCommand();

            GetPrepareCommandAction()(command, ConditionGroup);

            return command;
        }
    }
}
