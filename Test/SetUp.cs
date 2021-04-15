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
            Environment.SetEnvironmentVariable("DB_NAME", "main");
            Environment.SetEnvironmentVariable("DB_HOST", "127.0.0.1");
            Environment.SetEnvironmentVariable("DB_PORT", "5433");
            Environment.SetEnvironmentVariable("DB_MASTER_USER", "postgres");
            Environment.SetEnvironmentVariable("DB_MASTER_PASSWORD", "postgres");

            // Create test database.
            using var sqlSession = new SqlSession();
            new NpgsqlCommand("DROP DATABASE IF EXISTS test", sqlSession.Connection).ExecuteNonQuery();
            new NpgsqlCommand("CREATE DATABASE test", sqlSession.Connection).ExecuteNonQuery();
            
            Environment.SetEnvironmentVariable("DB_NAME", "test");
        }
        
        [TestMethod]
        public void EnsureSetUpOk()
        {
        }
    }
}