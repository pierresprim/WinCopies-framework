using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Data.SQL;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.Util;

using static WinCopies.Collections.Enumerable;

using IEnumerable = System.Collections.Generic.IEnumerable<WinCopies.IO.ObjectModel.IBrowsableObjectInfo>;

namespace WinCopies.IO.SQL.Common
{
    public struct SQLConnectionInfoProvider
    {
        internal ISQLConnection Connection { get; private set; }

        public SQLConnectionInfoProvider(in ISQLConnection connection) => Connection = connection;

        public ObjectModel.ISQLConnectionInfo GetBrowsableObjectInfo() => new ObjectModel.SQLConnectionInfo(Connection, IO.ObjectModel.BrowsableObjectInfo.GetDefaultClientVersion());
    }

    public class SQLProtocolInfo : ProtocolInfo
    {
        private readonly System.Collections.Generic.IDictionary<string, ConnectionParameter> _dic = new Dictionary<string, ConnectionParameter>();

        protected override System.Collections.Generic.IReadOnlyDictionary<string, ConnectionParameter>
#if CS8
            ?
#endif
            ConnectionParametersOverride
        { get; }

        protected override IItemSourcesProvider ItemSourcesOverride { get; }

        public SQLProtocolInfo(string protocol, in IBrowsableObjectInfo parent, in ClientVersion clientVersion) : base(protocol, parent, clientVersion)
        {
            ItemSourcesOverride = new ItemSourcesProvider(GetItems);

            ConnectionParametersOverride = new ReadOnlyDictionary<string, ConnectionParameter>(_dic);

            void add(in string key) => _dic.Add(key, new ConnectionParameter(null, null));

            add(nameof(SQLConnectionParametersStruct.ServerName));
            add(nameof(SQLConnectionParametersStruct.UserName));
            add(nameof(SQLConnectionParametersStruct.Credential));
        }

        private System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems()
        {
            string getValue(in string key) => _dic[key].Value;

            string userName = getValue(nameof(SQLConnectionParametersStruct.UserName));

            yield return BrowsableObjectInfoPlugin.GetSQLConnectionInfo(new SQLConnectionParametersStruct(getValue(nameof(SQLConnectionParametersStruct.ServerName)), userName, new System.Net.NetworkCredential(userName, getValue(nameof(SQLConnectionParametersStruct.Credential)))));
        }
    }

    public class BrowsableObjectInfoPlugin : IO.BrowsableObjectInfoPlugin
    {
        private static readonly ILinkedList<SQLConnectionInfoProvider> _connectionProviders = new Collections.DotNetFix.Generic.LinkedList<SQLConnectionInfoProvider>();

        public static IReadOnlyLinkedList2<SQLConnectionInfoProvider> ConnectionProviders { get; } = new ReadOnlyLinkedList<SQLConnectionInfoProvider>(_connectionProviders);

        public override IBitmapSourceProvider BitmapSourceProvider => null;

        public override IEnumerable GetStartPages(ClientVersion clientVersion) => ConnectionProviders.Select(item => item.GetBrowsableObjectInfo());

        public static ObjectModel.ISQLConnectionInfo GetSQLConnectionInfo(in SQLConnectionParametersStruct connectionParameters)
        {
            var _connection = new Data.MySQL.MySQLConnection(new Data.MySQL.MySQLConnectionParameters(connectionParameters), null);

            _ = _connection.Open();

            ObjectModel.SQLConnectionInfo connection = new ObjectModel.SQLConnectionInfo(_connection, IO.ObjectModel.BrowsableObjectInfo.GetDefaultClientVersion());

            connection.InnerObjectGeneric.Opened += (ISQLConnection sender, EventArgs e) => _connectionProviders.AddLast(new SQLConnectionInfoProvider(sender));
            connection.InnerObjectGeneric.Closed += (ISQLConnection sender, EventArgs e) => _connectionProviders.Remove(_connectionProviders.AsNodeEnumerable().First(node => node.Value.Connection == connection.InnerObjectGeneric));

            return connection;
        }

        public override IEnumerable GetProtocols(IBrowsableObjectInfo parent, ClientVersion clientVersion) => GetEnumerable(new SQLProtocolInfo("mysql", parent, clientVersion));
    }

    public static class BrowsableObjectInfo
    {
        public static IBrowsableObjectInfoPlugin GetPluginParameters() => new BrowsableObjectInfoPlugin();
    }
}
