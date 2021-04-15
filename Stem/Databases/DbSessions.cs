using System;
using Npgsql;

namespace Stem.Databases
{
    /**
     * Connects to an SQL database using user credentials.
     */
    public class SqlSession : IDisposable
    {
        public readonly NpgsqlConnection Connection;
            
        public SqlSession()
        {
            Connection = new NpgsqlConnection(GetConnectionString());
            Connection.Open();
        }
        
        private static string GetConnectionString(bool masterCredentials = true)
        {
            var username = masterCredentials ? Environment.GetEnvironmentVariable("DB_MASTER_USER") : "";
            var password = masterCredentials ? Environment.GetEnvironmentVariable("DB_MASTER_PASSWORD") : "";
            var mainDatabase = Environment.GetEnvironmentVariable("DB_NAME");
            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var port = Environment.GetEnvironmentVariable("DB_PORT");
            return $"Host={host};Port={port};Username={username};Password={password};Database={mainDatabase}";
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }

    /**
     * Connects to a NoSQL document storages.
     */
    public class Document
    {

        public Document()
        {
        }
    }
}