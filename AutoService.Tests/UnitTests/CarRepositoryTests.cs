using Xunit;
using Moq;
using AutoService;
using System.Data.SQLite;

namespace AutoService.Tests.UnitTests
{
    public class CarRepositoryTests
    {
        [Fact]
        public void AddCar_ShouldInsertValidCar()
        {
            var mockDb = new Mock<IDatabaseManager>();
            var repository = new CarRepository(mockDb.Object);
            var testCar = new Car
            {
                Make = "Toyota",
                Model = "Camry",
                VIN = "JT2BF22K3W0123456",
                Mileage = 150000,
                Year = 2018,
                Owner = "Иванов И.И."
            };

            repository.AddCar(testCar);

            mockDb.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(null, "Model", "VIN12345678901234", 2020, 10000, "Owner")]
        [InlineData("Make", null, "VIN12345678901234", 2020, 10000, "Owner")]
        public void AddCar_ShouldThrowOnInvalidData(string make, string model, string vin, int year, int mileage, string owner)
        {
            
            var repository = new CarRepository(new DatabaseManager());
            var invalidCar = new Car { Make = make, Model = model, VIN = vin, Year = year, Mileage = mileage, Owner = owner };

            Assert.Throws<ArgumentException>(() => repository.AddCar(invalidCar));
        }
    }
}