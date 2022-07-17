using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMate.Data;
using AutoMate.Data.Entities;
using AutoMate.WebApp.Models;

namespace AutoMate.WebApp.Controllers.Api {
    public class VehiclesController : ApiController {
        private readonly IAutoMateDatabase db;

        public VehiclesController(IAutoMateDatabase db) {
            this.db = db;
        }

        public IHttpActionResult Get(int index = 0, int count = 10) {
            var items = db.ListVehicles().Skip(index).Take(count);
            var total = db.CountVehicles();
            // ReSharper disable once InconsistentNaming
            var _links = Hal.Paginate("/api/vehicles", index, count, total);
            return Ok(new {
                _links,
                items,
            });
        }

        public IHttpActionResult Get(string id) {
            var vehicle = db.FindVehicle(id);
            if (vehicle == default) return NotFound();
            var resource = vehicle.ToResource();
            resource._actions = new {
                delete = new {
                    name = "Delete this vehicle",
                    href = $"/api/vehicles/{id}",
                    method = "DELETE"
                }
            };
            return Ok(resource);
        }


        public IHttpActionResult Put(string id, [FromBody] VehicleDto dto) {
            var vehicleModel = db.FindModel(dto.ModelCode);
            var vehicle = new Vehicle {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                ModelCode = vehicleModel.Code
            };
            db.UpdateVehicle(vehicle);
            return Ok(vehicle);
        }

        public IHttpActionResult Delete(string id) {
            var vehicle = db.FindVehicle(id);
            if (vehicle == default) return NotFound();
            db.DeleteVehicle(vehicle);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
