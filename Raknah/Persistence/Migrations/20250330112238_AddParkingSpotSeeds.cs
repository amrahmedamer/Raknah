using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Raknah.Migrations
{
    /// <inheritdoc />
    public partial class AddParkingSpotSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ParkingSpots",
                columns: new[] { "Id", "Name", "SensorCode", "SensorStatus", "SpotStatus" },
                values: new object[,]
                {
                    { 1, "Spot-1", "Sensor-1", 0, 0 },
                    { 2, "Spot-2", "Sensor-2", 0, 0 },
                    { 3, "Spot-3", "Sensor-3", 0, 0 },
                    { 4, "Spot-4", "Sensor-4", 0, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
