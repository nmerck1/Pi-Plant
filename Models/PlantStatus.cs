namespace Pi_Plant.Models
{
    public class PlantStatus
    {
        public int PlantId { get; set; }
        public string PlantName { get; set; } = string.Empty;
        public SensorReading? LatestReading { get; set; }
        public bool NeedsWater => LatestReading?.SoilMoisture < 30.0;
        public bool NeedsAttention => LatestReading?.IsHealthy == false;
        public string Status => NeedsWater ? "Needs Water" :
                               NeedsAttention ? "Needs Attention" :
                               "Healthy";
    }
}
