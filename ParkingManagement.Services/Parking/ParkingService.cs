using ParkingManagement.Application;
using ParkingManagement.Core.Models;
using System.Net;

namespace ParkingManagement.Services.Parking
{
    public class ParkingService : Interfaces.IParkingService
    {
        private Application.Parking.Interfaces.IParkingRepository _repository;

        public ParkingService(
            Application.Parking.Interfaces.IParkingRepository repository)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public DataStore GetDataStore()
        {
            return this._repository.GetDataStore();
        }

        public (ServiceResponse response, IEnumerable<string>? availablParkingSpaces) GetAvailability(GetAvailabilityRequest request)
        {            
            if (!request.Validate())
            {
                return (new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest, Message = "Invalid inputs." }, null);
            }

            var parkingSpaces = _repository.GetAvailability(request);            
            var spaces = Enumerable.Range(0, (request.ToDate - request.FromDate).Days + 1)
                .Select(d => new ParkingSpaceAvilability
                {
                    Date = DateOnly.FromDateTime(request.FromDate.AddDays(d)),
                    FreeSpace = "all free spaces"
                }).ToList();

            if (parkingSpaces != null && parkingSpaces.Any())
            {
                foreach (var space in spaces)
                {
                    if (parkingSpaces.Any(s => s.Date == space.Date))
                    {
                        space.FreeSpace = parkingSpaces.First(s => s.Date == space.Date).FreeSpace;
                    }
                }
            }

            return (new ServiceResponse { IsSuccess = true, }, spaces.Select(s => $"{s.Date} - {s.FreeSpace}"));
        }

        public (ServiceResponse response, ParkingSpace? parkingSpace) AddReservation(AddParkingRequest request)
        {
            if (request.Validate() == false)
            {
                return (new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest, Message = "Inputs are not valid." }, null);
            }
            var store = this.GetDataStore();
            if (store.ParkingSpaces.Where(s => s.FromDate >= request.FromDate && s.ToDate <= request.ToDate).Count() >= 10)
            {
                return (new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest, Message = "Parking space is not available." }, null);
            }

            if (store.ParkingSpaces.Any(s => s.RegistrationNumber.ToLower() == request.RegistrationNumber.ToLower()))
            {
                return (new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest, Message = "Duplicate." }, null);
            }

            return (new ServiceResponse { IsSuccess = true, }, this._repository.AddParking(request));
        }

        public (ServiceResponse response, ParkingSpace? parkingSpace) ModifyReservation(int id, UpdateParkingRequest request)
        {
            if (request.Validate() == false || id <= 0)
            {
                return (new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest, Message = "Inputs are not valid." }, null);
            }

            var updatedParkingSpace = this._repository.ModifyParkingSpace(id, request);
            if (updatedParkingSpace == null)
            {
                return (new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.NotFound, }, null);
            }

            return (new ServiceResponse { IsSuccess = true, }, updatedParkingSpace);
        }

        public ServiceResponse CancelReservation(int id)
        {
            if (id <= 0)
            {
                return new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest, Message = "Inputs are not valid." };
            }

            if (!this.GetDataStore().ParkingSpaces.Any(s => s.Id == id))
            {
                return new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.NotFound, };
            }

            this._repository.CancelParkingSpace(id);

            return new ServiceResponse { IsSuccess = true, };
        }

        public (ServiceResponse response, IEnumerable<ParkingSpace>? parkingSpaces) GetAllOccuppiedSpaces()
        {
            var occupiedSpaces = this._repository.GetAllOccuppiedSpaces();
            if (!occupiedSpaces.Any())
            {
                return (new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.NotFound, }, null);
            }

            return (new ServiceResponse { IsSuccess = true, }, occupiedSpaces);
        }
    }
}
