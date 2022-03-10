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
        public static ulong RemoveItem(in IEntity entity, in Func<string, ulong> func, out uint tables)
        {
            tables = 0;
            ulong rows = 0;

            Type t = entity.GetType();
            EntityAttribute
#if CS8
            ?
#endif
            attribute = t.GetCustomAttributes<EntityAttribute>(true).FirstOrDefault();

            if (attribute != null)
            {
                bool defaultPredicate(in PropertyInfo property, in Predicate predicate) => property.GetCustomAttributes(true).AnyPredicate(predicate);

                IEnumerable<PropertyInfo> getProperties(in FuncIn<Type, IEnumerable<PropertyInfo>> _func, in Predicate<PropertyInfo> predicate) => _func(t).WherePredicate(predicate);

                IEntity
#if CS8
            ?
#endif
            tmp;
                object
#if CS8
            ?
#endif
            collection;

                foreach (PropertyInfo property in getProperties(GetDBProperties, property => property.PropertyType.IsAssignableTo<IDBEntityItemCollection>() && defaultPredicate(property, _attribute => _attribute is OneToManyForeignKeyAttribute oneToManyForeignKeyAttribute)))

                    if ((collection = property.GetValue(entity)) != null)

                        foreach (IEntity
#if CS8
            ?
#endif
            item in ((IDBEntityItemCollection)collection).AsEnumerable())

                            _ = item.Remove(out _);

                if ((rows = func(attribute.Table ?? t.Name)) > 0)
                {
                    tables++;

                    foreach (PropertyInfo property in getProperties(GetEntityProperties, property => defaultPredicate(property, _attribute =>
#if !CS9
                    !(
#endif
                        _attribute is
#if CS9
                        not
#endif
                        OneToManyForeignKeyAttribute
#if !CS9
                    )
#endif
                    && _attribute is ForeignKeyAttribute foreignKeyAttribute && foreignKeyAttribute.RemoveAlso)))

                        if ((tmp = (IEntity
#if CS8
            ?
#endif
            )property.GetValue(entity)) != null)
                        {
                            rows += tmp.Remove(out uint _tables);

                            tables += _tables;
                        }

                    entity.Dispose();
                }
            }

            return rows;
        }
    }
}
