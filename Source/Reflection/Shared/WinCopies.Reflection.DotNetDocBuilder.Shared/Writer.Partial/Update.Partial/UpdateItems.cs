using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Extensions;
using WinCopies.Linq;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Util;

using static System.Reflection.GenericParameterAttributes;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        public struct UpdateItemsStruct
        {
            public DBEntityCollection<Type> TColl { get; }

            public DBEntityCollection<AccessModifier> AMColl { get; }

            public DBEntityCollection<Namespace> NColl { get; }

            public DBEntityCollection<TypeType> TTColl { get; }

            public DBEntityCollection<Class> CColl { get; }

            public UpdateItemsStruct(in DBEntityCollection<Type> tColl, in DBEntityCollection<AccessModifier> amColl, in DBEntityCollection<Namespace> nColl, in DBEntityCollection<TypeType> ttColl, in DBEntityCollection<Class> cColl)
            {
                TColl = tColl;
                AMColl = amColl;
                NColl = nColl;
                TTColl = ttColl;
                CColl = cColl;
            }
        }

        protected void UpdateItems<T, U>(in string name, bool checkGenericity, Predicate<T> itemPredicate, Action<T, U>
#if CS8
            ?
#endif
            onAdded, Action<T>
#if CS8
            ?
#endif
            onDeleting, Converter<T, Type> converter, in Func<IEnumerable<U>> func, in Func<U, Type, DBEntityCollection<T>, T> getItem, Action<U, T, DBEntityCollection<T>> update, Action<U, ulong> createFile, Action<UpdateItemsStruct>
#if CS8
            ?
#endif
            updateItemsStruct) where T : IDefaultEntity<ulong> where U : DotNetType
        {
            Logger($"Updating {name}s", true);

            ISQLConnection connection = GetConnection();

            DBEntityCollection<T> getCollection() => new
#if !CS9
                DBEntityCollection<T>     
#endif
                (connection);

            string wholeNamespace;
            uint i = 0;
            ulong rows = 0;
            Type type = null;
            uint tables = 0;
            using
#if !CS8
                    (
#endif
                var gtColl = new DBEntityCollection<GenericType>(Connection)
#if CS8
            ;
#else
                    )
                {
#endif

            #region Local Data Parsing
            Logger("Parsing local data.", true);

            long? id;

            using
#if !CS8
                        (
#endif
    var gtmColl = new DBEntityCollection<GenericTypeModifier>(Connection)
#if CS8
;
#else
                        )
#endif

            using
#if !CS8
                        (
#endif
                var tColl = new DBEntityCollection<Type>(Connection)
#if CS8
        ;
#else
                        )
#endif

            using
#if !CS8
                        (
#endif
                var nColl = new DBEntityCollection<Namespace>(Connection)
#if CS8
        ;
#else
                        )
#endif

            using
#if !CS8
                        (
#endif
                var ttColl = new DBEntityCollection<TypeType>(Connection)
#if CS8
        ;
#else
                        )
#endif

            using
#if !CS8
                        (
#endif
                var amColl = new DBEntityCollection<AccessModifier>(Connection)
#if CS8
        ;
#else
                        )
                    {
#endif

            void tryRefreshAccessModifierId() => type.AccessModifier.TryRefreshId(true);

            void setModifier(in GenericType _gt, in string _name)
            {
                _gt.Modifier = new GenericTypeModifier(gtmColl) { Name = _name };

                Logger($"Variance: {_name}.", null);
            }

            bool hasFlag(in System.Type _gt, in GenericParameterAttributes gpa) => _gt.GenericParameterAttributes.HasFlag(gpa);

            void _setModifier(in System.Type _gt, in GenericType __gt)
            {
                __gt.Name = _gt.Name;

                if (hasFlag(_gt, Contravariant))

                    setModifier(__gt, "in");

                else if (hasFlag(_gt, Covariant))

                    setModifier(__gt, "out");

                else
                {
                    __gt.Modifier = null;

                    Logger($"No variance.", null);
                }

                __gt.MarkForRefresh();
            }

            IReadOnlyList<System.Type> getGenericTypeParameters(in System.Type t) => t.GetRealGenericParameters();

            void updateGenericTypes(in Type _item, in U _i)
            {

                System.Type _type = _i.Type;

                void writeLine(in string
#if CS8
    ?
#endif
                param1 = null, in string
#if CS8
    ?
#endif
                param2 = null) => Logger($"{_type.Namespace}.{_type.Name} is {param1}generic.{param2}", null);

                if (_i.Type.ContainsRealGenericParameters())
                {
                    writeLine(param2: " Adding.");

                    GenericType _gt;

                    foreach (System.Type gt in getGenericTypeParameters(_i.Type))
                    {
                        Logger($"Adding {gt.Name}.", true);

                        _gt = new GenericType(gtColl) { Type = _item };

                        _setModifier(gt, _gt);

                        Logger($"Added {_gt.Name}. Id: {gtColl.Add(_gt, out tables, out rows)}. Rows: {rows}. Tables: {tables}.", false);
                    }
                }

                else

                    writeLine("not ");
            }

            ActionIn<T, U> _onAdded = onAdded == null ?
#if !CS9
                            (ActionIn<T, U>)(
#endif
                (in T _item, in U _i) => { /* Left empty. */ }
#if !CS9
                        )
#endif
            : (in T _item, in U _i) =>
            {
                onAdded(_item, _i);

                updateGenericTypes(converter(_item), _i);
            };

            void setWholeNamespace(in DotNetType item) => wholeNamespace = item.Type.Namespace;

            Collections.DotNetFix.Generic.IEnumerableQueue<System.Type> browsed = new Collections.DotNetFix.Generic.EnumerableQueue<System.Type>();
            int j;
            Type typeTmp;
            System.Type genericTypeParameter;
            IReadOnlyList<System.Type> genericTypeParameters;

            using
#if !CS8
                (
#endif
                var cColl = new DBEntityCollection<Class>(Connection)
#if CS8
                ;
#else
                )
            {
#endif
            updateItemsStruct?.Invoke(new UpdateItemsStruct(tColl, amColl, nColl, ttColl, cColl));

            using
#if !CS8
                            (
#endif
                DBEntityCollection<T> items = getCollection()
#if CS8
    ;
#else
                            )
#endif

            foreach (U item in func().Where(item => !browsed.Contains(item.Type)))
            {
                Logger($"{++i}: Processing {item}.", true, ConsoleColor.DarkYellow);

                browsed.Enqueue(item.Type);

                setWholeNamespace(item);

                /*
                 * This method will check if the current type must be added to the DB and, if not, will update the corresponding type that was found in the DB. The adding process is delegated to further code.
                 */
                bool process()
                {
                    foreach (T _item in items.WherePredicate(_item => itemPredicate(_item) && GetWholeNamespace((type = converter(_item)).Namespace.Id) == item.Type.Namespace && checkAll(type, item)))
                    {
                        Logger("Exists. Updating.", true, ConsoleColor.DarkGreen);

                        update(item, _item, items);

                        type.AccessModifier = new AccessModifier(amColl) { Name = item.Type.IsPublicType() ? "public" : "protected" };

                        tryRefreshAccessModifierId();

                        if (item.Type.ContainsRealGenericParameters())
                        {
                            Logger($"Updating generic type parameters for {type.Namespace.Name}.{type.Name}.", true);

                            genericTypeParameters = getGenericTypeParameters(item.Type);

                            j = 0;

                            foreach (GenericType gt in gtColl.GetItems(Connection.GetOrderByColumns(OrderBy.Asc, nameof(GenericType.Id))).Where(_gt =>

                            _gt.Type.Id == type.Id))
                            {
                                Logger($"Updating generic type parameter {(genericTypeParameter = genericTypeParameters[j++]).Name}.", true);

                                _setModifier(genericTypeParameter, gt);

                                Logger($"Updated {gt.Update(out tables)} rows in {tables} {nameof(tables)}. Id: {gt.Id}.", false);
                            }

                            if (j != item.Type.GetRealGenericTypeParameterLength())

                                updateGenericTypes(type, item);

                            Logger($"Updated generic type parameters for {type.Namespace.Name}.{type.Name}.", false);
                        }

                        Logger($"Updated {_item.Update(out uint _tables)} rows in {_tables} {nameof(tables)}. Id: {_item.Id}.", false);

                        return false;
                    }

                    return true;
                }

                /*
                 * Checking if the current type must be added to the DB. If no corresponding type was found in the DB (based on the type pseudo-ids), that means that the current type does not exist there.
                 */
                if (process())
                {
                    Logger("Does not exist. Adding.", true, ConsoleColor.DarkRed);

                    T _item = getItem(item, typeTmp = GetType(item.Type, name, tColl, amColl, nColl, ttColl), items);



                    UpdateParentType(item.Type, typeTmp, tColl, amColl, nColl, ttColl, cColl);



                    UpdateNamespace(typeTmp, wholeNamespace);

                    void _process()
                    {
                        tryRefreshAccessModifierId();

                        _ = typeTmp.Namespace.TryRefreshId(true);

                        _ = type.TypeType.TryRefreshId(true);
                    }

                    if (Equals(type = converter(_item), _item))

                        _process();

                    else
                    {
                        _item.MarkForRefresh();

                        _process();

                        _ = _item.TryRefreshId(false);
                    }

                    id = items.Add(_item, out tables, out rows);

                    if (id.HasValue)
                    {
                        Logger($"Added {rows} {nameof(rows)} in {tables} {nameof(tables)}. Id: {id.Value}", null);

                        _onAdded(_item, item);

                        Logger($"Creating files for {item.Type.Name}.", null);

                        ActionIn<System.Type> action = (in System.Type __type) =>
                        {
                            action = (in System.Type ___type) => CreateTypeFile(___type, (type = type.ParentType.Type).Id, GetClassData);

                            createFile(item, typeTmp.Id);
                        };

                        foreach (System.Type _type in Collections.Enumerable.GetNullCheckWhileEnumerableC(item.Type, _type => _type.DeclaringType))

                            action(_type);

                        Logger($"Files created for {item.Type.Name}.", false);
                    }

                    else

                        throw new InvalidOperationException($"Failed to add {item.Type.Name}.");
                }

                Logger($"Processed {item.Type.Name}.", false);
            }
#if !CS8
            }
#endif

            Logger("Parsing local data completed.", false);
            #endregion

            #region DB Parsing
            Logger("Parsing DB", true);

            i = 0;

            bool checkAll(in Type _type, in DotNetType item)
            {
                setWholeNamespace(item);

                return CheckTypeEquality2(_type, item, checkGenericity);
            }

            void updateRows(in T item) => rows = item.Remove(out tables);

            ActionIn<T> delete = onDeleting == null ?
#if !CS9
                            (ActionIn<T>)
#endif
                updateRows : (in T item) =>
                {
                    onDeleting(item);

                    Type t;

                    string logMessage = $" generic type parameters for {t = converter(item)}.";

                    Logger("Removing" + logMessage, true);

                    foreach (GenericType genericType in gtColl.Where(gt => gt.Type.Id == t.Id))

                        Logger($"Removed generic type parameter '{genericType.Name}'. Rows: {genericType.Remove(out tables)}. Tables: {tables}.", null);

                    Logger("Removed" + logMessage, false);

                    updateRows(item);
                };

            using
#if !CS8
                        (
#endif
                DBEntityCollection<T> _items = getCollection()
#if CS8
        ;
#else
                        )
                    {
#endif
            _items.OrderByColumns = connection.GetOrderByColumns(OrderBy.Desc, "Id");

            foreach (T item in _items.WherePredicate(item => itemPredicate(item)))
            {
                Logger($"{++i}: Searching {item} in all packages.", true, ConsoleColor.DarkYellow);

                type = converter(item);

                if (func().Any(_item => checkAll(type, _item)))

                    Logger($"{item} found. Not removed from DB.", null, ConsoleColor.DarkGreen);

                else
                {
                    Logger($"{item} not found in any package. Deleting.", true, ConsoleColor.DarkRed);

                    delete(item);

                    wholeNamespace = GetWholeNamespace(type.Namespace.Id);

                    if (rows > 0)
                    {
                        Logger($"Removed {rows} {nameof(rows)} in {tables} {nameof(tables)}.", null);

                        TryDeleteDirectory(GetWholePath(wholeNamespace), type);

                        Logger($"{item} successfully deleted.", false);
                    }

                    else

                        throw new InvalidOperationException($"Could not remove {wholeNamespace}.{item}.");
                }

                Logger($"Processing {item} completed.", false);
            }
#if !CS8
                    }
#endif

            Logger("Parsing DB completed.", false);
            #endregion
#if !CS8
                }
            }
#endif

            Logger($"Updating {name}s completed.", false);
        }
    }
}
