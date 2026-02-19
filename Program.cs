using System;
using System.Collections.Generic;
using System.IO;

namespace AvtoparkLab
{
    class Program
    {
        static ExcelHelper excelHelper;
        static List<Car> cars;
        static List<Driver> drivers;
        static List<Trip> trips;
        static DatabaseHelper dbHelper;

        static void Main(string[] args)
        {
            string filePath = "LR5-var4.xlsx";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден: " + Path.GetFullPath(filePath));
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }

            excelHelper = new ExcelHelper(filePath);

            Console.WriteLine("Загрузка данных...");
            cars = excelHelper.ReadCars();
            drivers = excelHelper.ReadDrivers();
            trips = excelHelper.ReadTrips();

            Console.WriteLine("\n=== ИТОГИ ЗАГРУЗКИ ===");
            Console.WriteLine("Автомобилей: " + cars.Count);
            Console.WriteLine("Водителей: " + drivers.Count);
            Console.WriteLine("Рейсов: " + trips.Count);

            dbHelper = new DatabaseHelper(cars, drivers, trips);

            bool isWorking = true;

            while (isWorking)
            {
                ShowMenu();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": dbHelper.ViewAllCars(); WaitForKey(); break;
                    case "2": dbHelper.ViewAllDrivers(); WaitForKey(); break;
                    case "3": dbHelper.ViewAllTrips(); WaitForKey(); break;
                    case "4": DeleteMenu(); WaitForKey(); break;
                    case "5": AddMenu(); WaitForKey(); break;
                    case "6": ExecuteQueries(); WaitForKey(); break;
                    case "7": isWorking = false; break;
                    default:
                        Console.WriteLine("Ошибка: введите число от 1 до 7.");
                        WaitForKey();
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("ГЛАВНОЕ МЕНЮ");
            Console.WriteLine("1. Автомобили");
            Console.WriteLine("2. Водители");
            Console.WriteLine("3. Рейсы");
            Console.WriteLine("4. Удалить");
            Console.WriteLine("5. Добавить");
            Console.WriteLine("6. Запросы");
            Console.WriteLine("7. Выход");
            Console.Write("Ваш выбор: ");
        }

        static void WaitForKey()
        {
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        static void DeleteMenu()
        {
            Console.Clear();
            Console.WriteLine("УДАЛЕНИЕ");
            Console.WriteLine("1. Автомобиль");
            Console.WriteLine("2. Водитель");
            Console.WriteLine("3. Рейс");
            Console.Write("Выберите тип: ");

            string choice = Console.ReadLine();

            if (choice != "1" && choice != "2" && choice != "3")
            {
                Console.WriteLine("Ошибка: выберите 1, 2 или 3.");
                return;
            }

            Console.Write("Введите ID для удаления: ");
            string idInput = Console.ReadLine();

            if (!int.TryParse(idInput, out int id))
            {
                Console.WriteLine("Ошибка: ID должен быть целым числом.");
                return;
            }

            if (choice == "1") dbHelper.DeleteCarById(id);
            else if (choice == "2") dbHelper.DeleteDriverById(id);
            else if (choice == "3") dbHelper.DeleteTripById(id);

            excelHelper.SaveData(cars, drivers, trips);
        }

        static void AddMenu()
        {
            Console.Clear();
            Console.WriteLine("ДОБАВЛЕНИЕ");
            Console.WriteLine("1. Автомобиль");
            Console.WriteLine("2. Водитель");
            Console.WriteLine("3. Рейс");
            Console.Write("Выберите тип: ");

            string choice = Console.ReadLine();

            if (choice != "1" && choice != "2" && choice != "3")
            {
                Console.WriteLine("Ошибка: выберите 1, 2 или 3.");
                return;
            }

            bool success = false;

            if (choice == "1")
            {
                Console.Write("Марка: ");
                string brand = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(brand))
                {
                    Console.WriteLine("Ошибка: марка не может быть пустой.");
                    return;
                }

                Console.Write("Модель: ");
                string model = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(model))
                {
                    Console.WriteLine("Ошибка: модель не может быть пустой.");
                    return;
                }

                Console.Write("Год выпуска: ");
                string yearInput = Console.ReadLine();

                if (!int.TryParse(yearInput, out int year))
                {
                    Console.WriteLine("Ошибка: год должен быть числом (например, 2020)");
                    return;
                }

                if (year < 1900 || year > 2030)
                {
                    Console.WriteLine("Ошибка: год должен быть от 1900 до 2030");
                    return;
                }

                dbHelper.AddCar(brand, model, year);
                success = true;
            }
            else if (choice == "2")
            {
                Console.Write("Имя: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Ошибка: имя не может быть пустым.");
                    return;
                }

                Console.Write("Возраст: ");
                string ageInput = Console.ReadLine();

                if (!int.TryParse(ageInput, out int age))
                {
                    Console.WriteLine("Ошибка: возраст должен быть числом (например, 25)");
                    return;
                }

                if (age < 18 || age > 100)
                {
                    Console.WriteLine("Ошибка: возраст должен быть от 18 до 100 лет");
                    return;
                }

                Console.Write("Стаж вождения: ");
                string expInput = Console.ReadLine();

                if (!int.TryParse(expInput, out int exp))
                {
                    Console.WriteLine("Ошибка: стаж должен быть числом (например, 5)");
                    return;
                }

                if (exp < 0 || exp > age - 18)
                {
                    Console.WriteLine("Ошибка: стаж не может быть больше возраста минус 18 лет");
                    return;
                }

                dbHelper.AddDriver(name, age, exp);
                success = true;
            }
            else if (choice == "3")
            {
                Console.Write("ID автомобиля: ");
                string carIdInput = Console.ReadLine();

                if (!int.TryParse(carIdInput, out int carId))
                {
                    Console.WriteLine("Ошибка: ID должен быть числом");
                    return;
                }

                Console.Write("ID водителя: ");
                string driverIdInput = Console.ReadLine();

                if (!int.TryParse(driverIdInput, out int driverId))
                {
                    Console.WriteLine("Ошибка: ID должен быть числом");
                    return;
                }

                Console.Write("Дата начала (ГГГГ-ММ-ДД): ");
                string startInput = Console.ReadLine();

                DateTime startDate;
                if (!DateTime.TryParse(startInput, out startDate))
                {
                    Console.WriteLine("Ошибка: неправильный формат даты. Используйте ГГГГ-ММ-ДД (например, 2023-05-15)");
                    return;
                }

                Console.Write("Дата окончания (ГГГГ-ММ-ДД): ");
                string endInput = Console.ReadLine();

                DateTime endDate;
                if (!DateTime.TryParse(endInput, out endDate))
                {
                    Console.WriteLine("Ошибка: неправильный формат даты. Используйте ГГГГ-ММ-ДД (например, 2023-05-20)");
                    return;
                }

                if (endDate < startDate)
                {
                    Console.WriteLine("Ошибка: дата окончания не может быть раньше даты начала");
                    return;
                }

                Console.Write("Расстояние (км): ");
                string distInput = Console.ReadLine();

                if (!int.TryParse(distInput, out int dist))
                {
                    Console.WriteLine("Ошибка: расстояние должно быть числом");
                    return;
                }

                if (dist <= 0)
                {
                    Console.WriteLine("Ошибка: расстояние должно быть больше 0");
                    return;
                }

                Console.Write("Стоимость: ");
                string costInput = Console.ReadLine();

                if (!int.TryParse(costInput, out int cost))
                {
                    Console.WriteLine("Ошибка: стоимость должна быть числом");
                    return;
                }

                if (cost <= 0)
                {
                    Console.WriteLine("Ошибка: стоимость должна быть больше 0");
                    return;
                }

                dbHelper.AddTrip(carId, driverId, startDate, endDate, dist, cost);
                success = true;
            }

            if (success)
            {

                excelHelper.SaveData(cars, drivers, trips);
            }
        }

        static void ExecuteQueries()
        {
            Console.Clear();
            Console.WriteLine("РЕЗУЛЬТАТЫ ЗАПРОСОВ");
            dbHelper.Query1_ToyotaAfter2010();
            Console.WriteLine("\n" + new string('-', 50));
            dbHelper.Query2_TotalCostForDriver("Peter Parker");
            Console.WriteLine("\n" + new string('-', 50));
            dbHelper.Query3_LongTrips();
            Console.WriteLine("\n" + new string('-', 50));
            dbHelper.Query4_AverageExperienceForLADADrivers();
        }
    }
}