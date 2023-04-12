using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinCopies.Data.SQL;

using static WinCopies.Data.SQL.SQLConstants;
using static WinCopies.Data.MySQL.MySQLConnectionHelper;

namespace WinCopies.Data.MySQL
{
    public class MySQLConnection : SQLConnection<MySqlConnection, MySQLDatabase>, IConnection<MySqlCommand>
    {
        private string _userName;

        #region Properties
        public override string Normalization => "MySQL";

        public override string ServerName => Connection.DataSource;

        public override string UserName
        {
            get
            {
                if (_userName == null)

                    using (ISQLGetter getter = Collections.Enumerable.FromEnumerator(new MySQLEnumerator("SELECT CURRENT_USER();", this, false)).First())

                        _userName = (string)getter[0];

                return _userName;
            }
        }

        public override string
#if CS8
            ?
#endif
            DBName => Connection.Database;

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
        #endregion Properties

        public MySQLConnection(in MySQLConnectionParameters connectionParameters, in string
#if CS8
            ?
#endif
            dbName = null, in bool autoDispose = true) : base(connectionParameters.GetConnection(dbName), dbName, autoDispose) { /* Left empty. */ }

        public MySQLConnection(in MySqlConnection connection, in bool autoDispose) : base(connection, autoDispose) { /* Left empty. */ }

        #region Methods
        public new MySQLConnection GetConnection(bool autoDispose = true) => (MySQLConnection)base.GetConnection(autoDispose);

        protected override bool OpenOverride()
        {
            Connection.Open();

            return true;
        }

        protected override async Task<bool> OpenAsyncOverride()
        {
            await Connection.OpenAsync();

            return true;
        }

        protected override async Task<bool> OpenAsyncOverride(CancellationToken cancellationToken)
        {
            await Connection.OpenAsync(cancellationToken);

            return true;
        }

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

        public override IDatabaseSelect GetDatabases() => new DatabaseSelect(this, "information_schema.schemata", "schema_name");

        protected override MySQLDatabase GetDatabase(string dbName) => new MySQLDatabase(this, dbName);

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

        public override string GetCreateDBSQL(ISQLDatabase database, bool ifNotExists) => $"CREATE DATABASE {(ifNotExists ? "IF NOT EXISTS " : null)}{GetItemName(database.Name)} {DEFAULT} CHARACTER SET {database.CharacterSet} COLLATE {database.Collation}";

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

        public override
#if CS9
            MySQLConnection
#else
            ISQLConnection
#endif
            Clone(bool autoDispose = true) => new
#if !CS9
            MySQLConnection
#endif
            ((MySqlConnection)Connection.Clone(), autoDispose);

        protected override void CloseOverride() => Connection.Close();

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

        public override IEnumerable<
#if CS9
            MySQLDatabase
#else
            ISQLDatabase
#endif
            > AsEnumerable() => GetEnumerable();
        #endregion Methods
    }
}
