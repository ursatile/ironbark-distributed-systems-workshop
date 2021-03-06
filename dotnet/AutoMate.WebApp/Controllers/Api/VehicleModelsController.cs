using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMate.Data;
using AutoMate.Data.Entities;
using AutoMate.WebApp.Models;
using MassTransit;

namespace AutoMate.WebApp.Controllers.Api {
    public class VehicleModelsController : ApiController {

        private readonly IAutoMateDatabase db;
        private readonly IBus busControl;

        public VehicleModelsController(IAutoMateDatabase db, IBusControl busControl) {
            this.db = db;
            this.busControl = busControl;
        }

        public IHttpActionResult Get() {
            var models = db.ListModels().Select(m => m.ToResource());
            return Ok(models);
        }

        public IHttpActionResult Get(string id) {
            var vehicleModel = db.FindModel(id);
            if (vehicleModel == default) return NotFound();
            var resource = vehicleModel.ToResource();
            resource._actions = new {
                create = new {
                    href = $"/api/vehiclemodels/{id}",
                    method = "POST",
                    name = $"Create a new {vehicleModel.Manufacturer.Name} {vehicleModel.Name}",
                    type = "application/json"
                }
            };
            return Ok(resource);
        }

        public async Task<IHttpActionResult> Post(string id, [FromBody] VehicleDto dto) {
            var existing = db.FindVehicle(dto.Registration);
            if (existing != default) {
                var message =
                    $"Sorry, the vehicle with registration {dto.Registration} is already in our database and you can't list the same vehicle twice.";
                return Content(HttpStatusCode.Conflict, message);
            }
            var vehicleModel = db.FindModel(id);
            var vehicle = new Vehicle {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                VehicleModel = vehicleModel
            };
            db.CreateVehicle(vehicle);
            await SendSubmitVehicleListingCommand(vehicle);
            return Created(
                $"/api/vehicles/{vehicle.Registration}",
                vehicle.ToResource());
        }

        private async Task SendSubmitVehicleListingCommand(Vehicle vehicle) {
            var command = new {
                vehicle.Color,
                Manufacturer = vehicle.VehicleModel.Manufacturer.Name,
                VehicleModel = vehicle.VehicleModel.Name,
                vehicle.Registration,
                vehicle.Year
            };
            var endpoint = await busControl.GetSendEndpoint(new Uri("queue:submit-vehicle-listing"));
            await endpoint.Send(command);
        }
    }
}
