using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addIsMergeinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_OrderTable_IdOrderTable",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_IdOrderTable",
                table: "Invoice");

            migrationBuilder.AlterColumn<int>(
                name: "IdOrderTable",
                table: "Invoice",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCodePatern",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMerge",
                table: "Invoice",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_IdOrderTable",
                table: "Invoice",
                column: "IdOrderTable",
                unique: true,
                filter: "[IdOrderTable] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_OrderTable_IdOrderTable",
                table: "Invoice",
                column: "IdOrderTable",
                principalTable: "OrderTable",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_OrderTable_IdOrderTable",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_IdOrderTable",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "InvoiceCodePatern",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "IsMerge",
                table: "Invoice");

            migrationBuilder.AlterColumn<int>(
                name: "IdOrderTable",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_IdOrderTable",
                table: "Invoice",
                column: "IdOrderTable",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_OrderTable_IdOrderTable",
                table: "Invoice",
                column: "IdOrderTable",
                principalTable: "OrderTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
