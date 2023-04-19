using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class amountSupplierss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayingAmount",
                table: "PurchaseOrder",
                newName: "Amount");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ItemPurchaseOrder",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "ItemPurchaseOrder",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "PurchaseOrder",
                newName: "PayingAmount");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "ItemPurchaseOrder",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<double>(
                name: "DiscountAmount",
                table: "ItemPurchaseOrder",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");
        }
    }
}
