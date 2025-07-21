namespace Pi_Plant.Models
{
    public class HealthLog
    {
        public enum OVERALL_HEALTH_RESULT
        {
            GREAT = 1,
            GOOD = 2,
            POOR = 3,
            BAD = 4,
        }

        public enum STATUS
        {
            NEEDS_WATER,
            NEEDS_HEAT,
            NEEDS_COOL,
            NEEDS_SUN,
            NEEDS_LIGHT,
        }
        public HealthLog() { }
        public int Id { get; set; }
        public string Status
    }
}
