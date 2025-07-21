namespace Pi_Plant.Models
{
    public class CareLog
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int TimesWatered { get; set; }
        public bool HasLight { get; set; }
        public DateTime LogDate { get; set; }

        public CareLog() { }

    }
}
