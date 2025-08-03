using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi_Plant.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NickName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Species = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PlantedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdealMinTemperature = table.Column<double>(type: "REAL", nullable: false),
                    IdealMaxTemperature = table.Column<double>(type: "REAL", nullable: false),
                    IdealMinHumidity = table.Column<double>(type: "REAL", nullable: false),
                    IdealMaxHumidity = table.Column<double>(type: "REAL", nullable: false),
                    IdealMinSoilMoisture = table.Column<double>(type: "REAL", nullable: false),
                    IdealMaxSoilMoisture = table.Column<double>(type: "REAL", nullable: false),
                    IdealMinSunLight = table.Column<double>(type: "REAL", nullable: false),
                    IdealMaxSunLight = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Temperature = table.Column<double>(type: "REAL", nullable: false),
                    Humidity = table.Column<double>(type: "REAL", nullable: false),
                    SoilMoisture = table.Column<double>(type: "REAL", nullable: false),
                    LightLevel = table.Column<double>(type: "REAL", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorReadings_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Plants",
                columns: new[] { "Id", "IdealMaxHumidity", "IdealMaxSoilMoisture", "IdealMaxSunLight", "IdealMaxTemperature", "IdealMinHumidity", "IdealMinSoilMoisture", "IdealMinSunLight", "IdealMinTemperature", "IsActive", "Location", "Name", "NickName", "PlantedDate", "PurchaseDate", "Species" },
                values: new object[] { 1, 60.0, 70.0, 100.0, 28.0, 40.0, 40.0, 1.0, 20.0, true, "Office Shelf", "Turtle Plant", "", new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "???" });

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_PlantId_Timestamp",
                table: "SensorReadings",
                columns: new[] { "PlantId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_Timestamp",
                table: "SensorReadings",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorReadings");

            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
