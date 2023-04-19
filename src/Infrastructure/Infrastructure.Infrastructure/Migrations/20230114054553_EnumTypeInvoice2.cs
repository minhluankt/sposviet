using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class EnumTypeInvoice2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_TypeInvoice",
                table: "OrderTable",
                column: "TypeInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_TypeInvoice",
                table: "Invoice",
                column: "TypeInvoice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderTable_TypeInvoice",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_TypeInvoice",
                table: "Invoice");
        }
    }
}
