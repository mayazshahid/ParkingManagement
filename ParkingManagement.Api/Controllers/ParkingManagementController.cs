using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Core.Models;

namespace ParkingManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingManagementController : ControllerBase
    {
        private readonly Services.Parking.Interfaces.IParkingService _service;

        public ParkingManagementController(
            Services.Parking.Interfaces.IParkingService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(_service));
        }

        [HttpGet("GetAllOccuppiedSpaces")]
        public ActionResult<IEnumerable<ParkingSpace>> GetAllOccuppiedSpaces()
        {
            var (response, parkingSpaces) = this._service.GetAllOccuppiedSpaces();
            if (response.IsSuccess)
            {
                return Ok(parkingSpaces);
            }

            return this.GetActionResult(response);
        }

        [HttpGet("GetAvailability")]
        public ActionResult<IEnumerable<string>> GetAvailability([FromQuery]GetAvailabilityRequest request)
        {
            var (response, availablParkingSpaces) = this._service.GetAvailability(request);
            if (response.IsSuccess)
            {
                return Ok(availablParkingSpaces);
            }

            return this.GetActionResult(response);
        }       

        [HttpPost("AddReservation")]
        public ActionResult<ParkingSpace> AddReservation([FromBody] AddParkingRequest request)
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest("Inputs are not valid.");
            }           

            var (response, parkingSpace) = this._service.AddReservation(request);
            if (response.IsSuccess)
            {
                return this.Created("AddReservation", parkingSpace);
            }


            return this.GetActionResult(response);
        }

        [HttpPut("ModifyReservation/{id}")]
        public ActionResult ModifyReservation(int id, UpdateParkingRequest request)
        {
            var (response, parkingSpace) = this._service.ModifyReservation(id, request);
            if (response.IsSuccess)
            {
                return this.NoContent();                
            }

            return this.GetActionResult(response);
        }

        [HttpDelete("CancelReservation/{id}")]
        public ActionResult CancelReservation(int id)
        {            
            var response = this._service.CancelReservation(id);
            if (response.IsSuccess)
            {
                return this.NoContent();
            }

            return this.GetActionResult(response);
        }

        private ActionResult GetActionResult(ServiceResponse response)
        {
            return response.HttpStatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => BadRequest(response.Message),
                System.Net.HttpStatusCode.NotFound => NotFound(response.Message),
                _ => Ok(response),
            };
        }
    }
}
