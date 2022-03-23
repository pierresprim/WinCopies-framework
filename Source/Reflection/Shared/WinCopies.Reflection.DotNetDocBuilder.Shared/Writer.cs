using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Linq;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Temp;

using static System.Reflection.GenericParameterAttributes;

using static WinCopies.Data.SQL.SQLHelper;
using static WinCopies.Reflection.DotNetDocBuilder.WriterConsts;
using static WinCopies.Temp.Util;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    [Entity("docnamespace")]
    public class Namespace : DefaultDBEntity<Namespace>
    {
        private int _frameworkId;
        private string _name;
        private int? _parentId;

        [EntityProperty]
        public int FrameworkId { get => TryRefreshAndGet(ref _frameworkId); set => _frameworkId = value; }

        [EntityProperty(IsPseudoId = true)]
        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        [EntityProperty(IsPseudoId = true)]
        public int? ParentId { get => TryRefreshAndGet(ref _parentId); set => _parentId = value; }

        public Namespace(DBEntityCollection<Namespace> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("typetype")]
    public class TypeType : DefaultDBEntity<TypeType>
    {
        private string _name;

        [EntityProperty(IsPseudoId = true)]
        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        public TypeType(DBEntityCollection<TypeType> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("class_access_modifier")]
    public class AccessModifier : DefaultDBEntity<AccessModifier>
    {
        private string _name;

        [EntityProperty(IsPseudoId = true)]
        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        public AccessModifier(DBEntityCollection<AccessModifier> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("doctype")]
    public class Type : DefaultDBEntity<Type>
    {
        private string _name;
        private Namespace _namespace;
        private TypeType _typeType;
        private AccessModifier _accessModifier;
        private byte? _genericTypeCount;

        [EntityProperty(IsPseudoId = true)]
        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        [EntityProperty(
#if CS10
            $"{nameof(Namespace)}Id"
#else
            "NamespaceId"
#endif
            , IsPseudoId = true)]
        [ForeignKey]
        public Namespace Namespace { get => TryRefreshAndGet(ref _namespace); set => _namespace = value; }

        [EntityProperty(
#if CS10
            $"{nameof(TypeType)}Id"
#else
            "TypeTypeId"
#endif
            )]
        [ForeignKey]
        public TypeType TypeType { get => TryRefreshAndGet(ref _typeType); set => _typeType = value; }

        [EntityProperty(nameof(AccessModifier))]
        [ForeignKey]
        public AccessModifier AccessModifier { get => TryRefreshAndGet(ref _accessModifier); set => _accessModifier = value; }

        [EntityProperty(nameof(GenericTypeCount), IsPseudoId = true)]
        public byte? GenericTypeCount { get => TryRefreshAndGet(ref _genericTypeCount); set => _genericTypeCount = value; }

        public Type(DBEntityCollection<Type> collection) : base(collection) { /* Left empty. */ }

        public override string ToString() => Name;
    }

    [Entity("enumunderlyingtype")]
    public class UnderlyingType : DefaultDBEntity<UnderlyingType>
    {
        private string _name;

        [EntityProperty(IsPseudoId = true)]
        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        public UnderlyingType(DBEntityCollection<UnderlyingType> collection) : base(collection) { /* Left empty. */ }
    }

    public class TypeBase<T> : DefaultDBEntity<T> where T : IEntity
    {
        private Type _type;

        [EntityProperty(
#if CS10
            $"{nameof(Type)}Id"
#else
            "TypeId"
#endif
            )]
        [ForeignKey(RemoveAlso = true)]
        public Type Type { get => TryRefreshAndGet(ref _type); set => _type = value; }

        public TypeBase(DBEntityCollection<T> collection) : base(collection) { /* Left empty. */ }

        public override string ToString() => Type.ToString();
    }

    [Entity("docenum")]
    public class Enum : TypeBase<Enum>
    {
        private UnderlyingType _underlyingType;

        [EntityProperty]
        [ForeignKey]
        public UnderlyingType UnderlyingType { get => TryRefreshAndGet(ref _underlyingType); set => _underlyingType = value; }

        public Enum(DBEntityCollection<Enum> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("interfaceinterfaceimplementations")]
    public class InterfaceImplementation : DefaultDBEntity<InterfaceImplementation>
    {
        private Type _interface;
        private Type _implementedInterface;

        [EntityProperty]
        [ForeignKey]
        public Type Interface { get => TryRefreshAndGet(ref _interface); set => _interface = value; }

        [EntityProperty]
        [ForeignKey]
        public Type ImplementedInterface { get => TryRefreshAndGet(ref _implementedInterface); set => _implementedInterface = value; }

        public InterfaceImplementation(DBEntityCollection<InterfaceImplementation> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docgenerictypemodifier")]
    public class GenericTypeModifier : DefaultDBEntity<GenericTypeModifier>
    {
        private string _name;

        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        public GenericTypeModifier(DBEntityCollection<GenericTypeModifier> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docgenerictype")]
    public class GenericType : DefaultDBEntity<GenericType>
    {
        private string _name;
        private Type _type;
        private GenericTypeModifier _modifier;

        [EntityProperty]
        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        [EntityProperty("doctype")]
        [ForeignKey]
        public Type Type { get => TryRefreshAndGet(ref _type); set => _type = value; }

        [EntityProperty]
        [ForeignKey]
        public GenericTypeModifier Modifier { get => TryRefreshAndGet(ref _modifier); set => _modifier = value; }

        public GenericType(DBEntityCollection<GenericType> collection) : base(collection) { /* Left empty. */ }
    }

    public abstract class Writer : IWriter
    {
        private byte _tabsCount = 0;

        #region Properties
        protected ISQLConnection Connection { get; }

        public IEnumerable<DotNetPackage> Packages { get; }

        public IEnumerable<string> ValidPackages { get; }

        public string DBName { get; }

        public PackageInfo PackageInfo { get; }

        public string RootPath { get; }
        #endregion Properties

        public Writer(in ISQLConnection connection, in string dbName, in string rootPath, in IEnumerable<DotNetPackage> packages, in IEnumerable<string> validPackages, in PackageInfo packageInfo)
        {
            Connection = connection;

            DBName = dbName;

            RootPath = rootPath;

            Packages = packages;

            ValidPackages = validPackages;

            PackageInfo = packageInfo;
        }

        #region Methods
        protected void WriteLine(in string
#if CS8
            ?
#endif
            msg, in bool? increment, in ConsoleColor? color = null)
        {
            if (color.HasValue)

                Console.ForegroundColor = color.Value;

            Console.WriteLine(new string('\t', increment.HasValue ? increment.Value ? ++_tabsCount : _tabsCount-- : _tabsCount) + msg);

            Console.ResetColor();
        }

        protected ISQLConnection GetConnection() => Connection.GetConnection();

        public IEnumerable<DotNetNamespace> GetNamespaces(IEnumerable<DotNetNamespace> parent)
        {
            foreach (DotNetNamespace @namespace in parent)
            {
                yield return @namespace;

                foreach (DotNetNamespace _namespace in GetNamespaces(@namespace.GetSubnamespaces()))

                    yield return _namespace;
            }
        }

        public IEnumerable<DotNetNamespace> GetAllNamespacesInPackages()
        {
            foreach (DotNetPackage
#if CS8
                ?
#endif
                package in Packages)

                foreach (DotNetNamespace
#if CS8
                ?
#endif
                @namespace in GetNamespaces(package))

                    yield return @namespace;
        }

        public void DeleteTypesBase<T>(string wholeNamespace) where T : TypeBase<T>
        {
            string typeTypeName = typeof(T).Name;

            void writeLine(in string
#if CS8
                ?
#endif
                value = null) => WriteLine($"Deleting all {typeTypeName}s from {wholeNamespace}{value}", value == null);

            writeLine();

            using
#if !CS9
#if !CS8
                (
#endif
                var types = new
#endif
                DBEntityCollection<T>
#if CS9
                types = new
#endif
                (GetConnection())
#if CS8
                ;
#else
                )
            {
#endif

            ulong rows = 0;

            string wholePath = GetWholePath(wholeNamespace);

            foreach (T type in types.Where(_type => GetWholeNamespace(_type.Type.Namespace.Id) == wholeNamespace))
            {
                WriteLine($"Removing {type}.", true);

                rows = type.Remove(out uint tables);

                if (rows > 0)
                {
                    WriteLine($"Removed {rows} {nameof(rows)} in {tables} {nameof(tables)}.", null);

                    Directory.Delete(Path.Combine(wholePath, type.Type.Name), true);

                    WriteLine($"Removing {type} completed.", false);
                }

                else

                    throw new InvalidOperationException($"Could not remove {wholeNamespace}.{type}.");
            }
#if !CS8
            }
#endif

            writeLine(" completed");
        }

        public void DeleteTypes(string wholeNamespace)
        {
            void writeLine(in string
#if CS8
            ?
#endif
            value = null) => WriteLine($"Deleting all types from {wholeNamespace}{value}.", value == null);

            writeLine();

            DeleteTypesBase<Enum>(wholeNamespace);

            writeLine(" completed");
        }

        protected static string GetNamespacePath(in string namespacePath) => namespacePath
#if CS8
                [..
#else
                .Substring(0,
#endif
                namespacePath.IndexOf('.')
#if CS8
                ]
#else
                )
#endif
                ;

        public IEnumerable<T> GetAllItemsInPackages<T>(Converter<DotNetNamespace, IEnumerable<T>> converter) where T : DotNetType
        {
            foreach (T
#if CS8
                ?
#endif
                item in GetAllNamespacesInPackages().ForEachConverter(converter))

                yield return item;
        }

        public IEnumerable<DotNetEnum> GetAllEnumsInPackages() => GetAllItemsInPackages(@namespace => @namespace.GetEnums());

        public IEnumerable<DotNetInterface> GetAllInterfacesInPackages()
        {
            WriteLine("Enqueueing interfaces...", true);

            Collections.DotNetFix.Generic.IEnumerableQueue<DotNetInterface> dotNetInterfaces = new Collections.DotNetFix.Generic.EnumerableQueue<DotNetInterface>();

            bool predicate(DotNetInterface @interface) => !dotNetInterfaces.Any(__i => __i.Type.Namespace == @interface.Type.Namespace && __i.Type.Name == @interface.Type.Name) && ValidPackages.Contains(@interface.Type.Assembly.GetName().Name);

            IEnumerable<DotNetInterface> getBaseInterfaces(DotNetInterface __i)
            {
                foreach (DotNetInterface ___i in __i.GetBaseInterfaces().Where(predicate))
                {
                    foreach (DotNetInterface ____i in getBaseInterfaces(___i))

                        yield return ____i;

                    yield return ___i;
                }

                yield return __i;
            }

            IEnumerable<DotNetInterface> getInterfaces() => GetAllItemsInPackages(@namespace => @namespace.GetInterfaces()).Where(predicate);

            uint index = 0;

            foreach (DotNetInterface i in getInterfaces())

                foreach (DotNetInterface _i in getBaseInterfaces(i))
                {
                    DotNetInterface get() => getInterfaces().First(___i => !(___i.Type.GenericTypeArguments?.Length > 0) && ___i.Type.Namespace == _i.Type.Namespace && ___i.Type.Name == _i.Type.Name);

                    dotNetInterfaces.Enqueue(_i.Type.GenericTypeArguments?.Length > 0 ? get() : _i);

                    WriteLine($"{++index}: Enqueued {i.Type}", null);
                }

            WriteLine("Enqueued interfaces.", false);

            return dotNetInterfaces;
        }

        protected abstract byte[] GetEnumData();

        protected abstract byte[] GetInterfaceData();

        protected void CreateTypeFile(in DotNetType type, in int id, in Func<byte[]> typeDataFunc)
        {
            string path;
            string namespacePath;
            System.Type t;

            _ = Directory.CreateDirectory(path = GetWholePath(namespacePath = Path.Combine((t = type.Type).Namespace, t.GetRealName())));

            FileStream
#if CS8
                ?
#endif
                writer = File.Create(path = Path.Combine(path, GetFileName(t.ContainsGenericParameters ? (ushort
#if !CS9
                ?
#endif
                )t.GetGenericArguments().Length : null)));

            namespacePath = namespacePath.Replace('\\', '.');

            byte[] data = typeDataFunc();

            writer.Write(data, 0, data.Length);

            writer.Dispose();

            string getDocHeader() => $"{PackageInfo.Header} Doc";

            File.WriteAllText(path, File.ReadAllText(path).Replace("{PackageId}", PackageInfo.FrameworkId.ToString())
                .Replace("{ItemId}", id.ToString())
                .Replace("{ItemName}", t.Name)
                .Replace("{DocURL}", $"2 => '<a href=\"/doc/{PackageInfo.URL}/{GetNamespacePath(namespacePath)}/index.php\">{getDocHeader()}</a>', {GetNamespaceURLArray(namespacePath, out _)}")
                .Replace("{PackageUrl}", PackageInfo.URL));
        }

        public void CreateEnumFile(DotNetEnum @enum, int id) => CreateTypeFile(@enum, id, GetEnumData);

        public void CreateInterfaceFile(DotNetInterface @interface, int id) => CreateTypeFile(@interface, id, GetInterfaceData);

        protected static T GetEntity<T>(in T entity) where T : IEntity
        {
            entity.MarkForRefresh();

            return entity;
        }

        public static bool CheckTypeEquality(in string wn, in Type _type, in DotNetType item, in bool checkGenericity) => wn == item.Type.Namespace && _type.Name == item.Name && (!checkGenericity || (_type.GenericTypeCount.HasValue
                            ? item.Type.ContainsGenericParameters && _type.GenericTypeCount.Value == item.Type.GetGenericArguments().Length
                            : !item.Type.ContainsGenericParameters));

        protected void UpdateItems<T, U>(in string name, in DotNetNamespace @namespace, bool checkGenericity, Predicate<T> itemPredicate, Action<T, U>
#if CS8
            ?
#endif
            onAdded, Action<T>
#if CS8
            ?
#endif
            onDeleting, Converter<T, Type> converter, in Func<IEnumerable<U>> func, in Func<U, DBEntityCollection<T>, Type, T> getItem, Action<U, T> update, Action<U, int> createFile) where T : IDefaultEntity where U : DotNetType
        {
            WriteLine($"Updating {name}s", true);

            WriteLine("Parsing DB", true);

            string wholeNamespace = @namespace.Path;

            uint i = 0;

            ulong rows = 0;

            using
#if !CS9
#if !CS8
                (
#endif
                var items = new
#endif
                DBEntityCollection<T>
#if CS9
                items = new
#endif
                (GetConnection())
#if CS8
                ;
#else
                )
            {
#endif

            bool checkAll(in Type _type, in DotNetType item) => CheckTypeEquality(wholeNamespace, _type, item, checkGenericity);

            Type type;
            uint tables = 0;

            void updateRows(in T item) => rows = item.Remove(out tables);

#if !CS9
            var gtColl = new
#endif
            DBEntityCollection<GenericType>
#if CS9
                gtColl = new
#endif
                (Connection);

            ActionIn<T> delete = onDeleting == null ?
#if !CS9
                (ActionIn<T>)
#endif
                updateRows : (in T item) =>
                 {
                     onDeleting(item);

                     T _item = item;
                     Type t;

                     string logMessage = $" generic type parameters for {(t = converter(item)).Namespace}.{t.Name}.";

                     WriteLine("Removing" + logMessage, true);

                     foreach (GenericType genericType in gtColl.Where(gt => gt.Type.Id == _item.Id))

                         WriteLine($"Removed generic type parameter '{genericType.Name}'. Rows: {genericType.Remove(out tables)}. Tables: {tables}.", null);

                     WriteLine("Removed" + logMessage, false);

                     updateRows(item);
                 };

            foreach (T item in items.WherePredicate(itemPredicate))

                if (GetWholeNamespace((type = converter(item)).Namespace.Id) == wholeNamespace)
                {
                    WriteLine($"{++i}: Searching {item} in all packages.", true, ConsoleColor.DarkYellow);

                    if (func().Any(_item => checkAll(type, _item)))

                        WriteLine($"{item} found. Not removed from DB.", null, ConsoleColor.DarkGreen);

                    else
                    {
                        WriteLine($"{item} not found in any package. Deleting.", true, ConsoleColor.DarkRed);

                        delete(item);

                        if (rows > 0)
                        {
                            WriteLine($"Removed {rows} {nameof(rows)} in {tables} {nameof(tables)}.", null);

                            Directory.Delete(Path.Combine(GetWholePath(wholeNamespace), type.Name), true);

                            WriteLine($"{item} successfully deleted.", false);
                        }

                        else

                            throw new InvalidOperationException($"Could not remove {wholeNamespace}.{item}.");
                    }

                    WriteLine($"Processing {wholeNamespace} completed.", false);
                }

            WriteLine("Parsing DB completed.", false);

            long? id;

            i = 0;

#if !CS9
            var gtmColl = new
#endif
            DBEntityCollection<GenericTypeModifier>
#if CS9
                gtmColl = new
#endif
                (Connection);
#if !CS9
            var tColl = new
#endif
            DBEntityCollection<Type>
#if CS9
                tColl = new
#endif
                (Connection);
#if !CS9
            var nColl = new
#endif
            DBEntityCollection<Namespace>
#if CS9
                 nColl = new
#endif
                (Connection);
#if !CS9
            var ttColl = new
#endif
            DBEntityCollection<TypeType>
#if CS9
                ttColl = new
#endif
                (Connection);
#if !CS9
            var amColl = new
#endif
            DBEntityCollection<AccessModifier>
#if CS9
                amColl = new
#endif
                (Connection);

            void tryRefreshAccessModifierId() => type.AccessModifier.TryRefreshId(true);

            void setModifier(in GenericType _gt, in string _name)
            {
                _gt.Modifier = new GenericTypeModifier(gtmColl) { Name = _name };

                WriteLine($"Variance: {_name}.", null);
            }

            bool hasFlag(in System.Type _gt, in GenericParameterAttributes gpa) => _gt.GenericParameterAttributes.HasFlag(gpa);

            void _setModifier(in System.Type _gt, in GenericType __gt)
            {
                if (hasFlag(_gt, Contravariant))

                    setModifier(__gt, "in");

                else if (hasFlag(_gt, Covariant))

                    setModifier(__gt, "out");

                else

                    WriteLine($"No variance.", null);
            }

            ActionIn<T, U> _onAdded = onAdded == null ?
#if !CS9
                (ActionIn<T, U>)(
#endif
                (in T _item, in U _i) => _item.Dispose()
#if !CS9
            )
#endif
                : (in T _item, in U _i) =>
            {
                onAdded(_item, _i);

                System.Type _type = _i.Type;

                void writeLine(in string
#if CS8
                    ?
#endif
                    param1 = null, in string
#if CS8
                    ?
#endif
                    param2 = null) => WriteLine($"{_type.Namespace}.{_type.Name} is {param1}generic.{param2}", null);

                if (_i.Type.ContainsGenericParameters)
                {
                    writeLine(param2: " Adding.");

                    GenericType _gt;

                    foreach (System.Type gt in _i.Type.GetGenericArguments())
                    {
                        WriteLine($"Adding {gt.Name}.", true);

                        _gt = new GenericType(gtColl) { Type = converter(_item), Name = gt.Name };

                        _setModifier(gt, _gt);

                        WriteLine($"Added {_gt.Name}. Id: {gtColl.Add(_gt, out tables, out rows)}. Rows: {rows}. Tables: {tables}.", false);
                    }
                }

                else

                    writeLine("not ");

                _item.Dispose();
            };

            Collections.DotNetFix.Generic.IEnumerableQueue<System.Type> browsed = new Collections.DotNetFix.Generic.EnumerableQueue<System.Type>();

            foreach (U item in func().Where(_item => _item.Type.Namespace == wholeNamespace))
            {
                if (browsed.Contains(item.Type))

                    continue;

                WriteLine($"{++i}: Processing {item}.", true, ConsoleColor.DarkYellow);

                browsed.Enqueue(item.Type);

                bool process()
                {
                    foreach (T _item in items.WherePredicate(itemPredicate))

                        if (GetWholeNamespace((type = converter(_item)).Namespace.Id) == item.Type.Namespace && checkAll(type, item))
                        {
                            WriteLine("Exists. Updating.", true, ConsoleColor.DarkGreen);

                            update(item, _item);

                            type.AccessModifier = GetEntity(new AccessModifier(amColl) { Name = item.Type.IsPublic ? "public" : "protected" });

                            tryRefreshAccessModifierId();

                            if (item.Type.ContainsGenericParameters)
                            {
                                WriteLine($"Updating generic type parameters for {type.Namespace.Name}.{type.Name}.", true);

                                foreach (GenericType gt in gtColl.GetItems(Connection.GetOrderByColumns(OrderBy.Asc, nameof(GenericType.Id))).Where(_gt => _gt.Type.Id == _item.Id))

                                    foreach (System.Type t in item.Type.GetGenericArguments())
                                    {
                                        WriteLine($"Updating generic type parameter {t.Name}.", true);

                                        gt.Name = t.Name;

                                        _setModifier(t, gt);

                                        WriteLine($"Updated {gt.Update(out tables)} rows in {tables} {nameof(tables)}.", false);
                                    }

                                WriteLine($"Updated generic type parameters for {type.Namespace.Name}.{type.Name}.", false);
                            }

                            WriteLine($"Updated {_item.Update(out uint _tables)} rows in {_tables} {nameof(tables)}. Id: {_item.Id}.", false);

                            return false;
                        }

                    return true;
                }

                if (process())
                {
                    WriteLine("Does not exist. Adding.", true, ConsoleColor.DarkRed);

                    T _item = getItem(item, items, new Type(tColl)
                    {
                        Name = item.Name,

                        AccessModifier = GetEntity(new AccessModifier(amColl) { Name = item.Type.IsPublic ? "public" : "protected" }),

                        Namespace = GetEntity(new Namespace(nColl)
                        {
                            Name = @namespace.Name,

                            ParentId = @namespace.Parent == null ? null : GetNamespaceId(@namespace.Parent.Path),

                            FrameworkId = PackageInfo.FrameworkId
                        }),

                        TypeType = GetEntity(new TypeType(ttColl)
                        {
                            Name = name
                        })
                    });

                    void _process()
                    {
                        tryRefreshAccessModifierId();

                        _ = type.Namespace.TryRefreshId(true);

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
                        WriteLine($"Added {rows} {nameof(rows)} in {tables} {nameof(tables)}. Id: {id.Value}", null);

                        _onAdded(_item, item);

                        WriteLine($"Creating file for {item.Type.Name}.", null);

                        createFile(item, (int)id.Value);

                        WriteLine($"File created for {item.Type.Name}.", false);
                    }

                    else
                    {
                        _item.Dispose();

                        throw new InvalidOperationException($"Failed to add {item.Type.Name}.");
                    }
                }

                WriteLine($"Processed {item.Type.Name}.", false);
            }
#if !CS8
            }
#endif

            WriteLine($"Updating {name}s completed.", false);
        }

        public void UpdateEnums(DotNetNamespace @namespace)
        {
#if CS8
            static
#endif
                string getUnderlyingTypeCSName(in DotNetEnum @enum) => @enum.UnderlyingType.ToCSName();
#if !CS9
            var utColl = new
#endif
            DBEntityCollection<UnderlyingType>
#if CS9
                utColl = new
#endif
                (Connection);

            UnderlyingType getUnderlyingType(DotNetEnum @enum) => new
#if !CS9
                UnderlyingType
#endif
                (utColl)
            { Name = getUnderlyingTypeCSName(@enum) };

            UpdateItems<Enum, DotNetEnum>("Enum", @namespace, false, @enum => true, null, null, item => item.Type, GetAllEnumsInPackages, (@enum, enums, type) => new
#if !CS9
                        Enum
#endif
                        (enums)
            {
                UnderlyingType = GetEntity(getUnderlyingType(@enum)),

                Type = type
            }, (@enum, _enum) =>
            {
                _enum.UnderlyingType = getUnderlyingType(@enum);

                _enum.UnderlyingType.MarkForRefresh();

                _ = _enum.UnderlyingType.TryRefreshId(true);
            }, CreateEnumFile);
        }

        public void UpdateInterfaces(DotNetNamespace @namespace)
        {
#if CS8
            static
#endif
                void setGenericTypeCount(in DotNetInterface @interface, in Type type) => type.GenericTypeCount = @interface.Type.ContainsGenericParameters ? (byte
#if !CS9
                    ?
#endif
                )@interface.Type.GetGenericArguments().Length : null;

            DBEntityCollection<InterfaceImplementation> interfaceImplementations = new
#if !CS9
                DBEntityCollection<InterfaceImplementation>
#endif
                (GetConnection());

            DBEntityCollection<Type> types = new
#if !CS9
                DBEntityCollection<Type>
#endif
                (GetConnection());

            DBEntityCollection<Namespace> namespaces = new
#if !CS9
                DBEntityCollection<Namespace>
#endif
                (GetConnection());

            Collections.DotNetFix.Generic.IEnumerableQueue<System.Type> interfaces = new Collections.DotNetFix.Generic.EnumerableQueue<System.Type>();
            System.Type[] _interfaces;
            Type
#if CS8
                ?
#endif
                t;
            long id;

            void getInterfaces(in System.Type type)
            {
                foreach (System.Type i in type.GetInterfaces())

                    getInterfaces(i);

                if (!interfaces.Contains(type))

                    interfaces.Enqueue(type);
            }

            void updateInterfaces(in DotNetType _t)
            {
                foreach (System.Type i in (_interfaces = _t.Type.GetInterfaces()))

                    foreach (System.Type _i in i.GetInterfaces())

                        getInterfaces(_i);
            }

            string wholeNamespace = @namespace.Path;

            bool checkAll(string wn, in Type _type, in DotNetType item) => CheckTypeEquality(wn, _type, item, true);

            bool checkInterface(System.Type _i) => ValidPackages.Contains(_i.Assembly.GetName().Name) && !interfaces.Contains(_i);

            bool checkInterfaceImplementation(in InterfaceImplementation _i, in System.Type i) => checkAll(GetWholeNamespace(_i.ImplementedInterface.Namespace.Id), _i.ImplementedInterface, new DotNetType(i));

            void addEntityAssociations(Type type, DotNetInterface @interface, in bool check)
            {
                WriteLine($"Adding entity-associations for {@interface}", true);

                updateInterfaces(@interface);

                void add(in System.Type i)
                {
                    (t = new Type(types)
                    {
                        Namespace = new Namespace(namespaces) { Id = GetNamespaceId(i.Namespace).Value },
                        Name = i.GetRealName(),
                        GenericTypeCount = i.ContainsGenericParameters ? (byte
#if !CS9
                        ?
#endif
                        )i.GetGenericArguments().Length : null
                    }).MarkForRefresh();

                    //t.Namespace.Refresh();

                    t.TryRefreshId(true);

                    id = interfaceImplementations.Add(new InterfaceImplementation(interfaceImplementations) { Interface = type, ImplementedInterface = t }, out uint tables, out ulong rows).Value;

                    WriteLine($"Added entity-association for {@interface} : {i}. Added {rows} {nameof(rows)} in {tables} {nameof(tables)} (last inserted id: {id}).", null);
                }

                ActionIn<System.Type> _add = check ? (in System.Type i) =>
                {
                    System.Type __i = i;

                    if (!interfaceImplementations.Where(_i => checkAll(wholeNamespace, _i.Interface, @interface)).Any(_i => checkInterfaceImplementation(_i, __i)))

                        add(i);
                }
                :
#if !CS9
                (ActionIn<System.Type>)
#endif
                add;

                foreach (System.Type i in _interfaces.Where(checkInterface))

                    _add(i);

                id = 0;
                t = null;
                interfaces.Clear();

                WriteLine($"Added entity-associations for {@interface}", false);
            }

            void removeEntityAssociations(in Type type, in DotNetInterface
#if CS8
                ?
#endif
                @interface)
            {
                Type _t = type;

                WriteLine($"Removing entity-associations for {_t.Name}.", true);

                bool defaultPredicate(InterfaceImplementation i) => i.Interface.Id == _t.Id;

                Predicate<InterfaceImplementation> predicate;

                if (@interface == null)

                    predicate = defaultPredicate;

                else
                {
                    updateInterfaces(@interface);

                    predicate = i => defaultPredicate(i) && !_interfaces.Any(_i => checkInterface(_i) && checkInterfaceImplementation(i, _i));
                }

                foreach (InterfaceImplementation interfaceImplementation in interfaceImplementations.WherePredicate(predicate))

                    WriteLine($"Removed {interfaceImplementation.Remove(out uint tables)} rows in {tables} {nameof(tables)} for {interfaceImplementation.Interface.Name} : {interfaceImplementation.ImplementedInterface.Namespace.Name}.{interfaceImplementation.ImplementedInterface.Name}.", null);

                interfaces.Clear();

                WriteLine($"Removed entity-associations for {type.Name}.", false);
            }

            IEnumerable<DotNetInterface> dotNetInterfaces = GetAllInterfacesInPackages();

            UpdateItems<Type, DotNetInterface>("Interface", @namespace, true, type => type.TypeType.Name == "Interface", (type, @interface) => addEntityAssociations(type, @interface, false), _t => removeEntityAssociations(_t, null),

                 Delegates.Self, () => dotNetInterfaces, (@interface, __interfaces, type) =>
                   {
                       setGenericTypeCount(@interface, type);

                       return type;

                   }, (@interface, _interface) =>
      {
          setGenericTypeCount(@interface, _interface);

          removeEntityAssociations(_interface, @interface);

          addEntityAssociations(_interface, @interface, true);
      }, CreateInterfaceFile);
        }

        public void UpdateTypes(DotNetNamespace @namespace)
        {
            UpdateEnums(@namespace);

            UpdateInterfaces(@namespace);
        }

        public void UpdateNamespaces()
        {
            WriteLine("Updating namespaces", true);

            using
#if !CS8
                (
#endif
                ISQLConnection connection = GetConnection()
#if CS8
                ;
#else
                )
#endif

            using
#if !CS8
                (
#endif
                DBEntityCollection<Namespace> namespaces = new
#if !CS9
                DBEntityCollection<Namespace>
#endif
                (connection)
#if CS8
                ;
#else
                )
            {
#endif

            string wholeNamespace;

            WriteLine("Parsing DB.", true);

            uint i = 0;

            ulong rows = 0;

            foreach (Namespace
#if CS8
                ?
#endif
                @namespace in namespaces.GetItems(connection.GetOrderByColumns(OrderBy.Desc, nameof(Namespace.Id))))
            {
                wholeNamespace = GetWholeNamespace(@namespace.Id);

                WriteLine($"{++i}: Searching {wholeNamespace} in all packages.", true, ConsoleColor.DarkYellow);

                if (GetAllNamespacesInPackages().Any(_namespace => _namespace.Path == wholeNamespace))

                    WriteLine($"{wholeNamespace} found. Not removed from DB.", null, ConsoleColor.DarkGreen);

                else
                {
                    WriteLine($"{wholeNamespace} not found in any package. Deleting.", true, ConsoleColor.DarkRed);

                    DeleteTypes(wholeNamespace);

                    rows = @namespace.Remove(out uint tables);

                    if (rows > 0)
                    {
                        WriteLine($"Removed {rows} {nameof(rows)} in {tables} {nameof(tables)}.", null);

                        Directory.Delete(GetWholePath(wholeNamespace), true);

                        WriteLine($"{wholeNamespace} successfully deleted.", false);
                    }

                    else

                        throw new InvalidOperationException($"Could not remove {wholeNamespace}.");
                }

                WriteLine($"Processing {wholeNamespace} completed.", false);
            }

            wholeNamespace = null;

            WriteLine("Parsing DB completed.", false);

            long? id;

            i = 0;

            foreach (DotNetNamespace
#if CS8
                ?
#endif
                @namespace in GetAllNamespacesInPackages())
            {
                WriteLine($"{++i}: Processing {@namespace.Path}.", true, ConsoleColor.DarkYellow);

                bool process()
                {
                    foreach (Namespace _namespace in namespaces)

                        if (_namespace.Name == @namespace.Name && _namespace.ParentId == (@namespace.Parent == null ? null : GetNamespaceId(@namespace.Parent.Path)))
                        {
                            WriteLine("Exists. Updating.", true, ConsoleColor.DarkGreen);

                            _namespace.FrameworkId = PackageInfo.FrameworkId;

                            WriteLine($"Updated {_namespace.Update(out uint tables)} rows in {tables} {nameof(tables)}. Id: {_namespace.Id}.", false);

                            return false;
                        }

                    return true;
                }

                if (process())
                {
                    WriteLine("Does not exist. Adding.", true, ConsoleColor.DarkRed);

                    Namespace _namespace = new
#if !CS9
                        Namespace
#endif
                        (namespaces)
                    {
                        Name = @namespace.Name,

                        ParentId = @namespace.Parent == null ? null : GetNamespaceId(@namespace.Parent.Path)
                    };

                    id = namespaces.Add(_namespace, out uint tables, out rows);

                    if (id.HasValue)
                    {
                        WriteLine($"Added {rows} {nameof(rows)} in {tables} {nameof(tables)}. Id: {id.Value}", null);

                        WriteLine($"Creating file for {@namespace.Path}.", null);

                        CreateNamespaceFile(@namespace, (int)id.Value);

                        WriteLine($"File created for {@namespace.Path}.", false);
                    }

                    else

                        throw new InvalidOperationException($"Failed to add {@namespace.Path}.");
                }

                else

                    UpdateTypes(@namespace);

                WriteLine($"Processed {@namespace.Path}.", false);
            }
#if !CS8
            }
#endif

            WriteLine("Updating namespaces completed.", false);
        }

        public int? GetNamespaceId(string @namespace)
        {
            ISQLConnection
#if CS8
                ?
#endif
                connection = GetConnection();

            string[] namespaces = @namespace.Split('.');

            int paramId = 0;
            string getParamName() => $"name{paramId++}";

            SQLColumn[]
#if CS8
                ?
#endif
                idColumn = GetArray(connection.GetColumn(ID));

            ISelect getSelect(in ActionIn<IConditionGroup> action)
            {
                ISelect _select = connection.GetSelect(GetArray(DOC_NAMESPACE), idColumn);

                IConditionGroup conditionGroup = new ConditionGroup("AND");

                _select.ConditionGroup = conditionGroup;

                action(conditionGroup);

                return _select;
            }

            ICondition
#if CS8
                ?
#endif
                getNameCondition(in string _namespace) => connection.GetCondition(NAME, getParamName(), _namespace);

            ISelect select = getSelect((in IConditionGroup conditionGroup) => conditionGroup.Conditions = GetEnumerable(connection.GetNullityCondition(PARENT_ID), getNameCondition(namespaces[0])));

            for (int i = 1; i < namespaces.Length; i++)

                select = getSelect((in IConditionGroup conditionGroup) =>
                {
                    conditionGroup.Conditions = GetEnumerable(getNameCondition(namespaces[i]));

                    conditionGroup.Selects = GetEnumerable(new KeyValuePair<SQLColumn, ISelect>(connection.GetColumn(PARENT_ID), select));
                });

            foreach (ISQLGetter
#if CS8
                ?
#endif
                value in select.ExecuteQuery())

                return (int)value[0];

            return null;
        }

        public string GetWholeNamespace(int id)
        {
            IPrependableExtensibleEnumerable<string> namespaces = new TO_BE_DELETED.LinkedList<string>();

            KeyValuePair<string, int?>? first = null;

            bool whileCondition()
            {
                if (first.HasValue && first.Value.Value.HasValue)
                {
                    id = first.Value.Value.Value;

                    return true;
                }

                return false;
            }

            using
#if !CS8
                (
#endif
                ISQLConnection
#if CS8
                ?
#endif
                connection = GetConnection()
#if CS8
                ;
#else
                )
#endif

            do
            {
                IEnumerable<ISQLGetter>
#if CS8
                ?
#endif
                result = connection.GetSelect(GetArray(DOC_NAMESPACE), GetArray(connection.GetColumn(NAME), connection.GetColumn(PARENT_ID)), conditions: GetArray(connection.GetCondition(ID, nameof(id), id))).ExecuteQuery();

                foreach (ISQLGetter item in result)
                {
                    namespaces.Prepend((first = new KeyValuePair<string, int?>((string)item[NAME], item[PARENT_ID] is int _id ?
#if !CS9
                        (int?)
#endif
                        _id : null)).Value.Key);

                    break;
                }
            }
            while (whileCondition());

            return string.Join(
#if CS8
                '.'
#else
                "."
#endif
                , namespaces);
        }

        public string GetWholePath(string wholeNamespace) => Path.Combine(RootPath, wholeNamespace.Replace('.', TO_BE_DELETED.Path.PathSeparator));

        public void CreateNamespaceFile(DotNetNamespace @namespace, int id)
        {
            string path;
            string namespacePath;

            _ = Directory.CreateDirectory(path = GetWholePath(namespacePath = @namespace.Path));

            FileStream
#if CS8
                ?
#endif
                writer = File.Create(path = Path.Combine(path, GetFileName(null)));

            byte[] data = GetNamespaceData();

            writer.Write(data, 0, data.Length);

            writer.Dispose();

            string getDocHeader() => $"{PackageInfo.Header} Doc";

            File.WriteAllText(path, File.ReadAllText(path).Replace("{PackageId}", PackageInfo.FrameworkId.ToString())
                .Replace("{ItemId}", id.ToString())
                .Replace("{ItemName}", @namespace.Name)
                .Replace("{DocURL}", namespacePath.Contains('.') ? $"2 => '<a href=\"/doc/{PackageInfo.URL}/{GetNamespacePath(namespacePath)}/index.php\">{getDocHeader()}</a>', {GetNamespaceURLArray(namespacePath, out _)}" : $"2 => '{getDocHeader()}'")
                .Replace("{PackageUrl}", PackageInfo.URL));
        }

        public string GetNamespaceURLArray(string @namespace, out int index)
        {
            string[] namespaces = @namespace.Split('.');

            int length = namespaces.Length - 1;

            StringBuilder sb = new
#if !CS9
                StringBuilder
#endif
                ();
            StringBuilder aux = new
#if !CS9
                StringBuilder
#endif
                ();

            int i = 0;

            for (; i < length; i++)
            {
                _ = aux.Append('/');
                _ = aux.Append(@namespace = namespaces[i]);

                _ = sb.Append($"{i + 3} => '<a href=\"/doc/{PackageInfo.URL}{aux}/index.php\">{@namespace}</a> ', ");
            }

            _ = sb.Append($"{i + 3} => '{namespaces[i]}'");

            index = i + 3;

            return sb.ToString();
        }

        protected abstract byte[] GetNamespaceData();

        protected abstract string GetFileName(ushort? genericTypeCount);

        /*private void DoAction(ISQLConnection? connection, ActionIn<ISQLConnection> action)
        {
            if (connection == null)
            {
                using ISQLConnection? _connection = GetConnection();

                action(_connection);
            }

            else

                action(connection);
        }

        protected ISQLConnection GetConnection() => Connection.GetConnection();

        public int? AddEnum(DotNetEnum @enum)
        {
            int namespaceId = GetNamespaceId(@enum.Type.Namespace).Value;

            int? enumId = GetEnumId(@enum, namespaceId, out int? _typeId);

            if (enumId.HasValue)
            {
                _ = UpdateEnum(@enum, enumId.Value, _typeId.Value);

                return null;
            }

            using ISQLConnection? connection = GetConnection();

            int typeId = (int)AddType(namespaceId, @enum.Type.Name, GetTypeTypeId(@enum).Value, 0, GetAccessModifierId(@enum.Type).Value, null).Value;

            _ = connection.GetInsert(DOC_ENUM, new SQLItemCollection<string>(TYPE_ID, "UnderlyingType"), new SQLItemCollection<SQLItemCollection<IParameter>>(new SQLItemCollection<IParameter>(connection.GetParameter(nameof(typeId), typeId), connection.GetParameter("underlyingType", @enum.UnderlyingType.ToCSName())))).ExecuteNonQuery(out long? lastInsertedId);

            AddEnumValues(@enum, (int)lastInsertedId.Value, typeId);

            return (int?)lastInsertedId;
        }

        public int? AddEnumValue(int docTypeMember, long? value, ulong? uValue)
        {
            using ISQLConnection connection = GetConnection();

            _ = connection.GetInsert("docenumvalue", new SQLItemCollection<string>("DocTypeMember", "Value", "UValue"), new SQLItemCollection<SQLItemCollection<IParameter>>(new SQLItemCollection<IParameter>(connection.GetParameter(nameof(docTypeMember), docTypeMember), connection.GetParameter(nameof(value), value), connection.GetParameter(nameof(uValue), uValue)))).ExecuteNonQuery(out long? lastInsertedId);

            return (int?)lastInsertedId;
        }

        public void AddEnumValues(DotNetEnum @enum, int enumId, int? typeId = null)
        {
            if (typeId == null)

                typeId = GetTypeIdFromEnum(enumId);

            if (typeId.HasValue)

                foreach (DotNetEnumValue value in @enum)

                    _ = AddEnumValue(AddTypeMember(value.Name, typeId.Value).Value, value.Value, value.UValue);
        }

        public void AddItem<T>(IEnumerable<T> items, Func<T, int?> func, Action<T, int> action)
        {
            int? id;

            foreach (T item in items)

                if ((id = func(item)).HasValue)

                    action(item, id.Value);
        }

        public int? AddNamespace(string @namespace)
        {
            if (GetNamespaceId(@namespace).HasValue)

                return null;

            ISQLConnection? connection = GetConnection();

            int i = @namespace.LastIndexOf('.');

            int? parentId;

            if (i > -1)
            {
                parentId = GetNamespaceId(@namespace[..i]);

                @namespace = @namespace[(i + 1)..];
            }

            else

                parentId = null;

            _ = connection.GetInsert(DOC_NAMESPACE, new SQLItemCollection<string>("FrameworkId", "Name", "ParentId"), new SQLItemCollection<SQLItemCollection<IParameter>>(new SQLItemCollection<IParameter>(connection.GetParameter<int>("frameworkId", PackageInfo.Id), connection.GetParameter<string>("name", @namespace), connection.GetParameter<int?>("parentId", parentId)))).ExecuteNonQuery(out long? lastInsertedId);

            return (int?)lastInsertedId;
        }

        public void AddNewEnums() => AddItem(GetAllNamespacesInPackages().ForEach(@namespace => @namespace.GetEnums()), AddEnum, CreateEnumFile);

        public void AddNewNamespaces()
        {
            List<DotNetNamespace> namespaces = new ArrayBuilder<DotNetNamespace>(GetAllNamespacesInPackages()).ToList();

            namespaces.Sort();

            AddItem(namespaces, @namespace => AddNamespace(@namespace.Path), CreateNamespaceFile);
        }

        public long? AddType(int namespaceId, string name, int typeTypeId, short genericTypeCount, int accessModifier, int? parentType)
        {
            using ISQLConnection connection = GetConnection();

            _ = connection.GetInsert(DOC_TYPE, new SQLItemCollection<string>(NAME, NAMESPACE_ID, "TypeTypeId", "GenericTypeCount", "AccessModifier", "ParentType"), new SQLItemCollection<SQLItemCollection<IParameter>>(new SQLItemCollection<IParameter>(connection.GetParameter(nameof(name), name), connection.GetParameter(nameof(namespaceId), namespaceId), connection.GetParameter(nameof(typeTypeId), typeTypeId), connection.GetParameter(nameof(genericTypeCount), genericTypeCount), connection.GetParameter(nameof(accessModifier), accessModifier), connection.GetParameter(nameof(parentType), parentType)))).ExecuteNonQuery(out long? lastInsertedId);

            return lastInsertedId;
        }

        public int? AddTypeMember(string name, int typeId)
        {
            using ISQLConnection connection = GetConnection();

            connection.GetInsert(DOC_TYPE_MEMBER, new SQLItemCollection<string>(NAME, TYPE_ID), new SQLItemCollection<SQLItemCollection<IParameter>>(new SQLItemCollection<IParameter>(connection.GetParameter(nameof(name), name), connection.GetParameter(nameof(typeId), typeId)))).ExecuteNonQuery(out long? lastInsertedId);

            return (int?)lastInsertedId;
        }

        public uint DeleteEnum(int id, int? typeId = null)
        {
            _ = DeleteEnumValues(id);

            ISQLConnection connection = GetConnection();

            ISQLTableRequest2? delete = connection.GetDelete(GetArray(DOC_ENUM), conditions: GetEnumerable(connection.GetCondition(ID, nameof(id), id)));

            uint result = delete.ExecuteNonQuery();

            if (typeId.HasValue || (typeId = GetTypeIdFromEnum(id)).HasValue)

                _ = DeleteType(typeId.Value);

            return result;
        }

        public void DeleteEnums(int namespaceId, ISQLConnection? connection = null) => DoAction(connection, (in ISQLConnection connection) =>
        {
            foreach (int? enumId in GetEnumIds(namespaceId))

                if (enumId.HasValue)

                    _ = DeleteEnum(enumId.Value);
        });

        public uint DeleteEnumValues(int enumId)
        {
            using ISQLConnection connection = GetConnection();

            int? typeId = GetTypeIdFromType(enumId, "enum");

            ISQLTableRequest2? delete = connection.GetDelete(GetArray(DOC_ENUM_VALUE));

            delete.ConditionGroup = new ConditionGroup(null)
            {
                Selects = GetEnumerable(new KeyValuePair<SQLColumn, ISelect>(connection.GetColumn(DOC_TYPE_MEMBER_COLUMN), connection.GetSelect(GetArray(DOC_TYPE_MEMBER), GetArray(connection.GetColumn(ID)), conditions: GetArray(connection.GetCondition(TYPE_ID, nameof(typeId), typeId)))))
            };

            return delete.ExecuteNonQuery();
        }

        public uint DeleteNamespace(int id)
        {
            using ISQLConnection connection = GetConnection();

            DeleteTypes(id, connection);

            return connection.GetDelete(GetArray(DOC_NAMESPACE), conditions: GetArray(connection.GetCondition(ID, nameof(id), id))).ExecuteNonQuery();
        }

        public void DeleteOldEnums()
        {
            using ISQLConnection? connection = GetConnection();

            string wholeNamespace;
            int? enumId;

            bool contains(in string enumName)
            {
                foreach (DotNetNamespace? packageNamespace in GetAllNamespacesInPackages())

                    if (packageNamespace.Path == wholeNamespace && packageNamespace.GetEnums().Select(@enum => @enum.Type.Name).Contains(enumName))

                        return true;

                return false;
            }

            foreach (KeyValuePair<int, string> @namespace in GetNamespaces())

                foreach (KeyValuePair<int, string> type in GetTypes(@namespace.Key))
                {
                    wholeNamespace = GetWholeNamespace(@namespace.Key);

                    if (!contains(type.Value))
                    {
                        enumId = GetEnumId(type.Key);

                        _ = enumId.HasValue ? DeleteEnum(enumId.Value, type.Key) : DeleteType(type.Key);

                        Directory.Delete(GetWholePath(Path.Combine(wholeNamespace, type.Value)), true);
                    }
                }
        }

        public void DeleteOldNamespaces()
        {
            using ISQLConnection? connection = GetConnection();

            string wholeNamespace;

            foreach (KeyValuePair<int, string> @namespace in GetNamespaces())

                if (!GetAllNamespacesInPackages().Select(@namespace => @namespace.Path).Contains(wholeNamespace = GetWholeNamespace(@namespace.Key)))
                {
                    _ = DeleteNamespace(@namespace.Key);

                    Directory.Delete(GetWholePath(wholeNamespace), true);
                }
        }

        public uint DeleteType(int id)
        {
            ISQLConnection connection = GetConnection();

            _ = DeleteTypeMembers(id);

            return connection.GetDelete(GetArray(DOC_TYPE), conditions: GetEnumerable(connection.GetCondition(ID, nameof(id), id))).ExecuteNonQuery();
        }

        public uint DeleteTypeMembers(int typeId)
        {
            using ISQLConnection connection = GetConnection();

            return connection.GetDelete(GetArray(DOC_TYPE_MEMBER), conditions: GetArray(connection.GetCondition(TYPE_ID, nameof(typeId), typeId))).ExecuteNonQuery();
        }

        public void DeleteTypes(int namespaceId, ISQLConnection? connection = null)
        {
            DeleteEnums(@namespaceId, connection);
        }

        public short? GetAccessModifierId(Type type)
        {
            using ISQLConnection connection = GetConnection();

            foreach (ISQLGetter getter in connection.GetSelect(GetArray("class_access_modifier"), GetArray(connection.GetColumn(ID)), conditions: GetArray(connection.GetCondition(NAME, "name", DotNetHelper.GetCSAccessModifier(type)))).ExecuteQuery())

                return (short)getter[0];

            return null;
        }

        public int? GetEnumId(DotNetEnum @enum, int namespaceId, out int? typeId)
        {
            using ISQLConnection connection = GetConnection();

            typeId = null;

            foreach (ISQLGetter getter in connection.GetSelect(GetArray(DOC_TYPE), GetArray(connection.GetColumn("Id")), "AND", GetArray(connection.GetCondition(NAME, "name", @enum.Type.Name), connection.GetCondition(NAMESPACE_ID, nameof(namespaceId), namespaceId))).ExecuteQuery())
            {
                typeId = (int)getter[0];

                break;
            }

            if (typeId.HasValue)

                foreach (ISQLGetter getter in connection.GetSelect(GetArray(DOC_ENUM), GetArray(connection.GetColumn("Id")), conditions: GetArray(connection.GetCondition(TYPE_ID, nameof(typeId), typeId.Value))).ExecuteQuery())

                    return (int)getter[0];

            return null;
        }

        public int? GetEnumId(int typeId)
        {
            using ISQLConnection connection = GetConnection();

            foreach (ISQLGetter? getter in connection.GetSelect(GetArray(DOC_ENUM), GetArray(connection.GetColumn(ID)), conditions: GetArray(connection.GetCondition(DOC_TYPE, nameof(typeId), typeId))).ExecuteQuery())

                return (int)getter[0];

            return null;
        }

        public IEnumerable<int?> GetEnumIds(int namespaceId)
        {
            using ISQLConnection connection = GetConnection();

            foreach (int typeId in GetTypeIds(namespaceId))

                foreach (ISQLGetter getter in connection.GetSelect(GetArray(DOC_ENUM), GetArray(connection.GetColumn(ID)), conditions: GetArray(connection.GetCondition(DOC_TYPE, nameof(typeId), typeId))).ExecuteQuery())
                {
                    yield return (int)getter[0];

                    break;
                }
        }*/

        /*public IEnumerable<KeyValuePair<int, string>> GetNamespaces()
        {
            using ISQLConnection? connection = GetConnection();

            SQLColumn id = connection.GetColumn(ID);

            ISelect? select = connection.GetSelect(GetArray(DOC_NAMESPACE), GetArray(id, connection.GetColumn(NAME)));

            select.OrderBy = new OrderByColumns(new SQLItemCollection<SQLColumn>(id), OrderBy.Desc);

            static KeyValuePair<int, string> getKeyValuePair(in int id, in string name) => new(id, name);

            IEnumerable<ISQLGetter>? results = select.ExecuteQuery();

            foreach (ISQLGetter? result in results)

                yield return result.IsStringIndexable ? getKeyValuePair((int)result[ID], (string)result[NAME]) : getKeyValuePair((int)result[0], (string)result[1]);
        }

        public int? GetTypeId(int namespaceId, string name)
        {
            using ISQLConnection connection = GetConnection();

            foreach (ISQLGetter? getter in connection.GetSelect(GetArray(DOC_TYPE), GetArray(connection.GetColumn(ID)), "AND", GetArray(connection.GetCondition(NAMESPACE_ID, nameof(namespaceId), namespaceId), connection.GetCondition(NAME, nameof(name), name))).ExecuteQuery())

                return (int)getter[0];

            return null;
        }

        public int? GetTypeIdFromEnum(int enumId) => GetTypeIdFromType(enumId, "enum");

        public int? GetTypeIdFromType(int typeId, string typeName)
        {
            using ISQLConnection connection = GetConnection();

            foreach (ISQLGetter getter in connection.GetSelect(GetArray(DOC + typeName), GetArray(connection.GetColumn(TYPE_ID)), conditions: GetArray(connection.GetCondition(ID, nameof(typeId), typeId))).ExecuteQuery())

                return (int)getter[0];

            return null;
        }

        public IEnumerable<int> GetTypeIds(int namespaceId)
        {
            using ISQLConnection connection = GetConnection();

            return connection.GetSelect(GetArray(DOC_TYPE), GetArray(connection.GetColumn(ID)), conditions: GetArray(connection.GetCondition(NAMESPACE_ID, nameof(namespaceId), namespaceId))).ExecuteQuery().Select(getter => (int)getter[0]);
        }

        public IEnumerable<KeyValuePair<int, string>> GetTypes(int namespaceId)
        {
            using ISQLConnection? connection = GetConnection();

            SQLColumn id = connection.GetColumn(ID);

            ISelect? select = connection.GetSelect(GetArray(DOC_TYPE), GetArray(id, connection.GetColumn(NAME)), conditions: GetEnumerable(connection.GetCondition("NamespaceId", nameof(namespaceId), namespaceId)));

            select.OrderBy = new OrderByColumns(new SQLItemCollection<SQLColumn>(id), OrderBy.Desc);

            static KeyValuePair<int, string> getKeyValuePair(in int id, in string name) => new(id, name);

            IEnumerable<ISQLGetter>? results = select.ExecuteQuery();

            foreach (ISQLGetter? result in results)

                yield return result.IsStringIndexable ? getKeyValuePair((int)result[ID], (string)result[NAME]) : getKeyValuePair((int)result[0], (string)result[1]);
        }

        public string? GetTypeType(DotNetType type)
        {
            if (type is DotNetEnum)

                return "Enum";

            // TODO

            return null;
        }

        public short? GetTypeTypeId(DotNetType type)
        {
            using ISQLConnection connection = GetConnection();

            foreach (ISQLGetter getter in connection.GetSelect(GetArray("typetype"), GetArray(connection.GetColumn("Id")), conditions: GetArray(connection.GetCondition(NAME, "name", GetTypeType(type)))).ExecuteQuery())

                return (short)getter[0];

            return null;
        }*/

        /*public uint UpdateEnum(DotNetEnum @enum, int enumId, int typeId)
        {
            using ISQLConnection connection = GetConnection();

            _ = UpdateType(@enum, typeId, connection);

            long underlyingTypeId = 0;

            foreach (ISQLGetter getter in connection.GetSelect(GetArray("enumunderlyingtype"), GetArray(connection.GetColumn(ID)), conditions: GetArray(connection.GetCondition("Name", "name", @enum.UnderlyingType.ToCSName()))).ExecuteQuery())

                underlyingTypeId = (long)getter[0];

            IUpdate update = connection.GetUpdate(DOC_ENUM, new SQLItemCollection<ICondition>(connection.GetCondition("UnderlyingType", "underlyingType", underlyingTypeId)));

            update.ConditionGroup = new ConditionGroup(null)
            {
                Conditions = GetEnumerable(connection.GetCondition(ID, nameof(enumId), enumId))
            };

            uint result = update.ExecuteNonQuery();

            _ = DeleteEnumValues(enumId);

            AddEnumValues(@enum, enumId);

            return result;
        }

        public int UpdateType(DotNetType type, int typeId, ISQLConnection? connection) { /* TODO */ /*return 0; }*/
        #endregion
    }
}
