using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WinCopies.Data.SQL;
using WinCopies.Reflection.DotNetParser;

using static WinCopies.Collections.DotNetFix.LinkedList;
using static WinCopies.Reflection.DotNetDocBuilder.WriterConsts;
using static WinCopies.UtilHelpers;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        public string GetNamespaceURLArray(string @namespace, IReadOnlyList<string> paths, out int index)
        {
            string[] namespaces = @namespace.Split('.');

            IList<string> items = paths == null
                ? GetList(namespaces)
                : GetList(namespaces, new Collections.Generic.SubReadOnlyList<string>(paths, 1, paths.Count - 2));

            int length = items.Count - 1;

            var sb = new StringBuilder();
            var aux = new StringBuilder();

            int i = 0;

            for (; i < length; i++)
            {
                _ = aux.Append('/');
                _ = aux.Append(@namespace = items[i]);

                _ = sb.Append($"{i + 3} => '<a href=\"/doc/{PackageInfo.URL}{aux}/index.php\">{@namespace}</a> ', ");
            }

            _ = sb.Append($"{i + 3} => '{items[i]}'");

            index = i + 3;

            return sb.ToString();
        }

        protected abstract byte[] GetNamespaceData();

        public ulong? GetNamespaceId(string @namespace)
        {
            ISQLConnection
#if CS8
                ?
#endif
                connection = GetConnection();

            string[] namespaces = @namespace.Split('.');

            int paramId = 0;
            string getParamName() => $"name{paramId++}";

            SQLColumn[]
#if CS8
                ?
#endif
                idColumn = GetArray(connection.GetColumn(ID));

            ISelect getSelect(in ActionIn<IConditionGroup> action)
            {
                ISelect _select = connection.GetSelect(GetArray(DOC_NAMESPACE), idColumn);

                IConditionGroup conditionGroup = new ConditionGroup("AND");

                _select.ConditionGroup = conditionGroup;

                action(conditionGroup);

                return _select;
            }

            ICondition
#if CS8
                ?
#endif
                getNameCondition(in string _namespace) => connection.GetCondition(NAME, getParamName(), _namespace);

            ISelect select = getSelect((in IConditionGroup conditionGroup) => conditionGroup.Conditions = GetLinkedList(connection.GetNullityCondition(PARENT_ID), getNameCondition(namespaces[0])));

            for (int i = 1; i < namespaces.Length; i++)

                select = getSelect((in IConditionGroup conditionGroup) =>
                {
                    conditionGroup.Conditions = GetLinkedList(getNameCondition(namespaces[i]));

                    conditionGroup.Selects = GetLinkedList(new KeyValuePair<SQLColumn, ISelect>(connection.GetColumn(PARENT_ID), select));
                });

            return PerformActionIfNotNull(select.ExecuteQuery(false).FirstOrDefault(), value => Convert.ChangeType<ulong>(value[0]));
        }

        public static string GetWholeNamespace(ulong id, in ISQLConnection connection)
        {
            Collections.Generic.IPrependableExtensibleEnumerable<string> namespaces = new Collections.DotNetFix.Generic.LinkedList<string>();

            KeyValuePair<string, ulong?>? first = null;

            bool whileCondition()
            {
                if (first.HasValue && first.Value.Value.HasValue)
                {
                    id = first.Value.Value.Value;

                    return true;
                }

                return false;
            }

            object
#if CS8
    ?
#endif
                obj;

            do
            {
                ISQLGetter item = connection.GetSelect(GetArray(DOC_NAMESPACE), GetArray(connection.GetColumn(NAME), connection.GetColumn(PARENT_ID)), conditions: GetArray(connection.GetCondition(ID, nameof(id), id))).ExecuteQuery(false).First();

                obj = item[PARENT_ID];

                namespaces.Prepend((first = new KeyValuePair<string, ulong?>((string)item[NAME], obj == null || obj == DBNull.Value ? null : (ulong
#if !CS9
                    ?
#endif
                    )Convert.TryChangeType<ulong>(obj))).Value.Key);
            }
            while (whileCondition());

            return string.Join(
#if CS8
                '.'
#else
                "."
#endif
                , namespaces);
        }

        public string GetWholeNamespace(ulong id)
        {
            using
#if !CS8
                (
#endif
                ISQLConnection
#if CS8
                ?
#endif
                connection = GetConnection()
#if CS8
                ;
#else
                )
#endif

            return GetWholeNamespace(id, connection);
        }

        public IEnumerable<DotNetNamespace> GetNamespaces(IEnumerable<DotNetNamespace> parent)
        {
            foreach (DotNetNamespace @namespace in parent)
            {
                yield return @namespace;

                foreach (DotNetNamespace _namespace in GetNamespaces(@namespace.GetSubnamespaces()))

                    yield return _namespace;
            }
        }
    }
}
