using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updateAdJusPaymentSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "PurchaseOrder",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AdJusPaymentSupplier",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComId",
                table: "AdJusPaymentSupplier",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdSuppliers",
                table: "AdJusPaymentSupplier",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AdJusPaymentSupplier_Code_ComId",
                table: "AdJusPaymentSupplier",
                columns: new[] { "Code", "ComId" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AdJusPaymentSupplier_IdSuppliers",
                table: "AdJusPaymentSupplier",
                column: "IdSuppliers");

            migrationBuilder.AddForeignKey(
                name: "FK_AdJusPaymentSupplier_Suppliers_IdSuppliers",
                table: "AdJusPaymentSupplier",
                column: "IdSuppliers",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdJusPaymentSupplier_Suppliers_IdSuppliers",
                table: "AdJusPaymentSupplier");

            migrationBuilder.DropIndex(
                name: "IX_AdJusPaymentSupplier_Code_ComId",
                table: "AdJusPaymentSupplier");

            migrationBuilder.DropIndex(
                name: "IX_AdJusPaymentSupplier_IdSuppliers",
                table: "AdJusPaymentSupplier");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "AdJusPaymentSupplier");

            migrationBuilder.DropColumn(
                name: "ComId",
                table: "AdJusPaymentSupplier");

            migrationBuilder.DropColumn(
                name: "IdSuppliers",
                table: "AdJusPaymentSupplier");
        }
    }
}
