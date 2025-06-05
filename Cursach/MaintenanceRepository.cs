using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace AutoServiceManagement
{
    public static class MaintenanceRepository
    {
        public static void AddMaintenance(Maintenance maintenance)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@carId", maintenance.CarId},
                {"@date", maintenance.Date.ToString("yyyy-MM-dd")},
                {"@workType", maintenance.WorkType},
                {"@description", maintenance.Description ?? (object)DBNull.Value},
                {"@cost", maintenance.Cost},
                {"@nextDate", maintenance.NextMaintenanceDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value},
                {"@nextMileage", maintenance.NextMaintenanceMileage ?? (object)DBNull.Value}
            };

            DatabaseManager.ExecuteNonQuery(
                "INSERT INTO Maintenance (CarId, Date, WorkType, Description, Cost, NextMaintenanceDate, NextMaintenanceMileage) " +
                "VALUES (@carId, @date, @workType, @description, @cost, @nextDate, @nextMileage)",
                parameters);
        }

        public static List<Maintenance> GetMaintenanceHistory(int carId)
        {
            var history = new List<Maintenance>();
            var parameters = new Dictionary<string, object> { { "@carId", carId } };

            using (var reader = DatabaseManager.ExecuteReader(
                "SELECT * FROM Maintenance WHERE CarId = @carId ORDER BY Date DESC", parameters))
            {
                while (reader.Read())
                {
                    history.Add(new Maintenance
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarId = Convert.ToInt32(reader["CarId"]),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        WorkType = reader["WorkType"].ToString(),
                        Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                        Cost = Convert.ToDecimal(reader["Cost"]),
                        NextMaintenanceDate = reader["NextMaintenanceDate"] == DBNull.Value
                            ? (DateTime?)null
                            : DateTime.Parse(reader["NextMaintenanceDate"].ToString()),
                        NextMaintenanceMileage = reader["NextMaintenanceMileage"] == DBNull.Value
                            ? (int?)null
                            : Convert.ToInt32(reader["NextMaintenanceMileage"])
                    });
                }
            }
            return history;
        }

        public static void UpdateMaintenance(Maintenance maintenance)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@id", maintenance.Id},
                {"@date", maintenance.Date.ToString("yyyy-MM-dd")},
                {"@workType", maintenance.WorkType},
                {"@description", maintenance.Description ?? (object)DBNull.Value},
                {"@cost", maintenance.Cost},
                {"@nextDate", maintenance.NextMaintenanceDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value},
                {"@nextMileage", maintenance.NextMaintenanceMileage ?? (object)DBNull.Value}
            };

            DatabaseManager.ExecuteNonQuery(
                "UPDATE Maintenance SET " +
                "Date = @date, " +
                "WorkType = @workType, " +
                "Description = @description, " +
                "Cost = @cost, " +
                "NextMaintenanceDate = @nextDate, " +
                "NextMaintenanceMileage = @nextMileage " +
                "WHERE Id = @id",
                parameters);
        }

        public static void DeleteMaintenance(int id)
        {
            var parameters = new Dictionary<string, object> { { "@id", id } };
            DatabaseManager.ExecuteNonQuery("DELETE FROM Maintenance WHERE Id = @id", parameters);
        }

        public static List<Maintenance> GetUpcomingMaintenanceByDate()
        {
            var upcoming = new List<Maintenance>();
            using (var reader = DatabaseManager.ExecuteReader(
                "SELECT m.* FROM Maintenance m " +
                "JOIN Cars c ON m.CarId = c.Id " +
                "WHERE m.NextMaintenanceDate IS NOT NULL AND m.NextMaintenanceDate <= date('now', '+14 days') " +
                "ORDER BY m.NextMaintenanceDate"))
            {
                while (reader.Read())
                {
                    upcoming.Add(new Maintenance
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarId = Convert.ToInt32(reader["CarId"]),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        WorkType = reader["WorkType"].ToString(),
                        NextMaintenanceDate = DateTime.Parse(reader["NextMaintenanceDate"].ToString())
                    });
                }
            }
            return upcoming;
        }

        public static List<Maintenance> GetUpcomingMaintenanceByMileage()
        {
            var upcoming = new List<Maintenance>();
            using (var reader = DatabaseManager.ExecuteReader(
                "SELECT m.*, c.Mileage FROM Maintenance m " +
                "JOIN Cars c ON m.CarId = c.Id " +
                "WHERE m.NextMaintenanceMileage IS NOT NULL AND c.Mileage >= m.NextMaintenanceMileage - 500 " +
                "ORDER BY m.NextMaintenanceMileage - c.Mileage"))
            {
                while (reader.Read())
                {
                    upcoming.Add(new Maintenance
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarId = Convert.ToInt32(reader["CarId"]),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        WorkType = reader["WorkType"].ToString(),
                        NextMaintenanceMileage = Convert.ToInt32(reader["NextMaintenanceMileage"])
                    });
                }
            }
            return upcoming;
        }
        public static Maintenance GetMaintenanceById(int id)
        {
            var parameters = new Dictionary<string, object> { { "@id", id } };
            using (var reader = DatabaseManager.ExecuteReader(
                "SELECT * FROM Maintenance WHERE Id = @id", parameters))
            {
                if (reader.Read())
                {
                    return new Maintenance
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarId = Convert.ToInt32(reader["CarId"]),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        WorkType = reader["WorkType"].ToString(),
                        Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                        Cost = Convert.ToDecimal(reader["Cost"]),
                        NextMaintenanceDate = reader["NextMaintenanceDate"] == DBNull.Value ?
                            null : (DateTime?)DateTime.Parse(reader["NextMaintenanceDate"].ToString()),
                        NextMaintenanceMileage = reader["NextMaintenanceMileage"] == DBNull.Value ?
                            null : (int?)Convert.ToInt32(reader["NextMaintenanceMileage"])
                    };
                }
            }
            return null;
        }

        // Добавляем метод для проверки и обработки просроченных ТО
        public static void ProcessExpiredMaintenance()
        {
            var expiredRecords = GetExpiredMaintenanceRecords();

            if (expiredRecords.Count == 0)
            {
                Console.WriteLine("\nПросроченных ТО не найдено.");
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            foreach (var record in expiredRecords)
            {
                Console.WriteLine($"Найдено просроченное ТО: {record.WorkType} (ID: {record.Id})");
                Console.WriteLine("1. Удалить запись");
                Console.WriteLine("2. Перенести в историю");
                Console.WriteLine("3. Пропустить");
                Console.Write("Выберите действие: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DeleteMaintenance(record.Id);
                        Console.WriteLine("Запись удалена.");
                        break;
                    case "2":
                        MoveToHistory(record);
                        Console.WriteLine("Запись перенесена в историю.");
                        break;
                    default:
                        Console.WriteLine("Запись оставлена без изменений.");
                        break;
                }
            }
        }

        // Получаем просроченные записи ТО
        private static List<Maintenance> GetExpiredMaintenanceRecords()
        {
            var expiredRecords = new List<Maintenance>();
            string query = "SELECT * FROM Maintenance WHERE NextMaintenanceDate < date('now')";

            using (var reader = DatabaseManager.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    expiredRecords.Add(new Maintenance
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarId = Convert.ToInt32(reader["CarId"]),
                        WorkType = reader["WorkType"].ToString(),
                        // другие поля
                    });
                }
            }
            return expiredRecords;
        }

        private static void MoveToHistory(Maintenance record)
        {
            // 1. Сначала копируем запись в историю
            string insertHistoryQuery = @"
            INSERT INTO MaintenanceHistory 
            (CarId, OriginalMaintenanceId, Date, WorkType, Description, 
            Cost, NextMaintenanceDate, NextMaintenanceMileage)
            SELECT 
            CarId, Id, Date, WorkType, Description, 
            Cost, NextMaintenanceDate, NextMaintenanceMileage
            FROM Maintenance 
            WHERE Id = @id";

            // 2. Затем удаляем из основной таблицы
            string deleteQuery = "DELETE FROM Maintenance WHERE Id = @id";

            // Используем параметры правильно
            var parameters = new Dictionary<string, object>
            {
                { "@id", record.Id }
            };

            // Выполняем в транзакции для безопасности
            using (var connection = new SQLiteConnection(DatabaseManager.GetConnection()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Копируем в историю
                        DatabaseManager.ExecuteNonQuery(connection, insertHistoryQuery, parameters);

                        // Удаляем оригинал
                        DatabaseManager.ExecuteNonQuery(connection, deleteQuery, parameters);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
