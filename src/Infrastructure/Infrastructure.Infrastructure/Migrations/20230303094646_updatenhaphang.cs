using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatenhaphang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountSuppliers",
                table: "PurchaseOrder",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseNo",
                table: "PurchaseOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuppliersCode",
                table: "PurchaseOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuppliersName",
                table: "PurchaseOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountSuppliers",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "PurchaseNo",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "SuppliersCode",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "SuppliersName",
                table: "PurchaseOrder");
        }
    }
}
