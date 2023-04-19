using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class VFkeyCusTaxCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyAdminInfo_CusTaxCode",
                table: "CompanyAdminInfo");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "CompanyAdminInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CusTaxCode",
                table: "CompanyAdminInfo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "CompanyAdminInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateExpiration",
                table: "CompanyAdminInfo",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCity",
                table: "CompanyAdminInfo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdDichVu",
                table: "CompanyAdminInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdDistrict",
                table: "CompanyAdminInfo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdWard",
                table: "CompanyAdminInfo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberDateExpiration",
                table: "CompanyAdminInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "CompanyAdminInfo",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VFkeyCusTaxCode",
                table: "CompanyAdminInfo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VFkeyPhone",
                table: "CompanyAdminInfo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdminInfo_VFkeyCusTaxCode_VFkeyPhone",
                table: "CompanyAdminInfo",
                columns: new[] { "VFkeyCusTaxCode", "VFkeyPhone" },
                unique: true,
                filter: "[VFkeyCusTaxCode] IS NOT NULL AND [VFkeyPhone] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyAdminInfo_VFkeyCusTaxCode_VFkeyPhone",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "DateExpiration",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "IdCity",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "IdDichVu",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "IdDistrict",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "IdWard",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "NumberDateExpiration",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "VFkeyCusTaxCode",
                table: "CompanyAdminInfo");

            migrationBuilder.DropColumn(
                name: "VFkeyPhone",
                table: "CompanyAdminInfo");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "CompanyAdminInfo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CusTaxCode",
                table: "CompanyAdminInfo",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdminInfo_CusTaxCode",
                table: "CompanyAdminInfo",
                column: "CusTaxCode",
                unique: true,
                filter: "[CusTaxCode] IS NOT NULL");
        }
    }
}
