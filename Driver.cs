using System;

namespace AvtoparkLab
{
    class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Experience { get; set; }

        public Driver(int id, string name, int age, int experience)
        {
            Id = id;
            Name = name;
            Age = age;
            Experience = experience;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Имя: {Name}, Возраст: {Age}, Стаж: {Experience}";
        }
    }
}