using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class remvoecolumeinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TGTKCKhac",
                table: "EInvoice");

            migrationBuilder.RenameColumn(
                name: "TGTKCThue",
                table: "EInvoice",
                newName: "DiscountNonTax");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountNonTax",
                table: "EInvoice",
                newName: "TGTKCThue");

            migrationBuilder.AddColumn<decimal>(
                name: "TGTKCKhac",
                table: "EInvoice",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
