using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class BankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IdTransportFee",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodId",
                table: "Order",
                newName: "IdDeliveryCompany");

            migrationBuilder.RenameIndex(
                name: "IX_Order_PaymentMethodId",
                table: "Order",
                newName: "IX_Order_IdDeliveryCompany");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "PaymentMethod",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdPaymentMethod",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountContent",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdBankAccount",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodContent",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BankAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaymentMethod = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BankAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryCompany",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryCompany", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_IdBankAccount",
                table: "Order",
                column: "IdBankAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IdPaymentMethod",
                table: "Order",
                column: "IdPaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_BankNumber",
                table: "BankAccount",
                column: "BankNumber",
                unique: true,
                filter: "[BankNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_BankAccount_IdBankAccount",
                table: "Order",
                column: "IdBankAccount",
                principalTable: "BankAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryCompany_IdDeliveryCompany",
                table: "Order",
                column: "IdDeliveryCompany",
                principalTable: "DeliveryCompany",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentMethod_IdPaymentMethod",
                table: "Order",
                column: "IdPaymentMethod",
                principalTable: "PaymentMethod",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_BankAccount_IdBankAccount",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryCompany_IdDeliveryCompany",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentMethod_IdPaymentMethod",
                table: "Order");

            migrationBuilder.DropTable(
                name: "BankAccount");

            migrationBuilder.DropTable(
                name: "DeliveryCompany");

            migrationBuilder.DropIndex(
                name: "IX_Order_IdBankAccount",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_IdPaymentMethod",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "PaymentMethod");

            migrationBuilder.DropColumn(
                name: "BankAccountContent",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IdBankAccount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentMethodContent",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "IdDeliveryCompany",
                table: "Order",
                newName: "PaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_IdDeliveryCompany",
                table: "Order",
                newName: "IX_Order_PaymentMethodId");

            migrationBuilder.AlterColumn<int>(
                name: "IdPaymentMethod",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdTransportFee",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                table: "Order",
                column: "PaymentMethodId",
                principalTable: "PaymentMethod",
                principalColumn: "Id");
        }
    }
}
