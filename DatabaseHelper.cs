using System;
using System.Collections.Generic;

namespace AvtoparkLab
{
    class DatabaseHelper
    {
        private List<Car> cars;
        private List<Driver> drivers;
        private List<Trip> trips;

        public DatabaseHelper(List<Car> c, List<Driver> d, List<Trip> t)
        {
            cars = c;
            drivers = d;
            trips = t;
        }

        // Просмотр
        public void ViewAllCars()
        {
            Console.WriteLine("\n=== АВТОМОБИЛИ ===");
            if (cars.Count == 0) Console.WriteLine("Список пуст.");
            else foreach (Car car in cars) Console.WriteLine(car.ToString());
            Console.WriteLine("Всего: " + cars.Count);
        }

        public void ViewAllDrivers()
        {
            Console.WriteLine("\n=== ВОДИТЕЛИ ===");
            if (drivers.Count == 0) Console.WriteLine("Список пуст.");
            else foreach (Driver driver in drivers) Console.WriteLine(driver.ToString());
            Console.WriteLine("Всего: " + drivers.Count);
        }

        public void ViewAllTrips()
        {
            Console.WriteLine("\n=== РЕЙСЫ ===");
            if (trips.Count == 0) Console.WriteLine("Список пуст.");
            else foreach (Trip trip in trips) Console.WriteLine(trip.ToString());
            Console.WriteLine("Всего: " + trips.Count);
        }

        // Удаление
        public void DeleteCarById(int id)
        {
            for (int i = 0; i < cars.Count; i++)
            {
                if (cars[i].Id == id)
                {
                    cars.RemoveAt(i);
                    Console.WriteLine("Автомобиль удален.");
                    return;
                }
            }
            Console.WriteLine("Автомобиль не найден.");
        }

        public void DeleteDriverById(int id)
        {
            for (int i = 0; i < drivers.Count; i++)
            {
                if (drivers[i].Id == id)
                {
                    drivers.RemoveAt(i);
                    Console.WriteLine("Водитель удален.");
                    return;
                }
            }
            Console.WriteLine("Водитель не найден.");
        }

        public void DeleteTripById(int id)
        {
            for (int i = 0; i < trips.Count; i++)
            {
                if (trips[i].Id == id)
                {
                    trips.RemoveAt(i);
                    Console.WriteLine("Рейс удален.");
                    return;
                }
            }
            Console.WriteLine("Рейс не найден.");
        }

        // Добавление с АВТОМАТИЧЕСКИМ ID
        public void AddCar(string brand, string model, int year)
        {
            int newId = 1;
            foreach (Car car in cars)
            {
                if (car.Id >= newId) newId = car.Id + 1;
            }

            Car newCar = new Car(newId, brand, model, year);
            cars.Add(newCar);
            Console.WriteLine("Автомобиль добавлен с ID: " + newId);
        }

        public void AddDriver(string name, int age, int experience)
        {
            int newId = 1;
            foreach (Driver driver in drivers)
            {
                if (driver.Id >= newId) newId = driver.Id + 1;
            }

            Driver newDriver = new Driver(newId, name, age, experience);
            drivers.Add(newDriver);
            Console.WriteLine("Водитель добавлен с ID: " + newId);
        }

        public void AddTrip(int carId, int driverId, DateTime start, DateTime end, int distance, int cost)
        {
            // Проверяем существование авто
            bool carExists = false;
            foreach (Car car in cars)
                if (car.Id == carId) carExists = true;

            if (!carExists)
            {
                Console.WriteLine("Ошибка: Автомобиль с ID " + carId + " не найден.");
                return;
            }

            // Проверяем существование водителя
            bool driverExists = false;
            foreach (Driver driver in drivers)
                if (driver.Id == driverId) driverExists = true;

            if (!driverExists)
            {
                Console.WriteLine("Ошибка: Водитель с ID " + driverId + " не найден.");
                return;
            }

            int newId = 1;
            foreach (Trip trip in trips)
            {
                if (trip.Id >= newId) newId = trip.Id + 1;
            }

            Trip newTrip = new Trip(newId, carId, driverId, start, end, distance, cost);
            trips.Add(newTrip);
            Console.WriteLine("Рейс добавлен с ID: " + newId);
        }

        // Запросы
        public void Query1_ToyotaAfter2010()
        {
            Console.WriteLine("\n=== Toyota после 2020 ===");
            int count = 0;
            foreach (Car car in cars)
            {
                if (car.Brand == "Toyota" && car.Year >= 2020)
                {
                    Console.WriteLine(car.ToString());
                    count++;
                }
            }
            Console.WriteLine("Найдено: " + count);
        }

        public void Query2_TotalCostForDriver(string driverName)
        {
            Console.WriteLine("\n=== Стоимость рейсов для " + driverName + " ===");

            Driver foundDriver = null;
            foreach (Driver driver in drivers)
            {
                if (driver.Name.ToLower().Contains(driverName.ToLower()))
                {
                    foundDriver = driver;
                    break;
                }
            }

            if (foundDriver == null)
            {
                Console.WriteLine("Водитель не найден.");
                return;
            }

            int totalCost = 0;
            int tripCount = 0;

            foreach (Trip trip in trips)
            {
                if (trip.DriverId == foundDriver.Id)
                {
                    totalCost += trip.Cost;
                    tripCount++;
                }
            }

            Console.WriteLine("Водитель: " + foundDriver.Name);
            Console.WriteLine("Рейсов: " + tripCount);
            Console.WriteLine("Стоимость: " + totalCost);
        }

        public void Query3_LongTrips()
        {
            Console.WriteLine("\n=== Рейсы > 2950 км ===");
            int count = 0;

            foreach (Trip trip in trips)
            {
                if (trip.Distance > 2950)
                {
                    Car car = null;
                    foreach (Car c in cars)
                        if (c.Id == trip.CarId) car = c;

                    Driver driver = null;
                    foreach (Driver d in drivers)
                        if (d.Id == trip.DriverId) driver = d;

                    Console.WriteLine("Рейс " + trip.Id + ": " + trip.StartDate.ToShortDateString() +
                                      ", " + car.Brand + " " + car.Model + ", " + driver.Name +
                                      ", " + trip.Distance + " км");
                    count++;
                }
            }
            Console.WriteLine("Всего: " + count);
        }

        public void Query4_AverageExperienceForLADADrivers()
        {
            Console.WriteLine("\nСредний стаж водителей LADA");

            List<int> driverIds = new List<int>();

            foreach (Car car in cars)
            {
                if (car.Brand == "LADA")
                {
                    foreach (Trip trip in trips)
                    {
                        if (trip.CarId == car.Id)
                        {
                            bool exists = false;
                            foreach (int id in driverIds)
                                if (id == trip.DriverId) exists = true;
                            if (!exists) driverIds.Add(trip.DriverId);
                        }
                    }
                }
            }

            if (driverIds.Count == 0)
            {
                Console.WriteLine("Водители не найдены.");
                return;
            }

            int totalExp = 0;
            foreach (int driverId in driverIds)
            {
                foreach (Driver driver in drivers)
                {
                    if (driver.Id == driverId)
                    {
                        totalExp += driver.Experience;
                        break;
                    }
                }
            }

            int average = totalExp / driverIds.Count;
            Console.WriteLine("Водителей: " + driverIds.Count);
            Console.WriteLine("Средний стаж: " + average + " лет");
        }
    }
}