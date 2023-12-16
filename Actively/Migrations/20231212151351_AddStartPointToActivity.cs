using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Actively.Migrations
{
    /// <inheritdoc />
    public partial class AddStartPointToActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "StartLatitude",
                table: "Activities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartLongitude",
                table: "Activities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartLatitude",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "StartLongitude",
                table: "Activities");
        }
    }
}
