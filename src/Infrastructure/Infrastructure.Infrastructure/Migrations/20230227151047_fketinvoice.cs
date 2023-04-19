using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class fketinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FkeyEInvoice",
                table: "EInvoice",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_FkeyEInvoice",
                table: "EInvoice",
                column: "FkeyEInvoice",
                unique: true,
                filter: "[FkeyEInvoice] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EInvoice_FkeyEInvoice",
                table: "EInvoice");

            migrationBuilder.DropColumn(
                name: "FkeyEInvoice",
                table: "EInvoice");
        }
    }
}
