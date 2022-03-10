using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Linq;
using WinCopies.Temp;

using static WinCopies.ThrowHelper;

namespace WinCopies.EntityFramework
{
    public interface IEntityCollection : IAsEnumerable<IEntity>, DotNetFix.IDisposable
    {
        long? Add(IEntity entity, out uint tables, out ulong rows, IReadOnlyDictionary<string, object>
#if CS8
                ?
#endif
                extraColumns);
    }

    public interface IEntityCollection<T> : IEntityCollection, IMultiTypeEnumerable<T, IEntity> where T : IEntity
    {
        long? Add(T entity, out uint tables, out ulong rows, IReadOnlyDictionary<string, object>
#if CS8
                ?
#endif
                extraColumns);

#if CS8
        long? IEntityCollection.Add(IEntity entity, out uint tables, out ulong rows, IReadOnlyDictionary<string, object>? extraColumns) => Add(entity is T _entity ? _entity : throw new InvalidArgumentException(nameof(entity)), out tables, out rows, extraColumns);
#endif
    }

    public abstract class EntityCollection<TItems, TCollection> : IEntityCollection<TItems> where TItems : IEntity where TCollection : IEntityCollection<TItems>
    {
        public abstract bool IsDisposed { get; }

        public EntityCollection() => _ = EntityCollection.ValidateConstructor<TItems, TCollection>();

#if !CS8
        IEnumerable<IEntity> IAsEnumerable<IEntity>.AsEnumerable() => this.As<TItems, IEntity>();
#endif

        public abstract long? Add(TItems entity, out uint tables, out ulong rows, IReadOnlyDictionary<string, object>
#if CS8
                ?
#endif
                extraColumns = null);

#if !CS8
        long? IEntityCollection.Add(IEntity entity, out uint tables, out ulong rows, IReadOnlyDictionary<string, object> extraColumns) => Add(entity is TItems _entity ? _entity : throw new InvalidArgumentException(nameof(entity)), out tables, out rows, extraColumns);
#endif

        public abstract System.Collections.Generic.IEnumerable<TItems> GetItems();

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

    public interface IEntity : System.IDisposable
    {
        bool RefreshNeeded { get; }

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

        void AddOrUpdate(IReadOnlyDictionary<string, object>
#if CS8
                ?
#endif
                extraColumns);

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
        IEnumerable<IEntity> IAsEnumerable<IEntity>.AsEnumerable() => this.As<TItems, IEntity>();
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

    public abstract class Entity : IEntity
    {
        public bool IsParsing { get; private set; }

        public bool RefreshNeeded { get; protected set; }

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
            PropertyInfo
#if CS8
                ?
#endif
                foreignKeyProperty;

            foreach (KeyValuePair<PropertyInfo, EntityPropertyAttribute> property in properties)
            {
                _propertyInfo = property.Key;

                refresher.Add(column = property.Value.Column ?? _propertyInfo.Name, column.FirstCharToLower(), (value = _propertyInfo.GetValue(this)) is IEntity && !(_propertyInfo.GetCustomAttributes<ForeignKeyAttribute>().FirstOrDefault() == null || (foreignKeyProperty = EntityCollection.GetEntityProperties(value.GetType()).FirstOrDefault()) == null) ? foreignKeyProperty.GetValue(value) : value);
            }

            if (refresher.TryGetId(table, idColumn, out object
#if CS8
                ?
#endif
                id))

                idProperty.SetValue(this, Convert.ChangeType(id, idProperty.PropertyType));

            RefreshNeeded = false;

            return true;
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

        public void AddOrUpdate(IReadOnlyDictionary<string, object>
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

        protected virtual void Dispose(bool disposing) { /* Left empty. */ }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }

    public abstract class DefaultEntity : Entity
    {
        [EntityProperty(nameof(Id), IsId = true)]
        public int Id { get; set; }

        protected DefaultEntity(IEntityCollection collection) : base(collection) { /* Left empty. */ }
    }
}
