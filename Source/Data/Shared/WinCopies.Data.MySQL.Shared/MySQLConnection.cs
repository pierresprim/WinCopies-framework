using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WinCopies.Data.SQL;

using static WinCopies.Data.SQL.SQLConstants;
using static WinCopies.Data.MySQL.MySQLConnectionHelper;

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

    public class MySQLColumnInfo : SQLColumnInfo, ISQLColumnInfo
    {
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

    public class MySQLUnicityIndex : SQLUnicityIndex2
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
    }

    public class MySQLTable : SQLTable2
    {
        public override string GetCreateTableSQL(bool ifNotExists)
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

            foreach (ISQLColumnInfo column in Columns)
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

            foreach (IUnicityIndex unicityIndex in UnicityKeys)

                append($",\r\n{unicityIndex.GetSQL()}");

            append($"\n) ENGINE={Engine} {AUTO_INCREMENT}={AutoIncrement} {DEFAULT} CHARSET={CharacterSet}");

            return result.ToString();
        }

        public override string GetRemoveTableSQL(bool ifExists) => $"DROP TABLE {(ifExists ? "IF EXISTS " : null)}{GetItemName(Name)}";
    }

    public class MySQLConnection : SQLConnection<MySqlConnection>, IConnection<MySqlCommand>
    {
        public override bool IsClosed => Connection.State != System.Data.ConnectionState.Open;

        public override string EqualityOperator => EQUAL;

        public override string NullityOperator => IS;

        public override string NullityOperand => NULL;

        public override string ColumnDecorator => COLUMN_DECORATOR;

        internal MySqlConnection InnerConnection => Connection;

        public override bool CanUseDB => true;
        public override bool CanCreateDB => true;
        public override bool CanSelect => true;
        public override bool CanCount => true;
        public override bool CanInsert => true;
        public override bool CanUpdate => true;
        public override bool CanDelete => true;

        public MySQLConnection(in MySqlConnection connection, in bool autoDispose = true) : base(connection, autoDispose) { /* Left empty. */ }

        public MySQLConnection(in MySqlConnection connection, in string dbName, in bool autoDispose = true) : base(connection, dbName, autoDispose) { /* Left empty. */ }

        public override bool Open()
        {
            Connection.Open();

            return true;
        }

        public override Task OpenAsync() => Connection.OpenAsync();

        public override Task OpenAsync(CancellationToken cancellationToken) => Connection.OpenAsync(cancellationToken);

        public MySqlCommand GetCommand(string sql) => new
#if !CS9
               MySqlCommand
#endif
               (sql, Connection);

        public MySqlCommand GetPreparedCommand(string sql)
        {
            var command = GetCommand(sql);

            command.Prepare();

            return command;
        }

        public override ISelect GetSelect(SQLItemCollection<string> defaultTables, SQLItemCollection<SQLColumn> defaultColumns) => new Select(this, defaultTables, defaultColumns);

        public override ISelect GetCountSelect(string tableName, IConditionGroup conditionGroup) => new Select(this, new SQLItemCollection<string>(tableName), new SQLItemCollection<SQLColumn>(new SQLColumn("COUNT(*)"))) { ConditionGroup = conditionGroup };

        public override IInsert GetInsert(string tableName, SQLItemCollection<StringSQLColumn>
#if CS8
            ?
#endif
            columns, SQLItemCollection<SQLItemCollection<IParameter>> values) => new Insert(this, tableName, columns, values);

        public override ISQLTableRequest2 GetDelete(SQLItemCollection<string> defaultTables) => new Delete(this, defaultTables is SQLItemCollection<string> _defaultTables ? _defaultTables : throw ThrowHelper.GetArgumentException(nameof(defaultTables)));

        public override IUpdate GetUpdate(string tableName, SQLItemCollection<ICondition> columns) => new Update(this, tableName, columns);

        protected override uint? UseDBOverride(string dbName) => ExecuteNonQuery($"USE {dbName};");

        public override string GetItemName(string itemName) => MySQLConnectionHelper.GetItemName(itemName);



        public override string GetBeginTransactionSQL() => "START TRANSACTION";

        public override string GetCreateDBSQL(ISQLDatabase database, bool ifNotExists) => $"CREATE DATABASE {(ifNotExists ? "IF NOT EXISTS " : null)}{GetItemName(database.Name)} {DEFAULT} CHARACTER SET {database.CharacterSet} COLLATE {database.Collate}";

        public override string GetEndTransactionSQL() => "COMMIT";

        public override string GetResetTransactionSQL() => "ROLLBACK";



        public static ICondition GetCondition<T>(string columnName, SQL.Parameter<T>
#if CS8
            ?
#endif
            parameter, string
#if CS8
            ?
#endif
            tableName = null, string @operator = EQUAL) => new Condition<T>(@operator)
            {
                InnerCondition = new ColumnCondition<T>(new KeyValuePair<string
#if CS8
            ?
#endif
            , string>(tableName, columnName), parameter)
            };

        public override ICondition GetCondition<T>(string columnName, string paramName, T value, string
#if CS8
            ?
#endif
            tableName = null, string @operator = EQUAL) => GetCondition(columnName, new Parameter<T>(paramName, value), tableName, @operator);

        public override ICondition GetNullityCondition(string columnName, string
#if CS8
            ?
#endif
            tableName = null, string @operator = IS) => GetCondition<object
#if CS8
            ?
#endif
            >(columnName, null, tableName, @operator);

        public override ISQLConnection Clone(bool autoDispose = true) => new MySQLConnection((MySqlConnection)Connection.Clone(), autoDispose);

        public override void Close() => Connection.Close();

        public static long? GetLastInsertedId(MySqlCommand command) => command.LastInsertedId;

        private static uint? GetResult(int result) => result > -1 ? (uint
#if !CS9
            ?
#endif
            )result : null;

        private static uint GetResultOrThrow(uint? result) => result ?? throw new InvalidOperationException("The request was not a modifier one.");

        public override uint? ExecuteNonQuery(string sql) => ExecuteNonQuery(GetCommand(sql));

        public static uint? ExecuteNonQuery(MySqlCommand command) => GetResult(command.ExecuteNonQuery());

        public static async Task<uint?> ExecuteNonQueryAsync(MySqlCommand command) => GetResult(await command.ExecuteNonQueryAsync());

        public static uint ExecuteNonQuery2(MySqlCommand command) => GetResultOrThrow(ExecuteNonQuery(command));

        public static async Task<uint> ExecuteNonQueryAsync2(MySqlCommand command) => GetResultOrThrow(await ExecuteNonQueryAsync(command));

        public static void PrepareCommandParameters(MySqlCommand command, IEnumerable<IParameter> parameters)
        {
            foreach (IParameter
#if CS8
            ?
#endif
            parameter in parameters)

                _ = command.Parameters.Add(new MySqlParameter(parameter.Name, parameter.Value));
        }

        public static void PrepareCommand(MySqlCommand command, IEnumerable<IParameter> parameters)
        {
            PrepareCommandParameters(command, parameters);

            command.Prepare();
        }

        public static void PrepareCommand(MySqlCommand command, IConditionGroup
#if CS8
            ?
#endif
            conditionGroup)
        {
            if (conditionGroup != null)

                PrepareCommandParameters(command, conditionGroup.AsEnumerable());

            command.Prepare();
        }

        protected override void DisposeUnmanaged()
        {
            Close();

            base.DisposeUnmanaged();
        }

        public override IParameter<T> GetParameter<T>(string name, T value) => new Parameter<T>(name, value);

        public override string
#if CS8
            ?
#endif
            GetOperator(ConditionGroupOperator @operator)
#if CS8
            =>
#else
        {
            switch (
#endif
            @operator
#if CS8
            switch
#else
            )
#endif
            {
#if !CS8
                case
#endif
                ConditionGroupOperator.And
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "AND"
#if CS8
                    ,
#else
                    ;
                case
#endif
                ConditionGroupOperator.Or
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "OR"
#if CS8
                    ,
                _ =>
#else
                    ;
            }

            return
#endif
                null
#if !CS8
                ;
#endif
            }
#if CS8
        ;
#endif
    }
}
