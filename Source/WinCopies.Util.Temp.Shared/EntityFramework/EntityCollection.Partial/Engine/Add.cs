using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using WinCopies.Linq;
using WinCopies.Temp;

namespace WinCopies.EntityFramework
{
    public static partial class EntityCollection
    {
        public static TResult? Add<TParameter, TResult>(Func<string, object?, TParameter> parameterFunc, Func<IEntityCollectionUpdater<TParameter, TResult>> updaterProvider, IEntity entity, out uint tables, out ulong rows, IReadOnlyDictionary<string, object>? extraColumns = null)
        {
            uint _tables = 0;
            ulong _rows = 0;

            IEntity? tmp = null;
            EntityAttribute? attribute;
            Type? t = null;
            IEntityCollectionUpdater<TParameter, TResult>? updater = null;
            TResult? result = default;
            object? value = null;
            PropertyInfo? idProperty = null;
            object? id = null;
            OneToManyForeignKeyAttribute oneToManyForeignKeyAttribute;
            ActionIn<KeyValuePair<PropertyInfo, EntityPropertyAttribute>, IEntity>? action = null;
            IDictionary<string, IEnumerable<IEntity>> _extraColumns;
            ActionIn<string> oneToManyForeignKeysUpdater = null;
            Func<ActionIn<string>> oneToManyForeignKeysUpdaterUpdater;
            IEnumerable<IEntity>? entities = null;
            KeyValuePair<string?, PropertyInfo>? idPropertyInfo;
            object? _result;
            uint __rows;
            ActionIn<string> _enumerate;

            void addValue(in string column, in bool isId) => updater.AddValue(column, parameterFunc(column.FirstCharToLower(), value is IEntity ? GetIdProperties(value.GetType()).FirstOrDefaultValue(out KeyValuePair<string?, PropertyInfo> idProperty) ? idProperty.Value.GetValue(value) : throw new InvalidOperationException($"{value.GetType()} does not have any ID property.") : value), isId);

            void _action(in KeyValuePair<PropertyInfo, EntityPropertyAttribute> property, in IEntity entity)
            {
                value = property.Key.GetValue(entity);

                if ((oneToManyForeignKeyAttribute = property.Key.GetCustomAttributes(true).FirstOrDefault<OneToManyForeignKeyAttribute>()) == null)

                    addValue(property.Value.Column ?? property.Key.Name, property.Value.IsId);

                else if (!(value == null || (idProperty == null && (idProperty = GetIdProperties(t).FirstOrNull()?.Value) == null)))

                    _extraColumns.Add(oneToManyForeignKeyAttribute.ForeignKeyIdColumn, ((IDBEntityItemCollection)value).AsEnumerable().AsReadOnlyEnumerable());
            }

            void _addValues(in IEntity _entity)
            {
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

                    updater.Delete(getTable(), idPropertyInfo.Value.Key ?? idProperty.Name, foreignKeyIdColumn, _result, id => !entities.Any(entity => Equals(id, idProperty.GetValue(entity))));
                }

                (oneToManyForeignKeysUpdater = addOrUpdate)(foreignKeyIdColumn);
            }

            bool enumerate(in string key)
            {
                bool result = true;

                foreach (IEntity __entity in entities)
                {
                    tmp = __entity;

                    oneToManyForeignKeysUpdater(key);

                    result = false;
                }

                return result;
            }

            void add(in IEntity _entity, in ActionIn<IEntity> addValues)
            {
                IEntity? tmp;

                _result = null;

                foreach (PropertyInfo property in GetEntityProperties(_entity.GetType()))

                    if ((tmp = (IEntity?)property.GetValue(_entity)) != null)

                        add(tmp, _addValues);

                if ((attribute = (t = _entity.GetType()).GetCustomAttributes<EntityAttribute>(true).FirstOrDefault()) != null)
                {
                    updater = updaterProvider();

                    _extraColumns = new Dictionary<string, IEnumerable<IEntity>>();

                    action = (in KeyValuePair<PropertyInfo, EntityPropertyAttribute> property, in IEntity _entity) =>
                    {
                        if (property.Value.IsId /*&& property.Key.PropertyType.IsAssignableFrom<TResult>()*/)
                        {
                            idProperty = property.Key;

                            action = _action;
                        }

                        _action(property, _entity);
                    };

                    addValues(_entity);

                    _rows += (__rows = updater.ExecuteRequest(attribute.Table ?? t.Name, out result));

                    if (__rows > 0)
                    {
                        if (result == null)
                        {
                            if (idProperty != null)

                                _result = idProperty.GetValue(_entity);
                        }

                        else

                            idProperty?.SetValue(_entity, _result = Convert.ChangeType(result, idProperty.PropertyType));

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
