using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class adddateinvoiceitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreateService",
                table: "InvoiceItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEndService",
                table: "InvoiceItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsServiceDate",
                table: "InvoiceItem",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreateService",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "DateEndService",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "IsServiceDate",
                table: "InvoiceItem");
        }
    }
}
