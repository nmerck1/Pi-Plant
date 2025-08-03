
using global::Pi_Plant.Models;
using Microsoft.EntityFrameworkCore;

namespace Pi_Plant.Data
{
    public class PlantMonitoringContext : DbContext
    {
        public PlantMonitoringContext(DbContextOptions<PlantMonitoringContext> options)
            : base(options) { }

        public DbSet<Plant> Plants { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Plant entity
            modelBuilder.Entity<Plant>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Species).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Location).IsRequired().HasMaxLength(100);
                entity.Property(p => p.NickName).HasMaxLength(100);

                // Set up the relationship
                entity.HasMany(p => p.SensorReadings)
                        .WithOne(sr => sr.Plant)
                        .HasForeignKey(sr => sr.PlantId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SensorReading entity
            modelBuilder.Entity<SensorReading>(entity =>
            {
                entity.HasKey(sr => sr.Id);
                entity.Property(sr => sr.Timestamp).IsRequired();

                // Index on PlantId and Timestamp for better query performance
                entity.HasIndex(sr => new { sr.PlantId, sr.Timestamp });
                entity.HasIndex(sr => sr.Timestamp);
            });

            // Seed some default data 
            modelBuilder.Entity<Plant>().HasData(
                new Plant
                {
                    Id = 1,
                    Name = "Turtle Plant",
                    Species = "???",
                    Location = "Office Shelf",
                    IsActive = true,
                    PlantedDate = new DateTime(2024, 7, 1),
                    IdealMinTemperature = 20,
                    IdealMaxTemperature = 28,
                    IdealMinSoilMoisture = 40,
                    IdealMaxSoilMoisture = 70,
                    IdealMinSunLight = 1.0,
                    IdealMaxSunLight = 100.0
                }
            );
        }
    }
}

