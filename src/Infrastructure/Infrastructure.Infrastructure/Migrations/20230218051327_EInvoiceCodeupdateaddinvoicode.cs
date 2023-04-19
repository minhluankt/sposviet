using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class EInvoiceCodeupdateaddinvoicode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EInvoice_Fkey",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_InvoiceNo_IdInvoice_ComId",
                table: "EInvoice");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCode",
                table: "EInvoice",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_Fkey_InvoiceCode_MCQT",
                table: "EInvoice",
                columns: new[] { "Fkey", "InvoiceCode", "MCQT" },
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL AND [MCQT] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_IdInvoice_CusCode_ComId",
                table: "EInvoice",
                columns: new[] { "IdInvoice", "CusCode", "ComId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EInvoice_Fkey_InvoiceCode_MCQT",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_IdInvoice_CusCode_ComId",
                table: "EInvoice");

            migrationBuilder.DropColumn(
                name: "InvoiceCode",
                table: "EInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_Fkey",
                table: "EInvoice",
                column: "Fkey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_InvoiceNo_IdInvoice_ComId",
                table: "EInvoice",
                columns: new[] { "InvoiceNo", "IdInvoice", "ComId" });
        }
    }
}
