using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class udpateinvoiceitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CusCode",
                table: "OrderTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdStaff",
                table: "OrderTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffName",
                table: "OrderTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "InvoiceItem",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CusCode",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdStaff",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffName",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CusCode",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "IdStaff",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "StaffName",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "CusCode",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "IdStaff",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "StaffName",
                table: "Invoice");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "InvoiceItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
