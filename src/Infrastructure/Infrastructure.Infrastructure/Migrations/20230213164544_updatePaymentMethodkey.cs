using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatePaymentMethodkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentMethod_Code",
                table: "PaymentMethod");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "PaymentMethod",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PaymentMethod",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vkey",
                table: "PaymentMethod",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_Vkey",
                table: "PaymentMethod",
                column: "Vkey",
                unique: true,
                filter: "[Vkey] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentMethod_Vkey",
                table: "PaymentMethod");

            migrationBuilder.DropColumn(
                name: "Vkey",
                table: "PaymentMethod");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "PaymentMethod",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PaymentMethod",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_Code",
                table: "PaymentMethod",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }
    }
}
