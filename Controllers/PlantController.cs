using Microsoft.AspNetCore.Mvc;

namespace Pi_Plant.Controllers
{
    public class PlantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
