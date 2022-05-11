using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using WinCopies.Linq;
using WinCopies.Util;

namespace WinCopies.EntityFramework
{
    public interface IEntityCollectionLoadingFactory<TItem, TCollection, TCondition> : IEnumerable<IPopable<string, object
#if CS8
                ?
#endif
                >>//, ICloneable
    {
        object GetCollectionConstructorParameter();

        TItem GetItem(TCollection collection);

        void InitSelector(string table, IEnumerable<string> columns);

        void InitConditionGroup(bool multiple);

        TCondition GetCondition(string column, string param, object
#if CS8
            ?
#endif
            value);

        void SetConditions(params TCondition[] conditions);

        IEnumerable GetOnlyFirstItems(string column);

        void OnColumnsLoaded(uint columnsCount);

        void Reset();
    }

    public static partial class EntityCollection
    {
        private static ConstructorInfo _GetConstructor<T>(in Type t) => t.AssertGetConstructor(typeof(T));

        public static void ValidateType(in Type t, in Type to, in string paramName)
        {
            if (!t.IsAssignableTo(to))

                throw new ArgumentException($"{paramName} must be assignable to {to.Name}.", paramName);
        }

        public static void ValidateType<T>(in Type t, in string paramName) => ValidateType(t, typeof(T), paramName);

        public static ConstructorInfo GetConstructor<T>(in Type t)
        {
            ValidateType<IEntity>(t, nameof(t));

            return _GetConstructor<T>(t);
        }

        public static ConstructorInfo GetConstructor<TEntity, TCollection>() => _GetConstructor<TCollection>(typeof(TEntity));

#if CS8
        [return: NotNull]
#endif
        private static ConstructorInfo _ValidateConstructor(in Type t, bool check, out ConstructorInfo
#if CS8
            ?
#endif
            collectionConstructor, Type
#if CS8
            ?
#endif
            to = null, bool leave = false, in bool checkEntityAttribute = true)
        {
            ConstructorInfo[] constructors = t.GetConstructors();
            ParameterInfo[]
#if CS8
                ?
#endif
                parameters;
            ConstructorInfo constructor;

            void assert(out ConstructorInfo
#if CS8
                ?
#endif
                _collectionConstructor)
            {
                foreach (ConstructorInfo _constructor in constructors)
                {
                    parameters = _constructor.GetParameters();

                    if (check ? (parameters != null && parameters.Length == 1 && !(parameters[0].IsIn || parameters[0].IsOut) && (to == null || parameters[0].ParameterType == to)) : (leave || parameters == null || parameters.Length == 0))
                    {
                        _collectionConstructor = to == null && !leave ? _ValidateConstructor(parameters[0].ParameterType, false, out _, null, true) : null;

                        constructor = _constructor;

                        return;
                    }
                }

                throw new InvalidOperationException("No constructor matches the required constraints.");
            }

            assert(out collectionConstructor);

            if (constructor.ContainsGenericParameters && to == null)

                throw new InvalidOperationException("The constructor of the given type contains generic parameters.");

            if (leave || !checkEntityAttribute)

                return constructor;

            foreach (CustomAttributeData
#if CS8
                ?
#endif
                attributeData in t.CustomAttributes)

                if (attributeData.AttributeType == typeof(EntityAttribute))

                    return constructor;

            throw new InvalidOperationException($"No {nameof(EntityAttribute)} found.");
        }

        public static ConstructorInfo ValidateConstructor<U>(in Type t, out ConstructorInfo
#if CS8
            ?
#endif
            collectionConstructor, in bool checkEntityAttribute = true)
        {
            ValidateType<U>(t, nameof(t));

            return _ValidateConstructor(t, true, out collectionConstructor, checkEntityAttribute: checkEntityAttribute);
        }

        public static ConstructorInfo ValidateConstructor<T, U>() => _ValidateConstructor(typeof(T), true, out _, typeof(U));

        public static TEntity GetDBEntity<TEntity, TCollection>(in TCollection value) where TEntity : IEntity => (TEntity)GetConstructor<TEntity, TCollection>().Invoke(new object
#if CS8
            ?
#endif
            [] { value });

        public static U GetValidatedDBEntity<U>(in Type t, in object value, in bool checkEntityAttribute = true) => t.IsAssignableTo<U>() ? (U)ValidateConstructor<U>(t, out ConstructorInfo
#if CS8
            ?
#endif
            collectionConstructor, checkEntityAttribute).Invoke(new object
#if CS8
            ?
#endif
            [] { checkEntityAttribute ? collectionConstructor.Invoke(UtilHelpers.GetArray(value)) : value }) : throw new ArgumentException($"Can not assign a value of {nameof(t)} to {nameof(U)}.");

        public static bool IsDBAttribute(object attribute) => attribute is EntityPropertyAttribute;

        public static IEnumerable<Predicate<PropertyInfo>> GetPropertyDefaultPredicates() => UtilHelpers.GetArray<Predicate<PropertyInfo>>(property => property.CanRead && property.CanWrite, CheckIndexParameters, CheckAccessors);

        public static IEnumerable<Predicate<PropertyInfo>> GetDBPropertyPredicates() => GetPropertyDefaultPredicates().Append(property => property.GetCustomAttributes(true).Any(IsDBAttribute));

        public static IEnumerable<Predicate<PropertyInfo>> GetEntityPropertyPredicates() => GetDBPropertyPredicates().Append(property => property.PropertyType.IsAssignableTo<IEntity>());

        private static bool RunPredicates(PropertyInfo property, in Func<IEnumerable<Predicate<PropertyInfo>>> predicates) => predicates().All(predicate => predicate(property));

        // TODO: Property search should filter only those that come from IEntity or derived interfaces.

        /*public static IEnumerable<PropertyInfo> GetAllProperties(in Type t) => t.GetProperties();

        public static IEnumerable<PropertyInfo> GetAllProperties<T>() => typeof(T).GetProperties();*/

        public static IEnumerable<PropertyInfo> GetProperties(in Type t, Func<IEnumerable<Predicate<PropertyInfo>>> predicates) => t.GetProperties().Where(property => RunPredicates(property, predicates));

        public static IEnumerable<PropertyInfo> GetProperties(in Type t) => GetProperties(t, GetPropertyDefaultPredicates);

        public static IEnumerable<PropertyInfo> GetProperties<T>() => GetProperties(typeof(T));

        public static IEnumerable<PropertyInfo> GetDBProperties(in Type t) => GetProperties(t, GetDBPropertyPredicates);

        public static IEnumerable<PropertyInfo> GetDBProperties<T>() => GetDBProperties(typeof(T));

        public static IEnumerable<PropertyInfo> GetEntityProperties(in Type t) => GetProperties(t, GetEntityPropertyPredicates);

        public static IEnumerable<PropertyInfo> GetEntityProperties<T>() => GetEntityProperties(typeof(T));

        public static IEnumerable<KeyValuePair<string
#if CS8
                ?
#endif
                , PropertyInfo>> GetIdProperties(Type t)
        {
            EntityPropertyAttribute
#if CS8
        ?
#endif
        attribute;

            foreach (PropertyInfo property in GetDBProperties(t))

                if ((attribute = property.GetCustomAttributes<EntityPropertyAttribute>(true).FirstOrDefault()) != null && attribute.IsId)

                    yield return new KeyValuePair<string
#if CS8
                ?
#endif
                , PropertyInfo>(attribute.Column, property);
        }

        public static IEnumerable<KeyValuePair<string
#if CS8
                ?
#endif
                , PropertyInfo>> GetIdProperties<T>() => GetIdProperties(typeof(T));

        public static IEnumerable<KeyValuePair<PropertyInfo, EntityPropertyAttribute>> GetDBPropertyInfo(Type t)
        {
            EntityPropertyAttribute
#if CS8
        ?
#endif
        attribute;

            foreach (PropertyInfo
#if CS8
                ?
#endif
                property in GetDBProperties(t))

                if ((attribute = property.GetCustomAttributes<EntityPropertyAttribute>(true).FirstOrDefault()) != null)

                    yield return new KeyValuePair<PropertyInfo, EntityPropertyAttribute>(property, attribute);
        }

        public static IEnumerable<KeyValuePair<PropertyInfo, EntityPropertyAttribute>> GetDBPropertyInfo<T>() => GetDBPropertyInfo(typeof(T));

        public static bool CheckIndexParameters(PropertyInfo property)
        {
            ParameterInfo[]
#if CS8
                ?
#endif
                indexParameters = property.GetIndexParameters();

            return indexParameters == null || indexParameters.Length == 0;
        }

        public static bool CheckAccessors(PropertyInfo property)
        {
            foreach (MethodInfo method in property.GetAccessors())

                if (!method.IsPublic)

                    return false;

            return true;
        }
    }
}
