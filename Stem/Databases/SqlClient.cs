
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Npgsql;
using NpgsqlTypes;

namespace Stem.Databases
{
    public class SqlClient : IDisposable
    {
        private readonly SqlSession _sqlSession;
        private readonly NpgsqlTransaction _transaction;
        
        public SqlClient()
        {
            _sqlSession = new SqlSession();
            _transaction = _sqlSession.Connection.BeginTransaction();
        }

        /**
         * Returns one record, when multiple ones are found it will raise an error.
         */
        public TModel? FindById<TModel>(string table, string[] columns, int id) where TModel : BaseModel, new()
        {
            string[] queryColumns = columns.Prepend("id").ToArray();
            string columnString = string.Join(", ", queryColumns);
            string statement = $"SELECT {columnString} FROM {table} WHERE id=(@id)";
            var command = new NpgsqlCommand(statement, _sqlSession.Connection);
            command.Parameters.AddWithValue("id", id);
            
            using NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                var model = castToModel<TModel>(reader, queryColumns);
                if (reader.Read())
                {
                    throw new MultipleRowsFoundException($"Table '{table}' has multiple rows with id of '{id}'.");
                }
                return model;
            }
            return null;
        }
        
        
        /**
         * Adds new row to database. Keep the statement safe, it will throw errors if it contains unsafe contents.
         * It is possible to only execute one SQL command, using ";" will not work.
         */
        public int Insert(string table, string[] columns, string[] parameters)
        {
            string columnString = string.Join(", ", columns);
            string parameterString = string.Join(", ", columns.Select((x, i) => $"@p{i.ToString()}"));
            string statement = $"INSERT INTO {table} ({columnString}) VALUES ({parameterString}) RETURNING id";
            var command = new NpgsqlCommand(statement, _sqlSession.Connection, _transaction);
            int i = 0;
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue($"p{i.ToString()}", parameter);
                i++;
            }
            var x = command.ExecuteScalar();
            if (x != null)
            {
                return (int) x;
            }
            _transaction.Rollback();
            throw new InsertFailedException("Failed to insert row.");
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
 
        public void Dispose()
        {
            _sqlSession.Dispose();
        }

        private TModel castToModel<TModel>(NpgsqlDataReader row, IEnumerable<string> columns) where TModel : BaseModel, new()
        {
            var model = new TModel();
            var properties = model.GetType();
            int i = 0;
            foreach (var column in columns)
            {
                var property = properties.GetProperty(column);
                if (property != null)
                {
                    property.SetValue(model, row[i]);
                }
                i++;
            }

            return model;
        }
        
        private bool isSqlSafe(string statement)
        {
            // Prevents constructing string 
            if (statement.Contains("\"") || statement.Contains("'") || statement.Contains(";"))
            {
                //_transaction.Rollback();
                throw new UnsafeSqlException($"Unsafe sql statement: {statement}!");
            }

            return true;
        }
    }

    public class MultipleRowsFoundException : Exception
    {
        public MultipleRowsFoundException(string msg) : base(msg)
        {
            
        }
    }
    public class InsertFailedException : Exception
    {
        public InsertFailedException(string msg) : base(msg)
        {
            
        }
    }
    public class UnsafeSqlException : Exception
    {
        public UnsafeSqlException(string msg) : base(msg)
        {
            
        }
    }
}
