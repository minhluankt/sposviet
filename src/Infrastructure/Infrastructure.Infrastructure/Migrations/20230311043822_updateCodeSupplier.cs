using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updateCodeSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodeSupplier",
                table: "Suppliers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_CodeSupplier_ComId",
                table: "Suppliers",
                columns: new[] { "CodeSupplier", "ComId" },
                unique: true,
                filter: "[CodeSupplier] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_CodeSupplier_ComId",
                table: "Suppliers");

            migrationBuilder.AlterColumn<string>(
                name: "CodeSupplier",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
