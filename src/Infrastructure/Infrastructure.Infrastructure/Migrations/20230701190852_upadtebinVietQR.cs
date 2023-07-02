using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class upadtebinVietQR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ACQId",
                table: "VietQR");

            migrationBuilder.AddColumn<int>(
                name: "BinVietQR",
                table: "BankAccount",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinVietQR",
                table: "BankAccount");

            migrationBuilder.AddColumn<int>(
                name: "ACQId",
                table: "VietQR",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
