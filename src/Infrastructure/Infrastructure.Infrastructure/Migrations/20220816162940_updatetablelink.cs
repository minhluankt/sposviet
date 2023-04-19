using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatetablelink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TableLink_ID",
                table: "TableLink");

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "TableLink",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableLink_ID_slug",
                table: "TableLink",
                columns: new[] { "ID", "slug" },
                unique: true,
                filter: "[slug] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TableLink_ID_slug",
                table: "TableLink");

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "TableLink",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableLink_ID",
                table: "TableLink",
                column: "ID");
        }
    }
}
