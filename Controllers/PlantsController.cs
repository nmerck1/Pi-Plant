using Microsoft.AspNetCore.Mvc;
using Pi_Plant.Models;

namespace Pi_Plant.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class PlantsController : ControllerBase
        {
            // In-memory storage for now (replace with database later)
            private static List<Plant> _plants = new List<Plant>
        {
            new Plant
            {
                Id = 1,
                Name = "Basil",
                Species = "Ocimum basilicum",
                Location = "Kitchen Window",
                PlantedDate = DateTime.Now.AddDays(-30),
                IdealMinTemperature = 20,
                IdealMaxTemperature = 28,
                IdealMinSoilMoisture = 40,
                IdealMaxSoilMoisture = 70
            },
            new Plant
            {
                Id = 2,
                Name = "Snake Plant",
                Species = "Sansevieria trifasciata",
                Location = "Office Shelf",
                PlantedDate = DateTime.Now.AddDays(-90),
                IdealMinTemperature = 18,
                IdealMaxTemperature = 27,
                IdealMinSoilMoisture = 20,
                IdealMaxSoilMoisture = 40
            }
        };

            // GET: api/plants
            [HttpGet]
            public ActionResult<IEnumerable<Plant>> GetPlants()
            {
                return Ok(_plants.Where(p => p.IsActive));
            }

            // GET: api/plants/5
            [HttpGet("{id}")]
            public ActionResult<Plant> GetPlant(int id)
            {
                var plant = _plants.FirstOrDefault(p => p.Id == id && p.IsActive);
                if (plant == null)
                    return NotFound();

                return Ok(plant);
            }

            // POST: api/plants
            [HttpPost]
            public ActionResult<Plant> CreatePlant(Plant plant)
            {
                plant.Id = _plants.Count > 0 ? _plants.Max(p => p.Id) + 1 : 1;
                plant.PlantedDate = DateTime.UtcNow;
                _plants.Add(plant);

                return CreatedAtAction(nameof(GetPlant), new { id = plant.Id }, plant);
            }

            // PUT: api/plants/5
            [HttpPut("{id}")]
            public IActionResult UpdatePlant(int id, Plant plant)
            {
                var existingPlant = _plants.FirstOrDefault(p => p.Id == id);
                if (existingPlant == null)
                    return NotFound();

                existingPlant.Name = plant.Name;
                existingPlant.Species = plant.Species;
                existingPlant.Location = plant.Location;
                existingPlant.IdealMinTemperature = plant.IdealMinTemperature;
                existingPlant.IdealMaxTemperature = plant.IdealMaxTemperature;
                existingPlant.IdealMinHumidity = plant.IdealMinHumidity;
                existingPlant.IdealMaxHumidity = plant.IdealMaxHumidity;
                existingPlant.IdealMinSoilMoisture = plant.IdealMinSoilMoisture;
                existingPlant.IdealMaxSoilMoisture = plant.IdealMaxSoilMoisture;

                return NoContent();
            }

            // DELETE: api/plants/5
            [HttpDelete("{id}")]
            public IActionResult DeletePlant(int id)
            {
                var plant = _plants.FirstOrDefault(p => p.Id == id);
                if (plant == null)
                    return NotFound();

                plant.IsActive = false; // Soft delete
                return NoContent();
            }
        }
}
