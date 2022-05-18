using System;
using System.Collections.Generic;
using System.Linq;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Linq;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Util;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        protected void UpdateTypes<TType, TDBType, TInterfaceImplementation>(
            string typeName,
            DBEntityCollection<TInterfaceImplementation> interfaceImplementations,
            IEnumerable<IEnumerable<IInterfaceImplementation>> otherInterfaceImplementations,
            Converter<TDBType, Type> convertFrom,
            Func<TType, Type, DBEntityCollection<TDBType>, TDBType> convertTo,
            Converter<TDBType, TInterfaceImplementation> interfaceImplementationInitializer,
            Action<TType, TDBType, DBEntityCollection<TDBType>>
#if CS8
            ?
#endif
            update,
            IEnumerable<TType> dotNetInterfaces,
            Action<TType, ulong> action,
            Action<TDBType>
#if CS8
            ?
#endif
            onRemove) where TType : DotNetType where TInterfaceImplementation : InterfaceImplementation<TInterfaceImplementation> where TDBType : IDefaultEntity<ulong>
        {
            using
#if !CS8
                (
#endif
                var types = GetCollection<Type>()
#if CS8
                ;
#else
                )
#endif
            using
#if !CS8
                (
#endif
                var namespaces = GetCollection<Namespace>()
#if CS8
                ;
#else
                )
            {
#endif
            Collections.DotNetFix.Generic.IEnumerableQueue<System.Type> interfaces = new Collections.DotNetFix.Generic.EnumerableQueue<System.Type>();
            IEnumerable<System.Type> _interfaces;
            Type
#if CS8
            ?
#endif
                t;
            long? id;
            TInterfaceImplementation interfaceImplementation;
            Type tmpType;

            IEnumerable<System.Type> _getInterfaces(in System.Type type) => type.GetInterfaces().Where(DotNetNamespace.DefaultTypePredicate);

            void getInterfaces(in System.Type type)
            {
                foreach (System.Type i in _getInterfaces(type))

                    getInterfaces(i);

                if (!interfaces.Contains(type))

                    interfaces.Enqueue(type);
            }

            void updateInterfaces(in DotNetType _t)
            {
                foreach (System.Type i in _interfaces = _getInterfaces(_t.Type))

                    foreach (System.Type _i in _getInterfaces(i))

                        getInterfaces(_i);
            }

            bool checkAll(in Type _type, in DotNetType item) => CheckTypeEquality2(_type, item, true);

            bool checkInterface(System.Type _i) => ValidPackages.Contains(_i.Assembly.GetName().Name) && !interfaces.Contains(_i);

            bool checkInterfaceImplementation(in IInterfaceImplementation _i, in System.Type i) => checkAll(_i.GetImplementedInterface(), new DotNetType(i));

            UpdateItemsStruct updateItemsStruct = new UpdateItemsStruct();

            void addEntityAssociations(TDBType type, TType @interface, in bool check)
            {
                Logger($"Adding entity-associations for {@interface}", true);

                updateInterfaces(@interface);

                void add(in System.Type i)
                {
                    (t = new Type(types)
                    {
                        Namespace = new Namespace(namespaces) { Id = GetNamespaceId(i.Namespace).Value, FrameworkId = PackageInfo.FrameworkId },
                        Name = i.GetRealName(),
                        GenericTypeCount = i.GetRealGenericTypeParameterLength()
                    }).MarkForRefresh();

                    UpdateParentType(i, t, updateItemsStruct.TColl, updateItemsStruct.AMColl, updateItemsStruct.NColl, updateItemsStruct.TTColl, updateItemsStruct.CColl);

                    //t.Namespace.Refresh();

                    t.TryRefreshId(true);

                    interfaceImplementation = interfaceImplementationInitializer(type);
                    interfaceImplementation.SetImplementedInterface(t);

                    id = interfaceImplementations.Add(interfaceImplementation, out uint tables, out ulong rows);

                    Logger($"Added entity-association for {@interface} : {i}. Added {rows} {nameof(rows)} in {tables} {nameof(tables)} (last inserted id: {id}).", null);
                }

                ActionIn<System.Type> _add = check ? (in System.Type i) =>
                {
                    System.Type __i = i;

                    if (!interfaceImplementations.Where(_i => checkAll(_i.ImplementorType, @interface)).Any(_i => checkInterfaceImplementation(_i, __i)))

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

                Logger($"Added entity-associations for {@interface}", false);
            }

            void removeEntityAssociations(in TDBType type, in TType
#if CS8
            ?
#endif
                @interface)
            {
                tmpType = convertFrom(type);

                Logger($"Removing entity-associations for {tmpType.Name}.", true);

                bool defaultPredicate(IInterfaceImplementation i) => i.ImplementorType.Id == tmpType.Id;

                Predicate<IInterfaceImplementation> predicate;

                if (@interface == null)

                    predicate = i => defaultPredicate(i) || i.GetImplementedInterface().Id == tmpType.Id;

                else
                {
                    updateInterfaces(@interface);

                    predicate = i => defaultPredicate(i) && !_interfaces.Any(_i => checkInterface(_i) && checkInterfaceImplementation(i, _i));
                }

                void remove(in IEnumerable<IInterfaceImplementation> _interfaceImplementations)
                {
                    foreach (IInterfaceImplementation _interfaceImplementation in _interfaceImplementations.WherePredicate(predicate))

                        Logger($"Removed {_interfaceImplementation.Remove(out uint tables)} rows in {tables} {nameof(tables)} for {_interfaceImplementation.ImplementorType} : {_interfaceImplementation.GetImplementedInterface()}.", null);
                }

                remove(interfaceImplementations);

                if (otherInterfaceImplementations != null)

                    foreach (IEnumerable<IInterfaceImplementation> _otherInterfaceImplementations in otherInterfaceImplementations)

                        remove(_otherInterfaceImplementations);

                interfaces.Clear();

                Logger($"Removed entity-associations for {tmpType.Name}.", false);
            }

            void removeEntityAssociations2(in TDBType _t) => removeEntityAssociations(_t, null);

            ActionIn<TDBType> _onRemove = onRemove == null ?
#if !CS9
                    (ActionIn<TDBType>)
#endif
                removeEntityAssociations2 : (in TDBType _t) =>
            {
                removeEntityAssociations2(_t);

                onRemove(_t);
            };

            void _update(TType type, TDBType dbType, DBEntityCollection<TDBType> collection)
            {
                // setGenericTypeCount(type, tmpType = convertFrom(dbType));

                removeEntityAssociations(dbType, type);

                addEntityAssociations(dbType, type, true);
            }

            Action<TType, TDBType, DBEntityCollection<TDBType>> _action = update == null
                ?
#if !CS9
                    (Action<TType, TDBType, DBEntityCollection<TDBType>>)
#endif
                _update
                : (type, dbType, collection) =>
                    {
                        _update(type, dbType, collection);

                        update(type, dbType, collection);
                    };

            UpdateItems(typeName, true, type => convertFrom(type).TypeType.Name == typeName, (type, @interface) => addEntityAssociations(type, @interface, false), _t => _onRemove(_t), convertFrom, () => dotNetInterfaces, convertTo,
            _action,
            action, _updateItemsStruct => updateItemsStruct = _updateItemsStruct, true);
#if !CS8
            }
#endif
        }

        protected void UpdateTypes<TType, TDBType, TInterfaceImplementation>(
            string typeName,
            DBEntityCollection<TInterfaceImplementation> interfaceImplementations,
            Converter<TDBType, Type> convertFrom,
            Func<TType, Type, DBEntityCollection<TDBType>, TDBType> convertTo,
            Converter<TDBType, TInterfaceImplementation> interfaceImplementationInitializer,
            Action<TType, TDBType, DBEntityCollection<TDBType>>
#if CS8
            ?
#endif
            update,
            IEnumerable<TType> dotNetInterfaces,
            Action<TType, ulong> action,
            Action<TDBType>
#if CS8
            ?
#endif
            onRemove,
            params IEnumerable<IInterfaceImplementation>[] otherInterfaceImplementations) where TType : DotNetType where TInterfaceImplementation : InterfaceImplementation<TInterfaceImplementation> where TDBType : IDefaultEntity<ulong> => UpdateTypes(typeName, interfaceImplementations, otherInterfaceImplementations, convertFrom, convertTo, interfaceImplementationInitializer, update, dotNetInterfaces, action, onRemove);

        protected void UpdateTypes<TType, TDBType, TInterfaceImplementation>(
            string typeName,
            IEnumerable<IEnumerable<IInterfaceImplementation>> otherInterfaceImplementations,
            Converter<TDBType, Type> convertFrom,
            Func<TType, Type, DBEntityCollection<TDBType>, TDBType> convertTo,
            Func<TDBType, DBEntityCollection<TInterfaceImplementation>, TInterfaceImplementation> interfaceImplementationInitializer,
            Action<TType, TDBType, DBEntityCollection<TDBType>>
#if CS8
            ?
#endif
            update,
            IEnumerable<TType> dotNetInterfaces,
            Action<TType, ulong> action,
            Action<TDBType>
#if CS8
            ?
#endif
            onRemove) where TType : DotNetType where TInterfaceImplementation : InterfaceImplementation<TInterfaceImplementation> where TDBType : IDefaultEntity<ulong> => UtilHelpers.Using(GetCollection<TInterfaceImplementation>, items => UpdateTypes(
            typeName,
            items,
            otherInterfaceImplementations,
            convertFrom,
            convertTo,
            type => interfaceImplementationInitializer(type, items),
            update,
            dotNetInterfaces,
            action,
            onRemove));

        protected void UpdateTypes<TType, TDBType, TInterfaceImplementation>(
            string typeName,
            Converter<TDBType, Type> convertFrom,
            Func<TType, Type, DBEntityCollection<TDBType>, TDBType> convertTo,
            Func<TDBType, DBEntityCollection<TInterfaceImplementation>, TInterfaceImplementation> interfaceImplementationInitializer,
            Action<TType, TDBType, DBEntityCollection<TDBType>>
#if CS8
            ?
#endif
            update,
            IEnumerable<TType> dotNetInterfaces,
            Action<TType, ulong> action,
            Action<TDBType>
#if CS8
            ?
#endif
            onRemove,
            params IEnumerable<IInterfaceImplementation>[] otherInterfaceImplementations) where TType : DotNetType where TInterfaceImplementation : InterfaceImplementation<TInterfaceImplementation> where TDBType : IDefaultEntity<ulong> => UpdateTypes(
            typeName,
            otherInterfaceImplementations,
            convertFrom,
            convertTo,
            interfaceImplementationInitializer,
            update,
            dotNetInterfaces,
            action,
            onRemove);

        protected void UpdateTypes<T>(in string typeName, in IEnumerable<T> items, in Action<T, ulong> action, IEnumerable<IEnumerable<IInterfaceImplementation>> interfaceImplementations) where T : DotNetType => UpdateTypes<T, Type, InterfaceImplementation>(typeName, interfaceImplementations, Delegates.Self, (dotNetType, dbType, types) => dbType, (type, _items) => new InterfaceImplementation(_items) { Interface = type }, null, items, action, null);

        protected void UpdateTypes<T>(in string typeName, in IEnumerable<T> items, in Action<T, ulong> action, params IEnumerable<IInterfaceImplementation>[] interfaceImplementations) where T : DotNetType => UpdateTypes(typeName, items, action, interfaceImplementations.AsEnumerable());

        public void UpdateInterfaces() => UtilHelpers.Using(GetCollection<ClassInterfaceImplementation>, items => UpdateTypes("Interface", DotNetInterfaces, CreateInterfaceFile, items));

        public void UpdateStructs() => UpdateTypes("Struct", DotNetStructs, CreateStructFile);
    }
}
