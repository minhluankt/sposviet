using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class PurchaseDate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EntryPrice",
                table: "OrderTableItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EntryPrice",
                table: "InvoiceItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryPrice",
                table: "OrderTableItem");

            migrationBuilder.DropColumn(
                name: "EntryPrice",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Invoice");
        }
    }
}
