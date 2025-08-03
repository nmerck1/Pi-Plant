namespace Pi_Plant.Models
{
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // nickname or basic name "Lily", "Tulip", etc. 
        public string? NickName { get; set; } = string.Empty;        // what you want to call your plant
        public string Species { get; set; } = string.Empty;     // official science name for this plant
        public string Location { get; set; } = string.Empty;    // just a simple way to describe where the plant is located
        public DateTime? PlantedDate { get; set; }  // this can be null if there is a purchase date and you didn't plant the seed
        public DateTime? PurchaseDate { get; set; } // this can be null if the seed was heirloom and got planted by seeds you already had
        public bool IsActive { get; set; } = true;

        // Ideal ranges for this plant
        public double IdealMinTemperature { get; set; } = 18.0; // Celsius
        public double IdealMaxTemperature { get; set; } = 25.0;
        public double IdealMinHumidity { get; set; } = 40.0; // Percentage
        public double IdealMaxHumidity { get; set; } = 60.0;
        public double IdealMinSoilMoisture { get; set; } = 30.0; // Percentage
        public double IdealMaxSoilMoisture { get; set; } = 70.0;
        public double IdealMinSunLight { get; set; } = 1.0;    // Percentage
        public double IdealMaxSunLight { get; set; } = 100.0; 

        public List<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();


    }
}
