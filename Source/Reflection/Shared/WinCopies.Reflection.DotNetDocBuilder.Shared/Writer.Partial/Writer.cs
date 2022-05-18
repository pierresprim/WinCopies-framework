using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Linq;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Util;

using static System.Console;

using static WinCopies.IO.Path;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public abstract partial class Writer
    {
#if CS9
        public delegate Writer Constructor(ISQLConnection connection, string dbName, string rootPath, IEnumerable<DotNetPackage> packages, IEnumerable<string> validPackages, PackageInfo packageInfo);

        public delegate Constructor ConstructorConverter(string path, Func<string?> readLineFunc);

        public delegate ISQLConnection ConnectionConstructor(string? server, string? uid, NetworkCredential networkCredential);
#endif

        private const string TYPE_PREFIX = "T:";

        private IEnumerable<DotNetNamespace> _dotNetNamespaces;
        private IEnumerable<DotNetInterface> _dotNetInterfaces;
        private IEnumerable<DotNetClass> _dotNetClasses;
        private IEnumerable<DotNetStruct> _dotNetStructs;

        #region Properties
        protected ISQLConnection Connection { get; }

        protected Logger Logger { get; }

        public IEnumerable<DotNetPackage> Packages { get; }

        public IEnumerable<string> ValidPackages { get; }

        public string DBName { get; }

        public PackageInfo PackageInfo { get; }

        public string RootPath { get; }

        public IEnumerable<DotNetInterface> DotNetInterfaces => _dotNetInterfaces
#if CS8
            ??=
#else
            ?? (_dotNetInterfaces =
#endif
            GetAllInterfacesInPackages()
#if !CS8
            )
#endif
            ;

        public IEnumerable<DotNetClass> DotNetClasses => _dotNetClasses
#if CS8
            ??=
#else
            ?? (_dotNetClasses =
#endif
            GetAllClassesInPackages()
#if !CS8
            )
#endif
            ;

        public IEnumerable<DotNetStruct> DotNetStructs => _dotNetStructs
#if CS8
            ??=
#else
            ?? (_dotNetStructs =
#endif
            GetAllStructsInPackages()
#if !CS8
            )
#endif
            ;
        #endregion Properties

        #region Constructors
        public Writer(in ISQLConnection connection, in string dbName, in string rootPath, IEnumerable<DotNetPackage> packages, in IEnumerable<string> validPackages, in PackageInfo packageInfo, in Logger logger)
        {
            Connection = connection;
            DBName = dbName;
            RootPath = rootPath;
            Packages = packages;
            ValidPackages = validPackages;
            PackageInfo = packageInfo;
            Logger = logger;
        }

        public Writer(in ISQLConnection connection, in string dbName, in string rootPath, IEnumerable<DotNetPackage> packages, in IEnumerable<string> validPackages, in PackageInfo packageInfo, in byte initialTabsCount) : this(connection, dbName, rootPath, packages, validPackages, packageInfo, new ConsoleLogger(initialTabsCount).WriteLine) { /* Left empty. */ }

        public Writer(in ISQLConnection connection, in string dbName, in string rootPath, IEnumerable<DotNetPackage> packages, in IEnumerable<string> validPackages, in PackageInfo packageInfo) : this(connection, dbName, rootPath, packages, validPackages, packageInfo, 0) { /* Left empty. */ }
        #endregion Constructors

        #region Methods
#if CS9
        public static bool UpdateLocally(in ConstructorConverter constructor, in ConnectionConstructor connectionConstructor)
        {
            string
#if CS8
        ?
#endif
                path;

            Collections.DotNetFix.Generic.IEnumerableQueue<DotNetPackage> dotNetPackages = new Collections.DotNetFix.Generic.EnumerableQueue<DotNetPackage>();

            string dllPath = null;
            DotNetPackage dotNetPackage;
            IEnumerable<string> validPackages;

            path = Console.ReadLine("Please enter the path to the options file:");

            using
#if !CS8
            (
#endif
                var streamReader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read))
#if CS8
                ;
#else
            )
            {
#endif
            string directory;

            ActionIn<string> _action = (in string _path) =>
            {
                directory = _path;

                if (!directory.EndsWith(PathSeparator))

                    directory += PathSeparator;

                _action = (in string __path) =>
                {
                    dotNetPackages.Enqueue(dotNetPackage = new DotNetPackage(dllPath = Path.Combine(directory, __path + ".dll")));

                    dotNetPackage.Open();
                };
            };

            path = Path.GetDirectoryName(path);

            string? readLine() => streamReader.ReadLine();

            string getCompletePath() => GetCompletePath(path, readLine());

            IEnumerable<string> readLines() => File.ReadLines(getCompletePath());

            try
            {
                foreach (string _path in readLines())

                    _action(_path);
            }

            catch (Exception ex)
            {
                streamReader.Dispose();

                WriteLine($"Invalid dll: {dllPath}. Press any key to go back to the main menu.\nException info: {ex.Message} HResult: {ex.HResult}");

                _ = ReadKey();

                return true;
            }

            validPackages = readLines();

            if (dotNetPackages.AsFromType<Collections.DotNetFix.Generic.IEnumerableQueue<DotNetPackage>, Collections.IUIntCountable>().Count == 0)
            {
                WriteLine("No dll path provided.");

                return true;
            }

            string
#if CS8
        ?
#endif
                rootPath = getCompletePath();
            string
#if CS8
        ?
#endif
                server = readLine();
            string
#if CS8
        ?
#endif
                dbName = readLine();
            string
#if CS8
        ?
#endif
                uid = readLine();

            var password = new SecureString();
            ConsoleKeyInfo key;

            WriteLine("Please enter the password:");

            while ((key = ReadKey(true)).Key != ConsoleKey.Enter)

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.RemoveAt(password.Length - 1);

                        Write("\b \b");
                    }
                }

                else if (key.KeyChar != '\0')
                {
                    password.AppendChar(key.KeyChar);

                    Write("*");
                }

            ISQLConnection connection = connectionConstructor(server, uid, new NetworkCredential(null, password));

            connection.Open();

            connection.UseDB(dbName);

            string header = readLine();
            int frameworkId = int.Parse(readLine());
            string url = readLine();

            constructor(path, readLine)(connection, dbName, rootPath, dotNetPackages, validPackages, new PackageInfo(header, frameworkId, url)).UpdateAll();
#if !CS8
            }
#endif

            /*while (dotNetPackages.TryDequeue(out dotNetPackage))

                dotNetPackage.Close();*/

            WriteLine("Update completed successfully. Press any key to go back to the main menu.");

            _ = ReadKey();

            return true;
        }

        public static bool UpdateLocally(in ConnectionConstructor connectionConstructor) => UpdateLocally((string path, Func<string
#if CS8
            ?
#endif
            > readPathfunc) => (ISQLConnection connection, string dbName, string rootPath, IEnumerable<DotNetPackage> packages, IEnumerable<string> validPackages, PackageInfo packageInfo) =>
            {
                string? readLine(out StreamReader streamReader, out string filePath) => (readPathfunc = (streamReader = new StreamReader(filePath = GetCompletePath(path, readPathfunc()))).ReadLine)();

                string fileName = readLine(out StreamReader settings, out _);
                string extension = readPathfunc();
                StreamReader fileNames;
                string? _readLine() => fileNames.ReadLine();

                var filePaths = new WriterFilePaths()
                {
                    ClassFile = readLine(out fileNames, out string filePath),
                    EnumFile = _readLine(),
                    InterfaceFile = _readLine(),
                    NamespaceFile = _readLine(),
                    StructFile = _readLine()
                };

                var writer = new DefaultWriter(connection, dbName, rootPath, packages, validPackages, packageInfo, fileName, new WriterFilePathSettings(Path.GetDirectoryName(filePath), filePaths, extension));

                settings.Dispose();
                fileNames.Dispose();

                return writer;
            }
            , connectionConstructor);

        private static void Main(in Func<bool> func) => Console.HandleMenu("Welcome to DotNetDocBuilder!\n", true, new KeyValuePair<string, Func<bool>>("Update Locally", func), new KeyValuePair<string, Func<bool>>("Update FTP Server", () => true), new KeyValuePair<string, Func<bool>>("Quit", () => false));

        public static void Main(ConstructorConverter constructor, ConnectionConstructor connectionConstructor) => Main(() => UpdateLocally(constructor, connectionConstructor));
        public static void Main(ConnectionConstructor connectionConstructor) => Main(() => UpdateLocally(connectionConstructor));
#endif

        protected ISQLConnection GetConnection() => Connection.GetConnection();

        protected DBEntityCollection<T> GetCollection<T>() where T : IEntity => new
#if !CS9
            DBEntityCollection<T>
#endif
            (GetConnection());

        protected ulong Remove<T>() where T : IEntity => DBEntityCollection<T>.Remove(GetConnection());

        protected ulong Remove<T>(in Func<T, bool> predicate) where T : IEntity => DBEntityCollection<T>.Remove(GetConnection(), predicate);

        protected ulong RemovePredicate<T>(in Predicate<T> predicate) where T : IEntity => DBEntityCollection<T>.RemovePredicate(GetConnection(), predicate);

        public IEnumerable<DotNetNamespace> GetAllNamespacesInPackages()
        {
            if (_dotNetNamespaces == null)
            {
                IEnumerable<DotNetNamespace> getNamespaces()
                {
                    foreach (DotNetNamespace
#if CS8
                    ?
#endif
                    @namespace in Packages.ForEach(GetNamespaces))

                        yield return @namespace;
                }

                var namespaces = new Collections.Generic.ArrayBuilder<DotNetNamespace>();

                foreach (DotNetNamespace @namespace in getNamespaces())

                    namespaces.AddLast(@namespace);

                _dotNetNamespaces = namespaces.ToList().AsReadOnly();
            }

            return _dotNetNamespaces;
        }

        public IEnumerable<T> GetAllItemsInPackages<T>(Converter<DotNetPackage, Func<IEnumerable<T>>> converter) where T : DotNetType
        {
            foreach (T
#if CS8
                ?
#endif
                item in Packages.ForEachConverter(converter))

                yield return item;
        }

        public IEnumerable<DotNetEnum> GetAllEnumsInPackages() => GetAllItemsInPackages<DotNetEnum>(package => package.GetEnums);

        public static bool CheckTypeEquality(System.Type x, System.Type y) => x.Namespace == y.Namespace && x.Name == y.Name;

        public static bool CheckTypeEquality2(System.Type x, System.Type y) => x.GenericTypeArguments.Length == 0 && CheckTypeEquality(x, y);

        public bool CheckDeclaringTypes(in DotNetType x, in DotNetType y, Func<System.Type, System.Type, bool> func)
        {
            ActionIn<System.Type> _action;
            PredicateIn<System.Type> _predicate;

            Collections.Generic.EnumerableHelper<System.Type>.IEnumerableQueue declaringTypes = Collections.Generic.EnumerableHelper<System.Type>.GetEnumerableQueue();

            IEnumerable<System.Type> getEnumerable(in DotNetType ____i) => Collections.Enumerable.GetNullCheckWhileEnumerableC(____i.Type, _____i => _____i.DeclaringType);

            _action = (in System.Type t) => _action = (in System.Type _t) => declaringTypes.Enqueue(_t);

            foreach (System.Type t in getEnumerable(x))

                _action(t);

            _predicate = (in System.Type t) =>
            {
                _predicate = (in System.Type _t) =>

                declaringTypes.TryDequeue(out System.Type __t) && _t.GenericTypeArguments.Length == 0 && func(__t, _t);

                return true;
            };

            foreach (System.Type t in getEnumerable(y))

                if (!_predicate(t))

                    return false;

            return !declaringTypes.HasItems;
        }

        bool GetAllTypesInPackagesPredicate(DotNetType type, in IEnumerable<DotNetType> types) => !types.Any(__i => CheckTypeEquality(__i.Type, type.Type) && CheckDeclaringTypes(__i, type, CheckTypeEquality)) && ValidPackages.Contains(type.Type.Assembly.GetName().Name);

        protected void GetAllTypesInPackages<T>(IEnumerable<T> types, Converter<T, IEnumerable<T>> converter, Converter<DotNetPackage, Func<IEnumerable<T>>> _converter, in Action<T> action) where T : DotNetType
        {
            Logger($"Enqueueing types ({typeof(T).Name})...", true);

            bool predicate(DotNetType type) => GetAllTypesInPackagesPredicate(type, types);

            IEnumerable<T> getTypes()
            {
                var __types = new Collections.Generic.ArrayBuilder<T>();

                foreach (T type in GetAllItemsInPackages(_converter).Where(predicate))

                    _ = __types.AddLast(type);

                return __types.AsFromType<IEnumerable<T>>().AsReadOnlyEnumerable();
            }

            IEnumerable<T> _types = getTypes();

            T get(T i) => _types.First(___i =>
            {
                if (CheckTypeEquality2(___i.Type, i.Type))
                {
                    bool checkNullity(in T ____i) => ___i.Type.DeclaringType == null;

                    return (checkNullity(___i) && checkNullity(i)) || CheckDeclaringTypes(___i, i, CheckTypeEquality2);
                }

                return false;
            }
                );

            uint index = 0;

            foreach (T type in _types.ForEach(_i => converter(_i)).SelectWhere(i => i.Type.GetRealGenericTypeParameterLength() > 0 ? get(i) : i, predicate))
            {
                action(type);

                Logger($"{++index}: Enqueued {type}", null);
            }

            Logger("Enqueued types.", false);
        }

        protected IEnumerable<T> GetAllTypesInPackages<T>(Converter<Collections.DotNetFix.Generic.IEnumerableQueue<T>, Converter<T, IEnumerable<T>>> func, in Converter<DotNetPackage, Func<IEnumerable<T>>> converter) where T : DotNetType
        {
            Collections.DotNetFix.Generic.IEnumerableQueue<T> dotNetTypes = new Collections.DotNetFix.Generic.EnumerableQueue<T>();

            GetAllTypesInPackages(dotNetTypes, type => func(dotNetTypes)(type), converter, dotNetTypes.Enqueue);

            return dotNetTypes.AsReadOnlyEnumerable();
        }

        public IEnumerable<DotNetInterface> GetAllInterfacesInPackages() => GetAllTypesInPackages<DotNetInterface>(dotNetInterfaces =>
            {
                IEnumerable<DotNetInterface> getBaseInterfaces(DotNetInterface __i)
                {
                    foreach (DotNetInterface ___i in __i.GetBaseInterfaces().Where(type => GetAllTypesInPackagesPredicate(type, dotNetInterfaces)))
                    {
                        foreach (DotNetInterface ____i in getBaseInterfaces(___i))

                            yield return ____i;

                        yield return ___i;
                    }

                    yield return __i;
                }

                return getBaseInterfaces;
            },
            package => package.GetInterfaces);

        public IEnumerable<DotNetClass> GetAllClassesInPackages()
        {
            Collections.DotNetFix.Generic.LinkedList<DotNetClass> dotNetClasses = new Collections.DotNetFix.Generic.LinkedList<DotNetClass>();

            IEnumerable<DotNetClass> getBaseClasses(DotNetClass __c)
            {
                do
                {
                    if (GetAllTypesInPackagesPredicate(__c, dotNetClasses))

                        yield return __c;

                    else

                        break;

                    __c = __c.GetBaseClass();
                }
                while (__c != null);
            }

            GetAllTypesInPackages<DotNetClass>(dotNetClasses, getBaseClasses, package => package.GetClasses, c => dotNetClasses.AddFirst(c));

            return new Collections.Generic.Enumerable<DotNetClass>(dotNetClasses.GetReversedEnumerator);
        }

        public IEnumerable<DotNetStruct> GetAllStructsInPackages()
        {
            var enumerable = new Collections.Generic.SingletonEnumerable<DotNetStruct>();

            return GetAllTypesInPackages<DotNetStruct>(dotNetStructs => @struct =>
            {
                enumerable.UpdateCurrent(@struct);

                return enumerable;
            },
            package => package.GetStructs);
        }

        protected abstract byte[] GetEnumData();
        protected abstract byte[] GetInterfaceData();
        protected abstract byte[] GetClassData();
        protected abstract byte[] GetStructData();

        /*protected static T GetEntity<T>(in T entity) where T : IEntity
        {
            entity.MarkForRefresh();

            return entity;
        }*/

        public class WhileEnumerator<T> : Collections.Generic.Enumerator<T>
        {
            private T _current;
            private Func<bool> _moveNext;

            protected Converter<T, T> Converter { get; }

            protected Predicate<T> Predicate { get; }

            public override bool? IsResetSupported => false;

            protected override T CurrentOverride => _current;

            public WhileEnumerator(T first, Converter<T, T> converter, Predicate<T> predicate)
            {
                Converter = converter;

                Predicate = predicate;

                _moveNext = () => UtilHelpers.PerformActionIf(first, predicate, () =>
                {
                    _current = first;

                    _moveNext = () => predicate(_current = converter(_current));
                });
            }

            protected override bool MoveNextOverride() => _moveNext();

            protected override void ResetCurrent()
            {
                _current = default;

                base.ResetCurrent();
            }

            protected override void ResetOverride2() { /* Left empty. */ }
        }

        public bool CheckTypeEquality(in Type type, in System.Type item, in bool checkGenericity) => Diagnostics.Determine.AreNotNull(type, item) && GetWholeNamespace(type.Namespace.Id) == item.Namespace && type.Name == item.GetRealName() && (!checkGenericity || (type.GenericTypeCount == item.GetRealGenericTypeParameterLength()));

        public bool CheckTypeEquality2(Type type, in System.Type item, in bool checkGenericity)
        {
            Func<Type> func = () =>
            {
                func = () => type = type.ParentType?.Type;

                return type;
            };

            foreach (System.Type _item in Collections.Enumerable.GetNullCheckWhileEnumerable(item, _item => _item.DeclaringType))

                if (!CheckTypeEquality(func(), _item, checkGenericity))

                    return false;

            return type.ParentType == null;
        }

        public bool CheckTypeEquality(in Type type, in DotNetType item, in bool checkGenericity) => CheckTypeEquality(type, item.Type, checkGenericity);

        public bool CheckTypeEquality2(in Type type, in DotNetType item, in bool checkGenericity) => CheckTypeEquality2(type, item.Type, checkGenericity);

        /*
         * This method will create a new entity-framework-compatible type. Keep in mind that the type created here does not have all of its properties set already. Furthermore, an object is actually assigned to the Namespace property, but that Namespace object is not initialized yet.
         */
        public Type GetType(in System.Type dotNetType, in string typeTypeName, in DBEntityCollection<Type> tColl, in DBEntityCollection<AccessModifier> amColl, in DBEntityCollection<Namespace> nColl, in DBEntityCollection<TypeType> ttColl) => new
#if !CS9
            Type
#endif
            (tColl)
        {
            Name = dotNetType.GetRealName(),

            AccessModifier = new AccessModifier(amColl) { Name = dotNetType.IsPublicType() ? "public" : "protected" },

            Namespace = new Namespace(nColl) { FrameworkId = PackageInfo.FrameworkId },

            TypeType = new TypeType(ttColl) { Name = typeTypeName },

            GenericTypeCount = (byte)dotNetType.GetRealGenericTypeParameterLength()
        };

        public void UpdateNamespace(in Type dbType, in string wholeNamespace)
        {
            Namespace namespaceTmp = dbType.Namespace;

            void updateNamespace(in string _name, in ulong? parentId)
            {
                namespaceTmp.Name = _name;

                namespaceTmp.ParentId = parentId;
            }

            int dotIndex;

            (string parentNamespace, string namespaceName) tuple;

            if ((dotIndex = wholeNamespace.LastIndexOf('.')) > -1)

                updateNamespace((tuple = wholeNamespace.Split(dotIndex)).namespaceName, GetNamespaceId(tuple.parentNamespace));

            else

                updateNamespace(wholeNamespace, null);
        }

        public void UpdateParentType(in System.Type dotNetType, Type type, in DBEntityCollection<Type> tColl, in DBEntityCollection<AccessModifier> amColl, in DBEntityCollection<Namespace> nColl, in DBEntityCollection<TypeType> ttColl, in DBEntityCollection<Class> cColl)
        {
            Type typeTmp;

            foreach (System.Type _declaringType in Collections.Enumerable.GetNullCheckWhileEnumerable(dotNetType.DeclaringType, (_declaringType) => _declaringType.DeclaringType))
            {
                typeTmp = GetType(_declaringType, "Class", tColl, amColl, nColl, ttColl);

                UpdateNamespace(typeTmp, _declaringType.Namespace);

                type.ParentType = new Class(cColl) { Type = typeTmp };

                type = typeTmp;
            }
        }

        public string GetWholePath(string wholeNamespace) => Path.Combine(RootPath, wholeNamespace.Replace('.', '\\'));

        protected abstract string GetFileName(ushort? genericTypeCount);
        #endregion
    }

#if CS9
    public struct WriterFilePaths
    {
        public string ClassFile { get; init; }
        public string EnumFile { get; init; }
        public string InterfaceFile { get; init; }
        public string NamespaceFile { get; init; }
        public string StructFile { get; init; }
    }

    public struct WriterFilePathSettings
    {
        public string Directory { get; }

        public WriterFilePaths FilePaths { get; }

        public string Extension { get; }

        public WriterFilePathSettings(in string directory, in WriterFilePaths filePaths, in string extension)
        {
            Directory = directory;

            FilePaths = filePaths;

            Extension = extension;
        }
    }

    public class DefaultWriter : Writer
    {
        protected string FileName { get; }

        protected WriterFilePathSettings FilePathSettings { get; }
        protected WriterFilePaths FilePaths => FilePathSettings.FilePaths;

        public DefaultWriter(in ISQLConnection connection, in string dbName, in string rootPath, IEnumerable<DotNetPackage> packages, in IEnumerable<string> validPackages, in PackageInfo packageInfo, in string fileName, in WriterFilePathSettings filePathSettings) : base(connection, dbName, rootPath, packages, validPackages, packageInfo)
        {
            FileName = fileName;

            FilePathSettings = filePathSettings;
        }

        protected override string GetFileName(ushort? genericTypeCount) => string.Format(FileName, genericTypeCount);



        protected string GetDataFilePath(in string dataFileName) => Path.Combine(FilePathSettings.Directory, dataFileName + FilePathSettings.Extension);
        protected byte[] ReadAllBytes(in string dataFileName) => File.ReadAllBytes(GetDataFilePath(dataFileName));

        protected override byte[] GetClassData() => ReadAllBytes(FilePaths.ClassFile);
        protected override byte[] GetEnumData() => ReadAllBytes(FilePaths.EnumFile);
        protected override byte[] GetInterfaceData() => ReadAllBytes(FilePaths.InterfaceFile);
        protected override byte[] GetNamespaceData() => ReadAllBytes(FilePaths.NamespaceFile);
        protected override byte[] GetStructData() => ReadAllBytes(FilePaths.StructFile);
    }
#endif
}
