using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updateslug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TableLink_slug_tableId",
                table: "TableLink");

            migrationBuilder.AddColumn<int>(
                name: "Comid",
                table: "TableLink",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TableLink_slug_tableId_Comid",
                table: "TableLink",
                columns: new[] { "slug", "tableId", "Comid" },
                unique: true,
                filter: "[slug] IS NOT NULL AND [tableId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TableLink_slug_tableId_Comid",
                table: "TableLink");

            migrationBuilder.DropColumn(
                name: "Comid",
                table: "TableLink");

            migrationBuilder.CreateIndex(
                name: "IX_TableLink_slug_tableId",
                table: "TableLink",
                columns: new[] { "slug", "tableId" },
                unique: true,
                filter: "[slug] IS NOT NULL AND [tableId] IS NOT NULL");
        }
    }
}
