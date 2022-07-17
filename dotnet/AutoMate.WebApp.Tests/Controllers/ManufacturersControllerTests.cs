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
    public class ManufacturersControllerTests {
        private readonly ManufacturersController controller;
        public ManufacturersControllerTests() {
            var db = new AutoMateCsvFileDatabase();
            controller = new ManufacturersController(db);
        }

        [Fact]
        public void Index_Returns_All_Manufacturers() {
            var result = controller.Index() as ViewResult;
            result.ShouldNotBeNull();
            var manufacturers = result.Model as IEnumerable<Manufacturer>;
            manufacturers.ShouldNotBeNull();
        }

        [Fact]
        public void Models_Returns_All_Models_For_Manufacturer() {
            var result = controller.Models("dmc") as ViewResult;
            result.ShouldNotBeNull();
            var mfr = result.Model as Manufacturer;
            mfr.ShouldNotBeNull();
            mfr.Models.ShouldContain(m => m.Code == "dmc-delorean");
        }
    }
}
