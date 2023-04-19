using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class EnumTypeInvoice3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderTable_TypeInvoice",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_TypeInvoice",
                table: "Invoice");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_TypeInvoice_TypeProduct_ComId",
                table: "OrderTable",
                columns: new[] { "TypeInvoice", "TypeProduct", "ComId" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_TypeInvoice_TypeProduct_ComId",
                table: "Invoice",
                columns: new[] { "TypeInvoice", "TypeProduct", "ComId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderTable_TypeInvoice_TypeProduct_ComId",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_TypeInvoice_TypeProduct_ComId",
                table: "Invoice");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_TypeInvoice",
                table: "OrderTable",
                column: "TypeInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_TypeInvoice",
                table: "Invoice",
                column: "TypeInvoice");
        }
    }
}
