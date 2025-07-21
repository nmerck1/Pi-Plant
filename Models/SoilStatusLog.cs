using System.ComponentModel.DataAnnotations.Schema;

namespace Pi_Plant.Models
{
    public class SoilStatusLog
    {
        public int Id {  get; set; }

        //[ForeignKey]
        public int PlantId { get; set; }

        public string Status { get; set; }
        public DateTime LogDate { get; set; }

    }
}
