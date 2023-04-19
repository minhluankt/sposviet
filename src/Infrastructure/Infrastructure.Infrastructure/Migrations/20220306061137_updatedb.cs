using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_IdCategory",
                table: "Product");

            migrationBuilder.CreateIndex(
                name: "IX_Product_IdCategory_ViewNumber_isPromotion",
                table: "Product",
                columns: new[] { "IdCategory", "ViewNumber", "isPromotion" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_IdCategory_ViewNumber_isPromotion",
                table: "Product");

            migrationBuilder.CreateIndex(
                name: "IX_Product_IdCategory",
                table: "Product",
                column: "IdCategory");
        }
    }
}
