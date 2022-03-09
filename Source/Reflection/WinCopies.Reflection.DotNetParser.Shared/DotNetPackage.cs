using System.Collections;
using System.Reflection;

using WinCopies.Collections.Generic;
using WinCopies.Linq;

using static WinCopies.Temp.Util;
using static WinCopies.ThrowHelper;

using EUT = WinCopies.Reflection.DotNetParser.DotNetEnumUnderlyingType;

namespace WinCopies.Reflection.DotNetParser
{
    public class DotNetType : IComparable<DotNetType>
    {
        public Type Type { get; }

        public DotNetType(in Type type) => Type = type ?? throw GetArgumentNullException(nameof(type));

        public int CompareTo(DotNetType? other) => other == null ? 1 : Type.Name.CompareTo(other.Type.Name);

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

    public static class DotNetHelper
    {
        public static string? GetCSAccessModifier(Type type) => type.IsPublic
                ? "public"
                : type.IsNestedFamily
                ? type.IsNestedFamANDAssem ? "private protected" : type.IsNestedFamORAssem ? "protected internal" : "protected"
                : type.IsNestedAssembly ? "internal" : type.IsNestedPrivate ? "private" : null;

        public static string GetCSAccessModifier<T>() => GetCSAccessModifier(typeof(T));

        public static System.Collections.Generic.IEnumerable<Type?> GetDefinedTypes(this Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes;
            }

            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
        }

        public static ArgumentException GetTypeIsNotEnumException(in string paramName) => new($"The current type does not represent an {nameof(Enum)} type.", paramName);

        public static EUT ToDotNetEnumUnderlyingType(this Type type)
        {
            if (type.IsEnum)
            {
                Type t = type.GetEnumUnderlyingType();

                static KeyValuePair<Type, EUT> getType<T>(in EUT enumType) => new(typeof(T), enumType);

                KeyValuePair<Type, EUT>[] types = GetArray(getType<int>(EUT.Int32), getType<uint>(EUT.UInt32), getType<short>(EUT.Int16), getType<ushort>(EUT.UInt16), getType<long>(EUT.Int64), getType<ulong>(EUT.UInt64), getType<byte>(EUT.Byte), getType<sbyte>(EUT.SByte));

                foreach (KeyValuePair<Type, EUT> underlyingType in types)

                    if (t == underlyingType.Key)

                        return underlyingType.Value;
            }

            throw GetTypeIsNotEnumException(nameof(type));
        }

        public static string ToCSName(this EUT value) => value switch
        {
            EUT.Int32 => "int",
            EUT.UInt32 => "uint",
            EUT.Int16 => "short",
            EUT.UInt16 => "ushort",
            EUT.Int64 => "long",
            EUT.UInt64 => "ulong",
            EUT.Byte => "byte",
            EUT.SByte => "sbyte",
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        };
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
                    FieldInfo? current = InnerEnumerator.Current;

                    if (current != null)
                    {
                        object? value = current.GetRawConstantValue();

                        if (value != null)
                        {
                            value = Convert.ChangeType(value, UnderlyingType);

                            DotNetEnumValue getCurrent(in long? value, in ulong? uValue) => new(current.Name, value, uValue);

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

    public class DotNetNamespace : IComparable<DotNetNamespace>
    {
        public DotNetPackage DotNetPackage { get; }

        public DotNetNamespace? Parent { get; }

        public string Name { get; }

        public string Path
        {
            get
            {
                LinkedList<string> stringBuilder = new();

                DotNetNamespace? parent = Parent;

                _ = stringBuilder.AddFirst(Name);

                while (parent != null)
                {
                    _ = stringBuilder.AddFirst(parent.Name);

                    parent = parent.Parent;
                }

                return string.Join('.', stringBuilder);
            }
        }

        public DotNetNamespace(in DotNetPackage dotNetPackage, in DotNetNamespace? parent, in string name)
        {
            DotNetPackage = dotNetPackage ?? throw GetArgumentNullException(nameof(dotNetPackage));

            Parent = parent;

            ThrowIfNullEmptyOrWhiteSpace(name);

            Name = name;
        }

        public System.Collections.Generic.IEnumerable<DotNetNamespace> GetSubnamespaces() => new Enumerable<string>(() => new NamespaceEnumerator(DotNetPackage.Assembly, type => type.IsPublic, null)).Where(@namespace => @namespace.Length > Path.Length + 1 && @namespace.StartsWith(Path + '.') && !@namespace[(Path.Length + 1)..].Contains('.')).Select(@namespace => new DotNetNamespace(DotNetPackage, this, @namespace[(@namespace.LastIndexOf('.') + 1)..]));

        public System.Collections.Generic.IEnumerable<T> GetTypes<T>(Predicate<Type> predicate, in Func<Type, T> func) => func == null ? throw GetArgumentNullException(nameof(func)) : DotNetPackage.Assembly.GetDefinedTypes().WhereSelect(type => type != null && type.IsPublic && !type.IsNested && predicate(type) && type.Namespace == Path, func);

        public System.Collections.Generic.IEnumerable<DotNetEnum> GetEnums() => GetTypes(type => type.IsEnum, type => new DotNetEnum(type));

        public System.Collections.Generic.IEnumerable<DotNetType> GetTypes()
        {
            return GetEnums();

            // TODO
        }

        public int CompareTo(DotNetNamespace? other) => other == null ? 1 : Path.CompareTo(other.Path);

        public override string ToString() => Path;
    }

    public class DotNetPackage : System.Collections.Generic.IEnumerable<DotNetNamespace> //: IRecursiveEnumerableProviderEnumerable<DotNetItem>
    {
        public string Path { get; }

        public Assembly? Assembly { get; private set; }

        public bool IsOpen => Assembly != null;

        public DotNetPackage(in string path) => Path = path ?? throw GetArgumentNullException(nameof(path));

        private static InvalidOperationException GetPackageOpenStatusException(in string msg) => new($"The package is {msg} open.");

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
