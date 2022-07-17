using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMate.Data;
using AutoMate.Data.Entities;
using AutoMate.WebApp;
using AutoMate.WebApp.Controllers;
using Shouldly;
using Xunit;

namespace AutoMate.WebApp.Tests.Controllers {
    public class VehicleModelsControllerTests {
        private readonly VehicleModelsController controller;

        public VehicleModelsControllerTests() {
            var db = new AutoMateCsvFileDatabase();
            controller = new VehicleModelsController(db);
        }

        [Fact]
        public void Index_Returns_All_Manufacturers() {
            var result = controller.Index() as ViewResult;
            result.ShouldNotBeNull();
            var models = result.Model as IEnumerable<VehicleModel>;
            models.ShouldNotBeNull();
        }

        [Fact]
        public void Vehicles_Returns_All_Vehicles_Matching_Model() {
            var result = controller.Vehicles("dmc-delorean") as ViewResult;
            result.ShouldNotBeNull();
            var vm = result.Model as VehicleModel;
            vm.ShouldNotBeNull();
            vm.Vehicles.ShouldContain(v => v.Registration == "OUTATIME");
        }
    }
}



