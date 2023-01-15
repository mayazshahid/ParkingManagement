using ParkingManagement.Application;
using ParkingManagement.Core.Models;

namespace ParkingManagement.Services.Parking.Interfaces
{
    public interface IParkingService
    {
        DataStore GetDataStore();
        (ServiceResponse response, IEnumerable<ParkingSpace>? parkingSpaces) GetAllOccuppiedSpaces();
        (ServiceResponse response, IEnumerable<string>? availablParkingSpaces) GetAvailability(GetAvailabilityRequest request);
        (ServiceResponse response, ParkingSpace? parkingSpace) AddReservation(AddParkingRequest request);
        (ServiceResponse response, ParkingSpace? parkingSpace) ModifyReservation(int id, UpdateParkingRequest request);
        ServiceResponse CancelReservation(int id);
    }
}
