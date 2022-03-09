using MySql.Data.MySqlClient;

using System.Text;

using WinCopies.Collections.Generic;
using WinCopies.Data.SQL;
using WinCopies.Temp;

using static WinCopies.Data.SQL.SQLHelper;
using static WinCopies.Data.MySQL.MySQLConnection;

namespace WinCopies.Data.MySQL
{
    public class Select : Select<MySQLConnection, MySqlCommand, SQLColumn>
    {
        public Select(in MySQLConnection connection, in SQLItemCollection<string> defaultTables, in SQLItemCollection<SQLColumn> defaultColumns) : base(connection, defaultTables, defaultColumns) { /* Left empty. */ }

        protected override string GetSQL()
        {
            StringBuilder sb = new();

            _ = sb.Append($"SELECT {Columns} FROM {Tables.ToString(tableName => tableName.Surround('`'))}");

            AddConditions(ConditionGroup, sb);

            if (OrderBy.HasValue)
            {
                OrderByColumns orderBy = OrderBy.Value;

                SQLItemCollection<SQLColumn>? columns = orderBy.Columns;

                if (columns != null)

                    _ = sb.Append($" ORDER BY {columns} {(orderBy.OrderBy == SQL.OrderBy.Asc ? "ASC" : "DESC")}");
            }

            return sb.ToString();
        }

        protected override Action<MySqlCommand, IConditionGroup?> GetPrepareCommandAction() => PrepareCommand;

        public override System.Collections.Generic.IEnumerable<ISQLGetter> ExecuteQuery()
        {
            MySqlCommand command = GetCommand();

            return new Enumerable<ISQLGetter>(() => new MySQLEnumerator(command, Connection));
        }

        public override async Task<System.Collections.Generic.IEnumerable<ISQLGetter>> ExecuteQueryAsync()
        {
            MySQLEnumerator? result = await MySQLEnumerator.GetEnumerableAsync(GetCommand(), Connection);

            return new Enumerable<ISQLGetter>(() => result);
        }
    }

    public class Delete : SQLTableRequest<MySQLConnection, MySqlCommand>, ISQLTableRequest2
    {
        public Delete(MySQLConnection connection, SQLItemCollection<string> defaultTables) : base(connection, defaultTables) { /* Left empty. */ }

        protected override string GetSQL() => SQLHelper.GetSQL($"DELETE FROM {Tables.ToString(Temp.Delegates.GetSurrounder<string>("`"))}", ConditionGroup);

        protected override Action<MySqlCommand, IConditionGroup?> GetPrepareCommandAction() => PrepareCommand;

        public uint ExecuteNonQuery(out long? lastInsertedId) => SQLHelper.ExecuteNonQuery(() =>
        {
            MySqlCommand command = GetCommand();

            var result = (uint)command.ExecuteNonQuery();

            command.Dispose();

            return result;
        }, out lastInsertedId);

        public async Task<SQLAsyncNonQueryRequestResult> ExecuteNonQueryAsync()
        {
            MySqlCommand? command = GetCommand();

            uint result = (uint)await command.ExecuteNonQueryAsync();

            return new SQLAsyncNonQueryRequestResult(result, result > 0 ? command.LastInsertedId : null);
        }
    }

    public class Insert : Insert<MySQLConnection, MySqlCommand>
    {
        public Insert(in MySQLConnection connection, in string tableName, in SQLItemCollection<StringSQLColumn> columns, in SQLItemCollection<SQLItemCollection<IParameter>> values) : base(connection, tableName, columns, values) { /* Left empty. */ }

        protected override string GetSQL()
        {
            StringBuilder sb = new();

            _ = sb.Append($"INSERT INTO `{TableName}` ");

            static string getString(object obj) => $"({obj})";

            if (Columns != null)

                _ = sb.Append($"{getString(Columns.ToString(column => column.Name.Surround("`")))} ");

            _ = sb.Append("VALUES ");

            ActionIn<SQLItemCollection<IParameter>> append = (in SQLItemCollection<IParameter> values) =>
            {
                append = (in SQLItemCollection<IParameter> values) => sb.Append($",\n{getString(values)}");

                _ = sb.Append(getString(values));
            };

            foreach (SQLItemCollection<IParameter> values in Values)

                append(values);

            return sb.ToString();
        }

        protected override Action<MySqlCommand> GetPrepareCommandAction() => command =>
          {
              foreach (SQLItemCollection<IParameter>? paramCollection in Values)

                  PrepareCommand(command, paramCollection);
          };

        public override uint ExecuteNonQuery(out long? lastInsertedId) => ExecuteNonQuery(ExecuteNonQuery2, command => command.LastInsertedId, out lastInsertedId);

        public override async Task<SQLAsyncNonQueryRequestResult> ExecuteNonQueryAsync() => new SQLAsyncNonQueryRequestResult(await ExecuteNonQuery(ExecuteNonQueryAsync2, out long? lastInsertedId), lastInsertedId);
    }

    public class Update : Update<MySQLConnection, MySqlCommand>
    {
        public Update(in MySQLConnection connection, in string tableName, in SQLItemCollection<ICondition> columns) : base(connection, tableName, columns) { /* Left empty. */ }

        protected override string GetSQL() => SQLHelper.GetSQL($"UPDATE `{TableName}` SET {Columns.ToString(value => value.ToString("`"))}", ConditionGroup);

        protected override Action<MySqlCommand, IConditionGroup?> GetPrepareCommandAction() => (command, conditionGroup) =>
         {
             PrepareCommand(command, Columns.Select(condition => condition.InnerCondition.Parameter));

             PrepareCommand(command, ConditionGroup);
         };

        public override uint ExecuteNonQuery(out long? lastInsertedId) => ExecuteNonQuery(ExecuteNonQuery2, out lastInsertedId);

        public override async Task<SQLAsyncNonQueryRequestResult> ExecuteNonQueryAsync() => new SQLAsyncNonQueryRequestResult(await ExecuteNonQuery(ExecuteNonQueryAsync2, out long? lastInsertedId), lastInsertedId);
    }
}
