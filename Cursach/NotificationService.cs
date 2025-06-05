using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoServiceManagement
{
    public static class NotificationService
    {
        public static void CheckMaintenanceNotifications()
        {
            Console.Clear();
            Console.WriteLine("=== Уведомления о ТО ===");

            // Проверка по дате
            var dateNotifications = MaintenanceRepository.GetUpcomingMaintenanceByDate();
            if (dateNotifications.Count > 0)
            {
                Console.WriteLine("\nАвтомобили с предстоящим ТО по дате (в ближайшие 14 дней):");
                foreach (var notification in dateNotifications)
                {
                    var car = CarRepository.GetCarById(notification.CarId);
                    Console.WriteLine($"{car} - След. ТО: {notification.NextMaintenanceDate:dd.MM.yyyy}");
                }
            }
            else
            {
                Console.WriteLine("\nНет автомобилей с предстоящим ТО по дате.");
            }

            // Проверка по пробегу
            var mileageNotifications = MaintenanceRepository.GetUpcomingMaintenanceByMileage();
            if (mileageNotifications.Count > 0)
            {
                Console.WriteLine("\nАвтомобили с предстоящим ТО по пробегу (осталось менее 500 км):");
                foreach (var notification in mileageNotifications)
                {
                    var car = CarRepository.GetCarById(notification.CarId);
                    var remaining = notification.NextMaintenanceMileage - car.Mileage;
                    Console.WriteLine($"{car} - След. ТО: {notification.NextMaintenanceMileage} км (осталось {remaining} км)");
                }
            }
            else
            {
                Console.WriteLine("\nНет автомобилей с предстоящим ТО по пробегу.");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}