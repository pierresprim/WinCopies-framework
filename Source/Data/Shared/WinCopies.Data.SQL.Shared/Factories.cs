using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using WinCopies.EntityFramework;
using WinCopies.Linq;

using static WinCopies.EntityFramework.EntityCollection;

namespace WinCopies.Data.SQL
{

    public abstract class EntityCollectionLoadingFactory<T, U> : IEntityCollectionLoadingFactory<T, U, ICondition>
    {
        private ISelect
#if CS8
            ?
#endif
            _select;
        private IConditionGroup
#if CS8
            ?
#endif
            _conditionGroup;

        protected Func<ISQLGetter, IPopable<string, object
#if CS8
            ?
#endif
            >> ParseIntIndexableDelegate
        { get; private set; }

        protected ISQLConnection Connection { get; private set; }

        protected ISelect Select => _select ?? throw new InvalidOperationException("No select statement has been initialized.");

        public IConditionGroup ConditionGroup => _conditionGroup ?? throw new InvalidOperationException("No condition group has been initialized.");

        public uint ColumnsCount { get; private set; }

        public OrderByColumns? OrderBy { get; }

        public EntityCollectionLoadingFactory(in ISQLConnection connection, in OrderByColumns? orderBy, in IConditionGroup
#if CS8
            ?
#endif
            conditionGroup)
        {
            Connection = (connection ?? throw ThrowHelper.GetArgumentNullException(nameof(connection))).GetConnection();

            OrderBy = orderBy;

            _conditionGroup = conditionGroup;

            ResetDelegate();
        }

        public void ResetDelegate() => ParseIntIndexableDelegate = getter => ColumnsCount > int.MaxValue
            ? throw new InvalidOperationException("Too much columns.")
            : (ParseIntIndexableDelegate = _getter => new IntPopable(_getter))(getter);

        public void Reset()
        {
            ColumnsCount = 0;

            _select = null;
            _conditionGroup = null;

            ResetDelegate();
        }

        public ICondition GetCondition(string column, string param, object
#if CS8
            ?
#endif
            value) => Connection.GetCondition(column, param, value);

        public abstract T GetItem(U collection);

        public IEnumerable GetOnlyFirstItems(string column)
        {
            ISelect
#if CS8
            ?
#endif
            select = Select;

            foreach (IPopable<string, object
#if CS8
            ?
#endif
            >
#if CS8
            ?
#endif
            item in select.GetPopables())

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
            _select = (Connection = Connection.GetConnection()).GetSelect(UtilHelpers.GetArray(table), columns.Select(column => Connection.GetColumn(column)));

            _select.OrderBy = OrderBy;

            if (_conditionGroup != null)

                _select.ConditionGroup = _conditionGroup;
        }

        public void SetConditions(params ICondition[] conditions) => ConditionGroup.Conditions = SQLHelper.GetEnumerable(conditions);

        public IEnumerable<IPopable<string, object
#if CS8
            ?
#endif
            >> GetItems() => Select.GetItems(ParseIntIndexableDelegate, DBEntityCollection.GetStringPopable);

        public void OnColumnsLoaded(uint columnsCount) => ColumnsCount = columnsCount;

        public IEnumerator<IPopable<string, object
#if CS8
            ?
#endif
            >> GetEnumerator() => GetItems().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public object GetCollectionConstructorParameter() => Connection;
    }

    public class EntityCollectionNewItemLoadingFactory<T, U> : EntityCollectionLoadingFactory<T, U> where T : IEntity
    {
        public EntityCollectionNewItemLoadingFactory(in ISQLConnection connection, in OrderByColumns? orderBy, in IConditionGroup
#if CS8
            ?
#endif
            conditionGroup) : base(connection, orderBy, conditionGroup) { /* Left empty. */ }

        public override T GetItem(U collection) => GetDBEntity<T, U>(collection);
    }

    public class EntityCollectionItemRefreshingFactory<T, U> : EntityCollectionLoadingFactory<T, U>
    {
        private readonly T _item;

        public EntityCollectionItemRefreshingFactory(in ISQLConnection connection, in T item, in IConditionGroup
#if CS8
            ?
#endif
            conditionGroup) : base(connection, null, conditionGroup) => _item = item;

        public override T GetItem(U collection) => _item;
    }
}
