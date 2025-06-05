using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace AutoServiceManagement
{
    public static class CarRepository
    {
        public static void AddCar(Car car)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@make", car.Make},
                {"@model", car.Model},
                {"@vin", car.VIN},
                {"@mileage", car.Mileage},
                {"@year", car.Year},
                {"@owner", car.Owner}
            };

            DatabaseManager.ExecuteNonQuery(
                "INSERT INTO Cars (Make, Model, VIN, Mileage, Year, Owner) " +
                "VALUES (@make, @model, @vin, @mileage, @year, @owner)",
                parameters);
        }

        public static List<Car> GetAllCars(string filter = "")
        {
            var cars = new List<Car>();
            string query = string.IsNullOrEmpty(filter)
                ? "SELECT * FROM Cars"
                : $"SELECT * FROM Cars WHERE {filter}";

            using (var reader = DatabaseManager.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    cars.Add(new Car
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Make = reader["Make"].ToString(),
                        Model = reader["Model"].ToString(),
                        VIN = reader["VIN"].ToString(),
                        Mileage = Convert.ToInt32(reader["Mileage"]),
                        Year = Convert.ToInt32(reader["Year"]),
                        Owner = reader["Owner"].ToString()
                    });
                }
            }
            return cars;
        }

        public static Car GetCarById(int id)
        {
            var parameters = new Dictionary<string, object> { { "@id", id } };
            using (var reader = DatabaseManager.ExecuteReader("SELECT * FROM Cars WHERE Id = @id", parameters))
            {
                if (reader.Read())
                {
                    return new Car
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Make = reader["Make"].ToString(),
                        Model = reader["Model"].ToString(),
                        VIN = reader["VIN"].ToString(),
                        Mileage = Convert.ToInt32(reader["Mileage"]),
                        Year = Convert.ToInt32(reader["Year"]),
                        Owner = reader["Owner"].ToString()
                    };
                }
            }
            return null;
        }

        public static void UpdateCar(Car car)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@id", car.Id},
                {"@make", car.Make},
                {"@model", car.Model},
                {"@vin", car.VIN},
                {"@mileage", car.Mileage},
                {"@year", car.Year},
                {"@owner", car.Owner}
            };

            DatabaseManager.ExecuteNonQuery(
                "UPDATE Cars SET Make = @make, Model = @model, VIN = @vin, " +
                "Mileage = @mileage, Year = @year, Owner = @owner WHERE Id = @id",
                parameters);
        }

        public static void DeleteCar(int id)
        {
            var parameters = new Dictionary<string, object> { { "@id", id } };
            DatabaseManager.ExecuteNonQuery("DELETE FROM Cars WHERE Id = @id", parameters);
        }
    }
}
