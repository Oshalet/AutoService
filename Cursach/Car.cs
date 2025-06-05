using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoServiceManagement
{
    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string VIN { get; set; }
        public int Mileage { get; set; }
        public int Year { get; set; }
        public string Owner { get; set; }

        public override string ToString()
        {
            return $"{Make} {Model} ({Year}), VIN: {VIN}, Пробег: {Mileage}, Владелец: {Owner}";
        }
    }
}
