using ParkingManagement.Core.Models;

namespace ParkingManagement.Application.Parking.Interfaces
{
    public interface IParkingRepository
    {
        DataStore GetDataStore();
        IEnumerable<ParkingSpace> GetAllOccuppiedSpaces();
        IEnumerable<ParkingSpaceAvilability>? GetAvailability(GetAvailabilityRequest request);
        ParkingSpace AddParking(AddParkingRequest request);
        ParkingSpace? ModifyParkingSpace(int id, UpdateParkingRequest request);
        void CancelParkingSpace(int id);
    }
}
