using System;
using System.Diagnostics;
using Npgsql;

namespace Stem.Databases
{
    /**
     * Connects to an SQL database using user credentials.
     */
    public class SqlSession : IDisposable
    {
        public readonly NpgsqlConnection Connection;
        private readonly string _database;
        
        public SqlSession()
        {
            // Database is exceptionally defaulting to environment variable because of tests.
            _database = Environment.GetEnvironmentVariable("DB_NAME") ?? Constants.DbName;
            Connection = new NpgsqlConnection(GetConnectionString());
            Connection.Open();
        }
        
        private string GetConnectionString(bool masterCredentials = true)
        {
            var username = masterCredentials ? Constants.DbMasterUser : "";
            var password = masterCredentials ? Constants.DbMasterPassword : "";
            return $"Host={Constants.DbHost};Port={Constants.DbPort};Username={username};Password={password};Database={_database}";
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