using System;
using System.Collections.Generic;

using WinCopies.EntityFramework;

using static WinCopies.Temp.Util;

namespace WinCopies.Data.SQL
{
    public abstract class DBEntityCollectionUpdaterBase : EntityCollectionUpdater<IParameter, long?>
    {
        protected struct DBEntityCollectionDeleter : IEntityCollectionDeleter, DotNetFix.IDisposable
        {
            private ISQLConnection _connection;

            public bool IsDisposed => _connection == null;

            public DBEntityCollectionDeleter(in DBEntityCollectionUpdaterBase updater) => _connection = updater.Connection.GetConnection();

            public uint Delete(string table, string idColumn, object id) => IsDisposed
                    ? throw ThrowHelper.GetExceptionForDispose(false)
                    : _connection.GetDelete(GetArray(table), conditions: GetArray(_connection.GetCondition(idColumn, nameof(id), id))).ExecuteNonQuery();

            public void Dispose()
            {
                if (!IsDisposed)
                {
                    _connection.Dispose();

                    _connection = null;
                }
            }
        }

        protected ISQLConnection Connection { get; }

        public DBEntityCollectionUpdaterBase(in ISQLConnection connection) => Connection = (connection ?? throw ThrowHelper.GetArgumentNullException(nameof(connection))).GetConnection() ?? throw new InvalidOperationException("Could not retrieve a connection.");

        public abstract ISQLRequest
#if CS8
            ?
#endif
            GetRequest(string table);

        public sealed override uint ExecuteRequest(string table, out long? lastInsertedId)
        {
            ISQLRequest
#if CS8
            ?
#endif
            request = GetRequest(table);

            if (request == null)
            {
                lastInsertedId = null;

                return 0;
            }

            return request.ExecuteNonQuery(out lastInsertedId);
        }

        public static IEnumerable<ICondition> GetDefaultDeleteCondition(in ISQLConnection connection, in string foreignKeyIdColumn, in object foreignKeyId) => GetArray(connection.GetCondition(foreignKeyIdColumn, nameof(foreignKeyIdColumn), foreignKeyId));

        public override uint Delete(string table, string foreignKeyIdColumn, object foreignKeyId)
        {
            ISQLConnection connection = Connection.GetConnection();

            return connection.GetDelete(GetArray(table), conditions: GetDefaultDeleteCondition(connection, foreignKeyIdColumn, foreignKeyId)).ExecuteNonQuery();
        }

        protected override IEnumerable<Temp.IPopable<string, object
#if CS8
            ?
#endif
            >> GetValues(string table, string idColumn, string foreignKeyIdColumn, object foreignKeyId)
        {
            ISQLConnection connection = Connection.GetConnection();

            return connection.GetSelect(GetArray(table), GetArray(connection.GetColumn(idColumn)), conditions: GetDefaultDeleteCondition(connection, foreignKeyIdColumn, foreignKeyId)).GetPopables();
        }

        protected override IEntityCollectionDeleter GetDeleter() => new DBEntityCollectionDeleter();
    }

    public class DBEntityCollectionAdder : DBEntityCollectionUpdaterBase
    {
        private readonly TO_BE_DELETED.LinkedList<ICondition> _idColumns = new
#if !CS9
            TO_BE_DELETED.LinkedList<ICondition>
#endif
            ();

        protected SQLItemCollection<StringSQLColumn> Columns { get; } = new
#if !CS9
            SQLItemCollection<StringSQLColumn>
#endif
            ();

        protected SQLItemCollection<IParameter> Values { get; } = new
#if !CS9
            SQLItemCollection<IParameter>
#endif
            ();

        protected TO_BE_DELETED.LinkedListTEMP<ICondition> IdColumns { get; }

        public DBEntityCollectionAdder(in ISQLConnection connection) : base(connection) => IdColumns = new TO_BE_DELETED.LinkedListTEMP<ICondition>(_idColumns);

        public override void AddValue(string column, IParameter parameter, bool isId)
        {
            if (isId)
            {
                _ = IdColumns.List.AddLast(Connection.GetCondition(column, parameter.Name, parameter.Value));

                return;
            }

            Columns.Items.Append(new StringSQLColumn(column));

            Values.Items.Append(parameter);
        }

        public override ISQLRequest
#if CS8
            ?
#endif
            GetRequest(string table)
        {
            ISQLConnection
#if CS8
            ?
#endif
            connection = Connection.GetConnection();

            foreach (ISQLGetter
#if CS8
            ?
#endif
            getter in connection.GetSelect(SQLHelper.GetEnumerable(table), SQLHelper.GetEnumerable(connection.GetColumn(_idColumns.FirstValue.InnerCondition.Column.Value)), connection.GetOperator(ConditionGroupOperator.And), IdColumns).ExecuteQuery())

                return null;

            IInsert insert = Connection.GetInsert(table, Columns, new SQLItemCollection<SQLItemCollection<IParameter>>(Values));

            //insert.Ignore = true;

            return insert;
        }
    }

    public class DBEntityCollectionUpdater : DBEntityCollectionUpdaterBase
    {
        protected SQLItemCollection<ICondition> Values { get; } = new
#if !CS9
            SQLItemCollection<ICondition>
#endif
            ();

        protected TO_BE_DELETED.LinkedListTEMP<ICondition> IdColumns { get; } = new
#if !CS9
            TO_BE_DELETED.LinkedListTEMP<ICondition>
#endif
            (new TO_BE_DELETED.LinkedList<ICondition>());

        public DBEntityCollectionUpdater(in ISQLConnection connection) : base(connection) { /* Left empty. */ }

        public override void AddValue(string column, IParameter parameter, bool isId)
        {
            ICondition condition = Connection.GetCondition(column, parameter.Name, parameter.Value);

            if (isId)

                _ = IdColumns.List.AddLast(condition);

            else

                Values.Items.Append(condition);
        }

        public override ISQLRequest GetRequest(string table)
        {
            IUpdate request = Connection.GetUpdate(table, Values);

            request.ConditionGroup = new ConditionGroup(Connection.GetOperator(ConditionGroupOperator.And))
            {
                Conditions = IdColumns
            };

            return request;
        }
    }
}
