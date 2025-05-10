using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Raknah.Migrations
{
    /// <inheritdoc />
    public partial class updateColumnDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Reservations",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true,
                oldComputedColumnSql: "DATEDIFF(MINUTE, StartTimeOfParking, EndTimeOfParking)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Reservations",
                type: "time",
                nullable: true,
                computedColumnSql: "DATEDIFF(MINUTE, StartTimeOfParking, EndTimeOfParking)",
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);
        }
    }
}
