using System;

namespace AvtoparkLab
{
    class Trip
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int DriverId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Distance { get; set; }
        public int Cost { get; set; }

        public Trip(int id, int carId, int driverId, DateTime start, DateTime end, int distance, int cost)
        {
            Id = id;
            CarId = carId;
            DriverId = driverId;
            StartDate = start;
            EndDate = end;
            Distance = distance;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Авто ID: {CarId}, Водитель ID: {DriverId}, " +
                   $"Начало: {StartDate.ToShortDateString()}, Конец: {EndDate.ToShortDateString()}, " +
                   $"Расстояние: {Distance}, Стоимость: {Cost}";
        }
    }
}