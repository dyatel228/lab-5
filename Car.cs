using System;

namespace AvtoparkLab
{
    class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }

        public Car(int id, string brand, string model, int year)
        {
            Id = id;
            Brand = brand;
            Model = model;
            Year = year;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Марка: {Brand}, Модель: {Model}, Год: {Year}";
        }
    }
}