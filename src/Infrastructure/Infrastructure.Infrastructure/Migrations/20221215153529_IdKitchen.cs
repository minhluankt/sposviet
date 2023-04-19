using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class IdKitchen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kitchen_Id",
                table: "Kitchen");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_Id_IdKitchen",
                table: "Kitchen",
                columns: new[] { "Id", "IdKitchen" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kitchen_Id_IdKitchen",
                table: "Kitchen");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_Id",
                table: "Kitchen",
                column: "Id",
                unique: true);
        }
    }
}
