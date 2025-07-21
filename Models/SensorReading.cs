namespace Pi_Plant.Models
{
    public class SensorReading
    {
        public int Id { get; set; }
        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        public double Temperature { get; set; } // Celsius
        public double Humidity { get; set; } // Percentage
        public double SoilMoisture { get; set; } // Percentage
        public double LightLevel { get; set; } // Lux or percentage

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Calculated properties
        public bool IsHealthy =>
            Temperature >= Plant?.IdealMinTemperature && Temperature <= Plant?.IdealMaxTemperature &&
            Humidity >= Plant?.IdealMinHumidity && Humidity <= Plant?.IdealMaxHumidity &&
            SoilMoisture >= Plant?.IdealMinSoilMoisture && SoilMoisture <= Plant?.IdealMaxSoilMoisture &&
            LightLevel >= Plant?.IdealMinSunLight && LightLevel <= Plant?.IdealMaxSunLight;
    }
}
