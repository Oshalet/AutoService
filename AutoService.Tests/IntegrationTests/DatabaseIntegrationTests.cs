using Xunit;
using AutoService;
using System.IO;

namespace AutoService.Tests.IntegrationTests
{
    public class DatabaseIntegrationTests : IDisposable
    {
        private readonly string _testDbPath = "test_db.db";

        public DatabaseIntegrationTests()
        {
            if (File.Exists(_testDbPath))
                File.Delete(_testDbPath);

            DatabaseManager.InitializeDatabase(_testDbPath);
        }

        [Fact]
        public void Database_ShouldCreateTables()
        {
            
            using var connection = new SQLiteConnection($"Data Source={_testDbPath}");

            
            connection.Open();
            var command = new SQLiteCommand(
                "SELECT name FROM sqlite_master WHERE type='table'",
                connection);

            var reader = command.ExecuteReader();
            var tables = new List<string>();
            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }

            
            Assert.Contains("Cars", tables);
            Assert.Contains("Maintenance", tables);
        }

        public void Dispose()
        {
            if (File.Exists(_testDbPath))
                File.Delete(_testDbPath);
        }
    }
}