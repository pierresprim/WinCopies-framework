using System;
using System.Linq;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.EntityFramework;
using WinCopies.Util;

using static WinCopies.Data.SQL.DBEntityCollection;
using static WinCopies.ThrowHelper;
using static WinCopies.UtilHelpers;

namespace WinCopies.Data.SQL
{
    public class EntityIdRefresher : IEntityIdRefresher
    {
        private ISQLConnection
#if CS8
            ?
#endif
            _connection;
        private readonly IUIntCountable _columns;

        protected ISQLConnection Connection => IsDisposed ? throw GetExceptionForDispose(false) : _connection;

        public bool IsDisposed => _connection == null;

        protected IEnumerableQueue<ICondition> Columns { get; } = new EnumerableQueue<ICondition>();

        IUIntCountable IEntityIdRefresher.Columns => _columns;

        public EntityIdRefresher(in ISQLConnection connection)
        {
            _connection = connection.GetConnection();

            _columns = new ReadOnlyEnumerableQueueCollection<ICondition>(Columns);
        }

        protected void ThrowIfDisposed() => ThrowHelper.ThrowIfDisposed(this);

        public void Add(string column, string paramName, object
#if CS8
            ?
#endif
            value)
        {
            ThrowIfDisposed();

            Columns.Enqueue(value == null ? Connection.GetNullityCondition(column) : Connection.GetCondition(column, paramName, value));
        }

        public bool TryGetId(string table, string idColumn, out object
#if CS8
            ?
#endif
            id)
        {
            ThrowIfDisposed();

            foreach (IPopable<string, object
#if CS8
            ?
#endif
            >
#if CS8
            ?
#endif
            popable in Connection.GetSelect(GetArray(table), GetArray(Connection.GetColumn(idColumn)), @operator: Connection.GetOperator(ConditionGroupOperator.And), conditions: Columns).GetPopables())
            {
                id = popable?.Pop(idColumn);

                return true;
            }

            id = null;

            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _connection.Dispose();

                _connection = null;
            }
        }

        ~EntityIdRefresher() => Dispose(false);

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class DBEntity<T> : Entity where T : IEntity
    {
        protected new DBEntityCollection<T> Collection => (DBEntityCollection<T>)base.Collection;

        protected ISQLConnection Connection => Collection.Connection;

        public DBEntity(in DBEntityCollection<T> collection) : base(collection) { /* Left empty. */ }

        public System.Collections.Generic.IEnumerable<ICondition> GetIdProperties()
        {
            string column;

            return EntityCollection.GetIdProperties(GetType()).Select(property => Connection.GetCondition(column = property.Key ?? property.Value.Name, column.FirstCharToLower(), property.Value.GetValue(this)));
        }

        public string GetOperator() => Connection.GetOperator(ConditionGroupOperator.And);

        public override ulong Remove(out uint tables) => EntityCollection.RemoveItem(this, table => Connection.GetConnection().GetDelete(SQLHelper.GetEnumerable(table), GetOperator(), GetIdProperties()).ExecuteNonQuery(), out tables);

        protected override bool RefreshOverride() => RefreshItem(Collection, Connection, this, new ConditionGroup(GetOperator()) { Conditions = SQLHelper.GetEnumerable(GetIdProperties()) });

        protected override ulong UpdateOverride(out uint tables)
        {
            _ = Add(Connection, () => new DBEntityCollectionUpdater(Connection), this, out tables, out ulong rows);

            return rows;
        }

        protected override IEntityIdRefresher GetRefresher() => new EntityIdRefresher(Connection);
    }

    public class DefaultDBEntity<TItems, TId> : DBEntity<TItems>, IDefaultEntity<TId> where TItems : IEntity
    {
        [EntityProperty(nameof(Id), IsId = true)]
        public TId Id { get; set; }

        public override Nullable TryRefreshIdWhen { get; } = new Nullable(default(TId));

        public DefaultDBEntity(DBEntityCollection<TItems> collection) : base(collection) { /* Left empty. */ }
    }

    public class NamedEntity<TItems, TId> : DefaultDBEntity<TItems, TId> where TItems : IEntity
    {
        private string _name;

        protected string EntityName { get => TryRefreshAndGet(ref _name); set => _name = value; }

        public NamedEntity(DBEntityCollection<TItems> collection) : base(collection) { /* Left empty. */ }
    }

    public class DefaultNamedEntity<TItems, TId> : NamedEntity<TItems, TId> where TItems : IEntity
    {
        [EntityProperty]
        public string Name { get => EntityName; set => EntityName = value; }

        public DefaultNamedEntity(DBEntityCollection<TItems> collection) : base(collection) { /* Left empty. */ }
    }

    public class DefaultNamedEntity2<TItems, TId> : NamedEntity<TItems, TId> where TItems : IEntity
    {
        [EntityProperty(IsPseudoId = true)]
        public string Name { get => EntityName; set => EntityName = value; }

        public DefaultNamedEntity2(DBEntityCollection<TItems> collection) : base(collection) { /* Left empty. */ }
    }
}
