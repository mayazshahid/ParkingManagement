using System.ComponentModel.DataAnnotations;

namespace ParkingManagement.Core.Models
{
    public class UpdateParkingRequest
    {        
        public DateTime FromDate { get; set; }
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
