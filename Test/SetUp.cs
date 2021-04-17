using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Stem.Databases;

namespace Test
{
    [TestClass]
    public class Setup
    {
        [AssemblyInitialize]
        public static void SetUp(TestContext context)
        {
            using var sqlSession = new SqlSession();
            new NpgsqlCommand("DROP DATABASE IF EXISTS test", sqlSession.Connection).ExecuteNonQuery();
            new NpgsqlCommand("CREATE DATABASE test", sqlSession.Connection).ExecuteNonQuery();
            
            Environment.SetEnvironmentVariable("DB_NAME", "test");
        }
        
        /** Before any test can be run we need to apply migrations. */
        [TestMethod]
        public void Migrate()
        {
            var migrations = new MigrationManager();
            migrations.ensureMigrationTableExists();
            migrations.apply();
        }
    }
}