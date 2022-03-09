using System.Diagnostics;
using System.Text;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Linq;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Temp;

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

        [EntityProperty(IsPseudoId = true)]
        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        [EntityProperty($"{nameof(Namespace)}Id", IsPseudoId = true)]
        [ForeignKey]
        public Namespace Namespace { get => TryRefreshAndGet(ref _namespace); set => _namespace = value; }

        [EntityProperty($"{nameof(TypeType)}Id")]
        [ForeignKey]
        public TypeType TypeType { get => TryRefreshAndGet(ref _typeType); set => _typeType = value; }

        [EntityProperty($"{nameof(AccessModifier)}")]
        [ForeignKey]
        public AccessModifier AccessModifier { get => TryRefreshAndGet(ref _accessModifier); set => _accessModifier = value; }

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

        [EntityProperty($"{nameof(Type)}Id")]
        [ForeignKey(RemoveAlso = true)]
        public Type Type { get => TryRefreshAndGet(ref _type); set => _type = value; }

        public TypeBase(DBEntityCollection<T> collection) : base(collection) { /* Left empty. */ }

        public override string ToString() => Type.ToString();
    }

    [Entity("docenum")]
    public class Enum : TypeBase<Enum>
    {
        private UnderlyingType _underlyingType;

        [EntityProperty($"{nameof(UnderlyingType)}")]
        [ForeignKey]
        public UnderlyingType UnderlyingType { get => TryRefreshAndGet(ref _underlyingType); set => _underlyingType = value; }

        public Enum(DBEntityCollection<Enum> collection) : base(collection) { /* Left empty. */ }
    }

    public abstract class Writer : IWriter
    {
        private byte _tabsCount = 0;

        #region Properties
        protected ISQLConnection Connection { get; }

        protected IEnumerable<DotNetPackage> Packages { get; }

        public string DBName { get; }

        public PackageInfo PackageInfo { get; }

        public string RootPath { get; }
        #endregion Properties

        public Writer(in ISQLConnection connection, in string dbName, in string rootPath, in IEnumerable<DotNetPackage> packages, in PackageInfo packageInfo)
        {
            Connection = connection;

            DBName = dbName;

            RootPath = rootPath;

            Packages = packages;

            PackageInfo = packageInfo;
        }

        #region Methods
        protected void WriteLine(in string? msg, in bool? increment, in ConsoleColor? color = null)
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
            foreach (DotNetPackage? package in Packages)

                foreach (DotNetNamespace? @namespace in GetNamespaces(package))

                    yield return @namespace;
        }

        public void DeleteTypesBase<T>(string wholeNamespace) where T : TypeBase<T>
        {
            void writeLine(in string? value = null) => WriteLine($"Deleting all {nameof(T)}s from {wholeNamespace}{value}", value == null);

            writeLine();

            using DBEntityCollection<T> types = new(GetConnection());

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

            writeLine(" completed");
        }

        public void DeleteTypes(string wholeNamespace)
        {
            void writeLine(in string? value = null) => WriteLine($"Deleting all types from {wholeNamespace}{value}.", value == null);

            writeLine();

            DeleteTypesBase<Enum>(wholeNamespace);

            writeLine(" completed");
        }

        public IEnumerable<DotNetEnum> GetAllEnumsInPackages()
        {
            foreach (DotNetEnum? @enum in GetAllNamespacesInPackages().ForEach(@namespace => @namespace.GetEnums()))

                yield return @enum;
        }

        protected abstract byte[] GetEnumData();

        public void CreateEnumFile(DotNetEnum @enum, int id)
        {
            string path;
            string namespacePath;

            _ = Directory.CreateDirectory(path = GetWholePath(namespacePath = Path.Combine(@enum.Type.Namespace, @enum.Type.Name)));

            FileStream? writer = File.Create(path = Path.Combine(path, GetFileName(null)));

            namespacePath = namespacePath.Replace('\\', '.');

            byte[] data = GetEnumData();

            writer.Write(data, 0, data.Length);

            writer.Dispose();

            string getDocHeader() => $"{PackageInfo.Header} Doc";

            File.WriteAllText(path, File.ReadAllText(path).Replace("{PackageId}", PackageInfo.FrameworkId.ToString())
                .Replace("{ItemId}", id.ToString())
                .Replace("{ItemName}", @enum.Type.Name)
                .Replace("{DocURL}", $"2 => '<a href=\"/doc/{PackageInfo.URL}/{namespacePath[..namespacePath.IndexOf('.')]}/index.php\">{getDocHeader()}</a>', {GetNamespaceURLArray(namespacePath, out _)}")
                .Replace("{PackageUrl}", PackageInfo.URL));
        }

        public void UpdateEnums(string wholeNamespace, DotNetNamespace @namespace)
        {
            WriteLine("Updating enums", true);

            WriteLine("Parsing DB.", true);

            uint i = 0;

            ulong rows = 0;

            //while (true)

            //    try
            //    {
                    var c = GetConnection();

                    if (c.IsClosed)

                        throw new Exception();

                    using DBEntityCollection<Enum> enums = new(c);

                    foreach (Enum? @enum in enums)

                        if (GetWholeNamespace(@enum.Type.Namespace.Id) == wholeNamespace)
                        {
                            WriteLine($"{++i}: Searching {@enum} in all packages.", true, ConsoleColor.DarkYellow);

                            if (GetAllEnumsInPackages().Any(_enum => _enum.Type.Namespace == wholeNamespace))

                                WriteLine($"{@enum} found. Not removed from DB.", null, ConsoleColor.DarkGreen);

                            else
                            {
                                WriteLine($"{@enum} not found in any package. Deleting.", true, ConsoleColor.DarkRed);

                                rows = @enum.Remove(out uint tables);

                                if (rows > 0)
                                {
                                    WriteLine($"Removed {rows} {nameof(rows)} in {tables} {nameof(tables)}.", null);

                                    Directory.Delete(Path.Combine(GetWholePath(wholeNamespace), @enum.Type.Name), true);

                                    WriteLine($"{@enum} successfully deleted.", false);
                                }

                                else

                                    throw new InvalidOperationException($"Could not remove {wholeNamespace}.{@enum}.");
                            }

                            WriteLine($"Processing {wholeNamespace} completed.", false);
                        }

                //    break;
                //}

                //catch (Exception ex)
                //{

                //}

            WriteLine("Parsing DB completed.", false);

            long? id;

            i = 0;

            DBEntityCollection<UnderlyingType> utColl = new(Connection);
            DBEntityCollection<Type> tColl = new(Connection);
            DBEntityCollection<Namespace> nColl = new(Connection);
            DBEntityCollection<TypeType> ttColl = new(Connection);
            DBEntityCollection<AccessModifier> amColl = new(Connection);

            DBEntityCollection<Enum> _enums;

            foreach (DotNetEnum @enum in GetAllEnumsInPackages().Where(_enum => _enum.Type.Namespace == wholeNamespace))
            {
                WriteLine($"{++i}: Processing {@enum}.", true, ConsoleColor.DarkYellow);

                string wn;

                bool process()
                {
                    while (true)

                        try
                        {
                            var c = GetConnection();

                            if (c.IsClosed)

                                throw new Exception();

                             _enums = new(c);

                            foreach (Enum _enum in _enums)
                            {
                                wn = GetWholeNamespace(_enum.Type.Namespace.Id);

                                Debug.WriteLine(wn);

                                if (wn == @enum.Type.Namespace && _enum.Type.Name == @enum.Type.Name)
                                {
                                    WriteLine("Exists. Updating.", true, ConsoleColor.DarkGreen);

                                    _enum.UnderlyingType = new UnderlyingType(utColl) { Name = @enum.UnderlyingType.ToCSName() };

                                    _enum.UnderlyingType.MarkForRefresh();

                                    _ = _enum.UnderlyingType.TryRefreshId(true);

                                    WriteLine($"Updated {_enum.Update(out uint tables)} rows in {tables} {nameof(tables)}. Id: {_enum.Id}.", false);

                                    return false;
                                }
                            }

                            return true;
                        }

                        catch
                        {

                        }
                }

                if (process())
                {
                    WriteLine("Does not exist. Adding.", true, ConsoleColor.DarkRed);

                    T getEntity<T>(in T entity) where T : IEntity
                    {
                        entity.MarkForRefresh();

                        return entity;
                    }

                    Enum _enum = new(_enums)
                    {
                        UnderlyingType = getEntity(new UnderlyingType(utColl)
                        {
                            Name = @enum.UnderlyingType.ToCSName()
                        }),

                        Type = new Type(tColl)
                        {
                            Name = @enum.Type.Name,

                            AccessModifier = getEntity(new AccessModifier(amColl) { Name = "public" }),

                            Namespace = getEntity(new Namespace(nColl)
                            {
                                Name = @namespace.Name,

                                ParentId = @namespace.Parent == null ? null : GetNamespaceId(@namespace.Parent.Path),

                                FrameworkId = PackageInfo.FrameworkId
                            }),

                            TypeType = getEntity(new TypeType(ttColl)
                            {
                                Name = nameof(Enum)
                            })
                        }
                    };

                    _enum.MarkForRefresh();

                    _ = _enum.Type.AccessModifier.TryRefreshId(true);

                    _ = _enum.Type.Namespace.TryRefreshId(true);

                    _ = _enum.Type.TypeType.TryRefreshId(true);

                    _ = _enum.TryRefreshId(false);

                    id = _enums.Add(_enum, out uint tables, out rows);

                    _enum.Dispose();

                    if (id.HasValue)
                    {
                        WriteLine($"Added {rows} {nameof(rows)} in {tables} {nameof(tables)}. Id: {id.Value}", null);

                        WriteLine($"Creating file for {@enum.Type.Name}.", null);

                        CreateEnumFile(@enum, (int)id.Value);

                        WriteLine($"File created for {@enum.Type.Name}.", false);
                    }

                    else

                        throw new InvalidOperationException($"Failed to add {@enum.Type.Name}.");
                }

                WriteLine($"Processed {@enum.Type.Name}.", false);
            }

            WriteLine("Updating namespaces completed.", false);
        }

        public void UpdateTypes(string wholeNamespace, DotNetNamespace @namespace)
        {
            UpdateEnums(wholeNamespace, @namespace);
        }

        public void UpdateNamespaces()
        {
            WriteLine("Updating namespaces", true);

            using DBEntityCollection<Namespace> namespaces = new(GetConnection());

            string wholeNamespace;

            WriteLine("Parsing DB.", true);

            uint i = 0;

            ulong rows = 0;

            foreach (Namespace? @namespace in namespaces)
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

            foreach (DotNetNamespace? @namespace in GetAllNamespacesInPackages())
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

                    Namespace _namespace = new(namespaces)
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

                    UpdateTypes(@namespace.Path, @namespace);

                WriteLine($"Processed {@namespace.Path}.", false);
            }

            WriteLine("Updating namespaces completed.", false);
        }

        public int? GetNamespaceId(string @namespace)
        {
            ISQLConnection? connection = GetConnection();

            string[] namespaces = @namespace.Split('.');

            int paramId = 0;
            string getParamName() => $"name{paramId++}";

            SQLColumn[]? idColumn = GetArray(connection.GetColumn(ID));

            ISelect getSelect(in ActionIn<IConditionGroup> action)
            {
                ISelect select = connection.GetSelect(GetArray(DOC_NAMESPACE), idColumn);

                IConditionGroup conditionGroup = new ConditionGroup("AND");

                select.ConditionGroup = conditionGroup;

                action(conditionGroup);

                return select;
            }

            ICondition? getNameCondition(in string @namespace) => connection.GetCondition(NAME, getParamName(), @namespace);

            ISelect select = getSelect((in IConditionGroup conditionGroup) => conditionGroup.Conditions = GetEnumerable(connection.GetNullityCondition(PARENT_ID), getNameCondition(namespaces[0])));

            for (int i = 1; i < namespaces.Length; i++)

                select = getSelect((in IConditionGroup conditionGroup) =>
                {
                    conditionGroup.Conditions = GetEnumerable(getNameCondition(namespaces[i]));

                    conditionGroup.Selects = GetEnumerable(new KeyValuePair<SQLColumn, ISelect>(connection.GetColumn(PARENT_ID), select));
                });

            foreach (ISQLGetter? value in select.ExecuteQuery())

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

            using ISQLConnection? connection = GetConnection();

            do
            {
                IEnumerable<ISQLGetter>? result = connection.GetSelect(GetArray(DOC_NAMESPACE), GetArray(connection.GetColumn(NAME), connection.GetColumn(PARENT_ID)), conditions: GetArray(connection.GetCondition(ID, nameof(id), id))).ExecuteQuery();

                foreach (ISQLGetter item in result)
                {
                    namespaces.Prepend((first = new KeyValuePair<string, int?>((string)item[NAME], item[PARENT_ID] is int _id ? _id : null)).Value.Key);

                    break;
                }
            }
            while (whileCondition());

            connection.Close();

            return string.Join('.', namespaces);
        }

        public string GetWholePath(string wholeNamespace) => Path.Combine(RootPath, wholeNamespace.Replace('.', TO_BE_DELETED.Path.PathSeparator));

        public void CreateNamespaceFile(DotNetNamespace @namespace, int id)
        {
            string path;
            string namespacePath;

            _ = Directory.CreateDirectory(path = GetWholePath(namespacePath = @namespace.Path));

            FileStream? writer = File.Create(path = Path.Combine(path, GetFileName(null)));

            byte[] data = GetNamespaceData();

            writer.Write(data, 0, data.Length);

            writer.Dispose();

            string getDocHeader() => $"{PackageInfo.Header} Doc";

            File.WriteAllText(path, File.ReadAllText(path).Replace("{PackageId}", PackageInfo.FrameworkId.ToString())
                .Replace("{ItemId}", id.ToString())
                .Replace("{ItemName}", @namespace.Name)
                .Replace("{DocURL}", namespacePath.Contains('.') ? $"2 => '<a href=\"/doc/{PackageInfo.URL}/{namespacePath[..namespacePath.IndexOf('.')]}/index.php\">{getDocHeader()}</a>', {GetNamespaceURLArray(namespacePath, out _)}" : $"2 => '{getDocHeader()}'")
                .Replace("{PackageUrl}", PackageInfo.URL));
        }

        public string GetNamespaceURLArray(string @namespace, out int index)
        {
            string[] namespaces = @namespace.Split('.');

            int length = namespaces.Length - 1;

            StringBuilder sb = new();
            StringBuilder aux = new();

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
