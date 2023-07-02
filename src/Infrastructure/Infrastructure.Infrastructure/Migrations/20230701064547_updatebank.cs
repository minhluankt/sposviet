using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatebank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_BankAccount_IdBankAccount",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_IdBankAccount",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_BankAccount_BankNumber",
                table: "BankAccount");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "BankAccount");

            migrationBuilder.RenameColumn(
                name: "IdPaymentMethod",
                table: "BankAccount",
                newName: "ComId");

            migrationBuilder.AlterColumn<string>(
                name: "BankNumber",
                table: "BankAccount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                table: "BankAccount",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BankAddress",
                table: "BankAccount",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "BankAccount",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_BankNumber_ComId",
                table: "BankAccount",
                columns: new[] { "BankNumber", "ComId" },
                unique: true,
                filter: "[BankNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankAccount_BankNumber_ComId",
                table: "BankAccount");

            migrationBuilder.RenameColumn(
                name: "ComId",
                table: "BankAccount",
                newName: "IdPaymentMethod");

            migrationBuilder.AlterColumn<string>(
                name: "BankNumber",
                table: "BankAccount",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                table: "BankAccount",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BankAddress",
                table: "BankAccount",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "BankAccount",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "BankAccount",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_IdBankAccount",
                table: "Order",
                column: "IdBankAccount");

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
        }
    }
}
