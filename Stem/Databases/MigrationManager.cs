using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Npgsql;

namespace Stem.Databases
{
    public class MigrationManager
    {

        public void ensureMigrationTableExists()
        {
            using var sqlSession = new SqlSession();
            string statement = "CREATE TABLE IF NOT EXISTS migrations (id SERIAL PRIMARY KEY, migration VARCHAR(255))";
            var command = new NpgsqlCommand(statement, sqlSession.Connection);
            command.ExecuteNonQuery();
        }
        public void apply()
        {
            string[] files = Directory.GetFiles(Constants.MigrationFolder);
            foreach (var file in files)
            {
                Trace.WriteLine(file);
            }
        }
    }
}