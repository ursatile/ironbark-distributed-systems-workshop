using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web.Http.Results;
using AutoMate.Data;
using AutoMate.WebApp.Controllers.Api;
using AutoMate.WebApp.Models;
using Shouldly;
using Xunit;

namespace AutoMate.WebApp.Tests.Controllers.Api {
    public class VehicleModelsControllerTests {
        private readonly VehicleModelsController controller;

        public VehicleModelsControllerTests() {
            var db = new AutoMateCsvFileDatabase();
            controller = new VehicleModelsController(db);
        }

        [Fact]
        public void Get_Models_Returns_Enumerable() {
            var result = controller.Get() as OkNegotiatedContentResult<IEnumerable<object>>;
            result.ShouldNotBeNull();
            var models = result.Content.ToList();
            models.Count.ShouldBe(1059);
        }

        [Fact]
        public void Get_Model_By_Id_Returns_Model() {
            var result = controller.Get("dmc-delorean") as OkNegotiatedContentResult<ExpandoObject>;
            result.ShouldNotBeNull();
            result.Content.ShouldNotBeNull();
            dynamic vehicleModel = result.Content;
            ((string)vehicleModel.Code).ShouldBe("dmc-delorean");
        }

        [Fact]
        public void Get_Model_With_Invalid_Id_Returns_404() {
            var result = controller.Get("no-such-vehicle-model");
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void POST_New_Vehicle_With_Existing_Registration_Returns_Conflict() {
            var dto = new VehicleDto {
                Year = 1985,
                Color = "Green",
                Registration = "OUTATIME"
            };
            var result = controller.Post("dmc-delorean", dto) as NegotiatedContentResult<string>;
            result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }

        [Fact]
        public void POST_New_Vehicle_Creates_Vehicle() {
            var dto = new VehicleDto {
                Year = 1985,
                Color = "Green",
                Registration = "XUNIT001"
            };
            var result = controller.Post("dmc-delorean", dto) as CreatedNegotiatedContentResult<ExpandoObject>;
            result.ShouldNotBeNull();
            result.Location.ToString().ShouldBe("/api/vehicles/XUNIT001");
            dynamic vehicle = result.Content;
            ((string)vehicle.Registration).ShouldBe("XUNIT001");
            ((string)vehicle.Color).ShouldBe("Green");
            ((int)vehicle.Year).ShouldBe(1985);
        }
    }
}