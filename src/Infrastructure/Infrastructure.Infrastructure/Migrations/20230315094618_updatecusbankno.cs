using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatecusbankno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CusBankName",
                table: "EInvoice");

            migrationBuilder.DropColumn(
                name: "CusBankNo",
                table: "EInvoice");

            migrationBuilder.AddColumn<string>(
                name: "CusBankName",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CusBankNo",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_IdCustomer",
                table: "EInvoice",
                column: "IdCustomer");

            migrationBuilder.AddForeignKey(
                name: "FK_EInvoice_Customer_IdCustomer",
                table: "EInvoice",
                column: "IdCustomer",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EInvoice_Customer_IdCustomer",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_IdCustomer",
                table: "EInvoice");

            migrationBuilder.DropColumn(
                name: "CusBankName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CusBankNo",
                table: "Customer");

            migrationBuilder.AddColumn<string>(
                name: "CusBankName",
                table: "EInvoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CusBankNo",
                table: "EInvoice",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
