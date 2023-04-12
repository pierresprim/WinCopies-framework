using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Linq;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Util;

using static WinCopies.Reflection.DotNetDocBuilder.Writer.FieldUpdater;
using static WinCopies.Reflection.DotNetDocBuilder.Writer.FieldAdderParameters;
using static WinCopies.Reflection.DotNetDocBuilder.Writer.EventAdderParameters;
using static WinCopies.Reflection.DotNetDocBuilder.Writer.PropertyAdderParameters;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        internal sealed class FieldUpdaterStruct : System.IDisposable
        {
            #region Fields
            public readonly Writer Writer;
            public readonly DBEntityCollection<Field> Fields;
            public readonly DBEntityCollection<Property> Properties;
            public readonly DBEntityCollection<FieldModifier> FieldModifiers;
            public readonly DBEntityCollection<Member> Members;
            public readonly DBEntityCollection<AccessModifier> AccessModifiers;
            public readonly DBEntityCollection<ClassModifier> ClassModifiers;
            public readonly DBEntityCollection<Type> Types;
            public readonly DBEntityCollection<MemberType> MemberTypes;
            public readonly DBEntityCollection<Namespace> Namespaces;
            public readonly DBEntityCollection<TypeType> TypeTypes;
            public readonly DBEntityCollection<Class> Classes;
            #endregion Fields

            public FieldUpdaterStruct(in Writer writer)
            {
                ISQLConnection connection = (Writer = writer).Connection;

                DBEntityCollection<T> getCollection<T>() where T : IEntity => new
#if !CS9
                    DBEntityCollection<T>
#endif
                    (connection);

                Fields = getCollection<Field>();
                Properties = getCollection<Property>();
                FieldModifiers = getCollection<FieldModifier>();
                Members = getCollection<Member>();
                AccessModifiers = getCollection<AccessModifier>();
                ClassModifiers = getCollection<ClassModifier>();
                Types = getCollection<Type>();
                MemberTypes = getCollection<MemberType>();
                Namespaces = getCollection<Namespace>();
                TypeTypes = getCollection<TypeType>();
                Classes = getCollection<Class>();
            }

            public void Dispose()
            {
                Fields.Dispose();
                Properties.Dispose();
                FieldModifiers.Dispose();
                Members.Dispose();
                AccessModifiers.Dispose();
                ClassModifiers.Dispose();
                Types.Dispose();
                MemberTypes.Dispose();
                Namespaces.Dispose();
                TypeTypes.Dispose();
                Classes.Dispose();
            }
        }

        private sealed class FieldUpdaterCommonDataStruct<T>
        {
            private readonly Converter<T, Type> _converter;

            public Member Member;
            public MemberType MemberType;
            public System.Type Type;
            public Type MemberDBType;
            public ulong TypeId;
            public Type DBType;
            public uint Tables;

            public T Item { set => TypeId = (DBType = _converter(value)).Id; }

            public FieldUpdaterCommonDataStruct(in Converter<T, Type> converter) => _converter = converter;
        }

        private sealed class FieldUpdaterDisposableStruct<T> : DotNetFix.IDisposable
        {
            private FieldUpdaterStruct _data;

            public FieldUpdaterCommonDataStruct<T> CommonData { get; private set; }

            public FieldUpdaterStruct Data => _data;

            public bool IsDisposed => _data == null;

            public FieldUpdaterDisposableStruct(in Writer writer, in Converter<T, Type> converter)
            {
                _data = new FieldUpdaterStruct(writer);
                CommonData = new FieldUpdaterCommonDataStruct<T>(converter);
            }

            public void Dispose()
            {
                if (IsDisposed)

                    return;

                CommonData = null;

                UtilHelpers.Dispose(ref _data);
            }
        }

        private interface IAdderParameters<T, U> where T : IEntity
        {
            DBEntityCollection<T> Collection { get; }

            T Func();

            void Action(T t, U u);

            System.Type ConvertType(U u);

            bool IsPublic(U u);
        }

        internal abstract class AdderParameters<T, U> : IAdderParameters<T, U> where T : IEntity
        {
            protected readonly FieldUpdaterStruct Data;

            public abstract DBEntityCollection<T> Collection { get; }

            public AdderParameters(in FieldUpdaterStruct data) => Data = data;

            public abstract void Action(T t, U u);
            public abstract System.Type ConvertType(U u);
            public abstract T Func();
            public abstract bool IsPublic(U u);
        }

        internal abstract class FieldAdderParameters<T> : AdderParameters<Field, T>
        {
            public override DBEntityCollection<Field> Collection => Data.Fields;

            public FieldAdderParameters(in FieldUpdaterStruct data) : base(data) { /* Left empty. */ }

            protected FieldModifier GetFieldModifier(in string modifier) => new
#if !CS9
                        FieldModifier
#endif
                        (Data.FieldModifiers)
            { Name = modifier };

            protected abstract FieldModifier
#if CS8
                    ?
#endif
                    GetFieldModifier(in T fieldInfo);

            public override Field Func() => new
#if !CS9
                Field
#endif
                (Data.Fields);

            public override void Action(Field field, T fieldInfo) => field.Modifier = GetFieldModifier(fieldInfo);

            public override abstract System.Type ConvertType(T f);

            public override abstract bool IsPublic(T f);
        }

        internal sealed class FieldAdderParameters : FieldAdderParameters<FieldInfo>
        {
            public FieldAdderParameters(in FieldUpdaterStruct data) : base(data) { /* Left empty. */ }

            public static FieldAdderParameters GetFieldAdderParameters(in FieldUpdaterStruct data) => new
#if !CS9
                FieldAdderParameters
#endif
                (data);

            protected override FieldModifier
#if CS8
                ?
#endif
                GetFieldModifier(in FieldInfo fieldInfo) => fieldInfo.IsStatic ? GetFieldModifier("static") : fieldInfo.IsInitOnly ? GetFieldModifier("readonly") : null;

            public override System.Type ConvertType(FieldInfo f) => f.FieldType;

            public override bool IsPublic(FieldInfo f) => f.IsPublic;
        }

        internal sealed class EventAdderParameters : FieldAdderParameters<EventInfo>
        {
            public EventAdderParameters(in FieldUpdaterStruct data) : base(data) { /* Left empty. */ }

            public static EventAdderParameters GetEventAdderParameters(in FieldUpdaterStruct data) => new
#if !CS9
                EventAdderParameters
#endif
                (data);

            protected override FieldModifier GetFieldModifier(in EventInfo eventInfo) => GetFieldModifier("event");

            public override System.Type ConvertType(EventInfo e) => e.EventHandlerType;

            public override bool IsPublic(EventInfo e) => EventPredicate(e);
        }

        internal sealed class PropertyAdderParameters : AdderParameters<Property, PropertyInfo>
        {
            public override DBEntityCollection<Property> Collection => Data.Properties;

            public PropertyAdderParameters(in FieldUpdaterStruct data) : base(data) { /* Left empty. */ }

            public static PropertyAdderParameters GetPropertyAdderParameters(in FieldUpdaterStruct data) => new
#if !CS9
                PropertyAdderParameters
#endif
                (data);

            public override Property Func() => new
#if !CS9
                Property
#endif
                (Data.Properties);

            public override void Action(Property property, PropertyInfo propertyInfo)
            {
                ClassModifier
#if CS8
                    ?
#endif
                    getClassModifier()
                {
                    ClassModifier _getClassModifier(in string modifier) => new
#if !CS9
                               ClassModifier
#endif
                               (Data.ClassModifiers)
                    { Name = modifier };

                    foreach (MethodInfo m in propertyInfo.GetAccessors(true))

                        if (m.IsAbstract)

                            _getClassModifier("abstract");

                        else if (m.IsStatic)

                            _getClassModifier("static");

                        else if (m.IsFinal)

                            _getClassModifier("sealed");

                    return null;
                }

                property.Modifier = getClassModifier();
            }

            public override System.Type ConvertType(PropertyInfo u) => u.PropertyType;

            public override bool IsPublic(PropertyInfo p) => PropertyPredicate(p);
        }

        internal class FieldUpdater
        {
            internal static bool PropertyPredicate(in PropertyInfo p, in Predicate<MethodInfo> predicate) => p.GetAccessors(true).AnyPredicate(predicate);
            internal static bool PropertyPredicate(PropertyInfo p) => PropertyPredicate(p, m => m.IsPublic);

            internal static bool EventPredicate(in EventInfo e, Predicate<MethodInfo> predicate) => new MethodInfo[] { e.AddMethod, e.RemoveMethod }.AnyPredicate(predicate);
            internal static bool EventPredicate(EventInfo e) => EventPredicate(e, m => m.IsPublic);
        }

        protected class FieldUpdater<TDBType, TDotNetType> : IWriterCallback<TDBType, TDotNetType>, DotNetFix.IDisposable where TDBType : IEntity where TDotNetType : DotNetType
        {
            private FieldUpdaterDisposableStruct<TDBType> _data;

            private FieldUpdaterDisposableStruct<TDBType> Data => _data ?? throw ThrowHelper.GetExceptionForDispose(false);

            public bool IsDisposed => _data.IsDisposed;

            public FieldUpdater(in Writer writer, in Converter<TDBType, Type> converter) => _data = new FieldUpdaterDisposableStruct<TDBType>(writer, converter);

            private static IEnumerable<FieldInfo> GetRealFields(in DotNetType dotNetType) => dotNetType.Type.GetRealFields();
            private static IEnumerable<EventInfo> GetEvents(in DotNetType dotNetType) => dotNetType.Type.GetEvents();
            private static IEnumerable<PropertyInfo> GetProperties(in DotNetType dotNetType) => dotNetType.Type.GetProperties();

            private static bool FieldPredicate(FieldInfo f) => f.IsPublic || f.IsFamily || f.IsFamilyOrAssembly;
            private static bool MethodPredicate(MethodInfo m) => m.IsPublic || m.IsFamily || m.IsFamilyOrAssembly;
            private static bool PropertyPredicate(PropertyInfo p) => FieldUpdater.PropertyPredicate(p, MethodPredicate);
            private static bool EventPredicate(EventInfo e) => FieldUpdater.EventPredicate(e, MethodPredicate);

            protected bool DefaultPredicate<T>(in T f) where T : IMember => f.Member.Type.Id == Data.CommonData.TypeId;

            protected void Log(in string
#if CS8
                ?
#endif
                message, in bool? increment, in ConsoleColor? consoleColor = null) => Data.Data.Writer.Logger(message, increment, consoleColor);

            private void AddOrUpdate<T, U>(in T field, in U fieldInfo, in FieldUpdaterStruct data, FieldUpdaterCommonDataStruct<TDBType> commonData, in IAdderParameters<T, U> parameters) where T : IEntity
            {
                parameters.Action(field, fieldInfo);

                SetAccessModifier(data.AccessModifiers, parameters.IsPublic(fieldInfo), commonData.Member);

                void log(in string message) => Log($"{commonData.Type} is {message}.", null);

                if (_data.Data.Writer.IsTypeValid(commonData.Type = parameters.ConvertType(fieldInfo)))
                {
                    log($"declared in a known assembly. It will be referenced in {field} through {typeof(T)}.{nameof(IMember.MemberType)}.{nameof(MemberType.Type)}");

                    commonData.MemberType.Type = commonData.MemberDBType = new Type(data.Types) { GenericTypeCount = commonData.Type.GetRealGenericTypeParameterLength() };

                    data.Writer.UpdateType(commonData.Type, commonData.MemberDBType, data.Types, data.AccessModifiers, data.Namespaces, data.TypeTypes, data.Classes);
                }

                else
                {
                    log("not declared in a known assembly. It won't be referenced as a known type");

                    commonData.MemberType.FieldType = fieldInfo.ToString();
                }
            }

            private void Add<T, U>(in U fieldInfo, in Type type, in FieldUpdaterStruct data, in FieldUpdaterCommonDataStruct<TDBType> commonData, in ConverterIn<FieldUpdaterStruct, IAdderParameters<T, U>> parameters) where T : IMember where U : MemberInfo
            {
                Log($"Adding {fieldInfo}.\nInitializing {typeof(T)}.", true);

                IAdderParameters<T, U> _parameters = parameters(data);
                T field = _parameters.Func();

                field.Member = commonData.Member = new Member(data.Members) { Type = type };
                field.MemberType = commonData.MemberType = new MemberType(data.MemberTypes);

                AddOrUpdate(field, fieldInfo, data, commonData, _parameters);

                Log($"Added {_parameters.Collection.Add(field, out commonData.Tables, out ulong rows)} rows in {commonData.Tables} tables.", false);
            }

            private void Remove<T>(in T entity) where T : IEntity => Log($"Removed {entity.Remove(out Data.CommonData.Tables)} rows in {_data.CommonData.Tables} tables.", null);

            public void OnGetItem(TDBType item, Type type, TDotNetType @enum, DBEntityCollection<TDBType> enums) { /* Left empty. */ }

            public void OnAdded(TDBType item, Type type, TDotNetType dotNetType)
            {
                Log("Adding members.", true);

                FieldUpdaterDisposableStruct<TDBType> _data = Data;
                FieldUpdaterStruct data = _data.Data;
                FieldUpdaterCommonDataStruct<TDBType> commonData = _data.CommonData;

                void _parse<T, U>(in IEnumerable<U> fieldsFunc, ConverterIn<FieldUpdaterStruct, IAdderParameters<T, U>> parameters) where T : IMember where U : MemberInfo => fieldsFunc.ForEach((in U fieldInfo) => Add(fieldInfo, type, data, commonData, parameters));

                void parse<T, U>(in IEnumerable<U> fieldsFunc, ConverterIn<FieldUpdaterStruct, IAdderParameters<T, U>> parameters) where T : IMember where U : MemberInfo
                {
                    Log($"Adding all {typeof(T)}.", true);

                    _parse(fieldsFunc, parameters);

                    Log($"Added all {typeof(T)}.", false);
                }

                parse(GetRealFields(dotNetType).Where(FieldPredicate), GetFieldAdderParameters);
                parse(GetEvents(dotNetType).Where(EventPredicate), GetEventAdderParameters);
                parse(GetProperties(dotNetType).Where(PropertyPredicate), GetPropertyAdderParameters);

                Log("Added members.", false);
            }

            public void OnUpdated(TDBType item, TDotNetType dotNetType, DBEntityCollection<TDBType> collection)
            {
                Log("Updating members.", true);

                FieldUpdaterDisposableStruct<TDBType> _data = Data;
                FieldUpdaterStruct data = _data.Data;
                FieldUpdaterCommonDataStruct<TDBType> commonData = _data.CommonData;

                commonData.Item = item;

                void parse<T, U>(in DBEntityCollection<T> values, in IEnumerable<U> fields, in ConverterIn<FieldUpdaterStruct, IAdderParameters<T, U>> converter) where T : IMember where U : MemberInfo
                {
                    U
#if CS9
                        ?
#endif
                        fieldInfo;

                    foreach (T field in values.Where(f => f.Member.Type.Id == commonData.TypeId))

                        if ((fieldInfo = fields.FirstOrDefault(f => f.Name == dotNetType.Name)) == null)

                            Remove(field);

                        else

                            AddOrUpdate<T, U>(field, fieldInfo, data, commonData, converter(data));

                    foreach (U field in fields) Add(field, commonData.DBType, data, commonData, converter);
                }

                parse(data.Fields, GetRealFields(dotNetType), GetFieldAdderParameters);
                parse(data.Fields, GetEvents(dotNetType), GetEventAdderParameters);
                parse(data.Properties, GetProperties(dotNetType), GetPropertyAdderParameters);

                Log("Updated members.", true);
            }

            public void OnDeleting(TDBType item, Type type)
            {
                FieldUpdaterDisposableStruct<TDBType> _data = Data;
                FieldUpdaterStruct data = _data.Data;
                FieldUpdaterCommonDataStruct<TDBType> commonData = _data.CommonData;

                commonData.Item = item;

                data.Fields.AppendValues<IMember>(data.Properties).ForEach(DefaultPredicate, Remove); // Delete all fields, events and properties from item.
            }

            protected virtual void Dispose(bool disposing)
            {
                if (IsDisposed)

                    return;

                _data.Dispose();
                _data = null;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
