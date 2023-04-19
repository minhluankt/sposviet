using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class OrderTableModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderTable_PaymentMethod_IdPaymentMethod",
                table: "OrderTable");

            migrationBuilder.AlterColumn<int>(
                name: "IdPaymentMethod",
                table: "OrderTable",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTable_PaymentMethod_IdPaymentMethod",
                table: "OrderTable",
                column: "IdPaymentMethod",
                principalTable: "PaymentMethod",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderTable_PaymentMethod_IdPaymentMethod",
                table: "OrderTable");

            migrationBuilder.AlterColumn<int>(
                name: "IdPaymentMethod",
                table: "OrderTable",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTable_PaymentMethod_IdPaymentMethod",
                table: "OrderTable",
                column: "IdPaymentMethod",
                principalTable: "PaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
