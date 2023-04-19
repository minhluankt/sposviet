using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updateListNoteOrderModelViewPos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityNotifyKitchen",
                table: "OrderTableItem",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "OrderTableItem",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "OrderTable",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Kitchen",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "HistoryOrder",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "DetailtKitchen",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "QuantityNotifyKitchen",
                table: "OrderTableItem",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "OrderTableItem",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "OrderTable",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "Kitchen",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "HistoryOrder",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "DetailtKitchen",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");
        }
    }
}
