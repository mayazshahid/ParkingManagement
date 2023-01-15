using Microsoft.AspNetCore.Mvc;

namespace ParkingManagement.Core.Models
{
    public class GetAvailabilityRequest
    {
        [FromQuery]
        public DateTime FromDate { get; set; }

        [FromQuery]
        public DateTime ToDate { get; set; }

        public bool Validate()
        {
            if (this.FromDate.Date < DateTime.Now.Date || this.ToDate < this.FromDate)
            {
                return false;
            }

            return true;
        }
    }
}
