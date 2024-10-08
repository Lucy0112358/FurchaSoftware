using Dapper;
using Npgsql;
using System.Data;

namespace Domain.Repositories
{
    public class PostgreSqlConnection : IDisposable
    {
        private readonly string _connectionString;
        private NpgsqlConnection _connection;
        private bool _disposed = false;

        public PostgreSqlConnection(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
        }

        public void Close()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public NpgsqlConnection GetConnection()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Connection is not initialized. Call Open() first.");
            }

            return _connection;
        }

        public IEnumerable<dynamic> Query(string sql, object param = null, int? commandTimeout = null)
        {
            return GetConnection().Query(sql, param, commandTimeout: commandTimeout);
        }

        public IEnumerable<T> Query<T>(string sql, object param = null, int? commandTimeout = null)
        {
            return GetConnection().Query<T>(sql, param, commandTimeout: commandTimeout);
        }

        public IEnumerable<T> Query<T>(string sql)
        {
            return GetConnection().Query<T>(sql);
        }

        public IEnumerable<T> Query<T, T1>(string sql, Func<T, T1, T> map, object param = null, int? commandTimeout = null)
        {
            return GetConnection().Query(sql, map, param, commandTimeout: commandTimeout);
        }

        public IEnumerable<T> Query<T, T1, T2>(string sql, Func<T, T1, T2, T> map, object param = null, int? commandTimeout = null)
        {
            return GetConnection().Query(sql, map, param, commandTimeout: commandTimeout);
        }

        public IEnumerable<T> Query<T, T1, T2, T3>(string sql, Func<T, T1, T2, T3, T> map, object param = null, int? commandTimeout = null)
        {
            return GetConnection().Query(sql, map, param, commandTimeout: commandTimeout);
        }

        public IEnumerable<T> Query<T, T1, T2, T3, T4>(string sql, Func<T, T1, T2, T3, T4, T> map, object param = null, int? commandTimeout = null)
        {
            return GetConnection().Query(sql, map, param, commandTimeout: commandTimeout);
        }

        public IEnumerable<T> Query<T, T1, T2, T3, T4, T5>(string sql, Func<T, T1, T2, T3, T4, T5, T> map, object param = null, int? commandTimeout = null)
        {
            return GetConnection().Query(sql, map, param, commandTimeout: commandTimeout);
        }

        public IEnumerable<T> Query<T, T1, T2, T3, T4, T5, T6>(string sql, Func<T, T1, T2, T3, T4, T5, T6, T> map, object param = null, int? commandTimeout = null)
        {
            return GetConnection().Query(sql, map, param, commandTimeout: commandTimeout);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_connection != null)
                {
                    Close();
                    _connection.Dispose();
                    _connection = null;
                }

                _disposed = true;
            }
        }
    }
}
