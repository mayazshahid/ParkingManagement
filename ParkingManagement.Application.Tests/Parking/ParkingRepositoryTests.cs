using Moq;
using ParkingManagement.Application.Parking;
using ParkingManagement.Application.Parking.Interfaces;
using ParkingManagement.Core.Models;
using static System.Formats.Asn1.AsnWriter;

namespace ParkingManagement.Application.Tests.Parking
{
    [TestClass]
    public class ParkingRepositoryTests
    {
        private readonly DataStore _dataStore;
        public ParkingRepositoryTests()
        {
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
        public void GetAvailability_OnInvalidDate_ReturnsNull()
        {
            // Arrange
            var request = new GetAvailabilityRequest();
            var store = this._dataStore;
            var sut = new ParkingRepository(store);

            // Act
            var actual = sut.GetAvailability(request);

            // Assert
            Assert.IsNull(actual);            
        }

        [TestMethod]
        public void GetAvailability_OnValidDate_ReturnsAvailabilityList()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = fromDate.AddDays(1).Date;
            var request = new GetAvailabilityRequest { FromDate = fromDate, ToDate = toDate };
            var store = this._dataStore;
            store.ParkingSpaces.Add(new ParkingSpace { RegistrationNumber = "aa", FromDate = fromDate, ToDate = toDate });
            var sut = new ParkingRepository(store);

            // Act
            var actual = sut.GetAvailability(request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Any());
            Assert.IsTrue(actual.Count() == 1);
            Assert.AreEqual(DateOnly.FromDateTime(fromDate), actual.First().Date);
            Assert.AreEqual("9 free spaces", actual.First().FreeSpace);
        }

        [TestMethod]
        public void AddParking()
        {
            // Arrange
            var request = new AddParkingRequest { RegistrationNumber = "ABC", FromDate = DateTime.Now.Date, ToDate = DateTime.Now.AddDays(1).Date};
            var store = this._dataStore;
            var parkingSpace = new ParkingSpace
            {
                Id = 1,
                RegistrationNumber = request.RegistrationNumber.ToUpper(),
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Price = store.GetPrice(request.FromDate, request.ToDate),
            };
            var sut = new ParkingRepository(store);

            // Act
            var actual = sut.AddParking(request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(parkingSpace.Id, actual.Id);
        }

        [TestMethod]
        public void ModifyParkingSpace_InValidInput_ReturnsModifiedSpace()
        {
            // Arrange
            int id = 1;
            var fromDate = DateTime.Now.Date;
            var toDate = fromDate.AddDays(1).Date;
            var request = new UpdateParkingRequest { FromDate = fromDate, ToDate = toDate };
            var store = this._dataStore;
            store.ParkingSpaces.Add(new ParkingSpace
            {
                Id = 1,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.AddDays(2).Date,
                Price = store.GetPrice(request.FromDate, request.ToDate),
            });
            var sut = new ParkingRepository(store);

            // Act
            var actual = sut.ModifyParkingSpace(id, request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(fromDate.Date, actual.FromDate.Date);
            Assert.AreEqual(toDate.Date, actual.ToDate.Date);
        }

        [TestMethod]
        public void ModifyParkingSpace_InInvalidInput_ReturnsNull()
        {
            // Arrange
            int id = 1;
            var request = new UpdateParkingRequest();
            var store = this._dataStore;
            var sut = new ParkingRepository(store);

            // Act
            var actual = sut.ModifyParkingSpace(id, request);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void CancelParkingSpace()
        {
            // Arrange
            int id = 1;
            var fromDate = DateTime.Now.Date;
            var toDate = fromDate.AddDays(1).Date;
            var store = this._dataStore;
            store.ParkingSpaces.Add(new ParkingSpace
            {
                Id = 1,
                FromDate = fromDate,
                ToDate = toDate,
                Price = store.GetPrice(fromDate, toDate),
            });
            var sut = new ParkingRepository(store);

            // Act
            sut.CancelParkingSpace(id);

            // Assret
            Assert.IsTrue(store.ParkingSpaces.Count == 0);
        }
    }
}