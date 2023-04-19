using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatecity5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Ward_District_idDistrict",
                table: "Ward");

            migrationBuilder.AddColumn<int>(
                name: "idCity",
                table: "Ward",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ward_idCity",
                table: "Ward",
                column: "idCity");


            migrationBuilder.AddForeignKey(
                name: "FK_Ward_City_idCity",
                table: "Ward",
                column: "idCity",
                principalTable: "City",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ward_District_idDistrict",
                table: "Ward",
                column: "idDistrict",
                principalTable: "District",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_District_City_idCity",
                table: "District");

            migrationBuilder.DropForeignKey(
                name: "FK_Ward_City_idCity",
                table: "Ward");

            migrationBuilder.DropForeignKey(
                name: "FK_Ward_District_idDistrict",
                table: "Ward");

            migrationBuilder.DropIndex(
                name: "IX_Ward_idCity",
                table: "Ward");

            migrationBuilder.DropColumn(
                name: "idCity",
                table: "Ward");

            migrationBuilder.AddForeignKey(
                name: "FK_District_City_idCity",
                table: "District",
                column: "idCity",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ward_District_idDistrict",
                table: "Ward",
                column: "idDistrict",
                principalTable: "District",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
