﻿using System;
using System.Collections.Generic;
using System.Linq;

using WinCopies.Collections;
using WinCopies.EntityFramework;
using WinCopies.Linq;

using static WinCopies.Collections.DotNetFix.LinkedList;
using static WinCopies.Data.SQL.SQLItemCollection;
using static WinCopies.ThrowHelper;

namespace WinCopies.Data.SQL
{
    public struct IntPopable : IPopable<string, object
#if CS8
            ?
#endif
            >
    {
        private readonly ISQLGetter _getter;

        public int CurrentIndex { get; private set; }

        public IntPopable(in ISQLGetter getter)
        {
            _getter = getter;

            CurrentIndex = 0;
        }

        public object Pop(string key) => _getter[CurrentIndex++];
    }

    public struct StringPopable : IPopable<string, object
#if CS8
            ?
#endif
            >
    {
        private readonly ISQLGetter _getter;

        public StringPopable(in ISQLGetter getter) => _getter = getter;

        public object Pop(string key) => _getter[key];
    }

    public static class DBEntityCollection
    {
#if !CS8
        public static bool HasConditions(this IConditionGroup conditionGroup)
        {
            bool checkCount(in IUIntCountable collection) => collection != null && collection.Count > 0;

            return checkCount((conditionGroup ?? throw GetArgumentNullException(nameof(conditionGroup))).Conditions) || checkCount(conditionGroup.ConditionGroups) || checkCount(conditionGroup.Selects);
        }
#endif

        public static IEnumerable<IParameter> GetParameters(this IConditionGroup conditionGroup)
        {
            if ((conditionGroup ?? throw GetArgumentNullException(nameof(ConditionGroup))).Conditions != null)
            {
                IEnumerable<IParameter> enumerate()
                {
                    IParameter
#if CS8
            ?
#endif
            parameter;

                    foreach (ICondition
#if CS8
            ?
#endif
            condition in conditionGroup.Conditions)
                    {
                        if ((parameter = condition?.InnerCondition.Parameter) == null)

                            continue;

                        yield return parameter;
                    }
                }

                foreach (IParameter parameter in enumerate())

                    yield return parameter;
            }

            if (conditionGroup.ConditionGroups != null)
            {
                IEnumerable<IParameter> parameters;

                foreach (IConditionGroup
#if CS8
            ?
#endif
            _conditionGroup in conditionGroup.ConditionGroups)
                {
                    if (_conditionGroup == null)

                        continue;

                    parameters = _conditionGroup.GetParameters();

                    foreach (IParameter parameter in parameters)

                        yield return parameter;
                }
            }

            if (conditionGroup.Selects != null)
            {
                IConditionGroup
#if CS8
            ?
#endif
            _conditionGroup;

                foreach (KeyValuePair<SQLColumn, ISelect> select in conditionGroup.Selects)
                {
                    _conditionGroup = select.Value.ConditionGroup;

                    if (_conditionGroup == null)

                        continue;

                    foreach (IParameter parameter in _conditionGroup)

                        yield return parameter;
                }
            }
        }

        public static ISelect GetSelect(this ISQLConnection connection, in IEnumerable<string> defaultTables, in IEnumerable<SQLColumn>
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
        {
            ThrowIfNull(connection, nameof(connection));

            ISelect select = connection.GetSelect(GetCollection(defaultTables), defaultColumns == null ? null : GetCollection(defaultColumns));

            IConditionGroup initConditionGroup() => select.ConditionGroup = new ConditionGroup(@operator);

            void initConditionGroup2(Action<IConditionGroup> action) => action(select.ConditionGroup ?? initConditionGroup());

            if (conditions != null)

                initConditionGroup().Conditions = GetLinkedList(conditions);

            if (conditionGroups != null)

                initConditionGroup2(conditionGroup => conditionGroup.ConditionGroups = GetLinkedList(conditionGroups));

            if (selects != null)

                initConditionGroup2(conditionGroup => conditionGroup.Selects = GetLinkedList(selects));

            return select;
        }

        public static ISQLTableRequest2 GetDelete(this ISQLConnection connection, in IEnumerable<string> defaultTables, in string
#if CS8
            ?
#endif
            @operator = null, in IEnumerable<ICondition>
#if CS8
            ?
#endif
            conditions = null)
        {
            ThrowIfNull(connection, nameof(connection));

            ISQLTableRequest2
#if CS8
            ?
#endif
            delete = connection.GetDelete(GetCollection(defaultTables));

            if (conditions != null)

                delete.ConditionGroup = new ConditionGroup(@operator) { Conditions = GetLinkedList(conditions) };

            return delete;
        }

        public static T Parse<T>(in ISQLGetter getter, in Func<ISQLGetter, T> intAction, in Func<ISQLGetter, T> stringAction) => getter.IsIntIndexable
                ? intAction(getter)
                : getter.IsStringIndexable
                ? stringAction(getter)
                : throw new InvalidOperationException("The given getter can not be accessed neither as int nor as string indexable.");

        public static IEnumerable<T> GetItems<T>(this ISelect select, Func<ISQLGetter, T> intAction, Func<ISQLGetter, T> stringAction)
        {
            foreach (ISQLGetter getter in select.ExecuteQuery(true))

                yield return Parse(getter, intAction, stringAction);
        }

        public static IEnumerable<IPopable<string, object
#if CS8
            ?
#endif
            >> GetPopablesI(this ISelect select, Func<ISQLGetter, IPopable<string, object
#if CS8
            ?
#endif
            >> intAction) => select.GetItems(intAction, GetStringPopable);

        public static IEnumerable<IPopable<string, object
#if CS8
            ?
#endif
            >> GetPopablesS(this ISelect select, Func<ISQLGetter, IPopable<string, object
#if CS8
            ?
#endif
            >> stringAction) => select.GetItems(GetIntPopable, stringAction);

        public static IEnumerable<IPopable<string, object
#if CS8
            ?
#endif
            >> GetPopables(this ISelect select) => select.GetItems(GetIntPopable, GetStringPopable);

        public static IPopable<string, object
#if CS8
            ?
#endif
            > GetIntPopable(ISQLGetter getter) => new IntPopable(getter);

        public static IPopable<string, object
#if CS8
            ?
#endif
            > GetStringPopable(ISQLGetter getter) => new StringPopable(getter);

        public static long? Add(in ISQLConnection connection, Func<IEntityCollectionUpdater<IParameter, long?>> func, IEntity entity, out uint tables, out ulong rows) => EntityCollection.Add(connection.GetParameter, func, entity, out tables, out rows);

        public static IEnumerable<T> GetItems<T, U>(in U collection, in EntityCollectionLoadingFactory<T, U> loadingFactory, in Type
#if CS8
            ?
#endif
            itemsType = null) where T : IEntity => EntityCollection.GetItems(collection, loadingFactory, itemsType);

        public static bool RefreshItem<T, U>(in DBEntityCollection<U> collection, in ISQLConnection connection, in T item, in IConditionGroup conditionGroup) where T : IEntity where U : IEntity => GetItems(collection, new EntityCollectionItemRefreshingFactory<T, DBEntityCollection<U>>(connection.GetConnection(), item, conditionGroup), item.GetType()).FirstOrDefault() != null;
    }

    public interface IDBEntityCollection<T> : IEntityCollection<T> where T : IEntity
    {
        IOrderByColumns OrderByColumns { get; set; }
    }

    public class DBEntityCollection<T> : EntityCollection<T, DBEntityCollection<T>>, IDBEntityCollection<T> where T : IEntity
    {
        private ISQLConnection
#if CS8
            ?
#endif
            _connection;

        protected internal ISQLConnection
#if CS8
            ?
#endif
            Connection => IsDisposed ? throw GetExceptionForDispose(false) : _connection;

        public IOrderByColumns OrderByColumns { get; set; }

        public override bool IsDisposed => _connection == null;

        public DBEntityCollection(ISQLConnection connection) => _connection = connection;

        public static ulong Remove(in IEnumerable<T> collection)
        {
            ulong rows = 0;

            foreach (T c in collection)

                rows += c.Remove();

            return rows;
        }

        public static ulong Remove(in IEnumerable<T> collection, in Func<T, bool> predicate) => Remove(collection.Where(predicate));

        public static ulong RemovePredicate(in IEnumerable<T> collection, in Predicate<T> predicate) => Remove(collection.WherePredicate(predicate));

        public static DBEntityCollection<T> GetCollection(in ISQLConnection connection) => new
#if !CS9
            DBEntityCollection<T>
#endif
            (connection);

        public static Func<DBEntityCollection<T>> GetCollectionFunc(ISQLConnection connection) => () => GetCollection(connection);

        public static ulong Remove(ISQLConnection connection, Converter<DBEntityCollection<T>, IEnumerable<T>> converter) => UtilHelpers.UsingIn2(GetCollectionFunc(connection), (in DBEntityCollection<T> collection) => Remove(converter(collection)));

        public static ulong Remove(ISQLConnection connection) => Remove(connection, Delegates.Self);

        public static ulong Remove(in ISQLConnection connection, Func<T, bool> predicate) => Remove(connection, collection => collection.Where(predicate));

        public static ulong RemovePredicate(in ISQLConnection connection, Predicate<T> predicate) => Remove(connection, collection => collection.WherePredicate(predicate));

        public override long? Add(T entity, out uint tables, out ulong rows, IReadOnlyDictionary<string, object>
#if CS8
            ?
#endif
            extraColumns = null) => IsDisposed ? throw GetExceptionForDispose(false) : EntityCollection.Add(_connection.GetParameter, () => new DBEntityCollectionAdder(_connection), entity, out tables, out rows, extraColumns);

        public override IEnumerable<T> GetItems(IOrderByColumns
#if CS8
            ?
#endif
            orderBy) => IsDisposed ? throw GetExceptionForDispose(false) : DBEntityCollection.GetItems(this, new EntityCollectionNewItemLoadingFactory<T, DBEntityCollection<T>>(_connection, GetValueOrThrowIfInvalidType<OrderByColumns>(orderBy, nameof(orderBy), true), null));

        public IEnumerator<T> GetEnumerator() => GetItems(OrderByColumns).GetEnumerator();

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)

                UtilHelpers.TryDispose2(ref _connection);

            base.Dispose(disposing);
        }
    }
}
