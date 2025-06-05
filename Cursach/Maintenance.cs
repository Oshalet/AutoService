using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoServiceManagement
{
    public class Maintenance
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public DateTime Date { get; set; }
        public string WorkType { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public int? NextMaintenanceMileage { get; set; }

        public override string ToString()
        {
            return $"{Date:dd.MM.yyyy} - {WorkType} (Стоимость: {Cost}), " +
                   $"След. ТО: {NextMaintenanceDate:dd.MM.yyyy} или {NextMaintenanceMileage} км";
        }
    }
}
