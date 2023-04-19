using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updateEnumTypeRevenueExpenditure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComId",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdCity",
                table: "Suppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdDistrict",
                table: "Suppliers",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "IdCity",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "IdDistrict",
                table: "Suppliers");
        }
    }
}
