using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class Kitchenup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kitchen_Id_Code",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Kitchen");

            migrationBuilder.AddColumn<string>(
                name: "Cashername",
                table: "Kitchen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComId",
                table: "Kitchen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IdCashername",
                table: "Kitchen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdDichVu",
                table: "Kitchen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdProduct",
                table: "Kitchen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OrderCode",
                table: "Kitchen",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProCode",
                table: "Kitchen",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Kitchen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Kitchen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TableName",
                table: "Kitchen",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_Id",
                table: "Kitchen",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kitchen_Id",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "Cashername",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "ComId",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "IdCashername",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "IdDichVu",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "IdProduct",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "ProCode",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "TableName",
                table: "Kitchen");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Kitchen",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_Id_Code",
                table: "Kitchen",
                columns: new[] { "Id", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");
        }
    }
}
