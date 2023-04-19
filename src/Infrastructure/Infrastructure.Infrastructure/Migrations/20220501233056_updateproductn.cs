using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updateproductn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Specification",
                table: "Product",
                newName: "Packing");

            migrationBuilder.AddColumn<string>(
                name: "NoteProdName",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "OrderDetailts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRunPromotion",
                table: "OrderDetailts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "OrderDetailts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATAmount",
                table: "OrderDetailts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATRate",
                table: "OrderDetailts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "IdPharmaceutical",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteProdName",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Img",
                table: "OrderDetailts");

            migrationBuilder.DropColumn(
                name: "IsRunPromotion",
                table: "OrderDetailts");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "OrderDetailts");

            migrationBuilder.DropColumn(
                name: "VATAmount",
                table: "OrderDetailts");

            migrationBuilder.DropColumn(
                name: "VATRate",
                table: "OrderDetailts");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "Packing",
                table: "Product",
                newName: "Specification");

            migrationBuilder.AlterColumn<int>(
                name: "IdPharmaceutical",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
