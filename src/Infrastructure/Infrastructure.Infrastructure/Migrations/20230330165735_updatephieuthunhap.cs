using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatephieuthunhap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdCategoryCevenue",
                table: "RevenueExpenditure",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IdInvoice",
                table: "RevenueExpenditure",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdPurchaseOrder",
                table: "RevenueExpenditure",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "RevenueExpenditure",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Typecategory",
                table: "RevenueExpenditure",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdInvoice",
                table: "RevenueExpenditure");

            migrationBuilder.DropColumn(
                name: "IdPurchaseOrder",
                table: "RevenueExpenditure");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "RevenueExpenditure");

            migrationBuilder.DropColumn(
                name: "Typecategory",
                table: "RevenueExpenditure");

            migrationBuilder.AlterColumn<int>(
                name: "IdCategoryCevenue",
                table: "RevenueExpenditure",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
