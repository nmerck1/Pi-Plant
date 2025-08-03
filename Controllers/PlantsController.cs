using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pi_Plant.Models;
using Pi_Plant.Data; // DbContext

namespace Pi_Plant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly PlantMonitoringContext _context;

        public PlantsController(PlantMonitoringContext context)
        {
            _context = context;
        }

        // GET: api/plants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> GetPlants()
        {
            var plants = await _context.Plants
                .Where(p => p.IsActive)
                .Include(p => p.SensorReadings.OrderByDescending(sr => sr.Timestamp).Take(5)) // Include latest 5 readings
                .ToListAsync();

            return Ok(plants);
        }

        // GET: api/plants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Plant>> GetPlant(int id)
        {
            var plant = await _context.Plants
                .Include(p => p.SensorReadings.OrderByDescending(sr => sr.Timestamp))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (plant == null)
                return NotFound();

            return Ok(plant);
        }

        // POST: api/plants
        [HttpPost]
        public async Task<ActionResult<Plant>> CreatePlant(Plant plant)
        {
            // Remove Id if it's set (let the database generate it)
            plant.Id = 0;

            // Set default dates if not provided
            if (plant.PlantedDate == null && plant.PurchaseDate == null)
            {
                plant.PurchaseDate = DateTime.UtcNow;
            }

            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlant), new { id = plant.Id }, plant);
        }

        // PUT: api/plants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlant(int id, Plant plant)
        {
            if (id != plant.Id)
                return BadRequest("ID mismatch");

            var existingPlant = await _context.Plants.FindAsync(id);
            if (existingPlant == null)
                return NotFound();

            // Update properties
            existingPlant.Name = plant.Name;
            existingPlant.NickName = plant.NickName;
            existingPlant.Species = plant.Species;
            existingPlant.Location = plant.Location;
            existingPlant.PlantedDate = plant.PlantedDate;
            existingPlant.PurchaseDate = plant.PurchaseDate;
            existingPlant.IdealMinTemperature = plant.IdealMinTemperature;
            existingPlant.IdealMaxTemperature = plant.IdealMaxTemperature;
            existingPlant.IdealMinHumidity = plant.IdealMinHumidity;
            existingPlant.IdealMaxHumidity = plant.IdealMaxHumidity;
            existingPlant.IdealMinSoilMoisture = plant.IdealMinSoilMoisture;
            existingPlant.IdealMaxSoilMoisture = plant.IdealMaxSoilMoisture;
            existingPlant.IdealMinSunLight = plant.IdealMinSunLight;
            existingPlant.IdealMaxSunLight = plant.IdealMaxSunLight;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/plants/5 (Soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(int id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null)
                return NotFound();

            plant.IsActive = false; // Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/plants/5/health - Get latest health status
        [HttpGet("{id}/health")]
        public async Task<ActionResult<object>> GetPlantHealth(int id)
        {
            var latestReading = await _context.SensorReadings
                .Include(sr => sr.Plant)
                .Where(sr => sr.PlantId == id)
                .OrderByDescending(sr => sr.Timestamp)
                .FirstOrDefaultAsync();

            if (latestReading == null)
                return NotFound("No readings found for this plant");

            return Ok(new
            {
                PlantId = id,
                PlantName = latestReading.Plant.Name,
                NickName = latestReading.Plant.NickName,
                IsHealthy = latestReading.IsHealthy,
                LastReading = latestReading.Timestamp,
                CurrentReadings = new
                {
                    Temperature = latestReading.Temperature,
                    Humidity = latestReading.Humidity,
                    SoilMoisture = latestReading.SoilMoisture,
                    LightLevel = latestReading.LightLevel
                },
                IdealRanges = new
                {
                    Temperature = new { Min = latestReading.Plant.IdealMinTemperature, Max = latestReading.Plant.IdealMaxTemperature },
                    Humidity = new { Min = latestReading.Plant.IdealMinHumidity, Max = latestReading.Plant.IdealMaxHumidity },
                    SoilMoisture = new { Min = latestReading.Plant.IdealMinSoilMoisture, Max = latestReading.Plant.IdealMaxSoilMoisture },
                    LightLevel = new { Min = latestReading.Plant.IdealMinSunLight, Max = latestReading.Plant.IdealMaxSunLight }
                }
            });
        }

        // POST: api/plants/5/readings - Add sensor reading for a plant
        [HttpPost("{plantId}/readings")]
        public async Task<ActionResult<SensorReading>> AddSensorReading(int plantId, SensorReading reading)
        {
            var plant = await _context.Plants.FindAsync(plantId);
            if (plant == null)
                return NotFound("Plant not found");

            reading.PlantId = plantId;
            reading.Timestamp = DateTime.UtcNow;

            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlant), new { id = plantId }, reading);
        }

        // GET: api/plants/5/readings - Get readings for a plant
        [HttpGet("{plantId}/readings")]
        public async Task<ActionResult<IEnumerable<SensorReading>>> GetPlantReadings(
            int plantId,
            [FromQuery] int limit = 50,
            [FromQuery] DateTime? since = null)
        {
            var query = _context.SensorReadings
                .Where(sr => sr.PlantId == plantId);

            if (since.HasValue)
            {
                query = query.Where(sr => sr.Timestamp >= since.Value);
            }

            var readings = await query
                .OrderByDescending(sr => sr.Timestamp)
                .Take(limit)
                .ToListAsync();

            return Ok(readings);
        }

        private bool PlantExists(int id)
        {
            return _context.Plants.Any(e => e.Id == id);
        }
    }
}