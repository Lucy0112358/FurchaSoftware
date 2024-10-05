using MqttService.Infrastructure.Extensions;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MqttService.Application.Repositories
{
    public class BaseRepository
    {
        public readonly string furchaSchema = "furcha";
        private readonly NpgsqlConnection furchaContext;

        public BaseRepository(NpgsqlConnection dbConnection)
        {
            furchaContext = dbConnection;
        }

        /// <summary>
        /// Extracts the schema based on the type. Always default to phoenix schema in case of error.
        /// </summary>
        /// <param name="type">The type of a class having a Table attribute on it.</param>
        /// <returns>Schema name as string.</returns>
        private string GetSchema(Type type)
        {
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();

            // In case the schema is not defined, return the default schema.
            if (tableAttribute == null || string.IsNullOrEmpty(tableAttribute.Schema))
            {
                return furchaSchema;
            }

            return tableAttribute.Schema;
        }

        /// <summary>
        /// Executes a select query against the schema defined by the T TableAttribute.Schema.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="where">Optional. The WHERE condition (WITHOUT the WHERE keyword).</param>
        /// <param name="whereParam">Optional. The parameter used by the WHERE condition.</param>
        /// <returns>All the records returned by the SELECT statement.</returns>
        protected IEnumerable<T> GetAll<T>(string where = null, object whereParam = null)
        {
            var schema = GetSchema(typeof(T));
            var sql = $@"SELECT * 
                 FROM ""{schema}"".""{typeof(T).Name}""";

            if (where.IsNotNullOrEmpty())
            {
                sql += $" WHERE {where}";
            }

            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql, param: whereParam);
            }
        }

        /// <summary>
        /// Returns all records of the defined type. Can join another table, and there is the option of mapping the joined table result.
        /// </summary>
        /// <typeparam name="T">The type of the main entity.</typeparam>
        /// <typeparam name="T2">The type of the joined entity.</typeparam>
        /// <param name="joinedColumnFK">The foreign key column used for joining.</param>
        /// <param name="map">A function to map the joined results.</param>
        /// <param name="where">Optional. The WHERE condition (WITHOUT the WHERE keyword).</param>
        /// <param name="whereParam">Optional. The parameter used by the WHERE condition.</param>
        /// <returns>All the records returned by the SELECT statement with the joined table.</returns>
        protected IEnumerable<T> GetAll<T, T2>(string joinedColumnFK, Func<T, T2, T> map, string where = null, object whereParam = null)
        {
            var schema = GetSchema(typeof(T));
            var schema2 = GetSchema(typeof(T2));

            var sql = $@"SELECT ""{typeof(T).Name}"".*, ""{typeof(T2).Name}"".*  
                 FROM ""{schema}"".""{typeof(T).Name}"" 
                 JOIN ""{schema2}"".""{typeof(T2).Name}"" 
                 ON ""{typeof(T).Name}"".""{joinedColumnFK}"" = ""{typeof(T2).Name}"".""Id""";

            if (!string.IsNullOrEmpty(where))
            {
                sql += $" WHERE {where}";
            }

            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query(sql: sql, map, param: whereParam);
            }
        }

        /// <summary>
        /// Returns all records of the defined type. Can join another table, but there is no option of mapping the joined table result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinedColumnFK"></param>
        /// <param name="where"></param>
        /// <param name="whereParam"></param>
        /// <returns></returns>
        protected IEnumerable<T> GetAll<T, T2>(string joinedColumnFK, string where = null, object whereParam = null)
        {
            var schema = GetSchema(typeof(T));
            var schema2 = GetSchema(typeof(T2));

            var sql = $@"SELECT ""{typeof(T).Name}"".* 
                 FROM ""{schema}"".""{typeof(T).Name}"" 
                 JOIN ""{schema2}"".""{typeof(T2).Name}"" 
                 ON ""{typeof(T).Name}"".""{joinedColumnFK}"" = ""{typeof(T2).Name}"".""Id""";

            if (!string.IsNullOrEmpty(where))
            {
                sql += $" WHERE {where}";
            }

            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql, param: whereParam);
            }
        }

        /// <summary>
        /// Executes a SELECT TOP 1 * statement against the schema defined by the T TableAttribute.Schema.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="id">The id of the entity to retrieve</param>
        /// <returns>The retrieved entity or null</returns>
        protected T Get<T>(int id)
        {

            var schema = GetSchema(typeof(T));
            var sql = $@"SELECT * 
                 FROM ""{schema}"".""{typeof(T).Name}""
                 WHERE ""Id"" = @Id
                 LIMIT 1";
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql).FirstOrDefault();
            }
        }

        /// <summary>
        /// Executes a SELECT TOP 1 * statement against the schema defined by the T TableAttribute.Schema.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="id">The id of the entity to retrieve</param>
        /// <returns>The retrieved entity or an exception</returns>
        protected T GetSingle<T>(int id)
        {
            var schema = GetSchema(typeof(T));
            var sql = $@"SELECT * 
                 FROM ""{schema}"".""{typeof(T).Name}""
                 WHERE ""Id"" = {id}
                 LIMIT 1";
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql).Single();
            }
        }

        protected T GetSingleOrDefault<T>(int id)
        {
            var schema = GetSchema(typeof(T));
            var sql = $@"SELECT * 
                 FROM ""{schema}"".""{typeof(T).Name}""
                 WHERE ""Id"" = @Id
                 LIMIT 1";
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql).SingleOrDefault();
            }
        }

        /// <summary>
        /// Executes a SELECT TOP 1 * statement against the schema defined by the T TableAttribute.Schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="whereParam"></param>
        /// <returns>The identified object instance or an exception</returns>

        protected T GetSingle<T>(string where, object whereParam = null)
        {
            var schema = GetSchema(typeof(T));
            var sql = $@"SELECT * 
             FROM ""{schema}"".""{typeof(T).Name}""
             LIMIT 1";


            if (where.IsNotNullOrEmpty())
            {
                sql += $" WHERE {where}";
            }

            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql, param: whereParam).Single();
            }
        }

        /// <summary>
        /// Executes a SELECT TOP 1 * statement against the schema defined by the T TableAttribute.Schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="whereParam"></param>
        /// <returns>The identified object or null</returns>
        protected T GetSingleOrDefault<T>(string where, object whereParam = null, string orderBy = null)
        {
            var schema = GetSchema(typeof(T));
            var sql = $@"SELECT * 
             FROM ""{schema}"".""{typeof(T).Name}""
             LIMIT 1";


            if (where.IsNotNullOrEmpty())
            {
                sql += $" WHERE {where}";
            }

            if (orderBy.IsNotNullOrEmpty())
            {
                sql += $" ORDER BY {orderBy}";
            }

            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql, param: whereParam).SingleOrDefault();
            }
        }

        /// <summary>
        /// Execute the passed statement.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="sql">The sql statement to execute against the db</param>
        /// <param name="param">The object passed to map the parameters of the statement</param>
        /// <returns>A single entity or an exception</returns>
        protected T QuerySingle<T>(string sql, object param = null)
        {
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql, param: param).Single();
            }
        }

        /// <summary>
        /// Execute the passed statement.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="sql">The sql statement to execute against the db</param>
        /// <param name="param">The object passed to map the parameters of the statement</param>
        /// <returns>A single entity or null</returns>
        protected T QuerySingleOrDefault<T>(string sql, object param = null)
        {
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql, param: param).SingleOrDefault();
            }
        }

        /// <summary>
        /// Execute the passed statement.
        /// </summary>
        /// <param name="sql">The sql statement to execute against the db</param>
        /// <param name="param">The object passed to map the parameters of the statement</param>
        /// <returns>The retrieved entity or an empty enumerable</returns>
        protected IEnumerable<dynamic> Query(string sql, object param = null, int? commandTimeout = null)
        {
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query(sql: sql, param: param, commandTimeout: commandTimeout);
            }
        }

        /// <summary>
        /// Execute the passed statement.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="sql">The sql statement to execute against the db</param>
        /// <param name="param">The object passed to map the parameters of the statement</param>
        /// <returns>The retrieved entity or an empty enumerable</returns>
        protected IEnumerable<T> Query<T>(string sql, object param = null)
        {
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query<T>(sql: sql, param: param);
            }
        }

        /// <summary>
        /// Execute the passed statement.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="sql">The sql statement to execute against the db</param>
        /// <param name="param">The object passed to map the parameters of the statement</param>
        /// <returns>The retrieved entity or an empty enumerable</returns>
        protected IEnumerable<T> Query<T, T1>(string sql, Func<T, T1, T> map, object param = null)
        {
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query(sql: sql, map: map, param: param);
            }
        }

        /// <summary>
        /// Execute the passed statement.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="sql">The sql statement to execute against the db</param>
        /// <param name="param">The object passed to map the parameters of the statement</param>
        /// <returns>The retrieved entity or an empty enumerable</returns>
        protected IEnumerable<T> Query<T, T1, T2>(string sql, Func<T, T1, T2, T> map, object param = null)
        {
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query(sql: sql, map: map, param: param);
            }
        }

        /// <summary>
        /// Execute the passed statement.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="sql">The sql statement to execute against the db</param>
        /// <param name="param">The object passed to map the parameters of the statement</param>
        /// <returns>The retrieved entity or an empty enumerable</returns>
        protected IEnumerable<T> Query<T, T1, T2, T3>(string sql, Func<T, T1, T2, T3, T> map, object param = null)
        {
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query(sql: sql, map: map, param: param);
            }
        }

        /// <summary>
        /// Execute the passed statement.
        /// </summary>
        /// <typeparam name="T">The type of the entity retrieved by the query</typeparam>
        /// <param name="sql">The sql statement to execute against the db</param>
        /// <param name="param">The object passed to map the parameters of the statement</param>
        /// <returns>The retrieved entity or an empty enumerable</returns>
        protected IEnumerable<T> Query<T, T1, T2, T3, T4>(string sql, Func<T, T1, T2, T3, T4, T> map, object param = null)
        {
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                return sqlConnection.Query(sql: sql, map: map, param: param);
            }
        }

        /// <summary>
        /// Fetches a single item with its association using a left join.
        /// </summary>
        /// <param name="id">Id of the <see cref="T"/> entity</param>
        /// <param name="foreignKeyColumnName">Name of the column mapping to the <see cref="T1"/> entity</param>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <returns>The retrieved entity or an empty enumerable</returns>
        protected T SingleOrDefault<T, T1>(int id, string foreignKeyColumnName)
        {
            var schema = GetSchema(typeof(T));
            var sql = $@"SELECT * 
                FROM ""{schema}"".""{typeof(T).Name}"" T
                LEFT JOIN ""{schema}"".""{typeof(T1).Name}"" T1 ON T.""{foreignKeyColumnName}"" = T1.""Id""
                WHERE T.""Id"" = @Id
                LIMIT 1";

            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                var result = sqlConnection.Query<T, T1>(
                    sql: sql,
                    map: (t, t1) =>
                    {
                        t.GetType()
                        .GetProperties()
                        .Single(p => (p.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute)?.Name == foreignKeyColumnName)
                        ?.SetValue(t, t1);

                        return t;
                    },
                    param: new { Id = id });

                return result.SingleOrDefault();
            }
        }


        /// <summary>
        /// Fetches multiple items with their association using a left join.
        /// </summary>
        /// <param name="ids">Ids of the <see cref="T"/> entities</param>
        /// <param name="foreignKeyColumnName">Name of the column mapping to the <see cref="T1"/> entity</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <returns>The retrieved entities or an empty enumerable</returns>
        protected IEnumerable<T> GetMany<T, T1>(IEnumerable<int> ids, string foreignKeyColumnName)
        {
            var schema = GetSchema(typeof(T));
            var sql = $@"SELECT * 
                FROM ""{schema}"".""{typeof(T).Name}"" T
                LEFT JOIN ""{schema}"".""{typeof(T1).Name}"" T1 ON T.""{foreignKeyColumnName}"" = T1.""Id""
                WHERE T.""Id"" = ANY(@ids)";

            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                var result = sqlConnection.Query<T, T1>(
                    sql: sql,
                    map: (t, t1) =>
                    {
                        t.GetType()
                        .GetProperties()
                        .Single(p => (p.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute)?.Name == foreignKeyColumnName)
                        ?.SetValue(t, t1);

                        return t;
                    },
                    param: new { ids });

                return result;
            }
        }



        /// <summary>
        /// Fetches multiple items with their association using a left join.
        /// </summary>
        /// <param name="ids">Ids of the <see cref="T"/> entities</param>
        /// <param name="foreignKeyColumnName">Name of the column mapping to the <see cref="T1"/> entity</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <returns>The retrieved entities or an empty enumerable</returns>
        protected IEnumerable<T> GetMany<T, T1>(string foreignKeyColumnName, string where, object whereParam = null)
        {
            var schemaT = GetSchema(typeof(T));
            var schemaT1 = GetSchema(typeof(T1));

            var sql = $@"SELECT * 
                FROM ""{schemaT}"".""{typeof(T).Name}"" T
                LEFT JOIN ""{schemaT1}"".""{typeof(T1).Name}"" T1 ON T.""{foreignKeyColumnName}"" = T1.""Id""";

            if (!string.IsNullOrEmpty(where))
            {
                sql += $" WHERE {where}";
            }

            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                var result = sqlConnection.Query<T, T1>(
                    sql: sql,
                    map: (t, t1) =>
                    {
                        t.GetType()
                        .GetProperties()
                        .Single(p => (p.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute)?.Name == foreignKeyColumnName)
                        ?.SetValue(t, t1);

                        return t;
                    },
                    param: whereParam);

                return result;
            }
        }


    }
}


