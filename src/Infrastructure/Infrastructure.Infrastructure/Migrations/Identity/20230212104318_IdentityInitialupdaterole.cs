using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations.Identity
{
    public partial class IdentityInitialupdaterole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "Roles",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComId",
                schema: "dbo",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VKey",
                schema: "dbo",
                table: "Roles",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_VKey",
                schema: "dbo",
                table: "Roles",
                column: "VKey",
                unique: true,
                filter: "[VKey] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_VKey",
                schema: "dbo",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "dbo",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ComId",
                schema: "dbo",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "VKey",
                schema: "dbo",
                table: "Roles");
        }
    }
}
