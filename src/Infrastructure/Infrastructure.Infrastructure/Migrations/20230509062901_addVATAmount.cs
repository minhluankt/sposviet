using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addVATAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "OrderTableItem",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsVAT",
                table: "OrderTableItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceNoVAT",
                table: "OrderTableItem",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATAmount",
                table: "OrderTableItem",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATRate",
                table: "OrderTableItem",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "OrderTableItem");

            migrationBuilder.DropColumn(
                name: "IsVAT",
                table: "OrderTableItem");

            migrationBuilder.DropColumn(
                name: "PriceNoVAT",
                table: "OrderTableItem");

            migrationBuilder.DropColumn(
                name: "VATAmount",
                table: "OrderTableItem");

            migrationBuilder.DropColumn(
                name: "VATRate",
                table: "OrderTableItem");
        }
    }
}
