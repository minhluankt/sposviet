using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class isPromotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Discount",
                table: "Product",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DiscountRun",
                table: "Product",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "IsOutstock",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "PriceDiscount",
                table: "Product",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceDiscountRun",
                table: "Product",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "isBestseller",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isHotNew",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isPromotion",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isRunPromotion",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DiscountRun",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsOutstock",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "PriceDiscount",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "PriceDiscountRun",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "isBestseller",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "isHotNew",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "isPromotion",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "isRunPromotion",
                table: "Product");
        }
    }
}
