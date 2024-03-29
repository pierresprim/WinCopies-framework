﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using WinCopies.Collections.Generic;
using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.ThrowHelper;

namespace WinCopies.EntityFramework
{
    public static partial class EntityCollection
    {
        // TODO: This method retrieves data using a request by object. A unique cross-table request should be more efficient.

        public static System.Collections.Generic.IEnumerable<TItem> GetItems<TItem, TCollection, TCondition>(TCollection collection, IEntityCollectionLoadingFactory<TItem, TCollection, TCondition> loadingFactory, Type itemsType = null) where TItem : IEntity
        {
            /*
             * Initialization
            */

            ThrowIfNull(collection, nameof(collection));
            ThrowIfNull(loadingFactory, nameof(loadingFactory));

            if (itemsType == null)

                itemsType = typeof(TItem);

            ArrayBuilder<KeyValuePair<PropertyInfo, string>> columns = new
#if !CS9
                ArrayBuilder<KeyValuePair<PropertyInfo, string>>
#endif
                ();

            foreach (KeyValuePair<PropertyInfo, EntityPropertyAttribute> property in GetDBPropertyInfo(itemsType))

                _ = columns.AddLast(new KeyValuePair<PropertyInfo, string>(property.Key, property.Value.Column ?? property.Key.Name));

            loadingFactory.OnColumnsLoaded(columns.Count);

            /*
             * Variables
            */

            TItem
#if CS9
            ?
#endif
            obj = default;
            IEntity
#if CS8
            ?
#endif
            tmp;
            object
#if CS8
            ?
#endif
            _tmp;
            ForeignKeyAttribute
#if CS8
            ?
#endif
            attribute;
            PropertyInfo column;
            PropertyInfo
#if CS8
            ?
#endif
            idProperty;
            KeyValuePair<string
#if CS8
            ?
#endif
            , PropertyInfo>? foreignKeyIdProperty;
            PropertyInfo _foreignKeyIdProperty;
            Type adderType;
            KeyValuePair<string
#if CS8
            ?
#endif
            , PropertyInfo>? adderTypeIdProperty;
            object
#if CS8
            ?
#endif
            value = null;

            /*
             * Methods
            */

            V getValidatedDBEntityObject<V>(in Type type, in object param, in bool checkEntityAttribute) => GetValidatedDBEntity<V>(type, param, checkEntityAttribute);

            IEntity getValidatedDBEntity(in Type type) => getValidatedDBEntityObject<IEntity>(type, loadingFactory.GetCollectionConstructorParameter(), true);

            IDBEntityItemCollection getValidatedDBEntityCollection() => getValidatedDBEntityObject<IDBEntityItemCollection>(column.PropertyType, obj, false);

            TCondition getCondition(in string columnName, in string paramName, in object
#if CS8
            ?
#endif
            _value) => loadingFactory.GetCondition(columnName, paramName, _value);

            TCondition getForeignKeyCondition(in OneToManyForeignKeyAttribute _attribute) => getCondition(_attribute.ForeignKeyIdColumn, "foreignKeyId", idProperty.GetValue(obj));

            /*
             * Main process
            */

            // TODO: Is*Indexable properties should also be accessible via the enumerable returned by ExecuteQuery().

            loadingFactory.InitSelector(itemsType.GetCustomAttributes<EntityAttribute>(true).First().Table ?? itemsType.Name, columns.AsEnumerable<KeyValuePair<PropertyInfo, string>>().WhereSelect(_column => !_column.Key.GetCustomAttributes(true).Any<OneToManyForeignKeyAttribute>(), _column => _column.Value));

            foreach (IPopable<string, object
#if CS8
            ?
#endif
            > items in loadingFactory)
            {
                obj = loadingFactory.GetItem(collection);

                foreach (KeyValuePair<PropertyInfo, string> _column in columns)
                {
                    void process()
                    {
                        void popValue() => value = items.Pop(_column.Value);

                        void setValue() =>
                        /*{
                                object getNewEntity()
                                {
                                    object e = getValidatedDBEntity(column.PropertyType);

                                    KeyValuePair<string
    #if CS8
                                        ?
    #endif
                                        , PropertyInfo>? p = GetIdProperties(column.PropertyType).FirstOrNull();

                                    if (p.HasValue)

                                        p.Value.Value.SetValue(e, value);

                                    return e;
                                }*/

                            column.SetValue(obj, (value == null || value == DBNull.Value) ? null : Convert.TryChangeType(value, column.PropertyType));
                        //}

                        if ((attribute = (column = _column.Key).GetCustomAttributes<ForeignKeyAttribute>().FirstOrDefault()) != null)

                            if (attribute is OneToManyForeignKeyAttribute _attribute)
                            {
                                if (column.PropertyType.IsAssignableTo<IDBEntityItemCollection>() && (adderTypeIdProperty = GetIdProperties(adderType = obj.GetType()).FirstOrNull()).HasValue && (foreignKeyIdProperty = GetIdProperties(_attribute.Type).FirstOrNull()).HasValue)
                                {
                                    idProperty = adderTypeIdProperty.Value.Value;

                                    _foreignKeyIdProperty = foreignKeyIdProperty.Value.Value;

                                    loadingFactory.InitSelector(_attribute.Table, UtilHelpers.GetArray(_attribute.IdColumn));

                                    if (column.GetValue(obj) is IDBEntityItemCollection tmpCollection)
                                    {
                                        loadingFactory.InitConditionGroup(true);

                                        bool mustBeRemoved(in IEntity entity)
                                        {
                                            loadingFactory.SetConditions(getForeignKeyCondition(_attribute), getCondition(_attribute.IdColumn, "id", idProperty.GetValue(entity)));

                                            foreach (IEnumerable
#if CS8
                                                ?
#endif
                                                _value in loadingFactory)

                                                return false;

                                            return true;
                                        }

                                        foreach (IEntity entity in tmpCollection.AsEnumerable())

                                            if (mustBeRemoved(entity))

                                                _ = tmpCollection.Remove(entity);
                                    }

                                    else

                                        _column.Key.SetValue(obj, tmpCollection = getValidatedDBEntityCollection());

                                    loadingFactory.InitConditionGroup(false);

                                    loadingFactory.SetConditions(getForeignKeyCondition(_attribute));

                                    loadingFactory.InitSelector(_attribute.Table, UtilHelpers.GetArray(_attribute.IdColumn));

                                    foreach (object id in loadingFactory.GetOnlyFirstItems(_attribute.IdColumn))
                                    {
                                        if ((tmp = tmpCollection.AsEnumerable().FirstOrDefault(entity => Equals(_foreignKeyIdProperty.GetValue(entity), id))) == null)
                                        {
                                            _foreignKeyIdProperty.SetValue(tmp = getValidatedDBEntity(_attribute.Type), id);

                                            tmpCollection.Add(tmp);
                                        }

                                        tmp.MarkForRefresh();
                                    }
                                }

                                return;
                            }

                            else if ((adderTypeIdProperty = GetIdProperties(column.PropertyType).FirstOrDefault()) != null && (idProperty = adderTypeIdProperty.Value.Value).CanRead && idProperty.CanWrite)
                            {
                                popValue();

                                if (value == null || value == DBNull.Value)

                                    tmp = null;

                                else
                                {
                                    tmp = (_tmp = column.GetValue(obj)) == null
                                        ? getValidatedDBEntity(column.PropertyType)
                                        : (IEntity)_tmp;

                                    idProperty.SetValue(tmp, Convert.TryChangeType(value, idProperty.PropertyType));

                                    tmp.MarkForRefresh();
                                }

                                value = tmp;

                                setValue();

                                return;
                            }

                        popValue();

                        setValue();
                    }

                    process();
                }

                yield return obj;
            }

            loadingFactory.Reset();
        }
    }
}
