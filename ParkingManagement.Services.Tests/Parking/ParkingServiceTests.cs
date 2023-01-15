using Microsoft.VisualBasic;
using Moq;
using ParkingManagement.Application;
using ParkingManagement.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ParkingManagement.Services.Tests.Parking
{
    [TestClass]
    public class ParkingServiceTests
    {
        private readonly Mock<Application.Parking.Interfaces.IParkingRepository> _mockRepo;

        public ParkingServiceTests()
        {
            this._mockRepo = new Mock<Application.Parking.Interfaces.IParkingRepository>();
        }

        [TestMethod]
        public void GetAvailability_OnInvalidInputs_ReturnsResponseWithFalse()
        {
            // Arrange
            var request = new GetAvailabilityRequest();
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.GetAvailability(request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsNull(actual.availablParkingSpaces);
            Assert.IsFalse(actual.response.IsSuccess);
        }

        [TestMethod]
        public void GetAvailability_OnValidInputs_ReturnsParkingSpaces()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now.AddDays(1).Date;
            var request = new GetAvailabilityRequest { FromDate= fromDate, ToDate = toDate };
            this._mockRepo.Setup(s => s.GetAvailability(request)).Returns(new List<ParkingSpaceAvilability> { new ParkingSpaceAvilability { Date = DateOnly.FromDateTime(fromDate), FreeSpace = "9 free spaces" } });
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.GetAvailability(request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsTrue(actual.response.IsSuccess);
            Assert.IsNotNull(actual.availablParkingSpaces);
            Assert.IsTrue(actual.availablParkingSpaces.Count() == 2);            
        }

        [TestMethod]
        public void AddReservation_OnInvalidInputs_ReturnsResponseFalse()
        {
            // Arrange
            var request = new AddParkingRequest();
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.AddReservation(request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsNull(actual.parkingSpace);
            Assert.IsFalse(actual.response.IsSuccess);
        }

        [TestMethod]
        public void AddReservation_OnParkingSpaceNotAvailable_ReturnsResponseFalse()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now.AddDays(1).Date;
            var request = new AddParkingRequest { RegistrationNumber = "AA", FromDate = fromDate, ToDate = toDate };
            var store = new DataStore();
            for(int i = 0; i < 11; i++)
            {
                store.ParkingSpaces.Add(new ParkingSpace { RegistrationNumber = $"{request.RegistrationNumber}{i}", FromDate = fromDate, ToDate = toDate });
            }
            this._mockRepo.Setup(s => s.GetDataStore()).Returns(store);
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.AddReservation(request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsNull(actual.parkingSpace);
            Assert.IsFalse(actual.response.IsSuccess);
        }

        [TestMethod]
        public void AddReservation_OnDuplicate_ReturnsResponseFalse()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now.AddDays(1).Date;
            var request = new AddParkingRequest { RegistrationNumber = "AA", FromDate = fromDate, ToDate = toDate };
            var store = new DataStore();
            store.ParkingSpaces.Add(new ParkingSpace { RegistrationNumber = $"{request.RegistrationNumber}", FromDate = fromDate, ToDate = toDate });            this._mockRepo.Setup(s => s.GetDataStore()).Returns(store);
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.AddReservation(request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsNull(actual.parkingSpace);
            Assert.IsFalse(actual.response.IsSuccess);
        }

        [TestMethod]
        public void AddReservation_OnValidInput_ReturnsResponseTrueWithAddedParkingSpace()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now.AddDays(1).Date;
            var request = new AddParkingRequest { RegistrationNumber = "AA", FromDate = fromDate, ToDate = toDate };
            this._mockRepo.Setup(s => s.GetDataStore()).Returns(new DataStore());
            this._mockRepo.Setup(s => s.AddParking(request)).Returns(new ParkingSpace { RegistrationNumber = request.RegistrationNumber, FromDate = request.FromDate, ToDate = request.ToDate });
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.AddReservation(request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsTrue(actual.response.IsSuccess);
            Assert.IsNotNull(actual.parkingSpace);
        }

        [TestMethod]
        public void ModifyReservation_OnInvalidInputs_ReturnsResponseWithFalse()
        {
            // Arrange
            var request = new UpdateParkingRequest();
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.ModifyReservation(1, request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsNull(actual.parkingSpace);
            Assert.IsFalse(actual.response.IsSuccess);
        }

        [TestMethod]
        public void ModifyReservation_OnInvalidId_ReturnsResponseWithFalse()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now.AddDays(1).Date;
            var request = new UpdateParkingRequest { FromDate = fromDate, ToDate = toDate };
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.ModifyReservation(0, request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsNull(actual.parkingSpace);
            Assert.IsFalse(actual.response.IsSuccess);
        }

        [TestMethod]
        public void ModifyReservation_OnInvalidParkingSpace_ReturnsResponseWithFalse()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now.AddDays(1).Date;
            var request = new UpdateParkingRequest { FromDate = fromDate, ToDate = toDate };
            this._mockRepo.Setup(s => s.ModifyParkingSpace(1, request)).Returns((ParkingSpace)null);
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.ModifyReservation(1, request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsNull(actual.parkingSpace);
            Assert.IsFalse(actual.response.IsSuccess);
        }

        [TestMethod]
        public void ModifyReservation_OnValidParkingSpace_ReturnsResponseWithTrue()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now.AddDays(1).Date;
            var request = new UpdateParkingRequest { FromDate = fromDate, ToDate = toDate };
            this._mockRepo.Setup(s => s.ModifyParkingSpace(1, request)).Returns(new ParkingSpace { Id = 1, FromDate = request.FromDate, ToDate = request.ToDate});
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.ModifyReservation(1, request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.response);
            Assert.IsTrue(actual.response.IsSuccess);
            Assert.IsNotNull(actual.parkingSpace);            
        }

        [TestMethod]
        public void CancelReservation_OnInvalidReservationId_ReturnsResponseWithFalse()
        {
            // Arrange
            var id = 0;
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.CancelReservation(id);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccess);
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.HttpStatusCode);
        }

        [TestMethod]
        public void CancelReservation_OnParkingNotExists_ReturnsResponseWithFalse()
        {
            // Arrange
            var id = 1;
            this._mockRepo.Setup(s => s.GetDataStore()).Returns(new DataStore());
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.CancelReservation(id);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccess);
            Assert.AreEqual(HttpStatusCode.NotFound, actual.HttpStatusCode);
        }

        [TestMethod]
        public void CancelReservation_OnParkingExists_ReturnsResponseWithTrue()
        {
            // Arrange
            var id = 1;
            var store = new DataStore { ParkingSpaces = new List<ParkingSpace> { new ParkingSpace { Id = id, FromDate = DateTime.Now.Date, ToDate = DateTime.Now.AddDays(1).Date, RegistrationNumber = "AA" } } };
            this._mockRepo.Setup(s => s.GetDataStore()).Returns(store);
            this._mockRepo.Setup(s => s.CancelParkingSpace(id));
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.CancelReservation(id);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccess);
        }

        [TestMethod]
        public void GetAllOccuppiedSpaces_OnRepository_ReturnsResponeWithFalse()
        {
            // Arrange
            this._mockRepo.Setup(s => s.GetAllOccuppiedSpaces()).Returns(new List<ParkingSpace>());
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.GetAllOccuppiedSpaces();

            // Act
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.response.IsSuccess);
            Assert.IsNull(actual.parkingSpaces);
        }

        [TestMethod]
        public void GetAllOccuppiedSpaces_OnRepository_ReturnsResponeWithTrue()
        {
            // Arrange
            this._mockRepo.Setup(s => s.GetAllOccuppiedSpaces()).Returns(new List<ParkingSpace> { new ParkingSpace { Id = 1, } });
            var sut = new Services.Parking.ParkingService(this._mockRepo.Object);

            // Act
            var actual = sut.GetAllOccuppiedSpaces();

            // Act
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.response.IsSuccess);
            Assert.IsNotNull(actual.parkingSpaces);
        }
    }
}