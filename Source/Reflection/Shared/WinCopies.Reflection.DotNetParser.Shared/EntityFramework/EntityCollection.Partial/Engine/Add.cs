using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using WinCopies.Linq;

namespace WinCopies.EntityFramework
{
    public static partial class EntityCollection
    {
        public static TResult
#if CS9
            ?
#endif
            Add<TParameter, TResult>(Func<string, object
#if CS8
            ?
#endif
            , TParameter> parameterFunc, Func<IEntityCollectionUpdater<TParameter, TResult>> updaterProvider, IEntity entity, out uint tables, out ulong rows, IReadOnlyDictionary<string, object>
#if CS8
            ?
#endif
            extraColumns = null)
        {
            uint _tables = 0;
            ulong _rows = 0;

            IEntity
#if CS9
            ?
#endif
            tmp = null;
            EntityAttribute
#if CS8
            ?
#endif
            attribute;
            Type
#if CS8
            ?
#endif
            t = null;
            IEntityCollectionUpdater<TParameter, TResult>
#if CS8
            ?
#endif
            updater = null;
            TResult
#if CS9
            ?
#endif
            result = default;
            object
#if CS8
            ?
#endif
            value = null;
            PropertyInfo
#if CS8
            ?
#endif
            idProperty = null;
            object
#if CS8
                ?
#endif
                id = null;
            OneToManyForeignKeyAttribute oneToManyForeignKeyAttribute;
            ActionIn<KeyValuePair<PropertyInfo, EntityPropertyAttribute>, IEntity>
#if CS8
            ?
#endif
            action = null;
            IDictionary<string, IEnumerable<IEntity>> _extraColumns;
            ActionIn<string> oneToManyForeignKeysUpdater = null;
            Func<ActionIn<string>> oneToManyForeignKeysUpdaterUpdater;
            IEnumerable<IEntity>
#if CS8
                ?
#endif
                entities = null;
            KeyValuePair<string
#if CS8
                ?
#endif
                , PropertyInfo>? idPropertyInfo;
            object
#if CS8
                ?
#endif
                _result;
            uint __rows;
            ActionIn<string> _enumerate;

            void addValue(in string column, in bool isId) => updater.AddValue(column, parameterFunc(column.FirstCharToLower(), value is IEntity ? GetIdProperties(value.GetType()).FirstOrDefaultValue(out KeyValuePair<string
#if CS8
            ?
#endif
            , PropertyInfo> _idProperty) ? _idProperty.Value.GetValue(value) : throw new InvalidOperationException($"{value.GetType()} does not have any ID property.") : value), isId);

            void _action(in KeyValuePair<PropertyInfo, EntityPropertyAttribute> property, in IEntity _entity)
            {
                value = property.Key.GetValue(_entity);

                bool isId = property.Value.IsId;

                if (isId)
                {
                    Nullable tryRefreshIdWhen = _entity.TryRefreshIdWhen;

                    if (tryRefreshIdWhen.HasValue && tryRefreshIdWhen.Value?.Equals(value) == true)
                    {
                        _entity.MarkForRefresh();

                        _entity.TryRefreshId(true);

                        value = property.Key.GetValue(_entity);
                    }
                }

                if ((oneToManyForeignKeyAttribute = property.Key.GetCustomAttributes(true).FirstOrDefault<OneToManyForeignKeyAttribute>()) == null)

                    addValue(property.Value.Column ?? property.Key.Name, isId);

                else if (!(value == null || (idProperty == null && (idProperty = GetIdProperties(t).FirstOrNull()?.Value) == null)))

                    _extraColumns.Add(oneToManyForeignKeyAttribute.ForeignKeyIdColumn, ((IDBEntityItemCollection)value).AsEnumerable().AsReadOnlyEnumerable());
            }

            void _addValues(in IEntity _entity)
            {
                Type entityPropertyAttribute = typeof(EntityPropertyAttribute);

                foreach (KeyValuePair<PropertyInfo, EntityPropertyAttribute> _property in GetDBPropertyInfo(t))

                    action(_property, _entity);
            }

            void addOrUpdate(in string foreignKeyIdColumn) => tmp.AddOrUpdate(new Dictionary<string, object>() { { foreignKeyIdColumn, result } });

            string getTable() => t.GetCustomAttributes<EntityAttribute>(true).FirstOrDefault()?.Table ?? t.Name;

            void updateOneToManyForeignKeysUpdater(in string foreignKeyIdColumn)
            {
                if ((idPropertyInfo = GetIdProperties(t = tmp.GetType()).FirstOrNull()).HasValue)
                {
                    idProperty = idPropertyInfo.Value.Value;

                    updater.Delete(getTable(), idPropertyInfo.Value.Key ?? idProperty.Name, foreignKeyIdColumn, _result, _id => !entities.Any(_entity => Equals(_id, idProperty.GetValue(_entity))));
                }

                (oneToManyForeignKeysUpdater = addOrUpdate)(foreignKeyIdColumn);
            }

            bool enumerate(in string key)
            {
                bool __result = true;

                foreach (IEntity __entity in entities)
                {
                    tmp = __entity;

                    oneToManyForeignKeysUpdater(key);

                    __result = false;
                }

                return __result;
            }

            void add(in IEntity _entity, in ActionIn<IEntity> addValues)
            {
                IEntity
#if CS8
                ?
#endif
                _tmp;

                _result = null;

                foreach (PropertyInfo property in GetEntityProperties(_entity.GetType()))

                    if ((_tmp = (IEntity
#if CS8
                ?
#endif
                )property.GetValue(_entity)) != null)

                    add(_tmp, _addValues);

                if ((attribute = (t = _entity.GetType()).GetCustomAttributes<EntityAttribute>(true).FirstOrDefault()) != null)
                {
                    updater = updaterProvider();

                    _extraColumns = new Dictionary<string, IEnumerable<IEntity>>();

                    action = (in KeyValuePair<PropertyInfo, EntityPropertyAttribute> property, in IEntity __entity) =>
                    {
                        if (property.Value.IsId /*&& property.Key.PropertyType.IsAssignableFrom<TResult>()*/)
                        {
                            idProperty = property.Key;

                            action = _action;
                        }

                        _action(property, __entity);
                    };

                    addValues(_entity);

                    _rows += __rows = updater.ExecuteRequest(attribute.Table ?? t.Name, out result);

                    if (__rows > 0)
                    {
                        if (result == null)
                        {
                            if (idProperty != null)

                                _result = idProperty.GetValue(_entity);
                        }

                        else

                            idProperty?.SetValue(_entity, _result = Convert.TryChangeType(result, idProperty.PropertyType));

                        if (_result == null)
                        {
                            oneToManyForeignKeysUpdaterUpdater = () => addOrUpdate;

                            _enumerate = (in string key) => enumerate(key);
                        }

                        else
                        {
                            oneToManyForeignKeysUpdaterUpdater = () => updateOneToManyForeignKeysUpdater;

                            _enumerate = (in string key) =>
                            {
                                if (enumerate(key))

                                    _ = updater.Delete(getTable(), key, _result);
                            };
                        }

                        foreach (KeyValuePair<string, IEnumerable<IEntity>> item in _extraColumns)
                        {
                            oneToManyForeignKeysUpdater = oneToManyForeignKeysUpdaterUpdater();

                            entities = item.Value;

                            _enumerate(item.Key);
                        }
                    }

                    _tables++;
                }
            }

            add(entity, (in IEntity _entity) =>
            {
                _addValues(_entity);

                if (extraColumns != null)

                    foreach (KeyValuePair<string, object> item in extraColumns)
                    {
                        value = item.Value;

                        addValue(item.Key, false);
                    }
            });

            tables = _tables;
            rows = _rows;

            return result;
        }
    }
}
