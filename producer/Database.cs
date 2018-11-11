using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace producer
{
    public interface IDatabase
    {
        IDbConnection Connection { get; }
    }

    public class Database : IDatabase
    {
        private readonly string connectionString;

        public Database(IConfiguration config)
        {
            connectionString = config["ConnectionString"];
        }

        public IDbConnection Connection
        {
            get
            {
                var conn = new MySqlConnection(connectionString);

                conn.Open();

                return conn;
            }
        }
    }

}