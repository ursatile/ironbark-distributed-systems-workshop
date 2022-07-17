using System.Linq;
using System.Web.Mvc;
using AutoMate.Data;

namespace AutoMate.WebApp.Controllers {
    public class ManufacturersController : Controller {
        private readonly IAutoMateDatabase db;

        public ManufacturersController(IAutoMateDatabase db) {
            this.db = db;
        }
        public ActionResult Index() {
            var vehicles = db.ListManufacturers();
            return View(vehicles);
        }

        public ActionResult Models(string id) {
            var manufacturer = db.ListManufacturers().FirstOrDefault(m => m.Code == id);
            return View(manufacturer);
        }
    }
}