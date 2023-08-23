using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Actively.Migrations
{
    /// <inheritdoc />
    public partial class TypeToSport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Activities",
                newName: "Sport");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sport",
                table: "Activities",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
