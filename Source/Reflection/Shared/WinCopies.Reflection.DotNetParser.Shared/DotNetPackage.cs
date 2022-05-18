using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
#if CS8
using System.Runtime.Loader;
#endif

using WinCopies.Collections.Generic;
using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.ThrowHelper;

using EUT = WinCopies.Reflection.DotNetParser.DotNetEnumUnderlyingType;

namespace WinCopies.Reflection.DotNetParser
{
    public class DotNetType : IComparable<DotNetType>
    {
        public Type Type { get; }

        public string Name => Type.GetRealName();

        public DotNetType(in Type type) => Type = type ?? throw GetArgumentNullException(nameof(type));

        public int CompareTo(DotNetType
#if CS8
            ?
#endif
            other) => other == null ? 1 : Type.Name.CompareTo(other.Type.Name);

        public override string ToString() => Type.ToString();
    }

    public enum DotNetEnumUnderlyingType
    {
        Byte = 0,

        SByte = 1,

        Int16 = 2,

        UInt16 = 3,

        Int32 = 4,

        UInt32 = 5,

        Int64 = 6,

        UInt64 = 7
    }

    public struct DotNetEnumValue
    {
        public string Name { get; }

        public long? Value { get; }

        public ulong? UValue { get; }

        public DotNetEnumValue(in string name, in long? value, in ulong? uValue)
        {
            Name = name ?? throw GetArgumentNullException(nameof(value));

            Value = value;

            UValue = uValue;
        }
    }

    public class DotNetEnum : DotNetType, System.Collections.Generic.IEnumerable<DotNetEnumValue>
    {
        public class Enumerator : Enumerator<FieldInfo, DotNetEnumValue>
        {
            private DotNetEnumValue _current;

            public override bool? IsResetSupported => false;

            protected override DotNetEnumValue CurrentOverride => _current;

            protected Type UnderlyingType { get; }

            public Enumerator(DotNetEnum @enum) : base(@enum.Type.GetFields().Where(f => f.IsLiteral)) => UnderlyingType = @enum.Type.GetEnumUnderlyingType();

            protected override bool MoveNextOverride()
            {
                if (InnerEnumerator.MoveNext())
                {
                    FieldInfo
#if CS8
                    ?
#endif
                        current = InnerEnumerator.Current;

                    if (current != null)
                    {
                        object
#if CS8
                        ?
#endif
                            value = current.GetRawConstantValue();

                        if (value != null)
                        {
                            value = System.Convert.ChangeType(value, UnderlyingType);

                            DotNetEnumValue getCurrent(in long? _value, in ulong? uValue) => new
#if !CS9
                                DotNetEnumValue
#endif
                                (current.Name, _value, uValue);

                            if (value != null)
                            {
                                _current = UtilHelpers.IsSigned(value) ? getCurrent((long)value, null) : getCurrent(null, (ulong)value);

                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            protected override void ResetCurrent()
            {
                _current = default;

                base.ResetCurrent();
            }
        }

        public EUT UnderlyingType { get; }

        public DotNetEnum(in Type type) : base(type.IsEnum ? type : throw DotNetHelper.GetTypeIsNotEnumException(nameof(type))) => UnderlyingType = Type.ToDotNetEnumUnderlyingType();

        public Type GetEnumUnderlyingType() => Type.GetEnumUnderlyingType();

        public IEnumerator<DotNetEnumValue> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class DotNetInterfaceImplementorType : DotNetType
    {
        public DotNetInterfaceImplementorType(in Type type) : base(type) { /* Left empty. */ }

        public System.Collections.Generic.IEnumerable<DotNetInterface> GetBaseInterfaces() => Type.GetInterfaces().Select(type => new DotNetInterface(type));
    }

    public class DotNetInterface : DotNetInterfaceImplementorType
    {
        public DotNetInterface(in Type type) : base(type.IsInterface ? type : throw DotNetHelper.GetTypeHasWrongTypeTypeException(nameof(type), "an interface")) { /* Left empty. */ }
    }

    public class DotNetClass : DotNetInterfaceImplementorType
    {
        public DotNetClass(in Type type) : base(type.IsClass ? type : throw DotNetHelper.GetTypeHasWrongTypeTypeException(nameof(type), "a class")) { /* Left empty. */ }

        public DotNetClass
#if CS8
            ?
#endif
            GetBaseClass()
        {
            Type
#if CS8
                ?
#endif
                type = Type.BaseType;

            return type == null ? null : new
#if !CS9
                DotNetClass
#endif
                (type);
        }
    }

    public class DotNetStruct : DotNetInterfaceImplementorType
    {
        public DotNetStruct(in Type type) : base(type.IsStruct() ? type : throw DotNetHelper.GetTypeHasWrongTypeTypeException(nameof(type), "a struct")) { /* Left empty. */ }
    }

    public class DotNetNamespace : IComparable<DotNetNamespace>
    {
        ReadOnlyCollection<DotNetNamespace> _namespaces;

        public DotNetPackage DotNetPackage { get; }

        public DotNetNamespace
#if CS8
            ?
#endif
            Parent
        { get; }

        public string Name { get; }

        public string Path
        {
            get
            {
                Collections.DotNetFix.Generic.ILinkedList<string> stringBuilder = new Collections.DotNetFix.Generic.LinkedList<string>();

                DotNetNamespace
#if CS8
            ?
#endif
            parent = Parent;

                _ = stringBuilder.AddFirst(Name);

                while (parent != null)
                {
                    _ = stringBuilder.AddFirst(parent.Name);

                    parent = parent.Parent;
                }

                return string.Join(
#if CS8
                    '.'
#else
                    "."
#endif
                    , stringBuilder.AsEnumerable<string>());
            }
        }

        public System.Collections.Generic.IEnumerable<Type
#if CS8
            ?
#endif
            > DefinedTypes => DotNetPackage.DefinedTypes;

        public DotNetNamespace(in DotNetPackage dotNetPackage, in DotNetNamespace
#if CS8
            ?
#endif
            parent, in string name)
        {
            DotNetPackage = dotNetPackage ?? throw GetArgumentNullException(nameof(dotNetPackage));

            Parent = parent;

            ThrowIfNullEmptyOrWhiteSpace(name);

            Name = name;
        }

        public System.Collections.Generic.IEnumerable<DotNetNamespace> GetSubnamespaces()
        {
            if (_namespaces == null)
            {
                var namespaces = new ArrayBuilder<DotNetNamespace>();

                foreach (DotNetNamespace _namespace in new Enumerable<string>(() => new NamespaceEnumerator(DotNetPackage.DefinedTypes, type => type.IsPublic, null)).WhereSelect(@namespace => @namespace.Length > Path.Length + 1 && @namespace.StartsWith(Path + '.') && !@namespace
#if CS8
        [
#else
        .Substring
#endif
        (Path.Length + 1)
#if CS8
        ..]
#endif
        .Contains('.'), @namespace => new DotNetNamespace(DotNetPackage, this, @namespace
#if CS8
            [
#else
            .Substring
#endif
            (@namespace.LastIndexOf('.') + 1)
#if CS8
            ..]
#endif
            )))

                    namespaces.AddLast(_namespace);

                _namespaces = namespaces.ToList().AsReadOnly();
            }

            return _namespaces;
        }

        internal static bool _DefaultTypePredicate(Type type) => type.IsPublicType() || type.IsTypeNestedFamily();

        public static bool DefaultTypePredicate(Type type) => type != null && _DefaultTypePredicate(type);

        public bool DefaultTypePredicate2(Type type) => type != null && type.Namespace == Path;

        public bool DefaultTypeAllPredicates(Type type) => DefaultTypePredicate(type) && DefaultTypePredicate2(type);

        public static bool DefaultNestedTypePredicate(Type type) => type?.IsNested == true && (type.IsNestedPublic || type.IsNestedFamily);

        public System.Collections.Generic.IEnumerable<T> GetTypes<T>(Predicate<Type> predicate, in Converter<Type, T> func) => func == null ? throw GetArgumentNullException(nameof(func)) : predicate == null ? throw GetArgumentNullException(nameof(predicate)) : DefinedTypes.WhereSelectPredicateConverter(type => DefaultTypeAllPredicates(type) && predicate(type), func);

        protected System.Collections.Generic.IEnumerable<T> GetTypes<T>(in System.Collections.Generic.IEnumerable<T> types) where T : DotNetType => types.Where(type => DefaultTypePredicate2(type.Type));

        // TODO:

        //public System.Collections.Generic.IEnumerable<DotNetEnum> GetEnums() => GetTypes(type => type.IsEnum, type => new DotNetEnum(type));

        public System.Collections.Generic.IEnumerable<DotNetInterface> GetInterfaces() => GetTypes(DotNetPackage.GetInterfaces());

        public System.Collections.Generic.IEnumerable<DotNetClass> GetClasses() => GetTypes(DotNetPackage.GetClasses());

        public System.Collections.Generic.IEnumerable<DotNetStruct> GetStructs() => GetTypes(DotNetPackage.GetStructs());

        // TODO:

        //public System.Collections.Generic.IEnumerable<DotNetType> GetTypes() => Extensions.UtilHelpers.Enumerate<DotNetType>(GetEnums, GetInterfaces, GetClasses, GetStructs /* TODO */ );

        public int CompareTo(DotNetNamespace
#if CS8
            ?
#endif
            other) => other == null ? 1 : Path.CompareTo(other.Path);

        public override string ToString() => Path;
    }

#if CS8
    public class PackageLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public string Path { get; }

        public PackageLoadContext(string packagePath) : base(true) => _resolver = new AssemblyDependencyResolver(Path = packagePath);

        protected override Assembly Load(AssemblyName assemblyName) => UtilHelpers.PerformActionIfNotNull(_resolver.ResolveAssemblyToPath(assemblyName), LoadFromAssemblyPath);

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) => UtilHelpers.PerformActionIfNull(_resolver.ResolveUnmanagedDllToPath(unmanagedDllName), IntPtr.Zero, LoadUnmanagedDllFromPath);
    }
#endif

    public class DotNetPackage : System.Collections.Generic.IEnumerable<DotNetNamespace>
#if !CS8
        , DotNetFix.IDisposable
#endif
    //: IRecursiveEnumerableProviderEnumerable<DotNetItem>
    {
        private System.Collections.Generic.IEnumerable<Type
#if CS8
            ?
#endif
            > _definedTypes;
        private System.Collections.Generic.IEnumerable<DotNetEnum> _enums;
        private System.Collections.Generic.IEnumerable<DotNetInterface> _interfaces;
        private System.Collections.Generic.IEnumerable<DotNetClass> _classes;
        private System.Collections.Generic.IEnumerable<DotNetStruct> _structs;

        public string Path { get; }

        public Assembly
#if CS8
            ?
#endif
            Assembly
        { get; private set; }

        public bool IsOpen => !IsDisposed;

        public System.Collections.Generic.IEnumerable<Type
#if CS8
            ?
#endif
            > DefinedTypes => _definedTypes
#if CS8
            ??=
#else
            ?? (_definedTypes =
#endif
            Assembly?.GetDefinedTypes() ?? Enumerable.Empty<Type>()
#if !CS8
            )
#endif
            ;

        public bool IsDisposed => Assembly == null;

        public DotNetPackage(in string path) => Path = path ?? throw GetArgumentNullException(nameof(path));

        private static InvalidOperationException GetPackageOpenStatusException(in string msg) => new
#if !CS9
            InvalidOperationException
#endif
            ($"The package is {msg} open.");

#if CS8
        public void Open(PackageLoadContext packageLoadContext) => Assembly = Assembly == null ? ((packageLoadContext ?? throw GetArgumentNullException(nameof(packageLoadContext))).Path == Path ? packageLoadContext : throw new ArgumentException("The given context does not have the same path as the current package.", nameof(packageLoadContext))).LoadFromAssemblyPath(Path) : throw GetPackageOpenStatusException("already");
#endif

        public void Open() =>
#if CS8
            Open(new PackageLoadContext(Path));
#else
            Assembly = Assembly.ReflectionOnlyLoadFrom(Path);
#endif

#if !CS8
        public void Close()
        {
#if CS8
            var a = AssemblyLoadContext.GetLoadContext(Assembly);

            a.Unload();
#endif

            Assembly = null;
        }
#endif

        public IEnumerator<DotNetNamespace> GetEnumerator() => IsOpen ? new Enumerable<string>(() => new NamespaceEnumerator(DefinedTypes, type => type.IsPublic, null)).Where(@namespace => !@namespace.Contains('.')).Select(@namespace => new DotNetNamespace(this, null, @namespace)).GetEnumerator() : throw GetPackageOpenStatusException("not");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected System.Collections.Generic.IEnumerable<T> GetTypes<T>(ref System.Collections.Generic.IEnumerable<T> items, Predicate<Type> predicate, Converter<Type, T> converter) where T : DotNetType
        {
            if (items == null)
            {
                EnumerableHelper<T>.IEnumerableQueue types;

                void addType(T type)
                {
                    if (types.Any(t => t.Type == type.Type))

                        return;

                    if (Collections.Enumerable.GetNullCheckWhileEnumerableC(type.Type.DeclaringType, _t => _t.DeclaringType).All(DotNetNamespace._DefaultTypePredicate))

                        types.Enqueue(type);
                }

                bool _predicate(in Type t, in Predicate<Type> __predicate) => t != null && predicate(t) && t.Assembly == Assembly && __predicate(t);

                void addNestedTypes(in Type type)
                {
                    System.Collections.Generic.IEnumerable<Type> getNestedTypes(Type _type)
                    {
                        foreach (Type t in _type.GetNestedTypes())
                        {
                            foreach (Type _t in getNestedTypes(t))

                                yield return _t;

                            yield return t;
                        }
                    }

                    foreach (T t in getNestedTypes(type).WhereSelectPredicateConverter(t => _predicate(t, DotNetNamespace.DefaultNestedTypePredicate), converter))

                        addType(t);
                }

                items = (types = EnumerableHelper<T>.GetEnumerableQueue()).AsReadOnlyEnumerable();

                /*System.Collections.Generic.IEnumerable<DotNetInterface> _getInterfaces()
                {
#if CS8
                    static
#endif
                    System.Collections.Generic.IEnumerable<DotNetInterface> getInterfaces(Type t)
                    {
                        foreach (Type _t in t.GetInterfaces())

                            foreach (DotNetInterface __t in getInterfaces(_t))

                                yield return __t;

                        yield return new DotNetInterface(t);
                    }*/

                foreach (T type in DefinedTypes.WhereSelectPredicateConverter(t => _predicate(t, DotNetNamespace.DefaultTypePredicate), converter))

                    /*foreach (DotNetInterface @interface in getInterfaces(type))

                        yield return @interface;
            }

            foreach (DotNetInterface @interface in _getInterfaces())*/

                    addType(type);

                foreach (DotNetClass type in GetClasses())

                    addNestedTypes(type.Type);
            }

            return items;
        }

        public System.Collections.Generic.IEnumerable<DotNetEnum> GetEnums() => GetTypes(ref _enums, type => type.IsEnum, type => new DotNetEnum(type));

        public System.Collections.Generic.IEnumerable<DotNetInterface> GetInterfaces() => GetTypes(ref _interfaces, type => type.IsInterface, type => new DotNetInterface(type));

        public System.Collections.Generic.IEnumerable<DotNetStruct> GetStructs() => GetTypes(ref _structs, type => type.IsStruct(), type => new DotNetStruct(type));

        public System.Collections.Generic.IEnumerable<DotNetClass> GetClasses() => GetTypes(ref _classes, type => type.IsClass && !typeof(Delegate).IsAssignableFrom(type), type => new DotNetClass(type));

#if !CS8
        protected virtual void Dispose(bool disposing) => Close();

        public void Dispose() => Dispose(true);
#endif

        //public IEnumerator<IRecursiveEnumerable<DotNetItem>> GetRecursiveEnumerator() => new Enumerator(this);

        //public IEnumerator<DotNetItem> GetEnumerator() => new RecursiveEnumerator<DotNetItem>(this, RecursiveEnumerationOrder.ParentThenChildren);

        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //public class Enumerator : Enumerator<string, DotNetItem>
        //{
        //    private DotNetItem _current;

        //    public override bool? IsResetSupported => true;

        //    protected override DotNetItem CurrentOverride => _current;

        //    public Enumerator(in DotNetPackage dotNetPackage) : base(new NamespaceEnumerator(GetAssembly(dotNetPackage), type => type.IsPublic, null))
        //    {
        //        // Left empty.
        //    }

        //    private static Assembly GetAssembly(in DotNetPackage dotNetPackage)
        //    {
        //        Assembly? assembly = (dotNetPackage ?? throw GetArgumentNullException(nameof(dotNetPackage))).Assembly;

        //        return assembly ?? throw GetPackageOpenStatusException("not");
        //    }

        //    protected override bool MoveNextOverride()
        //    {
        //        if (InnerEnumerator.MoveNext())
        //        {
        //            _current = new DotNetItem(InnerEnumerator.Current);

        //            return true;
        //        }

        //        return false;
        //    }
        //}
    }
}
