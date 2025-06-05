using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace AutoServiceManagement
{
    public static class DatabaseManager
    {
        private static string connectionString = "Data Source=autoservice.db;Version=3;";

        public static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Создание таблицы автомобилей
                var createCarsTable = @"
                CREATE TABLE IF NOT EXISTS Cars (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Make TEXT NOT NULL,
                    Model TEXT NOT NULL,
                    VIN TEXT UNIQUE NOT NULL,
                    Mileage INTEGER NOT NULL,
                    Year INTEGER NOT NULL, 
                    Owner TEXT NOT NULL
                )";

                // Создание таблицы ТО 
                var createMaintenanceTable = @"
                CREATE TABLE IF NOT EXISTS Maintenance (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CarId INTEGER NOT NULL,
                    Date TEXT NOT NULL,
                    WorkType TEXT NOT NULL,
                    Description TEXT,
                    Cost REAL,
                    NextMaintenanceDate TEXT,
                    NextMaintenanceMileage INTEGER,
                    FOREIGN KEY (CarId) REFERENCES Cars(Id)
                )";
                // Новая таблица для истории ТО
                var createMaintenanceHistoryTable = @"
                CREATE TABLE IF NOT EXISTS MaintenanceHistory (
                    Id INTEGER PRIMARY KEY,
                    CarId INTEGER NOT NULL,
                    OriginalMaintenanceId INTEGER NOT NULL,
                    Date TEXT NOT NULL,
                    WorkType TEXT NOT NULL,
                    Description TEXT,
                    Cost REAL,
                    NextMaintenanceDate TEXT,
                    NextMaintenanceMileage INTEGER,
                    CompletedDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (CarId) REFERENCES Cars(Id)
                )";
                new SQLiteCommand(createCarsTable, connection).ExecuteNonQuery();
                new SQLiteCommand(createMaintenanceTable, connection).ExecuteNonQuery();
                new SQLiteCommand(createMaintenanceHistoryTable, connection).ExecuteNonQuery();
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        public static int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new SQLiteCommand(query, connection);

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                return command.ExecuteNonQuery();
            }
        }
        public static int ExecuteNonQuery(SQLiteConnection connection, string query, Dictionary<string, object> parameters = null)
        {
            var command = new SQLiteCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return command.ExecuteNonQuery();
        }

        public static SQLiteDataReader ExecuteReader(string query, Dictionary<string, object> parameters = null)
        {
            var connection = GetConnection();
            connection.Open();
            var command = new SQLiteCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return command.ExecuteReader();
        }
    }
}
