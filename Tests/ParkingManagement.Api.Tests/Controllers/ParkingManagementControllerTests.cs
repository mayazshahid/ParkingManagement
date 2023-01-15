using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingManagement.Api.Controllers;
using ParkingManagement.Application;
using ParkingManagement.Core.Models;
using System.Net;

namespace ParkingManagement.Api.Tests.Controllers
{
    [TestClass]
    public class ParkingManagementControllerTests
    {
        private readonly Mock<Services.Parking.Interfaces.IParkingService> _mockParkingService;
        private readonly DataStore _dataStore;
        public ParkingManagementControllerTests()
        {
            this._mockParkingService = new Mock<Services.Parking.Interfaces.IParkingService>();
            this._dataStore = new DataStore
            {
                ParkingPrices = new List<ParkingPrice>
                {
                    new ParkingPrice { SeasonName = "Summer", Price = 15, },
                    new ParkingPrice { SeasonName = "Winter", Price = 10, },
                },
            };
        }

        [TestMethod]
        public void Constractor_ServiceIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(new Action(() => new ParkingManagementController(null)));
        }

        [TestMethod]
        public void GetAllOccuppiedSpaces_OnZeroRecords_ReturnsNotFoundResult()
        {
            // Arrange
            this._mockParkingService.Setup(s => s.GetAllOccuppiedSpaces()).Returns((new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.NotFound }, null));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.GetAllOccuppiedSpaces();

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void GetAllOccuppiedSpaces_OnRecords_ReturnsOk()
        {
            // Arrange
            var parkingSpaces = new List<ParkingSpace> { new ParkingSpace { Id = 1, RegistrationNumber = "AB10", FromDate = DateTime.Now.Date, ToDate = DateTime.Now.AddDays(2).Date } };
            this._mockParkingService.Setup(s => s.GetAllOccuppiedSpaces()).Returns((new ServiceResponse { IsSuccess = true,}, parkingSpaces));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.GetAllOccuppiedSpaces();

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetAvailability_OnInvalidInputs_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new GetAvailabilityRequest();
            this._mockParkingService.Setup(s => s.GetAvailability(request)).Returns((new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest }, null));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.GetAvailability(request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void GetAvailability_OnParkingSpacesAvailable_ReturnsOk()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = fromDate.AddDays(2).Date;
            var request = new GetAvailabilityRequest { FromDate = fromDate, ToDate = toDate};
            this._mockParkingService.Setup(s => s.GetAvailability(request)).Returns((new ServiceResponse { IsSuccess = true, }, new List<string> { $"{fromDate} - all free spaces" }));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.GetAvailability(request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetAvailability_OnParkingSpacesNotAvailable_ReturnsNotFoundResult()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = fromDate.AddDays(2).Date;
            var request = new GetAvailabilityRequest { FromDate = fromDate, ToDate = toDate };
            this._mockParkingService.Setup(s => s.GetAvailability(request)).Returns((new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.NotFound }, null));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.GetAvailability(request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(NotFoundObjectResult));
        }
        
        [TestMethod]
        public void AddReservation_OnNoRegNumber_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new AddParkingRequest { FromDate = DateTime.Now.Date, ToDate = DateTime.Now.Date };            
            var sut = this.CreateInstance();
            sut.ModelState.AddModelError("RegistrationNumber", $"The 'RegistrationNumber' is required.");

            // Act
            var actual = sut.AddReservation(request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void AddReservation_OnNoDates_ReturnsBadRequest()
        {
            // Arrange
            var request = new AddParkingRequest { RegistrationNumber = "AAB" };
            this._mockParkingService.Setup(s => s.AddReservation(request)).Returns((new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest }, null));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.AddReservation(request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void AddReservation_OnParkingSpaceNotAvailable_ReturnsBadRequest()
        {
            // Arrange
            var fDate = DateTime.Now.Date.AddDays(1).Date;
            var tDate = fDate.AddDays(2).Date;
            var request = new AddParkingRequest { RegistrationNumber = "abc10", FromDate = fDate, ToDate = tDate };
            var store = this._dataStore;
            for (int i = 0; i < 11; i++)
            {                
                store.ParkingSpaces.Add(new ParkingSpace
                {
                    RegistrationNumber = $"ABC{i}",
                    FromDate= fDate, ToDate = tDate,
                    Price = store.GetPrice(fDate, tDate)
                });
            }
            this._mockParkingService.Setup(s => s.GetDataStore()).Returns(store);
            this._mockParkingService.Setup(s => s.AddReservation(request)).Returns((new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest }, null));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.AddReservation(request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(BadRequestObjectResult));
        }


        [TestMethod]
        public void AddReservation_OnDuplicateRegNumber_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new AddParkingRequest { RegistrationNumber = "abc10", FromDate = DateTime.Now.Date, ToDate = DateTime.Now.AddDays(1).Date };
            this._mockParkingService.Setup(s => s.AddReservation(request)).Returns((new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest }, null));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.AddReservation(request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void AddReservation_OnVaild_ReturnsCreatedResult()
        {
            // Arrange
            var fDate = DateTime.Now.Date.AddDays(2).Date;
            var tDate = fDate.AddDays(5).Date;
            var request = new AddParkingRequest { RegistrationNumber = "abc10", FromDate = fDate, ToDate = tDate };
            var store = this._dataStore;
            var expectedObject = new ParkingSpace { RegistrationNumber = "ABC10", FromDate = fDate, ToDate = tDate, Price = store.GetPrice(fDate, tDate) };
            this._mockParkingService.Setup(s => s.AddReservation(request)).Returns((new ServiceResponse { IsSuccess = true }, expectedObject)); 
            var sut = this.CreateInstance();

            // Act
            var actual = sut.AddReservation(request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual.Result, typeof(CreatedResult));
        }
        
        [TestMethod]
        public void ModifyReservation_OnInValidateFromDate_ReturnsBadRequestResult()
        {
            // Arrange
            var fDate = DateTime.Now.Date.AddDays(-1).Date;
            var tDate = fDate.AddDays(5).Date;
            var request = new UpdateParkingRequest { FromDate = fDate, ToDate = tDate };
            this._mockParkingService.Setup(s => s.ModifyReservation(1, request)).Returns((new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest }, null));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.ModifyReservation(1, request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(BadRequestObjectResult));
        }        

        [TestMethod]
        public void ModifyReservation_OnInValidReservation_ReturnsNotFoundResult()
        {
            // Arrange
            var fDate = DateTime.Now.Date.AddDays(1).Date;
            var tDate = fDate.AddDays(5).Date;
            var request = new UpdateParkingRequest { FromDate = fDate, ToDate = tDate };
            this._mockParkingService.Setup(s => s.ModifyReservation(6, request)).Returns((new ServiceResponse { IsSuccess = false, HttpStatusCode =  HttpStatusCode.NotFound}, null));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.ModifyReservation(6, request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void ModifyReservation_OnValidReservation_ReturnsNoContent()
        {
            // Arrange
            var fDate = DateTime.Now.Date.AddDays(1).Date;
            var tDate = fDate.AddDays(5).Date;
            var request = new UpdateParkingRequest { FromDate = fDate, ToDate = tDate };
            this._mockParkingService.Setup(s => s.ModifyReservation(6, request)).Returns((new ServiceResponse { IsSuccess = true}, new ParkingSpace { Id = 6, FromDate = fDate, ToDate = tDate }));
            var sut = this.CreateInstance();

            // Act
            var actual = sut.ModifyReservation(6, request);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(NoContentResult));
        }       
        

        [TestMethod]
        public void CancelReservation_OnInValidateReservation_ReturnsNotFoundResult()
        {
            // Arrange
            this._mockParkingService.Setup(s => s.GetDataStore()).Returns(this._dataStore);
            this._mockParkingService.Setup(s => s.CancelReservation(1)).Returns(new ServiceResponse { IsSuccess = false, HttpStatusCode = HttpStatusCode.NotFound });
            var sut = this.CreateInstance();

            // Act
            var actual = sut.CancelReservation(1);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void CancelReservation_OnValidateReservation_ReturnsNoContentResult()
        {
            // Arrange
            var store = this._dataStore;
            store.ParkingSpaces.Add(new ParkingSpace { Id = 1, RegistrationNumber = "AA", FromDate = DateTime.Now, ToDate = DateTime.Now.AddDays(5) });
            this._mockParkingService.Setup(s => s.GetDataStore()).Returns(this._dataStore);
            this._mockParkingService.Setup(s => s.CancelReservation(1)).Returns(new ServiceResponse { IsSuccess = true});
            var sut = this.CreateInstance();

            // Act
            var actual = sut.CancelReservation(1);

            // Arrange
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(NoContentResult));
        }

        private ParkingManagementController CreateInstance()
        {
            return new ParkingManagementController(this._mockParkingService.Object);
        }
    }
}
