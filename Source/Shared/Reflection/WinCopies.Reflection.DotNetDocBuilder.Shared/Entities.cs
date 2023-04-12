using System.Text;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Reflection.DotNetParser;

using static WinCopies.EntityFramework.IdStatus;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public class DefaultDBEntity<T> : DefaultDBEntity<T, ulong> where T : IEntity
    {
        public DefaultDBEntity(DBEntityCollection<T> collection) : base(collection) { /* Left empty. */ }
    }

    public class DefaultNamedEntity2<T> : DefaultNamedEntity2<T, ulong> where T : IEntity
    {
        public DefaultNamedEntity2(DBEntityCollection<T> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docnamespace")]
    public class Namespace : DefaultDBEntity<Namespace>
    {
        private int _frameworkId;
        private string _name;
        private ulong? _parentId;

        [EntityProperty]
        public int FrameworkId { get => TryRefreshAndGet(ref _frameworkId); set => _frameworkId = value; }

        [EntityProperty(IdStatus = PseudoId)]
        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        [EntityProperty(IdStatus = PseudoId)]
        public ulong? ParentId { get => TryRefreshAndGet(ref _parentId); set => _parentId = value; }

        public Namespace(DBEntityCollection<Namespace> collection) : base(collection) { /* Left empty. */ }

        public string ToString(ISQLConnection connection) => Writer.GetWholeNamespace(Id, connection);

        public override string ToString()
        {
            using
#if !CS8
            (
#endif
                ISQLConnection connection = Connection.GetConnection()
#if CS8
                ;
#else
            )
#endif
            return ToString(connection);
        }
    }

    [Entity("typetype")]
    public class TypeType : DefaultNamedEntity2<TypeType>
    {
        public TypeType(DBEntityCollection<TypeType> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("class_access_modifier")]
    public class AccessModifier : DefaultNamedEntity2<AccessModifier>
    {
        public AccessModifier(DBEntityCollection<AccessModifier> collection) : base(collection) { /* Left empty. */ }
    }

    public class DocItem<T> : DefaultNamedEntity2<T> where T : IEntity
    {
        private string _comment;
        private string _commentRemarks;

        [EntityProperty]
        public string Comment { get => TryRefreshAndGet(ref _comment); set => _comment = value; }

        [EntityProperty]
        public string CommentRemarks { get => TryRefreshAndGet(ref _commentRemarks); set => _commentRemarks = value; }

        public DocItem(DBEntityCollection<T> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("doctype")]
    public class Type : DocItem<Type>
    {
        private Namespace _namespace;
        private TypeType _typeType;
        private AccessModifier _accessModifier;
        private byte _genericTypeCount;
        private Class _parentType;
        private string _toStringValue;

        [EntityProperty(
#if CS10
            $"{nameof(Namespace)}Id"
#else
            "NamespaceId"
#endif
            , IdStatus = PseudoId)]
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

        [EntityProperty]
        [ForeignKey]
        public AccessModifier AccessModifier { get => TryRefreshAndGet(ref _accessModifier); set => _accessModifier = value; }

        [EntityProperty(IdStatus = PseudoId)]
        public byte GenericTypeCount { get => TryRefreshAndGet(ref _genericTypeCount); set => _genericTypeCount = value; }

        [EntityProperty(IdStatus = PseudoId)]
        [ForeignKey]
        public Class
#if CS8
            ?
#endif
            ParentType
        { get => TryRefreshAndGet(ref _parentType); set => _parentType = value; }

        public Type(DBEntityCollection<Type> collection) : base(collection) { /* Left empty. */ }

        public override string ToString()
        {
            if (_toStringValue == null)
            {
                Collections.Generic.EnumerableHelper<Type>.IEnumerableStack stack = null;

                ActionIn<Type> action = (in Type _type) =>
                {
                    stack = Collections.Generic.EnumerableHelper<Type>.GetEnumerableStack();

                    action = (in Type __type) => stack.Push(__type);
                };

                foreach (Type type in Collections.Enumerable.GetNullCheckWhileEnumerableC(this, type => type.ParentType?.Type))

                    action(type);

                var sb = new StringBuilder();

                void appendChar(in char c) => sb.Append(c);

                void appendText(in string text) => sb.Append(text);

                byte genericTypeCount;

                void append(in Type type)
                {
                    appendText(type.Name);

                    if ((genericTypeCount = type.GenericTypeCount) > 0)
                    {
                        appendChar('`');

                        appendText(genericTypeCount.ToString());
                    }
                }

                using
#if !CS8
            (
#endif
                    ISQLConnection connection = Connection.GetConnection()
#if CS8
                    ;
#else
            )
#endif

                appendText(Namespace.ToString(connection));

                appendChar('.');

                if (stack != null)

                    while (stack.TryPop(out Type type))
                    {
                        append(type);

                        appendChar('+');
                    }

                append(this);

                _toStringValue = sb.ToString();
            }

            return _toStringValue;
        }
    }

    [Entity("enumunderlyingtype")]
    public class UnderlyingType : DefaultNamedEntity2<UnderlyingType>
    {
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
            , IdStatus = PseudoId)]
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

        public static Enum GetNewEnum(Type type, DotNetEnum dotNetType, DBEntityCollection<Enum> dBTypes) => new
#if !CS9
            Enum
#endif
            (dBTypes)
        { Type = type };

        public override string ToString() => Type?.ToString();
    }

    public interface IInterfaceImplementation : IEntity
    {
        Type ImplementorType { get; }

        Type GetImplementedInterface();
    }

    public abstract class InterfaceImplementation<T> : DefaultDBEntity<T>, IInterfaceImplementation where T : IEntity
    {
        private Type _implementedInterface;

        protected Type ImplementedInterface { get => TryRefreshAndGet(ref _implementedInterface); set => _implementedInterface = value; }

        public abstract Type ImplementorType { get; }

        public InterfaceImplementation(DBEntityCollection<T> collection) : base(collection) { /* Left empty. */ }

        public Type GetImplementedInterface() => ImplementedInterface;

        public void SetImplementedInterface(Type type) => ImplementedInterface = type;
    }

    [Entity("class_modifier")]
    public class ClassModifier : DefaultNamedEntity2<ClassModifier>
    {
        public ClassModifier(DBEntityCollection<ClassModifier> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docclass")]
    public class Class : TypeBase<Class>
    {
        private ClassModifier
#if CS8
            ?
#endif
            _classModifier;
        private string
#if CS8
            ?
#endif
            _inheritsFrom;
        private Class
#if CS8
            ?
#endif
            _inheritsFromDocType;

        [EntityProperty]
        [ForeignKey]
        public ClassModifier
#if CS8
            ?
#endif
            Modifier
        { get => TryRefreshAndGet(ref _classModifier); set => _classModifier = value; }

        [EntityProperty]
        public string
#if CS8
            ?
#endif
            InheritsFrom
        { get => TryRefreshAndGet(ref _inheritsFrom); set => _inheritsFrom = value; }

        [EntityProperty]
        [ForeignKey]
        public Class
#if CS8
            ?
#endif
            InheritsFromDocType
        { get => TryRefreshAndGet(ref _inheritsFromDocType); set => _inheritsFromDocType = value; }

        public Class(DBEntityCollection<Class> collection) : base(collection) { /* Left empty. */ }

        public override string ToString() => Type?.ToString();
    }

    [Entity("classinterfaceimplementations")]
    public class ClassInterfaceImplementation : InterfaceImplementation<ClassInterfaceImplementation>
    {
        private Class _class;

        [EntityProperty(IdStatus = PseudoId)]
        [ForeignKey]
        public Class Class { get => TryRefreshAndGet(ref _class); set => _class = value; }

        [EntityProperty(IdStatus = PseudoId)]
        [ForeignKey]
        public Type Interface { get => ImplementedInterface; set => ImplementedInterface = value; }

        public override Type ImplementorType => Class.Type;

        public ClassInterfaceImplementation(DBEntityCollection<ClassInterfaceImplementation> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("interfaceinterfaceimplementations")]
    public class InterfaceImplementation : InterfaceImplementation<InterfaceImplementation>
    {
        private Type _interface;

        [EntityProperty(IdStatus = PseudoId)]
        [ForeignKey]
        public Type Interface { get => TryRefreshAndGet(ref _interface); set => _interface = value; }

        [EntityProperty(IdStatus = PseudoId)]
        [ForeignKey]
        public new Type ImplementedInterface { get => base.ImplementedInterface; set => base.ImplementedInterface = value; }

        public override Type ImplementorType => Interface;

        public InterfaceImplementation(DBEntityCollection<InterfaceImplementation> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docgenerictypemodifier")]
    public class GenericTypeModifier : DefaultNamedEntity2<GenericTypeModifier>
    {
        public GenericTypeModifier(DBEntityCollection<GenericTypeModifier> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docgenerictype")]
    public class GenericType : DefaultNamedEntity2<GenericType>
    {
        private Type _type;
        private GenericTypeModifier _modifier;

        [EntityProperty("DocType", IdStatus = PseudoId)]
        [ForeignKey]
        public Type Type { get => TryRefreshAndGet(ref _type); set => _type = value; }

        [EntityProperty]
        [ForeignKey]
        public GenericTypeModifier Modifier { get => TryRefreshAndGet(ref _modifier); set => _modifier = value; }

        public GenericType(DBEntityCollection<GenericType> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docmember")]
    public class Member : DocItem<Member>
    {
        private AccessModifier _accessModifier;
        private Type _type;

        [EntityProperty]
        [ForeignKey]
        public AccessModifier AccessModifier { get => TryRefreshAndGet(ref _accessModifier); set => _accessModifier = value; }

        [EntityProperty("TypeId", IdStatus = PseudoId)]
        [ForeignKey]
        public Type Type { get => TryRefreshAndGet(ref _type); set => _type = value; }

        public Member(DBEntityCollection<Member> collection) : base(collection) { /* Left empty. */ }
    }

    public interface IConst : IEntity
    {
        object Value { get; set; }

        Member Member { get; set; }
    }

    public class Const<TEntity, TValue> : DefaultDBEntity<TEntity>, IConst where TEntity : IEntity
    {
        private TValue _value;
        private Member _memberId;

        [EntityProperty]
        public TValue Value { get => TryRefreshAndGet(ref _value); set => _value = value; }

        object IConst.Value { get => Value; set => Value = (TValue)value; }

        [EntityProperty("MemberId", IdStatus = PseudoId)]
        [ForeignKey(RemoveAlso = true)]
        public Member Member { get => TryRefreshAndGet(ref _memberId); set => _memberId = value; }

        public Const(DBEntityCollection<TEntity> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docconst16")]
    public class Const16 : Const<Const16, ushort>
    {
        public Const16(DBEntityCollection<Const16> collection) : base(collection) { /* Left empty. */ }

        public static Const16 GetNewConstant(in DBEntityCollection<Const16> dbec) => new
#if !CS9
            Const16
#endif
            (dbec);
    }

    [Entity("docconst64")]
    public class Const64 : Const<Const64, ulong>
    {
        public Const64(DBEntityCollection<Const64> collection) : base(collection) { /* Left empty. */ }

        public static Const64 GetNewConstant(in DBEntityCollection<Const64> dbec) => new
#if !CS9
            Const64
#endif
            (dbec);
    }

    [Entity("docconstfp32")]
    public class ConstFP32 : Const<ConstFP32, float>
    {
        public ConstFP32(DBEntityCollection<ConstFP32> collection) : base(collection) { /* Left empty. */ }

        public static ConstFP32 GetNewConstant(in DBEntityCollection<ConstFP32> dbec) => new
#if !CS9
            ConstFP32
#endif
            (dbec);
    }

    [Entity("docconstfp64")]
    public class ConstFP64 : Const<ConstFP64, double>
    {
        public ConstFP64(DBEntityCollection<ConstFP64> collection) : base(collection) { /* Left empty. */ }

        public static ConstFP64 GetNewConstant(in DBEntityCollection<ConstFP64> dbec) => new
#if !CS9
            ConstFP64
#endif
            (dbec);
    }

    [Entity("docconstfp128")]
    public class ConstFP128 : Const<ConstFP128, decimal>
    {
        public ConstFP128(DBEntityCollection<ConstFP128> collection) : base(collection) { /* Left empty. */ }

        public static ConstFP128 GetNewConstant(in DBEntityCollection<ConstFP128> dbec) => new
#if !CS9
            ConstFP128
#endif
            (dbec);
    }

    [Entity("docconststring")]
    public class ConstString : Const<ConstString, decimal>
    {
        public ConstString(DBEntityCollection<ConstString> collection) : base(collection) { /* Left empty. */ }

        public static ConstString GetNewConstant(in DBEntityCollection<ConstString> dbec) => new
#if !CS9
            ConstString
#endif
            (dbec);
    }

    [Entity("field_modifier")]
    public class FieldModifier : DefaultNamedEntity2<FieldModifier>
    {
        public FieldModifier(DBEntityCollection<FieldModifier> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("membertype")]
    public class MemberType : DefaultDBEntity<MemberType>
    {
        private Type _type;
        private string __type;

        [EntityProperty("TypeId", IdStatus = PseudoId)]
        [ForeignKey]
        public Type Type { get => TryRefreshAndGet(ref _type); set => _type = value; }

        [EntityProperty(nameof(Type), IdStatus = PseudoId)]
        public string FieldType { get => TryRefreshAndGet(ref __type); set => __type = value; }

        public MemberType(DBEntityCollection<MemberType> collection) : base(collection) { /* Left empty. */ }
    }

    public interface IMember : IEntity
    {
        MemberType MemberType { get; set; }

        Member Member { get; set; }
    }

    public class MemberBase<TMember, TModifier> : DefaultDBEntity<TMember>, IMember where TMember : IEntity
    {
        private MemberType _memberType;
        private Member _member;
        private TModifier
#if CS9
            ?
#endif
            _modifier;

        [EntityProperty("MemberTypeId")]
        [ForeignKey(RemoveAlso = true)]
        public MemberType MemberType { get => TryRefreshAndGet(ref _memberType); set => _memberType = value; }

        [EntityProperty("MemberId", IdStatus = PseudoId)]
        [ForeignKey(RemoveAlso = true)]
        public Member Member { get => TryRefreshAndGet(ref _member); set => _member = value; }

        [EntityProperty]
        [ForeignKey]
        public TModifier
#if CS9
            ?
#endif
            Modifier
        { get => TryRefreshAndGet(ref _modifier); set => _modifier = value; }

        public MemberBase(DBEntityCollection<TMember> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docfield")]
    public class Field : MemberBase<Field, FieldModifier>
    {
        public Field(DBEntityCollection<Field> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docproperty")]
    public class Property : MemberBase<Property, ClassModifier>, IMember
    {
        public Property(DBEntityCollection<Property> collection) : base(collection) { /* Left empty. */ }
    }
}
