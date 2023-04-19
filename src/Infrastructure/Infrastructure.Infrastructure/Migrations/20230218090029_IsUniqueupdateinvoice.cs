using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class IsUniqueupdateinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EInvoice_Fkey_InvoiceCode_MCQT",
                table: "EInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_Fkey",
                table: "EInvoice",
                column: "Fkey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_InvoiceCode",
                table: "EInvoice",
                column: "InvoiceCode",
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_MCQT",
                table: "EInvoice",
                column: "MCQT",
                unique: true,
                filter: "[MCQT] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EInvoice_Fkey",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_InvoiceCode",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_MCQT",
                table: "EInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_Fkey_InvoiceCode_MCQT",
                table: "EInvoice",
                columns: new[] { "Fkey", "InvoiceCode", "MCQT" },
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL AND [MCQT] IS NOT NULL");
        }
    }
}
