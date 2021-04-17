using System.IO;
using System.Linq;
using Npgsql;

namespace Stem.Databases
{
    public class MigrationManager
    {
        public MigrationManager()
        {
            using var sqlSession = new SqlSession();
            string statement = "CREATE TABLE IF NOT EXISTS migrations (id SERIAL PRIMARY KEY, migration VARCHAR(255))";
            var command = new NpgsqlCommand(statement, sqlSession.Connection);
            command.ExecuteNonQuery();
        }
        
        public void ApplyAll()
        {
            using var sqlSession = new SqlSession();

            string[] files = Directory.GetFiles(Constants.MigrationFolder);
            string[] appliedMigrations = getAppliedMigrations(files.Length, sqlSession.Connection);
            
            foreach (string filepath in files)
            {
                string fileName = Path.GetFileName(filepath);
                if (!appliedMigrations.Contains(fileName))
                {
                    applySingleMigration(filepath, fileName, sqlSession.Connection);
                }
            }
        }

        private void applySingleMigration(string filepath, string fileName, NpgsqlConnection connection)
        {
            NpgsqlTransaction transaction = connection.BeginTransaction();
            
            string migrationFileContent = File.ReadAllText(filepath);
            new NpgsqlCommand(migrationFileContent, connection).ExecuteNonQuery();
            
            var migrationCommand = new NpgsqlCommand("INSERT INTO migrations (migration) VALUES (@migrationName)", connection);
            migrationCommand.Parameters.AddWithValue("migrationName", fileName);
            migrationCommand.ExecuteNonQuery();
            
            transaction.Commit();
        }

        private string[] getAppliedMigrations(int length, NpgsqlConnection connection)
        {
            string[] migrationsApplied = new string[length];
            var command = new NpgsqlCommand("SELECT migration FROM migrations", connection);
            using NpgsqlDataReader reader = command.ExecuteReader();
            
            int i = 0;
            while (reader.Read())
            {
                migrationsApplied[i] = reader.GetString(0);
                i++;
            }
            reader.Close();
            
            return migrationsApplied;
        }
    }
}