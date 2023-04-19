using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatecomid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComId",
                table: "RoomAndTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "RoomAndTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComId",
                table: "OrderTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComId",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComId",
                table: "RoomAndTable");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "RoomAndTable");

            migrationBuilder.DropColumn(
                name: "ComId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ComId",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "ComId",
                table: "Invoice");
        }
    }
}
