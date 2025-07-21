namespace Pi_Plant.Models
{
    public class Plant
    {
        public int Id { get; set; }
        public string? Name { get; set; }           // name will be my own nick name for it (Fred, Plant1, etc.)
        public string? Description { get; set; }    // describes the plant features such as viney, tall, upgright, hanging, etc. 
        public string Type { get; set; }            // this is the official science-y plant name
        //public string? HeightInFt { get; set; }      // height of plant
        //public string? Color {  get; set; }         
        public DateTime? PlantedDate { get; set; }  // date this plant/seed was planted (if not bought) This is helpful to tell age as well
        public DateTime? PurchaseDate { get; set; } // date this plant/seed was purchased

        public Plant(string newName, string desc, string type)
        {
            Name = newName;
            Description = desc;
            Type = type;
        }

    }
}
