using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyAdminInfo_CusTaxCode",
                table: "CompanyAdminInfo");

            migrationBuilder.AlterColumn<string>(
                name: "CusTaxCode",
                table: "CompanyAdminInfo",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "CompanyAdminInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdminInfo_CusTaxCode",
                table: "CompanyAdminInfo",
                column: "CusTaxCode",
                unique: true,
                filter: "[CusTaxCode] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyAdminInfo_CusTaxCode",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "CompanyAdminInfo");

            migrationBuilder.AlterColumn<string>(
                name: "CusTaxCode",
                table: "CompanyAdminInfo",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdminInfo_CusTaxCode",
                table: "CompanyAdminInfo",
                column: "CusTaxCode",
                unique: true);
        }
    }
}
