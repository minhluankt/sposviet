using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatecomidinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdInvoice",
                table: "OrderTable");

            migrationBuilder.AddColumn<int>(
                name: "TypeProduct",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Comid",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_IdOrderTable",
                table: "Invoice",
                column: "IdOrderTable",
                unique: true,
                filter: "[IdOrderTable] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_OrderTable_IdOrderTable",
                table: "Invoice",
                column: "IdOrderTable",
                principalTable: "OrderTable",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_OrderTable_IdOrderTable",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_IdOrderTable",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "TypeProduct",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "Comid",
                table: "Customer");

            migrationBuilder.AddColumn<int>(
                name: "IdInvoice",
                table: "OrderTable",
                type: "int",
                nullable: true);
        }
    }
}
