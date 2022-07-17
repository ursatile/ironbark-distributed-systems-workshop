using System;
using System.Dynamic;
using System.Web.Http.Results;
using AutoMate.Data;
using AutoMate.Data.Entities;
using AutoMate.WebApp.Controllers.Api;
using AutoMate.WebApp.Models;
using Shouldly;
using Xunit;

namespace AutoMate.WebApp.Tests.Controllers.Api {
    public class VehiclesControllerTests {
        private readonly VehiclesController controller;
        public VehiclesControllerTests() {
            var db = new AutoMateCsvFileDatabase();
            controller = new VehiclesController(db);
        }

        [Fact]
        public void GET_Vehicle_Returns_Vehicle() {
            var result = controller.Get("OUTATIME") as OkNegotiatedContentResult<ExpandoObject>;
            dynamic vehicle = result.Content;
            ((string)vehicle.Color).ShouldBe("Silver");
            ((int)vehicle.Year).ShouldBe(1985);
        }

        [Fact]
        public void GET_No_Such_Vehicle_Returns_404_Not_Found() {
            var result = controller.Get("NOT-EVEN-A-REAL-CAR");
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void PUT_Vehicle_Creates_Vehicle() {
            var reg = Guid.NewGuid().ToString("N");
            var dto = new VehicleDto {
                Color = "Brown",
                ModelCode = "dmc-delorean",
                Registration = reg,
                Year = 1985
            };
            var putResult = controller.Put(reg, dto);
            putResult.ShouldBeOfType<OkNegotiatedContentResult<Vehicle>>();

            var getResult = controller.Get(reg) as OkNegotiatedContentResult<ExpandoObject>;
            getResult.ShouldNotBeNull();
            dynamic vehicle = getResult.Content;
            ((string)vehicle.Color).ShouldBe("Brown");
            ((string)vehicle.Registration).ShouldBe(reg, StringCompareShould.IgnoreCase);
        }

        [Fact]
        public void DELETE_Vehicle_Removes_Vehicle() {
            var reg = Guid.NewGuid().ToString("N");
            var dto = new VehicleDto {
                Color = "Brown",
                ModelCode = "dmc-delorean",
                Registration = reg,
                Year = 1985
            };
            controller.Put(reg, dto);
            controller.Get(reg).ShouldBeOfType<OkNegotiatedContentResult<ExpandoObject>>();
            controller.Delete(reg);
            controller.Get(reg).ShouldBeOfType<NotFoundResult>();
        }
    }
}