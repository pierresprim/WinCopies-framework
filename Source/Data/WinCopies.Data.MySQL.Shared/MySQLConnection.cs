using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WinCopies.Data.SQL;

using static WinCopies.Data.MySQL.MySQLConnectionConstants;

namespace WinCopies.Data.MySQL
{
    public static class MySQLConnectionConstants
    {
        public const string EQUAL = "=";

        public const string IS = "IS";

        public const string NULL = "NULL";

        public const string COLUMN_DECORATOR = "`";
    }

    public class MySQLConnection : SQLConnection<MySqlConnection>, IConnection<MySqlCommand>
    {
        public override bool IsClosed => Connection.State != System.Data.ConnectionState.Open;

        public override string EqualityOperator => EQUAL;

        public override string NullityOperator => IS;

        public override string NullityOperand => NULL;

        public override string ColumnDecorator => COLUMN_DECORATOR;

        internal MySqlConnection InnerConnection => Connection;

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

        public override ISelect GetSelect(SQLItemCollection<string> defaultTables, SQLItemCollection<SQLColumn> defaultColumns) => new Select(this, defaultTables, defaultColumns);

        public override ISelect GetCountSelect(string tableName, IConditionGroup conditionGroup) => new Select(this, new SQLItemCollection<string>(tableName), new SQLItemCollection<SQLColumn>(new SQLColumn("COUNT(*)"))) { ConditionGroup = conditionGroup };

        public override IInsert GetInsert(string tableName, SQLItemCollection<StringSQLColumn>
#if CS8
            ?
#endif
            columns, SQLItemCollection<SQLItemCollection<IParameter>> values) => new Insert(this, tableName, columns, values);

        public override ISQLTableRequest2 GetDelete(SQLItemCollection<string> defaultTables) => new Delete(this, defaultTables is SQLItemCollection<string> _defaultTables ? _defaultTables : throw new InvalidArgumentException(nameof(defaultTables)));

        public override IUpdate GetUpdate(string tableName, SQLItemCollection<ICondition> columns) => new Update(this, tableName, columns);

        protected override int? UseDBOverride(string dbName) => GetCommand($"USE {dbName};").ExecuteNonQuery();

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

        public static uint? ExecuteNonQuery(MySqlCommand command) => GetResult(command.ExecuteNonQuery());

        public static async Task<uint?> ExecuteNonQueryAsync(MySqlCommand command) => GetResult(await command.ExecuteNonQueryAsync());

        public static uint ExecuteNonQuery2(MySqlCommand command) => GetResultOrThrow(ExecuteNonQuery(command));

        public static async Task<uint> ExecuteNonQueryAsync2(MySqlCommand command) => GetResultOrThrow(await ExecuteNonQueryAsync(command));

        public static void PrepareCommand(MySqlCommand command, IEnumerable<IParameter> parameters)
        {
            foreach (IParameter
#if CS8
            ?
#endif
            parameter in parameters)

                _ = command.Parameters.Add(new MySqlParameter(parameter.Name, parameter.Value));
        }

        public static void PrepareCommand(MySqlCommand command, IConditionGroup
#if CS8
            ?
#endif
            conditionGroup)
        {
            if (conditionGroup != null)

                PrepareCommand(command, conditionGroup.AsEnumerable());

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
                    , _ =>
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
