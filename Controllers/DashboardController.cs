using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pi_Plant.Models;
using Pi_Plant.Data;

namespace Pi_Plant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly PlantMonitoringContext _context;

        public DashboardController(PlantMonitoringContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/overview
        [HttpGet("overview")]
        public async Task<ActionResult<object>> GetDashboardOverview()
        {
            // Get all active plants with their latest sensor reading
            var plants = await _context.Plants
                .Where(p => p.IsActive)
                .Include(p => p.SensorReadings.OrderByDescending(sr => sr.Timestamp).Take(1))
                .ToListAsync();

            var plantStatuses = new List<PlantStatus>();

            foreach (var plant in plants)
            {
                var latestReading = plant.SensorReadings.FirstOrDefault();
                plantStatuses.Add(new PlantStatus
                {
                    PlantId = plant.Id,
                    PlantName = plant.Name,
                    NickName = plant.NickName,
                    LatestReading = latestReading,
                    NeedsAttention = latestReading != null && !latestReading.IsHealthy,
                    NeedsWater = latestReading != null && latestReading.SoilMoisture < plant.IdealMinSoilMoisture,
                    LastReadingTime = latestReading?.Timestamp
                });
            }

            return Ok(new
            {
                TotalPlants = plants.Count,
                HealthyPlants = plantStatuses.Count(p => !p.NeedsAttention),
                PlantsNeedingWater = plantStatuses.Count(p => p.NeedsWater),
                PlantsNeedingAttention = plantStatuses.Count(p => p.NeedsAttention),
                PlantsWithoutReadings = plantStatuses.Count(p => p.LatestReading == null),
                PlantStatuses = plantStatuses
            });
        }

        // GET: api/dashboard/summary
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetDashboardSummary()
        {
            var totalPlants = await _context.Plants.CountAsync(p => p.IsActive);
            var totalReadings = await _context.SensorReadings.CountAsync();

            var recentReadings = await _context.SensorReadings
                .Where(sr => sr.Timestamp >= DateTime.UtcNow.AddHours(-24))
                .CountAsync();

            var plantsWithRecentReadings = await _context.Plants
                .Where(p => p.IsActive && p.SensorReadings.Any(sr => sr.Timestamp >= DateTime.UtcNow.AddHours(-24)))
                .CountAsync();

            return Ok(new
            {
                TotalActivePlants = totalPlants,
                TotalReadings = totalReadings,
                ReadingsLast24Hours = recentReadings,
                PlantsWithRecentData = plantsWithRecentReadings,
                LastUpdated = DateTime.UtcNow
            });
        }

        // GET: api/dashboard/alerts
        [HttpGet("alerts")]
        public async Task<ActionResult<object>> GetDashboardAlerts()
        {
            var alerts = new List<PlantAlert>();

            // Get plants with their latest readings
            var plantsWithReadings = await _context.Plants
                .Where(p => p.IsActive)
                .Include(p => p.SensorReadings.OrderByDescending(sr => sr.Timestamp).Take(1))
                .ToListAsync();

            foreach (var plant in plantsWithReadings)
            {
                var latestReading = plant.SensorReadings.FirstOrDefault();
                if (latestReading == null)
                {
                    alerts.Add(new PlantAlert
                    {
                        PlantId = plant.Id,
                        PlantName = plant.Name,
                        AlertType = "No Data",
                        Message = "No sensor readings available",
                        Severity = AlertSeverity.Warning,
                        Timestamp = DateTime.UtcNow
                    });
                    continue;
                }

                // Check for various alert conditions
                if (latestReading.Temperature < plant.IdealMinTemperature)
                {
                    alerts.Add(CreateAlert(plant, latestReading, "Low Temperature",
                        $"Temperature ({latestReading.Temperature:F1}°C) is below ideal range", AlertSeverity.Warning));
                }
                else if (latestReading.Temperature > plant.IdealMaxTemperature)
                {
                    alerts.Add(CreateAlert(plant, latestReading, "High Temperature",
                        $"Temperature ({latestReading.Temperature:F1}°C) is above ideal range", AlertSeverity.Warning));
                }

                if (latestReading.Humidity < plant.IdealMinHumidity)
                {
                    alerts.Add(CreateAlert(plant, latestReading, "Low Humidity",
                        $"Humidity ({latestReading.Humidity:F1}%) is below ideal range", AlertSeverity.Info));
                }
                else if (latestReading.Humidity > plant.IdealMaxHumidity)
                {
                    alerts.Add(CreateAlert(plant, latestReading, "High Humidity",
                        $"Humidity ({latestReading.Humidity:F1}%) is above ideal range", AlertSeverity.Info));
                }

                if (latestReading.SoilMoisture < plant.IdealMinSoilMoisture)
                {
                    alerts.Add(CreateAlert(plant, latestReading, "Low Soil Moisture",
                        $"Soil moisture ({latestReading.SoilMoisture:F1}%) - Plant needs watering", AlertSeverity.Critical));
                }
                else if (latestReading.SoilMoisture > plant.IdealMaxSoilMoisture)
                {
                    alerts.Add(CreateAlert(plant, latestReading, "High Soil Moisture",
                        $"Soil moisture ({latestReading.SoilMoisture:F1}%) - Possible overwatering", AlertSeverity.Warning));
                }

                if (latestReading.LightLevel < plant.IdealMinSunLight)
                {
                    alerts.Add(CreateAlert(plant, latestReading, "Low Light",
                        $"Light level ({latestReading.LightLevel:F1}) is below ideal range", AlertSeverity.Info));
                }

                // Check for stale data
                if (latestReading.Timestamp < DateTime.UtcNow.AddHours(-6))
                {
                    alerts.Add(CreateAlert(plant, latestReading, "Stale Data",
                        $"Last reading was {GetTimeAgo(latestReading.Timestamp)}", AlertSeverity.Warning));
                }
            }

            return Ok(new
            {
                TotalAlerts = alerts.Count,
                CriticalAlerts = alerts.Count(a => a.Severity == AlertSeverity.Critical),
                WarningAlerts = alerts.Count(a => a.Severity == AlertSeverity.Warning),
                InfoAlerts = alerts.Count(a => a.Severity == AlertSeverity.Info),
                Alerts = alerts.OrderByDescending(a => a.Severity).ThenByDescending(a => a.Timestamp)
            });
        }

        private static PlantAlert CreateAlert(Plant plant, SensorReading reading, string alertType, string message, AlertSeverity severity)
        {
            return new PlantAlert
            {
                PlantId = plant.Id,
                PlantName = plant.Name,
                AlertType = alertType,
                Message = message,
                Severity = severity,
                Timestamp = reading.Timestamp
            };
        }

        private static string GetTimeAgo(DateTime timestamp)
        {
            var timeSpan = DateTime.UtcNow - timestamp;

            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} day(s) ago";
            if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours} hour(s) ago";
            if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes} minute(s) ago";

            return "Just now";
        }
    }

    // Supporting classes - add these to your Models folder or create a separate file
    public class PlantStatus
    {
        public int PlantId { get; set; }
        public string PlantName { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public SensorReading? LatestReading { get; set; }
        public bool NeedsAttention { get; set; }
        public bool NeedsWater { get; set; }
        public DateTime? LastReadingTime { get; set; }
    }

    public class PlantAlert
    {
        public int PlantId { get; set; }
        public string PlantName { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public AlertSeverity Severity { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum AlertSeverity
    {
        Info = 1,
        Warning = 2,
        Critical = 3
    }
}