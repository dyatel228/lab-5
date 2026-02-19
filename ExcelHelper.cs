using ExcelDataReader;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace AvtoparkLab
{
    class ExcelHelper
    {
        private string filePath;

        public ExcelHelper(string path)
        {
            filePath = path;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // Чтение автомобилей
        public List<Car> ReadCars()
        {
            List<Car> cars = new List<Car>();

            try
            {
                if (!File.Exists(filePath))
                {
                    return cars;
                }

                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            if (reader.Name == "Автомобили")
                            {
                                reader.Read();

                                while (reader.Read())
                                {
                                    int id = Convert.ToInt32(reader.GetValue(0));
                                    string brand = reader.GetValue(1).ToString();
                                    string model = reader.GetValue(2).ToString();
                                    int year = Convert.ToInt32(reader.GetValue(3));

                                    Car car = new Car(id, brand, model, year);
                                    cars.Add(car);
                                }
                                break;
                            }
                        } while (reader.NextResult());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка чтения: " + ex.Message);
            }

            return cars;
        }

        // Чтение водителей
        public List<Driver> ReadDrivers()
        {
            List<Driver> drivers = new List<Driver>();

            try
            {
                if (!File.Exists(filePath))
                {
                    return drivers;
                }

                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            if (reader.Name == "Водители")
                            {
                                reader.Read();

                                while (reader.Read())
                                {
                                    int id = Convert.ToInt32(reader.GetValue(0));
                                    string name = reader.GetValue(1).ToString();
                                    int age = Convert.ToInt32(reader.GetValue(2));
                                    int experience = Convert.ToInt32(reader.GetValue(3));

                                    Driver driver = new Driver(id, name, age, experience);
                                    drivers.Add(driver);
                                }
                                break;
                            }
                        } while (reader.NextResult());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка чтения: " + ex.Message);
            }

            return drivers;
        }

        // Чтение рейсов

        public List<Trip> ReadTrips()
        {
            List<Trip> trips = new List<Trip>();

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Файл не найден: " + filePath);
                    return trips;
                }

                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            if (reader.Name == "Рейсы")
                            {
                                reader.Read(); // пропускаем заголовок
                                int rowCount = 0;

                                while (reader.Read())
                                {
                                    try
                                    {
                                        // Проверяем что ячейки не пустые
                                        if (reader.GetValue(0) == null) break;

                                        int id = Convert.ToInt32(reader.GetValue(0));
                                        int carId = Convert.ToInt32(reader.GetValue(1));
                                        int driverId = Convert.ToInt32(reader.GetValue(2));

                                        // КОНВЕРТАЦИЯ ДАТ ИЗ ЧИСЕЛ EXCEL
                                        DateTime startDate;
                                        DateTime endDate;

                                        object startVal = reader.GetValue(3);
                                        object endVal = reader.GetValue(4);

                                        // Пробуем конвертировать разными способами
                                        if (startVal is DateTime)
                                        {
                                            startDate = (DateTime)startVal;
                                        }
                                        else if (startVal is double)
                                        {
                                            // Excel дата как число
                                            double excelDate = (double)startVal;
                                            startDate = DateTime.FromOADate(excelDate);
                                        }
                                        else
                                        {
                                            // Пробуем распарсить строку
                                            string dateStr = startVal.ToString();
                                            if (!DateTime.TryParse(dateStr, out startDate))
                                            {
                                                // Если не получилось, пробуем как число Excel
                                                double excelDate = Convert.ToDouble(dateStr);
                                                startDate = DateTime.FromOADate(excelDate);
                                            }
                                        }

                                        if (endVal is DateTime)
                                        {
                                            endDate = (DateTime)endVal;
                                        }
                                        else if (endVal is double)
                                        {
                                            double excelDate = (double)endVal;
                                            endDate = DateTime.FromOADate(excelDate);
                                        }
                                        else
                                        {
                                            string dateStr = endVal.ToString();
                                            if (!DateTime.TryParse(dateStr, out endDate))
                                            {
                                                double excelDate = Convert.ToDouble(dateStr);
                                                endDate = DateTime.FromOADate(excelDate);
                                            }
                                        }

                                        int distance = Convert.ToInt32(reader.GetValue(5));
                                        int cost = Convert.ToInt32(reader.GetValue(6));

                                        Trip trip = new Trip(id, carId, driverId, startDate, endDate, distance, cost);
                                        trips.Add(trip);
                                        rowCount++;
                                    }
                                    catch (Exception ex)
                                    {
                                        // Пропускаем ошибки, просто логируем
                                        Console.WriteLine("Ошибка в строке " + (rowCount + 2) + ": " + ex.Message);
                                    }
                                }

                                Console.WriteLine("Загружено рейсов: " + rowCount);
                                break;
                            }
                        } while (reader.NextResult());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при чтении рейсов: " + ex.Message);
            }

            return trips;
        }

        // СОХРАНЕНИЕ
        public void SaveData(List<Car> cars, List<Driver> drivers, List<Trip> trips)
        {
            string tempFile = "";

            try
            {
                // Закрываем все потоки перед сохранением
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Создаем временный файл
                tempFile = Path.GetTempFileName();

                using (ExcelPackage package = new ExcelPackage())
                {
                    // Лист Автомобили
                    ExcelWorksheet wsCars = package.Workbook.Worksheets.Add("Автомобили");
                    wsCars.Cells[1, 1].Value = "ID автомобиля";
                    wsCars.Cells[1, 2].Value = "Марка";
                    wsCars.Cells[1, 3].Value = "Модель";
                    wsCars.Cells[1, 4].Value = "Год выпуска";

                    for (int i = 0; i < cars.Count; i++)
                    {
                        wsCars.Cells[i + 2, 1].Value = cars[i].Id;
                        wsCars.Cells[i + 2, 2].Value = cars[i].Brand;
                        wsCars.Cells[i + 2, 3].Value = cars[i].Model;
                        wsCars.Cells[i + 2, 4].Value = cars[i].Year;
                    }

                    // Лист Водители
                    ExcelWorksheet wsDrivers = package.Workbook.Worksheets.Add("Водители");
                    wsDrivers.Cells[1, 1].Value = "ID водителя";
                    wsDrivers.Cells[1, 2].Value = "Имя";
                    wsDrivers.Cells[1, 3].Value = "Возраст";
                    wsDrivers.Cells[1, 4].Value = "Стаж вождения";

                    for (int i = 0; i < drivers.Count; i++)
                    {
                        wsDrivers.Cells[i + 2, 1].Value = drivers[i].Id;
                        wsDrivers.Cells[i + 2, 2].Value = drivers[i].Name;
                        wsDrivers.Cells[i + 2, 3].Value = drivers[i].Age;
                        wsDrivers.Cells[i + 2, 4].Value = drivers[i].Experience;
                    }

                    // Лист Рейсы
                    ExcelWorksheet wsTrips = package.Workbook.Worksheets.Add("Рейсы");
                    wsTrips.Cells[1, 1].Value = "ID рейса";
                    wsTrips.Cells[1, 2].Value = "ID автомобиля";
                    wsTrips.Cells[1, 3].Value = "ID водителя";
                    wsTrips.Cells[1, 4].Value = "Дата начала рейса";
                    wsTrips.Cells[1, 5].Value = "Дата окончания рейса";
                    wsTrips.Cells[1, 6].Value = "Расстояние";
                    wsTrips.Cells[1, 7].Value = "Стоимость";

                    for (int i = 0; i < trips.Count; i++)
                    {
                        wsTrips.Cells[i + 2, 1].Value = trips[i].Id;
                        wsTrips.Cells[i + 2, 2].Value = trips[i].CarId;
                        wsTrips.Cells[i + 2, 3].Value = trips[i].DriverId;
                        wsTrips.Cells[i + 2, 4].Value = trips[i].StartDate;
                        wsTrips.Cells[i + 2, 5].Value = trips[i].EndDate;
                        wsTrips.Cells[i + 2, 6].Value = trips[i].Distance;
                        wsTrips.Cells[i + 2, 7].Value = trips[i].Cost;
                    }

                    // Сохраняем во временный файл
                    FileInfo tempFileInfo = new FileInfo(tempFile);
                    package.SaveAs(tempFileInfo);
                }

                // Если все хорошо, заменяем оригинальный файл временным
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(tempFile, filePath);
                Console.WriteLine("Автомобилей: " + cars.Count + ", Водителей: " + drivers.Count + ", Рейсов: " + trips.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка сохранения: " + ex.Message);
                // Если ошибка, удаляем временный файл
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}