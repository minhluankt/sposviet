using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatechungthu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerialCert",
                table: "SupplierEInvoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeSeri",
                table: "SupplierEInvoice",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialCert",
                table: "SupplierEInvoice");

            migrationBuilder.DropColumn(
                name: "TypeSeri",
                table: "SupplierEInvoice");
        }
    }
}
