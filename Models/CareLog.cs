namespace Pi_Plant.Models
{
    public class CareLog
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsWatered { get; set; }
        public bool HasLight { get; set; }

        public CareLog() { }

    }
}
