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
            EntityAttribute? attribute = t.GetCustomAttributes<EntityAttribute>(true).FirstOrDefault();

            if (attribute != null)
            {
                bool defaultPredicate(in PropertyInfo property, in Predicate predicate) => property.GetCustomAttributes(true).AnyPredicate(predicate);

                IEnumerable<PropertyInfo> getProperties(in FuncIn<Type, IEnumerable<PropertyInfo>> func, in Predicate<PropertyInfo> predicate) => func(t).WherePredicate(predicate);

                IEntity? tmp;
                object? collection;

                foreach (PropertyInfo property in getProperties(GetDBProperties, property => property.PropertyType.IsAssignableTo<IDBEntityItemCollection>() && defaultPredicate(property, attribute => attribute is OneToManyForeignKeyAttribute oneToManyForeignKeyAttribute)))

                    if ((collection = property.GetValue(entity)) != null)

                        foreach (IEntity? item in ((IDBEntityItemCollection)collection).AsEnumerable())

                            _ = item.Remove(out _);

                if ((rows = func(attribute.Table ?? t.Name)) > 0)
                {
                    tables++;

                    foreach (PropertyInfo property in getProperties(GetEntityProperties, property => defaultPredicate(property, attribute => attribute is not OneToManyForeignKeyAttribute && attribute is ForeignKeyAttribute foreignKeyAttribute && foreignKeyAttribute.RemoveAlso)))

                        if ((tmp = (IEntity?)property.GetValue(entity)) != null)
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
