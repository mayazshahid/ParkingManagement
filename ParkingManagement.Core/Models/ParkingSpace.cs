namespace ParkingManagement.Core.Models
{
    public class ParkingSpace
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double Price { get; set; }
    }
}
