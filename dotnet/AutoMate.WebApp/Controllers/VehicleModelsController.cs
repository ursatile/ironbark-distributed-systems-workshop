using System.Linq;
using System.Web.Mvc;
using AutoMate.Data;

namespace AutoMate.WebApp.Controllers {
    public class VehicleModelsController : Controller {
        private readonly IAutoMateDatabase db;

        public VehicleModelsController(IAutoMateDatabase db) {
            this.db = db;
        }

        public ActionResult Vehicles(string id) {
            var model = db.ListModels().FirstOrDefault(m => m.Code == id);
            return View(model);
        }

        public ActionResult Index() {
            var models = db.ListModels();
            return View(models);
        }
    }
}