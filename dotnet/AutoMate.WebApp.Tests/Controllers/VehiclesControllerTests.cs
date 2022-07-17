using System.Collections.Generic;
using System.Web.Mvc;
using AutoMate.Data;
using AutoMate.Data.Entities;
using AutoMate.WebApp.Controllers;
using AutoMate.WebApp.Models;
using Shouldly;
using Xunit;

namespace AutoMate.WebApp.Tests.Controllers {
    public class VehiclesControllerTests {
        private readonly VehiclesController controller;
        private readonly IAutoMateDatabase db;

        public VehiclesControllerTests() {
            db = new AutoMateCsvFileDatabase();
            controller = new VehiclesController(db);
        }

        [Fact]
        public void Details_Shows_Vehicle() {
            var result = controller.Details("OUTATIME") as ViewResult;
            result.ShouldNotBeNull();
            var vehicle = result.Model as Vehicle;
            vehicle.ShouldNotBeNull();
            vehicle.Color.ShouldBe("Silver");
            vehicle.Year.ShouldBe(1985);
        }

        [Fact]
        public void Index_Lists_All_Vehicles() {
            var result = controller.Index() as ViewResult;
            result.ShouldNotBeNull();
            var vehicles = result.Model as IEnumerable<Vehicle>;
            vehicles.ShouldNotBeEmpty();
        }

        [Fact]
        public void Advertise_GET_Shows_New_Vehicle_Form() {
            var result = controller.Advertise("dmc-delorean") as ViewResult;
            result.ShouldNotBeNull();
            var dto = result.Model as VehicleDto;
            dto.ShouldNotBeNull();
            dto.ModelCode.ShouldBe("dmc-delorean");
        }

        [Fact]
        public void Advertise_POST_Adds_New_Vehicle() {
            var dto = new VehicleDto() {
                ModelCode = "dmc-delorean",
                Registration = "XUNIT001",
                Color = "Black",
                Year = 2022
            };
            controller.Advertise(dto);
            var record = db.FindVehicle("XUNIT001");
            record.ShouldNotBeNull();
            record.Color.ShouldBe("Black");
            record.Year.ShouldBe(2022);
        }
    }
}