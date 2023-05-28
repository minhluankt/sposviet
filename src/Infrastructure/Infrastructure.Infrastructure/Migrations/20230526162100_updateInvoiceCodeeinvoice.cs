using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updateInvoiceCodeeinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EInvoice_InvoiceCode_ComId",
                table: "EInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_InvoiceCode_ComId_StatusEinvoice",
                table: "EInvoice",
                columns: new[] { "InvoiceCode", "ComId", "StatusEinvoice" },
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EInvoice_InvoiceCode_ComId_StatusEinvoice",
                table: "EInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_InvoiceCode_ComId",
                table: "EInvoice",
                columns: new[] { "InvoiceCode", "ComId" },
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL");
        }
    }
}
