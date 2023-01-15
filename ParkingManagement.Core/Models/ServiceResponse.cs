using System.Net;

namespace ParkingManagement.Core.Models
{
    public class ServiceResponse
    {
        public bool IsSuccess { get; set; }        
        public HttpStatusCode HttpStatusCode { get; set; }
        public string? Message { get; set; }
    }
}
