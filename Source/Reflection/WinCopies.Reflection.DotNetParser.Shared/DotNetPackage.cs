using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using WinCopies.Collections.Generic;
using WinCopies.Linq;
using WinCopies.Temp;
using WinCopies.Temp.Linq;

using static WinCopies.Temp.Util;
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

        public override string ToString() => $"{Type.Namespace}.{Type.Name}";
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
                            value = Convert.ChangeType(value, UnderlyingType);

                            DotNetEnumValue getCurrent(in long? _value, in ulong? uValue) => new
#if !CS9
                                DotNetEnumValue
#endif
                                (current.Name, _value, uValue);

                            if (value != null)
                            {
                                _current = IsSigned(value) ? getCurrent((long)value, null) : getCurrent(null, (ulong)value);

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

        public IEnumerator<DotNetEnumValue> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class DotNetInterface : DotNetType
    {
        public DotNetInterface(in Type type) : base(type.IsInterface ? type : throw DotNetHelper.GetTypeIsNotEnumException(nameof(type))) { /* Left empty. */ }

        public System.Collections.Generic.IEnumerable<DotNetInterface> GetBaseInterfaces()
        {
            foreach (Type type in Type.GetInterfaces())

                yield return new DotNetInterface(type);
        }
    }

    public class DotNetNamespace : IComparable<DotNetNamespace>
    {
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

        public System.Collections.Generic.IEnumerable<DotNetNamespace> GetSubnamespaces() => new Enumerable<string>(() => new NamespaceEnumerator(DotNetPackage.Assembly, type => type.IsPublic, null)).WhereSelect(@namespace => @namespace.Length > Path.Length + 1 && @namespace.StartsWith(Path + '.') && !@namespace
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
            ));

        public bool DefaultTypePredicate(Type t) => t != null && t.IsPublic && !t.IsNested && t.Namespace == Path;

        public System.Collections.Generic.IEnumerable<Type> GetDefinedTypes() => DotNetPackage.Assembly.GetDefinedTypes();

        public System.Collections.Generic.IEnumerable<T> GetTypes<T>(Predicate<Type> predicate, in Converter<Type, T> func) => func == null ? throw GetArgumentNullException(nameof(func)) : predicate == null ? throw GetArgumentNullException(nameof(predicate)) : GetDefinedTypes().WhereSelectPredicateConverter(type => DefaultTypePredicate(type) && predicate(type), func);

        public System.Collections.Generic.IEnumerable<DotNetEnum> GetEnums() => GetTypes(type => type.IsEnum, type => new DotNetEnum(type));

        public System.Collections.Generic.IEnumerable<DotNetInterface> GetInterfaces()
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
            }

            foreach (Type type in GetDefinedTypes())

                if (type.IsInterface && DefaultTypePredicate(type))

                    foreach (DotNetInterface @interface in getInterfaces(type))

                        yield return @interface;
        }

        public System.Collections.Generic.IEnumerable<DotNetType> GetTypes() => Enumerate<DotNetType>(GetEnums(), GetInterfaces() /* TODO */);

        public int CompareTo(DotNetNamespace
#if CS8
            ?
#endif
            other) => other == null ? 1 : Path.CompareTo(other.Path);

        public override string ToString() => Path;
    }

    public class DotNetPackage : System.Collections.Generic.IEnumerable<DotNetNamespace> //: IRecursiveEnumerableProviderEnumerable<DotNetItem>
    {
        public string Path { get; }

        public Assembly
#if CS8
            ?
#endif
            Assembly
        { get; private set; }

        public bool IsOpen => Assembly != null;

        public DotNetPackage(in string path) => Path = path ?? throw GetArgumentNullException(nameof(path));

        private static InvalidOperationException GetPackageOpenStatusException(in string msg) => new
#if !CS9
            InvalidOperationException
#endif
            ($"The package is {msg} open.");

        public void Open() => Assembly = Assembly == null ? Assembly.LoadFrom(Path) : throw GetPackageOpenStatusException("already");

        public void Close() => Assembly = null;

        public IEnumerator<DotNetNamespace> GetEnumerator() => IsOpen ? new Enumerable<string>(() => new NamespaceEnumerator(Assembly, type => type.IsPublic, null)).Where(@namespace => !@namespace.Contains('.')).Select(@namespace => new DotNetNamespace(this, null, @namespace)).GetEnumerator() : throw GetPackageOpenStatusException("not");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
