using Microsoft.AspNetCore.Mvc;
using Pi_Plant.Models;

namespace Pi_Plant.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class SensorController : ControllerBase
        {
            // In-memory storage for sensor readings
            private static List<SensorReading> _sensorReadings = new List<SensorReading>();

            // GET: api/sensor/readings/{plantId}
            [HttpGet("readings/{plantId}")]
            public ActionResult<IEnumerable<SensorReading>> GetPlantReadings(int plantId,
                [FromQuery] int hours = 24)
            {
                var cutoffTime = DateTime.UtcNow.AddHours(-hours);
                var readings = _sensorReadings
                    .Where(r => r.PlantId == plantId && r.Timestamp >= cutoffTime)
                    .OrderByDescending(r => r.Timestamp)
                    .ToList();

                return Ok(readings);
            }

            // GET: api/sensor/latest/{plantId}
            [HttpGet("latest/{plantId}")]
            public ActionResult<SensorReading> GetLatestReading(int plantId)
            {
                var latest = _sensorReadings
                    .Where(r => r.PlantId == plantId)
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();

                if (latest == null)
                    return NotFound();

                return Ok(latest);
            }

            // POST: api/sensor/reading
            [HttpPost("reading")]
            public ActionResult<SensorReading> AddReading(SensorReading reading)
            {
                reading.Id = _sensorReadings.Count > 0 ? _sensorReadings.Max(r => r.Id) + 1 : 1;
                reading.Timestamp = DateTime.UtcNow;
                _sensorReadings.Add(reading);

                return CreatedAtAction(nameof(GetLatestReading),
                    new { plantId = reading.PlantId }, reading);
            }

            // GET: api/sensor/simulate/{plantId}
            [HttpPost("simulate/{plantId}")]
            public ActionResult<SensorReading> SimulateReading(int plantId)
            {
                var random = new Random();
                var reading = new SensorReading
                {
                    Id = _sensorReadings.Count > 0 ? _sensorReadings.Max(r => r.Id) + 1 : 1,
                    PlantId = plantId,
                    Temperature = Math.Round(random.NextDouble() * 15 + 15, 1), // 15-30°C
                    Humidity = Math.Round(random.NextDouble() * 40 + 30, 1), // 30-70%
                    SoilMoisture = Math.Round(random.NextDouble() * 60 + 20, 1), // 20-80%
                    LightLevel = Math.Round(random.NextDouble() * 100, 1), // 0-100%
                    Timestamp = DateTime.UtcNow
                };

                _sensorReadings.Add(reading);
                return Ok(reading);
            }
        }
 
}
