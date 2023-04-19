using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addDateCreateService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreate",
                table: "Product");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreateService",
                table: "OrderTableItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsServiceDate",
                table: "OrderTableItem",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreateService",
                table: "OrderTableItem");

            migrationBuilder.DropColumn(
                name: "IsServiceDate",
                table: "OrderTableItem");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreate",
                table: "Product",
                type: "datetime2",
                nullable: true);
        }
    }
}
