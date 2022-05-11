﻿using System.Text;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    [Entity("docnamespace")]
    public class Namespace : DefaultDBEntity<Namespace, ulong>
    {
        private int _frameworkId;
        private string _name;
        private ulong? _parentId;

        [EntityProperty]
        public int FrameworkId { get => TryRefreshAndGet(ref _frameworkId); set => _frameworkId = value; }

        [EntityProperty(IsPseudoId = true)]
        public string Name { get => TryRefreshAndGet(ref _name); set => _name = value; }

        [EntityProperty(IsPseudoId = true)]
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
    public class TypeType : DefaultNamedEntity2<TypeType, ulong>
    {
        public TypeType(DBEntityCollection<TypeType> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("class_access_modifier")]
    public class AccessModifier : DefaultNamedEntity2<AccessModifier, ulong>
    {
        public AccessModifier(DBEntityCollection<AccessModifier> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("doctype")]
    public class Type : DefaultDBEntity<Type, ulong>
    {
        private string _name;
        private Namespace _namespace;
        private TypeType _typeType;
        private AccessModifier _accessModifier;
        private byte _genericTypeCount;
        private Class _parentType;
        private string _comment;
        private string _commentRemarks;
        private string _toStringValue;

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

        [EntityProperty]
        [ForeignKey]
        public AccessModifier AccessModifier { get => TryRefreshAndGet(ref _accessModifier); set => _accessModifier = value; }

        [EntityProperty(IsPseudoId = true)]
        public byte GenericTypeCount { get => TryRefreshAndGet(ref _genericTypeCount); set => _genericTypeCount = value; }

        [EntityProperty(IsPseudoId = true)]
        [ForeignKey]
        public Class
#if CS8
            ?
#endif
            ParentType
        { get => TryRefreshAndGet(ref _parentType); set => _parentType = value; }

        [EntityProperty]
        public string Comment { get => TryRefreshAndGet(ref _comment); set => _comment = value; }

        [EntityProperty]
        public string CommentRemarks { get => TryRefreshAndGet(ref _commentRemarks); set => _commentRemarks = value; }

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
    public class UnderlyingType : DefaultNamedEntity2<UnderlyingType, ulong>
    {
        public UnderlyingType(DBEntityCollection<UnderlyingType> collection) : base(collection) { /* Left empty. */ }
    }

    public class TypeBase<T> : DefaultDBEntity<T, ulong> where T : IEntity
    {
        private Type _type;

        [EntityProperty(
#if CS10
            $"{nameof(Type)}Id"
#else
            "TypeId"
#endif
            , IsPseudoId = true)]
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

        public override string ToString() => Type?.ToString();
    }

    public interface IInterfaceImplementation : IEntity
    {
        Type ImplementorType { get; }

        Type GetImplementedInterface();
    }

    public abstract class InterfaceImplementation<T> : DefaultDBEntity<T, ulong>, IInterfaceImplementation where T : IEntity
    {
        private Type _implementedInterface;

        protected Type ImplementedInterface { get => TryRefreshAndGet(ref _implementedInterface); set => _implementedInterface = value; }

        public abstract Type ImplementorType { get; }

        public InterfaceImplementation(DBEntityCollection<T> collection) : base(collection) { /* Left empty. */ }

        public Type GetImplementedInterface() => ImplementedInterface;

        public void SetImplementedInterface(Type type) => ImplementedInterface = type;
    }

    [Entity("class_modifier")]
    public class ClassModifier : DefaultNamedEntity2<ClassModifier, ulong>
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

        [EntityProperty(IsPseudoId = true)]
        [ForeignKey]
        public Class Class { get => TryRefreshAndGet(ref _class); set => _class = value; }

        [EntityProperty(IsPseudoId = true)]
        [ForeignKey]
        public Type Interface { get => ImplementedInterface; set => ImplementedInterface = value; }

        public override Type ImplementorType => Class.Type;

        public ClassInterfaceImplementation(DBEntityCollection<ClassInterfaceImplementation> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("interfaceinterfaceimplementations")]
    public class InterfaceImplementation : InterfaceImplementation<InterfaceImplementation>
    {
        private Type _interface;

        [EntityProperty(IsPseudoId = true)]
        [ForeignKey]
        public Type Interface { get => TryRefreshAndGet(ref _interface); set => _interface = value; }

        [EntityProperty(IsPseudoId = true)]
        [ForeignKey]
        public new Type ImplementedInterface { get => base.ImplementedInterface; set => base.ImplementedInterface = value; }

        public override Type ImplementorType => Interface;

        public InterfaceImplementation(DBEntityCollection<InterfaceImplementation> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docgenerictypemodifier")]
    public class GenericTypeModifier : DefaultNamedEntity2<GenericTypeModifier, ulong>
    {
        public GenericTypeModifier(DBEntityCollection<GenericTypeModifier> collection) : base(collection) { /* Left empty. */ }
    }

    [Entity("docgenerictype")]
    public class GenericType : DefaultNamedEntity<GenericType, ulong>
    {
        private Type _type;
        private GenericTypeModifier _modifier;

        [EntityProperty("DocType")]
        [ForeignKey]
        public Type Type { get => TryRefreshAndGet(ref _type); set => _type = value; }

        [EntityProperty]
        [ForeignKey]
        public GenericTypeModifier Modifier { get => TryRefreshAndGet(ref _modifier); set => _modifier = value; }

        public GenericType(DBEntityCollection<GenericType> collection) : base(collection) { /* Left empty. */ }
    }
}
