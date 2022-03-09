using System.Collections;

using WinCopies.EntityFramework;
using WinCopies.Linq;
using WinCopies.Temp;

using static WinCopies.EntityFramework.EntityCollection;
using static WinCopies.Temp.Util;

namespace WinCopies.Data.SQL
{
    public struct IntPopable : IPopable<string, object?>
    {
        private readonly ISQLGetter _getter;

        public int CurrentIndex { get; private set; } = 0;

        public IntPopable(in ISQLGetter getter) => _getter = getter;

        public object Pop(string key) => _getter[CurrentIndex++];
    }

    public struct StringPopable : IPopable<string, object?>
    {
        private readonly ISQLGetter _getter;

        public StringPopable(in ISQLGetter getter) => _getter = getter;

        public object Pop(string key) => _getter[key];
    }

    public abstract class EntityCollectionLoadingFactory<T, U> : IEntityCollectionLoadingFactory<T, U, ICondition>
    {
        private ISelect? _select;
        private IConditionGroup? _conditionGroup;

        protected Func<ISQLGetter, IPopable<string, object?>> ParseIntIndexableDelegate { get; private set; }

        protected ISQLConnection Connection { get; private set; }

        protected ISelect Select => _select ?? throw new InvalidOperationException("No select statement has been initialized.");

        public IConditionGroup ConditionGroup => _conditionGroup ?? throw new InvalidOperationException("No condition group has been initialized.");

        public uint ColumnsCount { get; private set; }

        public EntityCollectionLoadingFactory(in ISQLConnection connection, in IConditionGroup? conditionGroup)
        {
            Connection = (connection ?? throw ThrowHelper.GetArgumentNullException(nameof(connection))).GetConnection();

            if (Connection.IsClosed)

                throw new Exception();

            _conditionGroup = conditionGroup;

            ResetDelegate();
        }

        public void ResetDelegate() => ParseIntIndexableDelegate = getter => ColumnsCount > int.MaxValue
            ? throw new InvalidOperationException("Too much columns.")
            : (ParseIntIndexableDelegate = getter => new IntPopable(getter))(getter);

        public void Reset()
        {
            ColumnsCount = 0;

            _select = null;
            _conditionGroup = null;

            ResetDelegate();
        }

        public ICondition GetCondition(string column, string param, object? value) => Connection.GetCondition(column, param, value);

        public abstract T GetItem(U collection);

        public IEnumerable GetOnlyFirstItems(string column)
        {
            ISelect? select = Select;

            foreach (IPopable<string, object?>? item in select.GetPopables())

                yield return item.Pop(column);
        }

        public void InitConditionGroup(bool multiple)
        {
            _conditionGroup = new ConditionGroup(multiple ? Connection.GetOperator(ConditionGroupOperator.And) : null);

            if (_select != null)

                _select.ConditionGroup = _conditionGroup;
        }

        public void InitSelector(string table, IEnumerable<string> columns)
        {
            _select = (Connection = Connection.GetConnection()).GetSelect(GetArray(table), columns.Select(column => Connection.GetColumn(column)));

            if (_conditionGroup != null)

                _select.ConditionGroup = _conditionGroup;
        }

        public void SetConditions(params ICondition[] conditions) => ConditionGroup.Conditions = SQLHelper.GetEnumerable(conditions);

        public IEnumerable<IPopable<string, object?>> GetItems() => Select.GetItems(ParseIntIndexableDelegate, DBEntityCollection.GetStringPopable);

        public void OnColumnsLoaded(uint columnsCount) => ColumnsCount = columnsCount;

        public IEnumerator<IPopable<string, object?>> GetEnumerator() => GetItems().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public object GetCollectionConstructorParameter() => Connection;
    }

    public class EntityCollectionNewItemLoadingFactory<T, U> : EntityCollectionLoadingFactory<T, U> where T : IEntity
    {
        public EntityCollectionNewItemLoadingFactory(in ISQLConnection connection, in IConditionGroup? conditionGroup) : base(connection, conditionGroup) { /* Left empty. */ }

        public override T GetItem(U collection) => GetDBEntity<T, U>(collection);
    }

    public class EntityCollectionItemRefreshingFactory<T, U> : EntityCollectionLoadingFactory<T, U>
    {
        private readonly T _item;

        public EntityCollectionItemRefreshingFactory(in ISQLConnection connection, in T item, in IConditionGroup? conditionGroup) : base(connection, conditionGroup) => _item = item;

        public override T GetItem(U collection) => _item;
    }

    public static class DBEntityCollection
    {
        public static T Parse<T>(in ISQLGetter getter, in Func<ISQLGetter, T> intAction, in Func<ISQLGetter, T> stringAction) => getter.IsIntIndexable
                ? intAction(getter)
                : getter.IsStringIndexable
                ? stringAction(getter)
                : throw new InvalidOperationException("The given getter can not be accessed neither as int nor as string indexable.");

        public static IEnumerable<T> GetItems<T>(this ISelect select, Func<ISQLGetter, T> intAction, Func<ISQLGetter, T> stringAction)
        {
            foreach (ISQLGetter getter in select.ExecuteQuery())

                yield return Parse(getter, intAction, stringAction);
        }

        public static IEnumerable<IPopable<string, object?>> GetPopablesI(this ISelect select, Func<ISQLGetter, IPopable<string, object?>> intAction) => select.GetItems(intAction, GetStringPopable);

        public static IEnumerable<IPopable<string, object?>> GetPopablesS(this ISelect select, Func<ISQLGetter, IPopable<string, object?>> stringAction) => select.GetItems(GetIntPopable, stringAction);

        public static IEnumerable<IPopable<string, object?>> GetPopables(this ISelect select) => select.GetItems(GetIntPopable, GetStringPopable);

        public static IPopable<string, object?> GetIntPopable(ISQLGetter getter) => new IntPopable(getter);

        public static IPopable<string, object?> GetStringPopable(ISQLGetter getter) => new StringPopable(getter);

        public static long? Add(in ISQLConnection connection, Func<IEntityCollectionUpdater<IParameter, long?>> func, IEntity entity, out uint tables, out ulong rows) => EntityCollection.Add(connection.GetParameter, func, entity, out tables, out rows);

        public static IEnumerable<T> GetItems<T, U>(in U collection, in EntityCollectionLoadingFactory<T, U> loadingFactory, in Type? itemsType = null) where T : IEntity => EntityCollection.GetItems(collection, loadingFactory, itemsType);

        public static bool RefreshItem<T, U>(in DBEntityCollection<U> collection, in ISQLConnection connection, in T item, in IConditionGroup conditionGroup) where T : IEntity where U : IEntity => GetItems(collection, new EntityCollectionItemRefreshingFactory<T, DBEntityCollection<U>>(connection.GetConnection(), item, conditionGroup), item.GetType()).FirstOrDefault() != null;
    }

    public class DBEntityCollection<T> : EntityCollection<T, DBEntityCollection<T>> where T : IEntity
    {
        private ISQLConnection? _connection;

        protected internal ISQLConnection? Connection => IsDisposed ? throw ThrowHelper.GetExceptionForDispose(false) : _connection;

        public override bool IsDisposed => _connection == null;

        public DBEntityCollection(ISQLConnection connection) => _connection = connection;

        public override long? Add(T entity, out uint tables, out ulong rows, IReadOnlyDictionary<string, object>? extraColumns = null) => IsDisposed ? throw ThrowHelper.GetExceptionForDispose(false) : EntityCollection.Add(_connection.GetParameter, () => new DBEntityCollectionAdder(_connection), entity, out tables, out rows, extraColumns);

        public override IEnumerable<T> GetItems() => IsDisposed ? throw ThrowHelper.GetExceptionForDispose(false) : DBEntityCollection.GetItems(this, new EntityCollectionNewItemLoadingFactory<T, DBEntityCollection<T>>(_connection, null));

        public IEnumerator<T> GetEnumerator() => GetItems().GetEnumerator();

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _connection.Dispose();

                _connection = null;
            }

            base.Dispose(disposing);
        }
    }
}
