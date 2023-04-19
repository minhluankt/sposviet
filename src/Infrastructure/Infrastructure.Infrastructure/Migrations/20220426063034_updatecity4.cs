using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatecity4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_District_IdDistrict",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_District_City_idCity",
                table: "District");

            migrationBuilder.DropForeignKey(
                name: "FK_Ward_District_idDistrict",
                table: "Ward");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_District_IdDistrict",
                table: "Customer",
                column: "IdDistrict",
                principalTable: "District",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);



            migrationBuilder.AddForeignKey(
                name: "FK_Ward_District_idDistrict",
                table: "Ward",
                column: "idDistrict",
                principalTable: "District",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_District_IdDistrict",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_District_City_idCity",
                table: "District");

            migrationBuilder.DropForeignKey(
                name: "FK_Ward_District_idDistrict",
                table: "Ward");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_District_IdDistrict",
                table: "Customer",
                column: "IdDistrict",
                principalTable: "District",
                principalColumn: "Id");



            migrationBuilder.AddForeignKey(
                name: "FK_Ward_District_idDistrict",
                table: "Ward",
                column: "idDistrict",
                principalTable: "District",
                principalColumn: "Id");
        }
    }
}
