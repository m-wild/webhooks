using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Consumer.Repositories
{
    public interface IDatabase
    {
        IDbConnection Connection { get; }
        
        IDbTransaction Transaction { get; }

        void BeginTransaction();

        void Commit();
    }

    public class Database : IDatabase, IDisposable
    {
        private readonly string connectionString;

        public Database(IConfiguration config)
        {
            connectionString = config["ConnectionString"];
            
            _connection = new Lazy<IDbConnection>(() =>
            {
                var conn = new MySqlConnection(connectionString);

                conn.Open();

                return conn;
            });
            
            _transaction = new Lazy<IDbTransaction>(() => _connection.Value.BeginTransaction());
        }


        private Lazy<IDbConnection> _connection;
        public IDbConnection Connection => _connection.Value;

        private Lazy<IDbTransaction> _transaction;
        public IDbTransaction Transaction => _transaction.IsValueCreated ? _transaction.Value : null;

        private bool _committed;

        public void BeginTransaction()
        {
            var _ = _transaction.Value; // initialize the Lazy variable
        }

        public void Commit()
        {
            if (_committed)
            {
                return;
            }
            
            _committed = true;
            
            if (_transaction.IsValueCreated)
            {
                _transaction.Value.Commit();
            }
        }
        
        public void Dispose()
        {
            if (_transaction.IsValueCreated && !_committed)
            {
                _transaction.Value.Rollback();
                _transaction.Value.Dispose();
            }

            if (_connection.IsValueCreated)
            {
                _connection.Value.Dispose();
            }   
        }
    }

}