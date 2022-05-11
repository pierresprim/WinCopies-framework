using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.ThrowHelper;

namespace WinCopies.EntityFramework
{
    public interface IEntityCollection : IAsEnumerable<IEntity>, DotNetFix.IDisposable
    {
        long? Add(IEntity entity, out uint tables, out ulong rows, System.Collections.Generic.IReadOnlyDictionary<string, object>
#if CS8
                ?
#endif
                extraColumns);
    }

    public interface IEntityCollection<T> : IEntityCollection, IMultiTypeEnumerable<T, IEntity> where T : IEntity
    {
        long? Add(T entity, out uint tables, out ulong rows, System.Collections.Generic.IReadOnlyDictionary<string, object>
#if CS8
                ?
#endif
                extraColumns);

        System.Collections.Generic.IEnumerable<T> GetItems(IOrderByColumns orderBy);

#if CS8
        long? IEntityCollection.Add(IEntity entity, out uint tables, out ulong rows, System.Collections.Generic.IReadOnlyDictionary<string, object>
#if CS8
            ?
#endif
            extraColumns) => Add(entity is T _entity ? _entity : throw new InvalidArgumentException(nameof(entity)), out tables, out rows, extraColumns);
#endif
    }

    public abstract class EntityCollection<TItems, TCollection> : IEntityCollection<TItems> where TItems : IEntity where TCollection : IEntityCollection<TItems>
    {
        public abstract bool IsDisposed { get; }

        public EntityCollection() => _ = EntityCollection.ValidateConstructor<TItems, TCollection>();

#if !CS8
        System.Collections.Generic.IEnumerable<IEntity> IAsEnumerable<IEntity>.AsEnumerable() => this.As<TItems, IEntity>();
#endif

        public abstract long? Add(TItems entity, out uint tables, out ulong rows, System.Collections.Generic.IReadOnlyDictionary<string, object>
#if CS8
                ?
#endif
                extraColumns = null);

#if !CS8
        long? IEntityCollection.Add(IEntity entity, out uint tables, out ulong rows, System.Collections.Generic.IReadOnlyDictionary<string, object> extraColumns) => Add(entity is TItems _entity ? _entity : throw new InvalidArgumentException(nameof(entity)), out tables, out rows, extraColumns);
#endif

        public abstract System.Collections.Generic.IEnumerable<TItems> GetItems(IOrderByColumns
#if CS8
            ?
#endif
            orderBy = null);

        public System.Collections.Generic.IEnumerator<TItems> GetEnumerator() => GetItems().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected virtual void Dispose(bool disposing) { /* Left empty. */ }

        ~EntityCollection() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public interface IEntityIdRefresher : DotNetFix.IDisposable
    {
        Collections.IUIntCountable Columns { get; }

        void Add(string column, string paramName, object
#if CS8
                ?
#endif
                value);

        bool TryGetId(string table, string idColumn, out object
#if CS8
                ?
#endif
                id);
    }

    public interface IEntity
    {
        bool RefreshNeeded { get; }

        Nullable TryRefreshIdWhen { get; }

        bool TryRefreshId(IEntityIdRefresher refresher, PropertyInfo idProperty, System.Collections.Generic.IEnumerable<string> properties);

        bool TryRefreshId(IEntityIdRefresher refresher, PropertyInfo idProperty, params string[] properties)
#if CS8
            => TryRefreshId(refresher, idProperty, properties.AsEnumerable())
#endif
            ;

        bool TryRefreshId(IEntityIdRefresher refresher, string idProperty, System.Collections.Generic.IEnumerable<string> properties);

        bool TryRefreshId(IEntityIdRefresher refresher, string idProperty, params string[] properties)
#if CS8
            => TryRefreshId(refresher, idProperty, properties.AsEnumerable())
#endif
            ;

        bool TryRefreshId(IEntityIdRefresher refresher, bool thisType);

        bool TryRefreshId(bool thisType);

        bool Refresh();

        void AddOrUpdate(System.Collections.Generic.IReadOnlyDictionary<string, object>
#if CS8
                ?
#endif
                extraColumns);

        ulong Update();

        ulong Remove();

        ulong Update(out uint tables);

        ulong Remove(out uint tables);

        void MarkForRefresh();

        void BeginParse();

        void EndParse();
    }

    public interface IDBEntityItemCollection : IAsEnumerable<IEntity>
    {
        void Add(IEntity item);

        bool Remove(IEntity item);

        void Clear();
    }

    public interface IDBEntityItemCollection<T> : IDBEntityItemCollection, IMultiTypeEnumerable<T, IEntity> where T : IEntity
    {
        void Add(T item);

        bool Remove(T item);

        void Clear();

#if CS8
        void IDBEntityItemCollection.Add(IEntity item) => Add(item is T _item ? _item : throw GetInvalidTypeArgumentException(nameof(item)));

        bool IDBEntityItemCollection.Remove(IEntity item) => item is T _item && Remove(_item);
#endif
    }

    public class DBEntityItemCollection<TItems, TParent> : IDBEntityItemCollection<TItems> where TItems : IEntity
    {
        private readonly Collections.DotNetFix.Generic.LinkedList<TItems> _items = new
#if !CS9
            Collections.DotNetFix.Generic.LinkedList<TItems>
#endif
            ();

        protected ILinkedList<TItems> Items => _items;

        protected TParent Parent { get; }

        public DBEntityItemCollection(TParent parent) => Parent = parent
#if CS8
            ??
#else
            == null ?
#endif
            throw GetArgumentNullException(nameof(parent))
#if !CS8
            : parent
#endif
            ;

#if !CS8
        System.Collections.Generic.IEnumerable<IEntity> IAsEnumerable<IEntity>.AsEnumerable() => this.As<TItems, IEntity>();
#endif

        public void Add(TItems item) => _items.AddLast(item);

        public bool Remove(TItems item) => _items.Remove2(item) != null;

#if !CS8
        void IDBEntityItemCollection.Add(IEntity item) => Add(item is TItems _item ? _item : throw GetInvalidTypeArgumentException(nameof(item)));

        bool IDBEntityItemCollection.Remove(IEntity item) => item is TItems _item && Remove(_item);
#endif

        public void Clear() => _items.Clear();

        public System.Collections.Generic.IEnumerator<TItems> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EntityAttribute : Attribute
    {
        public string
#if CS8
                ?
#endif
                Table
        { get; }

        public EntityAttribute(string
#if CS8
                ?
#endif
                table = null) => Table = table;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EntityPropertyAttribute : Attribute
    {
        public bool IsId { get; set; }

        public bool IsPseudoId { get; set; }

        public string
#if CS8
                ?
#endif
                Column
        { get; }

        public EntityPropertyAttribute(string
#if CS8
                ?
#endif
                column = null) => Column = column;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ForeignKeyAttribute : Attribute
    {
        public bool RemoveAlso { get; set; }
    }

    public class OneToManyForeignKeyAttribute : ForeignKeyAttribute
    {
        public string Table { get; }

        public string IdColumn { get; }

        public string ForeignKeyIdColumn { get; }

        public Type Type { get; }

        public OneToManyForeignKeyAttribute(string table, string idColumn, string foreignKeyIdColumn, Type type)
        {
            Table = table;

            IdColumn = idColumn;

            ForeignKeyIdColumn = foreignKeyIdColumn;

            Type = type;
        }
    }

    public struct EntityParser : System.IDisposable
    {
        public IEntity Entity { get; }

        public EntityParser(in IEntity entity)
        {
            Entity = entity;

            entity.BeginParse();
        }

        public void Dispose() => Entity.EndParse();
    }

    public static class EntityHelper
    {
        private static bool? Equals(IEntity x, IEntity y, Type tx, Type ty, in bool forcePseudoIdsEqualityComparison)
        {
            bool comparePseudoIdsEquality()
            {
#if CS8
                static
#endif
                    System.Collections.Generic.IEnumerable<PropertyInfo> getProperties(in Type type) => type.GetProperties().Where(property => property.GetCustomAttributes(true).FirstOrDefault<EntityPropertyAttribute>()?.IsPseudoId == true);

                return getProperties(tx).All(p => Equals(p.GetValue(x), getProperties(ty).First().GetValue(y)));
            }

            if (forcePseudoIdsEqualityComparison)

                return comparePseudoIdsEquality();

            PropertyInfo
#if CS8
                    ?
#endif
                    getPropertyInfo(in Type type) => type.GetProperties().FirstOrDefault(property => property.GetCustomAttributes(true).FirstOrDefault<EntityPropertyAttribute>()?.IsId == true);

            PropertyInfo
#if CS8
                    ?
#endif
                    px = getPropertyInfo(tx);

            if (px == null)

                return comparePseudoIdsEquality();

            else
            {
                PropertyInfo
#if CS8
                    ?
#endif
                    py = getPropertyInfo(ty);

                if (py == null)

                    return comparePseudoIdsEquality();

                else
                {
                    bool? refreshResult;

                    bool refresh(IEntity entity, in PropertyInfo p)
                    {
                        bool _refresh()
                        {
                            entity.MarkForRefresh();

                            return entity.TryRefreshId(true);
                        }

                        refreshResult = entity.RefreshNeeded ? _refresh() : entity.TryRefreshIdWhen.HasValue ? !Equals(p.GetValue(entity), entity.TryRefreshIdWhen.Value) || _refresh() :
#if !CS9
                        (bool?)
#endif
                        null;

                        if (refreshResult.HasValue)
                        {
                            if (refreshResult.Value)

                                return true;

                            refreshResult = null;
                        }

                        else

                            refreshResult = comparePseudoIdsEquality();

                        return false;
                    }

                    return refresh(x, px) && refresh(y, py) ? Equals(px.GetValue(x), py.GetValue(y)) : refreshResult;
                }
            }
        }

        private static bool? Equals(in IEntity x, in IEntity y, in FuncIn<Type, Type, bool?> func)
        {
            Type tx = (x ?? throw GetArgumentNullException(nameof(x))).GetType();
            Type ty = (y ?? throw GetArgumentNullException(nameof(y))).GetType();

            return tx == ty ? func(tx, ty) : false;
        }

        private static bool? Equals(IEntity x, IEntity y, bool forcePseudoIdsEqualityComparison) => Equals(x, y, (in Type tx, in Type ty) => Equals(x, y, tx, ty, forcePseudoIdsEqualityComparison));

        public static bool? Equals(IEntity x, IEntity y) => Equals(x, y, (in Type tx, in Type ty) => Equals(x, y, tx, ty, false));

        public static bool ComparePseudoIdsEquality(in IEntity x, in IEntity y) => Equals(x, y, true).Value;

        public static bool? TryComparePseudoIdsEquality(IEntity x, IEntity y) => Equals(x, y, (in Type tx, in Type ty) => x.RefreshNeeded || y.RefreshNeeded ? null : Equals(x, y, tx, ty, true));
    }

    public abstract class Entity : IEntity
    {
        public bool IsParsing { get; private set; }

        public bool RefreshNeeded { get; protected set; }

        public virtual Nullable TryRefreshIdWhen { get; } = new Nullable();

        protected IEntityCollection Collection { get; }

        protected Entity(in IEntityCollection collection) => Collection = collection;

#if !CS8
        public bool TryRefreshId(IEntityIdRefresher refresher, PropertyInfo idProperty, params string[] properties) => TryRefreshId(refresher, idProperty, properties.AsEnumerable());

        public bool TryRefreshId(IEntityIdRefresher refresher, string idProperty, params string[] properties) => TryRefreshId(refresher, idProperty, properties.AsEnumerable());
#endif

        protected bool TryRefreshId(in IEntityIdRefresher refresher, in PropertyInfo idProperty, in System.Collections.Generic.IEnumerable<KeyValuePair<PropertyInfo, EntityPropertyAttribute>> properties)
        {
            if (!RefreshNeeded)

                return false;

            ThrowIfNull(refresher, nameof(refresher));
            ThrowIfNull(idProperty, nameof(idProperty));
            ThrowIfNull(properties, nameof(properties));

            string idColumn = (idProperty.GetCustomAttributes<EntityPropertyAttribute>(true).FirstOrDefault() ?? throw new ArgumentException($"{idProperty.Name} does not have any {nameof(EntityPropertyAttribute)}.")).Column ?? idProperty.Name;

            Type t = GetType();

            string table = (t.GetCustomAttributes<EntityAttribute>(true).FirstOrDefault() ?? throw new InvalidOperationException($"{t.Namespace}.{t.Name} does not have the {nameof(EntityAttribute)} attribute.")).Table ?? t.Name;

            if (!t.IsAssignableTo(idProperty.DeclaringType))

                throw new ArgumentException($"{t.Namespace}.{t.Name} is not assignable to the given id property {idProperty.Name}'s declaring type ({idProperty.DeclaringType?.Namespace}.{idProperty.DeclaringType?.Name}).");

            string column;
            PropertyInfo _propertyInfo;
            object
#if CS8
                ?
#endif
                value;
            KeyValuePair<string
#if CS8
                ?
#endif
                , PropertyInfo>? foreignKeyProperty;

            foreach (KeyValuePair<PropertyInfo, EntityPropertyAttribute> property in properties)
            {
                _propertyInfo = property.Key;

                refresher.Add(column = property.Value.Column ?? _propertyInfo.Name, column.FirstCharToLower(), (value = _propertyInfo.GetValue(this)) is IEntity && _propertyInfo.GetCustomAttributes<ForeignKeyAttribute>().FirstOrDefault() != null && (foreignKeyProperty = EntityCollection.GetIdProperties(value.GetType()).FirstOrNull()).HasValue ? foreignKeyProperty.Value.Value.GetValue(value) : value);
            }

            bool result;

            if (refresher.Columns.Count > 0u)
            {
                if (refresher.TryGetId(table, idColumn, out object
#if CS8
                    ?
#endif
                    id))

                    idProperty.SetValue(this, Convert.TryChangeType(id, idProperty.PropertyType));

                result = true;
            }

            else

                result = false;

            RefreshNeeded = false;

            return result;
        }

        public bool TryRefreshId(IEntityIdRefresher refresher, PropertyInfo idProperty, System.Collections.Generic.IEnumerable<string> properties) => TryRefreshId(refresher, idProperty, EntityCollection.GetDBPropertyInfo(GetType()).Where(property => properties.Contains(property.Key.Name)));

        public bool TryRefreshId(IEntityIdRefresher refresher, string idProperty, System.Collections.Generic.IEnumerable<string> properties)
        {
            if (!RefreshNeeded)

                return false;

            ThrowIfNull(refresher, nameof(refresher));
            ThrowIfNull(properties, nameof(properties));
            ThrowIfNullEmptyOrWhiteSpace(idProperty, nameof(idProperty));

            Type t = GetType();

            return TryRefreshId(refresher, (EntityCollection.GetIdProperties(t).FirstOrNull(property => property.Value.Name == idProperty) ?? throw new ArgumentException($"{nameof(idProperty)} is not found among the DB properties of {nameof(t.Name)}.")).Value, properties);
        }

        public bool TryRefreshId(IEntityIdRefresher refresher, bool thisType)
        {
            if (!RefreshNeeded)

                return false;

            ThrowIfNull(refresher, nameof(refresher));

            Type t = GetType();

            foreach (PropertyInfo
#if CS8
                ?
#endif
                property in EntityCollection.GetEntityProperties(t))

                _ = ((IEntity
#if CS8
                ?
#endif
                )property.GetValue(this))?.TryRefreshId(true);

            return thisType && TryRefreshId(refresher, (EntityCollection.GetIdProperties(t).FirstOrNull() ?? throw new InvalidOperationException($"{t.Namespace}.{t.Name} does not have any id property.")).Value, EntityCollection.GetDBPropertyInfo(t).Where(property => property.Value.IsPseudoId));
        }

        protected abstract IEntityIdRefresher GetRefresher();

        public bool TryRefreshId(bool thisType)
        {
            if (!RefreshNeeded)

                return false;

            using
#if !CS8
                (
#endif
                IEntityIdRefresher
#if CS8
                ?
#endif
                refresher = GetRefresher()
#if CS8
                ;
#else
                )
#endif

            return TryRefreshId(refresher, thisType);
        }

        protected T RefreshAndAccess<T>(Func<T> func)
        {
            ThrowIfNull(func, nameof(func));

            _ = Refresh();

            return func();
        }

        protected T TryRefreshAndAccess<T>(Func<T> func)
        {
            ThrowIfNull(func, nameof(func));

            if (RefreshNeeded)

                _ = Refresh();

            return func();
        }

        public abstract ulong Remove(out uint tables);

        public ulong Remove() => Remove(out _);

        protected void TryEndRefresh(bool endRefresh)
        {
            if (endRefresh)

                RefreshNeeded = false;
        }

        protected abstract bool RefreshOverride();

        public bool Refresh()
        {
            bool result;

            using (EntityParser entityParser = new
#if !CS9
                EntityParser
#endif
                (this))

                result = RefreshOverride();

            TryEndRefresh(result);

            return result;
        }

        public void AddOrUpdate(System.Collections.Generic.IReadOnlyDictionary<string, object>
#if CS8
                ?
#endif
                extraColumns = null)
        {
            if (Collection.Add(this, out _, out _, extraColumns) == null)

                _ = Update(out _);
        }

        protected abstract ulong UpdateOverride(out uint tables);

        public ulong Update(out uint tables)
        {
            ulong result = UpdateOverride(out tables);

            TryEndRefresh(result > 0);

            return result;
        }

        public ulong Update() => Update(out _);

        public void MarkForRefresh() => RefreshNeeded = true;

        public T TryRefreshAndGet<T>(ref T value)
        {
            if (IsParsing)

                return value;

            if (RefreshNeeded)

                _ = Refresh();

            return value;
        }

        public T RefreshAndGet<T>(ref T value)
        {
            if (IsParsing)

                return value;

            _ = Refresh();

            return value;
        }

        public void BeginParse() => IsParsing = IsParsing ? throw new InvalidOperationException("Parsing already started.") : true;

        public void EndParse() => IsParsing = IsParsing ? false : throw new InvalidOperationException("Not parsing currently.");
    }

    public interface IDefaultEntity<T> : IEntity
    {
        T Id { get; set; }
    }

    public abstract class DefaultEntity<T> : Entity, IDefaultEntity<T>
    {
        [EntityProperty(nameof(Id), IsId = true)]
        public T Id { get; set; }

        public virtual Nullable TryRefreshIdWhen { get; } = new Nullable(default(T));

        protected DefaultEntity(IEntityCollection collection) : base(collection) { /* Left empty. */ }
    }

    public enum OrderBy
    {
        Asc = 0,

        Desc = 1
    }

    public interface IOrderByColumns
    {
        System.Collections.Generic.IEnumerable<string> Columns { get; }

        OrderBy OrderBy { get; }
    }
}
