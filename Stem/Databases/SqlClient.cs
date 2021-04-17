
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Npgsql;
using Stem.Models;

namespace Stem.Databases
{
    public class SqlClient : IDisposable
    {
        private readonly SqlSession _sqlSession;
        private readonly NpgsqlTransaction? _transaction;
        
        public SqlClient(bool autoCommit = false)
        {
            _sqlSession = new SqlSession();

            if (!autoCommit)
            {
                _transaction = _sqlSession.Connection.BeginTransaction();
            }
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
                var model = CastToModel<TModel>(reader, queryColumns);
                if (reader.Read())
                {
                    throw new MultipleRowsFoundException($"Table '{table}' has multiple rows with id of '{id}'.");
                }
                return model;
            }
            return null;
        }
        
        
        /**
         * Adds new row to database.
         * It is possible to only execute one SQL command, using ";" will not work.
         *
         * WARNING: Never let user set the column name! Always explicitly pass it.
         */
        public int Insert(string table, Dictionary<string, dynamic> columns)
        {
            string columnString = string.Join(", ", columns.Keys);
            string parameterString = string.Join(", ", columns.Select((x) => $"@{x.Key}"));
            string statement = $"INSERT INTO {table} ({columnString}) VALUES ({parameterString}) RETURNING id";
            if (IsSqlSafe(statement))
            {
                var command = new NpgsqlCommand(statement, _sqlSession.Connection);
                foreach (KeyValuePair<string, dynamic> column in columns)
                {
                    command.Parameters.AddWithValue(column.Key, column.Value);
                }

                var result = command.ExecuteScalar();
                if (result != null)
                {
                    return (int) result;
                }
            }
            _transaction?.Rollback();
            throw new InsertFailedException("Failed to insert row.");
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }
 
        public void Dispose()
        {
            _sqlSession.Dispose();
        }

        private TModel CastToModel<TModel>(NpgsqlDataReader row, IEnumerable<string> columns) where TModel : BaseModel, new()
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
        
        private bool IsSqlSafe(string statement)
        {
            if (statement.Contains("\"") || statement.Contains("'") || statement.Contains(";"))
            {
                _transaction?.Rollback();
                return false;
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
}
