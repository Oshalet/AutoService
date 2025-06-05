using System;
using System.Collections.Generic;

namespace AutoServiceManagement
{
    public static class UserInterface
    {
        public static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Система управления автосервисом ===");
                Console.WriteLine("1. Добавить автомобиль");
                Console.WriteLine("2. Просмотреть список автомобилей");
                Console.WriteLine("3. Поиск автомобиля");
                Console.WriteLine("4. Назначить ТО");
                Console.WriteLine("5. Просмотреть историю ТО");
                Console.WriteLine("6. Проверить уведомления о ТО");
                Console.WriteLine("7. Редактировать запись");
                Console.WriteLine("8. Удалить запись");
                Console.WriteLine("9. Проверить просроченные ТО");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddCarMenu();
                        break;
                    case "2":
                        ViewCarsMenu();
                        break;
                    case "3":
                        SearchCarMenu();
                        break;
                    case "4":
                        AddMaintenanceMenu();
                        break;
                    case "5":
                        ViewMaintenanceHistoryMenu();
                        break;
                    case "6":
                        NotificationService.CheckMaintenanceNotifications();
                        break;
                    case "7":
                        EditRecordMenu();
                        break;
                    case "8":
                        DeleteRecordMenu();
                        break;
                    case "9":
                        MaintenanceRepository.ProcessExpiredMaintenance();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void AddCarMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Добавление автомобиля ===");

            var car = new Car();

            Console.Write("Марка: ");
            car.Make = Console.ReadLine();

            Console.Write("Модель: ");
            car.Model = Console.ReadLine();

            Console.Write("VIN: ");
            car.VIN = Console.ReadLine();

            Console.Write("Пробег: ");
            if (!int.TryParse(Console.ReadLine(), out int mileage))
            {
                Console.WriteLine("Некорректный пробег!");
                return;
            }
            car.Mileage = mileage;

            Console.Write("Год выпуска: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("Некорректный год!");
                return;
            }
            car.Year = year;

            Console.Write("Владелец: ");
            car.Owner = Console.ReadLine();

            try
            {
                CarRepository.AddCar(car);
                Console.WriteLine("Автомобиль успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void ViewCarsMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Список автомобилей ===");

            Console.WriteLine("Фильтры:");
            Console.WriteLine("1. Без фильтра");
            Console.WriteLine("2. По марке");
            Console.WriteLine("3. По году выпуска");
            Console.WriteLine("4. По пробегу");
            Console.Write("Выберите фильтр: ");

            var filterChoice = Console.ReadLine();
            string filter = "";

            switch (filterChoice)
            {
                case "1":
                    filter = "";
                    break;
                case "2":
                    Console.Write("Введите марку: ");
                    var make = Console.ReadLine();
                    filter = $"Make LIKE '%{make}%'";
                    break;
                case "3":
                    Console.Write("Введите минимальный год: ");
                    var minYear = Console.ReadLine();
                    Console.Write("Введите максимальный год: ");
                    var maxYear = Console.ReadLine();
                    filter = $"Year BETWEEN {minYear} AND {maxYear}";
                    break;
                case "4":
                    Console.Write("Введите минимальный пробег: ");
                    var minMileage = Console.ReadLine();
                    Console.Write("Введите максимальный пробег: ");
                    var maxMileage = Console.ReadLine();
                    filter = $"Mileage BETWEEN {minMileage} AND {maxMileage}";
                    break;
                default:
                    Console.WriteLine("Неверный выбор фильтра!");
                    return;
            }

            var cars = CarRepository.GetAllCars(filter);

            Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-17} {4,-10} {5,-8} {6,-20}",
                "ID", "Марка", "Модель", "VIN", "Пробег", "Год", "Владелец");
            Console.WriteLine(new string('-', 90));

            foreach (var car in cars)
            {
                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-17} {4,-10} {5,-8} {6,-20}",
                    car.Id, car.Make, car.Model, car.VIN, car.Mileage, car.Year, car.Owner);
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void SearchCarMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Поиск автомобиля ===");

            Console.Write("Введите VIN, марку, модель или владельца для поиска: ");
            var searchTerm = Console.ReadLine();

            var cars = CarRepository.GetAllCars(
                $"VIN LIKE '%{searchTerm}%' OR Make LIKE '%{searchTerm}%' " +
                $"OR Model LIKE '%{searchTerm}%' OR Owner LIKE '%{searchTerm}%'");

            if (cars.Count == 0)
            {
                Console.WriteLine("Автомобили не найдены.");
            }
            else
            {
                Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-17} {4,-10} {5,-8} {6,-20}",
                    "ID", "Марка", "Модель", "VIN", "Пробег", "Год", "Владелец");
                Console.WriteLine(new string('-', 90));

                foreach (var car in cars)
                {
                    Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-17} {4,-10} {5,-8} {6,-20}",
                        car.Id, car.Make, car.Model, car.VIN, car.Mileage, car.Year, car.Owner);
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void AddMaintenanceMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Назначение ТО ===");

            // Показать список автомобилей для выбора
            ViewCarsMenu();

            Console.Write("\nВведите ID автомобиля для ТО: ");
            if (!int.TryParse(Console.ReadLine(), out int carId))
            {
                Console.WriteLine("Некорректный ID!");
                return;
            }

            var maintenance = new Maintenance { CarId = carId };

            Console.Write("Дата ТО (дд.мм.гггг): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                Console.WriteLine("Некорректная дата!");
                return;
            }
            maintenance.Date = date;

            Console.Write("Вид работ: ");
            maintenance.WorkType = Console.ReadLine();

            Console.Write("Описание (не обязательно): ");
            maintenance.Description = Console.ReadLine();

            Console.Write("Стоимость (не обязательно): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal cost))
            {
                cost = 0;
            }
            maintenance.Cost = cost;

            Console.Write("Дата следующего ТО (дд.мм.гггг, не обязательно): ");
            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime nextDate))
            {
                maintenance.NextMaintenanceDate = nextDate;
            }

            Console.Write("Пробег следующего ТО (не обязательно): ");
            if (int.TryParse(Console.ReadLine(), out int nextMileage))
            {
                maintenance.NextMaintenanceMileage = nextMileage;
            }

            try
            {
                MaintenanceRepository.AddMaintenance(maintenance);
                Console.WriteLine("ТО успешно добавлено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void ViewMaintenanceHistoryMenu()
        {
            Console.Clear();
            Console.WriteLine("=== История ТО ===");

            // Показать список автомобилей для выбора
            ViewCarsMenu();

            Console.Write("\nВведите ID автомобиля для просмотра истории ТО: ");
            if (!int.TryParse(Console.ReadLine(), out int carId))
            {
                Console.WriteLine("Некорректный ID!");
                return;
            }

            var car = CarRepository.GetCarById(carId);
            if (car == null)
            {
                Console.WriteLine("Автомобиль не найден!");
                return;
            }

            Console.WriteLine($"\nИстория ТО для: {car}");

            var history = MaintenanceRepository.GetMaintenanceHistory(carId);

            if (history.Count == 0)
            {
                Console.WriteLine("История ТО пуста.");
            }
            else
            {
                Console.WriteLine("\n{0,-5} {1,-12} {2,-20} {3,-30} {4,-10} {5,-15} {6,-15}",
                    "ID", "Дата", "Вид работ", "Описание", "Стоимость", "След. дата", "След. пробег");
                Console.WriteLine(new string('-', 110));

                foreach (var record in history)
                {
                    Console.WriteLine("{0,-5} {1,-12} {2,-20} {3,-30} {4,-10} {5,-15} {6,-15}",
                        record.Id,
                        record.Date.ToString("dd.MM.yyyy"),
                        record.WorkType,
                        record.Description ?? "",
                        record.Cost,
                        record.NextMaintenanceDate?.ToString("dd.MM.yyyy") ?? "",
                        record.NextMaintenanceMileage?.ToString() ?? "");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void EditRecordMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Редактирование записи ===");
            Console.WriteLine("1. Редактировать автомобиль");
            Console.WriteLine("2. Редактировать запись ТО");
            Console.Write("Выберите тип записи: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    EditCarMenu();
                    break;
                case "2":
                    EditMaintenanceMenu();
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }

        private static void EditCarMenu()
        {
            ViewCarsMenu();

            Console.Write("\nВведите ID автомобиля для редактирования: ");
            if (!int.TryParse(Console.ReadLine(), out int carId))
            {
                Console.WriteLine("Некорректный ID!");
                return;
            }

            var car = CarRepository.GetCarById(carId);
            if (car == null)
            {
                Console.WriteLine("Автомобиль не найден!");
                return;
            }

            Console.WriteLine("\nТекущие данные:");
            Console.WriteLine($"Марка: {car.Make}");
            Console.WriteLine($"Модель: {car.Model}");
            Console.WriteLine($"VIN: {car.VIN}");
            Console.WriteLine($"Пробег: {car.Mileage}");
            Console.WriteLine($"Год: {car.Year}");
            Console.WriteLine($"Владелец: {car.Owner}");

            Console.WriteLine("\nВведите новые данные (оставьте пустым, чтобы не изменять):");

            Console.Write("Марка: ");
            var make = Console.ReadLine();
            if (!string.IsNullOrEmpty(make)) car.Make = make;

            Console.Write("Модель: ");
            var model = Console.ReadLine();
            if (!string.IsNullOrEmpty(model)) car.Model = model;

            Console.Write("VIN: ");
            var vin = Console.ReadLine();
            if (!string.IsNullOrEmpty(vin)) car.VIN = vin;

            Console.Write("Пробег: ");
            var mileageStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(mileageStr)
                && int.TryParse(mileageStr, out int mileage))
            {
                car.Mileage = mileage;
            }

            Console.Write("Год выпуска: ");
            var yearStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(yearStr)
                && int.TryParse(yearStr, out int year))
            {
                car.Year = year;
            }

            Console.Write("Владелец: ");
            var owner = Console.ReadLine();
            if (!string.IsNullOrEmpty(owner)) car.Owner = owner;

            try
            {
                CarRepository.UpdateCar(car);
                Console.WriteLine("Автомобиль успешно обновлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void EditMaintenanceMenu()
        {
            // Показываем историю ТО для выбора записи
            ViewMaintenanceHistoryMenu();

            Console.Write("\nВведите ID записи ТО для редактирования: ");
            if (!int.TryParse(Console.ReadLine(), out int maintenanceId))
            {
                Console.WriteLine("Некорректный ID!");
                Console.ReadKey();
                return;
            }

            // Получаем текущие данные ТО
            var maintenance = MaintenanceRepository.GetMaintenanceById(maintenanceId);
            if (maintenance == null)
            {
                Console.WriteLine("Запись ТО не найдена!");
                Console.ReadKey();
                return;
            }

            // Получаем связанный автомобиль
            var car = CarRepository.GetCarById(maintenance.CarId);
            bool continueEditing = true;

            while (continueEditing)
            {
                Console.Clear();
                Console.WriteLine($"=== Редактирование ТО для {car.Make} {car.Model} ===");
                Console.WriteLine("Текущие данные:");
                Console.WriteLine($"1. Дата ТО: {maintenance.Date:dd.MM.yyyy}");
                Console.WriteLine($"2. Вид работ: {maintenance.WorkType}");
                Console.WriteLine($"3. Описание: {maintenance.Description ?? "нет"}");
                Console.WriteLine($"4. Стоимость: {maintenance.Cost}");
                Console.WriteLine($"5. След. ТО (дата): {maintenance.NextMaintenanceDate?.ToString("dd.MM.yyyy") ?? "не задано"}");
                Console.WriteLine($"6. След. ТО (пробег): {maintenance.NextMaintenanceMileage?.ToString() ?? "не задан"}");
                Console.WriteLine("\nВведите номера полей для редактирования через запятую (1-6)");
                Console.WriteLine("Или введите 0 для сохранения и выхода");

                var input = Console.ReadLine();
                if (input == "0")
                {
                    continueEditing = false;
                    break;
                }

                var fieldsToEdit = input.Split(',')
                                      .Select(s => s.Trim())
                                      .Where(s => !string.IsNullOrEmpty(s))
                                      .Distinct();

                foreach (var field in fieldsToEdit)
                {
                    switch (field)
                    {
                        case "1":
                            Console.Write("Новая дата ТО (дд.мм.гггг): ");
                            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null,
                                System.Globalization.DateTimeStyles.None, out DateTime newDate))
                            {
                                maintenance.Date = newDate;
                            }
                            else
                            {
                                Console.WriteLine("Некорректная дата! Поле не изменено.");
                                Console.ReadKey();
                            }
                            break;

                        case "2":
                            Console.Write("Новый вид работ: ");
                            maintenance.WorkType = Console.ReadLine();
                            break;

                        case "3":
                            Console.Write("Новое описание: ");
                            var description = Console.ReadLine();
                            maintenance.Description = string.IsNullOrWhiteSpace(description) ? null : description;
                            break;

                        case "4":
                            Console.Write("Новая стоимость: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal newCost))
                            {
                                maintenance.Cost = newCost;
                            }
                            else
                            {
                                Console.WriteLine("Некорректная стоимость! Поле не изменено.");
                                Console.ReadKey();
                            }
                            break;

                        case "5":
                            Console.Write("Новая дата след. ТО (дд.мм.гггг или пусто для сброса): ");
                            var dateInput = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(dateInput))
                            {
                                maintenance.NextMaintenanceDate = null;
                            }
                            else if (DateTime.TryParseExact(dateInput, "dd.MM.yyyy", null,
                                System.Globalization.DateTimeStyles.None, out DateTime nextDate))
                            {
                                maintenance.NextMaintenanceDate = nextDate;
                            }
                            else
                            {
                                Console.WriteLine("Некорректная дата! Поле не изменено.");
                                Console.ReadKey();
                            }
                            break;

                        case "6":
                            Console.Write("Новый пробег для след. ТО (число или пусто для сброса): ");
                            var mileageInput = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(mileageInput))
                            {
                                maintenance.NextMaintenanceMileage = null;
                            }
                            else if (int.TryParse(mileageInput, out int nextMileage))
                            {
                                maintenance.NextMaintenanceMileage = nextMileage;
                            }
                            else
                            {
                                Console.WriteLine("Некорректный пробег! Поле не изменено.");
                                Console.ReadKey();
                            }
                            break;

                        default:
                            Console.WriteLine($"Некорректный номер поля: {field}");
                            Console.ReadKey();
                            break;
                    }
                }
            }

            // Сохранение изменений
            try
            {
                MaintenanceRepository.UpdateMaintenance(maintenance);
                Console.WriteLine("\nВсе изменения успешно сохранены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при сохранении: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void DeleteRecordMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Удаление записи ===");
            Console.WriteLine("1. Удалить автомобиль");
            Console.WriteLine("2. Удалить запись ТО");
            Console.Write("Выберите тип записи: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DeleteCarMenu();
                    break;
                case "2":
                    DeleteMaintenanceMenu();
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }

        private static void DeleteCarMenu()
        {
            ViewCarsMenu();

            Console.Write("\nВведите ID автомобиля для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int carId))
            {
                Console.WriteLine("Некорректный ID!");
                return;
            }

            try
            {
                CarRepository.DeleteCar(carId);
                Console.WriteLine("Автомобиль успешно удален!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void DeleteMaintenanceMenu()
        {
            ViewMaintenanceHistoryMenu();

            Console.Write("\nВведите ID записи ТО для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int maintenanceId))
            {
                Console.WriteLine("Некорректный ID!");
                return;
            }

            try
            {
                MaintenanceRepository.DeleteMaintenance(maintenanceId);
                Console.WriteLine("Запись ТО успешно удалена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
} 
