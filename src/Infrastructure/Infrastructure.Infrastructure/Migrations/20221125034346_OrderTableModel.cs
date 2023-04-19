using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class OrderTableModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "OrderTable");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "OrderTableItem",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "IdProduct",
                table: "OrderTableItem",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amonut",
                table: "OrderTable",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ManagerInvNo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdProduct",
                table: "InvoiceItem",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IdProduct",
                table: "OrderTableItem");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ManagerInvNo");

            migrationBuilder.DropColumn(
                name: "IdProduct",
                table: "InvoiceItem");

            migrationBuilder.AlterColumn<double>(
                name: "DiscountAmount",
                table: "OrderTableItem",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Amonut",
                table: "OrderTable",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "OrderTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "OrderTable",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
