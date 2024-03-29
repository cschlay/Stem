using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Stem.Databases;
using Stem.Models;

namespace Test
{
    public class TestModel : BaseModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    
    [TestClass]
    public class SqlClientTest
    {
        
        [ClassInitialize]
        public static void SetUp(TestContext testContext)
        {
            using var session = new SqlSession();
            var command = new NpgsqlCommand("CREATE TABLE test_sqlclient (id SERIAL PRIMARY KEY , name VARCHAR(50) NOT NULL)", session.Connection);
            command.ExecuteNonQuery();
            
            // A test user
            NpgsqlDataReader reader = new NpgsqlCommand("INSERT INTO account (email) VALUES ('test@laych.dev') RETURNING pid", session.Connection).ExecuteReader();
            reader.Read();
            Trace.WriteLine(reader.GetGuid(0).ToString());
        }

        [TestMethod]
        public void InsertRow()
        {
            using var sqlClient = new SqlClient();
            int id = sqlClient.Insert("test_sqlclient", new Dictionary<string, dynamic>{{"name", "Lilac"}});
            sqlClient.Commit();
            TestModel result = sqlClient.FindById<TestModel>("test_sqlclient", new string[]{"name"}, id);
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.id);
            Assert.AreEqual("Lilac", result.name);
        }
        
        [TestMethod]
        public void TestMethod2()
        {
            //var connection = sqlSession.Connect();
        }
    }
}