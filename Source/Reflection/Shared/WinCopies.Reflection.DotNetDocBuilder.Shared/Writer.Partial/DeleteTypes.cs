using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;

using static WinCopies.Delegates;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        private void DeleteClassMetadata(Class @class, Type type)
        {
            using
#if !CS8
                (
#endif
                var _types = new DBEntityCollection<Class>(GetConnection())
#if CS8
                ;
#else
                )
#endif
            using
#if !CS8
                (
#endif
                var __types = new DBEntityCollection<Type>(GetConnection())
#if CS8
                ;
#else
                )
            {
#endif
            foreach (Class ___type in _types.Where(_type => _type.InheritsFromDocType?.Id == @class.Id))
            {
                ___type.InheritsFromDocType = null;

                _ = ___type.Update();
            }

            if (type != null)

                foreach (Type ___type in __types.Where(_type => _type.ParentType?.Type.Id == type.Id))
                {
                    ___type.ParentType = null;

                    _ = ___type.Update();
                }
#if !CS8
            }
#endif
        }

        public void TryDeleteDirectory(string wholePath, Type type)
        {
            string path = Path.GetDirectoryName(GetPath(type, new GetPathDelegates<Type>(() => wholePath, _type => _type.Name, _type => _type.ParentType?.Type, _type => _type.GenericTypeCount), out _, out _));

            if (Directory.Exists(path))

                try
                {
                    Directory.Delete(path, true);
                }

                catch (IOException) { /* Left empty. */ }
        }

        public void DeleteTypesBase<T>(string wholeNamespace, Converter<T, Type> converter, in Action<T, Type>
#if CS8
            ?
#endif
            beforeRemove) where T : IEntity
        {
            string typeTypeName = typeof(T).Name;

            void writeLine(in string
#if CS8
                ?
#endif
                value = null) => Logger($"Deleting all {typeTypeName} from {wholeNamespace}{value}", value == null);

            writeLine();

            ISQLConnection connection = GetConnection();

            using
#if !CS8
                (
#endif
                var types = new DBEntityCollection<T>(connection) { OrderByColumns = connection.GetOrderByColumns(OrderBy.Desc, "Id") }
#if CS8
                ;
#else
                )
            {
#endif
            ulong rows = 0;

            string wholePath = GetWholePath(wholeNamespace);

            Type __type = null;

            foreach (T type in types.Where(_type => GetWholeNamespace((__type = converter(_type)).Namespace.Id) == wholeNamespace))
            {
                Logger($"Removing {type}.", true);

                beforeRemove?.Invoke(type, __type);

                rows += type.Remove(out uint tables);

                if (rows > 0)
                {
                    Logger($"Removed {rows} {nameof(rows)} in {tables} {nameof(tables)}.", null);

                    TryDeleteDirectory(wholePath, __type);

                    Logger($"Deleted file for {type}.", false);
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
            value = null) => Logger($"Deleting all types from {wholeNamespace}{value}.", value == null);

            writeLine();

            void deleteTypes<T>(in Converter<T, Type> converter, in Action<T, Type>
#if CS8
                ?
#endif
                beforeRemove) where T : IEntity => DeleteTypesBase(wholeNamespace, converter, beforeRemove);

            void deleteTypes2<T>(in Action<T, Type>
#if CS8
                ?
#endif
                beforeRemove) where T : TypeBase<T> => deleteTypes<T>(type => type.Type, beforeRemove);

            deleteTypes2<Enum>(null);

            KeyValuePair<ulong, ulong?> keyValuePair = default;
            Func<bool> predicate;

            void deleteMetadata<TTypes, TInterfaceImplementation>(TTypes type, Type _type, in bool removeGenericTypes) where TInterfaceImplementation : InterfaceImplementation<TInterfaceImplementation>
            {
                void remove<TItems, TCollection>(in string metadataName, in Func<TCollection> func, in ConverterIn<TItems, KeyValuePair<ulong, ulong?>> converter, in ConverterIn<TItems, object> _converter) where TItems : IEntity where TCollection : IDBEntityCollection<TItems>, DotNetFix.IDisposable
                {
                    using
#if !CS8
                (
#endif
                        var collection = func()
#if CS8
                        ;
#else
                )
            {
#endif
                    Logger($"Removing {metadataName} for {_type}", true);

                    bool defaultPredicate() => keyValuePair.Key == _type.Id;

                    predicate = () => (predicate = keyValuePair.Value.HasValue ? () => defaultPredicate() || keyValuePair.Value.Value == _type.Id :
#if !CS9
                    (Func<bool>)
#endif
                    defaultPredicate)();

                    foreach (TItems item in collection)
                    {
                        keyValuePair = converter(item);

                        if (predicate())

                            Logger($"Removed {metadataName} for {_converter(item)}. Rows: {item.Remove(out uint tables)}; {nameof(tables)}: {tables}.", null);
                    }

                    Logger($"Removed {metadataName} for {_type}", false);
#if !CS8
                }
#endif
                }

                ISQLConnection connection = GetConnection();

                remove<TInterfaceImplementation, DBEntityCollection<TInterfaceImplementation>>("interface implementations", () => new DBEntityCollection<TInterfaceImplementation>(connection), (in TInterfaceImplementation interfaceImplementation) => new KeyValuePair<ulong, ulong?>(interfaceImplementation.ImplementorType.Id, interfaceImplementation.GetImplementedInterface().Id), (in TInterfaceImplementation interfaceImplementation) => $"{interfaceImplementation.ImplementorType} : {interfaceImplementation.GetImplementedInterface()}");

                if (removeGenericTypes)

                    remove("generic types", () => new DBEntityCollection<GenericType>(connection), (in GenericType genericType) => new KeyValuePair<ulong, ulong?>(genericType.Type.Id, null), (in GenericType genericType) => genericType.Name);
            }

            deleteTypes2((Class @class, Type type) =>
            {
                deleteMetadata<Class, ClassInterfaceImplementation>(@class, type, true);

                DeleteClassMetadata(@class, type);
            });

            deleteTypes(Self, (Type type, Type _type) =>
            {
                deleteMetadata<Type, InterfaceImplementation>(type, _type, false);
                deleteMetadata<Type, ClassInterfaceImplementation>(type, _type, true);
            });

            writeLine(" completed");
        }
    }
}
