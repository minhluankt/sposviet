using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class ApplicaticonsolidatedChildrenonInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewTableName",
                table: "HistoryOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderCode",
                table: "HistoryOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "HistoryOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewTableName",
                table: "HistoryOrder");

            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "HistoryOrder");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "HistoryOrder");
        }
    }
}
