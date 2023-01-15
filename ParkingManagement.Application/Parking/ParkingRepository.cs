using ParkingManagement.Core.Models;

namespace ParkingManagement.Application.Parking
{
    public class ParkingRepository : Interfaces.IParkingRepository
    {
        DataStore DataSore;
        public ParkingRepository(DataStore dataStore)
        {
            this.DataSore = dataStore;
        }
        
        public IEnumerable<ParkingSpaceAvilability>? GetAvailability(GetAvailabilityRequest request)
        {
            if (!request.Validate())
            {
                return null;
            }

            return this.DataSore.ParkingSpaces
                .Where(s => s.FromDate >= request.FromDate && s.ToDate <= request.ToDate)
                .GroupBy(group => DateOnly.FromDateTime(group.FromDate))
                .Select(group => new ParkingSpaceAvilability
                {
                    Date = group.Key,
                    FreeSpace = $"{10 - group.Count()} free spaces"
                }).ToList();
        }

        public IEnumerable<ParkingSpace> GetAllOccuppiedSpaces()
        {
            return this.DataSore.ParkingSpaces;
        }

        public ParkingSpace AddParking(AddParkingRequest request)
        {
            int id = this.DataSore.ParkingSpaces.Count() > 0 ? this.DataSore.ParkingSpaces.Max(s => s.Id) + 1 : 1;
            var parkingSpace = new ParkingSpace
            {
                Id = id,
                RegistrationNumber = request.RegistrationNumber.ToUpper(),
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Price = this.DataSore.GetPrice(request.FromDate, request.ToDate),
            };

            this.DataSore.ParkingSpaces.Add(parkingSpace);

            return parkingSpace;
        }

        public ParkingSpace? ModifyParkingSpace(int id, UpdateParkingRequest request)
        {
            var parkingSpace = this.DataSore.ParkingSpaces.FirstOrDefault(s => s.Id == id);
            if (parkingSpace != null)
            {
                parkingSpace.FromDate = request.FromDate;
                parkingSpace.ToDate = request.ToDate;
            }

            return parkingSpace;
        }

        public void CancelParkingSpace(int id)
        {
            this.DataSore.ParkingSpaces.Remove(this.DataSore.ParkingSpaces.First(s => s.Id == id));
        }

        public DataStore GetDataStore()
        {
            return this.DataSore;
        }
    }
}
