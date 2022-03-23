using System.Collections.Generic;
using WinCopies.Reflection.DotNetParser;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public interface IWriter
    {
        #region Properties
        string DBName { get; }

        PackageInfo PackageInfo { get; }

        string RootPath { get; }
        #endregion Properties

        #region Methods
        int? GetNamespaceId(string @namespace);

        void UpdateNamespaces();

        string GetWholeNamespace(int id);

        string GetWholePath(string wholeNamespace);

        void CreateNamespaceFile(DotNetNamespace @namespace, int id);

        string GetNamespaceURLArray(string @namespace, out int index);

        IEnumerable<DotNetNamespace> GetNamespaces(IEnumerable<DotNetNamespace> parent);

        IEnumerable<DotNetNamespace> GetAllNamespacesInPackages();

        /*int? AddEnum(DotNetEnum @enum);

        int? AddEnumValue(int docTypeMember, long? value, ulong? uValue);

        void AddEnumValues(DotNetEnum @enum, int enumId, int? typeId = null);

        void AddItem<T>(IEnumerable<T> items, Func<T, int?> func, Action<T, int> action);

        int? AddNamespace(string @namespace);

        void AddNewEnums();

        void AddNewNamespaces();

        long? AddType(int namespaceId, string name, int typeTypeId, short genericTypeCount, int accessModifier, int? parentType);

        int? AddTypeMember(string name, int typeId);

        void CreateEnumFile(DotNetEnum @enum, int id);

        int DeleteEnum(int id, int? typeId = null);

        void DeleteEnums(int namespaceId, ISQLConnection? connection);

        int DeleteEnumValues(int enumId);

        int DeleteNamespace(int id);

        void DeleteOldEnums();

        void DeleteOldNamespaces();

        int DeleteType(int id);

        int DeleteTypeMembers(int typeId);

        void DeleteTypes(int namespaceId, ISQLConnection? connection);

        short? GetAccessModifierId(Type type);

        IEnumerable<DotNetNamespace> GetAllNamespacesInPackages();

        int? GetEnumId(DotNetEnum @enum, int namespaceId, out int? typeId);

        int? GetEnumId(int typeId);

        IEnumerable<int?> GetEnumIds(int namespaceId);

        IEnumerable<KeyValuePair<int, string>> GetNamespaces();

        IEnumerable<DotNetNamespace> GetNamespaces(IEnumerable<DotNetNamespace> parent);

        int? GetTypeId(int namespaceId, string name);

        int? GetTypeIdFromEnum(int enumId);

        int? GetTypeIdFromType(int typeId, string typeName);

        IEnumerable<int> GetTypeIds(int namespaceId);

        public IEnumerable<KeyValuePair<int, string>> GetTypes(int namespaceId);

        public string? GetTypeType(DotNetType type);

        public short? GetTypeTypeId(DotNetType type);

         int UpdateEnum(DotNetEnum @enum, int enumId, int typeId);

        int UpdateType(DotNetType type, int typeId, ISQLConnection? connection);*/
        #endregion Methods
    }
}
