using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WinCopies.Collections;
using WinCopies.Data.SQL;
using WinCopies.Util;

using static WinCopies.Data.MySQL.MySQLConnectionHelper;
using static WinCopies.Data.SQL.SQLHelper;
using static WinCopies.UtilHelpers;

namespace WinCopies.Data.MySQL
{
    public static class MySQLConnectionHelper
    {
        public const string NULL = nameof(NULL);

        public const string NOT = nameof(NOT);

        public const string COLUMN_DECORATOR = "`";

        public const string AUTO_INCREMENT = nameof(AUTO_INCREMENT);

        public const string DEFAULT = nameof(DEFAULT);

        public static string GetItemName(string itemName) => $"`{itemName}`";
    }

    public class MySQLColumnInfo : SQLColumnInfo<MySQLConnection, MySQLTable>
    {
        private struct MySQLColumnInfoStruct
        {
            public bool AllowNull { get; }

            public bool AutoIncrement { get; }

            public string
#if CS8
                ?
#endif
                Collation
            { get; }

            public object
#if CS8
                ?
#endif
                Default
            { get; }

            public bool IsPrimaryKey { get; }

            public MySQLColumnInfoStruct(MySQLColumnInfo column)
            {
                _ = column.Connection.UseDB("information_schema");

                using
#if !CS8
                    (
#endif
                    ISQLGetter getter = column.GetSQLGetter()
#if CS8
                ;
#else
                    )
                {
#endif
                MySQLConnection connection = column.Connection.GetConnection();

                MySQLTable table = column.Parent;

                bool getIfAutoIncrement()
                {
                    foreach (ISQLGetter item in Collections.Enumerable.FromEnumerator(new MySQLEnumerator(new MySqlCommand($"SELECT (`EXTRA` LIKE '%auto_increment%') AS `{AUTO_INCREMENT}` FROM `information_schema`.`columns` WHERE `TABLE_SCHEMA`='{table.Parent.Name}' AND `TABLE_NAME`='{table.Name}' AND `COLUMN_NAME` = '{column.Name}';", connection.InnerConnection), connection, true)))

                        return true;

                    return false;
                }

                AutoIncrement = false;

                AllowNull = GetValue(getter["IS_NULLABLE"], out string
#if CS8
                    ?
#endif
                    obj) && obj != "NO";
                Collation = GetValue<string>(getter["COLLATION_NAME"]);
                Default = GetValue<object>(getter["COLUMN_DEFAULT"]);
                IsPrimaryKey = GetValue(getter["COLUMN_KEY"], out obj) && obj == "PRI";
#if !CS8
                }
#endif
            }
        }

        private MySQLColumnInfoStruct? _struct;

        public override bool AllowNull => Init().AllowNull;

        public override bool AutoIncrement => Init().AutoIncrement;

        public override string
#if CS8
            ?
#endif
            Collation => Init().Collation;

        public override object
#if CS8
            ?
#endif
            Default => Init().Default;

        public override bool IsPrimaryKey => Init().IsPrimaryKey;

        public MySQLColumnInfo(in MySQLTable table, in string name) : base(table, name) { /* Left empty. */ }

        private MySQLColumnInfoStruct Init() => InitializeStruct(ref _struct, () => new MySQLColumnInfoStruct(this));

        public override ISelect GetProperties()
        {
            ISQLTable table = Parent;
            ISQLDatabase database = table.Database;
            ISQLConnection connection = database.Connection;

            ICondition getCondition(in string columnName, in string paramName, in string paramValue) => connection.GetCondition(columnName, paramName, paramValue);

            return connection.GetSelect("columns", GetArray(getCondition("TABLE_SCHEMA", "databaseName", database.Name), getCondition("TABLE_NAME", "tableName", table.Name), getCondition("COLUMN_NAME", "columnName", Name)));
        }

        public override string GetSQL()
        {
            var result = new StringBuilder();

            result.Append($"{GetItemName(Name)} ");

            ISQLColumnType
#if CS8
                ?
#endif
                columnType = ColumnType;

            result.Append(columnType.Name);

            if (columnType.Length.HasValue)

                result.Append($"({columnType.Length.Value})");

            IEnumerable<string>
#if CS8
                ?
#endif
                columnTypeAttributes = columnType.Attributes;

            if (columnTypeAttributes != null)

                foreach (string columnTypeAttribute in columnTypeAttributes)

                    result.Append($" {columnTypeAttribute}");

            void appendDefault(in object value) => result.Append($" {DEFAULT} {value}");

            if (AllowNull)

                appendDefault(Default ?? NULL);

            else
            {
                result.Append($" {NOT} {NULL}");

                if (Default != null)

                    appendDefault(Default);
            }

            if (AutoIncrement)

                result.Append($" {AUTO_INCREMENT}");

            return result.ToString();
        }

        public override string ToString() => GetSQL();
    }

    /*public class MySQLUnicityIndex : SQLUnicityIndex2
    {
        public override string GetSQL()
        {
            var result = new StringBuilder();

            void append(in string text) => result.Append(text);

            append($"UNIQUE KEY {GetItemName(Name)} (");

            Action<string> action = column =>
            {
                action = _column => append($", {_column}");

                append(column);
            };

            foreach (string column in Columns)

                action(GetItemName(column));

            append(") USING BTREE");

            return result.ToString();
        }
    }*/

    public class MySQLTable : SQLTable<MySQLConnection, MySQLDatabase, MySQLColumnInfo>
    {
        private struct MySQLTableStruct
        {
            public string
#if CS8
            ?
#endif
            Collation
            { get; }

            public ulong AutoIncrement { get; }

            public string Engine { get; }

            public MySQLTableStruct(in MySQLTable table)
            {
                _ = table.Connection.UseDB("information_schema");

                using
#if !CS8
                    (
#endif
                    ISQLGetter getter = table.GetSQLGetter()
#if CS8
                ;
#else
                    )
                {
#endif
                Collation = GetValue<string>(getter["TABLE_COLLATION"]);
                AutoIncrement = (ulong)getter["AUTO_INCREMENT"];
                Engine = (string)getter["ENGINE"];
#if !CS8
                }
#endif
            }
        }

        private MySQLTableStruct? _struct;

        public override string
#if CS8
            ?
#endif
            Collation => Init().Collation;

        public override ulong AutoIncrement => Init().AutoIncrement;

        public override string Engine => Init().Engine;

        public MySQLTable(in MySQLDatabase database, in string name) : base(database, name) { /* Left empty. */ }

        private MySQLTableStruct Init() => InitializeStruct(ref _struct, () => new MySQLTableStruct(this));

        public override ISelect GetProperties()
        {
            ISQLDatabase database = Parent;
            ISQLConnection connection = database.Connection;

            ICondition getCondition(in string columnName, in string paramName, in string paramValue) => connection.GetCondition(columnName, paramName, paramValue);

            return connection.GetSelect("tables", GetArray(getCondition("TABLE_SCHEMA", "databaseName", database.Name), getCondition("TABLE_NAME", "tableName", Name)));
        }

        public override string GetCreateTableSQL(bool ifNotExists, string characterSet)
        {
            var result = new StringBuilder();
            Collections.DotNetFix.Generic.IQueue<string> primaryKeys = new Collections.DotNetFix.Generic.Queue<string>();

            void append(in string text) => result.Append(text);

            append("CREATE TABLE ");

            if (ifNotExists)

                append("IF NOT EXISTS ");

            append(GetItemName(Name));

            append(" (\n");

            Converter<ISQLColumnInfo, string> getSQL = column =>
            {
                getSQL = _column => ",\r\n" + _column.GetSQL();

                return column.GetSQL();
            };

            foreach (ISQLColumnInfo column in this)
            {
                append(getSQL(column));

                if (column.IsPrimaryKey)

                    primaryKeys.Enqueue(column.Name);
            }

            if (primaryKeys.Count > 0)
            {
                Converter<string, string> getPrimaryKeysSQL = column =>
                {
                    getPrimaryKeysSQL = _column => ", " + GetItemName(_column);

                    return GetItemName(column);
                };

                append(",\r\nPRIMARY KEY (");

                while (primaryKeys.TryDequeue(out string column))

                    append(getPrimaryKeysSQL(column));

                _ = result.Append(')');
            }

            /*foreach (IUnicityIndex unicityIndex in UnicityKeys)

                append($",\r\n{unicityIndex.GetSQL()}");*/

            append($"\n) ENGINE={Engine} {AUTO_INCREMENT}={AutoIncrement} {DEFAULT} CHARSET={characterSet}");

            return result.ToString();
        }

        public override string GetRemoveTableSQL(bool ifExists) => $"DROP TABLE {(ifExists ? "IF EXISTS " : null)}{GetItemName(Name)}";

        public override IDatabaseSelect GetColumns() => new DatabaseSelect(Connection, "information_schema.columns", "column_name") { ConditionGroup = new ConditionGroup("AND") { Conditions = Collections.DotNetFix.LinkedList.GetLinkedList(Connection.GetCondition("table_schema", "dbName", Parent.Name), Connection.GetCondition("table_name", "tableName", Name)) } };

        protected override MySQLColumnInfo GetColumn(string columnName) => new
#if !CS9
            MySQLColumnInfo
#endif
            (this, columnName);

        public override IEnumerable<
#if CS9
            MySQLColumnInfo
#else
            ISQLColumnInfo
#endif
            > AsEnumerable() => GetEnumerable();
    }

    public class MySQLConnectionParameters : SQLConnectionParameters<MySqlConnection>
    {
        public const string SERVER = "server";
        public const string USER_ID = "user id";
        public const string PASSWORD = "password";
        public const string DATABASE = "database";

        public MySQLConnectionParameters(in SQLConnectionParametersStruct parameters) : base(parameters) { /* Left empty. */ }

        public override MySqlConnection GetConnection(string
#if CS8
        ?
#endif
        dbName = null)
        {
            SQLConnectionParametersStruct parameters = Parameters;

            var connectionString = new StringBuilder();

            void append(in string text) => connectionString.Append(text);

            string[] defaultConnectionString = { $"{SERVER}={parameters.ServerName};", $"{USER_ID}={parameters.UserName};", $"{PASSWORD}={parameters.Credential.Password};" };

            IEnumerable<string> getDefaultParameters()
            {
                foreach (string param in defaultConnectionString)

                    yield return param;

                yield return dbName == null ? $"{DATABASE}=;" : $"{DATABASE}={dbName};";
            }

            append(defaultConnectionString.Join(false));

            if (parameters.DBName != null)

                append("database=;");

            if (parameters.Parameters != null)

                foreach (KeyValuePair<string, string> parameter in parameters.Parameters)

                    foreach (string param in defaultConnectionString)

                        if (parameter.Key.Equals(param, '='))

                            throw new InvalidOperationException("Cannot redeclare parameter.");

                        else

                            append(parameter.ToString('='));

            return new MySqlConnection(connectionString.ToString());
        }
    }

    public class MySQLDatabase : SQLDatabase<MySQLConnection, MySQLTable>
    {
        private struct MySQLDatabaseStruct
        {
            public string CharacterSet { get; }

            public string
#if CS8
            ?
#endif
            Collation
            { get; }

            public MySQLDatabaseStruct(in MySQLDatabase database)
            {
                _ = database.Connection.UseDB("information_schema");

                using
#if !CS8
                    (
#endif
                    ISQLGetter getter = database.GetSQLGetter()
#if CS8
                ;
#else
                    )
                {
#endif
                CharacterSet = (string)getter["DEFAULT_CHARACTER_SET_NAME"];
                Collation = GetValue<string>(getter["DEFAULT_COLLATION_NAME"]);
#if !CS8
                }
#endif
            }
        }

        private MySQLDatabaseStruct? _struct;

        public override string CharacterSet => Init().CharacterSet;

        public override string
#if CS8
            ?
#endif
            Collation => Init().Collation;

        public MySQLDatabase(in MySQLConnection connection, in string dbName) : base(connection, dbName) { /* Left empty. */ }

        private MySQLDatabaseStruct Init() => InitializeStruct(ref _struct, () => new MySQLDatabaseStruct(this));

        public override ISelect GetProperties() => Connection.GetSelect("schemata", GetArray(Connection.GetCondition("SCHEMA_NAME", "name", Name)));

        public override IDatabaseSelect GetTables() => new DatabaseSelect(Connection, "information_schema.tables", "table_name") { ConditionGroup = new ConditionGroup(null) { Conditions = Collections.DotNetFix.LinkedList.GetLinkedList(Connection.GetCondition("table_schema", "dbName", Name)) } };

        protected override MySQLTable GetTable(string tableName) => new
#if !CS9
            MySQLTable
#endif
            (this, tableName);

        public override IEnumerable<
#if CS9
            MySQLTable
#else
            ISQLTable
#endif
            > AsEnumerable() => GetEnumerable();
    }
}
