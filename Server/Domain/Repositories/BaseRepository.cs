using Domain.Attributes;
using Domain.Configuration;
using Domain.Entities;
using Domain.Extensions;
using Domain.Repositories;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MqttService.Application.Repositories
{
    public class BaseRepository
    {
        public readonly string furchaSchema = "furcha";
        private readonly NpgsqlConnection furchaContext;
        private const string _defaultPkColumnName = "Id";
        private const string _createdDate = "CreatedDate";
        private const string _modifiedDate = "ModifiedDate";
        private readonly ISanitizer sanitizer;

        public BaseRepository(NpgsqlConnection dbConnection, ISanitizer sanitizer)
        {
            furchaContext = dbConnection;
            this.sanitizer = sanitizer;
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

        /// <summary>
        /// Executes an insert statement against the schema defined by the T TableAttribute.Schema.
        /// </summary>
        /// <param name="objectToInsert"></param>
        /// <returns>The inserted entity</returns>
        protected T Insert<T>(T objectToInsert)
        {
            return Insert(objectToInsert, null);
        }

        /// <summary>
        /// Executes an insert statement against the schema defined by the T TableAttribute.Schema.
        /// The Db connection is managed internally but only 1 connection is used for all inserted items (reducing insert time for many elements by about 50-60%).
        /// </summary>
        protected IEnumerable<T> Insert<T>(IEnumerable<T> objectsToInsert)
        {
            using (var sqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
            {
                var insertedItems = new List<T>();

                foreach (var objectToInsert in objectsToInsert)
                {
                    insertedItems.Add(Insert(objectToInsert, sqlConnection));
                }

                return insertedItems;
            }
        }

        /// <summary>
        /// Executes an insert statement against the schema defined by the T TableAttribute.Schema.
        /// The Db connection is provided and managed by the caller for better performance.
        /// </summary>
        /// <param name="objectToInsert"></param>
        /// <param name="sqlConnection"></param>
        /// <returns>The inserted entity</returns>
        protected T Insert<T>(T objectToInsert, PostgreSqlConnection sqlConnection)
        {
            var entityType = typeof(T);
            var schema = GetSchema(entityType);
            var entityName = entityType.Name;

            var computedColumnsAttribute = entityType.GetCustomAttribute<ComputedColumnsAttribute>();
            var computedColumns = computedColumnsAttribute?.Columns;

            // Prepare insert parameters
            var (columnsString, parameterString, param) = PrepareInsert(objectToInsert, computedColumns);

            // Remove 'id' from the columns and parameters
            if (columnsString.Contains("[id]"))
            {
                columnsString = columnsString.Replace("[id]", string.Empty).Trim().TrimStart(',');
                // Remove corresponding parameter if it exists
                parameterString = parameterString.Replace($"@{nameof(Card.Id)}", string.Empty).Trim().TrimStart(',');
            }

            // Construct the insert SQL statement
            var sql = $@"INSERT INTO {schema}.{entityName} ({columnsString}) 
                 VALUES({parameterString}) 
                 RETURNING *;";

            // Execute the query and return the inserted entity
            if (sqlConnection != null)
            {
                return sqlConnection.Query<T>(sql: sql, param: param).Single();
            }
            else
            {
                using (var internalSqlConnection = new PostgreSqlConnection(furchaContext.ConnectionString))
                {
                    return internalSqlConnection.Query<T>(sql: sql, param: param).Single();
                }
            }
        }
2


        private (string columnsString, string parameterString, Dictionary<string, object> paramSanitized) PrepareInsert<T>(T objectToInsert, string[] computedColumns)
        {
            var properties = objectToInsert.GetType().GetProperties();

            var columnsString = "";
            var parameterString = "";

            // The param list must be created from scratch and all HTML will be removed from it (to avoid injection attacks)
            var paramSanitized = new Dictionary<string, object>();

            for (var i = 0; i < properties.Length; i++)
            {
                var columnAttribute = properties[i].GetCustomAttribute<ColumnAttribute>();

                // skip fields without [Column] attribute (=> computed fields that don't exist in the database)
                if (columnAttribute == null)
                {
                    continue;
                }

                // sometimes the property name of the model is different than the column name in the db table
                var dbColumnName = columnAttribute.Name ?? properties[i].Name;

                //We skip the Id column if it has the value 0 as it is automatically generated by the db
                if (dbColumnName == _defaultPkColumnName)
                {
                    var idValue = (int)properties[i].GetValue(objectToInsert);
                    if (idValue == default)
                    {
                        continue;
                    }
                }

                // skip fields with [ComputedColumn] attribute (=> computed column of the database)
                if (computedColumns != null && computedColumns.Contains(dbColumnName))
                {
                    continue;
                }

                // skip fields that are populated by the DB
                if (dbColumnName == _createdDate || dbColumnName == _modifiedDate)
                {
                    continue;
                }

                if (CanUseType(properties[i].PropertyType))
                {
                    var valueToInsert = sanitizer.SanitizeAndThrowExceptionIfHtml(properties[i].GetValue(objectToInsert));

                    paramSanitized.Add(dbColumnName, valueToInsert);

                    columnsString += $"{(string.IsNullOrEmpty(columnsString) ? string.Empty : ",")}\"{dbColumnName}\"";
                    parameterString += $"{(string.IsNullOrEmpty(parameterString) ? string.Empty : ",")}{GetInsertParameter(dbColumnName)}";
                }
            }

            return (columnsString, parameterString, paramSanitized);
        }

        /// <summary>
        /// Prepares the parameter value for a generated insert statement.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private string GetInsertParameter(string propertyName)
        {
            return $"@{propertyName}";
        }

        /// <summary>
        /// Defines which types can be used to do the mapping with the query generation.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool CanUseType(Type type)
        {
            if (type.IsPrimitive)
            {
                return true;
            }

            if (type.IsClass && type.Name == "String")
            {
                return true;
            }

            if (type.IsValueType && (type.Name == "DateTime" || type.Name == "DateOnly" || type.Name == "Decimal" || type.Name == "Guid" || type.Name.Contains("DbId")))
            {
                return true;
            }

            if (type.IsValueType && type.GenericTypeArguments.Length > 0)
            {
                return CanUseType(type.GenericTypeArguments[0]);
            }

            return false;
        }
    }
}


