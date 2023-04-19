using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatecity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ward_Code",
                table: "Ward");

            migrationBuilder.DropIndex(
                name: "IX_District_Code",
                table: "District");

            migrationBuilder.DropIndex(
                name: "IX_City_Code",
                table: "City");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Ward");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "District");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "City");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Ward",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "District",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "City",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ward_Code",
                table: "Ward",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_District_Code",
                table: "District",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_City_Code",
                table: "City",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ward_Code",
                table: "Ward");

            migrationBuilder.DropIndex(
                name: "IX_District_Code",
                table: "District");

            migrationBuilder.DropIndex(
                name: "IX_City_Code",
                table: "City");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Ward",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Ward",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "District",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "District",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "City",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "City",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Ward_Code",
                table: "Ward",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_District_Code",
                table: "District",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_City_Code",
                table: "City",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }
    }
}
