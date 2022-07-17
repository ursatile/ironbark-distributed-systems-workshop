using System.Web.Mvc;
using AutoMate.WebApp.Controllers;
using Shouldly;
using Xunit;

namespace AutoMate.WebApp.Tests.Controllers {
    public class HomeControllerTests {
        [Fact]
        public void Index() {
            var controller = new HomeController();
            var result = controller.Index() as ViewResult;
            result.ShouldNotBeNull();
            ((string)result.ViewBag.Title).ShouldBe("Home Page");
        }
    }
}