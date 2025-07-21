using Microsoft.AspNetCore.Mvc;
using Pi_Plant.Models;

namespace Pi_Plant.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class DashboardController : ControllerBase
        {
            private readonly PlantsController _plantsController;
            private readonly SensorController _sensorController;

            public DashboardController()
            {
                _plantsController = new PlantsController();
                _sensorController = new SensorController();
            }

            // GET: api/dashboard/overview
            [HttpGet("overview")]
            public ActionResult<object> GetDashboardOverview()
            {
                var plants = _plantsController.GetPlants().Value?.Cast<Plant>() ?? new List<Plant>();
                var plantStatuses = new List<PlantStatus>();

                foreach (var plant in plants)
                {
                    var latestReading = _sensorController.GetLatestReading(plant.Id).Value;
                    plantStatuses.Add(new PlantStatus
                    {
                        PlantId = plant.Id,
                        PlantName = plant.Name,
                        LatestReading = latestReading
                    });
                }

                return Ok(new
                {
                    TotalPlants = plants.Count(),
                    HealthyPlants = plantStatuses.Count(p => !p.NeedsAttention),
                    PlantsNeedingWater = plantStatuses.Count(p => p.NeedsWater),
                    PlantsNeedingAttention = plantStatuses.Count(p => p.NeedsAttention),
                    PlantStatuses = plantStatuses
                });
            }
        }
}
