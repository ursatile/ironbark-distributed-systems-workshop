using System.Reflection;
using System.Web.Http;

namespace AutoMate.WebApp.Controllers.Api {
    public class WelcomeController : ApiController {
        
        public IHttpActionResult Get() {
            return Ok(new {
                message = "Welcome to the AutoMate API",
                version = Assembly.GetExecutingAssembly().FullName,
                _links = new {
                    vehicles = new {
                        href = "/api/vehicles"
                    },
                    models = new {
                        href = "/api/vehiclemodels"
                    }
                }
            });
        }
    }
}