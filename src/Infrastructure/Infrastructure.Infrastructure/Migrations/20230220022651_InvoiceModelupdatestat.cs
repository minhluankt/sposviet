using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class InvoiceModelupdatestat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublishEInvoice",
                table: "Invoice");

            migrationBuilder.AddColumn<int>(
                name: "StatusPublishInvoiceOrder",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusPublishInvoiceOrder",
                table: "Invoice");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublishEInvoice",
                table: "Invoice",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
