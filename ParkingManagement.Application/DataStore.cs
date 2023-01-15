using ParkingManagement.Core.Models;

namespace ParkingManagement.Application
{
    public class DataStore
    {
        public DataStore()
        {
            this.ParkingPrices = new List<ParkingPrice>
            {
                new ParkingPrice { SeasonName = "Summer", Price = 15, },
                new ParkingPrice { SeasonName = "Winter", Price = 10, },
            };
        }

        public List<ParkingPrice> ParkingPrices { get; set; }

        public List<ParkingSpace> ParkingSpaces { get; set; } = new List<ParkingSpace>();

        public double GetPrice(DateTime fromDate, DateTime toDate)
        {            
            if (fromDate.IsDaylightSavingTime() &&
                toDate.IsDaylightSavingTime() &&
                ParkingPrices.Any(s => s.SeasonName.ToLower() == "summer"))
            {
                return ParkingPrices.First(s => s.SeasonName.ToLower() == "summer").Price;
            }

            return ParkingPrices.Any(s => s.SeasonName.ToLower() == "winter")
                ? ParkingPrices.First(s => s.SeasonName.ToLower() == "winter").Price
                : 0;
        }
    }
}