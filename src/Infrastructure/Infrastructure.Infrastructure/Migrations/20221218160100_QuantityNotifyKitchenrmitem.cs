using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class QuantityNotifyKitchenrmitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotifyKitchen",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "QuantityNotifyKitchen",
                table: "OrderTable");

            migrationBuilder.AddColumn<int>(
                name: "QuantityNotifyKitchen",
                table: "OrderTableItem",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityNotifyKitchen",
                table: "OrderTableItem");

            migrationBuilder.AddColumn<bool>(
                name: "IsNotifyKitchen",
                table: "OrderTable",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "QuantityNotifyKitchen",
                table: "OrderTable",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
